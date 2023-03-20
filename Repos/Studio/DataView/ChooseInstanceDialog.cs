using System;
using System.IO;
using System.Xml;
using System.Data;
using System.Drawing;
using System.Collections;
using System.ComponentModel;
using System.Windows.Forms;

using Newtera.Common.Core;
using Newtera.Common.MetaData;
using Newtera.Common.MetaData.Schema;
using Newtera.Common.MetaData.DataView;
using Newtera.Studio.UserControls;
using Newtera.WinClientCommon;
using Newtera.WindowsControl;

namespace Newtera.Studio
{
	/// <summary>
	/// Summary description for ChooseInstanceDialog.
	/// </summary>
	public class ChooseInstanceDialog : System.Windows.Forms.Form
	{
		private ClassElement _linkedClass;
        private ClassElement _leafClass;
        private SchemaModelElementCollection _leafClasses;
		private InstanceData _selectedInstance;
		private DataViewSlide _dataSlide;
		private MenuItemStates _menuItemStates;
		private SchemaModel _schemaModel;
		private WorkInProgressDialog _workInProgressDialog;
		private bool _isRequestComplete;
		private CMDataServiceStub _dataService;

		private System.Windows.Forms.TabControl tabControl;
		private System.Windows.Forms.TabPage resultTabPage;
		private System.Windows.Forms.TabPage searchTabPage;
		private System.Windows.Forms.Button okButton;
		private System.Windows.Forms.Button cancelButton;
		private System.Windows.Forms.Panel panel1;
		private System.Windows.Forms.Splitter splitter1;
		private System.Windows.Forms.Panel panel2;
		private System.Windows.Forms.DataGrid dataGrid1;
		private System.Windows.Forms.Button prevPageButton;
		private System.Windows.Forms.Button prevRowButton;
		private System.Windows.Forms.Button nextRowButton;
		private System.Windows.Forms.Button nextPageButton;
		private System.Windows.Forms.Button countRowButton;
		private System.Windows.Forms.TextBox countTextBox;
		private SearchPanel searchPanel;
		private System.Windows.Forms.Button findButton;
		private System.Windows.Forms.PropertyGrid rowViewGrid;
        private Panel panel3;
        private Label label1;
        private ComboBox bottomClassComboBox;
        private ToolTip toolTip1;
        private IContainer components;

		public ChooseInstanceDialog()
		{
			//
			// Required for Windows Form Designer support
			//
			InitializeComponent();

			//
			// TODO: Add any constructor code after InitializeComponent call
			//
			_linkedClass = null;
            _leafClass = null;
            _leafClasses = null;
			_selectedInstance = null;
			_schemaModel = null;
			_menuItemStates = new MenuItemStates();
			_dataService = new CMDataServiceStub();
			_workInProgressDialog = new WorkInProgressDialog();
		}

