/*
* @(#)CreateDataColumnVisitor.cs
*
* Copyright (c) 2005 Newtera, Inc. All rights reserved.
*
*/
namespace Newtera.ParserGen.ParseTree
{
	using System;
	using System.Data;
	using System.Collections.Specialized;

	/// <summary>
	/// Represents a visitor that traverse a parse tree and
	/// create data columns in a System.Data.DataTable. 
	/// </summary>
	/// <version> 1.0.0 03 Dec 2005 </version>
	/// <author> Yong Zhang</author>
	public class CreateDataColumnVisitor : IParseTreeNodeVisitor
	{
		private DataSet _dataSet = null;
		private DataTable _currentTable = null;
		private DataGridSettings _settings = null;
		private StringCollection _colNames;
		private int _maxColCount; // keep the max number of column count
		private int _colCount; // col count of a data row

		/// <summary>
		/// Instantiate an instance of CreateDataColumnVisitor class
		/// </summary>
		/// <param name="dataSet">The dataset to be filled.</param>
		/// <param name="settings">The specs for conversions</param>
		public CreateDataColumnVisitor(DataSet dataSet, DataGridSettings settings)
		{
			_dataSet = dataSet;
			_currentTable = dataSet.Tables[0];
			_settings = settings;

			_colNames = new StringCollection();
			_maxColCount = 0;
			_colCount = 0;
		}

		/// <summary>
		/// Create data columns in the given Data Table instance
		/// </summary>
		public void CreateDataColumns()
		{
			// get the max column count in case the last row has maxmum column count
			if (_colCount > _maxColCount)
			{
				_maxColCount = _colCount;
			}

			string colName;
			for (int i = 0; i < _maxColCount; i++)
			{
				colName = null;
				// get column name
				if (_settings.IsSelfDefinedColumnName)
				{
					// use self-defined column
					if (i < _settings.SelfDefinedColumnNames.Count)
					{
						colName = _settings.SelfDefinedColumnNames[i];
					}
				}
				else
				{
					// use parse tree column
					if (i < _colNames.Count)
					{
						colName = _colNames[i];
						if (_currentTable.Columns.Contains(colName))
						{
							colName += "_" + i;
						}
					}
				}

				if (colName == null)
				{
					// assign a default column name
					colName = "Col_" + i;
				}

				_currentTable.Columns.Add(colName);
			}
		}

		/// <summary>
		/// Viste a rule node.
		/// </summary>
		/// <param name="node">A ParseTreeRuleNode instance</param>
		/// <returns>true to contibute visiting nested nodes, false to stop</returns>
		public bool VisitRule(ParseTreeRuleNode node)
		{
			string path = node.ToPath();			

			if (this._settings.IsColumnNameNode(path))
			{
				if (!_colNames.Contains(node.Text))
				{
					_colNames.Add(node.Text);
				}
			}
			else if (this._settings.IsRowNode(path))
			{
				// start a new row, keep the colun count for previous row
				if (this._colCount > this._maxColCount)
				{
					this._maxColCount = this._colCount;
				}

				this._colCount = 0;
			}
			else if (this._settings.IsColumnDataNode(path))
			{
				this._colCount++;
			}

			return true;
		}

		/// <summary>
		/// Viste a token node.
		/// </summary>
		/// <param name="node">A ParseTreeTokenNode instance</param>
		/// <returns>true to contibute visiting nested nodes, false to stop</returns>
		public bool VisitToken(ParseTreeTokenNode node)
		{
			string path = node.ToPath();			

			if (this._settings.IsColumnNameNode(path))
			{
				if (!_colNames.Contains(node.Text))
				{
					_colNames.Add(node.Text);
				}
			}
			else if (this._settings.IsColumnDataNode(path))
			{
				this._colCount++;
			}

			return true;
		}

		/// <summary>
		/// Viste a literal node.
		/// </summary>
		/// <param name="node">A ParseTreeLiteralNode instance</param>
		/// <returns>true to contibute visiting nested nodes, false to stop</returns>
		public bool VisitLiteral(ParseTreeLiteralNode node)
		{
			string path = node.ToPath();			

			if (this._settings.IsColumnNameNode(path))
			{
				if (!_colNames.Contains(node.Text))
				{
					_colNames.Add(node.Text);
				}
			}
			else if (this._settings.IsColumnDataNode(path))
			{
				this._colCount++;
			}

			return true;
		}
	}
}