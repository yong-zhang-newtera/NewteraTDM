/*
* @(#)ElementFactory.cs
*
* Copyright (c) 2003 Newtera, Inc. All rights reserved.
*
*/
namespace Newtera.Common.MetaData.DataView
{
	using System;
	using System.Xml;

	using Newtera.Common.MetaData.DataView.Taxonomy;

	/// <summary>
	/// A singleton class that creates an instance of IDataViewElement
	/// based on a xml element
	/// </summary>
	/// <version>  	1.0.0 28 Oct 2003 </version>
	/// <author> Yong Zhang </author>
	public class ElementFactory
	{		
		/// <summary>
		/// Singleton's private instance.
		/// </summary>
		private static ElementFactory theFactory;
		
		static ElementFactory()
		{
			theFactory = new ElementFactory();
		}

		/// <summary>
		/// The private constructor.
		/// </summary>
		private ElementFactory()
		{
		}

		/// <summary>
		/// Gets the ElementFactory instance.
		/// </summary>
		/// <returns> The ElementFactory instance.</returns>
		static public ElementFactory Instance
		{
			get
			{
				return theFactory;
			}
		}
		
		/// <summary>
		/// Creates an instance of IDataViewElement type based on the xml element
		/// representing the element.
		/// </summary>
		/// <param name="xmlElement">the xml element.</param>
		/// <returns>A IDataViewElement instance</returns>
		public IDataViewElement Create(XmlElement xmlElement)
		{
			IDataViewElement obj = null;

			if (xmlElement != null)
			{
				string elemntName = xmlElement.Name;

				ElementType type = ConvertStringToType(elemntName);

				switch (type)
				{
					case ElementType.View:
						obj = new DataViewModel(xmlElement);
						break;
					case ElementType.Class:
						obj = new DataClass(xmlElement);
						break;
					case ElementType.Filter:
						obj = new Filter(xmlElement);
						break;
					case ElementType.SimpleAttribute:
						obj = new DataSimpleAttribute(xmlElement);
						break;
					case ElementType.ArrayAttribute:
						obj = new DataArrayAttribute(xmlElement);
						break;
                    case ElementType.VirtualAttribute:
                        obj = new DataVirtualAttribute(xmlElement);
                        break;
                    case ElementType.ImageAttribute:
                        obj = new DataImageAttribute(xmlElement);
                        break;
					case ElementType.RelationshipAttribute:
						obj = new DataRelationshipAttribute(xmlElement);
						break;
					case ElementType.ReferencedClasses:
						obj = new ReferencedClassCollection(xmlElement);
						break;
					case ElementType.ResultAttributes:
						obj = new ResultAttributeCollection(xmlElement);
						break;
                    case ElementType.ParenthesizedExpr:
                        obj = new ParenthesizedExpr(xmlElement);
                        break;
					case ElementType.And:
					case ElementType.Or:
						obj = new LogicalExpr(xmlElement);
						break;
					case ElementType.Equals:
					case ElementType.NotEquals:
					case ElementType.LessThan:
					case ElementType.GreaterThan:
					case ElementType.LessThanEquals:
					case ElementType.GreaterThanEquals:
                    case ElementType.Like:
                    case ElementType.IsNull:
                    case ElementType.IsNotNull:
						obj = new RelationalExpr(xmlElement);
						break;
					case ElementType.In:
					case ElementType.NotIn:
						obj = new InExpr(xmlElement);
						break;
					case ElementType.To:
						break;
					case ElementType.Parameter:
						obj = new Parameter(xmlElement);
						break;
					case ElementType.Contains:
						obj = new ContainsFunc(xmlElement);
						break;
					case ElementType.Parameters:
						obj = new ParameterCollection(xmlElement);
						break;
					case ElementType.Taxonomy:
						obj = new TaxonomyModel(xmlElement);
						break;
					case ElementType.TaxonNode:
						obj = new TaxonNode(xmlElement);
						break;
					case ElementType.TaxonNodes:
						obj = new TaxonNodeCollection(xmlElement);
						break;
                    case ElementType.AutoClassifyDef:
                        obj = new AutoClassifyDef(xmlElement);
                        break;
                    case ElementType.AutoClassifyLevel:
                        obj = new AutoClassifyLevel(xmlElement);
                        break;
                    case ElementType.AutoClassifyLevels:
                        obj = new AutoClassifyLevelCollection(xmlElement);
                        break;
					case ElementType.SortBy:
						obj = new SortBy(xmlElement);
						break;
                    case ElementType.SortAttribute:
                        obj = new SortAttribute(xmlElement);
                        break;
                    case ElementType.WFState:
                        obj = new WFStateFunction(xmlElement);
                        break;
                    case ElementType.Before:
                        obj = new BeforeValueFunction(xmlElement);
                        break;
                    case ElementType.Error:
                        obj = new ErrorFunction(xmlElement);
                        break;
				}
			}

			return obj;
		}

