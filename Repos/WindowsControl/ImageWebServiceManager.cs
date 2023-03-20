/*
* @(#)ImageWebServiceManager.cs
*
* Copyright (c) 2003 Newtera, Inc. All rights reserved.
*
*/
namespace Newtera.WindowsControl
{
	using System;
    using System.Collections;
	using System.Drawing;
	using System.Windows.Forms;

	using Newtera.Common.Core;
    using Newtera.Common.MetaData;
	using Newtera.Common.MetaData.DataView;
	using Newtera.Common.MetaData.Schema;
	
	/// <summary>
	/// The singleton that mamages an image web service.
	/// </summary>
	/// <version>  1.0.1 24 Oct 2008</version>
	public class ImageWebServiceManager
	{
        private IImageWebService _imageWebService;

		/// <summary>
		/// Singleton's private instance.
		/// </summary>
		private static ImageWebServiceManager theManager;
		
		static ImageWebServiceManager()
		{
			theManager = new ImageWebServiceManager();
		}

		/// <summary>
		/// The private constructor.
		/// </summary>
		private ImageWebServiceManager()
		{
            _imageWebService = null;
		}

		/// <summary>
		/// Gets the ImageWebServiceManager instance.
		/// </summary>
		/// <returns> The ImageWebServiceManager instance.</returns>
		static public ImageWebServiceManager Instance
		{
			get
			{
				return theManager;
			}
		}

        /// <summary>
        /// Gets or sets the ImageGetterFactory, set by window clients, such as DesignStudio etc.
        /// </summary>
        public IImageWebService ImageWebService
        {
            get
            {
                return _imageWebService;
            }
            set
            {
                _imageWebService = value;
            }
        }
	}
}