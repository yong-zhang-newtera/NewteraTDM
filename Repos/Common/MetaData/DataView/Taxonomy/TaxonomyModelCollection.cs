/*
* @(#)TaxonomyModelCollection.cs
*
* Copyright (c) 2003 Newtera, Inc. All rights reserved.
*
*/
namespace Newtera.Common.MetaData.DataView.Taxonomy
{
	using System;
	using System.Xml;
	using System.IO;
	using System.Text;
	using System.Collections;
	using System.ComponentModel;
	using System.Drawing.Design;

	using Newtera.Common.Core;
	using Newtera.Common.MetaData;
	using Newtera.Common.MetaData.DataView;
	using Newtera.Common.MetaData.XaclModel;

	/// <summary>
	/// Represents a collection of TaxonomyModel objects.
	/// </summary>
	/// <version>1.0.1 12 Feb 2004</version>
	/// <author>Yong Zhang</author>
	public class TaxonomyModelCollection : DataViewElementCollection, ITaxonomy
	{
		private bool _isAltered;
		private MetaDataModel _metaData;
		private string _xpath = null; // run-time data

		/// <summary>
		/// Initiating an instance of TaxonomyModelCollection class
		/// </summary>
		public TaxonomyModelCollection(MetaDataModel metaData) : base()
		{
			_isAltered = false;
			_metaData = metaData;

            if (GlobalSettings.Instance.IsWindowClient)
            {
                // listen to the value changed event from the data views
                this.ValueChanged += new EventHandler(this.ValueChangedHandler);
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
				return ElementType.Taxonomies;
			}
		}

        /// <summary>
        /// Find a node of the given xpath from taxonomies
        /// </summary>
        /// <param name="xpath">The xpath</param>
        /// <returns>The found node, null if not found</returns>
        public IMetaDataElement FindNodeByXPath(string xpath)
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
		/// Constrauct a TaxonomyModel collection from an XML file.
		/// </summary>
		/// <param name="fileName">the name of the XML file</param>
		/// <exception cref="TaxonomyException">TaxonomyException is thrown when it fails to
		/// read the XML file
		/// </exception>
		public void Read(string fileName)
		{
			try
			{
				//Open the stream and read XSD from it.
				using (FileStream fs = File.OpenRead(fileName)) 
				{
					Read(fs);					
				}
			}
			catch (Exception ex)
			{
                throw new TaxonomyException("Failed to read the file :" + fileName + " with reason " + ex.Message, ex);
			}
		}
		
