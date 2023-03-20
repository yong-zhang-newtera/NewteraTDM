/*
* @(#)DocumentDB.cs
*
* Copyright (c) 2003 Newtera, Inc. All rights reserved.
*
*/
namespace Newtera.Server.Engine.Vdom
{
	using System;
	using System.Xml;
	using System.Collections;
	using System.Text;
	using Newtera.Common.Core;
	using Newtera.Common.MetaData;
	using Newtera.Server.Engine.Cache;
	using Newtera.Server.Engine.Vdom;
	using Newtera.Server.Engine.Vdom.Common;
	using Newtera.Server.Engine.Vdom.Dbimp;
	using Newtera.Server.Engine.Interpreter;
	using Newtera.Server.Engine.Sqlbuilder;
	using Newtera.Server.Engine.Sqlbuilder.Sql;
    using Newtera.Common.MetaData.Schema.Generator;
    using Newtera.Server.DB;

	//using Newtera.Server.cache.MetaDataCacheMgr;

	/// <summary>
	/// An implementation of VDOM document for relational databases.
	/// </summary>
	/// <version>  	1.0.0 11 Nov 2003</version>
	/// <author>  Yong Zhang </author>
	internal class DocumentDB : VDocument
	{
		private MetaDataModel _metaData;
		private SQLBuilder _builder; // the sqlbuilder
		private SchemaInfo _schemaInfo; // the schema info
		private IDataProvider _dataProvider = null; // The DB provider
		private TreeManager _treeManager = null;
		private PagingExecutor _pagingExecutor = null;
        private Hashtable _virtualAttributeGenerators = new Hashtable();

		/// <summary>
		/// Initializing DocumentDB object.
		/// </summary>
		/// <param name="dataProvider">The db provider.</param>
		public DocumentDB(IDataProvider dataProvider) : base()
		{
			_dataProvider = dataProvider;
		}

		/// <summary>
		/// Return the meta data model of the document.
		/// </summary>
		/// <returns> meta data model </returns>
		public override MetaDataModel MetaData
		{
			get
			{
				return _metaData;
			}
			set
			{
				_metaData = value;
			}
		}

		/// <summary>
		/// Gets the schema information associated with the document.
		/// </summary>
		/// <value>The SchemaInfo object</value>
		public new SchemaInfo SchemaInfo
		{
			get
			{
				return _schemaInfo;
			}
		}

		/// <summary>
		/// Gets the information indicating whether the document is a db virtual document
		/// </summary>
		/// <returns>true if it is, false otherwise</returns>
		public override bool IsDB
		{
			get
			{
				return true;
			}
		}

        /// <summary>
        /// Gets information indicating whether a xml node represents a virtual attribute of a class
        /// </summary>
        /// <param name="xmlNode">The xml node</param>
        /// <returns>false always</returns>
        public override bool IsVirtualAttribute(XmlNode xmlNode)
        {
            // xmlNode.InnerText stores an unqiue ID for the virtual value generator
            if (xmlNode != null && _virtualAttributeGenerators[xmlNode.InnerText] != null)
            {
                return true;
            }
            else
            {
                return false;
            }
        }

        /// <summary>
        /// Obtains the value of a virtual attribute represented by a xml node
        /// </summary>
        /// <param name="xmlNode">The xml node</param>
        /// <returns>null</returns>
        public override string ObtainVirualAttributeValue(XmlNode xmlNode)
        {
            // xmlNode.InnerText stores an unqiue ID for the virtual value generator
            if (xmlNode != null && _virtualAttributeGenerators[xmlNode.InnerText] != null)
            {
                try
                {
                    VirtualAttributeValueGeneratorContext vContext = (VirtualAttributeValueGeneratorContext)_virtualAttributeGenerators[xmlNode.InnerText];

                    IFormula formula = (IFormula)vContext.Formular;
                    InstanceElementWrapper wrapper = vContext.Wrapper;
                    ServerExecutionContext context = new ServerExecutionContext();

                    return formula.Execute(wrapper, context);
                }
                catch (Exception ex)
                {
                    ErrorLog.Instance.WriteLine("Failed to obtain value for virtual attribute " + xmlNode.Name + " with error : " + ex.Message + "\n" + ex.StackTrace);
                    return "VirtualAttributeError";
                }
            }
            else
            {
                return "";
            }
        }

