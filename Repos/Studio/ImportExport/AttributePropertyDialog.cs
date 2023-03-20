using System;
using System.Drawing;
using System.Collections;
using System.ComponentModel;
using System.Windows.Forms;

using Newtera.Common.MetaData.Schema;

namespace Newtera.Studio.ImportExport
{
	/// <summary>
	/// AttributePropertyDialog 的摘要说明。
	/// </summary>
	public class AttributePropertyDialog : System.Windows.Forms.Form
	{
		private AttributeElementBase _attributeElement;
		private System.Windows.Forms.Button okButton;
		private System.Windows.Forms.PropertyGrid propertyGrid;
		/// <summary>
		/// 必需的设计器变量。
		/// </summary>
		private System.ComponentModel.Container components = null;

		public AttributePropertyDialog()
		{
			//
			// Windows 窗体设计器支持所必需的
			//
			InitializeComponent();

			//
			// TODO: 在 InitializeComponent 调用后添加任何构造函数代码
			//
		}

		/// <summary>
		/// Gets or sets the attribute element on which to edit the properties
		/// </summary>
		public AttributeElementBase AttributeElement
		{
			get
			{
				return _attributeElement;
			}
			set
			{
				_attributeElement = value;
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
			System.Resources.ResourceManager resources = new System.Resources.ResourceManager(typeof(AttributePropertyDialog));
			this.okButton = new System.Windows.Forms.Button();
			this.propertyGrid = new System.Windows.Forms.PropertyGrid();
			this.SuspendLayout();
			// 
			// okButton
			// 
			this.okButton.AccessibleDescription = resources.GetString("okButton.AccessibleDescription");
			this.okButton.AccessibleName = resources.GetString("okButton.AccessibleName");
			this.okButton.Anchor = ((System.Windows.Forms.AnchorStyles)(resources.GetObject("okButton.Anchor")));
			this.okButton.BackgroundImage = ((System.Drawing.Image)(resources.GetObject("okButton.BackgroundImage")));
			this.okButton.DialogResult = System.Windows.Forms.DialogResult.OK;
			this.okButton.Dock = ((System.Windows.Forms.DockStyle)(resources.GetObject("okButton.Dock")));
			this.okButton.Enabled = ((bool)(resources.GetObject("okButton.Enabled")));
			this.okButton.FlatStyle = ((System.Windows.Forms.FlatStyle)(resources.GetObject("okButton.FlatStyle")));
			this.okButton.Font = ((System.Drawing.Font)(resources.GetObject("okButton.Font")));
			this.okButton.Image = ((System.Drawing.Image)(resources.GetObject("okButton.Image")));
			this.okButton.ImageAlign = ((System.Drawing.ContentAlignment)(resources.GetObject("okButton.ImageAlign")));
			this.okButton.ImageIndex = ((int)(resources.GetObject("okButton.ImageIndex")));
			this.okButton.ImeMode = ((System.Windows.Forms.ImeMode)(resources.GetObject("okButton.ImeMode")));
			this.okButton.Location = ((System.Drawing.Point)(resources.GetObject("okButton.Location")));
			this.okButton.Name = "okButton";
			this.okButton.RightToLeft = ((System.Windows.Forms.RightToLeft)(resources.GetObject("okButton.RightToLeft")));
			this.okButton.Size = ((System.Drawing.Size)(resources.GetObject("okButton.Size")));
			this.okButton.TabIndex = ((int)(resources.GetObject("okButton.TabIndex")));
			this.okButton.Text = resources.GetString("okButton.Text");
			this.okButton.TextAlign = ((System.Drawing.ContentAlignment)(resources.GetObject("okButton.TextAlign")));
			this.okButton.Visible = ((bool)(resources.GetObject("okButton.Visible")));
			// 
			// propertyGrid
			// 
			this.propertyGrid.AccessibleDescription = resources.GetString("propertyGrid.AccessibleDescription");
			this.propertyGrid.AccessibleName = resources.GetString("propertyGrid.AccessibleName");
			this.propertyGrid.Anchor = ((System.Windows.Forms.AnchorStyles)(resources.GetObject("propertyGrid.Anchor")));
			this.propertyGrid.BackgroundImage = ((System.Drawing.Image)(resources.GetObject("propertyGrid.BackgroundImage")));
			this.propertyGrid.CommandsVisibleIfAvailable = true;
			this.propertyGrid.Dock = ((System.Windows.Forms.DockStyle)(resources.GetObject("propertyGrid.Dock")));
			this.propertyGrid.Enabled = ((bool)(resources.GetObject("propertyGrid.Enabled")));
			this.propertyGrid.Font = ((System.Drawing.Font)(resources.GetObject("propertyGrid.Font")));
			this.propertyGrid.HelpVisible = ((bool)(resources.GetObject("propertyGrid.HelpVisible")));
			this.propertyGrid.ImeMode = ((System.Windows.Forms.ImeMode)(resources.GetObject("propertyGrid.ImeMode")));
			this.propertyGrid.LargeButtons = false;
			this.propertyGrid.LineColor = System.Drawing.SystemColors.ScrollBar;
			this.propertyGrid.Location = ((System.Drawing.Point)(resources.GetObject("propertyGrid.Location")));
			this.propertyGrid.Name = "propertyGrid";
			this.propertyGrid.RightToLeft = ((System.Windows.Forms.RightToLeft)(resources.GetObject("propertyGrid.RightToLeft")));
			this.propertyGrid.Size = ((System.Drawing.Size)(resources.GetObject("propertyGrid.Size")));
			this.propertyGrid.TabIndex = ((int)(resources.GetObject("propertyGrid.TabIndex")));
			this.propertyGrid.Text = resources.GetString("propertyGrid.Text");
			this.propertyGrid.ToolbarVisible = false;
			this.propertyGrid.ViewBackColor = System.Drawing.SystemColors.Window;
			this.propertyGrid.ViewForeColor = System.Drawing.SystemColors.WindowText;
			this.propertyGrid.Visible = ((bool)(resources.GetObject("propertyGrid.Visible")));
			// 
			// AttributePropertyDialog
			// 
			this.AccessibleDescription = resources.GetString("$this.AccessibleDescription");
			this.AccessibleName = resources.GetString("$this.AccessibleName");
			this.AutoScaleBaseSize = ((System.Drawing.Size)(resources.GetObject("$this.AutoScaleBaseSize")));
			this.AutoScroll = ((bool)(resources.GetObject("$this.AutoScroll")));
			this.AutoScrollMargin = ((System.Drawing.Size)(resources.GetObject("$this.AutoScrollMargin")));
			this.AutoScrollMinSize = ((System.Drawing.Size)(resources.GetObject("$this.AutoScrollMinSize")));
			this.BackgroundImage = ((System.Drawing.Image)(resources.GetObject("$this.BackgroundImage")));
			this.ClientSize = ((System.Drawing.Size)(resources.GetObject("$this.ClientSize")));
			this.Controls.Add(this.propertyGrid);
			this.Controls.Add(this.okButton);
			this.Enabled = ((bool)(resources.GetObject("$this.Enabled")));
			this.Font = ((System.Drawing.Font)(resources.GetObject("$this.Font")));
			this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.SizableToolWindow;
			this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
			this.ImeMode = ((System.Windows.Forms.ImeMode)(resources.GetObject("$this.ImeMode")));
			this.Location = ((System.Drawing.Point)(resources.GetObject("$this.Location")));
			this.MaximumSize = ((System.Drawing.Size)(resources.GetObject("$this.MaximumSize")));
			this.MinimumSize = ((System.Drawing.Size)(resources.GetObject("$this.MinimumSize")));
			this.Name = "AttributePropertyDialog";
			this.RightToLeft = ((System.Windows.Forms.RightToLeft)(resources.GetObject("$this.RightToLeft")));
			this.StartPosition = ((System.Windows.Forms.FormStartPosition)(resources.GetObject("$this.StartPosition")));
			this.Text = resources.GetString("$this.Text");
			this.Load += new System.EventHandler(this.AttributePropertyDialog_Load);
			this.ResumeLayout(false);

		}
		#endregion

		private void AttributePropertyDialog_Load(object sender, System.EventArgs e)
		{
			if (_attributeElement != null)
			{
				this.propertyGrid.SelectedObject = _attributeElement;
			}
		}
	}
}
