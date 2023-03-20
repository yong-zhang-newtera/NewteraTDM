/*
* @(#)ImageGetterFactory.cs
*
* Copyright (c) 2008 Newtera, Inc. All rights reserved.
*
*/
namespace Newtera.Studio
{
	using System;
	using System.Drawing;
	using System.Windows.Forms;

	using Newtera.Common.Core;
	using Newtera.Common.MetaData;
	using Newtera.Common.MetaData.Schema;
    using Newtera.WindowsControl;
	
	/// <summary>
	/// A factory that create a IImageGetter instance.
	/// </summary>
	/// <version>  1.0.1 14 Jul 2008</version>
    public class ImageGetterFactory : IImageGetterFactory
	{
		/// <summary>
		/// Singleton's private instance.
		/// </summary>
		private static ImageGetterFactory theFactory;
		
		static ImageGetterFactory()
		{
			theFactory = new ImageGetterFactory();
		}

		/// <summary>
		/// The private constructor.
		/// </summary>
		private ImageGetterFactory()
		{
		}

		/// <summary>
		/// Gets the ImageGetterFactory instance.
		/// </summary>
		/// <returns> The ImageGetterFactory instance.</returns>
		static public ImageGetterFactory Instance
		{
			get
			{
				return theFactory;
			}
		}

		/// <summary>
		/// Create a IImageGetter for the given schema model element
		/// </summary>
		/// <param name="schemaModelElement">The schema model element</param>
        /// <returns>A IDataGridImageGetter instance</returns>
		public IDataGridImageGetter Create(ImageGetterType getterType, IMetaDataElement schemaModelElement, SchemaInfo schemaInfo)
		{
            IDataGridImageGetter getter = null;

            switch (getterType)
            {
                case ImageGetterType.ImageAttributeImageGetter:

                    getter = new ImageAttributeImageGetter((ImageAttributeElement)schemaModelElement, schemaInfo);

                    break;

                case ImageGetterType.EnumColumnImageGetter:

                    getter = new EnumColumnImageGetter((EnumElement)schemaModelElement);

                    break;
            }

            return getter;
		}
	}
}