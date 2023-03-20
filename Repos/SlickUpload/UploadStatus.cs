using System;

namespace Newtera.HttpUploadManager
{
	/// <summary>
	/// Contains information about the current status of an upload. The properties are updated
	/// in real time as the upload progresses.
	/// </summary>
	public sealed class UploadStatus
	{
		long _contentLength;
		long _position;
		DateTime _start;
		string _uploadId;

		#region Properties
		/// <summary>
		/// Gets the length of the entire upload data.
		/// </summary>
		public long ContentLength
		{
			get
			{
				return _contentLength;
			}
		}

		/// <summary>
		/// Gets the current position in the upload.
		/// </summary>
		public long Position
		{
			get
			{
				return _position;
			}
		}
		
		/// <summary>
		/// Returns a boolean that indicates whether the upload is complete.
		/// </summary>
		public bool IsComplete
		{
			get
			{
				return Position >= ContentLength;
			}
		}

		/// <summary>
		/// Returns a DateTime representing the moment when the upload started.
		/// </summary>
		public DateTime Start
		{
			get
			{
				return _start;
			}
		}
		
		internal string UploadId
		{
			get
			{
				return _uploadId;
			}
		}

		#endregion

		internal UploadStatus(long contentLength, string uploadId)
		{
			// TODO: make this all threadable
			_contentLength = contentLength;
			_start = DateTime.Now;
			_uploadId = uploadId;
		}

		internal void SetPosition(long position)
		{
			_position = position;
		}
	}
}
