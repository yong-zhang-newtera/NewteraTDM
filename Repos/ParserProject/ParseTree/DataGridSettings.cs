/*
* @(#)DataGridSettings.cs
*
* Copyright (c) 2005 Newtera, Inc. All rights reserved.
*
*/
namespace Newtera.ParserGen.ParseTree
{
	using System;
	using System.Xml;
	using System.Xml.Serialization;
	using System.IO;
	using System.Text;
	using System.Collections;
	using System.Collections.Specialized;


	/// <summary>
	/// Represents the settings for transforming data in a parse tree form
	/// into a data grid form. The settings can be written to a file and restored
	/// from a file.
	/// </summary>
	/// <version>1.0.1 03 Dec. 2005</version>
	/// <author>Yong Zhang</author>
	public class DataGridSettings
	{
		private string _parserName;
		private StringCollection _rowNodes;
		private StringCollection _colDataNodes;
		private StringCollection _colNameNodes;

		[System.Xml.Serialization.XmlIgnoreAttribute]
		private Hashtable _rowNodesTable;

		[System.Xml.Serialization.XmlIgnoreAttribute]
		private Hashtable _colDataNodesTable;

		[System.Xml.Serialization.XmlIgnoreAttribute]
		private Hashtable _colNameNodesTable;

		private StringCollection _selfDefinedColNames;
		private bool _isSelfDefinedColName;
		private bool _isRowNodesTableInit;
		private bool _isColDataNodesTableInit;
		private bool _isColNameNodesTableInit;

		/// <summary>
		/// Initiating an instance of DataGridSettings class
		/// </summary>
		public DataGridSettings()
		{
			_parserName = null;
			_rowNodes = new StringCollection();
			_colDataNodes = new StringCollection();
			_colNameNodes = new StringCollection();
			_rowNodesTable = new Hashtable();
			_colDataNodesTable = new Hashtable();
			_colNameNodesTable = new Hashtable();
			_selfDefinedColNames = new StringCollection();
			_isSelfDefinedColName = false;

			_isRowNodesTableInit = false;
			_isColDataNodesTableInit = false;
			_isColNameNodesTableInit = false;
		}

		/// <summary>
		/// Gets the parser name
		/// </summary>
		public string ParserName
		{
			get
			{
				return _parserName;
			}
			set
			{
				_parserName = value;
			}
		}

		/// <summary>
		/// Gets a collection of parse tree node paths representing rows in data grid.
		/// </summary>
		/// <value>A ICollection instance.</value>
		public StringCollection RowNodes
		{
			get
			{
				return _rowNodes;
			}
		}

		/// <summary>
		/// Gets a collection of parse tree node paths representing column data in data grid.
		/// </summary>
		/// <value>A ICollection instance.</value>
		public StringCollection ColumnDataNodes
		{
			get
			{
				return _colDataNodes;
			}
		}

		/// <summary>
		/// Gets a collection of parse tree node paths representing column names in data grid.
		/// </summary>
		/// <value>A ICollection.</value>
		public StringCollection ColumnNameNodes
		{
			get
			{
				return _colNameNodes;
			}
		}

		/// <summary>
		/// Gets or sets the information indicating whether to use self-defined column
		/// names instead of those those the parse tree.
		/// </summary>
		public bool IsSelfDefinedColumnName
		{
			get
			{
				return this._isSelfDefinedColName;
			}
			set
			{
				this._isSelfDefinedColName = value;
			}
		}

		/// <summary>
		/// Gets or sets a collection of self-defined column names
		/// </summary>
		public StringCollection SelfDefinedColumnNames
		{
			get
			{
				return this._selfDefinedColNames;
			}
			set
			{
				this._selfDefinedColNames = value;
			}
		}

		/// <summary>
		/// Add a node path as row node
		/// </summary>
		/// <param name="path">node path</param>
		public void AddRowNode(string path)
		{
			if (!_isRowNodesTableInit)
			{
				InitializeHashtable(_rowNodesTable, _rowNodes);
				_isRowNodesTableInit = true;
			}

			if (path != null && !_rowNodes.Contains(path))
			{
				_rowNodes.Add(path);
				_rowNodesTable.Add(path, (_rowNodes.Count - 1) + "");
			}
		}

