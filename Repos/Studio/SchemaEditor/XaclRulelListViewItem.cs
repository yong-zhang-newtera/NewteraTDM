/*
* @(#)XaclRuleListViewItem.cs
*
* Copyright (c) 2003 Newtera, Inc. All rights reserved.
*
*/
namespace Newtera.Studio
{
	using System;
	using System.Windows.Forms;

	using Newtera.Common.MetaData.XaclModel;
	
	/// <summary>
	/// Represents a ListView item for an XaclRule instance
	/// </summary>
	/// <version>  1.0.1 11 Dec 2003</version>
	/// <author>  Yong Zhang</author>
	public class XaclRuleListViewItem : ListViewItem
	{
		private XaclObject _obj;
		private XaclRule _rule;

		/// <summary>
		/// Initializes a new instance of the XaclRuleListViewItem class.
		/// </summary>
		/// <param name="obj">The XaclObject that the rule is for.</param>
		/// <param name="rule">The XaclRule instance</param>
		public XaclRuleListViewItem(XaclObject obj, XaclRule rule) : base()
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
		/// Gets the XaclObject instance
		/// </summary>
		/// <value> The XaclObject instance</value>
		public XaclObject Object
		{
			get
			{
				return _obj;
			}
		}

		/// <summary> 
		/// Gets the XaclRule instance
		/// </summary>
		/// <value> The XaclRule instance</value>
		public XaclRule Rule
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