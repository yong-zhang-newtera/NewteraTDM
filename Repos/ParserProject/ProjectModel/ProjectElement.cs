/*
* @(#)ProjectElement.cs
*
* Copyright (c) 2003 Newtera, Inc. All rights reserved.
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
	/// Represents the root element of a project model. It defines the information
	/// about the parser project and contains sub elements.
	/// </summary>
	/// <version>1.0.1 11 Nov. 2005</version>
	/// <author>Yong Zhang</author>
	public class ProjectElement : ProjectModelElementBase
	{
		private string _projectDir;
		private ParserElementCollection _parsers;

		/// <summary>
		/// Initiating an instance of ProjectElement class
		/// </summary>
		/// <param name="name">Project name.</param>
		public ProjectElement() : base()
		{
			_parsers = new ParserElementCollection();
			_parsers.ParentElement = this;

			// listen to the value changed event from the project
			this.ValueChanged += new EventHandler(this.ValueChangedHandler);
			_parsers.ValueChanged += new EventHandler(ValueChangedHandler);
		}

		/// <summary>
		/// Initiating an instance of ProjectElement class
		/// </summary>
		/// <param name="name">Project name.</param>
		public ProjectElement(string name) : base(name)
		{
			_parsers = new ParserElementCollection();
			_parsers.ParentElement = this;

			// listen to the value changed event from the project
			this.ValueChanged += new EventHandler(this.ValueChangedHandler);
			_parsers.ValueChanged += new EventHandler(ValueChangedHandler);
		}

		/// <summary>
		/// Initiating an instance of ProjectElement class
		/// </summary>
		/// <param name="xmlElement">The xml element conatins data of the instance</param>
		internal ProjectElement(XmlElement xmlElement) : base()
		{
			Unmarshal(xmlElement);

			this.ValueChanged += new EventHandler(this.ValueChangedHandler);
			_parsers.ValueChanged += new EventHandler(ValueChangedHandler);
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
		/// Gets or sets the project folder path.
		/// </summary>
		[
			CategoryAttribute("System"),
			DescriptionAttribute("The project directory."),
			ReadOnlyAttribute(true)
		]
		public string ProjectFolder
		{
			get
			{
				return _projectDir;
			}
			set
			{
				if (!value.Trim().EndsWith(@"\"))
				{
					_projectDir = value + @"\";
				}
				else
				{
					_projectDir = value;
				}
			}
		}

		/// <summary>
		/// Gets a list of parser elements in the project
		/// </summary>
		/// <value>A ParserElementCollection.</value>
		[BrowsableAttribute(false)]	
		public ParserElementCollection Parsers
		{
			get
			{
				return _parsers;
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
				return ElementType.Project;
			}
		}

		/// <summary>
		/// Reset the value change status in the project model
		/// </summary>
		public override void Reset()
		{
			foreach (ParserElement parser in this.Parsers)
			{
				parser.IsValueChanged = false;
			}
		}

		/// <summary>
		/// Accept a visitor of IProjectModelElementVisitor type to traverse its
		/// elements.
		/// </summary>
		/// <param name="visitor">A visitor</param>
		public override void Accept(IProjectModelElementVisitor visitor)
		{
			visitor.VisitProject(this);

			Parsers.Accept(visitor);
		}

		/// <summary>
		/// Constrauct a ProjectModel collection from an XML file.
		/// </summary>
		/// <param name="fileName">the name of the XML file</param>
		/// <exception cref="ProjectModelException">ProjectModelException is thrown when it fails to
		/// read the XML file
		/// </exception>
		public void Read(string fileName)
		{
			try
			{
				//Open the stream and read model from it.
				using (FileStream fs = File.OpenRead(fileName)) 
				{
					Read(fs);					
				}
			}
			catch (Exception ex)
			{
				throw new ProjectModelException("Failed to read the file :" + fileName, ex);
			}
		}
		
		/// <summary>
		/// Constrauct a ProjectModel from an stream.
		/// </summary>
		/// <param name="stream">the stream</param>
		/// <exception cref="ProjectModelException">ProjectModelException is thrown when it fails to
		/// read the stream
		/// </exception>
		public void Read(Stream stream)
		{
			if (stream != null)
			{
				try
				{
					XmlDocument doc = new XmlDocument();

					doc.Load(stream);
				
					// Initializing the objects from the xml document
					Unmarshal(doc.DocumentElement);
				}
				catch (Exception e)
				{
					throw new ProjectModelException(e.Message, e);
				}
			}
		}

		/// <summary>
		/// Constrauct a ProjectModel from a text reader.
		/// </summary>
		/// <param name="reader">the text reader</param>
		/// <exception cref="ProjectModelException">ProjectModelException is thrown when it fails to
		/// read the text reader
		/// </exception>
		public void Read(TextReader reader)
		{
			if (reader != null)
			{
				try
				{
					XmlDocument doc = new XmlDocument();

					doc.Load(reader);
				
					// Initializing the objects from the xml document
					Unmarshal(doc.DocumentElement);
				}
				catch (Exception e)
				{
					throw new ProjectModelException(e.Message, e);
				}
			}
		}

		/// <summary>
		/// Write the ProjectModel to an XML file.
		/// </summary>
		/// <param name="fileName">The output file name.</param>
		/// <exception cref="ProjectModelException">ProjectModelException is thrown when it fails to
		/// write to the file.
		/// </exception> 
		public void Write(string fileName)
		{
			try
			{
				//Open the stream and read model from it.
				using (FileStream fs = File.Open(fileName, FileMode.Create)) 
				{
					Write(fs);
					fs.Flush();
				}
			}
			catch (System.IO.IOException ex)
			{
				throw new ProjectModelException("Failed to write to file :" + fileName, ex);
			}
		}
		
		/// <summary>
		/// Write the data view as a XML data to a Stream.
		/// </summary>
		/// <param name="stream">the stream object to which to write a XML data</param>
		/// <exception cref="ProjectModelException">ProjectModelException is thrown when it fails to
		/// write to the stream.
		/// </exception>
		public void Write(Stream stream)
		{
			try
			{
				XmlDocument doc = GetXmlDocument();

				doc.Save(stream);
			}
			catch (System.IO.IOException ex)
			{
				throw new ProjectModelException("Failed to write the SchemaModel object", ex);
			}
		}

		/// <summary>
		/// Write the data view as a XML data to a TextWriter.
		/// </summary>
		/// <param name="writer">the TextWriter instance to which to write a XML schema
		/// </param>
		/// <exception cref="ProjectModelException">ProjectModelException is thrown when it fails to
		/// write to the stream.
		/// </exception>
		public void Write(TextWriter writer)
		{
			try
			{
				XmlDocument doc = GetXmlDocument();

				doc.Save(writer);
			}
			catch (System.IO.IOException ex)
			{
				throw new ProjectModelException("Failed to write the SchemaModel object", ex);
			}
		}

		/// <summary>
		/// Unmarshal an element representing data view collection
		/// </summary>
		/// <param name="parent">An xml element</param>
		public override void Unmarshal(XmlElement parent)
		{
			base.Unmarshal(parent);

			// restore _projectDir
			_projectDir = parent.GetAttribute("ProjectDir");

			_parsers = (ParserElementCollection) ElementFactory.Instance.Create((XmlElement) parent.ChildNodes[0]);
			_parsers.ValueChanged += new EventHandler(ValueChangedHandler);
			_parsers.ParentElement = this;
		}

		/// <summary>
		/// Write values of members to an xml element
		/// </summary>
		/// <param name="parent">An xml element for the element</param>
		public override void Marshal(XmlElement parent)
		{
			base.Marshal(parent);

			// write the _projectDir member
			parent.SetAttribute("ProjectDir", _projectDir);

			// write the _parsers collection
			XmlElement child = parent.OwnerDocument.CreateElement(ElementFactory.ConvertTypeToString(Parsers.ElementType));
			Parsers.Marshal(child);
			parent.AppendChild(child);
		}

		/// <summary>
		/// Gets the xml document that represents the project
		/// </summary>
		/// <returns>A XmlDocument instance</returns>
		private XmlDocument GetXmlDocument()
		{
			// Marshal the objects to xml document
			XmlDocument doc = new XmlDocument();

			XmlElement element = doc.CreateElement("Project");

			doc.AppendChild(element);

			Marshal(element);

			return doc;
		}
	}
}