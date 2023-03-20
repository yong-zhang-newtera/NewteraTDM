using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Text;
using System.Threading;
using System.Windows.Forms;

using Newtera.Common.Core;
using Newtera.Common.MetaData;
using Newtera.Common.MetaData.Schema;
using Newtera.Common.MetaData.DataView;
using Newtera.Common.MetaData.XaclModel;
using Newtera.Common.MetaData.Principal;

namespace Newtera.WindowsControl
{
    public partial class ResultDataControl : UserControl
    {
        public event EventHandler RowSelectedIndexChangedEvent;
        public event EventHandler RequestForCountEvent;
        public event EventHandler RequestForDataEvent;
        public event EventHandler DataGridClearedEvent;
        public event EventHandler RequestForSaveInstancesEvent;

        private DataViewSlide _currentSlide;
        private DataViewSlideCollection _slides;
        private MetaDataModel _metaData;
        private MenuItemStates _menuItemStates;
        private string _userName;
        private IUserManager _userManager;
        private IServerProxy _serverProxy;

        public ResultDataControl()
        {
            InitializeComponent();

            _metaData = null;
            _menuItemStates = null;
            _currentSlide = null;
            _slides = new DataViewSlideCollection();
            _slides.CountChanged += new EventHandler(dataViewSlideCountChanged);
            _userName = null;
            _userManager = null;
            _serverProxy = null;
        }

        /// <summary>
        /// Gets the current display slide.
        /// </summary>
        public DataViewSlide CurrentSlide
        {
            get
            {
                return _currentSlide;
            }
            set
            {
                _currentSlide = value;
            }
        }

        /// <summary>
        /// Gets all slides.
        /// </summary>
        public DataViewSlideCollection Slides
        {
            get
            {
                return _slides;
            }
        }

        /// <summary>
        /// Gets or sets the MetaDataModel
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
        /// Gets or sets the MenuItemStates
        /// </summary>
        public MenuItemStates MenuItemStates
        {
            get
            {
                return _menuItemStates;
            }
            set
            {
                _menuItemStates = value;
                if (_menuItemStates != null)
                {
                    _menuItemStates.StateChanged += new EventHandler(this.MenuItemStateChanged);
                }
            }
        }

        /// <summary>
        /// Get the DataGrid control
        /// </summary>
        public DataGrid TheDataGrid
        {
            get
            {
                return dataGrid1;
            }
        }

        /// <summary>
        /// Gets or sets the authorized user name
        /// </summary>
        public string UserName
        {
            get
            {
                return _userName;
            }
            set
            {
                _userName = value;
            }
        }

        /// <summary>
        /// Gets or sets the A custom user manager
        /// </summary>
        public IUserManager UserManager
        {
            get
            {
                return _userManager;
            }
            set
            {
                _userManager = value;
            }
        }

        /// <summary>
        /// Gets or sets the a server proxy
        /// </summary>
        public IServerProxy ServerProxy
        {
            get
            {
                return _serverProxy;
            }
            set
            {
                _serverProxy = value;
            }
        }

        /// <summary>
        /// Gets the selected indices of selected instances
        /// </summary>
        public SelectedIndexCollection SelectedIndices
        {
            get
            {
                SelectedIndexCollection selectedIndices = new SelectedIndexCollection();
                for (int row = 0; row < this.CurrentSlide.RowCount; row++)
                {
                    if (this.dataGrid1.IsSelected(row))
                    {
                        selectedIndices.Add(row);
                    }
                }

                return selectedIndices;
            }
        }

        /// <summary>
        /// Gets the information indicating whether a row is currently selected in the datagrid
        /// </summary>
        /// <param name="row">The row index</param>
        /// <returns>true if the row is selected, false otherwise.</returns>
        public bool IsRowSelected(int row)
        {
            return this.dataGrid1.IsSelected(row);
        }

