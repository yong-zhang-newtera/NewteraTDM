/*
* @(#) ImageInfoCache.cs
*
* Copyright (c) 2007 Newtera, Inc. All rights reserved.
*
*/
namespace Newtera.DataGridActiveX
{
	using System;
    using System.Drawing;
    using System.IO;
    using System.Text;
    using System.Collections;
    using System.Collections.Generic;

    using Newtera.DataGridActiveX.ActiveXControlWebService;

	/// <summary>
	/// The singleton class that serves as a cache for user images that will be displayed on
    /// user interface.
	/// </summary>
	/// <version> 	1.0.0 29 May 2007 </version>
	public class ImageInfoCache
	{		
		// Static cache object, all invokers will use this cache object.
		private static ImageInfoCache theCache;

        private byte[] _unknowImage = new byte[] {66,77,246,0,0,0,0,0,0,0,118,0,0,0,40,0,0,0,16,0,0,0,16,0,0,0,1,0,4,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,128,0,0,128,0,0,0,128,128,0,128,0,0,0,128,0,128,0,128,128,0,0,128,128,128,0,192,192,192,0,0,0,255,0,0,255,0,0,0,255,255,0,255,0,0,0,255,0,255,0,255,255,0,0,255,255,255,0,0,0,0,0,0,0,0,255,120,136,136,136,136,136,128,255,127,17,31,255,255,255,128,255,127,249,145,17,15,255,128,255,127,255,249,151,128,255,128,255,127,240,0,255,112,79,128,255,127,42,162,15,254,196,128,255,127,42,162,9,159,204,64,255,127,43,162,9,145,252,196,255,127,242,32,153,145,255,204,79,127,255,249,153,145,255,140,196,127,255,247,119,113,255,128,204,127,255,255,17,17,0,0,252,127,255,255,255,255,143,127,255,127,255,255,255,255,135,255,255,119,119,119,119,119,127,255,255};
        private Hashtable _imageInfoTable;
        private List<ImageInfo> _imageInfos;
        private ActiveXControlService _webService;
		
		/// <summary>
		/// Private constructor.
		/// </summary>
		private ImageInfoCache()
		{
            _imageInfoTable = new Hashtable();
            _imageInfos = null;
            _webService = null;
		}

		/// <summary>
		/// Gets the ImageInfoCache instance.
		/// </summary>
		/// <returns> The ImageInfoCache instance.</returns>
		static public ImageInfoCache Instance
		{
			get
			{
				return theCache;
			}
		}

        /// <summary>
        /// Gets or sets the web service used to get images from server
        /// </summary>
        public ActiveXControlService WebService
        {
            get
            {
                return _webService;
            }
            set
            {
                _webService = value;
            }
        }
		
		/// <summary>
		/// Get a image info of given name.
		/// </summary>
		/// <returns>A ImageInfo object.</returns>
		public ImageInfo GetImageInfo(string imageName)
		{
            ImageInfo imageInfo;
            if (!string.IsNullOrEmpty(imageName))
            {
                imageInfo = (ImageInfo)_imageInfoTable[imageName];
                if (imageInfo == null)
                {
                    imageInfo = GetImageInfoFromServer(imageName);
                    _imageInfoTable[imageName] = imageInfo;
                }
            }
            else
            {
                imageInfo = new ImageInfo();
                imageInfo.Name = "";
                MemoryStream ms = new MemoryStream(_unknowImage);
                imageInfo.Image = Image.FromStream(ms);
            }

            return imageInfo;
		}

        /// <summary>
        /// Get all user provided images.
        /// </summary>
        /// <returns>A list of ImageInfo object.</returns>
        public List<ImageInfo> GetAllImageInfos()
        {
            if (_imageInfos == null && _webService != null)
            {
                List<ImageInfo> imageInfos = new List<ImageInfo>();

                string[] imageNames = _webService.GetImageNames();
                ImageInfo imageInfo;
                MemoryStream ms;
                foreach (string imageName in imageNames)
                {
                    imageInfo = new ImageInfo();
                    imageInfo.Name = imageName;
                    try
                    {
                        byte[] imageData = _webService.GetImageBytes(imageName);
                        ms = new MemoryStream(imageData);
                        imageInfo.Image = Image.FromStream(ms);
                    }
                    catch (Exception)
                    {
                        // unable to find the image from the server.
                        // return an unknown image
                        ms = new MemoryStream(_unknowImage);
                        imageInfo.Image = Image.FromStream(ms);
                    }

                    imageInfos.Add(imageInfo);
                    _imageInfoTable[imageName] = imageInfo;
                }

                _imageInfos = imageInfos;
            }

            return _imageInfos;
        }

        /// <summary>
        /// Gets an image from the server
        /// </summary>
        /// <param name="imageName">The image name</param>
        /// <returns>An ImageInfo object</returns>
        private ImageInfo GetImageInfoFromServer(string imageName)
        {
            ImageInfo imageInfo = new ImageInfo();
            imageInfo.Name = imageName;
            MemoryStream ms;

            if (_webService != null)
            {
                byte[] imageData = _webService.GetImageBytes(imageName);
                ms = new MemoryStream(imageData);
                imageInfo.Image = Image.FromStream(ms);
            }
            else
            {
                // unable to get the image from the server.
                // return an unknown image
                ms = new MemoryStream(this._unknowImage);
                imageInfo.Image = Image.FromStream(ms);
            }

            return imageInfo;
        }

		static ImageInfoCache()
		{
			// Initializing the factory.
			{
				theCache = new ImageInfoCache();
			}
		}
	}
}