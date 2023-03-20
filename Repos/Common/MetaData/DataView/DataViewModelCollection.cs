/*
* @(#)DataViewModelCollection.cs
*
* Copyright (c) 2003 Newtera, Inc. All rights reserved.
*
*/
namespace Newtera.Common.MetaData.DataView
{
	using System;
	using System.Xml;
	using System.IO;
	using System.Text;
	using System.Collections;

	using Newtera.Common.Core;
	using Newtera.Common.MetaData;
	using Newtera.Common.MetaData.Schema;
	using Newtera.Common.MetaData.XaclModel;

	/// <summary>
	/// Represents a collection of DataViewModel objects.
	/// </summary>
	/// <version>1.0.1 29 Oct 2003</version>
	/// <author>Yong Zhang</author>
	public class DataViewModelCollection : DataViewElementCollection
	{
		private bool _isAltered;
		private SchemaInfo _schemaInfo;
		private SchemaModel _schemaModel;
		private MetaDataModel _metaData; // run-time only

		/// <summary>
		/// Initiating an instance of DataViewModelCollection class
		/// </summary>
		public DataViewModelCollection() : base()
		{
			_isAltered = false;
			_schemaInfo = null;
			_schemaModel = null;
			_xpath = null;
			_metaData = null;

            if (GlobalSettings.Instance.IsWindowClient)
            {
                // listen to the value changed event from the data views
                this.ValueChanged += new EventHandler(this.ValueChangedHandler);
            }
		}
		
		/// <summary>
		/// Gets or sets the schema info of the data view.
		/// </summary>
		/// <value>The SchemaInfo instance</value>
		public SchemaInfo SchemaInfo
		{
			get
			{
				return _schemaInfo;
			}
			set
			{
				_schemaInfo = value;
			}
		}

		/// <summary>
		/// Gets or sets the meta data that owns the Schema
		/// </summary>
		/// <returns> A MetaDataModel object</returns>
		public MetaDataModel MetaData
		{
			get
			{
				return _metaData;
			}
			set
			{
				_metaData = value;
			}
		}

		/// <summary>
		/// Gets or sets the schema model of the data view.
		/// </summary>
		/// <value>The SchemaModel instance</value>
		public SchemaModel SchemaModel
		{
			get
			{
				return _schemaModel;
			}
			set
			{
				_schemaModel = value;
			}
		}

		/// <summary>
		/// Gets or sets the information indicating whether the data views has been altered
		/// </summary>
		/// <value>true if it is altered, false otherwise.</value>
		public bool IsAltered
		{
			get
			{
				return _isAltered;
			}
			set
			{
				_isAltered = value;
			}
		}

		/// <summary>
		/// Gets the type of element
		/// </summary>
		/// <value>One of ElementType values</value>
		public override ElementType ElementType 
		{
			get
			{
				return ElementType.DataViews;
			}
		}

        /// <summary>
		/// Adds a DataViewModel to the DataViewModelCollection.
		/// </summary>
		/// <param name="value">the DataViewModel to be added</param>
		/// <returns>The position into which the new element was added</returns>
        public override int Add(IDataViewElement value)
        {
            int pos = 0;
            if (value is DataViewModel)
            {
                pos = base.Add(value);
                ((DataViewModel)value).Container = this;
            }
            else
            {
                throw new ArgumentException("value must be of type DataViewModel.", "value");
            }

            return pos;
        }

        /// <summary>
		/// inserts a DataViewModel to the collection at the specified position
		/// </summary>
		/// <param name="index">The zero-based index at which value should be inserted</param>
		/// <param name="value">The DataViewModel to insert into collection</param>
        public override void Insert(int index, IDataViewElement value)
        {
            if (value is DataViewModel)
            {
                base.Insert(index, value);
                ((DataViewModel)value).Container = this;
            }
            else
            {
                throw new ArgumentException("value must be of type DataViewModel.", "value");
            }
        }

        /// <summary>
        /// Find a DataViewModel object of the given xpath.
        /// </summary>
        /// <param name="xpath">The xpath</param>
        /// <returns>The found DataViewModel object, null if not found</returns>
        public IMetaDataElement FindDataViewByXPath(string xpath)
        {
            FindDataViewElementVisitor visitor = new FindDataViewElementVisitor(xpath);

            Accept(visitor); // start finding

            return visitor.DataViewElement;
        }

