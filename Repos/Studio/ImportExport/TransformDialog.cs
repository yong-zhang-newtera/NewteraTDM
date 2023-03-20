using System;
using System.Data;
using System.Drawing;
using System.Collections;
using System.ComponentModel;
using System.Windows.Forms;

using Newtera.Common.MetaData;
using Newtera.Common.MetaData.Schema;
using Newtera.Common.MetaData.DataView;
using Newtera.Common.MetaData.Mappings;

namespace Newtera.Studio.ImportExport
{
	/// <summary>
	/// Summary description for TransformDialog.
	/// </summary>
	public class TransformDialog : System.Windows.Forms.Form
	{
		private MetaDataModel _metaData;
		private DataTable _sourceDataTable;
		private ClassMapping _classMapping;
		private DataRowView _sourceDataRowView = null;
		private InstanceView _destinationInstanceView = null;
		private DefaultValueCollection _defaultValues;
		private ConnectLineAction _connectionAction = null;

		private System.Windows.Forms.Panel panel1;
		private System.Windows.Forms.Label label1;
		private System.Windows.Forms.Label label2;
		private System.Windows.Forms.Button okButton;
		private System.Windows.Forms.Button cancelButton;
		private System.Windows.Forms.ContextMenu contextMenu1;
		private System.Windows.Forms.Button prevRowButton;
		private System.Windows.Forms.Button nextRowButton;
		private Newtera.Studio.UserControls.PropertyGridEx.PropertyGridEx destinationPropertyGrid;
		private System.Windows.Forms.Splitter splitter2;
		private Newtera.Studio.UserControls.PropertyGridEx.PropertyGridEx sourcePropertyGrid;
		private System.Windows.Forms.Splitter splitter1;
		private System.Windows.Forms.MenuItem deleteMenuItem;
		private System.Windows.Forms.MenuItem menuItem1;
		private System.Windows.Forms.MenuItem propertyMenuItem;
		private System.Windows.Forms.ToolBar toolBar;
		private System.Windows.Forms.ImageList toolBarImageList;
		private System.Windows.Forms.ToolBarButton oneToOneButton;
		private System.Windows.Forms.ToolBarButton manyToOneButton;
		private System.Windows.Forms.ToolBarButton oneToManyButton;
		private System.Windows.Forms.ToolBarButton manyToManyButton;
		private System.Windows.Forms.ToolBarButton toolBarButton1;
		private System.Windows.Forms.ToolBarButton propertyButton;
		private System.Windows.Forms.ToolBarButton delButton;
		private System.Windows.Forms.ToolBarButton delAllButton;
		private System.Windows.Forms.MenuItem deleteAllMenuItem;
		private System.Windows.Forms.ToolBarButton arrowButton;
		private Newtera.Studio.ImportExport.MapPanel _mapPanel;
		private System.Windows.Forms.Button autoMappingButton;
		private System.Windows.Forms.Label srcNameLabel;
		private System.Windows.Forms.Label dstNameLabel;
		private System.Windows.Forms.Button advancedButton;
        private ToolBarButton selectSrcButton;
        private ToolBarButton locateDstButton;
		private System.ComponentModel.IContainer components;

		public TransformDialog()
		{
			//
			// Required for Windows Form Designer support
			//
			InitializeComponent();

			//
			// TODO: Add any constructor code after InitializeComponent call
			//
			_defaultValues = new DefaultValueCollection();
		}

		/// <summary>
		/// Gets or sets the MetaDataModel instance
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
		/// Gets or sets the ClassMapping instance that contains transformations
		/// </summary>
		public ClassMapping ClassMapping
		{
			get
			{
				return _classMapping;
			}
			set
			{
				_classMapping = value;
			}
		}

		/// <summary>
		/// Gets or sets the DataTable instance that contains source data.
		/// </summary>
		public DataTable SourceDataTable
		{
			get
			{
				return _sourceDataTable;
			}
			set
			{
				_sourceDataTable = value;
			}
		}

		/// <summary>
		/// Gets the attribute mappings created by the dialog
		/// </summary>
		/// <value> A AttributeMappingCollection instance</value>
		public AttributeMappingCollection AttributeMappings
		{
			get
			{
				return this._mapPanel.AttributeMappings;
			}
		}

