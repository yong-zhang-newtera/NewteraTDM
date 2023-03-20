/*
* @(#)InputParameter.cs
*
* Copyright (c) 2007 Newtera, Inc. All rights reserved.
*
*/
namespace Newtera.WFModel
{
	using System;
    using System.ComponentModel;
    using System.Drawing.Design;

    using Newtera.Common.MetaData;

	/// <summary>
	/// A class represents an input parameter to a workflow.
	/// </summary>
	/// <version>1.0.0 15 Nov 2007 </version>
    [Serializable]
    public class InputParameter : CustomTypeDescriptorBase
	{
        private string _name;
        private ParameterDataType _dataType;
        private object _value;
        private ParameterBindingInfo _bindingInfo;

        /// <summary>
        /// Default constructor
        /// </summary>
        public InputParameter()
        {
            _name = null;
            _dataType = ParameterDataType.String; // default to string
            _value = null;
            _bindingInfo = new ParameterBindingInfo();
        }

        public InputParameter(string name)
        {
            _name = name;
            _dataType = ParameterDataType.String; // default to string
            _value = null;
            _bindingInfo = new ParameterBindingInfo();
        }

        /// <summary>
        /// Gets or sets the name of the parameter
        /// </summary>
        [
            CategoryAttribute("System"),
            DescriptionAttribute("The name of the parameter"),
            ReadOnlyAttribute(true),
        ]
        public string Name
        {
            get
            {
                return _name;
            }
            set
            {
                _name = value;
            }
        }

        /// <summary>
        /// Gets or sets the value of the parameter
        /// </summary>
        [BrowsableAttribute(false)]
        public object Value
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
        /// Gets or sets the data type of the parameter
        /// </summary>
        [
            CategoryAttribute("System"),
            DescriptionAttribute("The data type of the parameter")
        ]
        public ParameterDataType DataType
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
        /// Gets or sets the data type of the parameter
        /// </summary>
        [
            CategoryAttribute("System"),
            DescriptionAttribute("The data binding to the parameter"),
            EditorAttribute("WorkflowStudio.ParameterBindingEditor, WorkflowStudio", typeof(UITypeEditor)),
            TypeConverterAttribute("WorkflowStudio.ParameterBindingConverter, WorkflowStudio")
        ]
        public ParameterBindingInfo ParameterBinding
        {
            get
            {
                return _bindingInfo;
            }
            set
            {
                _bindingInfo = value;
            }
        }

        /// <summary>
        /// Clone the InputParameter object
        /// </summary>
        /// <returns>The cloned object</returns>
        public InputParameter Clone()
        {
            InputParameter clone = new InputParameter(Name);
            clone.Value = Value;
            clone.DataType = DataType;
            clone.ParameterBinding = ParameterBinding.Clone();

            return clone;
        }

        public override string ToString()
        {
            // return value
            if (_value != null)
            {
                return _value.ToString();
            }
            else
            {
                return "";
            }
        }
	}
}