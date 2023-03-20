using System;
using System.Data;
using System.IO;
using System.Text;
using System.Drawing;
using System.Collections;
using System.ComponentModel;
using System.Windows.Forms;

using Newtera.WinClientCommon;
using Newtera.Common.MetaData;
using Newtera.Common.MetaData.Schema;
using Newtera.WindowsControl;

namespace Newtera.Studio.ImportExport
{
	/// <summary>
	/// Summary description for ChooseImportClassDialog.
	/// </summary>
	public class ChooseImportClassDialog : System.Windows.Forms.Form
	{
		private MetaDataModel _metaData = null;
		private string _rootClass;
		private ClassElement _selectedClass;
		private DataTable _srcDataTable;
		private bool _isDBAUser;
		private WorkInProgressDialog _workInProgressDialog;
		private bool _isRequestComplete;
		private MetaDataServiceStub _metaDataService;
		private ImportWizard _importWizard;

		private System.Windows.Forms.Button cancelButton;
		private System.Windows.Forms.ImageList treeImageList;
		private System.Windows.Forms.GroupBox groupBox2;
		private System.Windows.Forms.Button okButton;
		private System.Windows.Forms.TextBox className;
		private System.Windows.Forms.TreeView treeView;
		private System.Windows.Forms.Button createClsButton;
		private System.ComponentModel.IContainer components;