        /// <summary>
        /// Show the result of a query
        /// </summary>
        /// <param name="dataSet">The data set contains the result</param>
        public void ShowQueryResult(DataSet dataSet)
        {
            // The data grid need a table style to display the data, check
            // if a table style has alreadt existed for the base class in
            // the data view, if yes, delete the existing table style and
            // create a new table style
            DataGridTableStyle tableStyle = this.dataGrid1.TableStyles[_currentSlide.DataView.BaseClass.ClassName];
            if (tableStyle != null)
            {
                this.dataGrid1.TableStyles.Remove(tableStyle);
            }

            tableStyle = TableStyleFactory.Instance.Create(_currentSlide.DataView, this.imageList.Images[0]);

            this.dataGrid1.TableStyles.Add(tableStyle);

            // set the save button state
            if (TableStyleFactory.Instance.HasInlineEditColumns &&
                CurrentSlide.GoodForInlineEdit)
            {
                this.saveButton.Enabled = true;
            }
            else
            {
                this.saveButton.Enabled = false;
            }

            // Change the relation names created by DataSet to the names
            // defined by the data view
            if (dataSet != null)
            {
                ChangeRelationNames(dataSet);
            }

            _currentSlide.DataSet = dataSet;
            _currentSlide.SelectedRowIndex = 0;

            ShowDataSlide();
        }

        /// <summary>
        /// Show the instance count
        /// </summary>
        /// <param name="count">The count</param>
        public void ShowInstanceCount(int count)
        {
            // it is the UI thread, continue
            this.countTextBox.Text = count + "";
            this._currentSlide.TotalInstanceCount = count;
        }

        /// <summary>
        /// display the current data slide in the grid
        /// </summary>
        public void ShowDataSlide()
        {
            // make sure there is a data to show
            if (!_currentSlide.IsEmptyResult)
            {
                this.dataGrid1.SuspendLayout();
                string captionText = "";
                int index = 0;
                foreach (DataViewSlide slide in _slides)
                {
                    if (index > 0)
                    {
                        captionText += " => ";
                    }

                    captionText += slide.DataView.BaseClass.Caption;
                    index++;
                }

                // clear the has changed flag
                _currentSlide.DataSet.AcceptChanges();

                this.dataGrid1.CaptionText = captionText;
                this.dataGrid1.SetDataBinding(_currentSlide.DataSet, _currentSlide.DataView.BaseClass.ClassName);

                this.dataGrid1.Select(_currentSlide.SelectedRowIndex);

                if (_currentSlide.IsRowExpanded)
                {
                    this.dataGrid1.Expand(_currentSlide.SelectedRowIndex);
                }

                this.dataGrid1.CurrentCell = new DataGridCell(0, 0);
                this.dataGrid1.ResumeLayout(true);

                // set the count to unknow if the total count is unknow
                if (_currentSlide.TotalInstanceCount < 0)
                {
                    this.countTextBox.Text = "???";
                }
                else
                {
                    this.countTextBox.Text = _currentSlide.TotalInstanceCount + "";
                }

                // set the menu item enabling status according to the current slide
                // being displayed
                SetMenuItemStates(_currentSlide);

                // fire row selected index changed event
                FireRowSelectedIndexChangedEvent();
            }
            else
            {
                // it is an empty data set
                // if there is a parent slide, navigate back to parent
                if (_slides.Count > 1)
                {
                    NavigateBackToParent();
                }
                else
                {
                    ClearDataGrids();
                }

                MessageBox.Show(MessageResourceManager.GetString("DataViewer.NoResults"));
            }
        }

        public void MoveToPrevPage()
        {
            int currentPageIndex = _currentSlide.PageIndex;
            if (currentPageIndex > 0)
            {
                _currentSlide.PageIndex = currentPageIndex - 1;

                FireRequestForDataEvent(); // fetch the previous page
            }
        }

        public void MoveToNextPage()
        {
            int currentPageIndex = _currentSlide.PageIndex;
            int pageCount = DataViewModel.DEFAULT_PAGE_COUNT;
            if (_currentSlide.TotalInstanceCount >= 0)
            {
                pageCount = _currentSlide.TotalInstanceCount / _currentSlide.PageSize + 1;
            }

            if (_currentSlide.PageIndex < pageCount - 1)
            {
                _currentSlide.PageIndex = currentPageIndex + 1;

                FireRequestForDataEvent(); // fetch the previous page
            }
        }

