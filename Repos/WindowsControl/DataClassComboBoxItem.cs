/*
* @(#)DataClassComboBoxItem.cs
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
	/// Represents a ComboBox item for a DataClass in a data view
	/// </summary>
	/// <version>  1.0.1 26 Sept 2003</version>
	/// <author>  Yong Zhang</author>
	public class DataClassComboBoxItem
	{
		private DataClass _dataClass;

		/// <summary>
		/// Initializes a new instance of the DataClassComboBoxItem class.
		/// </summary>
		/// <param name="referencedClass">A data class that the item represents</param>
		public DataClassComboBoxItem(DataClass dataClass)
		{
			_dataClass = dataClass;
		}

		/// <summary> 
		/// Gets the data class.
		/// </summary>
		/// <value>A DataClass</value>
		public DataClass DataClass
		{
			get
			{
				return _dataClass;
			}
		}

		public override string ToString()
		{
			return _dataClass.Caption + " (" + _dataClass.ClassName + ")";
		}
	}
}