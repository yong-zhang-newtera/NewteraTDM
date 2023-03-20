/*
* @(#)SampleElement.cs
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
	/// Represents a Sample element in a project model. It defines the information
	/// about the sample file.
	/// </summary>
	/// <version>1.0.1 11 Nov. 2005</version>
	/// <author>Yong Zhang</author>
	public class SampleElement : ProjectModelElementBase
	{
		private string _sampleFilePath; // run-time use
		private bool _isParsed; // run-time use

		/// <summary>
		/// Initiating an instance of SampleElement class
		/// </summary>
		/// <param name="name">Parser name</param>
		public SampleElement(string name) : base(name)
		{
			_sampleFilePath = null;
			_isParsed = false;

			// listen to the value changed event from the data views
			this.ValueChanged += new EventHandler(this.ValueChangedHandler);
		}

		/// <summary>
		/// Initiating an instance of SampleElement class
		/// </summary>
		/// <param name="xmlElement">The xml element conatins data of the instance</param>
		internal SampleElement(XmlElement xmlElement) : base()
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
		/// Gets a path of the sample file.
		/// </summary>
		/// <value>A file path</value>
		[
		CategoryAttribute("System"),
		DescriptionAttribute("Path of the sample file."),
		ReadOnlyAttribute(true),
		]		
		public string SampleFilePath
		{
			get
			{
				if (_sampleFilePath == null)
				{
					_sampleFilePath = ((ParserElement) this.ParentElement).DirectoryPath + @"\" + this.Name;
				}

				return _sampleFilePath;
			}
		}

		/// <summary>
		/// Gets a path of the sample file.
		/// </summary>
		/// <value>A file path</value>
		[
		CategoryAttribute("System"),
		DescriptionAttribute("Whether the sample has been parsed successfully."),
		ReadOnlyAttribute(true),
		]		
		public bool IsParsed
		{
			get
			{
				return _isParsed;
			}
			set
			{
				_isParsed = value;
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
				return ElementType.Sample;
			}
		}

		/// <summary>
		/// Accept a visitor of IProjectModelElementVisitor type to traverse its
		/// elements.
		/// </summary>
		/// <param name="visitor">A visitor</param>
		public override void Accept(IProjectModelElementVisitor visitor)
		{
			visitor.VisitSample(this);
		}

		/// <summary>
		/// Unmarshal an element representing parser
		/// </summary>
		/// <param name="parent">An xml element</param>
		public override void Unmarshal(XmlElement parent)
		{
			base.Unmarshal(parent);
		}

		/// <summary>
		/// Write values of members to an xml element
		/// </summary>
		/// <param name="parent">An xml element for the element</param>
		public override void Marshal(XmlElement parent)
		{
			base.Marshal(parent);
		}
	}
}