/*
* @(#)ValidateRuleListViewItem.cs
*
* Copyright (c) 2003-2004 Newtera, Inc. All rights reserved.
*
*/
namespace Newtera.Studio
{
	using System;
	using System.Windows.Forms;

	using Newtera.Common.MetaData.Schema;
	using Newtera.Common.MetaData.Rules;
	
	/// <summary>
	/// Represents a ListView item for a RuleDef instance
	/// </summary>
	/// <version>  1.0.0 16 Jun 2004</version>
	/// <author>Yong Zhang</author>
	public class ValidateRuleListViewItem : ListViewItem
	{
		private ClassElement _classElement;
		private RuleDef _rule;

		/// <summary>
		/// Initializes a new instance of the ValidateRuleListViewItem class.
		/// </summary>
		/// <param name="classElement">The class element the rule is associated with</param>
		/// <param name="rule">The RuleDef instance</param>
		public ValidateRuleListViewItem(ClassElement classElement, RuleDef rule, string ownerClassCaption) : base()
		{
			_classElement = classElement;
			_rule = rule;

			this.Text = rule.ToString();
			this.ImageIndex = 12;
            this.SubItems.Add(rule.Priority.ToString());
            this.SubItems.Add(ownerClassCaption);
		}

		/// <summary> 
		/// Gets the class element the rule is associated with.
		/// </summary>
		/// <value> The ClassElement instance</value>
		public ClassElement ClassElement
		{
			get
			{
				return _classElement;
			}
		}

		/// <summary> 
		/// Gets the RuleDef instance associated with the list view item
		/// </summary>
		/// <value> The RuleDef instance</value>
		public RuleDef Rule
		{
			get
			{
				return _rule;
			}
            set
            {
                _rule = value;
            }
		}
	}
}