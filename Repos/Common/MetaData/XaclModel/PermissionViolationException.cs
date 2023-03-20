/*
* @(#)PermissionViolationException.cs
*
* Copyright (c) 2003 Newtera, Inc. All rights reserved.
*
*/
namespace Newtera.Common.MetaData.XaclModel
{
	using System;

	/// <summary> 
	/// The PermissionViolationException class is thrown when there is a permission
	/// violation.
	/// </summary>
	/// <version> 1.0.0 14 Dec 2003</version>
	/// <author> Yong Zhang </author>
	public class PermissionViolationException : XaclException
	{
		/// <summary>
		/// Initiate an instance of PermissionViolationException
		/// </summary>
		/// <param name="reason">The reason of the exception</param>
		public PermissionViolationException(string reason) : base(reason)
		{
		}

		/// <summary>
		/// Initiate an instance of PermissionViolationException
		/// </summary>
		/// <param name="actionType"></param>
		/// <param name="objectName"></param>
		public PermissionViolationException(XaclActionType actionType, string objectName) : base("")
		{
			string msg = "Do not have permission to ";
			switch (actionType)
			{
				case XaclActionType.Read:
					msg += "read";
					break;
				case XaclActionType.Write:
					msg += "write";
					break;
				case XaclActionType.Create:
					msg += "create";
					break;
				case XaclActionType.Delete:
					msg += "delete";
					break;
				case XaclActionType.Upload:
					msg += "upload";
					break;
				case XaclActionType.Download:
					msg += "download";
					break;
			}

			msg += " " + objectName;

			this.LocalizedMessage = msg;
		}
	}
}