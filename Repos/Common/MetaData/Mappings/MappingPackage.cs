/*
* @(#)MappingPackage.cs
*
* Copyright (c) 2003-2004 Newtera, Inc. All rights reserved.
*
*/
namespace Newtera.Common.MetaData.Mappings
{
	using System;
	using System.Xml;
	using System.Data;
	using System.Collections;
	using System.Text;
	using System.CodeDom.Compiler;
	using System.Reflection;

	using Newtera.Common.MetaData.Mappings.Generator;

	/// <summary>
	/// The class represents mapping package of between a source and
	/// destination. A mapping package contains one or more ClassMapping objects
	/// and parsing information for the source data.
	/// </summary>
	/// <version>  1.0.0 04 Sep 2004</version>
	/// <author> Yong Zhang </author>
	public class MappingPackage : MappingNodeBase
	{
		internal const string DelimitedTextFileConverterTypeName = "Newtera.Conveters.DelimitedTextFileConverter, Newtera.Conveters";
		internal const string ExcelFileConverterTypeName = "Newtera.Conveters.ExcelFileConverter, Newtera.Conveters";
		internal const string XmlFileConverterTypeName = "Newtera.Conveters.XmlFileConverter, Newtera.Conveters";
	
		private string _converterTypeName;
		private DataSourceType _dataSourceType;
		private TextFormat _textFormat;
		private bool _isPaging = false;
		private int _blockSize = 1000;
        private bool _needValidation = true;
        private bool _modifyExistingInstances = false;
        private InstanceIdentifier _instanceIdentifier = InstanceIdentifier.PrimaryKeys; // default
		
		private ClassMappingCollection _classMappings;
		
		/// <summary>
		/// Initiate an instance of a MappingPackage class.
		/// </summary>
		/// <param name="dataSourceType">One of DataSourceType enum</param>
		public MappingPackage(DataSourceType dataSourceType) : base()
		{
			_converterTypeName = GetDefaultConverterTypeName(dataSourceType);
			_dataSourceType = dataSourceType;
			_classMappings = new ClassMappingCollection();
            if (GlobalSettings.Instance.IsWindowClient)
            {
                _classMappings.ValueChanged += new EventHandler(this.ValueChangedHandler);
            }
			_textFormat = new TextFormat();
            if (GlobalSettings.Instance.IsWindowClient)
            {
                _textFormat.ValueChanged += new EventHandler(this.ValueChangedHandler);
            }
		}
		
		/// <summary>
		/// Initiating an instance of MappingPackage class
		/// </summary>
		/// <param name="xmlElement">The xml element conatins data of the instance</param>
		internal MappingPackage(XmlElement xmlElement) : base()
		{
			Unmarshal(xmlElement);
		}

		/// <summary>
		/// Gets or sets the converter type name
		/// </summary>
		/// <value>A converter type name consists of converter class name and assembly name.</value>
		public string ConverterTypeName
		{
			get
			{
				return _converterTypeName;
			}
			set
			{
				_converterTypeName = value;
			}
		}

		/// <summary>
		/// Gets or sets the type of data source
		/// </summary>
		public DataSourceType DataSourceType
		{
			get
			{
				return _dataSourceType;
			}
			set
			{
				_dataSourceType = value;
			}
		}

        /// <summary>
        /// Gets or sets the information indicating whether to perform validation on
        /// permissions and etc.
        /// </summary>
        /// <value>True to perform validation, false not to perform validation.</value>
        public bool NeedValidation
        {
            get
            {
                return _needValidation;
            }
            set
            {
                _needValidation = value;
            }
        }

        /// <summary>
        /// Gets or sets the information indicating whether to modify the existing instances if the primary keys of the instances match.
        /// </summary>
        /// <value>True to perform modification if the primary keys match, otherwise, add as new instances to the database.</value>
        public bool ModifyExistingInstances
        {
            get
            {
                return _modifyExistingInstances;
            }
            set
            {
                _modifyExistingInstances = value;
            }
        }

