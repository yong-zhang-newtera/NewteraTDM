/*
* @(#)VDocument.cs
*
* Copyright (c) 2003 Newtera, Inc. All rights reserved.
*
*/
namespace Newtera.Server.Engine.Vdom
{
	using System;
	using System.Xml;
	using System.Text.RegularExpressions;
	using System.Collections;
	using System.Collections.Specialized;
	using Newtera.Server.DB;
	using Newtera.Server.Engine.Vdom.Common;
	using Newtera.Common.MetaData;
	using Newtera.Server.Engine.Interpreter;
	using Newtera.Server.Engine.Sqlbuilder;
	using Newtera.Server.Engine.Sqlbuilder.Sql;
	using Newtera.Common.MetaData.Schema;
	
	/// <summary>
	/// This is an abstract class that provides default implementation for a vdom document.
	/// A document object is created when interpreter processes a document("name.xml")
	/// function. Document defines behavior for a Virtual XML Document. Methods allow
	/// the user to the root element as well as processing instructions and other
	/// document-level information. Plus the methods for preparing and navigating document
	/// nodes. In pass one, Interpreter tells a document what nodes are asked in a query
	/// in term of path expressions, and the document prepares it. In pass two, interpreter
	/// will be able to get XNode or ValueCollection objects from the document.
	/// </summary>
	/// <version>1.0.0 11 Jul 2003</version>
	/// <author>Yong Zhang</author>
	public abstract class VDocument : XmlDocument
	{	
		private bool _isLoaded = false;
        private bool _hasDuplicatedRootElements = false;
		private IList _emptyList;
		protected Hashtable _entityTable = null;
		protected string _docURL = null;
		
		private Interpreter _interpreter;

		/*
		* Hack, the interpreter member is added to get around problem of getting count
		* for a FLWR query.
		*/

		/// <summary>
		/// Initializing a VDocument object.
		/// </summary>
		public VDocument() : base()
		{
			_emptyList = new ArrayList(); // to provide a null enumerator
		}

		/// <summary>
		/// Gets the document itself
		/// </summary>
		/// <value>A VDocument instance</value>
		public override XmlDocument OwnerDocument
		{
			get
			{
				return this;
			}
		}

		/// <summary>
		/// Gets the information indicating whether the document is a db virtual document
		/// </summary>
		/// <returns>true if it is, false otherwise</returns>
		public abstract bool IsDB
		{
			get;
		}

        /// <summary>
        /// Gets information indicating whether a xml node represents a virtual attribute of a class
        /// </summary>
        /// <param name="xmlNode">The xml node</param>
        /// <returns>true if it is a virtual attribute, false otherwise</returns>
        public abstract bool IsVirtualAttribute(XmlNode xmlNode);

        /// <summary>
        /// Obtains the value of a virtual attribute represented by a xml node
        /// </summary>
        /// <param name="xmlNode">The xml node</param>
        /// <returns>The value of the virtual attribute, or null</returns>
        public abstract string ObtainVirualAttributeValue(XmlNode xmlNode);

        /// <summary>
        /// Keep a VirtualAttributeValueGeneratorContext in the document with an unique id
        /// </summary>
        /// <param name="id">unique id</param>
        /// <param name="vContext">Virtual Attribute Value GeneratorContext</param>
        public abstract void SetVirtualValueGeneratorContext(string id, VirtualAttributeValueGeneratorContext vContext);

        /// <summary>
        /// Gets or sets the information indicating whether the document has duplicated root elements
        /// which are the direct elements under the document element
        /// elements.
        /// </summary>
        public bool HasDuplicatedRootElements
        {
            get
            {
                return _hasDuplicatedRootElements;
            }
            set
            {
                _hasDuplicatedRootElements = value;
            }
        }

		/// <summary>
		/// Overring this method so that XmlAttribute's owner document is a VDocument type.
		/// </summary>
		/// <param name="prefix">The prefix of the attribute (if any). String.Empty and a null reference are equivalent.</param>
		/// <param name="localName">The local name of the attribute</param>
		/// <param name="namespaceURI">The namespace URI of the attribute (if any). String.Empty and a null reference are equivalent. If prefix is xmlns, then this parameter must be http://www.w3.org/2000/xmlns/; otherwise an exception is thrown. </param>
		/// <returns>The new XmlAttribute</returns>
		public override XmlAttribute CreateAttribute(string prefix, string localName, string namespaceURI)
		{
			return base.CreateAttribute(prefix, localName, namespaceURI);
		}

