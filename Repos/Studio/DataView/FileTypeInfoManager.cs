/*
* @(#) FileTypeInfoManager.cs
*
* Copyright (c) 2003 Newtera, Inc. All rights reserved.
*
*/
namespace Newtera.Studio
{
	using System;
	using System.IO;
	using System.Text;
	using System.Drawing;
	using System.Windows.Forms;
	using Microsoft.Web.Services;
	using Microsoft.Web.Services.Dime;

	using Newtera.Common.Core;
	using Newtera.Common.MetaData.FileType;
	using Newtera.WinClientCommon;

	/// <summary>
	/// Represent a singleton that provides File Type Information related services.
	/// </summary>
	/// <version> 1.0.0	16 Jan 2004</version>
	/// <author> Yong Zhang </author>
	public class FileTypeInfoManager
	{		
		// Static manager object, all invokers will use this manager object.
		private static FileTypeInfoManager theManager;
		
		private FileTypeInfoCollection _fileTypeInfos;
		private AttachmentServiceStub _attachmentService;
		private FileTypeInfo _unknownFileTypeInfo;

		/// <summary>
		/// Private constructor.
		/// </summary>
		private FileTypeInfoManager()
		{
			_fileTypeInfos = null;
			_attachmentService = new AttachmentServiceStub();
			_unknownFileTypeInfo = null;
		}

		/// <summary>
		/// Gets the FileTypeInfoManager instance.
		/// </summary>
		/// <returns> The FileTypeInfoManager instance.</returns>
		static public FileTypeInfoManager Instance
		{
			get
			{
				return theManager;
			}
		}
		
		/// <summary>
		/// Get a FileTypeInfo object based on the suffix of a file name
		/// </summary>
		/// <param name="fileName">The file name</param>
		/// <returns>A FileTypeInfo object that has a suffix being equals to the
		/// suffix of a provided file name. If it fails to find a matched FileTypeInfo object, the FileTypeInfo
		/// object for the unknown suffix is returned.</returns>
		public FileTypeInfo GetFileTypeInfoByName(string fileName)
		{
			FileTypeInfo found = null;

			if (_fileTypeInfos == null)
			{
				Initialize();
			}

			string suffix = "";

			int pos = fileName.LastIndexOf('.');
			if (pos >= 0)
			{
				suffix = fileName.Substring(pos + 1);
			}

			foreach (FileTypeInfo info in _fileTypeInfos)
			{
				foreach (FileSuffix obj in info.Suffixes)
				{
					if (obj.Suffix.ToUpper() == suffix.ToUpper())
					{
						found = info;
						break;
					}

					if (found != null)
					{
						break;
					}
				}
			}

			if (found == null)
			{
				found = GetUnknownFileTypeInfo();
			}

			return found;
		}

		/// <summary>
		/// Get a FileTypeInfo object based on the file type
		/// </summary>
		/// <param name="type">The file type</param>
		/// <returns>The found FileTypeInfo object. If it fails to find a matched FileTypeInfo object, the FileTypeInfo
		/// object for the unknown suffix is returned.</returns>
		public FileTypeInfo GetFileTypeInfoByType(string type)
		{
			FileTypeInfo found = null;

			if (_fileTypeInfos == null)
			{
				Initialize();
			}

			if (type != null)
			{
				foreach (FileTypeInfo info in _fileTypeInfos)
				{
					if (info.Type == type)
					{
						found = info;
						break;
					}
				}
			}

			if (found == null)
			{
				found = GetUnknownFileTypeInfo();
			}

			return found;
		}

		/// <summary>
		/// Load the images for small icons into the given image list
		/// </summary>
		/// <param name="imageList">The image list to which to load small images</param>
		public void LoadSmallImages(ImageList imageList)
		{
			if (_fileTypeInfos == null)
			{
				Initialize();
			}

			int index = 0;
			foreach (FileTypeInfo info in _fileTypeInfos)
			{
				if (info.SmallIconStream.Length > 0)
				{
					imageList.Images.Add(Image.FromStream(info.SmallIconStream));
				}
				else
				{
					imageList.Images.Add(Image.FromStream(GetUnknownFileTypeInfo().SmallIconStream));
				}
				info.ImageIndex = index;
				index++;
			}
		}

		/// <summary>
		/// Load the images for large icons into the given image list
		/// </summary>
		/// <param name="imageList">The image list to which to load small images</param>
		public void LoadLargeImages(ImageList imageList)
		{
			if (_fileTypeInfos == null)
			{
				Initialize();
			}

			foreach (FileTypeInfo info in _fileTypeInfos)
			{
				if (info.LargeIconStream.Length > 0)
				{
					imageList.Images.Add(Image.FromStream(info.LargeIconStream));
				}
				else
				{
					imageList.Images.Add(Image.FromStream(GetUnknownFileTypeInfo().LargeIconStream));
				}
			}
		}

		/// <summary>
		/// Gets the FileTypeInfo object defined for the unknown suffix type
		/// </summary>
		/// <returns>An FileTypeInfo</returns>
		private FileTypeInfo GetUnknownFileTypeInfo()
		{
			if (_unknownFileTypeInfo == null)
			{
				if (_fileTypeInfos.Count > 0)
				{
					// The first FileTypeInfo defined in a file must be the one for unknown file type
					_unknownFileTypeInfo = (FileTypeInfo) _fileTypeInfos[0];
				}
				else
				{
					_unknownFileTypeInfo = new FileTypeInfo();
					_unknownFileTypeInfo.Type = "unknown";
					_unknownFileTypeInfo.Description = "Unknown Type Document";
					_unknownFileTypeInfo.SmallIconPath = @"icons\sunknown.bmp";
					_unknownFileTypeInfo.LargeIconPath = @"icons\lunknown.bmp";
				}
			}


			return _unknownFileTypeInfo;
		}

		/// <summary>
		/// Load the file type information from server via a web service
		/// </summary>
		public void Initialize()
		{
			if (_fileTypeInfos == null)
			{
				_fileTypeInfos = new FileTypeInfoCollection();

				string xmlString = _attachmentService.GetFileTypeInfo();

                if (!string.IsNullOrEmpty(xmlString))
                {
                    // read the xml string for file type infos
                    StringReader reader = new StringReader(xmlString);
                    _fileTypeInfos.Read(reader);
                }
			}
		}

		/// <summary>
		/// Create a MemoryStream to store the image of icon from another stream
		/// </summary>
		/// <param name="filePath">The file path</param>
		/// <returns>A Stream object</returns>
		private Stream GetMemoryStream(Stream inStream)
		{
			MemoryStream outStream = new MemoryStream();
			BinaryWriter writer = new BinaryWriter(outStream);
			BinaryReader reader = new BinaryReader(inStream);
			int actual = 0;
			byte[] buffer = new byte[200];
			while((actual = reader.Read(buffer, 0/*buffer offset*/, buffer.Length/*count*/)) >0)
			{
				writer.Write(buffer, 0, actual); // a unicode counts two bytes
				writer.Flush();
			}

			return outStream;
		}

		static FileTypeInfoManager()
		{
			// Initializing the factory.
			{
				theManager = new FileTypeInfoManager();
			}
		}
	}
}