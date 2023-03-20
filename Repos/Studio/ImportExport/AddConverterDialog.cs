using System;
using System.IO;
using System.Drawing;
using System.Collections;
using System.ComponentModel;
using System.Windows.Forms;

using Newtera.ParserGen.ProjectModel;
using Newtera.Common.MetaData.Mappings;
using Newtera.WindowsControl;

namespace Newtera.Studio.ImportExport
{
	/// <summary>
	/// AddConverterDialog 的摘要说明。
	/// </summary>
	public class AddConverterDialog : System.Windows.Forms.Form
	{
		private bool _isProjectTab;
		private ProjectElement _selectedProject;
		private ArrayList _selectedConverters;
        private string _dllDir;
        private DataSourceType _dataSourceType = DataSourceType.Text;

		private System.Windows.Forms.Button cancelButton;
		private System.Windows.Forms.Button okButton;
		private System.Windows.Forms.Label label1;
		private System.Windows.Forms.Button browseButton;
		private System.Windows.Forms.Button selectButton;
        private System.Windows.Forms.Button deleteButton;
		private System.Windows.Forms.ListView selectedConverterListView;
		private System.Windows.Forms.ColumnHeader columnHeader1;
		private System.Windows.Forms.ColumnHeader columnHeader2;
        private System.Windows.Forms.ColumnHeader columnHeader3;
        private TabPage programTabPage;
        private Label label5;
        private Button findAssemblyButton;
        private TextBox classTextBox;
        private Label label4;
        private TextBox aseemblyTextBox;
        private Label label3;
        private TextBox converterNameTextBox;
        private Label label2;
        private TabPage projectTabPage;
        private ListBox projectParserListBox;
        private TabControl topTabControl;
		/// <summary>
		/// 必需的设计器变量。
		/// </summary>
		private System.ComponentModel.Container components = null;

		public AddConverterDialog()
		{
			//
			// Windows 窗体设计器支持所必需的
			//
			InitializeComponent();

			//
			// TODO: 在 InitializeComponent 调用后添加任何构造函数代码
			//
			_isProjectTab = true;
			_selectedConverters = new ArrayList();
			_dllDir = null;
		}

		/// <summary>
		/// Gets the selected converters
		/// </summary>
		public ArrayList SelectedConverters
		{
			get
			{
				return this._selectedConverters;
			}
		}

        /// <summary>
        /// gets or sets the data source type which is one of
        /// the DataSourceType enum values
        /// </summary>
        public DataSourceType DataSourceType
        {
            get
            {
                return _dataSourceType;
            }
            set
            {
                _dataSourceType = value;
            }
        }

		/// <summary>
		/// 清理所有正在使用的资源。
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

