/*
* @(#)ParseTreeProcessor.cs
*
* Copyright (c) 2005 Newtera, Inc. All rights reserved.
*
*/
namespace Newtera.ParserGen.ParseTree
{
	using System;
	using System.Xml;
	using System.Data;
    using System.IO;
	using System.Collections;
    using Microsoft.Win32;

	/// <summary>
	/// A utility class that processes a parse tree of IParseTreeNode type.
	/// </summary>
	/// <version>1.0.1 11 Nov. 2005</version>
	/// <author>Yong Zhang</author>
	public class ParseTreeProcessor
	{
		private string _parserName;
		private IParseTreeNode _root;
		private DataGridSettings _settings;

		/// <summary>
		/// Initiating an instance of ParseTreeProcessor class
		/// </summary>
		public ParseTreeProcessor(IParseTreeNode treeRoot)
		{
			_root = treeRoot;
			_parserName = null;
		}

		/// <summary>
		/// Initiating an instance of ParseTreeProcessor class
		/// </summary>
		public ParseTreeProcessor(IParseTreeNode treeRoot, string parserName)
		{
			_root = treeRoot;
			_parserName = parserName;
		}

		/// <summary>
		/// Gets or sets the root of a parse tree.
		/// </summary>
		public IParseTreeNode TreeRoot
		{
			get
			{
				return _root;
			}
			set
			{
				_root = value;
			}
		}

		/// <summary>
		/// Gets or sets settings for transforms a parse tree to DataSet.
		/// </summary>
		public DataGridSettings Settings
		{
			get
			{
				return _settings;
			}
			set
			{
				_settings = value;
			}
		}

		/// <summary>
		/// Transform a parse tree to a DataSet according to the specifications
		/// in the settings.
		/// </summary>
		/// <returns>The transformed dataset</returns>
		public DataSet Transform()
		{
			DataSet dataSet = new DataSet();

			// add a DataTable to the data set
			dataSet.Tables.Add("DataGrid");

			if (_settings == null && _parserName != null)
			{
				// read the setting from a XML file located at the same directory as the converter
                string fileName = GetConverterAssemblyDir() + _parserName + ".xml";
				_settings = DataGridSettings.Read(fileName);
			}

			if (_settings != null)
			{
				CreateDataColumnVisitor visitor1 = new CreateDataColumnVisitor(dataSet,
					this._settings);

				// create columns in the dataset
				_root.Accept(visitor1);
				visitor1.CreateDataColumns();

				CreateDataRowVisitor visitor2 = new CreateDataRowVisitor(dataSet,
					this._settings);

				// create rows in the dataset
				_root.Accept(visitor2);
			}

			return dataSet;
		}

        /// <summary>
        /// A utility that get the directory where the converter assemblies reside.
        /// </summary>
        /// <returns>A directory string</returns>
        private string GetConverterAssemblyDir()
        {
            string dir = GetAppToolDir();

            if (!dir.EndsWith(@"\"))
            {
                dir += @"\bin\";
            }
            else
            {
                dir += @"bin\";
            }

            // test if it is in production mode or debug mode
            if (!File.Exists(dir + @"Newtera.Common.dll"))
            {
                // it is the debug mode, add Debug directory at the end of dir
                dir += @"Debug\";
            }

            return dir;
        }

        /// <summary>
        /// Gets the base directory where the windows clients reside
        /// </summary>
        /// <returns>An absulute directory</returns>
        public string GetAppToolDir()
        {
            //create a reference to a valid key, which was created earlier
            //remember that the key name is case-insensitive
            RegistryKey rk = Registry.LocalMachine.OpenSubKey(GetRegistryPath());

            //get the data from a specified item in that key    
            //Note that if the item "TOOL_DIR" does not exist, then null is returned...
            String dir = (String)rk.GetValue("TOOL_DIR");

            return dir;
        }

        /// <summary>
        /// Gets the registry key path for Newtera Product
        /// </summary>
        /// <returns></returns>
        private string GetRegistryPath()
        {
            string registryPath;
            if (Is64BitMode())
            {
                // 64 bit kep path
                //registryPath = @"Software\Wow6432Node\Newtera";
                registryPath = @"Software\Newtera";
            }
            else
            {
                // 32 bit kep path
                registryPath = @"Software\Newtera";
            }

            return registryPath;
        }

        /// <summary>
        /// Gets information indicating whether it is a 64 bits system
        /// </summary>
        /// <returns>true if it is 64, false otherwise</returns>
        private bool Is64BitMode()
        {
            return System.Runtime.InteropServices.Marshal.SizeOf(typeof(IntPtr)) == 8;
        }
	}
}