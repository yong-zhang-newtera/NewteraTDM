/*
* @(#)DefaultValueCollection.cs
*
* Copyright (c) 2003-2004 Newtera, Inc. All rights reserved.
*
*/
namespace Newtera.Common.MetaData.Mappings
{
	using System;
	using System.Text;
	using System.Xml;

	/// <summary>
	/// Represents a collection of DefaultValue instances.
	/// </summary>
	/// <version>1.0.0 28 Sep 2004</version>
	/// <author>Yong Zhang</author>
	public class DefaultValueCollection : MappingNodeCollection
	{
		/// <summary>
		/// Initiating an instance of DefaultValueCollection class
		/// </summary>
		public DefaultValueCollection() : base()
		{
		}
		
		/// <summary>
		/// Initiating an instance of DefaultValueCollection class
		/// </summary>
		/// <param name="xmlElement">The xml element conatins data of the instance</param>
		internal DefaultValueCollection(XmlElement xmlElement) : base(xmlElement)
		{
		}

		/// <summary>
		/// Gets the type of node
		/// </summary>
		/// <value>One of NodeType values</value>
		public override NodeType NodeType
		{
			get
			{
				return NodeType.DefaultValueCollection;
			}
		}

        /// <summary>
        /// Clone the collection
        /// </summary>
        /// <returns></returns>
        public DefaultValueCollection Clone()
        {
            DefaultValueCollection defaultValues = new DefaultValueCollection();
            DefaultValue defaultValue;
            foreach (DefaultValue df in List)
            {
                defaultValue = new DefaultValue(df.DestinationAttributeName, df.Value);
                defaultValue.DestinationClassName = df.DestinationClassName;
                defaultValues.Add(defaultValue);
            }

            return defaultValues;
        }
	}
}