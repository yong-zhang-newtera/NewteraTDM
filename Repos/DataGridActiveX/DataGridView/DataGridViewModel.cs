/*
* @(#)DataGridViewModel.cs
*
* Copyright (c) 2006 Newtera, Inc. All rights reserved.
*
*/
namespace Newtera.DataGridActiveX.DataGridView
{
	using System;
	using System.IO;
	using System.Collections;
	using System.Collections.Specialized;
	using System.Xml;
	using System.Data;
	using System.ComponentModel;
	using System.Drawing.Design;

	/// <summary>
	/// A DataGridViewModel represents a view on the data of a certain class
	/// </summary>
	/// <version>1.0.1 28 May 2006</version>
	public class DataGridViewModel : DataGridViewElementBase
	{
		private string _viewType;
		private string _taxonomy;
		private string _taxon;
		private ViewClass _baseClass;
		private ViewFilter _filter;
		private ViewAttributeCollection _resultAttributes;
		private ViewSortBy _sortBy;
        private ViewLocaleInfo _localeInfo;

		/// <summary>
		/// Initiating an instance of DataGridViewModel class
		/// </summary>
		public DataGridViewModel() : this("")
		{
		}

		/// <summary>
		/// Initiating an instance of DataGridViewModel class
		/// </summary>
		/// <param name="name">Name of the class view</param>
		public DataGridViewModel(string name) : base(name)
		{
			_viewType = null;
			_taxonomy = null;
			_taxon = null;
			_baseClass = null;
			_resultAttributes = new ViewAttributeCollection();
			_resultAttributes.DataGridView = this;
			_filter = new ViewFilter();
			_sortBy = new ViewSortBy();
            _localeInfo = new ViewLocaleInfo();
		}

		/// <summary>
		/// Initiating an instance of DataGridViewModel class
		/// </summary>
		/// <param name="xmlElement">The xml element conatins data of the instance</param>
		internal DataGridViewModel(XmlElement xmlElement) : base()
		{
			Unmarshal(xmlElement);

			// register the value change handlers
			_baseClass.DataGridView = this;
			_resultAttributes.DataGridView = this;
			_filter.DataGridView = this;
			_sortBy.DataGridView = this;
		}

		/// <summary>
		/// Gets or sets the type of view to be associated with the DataGrid control
		/// the values are Class, Taxon, or Array
		/// </summary>
		public string ViewType
		{
			get
			{
				return _viewType;
			}
			set
			{
				_viewType = value;
			}
		}

		/// <summary>
		/// Gets or sets the name of the taxonomy tree. it could be null
		/// </summary>
		public string TaxonomyName
		{
			get
			{
				return _taxonomy;
			}
			set
			{
				_taxonomy = value;
			}
		}

		/// <summary>
		/// Gets or sets the name of the taxon node. it could be null
		/// </summary>
		public string TaxonName
		{
			get
			{
				return _taxon;
			}
			set
			{
				_taxon = value;
			}
		}

		/// <summary>
		/// Gets the base class of the class view.
		/// </summary>
		/// <value>The base class</value>
		public ViewClass BaseClass
		{
			get
			{
				return _baseClass;
			}
			set
			{
				_baseClass = value;
				value.DataGridView = this;
			}
		}

		/// <summary>
		/// Gets the referenced classes
		/// </summary>
		/// <value>A collection of ViewClass instances</value>
		public ViewAttributeCollection ResultAttributes
		{
			get
			{
				return _resultAttributes;
			}
		}
		
		/// <summary>
		/// Gets filter of the ClassView
		/// </summary>
		/// <value>A ViewFilter object</value>
		public ViewFilter Filter
		{
			get
			{
				return _filter;
			}
		}

		/// <summary>
		/// Gets sortby of the ClassView
		/// </summary>
		/// <value>A ViewSortBy object</value>
		public ViewSortBy SortBy
		{
			get
			{
				return _sortBy;
			}
		}

        /// <summary>
        /// Gets locale info
        /// </summary>
        /// <value>A ViewLocaleInfo object</value>
        public ViewLocaleInfo LocaleInfo
        {
            get
            {
                return _localeInfo;
            }
        }

		/// <summary>
		/// Gets the flattened search filter expressions
		/// </summary>
		/// <value>An DataGridViewElementCollection</value>
		public DataGridViewElementCollection FlattenedSearchFilters
		{
			get
			{
				FlattenSearchFiltersVisitor visitor = new FlattenSearchFiltersVisitor();

				if (_filter.Expression != null)
				{
					_filter.Expression.Accept(visitor);
				}

				return visitor.FlattenedSearchFilters;
			}
		}

		/// <summary>
		/// Gets or sets the ClassView that owns this element
		/// </summary>
		/// <value>DataGridViewModel object</value>
		public override DataGridViewModel DataGridView
		{
			get
			{
				return this;
			}
			set
			{
			}
		}

