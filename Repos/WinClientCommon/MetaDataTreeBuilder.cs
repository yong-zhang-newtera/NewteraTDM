/*
* @(#)MetaDataTreeBuilder.cs
*
* Copyright (c) 2003 Newtera, Inc. All rights reserved.
*/
namespace Newtera.WinClientCommon
{
	using System;
	using System.Collections;
	using System.Resources;
	using System.Windows.Forms;
	using System.Threading;

	using Newtera.Common.MetaData.Principal;
	using Newtera.Common.MetaData;
	using Newtera.Common.MetaData.Schema;
	using Newtera.Common.MetaData.XaclModel;
	using Newtera.Common.MetaData.DataView;
	using Newtera.Common.MetaData.DataView.Taxonomy;
    using Newtera.Common.MetaData.XMLSchemaView;
	using Newtera.Common.MetaData.Mappings;
	
	/// <summary>
	/// Builds a tree structure using MetaDataTreeNode objects according to a
	/// Schema Model.
	/// </summary>
	/// <version>  1.0.1 26 Sept 2003</version>
	/// <author>  Yong Zhang</author>
	public class MetaDataTreeBuilder
	{
		private MetaDataModel _metaData;
		private bool _showAttributes = true;
		private bool _showConstraints = true;
		private bool _showDataViews = true;
		private bool _showTaxonomies = true;
		private bool _showSelectors = false;
        private bool _showXMLSchemaViews = true;
		private Hashtable _mapping;
		private ResourceManager _resources;

		/// <summary>
		/// Initializes a new instance of the MetaDataTreeBuilder class.
		/// </summary>
		public MetaDataTreeBuilder()
		{
			_metaData = null;
			_mapping = new Hashtable();
			_resources = new ResourceManager(this.GetType());
		}

