/*
* @(#)MetaDataTreeNode.cs
*
* Copyright (c) 2003 Newtera, Inc. All rights reserved.
*
*/
namespace Newtera.WinClientCommon
{
	using System;
	using System.Windows.Forms;

	using Newtera.Common.MetaData;
	using Newtera.Common.MetaData.Schema;
	using Newtera.Common.MetaData.DataView;
	using Newtera.Common.MetaData.Mappings;
    using Newtera.Common.MetaData.XMLSchemaView;
	using Newtera.Common.MetaData.DataView.Taxonomy;
	
	/// <summary>
	/// Represents a tree node for a IMetaDataElement instance
	/// </summary>
	/// <version>  1.0.1 26 Sept 2003</version>
	/// <author>  Yong Zhang</author>
	public class MetaDataTreeNode : TreeNode
	{
		private NodeType _type;
		private IMetaDataElement _metaDataElement;

		/// <summary>
		/// Initializes a new instance of the MetaDataTreeNode class.
		/// </summary>
		/// <param name="type">The type of node</param>
		public MetaDataTreeNode(NodeType type)
		{
			_type = type;
			_metaDataElement = null;
		}

		/// <summary>
		/// Initializes a new instance of the MetaDataTreeNode class.
		/// </summary>
		/// <param name="type">The type of node</param>
		/// <param name="name">The schema model element</param>
		public MetaDataTreeNode(NodeType type, IMetaDataElement metaDataElement)
		{
			_type = type;
			_metaDataElement = metaDataElement;

			// listen to the caption changed event from the schema model element
			_metaDataElement.CaptionChanged += new EventHandler(this.CaptionChangedHandler);
		}

		/// <summary>
		/// Initializes a new instance of the MetaDataTreeNode class.
		/// </summary>
		/// <param name="name">The schema model element</param>
		public MetaDataTreeNode(IMetaDataElement metaDataElement)
		{
			_type = InferType(metaDataElement);
			_metaDataElement = metaDataElement;

			// listen to the caption changed event from the schema model element
			_metaDataElement.CaptionChanged += new EventHandler(this.CaptionChangedHandler);
		}

		/// <summary>
		/// Gets the type of node
		/// </summary>
		/// <value>One of NodeType enumeration</value>
		public NodeType Type
		{
			get
			{
				return _type;
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
				return _metaDataElement;
			}
			set
			{
				_metaDataElement = value;
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

			this.Text = (string) args.NewValue;
		}

		/// <summary>
		/// Infer the node type from a schema model element
		/// </summary>
		/// <param name="metaDataElement"></param>
		/// <returns></returns>
		private NodeType InferType(IMetaDataElement metaDataElement)
		{
			NodeType type = NodeType.Unknown;
			if (metaDataElement is ClassElement)
			{
				type = NodeType.ClassNode;
			}
			else if (metaDataElement is SimpleAttributeElement)
			{
				type = NodeType.SimpleAttributeNode;
			}
			else if (metaDataElement is RelationshipAttributeElement)
			{
				type = NodeType.RelationshipAttributeNode;
			}
			else if (metaDataElement is ArrayAttributeElement)
			{
				type = NodeType.ArrayAttributeNode;
			}
            else if (metaDataElement is VirtualAttributeElement)
            {
                type = NodeType.VirtualAttributeNode;
            }
            else if (metaDataElement is ImageAttributeElement)
            {
                type = NodeType.ImageAttributeNode;
            }
			else if (metaDataElement is SchemaInfoElement)
			{
				type = NodeType.SchemaInfoNode;
			}
			else if (metaDataElement is EnumElement)
			{
				type = NodeType.EnumConstraintNode;
			}
			else if (metaDataElement is RangeElement)
			{
				type = NodeType.RangeConstraintNode;
			}
			else if (metaDataElement is PatternElement)
			{
				type = NodeType.PatternConstraintNode;
			}
			else if (metaDataElement is ListElement)
			{
				type = NodeType.ListConstraintNode;
			}
			else if (metaDataElement is DataViewModel)
			{
				type = NodeType.DataViewNode;
			}
			else if (metaDataElement is TaxonomyModel)
			{
				type = NodeType.TaxonomyNode;
			}
			else if (metaDataElement is TaxonNode)
			{
				type = NodeType.TaxonNode;
			}
			else if (metaDataElement is Selector)
			{
				type = NodeType.SelectorNode;
			}
            else if (metaDataElement is XMLSchemaModel)
            {
                type = NodeType.XMLSchemaView;
            }

			return type;
		}
	}
}