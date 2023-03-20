/*
* @(#)ElementFactory.cs
*
* Copyright (c) 2004 Newtera, Inc. All rights reserved.
*
*/
namespace Newtera.ParserGen.ProjectModel
{
	using System;
	using System.Xml;

	/// <summary>
	/// A singleton class that creates an instance of IProjectModelElement
	/// based on a xml element
	/// </summary>
	/// <version>  	1.0.0 11 Nov 2005 </version>
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
		/// Creates an instance of IProjectModelElement type based on the xml element
		/// representing the element.
		/// </summary>
		/// <param name="xmlElement">the xml element.</param>
		/// <returns>A IProjectModelElement instance</returns>
		public IProjectModelElement Create(XmlElement xmlElement)
		{
			IProjectModelElement obj = null;

			if (xmlElement != null)
			{
				string elemntName = xmlElement.Name;

				ElementType type = ConvertStringToType(elemntName);

				switch (type)
				{
					case ElementType.Project:
						obj = new ProjectElement(xmlElement);
						break;
					case ElementType.Parser:
						obj = new ParserElement(xmlElement);
						break;
					case ElementType.Grammar:
						obj = new GrammarElement(xmlElement);
						break;
					case ElementType.Sample:
						obj = new SampleElement(xmlElement);
						break;
					case ElementType.ParserCollection:
						obj = new ParserElementCollection(xmlElement);
						break;
					case ElementType.SampleCollection:
						obj = new SampleElementCollection(xmlElement);
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
		internal static string ConvertTypeToString(ElementType type)
		{
			return Enum.GetName(typeof(ElementType), type);
		}

		/// <summary>
		/// Convert a type string to a ElementType value
		/// </summary>
		/// <param name="str">A type string</param>
		/// <returns>One of ElementType values</returns>
		internal static ElementType ConvertStringToType(string str)
		{
			ElementType type;

			try
			{
				type = (ElementType) Enum.Parse(typeof(ElementType), str);
			}
			catch (Exception)
			{
				type = ElementType.Unknown;
			}

			return type;
		}
	}
}