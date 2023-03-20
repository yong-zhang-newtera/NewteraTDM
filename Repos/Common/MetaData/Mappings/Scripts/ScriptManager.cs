/*
* @(#)ScriptManager.cs
*
* Copyright (c) 2004 Newtera, Inc. All rights reserved.
*
*/
namespace Newtera.Common.MetaData.Mappings.Scripts
{
	using System;
	using System.Xml;
	using System.IO;
	using System.Text;
	using System.Collections;

	using Newtera.Common.Core;

	/// <summary>
	/// This is the top level class that manages a classScript of executable scripts for
	/// an import job. It also provides methods to allow easy accesses, addition, and 
	/// deletion of mapping packages.
	/// </summary>
	/// <version> 1.0.0 23 Sep 2004 </version>
	/// <author> Yong Zhang </author>
	public class ScriptManager : ScriptNodeBase
	{
		private ClassScriptCollection _classScripts;

		/// <summary>
		/// Initiate an instance of ScriptManager class
		/// </summary>
		public ScriptManager(): base()
		{
			_classScripts = new ClassScriptCollection();
		}

		/// <summary>
		/// Gets or sets the information indicating whether the execution of
		/// all scripts are succeeded?
		/// </summary>
		/// <value>true if succeeded, false otherwise.</value>
		public bool IsSucceeded
		{
			get
			{
				return IsAllClassesSucceeded();
			}
		}

		/// <summary>
		/// Gets the collection of the ClassScript instances
		/// </summary>
		public ClassScriptCollection ClassScripts
		{
			get
			{
				return this._classScripts;
			}
		}

		/// <summary>
		/// Gets the information indicating whether a ClassScript of the given name has
		/// existed.
		/// </summary>
		/// <param name="name">The ClassScript name</param>
		/// <returns>true if it exists, false otherwise.</returns>
		public bool IsClassScriptExist(string name)
		{
			bool found = false;

			foreach (ClassScript classScript in _classScripts)
			{
				if (classScript.ClassName == name)
				{
					found = true;
					break;
				}
			}

			return found;
		}

		/// <summary>
		/// Add a ClassScript to the collection
		/// </summary>
		/// <param name="classScript">The ClassScript to be added</param>
		public void AddClassScript(ClassScript classScript)
		{	
			_classScripts.Add(classScript);
		}

		/// <summary>
		/// Remove a ClassScript from the collection.
		/// </summary>
		/// <param name="classScript">The classScript instance to be removed</param>
		public void RemoveClassScript(ClassScript classScript)
		{
			_classScripts.Remove(classScript);
		}

		/// <summary>
		/// Read import scripts from an XML file.
		/// </summary>
		/// <param name="fileName">the name of the XML file</param>
		/// <exception cref="ScriptException">ScriptException is thrown when it fails to
		/// read the XML file
		/// </exception>
		public void Read(string fileName)
		{
			try
			{
				//Open the stream and read XSD from it.
				using (FileStream fs = File.OpenRead(fileName)) 
				{
					Read(fs);					
				}
			}
			catch (Exception ex)
			{
                throw new ScriptException("Failed to read the file :" + fileName + " with reason " + ex.Message, ex);
			}
		}
		
		/// <summary>
		/// Read scripts from an stream.
		/// </summary>
		/// <param name="stream">the stream</param>
		/// <exception cref="ScriptException">ScriptException is thrown when it fails to
		/// read the stream.</exception>
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
					throw new ScriptException(e.Message, e);
				}
			}
		}

		/// <summary>
		/// Read scripts from a text reader.
		/// </summary>
		/// <param name="reader">the text reader</param>
		/// <exception cref="ScriptException">ScriptException is thrown when it fails to
		/// read the text reader</exception>
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
					throw new ScriptException(e.Message, e);
				}
			}
		}

		/// <summary>
		/// Write scripts to an XML file.
		/// </summary>
		/// <param name="fileName">The output file name.</param>
		/// <exception cref="ScriptException">ScriptException is thrown when it fails to
		/// write to the file.</exception> 
		public void Write(string fileName)
		{
			try
			{
				//Open the stream and read XSD from it.
				using (FileStream fs = File.Open(fileName, FileMode.Create)) 
				{
					Write(fs);
					fs.Flush();
				}
			}
			catch (System.IO.IOException ex)
			{
				throw new ScriptException("Failed to write to file :" + fileName, ex);
			}
		}
		
		/// <summary>
		/// Write mappings as a XML data to a Stream.
		/// </summary>
		/// <param name="stream">the stream object to which to write a XML data</param>
		/// <exception cref="ScriptException">ScriptException is thrown when it fails to
		/// write to the stream.</exception>
		public void Write(Stream stream)
		{
			try
			{
				XmlDocument doc = GetXmlDocument();

				doc.Save(stream);
			}
			catch (System.IO.IOException ex)
			{
				throw new ScriptException("Failed to write the scripts", ex);
			}
		}

		/// <summary>
		/// Write scripts as a XML data to a TextWriter.
		/// </summary>
		/// <param name="writer">the TextWriter instance to which to write a XML schema
		/// </param>
		/// <exception cref="ScriptException">ScriptException is thrown when it fails to
		/// write to the stream.</exception>
		public void Write(TextWriter writer)
		{
			try
			{
				XmlDocument doc = GetXmlDocument();

				doc.Save(writer);
			}
			catch (System.IO.IOException ex)
			{
				throw new ScriptException("Failed to write the scripts", ex);
			}
		}

		/// <summary>
		/// Gets the type of node
		/// </summary>
		/// <value>One of NodeType values</value>
		public override NodeType NodeType 
		{
			get
			{
				return NodeType.ScriptManager;
			}
		}

		/// <summary>
		/// create scripts from a xml document.
		/// </summary>
		/// <param name="parent">An xml element</param>
		public override void Unmarshal(XmlElement parent)
		{
			base.Unmarshal(parent);

			// a collection of RuleSet instances
			_classScripts = (ClassScriptCollection) ScriptNodeFactory.Instance.Create((XmlElement) parent.ChildNodes[0]);
		}

		/// <summary>
		/// write scripts to an xml document
		/// </summary>
		/// <param name="parent">An xml element for the element</param>
		public override void Marshal(XmlElement parent)
		{
			base.Marshal(parent);

			// write the rule defs
			XmlElement child = parent.OwnerDocument.CreateElement(ScriptNodeFactory.ConvertTypeToString(_classScripts.NodeType));
			_classScripts.Marshal(child);
			parent.AppendChild(child);
		}

		/// <summary>
		/// Gets the xml document that represents an xacl policy
		/// </summary>
		/// <returns>A XmlDocument instance</returns>
		private XmlDocument GetXmlDocument()
		{
			// Marshal the objects to xml document
			XmlDocument doc = new XmlDocument();

			XmlElement element = doc.CreateElement("ScriptManager");

			doc.AppendChild(element);

			Marshal(element);

			return doc;
		}

		/// <summary>
		/// Gets the information indicating whether all the classes have been
		/// imported successfully.
		/// </summary>
		/// <returns>true if they all succeeded, false, if any of them didn't</returns>
		private bool IsAllClassesSucceeded()
		{
			bool status = true;

			foreach (ClassScript classScript in _classScripts)
			{
				if (!classScript.IsSucceeded)
				{
					status = false;
					break;
				}
			}

			return status;
		}
	}
}