		/// <summary>
		/// Overring this method so that XmlElement's owner document is a VDocument type.
		/// </summary>
		/// <param name="prefix">The prefix of the new element (if any). String.Empty and a null reference are equivalent.</param>
		/// <param name="localName">The local name of the new element.</param>
		/// <param name="namespaceURI">The namespace URI of the new element (if any). String.Empty and a null reference are equivalent.</param>
		/// <returns></returns>
		public override XmlElement CreateElement(string prefix, string localName, string namespaceURI)
		{
			return base.CreateElement(prefix, localName, namespaceURI);
		}

		/// <summary>
		/// Gets the DBEntity object for the node.
		/// </summary>
		/// <param name="entityTable">the hash table that contains entities for all the nodes in the document.
		/// </param>
		/// <returns> Return null.</returns>
		public DBEntity GetEntity(Hashtable entityTable)
		{
			return null;
		}

		/// <summary>
		/// Clear the content of the VDocument
		/// </summary>
		public virtual void ClearContent()
		{
			// do nothing by default
		}

		/// <summary>
		/// Cleanup the resources used by the document, such as database connection.
		/// It is intended to be overried by the subclass
		/// </summary>
		public virtual void Close()
		{
			// do nothing by default
		}

		/// <summary>
		/// This method is to be overrided by the subclasses to prepare the nodes
		/// referred by the xpath.
		/// </summary>
		/// <param name="pathEnumerator">
		/// the path enumerator that provide path and predicate information.
		/// </param>
		public abstract void PrepareNodes(PathEnumerator pathEnumerator);

		/// <summary>
		/// This method is called during the evaluation phase of interprter.
		/// This method selects a ValueCollection based on the current node as a context and a xpath
		/// provided as an argument. It returns a ValueCollection that contains a list of XmlNode that
		/// interpreter can iterate through. This ValueCollection returned can be used by the interprter as
		/// a context node from which to select a new ValueCollection.
		/// </summary>
		/// <param name="contextNode">the node as a context node</param>  
		/// <param name="pathEnumerator">the path enumerator that provide path and qualifier information.</param>
		/// <returns>the ValueCollection representing the nodes selected by the path.</returns>
		public ValueCollection SelectNodes(XmlNode contextNode, PathEnumerator pathEnumerator)
		{
			ValueCollection nodeCollection;
			
			pathEnumerator.Reset();
			switch (contextNode.NodeType)
			{
				case XmlNodeType.Document:
					nodeCollection = ((VDocument) contextNode).SelectNodes(pathEnumerator);
					break;
				case XmlNodeType.Attribute:
				case XmlNodeType.Element:

					/*
					* iterateSteps requires the first parameter to be a ValueCollection object, so we
					* make a single item ValueCollection containing the root element.
					*/
					nodeCollection = new ValueCollection();
					nodeCollection.Add(new XNode(contextNode));
			
					// iterate through the path step by step to get the final list of nodes
					PathNavigator navigator = new PathNavigator();
					nodeCollection = navigator.Navigate(nodeCollection, pathEnumerator);
					break;
				default:
					throw new UnsupportedFeatureException("The node type " + contextNode.NodeType + " is not supported by SelectNodes.");
			}

			return nodeCollection;
		}

		/// <summary>
		/// This method selects a ValueCollection based on the current node as a context and a xpath
		/// provided as an argument. It returns a ValueCollection that contains a list of XmlNode that
		/// interpreter can iterate through.
		/// </summary>
		/// <param name="path">the path enumerator that provide path and qualifier information.</param>
		/// <returns>the ValueCollection representing the nodes selected by the path.</returns>
		public ValueCollection SelectNodes(PathEnumerator pathEnumerator)
		{
			/*
			 * this call allows DocumentDB to build SQL statements, execute the
			 * statements, and create a XML document at the first call to SelectNodes()
			 * method. This call is no-op for DocumentText.
			 */
			if (!IsLoaded)
			{
				Initialize();
			}
			
			/*
			* iterateSteps requires the first parameter to be a ValueCollection object, so we
			* make a single item ValueCollection containing the root element.
			*/
			ValueCollection nodeCollection = new ValueCollection();
			nodeCollection.Add(new XNode(this));
			
			// iterate through the path step by step to get the final list of nodes
			PathNavigator navigator = new PathNavigator();
			pathEnumerator.Reset();
			return navigator.Navigate(nodeCollection, pathEnumerator);
		}

