using System;
using System.IO;
using System.Xml;
using System.Data;
using System.Text;
using System.Collections;
using System.Configuration;

using Newtera.Data;
using Newtera.Common.Core;
using Newtera.Common.MetaData;
using Newtera.Common.MetaData.Schema;
using Newtera.Common.MetaData.Principal;
using Newtera.Common.MetaData.DataView;

namespace IndexRunner
{
	/// <summary>
	/// A utility class that combines instance property values into a string, save it
	/// to the Content attribute of Product class, and generate full-text search index on
	/// the Content attribute.
	/// </summary>
	public class FullTextIndexRunner
	{
		private const string PRODUCT_CLASS = "Product";
		private const string CONTENT_ATTRIBUTE_NAME = "Content";
		private const string DEFAULT_CONNECTION_STRING = "ConnectionString";
		
		private static int _instanceCount = 0;

		/// <summary>
		/// The main entry point for the application.
		/// </summary>
		[STAThread]
		static void Main(string[] args)
		{
			// Update the value of Content attribute of Product class
			System.Console.WriteLine("Start updating full-text search contents");

			string defaultConnectionString = ConfigurationSettings.AppSettings[DEFAULT_CONNECTION_STRING];
			
			if (defaultConnectionString == null)
			{
				throw new Exception("Unable to find " + DEFAULT_CONNECTION_STRING + " in AppSettings of the Config file");
			}

			// run this program as the super user
			// Attach a CustomPrincipal to the current thread
			CustomPrincipal.Attach(new ServerSideUserManager(), "admin");

			UpdateContentValues(defaultConnectionString);

			System.Console.WriteLine("End updating full-text search contents");
		}

		/// <summary>
		/// Go through every instance in Product class and check if value of the Content
		/// attribute needs to be updated or not.
		/// </summary>
		private static void UpdateContentValues(string defaultConnectionString)
		{
			using (CMConnection connection = new CMConnection(defaultConnectionString))
			{
				connection.Open();

				ClassElement theParent = connection.MetaDataModel.SchemaModel.FindClass(PRODUCT_CLASS);

				// walk through all leaf classes of PRODUCT_CLASS class and update content
				// attribute of each instance
				IEnumerator enumerator = connection.MetaDataModel.SchemaModel.GetClassEnumerator();
				
				enumerator.Reset();
				while (enumerator.MoveNext())
				{
					ClassElement classElement = (ClassElement) enumerator.Current;

					if (IsLeafOf(classElement, theParent))
					{
						DataViewModel dataView = connection.MetaDataModel.GetCompleteDataView(classElement.Name);

						dataView.PageSize = 500;

						int pageIndex = 0;

						while (true)
						{
							// set the query start index
							dataView.PageIndex = pageIndex++;

							// execute the query to get the xml result
							string query = dataView.SearchQuery;

							CMCommand cmd = connection.CreateCommand();
							cmd.CommandText = query;
								
							XmlReader reader = cmd.ExecuteXMLReader();
							DataSet ds = new DataSet();
							ds.ReadXml(reader);

							if (!DataSetHelper.IsEmptyDataSet(ds, classElement.Name))
							{
								// Create an instance view
								InstanceView instanceView = new InstanceView(dataView, ds);

								// iterate through each instance
								int rows = ds.Tables[classElement.Name].Rows.Count;
								for (int rowIndex = 0; rowIndex < rows; rowIndex++)
								{
									instanceView.SelectedIndex = rowIndex;

									UpdateInstanceContent(connection, instanceView);
								}
							}
							else
							{
								break;
							}
						}
					}
				}
			}			
		}

		/// <summary>
		/// Update value of the Content attribute of the specified instance
		/// </summary>
		/// <param name="connection">The CMConnection</param>
		/// <param name="instanceView">A instance view</param>
		private static void UpdateInstanceContent(CMConnection connection, InstanceView instanceView)
		{
			string query;
			CMCommand cmd;

			_instanceCount++;

			// concate values of selected instance propertties together
			StringBuilder builder = new StringBuilder(2000);
			foreach (InstanceAttributePropertyDescriptor property in instanceView.GetProperties(null))
			{
				if (property.IsBrowsable &&
					property.IsGoodForFullTextSearch &&
					property.GetValue() != null)
				{
					builder.Append(property.GetValue().ToString()).Append(" ");
				}
			}

			// set the new content
			InstanceAttributePropertyDescriptor contentProperty = (InstanceAttributePropertyDescriptor) instanceView.GetProperties(null)[CONTENT_ATTRIBUTE_NAME];
			if (contentProperty != null)
			{
				// TODO, the type of Content property can hold only 2000 unicode chars
				// truncate the string if it is too big
				string contentVal = builder.ToString();
				if (contentVal.Length > 2000)
				{
					contentVal = contentVal.Substring(0, 2000);
				}
				contentProperty.SetValue(null, contentVal);

				// do not update if the content does not change
				if (instanceView.IsDataChanged)
				{
					query = instanceView.DataView.UpdateQuery;

					cmd = connection.CreateCommand();
					cmd.CommandText = query;
						
					try
					{
						XmlDocument doc = cmd.ExecuteXMLDoc();

						System.Console.WriteLine("Updated instance " + _instanceCount + ".");
					}
					catch (Exception ex)
					{
						// do not stop if there is error, print the error on the
						// console
						System.Console.WriteLine("Updating error : " + ex.Message);
					}				
				}
			}
			else
			{
				throw new Exception("Missing " + CONTENT_ATTRIBUTE_NAME + " in class " + PRODUCT_CLASS);
			}
		}

		/// <summary>
		/// Gets the information indicating whether a child is a leaf class of a given
		/// parent class.
		/// </summary>
		/// <param name="child">The child class</param>
		/// <param name="parent">The parent class</param>
		/// <returns>true if it is a leaf class of the parent class, false otherwise.</returns>
		private static bool IsLeafOf(ClassElement child, ClassElement parent)
		{
			bool status = false;
			ClassElement temp = child;
			if (temp.IsLeaf)
			{
				while (temp != null)
				{
					if (temp == parent)
					{
						status = true;
						break;
					}
					
					temp = temp.ParentClass;
				}
			}

			return status;
		}
	}
}