        public void MoveToPrevRow()
        {
            int displayedRowIndex = this.dataGrid1.CurrentRowIndex;
            if (displayedRowIndex > 0)
            {
                displayedRowIndex--;

                // get corresponding row index in datatable
                int rowIndex = ConvertToDataTableRowIndex(displayedRowIndex);
                _currentSlide.SelectedRowIndex = rowIndex;

                this.dataGrid1.SuspendLayout();
                this.dataGrid1.CurrentRowIndex = displayedRowIndex;
                this.dataGrid1.UnSelect(displayedRowIndex + 1);
                this.dataGrid1.Select(displayedRowIndex);
                this.dataGrid1.ResumeLayout(true);

                FireRowSelectedIndexChangedEvent();
            }
        }

        public void MoveToNextRow()
        {
            int displayedRowIndex = this.dataGrid1.CurrentRowIndex;
            if (displayedRowIndex < _currentSlide.RowCount)
            {
                displayedRowIndex++;
                int rowIndex = ConvertToDataTableRowIndex(displayedRowIndex);
                _currentSlide.SelectedRowIndex = rowIndex;

                this.dataGrid1.SuspendLayout();
                this.dataGrid1.CurrentRowIndex = displayedRowIndex;
                this.dataGrid1.UnSelect(displayedRowIndex - 1);
                this.dataGrid1.Select(displayedRowIndex);
                this.dataGrid1.ResumeLayout(true);

                FireRowSelectedIndexChangedEvent();
            }
        }

        /// <summary>
        /// clear the content currently displayed in the grid
        /// </summary>
        public void ClearDataGrids()
        {
            this.dataGrid1.DataSource = null;
            this.dataGrid1.CaptionText = null;
            this.dataGrid1.CurrentCell = new DataGridCell(0, 0);
            if (this.DataGridClearedEvent != null)
            {
                DataGridClearedEvent(this, new EventArgs());
            }

            // set the edit button enabling status
            EnableInstanceEditButtons(false);

            if (_currentSlide != null)
            {
                ClassElement classElement = _currentSlide.DataView.SchemaModel.FindClass(_currentSlide.DataView.BaseClass.ClassName);
                if (classElement != null && classElement.IsLeaf)
                {
                    this._menuItemStates.SetState(MenuItemID.EditNewInstance, true);
                }
            }
        }

        public void SetCurrentSlideSelectedRowIndex(int displayedRowIndex)
        {
            _currentSlide.SelectedRowIndex = ConvertToDataTableRowIndex(displayedRowIndex);
        }

        /// <summary>
        /// enable/disable the buttons for editing an instance according to its permissions
        /// </summary>
        /// <param name="instanceData">The instance data</param>
        public void SetInstanceEditButtonStates(InstanceData instanceData)
        {
            if (instanceData.HasPermission(XaclActionType.Write))
            {
                _menuItemStates.SetState(MenuItemID.EditSaveInstance, true);
            }
            else
            {
                _menuItemStates.SetState(MenuItemID.EditSaveInstance, false);
                _menuItemStates.SetState(MenuItemID.ToolBuildFullTextIndex, false);
            }

            ClassElement classElement = _metaData.SchemaModel.FindClass(this._currentSlide.DataView.BaseClass.Name);
            if (classElement.IsLeaf &&
                PermissionChecker.Instance.HasPermission(_metaData.XaclPolicy,
                    classElement, XaclActionType.Create))
            {
                _menuItemStates.SetState(MenuItemID.EditNewInstance, true);
                _menuItemStates.SetState(MenuItemID.EditSaveInstanceAs, true);
            }
            else
            {
                _menuItemStates.SetState(MenuItemID.EditNewInstance, false);
                _menuItemStates.SetState(MenuItemID.EditSaveInstanceAs, false);
            }

            if (instanceData.HasPermission(XaclActionType.Delete))
            {
                _menuItemStates.SetState(MenuItemID.EditDeleteInstance, true);
            }
            else
            {
                _menuItemStates.SetState(MenuItemID.EditDeleteInstance, false);
            }
        }

        #region private methods

        /// <summary>
        /// Convert a row index of displayed row into the row index of corresponding data table row
        /// </summary>
        /// <param name="displayedRowIndex"></param>
        /// <returns></returns>
        private int ConvertToDataTableRowIndex(int displayedRowIndex)
        {
            CurrencyManager cm = (CurrencyManager)dataGrid1.BindingContext[_currentSlide.DataSet, _currentSlide.DataView.BaseClass.Name];
            DataView dv = (DataView)cm.List;
            DataRowView dataRowView = dv[displayedRowIndex];
            DataRow dataRow = dataRowView.Row;

            // convert to the row index in the datatable
            int selectedRowIndex = 0;
            DataTable dataTable = dataRow.Table;
            foreach (DataRow dr in dataTable.Rows)
            {
                if (dr == dataRow)
                {
                    break;
                }

                selectedRowIndex++;
            }

            return selectedRowIndex;
        }

