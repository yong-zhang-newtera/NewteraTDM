/*
* @(#)AttachmentLoader.cs
*
* Copyright (c) 2013 Newtera, Inc. All rights reserved.
*
*/
namespace Newtera.WebApi.Utils
{
	using System;
	using System.IO;
	using System.Xml;
	using System.Text;
	using System.Data;
	using System.Collections;
    using System.Collections.Specialized;

	using Newtera.Common.Core;
	using Newtera.Common.MetaData;
	using Newtera.Common.MetaData.Schema;
	using Newtera.Common.Attachment;
    using Newtera.Common.MetaData.FileType;
    using Newtera.Data;
    using Newtera.WebApi.Infrastructure;

    /// <summary>
    /// Upload attachment files onto the database.
    /// </summary>
    /// <version> 1.0.0 11 Jan 2013 </version>
    public class AttachmentLoader
	{
		private MetaDataModel _metaData;
		private string _attachmentDir;
		private Hashtable _objIdMappings;
        private CMConnection _connection;

		/// <summary>
		/// Instantiate an instance of AttachmentLoader class
		/// </summary>
		/// <param name="metaData">The Meta Data Model</param>
		/// <param name="attachmentDir">The directory where the attachment files reside.</param>
		/// <param name="objIdMappings">The mapping that translates an old obj_id into a new obj_id.</param>
		public AttachmentLoader(CMConnection connection, MetaDataModel metaData, string attachmentDir,
			Hashtable objIdMappings)
		{
            _connection = connection;
			_attachmentDir = attachmentDir;
			_metaData = metaData;
			_objIdMappings = objIdMappings;
		}

		/// <summary>
		/// Upload instance attachments onto the database.
		/// </summary>
		/// <remarks>The obj_ids contained in attachment.xml file represents the
		/// obj_ids in the backup database. since the instances are recreated in the
		/// new database, the obj_ids of the newly created instances are changed.
		/// therefore, we need to use the obj_id mappings to find the new obj_id
		/// using the old obj_id.</remarks>
		public void LoadInstanceAttachments()
		{
			// read attachment infos from attachments.xml file
            if (File.Exists(_attachmentDir + @"\attachments.xml"))
            {
                AttachmentInfoCollection infos = new AttachmentInfoCollection();
                infos.Read(_attachmentDir + @"\attachments.xml");
                string newObjId;
                string sourceAttachmentFile;
                string newAttachmentId;

                foreach (AttachmentInfo info in infos)
                {
                    newObjId = (string)_objIdMappings[info.ItemId];

                    if (newObjId == null)
                    {
                        continue;
                    }

                    try
                    {
                        newAttachmentId = AddAttachmentInfo(AttachmentType.Instance,
                            newObjId,
                            info.ClassName,
                            info.Name,
                            info.Type,
                            info.Size,
                            info.IsPublic
                            );

                        // if the backup have packed the attachment files, upload the files
                        sourceAttachmentFile = _attachmentDir + @"\" + info.ID;

                        if (File.Exists(sourceAttachmentFile))
                        {
                            AttachmentInfo newAttachmentInfo = GetAttachmentInfo(AttachmentType.Instance, newAttachmentId);

                            // copy the attachment file to a specified sun directory
                            string actualAttachmentDir = NewteraNameSpace.GetAttachmentSubDir(newAttachmentInfo.CreateTime);
                            string targerAttachmentFile = actualAttachmentDir + newAttachmentId;
                            if (File.Exists(targerAttachmentFile))
                            {
                                File.Delete(targerAttachmentFile);
                            }

                            File.Copy(sourceAttachmentFile, targerAttachmentFile);
                        }
                    }
                    catch (Exception ex)
                    {
                        // TODO, uploading an attachment failed
                        /*
                        sw.WriteLine("************ Message Begin ***********");
                        sw.WriteLine(ex.Message);
                        sw.WriteLine("************ Message End *************");
                        sw.WriteLine("");
                         */
                    }
                    finally
                    {
                    }
                }
            }
		}