		#region Windows 窗体设计器生成的代码
		/// <summary>
		/// 设计器支持所需的方法 - 不要使用代码编辑器修改
		/// 此方法的内容。
		/// </summary>
		private void InitializeComponent()
		{
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(AddConverterDialog));
            this.cancelButton = new System.Windows.Forms.Button();
            this.okButton = new System.Windows.Forms.Button();
            this.label1 = new System.Windows.Forms.Label();
            this.browseButton = new System.Windows.Forms.Button();
            this.selectButton = new System.Windows.Forms.Button();
            this.deleteButton = new System.Windows.Forms.Button();
            this.selectedConverterListView = new System.Windows.Forms.ListView();
            this.columnHeader1 = new System.Windows.Forms.ColumnHeader();
            this.columnHeader2 = new System.Windows.Forms.ColumnHeader();
            this.columnHeader3 = new System.Windows.Forms.ColumnHeader();
            this.programTabPage = new System.Windows.Forms.TabPage();
            this.label5 = new System.Windows.Forms.Label();
            this.findAssemblyButton = new System.Windows.Forms.Button();
            this.classTextBox = new System.Windows.Forms.TextBox();
            this.label4 = new System.Windows.Forms.Label();
            this.aseemblyTextBox = new System.Windows.Forms.TextBox();
            this.label3 = new System.Windows.Forms.Label();
            this.converterNameTextBox = new System.Windows.Forms.TextBox();
            this.label2 = new System.Windows.Forms.Label();
            this.projectTabPage = new System.Windows.Forms.TabPage();
            this.projectParserListBox = new System.Windows.Forms.ListBox();
            this.topTabControl = new System.Windows.Forms.TabControl();
            this.programTabPage.SuspendLayout();
            this.projectTabPage.SuspendLayout();
            this.topTabControl.SuspendLayout();
            this.SuspendLayout();
            // 
            // cancelButton
            // 
            this.cancelButton.AccessibleDescription = null;
            this.cancelButton.AccessibleName = null;
            resources.ApplyResources(this.cancelButton, "cancelButton");
            this.cancelButton.BackgroundImage = null;
            this.cancelButton.DialogResult = System.Windows.Forms.DialogResult.Cancel;
            this.cancelButton.Font = null;
            this.cancelButton.Name = "cancelButton";
            // 
            // okButton
            // 
            this.okButton.AccessibleDescription = null;
            this.okButton.AccessibleName = null;
            resources.ApplyResources(this.okButton, "okButton");
            this.okButton.BackgroundImage = null;
            this.okButton.DialogResult = System.Windows.Forms.DialogResult.OK;
            this.okButton.Font = null;
            this.okButton.Name = "okButton";
            // 
            // label1
            // 
            this.label1.AccessibleDescription = null;
            this.label1.AccessibleName = null;
            resources.ApplyResources(this.label1, "label1");
            this.label1.Font = null;
            this.label1.Name = "label1";
            // 
            // browseButton
            // 
            this.browseButton.AccessibleDescription = null;
            this.browseButton.AccessibleName = null;
            resources.ApplyResources(this.browseButton, "browseButton");
            this.browseButton.BackgroundImage = null;
            this.browseButton.Font = null;
            this.browseButton.Name = "browseButton";
            this.browseButton.Click += new System.EventHandler(this.browseButton_Click);
            // 
            // selectButton
            // 
            this.selectButton.AccessibleDescription = null;
            this.selectButton.AccessibleName = null;
            resources.ApplyResources(this.selectButton, "selectButton");
            this.selectButton.BackgroundImage = null;
            this.selectButton.Font = null;
            this.selectButton.Name = "selectButton";
            this.selectButton.Click += new System.EventHandler(this.selectButton_Click);
            // 
            // deleteButton
            // 
            this.deleteButton.AccessibleDescription = null;
            this.deleteButton.AccessibleName = null;
            resources.ApplyResources(this.deleteButton, "deleteButton");
            this.deleteButton.BackgroundImage = null;
            this.deleteButton.Font = null;
            this.deleteButton.Name = "deleteButton";
            this.deleteButton.Click += new System.EventHandler(this.deleteButton_Click);
            // 
            // selectedConverterListView
            // 
            this.selectedConverterListView.AccessibleDescription = null;
            this.selectedConverterListView.AccessibleName = null;
            resources.ApplyResources(this.selectedConverterListView, "selectedConverterListView");
            this.selectedConverterListView.BackgroundImage = null;
            this.selectedConverterListView.Columns.AddRange(new System.Windows.Forms.ColumnHeader[] {
            this.columnHeader1,
            this.columnHeader2,
            this.columnHeader3});
            this.selectedConverterListView.Font = null;
            this.selectedConverterListView.FullRowSelect = true;
            this.selectedConverterListView.Name = "selectedConverterListView";
            this.selectedConverterListView.UseCompatibleStateImageBehavior = false;
            this.selectedConverterListView.View = System.Windows.Forms.View.Details;
            // 
            // columnHeader1
            // 
            resources.ApplyResources(this.columnHeader1, "columnHeader1");
            // 
            // columnHeader2
            // 
            resources.ApplyResources(this.columnHeader2, "columnHeader2");
            // 
            // columnHeader3
            // 
            resources.ApplyResources(this.columnHeader3, "columnHeader3");
            // 
            // programTabPage
            // 
            this.programTabPage.AccessibleDescription = null;
            this.programTabPage.AccessibleName = null;
            resources.ApplyResources(this.programTabPage, "programTabPage");
            this.programTabPage.BackgroundImage = null;
            this.programTabPage.Controls.Add(this.label5);
            this.programTabPage.Controls.Add(this.findAssemblyButton);
            this.programTabPage.Controls.Add(this.classTextBox);
            this.programTabPage.Controls.Add(this.label4);
            this.programTabPage.Controls.Add(this.aseemblyTextBox);
            this.programTabPage.Controls.Add(this.label3);
            this.programTabPage.Controls.Add(this.converterNameTextBox);
            this.programTabPage.Controls.Add(this.label2);
            this.programTabPage.Font = null;
            this.programTabPage.Name = "programTabPage";
            this.programTabPage.UseVisualStyleBackColor = true;
            // 
            // label5
            // 
            this.label5.AccessibleDescription = null;
            this.label5.AccessibleName = null;
            resources.ApplyResources(this.label5, "label5");
            this.label5.Font = null;
            this.label5.Name = "label5";
            // 
            // findAssemblyButton
            // 
            this.findAssemblyButton.AccessibleDescription = null;
            this.findAssemblyButton.AccessibleName = null;
            resources.ApplyResources(this.findAssemblyButton, "findAssemblyButton");
            this.findAssemblyButton.BackgroundImage = null;
            this.findAssemblyButton.Font = null;
            this.findAssemblyButton.Name = "findAssemblyButton";
            this.findAssemblyButton.Click += new System.EventHandler(this.findAssemblyButton_Click);
            // 
            // classTextBox
            // 
            this.classTextBox.AccessibleDescription = null;
            this.classTextBox.AccessibleName = null;
            resources.ApplyResources(this.classTextBox, "classTextBox");
            this.classTextBox.BackgroundImage = null;
            this.classTextBox.Font = null;
            this.classTextBox.Name = "classTextBox";
            // 
            // label4
            // 
            this.label4.AccessibleDescription = null;
            this.label4.AccessibleName = null;
            resources.ApplyResources(this.label4, "label4");
            this.label4.Font = null;
            this.label4.Name = "label4";
            // 
            // aseemblyTextBox
            // 
            this.aseemblyTextBox.AccessibleDescription = null;
            this.aseemblyTextBox.AccessibleName = null;
            resources.ApplyResources(this.aseemblyTextBox, "aseemblyTextBox");
            this.aseemblyTextBox.BackgroundImage = null;
            this.aseemblyTextBox.Font = null;
            this.aseemblyTextBox.Name = "aseemblyTextBox";
            this.aseemblyTextBox.ReadOnly = true;
            // 
            // label3
            // 
            this.label3.AccessibleDescription = null;
            this.label3.AccessibleName = null;
            resources.ApplyResources(this.label3, "label3");
            this.label3.Font = null;
            this.label3.Name = "label3";
            // 
            // converterNameTextBox
            // 
            this.converterNameTextBox.AccessibleDescription = null;
            this.converterNameTextBox.AccessibleName = null;
            resources.ApplyResources(this.converterNameTextBox, "converterNameTextBox");
            this.converterNameTextBox.BackgroundImage = null;
            this.converterNameTextBox.Font = null;
            this.converterNameTextBox.Name = "converterNameTextBox";
            this.converterNameTextBox.TextChanged += new System.EventHandler(this.converterNameTextBox_TextChanged);
            // 
            // label2
            // 
            this.label2.AccessibleDescription = null;
            this.label2.AccessibleName = null;
            resources.ApplyResources(this.label2, "label2");
            this.label2.Font = null;
            this.label2.Name = "label2";
            // 
            // projectTabPage
            // 
            this.projectTabPage.AccessibleDescription = null;
            this.projectTabPage.AccessibleName = null;
            resources.ApplyResources(this.projectTabPage, "projectTabPage");
            this.projectTabPage.BackgroundImage = null;
            this.projectTabPage.Controls.Add(this.projectParserListBox);
            this.projectTabPage.Font = null;
            this.projectTabPage.Name = "projectTabPage";
            this.projectTabPage.UseVisualStyleBackColor = true;
            // 
            // projectParserListBox
            // 
            this.projectParserListBox.AccessibleDescription = null;
            this.projectParserListBox.AccessibleName = null;
            resources.ApplyResources(this.projectParserListBox, "projectParserListBox");
            this.projectParserListBox.BackgroundImage = null;
            this.projectParserListBox.Font = null;
            this.projectParserListBox.Name = "projectParserListBox";
            this.projectParserListBox.SelectedIndexChanged += new System.EventHandler(this.projectParserListBox_SelectedIndexChanged);
            // 
            // topTabControl
            // 
            this.topTabControl.AccessibleDescription = null;
            this.topTabControl.AccessibleName = null;
            resources.ApplyResources(this.topTabControl, "topTabControl");
            this.topTabControl.BackgroundImage = null;
            this.topTabControl.Controls.Add(this.projectTabPage);
            this.topTabControl.Controls.Add(this.programTabPage);
            this.topTabControl.Font = null;
            this.topTabControl.Name = "topTabControl";
            this.topTabControl.SelectedIndex = 0;
            this.topTabControl.SelectedIndexChanged += new System.EventHandler(this.topTabControl_SelectedIndexChanged);
            // 
            // AddConverterDialog
            // 
            this.AccessibleDescription = null;
            this.AccessibleName = null;
            resources.ApplyResources(this, "$this");
            this.BackgroundImage = null;
            this.Controls.Add(this.selectedConverterListView);
            this.Controls.Add(this.deleteButton);
            this.Controls.Add(this.selectButton);
            this.Controls.Add(this.browseButton);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.okButton);
            this.Controls.Add(this.cancelButton);
            this.Controls.Add(this.topTabControl);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.SizableToolWindow;
            this.Icon = null;
            this.Name = "AddConverterDialog";
            this.Load += new System.EventHandler(this.AddConverterDialog_Load);
            this.programTabPage.ResumeLayout(false);
            this.programTabPage.PerformLayout();
            this.projectTabPage.ResumeLayout(false);
            this.topTabControl.ResumeLayout(false);
            this.ResumeLayout(false);

		}
		#endregion

		#region controllers

		private void OpenProjectFile(string fileName)
		{
			try
			{
				ProjectElement project = new ProjectElement();

				project.Read(fileName);

				this.projectParserListBox.DataSource = project.Parsers;
				this.projectParserListBox.DisplayMember = "Caption";
				this.projectParserListBox.ValueMember = "Name";

				this._selectedProject = project;
			}
			catch (Exception e)
			{
				MessageBox.Show(e.Message, "Error Dialog",
					MessageBoxButtons.OK,
					MessageBoxIcon.Error);
			}
		}

		private void OpenDLLFile(string fileName)
		{
			FileInfo fileInfo = new FileInfo(fileName);

			// Because Assembly.Load() requires assembly names and doesn't work
			// with filenames, we have to trim off the extension
			string name =
				fileInfo.Name.Replace(fileInfo.Extension, "");

			this.aseemblyTextBox.Text = name;
			this._dllDir = fileInfo.DirectoryName + @"\";
		}

		private void SelectProjectConverter()
		{
			if (this.projectParserListBox.SelectedIndex >= 0)
			{
				string parserName = (string) this.projectParserListBox.SelectedValue;
				
				ParserElement parser = (ParserElement) _selectedProject.Parsers[parserName];

				// make sure the dll and xml files exist for the parser
				if (!File.Exists(parser.DirectoryPath + @"\" + parserName + ".dll"))
				{
					MessageBox.Show(MessageResourceManager.GetString("Import.MissingDLL"),
						"Error Dialog",
						MessageBoxButtons.OK,
						MessageBoxIcon.Error);

					return;
				}
				else if (!File.Exists(parser.DirectoryPath + @"\" + parserName + ".xml"))
				{
					MessageBox.Show(MessageResourceManager.GetString("Import.MissingXML"),
						"Error Dialog",
						MessageBoxButtons.OK,
						MessageBoxIcon.Error);

					return;
				}

				// add the conveter as selected converter
				// Create three items and three sets of subitems for each item.
				ConverterInfo info = new ConverterInfo();
				info.ConverterName = parser.Caption;
				info.AssemlyName = parserName;
				info.ClassName = parser.ConverterClass;
				info.AssemblyDir = parser.DirectoryPath;
				if (!IsConverterExist(info))
				{
					this._selectedConverters.Add(info);

					ListViewItem item = new ListViewItem(info.ConverterName);
					item.SubItems.Add(info.AssemlyName);
					item.SubItems.Add(info.ClassName);
					this.selectedConverterListView.Items.Add(item);
				}
				else
				{
					MessageBox.Show(MessageResourceManager.GetString("Import.DuplicateConverter"),
						"Error Dialog",
						MessageBoxButtons.OK,
						MessageBoxIcon.Error);
				}
			}
		}

		private void SelectProgramConverter()
		{
			if (aseemblyTextBox.Text == null ||
				aseemblyTextBox.Text.Length == 0)
			{
				MessageBox.Show(ImportExportResourceManager.GetString("Import.MissingAssemblyName"),
					"Error Dialog",
					MessageBoxButtons.OK,
					MessageBoxIcon.Error);

				return;
			}

			if (classTextBox.Text == null ||
				classTextBox.Text.Length == 0)
			{
				MessageBox.Show(ImportExportResourceManager.GetString("Import.MissingClassName"),
					"Error Dialog",
					MessageBoxButtons.OK,
					MessageBoxIcon.Error);

				return;
			}

			ConverterInfo info = new ConverterInfo();
			info.ConverterName = this.converterNameTextBox.Text.Trim();
			info.AssemlyName = this.aseemblyTextBox.Text.Trim();
			info.ClassName = this.classTextBox.Text.Trim();
			info.AssemblyDir = this._dllDir;

			if (!IsConverterExist(info))
			{
				this._selectedConverters.Add(info);

				ListViewItem item = new ListViewItem(info.ConverterName);
				item.SubItems.Add(info.AssemlyName);
				item.SubItems.Add(info.ClassName);
				this.selectedConverterListView.Items.Add(item);
			}
			else
			{
				MessageBox.Show(MessageResourceManager.GetString("Import.DuplicateConverter"),
					"Error Dialog",
					MessageBoxButtons.OK,
					MessageBoxIcon.Error);
			}
		}

		/// <summary>
		/// Check if the given converter is duplicated
		/// </summary>
		/// <param name="info">The converter info</param>
		/// <returns>true if already selected, false otherwise</returns>
		private bool IsConverterExist(ConverterInfo info)
		{
			bool status = false;

			foreach (ConverterInfo item in this._selectedConverters)
			{
				if (item.AssemlyName == info.AssemlyName)
				{
					status = true;
					break;
				}
			}

			return status;
		}

		#endregion

		private void browseButton_Click(object sender, System.EventArgs e)
		{
			string fileName = null;
			OpenFileDialog openFileDialog = new OpenFileDialog();

			openFileDialog.InitialDirectory = "c:\\" ;
			// open project file
			openFileDialog.Filter = "Project files (*.GDP)|*.GDP";
			
			openFileDialog.FilterIndex = 1 ;
			openFileDialog.RestoreDirectory = false ;

			if (openFileDialog.ShowDialog() == DialogResult.OK)
			{
				fileName = openFileDialog.FileName;

				// open project file
				OpenProjectFile(fileName);
			}
		}

		private void selectButton_Click(object sender, System.EventArgs e)
		{
			if (this._isProjectTab)
			{
				SelectProjectConverter();
			}
			else
			{
				SelectProgramConverter();
			}
		}

		private void deleteButton_Click(object sender, System.EventArgs e)
		{
			if (this.selectedConverterListView.SelectedIndices.Count == 1)
			{
				int index = this.selectedConverterListView.SelectedIndices[0];
				this.selectedConverterListView.Items.RemoveAt(index);
				this._selectedConverters.RemoveAt(index);
			}
		}

		private void topTabControl_SelectedIndexChanged(object sender, System.EventArgs e)
		{
			if (this.topTabControl.SelectedIndex == 0)
			{
				this.browseButton.Enabled = true;

				this._isProjectTab = true;
				if (this.projectParserListBox.SelectedIndex >= 0)
				{
					this.selectButton.Enabled = true;
				}
				else
				{
					this.selectButton.Enabled = false;
				}
			}
			else
			{
				this.browseButton.Enabled = false;

				this._isProjectTab = false;
				if (converterNameTextBox.Text == null ||
					converterNameTextBox.Text.Length == 0)
				{
					this.selectButton.Enabled = false;
				}
				else
				{
					this.selectButton.Enabled = true;
				}
			}
		}

		private void projectParserListBox_SelectedIndexChanged(object sender, System.EventArgs e)
		{
			if (projectParserListBox.SelectedIndex >= 0)
			{
				this.selectButton.Enabled = true;
			}
		}

		private void converterNameTextBox_TextChanged(object sender, System.EventArgs e)
		{
			if (converterNameTextBox.Text != null &&
				converterNameTextBox.Text.Length > 0)
			{
				this.selectButton.Enabled = true;
			}
			else
			{
				this.selectButton.Enabled = false;
			}
		}

		private void findAssemblyButton_Click(object sender, System.EventArgs e)
		{
			OpenFileDialog openFileDialog = new OpenFileDialog();
			openFileDialog.InitialDirectory = "c:\\" ;
			openFileDialog.Filter = "DLL files (*.dll)|*.dll";
			openFileDialog.FilterIndex = 1 ;
			openFileDialog.RestoreDirectory = false ;

			if (openFileDialog.ShowDialog() == DialogResult.OK)
			{
				string fileName = openFileDialog.FileName;

				// open dll file
				OpenDLLFile(fileName);
			}
		}

        private void AddConverterDialog_Load(object sender, EventArgs e)
        {
            // When the data source type is Excel, disable the project tab page
            // and make the program tab page as default
            if (_dataSourceType == DataSourceType.Excel ||
                _dataSourceType == DataSourceType.Other)
            {
                this.topTabControl.SelectedIndex = 1;
            }
        }

	}
}
