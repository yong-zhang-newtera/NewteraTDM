/*
* @(#)WFStateFunction.cs
*
* Copyright (c) 2003-2007 Newtera, Inc. All rights reserved.
*
*/
namespace Newtera.Common.MetaData.DataView
{
	using System;
	using System.Text;
	using System.Xml;

    using Newtera.Common.MetaData.Schema;

	/// <summary>
	/// A WFStateFunction instance represents a function that gets name of the current state
    /// of a state-machine workflow that is bound to a data instance. this function is used as
    /// a part of an XQuery.
	/// </summary>
	/// <version>1.0.0 15 Oct 2007</version>
    public class WFStateFunction : DataViewElementBase, IQueryElement, IFunctionElement
	{
        private string _objId = null; // run-time use

		/// <summary>
		/// Initiating an instance of WFStateFunction class
		/// </summary>
        public WFStateFunction() : base(@"wfstate")
		{
			Caption = @"wfstate()";
		}

		/// <summary>
		/// Initiating an instance of WFStateFunction class
		/// </summary>
		/// <param name="xmlElement">The xml element conatins data of the instance</param>
		internal WFStateFunction(XmlElement xmlElement) : base()
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
				return ElementType.WFState;
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
		}

		/// <summary>
		/// Write values of members to an xml element
		/// </summary>
		/// <param name="parent">An xml element for the element</param>
		public override void Marshal(XmlElement parent)
		{
			base.Marshal(parent);
		}

		/// <summary>
		/// Text representation of the element
		/// </summary>
		/// <returns>A String</returns>
		public override string ToString()
		{
			return Caption;
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
                return "wfstate(\"" + _objId + "\")";
            }
            else
            {
                return "wfstate(\"\")";
            }
		}

		#endregion

        #region IFunctionElement Members

        /// <summary>
        /// Gets or sets returned data type of the function.
        /// </summary>
        /// <returns>One of the DataType enum</returns>
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
        public string SchemaName {
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