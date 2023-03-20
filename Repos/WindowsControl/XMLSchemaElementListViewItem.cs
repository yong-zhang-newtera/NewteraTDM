/*
* @(#)XMLSchemaElementListViewItem.cs
*
* Copyright (c) 2003 Newtera, Inc. All rights reserved.
*
*/
namespace Newtera.WindowsControl
{
	using System;
	using System.Windows.Forms;

	using Newtera.Common.MetaData.Schema;
	using Newtera.Common.MetaData.XMLSchemaView;
	
	/// <summary>
	/// Represents a ListView item for a XMLSchemaElement object in XMLSchemaModel
	/// </summary>
	/// <version>  1.0.0 14 Nov 2014</version>
	public class XMLSchemaElementListViewItem : ListViewItem
	{
		private XMLSchemaElement _xmlSchemaElement;

		/// <summary>
		/// Initializes a new instance of the XMLSchemaElementListViewItem class.
		/// </summary>
        /// <param name="xmlSchemaElement">The XMLSchemaElement object</param>
        public XMLSchemaElementListViewItem(string text, XMLSchemaElement xmlSchemaElement)
            : base(text)
		{
            _xmlSchemaElement = xmlSchemaElement;
		}

		/// <summary> 
		/// Gets the xml schema element.
		/// </summary>
        /// <value>A XMLSchemaElement</value>
        public XMLSchemaElement TheElememnt
		{
			get
			{
				return _xmlSchemaElement;
			}
		}
	}
}