		/// <summary>
		/// Gets the attribute default values created by the dialog
		/// </summary>
		/// <value> A AttributeMappingCollection instance</value>
		public DefaultValueCollection DefaultValues
		{
			get
			{
				return _defaultValues;
			}
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


		#region Windows Form Designer generated code
		/// <summary>
		/// Required method for Designer support - do not modify
		/// the contents of this method with the code editor.
		/// </summary>
		private void InitializeComponent()
		{
            this.components = new System.ComponentModel.Container();
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(TransformDialog));
            this.panel1 = new System.Windows.Forms.Panel();
            this._mapPanel = new Newtera.Studio.ImportExport.MapPanel();
            this.contextMenu1 = new System.Windows.Forms.ContextMenu();
            this.deleteMenuItem = new System.Windows.Forms.MenuItem();
            this.deleteAllMenuItem = new System.Windows.Forms.MenuItem();
            this.menuItem1 = new System.Windows.Forms.MenuItem();
            this.propertyMenuItem = new System.Windows.Forms.MenuItem();
            this.destinationPropertyGrid = new Newtera.Studio.UserControls.PropertyGridEx.PropertyGridEx();
            this.toolBarImageList = new System.Windows.Forms.ImageList(this.components);
            this.sourcePropertyGrid = new Newtera.Studio.UserControls.PropertyGridEx.PropertyGridEx();
            this.splitter1 = new System.Windows.Forms.Splitter();
            this.splitter2 = new System.Windows.Forms.Splitter();
            this.label1 = new System.Windows.Forms.Label();
            this.label2 = new System.Windows.Forms.Label();
            this.okButton = new System.Windows.Forms.Button();
            this.cancelButton = new System.Windows.Forms.Button();
            this.prevRowButton = new System.Windows.Forms.Button();
            this.nextRowButton = new System.Windows.Forms.Button();
            this.toolBar = new System.Windows.Forms.ToolBar();
            this.arrowButton = new System.Windows.Forms.ToolBarButton();
            this.oneToOneButton = new System.Windows.Forms.ToolBarButton();
            this.manyToOneButton = new System.Windows.Forms.ToolBarButton();
            this.oneToManyButton = new System.Windows.Forms.ToolBarButton();
            this.manyToManyButton = new System.Windows.Forms.ToolBarButton();
            this.toolBarButton1 = new System.Windows.Forms.ToolBarButton();
            this.propertyButton = new System.Windows.Forms.ToolBarButton();
            this.selectSrcButton = new System.Windows.Forms.ToolBarButton();
            this.locateDstButton = new System.Windows.Forms.ToolBarButton();
            this.delButton = new System.Windows.Forms.ToolBarButton();
            this.delAllButton = new System.Windows.Forms.ToolBarButton();
            this.autoMappingButton = new System.Windows.Forms.Button();
            this.srcNameLabel = new System.Windows.Forms.Label();
            this.dstNameLabel = new System.Windows.Forms.Label();
            this.advancedButton = new System.Windows.Forms.Button();
            this.panel1.SuspendLayout();
            this.SuspendLayout();
            // 
            // panel1
            // 
            resources.ApplyResources(this.panel1, "panel1");
            this.panel1.BorderStyle = System.Windows.Forms.BorderStyle.Fixed3D;
            this.panel1.Controls.Add(this._mapPanel);
            this.panel1.Controls.Add(this.splitter1);
            this.panel1.Controls.Add(this.sourcePropertyGrid);
            this.panel1.Controls.Add(this.splitter2);
            this.panel1.Controls.Add(this.destinationPropertyGrid);
            this.panel1.Name = "panel1";
            // 
            // _mapPanel
            // 
            this._mapPanel.Action = null;
            this._mapPanel.BackColor = System.Drawing.SystemColors.ControlLightLight;
            this._mapPanel.ClassMapping = null;
            this._mapPanel.ContextMenu = this.contextMenu1;
            this._mapPanel.DestinationPropertyGrid = this.destinationPropertyGrid;
            resources.ApplyResources(this._mapPanel, "_mapPanel");
            this._mapPanel.ImageList = this.toolBarImageList;
            this._mapPanel.Name = "_mapPanel";
            this._mapPanel.SourcePropertyGrid = this.sourcePropertyGrid;
            this._mapPanel.SelectedComponentChanged += new Newtera.Studio.ImportExport.MapPanel.SelectedComponentChangedEventHandler(this.mapPanel_SelectedComponentChanged);
            // 
            // contextMenu1
            // 
            this.contextMenu1.MenuItems.AddRange(new System.Windows.Forms.MenuItem[] {
            this.deleteMenuItem,
            this.deleteAllMenuItem,
            this.menuItem1,
            this.propertyMenuItem});
            // 
            // deleteMenuItem
            // 
            resources.ApplyResources(this.deleteMenuItem, "deleteMenuItem");
            this.deleteMenuItem.Index = 0;
            this.deleteMenuItem.Click += new System.EventHandler(this.deleteMenuItem_Click);
            // 
            // deleteAllMenuItem
            // 
            this.deleteAllMenuItem.Index = 1;
            resources.ApplyResources(this.deleteAllMenuItem, "deleteAllMenuItem");
            this.deleteAllMenuItem.Click += new System.EventHandler(this.deleteAllMenuItem_Click);
            // 
            // menuItem1
            // 
            this.menuItem1.Index = 2;
            resources.ApplyResources(this.menuItem1, "menuItem1");
            // 
            // propertyMenuItem
            // 
            resources.ApplyResources(this.propertyMenuItem, "propertyMenuItem");
            this.propertyMenuItem.Index = 3;
            this.propertyMenuItem.Click += new System.EventHandler(this.propertyMenuItem_Click);
            // 
            // destinationPropertyGrid
            // 
            resources.ApplyResources(this.destinationPropertyGrid, "destinationPropertyGrid");
            this.destinationPropertyGrid.LineColor = System.Drawing.SystemColors.ScrollBar;
            this.destinationPropertyGrid.Name = "destinationPropertyGrid";
            this.destinationPropertyGrid.PropertySort = System.Windows.Forms.PropertySort.Categorized;
            this.destinationPropertyGrid.ToolbarVisible = false;
            this.destinationPropertyGrid.PropertyValueChanged += new System.Windows.Forms.PropertyValueChangedEventHandler(this.destinationPropertyGrid_PropertyValueChanged);
            this.destinationPropertyGrid.GridItemCollapsed += new Newtera.Studio.UserControls.PropertyGridEx.PropertyGridEx.GridItemCollapsedEventHandler(this.destinationPropertyGrid_GridItemCollapsed);
            this.destinationPropertyGrid.GridItemExpanded += new Newtera.Studio.UserControls.PropertyGridEx.PropertyGridEx.GridItemExpandedEventHandler(this.destinationPropertyGrid_GridItemExpanded);
            // 
            // toolBarImageList
            // 
            this.toolBarImageList.ImageStream = ((System.Windows.Forms.ImageListStreamer)(resources.GetObject("toolBarImageList.ImageStream")));
            this.toolBarImageList.TransparentColor = System.Drawing.Color.Transparent;
            this.toolBarImageList.Images.SetKeyName(0, "");
            this.toolBarImageList.Images.SetKeyName(1, "");
            this.toolBarImageList.Images.SetKeyName(2, "");
            this.toolBarImageList.Images.SetKeyName(3, "");
            this.toolBarImageList.Images.SetKeyName(4, "");
            this.toolBarImageList.Images.SetKeyName(5, "");
            this.toolBarImageList.Images.SetKeyName(6, "selectSrcRow.bmp");
            this.toolBarImageList.Images.SetKeyName(7, "locateDstRow.bmp");
            this.toolBarImageList.Images.SetKeyName(8, "");
            this.toolBarImageList.Images.SetKeyName(9, "");
            // 
            // sourcePropertyGrid
            // 
            resources.ApplyResources(this.sourcePropertyGrid, "sourcePropertyGrid");
            this.sourcePropertyGrid.LineColor = System.Drawing.SystemColors.ScrollBar;
            this.sourcePropertyGrid.Name = "sourcePropertyGrid";
            this.sourcePropertyGrid.PropertySort = System.Windows.Forms.PropertySort.NoSort;
            this.sourcePropertyGrid.ToolbarVisible = false;
            this.sourcePropertyGrid.GridItemCollapsed += new Newtera.Studio.UserControls.PropertyGridEx.PropertyGridEx.GridItemCollapsedEventHandler(this.sourcePropertyGrid_GridItemCollapsed);
            this.sourcePropertyGrid.GridItemExpanded += new Newtera.Studio.UserControls.PropertyGridEx.PropertyGridEx.GridItemExpandedEventHandler(this.sourcePropertyGrid_GridItemExpanded);
            // 
            // splitter1
            // 
            resources.ApplyResources(this.splitter1, "splitter1");
            this.splitter1.Name = "splitter1";
            this.splitter1.TabStop = false;
            // 
            // splitter2
            // 
            resources.ApplyResources(this.splitter2, "splitter2");
            this.splitter2.Name = "splitter2";
            this.splitter2.TabStop = false;
            // 
            // label1
            // 
            resources.ApplyResources(this.label1, "label1");
            this.label1.Name = "label1";
            // 
            // label2
            // 
            resources.ApplyResources(this.label2, "label2");
            this.label2.Name = "label2";
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
            // prevRowButton
            // 
            resources.ApplyResources(this.prevRowButton, "prevRowButton");
            this.prevRowButton.Name = "prevRowButton";
            this.prevRowButton.Click += new System.EventHandler(this.prevRowButton_Click);
            // 
            // nextRowButton
            // 
            resources.ApplyResources(this.nextRowButton, "nextRowButton");
            this.nextRowButton.Name = "nextRowButton";
            this.nextRowButton.Click += new System.EventHandler(this.nextRowButton_Click);
            // 
            // toolBar
            // 
            this.toolBar.Buttons.AddRange(new System.Windows.Forms.ToolBarButton[] {
            this.arrowButton,
            this.oneToOneButton,
            this.manyToOneButton,
            this.oneToManyButton,
            this.manyToManyButton,
            this.toolBarButton1,
            this.propertyButton,
            this.selectSrcButton,
            this.locateDstButton,
            this.delButton,
            this.delAllButton});
            resources.ApplyResources(this.toolBar, "toolBar");
            this.toolBar.ImageList = this.toolBarImageList;
            this.toolBar.Name = "toolBar";
            this.toolBar.ButtonClick += new System.Windows.Forms.ToolBarButtonClickEventHandler(this.toolBar_ButtonClick);
            // 
            // arrowButton
            // 
            resources.ApplyResources(this.arrowButton, "arrowButton");
            this.arrowButton.Name = "arrowButton";
            this.arrowButton.Pushed = true;
            this.arrowButton.Style = System.Windows.Forms.ToolBarButtonStyle.ToggleButton;
            // 
            // oneToOneButton
            // 
            resources.ApplyResources(this.oneToOneButton, "oneToOneButton");
            this.oneToOneButton.Name = "oneToOneButton";
            this.oneToOneButton.Style = System.Windows.Forms.ToolBarButtonStyle.ToggleButton;
            // 
            // manyToOneButton
            // 
            resources.ApplyResources(this.manyToOneButton, "manyToOneButton");
            this.manyToOneButton.Name = "manyToOneButton";
            this.manyToOneButton.Style = System.Windows.Forms.ToolBarButtonStyle.ToggleButton;
            // 
            // oneToManyButton
            // 
            resources.ApplyResources(this.oneToManyButton, "oneToManyButton");
            this.oneToManyButton.Name = "oneToManyButton";
            this.oneToManyButton.Style = System.Windows.Forms.ToolBarButtonStyle.ToggleButton;
            // 
            // manyToManyButton
            // 
            resources.ApplyResources(this.manyToManyButton, "manyToManyButton");
            this.manyToManyButton.Name = "manyToManyButton";
            this.manyToManyButton.Style = System.Windows.Forms.ToolBarButtonStyle.ToggleButton;
            // 
            // toolBarButton1
            // 
            this.toolBarButton1.Name = "toolBarButton1";
            this.toolBarButton1.Style = System.Windows.Forms.ToolBarButtonStyle.Separator;
            // 
            // propertyButton
            // 
            resources.ApplyResources(this.propertyButton, "propertyButton");
            this.propertyButton.Name = "propertyButton";
            // 
            // selectSrcButton
            // 
            resources.ApplyResources(this.selectSrcButton, "selectSrcButton");
            this.selectSrcButton.Name = "selectSrcButton";
            // 
            // locateDstButton
            // 
            resources.ApplyResources(this.locateDstButton, "locateDstButton");
            this.locateDstButton.Name = "locateDstButton";
            // 
            // delButton
            // 
            resources.ApplyResources(this.delButton, "delButton");
            this.delButton.Name = "delButton";
            // 
            // delAllButton
            // 
            resources.ApplyResources(this.delAllButton, "delAllButton");
            this.delAllButton.Name = "delAllButton";
            // 
            // autoMappingButton
            // 
            resources.ApplyResources(this.autoMappingButton, "autoMappingButton");
            this.autoMappingButton.Name = "autoMappingButton";
            this.autoMappingButton.Click += new System.EventHandler(this.autoMappingButton_Click);
            // 
            // srcNameLabel
            // 
            resources.ApplyResources(this.srcNameLabel, "srcNameLabel");
            this.srcNameLabel.Name = "srcNameLabel";
            // 
            // dstNameLabel
            // 
            resources.ApplyResources(this.dstNameLabel, "dstNameLabel");
            this.dstNameLabel.Name = "dstNameLabel";
            // 
            // advancedButton
            // 
            resources.ApplyResources(this.advancedButton, "advancedButton");
            this.advancedButton.Name = "advancedButton";
            this.advancedButton.Click += new System.EventHandler(this.advancedButton_Click);
            // 
            // TransformDialog
            // 
            resources.ApplyResources(this, "$this");
            this.Controls.Add(this.advancedButton);
            this.Controls.Add(this.dstNameLabel);
            this.Controls.Add(this.srcNameLabel);
            this.Controls.Add(this.autoMappingButton);
            this.Controls.Add(this.toolBar);
            this.Controls.Add(this.nextRowButton);
            this.Controls.Add(this.prevRowButton);
            this.Controls.Add(this.cancelButton);
            this.Controls.Add(this.okButton);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.panel1);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.SizableToolWindow;
            this.Name = "TransformDialog";
            this.Load += new System.EventHandler(this.TransformDialog_Load);
            this.MouseUp += new System.Windows.Forms.MouseEventHandler(this.TransformDialog_MouseUp);
            this.panel1.ResumeLayout(false);
            this.ResumeLayout(false);
            this.PerformLayout();

		}
		#endregion

