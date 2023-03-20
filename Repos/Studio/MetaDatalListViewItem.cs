/*
* @(#)MetaDataListViewItem.cs
*
* Copyright (c) 2003 Newtera, Inc. All rights reserved.
*
*/
namespace Newtera.Studio
{
	using System;
	using System.Windows.Forms;

    using Newtera.WinClientCommon;
	using Newtera.Common.MetaData;
	using Newtera.Common.MetaData.Schema;
	
	/// <summary>
	/// Represents a ListView item for a IMetaDataElement instance
	/// </summary>
	/// <version>  1.0.1 26 Sept 2003</version>
	/// <author>  Yong Zhang</author>
	public class MetaDataListViewItem : ListViewItem
	{
		private MetaDataTreeNode _treeNode;

		/// <summary>
		/// Initializes a new instance of the MetaDataListViewItem class.
		/// </summary>
		/// <param name="treeNode">The corresponding tree node</param>
		public MetaDataListViewItem(MetaDataTreeNode treeNode) : base(treeNode.Text)
		{
			_treeNode = treeNode;

			if (_treeNode.MetaDataElement != null)
			{
				_treeNode.MetaDataElement.CaptionChanged += new EventHandler(this.CaptionChangedHandler);
			}
		}

		/// <summary>
		/// Gets the type of tree node
		/// </summary>
		public NodeType Type
		{
			get
			{
				return _treeNode.Type;
			}
		}

		/// <summary>
		/// Get the tree node corresponding to the list view node
		/// </summary>
		public MetaDataTreeNode TreeNode
		{
			get
			{
				return _treeNode;
			}
		}

		/// <summary> 
		/// Gets or sets the schema model element.
		/// </summary>
		/// <value> The name of the element</value>
		public IMetaDataElement MetaDataElement
		{
			get
			{
				return _treeNode.MetaDataElement;
			}
		}

		/// <summary>
		/// A handler to call when a caption of the schema model element changed
		/// </summary>
		/// <param name="sender">the schema model element that cause the event</param>
		/// <param name="e">the arguments</param>
		private void CaptionChangedHandler(object sender, EventArgs e)
		{
			ValueChangedEventArgs args = (ValueChangedEventArgs) e;

			// change display text of the item
			this.Text = (string) args.NewValue;
		}
	}
}