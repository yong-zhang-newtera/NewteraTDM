/*
* @(#)ConfigKeyValueCollection.cs
*
* Copyright (c) 2005 Newtera, Inc. All rights reserved.
*
*/
namespace Newtera.Common.Config
{
	using System;
	using System.Collections.Specialized;

	/// <summary> 
	/// A writable key/value collection for config
	/// </summary>
	/// <version> 1.0.0 03 Dec 2005 </version>
	/// <author>Yong Zhang</author>
	/// <remarks>
	/// Since NameValueCollection generated by ConfigurationSettings is read only
	/// we provide this writable version.
	/// </remarks>
	public class ConfigKeyValueCollection : NameValueCollection
	{
		/// <summary>
		/// Initiate an instance of ConfigKeyValueCollection
		/// </summary>
		/// <remarks>It will locate the config file automatically</remarks>
		public ConfigKeyValueCollection() : base()
		{
			this.IsReadOnly = false;
		}
	}
}