		/// <summary>
		/// Gets the type of element
		/// </summary>
		/// <value>One of ViewElementType values</value>
		public override ViewElementType ElementType 
		{
			get
			{
				return ViewElementType.View;
			}
		}

		/// <summary>
		/// Set a search value to a specified attribute
		/// </summary>
		/// <param name="classAlias">The alias of the attribute owner class.</param>
		/// <param name="attributeName">The name of an attribute</param>
		/// <param name="searchValue">A search value</param>
		public void SetSearchValue(string classAlias, string attributeName, string searchValue)
		{
			FindSearchParameterVisitor visitor = new FindSearchParameterVisitor(classAlias, attributeName);

			if (_filter.Expression != null)
			{
				_filter.Expression.Accept(visitor);
				if (visitor.SearchParameter != null)
				{
					visitor.SearchParameter.ParameterValue = searchValue;
				}
			}
		}

		/// <summary>
		/// Clear the search expression
		/// </summary>
		public void ClearSearchExpression()
		{
			_filter.Expression = null;
		}

		/// <summary>
		/// Clear the sortby attributes
		/// </summary>
		public void ClearSortBy()
		{
			_sortBy.SortAttributes.Clear();
		}

		/// <summary>
		/// Add a search expression to the class view with a logical operator.
		/// If there exists an expression, the new expression is appended to the
		/// end.
		/// </summary>
		/// <param name="expr">The expression to be appended.</param>
		/// <param name="type">The logical operator type, either And or Or</param>
		public void AddSearchExpr(IDataGridViewElement expr, ViewElementType type)
		{
			IDataGridViewElement existing = _filter.Expression;

			if (existing != null)
			{
				ViewLogicalExpr logicalExpr;
				switch (type)
				{
					case ViewElementType.And:
						logicalExpr = new ViewLogicalExpr(ViewElementType.And, existing, expr);
						break;
					case ViewElementType.Or:
						logicalExpr = new ViewLogicalExpr(ViewElementType.Or, existing, expr);
						break;
					default:
						// default is And operator
						logicalExpr = new ViewLogicalExpr(ViewElementType.And, existing, expr);
						break;
				}
				_filter.Expression = logicalExpr;
			}
			else
			{
				// the first expression, set it as root
				_filter.Expression = expr;
			}

			expr.DataGridView = this;
		}

		/// <summary>
		/// Remove the last relational expression from the search expression.
		/// </summary>
		/// <remarks>If there doesn't exist a search expression, this method does nothing.</remarks>
		public void RemoveLastSearchExpr()
		{
			IDataGridViewElement existing = _filter.Expression;

			if (existing != null)
			{
				if (existing is ViewLogicalExpr)
				{
					// there are more than one relational expressions in the search
					// expression, remove the last one
					_filter.Expression = ((ViewBinaryExpr) existing).Left;
					((ViewBinaryExpr) existing).Left = null;
				}
				else if (existing is ViewRelationalExpr || existing is ViewInExpr ||
						 existing is ViewParenthesizedExpr)
				{
					// this is only one relational expression, remove it
					_filter.Expression = null;
				}
			}
		}

		/// <summary>
		/// Read model from an XML file.
		/// </summary>
		/// <param name="fileName">the name of the XML file</param>
		/// <exception cref="DataGridViewException">DataGridViewException is thrown when it fails to
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
			catch (Exception)
			{
				throw new DataGridViewException("Failed to read the file :" + fileName);
			}
		}
		
