/*
* @(#)IAttributeGetter.cs
*
* Copyright (c) 2005 Newtera, Inc. All rights reserved.
*
*/
namespace Newtera.Common.MetaData.Mappings.Transform
{
	using System;
	using System.Xml;
	using System.Data;

	/// <summary>
	/// Represents a interface for a getter that gets a string value
	/// from a source attribute.
	/// </summary>
	/// <version> 1.0.0 20 Jan 2005</version>
	/// <author>  Yong Zhang </author>
	public interface IAttributeGetter
	{
		/// <summary>
		/// Gets the type of getter
		/// </summary>
		/// <value>One of GetterType enum values</value>
		GetterType Type { get; }

		/// <summary>
		/// Get a value from a source attribute.
		/// </summary>
		string GetValue();
	}
}