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
using Newtera.Common.MetaData.Schema;
using Newtera.Common.MetaData.Schema.Generator;
using Newtera.WindowsControl;

namespace Newtera.Studio
{
	/// <summary>
	/// Summary description for WriteVirtualAttributeCodeDialog.
	/// </summary>
	public class WriteVirtualAttributeCodeDialog : System.Windows.Forms.Form
	{
        private string _code;
		private ScriptLanguage _language = ScriptLanguage.CSharp;
		private CodeCompileUnit _codeStub;
		private SchemaInfo _schemaInfo;
		private bool _isCompileSucceeded;
        private VirtualAttributeElement _attribute;

		private System.Windows.Forms.Button OKButton;
		private System.Windows.Forms.Button compileButton;
		private System.Windows.Forms.Label label1;
		private System.Windows.Forms.ComboBox languageComboBox;
		private System.Windows.Forms.TabControl tabControl1;
		private System.Windows.Forms.TabPage codeTabPage;
        private System.Windows.Forms.TabPage resultTabPage;
		private System.Windows.Forms.TextBox resultTextBox;
        private System.Windows.Forms.Button cancelButton;
        private Button resetButton;
        private RichTextBox codeTextBox;

		/// <summary>
		/// Required designer variable.
		/// </summary>
		private System.ComponentModel.Container components = null;

        public WriteVirtualAttributeCodeDialog()
		{
			//
			// Required for Windows Form Designer support
			//
			InitializeComponent();

			languageComboBox.SelectedIndex = 0;
			_codeStub = null;
			_isCompileSucceeded = false;
		}

        /// <summary>
        /// Gets or sets the schema info
        /// </summary>
        public SchemaInfo SchemaInfo
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
        /// Gets or sets the virtual attribute
        /// </summary>
        public VirtualAttributeElement Attribute
        {
            get
            {
                return _attribute;
            }
            set
            {
                _attribute = value;
                _code = value.Code;
            }
        }

		/// <summary>
		/// Get or sets the code
		/// </summary>
		public string Code
		{
			get
			{
				return _code;
			}
			set
			{
				_code = value;
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(WriteVirtualAttributeCodeDialog));
            this.cancelButton = new System.Windows.Forms.Button();
            this.OKButton = new System.Windows.Forms.Button();
            this.compileButton = new System.Windows.Forms.Button();
            this.label1 = new System.Windows.Forms.Label();
            this.languageComboBox = new System.Windows.Forms.ComboBox();
            this.tabControl1 = new System.Windows.Forms.TabControl();
            this.codeTabPage = new System.Windows.Forms.TabPage();
            this.resultTabPage = new System.Windows.Forms.TabPage();
            this.resultTextBox = new System.Windows.Forms.TextBox();
            this.resetButton = new System.Windows.Forms.Button();
            this.codeTextBox = new System.Windows.Forms.RichTextBox();
            this.tabControl1.SuspendLayout();
            this.codeTabPage.SuspendLayout();
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
            // 
            // tabControl1
            // 
            resources.ApplyResources(this.tabControl1, "tabControl1");
            this.tabControl1.Controls.Add(this.codeTabPage);
            this.tabControl1.Controls.Add(this.resultTabPage);
            this.tabControl1.Name = "tabControl1";
            this.tabControl1.SelectedIndex = 0;
            // 
            // codeTabPage
            // 
            this.codeTabPage.Controls.Add(this.codeTextBox);
            resources.ApplyResources(this.codeTabPage, "codeTabPage");
            this.codeTabPage.Name = "codeTabPage";
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
            // resetButton
            // 
            resources.ApplyResources(this.resetButton, "resetButton");
            this.resetButton.Name = "resetButton";
            this.resetButton.UseVisualStyleBackColor = true;
            this.resetButton.Click += new System.EventHandler(this.resetButton_Click);
            // 
            // codeTextBox
            // 
            resources.ApplyResources(this.codeTextBox, "codeTextBox");
            this.codeTextBox.Name = "codeTextBox";
            // 
            // WriteVirtualAttributeCodeDialog
            // 
            resources.ApplyResources(this, "$this");
            this.Controls.Add(this.resetButton);
            this.Controls.Add(this.tabControl1);
            this.Controls.Add(this.languageComboBox);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.compileButton);
            this.Controls.Add(this.OKButton);
            this.Controls.Add(this.cancelButton);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.SizableToolWindow;
            this.Name = "WriteVirtualAttributeCodeDialog";
            this.Load += new System.EventHandler(this.WriteVirtualAttributeCodeDialog_Load);
            this.tabControl1.ResumeLayout(false);
            this.codeTabPage.ResumeLayout(false);
            this.resultTabPage.ResumeLayout(false);
            this.resultTabPage.PerformLayout();
            this.ResumeLayout(false);

		}

