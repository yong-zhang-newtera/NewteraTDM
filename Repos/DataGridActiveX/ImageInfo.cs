/*
* @(#)ImageInfo.cs
*
* Copyright (c) 2006 Newtera, Inc. All rights reserved.
*
*/
namespace Newtera.DataGridActiveX
{
	using System;
	using System.Drawing;
    using System.Reflection;

	/// <summary>
	/// A class represents an user image
	/// </summary>
	/// <version>  	1.0.0 29 May 2007</version>
    [Serializable]
    public class ImageInfo
	{
		private string _name;
		private Image _image;

		/// <summary>
		/// Initiate an instance of ImageInfo
		/// </summary>
		public ImageInfo()
		{
			_name = null;
            _image = null;
		}

		/// <summary>
		/// Gets or sets the name of the column
		/// </summary>
		public string Name
		{
			get
			{
				return _name;
			}
			set
			{
				_name = value;
			}
		}

		/// <summary>
		/// Gets or sets the image
		/// </summary>
		public Image Image
		{
			get
			{
                return _image;
			}
			set
			{
                _image = value;
			}
		}
	}
}