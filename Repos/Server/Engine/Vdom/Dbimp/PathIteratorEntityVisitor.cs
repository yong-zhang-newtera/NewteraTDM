/*
* @(#)PathIteratorEntityVisitor.cs
*
* Copyright (c) 2003 Newtera, Inc. All rights reserved.
*
*/
namespace Newtera.Server.Engine.Vdom.Dbimp
{
	using System;
	using Newtera.Common.MetaData;
	using Newtera.Server.Engine.Interpreter;
	using Newtera.Server.Engine.Sqlbuilder;
	using Newtera.Server.Engine.Sqlbuilder.Sql;
	using Newtera.Common.MetaData.Schema;

	/// <summary>
	/// PathIteratorInfoEntityVisitor class declares a common class for all entity
	/// visitors used by PathIterator object.
	/// </summary>
	/// <version>  	1.0.1 31 Jul 2003 </version>
	/// <author>  Yong Zhang </author>
	public class PathIteratorEntityVisitor
	{
		// protected instance members
		protected MetaDataModel _metaData; // The meta data model
		protected Step _currentStep = null;
		protected DBEntity _currentEntity = null;
		protected bool _stopIteration = false;
		protected PathEnumerator _pathEnumerator = null;
		
		/// <summary>
		/// Initiating an instance of PathIteratorEntityVisitor class.
		/// </summary>
		/// <param name="metaData">the meta model object.</param>
		protected PathIteratorEntityVisitor(MetaDataModel metaData)
		{
			_metaData = metaData;
		}
		
		/// <summary>
		/// Gets or sets the current step to the visitor.
		/// </summary>
		/// <value>the current step.</value>
		public Step CurrentStep
		{
			get
			{
				return _currentStep;
			}
			set
			{
				_currentStep = value;
			}
		}

		/// <summary>
		/// Gets the current entity from the visitor.
		/// </summary>
		/// <returns> DBEntity object.</returns>
		public DBEntity CurrentEntity
		{
			get
			{
				return _currentEntity;
			}
		}

		/// <summary>
		/// Gets the path from the visitor.
		/// </summary>
		/// <value> path the visitor is iteraring.</value>
		public PathEnumerator PathEnumerator
		{
			get
			{
				return _pathEnumerator;
			}
			set
			{
				_pathEnumerator = value;
			}
		}
		
		/// <summary>
		/// Gets the information indicatingstatus whether to stop the iteration of a path.
		/// </summary>
		/// <returns> true if to stop the iteration, false to continue.</returns>
		public bool StopIteration()
		{
			return _stopIteration;
		}
		
		/// <summary>
		/// Check if there is a class for the name
		/// </summary>
		/// <param name="name">the name of the class.</param>
		/// <returns> true if the name represents a valid class, false otherwise.</returns>
		protected bool IsValidClassName(string name)
		{
			bool status = false;

			if (_metaData.SchemaModel.FindClass(name) != null)
			{
				status = true;
			}
			
			return status;
		}
		
		/// <summary>
		/// check if the name is a valid name for class list element.
		/// </summary>
		/// <param name="name">the name of the class list element. </param>
		/// <param name="className">the name of a class to be matched.</param>
		/// <returns> true if it represents a valid class list name, false, otherwise.</returns>
		protected bool IsValidClassListName(string name, string className)
		{
			bool status = false;
			
			if (IsValidClassName(className))
			{
				System.Text.StringBuilder buf = new System.Text.StringBuilder();
				buf.Append(className).Append(SQLElement.ELEMENT_CLASS_NAME_SUFFIX);
				
				if (buf.ToString() == name)
				{
					status = true;
				}
			}
			
			return status;
		}
		
		/// <summary>
		/// check if the name is a valid name for class list element.
		/// </summary>
		/// <param name="name">the name of the class list element.</param>
		/// <returns> true if the name represents a valid class list name, false otherwise.</returns>
		protected bool IsValidClassListName(string name)
		{
			bool status = false;
			int pos = name.LastIndexOf(SQLElement.ELEMENT_CLASS_NAME_SUFFIX);
			if (pos > 0)
			{
				string className = name.Substring(0, (pos) - (0));
				
				status = IsValidClassName(className);
			}
			
			return status;
		}

        /// <summary>
        /// Gets the information indicating whether a class is a child of a parent class
        /// </summary>
        /// <param name="parentClassName">The parent class name</param>
        /// <param name="childClassName">The child class name</param>
        /// <returns>true if it is a child class, false otherwise</returns>
        protected bool IsParentOf(string parentClassName, string childClassName)
        {
            ClassElement childClassElement = _metaData.SchemaModel.FindClass(childClassName);
            if (childClassElement == null)
            {
                throw new Exception("Unable to find a class with name " + childClassName);
            }

            if (childClassElement.FindParentClass(parentClassName) != null)
            {
                return true;
            }
            else
            {
                return false;
            }
        }
	}
}