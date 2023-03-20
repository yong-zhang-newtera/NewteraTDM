using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using System.IO;

using Newtera.Common.Core;
using Newtera.Common.Attachment;
using Newtera.Common.MetaData.FileType;

namespace Newtera.WindowsControl
{
    public partial class UploadImageDialog : Form
    {
        private string _instanceId;
        private string _className;
        private string _attributeName;
        private SchemaInfo _schemaInfo;
        private string _filePath = null;
        private string _imageId = null;

        public UploadImageDialog()
        {
            InitializeComponent();
        }

        /// <summary>
        /// Gets or sets the id of the image created by the server.
        /// </summary>
        /// <value>A string representing an image id</value>
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
        /// Gets or sets the name of class that owns the image attribute.
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
        /// Gets the selected image file path
        /// </summary>
        public string ImageFilePath
        {
            get
            {
                return _filePath;
            }
        }

        #region Private Methods

        private void UploadImage()
        {
            FileStream fileStream = null;
            try
            {
                // Change the cursor to indicate that we are waiting
                Cursor.Current = Cursors.WaitCursor;

                IImageWebService service = ImageWebServiceManager.Instance.ImageWebService;

                if (service != null)
                {
                    // delete the existing image file on the server
                    if (!string.IsNullOrEmpty(_imageId))
                    {   
                        service.DeleteImage(_schemaInfo,
                                            _instanceId,
                                            _attributeName,
                                            _className,
                                            _imageId);
                    }

                    // Get the a stream to read the selected file
                    fileStream = File.OpenRead(_filePath);

                    // infer the mime type from the suffix of a file name
                    string type = service.GetMIMEType(_filePath);

                    // create an unique image name and
                    // save it to the corresponding image column
                    _imageId = service.UpdateImageAttributeValue(_schemaInfo,
                        _instanceId,
                        _attributeName,
                        _className,
                        _filePath,
                        type);

                    // upload the selected image to a server
                    // directory
                    service.UploadImage(_schemaInfo, _imageId, type, fileStream);
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

                if (fileStream != null)
                {
                    fileStream.Close();
                }
            }
        }

        #endregion

        private void browseButton_Click(object sender, EventArgs e)
        {
            OpenFileDialog dialog = new OpenFileDialog();

            dialog.InitialDirectory = "c:\\";
            dialog.Filter = "Image Files(*.BMP;*.JPG;*.GIF;*.PNG)|*.BMP;*.JPG;*.GIF;*.PNG";
            dialog.FilterIndex = 1;
            dialog.RestoreDirectory = true;

            if (dialog.ShowDialog() == DialogResult.OK)
            {
                _filePath = dialog.FileName;
                this.textBox1.Text = _filePath;
                if (!string.IsNullOrEmpty(_filePath))
                {
                    this.okButton.Enabled = true;
                }
            }
        }

        private void okButton_Click(object sender, EventArgs e)
        {
            UploadImage();
        }
    }
}