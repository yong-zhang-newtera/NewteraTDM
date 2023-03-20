using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.IO;
using System.Threading;
using System.Windows.Forms;

using Newtera.DataGridActiveX.Pivot;

using Newtera.DataGridActiveX;

namespace Newtera.WindowsControl
{
    public partial class PivotGridDialog : Form
    {
        private IDataGridControl _dataGridControl;
        private IPivotLayoutManager _layoutManager;
        private PivotLayout _selectedLayout;
        private PivotLayoutCollection _pivotLayouts;

        public PivotGridDialog()
        {
            InitializeComponent();

            _dataGridControl = null;
            _selectedLayout = null;
            _layoutManager = null;
            _pivotLayouts = null;

            pivotGridControl.FieldsCustomization();
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
        /// Gets or sets a IPivotLayoutManager instance
        /// </summary>
        public IPivotLayoutManager PivotLayoutManager
        {
            get
            {
                return _layoutManager;
            }
            set
            {
                _layoutManager = value;
            }
        }

        #region private methods

        void ExportTo(string title, string filter, string exportFormat)
        {
            string fileName = ShowSaveFileDialog(title, filter);

            if (fileName != "")
            {
                Cursor currentCursor = Cursor.Current;
                Cursor.Current = Cursors.WaitCursor;

                switch (exportFormat)
                {
                    case "HTML": this.pivotGridControl.ExportToHtml(fileName);
                        break;
                    case "MHT": this.pivotGridControl.ExportToMht(fileName);
                        break;
                    case "PDF": this.pivotGridControl.ExportToPdf(fileName);
                        break;
                    case "XLS": this.pivotGridControl.ExportToXls(fileName);
                        break;
                    case "RTF": this.pivotGridControl.ExportToRtf(fileName);
                        break;
                    case "TXT": this.pivotGridControl.ExportToText(fileName);
                        break;
                }

                Cursor.Current = currentCursor;
                OpenFile(fileName);
            }
        }

        protected string ShowSaveFileDialog(string title, string filter)
        {
            SaveFileDialog dlg = new SaveFileDialog();
            string name = Application.ProductName;
            int n = name.LastIndexOf(".") + 1;
            if (n > 0) name = name.Substring(n, name.Length - n);
            dlg.Title = "Export To " + title;
            dlg.FileName = name;
            dlg.Filter = filter;
            if (dlg.ShowDialog() == DialogResult.OK) return dlg.FileName;
            return "";
        }

        private void OpenFile(string fileName)
        {
            if (DevExpress.XtraEditors.XtraMessageBox.Show("Do you want to open this file?", "Export To...", MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.Yes)
            {
                try
                {
                    System.Diagnostics.Process process = new System.Diagnostics.Process();
                    process.StartInfo.FileName = fileName;
                    process.StartInfo.Verb = "Open";
                    process.StartInfo.WindowStyle = System.Diagnostics.ProcessWindowStyle.Normal;
                    process.Start();
                }
                catch
                {
                    DevExpress.XtraEditors.XtraMessageBox.Show("Cannot find an application on your system suitable for openning the file with exported data.", Application.ProductName, MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
        }

        // init view options of the pivot grid control
        private void InitViewOptions()
        {
            this.pivotGridControl.OptionsView.ShowColumnGrandTotals = true;
            this.pivotGridControl.OptionsView.ShowColumnTotals = false;
            this.pivotGridControl.OptionsView.ShowCustomTotalsForSingleValues = false;
            this.pivotGridControl.OptionsView.ShowColumnHeaders = true;
            this.pivotGridControl.OptionsView.ShowDataHeaders = true;
            this.pivotGridControl.OptionsView.ShowFilterHeaders = true;
            this.pivotGridControl.OptionsView.ShowRowHeaders = true;
            this.pivotGridControl.OptionsView.ShowRowGrandTotals = false;
            this.pivotGridControl.OptionsView.ShowRowTotals = false;
            this.pivotGridControl.OptionsView.ShowTotalsForSingleValues = false;
        }

        // return the unique id of saved pivot layout
        private string SavePivotLayout(PivotLayout pivotLayout)
        {
            try
            {
                // Change the cursor to indicate that we are waiting
                Cursor.Current = Cursors.WaitCursor;

                // Get layout xml
                MemoryStream stream = new MemoryStream();
                pivotGridControl.SaveLayoutToStream(stream, DevExpress.Utils.OptionsLayoutBase.FullLayout);
                stream.Seek(0, System.IO.SeekOrigin.Begin);
                StreamReader reader = new StreamReader(stream, Encoding.UTF8);
                string xml = reader.ReadToEnd();
                return _layoutManager.SaveNamedPivotLayout(pivotLayout.ClassName, pivotLayout.Name,
                    pivotLayout.Description, pivotLayout.ViewName, xml);

            }
            finally
            {
                // Restore the cursor
                Cursor.Current = this.Cursor;
            }
        }

        private void RestorePivotLayout(string pivotLayoutId)
        {
            try
            {
                // Change the cursor to indicate that we are waiting
                Cursor.Current = Cursors.WaitCursor;

                string xml = _layoutManager.GetPivotLayoutXmlById(pivotLayoutId);
                if (!string.IsNullOrEmpty(xml))
                {
                    byte[] bytes = System.Text.Encoding.UTF8.GetBytes(xml);
                    MemoryStream stream = new MemoryStream(bytes);
                    stream.Seek(0, System.IO.SeekOrigin.Begin);
                    pivotGridControl.RestoreLayoutFromStream(stream, DevExpress.Utils.OptionsLayoutBase.FullLayout);
                }
            }
            finally
            {
                // Restore the cursor
                Cursor.Current = this.Cursor;
            }
        }

        private void DeletePivotLayout(PivotLayout pivotLayout)
        {
            try
            {
                // Change the cursor to indicate that we are waiting
                Cursor.Current = Cursors.WaitCursor;

                _layoutManager.DeletePivotLayoutById(pivotLayout.ID);

                _pivotLayouts = null; // reload next time
            }
            finally
            {
                // Restore the cursor
                Cursor.Current = this.Cursor;
            }
        }

        private void SavePivotLayoutAs()
        {
            SavePivotLayoutAsDialog dialog = new SavePivotLayoutAsDialog();
            dialog.ExistingPivotLayouts = GetExistintingPivotLayouts();

            if (dialog.ShowDialog() == DialogResult.OK)
            {
                _selectedLayout = new PivotLayout();
                _selectedLayout.Name = dialog.PivotLayoutName;
                _selectedLayout.ClassName = _dataGridControl.BaseClassName;
                _selectedLayout.Description = "";
                _selectedLayout.ViewName = "";

                _selectedLayout.ID = SavePivotLayout(_selectedLayout);

                _pivotLayouts = null; // refresh next time
            }
        }

        private PivotLayoutCollection GetExistintingPivotLayouts()
        {
            if (_pivotLayouts == null)
            {
                _pivotLayouts = _layoutManager.GetPivotLayouts(_dataGridControl.BaseClassName);
            }

            return _pivotLayouts;
        }

        #endregion

        private void pivotGridControl_ShowingCustomizationForm(object sender, CustomizationFormShowingEventArgs e)
        {
            e.ParentControl = splitContainerControl1.Panel1;
            e.CustomizationForm.Dock = DockStyle.Fill;
        }

        private void PivotGridDialog_Load(object sender, EventArgs e)
        {
            if (_dataGridControl != null)
            {
                pivotGridControl.DataSource = _dataGridControl.DataView;

                InitViewOptions();

                // create pivot grid fields from the column infos
                PivotGridField field;
                foreach (ColumnInfo columnInfo in _dataGridControl.ColumnInfos)
                {
                    field = new PivotGridField(columnInfo.Name, PivotArea.FilterArea);
                    field.Caption = columnInfo.Caption;
                    field.Visible = false;

                    pivotGridControl.Fields.Add(field);
                }
            }
        }

        private void pivotGridControl_FieldAreaChanged(object sender, PivotFieldEventArgs e)
        {
            PivotGridField field = e.Field;
            if (field.Area == PivotArea.DataArea && field.Visible)
            {
                field.Options.ShowSummaryTypeName = true;
                field.Options.AllowRunTimeSummaryChange = true;
                field.CellFormat.FormatType = DevExpress.Utils.FormatType.Numeric;
            }
            else
            {
                field.Options.ShowSummaryTypeName = false;
                field.Options.AllowRunTimeSummaryChange = false;
                field.CellFormat.FormatType = DevExpress.Utils.FormatType.None;
            }
        }

        private void exportToPDFToolStripMenuItem_Click(object sender, EventArgs e)
        {
            ExportTo("PDF Document", "PDF Documents|*.pdf", "PDF");
        }

        private void exportToRTFToolStripMenuItem_Click(object sender, EventArgs e)
        {
            ExportTo("RTF Document", "RTF Documents|*.rtf", "RTF");
        }

        private void exportToXSLToolStripMenuItem_Click(object sender, EventArgs e)
        {
            ExportTo("Microsoft Excel Document", "Microsoft Excel|*.xls", "XLS");
        }

        private void exportToTextToolStripMenuItem_Click(object sender, EventArgs e)
        {
            ExportTo("Text Document", "Text Files|*.txt", "TXT");
        }

        private void exportToHTMLToolStripMenuItem_Click(object sender, EventArgs e)
        {
            ExportTo("HTML Document", "HTML Documents|*.html", "HTML");
        }

        private void openToolStripMenuItem_Click(object sender, EventArgs e)
        {
            ChoosePivotLayoutDialog dialog = new ChoosePivotLayoutDialog();
            dialog.ExistingPivotLayouts = GetExistintingPivotLayouts();
            dialog.SelectedPivotLayout = _selectedLayout;

            if (dialog.ShowDialog() == DialogResult.OK)
            {
                _selectedLayout = dialog.SelectedPivotLayout;

                RestorePivotLayout(_selectedLayout.ID);
            }
        }

        private void saveToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (_selectedLayout != null)
            {
                _selectedLayout.ID = SavePivotLayout(_selectedLayout);
            }
            else
            {
                SavePivotLayoutAs();
            }
        }

        private void saveAsToolStripMenuItem_Click(object sender, EventArgs e)
        {
            SavePivotLayoutAs();
        }

        private void deleteLayoutToolStripMenuItem_Click(object sender, EventArgs e)
        {
            ChoosePivotLayoutDialog dialog = new ChoosePivotLayoutDialog();
            dialog.ExistingPivotLayouts = GetExistintingPivotLayouts();
            dialog.SelectedPivotLayout = _selectedLayout;

            if (dialog.ShowDialog() == DialogResult.OK)
            {
                PivotLayout selectedLayout = dialog.SelectedPivotLayout;

                DeletePivotLayout(selectedLayout);
            }
        
        }

        private void showColumnTotalsToolStripMenuItem_CheckedChanged(object sender, EventArgs e)
        {
            this.pivotGridControl.OptionsView.ShowColumnTotals = ((ToolStripMenuItem)sender).Checked;
        }

        private void showColumnGrToolStripMenuItem_CheckedChanged(object sender, EventArgs e)
        {
            this.pivotGridControl.OptionsView.ShowColumnGrandTotals = ((ToolStripMenuItem)sender).Checked;
        }

        private void showRowTotalsToolStripMenuItem_CheckedChanged(object sender, EventArgs e)
        {
            this.pivotGridControl.OptionsView.ShowRowTotals = ((ToolStripMenuItem)sender).Checked;
        }

        private void showRowGrandTotalsToolStripMenuItem_CheckedChanged(object sender, EventArgs e)
        {
            this.pivotGridControl.OptionsView.ShowRowGrandTotals = ((ToolStripMenuItem)sender).Checked;
        }

        private void showColumnHeadersToolStripMenuItem_CheckedChanged(object sender, EventArgs e)
        {
            this.pivotGridControl.OptionsView.ShowColumnHeaders = ((ToolStripMenuItem)sender).Checked;
        }

        private void showRowHeadersToolStripMenuItem_CheckedChanged(object sender, EventArgs e)
        {
            this.pivotGridControl.OptionsView.ShowRowHeaders = ((ToolStripMenuItem)sender).Checked;
        }

        private void showDataHeadersToolStripMenuItem_CheckedChanged(object sender, EventArgs e)
        {
            this.pivotGridControl.OptionsView.ShowDataHeaders = ((ToolStripMenuItem)sender).Checked;
        }

        private void showFilterHeadersToolStripMenuItem_CheckedChanged(object sender, EventArgs e)
        {
            this.pivotGridControl.OptionsView.ShowFilterHeaders = ((ToolStripMenuItem)sender).Checked;
        }

        private void viewOptionsToolStripMenuItem_DropDownOpening(object sender, EventArgs e)
        {
            foreach (ToolStripMenuItem menuItem in this.viewOptionsToolStripMenuItem.DropDownItems)
            {
                if (menuItem.Tag != null)
                {
                    switch (((string )menuItem.Tag))
                    {
                        case "ShowColumnGrandTotals":
                           menuItem.Checked = this.pivotGridControl.OptionsView.ShowColumnGrandTotals;
                           break;

                        case "ShowColumnTotals":
                            menuItem.Checked = this.pivotGridControl.OptionsView.ShowColumnTotals;
                            break;

                        case "ShowCustomTotalsForSingleValues":
                            menuItem.Checked = this.pivotGridControl.OptionsView.ShowCustomTotalsForSingleValues;
                            break;

                        case "ShowColumnHeaders":
                            menuItem.Checked = this.pivotGridControl.OptionsView.ShowColumnHeaders;
                            break;

                        case "ShowDataHeaders":
                            menuItem.Checked = this.pivotGridControl.OptionsView.ShowDataHeaders;
                            break;

                        case "ShowFilterHeaders":
                            menuItem.Checked = this.pivotGridControl.OptionsView.ShowFilterHeaders;
                            break;

                        case "ShowRowHeaders":
                            menuItem.Checked = this.pivotGridControl.OptionsView.ShowRowHeaders;
                            break;

                        case "ShowRowGrandTotals":
                            menuItem.Checked = this.pivotGridControl.OptionsView.ShowRowGrandTotals;
                            break;

                        case "ShowRowTotals":
                            menuItem.Checked = this.pivotGridControl.OptionsView.ShowRowTotals;
                            break;

                        case "ShowTotalsForSingleValues":
                            menuItem.Checked = this.pivotGridControl.OptionsView.ShowTotalsForSingleValues;
                            break;
                    }
                }
            }
        }
    }
}