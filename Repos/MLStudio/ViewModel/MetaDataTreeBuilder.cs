
namespace Newtera.MLStudio.ViewModel
{
	using System;
	using System.Collections;
	using System.Resources;
	using System.Threading;
    using System.Windows.Controls;
	
	/// <summary>
	/// Builds a tree structure using TreeViewItem objects according to a
	/// Schema Model.
	/// </summary>
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
		/// Builds a tree with a schema model tree nodes
		/// </summary>
		/// <param name="metaData">The meta data that a tree is to present</param>
		/// <returns>The root of the tree</returns>
		public TreeViewItem BuildTree(MetaDataModel metaData)
		{
			int nodeIndex = 0;
			TreeViewItem root = BuildTreeFrame(metaData.SchemaModel.SchemaInfo);

			// build classes and attributes nodes as children nodes
			_metaData = metaData;
			TreeViewItem classNode = (TreeViewItem) root.Nodes[nodeIndex++];
			SchemaModelElementCollection rootClasses = metaData.SchemaModel.RootClasses;

			// display the class nodes based on their display positions
			foreach (ClassElement classElement in rootClasses)
			{
				AddClassNode(classNode, classElement);
			}

			return root;
		}

		/// <summary>
		/// Create a TreeViewItem
		/// </summary>
		/// <param name="metaDataElement">The corresponding meta data element.</param>
		/// <returns>The created tree node</returns>
		public TreeViewItem CreateTreeNode(IMetaDataElement metaDataElement)
		{
			TreeViewItem treeNode = new TreeViewItem(metaDataElement);

			SetTreeNodeProperties(treeNode);

			_mapping[metaDataElement] = treeNode;

			return treeNode;
		}

		/// <summary>
		/// Create a TreeViewItem
		/// </summary>
		/// <param name="type">The tree node type</param>
		/// <param name="metaDataElement">The corresponding meta data element.</param>
		/// <returns>The created tree node</returns>
		private TreeViewItem CreateTreeNode(NodeType type, IMetaDataElement metaDataElement)
		{
			TreeViewItem treeNode = new TreeViewItem(type, metaDataElement);

			SetTreeNodeProperties(treeNode);

			_mapping[metaDataElement] = treeNode;

			return treeNode;
		}

		/// <summary>
		/// Create a tree node
		/// </summary>
		/// <param name="type">The type of tree node</param>
		/// <returns>The created tree node</returns>
		private TreeViewItem CreateTreeNode(NodeType type)
		{
			TreeViewItem treeNode = new TreeViewItem(type);

			SetTreeNodeProperties(treeNode);

			return treeNode;
		}

        /// <summary>
        /// Create a tree node given a type and display text
        /// </summary>
        /// <param name="type">The type of tree node</param>
        /// <param name="text">The display text of tree node</param>
        /// <returns>The created tree node</returns>
        private TreeViewItem CreateTreeNode(NodeType type, string text)
        {
            TreeViewItem treeNode = new TreeViewItem(type);

            treeNode.Text = text;
            SetTreeNodeProperties(treeNode);

            return treeNode;
        }

		/// <summary>
		/// Set other properties of the tree node, such as imageindex
		/// </summary>
		/// <param name="treeNode"></param>
		private void SetTreeNodeProperties(TreeViewItem treeNode)
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
			}
		}

		/// <summary>
		/// Build a schema model tree frame that consists of fixed nodes, such as schema node
		/// Class Node and Constraint Node, from which to add the schema model nodes.
		/// </summary>
		/// <param name="schemaInfo">The schema info instance</param>
		/// <returns>The root of the tree</returns>
		private TreeViewItem BuildTreeFrame(SchemaInfoElement schemaInfo)
		{
			TreeViewItem root = CreateTreeNode(NodeType.SchemaInfoNode, schemaInfo);

			TreeViewItem classesNode = CreateTreeNode(NodeType.ClassesFolder);

			root.Nodes.Add(classesNode);

			return root;
		}

		/// <summary>
		/// Add a class node as child node to the parent node. This method
		/// can be called recursively.
		/// </summary>
		/// <param name="parentNode">The parent node</param>
		/// <param name="classElement">The class element</param>
		private void AddClassNode(TreeViewItem parentNode, ClassElement classElement)
		{
			MetaDataElementSortedList sortedChildrenList = new MetaDataElementSortedList();

			// only add a tree node for a class to which the principal has
			// permission to read
			if (!CheckPermission || PermissionChecker.Instance.HasPermission(_metaData.XaclPolicy, classElement, XaclActionType.Read))
			{
				TreeViewItem classNode = CreateTreeNode(NodeType.ClassNode, classElement);

                // if the class belongs to a category, add the class tree node under
                // a folder represents a category
                if (!string.IsNullOrEmpty(classElement.Category))
                {
                    // Get or create a category node, and add the class node under the category node
                    TreeViewItem categoryNode = GetCategoryNode(parentNode, classElement);
                    categoryNode.Nodes.Add(classNode);
                }
                else
                {
                    // no category, add the class node directly under the parent class node
                    parentNode.Nodes.Add(classNode);
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
				TreeViewItem attributeNode;				
				foreach (IMetaDataElement metaElement in sortedChildrenList.Values)
				{
					if (metaElement is ClassElement)
					{
						// add a subclasse
						AddClassNode(classNode, (ClassElement) metaElement);
					}
				}
			}
		}

        /// <summary>
		/// Get a category node under the parent node, if the category node deosn't exist,
        /// create a category node, add it to the parent node, then return it.
		/// </summary>
		/// <param name="parentNode">The parent node</param>
		/// <param name="classElement">The class element</param>
        private TreeViewItem GetCategoryNode(TreeViewItem parentNode, ClassElement classElement)
        {
            TreeViewItem categoryNode = null;
            foreach (TreeViewItem childNode in parentNode.Nodes)
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