        /// <summary>
        /// Navigate back to parent data slide
        /// </summary>
        private void NavigateBackToParent()
        {
            if (_slides.Count > 1)
            {
                _slides.Remove(_currentSlide); // remove the last slide
                _currentSlide = (DataViewSlide)_slides[_slides.Count - 1]; // make the last slide as current slide
                ShowDataSlide();
            }
        }

        /// <summary>
        /// Set the menu item states related to the current data slide
        /// </summary>
        private void SetMenuItemStates(DataViewSlide dataSlide)
        {
            if (dataSlide != null)
            {
                ClassElement classElement = _metaData.SchemaModel.FindClass(dataSlide.DataView.BaseClass.Name);
                if (classElement.IsLeaf &&
                    Newtera.Common.MetaData.XaclModel.PermissionChecker.Instance.HasPermission(_metaData.XaclPolicy,
                    classElement, XaclActionType.Delete))
                {
                    this._menuItemStates.SetState(MenuItemID.EditDeleteAllInstances, true);
                }
                else
                {
                    this._menuItemStates.SetState(MenuItemID.EditDeleteAllInstances, false);
                }

                if (Newtera.Common.MetaData.XaclModel.PermissionChecker.Instance.HasPermission(_metaData.XaclPolicy,
                    classElement, XaclActionType.Download))
                {
                    this._menuItemStates.SetState(MenuItemID.FileExport, true);
                }
                else
                {
                    this._menuItemStates.SetState(MenuItemID.FileExport, false);
                }
            }
        }

        private void FireRowSelectedIndexChangedEvent()
        {
            if (this.RowSelectedIndexChangedEvent != null)
            {
                RowSelectedIndexChangedEvent(this, new EventArgs());
            }
        }

        private void FireRequestForDataEvent()
        {
            if (this.RequestForDataEvent != null)
            {
                RequestForDataEvent(this, new EventArgs());
            }
        }

        private void FireRequestForCountEvent()
        {
            if (this.RequestForCountEvent != null)
            {
                RequestForCountEvent(this, new EventArgs());
            }
        }

        private void FireRequestForSaveInstancesEvent()
        {
            if (this.RequestForSaveInstancesEvent != null)
            {
                RequestForSaveInstancesEvent(this, new EventArgs());
            }
        }

        /// <summary>
        /// Add search filters to the data view so that only qualified instances
        /// that are related to the instance of given obj_id are returned.
        /// </summary>
        /// <param name="dataView">Adding search filter to this data view</param>
        /// <param name="referringRelationship">Referring relationship providing info to construct search filter</param>
        /// <param name="objId">The obj id as filter value</param>
        private void AddSearchFilter(DataViewModel dataView, RelationshipAttributeElement referringRelationship, string objId)
        {
            if (referringRelationship.IsForeignKeyRequired)
            {
                // the parent relationship is of ManyToOne or OneToOne/Not Join manager type
                // The given obj_id is the id of the single related instance
                // in this base class. Therefore, we should add a
                // $baseClass/@obj_id = objId expression to the data view so that the query
                // only returns one instance
                DataSimpleAttribute left = new DataSimpleAttribute(NewteraNameSpace.OBJ_ID_ATTRIBUTE, dataView.BaseClass.Alias);
                Parameter right = new Parameter(NewteraNameSpace.OBJ_ID_ATTRIBUTE, dataView.BaseClass.Alias, DataType.String);
                right.ParameterValue = objId;
                RelationalExpr relationalExpr = new RelationalExpr(ElementType.Equals, left, right);
                dataView.AddSearchExpr(relationalExpr, ElementType.And);
            }
            else
            {
                // the referring relationship is of OneToMany, ManyToMany or OneToOne/Join Manager
                // type. The given obj_id is the id of the instance at the parent class.
                // Therefore, we should add a $referringClass/@obj_id = objId
                // expression to the data view. where $referringClass is obtained
                // from a let $referringClass := $baseClass/@relationship=>ParentClass clause.
                // The query will return zero or more instances

                // find the referenced class from data view that represents the referring class
                DataClass referencedClass = null;
                foreach (DataClass refClass in dataView.ReferencedClasses)
                {
                    if (refClass.ReferringRelationshipName == referringRelationship.BackwardRelationshipName)
                    {
                        referencedClass = refClass;
                        break;
                    }
                }

                DataSimpleAttribute left = new DataSimpleAttribute(NewteraNameSpace.OBJ_ID_ATTRIBUTE, referencedClass.Alias);
                Parameter right = new Parameter(NewteraNameSpace.OBJ_ID_ATTRIBUTE, referencedClass.Alias, DataType.String);
                right.ParameterValue = objId;
                RelationalExpr relationalExpr = new RelationalExpr(ElementType.Equals, left, right);
                dataView.AddSearchExpr(relationalExpr, ElementType.And);
            }
        }

