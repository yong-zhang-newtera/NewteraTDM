using System;
using System.IO;
using System.Drawing;
using System.Text;
using System.Threading;
using System.Collections;
using System.ComponentModel;
using System.Windows.Forms;
using System.CodeDom;
using System.CodeDom.Compiler;

using Newtera.Common.Core;
using Newtera.Common.MetaData.Mappings;
using Newtera.Common.MetaData.Mappings.Generator;
using Newtera.WinClientCommon;
using Newtera.WindowsControl;

namespace Newtera.Studio.ImportExport
{
	/// <summary>
	/// Summary description for MappingScriptDialog.
	/// </summary>
	public class MappingScriptDialog : System.Windows.Forms.Form
	{
		private ITransformable _mapping;
		private ScriptLanguage _language = ScriptLanguage.CSharp;
		private CodeCompileUnit _codeStub;
		private MetaDataServiceStub _metaDataService;
		private Newtera.Common.Core.SchemaInfo _schemaInfo;
		private string _classType;
		private bool _isCompileSucceeded;
		private bool _scriptEnabled;

		private System.Windows.Forms.Button OKButton;
		private System.Windows.Forms.Button compileButton;
		private System.Windows.Forms.Label label1;
		private System.Windows.Forms.ComboBox languageComboBox;
		private System.Windows.Forms.TabControl tabControl1;
		private System.Windows.Forms.TabPage scriptTabPage;
		private System.Windows.Forms.TabPage resultTabPage;
		private System.Windows.Forms.TextBox scriptTextBox;
		private System.Windows.Forms.TextBox resultTextBox;
		private System.Windows.Forms.Button cancelButton;
		private System.Windows.Forms.RadioButton directRadioButton;
		private System.Windows.Forms.RadioButton indirectRadioButton;

		/// <summary>
		/// Required designer variable.
		/// </summary>
		private System.ComponentModel.Container components = null;

		public MappingScriptDialog()
		{
			//
			// Required for Windows Form Designer support
			//
			InitializeComponent();

			languageComboBox.SelectedIndex = 0;
			_codeStub = null;
			_metaDataService = new MetaDataServiceStub();
			_classType = null;
			_isCompileSucceeded = false;
			_scriptEnabled = false;
		}

		/// <summary>
		/// Get or sets the mapping of ITransformable
		/// </summary>
		public ITransformable Mapping
		{
			get
			{
				return _mapping;
			}
			set
			{
				_mapping = value;
			}
		}

		/// <summary>
		/// Get or sets the schema info
		/// </summary>
		public Newtera.Common.Core.SchemaInfo SchemaInfo
		{
			get
			{
				return _schemaInfo;
			}
			set
			{
				_schemaInfo = value;
			}
		}

		/// <summary>
		/// Gets or sets the information indicating whether the script is enabled
		/// for transformation.
		/// </summary>
		/// <value>true if it is enabled, false otherwise.</value>
		public bool ScriptEnabled
		{
			get
			{
				return _scriptEnabled;
			}
			set
			{
				_scriptEnabled = value;
				if (value)
				{
					this.directRadioButton.Checked = false;
					this.indirectRadioButton.Checked = true;
				}
				else
				{
					this.directRadioButton.Checked = true;
					this.indirectRadioButton.Checked = false;
				}
			}
		}

		/// <summary>
		/// Gets or sets the selected script language
		/// </summary>
		/// <value>One of ScriptLanguage enum types</value>
		public ScriptLanguage ScriptLanguage
		{
			get
			{
				return _language;
			}
			set
			{
				_language = value;
			}
		}

		/// <summary>
		/// Gets or sets the class type of script.
		/// </summary>
		/// <value>class type</value>
		public string ClassType
		{
			get
			{
				return _classType;
			}
			set
			{
				_classType = value;
			}
		}

