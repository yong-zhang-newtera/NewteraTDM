using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;

namespace Newtera.DataGridActiveX
{
    public partial class ViewArrayDataDialog : Form
    {
        public event GraphCallback GraphEvent;
        public event DownloadFileCallback DownloadFileEvent;
        public event DownloadGraphCallback DownloadGraphEvent;
        public event RunAlgorithmCallback RunAlgorithmEvent;
        public event ShowPivotGridCallback ShowPivotGridEvent;

        public ViewArrayDataDialog()
        {
            InitializeComponent();

            this.dataGridControl.IsWebHosted = false; // hosted by windows form
        }

        /// <summary>
        /// Gets or sets the type of view to be associated with the DataGrid control
        /// the values are Class, Taxon, or Array
        /// </summary>
        public string ViewType
        {
            get
            {
                return this.dataGridControl.ViewType;
            }
            set
            {
                this.dataGridControl.ViewType = value;
            }
        }

        /// <summary>
		/// Gets or sets the id of the data instance that carries array data
		/// </summary>
        public string InstanceId
        {
            get
            {
                return this.dataGridControl.InstanceId;
            }
            set
            {
                this.dataGridControl.InstanceId = value;
            }
        }

        /// <summary>
        /// Gets or sets the name of the array whose data the DataGrid displays
        /// </summary>
        public string ArrayName
        {
            get
            {
                return this.dataGridControl.ArrayName;
            }
            set
            {
                this.dataGridControl.ArrayName = value;
            }
        }

        /// <summary>
        /// Gets or sets the caption of the array whose data the DataGrid displays
        /// </summary>
        public string ArrayCaption
        {
            get
            {
                return this.dataGridControl.ArrayCaption;
            }
            set
            {
                this.dataGridControl.ArrayCaption = value;
            }
        }

        /// <summary>
        /// Gets or sets the name of the class that DataGrid is linked with
        /// </summary>
        public string ClassName
        {
            get
            {
                return this.dataGridControl.ClassName;
            }
            set
            {
                this.dataGridControl.ClassName = value;
            }
        }

        /// <summary>
        /// Gets or sets the connection string that is used to connect the server
        /// </summary>
        public string ConnectionString
        {
            get
            {
                return this.dataGridControl.ConnectionString;
            }
            set
            {
                this.dataGridControl.ConnectionString = value;
            }
        }

        /// <summary>
        /// Gets or sets the base URL of the web service
        /// </summary>
        public string BaseURL
        {
            get
            {
                return this.dataGridControl.BaseURL;
            }
            set
            {
                this.dataGridControl.BaseURL = value;
            }
        }

        /// <summary>
        /// Gets or sets the base Path of the web virtual directory
        /// </summary>
        public string BasePath
        {
            get
            {
                return this.dataGridControl.BasePath;
            }
            set
            {
                this.dataGridControl.BasePath = value;
            }
        }

        /// <summary>
        /// Gets or sets the instance count of result
        /// </summary>
        public int TotalCount
        {
            get
            {
                return this.dataGridControl.TotalCount;
            }
            set
            {
                this.dataGridControl.TotalCount = value;
            }
        }

        private void dataGridControl_GraphEvent(int graphType)
        {
            if (this.GraphEvent != null)
            {
                this.GraphEvent(graphType);
            }
        }

        private void dataGridControl_DownloadFileEvent(string fileName)
        {
            if (this.DownloadFileEvent != null)
            {
                this.DownloadFileEvent(fileName);
            }
        }

        private void dataGridControl_DownloadGraphEvent(string formatName)
        {
            if (this.DownloadGraphEvent != null)
            {
                this.DownloadGraphEvent(formatName);
            }
        }

        private void cancelButton_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private void dataGridControl_RunAlgorithmEvent(string displayUrl)
        {
            if (this.RunAlgorithmEvent != null)
            {
                this.RunAlgorithmEvent(displayUrl);
            }
        }

        private void dataGridControl_ShowPivotGridEvent(string dataSourceId)
        {
            if (this.ShowPivotGridEvent != null)
            {
                this.ShowPivotGridEvent(dataSourceId);
            }
        }
    }
}