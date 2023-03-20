/*
* @(#) ImageAttributeImageGetter.cs
*
* Copyright (c) 2008 Newtera, Inc. All rights reserved.
*
*/
namespace Newtera.Studio
{
	using System;
	using System.Drawing;
    using System.IO;
    using Microsoft.Web.Services.Dime;

    using Newtera.Common.Core;
    using Newtera.Common.MetaData.Schema;
    using Newtera.WindowsControl;
    using Newtera.Common.Attachment;
    using Newtera.WinClientCommon;

	/// <summary>
    /// A implementation class of IDataGridImageGetter for getting thumbnail images for a image attribute.
	/// </summary>
	/// <version> 	1.0.0 09 Jul 2008 </version>
	public class ImageAttributeImageGetter : IDataGridImageGetter
	{
        private SchemaInfo _schemaInfo;
        private ImageAttributeElement _imageAttribute;
        private AttachmentServiceStub _service;

        /// <summary>
        /// Create an instance of ImageAttributeImageGetter
        /// </summary>
        /// <param name="image">The image for attachment column</param>
        public ImageAttributeImageGetter(ImageAttributeElement imageAttribute, SchemaInfo schemaInfo)
        {
            _schemaInfo = schemaInfo;
            _imageAttribute = imageAttribute;
            _service = new AttachmentServiceStub();
        }

		/// <summary>
		/// Gets an image that is a thunbnail of the actual image
		/// </summary>
        /// <param name="val">An enum display text</param>
        /// <returns>An thumbnial Image</returns>
        public Image GetImage(object val)
        {
            Image image = null;

            if (val != null && !(val is System.DBNull) && ((string) val).Length > 0)
            {
                if (image == null)
                {
                    // display a default image of size 16X16
                    ImageInfo imageInfo = ImageInfoCache.Instance.GetImageInfo(null);
                    image = imageInfo.Image;
                }
            }

            return image;
        }

        public bool ThumbnailCallback()
        {
            return false;
        }
	}
}