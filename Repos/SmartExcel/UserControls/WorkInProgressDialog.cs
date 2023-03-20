using System;
using System.Drawing;
using System.Collections;
using System.ComponentModel;
using System.Windows.Forms;

namespace SmartExcel
{
	/// <summary>
	/// Summary description for WorkInProgressDialog.
	/// </summary>
	public class WorkInProgressDialog : System.Windows.Forms.Form
	{
		private bool _enableCancel = false;
		private MethodInvoker _cancelCallback = null;
		private string _defaultMessage = "Work in Progress...";
		private bool _isTimerEnabled;

		private System.Windows.Forms.ProgressBar progressBar1;
		private System.Windows.Forms.Label label1;
		private System.Windows.Forms.Button cancelButton;
		private System.Windows.Forms.Timer timer1;
		private System.ComponentModel.IContainer components;

		public WorkInProgressDialog()
		{
			//
			// Required for Windows Form Designer support
			//
			InitializeComponent();

			//
			// TODO: Add any constructor code after InitializeComponent call
			//
			_defaultMessage = this.label1.Text;
			_isTimerEnabled = true;
			this.progressBar1.Minimum = 1;
			this.progressBar1.Step = 1;
		}

		/// <summary>
		/// Gets or sets the information indicating whether the timer is enabled.
		/// </summary>
		/// <remarks>When timer is enabled, it moves steps by itself.</remarks>
		public bool IsTimerEnabled
		{
			get
			{
				return _isTimerEnabled;
			}
			set
			{
				_isTimerEnabled = value;
			}
		}

		/// <summary>
		/// Gets or sets the maximum steps of progress bar
		/// </summary>
		public int MaximumSteps
		{
			get
			{
				return this.progressBar1.Maximum;
			}
			set
			{
				this.progressBar1.Maximum = value;
			}
		}

		/// <summary>
		/// Gets or sets the current value of progress bar
		/// </summary>
		public int Value
		{
			get
			{
				return this.progressBar1.Value;
			}
			set
			{
				this.progressBar1.Value = value;
			}
		}

		/// <summary>
		/// Perform a step in progress
		/// </summary>
		public void PerformStep()
		{
			// recycle the bar if it reaches maximum value
			if (this.progressBar1.Value < this.progressBar1.Maximum)
			{
				this.progressBar1.PerformStep();
			}
		}

		/// <summary>
		/// The text to display in the dialog box
		/// </summary>
		public string DisplayText
		{
			get
			{
				return this.label1.Text;
			}
			set
			{
				if (value == null)
				{
					this.label1.Text = this._defaultMessage;
				}
				else
				{
					this.label1.Text = value;
				}
			}
		}

		/// <summary>
		/// Gets or sets the information indicating whether to enable cncelling a
		/// request.
		/// </summary>
		public bool EnableCancel
		{
			get
			{
				return _enableCancel;
			}
			set
			{
				_enableCancel = value;
				if (value)
				{
					this.cancelButton.Enabled = value;
				}
			}
		}

		/// <summary>
		/// Gets or sets the cancel callback
		/// </summary>
		public MethodInvoker CancelCallback
		{
			get
			{
				return _cancelCallback;
			}
			set
			{
				_cancelCallback = value;
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(WorkInProgressDialog));
            this.progressBar1 = new System.Windows.Forms.ProgressBar();
            this.label1 = new System.Windows.Forms.Label();
            this.cancelButton = new System.Windows.Forms.Button();
            this.timer1 = new System.Windows.Forms.Timer(this.components);
            this.SuspendLayout();
            // 
            // progressBar1
            // 
            resources.ApplyResources(this.progressBar1, "progressBar1");
            this.progressBar1.Name = "progressBar1";
            // 
            // label1
            // 
            resources.ApplyResources(this.label1, "label1");
            this.label1.Name = "label1";
            // 
            // cancelButton
            // 
            resources.ApplyResources(this.cancelButton, "cancelButton");
            this.cancelButton.DialogResult = System.Windows.Forms.DialogResult.Cancel;
            this.cancelButton.Name = "cancelButton";
            this.cancelButton.Click += new System.EventHandler(this.cancelButton_Click);
            // 
            // timer1
            // 
            this.timer1.Enabled = true;
            this.timer1.Interval = 200;
            this.timer1.Tick += new System.EventHandler(this.timer1_Tick);
            // 
            // WorkInProgressDialog
            // 
            resources.ApplyResources(this, "$this");
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Inherit;
            this.Controls.Add(this.cancelButton);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.progressBar1);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedToolWindow;
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "WorkInProgressDialog";
            this.ResumeLayout(false);

		}
		#endregion

		private void timer1_Tick(object sender, System.EventArgs e)
		{
			if (_isTimerEnabled)
			{
				// recycle the bar if it reaches maximum value
				if (this.progressBar1.Value >= this.progressBar1.Maximum)
				{
					this.progressBar1.Value = this.progressBar1.Minimum;
				}

				this.progressBar1.PerformStep();
			}
		}

		private void cancelButton_Click(object sender, System.EventArgs e)
		{
			if (_cancelCallback != null)
			{
				_cancelCallback.DynamicInvoke(null);
			}
		}
	}
}
