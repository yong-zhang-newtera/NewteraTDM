/*
* @(#) DataGridEnumColumnImageGetter.cs
*
* Copyright (c) 2007 Newtera, Inc. All rights reserved.
*
*/
namespace Newtera.DataGridActiveX
{
	using System;
	using System.Drawing;

    using Newtera.DataGridActiveX.DataGridView;

	/// <summary>
	/// A implementation class of IDataGridControlImageGetter for getting images for a column displaying enum images.
	/// </summary>
	/// <version> 	1.0.0 31 May 2007 </version>
	public class DataGridEnumColumnImageGetter : IDataGridControlImageGetter
	{
        private ViewEnumElement _enumElement;

        /// <summary>
        /// Create an instance of DataGridEnumColumnImageGetter
        /// </summary>
        /// <param name="image">The image for attachment column</param>
        public DataGridEnumColumnImageGetter(ViewEnumElement enumElement)
        {
            _enumElement = enumElement;
        }

		/// <summary>
		/// Gets an image for an enum display text
		/// </summary>
        /// <param name="val">An enum display text</param>
        /// <returns>An Image object for the enum display text, or null if the val isn't one of the valid enum values</returns>
        public Image GetImage(object val)
        {
            Image image = null;

            if (!(val is DBNull))
            {
                // convert the enum display text to its value
                string enumVal = _enumElement.GetValue(val.ToString());
                if (!string.IsNullOrEmpty(enumVal))
                {
                    string imageName = _enumElement.GetImageName(enumVal);
                    if (!string.IsNullOrEmpty(imageName))
                    {
                        ImageInfo imageInfo = ImageInfoCache.Instance.GetImageInfo(imageName);
                        image = imageInfo.Image;
                    }
                }
            }

            return image;
        }
	}
}