		/// <summary>
		/// Add a node path as col data node
		/// </summary>
		/// <param name="path">node path</param>
		public void AddColumnDataNode(string path)
		{
			if (!_isColDataNodesTableInit)
			{
				InitializeHashtable(_colDataNodesTable, _colDataNodes);
				_isColDataNodesTableInit = true;
			}

			if (path != null && !_colDataNodes.Contains(path) &&
				!_rowNodes.Contains(path))
			{
				_colDataNodes.Add(path);
				_colDataNodesTable.Add(path, (_colDataNodes.Count - 1) + "");
			}
		}

		/// <summary>
		/// Add a node path as col name node
		/// </summary>
		/// <param name="path">node path</param>
		public void AddColumnNameNode(string path)
		{
			if (!_isColNameNodesTableInit)
			{
				InitializeHashtable(_colNameNodesTable, _colNameNodes);
				_isColNameNodesTableInit = true;
			}

			if (path != null && !_colNameNodes.Contains(path) &&
				!_rowNodes.Contains(path) && !_colDataNodes.Contains(path))
			{
				_colNameNodes.Add(path);
				_colNameNodesTable.Add(path, (_colNameNodes.Count - 1) + "");
			}
		}

		/// <summary>
		/// Remove a node path from row nodes
		/// </summary>
		/// <param name="path">node path</param>
		public void RemoveRowNode(string path)
		{
			if (!_isRowNodesTableInit)
			{
				InitializeHashtable(_rowNodesTable, _rowNodes);
				_isRowNodesTableInit = true;
			}

			if (path != null && _rowNodes.Contains(path))
			{
				_rowNodes.Remove(path);
				_rowNodesTable.Clear();
				InitializeHashtable(_rowNodesTable, _rowNodes);
				_isRowNodesTableInit = true;
			}
		}

		/// <summary>
		/// Remove a node path from col data nodes
		/// </summary>
		/// <param name="path">node path</param>
		public void RemoveColumnDataNode(string path)
		{
			if (!_isColDataNodesTableInit)
			{
				InitializeHashtable(_colDataNodesTable, _colDataNodes);
				_isColDataNodesTableInit = true;
			}

			if (path != null && _colDataNodes.Contains(path))
			{
				_colDataNodes.Remove(path);
				_colDataNodesTable.Clear();
				InitializeHashtable(_colDataNodesTable, _colDataNodes);
				_isColDataNodesTableInit = true;
			}
		}

		/// <summary>
		/// Remove a node path from col name nodes
		/// </summary>
		/// <param name="path">node path</param>
		public void RemoveColumnNameNode(string path)
		{
			if (!_isColNameNodesTableInit)
			{
				InitializeHashtable(_colNameNodesTable, _colNameNodes);
				_isColNameNodesTableInit = true;
			}

			if (path != null && _colNameNodes.Contains(path))
			{
				_colNameNodes.Remove(path);
				_colNameNodesTable.Clear();
				InitializeHashtable(_colNameNodesTable, _colNameNodes);
				_isColNameNodesTableInit = true;
			}
		}

		/// <summary>
		/// Gets the information indicating whether the given node represents
		/// a row
		/// </summary>
		/// <param name="path">The node path</param>
		/// <returns>true if it represents a row node, false otherwise.</returns>
		public bool IsRowNode(string path)
		{
			bool status = false;

			if (!_isRowNodesTableInit)
			{
				InitializeHashtable(_rowNodesTable, _rowNodes);
				_isRowNodesTableInit = true;
			}

			if (path != null && _rowNodesTable[path] != null)
			{
				status = true;
			}
			
			return status;
		}

		/// <summary>
		/// Gets the information indicating whether the given node represents
		/// a col data
		/// </summary>
		/// <param name="path">The node path</param>
		/// <returns>true if it represents a col data node, false otherwise.</returns>
		public bool IsColumnDataNode(string path)
		{
			bool status = false;

			if (!_isColDataNodesTableInit)
			{
				InitializeHashtable(_colDataNodesTable, _colDataNodes);
				_isColDataNodesTableInit = true;
			}

			if (path != null && _colDataNodesTable[path] != null)
			{
				status = true;
			}
			
			return status;
		}

		/// <summary>
		/// Gets the column index of a parse tree path if it is a defined as column data.
		/// </summary>
		/// <param name="path">The parse tree path</param>
		/// <returns>A zero-based column index if it is a column data node, -1 if it is not a column data node.</returns>
		public int GetColumnDataIndex(string path)
		{
			int index = -1;

			if (!_isColDataNodesTableInit)
			{
				InitializeHashtable(_colDataNodesTable, _colDataNodes);
				_isColDataNodesTableInit = true;
			}
			
			string indexStr;
			if (path != null && (indexStr = (string) _colDataNodesTable[path]) != null)
			{
				index = Int32.Parse(indexStr);
			}
			
			return index;
		}