		/// <summary>
		/// Constrauct a TaxonomyModel from an stream.
		/// </summary>
		/// <param name="stream">the stream</param>
		/// <exception cref="TaxonomyException">TaxonomyException is thrown when it fails to
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
					throw new TaxonomyException(e.Message, e);
				}
			}
		}

		/// <summary>
		/// Constrauct a TaxonomyModel from a text reader.
		/// </summary>
		/// <param name="reader">the text reader</param>
		/// <exception cref="TaxonomyException">TaxonomyException is thrown when it fails to
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
					throw new TaxonomyException(e.Message, e);
				}
			}
		}

		/// <summary>
		/// Write the TaxonomyModel to an XML file.
		/// </summary>
		/// <param name="fileName">The output file name.</param>
		/// <exception cref="TaxonomyException">TaxonomyException is thrown when it fails to
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
				throw new TaxonomyException("Failed to write to file :" + fileName, ex);
			}
		}
		
		/// <summary>
		/// Write the data view as a XML data to a Stream.
		/// </summary>
		/// <param name="stream">the stream object to which to write a XML data</param>
		/// <exception cref="TaxonomyException">TaxonomyException is thrown when it fails to
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
				throw new TaxonomyException("Failed to write the SchemaModel object", ex);
			}
		}

		/// <summary>
		/// Write the data view as a XML data to a TextWriter.
		/// </summary>
		/// <param name="writer">the TextWriter instance to which to write a XML schema
		/// </param>
		/// <exception cref="TaxonomyException">TaxonomyException is thrown when it fails to
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
				throw new TaxonomyException("Failed to write the SchemaModel object", ex);
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
				TaxonomyModel taxonomyModel = (TaxonomyModel) ElementFactory.Instance.Create(xmlElement);

				this.Add(taxonomyModel);

				taxonomyModel.ParentNode = this;
			}
		}

		/// <summary>
		/// Add a TaxonomyModel to the collection
		/// </summary>
		/// <param name="value">An TaxonomyModel</param>
		/// <returns></returns>
		public override int Add(IDataViewElement value)
		{
			if (value is TaxonomyModel)
			{
				((TaxonomyModel) value).MetaData = this._metaData;
				return base.Add (value);
			}
			else
			{
				return 0;
			}
		}


		/// <summary>
		/// Get information indicating whether a class is referenced by any of
		/// the taxonomies in the collection
		/// </summary>
		/// <param name="className">The class name</param>
        /// <param name="taxonomyCaption">The caption of the referencing taxonomy first found.</param>
		/// <returns>true if the class is referenced, false otherwise.</returns>
		public bool IsClassReferenced(string className, out string taxonomyCaption)
		{
			bool status = false;
            taxonomyCaption = null;

			foreach (TaxonomyModel taxonomy in this.List)
			{
				if (taxonomy.IsClassReferenced(className))
				{
					status = true;
                    taxonomyCaption = taxonomy.Caption;
					break;
				}
			}

			return status;
		}

		/// <summary>
		/// Get information indicating whether a data view is referenced by any of
		/// the taxonomies in the collection
		/// </summary>
		/// <param name="dataViewName">The data view name</param>
        /// <param name="taxonomyCaption">The caption of the referencing taxonomy first found</param>
		/// <returns>true if the data view is referenced, false otherwise.</returns>
		public bool IsDataViewReferenced(string dataViewName, out string taxonomyCaption)
		{
			bool status = false;
            taxonomyCaption = null;

			foreach (TaxonomyModel taxonomy in this.List)
			{
				if (taxonomy.IsDataViewReferenced(dataViewName))
				{
                    taxonomyCaption = taxonomy.Caption;
					status = true;
					break;
				}
			}

			return status;
		}

        /// <summary>
        /// Get information indicating whether an attribute is referenced by expressions in any of
        /// the taxonomies in the collection
        /// </summary>
        /// <param name="className">The name of attribute owner class</param>
        /// <param name="attributeName">The name of the attribute to be found</param>
        /// <returns>true if the attribute is referenced, false otherwise.</returns>
        public bool IsAttributeReferenced(string className, string attributeName, out string taxonomyCaption)
        {
            bool status = false;
            taxonomyCaption = null;

            foreach (TaxonomyModel taxonomy in this.List)
            {
                if (taxonomy.IsAttributeReferenced(className, attributeName))
                {
                    taxonomyCaption = taxonomy.Caption;
                    status = true;
                    break;
                }
            }

            return status;
        }

		/// <summary>
		/// Gets the xml document that represents the data view
		/// </summary>
		/// <returns>A XmlDocument instance</returns>
		private XmlDocument GetXmlDocument()
		{
			// Marshal the objects to xml document
			XmlDocument doc = new XmlDocument();

			XmlElement element = doc.CreateElement("Taxonomies");

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

		#region ITaxonomy

		/// <summary>
		/// Gets the meta data model that owns the ITaxonomy object
		/// </summary>
		/// <value>A MetaDataModel</value>
		[BrowsableAttribute(false)]		
		public MetaDataModel MetaDataModel 
		{
			get
			{
				return _metaData;
			}
		}

		/// <summary>
		/// Gets the base class name for the taxonomy.
		/// </summary>
		[BrowsableAttribute(false)]		
		public string ClassName
		{
			get
			{
				return null;
			}
			set
			{
			}
		}

		/// <summary>
		/// Gets or sets the data view name.
		/// </summary>
		/// <value>The data view name, can be null.</value>		
		[BrowsableAttribute(false)]				
		public string DataViewName
		{
			get
			{
				return null;
			}
			set
			{
			}
		}

		/// <summary>
		/// Gets or sets the parent node of this node
		/// </summary>
		/// <value>A IDataViewElement object.</value>
		[BrowsableAttribute(false)]		
		public ITaxonomy ParentNode
		{
			get
			{
				return null;
			}
			set
			{
			}
		}

		/// <summary>
		/// Gets the first level of children Taxon nodes.
		/// </summary>
		/// <value>A TaxonNodeCollection</value>
		[BrowsableAttribute(false)]		
		public TaxonNodeCollection ChildrenNodes
		{
			get
			{
				return null;
			}
		}

        /// <summary>
        /// Gets or sets the definition for auto-generated hierarchy.
        /// </summary>
        /// <value>A AutoClassifyDef object</value>
        [BrowsableAttribute(false)]
        public AutoClassifyDef AutoClassifyDef
        {
            get
            {
                return null;
            }
            set
            {
            }
        }

		/// <summary>
		/// Return a xpath representation of the Taxonomy node
		/// </summary>
		/// <returns>a xapth representation</returns>
		public string ToXPath()
		{
			if (_xpath == null)
			{
				_xpath = this.Parent.ToXPath() + "/taxonomy";
			}

			return _xpath;
		}

		/// <summary>
		/// Return a  parent of the Taxonomy node
		/// </summary>
		/// <returns>The parent of the Taxonomy node</returns>
		[BrowsableAttribute(false)]		
		public IXaclObject Parent
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
		public IEnumerator GetChildren()
		{
			return this.GetEnumerator();
		}

		/// <summary>
		/// Return null value
		/// </summary>
		public DataViewModel GetDataView(string sectionString)
		{
			return null;
		}

		#endregion
	}
}