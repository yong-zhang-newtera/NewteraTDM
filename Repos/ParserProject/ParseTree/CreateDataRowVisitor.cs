/*
* @(#)CreateDataRowVisitor.cs
*
* Copyright (c) 2005 Newtera, Inc. All rights reserved.
*
*/
namespace Newtera.ParserGen.ParseTree
{
	using System;
	using System.Data;

	/// <summary>
	/// Represents a visitor that traverse a parse tree and
	/// create data rows in a System.Data.DataTable. 
	/// </summary>
	/// <version> 1.0.0 03 Dec 2005 </version>
	/// <author> Yong Zhang</author>
	public class CreateDataRowVisitor : IParseTreeNodeVisitor
	{
		private DataSet _dataSet = null;
		private DataTable _currentTable = null;
		private DataGridSettings _settings = null;
		private DataRow _currentRow = null;
		private int _colIndex;

		/// <summary>
		/// Instantiate an instance of CreateDataRowVisitor class
		/// </summary>
		/// <param name="dataSet">The dataset to be filled.</param>
		/// <param name="settings">The specs for conversions</param>
		public CreateDataRowVisitor(DataSet dataSet, DataGridSettings settings)
		{
			_dataSet = dataSet;
			_currentTable = dataSet.Tables[0];
			_settings = settings;
			_colIndex = 0;
		}

		/// <summary>
		/// Gets the data set converted
		/// </summary>
		/// <value>A DataSet object.</value>
		public DataSet DataSet
		{
			get
			{
				return _dataSet;
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

			if (this._settings.IsRowNode(path))
			{
				_currentRow = _currentTable.NewRow();
				_currentTable.Rows.Add(_currentRow);
				_colIndex = 0;
			}
			else if (this._settings.IsColumnDataNode(path))
			{
				if (_currentRow != null)
				{
					_currentRow[_colIndex++] = node.Text;
				}
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

			if (this._settings.IsColumnDataNode(path))
			{
				if (_currentRow != null)
				{
					_currentRow[_colIndex++] = node.Text;
				}
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

			if (this._settings.IsColumnDataNode(path))
			{
				if (_currentRow != null)
				{
					_currentRow[_colIndex++] = node.Text;
				}
			}

			return true;
		}
	}
}