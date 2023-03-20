/*
* @(#)MappingManager.cs
*
* Copyright (c) 2004 Newtera, Inc. All rights reserved.
*
*/
namespace Newtera.Common.MetaData.Mappings
{
	using System;
	using System.Xml;
	using System.IO;
	using System.Text;
	using System.Collections;

	using Newtera.Common.Core;
	using Newtera.Common.MetaData.Schema;

	/// <summary>
	/// This is the top level class that manages all Mapping packages for a
	/// schema. It also provides methods to allow easy accesses, addition, and 
	/// deletion of mapping packages.
	/// </summary>
	/// <version> 1.0.0 03 Sep 2004 </version>
	/// <author> Yong Zhang </author>
	public class MappingManager : MappingNodeBase
	{
		private bool _isAltered;
		
		private MappingPackageCollection _mappingPackages;

		/// <summary>
		/// Initiate an instance of MappingManager class
		/// </summary>
		public MappingManager(): base()
		{
			_isAltered = false;
			_mappingPackages = new MappingPackageCollection();
            if (GlobalSettings.Instance.IsWindowClient)
            {
                _mappingPackages.ValueChanged += new EventHandler(this.ValueChangedHandler);
            }
		}

		/// <summary>
		/// Gets or sets the information indicating whether rule information has been
		/// altered.
		/// </summary>
		/// <value>true if it is altered, false otherwise.</value>
		public bool IsAltered
		{
			get
			{
				return _isAltered;
			}
			set
			{
				_isAltered = value;
			}
		}

		/// <summary>
		/// Gets the information indicating whether it is an empty rule set
		/// </summary>
		/// <value>true if it is an empty rule set, false otherwise.</value>
		public bool IsEmpty
		{
			get
			{
				if (this._mappingPackages.Count == 0)
				{
					return true;
				}
				else
				{
					return false;
				}
			}
		}
		
		/// <summary>
		/// Gets a mapping packages of given name
		/// </summary>
		/// <param name="name">The package name</param>
		/// <returns>A MappingPackage instance, null if not found.</returns>
		public MappingPackage GetMappingPackage(string name)
		{
			MappingPackage found = null;

			foreach (MappingPackage package in _mappingPackages)
			{
				if (package.Name == name)
				{
					found = package;
					break;
				}
			}

			return found;
		}

		/// <summary>
		/// Gets the information indicating whether a package of the given name has
		/// existed.
		/// </summary>
		/// <param name="name">The package name</param>
		/// <returns>true if it exists, false otherwise.</returns>
		public bool IsPackageExist(string name)
		{
			bool found = false;

			foreach (MappingPackage package in _mappingPackages)
			{
				if (package.Name == name)
				{
					found = true;
					break;
				}
			}

			return found;
		}

        /// <summary>
        /// Gets all mapping packages defined for a schema
        /// </summary>
        /// <returns>A collection of MappingPackage objects, it can be an empty collection.</returns>
        public MappingPackageCollection GetMappingPackages()
        {
            return _mappingPackages;
        }

		/// <summary>
		/// Gets mapping packages for a source data type
		/// </summary>
		/// <param name="type">One of the DataSourceType enum values</param>
		/// <returns>A collection of MappingPackage objects, it can be an empty collection.</returns>
		public MappingPackageCollection GetMappingPackages(DataSourceType type)
		{
			MappingPackageCollection packages = new MappingPackageCollection();

			foreach (MappingPackage package in _mappingPackages)
			{
				if (package.DataSourceType == type)
				{
					packages.Add(package);
				}
			}

			return packages;
		}

        /// <summary>
        /// Gets mapping packages of a source data type for a specific destination class.
        /// </summary>
        /// <param name="type">One of the DataSourceType enum values</param>
        /// <param name="destinationClass">The name of the destination class.</param>
        /// <returns>A collection of MappingPackage objects, it can be an empty collection.</returns>
        /// <remarks>Return the packages that have only one destination class and the class name is the same as the specified one.</remarks>
        public MappingPackageCollection GetMappingPackagesByClass(DataSourceType type, string destinationClass)
        {
            MappingPackageCollection packages = new MappingPackageCollection();

            foreach (MappingPackage package in _mappingPackages)
            {
                if ((package.DataSourceType == type || type == DataSourceType.Unknown)
                    && ContainsDestinationClass(package, destinationClass))
                {
                    packages.Add(package);
                }
            }

            return packages;
        }

		/// <summary>
		/// Add a mapping package to the collection
		/// </summary>
		/// <param name="package">The MappingPackage to be added</param>
		public void AddMappingPackage(MappingPackage package)
		{	
			_mappingPackages.Add(package);
		}

		/// <summary>
		/// Remove a mapping package from the collection.
		/// </summary>
		/// <param name="package">The package instance to be removed</param>
		public void RemoveMappingPackage(MappingPackage package)
		{
			_mappingPackages.Remove(package);
		}