		/// <summary>
		/// Read model from an stream.
		/// </summary>
		/// <param name="stream">the stream</param>
		/// <exception cref="DataGridViewException">DataGridViewException is thrown when it fails to
		/// read the stream.</exception>
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
					throw new DataGridViewException(e.Message);
				}
			}
		}

		/// <summary>
		/// Read model from a text reader.
		/// </summary>
		/// <param name="reader">the text reader</param>
		/// <exception cref="DataGridViewException">DataGridViewException is thrown when it fails to
		/// read the text reader</exception>
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
					throw new DataGridViewException(e.Message);
				}
			}
		}

		/// <summary>
		/// Write model to an XML file.
		/// </summary>
		/// <param name="fileName">The output file name.</param>
		/// <exception cref="DataGridViewException">DataGridViewException is thrown when it fails to
		/// write to the file.</exception> 
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
			catch (System.IO.IOException)
			{
				throw new DataGridViewException("Failed to write to file :" + fileName);
			}
		}
		
		/// <summary>
		/// Write model as a XML data to a Stream.
		/// </summary>
		/// <param name="stream">the stream object to which to write a XML data</param>
		/// <exception cref="DataGridViewException">DataGridViewException is thrown when it fails to
		/// write to the stream.</exception>
		public void Write(Stream stream)
		{
			try
			{
				XmlDocument doc = GetXmlDocument();

				doc.Save(stream);
			}
			catch (System.IO.IOException)
			{
				throw new DataGridViewException("Failed to write the model");
			}
		}

		/// <summary>
		/// Write model as a XML data to a TextWriter.
		/// </summary>
		/// <param name="writer">the TextWriter instance to which to write a XML schema
		/// </param>
		/// <exception cref="DataGridViewException">DataGridViewException is thrown when it fails to
		/// write to the stream.</exception>
		public void Write(TextWriter writer)
		{
			try
			{
				XmlDocument doc = GetXmlDocument();

				doc.Save(writer);
			}
			catch (System.IO.IOException)
			{
				throw new DataGridViewException("Failed to write the model");
			}
		}

		/// <summary>
		/// Accept a visitor of IDataGridViewElementVisitor type to traverse its
		/// elements.
		/// </summary>
		/// <param name="visitor">A visitor</param>
		public override void Accept(IDataGridViewElementVisitor visitor)
		{
			visitor.VisitClassView(this);
			
			_baseClass.Accept(visitor);

			_filter.Accept(visitor);

			_resultAttributes.Accept(visitor);

			_sortBy.Accept(visitor);

            _localeInfo.Accept(visitor);
		}
		
		/// <summary>
		/// sets the element members from a XML element.
		/// </summary>
		/// <param name="parent">An xml element</param>
		public override void Unmarshal(XmlElement parent)
		{
			base.Unmarshal(parent);

			string str = parent.GetAttribute("viewtype");
			if (str != null && str.Length > 0)
			{
				this._viewType = str;
			}

			str = parent.GetAttribute("taxonomy");
			if (str != null && str.Length > 0)
			{
				this._taxonomy = str;
			}

			str = parent.GetAttribute("taxon");
			if (str != null && str.Length > 0)
			{
				this._taxon = str;
			}

			// the first child is for base class spec
			_baseClass = (ViewClass) ViewElementFactory.Instance.Create((XmlElement) parent.ChildNodes[0]);

			// then comes filter
			_filter = (ViewFilter) ViewElementFactory.Instance.Create((XmlElement) parent.ChildNodes[1]);

			// then comes a collection of result attributes
			_resultAttributes = (ViewAttributeCollection) ViewElementFactory.Instance.Create((XmlElement) parent.ChildNodes[2]);

			// then comes sortby
			_sortBy = (ViewSortBy) ViewElementFactory.Instance.Create((XmlElement) parent.ChildNodes[3]);

            // then comes locale info
            _localeInfo = (ViewLocaleInfo)ViewElementFactory.Instance.Create((XmlElement)parent.ChildNodes[4]);
        }

		/// <summary>
		/// Write values of members to an xml element
		/// </summary>
		/// <param name="parent">An xml element for the element</param>
		public override void Marshal(XmlElement parent)
		{
			base.Marshal(parent);

			if (this._viewType != null)
			{
				parent.SetAttribute("viewtype", _viewType);
			}

			if (this._taxonomy != null)
			{
				parent.SetAttribute("taxonomy", _taxonomy);
			}

			if (this._taxon != null)
			{
				parent.SetAttribute("taxon", _taxon);
			}

			// write the _baseClass
			XmlElement child = parent.OwnerDocument.CreateElement(ViewElementFactory.ConvertTypeToString(_baseClass.ElementType));
			_baseClass.Marshal(child);
			parent.AppendChild(child);

			// write the _filter
			child = parent.OwnerDocument.CreateElement(ViewElementFactory.ConvertTypeToString(_filter.ElementType));
			_filter.Marshal(child);
			parent.AppendChild(child);

			// write the _resultAttributes
			child = parent.OwnerDocument.CreateElement(ViewElementFactory.ConvertTypeToString(_resultAttributes.ElementType));
			_resultAttributes.Marshal(child);
			parent.AppendChild(child);

			// write the _sortBy
			child = parent.OwnerDocument.CreateElement(ViewElementFactory.ConvertTypeToString(_sortBy.ElementType));
			_sortBy.Marshal(child);
			parent.AppendChild(child);

            // write the _localeInfo
            child = parent.OwnerDocument.CreateElement(ViewElementFactory.ConvertTypeToString(_localeInfo.ElementType));
            _localeInfo.Marshal(child);
            parent.AppendChild(child);
		}

		/// <summary>
		/// Gets the xml document that represents an xacl policy
		/// </summary>
		/// <returns>A XmlDocument instance</returns>
		private XmlDocument GetXmlDocument()
		{
			// Marshal the objects to xml document
			XmlDocument doc = new XmlDocument();

			XmlElement element = doc.CreateElement("ClassView");

			doc.AppendChild(element);

			Marshal(element);

			return doc;
		}
	}
}