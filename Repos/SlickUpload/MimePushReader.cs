using System;
using System.Collections;
using System.IO;
using System.Text;

namespace Newtera.HttpUploadManager
{
	/// <summary>
	/// Summary description for MimePushReader.
	/// </summary>
	internal sealed class MimePushReader
	{
		Stream stream;
		IMimePushHandler handler;
		byte[] boundary;
		Encoding _contentEncoding;

		enum MimeReaderState
		{
			ReadingHeaders,
			ReadingBody,
			CheckingEnd,
			Finished
		}

		public MimePushReader(Stream s, byte[] b, IMimePushHandler h, Encoding contentEncoding)
		{
			stream = s;
			handler = h;
			boundary = b;
			_contentEncoding = contentEncoding;
		}

		public void Parse()
		{
			MimeReaderState state = MimeReaderState.ReadingHeaders;
			MimeHeaderReader headerReader = new MimeHeaderReader(_contentEncoding);
			byte[] buffer = new byte[8192];

			// Read the first chunk
			int read = stream.Read(buffer, 0, 8192);

			// Skip past the first boundary
			int position = SearchForBoundary(buffer, boundary, 0, read) + boundary.Length;

			// Skip past the end line
			if ((char)buffer[position] == '\r')
				position += 2;
			else if ((char)buffer[position] == '\n')
				position += 1;

			while (read > 0 && state != MimeReaderState.Finished)
			{
				switch (state)
				{
					case MimeReaderState.ReadingHeaders:
						int bytesRead;
						
						bytesRead = headerReader.Read(buffer, position);

						position += bytesRead;

						if (headerReader.HeaderComplete)
						{
							state = MimeReaderState.ReadingBody;

							handler.BeginPart(headerReader.Headers);

							headerReader.Reset();
						}

						break;
					case MimeReaderState.ReadingBody:
						int boundaryPos = SearchForBoundary(buffer, boundary, position, read);

						// If boundary not found
						if (boundaryPos == -1)
						{
							// If it's a full chunk
							if (position == 0)
							{
								handler.PartData(ref buffer);
							}
							else
							{
								// Copy the data to a temp buffer that is the right size
								byte[] tempBuff = new byte[read - position];

								//for (int i = 0; i < tempBuff.Length; i++)
								//	tempBuff[i] = buffer[position + i];
								Buffer.BlockCopy(buffer, position, tempBuff, 0, tempBuff.Length);

								handler.PartData(ref tempBuff);
							}

							position += read - position;
						} 
						else
						{
							int actualLength;

							actualLength = boundaryPos - position;

							if ((char)buffer[boundaryPos - 2] == '\r')
								actualLength -= 2;
							else if ((char)buffer[boundaryPos - 2] == '\n')
								actualLength -= 1;

							// Copy the data to a temp buffer that is the right size
							byte[] tempBuff = new byte[actualLength];

							Buffer.BlockCopy(buffer, position, tempBuff, 0, tempBuff.Length);
							//for (int i = 0; i < tempBuff.Length; i++)
							//	tempBuff[i] = buffer[position + i];

							handler.PartData(ref tempBuff);

							// If there is no lapping over
							if (boundaryPos < buffer.Length - boundary.Length)
							{
								// Check for end
								if (boundaryPos < buffer.Length - (boundary.Length + 2))
								{
									// Check for "--" which means last part
									bool isLast = (Encoding.ASCII.GetString(buffer, boundaryPos + boundary.Length, 2) == "--");

									if (isLast)
									{
										state = MimeReaderState.Finished;
									}
									else
									{
										state = MimeReaderState.ReadingHeaders;
									}
								
									handler.EndPart(isLast);

									position += (boundaryPos + boundary.Length - position + 2);
								}
								// Laps over
								else
								{
									state = MimeReaderState.CheckingEnd;

									// If there is one character left, move it
									if (boundaryPos + 2 - buffer.Length == 1)
									{
										boundaryPos = 1;

										buffer[0] = buffer[buffer.Length - 1];
									}
									else
									{
										boundaryPos = 0;
									}

									// Read in another chunk
									read = stream.Read(buffer, boundaryPos, buffer.Length - boundaryPos);

									position = 0;
								}
							}
							// Boundary laps over
							else
							{
								// Also take the possible cr/lf combo removed above
								boundaryPos -= ((boundaryPos - position) - actualLength);

								// Move the boundary data to the front of the buffer
								Buffer.BlockCopy(buffer, boundaryPos, buffer, 0, buffer.Length - boundaryPos);
								//for (int i = 0; i < buffer.Length - boundaryPos; i++)
								//	buffer[i] = buffer[boundaryPos + i];
								
								// Load in more data
								read = stream.Read(buffer, buffer.Length - boundaryPos, buffer.Length - (buffer.Length - boundaryPos));

								position = 0;
							}
						}

						break;
					case MimeReaderState.CheckingEnd:
						if (Encoding.ASCII.GetString(buffer, 0, 2) == "--")
						{
							state = MimeReaderState.Finished;
						}
						else
						{
							position += 2;

							state = MimeReaderState.ReadingHeaders;
						}

						handler.EndPart(state == MimeReaderState.Finished);

						break;
				}

				// Get more data if we need it
				if (state != MimeReaderState.Finished)
				{
					if (position >= buffer.Length || position >= read)
					{
						read = stream.Read(buffer, 0, buffer.Length);

						position = 0;
					}
				}
			}
		}

		int SearchForBoundary(byte[] buff, byte[] boundary, int offset, int count)
		{
			int matchStart = -1;
			int matched = 0;

			// Search through the buffer byte by byte
			for (int i = offset; i < count; i++)
			{
				// If the buffer starts matching the boundary
				if (buff[i] == boundary[matched])
				{
					// Continue matching
					if (matchStart == -1)
						matchStart = i;

					matched++;

					// If we matched the whole thing, we're done
					if (matched == boundary.Length)
						break;
				} 
				else
				{
					matchStart = -1;
					matched = 0;
				}
			}

			return matchStart;
		}
	}
}