        /// <summary>
        /// Gets or sets type of instance identifier.which is used to identifying the exsiting instances to modify
        /// </summary>
        /// <value>One of the InstanceIdentifier enum values</value>
        public InstanceIdentifier InstanceIdentifier
        {
            get
            {
                return _instanceIdentifier;
            }
            set
            {
                _instanceIdentifier = value;
            }
        }

		/// <summary>
		/// Gets or sets the information indicating whether to read data in pages
		/// </summary>
		/// <value>True to read data in pages, false to read data all at once.</value>
		public bool IsPaging
		{
			get
			{
				return _isPaging;
			}
			set
			{
				_isPaging = value;
			}
		}

		/// <summary>
		/// Gets or sets the number of rows in a block read
		/// </summary>
		public int BlockSize
		{
			get
			{
				return _blockSize;
			}
			set
			{
				_blockSize = value;
			}
		}

		/// <summary>
		/// Gets the TextFormat instance
		/// </summary>
		/// <remarks>Valid when the data source type is Text</remarks>
		public TextFormat TextFormat
		{
			get
			{
				return _textFormat;
			}
		}

		/// <summary>
		/// Gets the ClassMapping instances contained in a MappingPackage
		/// </summary>
		public ClassMappingCollection ClassMappings
		{
			get
			{
				return _classMappings;
			}
		}

		/// <summary>
		/// Gets the checked ClassMapping instances contained in a MappingPackage
		/// </summary>
		public ClassMappingCollection CheckedClassMappings
		{
			get
			{
				ClassMappingCollection clsMappings = new ClassMappingCollection();

				foreach (ClassMapping classMapping in _classMappings)
				{
					if (classMapping.IsChecked)
					{
						clsMappings.Add(classMapping);
					}
				}

				return clsMappings;
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
				return NodeType.MappingPackage;
			}
		}

		/// <summary>
		/// Gets the default converter type name based on the data source type
		/// </summary>
		/// <param name="dataSourceType">One of the DataSourceType values</param>
		/// <returns>A string representing a type name of default converter.</returns>
		public string GetDefaultConverterTypeName(DataSourceType dataSourceType)
		{
			string typeName = null;

			switch (dataSourceType)
			{
				case DataSourceType.Text:
					typeName = MappingPackage.DelimitedTextFileConverterTypeName;
					break;
				case DataSourceType.Excel:
					typeName = MappingPackage.ExcelFileConverterTypeName;
					break;
				//case DataSourceType.Xml:
				//	typeName = MappingPackage.XmlFileConverterTypeName;
				//	break;
			}

			return typeName;
		}

		/// <summary>
		/// Transform data from source format to destination format according to
		/// the class mappings.
		/// </summary>
		/// <param name="sourceDataSet">The DataSet for the source data</param>
		/// <param name="libPath">The directory where to find referenced assemblies of the compiled assembly.</param>
		/// <returns>The DataSet for destination data.</returns>
		public DataSet Transform(DataSet sourceDataSet, string libPath)
		{
			// compile the transform scripts defined in the package into an assembly
			Assembly assembly = CompileScripts(libPath);

			DataSet dstDataSet = new DataSet();
			
			// perform the transformation one class by one class
			foreach (ClassMapping classMapping in _classMappings)
			{
				if (classMapping.IsChecked)
				{
					classMapping.Transform(sourceDataSet, dstDataSet, assembly);
				}
			}

			return dstDataSet;
		}

		/// <summary>
		/// Transform data from source format to destination format for a specified
		/// class mappings.
		/// </summary>
		/// <param name="classMapping">The specified class mapping.</param>
		/// <param name="sourceDataSet">The DataSet for the source data</param>
		/// <param name="libPath">The directory where to place the compiled assembly for transform script.</param>
		/// <returns>The DataSet for transformed data.</returns>
		public DataSet Transform(ClassMapping classMapping, DataSet sourceDataSet, string libPath)
		{
			// compile the transform scripts defined in the package into an assembly
			Assembly assembly = CompileScripts(libPath);

			DataSet dstDataSet = new DataSet();
			
			// perform the transformation for the class
			if (classMapping.IsChecked)
			{
				classMapping.Transform(sourceDataSet, dstDataSet, assembly);
			}

			return dstDataSet;
		}