        /// <summary>
        /// Keep a VirtualAttributeValueGeneratorContext in the document with an unique id
        /// </summary>
        /// <param name="id">unique id</param>
        /// <param name="vContext">Virtual Attribute Value GeneratorContext</param>
        public override void SetVirtualValueGeneratorContext(string id, VirtualAttributeValueGeneratorContext vContext)
        {
            _virtualAttributeGenerators[id] = vContext;
        }

        /// <summary>
        /// Return a data provider associated with the document.
        /// </summary>
        /// <value>A DataProvider</value>
        public override IDataProvider DataProvider
		{
			get
			{
				return this._dataProvider;
			}
		}
		
		/// <summary>
		/// This method is called after all PrepareNodes() calls are done and at the time
		/// the first SelectNodes() call is made. It gives the DocumentDB object a chance
		/// to build SQL statements, execute the statements, and create a XML document for
		/// further processing.
		/// </summary>
		public override void Initialize()
		{	
			_entityTable = new Hashtable();
			
			QueryInfo queryInfo = _treeManager.QueryInfo;
			
			IsLoaded = true;

			/*
			* if a query is already a function query, we do not turn it into a count
			* even through the flag is true.
			*/
			if (this.IsForCount && !queryInfo.IsForFunction)
			{
				CountExecutor executor = new CountExecutor(_metaData, _dataProvider, _builder, this.Interpreter);
				
				/*
				* CountExecutor builds a count sql, get count from the result,
				* set the count value to the interprter
				*/
				executor.Execute(queryInfo);
			}
            else if (this.IsForClassNames)
            {
                SearchClassNameExecutor executor = new SearchClassNameExecutor(_metaData, _dataProvider, _builder, this.Interpreter);

                /*
                * SearchClassNameExecutor builds a sql that gets leaf class names from the result,
                * set a list of class names to the interprter
                */
                executor.Execute(queryInfo);
            }
            else if (!this.IsPaging)
            {
                if (!Interpreter.IsForSQLActions)
                {
                    SearchExecutor executor = new SearchExecutor(_metaData, _dataProvider, _builder, this, _entityTable);
                    executor.OmitArrayData = this.OmitArrayData;
                    executor.DelayVirtualAttributeCalculation = this.DelayVirtualAttributeCalculation;
                    executor.CheckReadPermissionOnly = this.CheckReadPermissionOnly;
                    executor.ShowEncryptedData = this.ShowEncryptedData;
                    executor.ObtainCachedObjId = this.ObtainCachedObjId;

                    /*
                    * SearchExecutor handles all the details of building SQL statements,
                    * executing the SQLs, convert the query result into XML elements, and
                    * add elements to the XML document
                    */
                    executor.Execute(queryInfo);
                }
                else
                {
                    GetSearchStatementExecutor executor = new GetSearchStatementExecutor(_metaData, _dataProvider, _builder, this, _entityTable);

                    // save the sql actions on the interpreter so that they will be picked up
                    Interpreter.SQLActions = executor.Execute(queryInfo);
                }
            }
            else
            {
                // gets the result in paging mode
                if (_pagingExecutor == null)
                {
                    _pagingExecutor = new PagingExecutor(_metaData, _dataProvider, _builder, this, _entityTable);
                    _pagingExecutor.PageSize = this.PageSize;
                    _pagingExecutor.OmitArrayData = this.OmitArrayData;
                    _pagingExecutor.DelayVirtualAttributeCalculation = this.DelayVirtualAttributeCalculation;
                    _pagingExecutor.CheckReadPermissionOnly = this.CheckReadPermissionOnly;
                    _pagingExecutor.ShowEncryptedData = this.ShowEncryptedData;
                    _pagingExecutor.ObtainCachedObjId = this.ObtainCachedObjId;
                }

                /*
                * Instantiate the XmlDocument with the first page of result data
                */
                _pagingExecutor.Execute(queryInfo);
            }
		}
		
