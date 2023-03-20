/*
* @(#)AttachmentLoader.cs
*
* Copyright (c) 2003 Newtera, Inc. All rights reserved.
*
*/
namespace Newtera.Studio.PackUnpack
{
	using System;
	using System.IO;
	using System.Xml;
	using System.Text;
	using System.Data;
	using System.Collections;
    using System.Collections.Specialized;

	using Microsoft.Web.Services.Dime;

	using Newtera.Common.Core;
	using Newtera.Common.MetaData;
	using Newtera.Common.MetaData.Schema;
	using Newtera.Common.Attachment;
    using Newtera.Common.MetaData.FileType;
	using Newtera.WinClientCommon;
    using Newtera.WindowsControl;

	/// <summary>
	/// Upload attachment files onto the database.
	/// </summary>
	/// <version> 1.0.0 24 Apr 2005 </version>
	/// <author> Yong Zhang</author>
	public class AttachmentLoader
	{
		private MetaDataModel _metaData;
		private string _attachmentDir;
		private Hashtable _objIdMappings;
		private WorkInProgressDialog _workingDialog;

		/// <summary>
		/// Instantiate an instance of AttachmentLoader class
		/// </summary>
		/// <param name="metaData">The Meta Data Model</param>
		/// <param name="attachmentDir">The directory where the attachment files reside.</param>
		/// <param name="objIdMappings">The mapping that translates an old obj_id into a new obj_id.</param>
		public AttachmentLoader(MetaDataModel metaData, string attachmentDir,
			Hashtable objIdMappings, WorkInProgressDialog workingDialog)
		{
			_attachmentDir = attachmentDir;
			_metaData = metaData;
			_objIdMappings = objIdMappings;
			_workingDialog = workingDialog;
		}

		/// <summary>
		/// Upload instance attachments onto the database.
		/// </summary>
		/// <remarks>The obj_ids contained in attachment.xml file represents the
		/// obj_ids in the backup database. since the instances are recreated in the
		/// new database, the obj_ids of the newly created instances are changed.
		/// therefore, we need to use the obj_id mappings to find the new obj_id
		/// using the old obj_id.</remarks>
		public void LoadInstanceAttachments(StreamWriter sw)
		{
		}

		/// <summary>
		/// Upload class attachments onto the database.
		/// </summary>
		/// <remarks>The clsId contained in attachment.xml file represents the
		/// class id in the backup database. since the classes are recreated in the
		/// new database, the class ids of the newly created classes are changed.
		/// therefore, we need to use the classId mappings to find the new classId
		/// using the old classId.</remarks>
		public void LoadClassAttachments(StreamWriter sw)
		{
		}

        /// <summary>
        /// Upload instance images onto the database.
        /// </summary>
        /// <remarks>The obj_ids contained in image file names represents the
        /// obj_ids in the backup database. since the instances are recreated in the
        /// new database, the obj_ids of the newly created instances are changed.
        /// therefore, we need to use the obj_id mappings to replace the old obj_id with the new one.</remarks>
        public void LoadImages(StreamWriter sw)
        {
        }

        /// <summary>
        /// Gets the type of a file based on its suffix
        /// </summary>
        /// <param name="fileName">The file name</param>
        /// <returns>A string represents a type</returns>
        private string GetMIMEType(string fileName)
        {
            FileTypeInfo fileTypeInfo = FileTypeInfoManager.Instance.GetFileTypeInfoByName(fileName);

            return fileTypeInfo.Type;
        }
	}
}