        /// <summary>
        /// Remove a mapping package of a given name from the collection.
        /// </summary>
        /// <param name="packageName">The package name</param>
        public void RemoveMappingPackage(string packageName)
        {
            foreach (MappingPackage package in _mappingPackages)
            {
                if (package.Name == packageName)
                {
                    _mappingPackages.Remove(package);
                    break;
                }
            }
        }

		/// <summary>
		/// Read mappings from an XML file.
		/// </summary>
		/// <param name="fileName">the name of the XML file</param>
		/// <exception cref="MappingException">MappingException is thrown when it fails to
		/// read the XML file
		/// </exception>
		public void Read(string fileName)
		{
			try
			{
				//Open the stream and read XSD from it.
				if (File.Exists(fileName))
				{
					using (FileStream fs = File.OpenRead(fileName)) 
					{
						Read(fs);					
					}
				}
			}
			catch (Exception ex)
			{
                throw new MappingException("Failed to read the file :" + fileName + " with reason " + ex.Message, ex);
			}
		}
		
		/// <summary>
		/// Read mappings from an stream.
		/// </summary>
		/// <param name="stream">the stream</param>
		/// <exception cref="MappingException">MappingException is thrown when it fails to
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
					throw new MappingException(e.Message, e);
				}
			}
		}

		/// <summary>
		/// Read mappings from a text reader.
		/// </summary>
		/// <param name="reader">the text reader</param>
		/// <exception cref="MappingException">MappingException is thrown when it fails to
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
					throw new MappingException(e.Message, e);
				}
			}
		}

		/// <summary>
		/// Write mappings to an XML file.
		/// </summary>
		/// <param name="fileName">The output file name.</param>
		/// <exception cref="MappingException">MappingException is thrown when it fails to
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
				throw new MappingException("Failed to write to file :" + fileName, ex);
			}
		}
		
		/// <summary>
		/// Write mappings as a XML data to a Stream.
		/// </summary>
		/// <param name="stream">the stream object to which to write a XML data</param>
		/// <exception cref="MappingException">MappingException is thrown when it fails to
		/// write to the stream.</exception>
		public void Write(Stream stream)
		{
			try
			{
				XmlDocument doc = GetXmlDocument();

				doc.Save(stream);
			}
			catch (Exception ex)
			{
				throw new MappingException("Failed to write the mappings", ex);
			}
		}

		/// <summary>
		/// Write mappings as a XML data to a TextWriter.
		/// </summary>
		/// <param name="writer">the TextWriter instance to which to write a XML schema
		/// </param>
		/// <exception cref="MappingException">MappingException is thrown when it fails to
		/// write to the stream.</exception>
		public void Write(TextWriter writer)
		{
			try
			{
				XmlDocument doc = GetXmlDocument();

				doc.Save(writer);
			}
			catch (Exception ex)
			{
				throw new MappingException("Failed to write the mappings", ex);
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
				return NodeType.MappingManager;
			}
		}

		/// <summary>
		/// create mappings from a xml document.
		/// </summary>
		/// <param name="parent">An xml element</param>
		public override void Unmarshal(XmlElement parent)
		{
			base.Unmarshal(parent);

			// a collection of RuleSet instances
			_mappingPackages = (MappingPackageCollection) MappingNodeFactory.Instance.Create((XmlElement) parent.ChildNodes[0]);

            if (GlobalSettings.Instance.IsWindowClient)
            {
                _mappingPackages.ValueChanged += new EventHandler(this.ValueChangedHandler);
            }
		}

		/// <summary>
		/// write mappings to an xml document
		/// </summary>
		/// <param name="parent">An xml element for the element</param>
		public override void Marshal(XmlElement parent)
		{
			base.Marshal(parent);

			// write the rule defs
			XmlElement child = parent.OwnerDocument.CreateElement(MappingNodeFactory.ConvertTypeToString(_mappingPackages.NodeType));
			_mappingPackages.Marshal(child);
			parent.AppendChild(child);
		}

		/// <summary>
		/// Gets the xml document that represents an mappings
		/// </summary>
		/// <returns>A XmlDocument instance</returns>
		private XmlDocument GetXmlDocument()
		{
			// Marshal the objects to xml document
			XmlDocument doc = new XmlDocument();

			XmlElement element = doc.CreateElement("MappingManager");

			doc.AppendChild(element);

			Marshal(element);

			return doc;
		}

		/// <summary>
		/// A handler to call when a value of the mappings changed
		/// </summary>
		/// <param name="sender">the IXaclNode that cause the event</param>
		/// <param name="e">the arguments</param>
		protected override void ValueChangedHandler(object sender, EventArgs e)
		{
			_isAltered = true;
		}

        /// <summary>
        /// gets the information indicating whether a destination class is one of the package's destination classes
        /// </summary>
        /// <param name="package"></param>
        /// <param name="destinationClass"></param>
        /// <returns></returns>
        private bool ContainsDestinationClass(MappingPackage package, string destinationClassName)
        {
            bool status = false;

            foreach (ClassMapping classMapping in package.ClassMappings)
            {
                if (classMapping.DestinationClassName == destinationClassName)
                {
                    status = true;
                    break;
                }
            }

            return status;
        }
	}
}