		/// <summary>
		/// Accept a visitor of IDataViewElementVisitor type to traverse its
		/// elements.
		/// </summary>
		/// <param name="visitor">A visitor</param>
		public override void Accept(IDataViewElementVisitor visitor)
		{
			foreach (IDataViewElement element in List)
			{
				element.Accept(visitor);
			}
		}

		/// <summary>
		/// Get data views for the specified base class
		/// </summary>
		/// <param name="className">The class name</param>
		/// <returns>A DataViewModelCollection</returns>
		public DataViewModelCollection GetDataViewsForClass(string className)
		{
			DataViewModelCollection dataViews = new DataViewModelCollection();

			foreach (DataViewModel dataView in this.List)
			{
				if (dataView.BaseClass.ClassName == className)
				{
					dataViews.Add(dataView);
				}
			}

			return dataViews;
		}

		/// <summary>
		/// Get information indicating whether a class is referenced by any of
		/// the DataViews in the collection
		/// </summary>
		/// <param name="className">The class name</param>
        /// <param name="dataViewCaption">The caption of the referencing data view first found.</param>
		/// <returns>true if the class is referenced, false otherwise.</returns>
		public bool IsClassReferenced(string className, out string dataViewCaption)
		{
			bool status = false;
            dataViewCaption = null;

			foreach (DataViewModel dataView in this.List)
			{
				if (dataView.IsClassReferenced(className))
				{
					status = true;
                    dataViewCaption = dataView.Caption;
					break;
				}
			}

			return status;
		}

        /// <summary>
        /// Get information indicating whether an attribute is referenced by any of
        /// the DataViews in the collection
        /// </summary>
        /// <param name="className">The class name</param>
        /// <param name="attributeName">The attribute name</param>
        /// <param name="dataViewCaption">The caption of the referencing data view first found.</param>
        /// <returns>true if the attribute is referenced, false otherwise.</returns>
        public bool IsAttributeReferenced(string className, string attributeName, out string dataViewCaption)
        {
            bool status = false;
            dataViewCaption = null;

            foreach (DataViewModel dataView in this.List)
            {
                if (dataView.IsAttributeReferenced(className, attributeName))
                {
                    status = true;
                    dataViewCaption = dataView.Caption;
                    break;
                }
            }

            return status;
        }

		/// <summary>
		/// Constrauct a data view model collection from an XML file.
		/// </summary>
		/// <param name="fileName">the name of the XML file</param>
		/// <exception cref="DataViewException">DataViewException is thrown when it fails to
		/// read the XML file
		/// </exception>
		public void Read(string fileName)
		{
			try
			{
				if (File.Exists(fileName))
				{
					//Open the stream and read XSD from it.
					using (FileStream fs = File.OpenRead(fileName)) 
					{
						Read(fs);					
					}
				}
			}
			catch (Exception ex)
			{
				throw new DataViewException("Failed to read the file :" + fileName + " with reason " + ex.Message, ex);
			}
		}
		
		/// <summary>
		/// Constrauct a data view model from an stream.
		/// </summary>
		/// <param name="stream">the stream</param>
		/// <exception cref="DataViewException">DataViewException is thrown when it fails to
		/// read the stream
		/// </exception>
		public void Read(Stream stream)
		{
			if (stream != null)
			{
				try
				{
					XmlDocument doc = new XmlDocument();

					doc.Load(stream);
				
					// Initializing the objects from the xml document
					Unmarshal(doc.DocumentElement);
				}
				catch (Exception e)
				{
					throw new DataViewException(e.Message, e);
				}
			}
		}

		/// <summary>
		/// Constrauct a data view model from a text reader.
		/// </summary>
		/// <param name="reader">the text reader</param>
		/// <exception cref="DataViewException">DataViewException is thrown when it fails to
		/// read the text reader
		/// </exception>
		public void Read(TextReader reader)
		{
			if (reader != null)
			{
				try
				{
					XmlDocument doc = new XmlDocument();

					doc.Load(reader);
				
					// Initializing the objects from the xml document
					Unmarshal(doc.DocumentElement);
				}
				catch (Exception e)
				{
					throw new DataViewException(e.Message, e);
				}
			}
		}

