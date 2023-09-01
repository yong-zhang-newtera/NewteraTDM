using System;
using System.IO;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using System.Management;
using System.Runtime.InteropServices;
using Microsoft.Win32;
using System.Windows.Forms;


namespace Newtera.CustomActions
{
    /// <summary>
    ////Operating System Infomation
    /// </summary>
    public class SetOperatingSystem
    {     
        [DllImport("advapi32.dll")]

        public static extern bool LookupAccountName(string lpSystemName, string lpAccountName, byte[] sid, ref int cbSid, StringBuilder ReferencedDomainName, ref int cbReferencedDomainName, ref int peUse);

        private  string installPath;
        private string ebaaasInstallPath;

        /// <summary>
        //设置文件夹权限 
        /// </summary>
        public void SetFilePermiss()
        {
            //得到程序运行目录
            installPath = basePath();
            string[] strName = new string[] {"Everyone" };
            DirectoryInfo Dir = new DirectoryInfo(installPath);
             try
             {

                 foreach (string userName in strName)
                 {
                     //给Ebaas子目录写权限   
                     foreach (DirectoryInfo d in Dir.GetDirectories())
                     {
                         string accessFolder = installPath + d.Name;

                         SetDACL(accessFolder, userName);
                     }
                 }
             }
             catch (Exception e)
             {
                 MessageBox.Show(e.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);  
             }

            #region 给指定文件夹写权限
            //if (inPath != "WebClient")
            //{
            //    string myPath1 = inPath + "bin";
            //    string myPath2 = inPath + "Config";
            //    string myPath3 = inPath + "Templates";    

            //    string[] strName = new string[] { "ASPNET", "Everyone" };
            //    string[] _FilePath = new string[] { myPath1, myPath2, myPath3};

            //    foreach (string userName in strName)
            //    {
            //        foreach (string filePath in _FilePath)
            //        {

            //            if (GetFileSystem(filePath.Substring(0, 1)) != "NTFS")
            //                return;

            //            SetDACL(filePath, userName);
            //        }
            //    }
            //}
            //else
            //{
            //    return;
            //}
            #endregion
        }

        /// <summary>
        //判断磁盘文件格式FAT NTF
        /// </summary>
        ///
        private string GetFileSystem(string diskName)
        {

            string fileSystem = "";

            System.Management.ManagementObjectSearcher diskClass = new ManagementObjectSearcher("select filesystem from Win32_LogicalDisk where name=\"" + diskName + ":\"");

            ManagementObjectCollection disks = diskClass.Get();

            foreach (ManagementObject disk in disks)
            {

                PropertyDataCollection diskProperties = disk.Properties;

                foreach (PropertyData diskProperty in diskProperties)
                {

                    fileSystem = diskProperty.Value.ToString();

                }

            }

            return fileSystem;
        }


        /// <summary>
        //获取帐户信息 
        /// </summary>
        private void SetDACL(string filePath, string userName)
        {
            int cbSid = 100;
            byte[] userSid = new byte[28];
            StringBuilder domainName = new StringBuilder(255);
            int domainNameLength = 255;
            int sidType = 255;
            bool result = LookupAccountName(null, userName, userSid, ref cbSid, domainName, ref domainNameLength, ref sidType);

            if (!result)

                return;

            //获取文件描述符 

            ManagementPath path = new ManagementPath();

            path.Server = ".";

            path.NamespacePath = @"root\cimv2";

            path.RelativePath = @"Win32_LogicalFileSecuritySetting.Path='" + filePath + "'";

            ManagementObject dir = new ManagementObject(path);

            ManagementBaseObject outParams = dir.InvokeMethod("GetSecurityDescriptor", null, null);

            if (((uint)(outParams.Properties["ReturnValue"].Value)) != 0)
            {
                throw new Exception("Failed to get file descriptor");
            }

            ManagementBaseObject Descriptor = ((ManagementBaseObject)(outParams.Properties["Descriptor"].Value));

            //获取访问控制列表 

            ManagementBaseObject[] DaclObject = ((ManagementBaseObject[])(Descriptor.Properties["Dacl"].Value));


            //复制一个访问控制项 

            ManagementBaseObject ace = (ManagementBaseObject)DaclObject[0].Clone();


            //设置访问控制项属性 

            ManagementBaseObject trustee = (ManagementBaseObject)ace.Properties["Trustee"].Value;

            trustee.Properties["Domain"].Value = domainName.ToString();

            trustee.Properties["Name"].Value = userName;

            trustee.Properties["SID"].Value = userSid;

            trustee.Properties["SidLength"].Value = 28;

            //trustee.Properties["SIDString"].Value="S-1-5-21-602162358-708899826-854245398-1005"; 

            ace.Properties["Trustee"].Value = trustee;

            ace.Properties["AccessMask"].Value = 2032127;

            ace.Properties["AceFlags"].Value = 3;

            ace.Properties["AceType"].Value = 0;

            //复制一份访问控制列表，并将以上生成的访问控制项添加到其后。 

            ManagementBaseObject[] newDacl = new ManagementBaseObject[DaclObject.Length + 1];

            for (int i = 0; i < DaclObject.Length; i++)
            {

                newDacl[i] = DaclObject[i];

            }

            newDacl[DaclObject.Length] = ace;

            //将安全描述符的DACL属性设为新生成的访问控制列表 

            Descriptor.Properties["Dacl"].Value = newDacl;

            //设置安全描述符 

            dir.Scope.Options.EnablePrivileges = true;

            ManagementBaseObject inProperties = dir.GetMethodParameters("SetSecurityDescriptor");

            inProperties["Descriptor"] = Descriptor;

            outParams = dir.InvokeMethod("SetSecurityDescriptor", inProperties, null);
        }

