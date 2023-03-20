using System;
using System.Collections;

namespace FileInfomation
{
	/// <summary>
	/// 存放多组文件信息。
	/// </summary>
	internal class MyFileInfo
	{
		private string _key;
		private string _values;
		private string _comment;

		public MyFileInfo(string key,string values,string comment)
		{
			Key=key;
			Values=values;
			Comment=comment;
		}


		public string Key
		{
			get 
			{
				return _key;
			}
			set
			{
				_key=value;
			}
		}
		public string Values
		{
			get 
			{
				return _values;
			}
			set
			{
				_values=value;
			}
		}
		public string Comment
		{
			get 
			{
				return _comment;
			}
			set
			{
				_comment=value;
			}
		}
	}
}