        /// <summary>
        /// Enable or disable instance edit buttons
        /// </summary>
        /// <param name="state">true to enable, false to disable</param>
        public void EnableInstanceEditButtons(bool state)
        {
            this._menuItemStates.SetState(MenuItemID.EditNewInstance, state);
            this._menuItemStates.SetState(MenuItemID.EditSaveInstance, state);
            this._menuItemStates.SetState(MenuItemID.EditSaveInstanceAs, state);
            this._menuItemStates.SetState(MenuItemID.EditDeleteInstance, state);
            this._menuItemStates.SetState(MenuItemID.EditDeleteAllInstances, state);
        }

        /// <summary>
        /// Get the relationship attribute of a given name
        /// </summary>
        /// <param name="classAlias">The unique class alias</param>
        /// <param name="relationshipName">The relationship name</param>
        /// <returns>The relationship attribute</returns>
        private RelationshipAttributeElement GetRelationshipAttribute(string classAlias, string relationshipName)
        {
            // first find the class in data view using class alias
            DataClass dataClass = _currentSlide.DataView.FindClass(classAlias);
            // Then get the corresponding class element from schema model
            ClassElement classElement = _currentSlide.DataView.SchemaModel.FindClass(dataClass.ClassName);

            // lastly, find the relationship attribute element from the class element
            RelationshipAttributeElement relationshipAttribute = classElement.FindInheritedRelationshipAttribute(relationshipName);

            return relationshipAttribute;
        }

        /// <summary>
        /// Event handler called when the Data View Slide Count is changed
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void dataViewSlideCountChanged(object sender, System.EventArgs e)
        {
            DataViewSlideCollection slides = (DataViewSlideCollection)sender;

            if (slides.Count > 1)
            {
                this.backButton.Enabled = true;
            }
            else
            {
                this.backButton.Enabled = false;
            }
        }

        /// <summary>
        /// Change the relation names generated by DataSet to the more miningful names
        /// defined by the data view.
        /// </summary>
        /// <param name="dataSet">The data set</param>
        private void ChangeRelationNames(DataSet dataSet)
        {
            DataRelationCollection dataRelations = dataSet.Relations;

            foreach (DataRelation dataRelation in dataRelations)
            {
                string relationName = dataRelation.RelationName;

                // the relation name is in format of ClassName_ClassName_RelationshipName
                int pos = relationName.LastIndexOf("_");
                string relationshipAttributeName = relationName.Substring(pos + 1);

                // Get the caption of the relationship as relation name
                ResultAttributeCollection resultAttributes = _currentSlide.DataView.ResultAttributes;
                foreach (IDataViewElement resultAttribute in resultAttributes)
                {
                    if (resultAttribute.ElementType == ElementType.RelationshipAttribute &&
                        resultAttribute.Name == relationshipAttributeName)
                    {
                        dataRelation.RelationName = resultAttribute.Caption;
                        break;
                    }
                }
            }
        }