		#region Controller Code

		/// <summary>
		/// Set the enabling status of Prev and Next buttons
		/// </summary>
		private void SetPrevNextButtonStatus()
		{
			if (_sourceDataRowView.SelectedIndex == 0)
			{
				this.prevRowButton.Enabled = false;
			}
			else
			{
				this.prevRowButton.Enabled = true;
			}

			if (_sourceDataRowView.SelectedIndex < this._sourceDataTable.Rows.Count - 1)
			{
				this.nextRowButton.Enabled = true;
			}
			else
			{
				this.nextRowButton.Enabled = false;
			}
		}

		/// <summary>
		/// Set the default values specified for the destination attributes
		/// </summary>
		private void SetDefaultValues(InstanceView instanceView)
		{
			foreach (DefaultValue defaultValue in _classMapping.DefaultValues)
			{
				// keep a local copy of default values
				_defaultValues.Add(defaultValue.Copy());
				instanceView.InstanceData.SetAttributeStringValue(defaultValue.DestinationAttributeName,
					defaultValue.Value);
			}

			_defaultValues.IsAltered = false; // reset the flag
		}

		/// <summary>
		/// set tool bar toggle button status so that only one of toggle buttons
		/// is pushed in
		/// </summary>
		/// <param name="e">The event</param>
		private void SetToolBarToggleButtonStatus(System.Windows.Forms.ToolBarButtonClickEventArgs e)
		{
			MapComponentType type = MapComponentType.Pointer;

			if (e.Button == this.arrowButton)
			{
				type = MapComponentType.Pointer;
			}
			else if (e.Button == this.oneToOneButton)
			{
				type = MapComponentType.OneToOne;
			}
			else if (e.Button == this.oneToManyButton)
			{
				type = MapComponentType.OneToMany;
			}
			else if (e.Button == this.manyToOneButton)
			{
				type = MapComponentType.ManyToOne;
			}
			else if (e.Button == this.manyToManyButton)
			{
				type = MapComponentType.ManyToMany;
			}
			
			SetToolBarToggleButtonStatus(type);
		}

