/*
* @(#)BeforeValueFunction.cs
*
* Copyright (c) 2008 Newtera, Inc. All rights reserved.
*
*/
namespace Newtera.Common.MetaData.DataView
{
	using System;
	using System.Text;
	using System.Xml;

    using Newtera.Common.MetaData.Schema;

	/// <summary>
	/// A BeforeValueFunction represents a function that gets a value of a simple attribute before
    /// update operation. this function is used as a part of an XQuery for validating rules.
	/// </summary>
	/// <version>1.0.0 06 May 2008</version>
    public class BeforeValueFunction : DataViewElementBase, IQueryElement, IFunctionElement
	{
        private DataType _dataType = DataType.String;
        private string _schemaName = null;
        private string _schemaVersion = null;
        private string _className = null;
        private string _attributeName = null;
        private string _attributeCaption = null;
        private string _objId = null; // run-time use

		/// <summary>
		/// Initiating an instance of BeforeValueFunction class
		/// </summary>
        public BeforeValueFunction() : base(@"before")
		{
			Caption = @"before";
		}

		/// <summary>
		/// Initiating an instance of BeforeValueFunction class
		/// </summary>
		/// <param name="xmlElement">The xml element conatins data of the instance</param>
		internal BeforeValueFunction(XmlElement xmlElement) : base()
		{
			Unmarshal(xmlElement);
		}

		/// <summary>
		/// Gets the type of element
		/// </summary>
		/// <value>One of ElementType values</value>
		public override ElementType ElementType 
		{
			get
			{
				return ElementType.Before;
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

            // set value of _schemaName members etc.
            _schemaName = parent.GetAttribute("schemaName");
            _schemaVersion = parent.GetAttribute("schemaVersion");
            _className = parent.GetAttribute("className");
            _attributeName = parent.GetAttribute("attributeName");
            _attributeCaption = parent.GetAttribute("attributeCaption");
          
            // set _dataType member
            string dataTypeString = parent.GetAttribute("dataType");
            if (!string.IsNullOrEmpty(dataTypeString))
            {
                _dataType = DataTypeConverter.ConvertToTypeEnum(dataTypeString);
            }
            else
            {
                _dataType = DataType.String;
            }
		}

		/// <summary>
		/// Write values of members to an xml element
		/// </summary>
		/// <param name="parent">An xml element for the element</param>
		public override void Marshal(XmlElement parent)
		{
			base.Marshal(parent);

            if (!string.IsNullOrEmpty(_schemaName))
            {
                parent.SetAttribute("schemaName", _schemaName);
            }

            if (!string.IsNullOrEmpty(_schemaVersion))
            {
                parent.SetAttribute("schemaVersion", _schemaVersion);
            }

            if (!string.IsNullOrEmpty(_className))
            {
                parent.SetAttribute("className", _className);
            }

            if (!string.IsNullOrEmpty(_attributeName))
            {
                parent.SetAttribute("attributeName", _attributeName);
            }

            if (!string.IsNullOrEmpty(_attributeCaption))
            {
                parent.SetAttribute("attributeCaption", _attributeCaption);
            }

            if (_dataType != DataType.Unknown || _dataType != DataType.String)
            {
                parent.SetAttribute("dataType", DataTypeConverter.ConvertToTypeString(_dataType));
            }
		}

		/// <summary>
		/// Text representation of the element
		/// </summary>
		/// <returns>A String</returns>
		public override string ToString()
		{
			return Caption + "(" + _attributeCaption + ")";
		}

		#region IQueryElement Members

		/// <summary>
		/// Gets the XQuery representation of the element.
		/// </summary>
		/// <returns>A XQuery segmentation</returns>
		public string ToXQuery()
		{
            if (_objId != null)
            {
                return "before(\"" + _schemaName + "\", \"" + _schemaVersion + "\", \"" + _className + "\", \"" + _attributeName + "\", \"" + _objId + "\")";
            }
            else
            {
                return "before(\"\")";
            }
		}

		#endregion

        #region IFunctionElement Members

        /// <summary>
        /// Gets or sets data type of the function.
        /// </summary>
        /// <returns>One of the DataType enum</returns>
        public DataType DataType
        {
            get
            {
                return _dataType;
            }
            set
            {
                _dataType = value;
            }
        }

        /// <summary>
        /// Gets or sets the schema name of the data instance.
        /// </summary>
        /// <value>The schema name</value>
        /// <remarks> Run-time use only, no need to write to data view xml</remarks>
        public string SchemaName
        {
            get
            {
                return _schemaName;
            }
            set
            {
                _schemaName = value;
            }
        }

        /// <summary>
        /// Gets or sets the schema version of the data instance.
        /// </summary>
        /// <value>The schema version</value>
        /// <remarks> Run-time use only, no need to write to data view xml</remarks>
        public string SchemaVersion
        {
            get
            {
                return _schemaVersion;
            }
            set
            {
                _schemaVersion = value;
            }
        }

        /// <summary>
        /// Gets or sets the class name of the data instance.
        /// </summary>
        /// <value>The class name</value>
        /// <remarks> Run-time use only, no need to write to data view xml</remarks>
        public string ClassName
        {
            get
            {
                return _className;
            }
            set
            {
                _className = value;
            }
        }

        /// <summary>
        /// Gets or sets the attribute name of the data instance from which to get a value.
        /// </summary>
        /// <value>The attribute name</value>
        /// <remarks> Run-time use only, no need to write to data view xml</remarks>
        public string AttributeName
        {
            get
            {
                return _attributeName;
            }
            set
            {
                _attributeName = value;
            }
        }

        /// <summary>
        /// Gets or sets the attribute caption of the data instance from which to get a value.
        /// </summary>
        /// <value>The attribute caption</value>
        /// <remarks> Run-time use only, no need to write to data view xml</remarks>
        public string AttributeCaption
        {
            get
            {
                return _attributeCaption;
            }
            set
            {
                _attributeCaption = value;
            }
        }

        /// <summary>
        /// Gets or sets the id of the data instance.
        /// </summary>
        /// <value>The unique obj_id</value>
        /// <remarks> Run-time use only, no need to write to data view xml</remarks>
        public string ObjId
        {
            get
            {
                return _objId;
            }
            set
            {
                _objId = value;
            }
        }

        #endregion
    }
}