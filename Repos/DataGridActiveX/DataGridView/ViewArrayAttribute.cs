/*
* @(#)ViewArrayAttribute.cs
*
* Copyright (c) 2007 Newtera, Inc. All rights reserved.
*
*/
namespace Newtera.DataGridActiveX.DataGridView
{
	using System;
	using System.Text;
	using System.Collections.Specialized;
    using System.Text.RegularExpressions;
	using System.Xml;

	/// <summary>
	/// A ViewArrayAttribute instance represents an array attribute in a class view.
	/// It does not use as search attribute
	/// </summary>
	/// <version>1.0.0 29 Apr 2007</version>
    public class ViewArrayAttribute : ViewAttribute
	{
        private StringCollection _columnTitles = null;
        private const string DELIMETER = ";";

		/// <summary>
		/// Initiating an instance of ViewArrayAttribute class
		/// </summary>
		/// <param name="name">Name of the array attribute</param>
		/// <param name="ownerClassAlias">Owner class alias</param>
		public ViewArrayAttribute(string name, string ownerClassAlias) : this(name, ViewDataType.Unknown, ownerClassAlias)
		{
            _columnTitles = new StringCollection();
		}

		/// <summary>
		/// Initiating an instance of ViewArrayAttribute class
		/// </summary>
		/// <param name="name">Name of the simple attribute</param>
		/// <param name="dataType">Data Type</param>
		/// <param name="ownerClassAlias">The class alias</param>
        public ViewArrayAttribute(string name, ViewDataType dataType, string ownerClassAlias)
            : base(name, dataType, ownerClassAlias)
		{

		}

		/// <summary>
		/// Initiating an instance of ViewArrayAttribute class
		/// </summary>
		/// <param name="xmlElement">The xml element conatins data of the instance</param>
        internal ViewArrayAttribute(XmlElement xmlElement) : base()
		{
			Unmarshal(xmlElement);
		}

        /// <summary>
        /// Gets or sets a collection of array column titles
        /// </summary>
        public StringCollection ColumnTitles
        {
            get
            {
                return _columnTitles;
            }
            set
            {
                _columnTitles = value;
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
				return ViewElementType.ArrayAttribute;
			}
		}

		/// <summary>
		/// Accept a visitor of IDataGridViewElementVisitor type to traverse its
		/// elements.
		/// </summary>
		/// <param name="visitor">A visitor</param>
		public override void Accept(IDataGridViewElementVisitor visitor)
		{
			visitor.VisitArrayAttribute(this);
		}

		/// <summary>
		/// sets the element members from a XML element.
		/// </summary>
		/// <param name="parent">An xml element</param>
		public override void Unmarshal(XmlElement parent)
		{
			base.Unmarshal(parent);

            // Set _columnTitles member
            _columnTitles = ConvertToCollection(parent.GetAttribute("ColumnTitles"));
		}

		/// <summary>
		/// Write values of members to an xml element
		/// </summary>
		/// <param name="parent">An xml element for the element</param>
		public override void Marshal(XmlElement parent)
		{
			base.Marshal(parent);

            // Write _columnTitles
            if (_columnTitles.Count > 0)
            {
                parent.SetAttribute("ColumnTitles", ConvertToString(_columnTitles));
            }
		}

		/// <summary>
		/// Text representation of the element
		/// </summary>
		/// <returns>A String</returns>
		public override string ToString()
		{
			return Caption;
		}

        /// <summary>
        /// Convert a comma separated column title string into a StringCollection object
        /// </summary>
        /// <param name="colTitles">A comma separated column title string</param>
        /// <returns>A StringCollection object</returns>
        private StringCollection ConvertToCollection(string colTitles)
        {
            StringCollection columnTitles = new StringCollection();

            if (colTitles != null && colTitles.Length > 0)
            {
                Regex regex = new Regex(DELIMETER);
                string[] values = regex.Split(colTitles);
                columnTitles.AddRange(values);
            }

            return columnTitles;
        }

        /// <summary>
        /// Convert a StringCollection object into a comma separated column title string.
        /// </summary>
        /// <param name="columnTitles">A StringCollection object</param>
        /// <returns>A comma separated column title string</returns>
        private string ConvertToString(StringCollection columnTitles)
        {
            StringBuilder builder = new StringBuilder();

            for (int i = 0; i < columnTitles.Count; i++)
            {
                if (i > 0)
                {
                    builder.Append(DELIMETER);
                }

                builder.Append(columnTitles[i]);
            }

            return builder.ToString();
        }
	}
}