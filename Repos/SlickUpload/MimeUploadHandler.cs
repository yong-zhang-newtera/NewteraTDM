using System;
using System.Collections.Specialized;
using System.IO;
using System.Text;
using System.Configuration;

namespace Newtera.HttpUploadManager
{
	/// <summary>
	/// Summary description for MultipartParser.
	/// </summary>
	internal class MimeUploadHandler : IMimePushHandler
	{
		protected Stream _s;
		protected byte[] _boundary;
		protected UploadStatus _uploadStatus;
		protected StringBuilder _textParts;
		protected Encoding _contentEncoding;
		protected string _serverPath;

		//UploadedFile currentFile;
		FileStream currentStream;

		protected UploadedFileCollection _uploadedFiles;

		#region Properties
		public long ContentLength
		{
			get
			{
				return _s.Length;
			}
		}

		public int Progress
		{
			get
			{
				return (int)_s.Position;
			}
		}

		public UploadedFileCollection UploadedFiles
		{
			get
			{
				return _uploadedFiles;
			}
		}
		
		public bool IsComplete
		{
			get
			{
				return Progress >= ContentLength;
			}
		}

		public UploadStatus UploadStatus
		{
			get
			{
				return _uploadStatus;
			}
		}

		public string TextParts
		{
			get
			{
				return _textParts.ToString();
			}
		}
		#endregion

		public MimeUploadHandler(Stream s, byte[] boundary, string uploadId,
			Encoding contentEncoding,
			string serverPath)
		{
			_s = s;
			_boundary = boundary;
			_uploadStatus = new UploadStatus(ContentLength, uploadId);
			_contentEncoding = contentEncoding;
			_serverPath = serverPath;
		}

		public void Parse()
		{
			_uploadedFiles = new UploadedFileCollection();
			_textParts = new StringBuilder();

			MimePushReader reader = new MimePushReader(_s, _boundary, this, _contentEncoding);

			try
			{
				reader.Parse();
			}
			catch (DisconnectedException)
			{
				if (currentStream != null)
					currentStream.Close();

				throw;
			}
		}

		string GetUploadFileName(NameValueCollection headers)
		{
			string[] dispositionParts = headers["content-disposition"].Split(';');
			string fileName;

			if (dispositionParts.Length > 2)
			{
				fileName = dispositionParts[2].Split('=')[1];
			}
			else
			{
				return null;
			}


			if (fileName != "\"\"")
			{
				fileName = fileName.Replace("\"", string.Empty);

				return fileName;
			}
			else
			{
				return null;
			}
		}

		#region IMimePushHandler Members
		public void BeginPart(NameValueCollection headers)
		{
			string fileName = GetUploadFileName(headers);

			if (fileName != null)
			{
				// Create a part for the file
				UploadedFile file = new UploadedFile(fileName, headers["content-type"], _serverPath);

				UploadedFiles.Add(file);
					
				// Create a stream for the file
				currentStream = File.Create(file.ServerPath);
			}
			else
			{
				// Write out the boundary start
				_textParts.Append(Encoding.ASCII.GetString(_boundary) + "\r\n");

				// Write out the headers as textparts
				for (int i = 0; i < headers.Count; i++)
				{
					_textParts.Append(headers.Keys[i] + ": " + headers[i] + "\r\n");
				}

				_textParts.Append("\r\n");

				//currentPart = null;
				//currentStream = null;
			}
		}

		public void PartData(ref byte[] data)
		{
			// If it's a UploadedFile
			if (currentStream != null)
			{
				currentStream.Write(data, 0, data.Length);
			}
			else
			{
				_textParts.Append(Encoding.ASCII.GetString(data));
			}

			_uploadStatus.SetPosition(_s.Position);
		}

		public void EndPart(bool isLast)
		{
			if (currentStream != null)
			{
				// Set the content length of the file
				UploadedFiles[UploadedFiles.Count - 1].SetContentLength(currentStream.Length);

				currentStream.Close();
				currentStream = null;
			}
			else
			{
				_textParts.Append("\r\n");
			}

			if (isLast)
			{
				if (_textParts.Length > 0)
				{
					// Write out the boundary end
					_textParts.Append(Encoding.ASCII.GetString(_boundary) + "--\r\n\r\n");
				}
			}
		}
		#endregion
	}
}