/*
* @(#)LoggingRuleListViewItem.cs
*
* Copyright (c) 2008 Newtera, Inc. All rights reserved.
*
*/
namespace Newtera.Studio
{
	using System;
	using System.Windows.Forms;

	using Newtera.Common.MetaData.Logging;
	
	/// <summary>
	/// Represents a ListView item for a LoggingRule instance
	/// </summary>
	/// <version>  1.0.1 06 Jan 2009</version>
	public class LoggingRuleListViewItem : ListViewItem
	{
		private LoggingObject _obj;
		private LoggingRule _rule;

		/// <summary>
		/// Initializes a new instance of the LoggingRuleListViewItem class.
		/// </summary>
		/// <param name="obj">The LoggingObject that the rule is for.</param>
		/// <param name="rule">The LoggingRule instance</param>
		public LoggingRuleListViewItem(LoggingObject obj, LoggingRule rule) : base()
		{
			_obj = obj;
			_rule = rule;

			if (rule.Subject.Uid != null && rule.Subject.Uid.Length > 0)
			{
				this.Text = rule.Subject.Uid;
				this.ImageIndex = 8;
			}
			else if (rule.Subject.Roles != null && rule.Subject.Roles.Length > 0)
			{
				this.Text = ConvertArrayToString(rule.Subject.Roles);
				this.ImageIndex = 7;
			}
			else if (rule.Subject.Groups != null && rule.Subject.Groups.Length > 0)
			{
				this.Text = ConvertArrayToString(rule.Subject.Groups);
				this.ImageIndex = 7;
			}
		}

		/// <summary> 
		/// Gets the LoggingObject instance
		/// </summary>
		/// <value> The LoggingObject instance</value>
		public LoggingObject Object
		{
			get
			{
				return _obj;
			}
		}

		/// <summary> 
		/// Gets the LoggingRule instance
		/// </summary>
		/// <value> The LoggingRule instance</value>
		public LoggingRule Rule
		{
			get
			{
				return _rule;
			}
		}

		/// <summary>
		/// Convert a string array into a semi-colon separated string.
		/// </summary>
		/// <param name="values">An string array</param>
		/// <returns>A semi-colon separated string</returns>
		private string ConvertArrayToString(string[] values)
		{
			string converted = null;
			foreach (string val in values)
			{
				if (converted == null)
				{
					converted = val;
				}
				else
				{
					converted += ";";
					converted += val;
				}
			}

			return converted;
		}
	}
}