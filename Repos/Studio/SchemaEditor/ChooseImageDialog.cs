using System;
using System.IO;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;

using Newtera.WinClientCommon;

namespace Newtera.Studio
{
    public partial class ChooseImageDialog : Form
    {
        private string _imageName = null;
        private ImageInfo _selectedImageInfo = null;
        private List<ImageInfo> _imageInfos;

        public ChooseImageDialog()
        {
            InitializeComponent();
        }

        public string ImageName
        {
            get
            {
                return _imageName;
            }
            set
            {
                _imageName = value;
            }
        }

        /// <summary>
        /// Gets or sets the selected image info
        /// </summary>
        public ImageInfo SelectedImageInfo
        {
            get
            {
                return _selectedImageInfo;
            }
        }

        private void ChooseImageDialog_Load(object sender, EventArgs e)
        {
            _imageInfos = ImageInfoCache.Instance.GetAllImageInfos();
            int index = 0;
            foreach (ImageInfo imageInfo in _imageInfos)
            {
                this.imageList.Images.Add(imageInfo.Image);

                ListViewItem item = new ListViewItem(imageInfo.Name);
                item.ImageIndex = index++;

                if (_imageName != null &&
                    _imageName == imageInfo.Name)
                {
                    item.Selected = true;
                }

                this.imageListView.Items.Add(item);
            }
        }

        private void imageListView_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (this.imageListView.SelectedIndices.Count == 1)
            {
                _selectedImageInfo = _imageInfos[this.imageListView.SelectedIndices[0]];
                this.okButton.Enabled = true;
            }
        }
    }
}