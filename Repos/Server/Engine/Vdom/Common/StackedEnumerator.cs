/*
* @(#)StackedEnumerator.cs
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
	/// This class uses a stack to put together enumerators for nodes at different levels.
	/// It presents to the caller as a single enumerator.
	/// </summary>
	/// <version>  	1.0.0 19 Nov 2003
	/// </version>
	/// <author> Yong Zhang</author>
	public abstract class StackedEnumerator : IEnumerator, IEnumerable
	{
		private Queue _enumeratorStack;
		private VDocument _document;
		private ArrayList _created;	// Nodes that their interators have been created
		private XmlNode _currentNode = null;

		/// <summary>
		/// Initialize a StackedEnumerator object
		/// </summary>
		/// <param name="node">the context node</param>
		/// <param name="the">The document of the context node</param>
		/// <param name="the">The selfEnumerator for the context node to put on top of stack.</param>
		public StackedEnumerator(XmlNode contextNode, VDocument document, IEnumerator selfEnumerator)
		{
			_document = document;
			_enumeratorStack = new Queue();
			_created = new ArrayList();
			
			if (selfEnumerator != null)
			{
				// SelfEnumerator is the first enumerator to be traversed
				AppendEnumerator(selfEnumerator);
			}
			
			// this calls the CreateEnumerator() method in a subclass to return an enumerator
			AppendEnumerator(InternalCreateEnumerator(contextNode));
		}
		
		/// <summary>
		/// Initialize a StackedEnumerator object
		/// </summary>
		/// <param name="node">the context node.</param>
		/// <param name="the">The document of the context node.</param>
		public StackedEnumerator(XmlNode contextNode, VDocument document) : this(contextNode, document, null)
		{
		}

		/// <summary>
		///  Default and no-arg constructor for StackedEnumerator
		/// </summary>
		protected StackedEnumerator()
		{
			_enumeratorStack = new Queue();
			_created = new ArrayList();
		}

		/// <summary>
		/// Gets the document that has been iterated
		/// </summary>
		/// <value> The document</returns>
		public VDocument Document
		{
			get
			{
				return _document;
			}
		}

		/// <summary>
		/// Create an enumerator for the context node. The derived class will override
		/// the method.
		/// </summary>
		/// <param name="contextNode">the context node.</param>
		abstract protected IEnumerator CreateEnumerator(XmlNode contextNode);

		/// <summary>
		/// Append an enumerator to the end of queue.
		/// </summary>
		/// <param name="enumerator">the enumerator to append</param>
		protected void AppendEnumerator(IEnumerator enumerator)
		{
			if (enumerator != null)
			{
				_enumeratorStack.Enqueue(enumerator);
			}
		}

		#region IEnumerator Members

		/// <summary>
		/// Sets the enumerator to its initial position, which is before the first element in the collection.
		/// </summary>
		public void Reset()
		{
			IEnumerator currentEnumerator = GetCurrentEnumerator();
			if (currentEnumerator != null)
			{
				currentEnumerator.Reset();
			}
		}

		/// <summary>
		/// Gets the current element in the collection.
		/// </summary>
		/// <value>The current element in the collection.</value>
		public object Current
		{	
			get
			{				
				return _currentNode;
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
			IEnumerator currentEnumerator = GetCurrentEnumerator();
			
			if (currentEnumerator != null)
			{
				_currentNode = (XmlNode) currentEnumerator.Current;
					
				AppendEnumerator(InternalCreateEnumerator(_currentNode));

				return true;
			}
			else
			{
				return false;
			}
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

		/// <summary>
		/// Make sure that there is only one enumerator created for given context node.
		/// </summary>
		/// <param name="node">the context node</param>
		private IEnumerator InternalCreateEnumerator(XmlNode contextNode)
		{
			IEnumerator enumerator = null;
			
			if (!_created.Contains(contextNode))
			{
				_created.Add(contextNode);
				enumerator = CreateEnumerator(contextNode);
			}
			
			return enumerator;
		}

		/// <summary>
		/// pop an enumerator out of stack.
		/// </summary>
		/// <returns>
		/// The enumerator that is poped.
		/// </returns>
		private IEnumerator GetCurrentEnumerator()
		{
			IEnumerator enumerator = null;
			
			while (_enumeratorStack.Count > 0)
			{
				IEnumerator nextEnum = (IEnumerator) _enumeratorStack.Peek();
				
				if (nextEnum.MoveNext())
				{
					enumerator = nextEnum;
					break;
				}
				else
				{
					// the enumerator is at the end, remove it from the queue
					_enumeratorStack.Dequeue();
				}
			}
			
			return enumerator;
		}
	}
}