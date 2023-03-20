/*
* @(#)AttachmentListViewItem.cs
*
* Copyright (c) 2003 Newtera, Inc. All rights reserved.
*
*/
namespace Newtera.Studio
{
	using System;
	using System.Windows.Forms;

	using Newtera.Common.Attachment;
	
	/// <summary>
	/// Represents a ListView item for an AttachmentInfo instance
	/// </summary>
	/// <version>  1.0.1 10 Jan 2004</version>
	/// <author>  Yong Zhang</author>
	public class AttachmentListViewItem : ListViewItem
	{
		private AttachmentInfo _info;

		/// <summary>
		/// Initializes a new instance of the AttachmentListViewItem class.
		/// </summary>
		/// <param name="info">An AttachmentInfo object</param>
		public AttachmentListViewItem(AttachmentInfo info) : base(info.Name)
		{
			_info = info;
		}

		/// <summary> 
		/// Gets or sets the AttachmentInfo object represented by the list item.
		/// </summary>
		/// <value> An AttachmentInfo</value>
		public AttachmentInfo AttachmentInfo
		{
			get
			{
				return _info;
			}
		}
	}
}