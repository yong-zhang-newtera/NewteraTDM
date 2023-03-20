/*
* @(#)SQLComposite.cs
*
* Copyright (c) 2003 Newtera, Inc. All rights reserved.
*
*/
namespace Newtera.Server.Engine.Sqlbuilder.Sql
{
	using System;
	using System.Collections;

	/// <summary>
	/// The SQLComposite class defines the common behaviors for the
	/// elements that have children.
	/// The subclasses of SQLComposite (like SelectClause and WhereClause)
	/// inherite default behavious of the SQLComposite class.
	/// </summary>
	/// <version>  	1.0.1 14 Jul 2003 </version>
	/// <author>  		Yong Zhang </author>
	abstract public class SQLComposite : SQLElement
	{
		// private instance member
		private SQLElementCollection _children;
		
		/// <summary> 
		/// Initiating a SQLComposite object
		/// </summary>
		public SQLComposite() : base()
		{
			_children = new SQLElementCollection();
		}

		/// <summary>
		/// Gets the children as SQLElementCollection.
		/// </summary>
		/// <value>SQLElementCollection contains children</value>
		public override SQLElementCollection Children
		{
			get
			{
				return _children;
			}
		}

		/// <summary>
		/// Gets the information indicating whether the element is a composit element.
		/// </summary>
		/// <value>
		/// return true.
		/// </value>
		public override bool IsComposite
		{
			get
			{
				return true;
			}
		}
		
		/// <summary>
		/// Gets the SQL representation of the element.
		/// </summary>
		/// <return>
		/// A partial SQL string.
		/// </return>
		public override string ToSQL()
		{
			SQLElementCollection children = Children;
			string text = " ";
			
			foreach (SQLElement child in children)
			{
				text += child.ToSQL();
				text += " ";
			}
			
			return text;
		}
		
		/// <summary>
		/// Add an element as a child.
		/// </summary>
		/// <param name="element">a SQLElement object to be added</param>
		public override void Add(SQLElement element)
		{
			element.Parent = this;
			_children.Add(element); // adds as the last child
		}
		
		/// <summary>
		/// Remove an element from the children.
		/// </summary>
		/// <param name="element">a SQLElement object to be removed </param>
		public override void Remove(SQLElement element)
		{
			_children.Remove(element);
		}
		
		
		/// <summary>
		/// Sort the children in an order of thier positions.
		/// </summary>
		public override void SortChildren()
		{
			_children.Sort();
		}
		
	}
}