/*
* @(#)SelfAxisEnumerator.cs
*
* Copyright (c) 2003 Newtera, Inc. All rights reserved.
*
*/
namespace Newtera.Server.Engine.Vdom.Common
{
	using System;
	using System.Xml;
	using System.Collections;
	using Newtera.Server.Engine.Vdom;

	/// <summary> This is an implementation of enumerator for a single node
	/// 
	/// </summary>
	/// <version>  	1.0.1 20 Nov 2003
	/// </version>
	/// <author>Yong Zhang</author>
	public class SingleNodeEnumerator : IEnumerator, IEnumerable
	{
		/* private members */
		private XmlNode _node;
		private bool _seen;

		/// <summary>
		/// Initializing a SingleNodeEnumerator object.
		/// </summary>
		/// <param name="node">the current node.</param>
		public SingleNodeEnumerator(XmlNode node)
		{
			_node = node;
			_seen = false;
		}

		/// <summary> Returns whether it has next node
		/// 
		/// </summary>
		/// <returns> true if it has next node, false otherwise.
		/// 
		/// </returns>
		public virtual bool HasNext()
		{
			return !_seen;
		}
		/// <summary> Get the next node
		/// 
		/// </summary>
		/// <returns> the next node.
		/// 
		/// </returns>
		public virtual object Next()
		{
			
			if (HasNext())
			{
				_seen = true;
				return _node;
			}
			
			throw new System.Exception();
		}

		#region IEnumerator Members

		/// <summary>
		/// Sets the enumerator to its initial position, which is before the first element in the collection.
		/// </summary>
		public void Reset()
		{
			_seen = false;
		}

		/// <summary>
		/// Gets the current element in the collection.
		/// </summary>
		/// <value>The current element in the collection.</value>
		public object Current
		{
			get
			{
				return _node;
			}
		}

		/// <summary>
		/// Advances the enumerator to the next element of the collection.
		/// </summary>
		/// <returns>
		/// true if the enumerator was successfully advanced to the next element;
		/// false if the enumerator has passed the end of the collection.
		/// </returns>
		public bool MoveNext()
		{
			bool status = !_seen;

			if (!_seen)
			{
				_seen = true;
			}

			return status;
		}

		#endregion

		#region IEnumerable Members

		/// <summary>
		/// Returns an enumerator that can iterate through a collection.
		/// </summary>
		/// <returns>An IEnumerator that can be used to iterate through the collection.</returns>
		public IEnumerator GetEnumerator()
		{
			return this;
		}

		#endregion
	}
}