		/// <summary>
		/// This method is called during the restrict phase of interprter. It prepares
		/// a qualifier in a path expression. This method is to give the document a
		/// chance to filter out the nodes that do not qualified.
		/// </summary>
		/// <param name="contextNode">The context node</param>
		/// <param name="qualifier">the interpreter's IExpr object</param>
		public virtual void PrepareQualifier(XmlNode contextNode, IExpr qualifier)
		{
			if (contextNode.NodeType == XmlNodeType.Document)
			{
				((VDocument) contextNode).PrepareQualifier(qualifier);
			}
		}

		/// <summary>
		/// This method is to be overrided by the subclasses
		/// </summary>
		/// <param name="qualifier">the interpreter's IExpr object</param>
		public abstract void PrepareQualifier(IExpr qualifier);

		/// <summary>
		/// This method is to be overrided by the subclasses
		/// </summary>
		/// <param name="func">the function</param>
		/// <param name="args">a list of function arguments </param>
		public abstract void PrepareFunction(IDBFunction func, ExprCollection args);

		/// <summary>
		/// This method is called during the restrict phase of interprter. It let
		/// the document to prepare for the sorting
		/// </summary>
		/// <param name="contextNode">The context node</param> 
		/// <param name="sortby">the spec of a sortby </param>
		public virtual void PrepareSortBy(XmlNode contextNode, SortbySpec sortBy)
		{
			if (contextNode.NodeType == XmlNodeType.Document)
			{
				((VDocument) contextNode).PrepareSortBy(sortBy);
			}		
		}

		/// <summary>
		/// This method is to be overrided by the subclasses.
		/// </summary>
		/// <param name="sortby">the spec of a sortby </param>
		public abstract void PrepareSortBy(SortbySpec sortBy);

		/// <summary>
		/// This method is called by built-in functions to insert instance nodes into
		/// a document. Throw an exception by default. VDocument subclasses override it.
		/// </summary>
		/// <param name="instanceNodes">the instance nodes to be inserted.
		/// </param>
		/// <param name="interpreter">the interpreter to run the xqueries.
		/// </param>
		/// <returns>
		/// a string consisting of obj_id(s) created for the inserted instances.
		/// </returns>
		public virtual string InsertNodes(ValueCollection instanceNodes, Interpreter interpreter)
		{
			// subclass can override it
			return string.Empty;
		}

		/// <summary>
		/// This method is called by built-in functions to delete instance nodes from
		/// a document. Throw an exception by default. VDocument subclasses override it.
		/// </summary>
		/// <param name="instanceNodes">the instance nodes to be deleted.</param>
		/// <param name="interpreter">the interpreter to run the xqueries.</param>
		/// <returns>
		/// A string consisting of obj_ids of deleted instances
		/// </returns>
		public virtual string DeleteNodes(ValueCollection instanceNodes, Interpreter interpreter)
		{
			// subclass can override it
			return string.Empty;
		}

		/// <summary>
		/// This method is called by built-in functions to update instances in
		/// a document. Throw an exception by default. VDocument subclasses override it.
		/// </summary>
		/// <param name="instanceNodes">the instance nodes containing data to be updated.</param>
        /// <param name="interpreter">the interpreter to run the xqueries.</param>
		/// <returns>
		/// A string consisting of obj_ids of updated instances
		/// </returns>
        public virtual string UpdateNodes(ValueCollection instanceNodes, Interpreter interpreter)
		{
			// subclass can override it
			return string.Empty;
		}

		/// <summary>
		/// Gets the url of document
		/// </summary>
		/// <value> document url string.</value>
		public string URL
		{
			get
			{
				return _docURL;
			}
			set
			{
				_docURL = value;
			}
		}

