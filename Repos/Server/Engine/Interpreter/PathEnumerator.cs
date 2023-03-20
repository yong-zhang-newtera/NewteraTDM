/*
* @(#)PathEnumerator.cs
*
* Copyright (c) 2003 Newtera, Inc. All rights reserved.
*
*/
namespace Newtera.Server.Engine.Interpreter
{
	using System;
	using System.Text;
	using System.Collections;
	using System.Xml;

	/// <summary>
	/// Represents a double value
	/// </summary>
	/// <version> 1.0.0 14 Aug 2003 </version>
	/// <author> Yong Zhang</author>
	public class PathEnumerator : IEnumerable, IEnumerator
	{
		private IList _enumerators;
		private IEnumerator _currentEnumerator;
		private int _currentIndex;
		private bool _noCurrentStep;
		private Step _previous;

		/// <summary>
		/// Initiate an instance of PathEnumerator class.
		/// </summary>
		public PathEnumerator()
		{
			_enumerators = new ArrayList();
			_currentEnumerator = null;
			_currentIndex = -1;
			_noCurrentStep = true;
			_previous = null;
		}

		/// <summary>
		/// Append an IEnumerator to the pathEnumerator
		/// </summary>
		/// <param name="enumerator">An IEnumerator object</param>
		public void Append(IEnumerator enumerator)
		{
			_enumerators.Add(enumerator);
		}

		/// <summary>
		/// Append a Step to the pathEnumerator
		/// </summary>
		/// <param name="step">A Step</param>
		public void Append(Step step)
		{
			ExprCollection coll = new ExprCollection();
			coll.Add(step);

			_enumerators.Add(coll.GetEnumerator());
		}

		/// <summary>
		/// Append a PathEnumerator to the pathEnumerator
		/// </summary>
		/// <param name="enumerator">A PathEnumerator object</param>
		public void Append(PathEnumerator enumerator)
		{
			_enumerators.Add(enumerator.GetEnumerator());
		}

		/// <summary>
		/// Gets the information indicating whether the path enumerator has reached the end
		/// </summary>
		/// <value>true if it is, false otherwise.</value>
		public bool IsEnd
		{
			get
			{
				return (_currentEnumerator != null ? false : true);
			}
		}

		/// <summary>
		/// Move the previous step
		/// </summary>
		/// <returns>false if it reaches the start of path, true otherwise </returns>
		public bool MovePrevious()
		{
			bool status = false;
			Step previous = _previous;

			Reset(); // start from beginning

			if (previous != null)
			{
				while (this.MoveNext())
				{
					if (this.Current == previous)
					{
						status = true;
						break;
					}
				}
			}

			return status;
		}

		#region IEnumerable Members

		/// <summary>
		/// Returns an enumerator that can iterate through a collection.
		/// </summary>
		/// <returns>An IEnumerator that can be used to iterate through the collection.</returns>
		public IEnumerator GetEnumerator()
		{
			Reset();

			return this;
		}

		#endregion

		#region IEnumerator Members

		/// <summary>
		/// Reset the path enumerator
		/// </summary>
		public void Reset()
		{
			if (_enumerators.Count > 0)
			{
				// Reset all the enumerators
				foreach (IEnumerator enumerator in _enumerators)
				{
					enumerator.Reset();
				}

				// set the first enumerator as the current
				_currentIndex = 0;
				_currentEnumerator = (IEnumerator) _enumerators[_currentIndex];
				_noCurrentStep = true;
				_previous = null;
			}
		}

		/// <summary>
		/// Return the current Step object in the path
		/// </summary>
		public object Current
		{
			get
			{
				if (_currentEnumerator != null)
				{
					return _currentEnumerator.Current;
				}
				else
				{
					throw new InvalidOperationException("The PathEnumerator is in an invalid state");
				}
			}
		}

		/// <summary>
		/// Move to next step.
		/// </summary>
		/// <returns>false if it reachs the end of path, true otherwise.</returns>
		public bool MoveNext()
		{
			bool status = false;

			// remember the previous step
			if (!_noCurrentStep && _currentEnumerator != null)
			{
				_previous = (Step) _currentEnumerator.Current;
			}

			while (_currentEnumerator != null && !(status = _currentEnumerator.MoveNext()))
			{
				if (_currentIndex < _enumerators.Count - 1)
				{
					_currentIndex++;
					_currentEnumerator = (IEnumerator) _enumerators[_currentIndex];
				}
				else
				{
					_currentEnumerator = null;
				}
			}

			if (status)
			{
				_noCurrentStep = false; // The enumerator has a valid current step
			}

			return status;
		}

		#endregion

		/// <summary>
		/// Gets the string representtaion of the path enumerator
		/// </summary>
		/// <returns>a path string</returns>
		public override string ToString()
		{
			StringBuilder builder = new StringBuilder();

			this.Reset();

			while (this.MoveNext())
			{
				builder.Append(this.Current.ToString());
			}

			return builder.ToString();
		}
	}
}