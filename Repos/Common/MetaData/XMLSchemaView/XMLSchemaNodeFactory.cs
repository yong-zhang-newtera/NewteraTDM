/*
* @(#)XMLSchemaNodeFactory.cs
*
* Copyright (c) 2014 Newtera, Inc. All rights reserved.
*
*/
namespace Newtera.Common.MetaData.XMLSchemaView
{
	using System;
	using System.Xml;

	/// <summary>
	/// A singleton class that creates an instance of IXMLSchemaNode
	/// based on a xml element
	/// </summary>
    /// <version>  	1.0.0 10 Aug 2014</version>
	public class XMLSchemaNodeFactory
	{		
		/// <summary>
		/// Singleton's private instance.
		/// </summary>
		private static XMLSchemaNodeFactory theFactory;
		
		static XMLSchemaNodeFactory()
		{
			theFactory = new XMLSchemaNodeFactory();
		}

		/// <summary>
		/// The private constructor.
		/// </summary>
		private XMLSchemaNodeFactory()
		{
		}

		/// <summary>
		/// Gets the XMLSchemaNodeFactory instance.
		/// </summary>
		/// <returns> The XMLSchemaNodeFactory instance.</returns>
		static public XMLSchemaNodeFactory Instance
		{
			get
			{
				return theFactory;
			}
		}
		
		/// <summary>
		/// Creates an instance of IXMLSchemaNode type based on the xml element
		/// representing the element.
		/// </summary>
		/// <param name="xmlElement">the xml element.</param>
		/// <returns>A IXMLSchemaNode instance</returns>
		public IXMLSchemaNode Create(XmlElement xmlElement)
		{
			IXMLSchemaNode obj = null;

			if (xmlElement != null)
			{
				string elemntName = xmlElement.Name;

				XMLSchemaNodeType type = ConvertStringToType(elemntName);

				switch (type)
				{
					case XMLSchemaNodeType.XMLSchemaView:
						obj = new XMLSchemaModel(xmlElement);
						break;

                    case XMLSchemaNodeType.XMLSchemaComplexTypes:
                        obj = new XMLSchemaComplexTypeCollection(xmlElement);
                        break;

                    case XMLSchemaNodeType.XMLSchemaComplexType:
                        obj = new XMLSchemaComplexType(xmlElement);
                        break;

                    case XMLSchemaNodeType.XMLSchemaElements:
                        obj = new XMLSchemaElementCollection(xmlElement);
                        break;

                    case XMLSchemaNodeType.XMLSchemaElement:
                        obj = new XMLSchemaElement(xmlElement);
                        break;
				}
			}

			return obj;
		}

		/// <summary>
		/// Convert a XMLSchemaNodeType value to a string
		/// </summary>
		/// <param name="type">One of XMLSchemaNodeType values</param>
		/// <returns>The corresponding string</returns>
		public static string ConvertTypeToString(XMLSchemaNodeType type)
		{
			string str = "Unknown";

			switch (type)
			{
				case XMLSchemaNodeType.XMLSchemaViews:
                    str = "XMLSchemaViews";
					break;

				case XMLSchemaNodeType.XMLSchemaView:
                    str = "XMLSchemaView";
					break;

                case XMLSchemaNodeType.XMLSchemaComplexTypes:
                    str = "XMLSchemaComplexTypes";
                    break;

                case XMLSchemaNodeType.XMLSchemaComplexType:
                    str = "XMLSchemaComplexType";
                    break;

                case XMLSchemaNodeType.XMLSchemaElements:
                    str = "XMLSchemaElements";
                    break;

                case XMLSchemaNodeType.XMLSchemaElement:
                    str = "XMLSchemaElement";
                    break;
			}

			return str;
		}

		/// <summary>
		/// Convert a type string to a XMLSchemaNodeType value
		/// </summary>
		/// <param name="str">A type string</param>
		/// <returns>One of XMLSchemaNodeType values</returns>
		public static XMLSchemaNodeType ConvertStringToType(string str)
		{
			XMLSchemaNodeType type = XMLSchemaNodeType.Unknown;

			switch (str)
			{
                case "XMLSchemaViews":
					type = XMLSchemaNodeType.XMLSchemaViews;
					break;

                case "XMLSchemaView":
					type = XMLSchemaNodeType.XMLSchemaView;
					break;

                case "XMLSchemaComplexTypes":
                    type = XMLSchemaNodeType.XMLSchemaComplexTypes;
                    break;

                case "XMLSchemaComplexType":
                    type = XMLSchemaNodeType.XMLSchemaComplexType;
                    break;

                case "XMLSchemaElements":
                    type = XMLSchemaNodeType.XMLSchemaElements;
                    break;

                case "XMLSchemaElement":
                    type = XMLSchemaNodeType.XMLSchemaElement;
                    break;
			}

			return type;
		}
	}
}