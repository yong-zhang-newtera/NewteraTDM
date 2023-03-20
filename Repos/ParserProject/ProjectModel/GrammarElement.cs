/*
* @(#)GrammarElement.cs
*
* Copyright (c) 2005 Newtera, Inc. All rights reserved.
*
*/
namespace Newtera.ParserGen.ProjectModel
{
	using System;
	using System.Xml;
	using System.IO;
	using System.Text;
	using System.Collections;
	using System.ComponentModel;


	/// <summary>
	/// Represents a Grammar element in a project model. It defines the information
	/// about the grammar.
	/// </summary>
	/// <version>1.0.1 11 Nov. 2005</version>
	/// <author>Yong Zhang</author>
	public class GrammarElement : ProjectModelElementBase
	{
		private string _grammarFile;
		private string _grammarText;

		/// <summary>
		/// Initiating an instance of GrammarElement class
		/// </summary>
		/// <param name="name">Parser name</param>
		public GrammarElement(string name) : base(name)
		{
			_grammarFile = null;
			_grammarText = null;

			// listen to the value changed event from the data views
			this.ValueChanged += new EventHandler(this.ValueChangedHandler);
		}

		/// <summary>
		/// Initiating an instance of GrammarElement class
		/// </summary>
		/// <param name="xmlElement">The xml element conatins data of the instance</param>
		internal GrammarElement(XmlElement xmlElement) : base()
		{
			Unmarshal(xmlElement);

			this.ValueChanged += new EventHandler(this.ValueChangedHandler);
		}

		/// <summary>
		/// Gets or sets the caption of element
		/// </summary>
		/// <value>The element caption.</value>
		[
		CategoryAttribute("Appearance"),
		DescriptionAttribute("The display name of the item"),
		ReadOnlyAttribute(true)
		]		
		public override string Caption
		{
			get
			{
				return base.Caption;
			}
			set
			{
				base.Caption = value;
			}
		}

		/// <summary>
		/// Gets a file path of the grammar.
		/// </summary>
		/// <value>A file path</value>
		[
		CategoryAttribute("System"),
		DescriptionAttribute("Name of the grammar file."),
		ReadOnlyAttribute(true),
		]		
		public string GrammarFile
		{
			get
			{
				return _grammarFile;
			}
			set
			{
				_grammarFile = value;
			}
		}

		/// <summary>
		/// Gets text of the grammar.
		/// </summary>
		/// <value>A grammar text string.</value>
		[BrowsableAttribute(false)]		
		public string GrammarText
		{
			get
			{
				return _grammarText;
			}
			set
			{
				_grammarText = value;

				// Raise the event for value change
				FireValueChangedEvent(value);
			}
		}

		/// <summary>
		/// Gets the type of element
		/// </summary>
		/// <value>One of ElementType values</value>
		[BrowsableAttribute(false)]	
		public override ElementType ElementType 
		{
			get
			{
				return ElementType.Grammar;
			}
		}

		/// <summary>
		/// Accept a visitor of IProjectModelElementVisitor type to traverse its
		/// elements.
		/// </summary>
		/// <param name="visitor">A visitor</param>
		public override void Accept(IProjectModelElementVisitor visitor)
		{
			visitor.VisitGrammar(this);
		}

		/// <summary>
		/// Unmarshal an element representing parser
		/// </summary>
		/// <param name="parent">An xml element</param>
		public override void Unmarshal(XmlElement parent)
		{
			base.Unmarshal(parent);
			
			// restore the grammar file path
			_grammarFile = parent.GetAttribute("GrammarFile");
			
			// grammar text is saved as text of a subelement
			_grammarText = parent.ChildNodes[0].InnerText;
		}

		/// <summary>
		/// Write values of members to an xml element
		/// </summary>
		/// <param name="parent">An xml element for the element</param>
		public override void Marshal(XmlElement parent)
		{
			base.Marshal(parent);

			// write the _grammarFile element as an attribute
			parent.SetAttribute("GrammarFile", _grammarFile);

			// keep the grammar rules in a subelement
			XmlElement child = parent.OwnerDocument.CreateElement("GrammarText");
			child.InnerText = _grammarText;
			parent.AppendChild(child);
		}
	}
}