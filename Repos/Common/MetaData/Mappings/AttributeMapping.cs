/*
* @(#)AttributeMapping.cs
*
* Copyright (c) 2003 Newtera, Inc. All rights reserved.
*
*/
namespace Newtera.Common.MetaData.Mappings
{
	using System;
	using System.Data;
	using System.Xml;
	using System.Collections;
	using System.Collections.Specialized;
	using System.CodeDom.Compiler;
	using System.Reflection;

	using Newtera.Common.Core;
	using Newtera.Common.MetaData.DataView;
	using Newtera.Common.MetaData.Mappings.Transform;
	using Newtera.Common.MetaData.Mappings.Generator;

	/// <summary>
	/// The class represents mapping definition between a single source and
	/// a single destination attribute.
	/// </summary>
	/// <version>1.0.0 02 Sep 2004</version>
	/// <author> Yong Zhang </author>
	public class AttributeMapping : MappingNodeBase, ITransformable
	{
		private string _sourceAttribute;
		private string _destinationAttribute;
		private GetterType _getterType;

		internal TransformScript _transformScript;
		
		/// <summary>
		/// Initiate an instance of AttributeMapping class.
		/// </summary>
		public AttributeMapping(string sourceAttribute, string destinationAttribute) : base()
		{
			_sourceAttribute = sourceAttribute;
			_destinationAttribute = destinationAttribute;
			_getterType = GetterType.SimpleAttributeGetter;
			_transformScript = new TransformScript();
            if (GlobalSettings.Instance.IsWindowClient)
            {
                _transformScript.ValueChanged += new EventHandler(this.ValueChangedHandler);
            }
		}

		/// <summary>
		/// Initiating an instance of AttributeMapping class
		/// </summary>
		/// <param name="xmlElement">The xml element conatins data of the instance</param>
		internal AttributeMapping(XmlElement xmlElement) : base()
		{
			Unmarshal(xmlElement);
		}

		/// <summary>
		/// Gets  or sets name of the source attribute associated with a AttributeMapping
		/// </summary>
		public virtual string SourceAttributeName
		{
			get
			{
				return _sourceAttribute;
			}
			set
			{
				_sourceAttribute = value;

				FireValueChangedEvent(value);
			}
		}

		/// <summary>
		/// Gets or sets name of the destination attribute associated with a AttributeMapping
		/// </summary>
		public virtual string DestinationAttributeName
		{
			get
			{
				return _destinationAttribute;
			}
			set
			{
				_destinationAttribute = value;

				FireValueChangedEvent(value);
			}
		}

		/// <summary>
		/// Gets or sets getter type of the source attribute associated with a AttributeMapping
		/// </summary>
		/// <value>One of the GetterType enum</value>
		public virtual GetterType GetterType
		{
			get
			{
				return _getterType;
			}
			set
			{
				_getterType = value;

				FireValueChangedEvent(value);
			}
		}

		/// <summary>
		/// Gets the type of node
		/// </summary>
		/// <value>One of NodeType values</value>
		public override NodeType NodeType
		{
			get
			{
				return NodeType.AttributeMapping;
			}
		}

		/// <summary>
		/// create an AttributeMapping from a xml document.
		/// </summary>
		/// <param name="parent">An xml element</param>
		public override void Unmarshal(XmlElement parent)
		{
			base.Unmarshal(parent);

			string str = parent.GetAttribute("Source");
			if (str != null && str.Length > 0)
			{
				_sourceAttribute = str;
			}
			else
			{
				_sourceAttribute = null;
			}

			str = parent.GetAttribute("Destination");
			if (str != null && str.Length > 0)
			{
				_destinationAttribute = str;
			}
			else
			{
				_destinationAttribute = null;
			}

			str = parent.GetAttribute("SrcType");
			if (str != null && str.Length > 0)
			{
				_getterType = (GetterType) Enum.Parse(typeof(GetterType), str);
			}
			else
			{
				_getterType = GetterType.SimpleAttributeGetter; // SimpleAttributeGetter is default
			}

			if (parent.ChildNodes.Count > 0)
			{
				_transformScript = (TransformScript) MappingNodeFactory.Instance.Create((XmlElement) parent.ChildNodes[0]);
			}
			else
			{
				_transformScript = new TransformScript();
			}

            if (GlobalSettings.Instance.IsWindowClient)
            {
                _transformScript.ValueChanged += new EventHandler(this.ValueChangedHandler);
            }
		}

		/// <summary>
		/// write AttributeMapping to xml document
		/// </summary>
		/// <param name="parent">An xml element for the element</param>
		public override void Marshal(XmlElement parent)
		{
			base.Marshal(parent);

			// write the _sourceAttribute
			if (_sourceAttribute != null && _sourceAttribute.Length > 0)
			{
				parent.SetAttribute("Source", _sourceAttribute);
			}
		
			// write the _destinationAttribute
			if (_destinationAttribute != null && _destinationAttribute.Length > 0)
			{
				parent.SetAttribute("Destination", _destinationAttribute);
			}

			// write the _getterType
			if (_getterType != GetterType.SimpleAttributeGetter)
			{
				// do not write the default value
				parent.SetAttribute("SrcType", Enum.GetName(typeof(GetterType), _getterType));
			}

			// write the transformScript
			if (_transformScript != null)
			{
				XmlElement child = parent.OwnerDocument.CreateElement(MappingNodeFactory.ConvertTypeToString(_transformScript.NodeType));
				_transformScript.Marshal(child);
				parent.AppendChild(child);
			}
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
				if (_transformScript != null)
				{
					return _transformScript.Script;
				}
				else
				{
					return null;
				}
			}
			set
			{
				if (_transformScript == null)
				{
					_transformScript = new TransformScript();
				}

				_transformScript.Script = value;
			}
		}

		/// <summary>
		/// Perform transformation and return a collection of IAttributeSetter instances that set a source
		/// value to destination attribute.
		/// </summary>
		/// <param name="srcDataRow">The DataRow from source</param>
		/// <param name="srcDataView">The DataViewModel for source class, can be null.</param>		
		/// <param name="dstDataSet">The destination DataSet</param>
		/// <param name="dstDataView">The DataViewModel for destination class</param>
		/// <param name="rowIndex">The row index of current transformed row.</param>
		/// <param name="assembly">The assembly contains transformer classes</param>
		/// <returns>A AttributeSetterCollection instance.</returns>
		public virtual AttributeSetterCollection DoTransform(DataRow srcDataRow,
			DataViewModel srcDataView, DataSet dstDataSet,
			DataViewModel dstDataView, int rowIndex, Assembly assembly)
		{
			IAttributeGetter getter = GetterFactory.Instance.Create(this.GetterType,
				srcDataRow, _sourceAttribute, srcDataView);

			string srcValue = getter.GetValue();

			if (_transformScript.Script != null && _transformScript.Enabled)
			{
				if (Transformer == null)
				{
					string typeName = NewteraNameSpace.TRANSFORMER_NAME_SPACE + "." + _transformScript.ClassType;
					Transformer = (ITransformer) assembly.CreateInstance(typeName);
				}

				NameValueCollection inputValues = new NameValueCollection();
				inputValues[_sourceAttribute] = srcValue;
				NameValueCollection outputValues = new NameValueCollection();
				outputValues[_destinationAttribute] = "";

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

				srcValue = outputValues[0];
			}

			AttributeSetterCollection setters = new AttributeSetterCollection();

			IAttributeSetter setter = new SimpleAttributeSetter(srcValue,
				dstDataSet, dstDataView,
				rowIndex, _destinationAttribute);
			
			setters.Add(setter);

			return setters;
		}
		
		#endregion
	}
}