        /// <summary>
        //获取安全选项里访问控制用户
        /// </summary>
        /// 
        public bool FindUserTrustee(string filePath, string userName)
        {
            ArrayList trusteesName = new ArrayList();
            ManagementPath path = new ManagementPath();

            path.Server = ".";
            path.NamespacePath = @"root\cimv2";
            path.RelativePath = @"Win32_LogicalFileSecuritySetting.Path='" + filePath + "'";//定位到文件夹或文件

            ManagementObject dir = new ManagementObject(path);

            ManagementBaseObject outParams = dir.InvokeMethod("GetSecurityDescriptor", null, null); //获取安全描述符 

            if (((uint)(outParams.Properties["ReturnValue"].Value)) != 0) //OK 
            {
                throw new Exception("Failed in getting file descriptor");
            }

            ManagementBaseObject Descriptor = ((ManagementBaseObject)(outParams.Properties["Descriptor"].Value));

            ManagementBaseObject[] DaclObject = ((ManagementBaseObject[])(Descriptor.Properties["Dacl"].Value)); //获取访问控制用户列表

            for (int i = 0; i < DaclObject.Length; i++)
            {
                trusteesName.Add(((ManagementBaseObject)DaclObject[i].Properties["Trustee"].Value).Properties["Name"].Value);
            }
            return trusteesName.Contains(userName);
        }

        /// <summary>
        //Get Machine SerialNumber
        /// </summary> 
        public string  GetMachineSerialNumber()
        {
            string msg = null;
            System.Management.ManagementObjectSearcher cmicWmi = new System.Management.ManagementObjectSearcher("SELECT * FROM Win32_BaseBoard");
            foreach (System.Management.ManagementObject cmicWmiObj in cmicWmi.Get())
            {
                //Debug.WriteLine("bios序列号" & cmicWmiObj["SerialNumber"]); 
                 msg = "本机Bios序列号" + cmicWmiObj["product"];
            }
            return msg;
        }

        /// <summary>
        //Get Operating System Platform and Version
        /// </summary> 
        public string OSPlatform()
        {
            string msg = null;            
            OperatingSystem os = Environment.OSVersion;
            PlatformID pid = os.Platform;
            msg = pid.ToString();
            return msg;
        }

        /// <summary>
        /// Gets information indicating whether it is a 32 bits system
        /// </summary>
        /// <returns>true if it is 32, false otherwise</returns>
        public bool Is32BitMode()
        {
            return System.Runtime.InteropServices.Marshal.SizeOf(typeof(IntPtr)) == 4;
        }

        /// <summary>
        /// Gets information indicating whether it is a 64 bits system
        /// </summary>
        /// <returns>true if it is 64, false otherwise</returns>
        public bool Is64BitMode()
        {
            return System.Runtime.InteropServices.Marshal.SizeOf(typeof(IntPtr)) == 8;
        }

        /// <summary>
        /// Gets information indicating whether it is a 32 or 64 system
        /// </summary>
        /// <returns>string bit</returns>
        public string osBitMode()
        {
            string bit = null;
            int n = System.Runtime.InteropServices.Marshal.SizeOf(typeof(IntPtr));
            switch (n)
            {
                case 4:
                    bit = "32bit";
                    break;
                case 8:
                    bit = "64bit";
                    break;
                default:
                    bit = "32bit";
                    break;
            };

            return bit;
        }

     

        /// <summary>
        /// Gets directory of windows install programe.
        /// </summary>
        /// <returns>bool</returns>
        public bool SearchWindowsProgramDirectory(string diskName,string programDirectory)
        {
            bool is64BitProgramDir = false;
            string path = diskName + "\\";

            DirectoryInfo Dir = new DirectoryInfo(path);

            foreach (DirectoryInfo d in Dir.GetDirectories())     //查找子目录     
            {
                if (d.Name == programDirectory)
                {
                    is64BitProgramDir = true;
                    break;
                }
            }
            return is64BitProgramDir;
        }

        /// <summary>
        /// 得到当前程序bin的父目录
        /// </summary>
        private string getInstallPath(string dirName)
        {
            char[] sep = new char[] { '\\' };
            string[] split = dirName.Split(sep);
            int n = split.Length;
            string programPath=null;

            foreach (string iPath in split)
            {
                if (iPath != "bin")
                {
                    programPath += iPath + "\\";
                }
                else
                {
                    break;
                }
            }
            return dirName=programPath;
        }

        /// <summary>
        /// 动态获取程序路径
        /// Gets directory of windows install programe.
        /// </summary>
        private string basePath()
        {
            ebaaasInstallPath = System.Environment.CurrentDirectory.ToString();

            //动态获取程序路径
            SetOperatingSystem getInstall = new SetOperatingSystem();

            ebaaasInstallPath = getInstall.getInstallPath(ebaaasInstallPath);
            //安装路径调试用
            //ebaaasInstallPath = "C:\\Program Files\\Newtera\\";

            return ebaaasInstallPath;
        }
    }

}