		/// <summary>
		/// DocumentDB load data on demand, therefore, this method only establish the
		/// data provider with a database schema specified by url and get the meta data model
		/// for the schema.
		/// </summary>
		/// <param name="url">the url of document.</param>
		/// <remarks>
		/// The url for a DB source may look like db://mySchema.xml?version=1.0
		/// where ?version=1.0 part is optional
		/// </remarks>
		public override void Load(string url)
		{
			this.URL = url;
			
			//get the meta data model from the cache mgr
			DocumentFactory docFactory = DocumentFactory.Instance;
			_schemaInfo = docFactory.GetSchemaInfo(url);
			
			MetaDataCache metaDataCache = MetaDataCache.Instance;
			_metaData = metaDataCache.GetMetaData(_schemaInfo, _dataProvider);
			
			_treeManager = new TreeManager(_metaData, this);
			
			_builder = new SQLBuilder(_metaData, _dataProvider);

			// Create root element of the document
			CreateDocumentElement();
		}

		/// <summary>
		/// Cleanup the resources used by the document, such as database connection.
		/// </summary>
		public override void Close()
		{
			if (_pagingExecutor != null)
			{
				_pagingExecutor.Close(); // release the database connection held by the PageExecutor
				_pagingExecutor = null;
			}
		}

		/// <summary>
		/// Clear the content of the DocumentDB
		/// </summary>
		public override void ClearContent()
		{
			this.IsLoaded = false;

			this.DocumentElement.RemoveAll();
		}
		
		/// <summary>
		/// This method is called during the prepare phase of interprter. For database-based
		/// document, the prepareNodes call is to notify the DocumentDB to prepare for retrieving
		/// the corresponding data identified by the path.
		/// 
		/// DocumentDB takes a path and construct entity objects (such as
		/// ClassEnity, AttributeEntity, and RelationshipEntity) in a way specified by the
		/// path. Once the prepare phase of interprter is completed, the
		/// DocumentDB will have a tree structure containing all entities that are involved
		/// in a query. This structure is passed to SQLbuilder to generate SQL
		/// statements for retrieving the set of data from a database.
		/// </summary>
		/// <param name="path">the path that specifies the nodes to be prepared</param>
		public override void PrepareNodes(PathEnumerator pathEnumerator)
		{
			//System.Console.WriteLine("PrepareNode on " + pathEnumerator);

			// skip the prepare if the documnet is already loaded
			if (!IsLoaded)
			{
				/*
				* Create new entities for steps in the path that have not have their
				* corresponding entities created in the entity tree, and add them to
				* the entity tree.
				*/
				_treeManager.GrowTree(pathEnumerator);
			}
		}
		
		/// <summary>
		/// This method is called during the restrict phase of interprter. For database-based
		/// document, the doPrepareQualifier call is to let the document build these entities
		/// for the WHERE clause in a SQL statement.
		/// </summary>
		/// <param name="qualifier">the interpreter's IExpr qualifier.</param>
		public override void PrepareQualifier(IExpr qualifier)
		{
			if (!IsLoaded)
			{
				_treeManager.PrepareQualifier(qualifier);
			}
		}
		
		/// <summary>
		/// This method is called during the prepare phase of interprter. It prepares
		/// a function. This method is to give the document a chance to do something special
		/// for the function.
		/// </summary>
		/// <param name="func">the function</param>
		/// <param name="params">the list of parameters.</param>
		public override void PrepareFunction(IDBFunction func, ExprCollection parameters)
		{
			if (!IsLoaded)
			{
				_treeManager.PrepareFunction(func, parameters);
			}
		}
		
