/*
* @(#)QueryReader.cs
*
* Copyright (c) 2006 Newtera, Inc. All rights reserved.
*
*/
namespace Newtera.Server.Engine.Interpreter
{
	using System;
	using System.Xml;
	using System.Collections;

	using Newtera.Server.Engine.Vdom;

	/// <summary>
	/// Represents a forward-only query reader that returns xml result in pages.
	/// </summary>
	/// <version> 1.0.0 08 Jul 2006 </version>
	/// <author> Yong Zhang</author>
	public class QueryReader
	{
		private Interpreter _interpreter;
		private IExpr _parsedQuery;
		private ArrayList _docList;
        private bool _isEndOfQueryResult;

		/// <summary>
		/// Initiate an instance of QueryReader object.
		/// </summary>
		/// <param name="parsedQuery">The parsed query in IExpr instance</param>
		/// <param name="interpreter">The interpreter</param>
		public QueryReader(IExpr parsedQuery, Interpreter interpreter)
		{
			_parsedQuery = parsedQuery;
			_interpreter = interpreter;
			_docList = null;
            _isEndOfQueryResult = false;
		}

		/// <summary>
		/// Gets the result of next page
		/// </summary>
		/// <returns>A XmlDocument containing the results.</returns>
		public XmlDocument GetNextPage()
		{
            if (_isEndOfQueryResult)
            {
                return null;
            }

			// Clear the content of the VDocument(s) so that they will be loaded
			// with the content representing the data of next page when Eval()
			// method is called upon.
			ArrayList docList = GetVDocuments();
			foreach (VDocument vdoc in docList)
			{
                if (vdoc.IsDB)
                {
                    vdoc.ClearContent();
                }
                else
                {
                    _isEndOfQueryResult = true; // the regular xml doc does not support paging, GetNextPage is called only once
                }
			}

			XNode result = (XNode) _parsedQuery.Eval();

			return (XmlDocument) result.Content;
		}

		/// <summary>
		/// Cleanup the resource used by the query reader
		/// </summary>
		public void Close()
		{
			// close the database connection used by the VDocument(s)
			ArrayList docList = GetVDocuments();
			foreach (VDocument vdoc in docList)
			{
				vdoc.Close();
			}
		}

		/// <summary>
		/// Get a list of VDocument object associated with the parsed query
		/// </summary>
		/// <returns>A ArrayList of VDocument objects</returns>
		private ArrayList GetVDocuments()
		{
			if (_docList == null)
			{
				_docList = new ArrayList();

				Value val = ((ITraceable) _parsedQuery).TraceDocument();
				if (val != null)
				{
					if (val is XCollection)
					{
						ValueCollection docs = val.ToCollection();

						// TODO, this may have problems with multiple documents
						foreach (XNode node in docs)
						{
							_docList.Add(node.ToNode());
						}
					}
					else
					{
						_docList.Add(val.ToNode());
					}
				}
			}

			return _docList;
		}
	}
}