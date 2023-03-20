using System;
using System.IO;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;

using Newtera.DataGridActiveX.Export;
using Newtera.DataGridActiveX.ActiveXControlWebService;

namespace Newtera.DataGridActiveX
{
    public partial class ComputeResultDialog : Form
    {
        public event RunAlgorithmCallback RunAlgorithmEvent;

        private ActiveXControlService _service;
        private AlgorithmTypeCollection _algorithmTypes;
        private AlgorithmType _selectedType;
        private DataGridControl _dataGridControl;
        private ColumnInfoCollection _columnInfos;
        private string _selectedColumnName;
        private string _baseClassName;

        public ComputeResultDialog()
        {
            InitializeComponent();

            _service = null;
            _algorithmTypes = null;
            _selectedType = null;
            _dataGridControl = null;
            _selectedColumnName = null;
            _baseClassName = null;
        }

        /// <summary>
        /// Gets or sets the Web Service
        /// </summary>
        public ActiveXControlService WebService
        {
            get
            {
                return _service;
            }
            set
            {
                _service = value;
            }
        }

        /// <summary>
        /// Gets selected algorithm type
        /// </summary>
        public AlgorithmType SelectedAlgorithmType
        {
            get
            {
                return this._selectedType;
            }
        }

        /// <summary>
        /// Gets or sets the data grid control that launches this dialog.
        /// </summary>
        public DataGridControl DataGridControl
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
        /// Gets or sets a collection of column infos
        /// </summary>
        public ColumnInfoCollection ColumnInfos
        {
            get
            {
                return _columnInfos;
            }
            set
            {
                if (value != null)
                {
                    _columnInfos = value.Clone();
                }
                else
                {
                    _columnInfos = value;
                }
            }
        }

        /// <summary>
        /// Gets or sets the name of a selected column
        /// </summary>
        public string SelectedColumnName
        {
            get
            {
                return _selectedColumnName;
            }
            set
            {
                _selectedColumnName = value;
            }
        }

        /// <summary>
        /// Gets or sets the name of the base class
        /// </summary>
        public string BaseClassName
        {
            get
            {
                return _baseClassName;
            }
            set
            {
                _baseClassName = value;
            }
        }

        /// <summary>
        /// Set the result string to the result text box
        /// </summary>
        /// <param name="resultString">The result string</param>
        public void SetResult(string resultString)
        {
            this.singleResultTextBox.Text = resultString;
        }

        private void ShowVisibleColumns()
        {
            ListViewItem selectedListViewItem = null;
            foreach (ColumnInfo columnInfo in _columnInfos)
            {
                if (columnInfo.IsChecked)
                {
                    ListViewItem listViewItem = this.columnListView.Items.Add(columnInfo.Caption);
                    if (_selectedColumnName != null && columnInfo.Name == _selectedColumnName)
                    {
                        selectedListViewItem = listViewItem;
                    }
                }
            }

            if (selectedListViewItem != null)
            {
                selectedListViewItem.Selected = true; // make it selected

                selectedListViewItem.EnsureVisible();
            }
        }

        /// <summary>
        /// Sets the column info's visibilities to reflects the selections.
        /// </summary>
        private void SetColumnVisibilities()
        {
            foreach (ColumnInfo columnInfo in _columnInfos)
            {
                columnInfo.IsChecked = false;
            }

            for (int i = 0; i < this.columnListView.SelectedIndices.Count; i++)
            {
                _columnInfos[this.columnListView.SelectedIndices[i]].IsChecked = true;
            }
        }

        private delegate void ShowResultDelegate(string result);

        /// <summary>
        /// Show the result of executing an algorithm in the dialog's text box
        /// </summary>
        private void ShowExecuteResult(string result)
        {
            if (this.InvokeRequired == false)
            {
                // display the result
                this.SetResult(result);
            }
            else
            {
                // It is a Worker thread, pass the control to UI thread
                ShowResultDelegate showResult = new ShowResultDelegate(ShowExecuteResult);

                this.BeginInvoke(showResult, result);
            }
        }

        #region event handlers

        private void computeButton_Click(object sender, EventArgs e)
        {
            if (this.columnListView.SelectedIndices.Count > 0)
            {
                // change the column infos to reflects the column selection
                SetColumnVisibilities();

                if (this.algorithmComboBox.SelectedIndex >= 0 &&
                    this._algorithmTypes != null)
                {
                    _selectedType = this._algorithmTypes[this.algorithmComboBox.SelectedIndex];
                    bool allRows = true;
                    if (!this.allRowsRadioButton.Checked)
                    {
                        allRows = false;
                    }

                    string result = _dataGridControl.ExecuteAlgorithm(_selectedType, allRows);

                    ShowExecuteResult(result);

                    if (!string.IsNullOrEmpty(_selectedType.DisplayURL))
                    {
                        if (RunAlgorithmEvent != null)
                        {
                            RunAlgorithmEvent(_selectedType.DisplayURL);
                        }
                    }
                }
            }
            else
            {
                MessageBox.Show(MessageResourceManager.GetString("ComputeResult.NoColumn"), "Information",
                            MessageBoxButtons.OK,
                            MessageBoxIcon.Information);
            }
        }

        private void ComputeResultDialog_Load(object sender, EventArgs e)
        {
            try
            {
                // display all columns currently shown in the datagrid
                if (_columnInfos != null)
                {
                    ShowVisibleColumns();
                }

                // Change the cursor to indicate that we are waiting
                Cursor.Current = Cursors.WaitCursor;

                string xml = _service.GetAlgorithmTypesInXml();

                AlgorithmTypeCollection algorithmTypes = new AlgorithmTypeCollection();
                StringReader reader = new StringReader(xml);
                algorithmTypes.Read(reader);

                // remove the algorithm that are dedicated for other classes
                _algorithmTypes = new AlgorithmTypeCollection();
                foreach (AlgorithmType algorithmType in algorithmTypes)
                {
                    if (string.IsNullOrEmpty(algorithmType.OwnerClass) ||
                        algorithmType.OwnerClass == _baseClassName)
                    {
                        _algorithmTypes.Add(algorithmType);
                    }
                }

                if (_algorithmTypes.Count > 0)
                {
                    algorithmComboBox.DataSource = _algorithmTypes;
                    algorithmComboBox.DisplayMember = "Name";
                    algorithmComboBox.SelectedIndex = 0;
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Error",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Error);
            }
            finally
            {
                // Restore the cursor
                Cursor.Current = this.Cursor;
            }
        }

        #endregion
    }
}