		/// <summary>
		/// Gets or sets the transform script.
		/// </summary>
		/// <value>script code, null if empty</value>
		public string Script
		{
			get
			{
				if (scriptTextBox.Text != null && scriptTextBox.Text.Length > 0)
				{
					return scriptTextBox.Text;
				}
				else
				{
					return null;
				}
			}
			set
			{
				scriptTextBox.Text = value;
				_isCompileSucceeded = true;
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(MappingScriptDialog));
            this.cancelButton = new System.Windows.Forms.Button();
            this.OKButton = new System.Windows.Forms.Button();
            this.compileButton = new System.Windows.Forms.Button();
            this.label1 = new System.Windows.Forms.Label();
            this.languageComboBox = new System.Windows.Forms.ComboBox();
            this.tabControl1 = new System.Windows.Forms.TabControl();
            this.scriptTabPage = new System.Windows.Forms.TabPage();
            this.scriptTextBox = new System.Windows.Forms.TextBox();
            this.resultTabPage = new System.Windows.Forms.TabPage();
            this.resultTextBox = new System.Windows.Forms.TextBox();
            this.directRadioButton = new System.Windows.Forms.RadioButton();
            this.indirectRadioButton = new System.Windows.Forms.RadioButton();
            this.tabControl1.SuspendLayout();
            this.scriptTabPage.SuspendLayout();
            this.resultTabPage.SuspendLayout();
            this.SuspendLayout();
            // 
            // cancelButton
            // 
            resources.ApplyResources(this.cancelButton, "cancelButton");
            this.cancelButton.DialogResult = System.Windows.Forms.DialogResult.Cancel;
            this.cancelButton.Name = "cancelButton";
            // 
            // OKButton
            // 
            resources.ApplyResources(this.OKButton, "OKButton");
            this.OKButton.DialogResult = System.Windows.Forms.DialogResult.OK;
            this.OKButton.Name = "OKButton";
            this.OKButton.Click += new System.EventHandler(this.OKButton_Click);
            // 
            // compileButton
            // 
            resources.ApplyResources(this.compileButton, "compileButton");
            this.compileButton.Name = "compileButton";
            this.compileButton.Click += new System.EventHandler(this.compileButton_Click);
            // 
            // label1
            // 
            resources.ApplyResources(this.label1, "label1");
            this.label1.Name = "label1";
            // 
            // languageComboBox
            // 
            resources.ApplyResources(this.languageComboBox, "languageComboBox");
            this.languageComboBox.Items.AddRange(new object[] {
            resources.GetString("languageComboBox.Items")});
            this.languageComboBox.Name = "languageComboBox";
            this.languageComboBox.SelectedIndexChanged += new System.EventHandler(this.languageComboBox_SelectedIndexChanged);
            // 
            // tabControl1
            // 
            resources.ApplyResources(this.tabControl1, "tabControl1");
            this.tabControl1.Controls.Add(this.scriptTabPage);
            this.tabControl1.Controls.Add(this.resultTabPage);
            this.tabControl1.Name = "tabControl1";
            this.tabControl1.SelectedIndex = 0;
            // 
            // scriptTabPage
            // 
            this.scriptTabPage.Controls.Add(this.scriptTextBox);
            resources.ApplyResources(this.scriptTabPage, "scriptTabPage");
            this.scriptTabPage.Name = "scriptTabPage";
            // 
            // scriptTextBox
            // 
            resources.ApplyResources(this.scriptTextBox, "scriptTextBox");
            this.scriptTextBox.Name = "scriptTextBox";
            this.scriptTextBox.TextChanged += new System.EventHandler(this.scriptTextBox_TextChanged);
            // 
            // resultTabPage
            // 
            this.resultTabPage.Controls.Add(this.resultTextBox);
            resources.ApplyResources(this.resultTabPage, "resultTabPage");
            this.resultTabPage.Name = "resultTabPage";
            // 
            // resultTextBox
            // 
            resources.ApplyResources(this.resultTextBox, "resultTextBox");
            this.resultTextBox.Name = "resultTextBox";
            // 
            // directRadioButton
            // 
            this.directRadioButton.Checked = true;
            resources.ApplyResources(this.directRadioButton, "directRadioButton");
            this.directRadioButton.Name = "directRadioButton";
            this.directRadioButton.TabStop = true;
            this.directRadioButton.CheckedChanged += new System.EventHandler(this.directRadioButton_CheckedChanged);
            // 
            // indirectRadioButton
            // 
            resources.ApplyResources(this.indirectRadioButton, "indirectRadioButton");
            this.indirectRadioButton.Name = "indirectRadioButton";
            this.indirectRadioButton.Click += new System.EventHandler(this.indirectRadioButton_Click);
            // 
            // MappingScriptDialog
            // 
            resources.ApplyResources(this, "$this");
            this.Controls.Add(this.indirectRadioButton);
            this.Controls.Add(this.directRadioButton);
            this.Controls.Add(this.tabControl1);
            this.Controls.Add(this.languageComboBox);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.compileButton);
            this.Controls.Add(this.OKButton);
            this.Controls.Add(this.cancelButton);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.SizableToolWindow;
            this.Name = "MappingScriptDialog";
            this.tabControl1.ResumeLayout(false);
            this.scriptTabPage.ResumeLayout(false);
            this.scriptTabPage.PerformLayout();
            this.resultTabPage.ResumeLayout(false);
            this.resultTabPage.PerformLayout();
            this.ResumeLayout(false);

		}

