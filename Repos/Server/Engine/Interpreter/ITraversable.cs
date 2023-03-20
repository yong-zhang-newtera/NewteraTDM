/*
* @(#)ITraversable.cs
*
* Copyright (c) 2003 Newtera, Inc. All rights reserved.
*
*/
namespace Newtera.Server.Engine.Interpreter
{
	using System;

	/// <summary>
	/// Represents an interface for the expressions that can be traversed by an ExprVisitor
	/// </summary>
	/// <version> 1.0.0 14 Aug 2003 </version>
	/// <author> Yong Zhang</author>
	public interface ITraversable
	{
		/// <summary> 
		/// Gets child count the expression.
		/// </summary>
		/// <value> Child count </returns>
		int ChildCount { get; }
		
		/// <summary> 
		/// Accept an ExprVisitor that will traverse the expression
		/// </summary>
		/// <param name="visitor">The ExprVisitor</param>
		void Accept(IExprVisitor visitor);
	}
}