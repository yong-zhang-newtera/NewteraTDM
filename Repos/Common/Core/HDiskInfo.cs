/*
* @(#)DiskInfo.cs
*
* Copyright (c) 2008 Newtera, Inc. All rights reserved.
*
*/
using System;
using System.Runtime.InteropServices;

namespace Newtera.Common.Core
{

	[StructLayout(LayoutKind.Sequential, CharSet = CharSet.Ansi)]
	public struct DiskInfo
	{
		[MarshalAs(UnmanagedType.ByValTStr, SizeConst=20)]
		public string pSerialNumber;
		[MarshalAs(UnmanagedType.ByValTStr, SizeConst=40)]
		public string pModelNumber;
		[MarshalAs(UnmanagedType.ByValTStr, SizeConst=8)]
		public string pRevisionNo;
		public int pBufferSize;
		public int pCylinders;
		public int pHeads;
		public int pSectors;

		public DiskInfo(string pSerialNumber,string pModelNumber,string pRevisionNo,int pBufferSize,int pCylinders,int pHeads,int pSectors)
		{
			this.pSerialNumber = pSerialNumber;
			this.pModelNumber = pModelNumber;
			this.pRevisionNo = pRevisionNo;
			this.pBufferSize = pBufferSize;
			this.pCylinders = pCylinders;
			this.pHeads = pHeads;
			this.pSectors = pSectors;
		}
	}

	/// <summary>
	/// HDiskInfo 的摘要说明。
	/// </summary>
	public class HDiskInfo
	{
		[DllImport("GetDiskSerial")]
		public static extern int GetIdeDiskInfo(int DriveNo,ref DiskInfo tt, string sRegNumber);
		[DllImport("GetDiskSerial")]
		public static extern string GetSerialNumber(int DriveNo, string sRegNumber);
		[DllImport("GetDiskSerial")]
		public static extern string GetModelNumber(int DriveNo, string sRegNumber);
		[DllImport("GetDiskSerial")]
		public static extern string GetRevisionNo(int DriveNo, string sRegNumber);
		[DllImport("GetDiskSerial")]
		public static extern int GetBufferSize(int DriveNo, string sRegNumber);
		[DllImport("GetDiskSerial")]
		public static extern int GetCylinders(int DriveNo, string sRegNumber);
		[DllImport("GetDiskSerial")]
		public static extern int GetHeads(int DriveNo, string sRegNumber);
		[DllImport("GetDiskSerial")]
		public static extern int GetSectors(int DriveNo, string sRegNumber);

		public HDiskInfo()
		{
			//
			// TODO: 在此处添加构造函数逻辑
			//
		}
	}
}
