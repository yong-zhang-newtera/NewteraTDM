/*
* @(#)StringCollectionEditor.cs
*
* Copyright (c) 2003 Newtera, Inc. All rights reserved.
*
*/
namespace Newtera.Studio
{
	using System;
	using System.Collections.Specialized;
	using System.ComponentModel.Design;

	/// <summary>
	/// A Modal UI editor for editing StringCollection instance. It inherites
	/// from CollectionEditor class and overrides the CreateInstance method
	/// to return a empty string object.
	/// </summary>
	/// <version>  1.0.1 11 Oct 2003</version>
	/// <author> Yong Zhang</author>
	public class StringCollectionEditor : System.ComponentModel.Design.CollectionEditor
	{
		public StringCollectionEditor() : base(typeof(StringCollection))
		{
		}

		/// <summary> 
		/// Overrides the inherited method to return a string instance
		/// </summary>
		protected override object CreateInstance(Type itemType)
		{
			string newInstance = "New Value";
			return newInstance;
		}
	}
}