        /// <summary>
        /// Upload instance images onto the database.
        /// </summary>
        /// <remarks>The obj_ids contained in image file names represents the
        /// obj_ids in the backup database. since the instances are recreated in the
        /// new database, the obj_ids of the newly created instances are changed.
        /// therefore, we need to use the obj_id mappings to replace the old obj_id with the new one.</remarks>
        public void LoadImages()
        {
            // read image names from images.xml file
            string imageNamesDoc = _attachmentDir + @"\images.xml";
            if (File.Exists(imageNamesDoc))
            {
                StringCollection imageFileIds = new StringCollection();
                try
                {
                    //Open the stream and read XSD from it.
                    using (FileStream fs = File.OpenRead(imageNamesDoc))
                    {
 
                        XmlDocument doc = new XmlDocument();

                        doc.Load(fs);

                        if (doc.DocumentElement != null)
                        {
                            foreach (XmlElement child in doc.DocumentElement)
                            {
                                if (!string.IsNullOrEmpty(child.InnerText))
                                {
                                    imageFileIds.Add(child.InnerText);
                                }
                            }
                        }
                    }
                }
                catch (Exception e)
                {
                    throw new AttachmentException(e.Message, e);
                }
                
                string newObjId;
                string oldObjId;
                string fileName;
                string newImageId = null;
                string attributeName;
                string className;
                string suffix;

                foreach (string imageId in imageFileIds)
                {
                    NewteraNameSpace.GetImageInfo(imageId, out oldObjId, out attributeName, out className, out suffix);
                    newObjId = (string)_objIdMappings[oldObjId];

                    if (newObjId == null)
                    {
                        continue;
                    }

                    try
                    {
                        // delete the old image id from database and file indicated by old image id
                        DeleteImage(newObjId, attributeName,
                                            className,
                                            imageId);

                        fileName = _attachmentDir + @"\" + imageId;
                        // infer the mime type from the suffix of a file name
                        string type = GetMIMEType(fileName);

                        if (File.Exists(fileName))
                        {
                            AttachmentInfo info = new AttachmentInfo();
                            info.ItemId = newObjId;
                            info.AttributeName = attributeName;
                            info.ClassName = className;
                            info.Name = fileName;
                            info.Type = type;

                            CMCommand cmd = _connection.CreateCommand();
                            newImageId = cmd.AddAttachmentInfo(AttachmentType.Image, info);

                            string imageFile = _attachmentDir + @"\" + imageId;
                            

                            // since the image file is stored in an actual attachment directory which can be changed
                            // by web.config, we need to copy the image to the temp image directory
                            // Note: images file are stored at the base directory, unlike attachment files
                            string actualAttachmentDir = NewteraNameSpace.GetAttachmentDir();
                            string newImageFile = actualAttachmentDir + newImageId;
                            if (File.Exists(newImageFile))
                            {
                                File.Delete(newImageFile);
                            }

                            File.Copy(imageFile, newImageFile);
                        }
                    }
                    catch (Exception ex)
                    {
                        /*
                        sw.WriteLine("************ Message Begin ***********");
                        sw.WriteLine(ex.Message);
                        sw.WriteLine("************ Message End *************");
                        sw.WriteLine("");
                         */
                    }
                    finally
                    {
                    }
                }
            }
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
        private string AddAttachmentInfo(AttachmentType attachmentType, string itemId,
            string className, string name, string type, long size, bool isPublic)
        {
            AttachmentInfo info = new AttachmentInfo();
            info.ItemId = itemId;
            info.ClassName = className;
            info.Name = name;
            info.Type = type;
            info.Size = size;
            info.IsPublic = isPublic;

            CMCommand cmd = _connection.CreateCommand();
            string aid = cmd.AddAttachmentInfo(attachmentType, info);

            return aid;
        }

        /// <summary>
        /// Get an attachment info given an attachement id
        /// </summary>
        /// <param name="attachmentType">One of the AttachmentType enum values</param>
        /// <param name="attachmentId">The unique attachment id</param>
        /// <returns>A attachment info object</returns>
        private AttachmentInfo GetAttachmentInfo(AttachmentType attachmentType, string attachmentId)
        {
            CMCommand cmd = _connection.CreateCommand();
            AttachmentInfo info = cmd.GetAttachmentInfo(attachmentType, attachmentId);

            return info;
        }

        /// <summary>
        /// Delete an image and clear value of corresponding image column.
        /// </summary>
        /// <param name="instanceId">The id of an instance to which to add the attachment.</param>
        /// <param name="attributeName">The name of image attribute.</param>
        /// <param name="className">The name of class that the instance belongs to.</param>
        /// <param name="imageId">The image image.</param>
        private void DeleteImage(string instanceId, string attributeName, string className,
            string imageId)
        {
            AttachmentInfo info = new AttachmentInfo();
            info.ItemId = instanceId;
            info.AttributeName = attributeName;
            info.ClassName = className;
            info.ID = string.Empty; // clear the value

            CMCommand cmd = _connection.CreateCommand();
            cmd.DeleteAttachment(AttachmentType.Image, info);

            // delete the image file
            // Note: image files are stored at the base dir, unlike attachment files
            string path = NewteraNameSpace.GetAttachmentDir() + imageId;
            try
            {
                File.Delete(path);
            }
            catch (Exception)
            {
            }
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