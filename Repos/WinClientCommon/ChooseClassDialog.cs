using System;
using System.Drawing;
using System.Collections;
using System.ComponentModel;
using System.Windows.Forms;

using Newtera.Common.MetaData;
using Newtera.Common.MetaData.Schema;

namespace Newtera.WinClientCommon
{
	/// <summary>
	/// Summary description for ChooseClassDialog.
	/// </summary>
	public class ChooseClassDialog : System.Windows.Forms.Form
	{
		private MetaDataModel _metaData = null;
		private string _rootClass;
		private ClassElement _selectedClass;

		private System.Windows.Forms.Button cancelButton;
		private System.Windows.Forms.ImageList treeImageList;
		private System.Windows.Forms.GroupBox groupBox2;
		private System.Windows.Forms.Button okButton;
		private System.Windows.Forms.TextBox className;
		private System.Windows.Forms.TreeView treeView;
		private System.ComponentModel.IContainer components;

		public ChooseClassDialog()
		{
			//
			// Required for Windows Form Designer support
			//
			InitializeComponent();

			_metaData = null;
			_rootClass = null;
			_selectedClass = null;
		}

		/// <summary>
		/// Clean up any resources being used.
		/// </summary>
		protected override void Dispose( bool disposing )
		{
			if( disposing )
			{
				if(components != null)
				{
					components.Dispose();
				}
			}
			base.Dispose( disposing );
		}

		/// <summary>
		/// Gets or sets the meta data model from which to get classes
		/// </summary>
		public MetaDataModel MetaData
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
		/// Get the selected class
		/// </summary>
		public ClassElement SelectedClass
		{
			get
			{
				return _selectedClass;
			}
			set
			{
				_selectedClass = value;
			}
		}

		/// <summary>
		/// Get the selected class name
		/// </summary>
		public string SelectedClassName
		{
			get
			{
				return _selectedClass.Name;
			}
		}

		/// <summary>
		/// Gets or sets the root class of the display tree
		/// </summary>
		public string RootClass
		{
			get
			{
				return _rootClass;
			}
			set
			{
				_rootClass = value;
			}
		}

		#region Windows Form Designer generated code
		/// <summary>
		/// Required method for Designer support - do not modify
		/// the contents of this method with the code editor.
		/// </summary>
		private void InitializeComponent()
		{
            this.components = new System.ComponentModel.Container();
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(ChooseClassDialog));
            this.treeImageList = new System.Windows.Forms.ImageList(this.components);
            this.cancelButton = new System.Windows.Forms.Button();
            this.okButton = new System.Windows.Forms.Button();
            this.groupBox2 = new System.Windows.Forms.GroupBox();
            this.className = new System.Windows.Forms.TextBox();
            this.treeView = new System.Windows.Forms.TreeView();
            this.groupBox2.SuspendLayout();
            this.SuspendLayout();
            // 
            // treeImageList
            // 
            this.treeImageList.ImageStream = ((System.Windows.Forms.ImageListStreamer)(resources.GetObject("treeImageList.ImageStream")));
            this.treeImageList.TransparentColor = System.Drawing.Color.Transparent;
            this.treeImageList.Images.SetKeyName(0, "");
            this.treeImageList.Images.SetKeyName(1, "");
            this.treeImageList.Images.SetKeyName(2, "");
            this.treeImageList.Images.SetKeyName(3, "");
            // 
            // cancelButton
            // 
            resources.ApplyResources(this.cancelButton, "cancelButton");
            this.cancelButton.DialogResult = System.Windows.Forms.DialogResult.Cancel;
            this.cancelButton.Name = "cancelButton";
            // 
            // okButton
            // 
            resources.ApplyResources(this.okButton, "okButton");
            this.okButton.DialogResult = System.Windows.Forms.DialogResult.OK;
            this.okButton.Name = "okButton";
            // 
            // groupBox2
            // 
            resources.ApplyResources(this.groupBox2, "groupBox2");
            this.groupBox2.Controls.Add(this.className);
            this.groupBox2.Controls.Add(this.treeView);
            this.groupBox2.Name = "groupBox2";
            this.groupBox2.TabStop = false;
            // 
            // className
            // 
            resources.ApplyResources(this.className, "className");
            this.className.Name = "className";
            this.className.ReadOnly = true;
            // 
            // treeView
            // 
            resources.ApplyResources(this.treeView, "treeView");
            this.treeView.HideSelection = false;
            this.treeView.ImageList = this.treeImageList;
            this.treeView.ItemHeight = 16;
            this.treeView.Name = "treeView";
            this.treeView.AfterSelect += new System.Windows.Forms.TreeViewEventHandler(this.treeView_AfterSelect);
            // 
            // ChooseClassDialog
            // 
            resources.ApplyResources(this, "$this");
            this.Controls.Add(this.groupBox2);
            this.Controls.Add(this.okButton);
            this.Controls.Add(this.cancelButton);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.SizableToolWindow;
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "ChooseClassDialog";
            this.Load += new System.EventHandler(this.ChooseClassDialog_Load);
            this.groupBox2.ResumeLayout(false);
            this.groupBox2.PerformLayout();
            this.ResumeLayout(false);

		}
		#endregion

		#region Controller code
		private MetaDataTreeNode FindTreeNode(MetaDataTreeNode root, ClassElement classElement)
		{
			foreach (MetaDataTreeNode node in root.Nodes)
			{
				if (classElement == node.MetaDataElement)
				{
					return node;
				}
				else
				{
					MetaDataTreeNode found = FindTreeNode(node, classElement);
					if (found != null)
					{
						return found;
					}
				}
			}

			return null;
		}

		#endregion

		private void treeView_AfterSelect(object sender, System.Windows.Forms.TreeViewEventArgs e)
		{
			// Get the selected node
			MetaDataTreeNode node = (MetaDataTreeNode) e.Node;
			
			ClassElement theClass = node.MetaDataElement as ClassElement;

            if (theClass != null && theClass.Name != _rootClass)
            {
                this.className.Text = theClass.Caption;

                _selectedClass = theClass;

                this.okButton.Enabled = true;
            }
            else
            {
                // selected node is not a class node
                this.className.Text = null;

                _selectedClass = null;

                this.okButton.Enabled = false;
            }
		}

		private void ChooseClassDialog_Load(object sender, System.EventArgs e)
		{
			if (_metaData != null && _rootClass != null)
			{
				MetaDataTreeBuilder builder = new MetaDataTreeBuilder();

				// do not show attributes and constraints nodes
				builder.IsAttributesShown = false;
				builder.IsConstraintsShown = false;
				builder.IsTaxonomiesShown = false;

				MetaDataTreeNode root = (MetaDataTreeNode) builder.BuildClassTree(_metaData, _rootClass);

				treeView.BeginUpdate();
				treeView.Nodes.Clear();
				treeView.Nodes.Add(root);

				if (_selectedClass != null)
				{
					// make the initial selection
					TreeNode selectedNode = FindTreeNode(root, _selectedClass);
					if (selectedNode != null)
					{
						treeView.SelectedNode = selectedNode;
					}
				}
				else
				{
					// expand the root node
					if (treeView.Nodes.Count > 0)
					{
						treeView.Nodes[0].Expand();
					}
				}

				treeView.EndUpdate();
			}		
		}
	}
}
