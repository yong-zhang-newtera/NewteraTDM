/*
* @(#)WorkflowDataCacheEntry.cs
*
* Copyright (c) 2006 Newtera, Inc. All rights reserved.
*
*/
namespace Newtera.Server.DB
{
	using System;
	using Newtera.Common.Core;

	/// <summary>
	/// Contains workflow data of various types, such as xoml, rules, layout, and code
	/// </summary>
	/// <version>  	1.0.0 16 Dec 2006</version>
	public class WorkflowDataCacheEntry
	{
        public string Xoml = null; // xoml data
        public string Rules = null; // rules data
        public string Layout = null; // Layout
        public string Code = null; // code
	}
}