		private void SetToolBarToggleButtonStatus(MapComponentType type)
		{
			if (type == MapComponentType.Pointer)
			{
				this.arrowButton.Pushed = true;
			}
			else
			{
				this.arrowButton.Pushed = false;
			}

			if (type == MapComponentType.OneToOne)
			{
				this.oneToOneButton.Pushed = true;
			}
			else
			{
				this.oneToOneButton.Pushed = false;
			}

			if (type == MapComponentType.OneToMany)
			{
				this.oneToManyButton.Pushed = true;
				this._mapPanel.Action = new InsertRectAction(_mapPanel, type);
			}
			else
			{
				this.oneToManyButton.Pushed = false;
			}

			if (type == MapComponentType.ManyToOne)
			{
				this.manyToOneButton.Pushed = true;
				this._mapPanel.Action = new InsertRectAction(_mapPanel, type);
			}
			else
			{
				this.manyToOneButton.Pushed = false;
			}

			if (type == MapComponentType.ManyToMany)
			{
				this.manyToManyButton.Pushed = true;
				this._mapPanel.Action = new InsertRectAction(_mapPanel, type);
			}
			else
			{
				this.manyToManyButton.Pushed = false;
			}
		}

		/// <summary>
		/// Perform actions according to the selected component
		/// </summary>
		/// <param name="component">The selected component</param>
		private void PerformActions(IMapComponent component, MapComponentEventArgs e)
		{
			switch (component.ComponentType)
			{
				case MapComponentType.SourceEnd:
				case MapComponentType.OutputEnd:

					_mapPanel.Cursor = Cursors.Cross;

					_connectionAction = new ConnectLineAction(_mapPanel);
					
					_connectionAction.SrcComponent = component;
	
					// set the toolbar toggle button status
					SetToolBarToggleButtonStatus(MapComponentType.OneToOne);

					break;

				case MapComponentType.InputEnd:
				case MapComponentType.DestinationEnd:

					_mapPanel.Cursor = Cursors.Default;

					if (_connectionAction != null &&
						_connectionAction.ActionType == MapActionType.Connect)
					{
						_connectionAction.DstComponent = component;

						// make sure that two components can be connected
						if (!_connectionAction.IsValid)
						{
							MessageBox.Show(_connectionAction.ErrorMessage);
						}
						else
						{
							_mapPanel.Action = _connectionAction;
						}

						// connect action will be performed when mouse up event fired
					}
		
					// set the toolbar toggle button status
					SetToolBarToggleButtonStatus(MapComponentType.Pointer);

					break;

				case MapComponentType.Composite:

					if (component.Principal.ComponentType == MapComponentType.ManyToOne ||
						component.Principal.ComponentType == MapComponentType.OneToMany ||
						component.Principal.ComponentType == MapComponentType.ManyToMany)
					{
						_mapPanel.Cursor = Cursors.SizeAll;

						MoveRectAction moveAction = new MoveRectAction(_mapPanel,
							(MapRect) component.Principal);

						_mapPanel.Action = moveAction;
					}

					break;

				default:
					_mapPanel.Cursor = Cursors.Default;

					// set the toolbar toggle button status
					SetToolBarToggleButtonStatus(MapComponentType.Pointer);
					break;
			}
		}

