using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Xml;
using System.Threading;
using System.Windows.Forms;
using System.Security.Principal;

using Newtera.Common.Core;
using Newtera.Common.MetaData;
using Newtera.Common.MetaData.Schema;
using Newtera.Common.MetaData.DataView;
using Newtera.Common.MetaData.DataView.Taxonomy;
using Newtera.Common.MetaData.XaclModel;
using Newtera.Common.MetaData.Principal;
using Newtera.WindowsControl;
using Newtera.WinClientCommon;

namespace Newtera.Studio
{
    public partial class ViewLoggingMessageDialog : Form
    {
        private string _schemaId; // search value
        private string _className; // search value
        private LoggingServiceStub _loggingService;
        private MetaDataModel _metaData = null;
        private Newtera.Common.Core.SchemaInfo _schemaInfo; // Logging schema info
        private MenuItemStates _menuItemStates;

        public ViewLoggingMessageDialog()
        {
            InitializeComponent();

            _loggingService = new LoggingServiceStub();
            _schemaInfo = null;

            _schemaId = null;
            _className = null;

            _menuItemStates = new MenuItemStates();
            this.resultDataControl.MenuItemStates = _menuItemStates;
            this.resultDataControl.UserManager = new WindowClientUserManager();
            this.resultDataControl.ServerProxy = new WindowClientServerProxy();
        }

        /// <summary>
        /// Gets or sets the schema id in which the logging messages are restricted
        /// </summary>
        public string SchemaId
        {
            get
            {
                return _schemaId;
            }
            set
            {
                _schemaId = value;
            }
        }

        /// <summary>
        /// Gets or sets the name of a class in which the logging messages are restricted
        /// </summary>
        public string ClassName
        {
            get
            {
                return _className;
            }
            set
            {
                _className = value;
            }
        }

        private void ShowSearchDialog()
        {
            CreateSearchExprDialog dialog = new CreateSearchExprDialog();
 
            dialog.DataView = this.resultDataControl.CurrentSlide.DataView;
            if (dialog.ShowDialog() == DialogResult.OK)
            {
                ExecuteQuery();
            }
        }

        private Newtera.Common.Core.SchemaInfo LoggingSchemaInfo
        {
            get
            {
                if (_schemaInfo == null)
                {
                    _schemaInfo = new SchemaInfo();
                    _schemaInfo.Name = "LoggingInfo";
                    _schemaInfo.Version = "1.0";
                }

                return _schemaInfo;
            }
        }

        private DataViewModel GetRestrictedDataView(DataViewModel originalDataView)
        {
            // duplicate the data view so that we can add additional search criteria such
            // as SchemaId and ClassName without altering the original data view
            DataViewModel newDataView = originalDataView.Clone();
            string classAlias = originalDataView.BaseClass.Alias;
            newDataView.PageSize = originalDataView.PageSize;
            newDataView.PageIndex = originalDataView.PageIndex;

            DataSimpleAttribute left = new DataSimpleAttribute("dbId", classAlias);
            Parameter right = new Parameter("dbId", classAlias, DataType.String);
            right.ParameterValue = _schemaId;
            RelationalExpr expr = new RelationalExpr(ElementType.Equals, left, right);
            newDataView.AddSearchExpr(expr, ElementType.And);

            left = new DataSimpleAttribute("cName", classAlias);
            right = new Parameter("cName", classAlias, DataType.String);
            right.ParameterValue = _className;
            expr = new RelationalExpr(ElementType.Equals, left, right);
            newDataView.AddSearchExpr(expr, ElementType.And);

            return newDataView;
        }

        private void ShowLoggingMessages()
        {
            DataViewModel dataView;
            IPrincipal principal = Thread.CurrentPrincipal;
            try
            {
                // set thread's principal to a generic pricipal to disable permission checking in
                // GetDetailedDataView
                string[] rolesArray = new string[0];
                Thread.CurrentPrincipal = new GenericPrincipal(new GenericIdentity("DesignStudio"), rolesArray);

                dataView = _metaData.GetDetailedDataView("DataOperations"); // DataOperations is the class name for data related logging records
            }
            finally
            {
                Thread.CurrentPrincipal = principal; // restore the principal
            }

            if (dataView != null)
            {
                this.resultDataControl.Slides.Clear();
                this.resultDataControl.CurrentSlide = new DataViewSlide(_menuItemStates);
                this.resultDataControl.CurrentSlide.DataView = dataView;
                this.resultDataControl.CurrentSlide.InstanceDataView = dataView;

                // clear the search expression in the default data view, so that
                // users can build their own search expression using search
                // expression builder from the scratch
                this.resultDataControl.CurrentSlide.DataView.ClearSearchExpression();

                // get the instance data
                ExecuteQuery();
            }
        }

