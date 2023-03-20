/*
* @(#)PrioritizedRuleList.cs
*
* Copyright (c) 2005 Newtera, Inc. All rights reserved.
*
*/
namespace Newtera.Common.MetaData.Rules
{
	using System;
	using System.Collections;

	/// <summary>
	/// A collection class that sort the a list of RuleDef objects based on their Priority.
    /// The rules with high priorities are placed ahead of rules with low priorities.
	/// </summary>
	/// <version> 1.0.0 21 Oct 2007 </version>
    public class PrioritizedRuleList : RuleNodeBase
	{
		private IList _priorities;
        private RuleCollection _rules;

		/// <summary>
		///  Initializes a new instance of the PrioritizedRuleList class.
		/// </summary>
		public PrioritizedRuleList(IRuleNode owner) : base()
		{
			_priorities = new ArrayList();
            _rules = new RuleCollection();
            _rules.Owner = owner;
		}

		/// <summary>
		/// Get the collection of RuleDef objects that have been sorted based on their priorities.
		/// </summary>
		/// <value>A sorted collection of RuleDef objects.</value>
        public RuleCollection Rules
		{
			get
			{
				return _rules;
			}
		}

		/// <summary>
		/// Adds a RuleDef object to the list at the position based on its priority.
		/// </summary>
		/// <param name="priority">The integer represents a rule's priority.</param>
		/// <param name="ruleDef">the RuleDef to be added</param>
		/// <returns>The actual position into which the new rule was inserted</returns>
		public int Add(int priority, RuleDef ruleDef)  
		{
			int index = 0;

			foreach (int pro in _priorities)
			{
				// the value is added in descending order
				if (priority > pro)
				{
					// found a place in the list
					break;
				}
				else
				{
					index++;
				}
			}

			if (index < _priorities.Count)
			{
				_priorities.Insert(index, priority);
				_rules.Insert(index, ruleDef);
			}
			else
			{
				_priorities.Add(priority);
				_rules.Add(ruleDef);
			}

			return index;
		}

		/// <summary>
		/// determines the index of a specific rule in the collection
		/// </summary>
        /// <param name="ruleDef">The RuleDef object to locate in the collection</param>
        /// <returns>The index of ruleDef if found in the list; otherwise, -1</returns>
		public int IndexOf(RuleDef ruleDef )  
		{
            return (_rules.IndexOf(ruleDef));
		}


		/// <summary>
		/// removes the first occurrence of a specific rule from the collection
		/// </summary>
		/// <param name="ruleDef">The RuleDef object to remove from the collection.</param>
		public void Remove(RuleDef ruleDef )  
		{
            int index = _rules.IndexOf(ruleDef);
			if (index >= 0)
			{
				_priorities.RemoveAt(index);
				_rules.RemoveAt(index);
			}
		}

        /// <summary>
        /// Gets the type of node
        /// </summary>
        /// <value>One of NodeType values</value>
        public override NodeType NodeType
        {
            get
            {
                return NodeType.PrioritizedRuleList;
            }
        }
	}
}