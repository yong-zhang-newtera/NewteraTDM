/*
* @(#) ImageWebServiceProxy.cs
*
* Copyright (c) 2008 Newtera, Inc. All rights reserved.
*
*/
namespace Newtera.Studio
{
	using System;
    using System.IO;
	using System.Drawing;
    using Microsoft.Web.Services.Dime;

    using Newtera.Common.Core;
    using Newtera.Common.MetaData.FileType;
    using Newtera.WindowsControl;
    using Newtera.Common.Attachment;
    using Newtera.WinClientCommon;

	/// <summary>
	/// A interface for image web service.
	/// </summary>
	/// <version> 	1.0.0 24 Oct 2008 </version>
	public class ImageWebServiceProxy : IImageWebService
	{
		/// <summary>
		/// Gets an image from server
		/// </summary>
        public Image GetImage(SchemaInfo schemaInfo, string imageId)
        {
            Image image = null;

            return image;
        }

		/// <summary>
		/// Delete an image
		/// </summary>
        public void DeleteImage(SchemaInfo schemaInfo, 
                            string instanceId,
                            string attributeName,
                            string className,
                            string imageId)
        {
            AttachmentServiceStub service = new AttachmentServiceStub();

            // invoke the web service synchronously to delete the specified image
            service.DeleteImage(ConnectionStringBuilder.Instance.Create(schemaInfo),
                instanceId,
                attributeName,
                className,
                imageId);
        }

        /// <summary>
        /// Update image attribute
        /// </summary>
        /// <param name="instanceId"></param>
        /// <param name="attributeName"></param>
        /// <param name="className"></param>
        /// <param name="filePath"></param>
        /// <param name="type"></param>
        /// <returns></returns>
        public string UpdateImageAttributeValue(SchemaInfo schemaInfo, 
                    string instanceId,
                    string attributeName,
                    string className,
                    string filePath,
                    string type)
        {
            AttachmentServiceStub service = new AttachmentServiceStub();

            // invoke the web service synchronously to create an unique image name and
            // save it to the corresponding image column
            string imageId = service.UpdateImageAttributeValue(ConnectionStringBuilder.Instance.Create(schemaInfo),
                instanceId,
                attributeName,
                className,
                filePath,
                type);

            return imageId;
        }

        /// <summary>
        /// Upload an image
        /// </summary>
        /// <param name="imageId"></param>
        /// <param name="stream"></param>
        public void UploadImage(SchemaInfo schemaInfo, 
            string imageId, 
            string type,
            Stream stream)
        {
        }

        /// <summary>
        /// Gets the type of a file based on its suffix
        /// </summary>
        /// <param name="fileName">The file name</param>
        /// <returns>A string represents a type</returns>
        public string GetMIMEType(string fileName)
        {
            FileTypeInfo fileTypeInfo = FileTypeInfoManager.Instance.GetFileTypeInfoByName(fileName);

            return fileTypeInfo.Type;
        }
	}
}