using System;
using System.Data;
using System.Drawing;
using System.Collections;
using System.ComponentModel;
using System.Windows.Forms;

namespace Newtera.WindowsControl
{
	/// <summary>
	/// Summary description for EditArrayValueDialog.
	/// </summary>
	public class EditArrayValueDialog : System.Windows.Forms.Form
	{
		private DataTable _dataTable = null;

		private System.Windows.Forms.DataGrid dataGrid1;
		private System.Windows.Forms.Button OKBtn;
		private System.Windows.Forms.Button CancelBtn;
		private System.Windows.Forms.NumericUpDown rowNumericUpDown;
		private System.Windows.Forms.Label label1;

		/// <summary>
		/// Required designer variable.
		/// </summary>
		private System.ComponentModel.Container components = null;

		public EditArrayValueDialog()
		{
			//
			// Required for Windows Form Designer support
			//
			InitializeComponent();

			//
			// TODO: Add any constructor code after InitializeComponent call
			//
		}

		/// <summary>
		/// Gets or sets the DataTable instance for editing
		/// </summary>
		public DataTable DataTable
		{
			get
			{
				return _dataTable;
			}
			set
			{
				_dataTable = value.Copy();
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

		/// <summary>
		/// gets information indicating whether the array values in the DataTable object
		/// are valid.
		/// </summary>
		/// <returns>True if valid, false otherwise</returns>
		private bool ValidateDataTableValues()
		{ 
			bool status = true;

			for (int row = 0; row < _dataTable.Rows.Count; row++)
			{
				for (int col = 0; col < _dataTable.Columns.Count; col++)
				{
                    // TODO validate array values
                    /*
					if (_dataTable.Rows[row].IsNull(col))
					{
						status = false;
						break;
					}
                    */
				}
			}

			return status;
		}

		#region Windows Form Designer generated code
		/// <summary>
		/// Required method for Designer support - do not modify
		/// the contents of this method with the code editor.
		/// </summary>
		private void InitializeComponent()
		{
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(EditArrayValueDialog));
            this.dataGrid1 = new System.Windows.Forms.DataGrid();
            this.OKBtn = new System.Windows.Forms.Button();
            this.CancelBtn = new System.Windows.Forms.Button();
            this.rowNumericUpDown = new System.Windows.Forms.NumericUpDown();
            this.label1 = new System.Windows.Forms.Label();
            ((System.ComponentModel.ISupportInitialize)(this.dataGrid1)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.rowNumericUpDown)).BeginInit();
            this.SuspendLayout();
            // 
            // dataGrid1
            // 
            resources.ApplyResources(this.dataGrid1, "dataGrid1");
            this.dataGrid1.DataMember = "";
            this.dataGrid1.HeaderForeColor = System.Drawing.SystemColors.ControlText;
            this.dataGrid1.Name = "dataGrid1";
            // 
            // OKBtn
            // 
            resources.ApplyResources(this.OKBtn, "OKBtn");
            this.OKBtn.DialogResult = System.Windows.Forms.DialogResult.OK;
            this.OKBtn.Name = "OKBtn";
            this.OKBtn.Click += new System.EventHandler(this.OKBtn_Click);
            // 
            // CancelBtn
            // 
            resources.ApplyResources(this.CancelBtn, "CancelBtn");
            this.CancelBtn.DialogResult = System.Windows.Forms.DialogResult.Cancel;
            this.CancelBtn.Name = "CancelBtn";
            // 
            // rowNumericUpDown
            // 
            resources.ApplyResources(this.rowNumericUpDown, "rowNumericUpDown");
            this.rowNumericUpDown.Maximum = new decimal(new int[] {
            500,
            0,
            0,
            0});
            this.rowNumericUpDown.Name = "rowNumericUpDown";
            this.rowNumericUpDown.ValueChanged += new System.EventHandler(this.rowNumericUpDown_ValueChanged);
            // 
            // label1
            // 
            resources.ApplyResources(this.label1, "label1");
            this.label1.Name = "label1";
            // 
            // EditArrayValueDialog
            // 
            resources.ApplyResources(this, "$this");
            this.Controls.Add(this.label1);
            this.Controls.Add(this.rowNumericUpDown);
            this.Controls.Add(this.CancelBtn);
            this.Controls.Add(this.OKBtn);
            this.Controls.Add(this.dataGrid1);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.SizableToolWindow;
            this.Name = "EditArrayValueDialog";
            this.Load += new System.EventHandler(this.EditArrayValueDialog_Load);
            ((System.ComponentModel.ISupportInitialize)(this.dataGrid1)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.rowNumericUpDown)).EndInit();
            this.ResumeLayout(false);

		}
		#endregion

		private void EditArrayValueDialog_Load(object sender, System.EventArgs e)
		{
			if (_dataTable != null)
			{
				dataGrid1.DataSource = _dataTable;
				this.rowNumericUpDown.Value = _dataTable.Rows.Count;
			}
		}

		private void OKBtn_Click(object sender, System.EventArgs e)
		{
			if (!ValidateDataTableValues())
			{
				MessageBox.Show(MessageResourceManager.GetString("DataViewer.InvalidArrayValue"));

				this.DialogResult = DialogResult.None;
				return;
			}		
		}

		private void rowNumericUpDown_ValueChanged(object sender, System.EventArgs e)
		{
			int rows = Decimal.ToInt32(this.rowNumericUpDown.Value);
			
			if (rows > _dataTable.Rows.Count)
			{
				// add an row
				DataRow dataRow = _dataTable.NewRow();

				_dataTable.Rows.Add(dataRow);
			}
			else if (rows < _dataTable.Rows.Count)
			{
				// remove the last row
				_dataTable.Rows.RemoveAt(_dataTable.Rows.Count - 1);
			}
		}
	}
}
