/*
* @(#)MessageInfo.cs
*
* Copyright (c) 2006 Newtera, Inc. All rights reserved.
*
*/
namespace Newtera.WFModel
{
	using System;

	/// <summary>
	/// A message model class.
	/// </summary>
	/// <version>1.0.0 22 Sep 2016 </version>
	public class MessageInfo
	{
        public string SenderName;
        public string Subject;
        public string Content;
        public string SendTime;
        public string Url;
        public string UrlParams;
        public string SchemaName;
        public string ClassName;
        public string ObjId;
	}
}