		/// <summary>
		/// Gets the information indicating whether the document is loaded.
		/// </summary>
		/// <value> true if it is loaded, false otherwise.</value>
		public bool IsLoaded
		{
			get
			{
				return _isLoaded;
			}
			set
			{
				_isLoaded = value;
			}
		}

		/// <summary>
		/// Implements the delegate of value change event, defined in Ident.cs.
		/// This is invoked when value of identifier is changed which will make the
		/// content of this documnet outdated.
		/// </summary>
		/// <param name="identName">name of identifier</param>
		/// <param name="newVal">The new value object</param>
		public void ValueChangedHandler(string identName, Value newVal)
		{
			// set IsLoaded to false so that document will be reloaded next time
			_isLoaded = false;
		}

		/// <summary>
		/// Gets the information indicating whether the document is created
		/// for getting a count.
		/// </summary>
		/// <value> true if it is for getting a count, false otherwise.</value>
		public bool IsForCount
		{
			get
			{
				return _interpreter.IsForCount;
			}
		}

        /// <summary>
        /// Gets the information indicating whether the document is created
        /// for getting a list of class names as search result.
        /// </summary>
        /// <value> true if it is for getting a count, false otherwise.</value>
        public bool IsForClassNames
        {
            get
            {
                return _interpreter.IsForClassNames;
            }
        }

        /// <summary> 
        /// Gets the information indicating whether the interpreter
        /// is used for executing a nested query for a virtual attribute, for example
        /// </summary>
        /// <returns> true if it is executing a nested query , false otherwise</returns>
        public bool IsNestedQuery
        {
            get
            {
                return _interpreter.IsNestedQuery;
            }
        }

		/// <summary>
		/// Gets or sets the information indicating whether the result of a query
		/// is obtained in paging mode.
		/// </summary>
		/// <value> true if it is for paging, false otherwise.</value>
		public bool IsPaging
		{
			get
			{
				return _interpreter.IsPaging;
			}
		}

		/// <summary>
		/// Gets or sets the page size
		/// </summary>
		public int PageSize
		{
			get
			{
				return _interpreter.PageSize;
			}
		}

        /// <summary>
        /// Gets information indicates whether to raise events for update queries
        /// </summary>
        public bool NeedToRaiseEvents
        {
            get
            {
                return _interpreter.NeedToRaiseEvents;
            }
        }

        /// <summary>
        /// Gets or sets the information indicating whether to omit the array data in the result
        /// </summary>
        public bool OmitArrayData
        {
            get
            {
                return _interpreter.OmitArrayData;
            }
        }

        /// <summary>
        /// Gets or sets the information indicate whether to delay calculating values of virtual attributes
        /// </summary>
        /// <value>True to postpone the virtual value calculation at the select node stage. default is True</value>
        /// <remarks>Delay calculations to improve the query speed by avoiding calculating unneccessary virtual attributes</remarks>
        public bool DelayVirtualAttributeCalculation
        {
            get
            {
                return _interpreter.DelayVirtualAttributeCalculation;
            }
        }

        /// <summary>
        /// Gets the information indicate whether to check permissions other than read
        /// </summary>
        /// <value>True to check read permission only. false to check all permissions. default is false</value>
        public bool CheckReadPermissionOnly
        {
            get
            {
                return _interpreter.CheckReadPermissionOnly;
            }
        }

        /// <summary>
        /// Gets the information indicate whether to show encrypted data in the query result
        /// </summary>
        /// <value>True to show encrypted data in the result. false otherwise, default is false</value>
        public bool ShowEncryptedData
        {
            get
            {
                return _interpreter.ShowEncryptedData;
            }
        }

        /// <summary>
        /// Gets the information indicate whether to obtain cached objId for a primary key
        /// </summary>
        /// <value>True to obtain a cached objId, false to get objId from database. default is false</value>
        public bool ObtainCachedObjId
        {
            get
            {
                return _interpreter.ObtainCachedObjId;
            }
        }

        /// <summary> 
        /// Gets or sets the information indicating whether to return sql actions of a query
        /// instead of executing it
        /// </summary>
        /// <returns> true if it is for getting sql actions, false otherwise</returns>
        public bool IsForSQLActions
        {
            get
            {
                return _interpreter.IsForSQLActions;
            }
        }

