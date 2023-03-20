/*
* @(#)MultiAttributeMappingBase.cs
*
* Copyright (c) 2004 Newtera, Inc. All rights reserved.
*
*/
namespace Newtera.Common.MetaData.Mappings
{
	using System;
	using System.Xml;
	using System.Data;
	using System.Collections;
	using System.Collections.Specialized;
	using System.Reflection;

	using Newtera.Common.Core;
	using Newtera.Common.MetaData.DataView;
	using Newtera.Common.MetaData.Mappings.Transform;
	using Newtera.Common.MetaData.Mappings.Generator;

	/// <summary>
	/// The class represents mapping definition between multiple source attributes
	/// and multiple destination attributes
	/// </summary>
	/// <version>1.0.0 15 Nov 2004</version>
	/// <author> Yong Zhang </author>
	public abstract class MultiAttributeMappingBase : MappingNodeBase, ITransformable
	{
		private int _x;
		private int _y;
		private TransformScript _transformScript;
		private AttributeMappingCollection _inputAttributes;
		private AttributeMappingCollection _outputAttributes;
		
		/// <summary>
		/// Initiate an instance of MultiAttributeMappingBase class.
		/// </summary>
		public MultiAttributeMappingBase() : base()
		{
			_x = 0;
			_y = 0;
			_transformScript = new TransformScript();
            if (GlobalSettings.Instance.IsWindowClient)
            {
                _transformScript.ValueChanged += new EventHandler(this.ValueChangedHandler);
            }
			_inputAttributes = new AttributeMappingCollection();
            if (GlobalSettings.Instance.IsWindowClient)
            {
                _inputAttributes.ValueChanged += new EventHandler(this.ValueChangedHandler);
            }
			_outputAttributes = new AttributeMappingCollection();
            if (GlobalSettings.Instance.IsWindowClient)
            {
                _outputAttributes.ValueChanged += new EventHandler(this.ValueChangedHandler);
            }
		}

		/// <summary>
		/// Initiating an instance of MultiAttributeMappingBase class
		/// </summary>
		/// <param name="xmlElement">The xml element conatins data of the instance</param>
		internal MultiAttributeMappingBase(XmlElement xmlElement) : base()
		{
			Unmarshal(xmlElement);
		}

		/// <summary>
		/// Gets or sets the x coordinate of top-left corner of rectangle representing
		/// the mapping.
		/// </summary>
		/// <value>The X </value>
		public int X
		{
			get
			{
				return _x;
			}
			set
			{
				_x = value;

				FireValueChangedEvent(value);
			}
		}

		/// <summary>
		/// Gets or sets the y coordinate of top-left corner of rectangle representing
		/// the mapping.
		/// </summary>
		/// <value>The y </value>
		public int Y
		{
			get
			{
				return _y;
			}
			set
			{
				_y = value;

				FireValueChangedEvent(value);
			}
		}

		/// <summary>
		/// Gets a collection of input attributes
		/// </summary>
		public virtual AttributeMappingCollection InputAttributes
		{
			get
			{
				return _inputAttributes;
			}
			set
			{
				_inputAttributes = value;

				FireValueChangedEvent(value);
			}
		}

		/// <summary>
		/// Gets a collection of output attributes
		/// </summary>
		public virtual AttributeMappingCollection OutputAttributes
		{
			get
			{
				return _outputAttributes;
			}
			set
			{
				_outputAttributes = value;

				FireValueChangedEvent(value);
			}
		}

		/// <summary>
		/// Transform a string to another string instance
		/// </summary>
		/// <param name="srcData">The source string</param>
		/// <returns>The transformed string</returns>
		public string Transform(string srcData)
		{
			return srcData; // TODO, apply expression if any
		}

