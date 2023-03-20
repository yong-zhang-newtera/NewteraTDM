using System;
using System.Collections;

namespace FileInfoStore
{
	/// <summary>
	/// 存放多组文件信息。
	/// </summary>
	public struct MyFileInfo
	{
		public string Comment;
		public string Key;
		public string Values;
		
		

		public MyFileInfo(string comment,string key,string values)
		{
			Comment=comment;
			Key=key;
			Values=values;
		}

		// Declare a delegate type for processing a MyFileInfo:
		public delegate void ProcessMyFileInfoDelegate(MyFileInfo myfileinfo);

		// Maintains a myfileinfo database.	
		//listA
		public class MyFileList
		{
			public MyFileList(string flag)
			{
				string f=flag;
			}
			// List of all fileinfo in the database:
			ArrayList list = new ArrayList();   

			// Add a myfileinfo to the database:
			public void AddMyFileInfo(string comment,string key,string values)
			{
				list.Add(new MyFileInfo(comment,key,values));
			}

			// Call a passed-in delegate on each MyFileInfo to process it: 
			public void ProcessGetInfoDelegate(ProcessMyFileInfoDelegate processMyFileInfo,string flag)
			{
				foreach (MyFileInfo m in list) 
				{
					// Calling the delegate:
					processMyFileInfo(m);
				}
			}
		}
	
	}
}
