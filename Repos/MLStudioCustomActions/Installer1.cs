using System;
using System.IO;
using System.Xml;
using System.Xml.XPath;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Configuration.Install;
using System.Linq;
using System.Threading.Tasks;
using System.Security.Principal;
using System.Security.AccessControl;

namespace MLStudioCustomActions
{
    [RunInstaller(true)]
    public partial class Installer1 : System.Configuration.Install.Installer
    {
        public Installer1()
        {
            InitializeComponent();
        }

        [System.Security.Permissions.SecurityPermission(System.Security.Permissions.SecurityAction.Demand)]
        public override void Install(IDictionary stateSaver)
        {
            // This gets the named parameters passed in from your custom action
            string folder = Context.Parameters["targetdir"];

            if (Directory.Exists(folder))
            {
                DirectoryInfo dInfo = new DirectoryInfo(folder);
                ReplaceAllDescendantPermissionsFromObject(dInfo);
            }
            else
            {
                throw new Exception(folder + " diretory doesn't exist");
            }

            // update app.config
            UpdateConfigFile(folder);

            base.Install(stateSaver);
        }

        [System.Security.Permissions.SecurityPermission(System.Security.Permissions.SecurityAction.Demand)]
        public override void Commit(IDictionary savedState)
        {
            base.Commit(savedState);
            
        }

        [System.Security.Permissions.SecurityPermission(System.Security.Permissions.SecurityAction.Demand)]
        public override void Rollback(IDictionary savedState)
        {
            base.Rollback(savedState);
        }

        private void ReplaceAllDescendantPermissionsFromObject(
            DirectoryInfo dInfo)
        {
            DirectorySecurity dSecurity = dInfo.GetAccessControl();
            dSecurity.AddAccessRule(new FileSystemAccessRule(new SecurityIdentifier(WellKnownSidType.WorldSid, null), FileSystemRights.FullControl, InheritanceFlags.ObjectInherit | InheritanceFlags.ContainerInherit, PropagationFlags.NoPropagateInherit, AccessControlType.Allow));

            dInfo.SetAccessControl(dSecurity);

            foreach (DirectoryInfo sdi in dInfo.GetDirectories())
            {
                ReplaceAllDescendantPermissionsFromObject(sdi);
            }
        }

        private void UpdateConfigFile(string targetDir)
        {
            string xml = targetDir + @"\bin\MLStudio.exe.config";

            XmlDocument document = new XmlDocument();
            document.Load(xml);
            XPathNavigator navigator = document.CreateNavigator();

            XmlNode setting = document.SelectSingleNode(@"/configuration/appSettings/add[@key='HomeDir']");
            XmlNode valAttr = setting.Attributes["value"];
            if (valAttr == null)
            {
                throw new Exception("Unable to update HomeDir in app config");
            }
            else
            {
                if (targetDir.EndsWith(@"\"))
                {
                    targetDir = targetDir.Substring(0, targetDir.Length - 1);
                }

                valAttr.Value = targetDir;
            }

            document.Save(xml);

        }
    }
}