		/// <summary>
		/// create an MultiAttributeMappingBase from a xml document.
		/// </summary>
		/// <param name="parent">An xml element</param>
		public override void Unmarshal(XmlElement parent)
		{
			base.Unmarshal(parent);

			string str = parent.GetAttribute("X");
			if (str != null && str.Length > 0)
			{
				_x = Int32.Parse(str);
			}

			str = parent.GetAttribute("Y");
			if (str != null && str.Length > 0)
			{
				_y = Int32.Parse(str);
			}

			_transformScript = (TransformScript) MappingNodeFactory.Instance.Create((XmlElement) parent.ChildNodes[0]);
            if (GlobalSettings.Instance.IsWindowClient)
            {
                _transformScript.ValueChanged += new EventHandler(this.ValueChangedHandler);
            }

			// then a collection of  input attributes
			_inputAttributes = (AttributeMappingCollection) MappingNodeFactory.Instance.Create((XmlElement) parent.ChildNodes[1]);
            if (GlobalSettings.Instance.IsWindowClient)
            {
                _inputAttributes.ValueChanged += new EventHandler(this.ValueChangedHandler);
            }
	
			// then a collection of output attributes
			_outputAttributes = (AttributeMappingCollection) MappingNodeFactory.Instance.Create((XmlElement) parent.ChildNodes[2]);
            if (GlobalSettings.Instance.IsWindowClient)
            {
                _outputAttributes.ValueChanged += new EventHandler(this.ValueChangedHandler);
            }
		}

		/// <summary>
		/// write MultiAttributeMappingBase to xml document
		/// </summary>
		/// <param name="parent">An xml element for the element</param>
		public override void Marshal(XmlElement parent)
		{
			base.Marshal(parent);

			// write the _x
			parent.SetAttribute("X", _x.ToString());

			// write the _y
			parent.SetAttribute("Y", _y.ToString());

			// write the transformScript
			XmlElement child = parent.OwnerDocument.CreateElement(MappingNodeFactory.ConvertTypeToString(_transformScript.NodeType));
			_transformScript.Marshal(child);
			parent.AppendChild(child);

			// write the input attributes
			child = parent.OwnerDocument.CreateElement(MappingNodeFactory.ConvertTypeToString(_inputAttributes.NodeType));
			_inputAttributes.Marshal(child);
			parent.AppendChild(child);

			// write output attributes
			child = parent.OwnerDocument.CreateElement(MappingNodeFactory.ConvertTypeToString(_outputAttributes.NodeType));
			_outputAttributes.Marshal(child);
			parent.AppendChild(child);
		}

		#region ITransformable members

		/// <summary>
		/// Gets or sets the information indicating whether the script is enabled
		/// for transformation.
		/// </summary>
		/// <value>true if it is enabled, false otherwise.</value>
		public bool ScriptEnabled
		{
			get
			{
				if (_transformScript != null)
				{
					return _transformScript.Enabled;
				}
				else
				{
					return false;
				}
			}
			set
			{
				_transformScript.Enabled = value;
			}
		}

		/// <summary>
		/// Gets or sets type of the script language
		/// </summary>
		/// <value>One of ScriptLanguage enum values. Default is CSharp</value>
		public ScriptLanguage ScriptLanguage
		{
			get
			{
				if (_transformScript != null)
				{
					return _transformScript.ScriptLanguage;
				}
				else
				{
					return ScriptLanguage.CSharp;
				}
			}
			set
			{
				_transformScript.ScriptLanguage = value;
			}
		}

		/// <summary>
		/// Gets or sets class type of the script
		/// </summary>
		/// <value>Class type string</value>
		public string ClassType
		{
			get
			{
				if (_transformScript != null)
				{
					return _transformScript.ClassType;
				}
				else
				{
					return null;
				}
			}
			set
			{
				_transformScript.ClassType = value;
			}
		}

		/// <summary>
		/// Gets or sets a script of transforming attribute value.
		/// </summary>
		/// <value> The transform expression.</value>
		public string Script
		{
			get
			{
				return _transformScript.Script;
			}
			set
			{
				_transformScript.Script = value;

				FireValueChangedEvent(value);
			}
		}

