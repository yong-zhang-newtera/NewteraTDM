/*
* @(#)LinkedClassListViewItem.cs
*
* Copyright (c) 2003 Newtera, Inc. All rights reserved.
*
*/
namespace Newtera.WindowsControl
{
	using System;
	using System.Windows.Forms;

	using Newtera.Common.MetaData;
	using Newtera.Common.MetaData.Schema;
	using Newtera.Common.MetaData.DataView;
	
	/// <summary>
	/// Represents a ListView item for a referenced class
	/// </summary>
	/// <version>  1.0.1 26 Sept 2003</version>
	/// <author>  Yong Zhang</author>
	public class LinkedClassListViewItem : ListViewItem
	{
        private ClassElement _linkedClassElement;
		private DataClass _referringClass;
		private RelationshipAttributeElement _relationshipAttribute;

		/// <summary>
		/// Initializes a new instance of the LinkedClassListViewItem class.
		/// </summary>
		/// <param name="referringClass">The referring DataClass element to the referenced class</param>
		/// <param name="relationshipAttribute">The schema model element represents the relationship that refers to the referenced class.</param>
		public LinkedClassListViewItem(string text, ClassElement linkedClassElement, DataClass referringClass, RelationshipAttributeElement relationshipAttribute) : base(text)
		{
            _linkedClassElement = linkedClassElement;
			_referringClass = referringClass;
			_relationshipAttribute = relationshipAttribute;
		}

        /// <summary> 
        /// Gets the class element of the linked class.
        /// </summary>
        /// <value>A DataClass</value>
        public ClassElement LinkedClassElement
        {
            get
            {
                return _linkedClassElement;
            }
        }

		/// <summary> 
		/// Gets the parent class to the referenced class.
		/// </summary>
		/// <value>A DataClass</value>
		public DataClass ReferringClass
		{
			get
			{
				return _referringClass;
			}
		}

		/// <summary> 
		/// Gets the relationship attribute that leads to the referenced class.
		/// </summary>
		/// <value>A RelationshipAttributeElement</value>
		public RelationshipAttributeElement RelationshipAttribute
		{
			get
			{
				return _relationshipAttribute;
			}
		}
	}
}