/*
* @(#)ObjectCaller.cs
*
* Copyright (c) 2003 Newtera, Inc. All rights reserved.
*
*/
namespace Newtera.Server.Engine.Sqlbuilder
{
	using System;
	
	/// <summary>
	/// Calls an object and returns a value.
	/// 
	/// Originally created to deal with PrettyPrint problem; PrettyPrint
	/// was calling toString() on complex objects.  So I would print a
	/// Vector of these objects and get a ton of output; all I really
	/// wanted was their names.
	/// 
	/// The solution is to use an ObjectCaller to generate a string.
	/// If I want full output, I write an ObjectCaller that returns toString().
	/// For short output, I write an ObjectCaller to return getName().
	/// <P>
	/// I may add a throws clause later.  It would have to be an Exception,
	/// which is awkward.
	/// 
	/// </summary>
	/// <version>  	1.0.0 11 Aug 2001</version>
	/// <author>  		Yong Zhang </author>
	public interface ObjectCaller
	{
		/// <summary> Call method in o.
		/// *
		/// </summary>
		/// <param name="o">Object on which method will be called.  Guaranteed
		/// to be non-null.
		/// 
		/// </param>
		System.Object Call(System.Object o);
	}
}