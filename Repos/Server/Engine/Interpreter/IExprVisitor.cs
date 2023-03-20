/*
* @(#)IExprVisitor.cs
*
* Copyright (c) 2003 Newtera, Inc. All rights reserved.
*
*/
namespace Newtera.Server.Engine.Interpreter
{
	using System;
	
	/// <summary>
	/// Represent an interface for a visitor that can traverse through an expression tree
	/// and perform a certain action.
	/// </summary>
	/// <version>  1.0.0 20 Aug 2003</version>
	/// <author>  Yong Zhang </author>
	public interface IExprVisitor
	{		
		/// <summary> 
		/// Visit an expression object
		/// </summary>
		/// <param name="expr">IExpr to visit</param>
		/// <returns> false to stop visiting along this branch, true otherwise</returns>
		bool Visit(IExpr expr);
	}
}