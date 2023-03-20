/*
* @(#)ParserElement.cs
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

	using Newtera.ParserGen.ParseTree;

	/// <summary>
	/// Represents a parser element in a project model. It defines the information
	/// about the parser and contains a Grammar element and a set of sample files.
	/// </summary>
	/// <version>1.0.1 11 Nov. 2005</version>
	/// <author>Yong Zhang</author>
	public class ParserElement : ProjectModelElementBase
	{
		private const string GRAMMAR_FILE_SUFFIX = ".g";
		private const string GDL_SUFFIX = ".GDL";

		private GrammarElement _grammar;
		private SampleElementCollection _samples;
		private bool _isBuilt;
		private DataGridSettings _settings = null;
		private string _directory = null; // run-time

		private Hashtable _nonterminals = null; // run-time
		private Hashtable _predefines = null;  // run-time

		/// <summary>
		/// Initiating an instance of ParserElement class
		/// </summary>
		/// <param name="name">Parser name</param>
		public ParserElement(string name) : base(name)
		{
			_grammar = new GrammarElement(name.Trim() + ParserElement.GDL_SUFFIX);
			_grammar.GrammarFile = name.Trim() + ParserElement.GRAMMAR_FILE_SUFFIX;
			_grammar.ParentElement = this;
			_samples = new SampleElementCollection();
			_samples.ParentElement = this;

			_isBuilt = false; // runtime use, do not save

			// listen to the value changed event from the ParserElement
			this.ValueChanged += new EventHandler(this.ValueChangedHandler);
			_samples.ValueChanged += new EventHandler(ValueChangedHandler);
		}

		/// <summary>
		/// Initiating an instance of ParserElement class
		/// </summary>
		/// <param name="xmlElement">The xml element conatins data of the instance</param>
		internal ParserElement(XmlElement xmlElement) : base()
		{
			Unmarshal(xmlElement);

			this.ValueChanged += new EventHandler(this.ValueChangedHandler);
			_samples.ValueChanged += new EventHandler(ValueChangedHandler);
		}

		/// <summary>
		/// Gets or sets the information indicating whether the parser has been
		/// built successfully.
		/// </summary>
		/// <value>true if it has been built successfully, false otherwise.</value>
		[
		CategoryAttribute("System"),
		DescriptionAttribute("Whether the parser has been built."),
		ReadOnlyAttribute(true),
		]
		public bool IsBuilt
		{
			get
			{
				return _isBuilt;
			}
			set
			{
				_isBuilt = value;
			}
		}

		/// <summary>
		/// Gets or sets the information indicating whether the parser has been
		/// built successfully.
		/// </summary>
		/// <value>true if it has been built successfully, false otherwise.</value>
		[
		CategoryAttribute("System"),
		DescriptionAttribute("Parser directory path."),
		ReadOnlyAttribute(true),
		]
		public string DirectoryPath
		{
			get
			{
				if (this._directory == null)
				{
					_directory = ((ProjectElement) this.ParentElement).ProjectFolder + this.Name;
				}

				return _directory;
			}
		}

		/// <summary>
		/// Gets the grammar element of the parser
		/// </summary>
		[BrowsableAttribute(false)]	
		public GrammarElement Grammar
		{
			get
			{
				return this._grammar;
			}
		}

		/// <summary>
		/// Gets a list of sample elements for a parser.
		/// </summary>
		/// <value>A SampleElementCollection instance.</value>
		[BrowsableAttribute(false)]	
		public SampleElementCollection Samples
		{
			get
			{
				return _samples;
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
				return ElementType.Parser;
			}
		}

		/// <summary>
		/// Gets or sets a table of nonterminal symbols
		/// </summary>
		[BrowsableAttribute(false)]	
		public Hashtable Nonterminals
		{
			get
			{
				return this._nonterminals;
			}
			set
			{
				_nonterminals = value;
			}
		}

		/// <summary>
		/// Gets or sets a table of predefined symbols
		/// </summary>
		[BrowsableAttribute(false)]	
		public Hashtable Predefines
		{
			get
			{
				return this._predefines;
			}
			set
			{
				_predefines = value;
			}
		}

		/// <summary>
		/// Reset the value change status in the project model
		/// </summary>
		public override void Reset()
		{
			this.Grammar.IsValueChanged = false;

			foreach (SampleElement sample in this.Samples)
			{
				sample.IsValueChanged = false;
			}
		}

		/// <summary>
		/// Gets or sets the settings for transforming a parse tree to
		/// a Data Grid
		/// </summary>
		[BrowsableAttribute(false)]
		public DataGridSettings Settings
		{
			get
			{
				if (_settings == null)
				{
					// read the settings from the file first
					string filePath = this.DirectoryPath + @"\" + this.Name + ".xml";
				
					_settings = DataGridSettings.Read(filePath);

					if (_settings == null)
					{
						// the settings has not been saved
						// create a new one
						_settings = new DataGridSettings();
						_settings.ParserName = this.Name;
					}
				}

				return _settings;
			}
			set
			{
				_settings = value;
				this.FireValueChangedEvent(value);
			}
		}

		/// <summary>
		/// Gets the class name of the converter
		/// </summary>
		[
		CategoryAttribute("System"),
		DescriptionAttribute("Name of the converter for the parser."),
		ReadOnlyAttribute(true),
		]
		public string ConverterClass
		{
			get
			{
				return "Newtera.TextParser." + this.Name + "Converter";
			}
		}

		/// <summary>
		/// Save the data grid settings in an XML file with parser name.
		/// </summary>
		public void SaveSettings()
		{
			if (_settings != null)
			{
				// write the settings from to a file
				string filePath = this.DirectoryPath + @"\" + this.Name + ".xml";
				
				DataGridSettings.Write(filePath, _settings);
			}
		}

		/// <summary>
		/// Accept a visitor of IProjectModelElementVisitor type to traverse its
		/// elements.
		/// </summary>
		/// <param name="visitor">A visitor</param>
		public override void Accept(IProjectModelElementVisitor visitor)
		{
			visitor.VisitParser(this);

			_grammar.Accept(visitor);

			Samples.Accept(visitor);
		}

		/// <summary>
		/// Unmarshal an element representing parser
		/// </summary>
		/// <param name="parent">An xml element</param>
		public override void Unmarshal(XmlElement parent)
		{
			base.Unmarshal(parent);

			// restore the Grammar element
			_grammar = (GrammarElement) ElementFactory.Instance.Create((XmlElement) parent.ChildNodes[0]);
			_grammar.ParentElement = this;

			// restore the sample collection element which is the second child.
			_samples = (SampleElementCollection) ElementFactory.Instance.Create((XmlElement) parent.ChildNodes[1]);
			_samples.ParentElement = this;
		}

		/// <summary>
		/// Write values of members to an xml element
		/// </summary>
		/// <param name="parent">An xml element for the element</param>
		public override void Marshal(XmlElement parent)
		{
			base.Marshal(parent);

			// write the _grammar element as the first xml child
			XmlElement child = parent.OwnerDocument.CreateElement(ElementFactory.ConvertTypeToString(_grammar.ElementType));
			_grammar.Marshal(child);
			parent.AppendChild(child);

			// write the Samples collection
			child = parent.OwnerDocument.CreateElement(ElementFactory.ConvertTypeToString(Samples.ElementType));
			Samples.Marshal(child);
			parent.AppendChild(child);
		}
	}
}