/*
* @(#)ValueChangedEventArgs.cs
*
* Copyright (c) 2003 Newtera, Inc. All rights reserved.
*
*/
namespace Newtera.Common.MetaData
{
	using System;

	/// <summary>
	/// Represents a event argument for the ValueChanged event
	/// </summary>
	/// <version>1.0.1 16 Nov. 2003</version>
	/// <author>Yong Zhang</author>
	public class ValueChangedEventArgs : EventArgs
	{
		/// <summary>
		/// The property name
		/// </summary>
		public string Property;

        /// <summary>
        /// The old property value
        /// </summary>
        public object OldValue;

		/// <summary>
		/// The new property value
		/// </summary>
		public object NewValue;

		/// <summary>
		/// Instantiate an instance of ValueChangedEventArgs
		/// </summary>
		/// <param name="property"></param>
		/// <param name="newValue"></param>
		public ValueChangedEventArgs(string property, object newValue) 
		{
			this.Property = property;
            this.OldValue = null;
			this.NewValue = newValue;
		}

        /// <summary>
        /// Instantiate an instance of ValueChangedEventArgs
        /// </summary>
        /// <param name="property">rpoperty name</param>
        /// <param name="newValue">new property value</param>
        /// <param name="oldValue">old property value</param>
        public ValueChangedEventArgs(string property, object newValue, object oldValue)
        {
            this.Property = property;
            this.OldValue = oldValue;
            this.NewValue = newValue;
        }
	}
}