		public ChooseImportClassDialog()
		{
			//
			// Required for Windows Form Designer support
			//
			InitializeComponent();

			//
			// TODO: Add any constructor code after InitializeComponent call
			//
			_metaData = null;
			_rootClass = null;
			_selectedClass = null;
			_isDBAUser = false;
			_workInProgressDialog = new WorkInProgressDialog();
			_metaDataService = null;
			_importWizard = null;
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

		/// <summary>
		/// Gets or sets the DataTable instance that contains the data from
		/// a data source. This is used for creating a new import class
		/// </summary>
		public DataTable SourceDataTable
		{
			get
			{
				return this._srcDataTable;
			}
			set
			{
				this._srcDataTable = value;
			}
		}

		/// <summary>
		/// Gets or sets the information indicating whether the currently loggin user
		/// has DBA role.
		/// </summary>
		public bool IsDBAUser
		{
			get
			{
				return _isDBAUser;
			}
			set
			{
				_isDBAUser = value;
			}
		}

		/// <summary>
		/// Gets or sets the ImportWizard.
		/// </summary>
		public ImportWizard Wizard
		{
			get
			{
				return _importWizard;
			}
			set
			{
				_importWizard = value;
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(ChooseImportClassDialog));
            this.treeImageList = new System.Windows.Forms.ImageList(this.components);
            this.cancelButton = new System.Windows.Forms.Button();
            this.okButton = new System.Windows.Forms.Button();
            this.groupBox2 = new System.Windows.Forms.GroupBox();
            this.className = new System.Windows.Forms.TextBox();
            this.treeView = new System.Windows.Forms.TreeView();
            this.createClsButton = new System.Windows.Forms.Button();
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
            // createClsButton
            // 
            resources.ApplyResources(this.createClsButton, "createClsButton");
            this.createClsButton.Name = "createClsButton";
            this.createClsButton.Click += new System.EventHandler(this.createClsButton_Click);
            // 
            // ChooseImportClassDialog
            // 
            resources.ApplyResources(this, "$this");
            this.Controls.Add(this.createClsButton);
            this.Controls.Add(this.groupBox2);
            this.Controls.Add(this.okButton);
            this.Controls.Add(this.cancelButton);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.SizableToolWindow;
            this.Name = "ChooseImportClassDialog";
            this.Load += new System.EventHandler(this.ChooseImportClassDialog_Load);
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

		private void SaveClassToDatabase(ClassElement classElement)
		{
			// add the new class as subclass of selected class
			_selectedClass.AddSubclass(classElement);

			// set a fake TableName so that the SchemaEditor will treat it
			// as a class that a corresponding table in the database
			classElement.TableName = "New";

			StringBuilder builder;
			StringWriter writer;
			string[] xmlStrings = new string[12];

			builder = new StringBuilder();
			writer = new StringWriter(builder);
			_metaData.SchemaModel.Write(writer);
			// the first string is a xml string for schema model
			xmlStrings[0] = builder.ToString();

			xmlStrings[1] = "";
			xmlStrings[2] = "";
			xmlStrings[3] = "";
			xmlStrings[4] = "";
			xmlStrings[5] = "";
			xmlStrings[6] = "";
            xmlStrings[7] = "";
            xmlStrings[8] = "";
            xmlStrings[9] = "";
            xmlStrings[10] = "";
            xmlStrings[11] = "";

            _isRequestComplete = false;

			if (_metaDataService == null)
			{
				_metaDataService = new MetaDataServiceStub();
			}

            // invoke the web service asynchronously
            DateTime modifiedTime =  _metaDataService.SetMetaData(
				ConnectionStringBuilder.Instance.Create(_metaData.SchemaModel.SchemaInfo),
				xmlStrings);

            _metaData.SchemaModel.IsAltered = false;
            _metaData.SchemaInfo.ModifiedTime = modifiedTime;
            _metaData.SchemaModel.SchemaInfo.ModifiedTime = modifiedTime;

            RefreshClassTree();
        }

		/// <summary>
		/// Refresh the class tree to show the newly added class
		/// </summary>
		private void RefreshClassTree()
		{
			// it is the UI thread, continue
			// refresh the class tree to show the tree with the new class
			treeView.Nodes.Clear();
			this.ShowClassTree();

			RefreshMDIChildren();
		}

		/// <summary>
		/// Refresh the SchemaEditor and DataViewer in the DesignStudio
		/// </summary>
		private void RefreshMDIChildren()
		{
			_importWizard.DesignStudio.RefreshMDIChildMetaDataTree(this._metaData.SchemaInfo.NameAndVersion);
		}

		/// <summary>
		/// Show the working dialog
		/// </summary>
		/// <remarks>It has to deal with multi-threading issue</remarks>
		private void ShowWorkingDialog()
		{
			lock (_workInProgressDialog)
			{
				// check _isRequestComplete flag in case the worker thread
				// completes the request before the working dialog is shown
				if (!_isRequestComplete && !_workInProgressDialog.Visible)
				{
					_workInProgressDialog.ShowDialog();
				}
			}
		}

		private delegate void HideWorkingDialogDelegate();

		/// <summary>
		/// Hide the working dialog
		/// </summary>
		/// <remarks>Has to condider multi-threading issue</remarks>
		private void HideWorkingDialog()
		{
			if (this.InvokeRequired == false)
			{
				// It is the UI thread, go ahead to close the working dialog
				// lock it while updating _isRequestComplete
				lock(_workInProgressDialog)
				{
                    if (_workInProgressDialog.Visible)
                    {
                        _workInProgressDialog.Close();
                    }
					_isRequestComplete = true;
				}
			}
			else
			{
				// It is a worker thread, pass the control to the UI thread
				HideWorkingDialogDelegate hideWorkingDialog = new HideWorkingDialogDelegate(HideWorkingDialog);
				this.BeginInvoke(hideWorkingDialog);
			}
		}

		private void ShowClassTree()
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

        /// <summary>
        /// Gets the information indicating if the class is an empty class
        /// (without instances).
        /// </summary>
        /// <param name="classElement">The class element</param>
        /// <returns>true if the class is empty, false if it has instances</returns>
        internal bool IsClassEmpty(ClassElement classElement)
        {
            bool status = true;

            // if the class does not have a TableName, then it is just created and
            // has not been exported to the database, consider it is empty
            if (classElement.TableName != null && classElement.TableName.Length > 0)
            {
                try
                {
                    CMDataServiceStub dataService = new CMDataServiceStub();

                    // invoke the web service synchronously to get instance count
                    int count = dataService.GetInstanceCount(ConnectionStringBuilder.Instance.Create(_metaData.SchemaInfo),
                        classElement.Name);

                    if (count > 0)
                    {
                        status = false; // not empty
                    }
                }
                catch (Exception ex)
                {
                    MessageBox.Show(ex.Message, "Server Error",
                        MessageBoxButtons.OK,
                        MessageBoxIcon.Error);
                }
            }

            return status;
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

                // only DBA user is allowed to create a new subclass while the
                // a lock to the meta data is obtained
                if (this.IsDBAUser && _metaData.IsLockObtained)
                {
                    this.createClsButton.Enabled = true;
                }
                else
                {
                    this.createClsButton.Enabled = false;
                }
            }
            else
            {
                this.okButton.Enabled = false;
                this.createClsButton.Enabled = false;
            }
		}

		private void ChooseImportClassDialog_Load(object sender, System.EventArgs e)
		{
			this.ShowClassTree();
		}

		private void createClsButton_Click(object sender, System.EventArgs e)
		{
            if (_selectedClass.IsLeaf && !IsClassEmpty(_selectedClass))
            {
                // do not allow adding a subclass to a leaf class that already
                // has instances
                MessageBox.Show(Newtera.WindowsControl.MessageResourceManager.GetString("SchemaEditor.InvalidSubclass"),
                    "Info", MessageBoxButtons.OK,
                    MessageBoxIcon.Information);
            }
            else
            {
                CreateClassDialog dialog = new CreateClassDialog();
                dialog.SourceDataTable = this.SourceDataTable;
                dialog.MetaData = this.MetaData;
                dialog.ParentClass = this.SelectedClass;

                if (dialog.ShowDialog() == DialogResult.OK)
                {
                    // create a new class to the schema model
                    ClassElement classElement = this._metaData.SchemaModel.CreateClass(dialog.ClassName.Trim());
                    classElement.Caption = dialog.ClassCaption;
                    SchemaModelElementCollection attributes = dialog.Attributes;
                    foreach (AttributeElementBase attribute in attributes)
                    {
                        if (attribute is SimpleAttributeElement)
                        {
                            classElement.AddSimpleAttribute((SimpleAttributeElement)attribute);
                        }
                        else if (attribute is ArrayAttributeElement)
                        {
                            classElement.AddArrayAttribute((ArrayAttributeElement)attribute);
                        }
                    }

                    // save the new class to the database
                    SaveClassToDatabase(classElement);
                }
            }
        }
	}
}
