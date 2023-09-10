using System;
using System.IO;
using System.Xml;
using System.Collections;
using System.ComponentModel;
using System.Security.AccessControl;
using System.Security.Principal;

namespace Newtera.CustomActions
{
	/// <summary>
	/// Summary description for Installer.
	/// </summary>
	[RunInstaller(true)]
	public class NewteraInstaller : System.Configuration.Install.Installer
	{
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
		/// </summary>
		/// <param name="savedState"></param>
        public override void Commit(System.Collections.IDictionary savedState)
        {
            base.Commit(savedState);

            // Add everyone with full-control to installed directories
            GrantAccessToFolders();
		}

        public override void Uninstall(IDictionary savedState)
        {
            base.Uninstall(savedState);
        }

        private void GrantAccessToFolders()
        {
            string baseDir = @"C:\Program Files\Newtera";

            // Create the rules
            FileSystemAccessRule fullControlRule = new FileSystemAccessRule(
                new SecurityIdentifier(WellKnownSidType.WorldSid, null),
                FileSystemRights.FullControl,
                InheritanceFlags.ObjectInherit | InheritanceFlags.ContainerInherit,
                PropagationFlags.NoPropagateInherit,
                AccessControlType.Allow);

            if (!string.IsNullOrEmpty(baseDir) && Directory.Exists(baseDir))
            {
                // Get your file's ACL
                DirectorySecurity fsecurity = Directory.GetAccessControl(baseDir);

                // Add the new rule to the ACL
                fsecurity.AddAccessRule(fullControlRule);

                // Set the ACL back to the file
                Directory.SetAccessControl(baseDir, fsecurity);
            }

            DirectoryInfo directoryInfo = new DirectoryInfo(baseDir);
            DirectoryInfo[] dinfo = directoryInfo.GetDirectories();

            try
            {
                foreach (DirectoryInfo d in dinfo)
                {
                    directoryInfo = new DirectoryInfo(baseDir + @"\" + d.Name);

                    DirectorySecurity dSecurity = directoryInfo.GetAccessControl();

                    dSecurity.AddAccessRule(fullControlRule);
                    directoryInfo.SetAccessControl(dSecurity);
                    directoryInfo.Refresh();
                }
            }
            catch (Exception)
            {
                // exception handling    
            }
        }
    }
}
