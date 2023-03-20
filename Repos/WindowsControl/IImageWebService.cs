/*
* @(#) IImageWebService.cs
*
* Copyright (c) 2008 Newtera, Inc. All rights reserved.
*
*/
namespace Newtera.WindowsControl
{
	using System;
    using System.IO;
	using System.Drawing;

    using Newtera.Common.Core;

	/// <summary>
	/// A interface for image web service.
	/// </summary>
	/// <version> 	1.0.0 24 Oct 2008 </version>
	public interface IImageWebService
	{
		/// <summary>
		/// Gets an image from server
		/// </summary>
        Image GetImage(SchemaInfo schemaInfo, string imageId);

		/// <summary>
		/// Delete an image
		/// </summary>
        void DeleteImage(SchemaInfo schemaInfo, string instanceId,
                            string attributeName,
                            string className,
                            string imageId);

        /// <summary>
        /// Update image attribute
        /// </summary>
        /// <param name="instanceId"></param>
        /// <param name="attributeName"></param>
        /// <param name="className"></param>
        /// <param name="filePath"></param>
        /// <param name="type"></param>
        /// <returns></returns>
        string UpdateImageAttributeValue(SchemaInfo schemaInfo, 
                    string instanceId,
                    string attributeName,
                    string className,
                    string filePath,
                    string type);

        /// <summary>
        /// Upload an image
        /// </summary>
        /// <param name="imageId"></param>
        /// <param name="stream"></param>
        void UploadImage(SchemaInfo schemaInfo, string imageId, string type, Stream stream);

        /// <summary>
        /// Gets the type of a file based on its suffix
        /// </summary>
        /// <param name="fileName">The file name</param>
        /// <returns>A string represents a type</returns>
        string GetMIMEType(string fileName);
	}
}