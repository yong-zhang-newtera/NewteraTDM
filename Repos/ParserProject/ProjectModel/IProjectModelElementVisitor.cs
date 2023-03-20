/*
* @(#)IProjectModelElementVisitor.cs
*
* Copyright (c) 2005 Newtera, Inc. All rights reserved.
*
*/
namespace Newtera.ParserGen.ProjectModel
{
	using System;


	/// <summary>
	/// Represents an interface for visitors that traverse elements in a project model.
	/// </summary>
	/// <version> 1.0.0 11 Nov. 2005 </version>
	/// <author> Yong Zhang</author>
	public interface IProjectModelElementVisitor
	{
		/// <summary>
		/// Viste a Project element.
		/// </summary>
		/// <param name="element">A Project instance</param>
		/// <returns>true to contibute visiting nested elements, false to stop</returns>
		bool VisitProject(ProjectElement element);

		/// <summary>
		/// Viste a Grammar element.
		/// </summary>
		/// <param name="element">A Grammar instance</param>
		/// <returns>true to contibute visiting nested elements, false to stop</returns>
		bool VisitGrammar(GrammarElement element);

		/// <summary>
		/// Viste a Parser element.
		/// </summary>
		/// <param name="element">A Parser instance</param>
		/// <returns>true to contibute visiting nested elements, false to stop</returns>
		bool VisitParser(ParserElement element);

		/// <summary>
		/// Viste a Sample element.
		/// </summary>
		/// <param name="element">A Sample instance</param>
		/// <returns>true to contibute visiting nested elements, false to stop</returns>		
		bool VisitSample(SampleElement element);
	}
}