		/// <summary>
		/// Write the data view model to an XML file.
		/// </summary>
		/// <param name="fileName">The output file name.</param>
		/// <exception cref="DataViewException">DataViewException is thrown when it fails to
		/// write to the file.
		/// </exception> 
		public void Write(string fileName)
		{
			try
			{
				//Open the stream and read XSD from it.
				using (FileStream fs = File.Open(fileName, FileMode.Create)) 
				{
					Write(fs);
					fs.Flush();
				}
			}
			catch (System.IO.IOException ex)
			{
				throw new DataViewException("Failed to write to file :" + fileName, ex);
			}
		}
		
		/// <summary>
		/// Write the data view as a XML data to a Stream.
		/// </summary>
		/// <param name="stream">the stream object to which to write a XML data</param>
		/// <exception cref="DataViewException">DataViewException is thrown when it fails to
		/// write to the stream.
		/// </exception>
		public void Write(Stream stream)
		{
			try
			{
				XmlDocument doc = GetXmlDocument();

				doc.Save(stream);
			}
			catch (System.IO.IOException ex)
			{
				throw new DataViewException("Failed to write the SchemaModel object", ex);
			}
		}

		/// <summary>
		/// Write the data view as a XML data to a TextWriter.
		/// </summary>
		/// <param name="writer">the TextWriter instance to which to write a XML schema
		/// </param>
		/// <exception cref="DataViewException">DataViewException is thrown when it fails to
		/// write to the stream.
		/// </exception>
		public void Write(TextWriter writer)
		{
			try
			{
				XmlDocument doc = GetXmlDocument();

				doc.Save(writer);
			}
			catch (System.IO.IOException ex)
			{
				throw new DataViewException("Failed to write the SchemaModel object", ex);
			}
		}

		/// <summary>
		/// Unmarshal an element representing data view collection
		/// </summary>
		/// <param name="parent">An xml element</param>
		public override void Unmarshal(XmlElement parent)
		{
			this.List.Clear();

			foreach (XmlElement xmlElement in parent.ChildNodes)
			{
				DataViewModel dataView = (DataViewModel) ElementFactory.Instance.Create(xmlElement);

				dataView.SchemaInfo = _schemaInfo;
				dataView.SchemaModel = _schemaModel;

				this.Add(dataView);
			}
		}

		/// <summary>
		/// Gets the xml document that represents the data view
		/// </summary>
		/// <returns>A XmlDocument instance</returns>
		private XmlDocument GetXmlDocument()
		{
			// Marshal the objects to xml document
			XmlDocument doc = new XmlDocument();

			XmlElement element = doc.CreateElement("DataViews");

			doc.AppendChild(element);

			Marshal(element);

			return doc;
		}

		/// <summary>
		/// A handler to call when a value of the data views changed
		/// </summary>
		/// <param name="sender">the IDataViewElement that cause the event</param>
		/// <param name="e">the arguments</param>
		protected override void ValueChangedHandler(object sender, EventArgs e)
		{
			IsAltered = true;
		}

		#region IXaclObject Members

		/// <summary>
		/// Return a xpath representation of the Taxonomy node
		/// </summary>
		/// <returns>a xapth representation</returns>
		public override string ToXPath()
		{
            if (_xpath == null && this.Parent != null)
			{
				_xpath = this.Parent.ToXPath() + "/dataview";
			}

			return _xpath;
		}

		/// <summary>
		/// Return a  parent of the Taxonomy node
		/// </summary>
		/// <returns>The parent of the Taxonomy node</returns>
        public override IXaclObject Parent
		{
			get
			{
				return _metaData;
			}
		}

		/// <summary>
		/// Return a  of children of the Taxonomy node
		/// </summary>
		/// <returns>The collection of IXaclObject nodes</returns>
        public override IEnumerator GetChildren()
		{
			return this.GetEnumerator();
		}

		#endregion
	}
}