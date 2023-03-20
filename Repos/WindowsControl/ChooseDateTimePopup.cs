using System;
using System.Drawing;
using System.Collections;
using System.ComponentModel;
using System.Windows.Forms;

using Newtera.Common.Core;
using Newtera.Common.MetaData.DataView;

namespace Newtera.WindowsControl
{
	/// <summary>
	/// ChooseDateTimePopup ��ժҪ˵����
	/// </summary>
	public class ChooseDateTimePopup : System.Windows.Forms.Form, IExprPopup
	{
		public event EventHandler Accept;

		private string _value;
		private DataViewModel _dataView;
		private System.Windows.Forms.MonthCalendar monthCalendar1;
		/// <summary>
		/// ����������������
		/// </summary>
		private System.ComponentModel.Container components = null;

		public ChooseDateTimePopup()
		{
			//
			// Windows ���������֧���������
			//
			InitializeComponent();

			//
			// TODO: �� InitializeComponent ���ú�����κι��캯������
			//
			_value = null;
		}

		#region IExprPopup

		/// <summary>
		/// Gets or sets the value created by the popup
		/// </summary>
		public object Expression
		{
			get
			{
				return _value;
			}
			set
			{
				_value = (String) value;
			}
		}

		/// <summary>
		/// Gets or sets the data view to which a search expression is built
		/// </summary>
		/// <value>A DataVieModel instance</value>
		public DataViewModel DataView
		{
			get
			{
				return _dataView;
			}
			set
			{
				_dataView = value;
			}
		}

		/// <summary>
		/// Gets or sets the coordinates of the upper-left corner of the
		/// popup relative to the window.
		/// </summary>
		public new Point Location
		{
			get
			{
				return base.Location;
			}
			set
			{
				base.Location = value;
			}
		}

		/// <summary>
		/// Show the popup
		/// </summary>
		public new void Show()
		{
			base.Show();
		}

		#endregion

		/// <summary>
		/// ������������ʹ�õ���Դ��
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

		#region Windows ������������ɵĴ���
		/// <summary>
		/// �����֧������ķ��� - ��Ҫʹ�ô���༭���޸�
		/// �˷��������ݡ�
		/// </summary>
		private void InitializeComponent()
		{
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(ChooseDateTimePopup));
            this.monthCalendar1 = new System.Windows.Forms.MonthCalendar();
            this.SuspendLayout();
            // 
            // monthCalendar1
            // 
            resources.ApplyResources(this.monthCalendar1, "monthCalendar1");
            this.monthCalendar1.Name = "monthCalendar1";
            this.monthCalendar1.ShowToday = false;
            this.monthCalendar1.TodayDate = new System.DateTime(2005, 10, 30, 0, 0, 0, 0);
            this.monthCalendar1.DateSelected += new System.Windows.Forms.DateRangeEventHandler(this.monthCalendar1_DateSelected);
            // 
            // ChooseDateTimePopup
            // 
            resources.ApplyResources(this, "$this");
            this.Controls.Add(this.monthCalendar1);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.None;
            this.Name = "ChooseDateTimePopup";
            this.Deactivate += new System.EventHandler(this.ChooseDateTimePopup_Deactivate);
            this.Load += new System.EventHandler(this.ChooseDateTimePopup_Load);
            this.ResumeLayout(false);

		}
		#endregion

		private void ChooseDateTimePopup_Load(object sender, System.EventArgs e)
		{
			if (_value != null && _value.Length > 0)
			{
				try
				{
					DateTime dateTime = System.Convert.ToDateTime(_value);
					this.monthCalendar1.SelectionStart = dateTime;
					this.monthCalendar1.SelectionEnd = dateTime;
				}
				catch (Exception)
				{
				}
			}
		}

		private void ChooseDateTimePopup_Deactivate(object sender, System.EventArgs e)
		{
			this.Close();
		}

		private void monthCalendar1_DateSelected(object sender, System.Windows.Forms.DateRangeEventArgs e)
		{
            _value = e.Start.ToString(LocaleInfo.Instance.CSharpDateFormat);

			// fire the accept event
			if (Accept != null)
			{
				Accept(this, EventArgs.Empty);
			}

			this.Close();	
		}
	}
}
