using System;
using System.IO;
using System.Xml;
using System.Collections;
using System.ComponentModel;
using System.Configuration.Install;
using System.Windows.Forms;
using System.Security.AccessControl;
using Microsoft.Win32;

//using Newtera.Common.Config;

namespace Newtera.CustomActions
{
	/// <summary>
	/// Summary description for Installer.
	/// </summary>
	[RunInstaller(true)]
	public class NewteraInstaller : System.Configuration.Install.Installer
	{
		private const string CONFIG_FILE = "EbaasServer.exe.config";
        private const string APP_SETTINGS = "appSettings";
        private const string KEY_NAME = "key";
        private const string VALUE_NAME = "value";
        private const string ADD = "add";
        private const string STATIC_FILE_ROOT = "StaticFilesRoot";
        private const string WEB_APP = "WebApp";
        private const string CONFIG_DIR = "Config";
        private const string REGISTRY_FILE = "Registry.xml";
        private const string HOME_DIR_KEY = "HOME_DIR";
        private const string TOOL_DIR_KEY = "TOOL_DIR";
        private const string WORKING_DIR = @"C:\Program Files\Ebaas\bin";

        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.Container components = null;

        public NewteraInstaller()
		{
			// This call is required by the Designer.
			InitializeComponent();
 
        }

		/// <summary> 
		/// Clean up any resources being used.
		/// </summary>
		protected override void Dispose( bool disposing )
		{
			if( disposing )
			{
				if(components != null)
				{
					components.Dispose();
				}
			}
			base.Dispose( disposing );
		}


		#region Component Designer generated code
		/// <summary>
		/// Required method for Designer support - do not modify
		/// the contents of this method with the code editor.
		/// </summary>
		private void InitializeComponent()
		{
			components = new System.ComponentModel.Container();
		}
		#endregion
	
		public override void Install(IDictionary stateSaver)
		{
            // TODO:  Add Installer.Install implementation
            base.Install (stateSaver);
        }

		/// <summary>
        /// Change registry values
		/// Create FileList.xml
        /// Set IIS
        /// Set trust Newtera.DataGridActiveX
		/// </summary>
		/// <param name="savedState"></param>
        public override void Commit(System.Collections.IDictionary savedState)
        {
            base.Commit(savedState);

            // Add everyone with full-control to installed directories
            //ModifyFoldersPermissions();
            AccessFolder();

            System.Diagnostics.Process process = new System.Diagnostics.Process();
            System.Diagnostics.ProcessStartInfo startInfo = new System.Diagnostics.ProcessStartInfo();
            startInfo.WindowStyle = System.Diagnostics.ProcessWindowStyle.Hidden;
            startInfo.WorkingDirectory = WORKING_DIR;
            startInfo.FileName = "cmd.exe";
            startInfo.Arguments = "/C ebaasserver.exe install";
            process.StartInfo = startInfo;
            process.Start();

            return;

            string installedPath = this.Context.Parameters["DIR"];

            try
            {
                UpdateConfigs(installedPath);

            }
            catch (Exception ex)
            {
                StreamWriter sw = new StreamWriter(@"C:\Program Files\Ebaas\InstallTrace.txt");

                sw.WriteLine("Installation failed with " + ex.Message + @"\n" + ex.StackTrace);
                sw.Flush();
                sw.Close();
            }
            
		}

        public override void Uninstall(IDictionary savedState)
        {
            System.Diagnostics.Process process = new System.Diagnostics.Process();
            System.Diagnostics.ProcessStartInfo startInfo = new System.Diagnostics.ProcessStartInfo();
            startInfo.WindowStyle = System.Diagnostics.ProcessWindowStyle.Hidden;
            startInfo.WorkingDirectory = WORKING_DIR;
            startInfo.FileName = "cmd.exe";
            startInfo.Arguments = "/C ebaasserver.exe uninstall";
            process.StartInfo = startInfo;
            process.Start();

            //base.Uninstall(savedState);
        }

        private void AccessFolder()
        {
            //设置文件夹权限
            SetOperatingSystem sos = new SetOperatingSystem();
            sos.SetFilePermiss();
        }

        private void ModifyFoldersPermissions()
        {
            string baseDir = @"C:\Program Files\Ebaas";
            DirectoryInfo directoryInfo = new DirectoryInfo(baseDir);

            DirectorySecurity directorySecurity = directoryInfo.GetAccessControl();

            DirectoryInfo[] dinfo = directoryInfo.GetDirectories();

            try
            {
                foreach (DirectoryInfo d in dinfo)
                {
                    //User = System.Environment.UserDomainName + "\\" + System.Environment.UserName;

                    directoryInfo = new DirectoryInfo(baseDir + "\\" + d.Name);

                    DirectorySecurity dSecurity = directoryInfo.GetAccessControl();

                    dSecurity.AddAccessRule(new FileSystemAccessRule("everyone", FileSystemRights.FullControl, InheritanceFlags.ObjectInherit | InheritanceFlags.ContainerInherit, PropagationFlags.InheritOnly, AccessControlType.Allow));
                    directoryInfo.SetAccessControl(dSecurity);
                    directoryInfo.Refresh();

                    StreamWriter sw = new StreamWriter(@"C:\Program Files\Ebaas\InstallTrace.txt");

                    sw.WriteLine("Set full control to everyone for " + baseDir + "\\" + d.Name);
                    sw.Flush();
                    sw.Close();
                }
            }
            catch (Exception)
            {
                // exception handling    
            }
        }