		#endregion

		#region Controller code

		/// <summary>
		/// Generate code stub for the formula
		/// </summary>
		private void GenerateCodeStub()
		{
			_attribute.ClassType = GetClassName();

            this._codeStub = CodeStubGenerator.Instance.Generate(_attribute.ClassType);

			CodeDomProvider gen = CodeStubGenerator.Instance.GetProvider(_language);

			StringWriter stringWriter = new StringWriter();

			// Create a TextWriter to a StreamWriter to an output file.
			IndentedTextWriter tw = new IndentedTextWriter(stringWriter, "    ");
			
			// Generate source code using the code generator.
			gen.GenerateCodeFromCompileUnit(_codeStub, tw, new CodeGeneratorOptions());
			
			StringBuilder builder = stringWriter.GetStringBuilder();

            string code = builder.ToString();
            // get ride of comments
            int pos = code.IndexOf("namespace");
            if (pos > 0)
            {
                code = code.Substring(pos);
            }

			this.codeTextBox.Text = code;
			
			// Close the output .
			tw.Close();
		}

		/// <summary>
		/// Get an unique class name for the transformer
		/// </summary>
		/// <returns></returns>
		private string GetClassName()
		{
            int hashCode = Math.Abs(Guid.NewGuid().ToString().GetHashCode());
			return "F_" + hashCode;
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
			if (this.codeTextBox.Text != null)
			{
				CompilerResults cr = CodeStubGenerator.Instance.CompileFromSource(this._language,
                    this.codeTextBox.Text, GetToolBinDir());

				if(cr.Errors.Count > 0)
				{
					this.resultTextBox.Text = MessageResourceManager.GetString("SchemaEditor.CompileErrors") + "\n";
					foreach(CompilerError ce in cr.Errors)                
						this.resultTextBox.AppendText(ce.ToString()+"\n"); 
                    
					this._isCompileSucceeded = false;
				}
				else
				{
                    this.resultTextBox.Text = MessageResourceManager.GetString("SchemaEditor.CompileSuccess");
					this._isCompileSucceeded = true;
                    _code = this.codeTextBox.Text;
				}

				this.tabControl1.SelectedTab = this.resultTabPage;
			}
		}

		private void OKButton_Click(object sender, System.EventArgs e)
		{
			if (!this._isCompileSucceeded)
			{
				MessageBox.Show(MessageResourceManager.GetString("SchemaEditor.CompileFormulaCode"));

				this.DialogResult = DialogResult.None; // dimiss the OK event
			}
		}

		private void scriptTextBox_TextChanged(object sender, System.EventArgs e)
		{
			this._isCompileSucceeded = false;
		}

        private void WriteVirtualAttributeCodeDialog_Load(object sender, EventArgs e)
        {
            if (!string.IsNullOrEmpty(_attribute.Code))
            {
                this.codeTextBox.Text = _attribute.Code;
            }
            else
            {
                // generate the code template
                GenerateCodeStub();
            }
        }

        private void resetButton_Click(object sender, EventArgs e)
        {
            GenerateCodeStub();
        }
	}
}