		/// <summary>
		/// Gets the information indicating whether the given node represents
		/// a col name
		/// </summary>
		/// <param name="path">The node path</param>
		/// <returns>true if it represents a col data name, false otherwise.</returns>
		public bool IsColumnNameNode(string path)
		{
			bool status = false;

			if (!_isColNameNodesTableInit)
			{
				InitializeHashtable(_colNameNodesTable, _colNameNodes);
				_isColNameNodesTableInit = true;
			}

			if (path != null && _colNameNodesTable[path] != null)
			{
				status = true;
			}
			
			return status;
		}

		/// <summary>
		/// Gets a copy of the DataGridSettings object
		/// </summary>
		/// <returns></returns>
		public DataGridSettings Copy()
		{
			DataGridSettings settings = new DataGridSettings();

			settings.ParserName = this.ParserName;

			foreach (string path in this.RowNodes)
			{
				settings.AddRowNode(path);
			}

			foreach (string path in this.ColumnDataNodes)
			{
				settings.AddColumnDataNode(path);
			}

			foreach (string path in this.ColumnNameNodes)
			{
				settings.AddColumnNameNode(path);
			}

			settings.IsSelfDefinedColumnName = this.IsSelfDefinedColumnName;

			foreach (string colName in this.SelfDefinedColumnNames)
			{
				settings.SelfDefinedColumnNames.Add(colName);
			}

			return settings;
		}

		/// <summary>
		/// Read a DataGridSettings instance from an XML file.
		/// </summary>
		/// <param name="fileName">the name of the XML file</param>
		/// <returns>The restored object</returns>
		/// <exception cref="ParseTreeException">ParseTreeException is thrown when it fails to
		/// read the XML file
		/// </exception>
		public static DataGridSettings Read(string fileName)
		{
			try
			{
				DataGridSettings settings = null;

				if (File.Exists(fileName))
				{
					//Open the stream and read model from it.
					using (FileStream fs = File.OpenRead(fileName)) 
					{
						// Create an instance of the XmlSerializer class;
						// specify the type of object to be deserialized.
						XmlSerializer serializer = new XmlSerializer(typeof(DataGridSettings));

						// If the XML document has been altered with unknown 
						// nodes or attributes, handle them with the 
						// UnknownNode and UnknownAttribute events.*/
						serializer.UnknownNode += new 
							XmlNodeEventHandler(SerializerUnknownNode);
						serializer.UnknownAttribute+= new 
							XmlAttributeEventHandler(SerializerUnknownAttribute);
   
						// Use the Deserialize method to restore the object's state with
						// data from the XML document. */
						settings = (DataGridSettings) serializer.Deserialize(fs);
					}
				}

				return settings;
			}
			catch (Exception ex)
			{
				throw new ParseTreeException("Failed to read the file :" + fileName, ex);
			}
		}

		/// <summary>
		/// Write the DataGridSettings to an XML file.
		/// </summary>
		/// <param name="fileName">The output file name.</param>
		/// <param name="settings">The object to write to a file</param>
		/// <exception cref="ParseTreeException">ParseTreeException is thrown when it fails to
		/// write to the file.
		/// </exception> 
		public static void Write(string fileName, DataGridSettings settings)
		{
			try
			{
				//Open the stream and read model from it.
				using (FileStream fs = File.Open(fileName, FileMode.Create)) 
				{
					// Create an instance of the XmlSerializer class;
					// specify the type of object to serialize.
					XmlSerializer serializer = new XmlSerializer(typeof(DataGridSettings));

					// Serialize the object
					serializer.Serialize(fs, settings);
				}
			}
			catch (System.IO.IOException ex)
			{
				throw new ParseTreeException("Failed to write to file :" + fileName, ex);
			}
		}

		protected static void SerializerUnknownNode
			(object sender, XmlNodeEventArgs e)
		{
			throw new ParseTreeException("Unknown Node:" +   e.Name + "\t" + e.Text);
		}

		protected static void SerializerUnknownAttribute
			(object sender, XmlAttributeEventArgs e)
		{
			System.Xml.XmlAttribute attr = e.Attr;
			throw new ParseTreeException("Unknown attribute " + 
				attr.Name + "='" + attr.Value + "'");
		}

		private void InitializeHashtable(Hashtable table, StringCollection values)
		{
			for (int i = 0; i < values.Count; i++)
			{
				table.Add(values[i], i.ToString());
			}
		}
	}
}