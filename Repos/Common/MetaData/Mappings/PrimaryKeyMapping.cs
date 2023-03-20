/*
* @(#)PrimaryKeyMapping.cs
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

	/// <summary>
	/// The class represents mapping definition of a source attribute and
	/// an primary key of a relationship attribute.
	/// </summary>
	/// <version>1.0.0 17 Nov 2004</version>
	/// <author> Yong Zhang </author>
	public class PrimaryKeyMapping : AttributeMapping
	{
		private string _relationshipAttributeName = null;

		/// <summary>
		/// Initiate an instance of PrimaryKeyMapping class.
		/// </summary>
		public PrimaryKeyMapping(string sourceAttribute, string destinationAttribute) : base(sourceAttribute, destinationAttribute)
		{
		}

		/// <summary>
		/// Initiating an instance of PrimaryKeyMapping class
		/// </summary>
		/// <param name="xmlElement">The xml element conatins data of the instance</param>
		internal PrimaryKeyMapping(XmlElement xmlElement) : base(xmlElement)
		{
		}

		/// <summary>
		/// Gets name of the relationship attribute associated with a PrimaryKeyMapping
		/// </summary>
		public string RelationshipAttributeName
		{
			get
			{
				if (_relationshipAttributeName == null)
				{
					int pos = base.DestinationAttributeName.IndexOf("_");
					if (pos > 0)
					{
						_relationshipAttributeName = base.DestinationAttributeName.Substring(0, pos);
					}
					else
					{
						_relationshipAttributeName = base.DestinationAttributeName;
					}
				}

				return _relationshipAttributeName;
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
				return NodeType.PrimaryKeyMapping;
			}
		}

		/// <summary>
		/// create an PrimaryKeyMapping from a xml document.
		/// </summary>
		/// <param name="parent">An xml element</param>
		public override void Unmarshal(XmlElement parent)
		{
			base.Unmarshal(parent);
		}

		/// <summary>
		/// write PrimaryKeyMapping to xml document
		/// </summary>
		/// <param name="parent">An xml element for the element</param>
		public override void Marshal(XmlElement parent)
		{
			base.Marshal(parent);
		}

		/// <summary>
		/// Perform transformation and return a collection of IAttributeSetter instances that set a source
		/// value to destination attribute.
		/// </summary>
		/// <param name="srcDataRow">The DataRow from source</param>
		/// <param name="srcDataView">The DataViewModel for source class. Can be null</param>
		/// <param name="dstDataSet">The destination DataSet</param>
		/// <param name="dstDataView">The DataViewModel for destination class</param>
		/// <param name="rowIndex">The row index of current transformed row.</param>
		/// <param name="assembly">The assembly contains transformer classes</param>
		/// <returns>A AttributeSetterCollection instance.</returns>
		public override AttributeSetterCollection DoTransform(DataRow srcDataRow,
			DataViewModel srcDataView, DataSet dstDataSet,
			DataViewModel dstDataView, int rowIndex, Assembly assembly)
		{
			IAttributeGetter getter = GetterFactory.Instance.Create(this.GetterType,
				srcDataRow, SourceAttributeName, srcDataView);

			string srcValue = getter.GetValue();

			if (_transformScript.Script != null && _transformScript.Enabled)
			{
				if (Transformer == null)
				{
					string typeName = NewteraNameSpace.TRANSFORMER_NAME_SPACE + "." + _transformScript.ClassType;
					Transformer = (ITransformer) assembly.CreateInstance(typeName);
				}

				NameValueCollection inputValues = new NameValueCollection();
				inputValues[SourceAttributeName] = srcValue;
				NameValueCollection outputValues = new NameValueCollection();
				outputValues[DestinationAttributeName] = "";

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

				srcValue = outputValues[0]; // get transformed value
			}

			AttributeSetterCollection setters = new AttributeSetterCollection();

			IAttributeSetter setter = new PrimaryKeySetter(srcValue,
				dstDataSet, dstDataView,
				rowIndex, DestinationAttributeName,
				RelationshipAttributeName);
			
			setters.Add(setter);

			return setters;
		}
	}
}