using System;
using System.Configuration;
using System.IO;

namespace Newtera.HttpUploadManager
{
	/// <summary>
	/// Provides information about a file that has been uploaded.
	/// </summary>
	/// <remarks>
	/// All files are streamed to disk as they arrive. Each UploadedFile instance contains
	/// information about a single uploaded file, as well as methods that manipulate it.
	/// The <see cref="UploadedFileCollection"/> provides a collection of all of the files
	/// uploaded in a given request.
	/// </remarks>
	public sealed class UploadedFile
	{
		const string TEMP_DIR = "temp";

		string _clientName;
		string _clientPath;
		string _serverPath;
		string _contentType;
		long _contentLength;
		string _attachmentId;

		#region Properties
		/// <summary>
		/// Gets the file name of the uploaded file as it was on the client machine.
		/// </summary>
		public string ClientName
		{
			get
			{
				return _clientName;
			}
		}

		/// <summary>
		/// Gets the entire path of where the uploaded file was located on the client machine.
		/// </summary>
		public string ClientPath
		{
			get
			{
				return _clientPath;
			}
		}

		/// <summary>
		/// Gets the path to the temporary file on the server in which the uploaded file data is stored.
		/// </summary>
		public string ServerPath
		{
			get
			{
				return _serverPath;
			}
		}

		/// <summary>
		/// Gets the MIME content type of the uploaded file.
		/// </summary>
		public string ContentType
		{
			get
			{
				return _contentType;
			}
		}

		/// <summary>
		/// Gets the length (in bytes) of the uploaded file.
		/// </summary>
		public long ContentLength
		{
			get
			{
				return _contentLength;
			}
		}

		/// <summary>
		/// Gets or sets the id of attachment that the uploaded file represents
		/// </summary>
		public string AttachmentId
		{
			get
			{
				return _attachmentId;
			}
			set
			{
				_attachmentId = value;
			}
		}

		#endregion

		internal UploadedFile(string clientPath, string contentType, string serverPath)
		{
			_serverPath = GetTempFileName(serverPath);
			_clientPath = clientPath;
			_contentType = contentType;

			// TODO: ensure this is cross-platform compatible
			_clientName = Path.GetFileName(_clientPath);
		}

		/// <summary>
		/// Saves the uploaded file to a given path on the server, then deletes the temporary file.
		/// </summary>
		/// <param name="path">The path to which to save the file.</param>
		public void SaveAs(string path)
		{
			SaveAs(path, false);
		}

		/// <summary>
		/// Saves the uploaded file to a given path on the server, then deletes the temporary file.
		/// A boolean passed in determines whether to silently overwrite the destination file.
		/// </summary>
		/// <param name="path">The path to which to save the file.</param>
		/// <param name="overwrite">A boolean determining whether to overwrite the file silently if it exists.</param>
		public void SaveAs(string path, bool overwrite)
		{
			if (overwrite)
				File.Delete(path);

			File.Move(_serverPath, path);
		}

        public void DeleteTempFile()
        {
            if (!string.IsNullOrEmpty(_serverPath) && File.Exists(_serverPath))
            {
                File.Delete(_serverPath);
            }
        }

		internal void SetContentLength(long contentLength)
		{
			_contentLength = contentLength;
		}

		internal string GetTempFileName(string serverPath)
		{
			string path = serverPath + UploadedFile.TEMP_DIR;			
			// Determine whether the directory exists.
			if (!Directory.Exists(path)) 
			{
				// Create the directory.
				Directory.CreateDirectory(path);
			}

			path = Path.Combine(path, Guid.NewGuid().ToString("B"));

			return path;
		}
	}
}