        private void ExecuteQuery()
        {
            if (this.resultDataControl.CurrentSlide.DataView != null)
            {
                string query = GetRestrictedDataView(this.resultDataControl.CurrentSlide.DataView).SearchQuery;

                // invoke the web service synchronously
                XmlNode xmlNode = _loggingService.ExecuteLoggingQuery(query);

                DataSet ds = new DataSet();
                XmlReader xmlReader = new XmlNodeReader(xmlNode);
                ds.ReadXml(xmlReader);

                this.resultDataControl.ShowQueryResult(ds);
                // clear the InstanceView which may contains the old DataSet
                this.resultDataControl.CurrentSlide.InstanceView = null;
                if (this.resultDataControl.CurrentSlide != null && !this.resultDataControl.CurrentSlide.IsEmptyResult)
                {
                    this.deatilButton.Enabled = true;
                }
                else
                {
                    this.deatilButton.Enabled = false;
                }
            }
        }

        private void searchButton_Click(object sender, EventArgs e)
        {
            ShowSearchDialog();
        }

        private void deatilButton_Click(object sender, EventArgs e)
        {
            InstanceView instanceView = GetInstanceView();
            if (instanceView != null)
            {
                // show the instance data in a dialog
                InstanceDetailDialog dialog = new InstanceDetailDialog();
                dialog.InstanceView = instanceView;
                dialog.ShowDialog();
            }
        }

        /// <summary>
        /// Gets an InstanceView representing the detailed instance data for
        /// the currently selected row in the datagrid.
        /// </summary>
        /// <returns>An InstanceView representing the detailed instance for the currently selected row in the datagrid</returns>
        private InstanceView GetInstanceView()
        {
            return GetInstanceView(-1);
        }

        /// <summary>
        /// Gets an InstanceView representing the instance data of
        /// the given row index in the datagrid.
        /// </summary>
        /// <param name="rowIndex">The row index, -1 for the currently selected index.</param>
        /// <returns>An InstanceView representing the data instance for the specified row</returns>
        private InstanceView GetInstanceView(int rowIndex)
        {
            if (rowIndex >= 0)
            {
                this.resultDataControl.SetCurrentSlideSelectedRowIndex(rowIndex);
            }

            if (this.resultDataControl.CurrentSlide.InstanceView == null)
            {
                if (this.resultDataControl.CurrentSlide.InstanceDataView == null)
                {
                    // uses the same dataview of the data slide
                    this.resultDataControl.CurrentSlide.InstanceDataView = this.resultDataControl.CurrentSlide.DataView;
                }

                this.resultDataControl.CurrentSlide.InstanceView = new InstanceView(this.resultDataControl.CurrentSlide.InstanceDataView,
                    this.resultDataControl.CurrentSlide.DataSet);
            }

            // this assign the selected instance to the InstanceView object
            this.resultDataControl.CurrentSlide.InstanceView.SelectedIndex = this.resultDataControl.CurrentSlide.SelectedRowIndex;

            return this.resultDataControl.CurrentSlide.InstanceView;
        }

        /// <summary>
        /// Gets and display the total instance count of the current query.
        /// </summary>
        private void ExecuteQueryCount()
        {
            if (this.resultDataControl.CurrentSlide.DataView != null)
            {
                string query = GetRestrictedDataView(this.resultDataControl.CurrentSlide.DataView).SearchQuery;

                // invoke the web service synchronously

                int count = _loggingService.ExecuteLoggingCount(query);

                this.resultDataControl.ShowInstanceCount(count);
            }
        }

        private void GetLoggingMetaData()
        {
            if (_metaData == null)
            {
                _metaData = new MetaDataModel();

                // Note, make sure to set SchemaInfo to meta-data model before
                // loading xml strings
                _metaData.SchemaInfo = LoggingSchemaInfo;

                // create a MetaDataModel instance from the xml strings retrieved from the database
                string[] xmlStrings = _loggingService.GetLoggingMetaData();

                // read mete-data from xml strings
                _metaData.Load(xmlStrings);
            }

            this.resultDataControl.MetaData = _metaData;
		}

        #region EventHandlers

        private void ViewLoggingMessageDialog_Load(object sender, EventArgs e)
        {
            GetLoggingMetaData();

            ShowLoggingMessages(); // show the first page of logging message
        }

        private void resultDataControl1_RequestForCountEvent(object sender, EventArgs e)
        {
            ExecuteQueryCount();
        }

        private void resultDataControl1_RequestForDataEvent(object sender, EventArgs e)
        {
            ExecuteQuery();
        }

        private void resultDataControl1_RowSelectedIndexChangedEvent(object sender, EventArgs e)
        {

        }

        #endregion EvenetHandlers
    }
}