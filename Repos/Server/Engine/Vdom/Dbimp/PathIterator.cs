/*
* @(#)PathIterator.cs
*
* Copyright (c) 2003 Newtera, Inc. All rights reserved.
*
*/
namespace Newtera.Server.Engine.Vdom.Dbimp
{
	using System;
	using Newtera.Server.Engine.Vdom;
	using Newtera.Server.Engine.Interpreter;
	using Newtera.Server.Engine.Sqlbuilder;

	/// <summary>
	/// A PathIterator iterates through a path and apples a certain operation to
	/// an entity tree definded by a visitor object.
	/// </summary>
	/// <version>  	1.0.0 31 Jul 2003 </version>
	/// <author>  		Yong Zhang  </author>
	public class PathIterator
	{
		// private instance members
		private EntityVisitor _visitor;
		private DBEntity _currentEntity;
		private PathEnumerator _pathEnumerator;
		private Step _currentStep;
		
		/// <summary>
		/// Initiating an instance of PathIterator class.
		/// </summary>
		/// <param name="pathEnumerator">path enumerator. </param>
		/// <param name="root">the root entity of a entity tree.</param>
		/// <param name="vistor">which defines the operation to the entity tree.</param>
		public PathIterator(PathEnumerator pathEnumerator, DBEntity root, EntityVisitor visitor)
		{
			_pathEnumerator = pathEnumerator;
			_currentEntity = root;
			_visitor = visitor;
			_currentStep = null;
		}

		/// <summary>
		/// Gets the current step of iteration.
		/// </summary>
		/// <returns> Step object</returns>
		public Step CurrentStep
		{
			get
			{
				return _currentStep;
			}
		}
				
		/// <summary>
		/// Iterate through the path.
		/// </summary>
		public void Iterate()
		{
			// for error report purpose
			((PathIteratorEntityVisitor) _visitor).PathEnumerator = _pathEnumerator;
			
			while (_pathEnumerator.MoveNext())
			{
				_currentStep = (Step) _pathEnumerator.Current;
				
				((PathIteratorEntityVisitor) _visitor).CurrentStep = _currentStep;
				
				_currentEntity.Accept(_visitor); // apply operation to the entity
				
				// Entity can be different for each step
				_currentEntity = ((PathIteratorEntityVisitor) _visitor).CurrentEntity;
				
				if (((PathIteratorEntityVisitor) _visitor).StopIteration())
				{
					break;
				}
			}
		}
	}
}