		private void ShowMappingProperty(IMapComponent component)
		{
			if (component is MapComposite &&
				((MapComposite) component).Principal.MappingNode != null &&
				((MapComposite) component).Principal.MappingNode is ITransformable)
			{
				ITransformable pricipalComponent = (ITransformable) ((MapComposite) component).Principal.MappingNode;
				MappingScriptDialog dialog = new MappingScriptDialog();
				dialog.SchemaInfo = _metaData.SchemaInfo;
				dialog.Mapping = pricipalComponent;
				dialog.ScriptEnabled = pricipalComponent.ScriptEnabled;
				dialog.ScriptLanguage = pricipalComponent.ScriptLanguage;
				dialog.ClassType = pricipalComponent.ClassType;
				dialog.Script = pricipalComponent.Script;

				if (dialog.ShowDialog() == DialogResult.OK)
				{
					pricipalComponent.ScriptEnabled = dialog.ScriptEnabled;
					pricipalComponent.ScriptLanguage = dialog.ScriptLanguage;
					pricipalComponent.ClassType = dialog.ClassType;
					pricipalComponent.Script = dialog.Script;
				}
			}
		}

        private void ShowSelectSrcRowDialog(SelectRowScript scriptObj)
        {
            SelectSrcRowScriptDialog dialog = new SelectSrcRowScriptDialog();
            dialog.SchemaInfo = _metaData.SchemaInfo;
            dialog.ScriptObject = scriptObj;
            dialog.ScriptEnabled = scriptObj.Enabled;
            dialog.ScriptLanguage = scriptObj.ScriptLanguage;
            dialog.ClassType = scriptObj.ClassType;
            dialog.Script = scriptObj.Script;

            if (dialog.ShowDialog() == DialogResult.OK)
            {
                scriptObj.Enabled = dialog.ScriptEnabled;
                scriptObj.ScriptLanguage = dialog.ScriptLanguage;
                scriptObj.ClassType = dialog.ClassType;
                scriptObj.Script = dialog.Script;
            }
        }

