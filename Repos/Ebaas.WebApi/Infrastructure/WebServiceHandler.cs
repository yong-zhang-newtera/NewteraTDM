/*
* @(#)WebServiceHandler.cs
*
* Copyright (c) 2003 Newtera, Inc. All rights reserved.
*
*/
namespace Ebaas.WebApi.Infrastructure
{
	using System;
	using System.IO;
	using System.Text;
	using System.Xml;

	using Newtera.Common.Core;
	using Newtera.Data;
	using Newtera.Common.Attachment;
    using Newtera.Common.MetaData.Schema;
	using Newtera.Common.MetaData.FileType;
	using Newtera.Server.Engine.Cache;
	using Newtera.Server.DB;
	using Newtera.Server.Attachment;
	//using Newtera.ChartServer;
    using Newtera.Common.MetaData.Logging;
    using Newtera.Server.Logging;

	/// <summary> 
	/// This class represents a handler that serves as a helper to
	/// AttachmentWebService.
	/// </summary>
	/// <version> 1.0.0 08 Jan 2004 </version>
	public class WebServiceHandler
	{
		CMConnection _connection;

		/// <summary>
		/// Initiate an instance of WebServiceHandler class
		/// </summary>
		public WebServiceHandler()
		{
			_connection = null;
		}

		/// <summary>
		/// Initiate an instance of WebServiceHandler class
		/// </summary>
		public WebServiceHandler(CMConnection connection)
		{
			_connection = connection;
		}

		/// <summary>
		/// Add an attachment info for an item
		/// </summary>
		/// <param name="attachmentType">One of the AttachmentType enum values</param>
		/// <param name="itemId">The id of attaching item, class or instance.</param>
		/// <param name="className">The name of instance's class</param>
		/// <param name="name">The name of an attachment.</param>
		/// <param name="type">The attachment type</param>
		/// <param name="size">The size of an attachment</param>
		/// <param name="isPublic">Indicate whether the attachment is public or not.</param>
		/// <returns>A unique id for the attachment</returns>
		public string AddAttachmentInfo(AttachmentType attachmentType, string itemId, string className,
			string name, string type, long size, bool isPublic)
		{
            throw new NotImplementedException("AddAttachmentInfo is not supported");
		}

		/// <summary>
		/// Add an attachment info for an attachment which will be attached to a specified instance
		/// </summary>
		/// <param name="attachmentType">One of the AttachmentType enum values</param>
		/// <param name="itemId">The id of attaching item.</param>
		/// <param name="attachmentId">The unique attachment id</param>
		/// <param name="className">The name of instance's class</param>
		/// <param name="name">The name of an attachment.</param>
		/// <param name="type">The attachment type</param>
		/// <param name="size">The size of an attachment</param>
        /// <param name="isPublic">Is public</param>
		/// <returns>A unique id for the attachment</returns>
		public string AddAttachmentInfo(AttachmentType attachmentType, string itemId, string attachmentId,
			string className, string name, string type, long size, bool isPublic)
		{
            throw new NotImplementedException("AddAttachmentInfo is not supported");
        }

		/// <summary>
		/// Set an attachment which is contained in RequestContext
		/// </summary>
		/// <param name="attachmentType">One of the AttachmentType enum values.</param>
		/// <param name="attachmentId">The attachment id</param>
		public void SetAttachment(AttachmentType attachmentType, string attachmentId)
		{
            throw new NotImplementedException("SetAttachment is not supported");
        }

		/// <summary>
		/// Delete an attachment from a specified item
		/// </summary>
		/// <param name="attachmentType">One of the AttachmentType enum values.</param>
		/// <param name="itemId">The id of attaching item.</param>
		/// <param name="name">The name of an attachment to be deleted.</param>
		public void DeleteAttachment(AttachmentType attachmentType, string itemId, string name)
		{
            throw new NotImplementedException("DeleteAttachment is not supported");
        }