		/// <summary>
		/// Gets the information indicating whether it must check permissions
		/// before adding an item to the tree
		/// </summary>
		/// <value>true if it must check permissions, false otherwise.</value>
		public bool CheckPermission
		{
			get
			{
				// if the CustomerPrincipal is null, it means an unauthenticated user
				// do not check permission
				CustomPrincipal principal = Thread.CurrentPrincipal as CustomPrincipal;

				if (principal != null)
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
		/// Gets the Tree Node for the given meta data element
		/// </summary>
		/// <param name="element">The meta data element</param>
		/// <returns>The tree node</returns>
		public MetaDataTreeNode GetTreeNode(IMetaDataElement element)
		{
			return (MetaDataTreeNode) _mapping[element];
		}

		/// <summary>
		/// Remove the mapping of a meta data element to a tree node.
		/// </summary>
		/// <param name="element">The meta data element</param>
		public void RemoveMapping(IMetaDataElement element)
		{
			_mapping.Remove(element);
		}

		/// <summary> 
		/// Gets or sets the information indicating whether to show the attributes,
		/// including simple and relationship attributes, in the built tree.
		/// </summary>
		/// <value> true if attributes are shown, false otherwise. Default is true.</value>
		public bool IsAttributesShown
		{
			get
			{
				return _showAttributes;
			}
			set
			{
				_showAttributes = value;
			}
		}

		/// <summary> 
		/// Gets or sets the information indicating whether to show the constraints,
		/// including enumeration, range and pattern, in the built tree.
		/// </summary>
		/// <value> true if constraints are shown, false otherwise. Default is true.</value>
		public bool IsConstraintsShown
		{
			get
			{
				return _showConstraints;
			}
			set
			{
				_showConstraints = value;
			}
		}

		/// <summary> 
		/// Gets or sets the information indicating whether to show the data views.
		/// </summary>
		/// <value> true if data views are shown, false otherwise. Default is true.</value>
		public bool IsDataViewsShown
		{
			get
			{
				return _showDataViews;
			}
			set
			{
				_showDataViews = value;
			}
		}

		/// <summary> 
		/// Gets or sets the information indicating whether to show the taxonomies.
		/// </summary>
		/// <value> true if taxonomies are shown, false otherwise. Default is true.</value>
		public bool IsTaxonomiesShown
		{
			get
			{
				return _showTaxonomies;
			}
			set
			{
				_showTaxonomies = value;
			}
		}

		/// <summary> 
		/// Gets or sets the information indicating whether to show the selectors.
		/// </summary>
		/// <value> true if selectors are shown, false otherwise. Default is true.</value>
		public bool IsSelectorShown
		{
			get
			{
				return _showSelectors;
			}
			set
			{
				_showSelectors = value;
			}
		}

        /// <summary> 
        /// Gets or sets the information indicating whether to show the xml schema views.
        /// </summary>
        /// <value> true if xml schema views are shown, false otherwise. Default is true.</value>
        public bool IsXMLSchemaViewsShown
        {
            get
            {
                return _showXMLSchemaViews;
            }
            set
            {
                _showXMLSchemaViews = value;
            }
        }

		/// <summary>
		/// Builds a tree without a schema model tree nodes
		/// </summary>
		/// <returns>The root of the tree</returns>
		public TreeNode BuildTree()
		{
			// the default name for a new schema is NewSchema
			SchemaInfoElement schemaInfo = new SchemaInfoElement("NewSchema");
			schemaInfo.Version= "1.0"; // Initial version number

			return BuildTreeFrame(schemaInfo);
		}

		/// <summary>
		/// Builds a tree with a schema model tree nodes
		/// </summary>
		/// <param name="metaData">The meta data that a tree is to present</param>
		/// <returns>The root of the tree</returns>
		public TreeNode BuildTree(MetaDataModel metaData)
		{
			int nodeIndex = 0;
			MetaDataTreeNode root = BuildTreeFrame(metaData.SchemaModel.SchemaInfo);

			// build classes and attributes nodes as children nodes
			_metaData = metaData;
			MetaDataTreeNode classNode = (MetaDataTreeNode) root.Nodes[nodeIndex++];
			SchemaModelElementCollection rootClasses = metaData.SchemaModel.RootClasses;

			// display the class nodes based on their display positions
			foreach (ClassElement classElement in rootClasses)
			{
				AddClassNode(classNode, classElement);
			}

			// build constraint nodes as children nodes
			IList allConstraints = metaData.SchemaModel.AllConstraints;
			if (_showConstraints)
			{
				MetaDataTreeNode constraintNode = (MetaDataTreeNode) root.Nodes[nodeIndex++];

				// display the constraints based on their display positions
				MetaDataTreeNode cNode;
				foreach (IMetaDataElement metaElement in allConstraints)
				{
					if (metaElement is EnumElement)
					{
						cNode = CreateTreeNode(NodeType.EnumConstraintNode, metaElement);

						constraintNode.Nodes.Add(cNode);
					}
					else if (metaElement is RangeElement)
					{
						cNode = CreateTreeNode(NodeType.RangeConstraintNode, metaElement);
					
						constraintNode.Nodes.Add(cNode);
					}
					else if (metaElement is PatternElement)
					{
						cNode = CreateTreeNode(NodeType.PatternConstraintNode, metaElement);

						constraintNode.Nodes.Add(cNode);
					}
					else if (metaElement is ListElement)
					{
						cNode = CreateTreeNode(NodeType.ListConstraintNode, metaElement);

						constraintNode.Nodes.Add(cNode);
					}
				}
			}

			if (_showTaxonomies)
			{
				MetaDataTreeNode taxonomiesNode;
				MetaDataTreeNode taxonomyNode;
				taxonomiesNode = (MetaDataTreeNode) root.Nodes[nodeIndex++];

				// display the node based on their display positions
                foreach (TaxonomyModel taxonomy in metaData.Taxonomies)
                {
                    // only add a tree node for a taxonomy to which the principal has
                    // permission to see
                    if (!CheckPermission || PermissionChecker.Instance.HasPermission(_metaData.XaclPolicy, taxonomy, XaclActionType.Read))
                    {
                        taxonomyNode = CreateTreeNode(NodeType.TaxonomyNode, taxonomy);

                        taxonomiesNode.Nodes.Add(taxonomyNode);

                        // add children tree nodes recursively
                        AddTaxonNodes(taxonomyNode, taxonomy.ChildrenNodes);
                    }
                }
			}

			if (_showDataViews)
			{
				MetaDataTreeNode dataViewsNode;
				MetaDataTreeNode dvNode;
				dataViewsNode = (MetaDataTreeNode) root.Nodes[nodeIndex++];

				foreach (IMetaDataElement dataView in metaData.DataViews)
				{
                    dvNode = CreateTreeNode(NodeType.DataViewNode, dataView);

                    dataViewsNode.Nodes.Add(dvNode);
				}
			}

			if (_showSelectors)
			{
				MetaDataTreeNode selectorsNode;
				MetaDataTreeNode selectorNode;

				selectorsNode = (MetaDataTreeNode) root.Nodes[nodeIndex++];

				foreach (IMetaDataElement selector in metaData.SelectorManager.Selectors)
				{
					selectorNode = CreateTreeNode(NodeType.SelectorNode, selector);
					
					selectorsNode.Nodes.Add(selectorNode);
				}
			}

            if (_showXMLSchemaViews)
            {
                MetaDataTreeNode schemaViewsNode;
                MetaDataTreeNode schemaViewNode;

                schemaViewsNode = (MetaDataTreeNode)root.Nodes[nodeIndex++];

                foreach (XMLSchemaModel xmlSchemaModel in metaData.XMLSchemaViews)
                {
                    schemaViewNode = CreateTreeNode(NodeType.XMLSchemaView, xmlSchemaModel);

                    schemaViewsNode.Nodes.Add(schemaViewNode);

                    // add children tree nodes recursively
                    XMLSchemaComplexType complexType = (XMLSchemaComplexType)xmlSchemaModel.ComplexTypes[xmlSchemaModel.RootElement.Caption];
                    if (complexType != null && complexType.Name == xmlSchemaModel.RootElement.Name)
                    {
                        AddComplexTypeElement(schemaViewNode, xmlSchemaModel.RootElement, complexType, xmlSchemaModel);
                    }
                }
            }

			return root;
		}

		/// <summary>
		/// Builds a class tree with a specified root.
		/// </summary>
		/// <param name="metaData">The meta data that contains a complete class tree</param>
		/// <param name="rootClass">The root class name, if null, build a tree for all classes.</param>
		/// <returns>The root of the treeview</returns>
		public TreeNode BuildClassTree(MetaDataModel metaData, string rootClass)
		{
			MetaDataTreeNode root;
			SchemaModelElementCollection childrenClasses;
			this.IsAttributesShown = false;
			_metaData = metaData;

			if (rootClass.ToUpper() == "ALL")
			{
				root = CreateTreeNode(NodeType.ClassesFolder);

				childrenClasses = metaData.SchemaModel.RootClasses;
			}
			else
			{
				ClassElement rootClassElement = metaData.SchemaModel.FindClass(rootClass);

				root = CreateTreeNode(rootClassElement);

				childrenClasses = rootClassElement.Subclasses;
			}

			// display the class nodes based on their display position
			foreach (ClassElement clsElement in childrenClasses)
			{
				AddClassNode(root, clsElement);
			}

			return root;
		}

        /// <summary>
        /// Builds a xml schema tree based on a model
        /// </summary>
        /// <param name="xmlSchemaModel">The xml schema model</param>
        /// <returns>The root of the tree</returns>
        public TreeNode BuildXMLSchemaTree(XMLSchemaModel xmlSchemaModel)
        {
            MetaDataTreeNode schemaViewNode;

            schemaViewNode = CreateTreeNode(NodeType.XMLSchemaView, xmlSchemaModel);

            // add children tree nodes recursively
            XMLSchemaComplexType complexType = (XMLSchemaComplexType)xmlSchemaModel.ComplexTypes[xmlSchemaModel.RootElement.Caption];
            if (complexType != null && complexType.Name == xmlSchemaModel.RootElement.Name)
            {
                AddComplexTypeElement(schemaViewNode, xmlSchemaModel.RootElement, complexType, xmlSchemaModel);
            }

            return schemaViewNode;
        }

		/// <summary>
		/// Create a MetaDataTreeNode
		/// </summary>
		/// <param name="metaDataElement">The corresponding meta data element.</param>
		/// <returns>The created tree node</returns>
		public MetaDataTreeNode CreateTreeNode(IMetaDataElement metaDataElement)
		{
			MetaDataTreeNode treeNode = new MetaDataTreeNode(metaDataElement);

			SetTreeNodeProperties(treeNode);

			_mapping[metaDataElement] = treeNode;

			return treeNode;
		}

        /// <summary>
        /// Build a tree to represent a classifying hierarchy
        /// </summary>
        /// <param name="treeRoot">The root of the tree</param>
        /// <param name="rootNode">The root of the hierarchy</param>
        public void BuildTaxonomyTree(MetaDataTreeNode treeRoot, ITaxonomy rootNode)
        {
            // add children tree nodes recursively
            AddTaxonNodes(treeRoot, rootNode.ChildrenNodes);
        }

		/// <summary>
		/// Create a MetaDataTreeNode
		/// </summary>
		/// <param name="type">The tree node type</param>
		/// <param name="metaDataElement">The corresponding meta data element.</param>
		/// <returns>The created tree node</returns>
		private MetaDataTreeNode CreateTreeNode(NodeType type, IMetaDataElement metaDataElement)
		{
			MetaDataTreeNode treeNode = new MetaDataTreeNode(type, metaDataElement);

			SetTreeNodeProperties(treeNode);

			_mapping[metaDataElement] = treeNode;

			return treeNode;
		}

		/// <summary>
		/// Create a tree node
		/// </summary>
		/// <param name="type">The type of tree node</param>
		/// <returns>The created tree node</returns>
		private MetaDataTreeNode CreateTreeNode(NodeType type)
		{
			MetaDataTreeNode treeNode = new MetaDataTreeNode(type);

			SetTreeNodeProperties(treeNode);

			return treeNode;
		}

        /// <summary>
        /// Create a tree node given a type and display text
        /// </summary>
        /// <param name="type">The type of tree node</param>
        /// <param name="text">The display text of tree node</param>
        /// <returns>The created tree node</returns>
        private MetaDataTreeNode CreateTreeNode(NodeType type, string text)
        {
            MetaDataTreeNode treeNode = new MetaDataTreeNode(type);

            treeNode.Text = text;
            SetTreeNodeProperties(treeNode);

            return treeNode;
        }

		/// <summary>
		/// Set other properties of the tree node, such as imageindex
		/// </summary>
		/// <param name="treeNode"></param>
		private void SetTreeNodeProperties(MetaDataTreeNode treeNode)
		{
			switch (treeNode.Type)
			{
				case NodeType.SchemaInfoNode:
					treeNode.Text = treeNode.MetaDataElement.Caption;
					treeNode.ImageIndex = 0;
					treeNode.SelectedImageIndex = 1;
					break;

				case NodeType.ClassesFolder:
					treeNode.Text = _resources.GetString("Classes.Text");
					treeNode.ImageIndex = 0;
					treeNode.SelectedImageIndex = 1;
					break;

				case NodeType.ConstraintsFolder:
					treeNode.Text = _resources.GetString("Constraints.Text");
					treeNode.ImageIndex = 0;
					treeNode.SelectedImageIndex = 1;
					break;

				case NodeType.DataViewsFolder:
					treeNode.Text = _resources.GetString("DataViews.Text");
					treeNode.ImageIndex = 0;
					treeNode.SelectedImageIndex = 1;
					break;

				case NodeType.TaxonomiesFolder:
					treeNode.Text = _resources.GetString("Taxonomies.Text");
					treeNode.ImageIndex = 0;
					treeNode.SelectedImageIndex = 1;
					break;

                case NodeType.XMLSchemaViewsFolder:
                    treeNode.Text = _resources.GetString("XMLSchemaViews.Text");
                    treeNode.ImageIndex = 0;
                    treeNode.SelectedImageIndex = 1;
                    break;

				case NodeType.ClassNode:
					treeNode.Text = treeNode.MetaDataElement.Caption;
					treeNode.ImageIndex = 2;
					treeNode.SelectedImageIndex = 3;
					break;

				case NodeType.SimpleAttributeNode:
					treeNode.Text = treeNode.MetaDataElement.Caption;
					treeNode.ImageIndex = 4;
					treeNode.SelectedImageIndex = 4;
					break;

				case NodeType.RelationshipAttributeNode:
					treeNode.Text = treeNode.MetaDataElement.Caption;
					treeNode.ImageIndex = 5;
					treeNode.SelectedImageIndex = 5;
					break;

				case NodeType.ArrayAttributeNode:
					treeNode.Text = treeNode.MetaDataElement.Caption;
					treeNode.ImageIndex = 12;
					treeNode.SelectedImageIndex = 12;
					break;

                case NodeType.VirtualAttributeNode:
                    treeNode.Text = treeNode.MetaDataElement.Caption;
                    treeNode.ImageIndex = 15;
                    treeNode.SelectedImageIndex = 15;
                    break;

                case NodeType.ImageAttributeNode:
                    treeNode.Text = treeNode.MetaDataElement.Caption;
                    treeNode.ImageIndex = 16;
                    treeNode.SelectedImageIndex = 16;
                    break;

				case NodeType.EnumConstraintNode:
					treeNode.Text = treeNode.MetaDataElement.Caption;
					treeNode.ImageIndex = 6;
					treeNode.SelectedImageIndex = 6;
					break;

				case NodeType.RangeConstraintNode:
					treeNode.Text = treeNode.MetaDataElement.Caption;
					treeNode.ImageIndex = 7;
					treeNode.SelectedImageIndex = 7;
					break;

				case NodeType.PatternConstraintNode:
					treeNode.Text = treeNode.MetaDataElement.Caption;
					treeNode.ImageIndex = 8;
					treeNode.SelectedImageIndex = 8;
					break;

				case NodeType.ListConstraintNode:
					// TODO, need a new icon for List Constraint
					treeNode.Text = treeNode.MetaDataElement.Caption;
					treeNode.ImageIndex = 13;
					treeNode.SelectedImageIndex = 13;
					break;

				case NodeType.DataViewNode:
					treeNode.Text = treeNode.MetaDataElement.Caption;
					treeNode.ImageIndex = 9;
					treeNode.SelectedImageIndex = 9;
					break;

				case NodeType.TaxonomyNode:
					treeNode.Text = treeNode.MetaDataElement.Caption;
					treeNode.ImageIndex = 10;
					treeNode.SelectedImageIndex = 10;
					break;

				case NodeType.TaxonNode:
					treeNode.Text = treeNode.MetaDataElement.Caption;
					treeNode.ImageIndex = 11;
					treeNode.SelectedImageIndex = 11;
					break;

				case NodeType.SelectorsFolder:
					treeNode.Text = _resources.GetString("Selectors.Text");
					treeNode.ImageIndex = 0;
					treeNode.SelectedImageIndex = 1;
					break;

				case NodeType.SelectorNode:
					treeNode.Text = treeNode.MetaDataElement.Caption;
					treeNode.ImageIndex = 14; // use a new image
					treeNode.SelectedImageIndex = 14; // use a new image
					break;

                case NodeType.XMLSchemaView:
                    treeNode.Text = treeNode.MetaDataElement.Caption;
                    treeNode.ImageIndex = 11; // use a new image
                    treeNode.SelectedImageIndex = 11; // use a new image
                    break;

                case NodeType.XMLSchemaComplexType:
                    treeNode.Text = treeNode.MetaDataElement.Caption;
                    treeNode.ImageIndex = 2; // use a new image
                    treeNode.SelectedImageIndex = 3; // use a new image
                    break;

                case NodeType.XMLSchemaElement:
                    treeNode.Text = treeNode.MetaDataElement.Caption;
                    treeNode.ImageIndex = 4; // use a new image
                    treeNode.SelectedImageIndex = 4; // use a new image
                    break;

                case NodeType.CategoryFolder:
                    treeNode.ImageIndex = 0;
                    treeNode.SelectedImageIndex = 1;
                    break;
			}
		}

		/// <summary>
		/// Build a schema model tree frame that consists of fixed nodes, such as schema node
		/// Class Node and Constraint Node, from which to add the schema model nodes.
		/// </summary>
		/// <param name="schemaInfo">The schema info instance</param>
		/// <returns>The root of the tree</returns>
		private MetaDataTreeNode BuildTreeFrame(SchemaInfoElement schemaInfo)
		{
			MetaDataTreeNode root = CreateTreeNode(NodeType.SchemaInfoNode, schemaInfo);

			MetaDataTreeNode classesNode = CreateTreeNode(NodeType.ClassesFolder);

			root.Nodes.Add(classesNode);

			if (_showConstraints)
			{
				MetaDataTreeNode constraintNode = CreateTreeNode(NodeType.ConstraintsFolder);

				root.Nodes.Add(constraintNode);
			}

			if (_showTaxonomies)
			{
				MetaDataTreeNode taxonomiesNode = CreateTreeNode(NodeType.TaxonomiesFolder);

				root.Nodes.Add(taxonomiesNode);
			}

			if (_showDataViews)
			{
				MetaDataTreeNode dataViewNode = CreateTreeNode(NodeType.DataViewsFolder);

				root.Nodes.Add(dataViewNode);
			}

			if (_showSelectors)
			{
				MetaDataTreeNode selectorNode = CreateTreeNode(NodeType.SelectorsFolder);

				root.Nodes.Add(selectorNode);
			}

            if (_showXMLSchemaViews)
            {
                MetaDataTreeNode schemaViewsNode = CreateTreeNode(NodeType.XMLSchemaViewsFolder);

                root.Nodes.Add(schemaViewsNode);
            }

			return root;
		}

		/// <summary>
		/// Add a class node as child node to the parent node. This method
		/// can be called recursively.
		/// </summary>
		/// <param name="parentNode">The parent node</param>
		/// <param name="classElement">The class element</param>
		private void AddClassNode(MetaDataTreeNode parentNode, ClassElement classElement)
		{
			MetaDataElementSortedList sortedChildrenList = new MetaDataElementSortedList();

			// only add a tree node for a class to which the principal has
			// permission to read
			if (!CheckPermission || PermissionChecker.Instance.HasPermission(_metaData.XaclPolicy, classElement, XaclActionType.Read))
			{
				MetaDataTreeNode classNode = CreateTreeNode(NodeType.ClassNode, classElement);

                // if the class belongs to a category, add the class tree node under
                // a folder represents a category
                if (!string.IsNullOrEmpty(classElement.Category))
                {
                    // Get or create a category node, and add the class node under the category node
                    MetaDataTreeNode categoryNode = GetCategoryNode(parentNode, classElement);
                    categoryNode.Nodes.Add(classNode);
                }
                else
                {
                    // no category, add the class node directly under the parent class node
                    parentNode.Nodes.Add(classNode);
                }

				if (this._showAttributes)
				{
					// add attributes as children nodes
					SchemaModelElementCollection attributes = classElement.SimpleAttributes;
					if (attributes != null)
					{
						foreach (SimpleAttributeElement attributeElement in attributes)
						{
							sortedChildrenList.Add(attributeElement.Position, attributeElement);
						}
					}

					attributes = classElement.ArrayAttributes;
					if (attributes != null)
					{
						foreach (ArrayAttributeElement attributeElement in attributes)
						{
							sortedChildrenList.Add(attributeElement.Position, attributeElement);
						}
					}

                    attributes = classElement.VirtualAttributes;
                    if (attributes != null)
                    {
                        foreach (VirtualAttributeElement attributeElement in attributes)
                        {
                            sortedChildrenList.Add(attributeElement.Position, attributeElement);
                        }
                    }

                    attributes = classElement.ImageAttributes;
                    if (attributes != null)
                    {
                        foreach (ImageAttributeElement attributeElement in attributes)
                        {
                            sortedChildrenList.Add(attributeElement.Position, attributeElement);
                        }
                    }

					attributes = classElement.RelationshipAttributes;
					if (attributes != null)
					{
						foreach (RelationshipAttributeElement attributeElement in attributes)
						{
							sortedChildrenList.Add(attributeElement.Position, attributeElement);
						}
					}
				}

				SchemaModelElementCollection subclasses = classElement.Subclasses;
				if (subclasses != null)
				{
					foreach (ClassElement subclass in subclasses)
					{
						sortedChildrenList.Add(subclass.Position, subclass);
					}
				}

				// add the child tree node based on display order
				MetaDataTreeNode attributeNode;				
				foreach (IMetaDataElement metaElement in sortedChildrenList.Values)
				{
					if (metaElement is ClassElement)
					{
						// add a subclasse
						AddClassNode(classNode, (ClassElement) metaElement);
					}
					else if (metaElement is SimpleAttributeElement)
					{
						attributeNode = CreateTreeNode(NodeType.SimpleAttributeNode, metaElement);
	
						classNode.Nodes.Add(attributeNode);
					}
					else if (metaElement is ArrayAttributeElement)
					{
						attributeNode = CreateTreeNode(NodeType.ArrayAttributeNode, metaElement);
	
						classNode.Nodes.Add(attributeNode);
					}
                    else if (metaElement is VirtualAttributeElement)
                    {
                        attributeNode = CreateTreeNode(NodeType.VirtualAttributeNode, metaElement);

                        classNode.Nodes.Add(attributeNode);
                    }
                    else if (metaElement is ImageAttributeElement)
                    {
                        attributeNode = CreateTreeNode(NodeType.ImageAttributeNode, metaElement);

                        classNode.Nodes.Add(attributeNode);
                    }
					else if (metaElement is RelationshipAttributeElement)
					{
						attributeNode = CreateTreeNode(NodeType.RelationshipAttributeNode, metaElement);
							
						classNode.Nodes.Add(attributeNode);
					}
				}
			}
		}

		/// <summary>
		/// Add Taxon nodes as children nodes to the taxonomy node. This method
		/// can be called recursively.
		/// </summary>
		/// <param name="parentNode">The parent node</param>
		/// <param name="classes">The class collection</param>
		private void AddTaxonNodes(MetaDataTreeNode parentNode, TaxonNodeCollection nodes)
		{
			foreach (TaxonNode node in nodes)
			{
				// only add a tree node for a taxon to which the principal has
				// permission to see
				if (!CheckPermission || PermissionChecker.Instance.HasPermission(_metaData.XaclPolicy, node, XaclActionType.Read))
				{
					MetaDataTreeNode taxonTreeNode = CreateTreeNode(NodeType.TaxonNode, node);

					parentNode.Nodes.Add(taxonTreeNode);

					// add tree nodes for children nodes
					AddTaxonNodes(taxonTreeNode, node.ChildrenNodes);
				}
			}
		}

        /// <summary>
        /// Add a node for a comlextype elememnt and child nodes of the complex type element. This method
        /// can be called recursively.
        /// </summary>
        /// <param name="parentNode">The parent node</param>
        /// <param name="xmlSchemaElement">The complex type element</param>
        /// <param name="xmlSchemaModel">The xml schema model</param>
        private void AddComplexTypeElement(MetaDataTreeNode parentNode, XMLSchemaElement xmlSchemaElement, XMLSchemaComplexType complexType, XMLSchemaModel xmlSchemaModel)
        {
            MetaDataTreeNode complexTypeElementTreeNode = CreateTreeNode(NodeType.XMLSchemaComplexType, xmlSchemaElement);
            MetaDataTreeNode simpleTypeElementTreeNode;

            parentNode.Nodes.Add(complexTypeElementTreeNode);

            XMLSchemaComplexType childElementComplexType;
            foreach (XMLSchemaElement childElement in complexType.Elements)
            {
                // only add a tree node for a xml element that refers to a complex type
                childElementComplexType = (XMLSchemaComplexType)xmlSchemaModel.ComplexTypes[childElement.Caption];
                if (childElementComplexType != null &&
                    childElementComplexType.Name != childElement.Name)
                {
                    childElementComplexType = null;
                }

                if (childElementComplexType != null)
                {
                    // add tree nodes for the complex type element
                    AddComplexTypeElement(complexTypeElementTreeNode, childElement, childElementComplexType, xmlSchemaModel);
                }
                else
                {
                    // add tree node to represents a simple type element
                    simpleTypeElementTreeNode = CreateTreeNode(NodeType.XMLSchemaElement, childElement);

                    complexTypeElementTreeNode.Nodes.Add(simpleTypeElementTreeNode);
                }
            }
        }

        /// <summary>
		/// Get a category node under the parent node, if the category node deosn't exist,
        /// create a category node, add it to the parent node, then return it.
		/// </summary>
		/// <param name="parentNode">The parent node</param>
		/// <param name="classElement">The class element</param>
        private MetaDataTreeNode GetCategoryNode(MetaDataTreeNode parentNode, ClassElement classElement)
        {
            MetaDataTreeNode categoryNode = null;
            foreach (MetaDataTreeNode childNode in parentNode.Nodes)
            {
                if (childNode.Type == NodeType.CategoryFolder &&
                    childNode.Text == classElement.Category)
                {
                    categoryNode = childNode;
                    break;
                }
            }

            if (categoryNode == null)
            {
                categoryNode = CreateTreeNode(NodeType.CategoryFolder, classElement.Category);
                parentNode.Nodes.Add(categoryNode);
            }

            return categoryNode;
        }
	}
}