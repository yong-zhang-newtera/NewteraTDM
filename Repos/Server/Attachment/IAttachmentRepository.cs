/*
* @(#) IAttachmentRepository.cs
*
* Copyright (c) 2003 Newtera, Inc. All rights reserved.
*
*/
namespace Newtera.Server.Attachment
{
	using System;
	using System.IO;

	using Newtera.Common.Core;
	using Newtera.Common.Attachment;

	/// <summary>
	/// A common interface for attachment repositories.
	/// </summary>
	/// <version> 1.0.1	10 Jan 2004 </version>
	/// <author>Yong Zhang </author>
	public interface IAttachmentRepository
	{
		/// <summary>
		/// Add an attachment info for an attachment that is attached to a specified instance
		/// </summary>
		/// <param name="attachmentType">One of the AttachmentType enum values.</param>
		/// <param name="attachInfo">The attachment information that describes the attachment.</param>
		/// <param name="schemaInfo">The schema info</param>
		/// <param name="schemaId">The schema id</param>
		/// <returns>An unique id for the attachment</returns>
		string AddAttachmentInfo(AttachmentType attachmentType, AttachmentInfo attachInfo, SchemaInfo schemaInfo, string schemaId);

		/// <summary>
		/// Set an attachment to a specified instance.
		/// </summary>
		/// <param name="attachmentType">One of the AttachmentType enum values.</param>
		/// <param name="id">An unique id for the attachment</param>
		/// <param name="stream">A stream from which to read attachment data.</param>
		void SetAttachment(AttachmentType attachmentType, string id, Stream stream);

		/// <summary>
		/// Delete an attachment from a specified instance
		/// </summary>
		/// <param name="attachmentType">One of the AttachmentType enum values.</param>
		/// <param name="attachInfo">The AttachmentInfo that specifies an attachment to be deleted.</param>
		/// <param name="schemaInfo">The schema info</param>
		void DeleteAttachment(AttachmentType attachmentType, AttachmentInfo attachInfo, SchemaInfo schemaInfo);

		/// <summary>
		/// Get an attachment of a specified item as a stream.
		/// </summary>
		/// <param name="attachmentType">One of the AttachmentType enum values.</param>
		/// <param name="attachInfo">The AttachmentInfo that specifies an attachment to obtain.</param>
		/// <returns>A Stream object from which to read data of an attachment</returns>
		Stream GetAttachment(AttachmentType attachmentType, AttachmentInfo attachInfo);

		/// <summary>
		/// Get an attachment of a specified instance as a buffered byte array.
		/// </summary>
		/// <param name="attachmentType">One of the AttachmentType enum values.</param>
		/// <param name="attachInfo">The AttachmentInfo that specifies an attachment to obtain.</param>
		/// <returns>A buffered byte array that contains binary data of an attachment</returns>
		byte[] GetBufferedAttachment(AttachmentType attachmentType, AttachmentInfo attachInfo);

		/// <summary>
		/// Save an attachment to a stream
		/// </summary>
		/// <param name="attachmentType">One of the AttachmentType enum values.</param>
		/// <param name="attachInfo">The AttachmentInfo that specifies an attachment to be saved.</param>
		/// <param name="stream">The steam to write attachment to.</param>
		void SaveAttachmentAs(AttachmentType attachmentType, AttachmentInfo attachInfo, Stream stream);

		/// <summary>
		/// Gets information of a specified attachment.
		/// </summary>
		/// <param name="attachmentType">One of the AttachmentTye enum values.</param>
		/// <param name="itemId">The id of the attaching item.</param>
		/// <param name="name">The name of an attachment.</param>
		/// <returns>An AttachmentInfo object</returns>
		AttachmentInfo GetAttachmentInfo(AttachmentType attachmentType, string itemId, string name);

		/// <summary>
		/// Gets information of a specified attachment.
		/// </summary>
		/// <param name="attachmentType">One of the AttachmentTye enum values.</param>
		/// <param name="attachmentId">The id of attachment.</param>
		/// <returns>An AttachmentInfo object</returns>
		AttachmentInfo GetAttachmentInfo(AttachmentType attachmentType, string attachmentId);
		
		/// <summary>
		/// Gets information of all attachments of a specified item. If the user is
		/// unauthenticated, only return the public attachments
		/// </summary>
		/// <param name="attachmentType">One of the AttachmentType enum values.</param>
		/// <param name="itemId">The id of specified item.</param>
		/// <param name="schemaId">The id of the schema the instance belongs to.</param>
		/// <param name="startRow">Indicating starting row</param>
        /// <param name="pageSize">The page size</param>
		/// <returns>A collection of AttachmentInfo objects</returns>
		AttachmentInfoCollection GetAttachmentInfos(AttachmentType attachmentType, string itemId, string schemaId, int startRow, int pageSize);

        /// <summary>
        /// Gets count of all attachments of a specified item. If the user is
        /// unauthenticated, only return count of the public attachments
        /// </summary>
        /// <param name="attachmentType">One of the AttachmentType enum values.</param>
        /// <param name="itemId">The id of specified item.</param>
        /// <param name="schemaId">The id of the schema the instance belongs to.</param>
        /// <returns>A count of AttachmentInfo objects</returns>
        int GetAttachmentInfosCount(AttachmentType attachmentType, string itemId, string schemaId);

        /// <summary>
        /// Delete class or instance attachments belongs to a database schema.
        /// Used when deleting a database schema.
        /// </summary>
        /// <param name="attachmentType">One of the AttachmentType enum values.</param>
        /// <param name="schemaId">The id of the schem.</param>
        void DeleteAttachmentInfos(AttachmentType attachmentType, string schemaId);

        /// <summary>
        /// Delete attachment infos associated with a specific data instance.
        /// </summary>
        /// <param name="objId">The id of the instance that the attachments are associated with.</param>
        /// <returns>The number of records deleted</returns>
        int DeleteInstanceAttachmentInfos(string objId);
	}
}