		~ChooseInstanceDialog()
		{
			_workInProgressDialog.Dispose();
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
		/// Gets or sets the class from which to choose an instance
		/// </summary>
		public ClassElement LinkedClass
		{
			get
			{
				return _linkedClass;
			}
			set
			{
				_linkedClass = value;
			}
		}

		/// <summary>
		/// Gets or sets the schema model
		/// </summary>
		public SchemaModel SchemaModel
		{
			get
			{
				return _schemaModel;
			}
			set
			{
				_schemaModel = value;
			}
		}

		/// <summary>
		/// Gets the selected instance
		/// </summary>
		public InstanceData SelectedInstance
		{
			get
			{
				return _selectedInstance;
			}
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

		#region Windows Form Designer generated code
		/// <summary>
		/// Required method for Designer support - do not modify
		/// the contents of this method with the code editor.
		/// </summary>
		private void InitializeComponent()
		{
            this.components = new System.ComponentModel.Container();
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(ChooseInstanceDialog));
            this.tabControl = new System.Windows.Forms.TabControl();
            this.resultTabPage = new System.Windows.Forms.TabPage();
            this.rowViewGrid = new System.Windows.Forms.PropertyGrid();
            this.splitter1 = new System.Windows.Forms.Splitter();
            this.panel1 = new System.Windows.Forms.Panel();
            this.dataGrid1 = new System.Windows.Forms.DataGrid();
            this.panel3 = new System.Windows.Forms.Panel();
            this.bottomClassComboBox = new System.Windows.Forms.ComboBox();
            this.label1 = new System.Windows.Forms.Label();
            this.panel2 = new System.Windows.Forms.Panel();
            this.countTextBox = new System.Windows.Forms.TextBox();
            this.countRowButton = new System.Windows.Forms.Button();
            this.nextPageButton = new System.Windows.Forms.Button();
            this.nextRowButton = new System.Windows.Forms.Button();
            this.prevRowButton = new System.Windows.Forms.Button();
            this.prevPageButton = new System.Windows.Forms.Button();
            this.searchTabPage = new System.Windows.Forms.TabPage();
            this.findButton = new System.Windows.Forms.Button();
            this.searchPanel = new Newtera.Studio.UserControls.SearchPanel();
            this.okButton = new System.Windows.Forms.Button();
            this.cancelButton = new System.Windows.Forms.Button();
            this.toolTip1 = new System.Windows.Forms.ToolTip(this.components);
            this.tabControl.SuspendLayout();
            this.resultTabPage.SuspendLayout();
            this.panel1.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.dataGrid1)).BeginInit();
            this.panel3.SuspendLayout();
            this.panel2.SuspendLayout();
            this.searchTabPage.SuspendLayout();
            this.SuspendLayout();
            // 
            // tabControl
            // 
            resources.ApplyResources(this.tabControl, "tabControl");
            this.tabControl.Controls.Add(this.resultTabPage);
            this.tabControl.Controls.Add(this.searchTabPage);
            this.tabControl.Name = "tabControl";
            this.tabControl.SelectedIndex = 0;
            // 
            // resultTabPage
            // 
            this.resultTabPage.Controls.Add(this.rowViewGrid);
            this.resultTabPage.Controls.Add(this.splitter1);
            this.resultTabPage.Controls.Add(this.panel1);
            resources.ApplyResources(this.resultTabPage, "resultTabPage");
            this.resultTabPage.Name = "resultTabPage";
            // 
            // rowViewGrid
            // 
            resources.ApplyResources(this.rowViewGrid, "rowViewGrid");
            this.rowViewGrid.LineColor = System.Drawing.SystemColors.ScrollBar;
            this.rowViewGrid.Name = "rowViewGrid";
            this.rowViewGrid.ToolbarVisible = false;
            // 
            // splitter1
            // 
            resources.ApplyResources(this.splitter1, "splitter1");
            this.splitter1.Name = "splitter1";
            this.splitter1.TabStop = false;
            // 
            // panel1
            // 
            this.panel1.Controls.Add(this.dataGrid1);
            this.panel1.Controls.Add(this.panel3);
            this.panel1.Controls.Add(this.panel2);
            resources.ApplyResources(this.panel1, "panel1");
            this.panel1.Name = "panel1";
            // 
            // dataGrid1
            // 
            this.dataGrid1.AllowNavigation = false;
            resources.ApplyResources(this.dataGrid1, "dataGrid1");
            this.dataGrid1.DataMember = "";
            this.dataGrid1.HeaderForeColor = System.Drawing.SystemColors.ControlText;
            this.dataGrid1.Name = "dataGrid1";
            this.dataGrid1.ReadOnly = true;
            this.dataGrid1.Click += new System.EventHandler(this.dataGrid1_Click);
            // 
            // panel3
            // 
            resources.ApplyResources(this.panel3, "panel3");
            this.panel3.Controls.Add(this.bottomClassComboBox);
            this.panel3.Controls.Add(this.label1);
            this.panel3.Name = "panel3";
            // 
            // bottomClassComboBox
            // 
            resources.ApplyResources(this.bottomClassComboBox, "bottomClassComboBox");
            this.bottomClassComboBox.FormattingEnabled = true;
            this.bottomClassComboBox.Name = "bottomClassComboBox";
            this.bottomClassComboBox.SelectedIndexChanged += new System.EventHandler(this.bottomClassComboBox_SelectedIndexChanged);
            // 
            // label1
            // 
            resources.ApplyResources(this.label1, "label1");
            this.label1.Name = "label1";
            // 
            // panel2
            // 
            this.panel2.Controls.Add(this.countTextBox);
            this.panel2.Controls.Add(this.countRowButton);
            this.panel2.Controls.Add(this.nextPageButton);
            this.panel2.Controls.Add(this.nextRowButton);
            this.panel2.Controls.Add(this.prevRowButton);
            this.panel2.Controls.Add(this.prevPageButton);
            resources.ApplyResources(this.panel2, "panel2");
            this.panel2.Name = "panel2";
            // 
            // countTextBox
            // 
            resources.ApplyResources(this.countTextBox, "countTextBox");
            this.countTextBox.Name = "countTextBox";
            this.countTextBox.ReadOnly = true;
            // 
            // countRowButton
            // 
            resources.ApplyResources(this.countRowButton, "countRowButton");
            this.countRowButton.Name = "countRowButton";
            this.countRowButton.Click += new System.EventHandler(this.countRowButton_Click);
            // 
            // nextPageButton
            // 
            resources.ApplyResources(this.nextPageButton, "nextPageButton");
            this.nextPageButton.Name = "nextPageButton";
            this.toolTip1.SetToolTip(this.nextPageButton, resources.GetString("nextPageButton.ToolTip"));
            this.nextPageButton.Click += new System.EventHandler(this.nextPageButton_Click);
            // 
            // nextRowButton
            // 
            resources.ApplyResources(this.nextRowButton, "nextRowButton");
            this.nextRowButton.Name = "nextRowButton";
            this.toolTip1.SetToolTip(this.nextRowButton, resources.GetString("nextRowButton.ToolTip"));
            this.nextRowButton.Click += new System.EventHandler(this.nextRowButton_Click);
            // 
            // prevRowButton
            // 
            resources.ApplyResources(this.prevRowButton, "prevRowButton");
            this.prevRowButton.Name = "prevRowButton";
            this.toolTip1.SetToolTip(this.prevRowButton, resources.GetString("prevRowButton.ToolTip"));
            this.prevRowButton.Click += new System.EventHandler(this.prevRowButton_Click);
            // 
            // prevPageButton
            // 
            resources.ApplyResources(this.prevPageButton, "prevPageButton");
            this.prevPageButton.Name = "prevPageButton";
            this.toolTip1.SetToolTip(this.prevPageButton, resources.GetString("prevPageButton.ToolTip"));
            this.prevPageButton.Click += new System.EventHandler(this.prevPageButton_Click);
            // 
            // searchTabPage
            // 
            this.searchTabPage.Controls.Add(this.findButton);
            this.searchTabPage.Controls.Add(this.searchPanel);
            resources.ApplyResources(this.searchTabPage, "searchTabPage");
            this.searchTabPage.Name = "searchTabPage";
            // 
            // findButton
            // 
            resources.ApplyResources(this.findButton, "findButton");
            this.findButton.Name = "findButton";
            this.findButton.Click += new System.EventHandler(this.findButton_Click);
            // 
            // searchPanel
            // 
            resources.ApplyResources(this.searchPanel, "searchPanel");
            this.searchPanel.BorderStyle = System.Windows.Forms.BorderStyle.Fixed3D;
            this.searchPanel.Name = "searchPanel";
            this.searchPanel.SelectedDataView = null;
            // 
            // okButton
            // 
            resources.ApplyResources(this.okButton, "okButton");
            this.okButton.DialogResult = System.Windows.Forms.DialogResult.OK;
            this.okButton.Name = "okButton";
            // 
            // cancelButton
            // 
            resources.ApplyResources(this.cancelButton, "cancelButton");
            this.cancelButton.DialogResult = System.Windows.Forms.DialogResult.Cancel;
            this.cancelButton.Name = "cancelButton";
            // 
            // ChooseInstanceDialog
            // 
            resources.ApplyResources(this, "$this");
            this.Controls.Add(this.cancelButton);
            this.Controls.Add(this.okButton);
            this.Controls.Add(this.tabControl);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.SizableToolWindow;
            this.Name = "ChooseInstanceDialog";
            this.Load += new System.EventHandler(this.ChooseInstanceDialog_Load);
            this.tabControl.ResumeLayout(false);
            this.resultTabPage.ResumeLayout(false);
            this.panel1.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.dataGrid1)).EndInit();
            this.panel3.ResumeLayout(false);
            this.panel2.ResumeLayout(false);
            this.panel2.PerformLayout();
            this.searchTabPage.ResumeLayout(false);
            this.ResumeLayout(false);

		}
		#endregion

		#region Controller code

		private void MenuItemStateChanged(object sender, System.EventArgs e)
		{
			StateChangedEventArgs args = (StateChangedEventArgs) e;

			// set the toolbar button states
			switch (args.ID)
			{
				case MenuItemID.ViewNextRow:
					this.nextRowButton.Enabled = args.State;
					break;
				case MenuItemID.ViewPrevRow:
					this.prevRowButton.Enabled = args.State;
					break;
				case MenuItemID.ViewPrevPage:
					this.prevPageButton.Enabled = args.State;
					break;
				case MenuItemID.ViewNextPage:
					this.nextPageButton.Enabled = args.State;
					break;
				case MenuItemID.ViewRowCount:
					this.countRowButton.Enabled = args.State;
					break;		
			}
		}

		private void ExecuteQuery()
		{
			if (_dataSlide.DataView != null)
			{
				string query = _dataSlide.DataView.SearchQuery;

				_isRequestComplete = false;

                // invoke the web service asynchronously
                XmlNode xmlNode =  _dataService.ExecuteQuery(ConnectionStringBuilder.Instance.Create(_schemaModel.SchemaInfo),
					query);

                DataSet ds = new DataSet();

                XmlReader xmlReader = new XmlNodeReader(xmlNode);
                ds.ReadXml(xmlReader);

                ShowQueryResult(ds);
            }
		}

		/// <summary>
		/// Display the query result data set in a data grid
		/// </summary>
		/// <param name="dataSet">The query result in DataSet</param>
		private void ShowQueryResult(DataSet dataSet)
		{
			// The data grid need a table style to display the data, check
			// if a table style has alreadt existed for the base class in
			// the data view, if not, create a new table style and adds it
			// to the data grid
			if (!this.dataGrid1.TableStyles.Contains(_dataSlide.DataView.BaseClass.ClassName))
			{
				DataGridTableStyle tableStyle = TableStyleFactory.Instance.Create(_dataSlide.DataView);

				this.dataGrid1.TableStyles.Add(tableStyle);
			}

			_dataSlide.DataSet = dataSet;
			_dataSlide.SelectedRowIndex = 0;

			ShowDataSlide();
		}

		/// <summary>
		/// display the current data slide in the grid
		/// </summary>
		private void ShowDataSlide()
		{
            string captionText = _dataSlide.DataView.BaseClass.Caption;
            this.dataGrid1.CaptionText = captionText;

			// make sure there is a data to show
			if (!_dataSlide.IsEmptyResult)
			{
				this.dataGrid1.SuspendLayout();

				this.dataGrid1.SetDataBinding(_dataSlide.DataSet, _dataSlide.DataView.BaseClass.ClassName);

				this.dataGrid1.Select(_dataSlide.SelectedRowIndex);

				this.dataGrid1.ResumeLayout(true);

				// set the count to unknow if the total count is unknow
				if (_dataSlide.TotalInstanceCount < 0)
				{
					this.countTextBox.Text = "???";
				}
				else
				{
					this.countTextBox.Text = _dataSlide.TotalInstanceCount + "";
				}

				// display the currently selected row
				ShowRowView();

				this.okButton.Enabled = true;

				// switch to Result tab
				this.tabControl.SelectedIndex = 0;
			}
			else
			{
				MessageBox.Show(MessageResourceManager.GetString("DataViewer.NoResults"));

                this.dataGrid1.DataSource = null;
                this.countTextBox.Text = "0";
                this.okButton.Enabled = false;
			}
		}

		/// <summary>
		/// Display the currently selected row in row view
		/// </summary>
		private void ShowRowView()
		{
            if (!_dataSlide.IsEmptyResult)
            {
                InstanceView rowView = (InstanceView)this.rowViewGrid.SelectedObject;
                if (rowView != _dataSlide.RowView)
                {
                    this.rowViewGrid.SelectedObject = _dataSlide.RowView;

                    _selectedInstance = _dataSlide.RowView.InstanceData;
                }
                else
                {
                    // just refresh the view
                    this.rowViewGrid.Refresh();
                }
            }
            else
            {
                this.rowViewGrid.SelectedObject = null;
            }
		}

		/// <summary>
		/// Gets and display the total instance count of the current query.
		/// </summary>
		internal void ExecuteQueryCount()
		{
			if (_dataSlide.DataView != null)
			{	

				string query = _dataSlide.DataView.SearchQuery;

				_isRequestComplete = false;

				// invoke the web service asynchronously
				int count = _dataService.ExecuteCount(ConnectionStringBuilder.Instance.Create(_schemaModel.SchemaInfo),
					query);

                ExecuteCountDone(count);
            }
		}

		/// <summary>
		/// The AsyncCallback event handler for ExecuteCount web service method.
		/// </summary>
		/// <param name="res">The result</param>
		private void ExecuteCountDone(int count)
		{
			try
			{			
				ShowInstanceCount(count);
			}
			catch (Exception ex)
			{
				MessageBox.Show(ex.Message, "Server Error",
					MessageBoxButtons.OK,
					MessageBoxIcon.Error);
			}
		}

		private delegate void ShowInstanceCountDelegate(int count);

		/// <summary>
		/// Display the instance count of current query
		/// </summary>
		/// <param name="count">The count.</param>
		private void ShowInstanceCount(int count)
		{
			if (this.InvokeRequired == false)
			{
				// it is the UI thread, continue

				this.countTextBox.Text = count + "";
				_dataSlide.TotalInstanceCount = count;
			}
			else
			{
				// It is a Worker thread, pass the control to UI thread
				ShowInstanceCountDelegate showCount = new ShowInstanceCountDelegate(ShowInstanceCount);

				this.BeginInvoke(showCount, new object[] {count});
			}
		}

        private void CreateDataSlide(string className)
        {
            _dataSlide = new DataViewSlide(_menuItemStates);

            _dataSlide.DataView = MetaDataStore.Instance.GetMetaData(_schemaModel.SchemaInfo.Name,
                _schemaModel.SchemaInfo.Version).GetDetailedDataView(className);

            this.searchPanel.SelectedDataView = _dataSlide.DataView;

            this.rowViewGrid.SelectedObject = null;
        }

        private void DisplayLeafClasses(ClassElement baseClass)
        {
            bottomClassComboBox.Items.Add(MessageResourceManager.GetString("DataViewer.AllLeafClasses"));
            if (!baseClass.IsLeaf)
            {
                _leafClasses = MetaDataStore.Instance.GetMetaData(_schemaModel.SchemaInfo.Name,
                    _schemaModel.SchemaInfo.Version).GetBottomClasses(baseClass.Name);
                if (_leafClasses != null)
                {
                    foreach (ClassElement leafClass in _leafClasses)
                    {
                        bottomClassComboBox.Items.Add(leafClass.Caption);
                    }
                }

                bottomClassComboBox.SelectedIndex = 0;
            }
        }

		#endregion

		private void ChooseInstanceDialog_Load(object sender, System.EventArgs e)
		{
			if (_linkedClass != null)
			{
				// Register the menu item state change event handler
				_menuItemStates.StateChanged += new EventHandler(this.MenuItemStateChanged);

                CreateDataSlide(_linkedClass.Name);

                // display the leaf classes in the combobox
                DisplayLeafClasses(_linkedClass);

				ExecuteQuery();
			}		
		}

		private void dataGrid1_Click(object sender, System.EventArgs e)
		{
			int selectedRowIndex = this.dataGrid1.CurrentRowIndex;

			if (_dataSlide.SelectedRowIndex != selectedRowIndex)
			{
				_dataSlide.SelectedRowIndex = selectedRowIndex;

				ShowRowView();
			}
		}

		private void prevRowButton_Click(object sender, System.EventArgs e)
		{
			int rowIndex = this.dataGrid1.CurrentRowIndex;
			if (rowIndex > 0)
			{
				rowIndex--;
				_dataSlide.SelectedRowIndex = rowIndex;

				this.dataGrid1.SuspendLayout();
				this.dataGrid1.CurrentRowIndex = rowIndex;
				this.dataGrid1.UnSelect(rowIndex + 1);
				this.dataGrid1.Select(rowIndex);
				this.dataGrid1.ResumeLayout(true);

				ShowRowView();
			}
		}

		private void nextRowButton_Click(object sender, System.EventArgs e)
		{
			int rowIndex = this.dataGrid1.CurrentRowIndex;
			if (rowIndex < _dataSlide.RowCount - 1)
			{
				rowIndex++;
				_dataSlide.SelectedRowIndex = rowIndex;

				this.dataGrid1.SuspendLayout();
				this.dataGrid1.CurrentRowIndex = rowIndex;
				this.dataGrid1.UnSelect(rowIndex - 1);
				this.dataGrid1.Select(rowIndex);
				this.dataGrid1.ResumeLayout(true);

				ShowRowView();
			}
		}

		private void prevPageButton_Click(object sender, System.EventArgs e)
		{
			int currentPageIndex = _dataSlide.PageIndex;
			if (currentPageIndex > 0)
			{
				_dataSlide.PageIndex = currentPageIndex - 1;

				ExecuteQuery(); // fetch the previous page
			}
		}

		private void nextPageButton_Click(object sender, System.EventArgs e)
		{
			int currentPageIndex = _dataSlide.PageIndex;
			int pageCount = DataViewModel.DEFAULT_PAGE_COUNT;
			if (_dataSlide.TotalInstanceCount >= 0)
			{
				pageCount = _dataSlide.TotalInstanceCount / _dataSlide.PageSize + 1;
			}

			if (_dataSlide.PageIndex < pageCount - 1)
			{
				_dataSlide.PageIndex = currentPageIndex + 1;

				ExecuteQuery(); // fetch the previous page
			}
		}

		private void countRowButton_Click(object sender, System.EventArgs e)
		{
			ExecuteQueryCount();
		}

		private void findButton_Click(object sender, System.EventArgs e)
		{
			_dataSlide.PageIndex = 0;
			
			ExecuteQuery();
		}

        private void bottomClassComboBox_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (bottomClassComboBox.SelectedIndex == 0 && _leafClass == null)
            {
                // do nothing
                return;
            }
            else if (bottomClassComboBox.SelectedIndex == 0 && _leafClass != null)
            {
                // display all instances
                _leafClass = null;

                CreateDataSlide(_linkedClass.Name);

                ExecuteQuery();
            }
            else if (bottomClassComboBox.SelectedIndex > 0)
            {
                _leafClass = (ClassElement)_leafClasses[bottomClassComboBox.SelectedIndex - 1];

                CreateDataSlide(_leafClass.Name);

                // restrict the search to the selected leaf class
                ExecuteQuery();
            }
        }
	}
}