		/// <summary>
		/// Perform transformation and return a collection of IAttributeSetter instances that set a source
		/// value to destination attribute.
		/// </summary>
		/// <param name="srcDataRow">The DataRow from source</param>
		/// <param name="srcDataView">The DataViewModel for source class.</param>
		/// <param name="dstDataSet">The destination DataSet</param>
		/// <param name="dstDataView">The DataViewModel for destination class</param>
		/// <param name="rowIndex">The row index of current transformed row.</param>
		/// <param name="assembly">The assembly contains transformer classes</param>
		/// <returns>A AttributeSetterCollection instance.</returns>
		public AttributeSetterCollection DoTransform(DataRow srcDataRow, DataViewModel srcDataView,
			DataSet dstDataSet, DataViewModel dstDataView, int rowIndex, Assembly assembly)
		{
			if (_transformScript.Script != null && _transformScript.Enabled)
			{
				if (Transformer == null)
				{
					string typeName = NewteraNameSpace.TRANSFORMER_NAME_SPACE + "." + _transformScript.ClassType;
					Transformer = (ITransformer) assembly.CreateInstance(typeName);
				}
				
				// get the input values
				NameValueCollection inputValues = new NameValueCollection();
				foreach (InputOutputAttribute input in _inputAttributes)
				{
					IAttributeGetter getter = GetterFactory.Instance.Create(input.GetterType,
						srcDataRow, input.AttributeName, srcDataView);

					string srcValue = getter.GetValue();

					inputValues[input.AttributeName] = srcValue;
				}

				// construct an empty output value array
				NameValueCollection outputValues = new NameValueCollection();
				foreach (InputOutputAttribute output in _outputAttributes)
				{
					outputValues[output.AttributeName] = ""; // initialize with empty string
				}

                Transformer.SourceDataTable = srcDataRow.Table;

				try
				{
					// perform the transformation
					Transformer.Transform(inputValues, outputValues);
				}
				catch (Exception ex)
				{
					throw new MappingException("Encounter erros while executing " + _transformScript.ClassType + ":" + ex.Message);
				}

				// set the transformed values to out attributes
				foreach (InputOutputAttribute output in _outputAttributes)
				{
					output.AttributeValue = outputValues[output.AttributeName];
				}
			}

			AttributeSetterCollection setters = new AttributeSetterCollection();
			IAttributeSetter setter = null;

			foreach (InputOutputAttribute output in _outputAttributes)
			{
				// create an appropriate setter according to the destination
				// setter type
				switch (output.SetterType)
				{
					case SetterType.SimpleAttributeSetter:
						setter = new SimpleAttributeSetter(output.AttributeValue,
							dstDataSet, dstDataView,
							rowIndex, output.AttributeName);
						break;
					case SetterType.PrimaryKeySetter:
						setter = new PrimaryKeySetter(output.AttributeValue,
							dstDataSet, dstDataView,
							rowIndex, output.AttributeName,
							output.RelationshipAttributeName);
						break;
					case SetterType.ArrayDataCellSetter:
						setter = new ArrayDataCellSetter(output.AttributeValue,
							dstDataSet, dstDataView,
							rowIndex, GetArrayAttributeName(output.AttributeName),
							output.RowIndex, output.ColIndex);
						break;
				}
			
				if (setter != null)
				{
					setters.Add(setter);
				}
			}

			return setters;
		}

		#endregion

		/// <summary>
		/// Gets name of the array attribute from an array cell name
		/// </summary>
		private string GetArrayAttributeName(string cellName)
		{
			string arrayAttributeName = null;

			int pos = cellName.IndexOf("_");
			if (pos > 0)
			{
				arrayAttributeName = cellName.Substring(0, pos);
			}
			else
			{
				arrayAttributeName = cellName;
			}

			return arrayAttributeName;
		}
	}
}