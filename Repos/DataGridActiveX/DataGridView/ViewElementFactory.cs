/*
* @(#)ViewElementFactory.cs
*
* Copyright (c) 2006 Newtera, Inc. All rights reserved.
*
*/
namespace Newtera.DataGridActiveX.DataGridView
{
	using System;
	using System.Xml;


	/// <summary>
	/// A singleton class that creates an instance of IDataGridViewElement
	/// based on a xml element
	/// </summary>
	/// <version>  	1.0.0 28 May 2006 </version>
	public class ViewElementFactory
	{		
		/// <summary>
		/// Singleton's private instance.
		/// </summary>
		private static ViewElementFactory theFactory;
		
		static ViewElementFactory()
		{
			theFactory = new ViewElementFactory();
		}

		/// <summary>
		/// The private constructor.
		/// </summary>
		private ViewElementFactory()
		{
		}

		/// <summary>
		/// Gets the ViewElementFactory instance.
		/// </summary>
		/// <returns> The ViewElementFactory instance.</returns>
		static public ViewElementFactory Instance
		{
			get
			{
				return theFactory;
			}
		}
		
		/// <summary>
		/// Creates an instance of IDataGridViewElement type based on the xml element
		/// representing the element.
		/// </summary>
		/// <param name="xmlElement">the xml element.</param>
		/// <returns>A IDataGridViewElement instance</returns>
		public IDataGridViewElement Create(XmlElement xmlElement)
		{
			IDataGridViewElement obj = null;

			if (xmlElement != null)
			{
				string elemntName = xmlElement.Name;

				ViewElementType type = (ViewElementType) Enum.Parse(typeof(ViewElementType), elemntName);

				switch (type)
				{
					case ViewElementType.View:
						obj = new DataGridViewModel(xmlElement);
						break;
					case ViewElementType.Class:
						obj = new ViewClass(xmlElement);
						break;
					case ViewElementType.Filter:
						obj = new ViewFilter(xmlElement);
						break;
					case ViewElementType.SimpleAttribute:
						obj = new ViewSimpleAttribute(xmlElement);
						break;
                    case ViewElementType.ArrayAttribute:
                        obj = new ViewArrayAttribute(xmlElement);
                        break;
                    case ViewElementType.VirtualAttribute:
                        obj = new ViewVirtualAttribute(xmlElement);
                        break;
                    case ViewElementType.RelationshipAttribute:
                        obj = new ViewRelationshipAttribute(xmlElement);
                        break;
					case ViewElementType.ResultAttributes:
						obj = new ViewAttributeCollection(xmlElement);
						break;
                    case ViewElementType.EnumElement:
                        obj = new ViewEnumElement(xmlElement);
                        break;
					case ViewElementType.And:
					case ViewElementType.Or:
						obj = new ViewLogicalExpr(xmlElement);
						break;
					case ViewElementType.Equals:
					case ViewElementType.NotEquals:
					case ViewElementType.LessThan:
					case ViewElementType.GreaterThan:
					case ViewElementType.LessThanEquals:
					case ViewElementType.GreaterThanEquals:
                    case ViewElementType.Like:
                    case ViewElementType.IsNull:
                    case ViewElementType.IsNotNull:
						obj = new ViewRelationalExpr(xmlElement);
						break;
					case ViewElementType.In:
					case ViewElementType.NotIn:
						obj = new ViewInExpr(xmlElement);
						break;
					case ViewElementType.To:
						break;
					case ViewElementType.Parameter:
						obj = new ViewParameter(xmlElement);
						break;
					case ViewElementType.Contains:
						obj = new ViewContainsFunc(xmlElement);
						break;
					case ViewElementType.Parameters:
						obj = new ViewParameterCollection(xmlElement);
						break;
					case ViewElementType.SortBy:
						obj = new ViewSortBy(xmlElement);
						break;
                    case ViewElementType.EnumValues:
                        obj = new ViewEnumValueCollection(xmlElement);
                        break;
                    case ViewElementType.EnumValue:
                        obj = new ViewEnumValue(xmlElement);
                        break;
                    case ViewElementType.LocaleInfo:
                        obj = new ViewLocaleInfo(xmlElement);
                        break;
				}
			}

			return obj;
		}

		/// <summary>
		/// Convert a ViewElementType value to a string
		/// </summary>
		/// <param name="type">One of ViewElementType values</param>
		/// <returns>The corresponding string</returns>
		internal static string ConvertTypeToString(ViewElementType type)
		{
			string str = Enum.GetName(typeof(ViewElementType), type);

			return str;
		}
	}
}