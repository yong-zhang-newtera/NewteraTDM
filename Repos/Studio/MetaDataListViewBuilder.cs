/*
* @(#)MetaDataListViewBuilder.cs
*
* Copyright (c) 2003 Newtera, Inc. All rights reserved.
*/
namespace Newtera.Studio
{
	using System;
	using System.Resources;
	using System.Collections;
	using System.Windows.Forms;

    using Newtera.WinClientCommon;
	using Newtera.Common.MetaData;
	using Newtera.Common.MetaData.Schema;
	
	/// <summary>
	/// Builds a collection of MetaDataListViewItem objects for displaying
	/// in a ListView
	/// </summary>
	/// <version>  1.0.1 26 Sept 2003</version>
	/// <author>  Yong Zhang</author>
	public class MetaDataListViewBuilder
	{
		private ResourceManager _resources;

		/// <summary>
		/// Initializes a new instance of the MetaDataListViewBuilder class.
		/// </summary>
		public MetaDataListViewBuilder()
		{
			_resources = new ResourceManager(this.GetType());
		}

		/// <summary>
		/// Build list view items
		/// </summary>
		/// <param name="listView">The list view</param>
		/// <param name="treeNode">The Currently selected tree node</param>
		public void BuildItems(ListView listView, MetaDataTreeNode treeNode)
		{
			NodeType type = treeNode.Type;

			listView.Items.Clear(); // clear the old items

			TreeNodeCollection children = treeNode.Nodes;

			// tree node is sorted in ascending order based on their positions
			foreach (MetaDataTreeNode child in children)
			{
				listView.Items.Add(BuildItem(child));
			}
		}

		/// <summary>
		/// Builds a ListViewItem for a schema model element
		/// </summary>
		/// <param name="treeNode">The corresponding tree node</param>
		/// <returns>An ListViewItem</returns>
		public ListViewItem BuildItem(MetaDataTreeNode treeNode)
		{
			MetaDataListViewItem item = new MetaDataListViewItem(treeNode);

			switch (item.Type)
			{
				case NodeType.ClassNode:
					item.ImageIndex = 0;
					item.SubItems.Add(item.MetaDataElement.Name);
					item.SubItems.Add(_resources.GetString("ClassType.Text"));
					break;

				case NodeType.SimpleAttributeNode:
					item.ImageIndex = 1;
					item.SubItems.Add(item.MetaDataElement.Name);
					item.SubItems.Add(_resources.GetString("SimpleAttributeType.Text"));
					break;

				case NodeType.RelationshipAttributeNode:
					item.ImageIndex = 2;
					item.SubItems.Add(item.MetaDataElement.Name);
					item.SubItems.Add(_resources.GetString("RelationshipAttributeType.Text"));
					break;

				case NodeType.ArrayAttributeNode:
					item.ImageIndex = 13;
					item.SubItems.Add(item.MetaDataElement.Name);
					item.SubItems.Add(_resources.GetString("ArrayAttributeType.Text"));
					break;

                case NodeType.VirtualAttributeNode:
                    item.ImageIndex = 16;
                    item.SubItems.Add(item.MetaDataElement.Name);
                    item.SubItems.Add(_resources.GetString("VirtualAttributeType.Text"));
                    break;

                case NodeType.ImageAttributeNode:
                    item.ImageIndex = 17;
                    item.SubItems.Add(item.MetaDataElement.Name);
                    item.SubItems.Add(_resources.GetString("ImageAttributeType.Text"));
                    break;

				case NodeType.EnumConstraintNode:
					item.ImageIndex = 3;
					item.SubItems.Add(item.MetaDataElement.Name);
					item.SubItems.Add(_resources.GetString("EnumType.Text"));
					break;

				case NodeType.RangeConstraintNode:
					item.ImageIndex = 4;
					item.SubItems.Add(item.MetaDataElement.Name);
					item.SubItems.Add(_resources.GetString("RangeType.Text"));
					break;

				case NodeType.PatternConstraintNode:
					item.ImageIndex = 5;
					item.SubItems.Add(item.MetaDataElement.Name);
					item.SubItems.Add(_resources.GetString("PatternType.Text"));
					break;

				case NodeType.ListConstraintNode:
					item.ImageIndex = 14;
					item.SubItems.Add(item.MetaDataElement.Name);
					item.SubItems.Add(_resources.GetString("ListType.Text"));
					break;

				case NodeType.TaxonomyNode:
					item.ImageIndex = 9;
					item.SubItems.Add(item.MetaDataElement.Name);
					item.SubItems.Add(_resources.GetString("TaxonomyType.Text"));
					break;

				case NodeType.TaxonNode:
					item.ImageIndex = 10;
					item.SubItems.Add(item.MetaDataElement.Name);
					item.SubItems.Add(_resources.GetString("TaxonType.Text"));
					break;

				case NodeType.ClassesFolder:
					item.ImageIndex = 6;
					item.SubItems.Add(item.TreeNode.Text);
					item.SubItems.Add(_resources.GetString("FolderType.Text"));
					break;

				case NodeType.ConstraintsFolder:
					item.ImageIndex = 6;
					item.SubItems.Add(item.TreeNode.Text);
					item.SubItems.Add(_resources.GetString("FolderType.Text"));
				   break;

				case NodeType.TaxonomiesFolder:
					item.ImageIndex = 6;
					item.SubItems.Add(item.TreeNode.Text);
					item.SubItems.Add(_resources.GetString("FolderType.Text"));
					break;

				case NodeType.DataViewsFolder:
					item.ImageIndex = 6;
					item.SubItems.Add(item.TreeNode.Text);
					item.SubItems.Add(_resources.GetString("FolderType.Text"));
					break;

				case NodeType.SelectorsFolder:
					item.ImageIndex = 6;
					item.SubItems.Add(item.TreeNode.Text);
					item.SubItems.Add(_resources.GetString("FolderType.Text"));
					break;

				case NodeType.DataViewNode:
					item.ImageIndex = 11;
					item.SubItems.Add(item.MetaDataElement.Name);
					item.SubItems.Add(_resources.GetString("DataViewType.Text"));
					break;

				case NodeType.SelectorNode:
					item.ImageIndex = 15;
					item.SubItems.Add(item.MetaDataElement.Name);
					item.SubItems.Add(_resources.GetString("SelectorType.Text"));
					break;

                case NodeType.CategoryFolder:
                    item.ImageIndex = 6;
                    item.SubItems.Add(item.TreeNode.Text);
                    item.SubItems.Add(_resources.GetString("FolderType.Text"));
                    break;

                case NodeType.XMLSchemaView:
                    item.ImageIndex = 10;
                    item.SubItems.Add(item.MetaDataElement.Name);
                    item.SubItems.Add(_resources.GetString("XMLSchemaView.Text"));
                    break;

                case NodeType.XMLSchemaComplexType:
                    item.ImageIndex = 11;
                    item.SubItems.Add(item.MetaDataElement.Name);
                    item.SubItems.Add(_resources.GetString("XMLSchemaComplexType.Text"));
                    break;
			}

			return item;
		}
	}
}