		/// <summary>
		/// Update EbaasServer.exe.config and Registry.xml
		/// </summary>
		private void UpdateConfigs(string installedPath)
		{
			if (!string.IsNullOrEmpty(installedPath))
			{
                string serverConfig = GetServerConfig(installedPath);

                if (File.Exists(serverConfig))
                {
                    XmlDocument doc = new XmlDocument();
                    doc.Load(serverConfig);

                    SetAppSetting(doc, STATIC_FILE_ROOT, GetWebAppPath(installedPath));

                    Flush(doc, serverConfig);
                }
                else
                {
                    throw new Exception(serverConfig + " does not exist.");
                }

                string registryConfig = GetRegistryConfig(installedPath);

                if (File.Exists(registryConfig))
                {
                    XmlDocument doc = new XmlDocument();
                    doc.Load(registryConfig);

                    SetSetting(doc, HOME_DIR_KEY, installedPath);

                    SetSetting(doc, TOOL_DIR_KEY, installedPath);

                    Flush(doc, registryConfig);
                }
                else
                {
                    throw new Exception(registryConfig + " does not exist.");
                }
            }
            else
            {
                StreamWriter sw = new StreamWriter(@"C:\Program Files\Ebaas\InstallTrace.txt");

                sw.WriteLine("Installed path is empty ");
                sw.Flush();
                sw.Close();
            }
		}

        private string GetServerConfig(string installedPath)
        {
            if (installedPath.EndsWith(@"\"))
            {
                return installedPath + CONFIG_FILE;
            }
            else
            {
                return installedPath + @"\" + CONFIG_FILE;
            }
        }

        private string GetRegistryConfig(string installedPath)
        {
            if (installedPath.EndsWith(@"\"))
            {
                return installedPath + CONFIG_DIR + @"\" + REGISTRY_FILE;
            }
            else
            {
                return installedPath + @"\" + CONFIG_DIR + @"\" + REGISTRY_FILE;
            }
        }

        private string GetWebAppPath(string installedPath)
        {
            if (installedPath.EndsWith(@"\"))
            {
                return installedPath + WEB_APP;
            }
            else
            {
                return installedPath + @"\" + WEB_APP;
            }
        }

        /// <summary>
        /// Sets an AppSetting value of a given key
        /// </summary>
        /// <param name="key">The key</param>
        /// <param name="value">The value</param>
        public void SetAppSetting(XmlDocument doc, string key, string value)
        {
            if (doc != null)
            {
                XmlElement appSettings = doc.DocumentElement[APP_SETTINGS];
                if (appSettings == null)
                {
                    appSettings = doc.CreateElement(APP_SETTINGS);
                    doc.DocumentElement.AppendChild(appSettings);
                }

                XmlElement appSetting = null;
                foreach (XmlNode node in appSettings.ChildNodes)
                {
                    if (node is XmlElement)
                    {
                        XmlElement element = (XmlElement)node;
                        if (element.GetAttribute(KEY_NAME) == key)
                        {
                            appSetting = element;
                            break;
                        }
                    }
                }

                if (appSetting == null)
                {
                    appSetting = doc.CreateElement(ADD);
                    appSetting.SetAttribute(KEY_NAME, key);
                    appSetting.SetAttribute(VALUE_NAME, value);
                    appSettings.AppendChild(appSetting);
                }
                else
                {
                    appSetting.SetAttribute(VALUE_NAME, value);
                }
            }
        }

        /// <summary>
        /// Sets a value of a given key in a given section.
        /// </summary>
        /// <param name="section">The section</param>
        /// <param name="key">The key</param>
        /// <param name="value">The value</param>
        public void SetSetting(XmlDocument doc, string key, string value)
        {
            if (doc != null)
            {
                XmlElement setting = null;
                foreach (XmlNode node in doc.DocumentElement.ChildNodes)
                {
                    if (node is XmlElement)
                    {
                        XmlElement element = (XmlElement)node;
                        if (element.Name == key)
                        {
                            setting = element;
                            break;
                        }
                    }
                }

                if (setting != null)
                {
                    setting.SetAttribute(VALUE_NAME, value);
                }
                else
                {
                    throw new Exception("Unable to find a element with name " + key + " in xml document");
                }
            }
        }

        private void Flush(XmlDocument doc, string configFileName)
        {
            XmlTextWriter writer = new XmlTextWriter(configFileName, null);
            writer.Formatting = Formatting.Indented;
            doc.WriteTo(writer);
            writer.Flush();
            writer.Close();
        }
    }
}