        /// <summary>
        /// Gets or sets the xquery interpreter that creates the document
        /// </summary>
        /// <value> The interpreter.</value>
        public Interpreter Interpreter
		{
			get
			{
				return _interpreter;
			}
			set
			{
				_interpreter = value;
			}
		}

		/// <summary>
		/// Return the meta data model of the document, the DocumentDB overrides it.
		/// </summary>
		/// <returns> The meta data model</returns>
		public virtual MetaDataModel MetaData
		{
			get
			{
				return null; // subclass overides it
			}
			set
			{
			}
		}

		/// <summary>
		/// this abstract method will be overrided by subclass to initialize the document.
		/// </summary>
		public abstract void Initialize();
		
		/// <summary>
		/// Return a data provider associated with the document.
		/// </summary>
		/// <value>A DataProvider</value>
		public abstract IDataProvider DataProvider
		{
			get;
		}

		/// <summary>
		/// Gets an Enumerator for child axis of the context node. Three types of node can have
		/// child axis. They are VDocument, XmlElement, and XmlAttribute.
		/// </summary>
		/// <param name="contextNode">the context node.</param>
		/// <returns> Enumerator for child axis of the context node</returns>
		/// <remarks>This method returns null for those node types that do not have a child axis.</remarks>
		public virtual IEnumerator GetChildAxisEnumerator(XmlNode contextNode)
		{
			IEnumerator enumerator = null;
			
			if (contextNode.NodeType == XmlNodeType.Element)
			{
				XmlElement element = (XmlElement) contextNode;
				if (element.ChildNodes.Count == 1 && element.FirstChild.NodeType == XmlNodeType.Text)
				{
					// return an empty enumerator if the only child is a text
					enumerator = _emptyList.GetEnumerator();
				}
				else
				{
					enumerator = ((XmlElement) contextNode).ChildNodes.GetEnumerator();
				}
			}
			else if (contextNode.NodeType == XmlNodeType.Document)
			{
				// return the children of the document element
				enumerator = ((VDocument) contextNode).DocumentElement.ChildNodes.GetEnumerator();
			}
			else
			{
				throw new UnsupportedAxisException("Child axis is not supported for " + contextNode.Name);
			}
			
			return enumerator;
		}
		
		/// <summary>
		/// Gets an Enumerator for descendant axis of the context node.
		/// </summary>
		/// <param name="contextNode">the context node</param>
		/// <returns> Enumerator for Descendant Axis of the context node</returns>
		public virtual IEnumerator GetDescendantAxisEnumerator(XmlNode contextNode)
		{
			return new DescendantAxisEnumerator(contextNode, this);
		}
		
		/// <summary>
		/// Gets an Enumerator for parent axis of the context node.
		/// </summary>
		/// <param name="contextNode">the context node</param>
		/// <returns> Enumerator for parent axis of the context node.</returns>
		/// <remarks> Default implementation is to throw UnsupportedAxisException</remarks>
		public virtual IEnumerator GetParentAxisEnumerator(XmlNode contextNode)
		{
			IEnumerator enumerator = null;
			XmlNode parent = null;
			
			if (contextNode.NodeType == XmlNodeType.Document)
			{
				// the parent of a document is itself
				parent = contextNode;
			}
			else if (contextNode.NodeType == XmlNodeType.Element)
			{
				parent = ((XmlElement) contextNode).ParentNode;
				
				// if it is a root element, then the parent is the document
				if (parent == null)
				{
					parent = ((XmlElement) contextNode).OwnerDocument;
				}
			}
			else if (contextNode.NodeType == XmlNodeType.Attribute)
			{
				// Parent of an attribute is an element
				parent = ((XmlAttribute) contextNode).ParentNode;
			}
			else
			{
				throw new UnsupportedAxisException("Parent axis is not supported for " + contextNode.Name);
			}
			
			enumerator = new SingleNodeEnumerator(parent);
			
			return enumerator;
		}
		
		/// <summary>
		/// Gets an Enumerator for Ancestor axis of the context node. 
		/// </summary>
		/// <param name="contextNode">the context node</param>
		/// <returns> Enumerator for ancestor axis of the context node</returns>
		public virtual IEnumerator GetAncestorAxisEnumerator(XmlNode contextNode)
		{
			return new AncestorAxisEnumerator(contextNode, this);
		}
		
