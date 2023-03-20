/*
* @(#)ImageResultElement.cs
*
* Copyright (c) 2008 Newtera, Inc. All rights reserved.
*
*/
namespace Newtera.Common.MetaData.DataView.QueryBuilder
{
	using System;
	using System.Text;
	using System.Collections;

	/// <summary>
	/// Represents a root xml element in return clause of a XQuery.
	/// </summary>
	/// <version>  	1.0.0 05 Jul 2008</version>
	internal class ImageResultElement : QueryElementBase
	{
		private DataImageAttribute _imageAttribute;

		/// <summary>
		/// Initiating an instance of ImageResultElement class
		/// </summary>
        /// <param name="imageAttribute">The image attribute</param>
        public ImageResultElement(DataImageAttribute imageAttribute)
            : base()
		{
            _imageAttribute = imageAttribute;
		}

		/// <summary>
		/// Gets the XQuery representation of the element.
		/// </summary>
		/// <returns>A XQuery segmentation</returns>
		public override string ToXQuery()
		{
			StringBuilder query = new StringBuilder();

            query.Append("{").Append(((IQueryElement)_imageAttribute).ToXQuery()).Append("}");

			query.Append("\n");

			return query.ToString();
		}
	}
}