		/// <summary>
		/// Convert a ElementType value to a string
		/// </summary>
		/// <param name="type">One of ElementType values</param>
		/// <returns>The corresponding string</returns>
		public static string ConvertTypeToString(ElementType type)
		{
			string str = "Unknown";

			switch (type)
			{
				case ElementType.DataViews:
					str = "DataViews";
					break;
				case ElementType.View:
					str = "View";
					break;
				case ElementType.Class:
					str = "Class";
					break;
				case ElementType.Filter:
					str = "Filter";
					break;
				case ElementType.SimpleAttribute:
					str = "SimpleAttribute";
					break;
				case ElementType.ArrayAttribute:
					str = "ArrayAttribute";
					break;
                case ElementType.VirtualAttribute:
                    str = "VirtualAttribute";
                    break;
                case ElementType.ImageAttribute:
                    str = "ImageAttribute";
                    break;
				case ElementType.RelationshipAttribute:
					str = "RelationshipAttribute";
					break;
				case ElementType.And:
					str = "And";
					break;
				case ElementType.Or:
					str = "Or";
					break;
				case ElementType.Equals:
					str = "Equals";
					break;
				case ElementType.NotEquals:
					str = "NotEquals";
					break;
				case ElementType.LessThan:
					str = "LessThan";
					break;
				case ElementType.GreaterThan:
					str = "GreaterThan";
					break;
				case ElementType.LessThanEquals:
					str = "LessThanEquals";
					break;
				case ElementType.GreaterThanEquals:
					str = "GreaterThanEquals";
					break;
                case ElementType.Like:
                    str = "Like";
                    break;
                case ElementType.IsNull:
                    str = "IsNull";
                    break;
                case ElementType.IsNotNull:
                    str = "IsNotNull";
                    break;
				case ElementType.In:
					str = "In";
					break;
				case ElementType.NotIn:
					str = "NotIn";
					break;
				case ElementType.To:
					str = "To";
					break;
				case ElementType.Parameter:
					str = "Parameter";
					break;
				case ElementType.Contains:
					str = "Contains";
					break;
				case ElementType.Parameters:
					str = "Parameters";
					break;
				case ElementType.ParenthesizedExpr:
					str = "Parenthesis";
					break;
				case ElementType.ReferencedClasses:
					str = "ReferencedClasses";
					break;
				case ElementType.ResultAttributes:
					str = "ResultAttributes";
					break;
				case ElementType.Taxonomy:
					str = "Taxonomy";
					break;
				case ElementType.Taxonomies:
					str = "Taxonomies";
					break;
				case ElementType.TaxonNode:
					str = "TaxonNode";
					break;
				case ElementType.TaxonNodes:
					str = "TaxonNodes";
					break;
                case ElementType.AutoClassifyDef:
                    str = "AutoClassifyDef";
                    break;
                case ElementType.AutoClassifyLevel:
                    str = "AutoClassifyLevel";
                    break;
                case ElementType.AutoClassifyLevels:
                    str = "AutoClassifyLevels";
                    break;
				case ElementType.SortBy:
					str = "SortBy";
					break;
                case ElementType.SortAttribute:
                    str = "SortAttribute";
                    break;
                case ElementType.WFState:
                    str = "WFState";
                    break;
                case ElementType.Before:
                    str = "Before";
                    break;
                case ElementType.Error:
                    str = "Error";
                    break;
			}

			return str;
		}

