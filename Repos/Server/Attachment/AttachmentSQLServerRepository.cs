/*
* @(#) AttachmentSQLServerRepository.cs
*
* Copyright (c) 2003 Newtera, Inc. All rights reserved.
*
*/
namespace Newtera.Server.Attachment
{
	using System;
	using System.IO;
	using System.Data;
	using System.Data.SqlClient;

	using Newtera.Common.Core;
	using Newtera.Common.Attachment;
	using Newtera.Server.DB;

	/// <summary>
	/// Represents a database implementation of IAttachmentRepository interface
	/// where attachment files are stored as BLOB in database tables.
	/// </summary>
	/// <version> 1.0.1	10 Jan 2004 </version>
	/// <author>Yong Zhang </author>
	public class AttachmentSQLServerRepository : AttachmentRepositoryBase
	{
		/// <summary>
		/// Instantiate an instance of AttachmentSQLServerRepository class.
		/// </summary>
		public AttachmentSQLServerRepository() : base()
		{
		}

		/// <summary>
		/// Set an attachment to a specified instance
		/// </summary>
		/// <param name="attachmentType">One of the AttachmentType enum values</param>
		/// <param name="id">An unique id for the attachment</param>
		/// <param name="stream">A stream from which to read attachment data.</param>
		public override void SetAttachment(AttachmentType attachmentType, string id, Stream stream)
		{
			using (IClobDAO blobDAO = ClobDAOFactory.Instance.Create(_dataProvider))
			{
				blobDAO.WriteAttachmentBlob(attachmentType, stream, id);			
			}
		}

		/// <summary>
		/// Delete an attachment from a specified item
		/// </summary>
		/// <param name="attachmentType">One of the AttachmentType enum values</param>
		/// <param name="attachInfo">The AttachmentInfo that specifies an attachment to be deleted.</param>
		/// <param name="schemaInfo">The schema info</param>		
		public override void DeleteAttachment(AttachmentType attachmentType, AttachmentInfo attachInfo, SchemaInfo schemaInfo)
		{
            if (attachmentType != AttachmentType.Image)
            {
                // Delete the record in database that represents an attachment
                // The Blob storing the attachment data will be deleted also.
                int count = DeleteAttachmentInfo(attachmentType, attachInfo);

                if (count == 1 && attachmentType == AttachmentType.Instance)
                {
                    // Update the ANUM column of physical tables storing instance data
                    DecreamentANUMValue(attachInfo, schemaInfo);
                }
            }
            else
            {
                // clear the image attribute value
                UpdateImageAttributeValue(attachInfo, schemaInfo);
            }
		}

		/// <summary>
		/// Get an attachment of a specified instance as a stream.
		/// </summary>
		/// <param name="attachmentType">One of the AttachmentType enum values</param>
		/// <param name="attachInfo">The AttachmentInfo that specifies an attachment to obtain.</param>
		/// <returns>A Stream object from which to read data of an attachment</returns>
		public override Stream GetAttachment(AttachmentType attachmentType, AttachmentInfo attachInfo)
		{
			MemoryStream mStream = null;
	
			byte[] buffer = new byte[attachInfo.Size];
			mStream = new MemoryStream(buffer);

			using (IClobDAO blobDAO = ClobDAOFactory.Instance.Create(_dataProvider))
			{				
				// TODO, send the attachment data back in chunks using DimeWriter
				blobDAO.ReadAttachmentBlob(attachmentType, mStream, attachInfo.ItemId, attachInfo.Name);				
			}

			return mStream;
		}

		/// <summary>
		/// Get an attachment of a specified instance as a buffered byte array.
		/// </summary>
		/// <param name="attachmentType">One of the AttachmentType enum values</param>
		/// <param name="attachInfo">The AttachmentInfo that specifies an attachment to obtain.</param>
		/// <returns>A buffered byte array that contains binary data of an attachment</returns>
		public override byte[] GetBufferedAttachment(AttachmentType attachmentType, AttachmentInfo attachInfo)
		{
			byte[] buffer;
	
			using (IClobDAO blobDAO = ClobDAOFactory.Instance.Create(_dataProvider))
			{				
				// TODO, send the attachment data back in chunks using DimeWriter
				buffer = new byte[attachInfo.Size];
				MemoryStream mStream = new MemoryStream(buffer);

				blobDAO.ReadAttachmentBlob(attachmentType, mStream, attachInfo.ItemId, attachInfo.Name);				
			}

			return buffer;
		}

		/// <summary>
		/// Save an attachment to a stream
		/// </summary>
		/// <param name="attachmentType">One of the AttachmentType enum values</param>
		/// <param name="attachInfo">The AttachmentInfo that specifies an attachment to be saved.</param>
		/// <param name="stream">The steam to write attachment to.</param>
		public override void SaveAttachmentAs(AttachmentType attachmentType, AttachmentInfo attachInfo, Stream stream)
		{
			using (IClobDAO blobDAO = ClobDAOFactory.Instance.Create(_dataProvider))
			{				
				blobDAO.ReadAttachmentBlob(attachmentType, stream, attachInfo.ItemId, attachInfo.Name);				
			}
		}
	}
}