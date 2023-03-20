using System;
using System.Collections.Specialized;

namespace Newtera.HttpUploadManager
{
	/// <summary>
	/// Summary description for IMimePushHandler.
	/// </summary>
	internal interface IMimePushHandler
	{
		void BeginPart(NameValueCollection headers);
		void PartData(ref byte[] data);
		void EndPart(bool isLast);
	}
}
