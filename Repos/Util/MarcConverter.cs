/*
* @(#)MarcConverter.cs
*
* Copyright (c) 2003-2004 Newtera, Inc. All rights reserved.
*
*/
namespace Newtera.Util
{
	using System;
	using System.Xml;
	using System.IO;
	using System.Data;
	using System.Text;
	using System.ComponentModel;
	using System.Collections;

	using Newtera.Common.Core;
	using Newtera.Common.MetaData.DataView;

	/// <summary> 
	/// The class implements the MarcConverter
	/// </summary>
	/// <version> 1.0.0 09 Jul 2005</version>
	public class MarcConverter
	{
		private int _recordNum = 0;

		/// <summary>
		/// Initiate an instance of MarcConverter class
		/// </summary>
		public MarcConverter()
		{
		}

		/// <summary>
		/// Converting a set of instances data into MARC format.
		/// </summary>
		/// <param name="dataView">The data view that provides meta-data for conversion</param>
		/// <param name="ds">The DataSet that contains data to be converted.</param>
		/// <param name="path">The path where to place the generated file</param>
		public string Convert(DataViewModel dataView, DataSet ds, string path)
		{
			string fileName = path + "data.sol";

			string recordPatter = GetRecordPattern(dataView, path);
			string record;

			using (StreamWriter sw = new StreamWriter(fileName))
			{
				InstanceView instanceView = new InstanceView(dataView, ds);

				// write the instance data to the file using MARC format
				int count = ds.Tables[dataView.BaseClass.Name].Rows.Count;
				PropertyDescriptorCollection properties;
				object val;
				string str;
				for (int row = 0; row < count; row++)
				{
					record = recordPatter;

					instanceView.SelectedIndex = row;
					
					properties = instanceView.GetProperties(null);

					foreach (InstanceAttributePropertyDescriptor pd in properties)
					{
						val = pd.GetValue();
						if (val != null)
						{
							str = val.ToString();
						}
						else
						{
							str = " ";
						}

						record = record.Replace(pd.Description, str);
					}

					// set the record controld number
					_recordNum++;
					record = record.Replace("{001}", _recordNum.ToString());

					// set the record length
					str = GetRecordLength(record);
					record = record.Replace("{num}", str);
					
					sw.WriteLine(record);
				}
			}

			return fileName;
		}

		/// <summary>
		/// Get a string representing a pattern of record line.
		/// </summary>
		/// <param name="dataView">The DataViewModel instance.</param>
		/// <param name="path">The path where the pattern files are placed.</param>
		/// <returns>A pattern string.</returns>
		private string GetRecordPattern(DataViewModel dataView, string path)
		{
			string pattern = null;

			// the patter file name is stored in the dataview as description
			string fileName = path + dataView.Description;

			if (File.Exists(fileName))
			{
				using(StreamReader sr = new StreamReader(fileName))
				{
					pattern = sr.ReadLine();
				}
			}
			else
			{
				throw new Exception("File " + fileName + " does not exist.");
			}

			return pattern;
		}

		/// <summary>
		/// Get a fixed lenghth of string representing number of chars in a record
		/// </summary>
		/// <param name="record">The record</param>
		/// <returns>The fixed lenght str</returns>
		private string GetRecordLength(string record)
		{
			int length = record.Length + 1; // plus a newline char
			string str = length.ToString();

			// prepend zeros if it is less than five chars
			int numOfZeros = 5 - str.Length;
			for (int i = 0; i < numOfZeros; i++)
			{
				str = "0" + str;
			}

			return str;
		}
	}
}