        private void ShowIdentifyDstRowDialog(IdentifyRowScript scriptObj)
        {
            IdentifyDstRowScriptDialog dialog = new IdentifyDstRowScriptDialog();
            dialog.SchemaInfo = _metaData.SchemaInfo;
            dialog.ScriptObject = scriptObj;
            dialog.ScriptEnabled = scriptObj.Enabled;
            dialog.ScriptLanguage = scriptObj.ScriptLanguage;
            dialog.ClassType = scriptObj.ClassType;
            dialog.Script = scriptObj.Script;

            if (dialog.ShowDialog() == DialogResult.OK)
            {
                scriptObj.Enabled = dialog.ScriptEnabled;
                scriptObj.ScriptLanguage = dialog.ScriptLanguage;
                scriptObj.ClassType = dialog.ClassType;
                scriptObj.Script = dialog.Script;
            }
        }

		/// <summary>
		/// Create an empty row for each array attribute so that the PropertyGrid
		/// can display subitems of array attributes.
		/// </summary>
		/// <param name="instanceView">The InstanceView</param>
		private void InitArrayAttributes(InstanceView instanceView)
		{
			foreach (IDataViewElement element in instanceView.DataView.ResultAttributes)
			{
				if (element is DataArrayAttribute)
				{
					DataArrayAttribute arrayAttr = (DataArrayAttribute) element;
					// create an empty row if it is an empty array
					if (!arrayAttr.HasValue)
					{
						arrayAttr.AddEmptyRow();
					}
				}
			}
		}

		#endregion