		/// <summary>
		/// Add a ClassMapping instance to the MappingPackage's class mapping collection
		/// if the mapping does not exist, otherwise, modify the existing ClassMapping instance
		/// A ClassMapping instance is identified by the source name.
		/// </summary>
		/// <param name="sourceName">The source name</param>
		/// <param name="destinationName">The destination name</param>
		/// <returns>The ClassMapping instance added or located.</returns>
		public ClassMapping AddClassMapping(string sourceName, string destinationName)
		{
			ClassMapping theClassMapping = null;

			theClassMapping = new ClassMapping(sourceName, destinationName);
			this.ClassMappings.Add(theClassMapping);

			return theClassMapping;
		}

		/// <summary>
		/// Set a destination class to the class mapping by clearing the old attribute
		/// mappings.
		/// </summary>
		/// <param name="classMapping">The class mapping object</param>
		/// <param name="destinationName">The destination name</param>
		public void SetClassMappingDestination(ClassMapping classMapping, string destinationName)
		{
			if (classMapping.DestinationClassName != destinationName)
			{
				// destination has been changed, clear the attribute mapping
				classMapping.DestinationClassName = destinationName;
				classMapping.Clear();
			}
		}

		/// <summary>
		/// Gets the first ClassMapping instance whose source class name equals
		/// to the given one.
		/// </summary>
		/// <param name="srcName">The source class name</param>
		/// <returns>The matched ClassMapping instance or null if not matched.</returns>
		public ClassMapping FindClassMappingBySrcName(string srcName)
		{
			ClassMapping found = null;

			foreach (ClassMapping classMapping in _classMappings)
			{
				if (classMapping.SourceClassName == srcName)
				{
					found = classMapping;
					break;
				}
			}

			return found;
		}

		/// <summary>
		/// create Ruleset instance from a xml document.
		/// </summary>
		/// <param name="parent">An xml element</param>
		public override void Unmarshal(XmlElement parent)
		{
			base.Unmarshal(parent);

			string str = parent.GetAttribute("Converter");
			if (str != null && str.Length > 0)
			{
				_converterTypeName = str;
			}
			else
			{
				_converterTypeName = null;
			}

			str = parent.GetAttribute("SourceType");
			if (str != null && str.Length > 0)
			{
				_dataSourceType = (DataSourceType) Enum.Parse(typeof(DataSourceType), str);
			}
			else
			{
				_dataSourceType = DataSourceType.Unknown;
			}

			str = parent.GetAttribute("paging");
			if (str != null && str == "true")
			{
				this._isPaging = true;
			}
			else
			{
				this._isPaging = false; // default is false
			}

			str = parent.GetAttribute("BlockSize");
			if (str != null && str.Length > 0)
			{
				this._blockSize = int.Parse(str);
			}

            str = parent.GetAttribute("validate");
            if (str != null && str == "false")
            {
                this._needValidation = false;
            }
            else
            {
                this._needValidation = true; // default is true
            }

            str = parent.GetAttribute("modifyExisting");
            if (!string.IsNullOrEmpty(str) && str == "true")
            {
                this._modifyExistingInstances = true;
            }
            else
            {
                this._modifyExistingInstances = false; // default is false
            }

            str = parent.GetAttribute("identifier");
            if (!string.IsNullOrEmpty(str))
            {
                try
                {
                    this.InstanceIdentifier = ((InstanceIdentifier)Enum.Parse(typeof(InstanceIdentifier), str));
                }
                catch (Exception)
                {
                    this.InstanceIdentifier = InstanceIdentifier.PrimaryKeys;
                }
            }
            else
            {
                this.InstanceIdentifier = InstanceIdentifier.PrimaryKeys; // default identifier is priamry keys
            }

			// the first xml child is representing text format instance
			_textFormat = (TextFormat) MappingNodeFactory.Instance.Create((XmlElement) parent.ChildNodes[0]);
            if (GlobalSettings.Instance.IsWindowClient)
            {
                _textFormat.ValueChanged += new EventHandler(this.ValueChangedHandler);
            }

			// then a collection of  class mapping instances
			_classMappings = (ClassMappingCollection) MappingNodeFactory.Instance.Create((XmlElement) parent.ChildNodes[1]);
            if (GlobalSettings.Instance.IsWindowClient)
            {
                _classMappings.ValueChanged += new EventHandler(this.ValueChangedHandler);
            }
		}

