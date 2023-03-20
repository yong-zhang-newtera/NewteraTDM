/*
* @(#)IJoinElement.cs
*
* Copyright (c) 2003 Newtera, Inc. All rights reserved.
*
*/
namespace Newtera.Server.Engine.Sqlbuilder.Sql
{
	using System;

	/// <summary>
	/// Represents a common interface for join elements including inner join and outer join
	/// of SQLElememnt.
	/// </summary>
	/// <version> 1.0.0 09 Jun. 2005</version>
	/// <author>  Yong Zhang </author>
	public interface IJoinElement
	{
		/// <summary>
		/// Gets the information indicating whether the join chain contains a
		/// class with the provided alias.
		/// </summary>
		/// <param name="alias">The alias</param>
		/// <returns>True if the join chain contains it, false otherwise.</returns>
		bool ContainsAlias(string alias);	
	}
}