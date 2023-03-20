using System;
using System.IO;
using System.Web;

namespace Newtera.HttpUploadManager
{
	/// <summary>
	/// Summary description for RequestStream.
	/// </summary>
	internal class RequestStream : Stream
	{
		protected HttpWorkerRequest request;
		protected long _position;

		byte[] tempBuff;
		bool isInPreloaded = true;

		#region Properties
		public override bool CanRead
		{
			get
			{
				return true;
			}
		}

		public override bool CanSeek
		{
			get
			{
				return false;
			}
		}

		public override bool CanWrite
		{
			get
			{
				return false;
			}
		}

		public override long Position
		{
			get
			{
				return _position;
			}
			set
			{
				throw new NotImplementedException();
			}
		}

		public override long Length
		{
			get
			{
				return long.Parse(request.GetKnownRequestHeader(HttpWorkerRequest.HeaderContentLength));
			}
		}
		#endregion

		public RequestStream(HttpWorkerRequest request)
		{
			this.request = request;

			tempBuff = request.GetPreloadedEntityBody();
		}

		public override void Flush()
		{
			throw new NotImplementedException();	
		}

		public override int Read(byte[] buffer, int offset, int count)
		{
			int read = 0;

			if (isInPreloaded)
			{
				read = ReadPreloaded(buffer, offset, count);

				// If this request laps over, read the rest from the normal body
				if (read < count)
				{
					if (request.IsClientConnected() &&
						!request.IsEntireEntityBodyIsPreloaded())
					{
						read += ReadNormal(buffer, read, count - read);
					}

					isInPreloaded = false;
				}
			}
			else if (request.IsClientConnected() &&
				!request.IsEntireEntityBodyIsPreloaded()
				)
			{
				read = ReadNormal(buffer, offset, count);
			}
			/*else
			{
				read = 0;
			}*/

			_position += read;

			if (read == 0)
				throw new DisconnectedException();

			return read;
		}

		int ReadPreloaded(byte[] buffer, int offset, int count)
		{
			int read;

			if (_position + count < tempBuff.Length)
				read = count;
			else
				read = tempBuff.Length - (int)_position;

			Buffer.BlockCopy(tempBuff, (int)_position, buffer, offset, read);
			//for (int i = 0; i < read; i++)
			//	buffer[offset + i] = tempBuff[_position + i];

			return read;
		}

		int ReadNormal(byte[] buffer, int offset, int count)
		{
			if (_position + count > Length)
				count = (int)(Length - _position);

			if (offset > 0)
			{
				// Allocate a bigger buffer if necessary
				if (count > tempBuff.Length)
					tempBuff = new byte[count];

				int read;
				
				read = request.ReadEntityBody(tempBuff, count);

				Buffer.BlockCopy(tempBuff, 0, buffer, offset, read);
				//for (int i = 0; i < read; i++)
				//	buffer[offset + i] = tempBuff[i];

				return read;
			}
			else
			{
				return request.ReadEntityBody(buffer, count);
			}
		}

		public override long Seek(long offset, SeekOrigin origin)
		{
			throw new NotImplementedException();
		}

		public override void SetLength(long value)
		{
			throw new NotImplementedException();	
		}

		public override void Write(byte[] buffer, int offset, int count)
		{
			throw new NotImplementedException();
		}
	}
}