		/// <summary>
		/// Get an attachment of a specified item.
		/// </summary>
		/// <param name="attachmentType">One of the AttachmentType enum values.</param>
		/// <param name="itemId">The id of specified item.</param>
		/// <param name="name">The name of attachment to obtain.</param>
		/// <remarks>
		/// The attachment is sent back to a client through ResponseContext
		/// </remarks>
		public void GetAttachment(AttachmentType attachmentType, string itemId, string name)
		{
            throw new NotImplementedException("GetAttachment is not supported");
        }

        /// <summary>
        /// Gets count of all attachments of a specified instance.
        /// </summary>
        /// <param name="attachmentType">One of AttachmentType enum values.</param>
        /// <param name="itemId">The id of specified item.</param>
        /// <returns>Count of all attachments of an instance.</returns>
        public int GetAttachmentInfosCount(AttachmentType attachmentType, string itemId)
        {
            throw new NotImplementedException("GetAttachmentInfosCount is not supported");
        }

		/// <summary>
		/// Gets information of all attachments of a specified instance.
		/// </summary>
		/// <param name="attachmentType">One of AttachmentType enum values.</param>
		/// <param name="itemId">The id of specified item.</param>
        /// <param name="startRow">The start row</param>
        /// <param name="pageSize">The page size</param>
		/// <returns>An xml string representing information of all attachments of an instance.</returns>
		public string GetAttachmentInfos(AttachmentType attachmentType, string itemId, int startRow, int pageSize)
		{
            throw new NotImplementedException("GetAttachmentInfos is not supported");
        }

		/// <summary>
		/// Get an attachment of a specified instance.
		/// </summary>
		/// <remarks>
		/// The icon images are returned though DIME records
		/// </remarks>
		public string GetFileTypeInfo()
		{
            throw new NotImplementedException("GetFileTypeInfo is not supported");
        }

		/// <summary>
		/// Get a chart file
		/// </summary>
		/// <param name="connectionStr">The connection string indicating the schema to connect to</param>
		/// <param name="formatName">The format name of the graph file.</param>
		/// <param name="chartId">The id of the saved chart.</param>
		/// <remarks>
		/// The file is sent back to a client through ResponseContext
		/// </remarks>
		public void GetChartFile(string connectionStr, string formatName, string chartId)
		{
		}

        /// <summary>
        /// Called after a schema is deleted
        /// </summary>
        /// <param name="schemaName">The schema name</param>
        /// <param name="version">The schema version</param>
        public void CleanupSchema(string schemaName, string version)
        {
        }

        /// <summary>
        /// Update the value of an image attribute for an data instance
        /// </summary>
        /// <param name="instanceId">The id of an instance to which to add the attachment.</param>
        /// <param name="attributeName">The name of image attribute.</param>
        /// <param name="className">The name of class that the instance belongs to.</param>
        /// <param name="imageFilePath">The path of the uploaded image.</param>
        /// <param name="type">The type of an attachment.</param>
        /// <returns>An unique string representing the image name</returns>
        public string UpdateImageAttributeValue(string instanceId, string attributeName, string className,
            string imageFilePath, string type)
        {
            AttachmentInfo info = new AttachmentInfo();
            info.ItemId = instanceId;
            info.AttributeName = attributeName;
            info.ClassName = className;
            info.Name = imageFilePath;
            info.Type = type;

            CMCommand cmd = _connection.CreateCommand();
            return cmd.AddAttachmentInfo(AttachmentType.Image, info);
        }

        /// <summary>
        /// Get an image of a specified id.
        /// </summary>
        /// <param name="imageId">The id of specified image.</param>
        /// <remarks>
        /// The image is sent back to a client through ResponseContext
        /// </remarks>
        public void GetImage(string imageId)
        {
            throw new NotImplementedException("GetImage is not supported");
        }

        /// <summary>
        /// Delete an image and clear value of corresponding image column.
        /// </summary>
        /// <param name="instanceId">The id of an instance to which to add the attachment.</param>
        /// <param name="attributeName">The name of image attribute.</param>
        /// <param name="className">The name of class that the instance belongs to.</param>
        /// <param name="imageId">The image image.</param>
        public void DeleteImage(string instanceId, string attributeName, string className,
            string imageId)
        {
            throw new NotImplementedException("DeleteImage is not supported");
        }
	}
}