		/// <summary>
		/// Gets an Enumerator for following sibling axis of the context node.
		/// </summary>
		/// <param name="contextNode">the context node.</param>
		/// <returns> Enumerator for following sibling axis of the context node</returns>
		/// <remarks>The default implementation is to throw an exception</remarks>
		public virtual IEnumerator GetFollowingSiblingAxisEnumerator(XmlNode contextNode)
		{
			throw new UnsupportedAxisException("Following sibling axis is not supported");
		}
		
		/// <summary>
		/// Gets an Enumerator for preceding sibling axis of the context node.</summary>
		/// <param name="contextNode">the context node.</param>
		/// <returns> Enumerator for Preceding Sibling axis of the context node</returns>
		/// <remarks> Default implementation is to throw UnsupportedAxisException</remarks>
		public virtual IEnumerator GetPrecedingSiblingAxisEnumerator(XmlNode contextNode)
		{
			throw new UnsupportedAxisException("Preceding sibling axis is not supported");
		}
		
		/// <summary>
		/// Gets an Enumerator for following axis of the context node.
		/// </summary>
		/// <param name="contextNode">the context node.</param>
		/// <returns> Enumerator for Following axis of context node.</returns>
		/// <remarks> Default implementation is to throw UnsupportedAxisException</remarks>
		public virtual IEnumerator GetFollowingAxisEnumerator(XmlNode contextNode)
		{
			throw new UnsupportedAxisException("Following axis is not supported");
		}
		
		/// <summary>
		/// Gets an Enumerator for preceding axis of the context node.
		/// </summary>
		/// <param name="contextNode">the context node.</param>
		/// <returns> Enumerator for Preceding axis of the context node.</returns>
		/// <remarks> Default implementation is to throw UnsupportedAxisException</remarks>
		public virtual IEnumerator GetPrecedingAxisEnumerator(XmlNode contextNode)
		{
			throw new UnsupportedAxisException("Preceding axis is not supported");
		}
		
		/// <summary>
		/// Gets an Enumerator for attribute axis of the context node.
		/// </summary>
		/// <param name="contextNode">the context node.</param>
		/// <returns> Enumerator for Attribute axis</returns>
		public virtual IEnumerator GetAttributeAxisEnumerator(XmlNode contextNode)
		{
			
			IEnumerator enumerator = null;
			
			if (contextNode is XmlElement)
			{
				XmlElement element = (XmlElement) contextNode;
				enumerator = element.Attributes.GetEnumerator();
			}
			else
			{
				throw new UnsupportedAxisException("Attribute axis is not supported for " + contextNode.Name);
			}
			
			return enumerator;
		}
		
		/// <summary>
		/// Geta an Enumerator for dereference axis of the context node. The context node
		/// is an IDREF or IDREFS type and contains ID(s) of referenced elements.
		/// </summary>
		/// <param name="contextNode">the context node</param>
		/// <returns> Enumerator for Dereference axis of the context node.</returns>
		public virtual IEnumerator GetDereferenceAxisEnumerator(XmlNode contextNode)
		{
			
			ArrayList referencedNodes = new ArrayList();
			
			// get elements that have id matched by the id(s) in the attribute value
			if (contextNode.NodeType == XmlNodeType.Attribute)
			{
				XmlAttribute attribute = (XmlAttribute) contextNode;
				
				string[] refIds = BreakupIdValues(attribute.Value);
				
				/*
				* search the elements that have matched id.
				* 
				* PERFORMANCE: getting a DescendantAxisEnumerator is not every efficient way
				* need to improve.
				*/
				IEnumerator elements = GetDescendantAxisEnumerator(this.DocumentElement);
				while (elements.MoveNext())
				{
					XmlElement element = (XmlElement) elements.Current;
					
					/*
					* TODO: Since XML has not support ID, IDREF, and IDREFS attributes,
					* we set a constraint that an element ID attribute has to be
					* named as obj_id.
					*/
					XmlAttribute idAttribute = element.GetAttributeNode(SQLElement.OBJ_ID);
					if (idAttribute != null)
					{
						foreach (string idValue in refIds)
						{
							if (idValue.Equals(idAttribute.Value))
							{
								referencedNodes.Add(element);
								break;
							}
						}
					}
				}
			}
			else
			{
				throw new UnsupportedAxisException("Dereference axis is not supported for " + contextNode.Name);
			}
			
			return referencedNodes.GetEnumerator();
		}
		
