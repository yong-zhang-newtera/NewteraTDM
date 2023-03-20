/*
* @(#)ErrorFunction.cs
*
* Copyright (c) 2003-2007 Newtera, Inc. All rights reserved.
*
*/
namespace Newtera.Common.MetaData.DataView
{
	using System;
	using System.Text;
	using System.Xml;
    using System.ComponentModel;

    using Newtera.Common.MetaData.Schema;

	/// <summary>
	/// A ErrorFunction instance represents a function that generates an error message.
    /// this function is used as a part of an XQuery.
	/// </summary>
	/// <version>1.0.0 16 Oct 2007</version>
    public class ErrorFunction : DataViewElementBase, IQueryElement, IFunctionElement
	{
        private const string DefaultMessage = "Validating error occures, no specific message defined";
        private string _message = null;

		/// <summary>
		/// Initiating an instance of ErrorFunction class
		/// </summary>
		public ErrorFunction() : base(@"error")
		{
		}

		/// <summary>
		/// Initiating an instance of ErrorFunction class
		/// </summary>
		/// <param name="xmlElement">The xml element conatins data of the instance</param>
		internal ErrorFunction(XmlElement xmlElement) : base()
		{
			Unmarshal(xmlElement);
		}

		/// <summary>
		/// Gets the type of element
		/// </summary>
		/// <value>One of ElementType values</value>
        [BrowsableAttribute(false)]	
        public override ElementType ElementType 
		{
			get
			{
				return ElementType.Error;
			}
		}

		/// <summary>
		/// Gets or sets the error message.
		/// </summary>
		/// <value>The error message</value>
        public string Message
		{
			get
			{
                if (!string.IsNullOrEmpty(_message))
                {
                    return _message;
                }
                else
                {
                    return DefaultMessage;
                }
			}
			set
			{
                // escape the error message by replace the double quote with signle quote
                _message = value;
                if (_message != null)
                {
                    _message = _message.Replace("\"", "'");
                }
			}
		}

		/// <summary>
		/// Accept a visitor of IDataViewElementVisitor type to traverse its
		/// elements.
		/// </summary>
		/// <param name="visitor">A visitor</param>
		public override void Accept(IDataViewElementVisitor visitor)
		{
			visitor.VisitFunction(this);
		}

		/// <summary>
		/// sets the element members from a XML element.
		/// </summary>
		/// <param name="parent">An xml element</param>
		public override void Unmarshal(XmlElement parent)
		{
			base.Unmarshal(parent);

            // set value of _message member
            _message = parent.GetAttribute("msg");
		}

		/// <summary>
		/// Write values of members to an xml element
		/// </summary>
		/// <param name="parent">An xml element for the element</param>
		public override void Marshal(XmlElement parent)
		{
			base.Marshal(parent);

            if (!string.IsNullOrEmpty(_message))
            {
                parent.SetAttribute("msg", _message);
            }
		}

		/// <summary>
		/// Text representation of the element
		/// </summary>
		/// <returns>A String</returns>
		public override string ToString()
		{
			return "error(\"" + _message + "\")";
		}

        /// <summary>
        /// Show only specific properties in the PropertyGrid
        /// </summary>
        /// <returns>true if the property is hidden, false otherwise.</returns>
        protected override bool IsHiddenProperty(PropertyDescriptor property)
        {
            if (property.Name == "Message")
            {
                return false;
            }
            else
            {
                return true;
            }
        }

		#region IQueryElement Members

		/// <summary>
		/// Gets the XQuery representation of the element.
		/// </summary>
		/// <returns>A XQuery segmentation</returns>
		public string ToXQuery()
		{
			return "error(\"" + _message + "\")";
		}

		#endregion

        #region IFunctionElement Members

        /// <summary>
        /// Gets returned data type of the function.
        /// </summary>
        /// <returns>One of the DataType enum</returns>
        [BrowsableAttribute(false)]	
        public DataType DataType
        {
            get
            {
                return DataType.String;
            }
            set
            {
            }
        }

        /// <summary>
        /// Gets or sets schema name of a data instance as function parameter, can be null
        /// </summary>
        [BrowsableAttribute(false)]	
        public string SchemaName
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
        /// Gets or sets schema version of a data instance as function parameter, can be null
        /// </summary>
        [BrowsableAttribute(false)]	
        public string SchemaVersion
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
        /// Gets or sets class name of a data instance as function parameter, can be null
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
        /// Gets or sets attribute name of a data instance as function parameter, can be null
        /// </summary>
        [BrowsableAttribute(false)]	
        public string AttributeName
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
        /// Gets or sets the attribute caption of the data instance from which to get a value.
        /// </summary>
        /// <value>The attribute caption</value>
        /// <remarks> Run-time use only, no need to write to data view xml</remarks>
        [BrowsableAttribute(false)]	
        public string AttributeCaption
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
        /// Gets or sets a data instance id to the function
        /// </summary>
        [BrowsableAttribute(false)]	
        public string ObjId
        {
            get
            {
                return null;
            }
            set
            {
            }
        }

        #endregion
    }
}