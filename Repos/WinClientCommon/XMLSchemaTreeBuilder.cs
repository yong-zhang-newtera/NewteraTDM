/*
* @(#)XMLSchemaTreeBuilder.cs
*
* Copyright (c) 2014 Newtera, Inc. All rights reserved.
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
    using Newtera.Common.MetaData.XMLSchemaView;
	
	/// <summary>
	/// Builds a tree structure for a xml schema model
	/// </summary>
	/// <version>  1.0.1 13 Aug 2014</version>
	public class XMLSchemaTreeBuilder
	{
		private MetaDataModel _metaData;
        private XMLSchemaModel _xmlSchemaModel;
        private bool _showAttributes = true;
		private Hashtable _mapping;
		private ResourceManager _resources;

		/// <summary>
		/// Initializes a new instance of the XMLSchemaTreeBuilder class.
		/// </summary>
		public XMLSchemaTreeBuilder(MetaDataModel metaData, XMLSchemaModel xmlSchemaModel)
		{
            _metaData = metaData;
            _xmlSchemaModel = xmlSchemaModel;
			_mapping = new Hashtable();
			_resources = new ResourceManager(this.GetType());
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
		/// Gets or sets the information indicating whether to show the attributes in the built tree.
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
		/// Builds a tree to represents nodes in  a xml schema
		/// </summary>
		/// <returns>The root of the tree</returns>
		public TreeNode BuildTree()
		{
            MetaDataTreeNode rootNode = CreateTreeNode(NodeType.XMLSchemaView, _xmlSchemaModel.RootElement);

            XMLSchemaElement rootElement = _xmlSchemaModel.RootElement;
            XMLSchemaComplexType complexType = (XMLSchemaComplexType)_xmlSchemaModel.ComplexTypes[rootElement.Caption];
            if (complexType != null)
            {
                AddComplexTypeNodes(rootNode, complexType);
            }

            return rootNode;
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
		/// Set other properties of the tree node, such as imageindex
		/// </summary>
		/// <param name="treeNode"></param>
		private void SetTreeNodeProperties(MetaDataTreeNode treeNode)
		{
			switch (treeNode.Type)
			{
                case NodeType.XMLSchemaView:
                    treeNode.Text = treeNode.MetaDataElement.Caption;
                    treeNode.ImageIndex = 0; // use a new image
                    treeNode.SelectedImageIndex = 1; // use a new image
                    break;

                case NodeType.XMLSchemaComplexType:
                    treeNode.Text = treeNode.MetaDataElement.Caption;
                    treeNode.ImageIndex = 0;
                    treeNode.SelectedImageIndex = 1;
                    break;

                case NodeType.XMLSchemaElement:
                    treeNode.Text = treeNode.MetaDataElement.Caption;
                    treeNode.ImageIndex = 2;
                    treeNode.SelectedImageIndex = 2;
                    break;
			}
		}

		/// <summary>
		/// Add elements in a xml schema complex type as children nodes to the parent node. This method
		/// can be called recursively.
		/// </summary>
		/// <param name="parentNode">The parent node</param>
		/// <param name="complexType">A xml schema complex type</param>
        private void AddComplexTypeNodes(MetaDataTreeNode parentNode, XMLSchemaComplexType complexType)
		{
            MetaDataTreeNode childTreeNode;
            XMLSchemaComplexType childComplexType;

            foreach (XMLSchemaElement schemaElement in complexType.Elements)
			{
                childComplexType = (XMLSchemaComplexType)_xmlSchemaModel.ComplexTypes[schemaElement.Caption];
                if (childComplexType != null &&
                    childComplexType.Name != schemaElement.Name)
                {
                    // make sure both caption and name have to be the same
                    childComplexType = null;
                }

                if (childComplexType == null && _showAttributes)
                {
                    // simple type element, add as an child node
                    childTreeNode = CreateTreeNode(NodeType.XMLSchemaElement, schemaElement);

                    parentNode.Nodes.Add(childTreeNode);
                }
                else if (childComplexType != null)
                {
                    // complex type element, add as an child node
                    childTreeNode = CreateTreeNode(NodeType.XMLSchemaComplexType, schemaElement);

                    parentNode.Nodes.Add(childTreeNode);

                    // complex type element,call the method recursively
                    AddComplexTypeNodes(childTreeNode, childComplexType);
                }
			}
		}
	}
}