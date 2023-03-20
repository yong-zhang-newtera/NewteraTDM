using System;
using System.IO;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;

using Newtera.DataGridActiveX.Export;
using Newtera.DataGridActiveX.ChartModel;
using Newtera.DataGridActiveX.ActiveXControlWebService;

namespace Newtera.DataGridActiveX
{
    public partial class ShowTemplatesDialog : Form
    {
        private const int XML_BLOCK_SIZE = 50000; // xml block size to be sent over intranet

        private bool _isForWindowsClient;
        private ChartDef _chartDef;
        private WorkInProgressDialog _workInProgressDialog;
        private bool _isRequestComplete;
        private string _connectionString;
        private ActiveXControlService _service;
        private ChartInfoCollection _chartTemplates;
        private ChartType _chartType;
        private StringBuilder _currentXMLBuffer;
        private int _currentBlockNum;
        private EventType _eventType = EventType.Unknown;
        private ChartFormat _chartFormat;
        private ChartFormatCollection _chartFormats;
        private IDataGridControl _dataGridControl;

        public ShowTemplatesDialog()
        {
            InitializeComponent();

            _isForWindowsClient = false;
            _dataGridControl = null;
            _chartDef = null;
            _isRequestComplete = false;
            _workInProgressDialog = new WorkInProgressDialog();
            _service = null;
            _connectionString = null;
            _chartTemplates = null;
            _chartType = ChartType.Line;
        }

        /// <summary>
        /// Gets or sets the DataGridControl instance
        /// </summary>
        public IDataGridControl DataGridControl
        {
            get
            {
                return _dataGridControl;
            }
            set
            {
                _dataGridControl = value;
            }
        }

        /// <summary>
        /// Gets or sets the connection string that is used to connect the server
        /// </summary>
        public string ConnectionString
        {
            get
            {
                return _connectionString;
            }
            set
            {
                _connectionString = value;
            }
        }

        /// <summary>
        /// Gets or sets the information indicating whether the wizard is launched
        /// from the Windows Client or not
        /// </summary>
        public bool IsForWindowsClient
        {
            get
            {
                return _isForWindowsClient;
            }
            set
            {
                _isForWindowsClient = value;
            }
        }

        #region private methods

        private void LoadChartTemplates()
        {
            // Change the cursor to indicate that we are waiting
            Cursor.Current = Cursors.WaitCursor;

            if (_service == null)
            {
                _service = this._dataGridControl.CreateActiveXControlWebService();
            }

            _chartTemplates = HandleChartUtil.GetChartTemplates(_service, ConnectionString, _dataGridControl.BaseClassName);
        }

        private delegate void FireEventDelegate();

        /// <summary>
        /// Fire the corresponding event
        /// </summary>
        private void FireEvent()
        {
            if (this.InvokeRequired == false)
            {
                // it is the UI thread, fire event
                switch (this._eventType)
                {
                    case EventType.ChartEvent:
                        this._dataGridControl.FireGraphEvent();
                        break;

                    case EventType.DownloadEvent:
                        this._dataGridControl.FireDownloadGraphEvent(_chartFormat.Name, _chartFormat.Suffix);
                        break;
                }
            }
            else
            {
                // It is a Worker thread, pass the control to UI thread
                FireEventDelegate fireEvent = new FireEventDelegate(FireEvent);

                this.BeginInvoke(fireEvent);
            }
        }

        /// <summary>
        /// Save an unnamed chart data to the server for a web client to download
        /// </summary>
        private void SaveUnnamedChartInfo()
        {
            StringBuilder builder = new StringBuilder();
            StringWriter writer = new StringWriter(builder);
            this._chartDef.Write(writer);
            int length;

            if (builder.Length > XML_BLOCK_SIZE)
            {
                // save the xml for subsequent transmitting
                this._currentXMLBuffer = builder;
                length = XML_BLOCK_SIZE;
            }
            else
            {
                this._currentXMLBuffer = null;
                length = builder.Length;
            }
            this._currentBlockNum = 0;

            // invoke the web service asynchronously
            string chartType = HandleChartUtil.GetChartTypeStr(_chartType);
            _isRequestComplete = false;
            if (_service == null)
            {
                _service = this._dataGridControl.CreateActiveXControlWebService();
            }
            _service.BeginSaveWorkingChartInfo(ConnectionString, chartType,
                builder.ToString(0, length),
                _currentBlockNum,
                new AsyncCallback(SaveUnnamedChartInfoDone),
                null);

            // launch a work in progress dialog
            this._workInProgressDialog.MaximumSteps = builder.Length / XML_BLOCK_SIZE + 1;
            this._workInProgressDialog.Value = 1;
            ShowWorkingDialog(MessageResourceManager.GetString("GraphWizard.SaveChartData"));
        }

        /// <summary>
        /// The AsyncCallback event handler for Web service method CacheChartInfoDone.
        /// </summary>
        /// <param name="res">The result</param>
        private void SaveUnnamedChartInfoDone(IAsyncResult res)
        {
            try
            {
                _service.EndSaveWorkingChartInfo(res);

                this._workInProgressDialog.PerformStep(); // move one step forward

                // It is a Worker thread, continue to send xml data if necessary
                if (_currentXMLBuffer != null)
                {
                    string chartType = HandleChartUtil.GetChartTypeStr(_chartType);

                    while (true)
                    {
                        _currentBlockNum++;
                        int start = _currentBlockNum * XML_BLOCK_SIZE;
                        int length;
                        if (start >= _currentXMLBuffer.Length)
                        {
                            _currentXMLBuffer = null;
                            break;
                        }
                        else
                        {
                            if (_currentXMLBuffer.Length > (start + XML_BLOCK_SIZE))
                            {
                                length = XML_BLOCK_SIZE;
                            }
                            else
                            {
                                length = _currentXMLBuffer.Length - start;
                            }
                        }

                        _service.SaveWorkingChartInfo(ConnectionString,
                            chartType,
                            _currentXMLBuffer.ToString(start, length),
                            _currentBlockNum);

                        this._workInProgressDialog.PerformStep(); // move one step forward
                    }
                }

                this._chartDef.IsAltered = false;

                FireEvent();
            }
            catch (Exception ex)
            {
                //MessageBox.Show(ex.Message);
                MessageBox.Show(ex.StackTrace);
            }
            finally
            {
                //Bring down the work in progress dialog
                HideWorkingDialog();
            }
        }

