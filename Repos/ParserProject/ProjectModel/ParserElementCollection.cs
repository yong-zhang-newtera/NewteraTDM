/*
* @(#)ParserElementCollection.cs
*
* Copyright (c) 2003 Newtera, Inc. All rights reserved.
*
*/
namespace Newtera.ParserGen.ProjectModel
{
	using System;
	using System.Text;
	using System.Xml;

	/// <summary>
	/// Represents a collection of ParserElement objects.
	/// </summary>
	/// <version>1.0.1 23 Feb 2004</version>
	/// <author>Yong Zhang</author>
	public class ParserElementCollection : ProjectModelElementCollection
	{
		/// <summary>
		/// Initiating an instance of ParserElementCollection class
		/// </summary>
		public ParserElementCollection() : base()
		{
		}
		
		/// <summary>
		/// Initiating an instance of ParserElementCollection class
		/// </summary>
		/// <param name="xmlElement">The xml element conatins data of the instance</param>
		internal ParserElementCollection(XmlElement xmlElement) : base(xmlElement)
		{
		}

		/// <summary>
		/// Gets the type of element
		/// </summary>
		/// <value>One of ElementType values</value>
		public override ElementType ElementType {
			get
			{
				return ElementType.ParserCollection;
			}
		}
	}
}