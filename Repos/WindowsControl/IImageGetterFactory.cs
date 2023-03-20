/*
* @(#)IImageGetterFactory.cs
*
* Copyright (c) 2008 Newtera, Inc. All rights reserved.
*
*/
namespace Newtera.WindowsControl
{
	using System;
	using System.Drawing;
	using System.Windows.Forms;

	using Newtera.Common.Core;
	using Newtera.Common.MetaData;
	using Newtera.Common.MetaData.Schema;
    using Newtera.WindowsControl;
	
	/// <summary>
	/// A Interface for a factory getting an image getter.
	/// </summary>
	/// <version>  1.0.1 14 Jul 2008</version>
	public interface IImageGetterFactory
	{
		/// <summary>
		/// Create a IImageGetter for the given getter type
		/// </summary>
		/// <param name="schemaModelElement">The schema model element</param>
        /// <returns>A IDataGridImageGetter instance</returns>
        IDataGridImageGetter Create(ImageGetterType getterType, IMetaDataElement schemaModelElement, SchemaInfo schemaInfo);
	}

    // Image Getter types
    public enum ImageGetterType
    {
        EnumColumnImageGetter,
        ImageAttributeImageGetter
    }
}