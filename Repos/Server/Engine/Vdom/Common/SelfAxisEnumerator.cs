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

	/// <summary>
	/// This is an implementation of enumerator for Self Axis of a node
	/// </summary>
	/// <version>  	1.0.1 11 Nov 2003
	/// </version>
	/// <author>Yong Zhang</author>
	public class SelfAxisEnumerator : IEnumerator, IEnumerable
	{
		/* private memeber */
		private bool _hasNext;
		private XmlNode _node;

		/// <summary>
		/// Initializing SelfAxisEnumerator
		/// </summary>
		/// <param name="node">the current node</param>
		public SelfAxisEnumerator(XmlNode node)
		{
			_hasNext = true;
			_node = node;
		}

		#region IEnumerator Members

		/// <summary>
		/// Sets the enumerator to its initial position, which is before the first element in the collection.
		/// </summary>
		public void Reset()
		{
			_hasNext = true;
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
			bool status = _hasNext;

			if (_hasNext)
			{
				_hasNext = false;
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