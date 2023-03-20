/*
* @(#)ArrayColumnCollectionEditor.cs
*
* Copyright (c) 2003 Newtera, Inc. All rights reserved.
*
*/
namespace Newtera.Studio
{
	using System;
	using System.Collections.Specialized;
	using System.ComponentModel.Design;
	
	using Newtera.Common.MetaData.Schema;

	/// <summary>
	/// A Modal UI editor for editing array column StringCollection instance. It inherites
	/// from CollectionEditor class and overrides the CreateInstance method
	/// to return a empty string object.
	/// </summary>
	/// <version>  1.0.1 27 Aug 2004</version>
	/// <author> Yong Zhang</author>
	public class ArrayColumnCollectionEditor : System.ComponentModel.Design.CollectionEditor
	{
		public ArrayColumnCollectionEditor() : base(typeof(StringCollection))
		{
		}

		public override object EditValue(System.ComponentModel.ITypeDescriptorContext context, IServiceProvider provider, object value)
		{
			string[] oldValues = new string[((StringCollection) value).Count];
			((StringCollection) value).CopyTo(oldValues, 0);

			object val = base.EditValue (context, provider, value);

			string[] newValues = new string[((StringCollection) val).Count];
			((StringCollection) val).CopyTo(newValues, 0);

			if (!IsSame(oldValues, newValues))
			{
				// to trigger value change event
				ArrayAttributeElement arrayAttribute = (ArrayAttributeElement) context.Instance;

				arrayAttribute.ColumnTitles = (StringCollection) val;
			}
			
			return val;
		}

		/// <summary> 
		/// Overrides the inherited method to return a string instance
		/// </summary>
		protected override object CreateInstance(Type itemType)
		{
			string newInstance = "New Value";
			return newInstance;
		}

		/// <summary>
		/// Gets the information indicating whether two string array are equal
		/// </summary>
		/// <param name="oldValues">old array</param>
		/// <param name="newValues">new array</param>
		/// <returns></returns>
		private bool IsSame(string[] oldValues, string[] newValues)
		{
			bool status = true;

			if (oldValues.Length != newValues.Length)
			{
				status = false;
			}
			else
			{
				for (int i = 0; i < oldValues.Length; i++)
				{
					if (oldValues[i] != newValues[i])
					{
						status = false;
						break;
					}
				}
			}

			return status;
		}
	}
}