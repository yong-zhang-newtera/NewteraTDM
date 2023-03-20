/*
* @(#) AttachmentColumnImageGetter.cs
*
* Copyright (c) 2007 Newtera, Inc. All rights reserved.
*
*/
namespace Newtera.WindowsControl
{
	using System;
	using System.Drawing;


	/// <summary>
	/// A implementation class of IDataGridImageGetter for getting an image for the attachment column.
	/// </summary>
	/// <version> 	1.0.0 31 May 2007 </version>
	public class AttachmentColumnImageGetter : IDataGridImageGetter
	{
        private Image _image;

        /// <summary>
        /// Create an instance of AttachmentColumnImageGetter
        /// </summary>
        /// <param name="image">The image for attachment column</param>
        public AttachmentColumnImageGetter(Image image)
        {
            _image = image;
        }

		/// <summary>
		/// Gets an image for a cell val
		/// </summary>
        /// <param name="val">A cell value</param>
        /// <returns>An Image object for the cell value, can be null</returns>
        public Image GetImage(object val)
        {
            Image image = null;

            if (!(val is DBNull))
            {
                int numOfAttachment = System.Convert.ToInt32(val);
                if (numOfAttachment > 0)
                {
                    image = _image;
                }
            }

            return image;
        }
	}
}