		/// <summary>
		/// Convert a type string to a ElementType value
		/// </summary>
		/// <param name="str">A type string</param>
		/// <returns>One of ElementType values</returns>
		public static ElementType ConvertStringToType(string str)
		{
			ElementType type = ElementType.Unknown;

			switch (str)
			{
				case "DataViews":
					type = ElementType.DataViews;
					break;
				case "View":
					type = ElementType.View;
					break;
				case "Class":
					type = ElementType.Class;
					break;
				case "Filter":
					type = ElementType.Filter;
					break;
				case "SimpleAttribute":
					type = ElementType.SimpleAttribute;
					break;
				case "ArrayAttribute":
					type = ElementType.ArrayAttribute;
					break;
                case "VirtualAttribute":
                    type = ElementType.VirtualAttribute;
                    break;
                case "ImageAttribute":
                    type = ElementType.ImageAttribute;
                    break;
				case "RelationshipAttribute":
					type = ElementType.RelationshipAttribute;
					break;
				case "And":
					type = ElementType.And;
					break;
				case "Or":
					type = ElementType.Or;
					break;
				case "Equals":
					type = ElementType.Equals;
					break;
				case "NotEquals":
					type = ElementType.NotEquals;
					break;
				case "LessThan":
					type = ElementType.LessThan;
					break;
				case "GreaterThan":
					type = ElementType.GreaterThan;
					break;
				case "LessThanEquals":
					type = ElementType.LessThanEquals;
					break;
				case "GreaterThanEquals":
					type = ElementType.GreaterThanEquals;
					break;
                case "Like":
                    type = ElementType.Like;
                    break;
                case "IsNull":
                    type = ElementType.IsNull;
                    break;
                case "IsNotNull":
                    type = ElementType.IsNotNull;
                    break;
				case "In":
					type = ElementType.In;
					break;
				case "NotIn":
					type = ElementType.NotIn;
					break;
				case "To":
					type = ElementType.To;
					break;
				case "Parameter":
					type = ElementType.Parameter;
					break;
				case "Contains":
					type = ElementType.Contains;
					break;
				case "Parameters":
					type = ElementType.Parameters;
					break;
				case "Parenthesis":
					type = ElementType.ParenthesizedExpr;
					break;
				case "ReferencedClasses":
					type = ElementType.ReferencedClasses;
					break;
				case "ResultAttributes":
					type = ElementType.ResultAttributes;
					break;
				case "Taxonomy":
					type = ElementType.Taxonomy;
					break;
				case "Taxonomies":
					type = ElementType.Taxonomies;
					break;
				case "TaxonNode":
					type = ElementType.TaxonNode;
					break;
				case "TaxonNodes":
					type = ElementType.TaxonNodes;
					break;
                case "AutoClassifyDef":
                    type = ElementType.AutoClassifyDef;
                    break;
                case "AutoClassifyLevel":
                    type = ElementType.AutoClassifyLevel;
                    break;
                case "AutoClassifyLevels":
                    type = ElementType.AutoClassifyLevels;
                    break;
				case "SortBy":
					type = ElementType.SortBy;
					break;
                case "SortAttribute":
                    type = ElementType.SortAttribute;
                    break;
                case "WFState":
                    type = ElementType.WFState;
                    break;
                case "Before":
                    type = ElementType.Before;
                    break;
                case "Error":
                    type = ElementType.Error;
                    break;
			}

			return type;
		}
	}
}