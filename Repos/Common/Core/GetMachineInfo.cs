/*
* @(#)GetMachineInfo.cs
*
* Copyright (c) 2007 Newtera, Inc. All rights reserved.
*
*/
using System;
using System.Text;
using System.Runtime.InteropServices;
using System.Management;

namespace Newtera.Common.Core
{
    public class GetMachineInfo
	{
        /// <summary>
        /// return Volume Serial Number from hard drive
        /// </summary>
        /// <param name="strDriveLetter">[optional] Drive letter</param>
        /// <returns>[string] VolumeSerialNumber</returns>
		public string GetVolumeSerial(string strDriveLetter)
		{
            if (strDriveLetter == "" || strDriveLetter == null)
            {
                strDriveLetter = "C";
            }

		    ManagementObject disk = new ManagementObject("win32_logicaldisk.deviceid=\"" + strDriveLetter +":\"");
		    disk.Get();
		    return disk["VolumeSerialNumber"].ToString();
		}
		
		/// <summary>
		/// Returns MAC Address from first Network Card in Computer
		/// </summary>
		/// <returns>[string] MAC Address</returns>
		public string GetMACAddress()
		{
            try
            {
                ManagementClass mc = new ManagementClass("Win32_NetworkAdapterConfiguration");
                ManagementObjectCollection moc = mc.GetInstances();
                string MACAddress = String.Empty;
                foreach (ManagementObject mo in moc)
                {
                    if (MACAddress == String.Empty)  // only return MAC Address from first card
                    {
                        if ((bool)mo["IPEnabled"] == true)
                        {
                            MACAddress = mo["MacAddress"].ToString();
                        }
                    }
                    mo.Dispose();
                }

                MACAddress = MACAddress.Replace(":", "");
                return MACAddress;
            }
            catch (Exception)
            {
                return "888888";
            }
		}

		/// <summary>
		/// Return processorId from first CPU in machine
		/// </summary>
		/// <returns>[string] ProcessorId</returns>
		public string GetCPUId()
		{
			string cpuInfo =  String.Empty;
			string temp = String.Empty;
			ManagementClass mc = new ManagementClass("Win32_Processor");
            try {
                ManagementObjectCollection moc = mc.GetInstances();

                foreach (ManagementObject mo in moc)
                {
                    if (cpuInfo == String.Empty)
                    {
                        // only return cpuInfo from first CPU
                        if (mo.Properties != null &&
                            mo.Properties["ProcessorId"] != null &&
                            mo.Properties["ProcessorId"].Value != null)
                        {
                            cpuInfo = mo.Properties["ProcessorId"].Value.ToString();
                        }
                    }
                    else
                    {
                        break;
                    }
                }
            }
            catch (Exception)
            {
                cpuInfo = "888888";
            }

			return cpuInfo;
		}
	}
}