		/// <summary>
		/// This method is called during the restrict phase of interprter. For database-based
		/// document, the doPrepareSortBy call is to let the document build GROUP BY clause
		/// for a SQL.
		/// </summary>
		/// <param name="sortBy">the sort by spec</param>
		public override void PrepareSortBy(SortbySpec sortBy)
		{
			if (!IsLoaded)
			{
				_treeManager.PrepareSortBy(sortBy);
			}
		}
		
		/// <summary>
		/// This method is called by built-in functions to insert instance nodes into
		/// a document.
		/// </summary>
		/// <param name="instanceNodes">the instance nodes to be inserted.</param>
		/// <param name="interpreter">the interpreter to run the xqueries.</param>
		/// <returns> a string consisting of obj_id(s) created for the inserted instances.</returns>
		public override string InsertNodes(ValueCollection instanceNodes, Interpreter interpreter)
		{
            string objIds = string.Empty;
            if (!Interpreter.IsForSQLActions)
            {
                InsertExecutor executor = new InsertExecutor(_metaData, _dataProvider, _builder, this, interpreter);
                executor.ObtainCachedObjId = this.ObtainCachedObjId;
                executor.NeedToRaiseEvents = this.NeedToRaiseEvents;

                objIds = executor.Execute(instanceNodes);

                // Raise the insert events, this call will return immediately
                executor.RaiseEvents();
            }
            else
            {
                GetInsertSQLActionsExecutor executor = new GetInsertSQLActionsExecutor(_metaData, _dataProvider, _builder, this, interpreter);

                // save the sql actions on the interpreter so that they will be picked up
                Interpreter.SQLActions = executor.Execute(instanceNodes);
            }

            return objIds;
		}
		
		/// <summary>
		/// This method is called by built-in functions to delete instance nodes from
		/// a document.
		/// </summary>
		/// <param name="instanceNodes">the instance nodes to be deleted.</param>
		/// <param name="interpreter">the interpreter to run the xqueries.</param>
		/// <returns> a string consisting of obj_ids of deleted instances.</returns>
		public override string DeleteNodes(ValueCollection instanceNodes, Interpreter interpreter)
		{
			DeleteExecutor executor = new DeleteExecutor(_metaData, _dataProvider, _builder, this, interpreter);
            executor.NeedToRaiseEvents = this.NeedToRaiseEvents;

			string objIds = executor.Execute(instanceNodes);

            // Raise the insert events, this call will return immediately
            executor.RaiseEvents();

            return objIds;
		}
		
		/// <summary>
		/// This method is called by built-in functions to update instances in
		/// a document.
		/// </summary>
		/// <param name="instanceNodes">the instance nodes to be updated.</param>
        /// <param name="interpreter">the interpreter to run the xqueries.</param>
		/// <returns> a string consisting of obj_ids of updated instances.</returns>
        public override string UpdateNodes(ValueCollection instanceNodes, Interpreter interpreter)
		{
            UpdateExecutor executor = new UpdateExecutor(_metaData, _dataProvider, _builder, this, interpreter);
            executor.ObtainCachedObjId = this.ObtainCachedObjId;
            executor.NeedToRaiseEvents = this.NeedToRaiseEvents;

			string objIds = executor.Execute(instanceNodes);

            // Raise the insert events, this call will return immediately
            executor.RaiseEvents();

            return objIds;
		}
				
		/// <summary>
		/// Create the root element of the document.
		/// </summary>
		private void CreateDocumentElement()
		{
			StringBuilder builder = new StringBuilder();

			// The document element has the schema name as its name
			builder.Append("<?xml version='1.0' encoding='Unicode'?>");
			builder.Append("<").Append(this.SchemaInfo.Name).Append(">");
			builder.Append("</").Append(this.SchemaInfo.Name).Append(">");

			this.LoadXml(builder.ToString());
		}
	}
}