		private void TransformDialog_Load(object sender, System.EventArgs e)
		{
			if (_sourceDataTable != null)
			{
				_sourceDataRowView = new DataRowView(_sourceDataTable);
				
				// display the source data in the property grid
				sourcePropertyGrid.SelectedObject = _sourceDataRowView;
				sourcePropertyGrid.CalculateGridItemPositions();
			}

			if (_metaData != null && _classMapping != null)
			{
				DataViewModel instanceDataView = _metaData.GetDetailedDataView(_classMapping.DestinationClassName);

				// display the destination class in the property grid
				_destinationInstanceView = new InstanceView(instanceDataView);

				// set the default values to corresponding destination attributes
				SetDefaultValues(_destinationInstanceView);

				// create an empty row for each array attribute so that property
				// grid can create GridItem for array sub-element
				InitArrayAttributes(_destinationInstanceView);

				destinationPropertyGrid.SelectedObject = _destinationInstanceView;
				destinationPropertyGrid.CalculateGridItemPositions();
			}

			// Set the AutoScrollMinSize to the max of heights of sourcePropertyGrid
			// and destinationPropertyGrid, so that the scroll bars of both
			// PropertyGrid are not visible.
			int srcHeight = this.sourcePropertyGrid.GetActualHeight();
			int desHeight = this.destinationPropertyGrid.GetActualHeight();
			int panelMinHeight = (srcHeight > desHeight ? srcHeight : desHeight);
			Size size = new Size(this.panel1.AutoScrollMinSize.Width, panelMinHeight);
			this.panel1.AutoScrollMinSize = size;

			// initialize the MapPanel instance
			_mapPanel.ClassMapping = _classMapping;

			// set status of next button
			if (this._sourceDataTable.Rows.Count > 1)
			{
				this.nextRowButton.Enabled = true;
			}

			// display the src and dst name
			this.srcNameLabel.Text = _classMapping.SourceClassName;
			ClassElement dstClass = _metaData.SchemaModel.FindClass(_classMapping.DestinationClassName);
			this.dstNameLabel.Text = dstClass.Caption;
		}

		private void mapPanel_SelectedComponentChanged(object sender, Newtera.Studio.ImportExport.MapComponentEventArgs e)
		{
			if (e.SelectedComponent != null)
			{
				this.delButton.Enabled = true;
				this.deleteMenuItem.Enabled = true;
				if (e.SelectedComponent is MapComposite &&
					((MapComposite) e.SelectedComponent).Principal.MappingNode != null)
				{
					this.propertyButton.Enabled = true;
					this.propertyMenuItem.Enabled = true;
				}
				else
				{
					this.propertyButton.Enabled = false;
					this.propertyMenuItem.Enabled = false;
				}

				PerformActions(e.SelectedComponent, e);
			}
			else
			{
				// nothing selected
				this.delButton.Enabled = false;
				this.deleteMenuItem.Enabled = false;
				this.propertyButton.Enabled = false;
				this.propertyMenuItem.Enabled = false;

				this._connectionAction = null;

				// Restore the Cursor and discard the src item selection
				_mapPanel.Cursor = Cursors.Default;
		
				// set the toolbar toggle button status
				SetToolBarToggleButtonStatus(MapComponentType.Pointer);
			}
		}

		private void deleteButton_Click(object sender, System.EventArgs e)
		{
			this._mapPanel.DeleteComponent(_mapPanel.SelectedComponent);
		}

		private void deleteAllButton_Click(object sender, System.EventArgs e)
		{
			this._mapPanel.ClearMappings();
		}

		private void destinationPropertyGrid_GridItemCollapsed(object sender, Newtera.Studio.UserControls.PropertyGridEx.GridItemEventArgs e)
		{
            InstanceAttributePropertyDescriptor pd = e.Item.PropertyDescriptor as InstanceAttributePropertyDescriptor;
            if (pd == null || !pd.IsRelationship)
            {
                this.destinationPropertyGrid.CalculateGridItemPositions();
                this._mapPanel.Redraw();
            }
		}

		private void destinationPropertyGrid_GridItemExpanded(object sender, Newtera.Studio.UserControls.PropertyGridEx.GridItemEventArgs e)
		{
            this.destinationPropertyGrid.CalculateGridItemPositions();
            this._mapPanel.Redraw();
		}

		private void sourcePropertyGrid_GridItemCollapsed(object sender, Newtera.Studio.UserControls.PropertyGridEx.GridItemEventArgs e)
		{
			this.sourcePropertyGrid.CalculateGridItemPositions();
			this._mapPanel.Redraw();	
		}

		private void sourcePropertyGrid_GridItemExpanded(object sender, Newtera.Studio.UserControls.PropertyGridEx.GridItemEventArgs e)
		{
			this.sourcePropertyGrid.CalculateGridItemPositions();
			this._mapPanel.Redraw();		
		}

		private void TransformDialog_MouseUp(object sender, System.Windows.Forms.MouseEventArgs e)
		{
			Cursor.Current = Cursors.Default;	
		}

		private void prevRowButton_Click(object sender, System.EventArgs e)
		{
			_sourceDataRowView.SelectedIndex = _sourceDataRowView.SelectedIndex - 1;
			this.sourcePropertyGrid.Refresh();

			SetPrevNextButtonStatus();
		}

		private void nextRowButton_Click(object sender, System.EventArgs e)
		{
			_sourceDataRowView.SelectedIndex = _sourceDataRowView.SelectedIndex + 1;
			this.sourcePropertyGrid.Refresh();
		
			SetPrevNextButtonStatus();
		}

