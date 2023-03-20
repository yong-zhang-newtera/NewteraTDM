/*
* @(#)AttachmentTypeEnum.cs
*
* Copyright (c) 2003 Newtera, Inc. All rights reserved.
*
*/
namespace Newtera.Common.Attachment
{
	/// <summary>
	/// Specify the types of attachment
	/// </summary>
	public enum AttachmentType
	{
		/// <summary>
		/// The Unknown node type
		/// </summary>
		Unknown,
		/// <summary>
		/// The instances's attachment
		/// </summary>
		Instance,
		/// <summary>
		/// The class' attachment.
		/// </summary>
		Class,
        /// <summary>
        /// The image that is attached to an instance's image attribute
        /// </summary>
        Image
	}
}