using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using System.IO;

using Newtera.Common.Core;

namespace Newtera.WindowsControl
{
    public partial class EditImageValueDialog : Form
    {
        private string _instanceId;
        private string _className;
        private string _attributeName;
        private SchemaInfo _schemaInfo;
        private string _imageId;
        private bool _isReadOnly = false;

        public EditImageValueDialog()
        {
            InitializeComponent();
        }

        /// <summary>
        /// Gets or sets the id of an instance to which to add an image.
        /// </summary>
        /// <value>A string representing an instance</value>
        public string InstanceId
        {
            get
            {
                return _instanceId;
            }
            set
            {
                _instanceId = value;
            }
        }

        /// <summary>
        /// Gets or sets the name of an image attribute.
        /// </summary>
        /// <value>A string representing an image attribute name</value>
        public string AttributeName
        {
            get
            {
                return _attributeName;
            }
            set
            {
                _attributeName = value;
            }
        }

        /// <summary>
        /// Gets or sets the name of an instance class.
        /// </summary>
        /// <value>A string representing a class name</value>
        public string ClassName
        {
            get
            {
                return _className;
            }
            set
            {
                _className = value;
            }
        }

        /// <summary>
        /// Gets or sets the schema info.
        /// </summary>
        /// <value>A SchemaInfo object</value>
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
        /// Gets or sets the image name
        /// </summary>
        public string ImageId
        {
            get
            {
                return _imageId;
            }
            set
            {
                _imageId = value;
            }
        }

        /// <summary>
        /// Gets or sets information indicating whether the image property is read only
        /// </summary>
        public bool IsReadOnly
        {
            get
            {
                return _isReadOnly;
            }
            set
            {
                _isReadOnly = value;
            }
        }

        #region private methods

        private void DisplayImage()
        {
            try
            {
                Cursor.Current = Cursors.WaitCursor;

                IImageWebService service = ImageWebServiceManager.Instance.ImageWebService;

                if (service != null)
                {
                    Image image = service.GetImage(_schemaInfo, _imageId);

                    this.pictureBox1.Image = image;
                }
            }
            catch (IOException ex)
            {
                MessageBox.Show(ex.Message, "Server Error",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Error);
            }
            finally
            {
                // Restore the cursor
                Cursor.Current = this.Cursor;
            }
        }

        private void ClearImage()
        {
            if (!string.IsNullOrEmpty(_imageId))
            {
                try
                {
                    Cursor.Current = Cursors.WaitCursor;

                    IImageWebService service = ImageWebServiceManager.Instance.ImageWebService;

                    if (service != null)
                    {
                        // invoke the web service synchronously to delete the specified image
                        service.DeleteImage(_schemaInfo, _instanceId,
                            _attributeName,
                            _className,
                            _imageId);

                        // clear the displayed image
                        this.pictureBox1.Image = this.pictureBox1.InitialImage;
                        _imageId = null;
                    }

                    this.DeleteButton.Enabled = false;
                }
                catch (IOException ex)
                {
                    MessageBox.Show(ex.Message, "Server Error",
                        MessageBoxButtons.OK,
                        MessageBoxIcon.Error);
                }
                finally
                {
                    // Restore the cursor
                    Cursor.Current = this.Cursor;
                }
            }
        }

        #endregion

        private void EditImageValueDialog_Load(object sender, EventArgs e)
        {
            if (!string.IsNullOrEmpty(_imageId))
            {
                DisplayImage();
            }
            else
            {
                this.DeleteButton.Enabled = false;
            }

            if (string.IsNullOrEmpty(InstanceId))
            {
                // do not allow uploading an image when the instance does not exist
                uploadButton.Enabled = false;
            }

            if (IsReadOnly)
            {
                uploadButton.Enabled = false;
                DeleteButton.Enabled = false;
            }
        }

        private void uploadButton_Click(object sender, EventArgs e)
        {
            UploadImageDialog dialog = new UploadImageDialog();
            dialog.InstanceId = InstanceId;
            dialog.AttributeName = AttributeName;
            dialog.ClassName = ClassName;
            dialog.SchemaInfo = SchemaInfo;
            dialog.ImageId = _imageId;

            if (dialog.ShowDialog() == DialogResult.OK)
            {
                _imageId = dialog.ImageId;

                if (!string.IsNullOrEmpty(_imageId))
                {
                    DisplayImage();
                    this.DeleteButton.Enabled = true;
                }
            }
        }

        private void DeleteButton_Click(object sender, EventArgs e)
        {
            ClearImage();
        }
    }
}