		/// <summary>
		/// Gets an Enumerator for self axis of the context node.
		/// </summary>
		/// <param name="contextNode">the context node</param>
		/// <returns> Enumerator for Self axis</returns>
		public virtual IEnumerator GetSelfAxisEnumerator(XmlNode contextNode)
		{
			return new SelfAxisEnumerator(contextNode);
		}
		
		/// <summary>
		/// Gets an Enumerator for Descendant or self axis of the context node.
		/// </summary>
		/// <param name="contextNode">the context node.</param>
		/// <returns> Enumerator for the specified axis.</returns>
		public virtual IEnumerator GetDescendantOrSelfAxisEnumerator(XmlNode contextNode)
		{
			return new DescendantOrSelfAxisEnumerator(contextNode, this);
		}

		/// <summary>
		/// Gets an Enumerator for Ancestor or self axis of the context node.
		/// </summary>
		/// <param name="contextNode">the context node.</param>
		/// <returns> Enumerator for Ancestor Or Self axis.</returns>
		public virtual IEnumerator GetAncestorOrSelfAxisEnumerator(XmlNode contextNode)
		{
			return new AncestorOrSelfAxisEnumerator(contextNode, this);
		}
		
		/// <summary> Get type of the node from the schema.
		/// 
		/// </summary>
		/// <param name="node">a node in a document
		/// 
		/// </param>
		/// <returns> integer type
		/// 
		/// </returns>
		public virtual ExprType GetNodeType(XmlNode node)
		{
			ExprType type = ExprType.XNODE;
			
			if (_entityTable != null)
			{
				DBEntity entity = (DBEntity) _entityTable[node];
				if (entity != null)
				{
					type = ConvertToInterpretType(entity.Type);
				}
			}
			
			return type;
		}
		
		/// <summary>
		/// Print the xml to the Console.
		/// </summary>
		public void PrintXml()
		{
			XmlTextWriter writer = new XmlTextWriter(System.Console.Out);
			writer.Formatting = Formatting.Indented;
			writer.Indentation = 2;

			this.WriteTo(writer);
		}

		/// <summary>
		/// Convert a VDOM Type to the corresponding Interpreter type.
		/// </summary>
		/// <param name="type">the VDOM type</param>
		/// <param name="an">integer representing the interpreter type</param>
		/// <remarks> TODO, consolidate the type definitions.</remarks>
		private ExprType ConvertToInterpretType(Newtera.Common.MetaData.Schema.DataType type)
		{
			switch (type)
			{
				case Newtera.Common.MetaData.Schema.DataType.Boolean: 
					return ExprType.XBOOLEAN;
				
				case Newtera.Common.MetaData.Schema.DataType.Byte: 
					return ExprType.XINTEGER; // TODO
				
				case Newtera.Common.MetaData.Schema.DataType.Date: 
					return ExprType.XSTRING; // TODO
				
				case Newtera.Common.MetaData.Schema.DataType.DateTime: 
					return ExprType.XSTRING; // TODO
				
				case Newtera.Common.MetaData.Schema.DataType.Decimal: 
					return ExprType.XDECIMAL;
				
				case Newtera.Common.MetaData.Schema.DataType.Double: 
					return ExprType.XDOUBLE;
				
				case Newtera.Common.MetaData.Schema.DataType.Float: 
					return ExprType.XFLOAT;
				
				case Newtera.Common.MetaData.Schema.DataType.Integer: 
					return ExprType.XINTEGER;

				case Newtera.Common.MetaData.Schema.DataType.BigInteger: 
					return ExprType.XINTEGER;
				
				case Newtera.Common.MetaData.Schema.DataType.String:
				case Newtera.Common.MetaData.Schema.DataType.Text: 
					return ExprType.XSTRING;
				
				default: 
					return ExprType.XSTRING;
			}
		}

		/*
		 * A convinient method to break up id values in a String.
		 */
		private string[] BreakupIdValues(string idValueStr)
		{
			// TODO: Compile a regular expression to find objId
			Regex exp = new Regex(" ");

			return exp.Split(idValueStr);
		}
	}
}