        /// <summary>
        /// Show the working dialog
        /// </summary>
        /// <param name="msg">The message to display on the working dialog</param>
        /// <remarks>It has to deal with multi-threading issue</remarks>
        private void ShowWorkingDialog(string msg)
        {
            lock (_workInProgressDialog)
            {
                // check _isRequestComplete flag in case the worker thread
                // completes the request before the working dialog is shown
                if (!_isRequestComplete && !_workInProgressDialog.Visible)
                {
                    _workInProgressDialog.DisplayText = msg;
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
                lock (_workInProgressDialog)
                {
                    _workInProgressDialog.Close();
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


        #endregion

        private void ShowTemplatesDialog_Load(object sender, EventArgs e)
        {
            LoadChartTemplates();

            this.chartTemplatesListBox.Items.Clear();
            this.chartTemplatesListBox.DisplayMember = "Name";
            for (int i = 0; i < _chartTemplates.Count; i++)
            {
                this.chartTemplatesListBox.Items.Add(_chartTemplates[i]);
            }

            if (_chartTemplates.Count > 0)
            {
                this.chartTemplatesListBox.SelectedIndex = 0;
            }

            _chartFormats = HandleChartUtil.GetChartFormats(_service);

            if (_chartFormats.Count > 0)
            {
                chartFormatComboBox.DataSource = _chartFormats;
                chartFormatComboBox.DisplayMember = "Name";
                chartFormatComboBox.SelectedIndex = 0;
            }
        }

        private void viewChartButton_Click(object sender, EventArgs e)
        {
            try
            {
                if (this.chartTemplatesListBox.SelectedIndex >= 0)
                {
                    ChartInfo chartInfo = (ChartInfo)this.chartTemplatesListBox.SelectedItem;
                    if (_service == null)
                    {
                        _service = this._dataGridControl.CreateActiveXControlWebService();
                    }

                    if (_chartDef == null)
                    {
                        _chartDef = HandleChartUtil.GetChartDef(_service, chartInfo);
                        _chartDef.IsAltered = true;
                    }

                    this._eventType = EventType.ChartEvent;

                    if (this._chartDef.IsAltered)
                    {
                        // re-generate chart info
                        HandleChartUtil.FillDataSeries(_chartDef, _dataGridControl);

                        if (!this.IsForWindowsClient)
                        {
                            // save the chart to server so that web client can download it from the server
                            SaveUnnamedChartInfo();
                        }
                        else
                        {
                            this._chartDef.IsAltered = false;
                            FireEvent();
                        }
                    }
                    else
                    {
                        // nothing has been changed, the chart info has been cached on
                        // server side. just fire the event
                        FireEvent();
                    }
                }
            }
            catch (Exception ex)
            {
                //MessageBox.Show(ex.Message);
                MessageBox.Show(ex.StackTrace);
            }
        }

        private void chartTemplatesListBox_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (this.chartTemplatesListBox.SelectedIndex >= 0)
            {
                ChartInfo chartInfo = (ChartInfo)this.chartTemplatesListBox.SelectedItem;
                this.viewChartButton.Enabled = true;
                this.downLoadChartButton.Enabled = true;
                this.templateDescTextBox.Text = chartInfo.Description;
                this._chartDef = null;
                this._chartType = (ChartType)Enum.Parse(typeof(ChartType), chartInfo.Type);
            }
            else
            {
                this.viewChartButton.Enabled = false;
                this.downLoadChartButton.Enabled = false;
                this.templateDescTextBox.Text = "";
                this._chartType = ChartType.Line;
            }
        }

        private void downLoadChartButton_Click(object sender, EventArgs e)
        {
            try
            {
                if (this.chartTemplatesListBox.SelectedIndex >= 0)
                {
                    ChartInfo chartInfo = (ChartInfo)this.chartTemplatesListBox.SelectedItem;
                    if (_service == null)
                    {
                        _service = this._dataGridControl.CreateActiveXControlWebService();
                    }

                    if (_chartDef == null)
                    {
                        _chartDef = HandleChartUtil.GetChartDef(_service, chartInfo);
                        _chartDef.IsAltered = true;
                    }

                    this._eventType = EventType.DownloadEvent;
                    this._chartFormat = (ChartFormat)_chartFormats[this.chartFormatComboBox.SelectedIndex];

                    if (this._chartDef.IsAltered)
                    {
                        // re-generate chart info
                        HandleChartUtil.FillDataSeries(_chartDef, _dataGridControl);
                        if (!this.IsForWindowsClient)
                        {
                            SaveUnnamedChartInfo();
                        }
                        else
                        {
                            this._chartDef.IsAltered = false;
                            FireEvent();
                        }
                    }
                    else
                    {
                        // nothing has been changed, the chart info has been cached on
                        // server side. just fire the event
                        FireEvent();
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        private void cancelButton_Click(object sender, EventArgs e)
        {
            this.Close();
        }
    }
}