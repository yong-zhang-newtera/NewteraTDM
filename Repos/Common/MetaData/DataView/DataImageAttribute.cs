/*
* @(#)DataImageAttribute.cs
*
* Copyright (c) 2008 Newtera, Inc. All rights reserved.
*
*/
namespace Newtera.Common.MetaData.DataView
{
	using System;
	using System.Text;
	using System.Data;
	using System.Text.RegularExpressions;
	using System.Xml;
	using System.Collections;

    using Newtera.Common.Core;
	using Newtera.Common.MetaData.Schema;
    using Newtera.Common.MetaData.XaclModel;

	/// <summary>
	/// A DataImageAttribute instance represents an image attribute in a data view.
	/// It can appears in the result attribute collection.
	/// </summary>
	/// <version>1.0.1 04 Jul 2008</version>
	public class DataImageAttribute : DataViewElementBase, IQueryElement
	{
		private string _ownerClassAlias;
		private string _value;

		/// <summary>
		/// Initiating an instance of DataImageAttribute class
		/// </summary>
		/// <param name="name">Name of the array attribute</param>
		/// <param name="ownerClassAlias">The unique alias of DataClass element that owns this attribute</param>
		public DataImageAttribute(string name, string ownerClassAlias) : base(name)
		{
			_ownerClassAlias = ownerClassAlias;
			_value = null; // run-time use only
		}

		/// <summary>
		/// Initiating an instance of DataImageAttribute class
		/// </summary>
		/// <param name="xmlElement">The xml element conatins data of the instance</param>
		internal DataImageAttribute(XmlElement xmlElement) : base()
		{
			Unmarshal(xmlElement);
			_value = null; // run-time use only
		}

		/// <summary>
		/// Gets the type of element
		/// </summary>
		/// <value>One of ElementType values</value>
		public override ElementType ElementType 
		{
			get
			{
				return ElementType.ImageAttribute;
			}
		}

        /// <summary>
        /// Gets or sets an alias that is used to identifies the simple attribute when used
        /// in the search expression of data view.
        /// </summary>
        /// <value>A string, can be null.</value>
        /// <remarks> Run-time use only, no need to write to data view xml</remarks>
        public override string Alias
        {
            get
            {
                if (string.IsNullOrEmpty(base.Alias))
                {
                    // default alias
                    string alias = _ownerClassAlias + "_" + Name;
                    base.Alias = alias;
                }

                return base.Alias;
            }
            set
            {
                base.Alias = value;
            }
        }

		/// <summary>
		/// Gets alias of class that owns this attribute
		/// </summary>
		/// <value>Owner class alias</value>
		public string OwnerClassAlias
		{
			get
			{
				return _ownerClassAlias;
			}
		}

        /// <summary>
        /// Gets the attribute value
        /// </summary>
        public string AttributeValue
        {
            get
            {
                return _value;
            }
            set
            {
                _value = value;
            }
        }

		/// <summary>
		/// Gets the information indicating whether the attribute has an value
		/// </summary>
		/// <value>true if it has, false, otherwise</value>
		public bool HasValue
		{
			get
			{
				if (_value != null && _value.Length > 0)
				{
					return true;
				}
				else
				{
					return false;
				}
			}
		}

        /// <summary>
        /// Gets the information indicating whether the given value is different from
        /// the attribute value
        /// </summary>
        /// <param name="val">The given value</param>
        /// <returns>true if they are different, false otherwise.</returns>
        /// <remarks>If the give value is null or empty string, and the attribute
        /// value is null or empty string, they are considered to be same</remarks>
        public bool IsValueDifferent(string val)
        {
            string attributeValue = this.AttributeValue;
            if (attributeValue != null && attributeValue.Length == 0)
            {
                attributeValue = null;
            }

            if (val != null && val.Length == 0)
            {
                val = null;
            }

            if (attributeValue != val)
            {
                return true;
            }
            else
            {
                return false;
            }
        }

		/// <summary>
		/// Accept a visitor of IDataViewElementVisitor type to traverse its
		/// elements.
		/// </summary>
		/// <param name="visitor">A visitor</param>
		public override void Accept(IDataViewElementVisitor visitor)
		{
            visitor.VisitImageAttribute(this);
		}
		
		/// <summary>
		/// Gets or sets the schema model element that the data view element associates with.
		/// </summary>
		/// <value>The SchemaModelElement.</value>
		public override SchemaModelElement GetSchemaModelElement()
		{
			if (_schemaModelElement == null && DataView != null)
			{
				DataClass ownerClass = DataView.FindClass(_ownerClassAlias);
                if (ownerClass != null)
                {
                    ClassElement classElement = DataView.SchemaModel.FindClass(ownerClass.ClassName);
                    _schemaModelElement = classElement.FindInheritedImageAttribute(Name);
                }
			}

			return _schemaModelElement;
		}

		/// <summary>
		/// sets the element members from a XML element.
		/// </summary>
		/// <param name="parent">An xml element</param>
		public override void Unmarshal(XmlElement parent)
		{
			base.Unmarshal(parent);

			// set value of _ownerClassAlias member
			_ownerClassAlias = parent.GetAttribute("OwnerAlias");
		}

		/// <summary>
		/// Write values of members to an xml element
		/// </summary>
		/// <param name="parent">An xml element for the element</param>
		public override void Marshal(XmlElement parent)
		{
			base.Marshal(parent);

			// write the _ownerName member
			parent.SetAttribute("OwnerAlias", _ownerClassAlias);
		}

		/// <summary>
		/// Text representation of the element
		/// </summary>
		/// <returns>A String</returns>
		public override string ToString()
		{
			return Caption;
		}



        #region IXaclObject Members

        /// <summary>
        /// Return a xpath representation of the SchemaModelElement
        /// </summary>
        /// <returns>a xapth representation</returns>
        public override string ToXPath()
        {
            if (_xpath == null)
            {
                _xpath = this.Parent.ToXPath() + "/" + this.Name + NewteraNameSpace.ATTRIBUTE_SUFFIX;
            }

            return _xpath;
        }

        /// <summary>
        /// Return a  parent of the SchemaModelElement
        /// </summary>
        /// <returns>The parent of the SchemaModelElement</returns>
        public override IXaclObject Parent
        {
            get
            {
                ClassElement classElement = null;
                DataClass ownerClass = DataView.FindClass(_ownerClassAlias);
                if (ownerClass != null)
                {
                    classElement = DataView.SchemaModel.FindClass(ownerClass.ClassName);
                    if (classElement == null)
                    {
                        throw new Exception("Unable to find the class whose caption is " + ownerClass.Caption + " in the schema " + DataView.SchemaModel.SchemaInfo.Name);
                    }
                }
                else
                {
                    throw new Exception("Unable to find the class whose alias is " + _ownerClassAlias + " in a data view for class " + DataView.BaseClass.Caption);
                }

                // the parent is the owner class element
                return classElement;
            }
        }

        /// <summary>
        /// Return a  of children of the SchemaModelElement
        /// </summary>
        /// <returns>The collection of IXaclObject nodes</returns>
        public override IEnumerator GetChildren()
        {
            // return an empty enumerator
            ArrayList children = new ArrayList();
            return children.GetEnumerator();
        }

        #endregion

		#region IQueryElement Members

		/// <summary>
		/// Gets the XQuery representation of the element.
		/// </summary>
		/// <returns>A XQuery segmentation</returns>
		public string ToXQuery()
		{
			StringBuilder query = new StringBuilder();

			query.Append("$").Append(_ownerClassAlias).Append("/").Append(Name);

			return query.ToString();
		}

		#endregion
	}
}