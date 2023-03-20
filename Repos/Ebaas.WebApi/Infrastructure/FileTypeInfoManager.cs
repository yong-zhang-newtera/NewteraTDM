/*
* @(#) FileTypeInfoManager.cs
*
* Copyright (c) 2003 Newtera, Inc. All rights reserved.
*
*/
namespace Ebaas.WebApi.Infrastructure
{
	using System;

	using Newtera.Common.Core;
	using Newtera.Server.DB;
	using Newtera.Common.MetaData.FileType;
	using Newtera.Server.Engine.Cache;

	/// <summary>
	/// Represent a singleton that provides File Type Information for attachments.
	/// </summary>
	/// <version> 1.0.0	29 Feb 2004</version>
	/// <author> Yong Zhang </author>
	public class FileTypeInfoManager
	{		
		// Static manager object, all invokers will use this manager object.
		private static FileTypeInfoManager theManager;
		
		private IDataProvider _dataProvider;
		private FileTypeInfoCollection _fileTypeInfos;
		private FileTypeInfo _unknownFileTypeInfo;

		/// <summary>
		/// Private constructor.
		/// </summary>
		private FileTypeInfoManager()
		{
			_dataProvider = DataProviderFactory.Instance.Create();
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

			string suffix = "";

			int pos = fileName.LastIndexOf('.');
			if (pos >= 0)
			{
				suffix = fileName.Substring(pos + 1);
			}

			if (_fileTypeInfos == null)
			{
				_fileTypeInfos = MetaDataCache.Instance.GetFileTypeInfo(_dataProvider);
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
				}

				
				if (found != null)
				{
					break;
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

			if (type != null)
			{
				if (_fileTypeInfos == null)
				{
					_fileTypeInfos = MetaDataCache.Instance.GetFileTypeInfo(_dataProvider);
				}

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
		/// Gets the FileTypeInfo object defined for the unknown suffix type
		/// </summary>
		/// <returns>An FileTypeInfo</returns>
		private FileTypeInfo GetUnknownFileTypeInfo()
		{
			if (_unknownFileTypeInfo == null)
			{
				// The first FileTypeInfo defined in a file must be the one for unknown file type
				if (MetaDataCache.Instance.GetFileTypeInfo(_dataProvider).Count > 0)
				{
					_unknownFileTypeInfo = (FileTypeInfo) MetaDataCache.Instance.GetFileTypeInfo(_dataProvider)[0];
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

		static FileTypeInfoManager()
		{
			// Initializing the factory.
			{
				theManager = new FileTypeInfoManager();
			}
		}
	}
}