		/// <summary>
		/// write MappingPackage instance to xml document
		/// </summary>
		/// <param name="parent">An xml element for the element</param>
		public override void Marshal(XmlElement parent)
		{
			base.Marshal(parent);

			// write the _converterTypeName
			if (_converterTypeName != null && _converterTypeName.Length > 0)
			{
				parent.SetAttribute("Converter", _converterTypeName);
			}

			// write the _dataSourceType
			if (_dataSourceType != DataSourceType.Unknown)
			{
				parent.SetAttribute("SourceType", Enum.GetName(typeof(DataSourceType), _dataSourceType));
			}

			// write _isPaging
			if (!_isPaging)
			{
				parent.SetAttribute("paging", "true"); // default is false
			}

			// write blockSize
			parent.SetAttribute("BlockSize", this._blockSize.ToString());

            // write _needValidation
            if (!_needValidation)
            {
                parent.SetAttribute("validate", "false"); // default is true
            }

            // write _modifyExistingInstances
            if (_modifyExistingInstances)
            {
                parent.SetAttribute("modifyExisting", "true");
            }

            // write instanceIdentifier
            if (_instanceIdentifier != InstanceIdentifier.PrimaryKeys)
            {
                parent.SetAttribute("identifier", Enum.GetName(typeof(InstanceIdentifier), _instanceIdentifier));
            }
		
			// write the format mapping instance
			XmlElement child = parent.OwnerDocument.CreateElement(MappingNodeFactory.ConvertTypeToString(_textFormat.NodeType));
			_textFormat.Marshal(child);
			parent.AppendChild(child);

			// write the class mapping instances
			child = parent.OwnerDocument.CreateElement(MappingNodeFactory.ConvertTypeToString(_classMappings.NodeType));
			_classMappings.Marshal(child);
			parent.AppendChild(child);
		}

		/// <summary>
		/// Compile the transform scripts defined in this package into an assembly
		/// and place the assembly in a given path
		/// </summary>
		/// <param name="libPath">The path where to place the assembly</param>
		/// <returns>The compiled assembly</returns>
		private Assembly CompileScripts(string libPath)
		{
			Assembly assembly = null;
			CompilerResults cr = null;
			StringBuilder builder = new StringBuilder();
			ScriptLanguage language = ScriptLanguage.CSharp; // TODO, handle the scripts in different languages

			// collect individual scripts and combine them together so that
			// we can compile them into a single assembly
			foreach (ClassMapping classMapping in _classMappings)
			{
				if (classMapping.IsChecked)
				{
					// clear the cached transformers firs
					classMapping.UnsetTransformers();

					classMapping.AppendScripts(builder);
				}
			}

			// compile the scripts into an assembly at the given libpath
			if (builder.Length > 0)
			{

				cr = CodeStubGenerator.Instance.CompileFromSource(language, builder.ToString(),
					libPath);

				if (cr.Errors.Count > 0)
				{
					throw new MappingException("There are errors while compiling transformation scripts.");
				}
				else
				{
					assembly = cr.CompiledAssembly;
				}
			}

			return assembly;
		}
	}

    public enum InstanceIdentifier
    {
        PrimaryKeys,
        UniqueKeys
    }
}