        private void MenuItemStateChanged(object sender, System.EventArgs e)
        {
            StateChangedEventArgs args = (StateChangedEventArgs)e;

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

        private void AttachCustomPrincipal()
        {
            CustomPrincipal principal = Thread.CurrentPrincipal as CustomPrincipal;

            if (principal == null && _userManager != null && _userName != null)
            {
                // attach a custom principal object to the thread
                CustomPrincipal.Attach(_userManager, _serverProxy, _userName);
            }
        }

        #endregion private methods

        #region event handlers

        private void dataGrid1_Click(object sender, EventArgs e)
        {
            if (this.dataGrid1.CurrentRowIndex >= 0)
            {
                int selectedRowIndex = ConvertToDataTableRowIndex(this.dataGrid1.CurrentRowIndex);

                if (_currentSlide != null && _currentSlide.SelectedRowIndex != selectedRowIndex)
                {
                    _currentSlide.SelectedRowIndex = selectedRowIndex;

                    FireRowSelectedIndexChangedEvent();
                }
            }
        }

        private void dataGrid1_Navigate(object sender, NavigateEventArgs ne)
        {
            try
            {
                if (ne.Forward)
                {
                    string caption = this.dataGrid1.DataMember;

                    // stay in the parent table because the child table is useless.
                    this.dataGrid1.NavigateBack();

                    // when navigate to a child table, the dataMember is in format of
                    // ParentTable.RelationshipCaption
                    int pos = caption.IndexOf(".");
                    if (pos > 0)
                    {
                        caption = caption.Substring(pos + 1);
                    }

                    // Get the name of the relationship, asumming that the caption is unique among
                    // the all relationships in results
                    string relationshipName = null;
                    string classAlias = null;
                    ResultAttributeCollection resultAttributes = _currentSlide.DataView.ResultAttributes;
                    foreach (IDataViewElement resultAttribute in resultAttributes)
                    {
                        if (resultAttribute.ElementType == ElementType.RelationshipAttribute &&
                            resultAttribute.Caption == caption)
                        {
                            relationshipName = resultAttribute.Name;
                            classAlias = ((DataRelationshipAttribute)resultAttribute).OwnerClassAlias;
                            break;
                        }
                    }

                    RelationshipAttributeElement relationship = GetRelationshipAttribute(classAlias, relationshipName);

                    // Get the obj_id of currently selected row
                    int displayRowIndex = this.dataGrid1.CurrentRowIndex;
                    if (displayRowIndex >= 0)
                    {
                        int currentRowIndex = ConvertToDataTableRowIndex(displayRowIndex);
                        string objId = _currentSlide.DataSet.Tables[DataRelationshipAttribute.GetRelationshipDataTableName(_currentSlide.DataView.BaseClass.Name, relationshipName)].Rows[currentRowIndex][NewteraNameSpace.OBJ_ID].ToString();

                        // keep the current selected row and its expansion status
                        _currentSlide.SelectedRowIndex = currentRowIndex;
                        _currentSlide.IsRowExpanded = true;

                        if (objId.Length > 0)
                        {
                            // create a new data view slide as current slide and add to the slide list
                            _currentSlide = new DataViewSlide(_menuItemStates);
                            _currentSlide.DataView = _metaData.GetDefaultDataView(relationship.LinkedClassName);

                            // clear the existing search expression in the default data view
                            // so that users can build their own search expression from the
                            // scratch.
                            _currentSlide.DataView.ClearSearchExpression();

                            _slides.Add(_currentSlide);

                            AddSearchFilter(_currentSlide.DataView, relationship, objId);

                            FireRequestForDataEvent();
                        }
                        else
                        {
                            MessageBox.Show(MessageResourceManager.GetString("DataViewer.NoResults"));
                        }
                    }
                }
            }
            catch (Exception e)
            {
                MessageBox.Show(e.Message);
            }
        }

        private void backButton_Click(object sender, EventArgs e)
        {
            AttachCustomPrincipal(); // when the control is embedded in SmartDoc, it creates a new thread each time, we have to attach a custom principal

            NavigateBackToParent();
        }

        private void prevPageButton_Click(object sender, EventArgs e)
        {
            MoveToPrevPage();
        }

        private void prevRowButton_Click(object sender, EventArgs e)
        {
            MoveToPrevRow();
        }

        private void nextRowButton_Click(object sender, EventArgs e)
        {
            MoveToNextRow();
        }

        private void nextPageButton_Click(object sender, EventArgs e)
        {
            MoveToNextPage();
        }

        private void countRowButton_Click(object sender, EventArgs e)
        {
            FireRequestForCountEvent();
        }

        private void saveButton_Click(object sender, EventArgs e)
        {
            FireRequestForSaveInstancesEvent();
        }

        #endregion event handlers
    }
}