		#endregion

		#region Controller code

		/// <summary>
		/// Generate code stub for the transformer
		/// </summary>
		private void GenerateCodeStub()
		{
			_classType = GetClassName();

			this._codeStub = CodeStubGenerator.Instance.Generate(_classType, ((IMappingNode) _mapping).NodeType);

			CodeDomProvider gen = CodeStubGenerator.Instance.GetProvider(_language);

			StringWriter stringWriter = new StringWriter();

			// Create a TextWriter to a StreamWriter to an output file.
			IndentedTextWriter tw = new IndentedTextWriter(stringWriter, "    ");
			
			// Generate source code using the code generator.
			gen.GenerateCodeFromCompileUnit(_codeStub, tw, new CodeGeneratorOptions());
			
			StringBuilder builder = stringWriter.GetStringBuilder();

			this.scriptTextBox.Text = builder.ToString();
			
			// Close the output .
			tw.Close();
		}

		/// <summary>
		/// Get an unique class name for the transformer
		/// </summary>
		/// <returns></returns>
		private string GetClassName()
		{
			// invoke the web service synchronously
			string id = _metaDataService.GetTransformerId(ConnectionStringBuilder.Instance.Create(_schemaInfo));

			return "Transformer_" + id;
		}

        /// <summary>
        /// Get the lib dir
        /// </summary>
        /// <returns></returns>
        private string GetToolBinDir()
        {
            string toolBinDir = NewteraNameSpace.GetAppToolDir();

            if (!toolBinDir.EndsWith(@"\"))
            {
                toolBinDir += @"\bin\";
            }
            else
            {
                toolBinDir += @"bin\";
            }

            if (!File.Exists(toolBinDir + "Newtera.Common.dll"))
            {
                // development enviroment
                toolBinDir += @"Debug\";
            }

            return toolBinDir;
        }

		#endregion

		private void compileButton_Click(object sender, System.EventArgs e)
		{
			if (this.scriptTextBox.Text != null)
			{
				CompilerResults cr = CodeStubGenerator.Instance.CompileFromSource(this._language,
					this.scriptTextBox.Text, GetToolBinDir());

				if(cr.Errors.Count > 0)
				{
					this.resultTextBox.Text = MessageResourceManager.GetString("ImportExport.CompileErrors") + "\n";
					foreach(CompilerError ce in cr.Errors)                
						this.resultTextBox.AppendText(ce.ToString()+"\n"); 
                    
					this._isCompileSucceeded = false;
				}
				else
				{
					this.resultTextBox.Text = MessageResourceManager.GetString("ImportExport.CompileSuccess");
					this._isCompileSucceeded = true;
				}

				this.tabControl1.SelectedTab = this.resultTabPage;
			}
		}

		private void languageComboBox_SelectedIndexChanged(object sender, System.EventArgs e)
		{
			switch (this.languageComboBox.SelectedIndex)
			{
				case 0:
					_language = ScriptLanguage.CSharp;
					break;
				case 1:
					_language = ScriptLanguage.VBScript;
					break;
				case 2:
					_language = ScriptLanguage.JScript;
					break;
			}
		}

		private void OKButton_Click(object sender, System.EventArgs e)
		{
			if (!this._isCompileSucceeded && ScriptEnabled)
			{
				MessageBox.Show(MessageResourceManager.GetString("ImportExport.CompileCode"));

				this.DialogResult = DialogResult.None; // dimiss the OK event
			}
		}

		private void indirectRadioButton_Click(object sender, System.EventArgs e)
		{
			if (this.languageComboBox.SelectedIndex >= 0 &&
				this.Script == null)
			{
				GenerateCodeStub();
			}
		}

		private void scriptTextBox_TextChanged(object sender, System.EventArgs e)
		{
			this._isCompileSucceeded = false;
		}

		private void directRadioButton_CheckedChanged(object sender, System.EventArgs e)
		{
			if (this.directRadioButton.Checked)
			{
				this._scriptEnabled = false;
			}
			else
			{
				this._scriptEnabled = true;
			}
		}
	}
}
