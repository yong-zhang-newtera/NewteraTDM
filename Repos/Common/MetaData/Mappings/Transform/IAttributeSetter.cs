/*
* @(#)IAttributeSetter.cs
*
* Copyright (c) 2004 Newtera, Inc. All rights reserved.
*
*/
namespace Newtera.Common.MetaData.Mappings.Transform
{
	using System;
	using System.Xml;
	using System.Data;

	/// <summary>
	/// Represents a interface for a setter that sets a string value
	/// to a destination attribute.
	/// </summary>
	/// <version> 1.0.0 16 Nov 2004</version>
	/// <author>  Yong Zhang </author>
	public interface IAttributeSetter
	{
		/// <summary>
		/// Gets the type of setter
		/// </summary>
		/// <value>One of SetterType enum values</value>
		SetterType Type { get; }

		/// <summary>
		/// Assign a value to an attribute.
		/// </summary>
		void AssignValue();
	}
}