		private void destinationPropertyGrid_PropertyValueChanged(object s, System.Windows.Forms.PropertyValueChangedEventArgs e)
		{
			if (e.ChangedItem.GridItemType == GridItemType.Property)
			{
				string attributeName = e.ChangedItem.PropertyDescriptor.Name;
				string attributeValue = _destinationInstanceView.InstanceData.GetAttributeStringValue(attributeName);
				
				InstanceAttributePropertyDescriptor ipd = e.ChangedItem.PropertyDescriptor as InstanceAttributePropertyDescriptor;

				if (ipd != null && ipd.IsRelationship)
				{
					DataRelationshipAttribute relationshipAttr = (DataRelationshipAttribute) ipd.DataViewElement;

					// the value of a relationship attribite consists of values of its
					// primary keys separated by &. Splits the pk values and set them
					// to each primary keys
					string[] pkValues = relationshipAttr.SplitPKValues(attributeValue);

					if (relationshipAttr.PrimaryKeys != null)
					{
						int index = 0;
						foreach (DataSimpleAttribute pk in relationshipAttr.PrimaryKeys)
						{
							if (pkValues != null && pkValues.Length > index)
							{
								SetDefaultValue(relationshipAttr.GetUniquePKName(pk.Name), pkValues[index]);
							}
							else
							{
								// remove the default value for the primary key
								SetDefaultValue(relationshipAttr.GetUniquePKName(pk.Name), null);
							}

							index ++;
						}
					}
				}
				else
				{
					SetDefaultValue(attributeName, attributeValue);
				}

				if (ipd != null && ipd.IsArray)
				{
					// the number of grid items in destination property grid that
					// represent array cells might have been changed, we need to
					// redraw the dots in the map panel that represents the
					// grid items
					this._mapPanel.Redraw();
				}
			}
		}

		/// <summary>
		/// Create, modify or delete a DefaultValue instance in the ClassMapping.
		/// </summary>
		/// <param name="attributeName">The attribute name.</param>
		/// <param name="value">The default value.</param>
		private void SetDefaultValue(string attributeName, string attributeValue)
		{
			DefaultValue defaultValue = null;
			foreach (DefaultValue dv in this._defaultValues)
			{
				if (dv.DestinationAttributeName == attributeName)
				{
					defaultValue = dv;
					break;
				}
			}

			if (defaultValue == null)
			{
				// create a Default Value instance
				defaultValue = new DefaultValue(attributeName, attributeValue);
				_defaultValues.Add(defaultValue);
			}
			else
			{
				if (attributeValue != null && attributeValue.Length > 0)
				{
					// change the default value
					defaultValue.Value = attributeValue;
				}
				else
				{
					// remove the default value
					_defaultValues.Remove(defaultValue);
				}
			}
		}

		private void deleteMenuItem_Click(object sender, System.EventArgs e)
		{
			this._mapPanel.DeleteComponent(_mapPanel.SelectedComponent);		
		}

		private void propertyMenuItem_Click(object sender, System.EventArgs e)
		{
			ShowMappingProperty(_mapPanel.SelectedComponent);
		}

		private void toolBar_ButtonClick(object sender, System.Windows.Forms.ToolBarButtonClickEventArgs e)
		{
			if (e.Button == this.delButton)
			{
				this._mapPanel.DeleteComponent(_mapPanel.SelectedComponent);
			}
			else if (e.Button == this.delAllButton)
			{
				this._mapPanel.ClearMappings();
			}
			else if (e.Button == this.propertyButton)
			{
				ShowMappingProperty(_mapPanel.SelectedComponent);
			}
            else if (e.Button == this.selectSrcButton)
            {
                ShowSelectSrcRowDialog(_classMapping.SelectSrcRowScript);
            }
            else if (e.Button == this.locateDstButton)
            {
                ShowIdentifyDstRowDialog(_classMapping.IdentifyDstRowScript);
            }
            else
            {
                SetToolBarToggleButtonStatus(e); // set the tool bar toggle button status
            }
		}

		private void deleteAllMenuItem_Click(object sender, System.EventArgs e)
		{
			this._mapPanel.ClearMappings();
		}

		private void autoMappingButton_Click(object sender, System.EventArgs e)
		{
			this._mapPanel.AutoMappings();
		}

		private void advancedButton_Click(object sender, System.EventArgs e)
		{
			TransformAdvancedDialog dialog = new TransformAdvancedDialog();

			dialog.TransformCardinalType = _classMapping.TransformCardinalType;

			if (dialog.ShowDialog() == DialogResult.OK)
			{
				_classMapping.TransformCardinalType = dialog.TransformCardinalType;
			}
		}
	}
}
