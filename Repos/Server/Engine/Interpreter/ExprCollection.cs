/*
* @(#)ExprCollection.cs
*
* Copyright (c) 2003 Newtera, Inc. All rights reserved.
*
*/
namespace Newtera.Server.Engine.Interpreter
{
	using System;
	using System.Collections;

	/// <summary>
	/// An object collection class to handle IExpr when collections are
	/// returned from method calls. It also implements AST interface so that it can
	/// be referenced in ANTLR parser.
	/// </summary>
	/// <version> 1.0.1 26 Jun 2003 </version>
	/// <author> Yong Zhang</author>
	public class ExprCollection : CollectionBase, IExpr, ITraversable, ICloneable
	{
		/// <summary>
		///  Initializes a new instance of the ExprCollection class.
		/// </summary>
		public ExprCollection() : base()
		{
		}

		/// <summary>
		/// Implemention of Indexer member
		/// </summary>
		public IExpr this[int index]  
		{
			get  
			{
				return ((IExpr) List[index]);
			}
			set  
			{
				List[index] = value;
			}
		}

		/// <summary>
		/// Adds a IExpr to the ExprCollection.
		/// </summary>
		/// <param name="value">the object to be added</param>
		/// <returns>The position into which the new element was inserted</returns>
		public int Add(IExpr value )  
		{
			return (List.Add( value ));
		}

		/// <summary>
		/// determines the index of a specific item in the collection
		/// </summary>
		/// <param name="value">The Object to locate in the collection</param>
		/// <returns>The index of value if found in the list; otherwise, -1</returns>
		public int IndexOf(IExpr value )  
		{
			return (List.IndexOf( value ));
		}

		/// <summary>
		/// inserts an item to the collection at the specified position
		/// </summary>
		/// <param name="index">The zero-based index at which value should be inserted</param>
		/// <param name="value">The Object to insert into collection</param>
		public void Insert(int index, IExpr value)  
		{
			List.Insert(index, value );
		}

		/// <summary>
		/// removes the first occurrence of a specific object from the collection
		/// </summary>
		/// <param name="value">The Object to remove from the collection.</param>
		public void Remove(IExpr value )  
		{
			List.Remove(value);
		}

		/// <summary>
		/// Sort elements in the collection based on their position
		/// </summary>
		/// <remarks>IExpr implements IComparable interface that uses position as comparator</remarks>
		public void Sort()  
		{
			this.InnerList.Sort();
		}

		/// <summary>
		/// determines whether the collection contains a specific value
		/// </summary>
		/// <param name="value">The Object to locate in the collection.</param>
		/// <returns>true if the Object is found in the List; otherwise, false.</returns>
		public bool Contains(IExpr value )  
		{
			// If value is not of type SchemaModelElement, this will return false.
			return (List.Contains( value ));
		}

		/// <summary>
		/// Performs additional custom processes before inserting a new element into the Collection
		/// </summary>
		/// <param name="index">The zero-based index at which to insert value.</param>
		/// <param name="value">The new value of the element at index.</param>
		protected override void OnInsert(int index, Object value)  
		{
			if (!(value is IExpr))
			{
				throw new ArgumentException( "value must be of type IExpr.", "value" );
			}
		}

		/// <summary>
		/// Performs additional custom processes when removing an element from the Collection.
		/// </summary>
		/// <param name="index">The zero-based index at which to remove value.</param>
		/// <param name="value">The new value of the element at index.</param>
		protected override void OnRemove( int index, Object value)  
		{
			if (!(value is IExpr))
			{
				throw new ArgumentException( "value must be of type IExpr.", "value" );
			}
		}

		/// <summary>
		/// Performs additional custom processes before setting a value in the Collection.
		/// </summary>
		/// <param name="index">The zero-based index at which oldValue can be found.</param>
		/// <param name="oldValue">The value to replace with newValue.</param>
		/// <param name="newValue">The new value of the element at index.</param>
		protected override void OnSet(int index, Object oldValue, Object newValue)  
		{
			if (!(newValue is IExpr))
			{
				throw new ArgumentException( "newValue must be of type IExpr.", "newValue" );
			}
		}

		/// <summary>
		/// Performs additional custom processes when validating a value.
		/// </summary>
		/// <param name="value">The object to validate.</param>
		protected override void OnValidate(Object value)  
		{
			if (!(value is IExpr))
			{
				throw new ArgumentException( "value must be of type IExpr." );
			}
		}

		#region IExpr Members

		/// <summary>
		/// Gets the interpreter that stores the symbol table
		/// </summary>
		public Interpreter Interpreter
		{
			get
			{
				if (this.Count > 0)
				{
					return ((IExpr) this[0]).Interpreter;
				}
				else
				{
					return null;
				}
			}
		}

		/// <summary>
		/// Gets or sets the data type of the expression.
		/// </summary>
		/// <value>The DataType object</value>
		public DataType DataType
		{
			get
			{
				return null;
			}
			set
			{
			}
		}

		/// <summary>
		/// Gets or sets the data value of the expression.
		/// </summary>
		/// <value>The Value object</value>
		public Value Value
		{
			get
			{
				return null;
			}
			set
			{
			}
		}

		/// <summary>
		/// Gets the expression type.
		/// </summary>
		/// <value>One of the ExprType enum values</value>
		public virtual ExprType ExprType
		{
			get
			{
				return ExprType.COLLECTION;
			}
		}

		/// <summary>
		/// Prepare the expression in the phase one of interpreting
		/// </summary>
		public void Prepare()
		{
			foreach (IExpr expr in this.List)
			{
				expr.Prepare();
			}
		}

		/// <summary>
		/// Restrict the expression in the phase two of interpreting
		/// </summary>
		public void Restrict()
		{
			foreach (IExpr expr in this.List)
			{
				expr.Restrict();
			}
		}

		/// <summary>
		/// Evaluate the expression in the phase three of interpreting
		/// </summary>
		public Value Eval()
		{
			ValueCollection values = new ValueCollection();

			foreach (IExpr expr in List)
			{
				values.Add(expr.Eval());
			}

			return new XCollection(values);
		}

		/// <summary>
		/// Print the information about the symbol for debug purpose.
		/// </summary>
		public void Print()
		{
			int i = 0;
			System.Console.Write("(");
			foreach (IExpr expr in this.List)
			{
				if (i > 0)
				{
					System.Console.Write(", " + expr.Eval());
				}
				else
				{
					System.Console.Write(expr.Eval());
				}

				i++;
			}

			System.Console.WriteLine(")");
		}

		#endregion

		#region ITraversable Members

		/// <summary> 
		/// Gets child count the expression collection.
		/// </summary>
		/// <value> Child count </returns>
		public int ChildCount
		{
			get
			{
				return this.Count;
			}
		}

		/// <summary> 
		/// Accept an ExprVisitor that will traverse the expression collection
		/// </summary>
		/// <param name="visitor">The ExprVisitor</param>
		public void Accept(IExprVisitor visitor)
		{
			if (visitor.Visit(this))
			{
				foreach (IExpr expr in List)
				{
					if (expr is ITraversable)
					{
						((ITraversable) expr).Accept(visitor);
					}
					else
					{
						visitor.Visit(expr);
					}
				}
			}
		}

		#endregion

		#region ICloneable Members

		public object Clone()
		{
			return null;
		}

		#endregion
	}
}