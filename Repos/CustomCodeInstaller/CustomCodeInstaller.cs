using System;
using System.IO;
using System.Drawing;
using System.Threading;
using System.Collections;
using System.ComponentModel;
using System.Windows.Forms;


namespace RestoreTool
{
	/// <summary>
	/// CustomCodeInstaller 的摘要说明。
	/// </summary>
	public class CCI_Form : System.Windows.Forms.Form
	{
		private System.Windows.Forms.MainMenu mainMenu1;
		private System.Windows.Forms.MenuItem File_MI;
		private System.Windows.Forms.MenuItem Tool_MI;
		private System.Windows.Forms.MenuItem Help_MI;
		private System.Windows.Forms.ToolBar toolBar1;
		private System.Windows.Forms.ImageList imageList1;
		private System.Windows.Forms.ToolBarButton Search_tbbt;
		private System.Windows.Forms.ToolBarButton save_tbbt;
		private System.Windows.Forms.ToolBarButton open_tbbt;
		private System.Windows.Forms.ToolBarButton restore_tbbt;
		private System.Windows.Forms.Panel panel1;
		private System.Windows.Forms.StatusBar statusBar1;
		private System.Windows.Forms.StatusBarPanel statusBarPanel1;
		private System.Windows.Forms.ListView listView1;
		private System.Windows.Forms.ColumnHeader Name_LV;
		private System.Windows.Forms.ColumnHeader Size_LV;
		private System.Windows.Forms.ColumnHeader Type_LV;
		private System.Windows.Forms.ColumnHeader Path_LV;
		private System.Windows.Forms.ColumnHeader Modified_LV;
		private System.Windows.Forms.ToolBarButton Separator1;
		private System.Windows.Forms.ToolBarButton Separator2;
		private System.Windows.Forms.ToolBarButton Separator3;
		private System.ComponentModel.IContainer components;
		private System.Windows.Forms.StatusBarPanel statusBarPanel2;
		private System.Windows.Forms.StatusBarPanel statusBarPanel3;
		private System.Windows.Forms.ToolBarButton Add_tbbt;
		private System.Windows.Forms.ToolBarButton Remove_tbbt;
		private System.Windows.Forms.ToolBarButton View_tbbt;
		private System.Windows.Forms.ToolBarButton Separator4;
		private System.Windows.Forms.ToolBarButton Help_tbbt;
		private System.Windows.Forms.MenuItem Search_MI;
		private System.Windows.Forms.MenuItem Restore_MI;
		private System.Windows.Forms.MenuItem Add_MI;
		private System.Windows.Forms.MenuItem Remove_MI;
		private System.Windows.Forms.MenuItem View_MI;
		private System.Windows.Forms.MenuItem Open_MI;
		private System.Windows.Forms.MenuItem Save_MI;
		private System.Windows.Forms.MenuItem Exit_MI;
		private System.Windows.Forms.MenuItem Using_MI;
		private System.Windows.Forms.MenuItem About_MI;
		private System.Windows.Forms.ContextMenu contextMenu1;
		private System.Windows.Forms.MenuItem ctxViewMenuItem;
		private System.Windows.Forms.MenuItem ctxAddMenuItem;
		private System.Windows.Forms.MenuItem ctxRemoveMenuItem;
		private bool _layoutCalled = false;
		private bool tag=true;
        private string opSystem;
        
		public CCI_Form()
		{
			//
			// Windows 窗体设计器支持所必需的
			//
			InitializeComponent();
			this.toolBar1.Buttons[2].Enabled=false;
			this.toolBar1.Buttons[5].Enabled=false;
			this.toolBar1.Buttons[7].Enabled=false;
			this.toolBar1.Buttons[8].Enabled=false;
			this.toolBar1.Buttons[9].Enabled=false;
			this.Save_MI.Enabled=false;
			this.Restore_MI.Enabled=false;
			this.View_MI.Enabled=false;
			this.Add_MI.Enabled=false;
			this.Remove_MI.Enabled=false;
			contextMenuItem_Enabled();

            opSystem = DataProviders.GetServerSystem();
			//
			// TODO: 在 InitializeComponent 调用后添加任何构造函数代码
			//
		}

		/// <summary>
		/// 清理所有正在使用的资源。
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

		#region Windows 窗体设计器生成的代码
		/// <summary>
		/// 设计器支持所需的方法 - 不要使用代码编辑器修改
		/// 此方法的内容。
		/// </summary>
		private void InitializeComponent()
		{
            this.components = new System.ComponentModel.Container();
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(CCI_Form));
            this.mainMenu1 = new System.Windows.Forms.MainMenu(this.components);
            this.File_MI = new System.Windows.Forms.MenuItem();
            this.Open_MI = new System.Windows.Forms.MenuItem();
            this.Save_MI = new System.Windows.Forms.MenuItem();
            this.Exit_MI = new System.Windows.Forms.MenuItem();
            this.Tool_MI = new System.Windows.Forms.MenuItem();
            this.Search_MI = new System.Windows.Forms.MenuItem();
            this.Restore_MI = new System.Windows.Forms.MenuItem();
            this.Add_MI = new System.Windows.Forms.MenuItem();
            this.Remove_MI = new System.Windows.Forms.MenuItem();
            this.View_MI = new System.Windows.Forms.MenuItem();
            this.Help_MI = new System.Windows.Forms.MenuItem();
            this.Using_MI = new System.Windows.Forms.MenuItem();
            this.About_MI = new System.Windows.Forms.MenuItem();
            this.toolBar1 = new System.Windows.Forms.ToolBar();
            this.Separator1 = new System.Windows.Forms.ToolBarButton();
            this.Search_tbbt = new System.Windows.Forms.ToolBarButton();
            this.save_tbbt = new System.Windows.Forms.ToolBarButton();
            this.Separator2 = new System.Windows.Forms.ToolBarButton();
            this.open_tbbt = new System.Windows.Forms.ToolBarButton();
            this.restore_tbbt = new System.Windows.Forms.ToolBarButton();
            this.Separator3 = new System.Windows.Forms.ToolBarButton();
            this.Add_tbbt = new System.Windows.Forms.ToolBarButton();
            this.Remove_tbbt = new System.Windows.Forms.ToolBarButton();
            this.View_tbbt = new System.Windows.Forms.ToolBarButton();
            this.Separator4 = new System.Windows.Forms.ToolBarButton();
            this.Help_tbbt = new System.Windows.Forms.ToolBarButton();
            this.imageList1 = new System.Windows.Forms.ImageList(this.components);
            this.panel1 = new System.Windows.Forms.Panel();
            this.listView1 = new System.Windows.Forms.ListView();
            this.Name_LV = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.Size_LV = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.Type_LV = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.Path_LV = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.Modified_LV = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.contextMenu1 = new System.Windows.Forms.ContextMenu();
            this.ctxViewMenuItem = new System.Windows.Forms.MenuItem();
            this.ctxAddMenuItem = new System.Windows.Forms.MenuItem();
            this.ctxRemoveMenuItem = new System.Windows.Forms.MenuItem();
            this.statusBar1 = new System.Windows.Forms.StatusBar();
            this.statusBarPanel1 = new System.Windows.Forms.StatusBarPanel();
            this.statusBarPanel2 = new System.Windows.Forms.StatusBarPanel();
            this.statusBarPanel3 = new System.Windows.Forms.StatusBarPanel();
            this.panel1.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.statusBarPanel1)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.statusBarPanel2)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.statusBarPanel3)).BeginInit();
            this.SuspendLayout();
            // 
            // mainMenu1
            // 
            this.mainMenu1.MenuItems.AddRange(new System.Windows.Forms.MenuItem[] {
            this.File_MI,
            this.Tool_MI,
            this.Help_MI});
            // 
            // File_MI
            // 
            this.File_MI.Index = 0;
            this.File_MI.MenuItems.AddRange(new System.Windows.Forms.MenuItem[] {
            this.Open_MI,
            this.Save_MI,
            this.Exit_MI});
            resources.ApplyResources(this.File_MI, "File_MI");
            // 
            // Open_MI
            // 
            this.Open_MI.Index = 0;
            resources.ApplyResources(this.Open_MI, "Open_MI");
            this.Open_MI.Click += new System.EventHandler(this.menuItem1_Click);
            // 
            // Save_MI
            // 
            this.Save_MI.Index = 1;
            resources.ApplyResources(this.Save_MI, "Save_MI");
            this.Save_MI.Click += new System.EventHandler(this.menuItem2_Click);
            // 
            // Exit_MI
            // 
            this.Exit_MI.Index = 2;
            resources.ApplyResources(this.Exit_MI, "Exit_MI");
            this.Exit_MI.Click += new System.EventHandler(this.menuItem5_Click);
            // 
            // Tool_MI
            // 
            this.Tool_MI.Index = 1;
            this.Tool_MI.MenuItems.AddRange(new System.Windows.Forms.MenuItem[] {
            this.Search_MI,
            this.Restore_MI,
            this.Add_MI,
            this.Remove_MI,
            this.View_MI});
            resources.ApplyResources(this.Tool_MI, "Tool_MI");
            // 
            // Search_MI
            // 
            this.Search_MI.Index = 0;
            resources.ApplyResources(this.Search_MI, "Search_MI");
            this.Search_MI.Click += new System.EventHandler(this.menuItem3_Click);
            // 
            // Restore_MI
            // 
            this.Restore_MI.Index = 1;
            resources.ApplyResources(this.Restore_MI, "Restore_MI");
            this.Restore_MI.Click += new System.EventHandler(this.menuItem6_Click);
            // 
            // Add_MI
            // 
            this.Add_MI.Index = 2;
            resources.ApplyResources(this.Add_MI, "Add_MI");
            this.Add_MI.Click += new System.EventHandler(this.Add_MI_Click);
            // 
            // Remove_MI
            // 
            this.Remove_MI.Index = 3;
            resources.ApplyResources(this.Remove_MI, "Remove_MI");
            this.Remove_MI.Click += new System.EventHandler(this.Remove_MI_Click);
            // 
            // View_MI
            // 
            this.View_MI.Index = 4;
            resources.ApplyResources(this.View_MI, "View_MI");
            // 
            // Help_MI
            // 
            this.Help_MI.Index = 2;
            this.Help_MI.MenuItems.AddRange(new System.Windows.Forms.MenuItem[] {
            this.Using_MI,
            this.About_MI});
            resources.ApplyResources(this.Help_MI, "Help_MI");
            // 
            // Using_MI
            // 
            this.Using_MI.Index = 0;
            resources.ApplyResources(this.Using_MI, "Using_MI");
            this.Using_MI.Click += new System.EventHandler(this.Using_MI_Click);
            // 
            // About_MI
            // 
            this.About_MI.Index = 1;
            resources.ApplyResources(this.About_MI, "About_MI");
            this.About_MI.Click += new System.EventHandler(this.About_MI_Click);
            // 
            // toolBar1
            // 
            this.toolBar1.AccessibleRole = System.Windows.Forms.AccessibleRole.None;
            resources.ApplyResources(this.toolBar1, "toolBar1");
            this.toolBar1.Buttons.AddRange(new System.Windows.Forms.ToolBarButton[] {
            this.Separator1,
            this.Search_tbbt,
            this.save_tbbt,
            this.Separator2,
            this.open_tbbt,
            this.restore_tbbt,
            this.Separator3,
            this.Add_tbbt,
            this.Remove_tbbt,
            this.View_tbbt,
            this.Separator4,
            this.Help_tbbt});
            this.toolBar1.ImageList = this.imageList1;
            this.toolBar1.Name = "toolBar1";
            this.toolBar1.ButtonClick += new System.Windows.Forms.ToolBarButtonClickEventHandler(this.toolBar1_ButtonClick);
            // 
            // Separator1
            // 
            resources.ApplyResources(this.Separator1, "Separator1");
            this.Separator1.Name = "Separator1";
            this.Separator1.Style = System.Windows.Forms.ToolBarButtonStyle.Separator;
            // 
            // Search_tbbt
            // 
            resources.ApplyResources(this.Search_tbbt, "Search_tbbt");
            this.Search_tbbt.Name = "Search_tbbt";
            // 
            // save_tbbt
            // 
            resources.ApplyResources(this.save_tbbt, "save_tbbt");
            this.save_tbbt.Name = "save_tbbt";
            // 
            // Separator2
            // 
            this.Separator2.Name = "Separator2";
            this.Separator2.Style = System.Windows.Forms.ToolBarButtonStyle.Separator;
            // 
            // open_tbbt
            // 
            resources.ApplyResources(this.open_tbbt, "open_tbbt");
            this.open_tbbt.Name = "open_tbbt";
            // 
            // restore_tbbt
            // 
            resources.ApplyResources(this.restore_tbbt, "restore_tbbt");
            this.restore_tbbt.Name = "restore_tbbt";
            // 
            // Separator3
            // 
            this.Separator3.Name = "Separator3";
            this.Separator3.Style = System.Windows.Forms.ToolBarButtonStyle.Separator;
            // 
            // Add_tbbt
            // 
            resources.ApplyResources(this.Add_tbbt, "Add_tbbt");
            this.Add_tbbt.Name = "Add_tbbt";
            // 
            // Remove_tbbt
            // 
            resources.ApplyResources(this.Remove_tbbt, "Remove_tbbt");
            this.Remove_tbbt.Name = "Remove_tbbt";
            // 
            // View_tbbt
            // 
            resources.ApplyResources(this.View_tbbt, "View_tbbt");
            this.View_tbbt.Name = "View_tbbt";
            // 
            // Separator4
            // 
            this.Separator4.Name = "Separator4";
            this.Separator4.Style = System.Windows.Forms.ToolBarButtonStyle.Separator;
            // 
            // Help_tbbt
            // 
            resources.ApplyResources(this.Help_tbbt, "Help_tbbt");
            this.Help_tbbt.Name = "Help_tbbt";
            // 
            // imageList1
            // 
            this.imageList1.ImageStream = ((System.Windows.Forms.ImageListStreamer)(resources.GetObject("imageList1.ImageStream")));
            this.imageList1.TransparentColor = System.Drawing.SystemColors.Control;
            this.imageList1.Images.SetKeyName(0, "");
            this.imageList1.Images.SetKeyName(1, "");
            this.imageList1.Images.SetKeyName(2, "");
            this.imageList1.Images.SetKeyName(3, "");
            this.imageList1.Images.SetKeyName(4, "");
            this.imageList1.Images.SetKeyName(5, "");
            this.imageList1.Images.SetKeyName(6, "");
            this.imageList1.Images.SetKeyName(7, "");
            // 
            // panel1
            // 
            resources.ApplyResources(this.panel1, "panel1");
            this.panel1.BackColor = System.Drawing.SystemColors.ActiveCaptionText;
            this.panel1.BorderStyle = System.Windows.Forms.BorderStyle.Fixed3D;
            this.panel1.Controls.Add(this.listView1);
            this.panel1.Name = "panel1";
            // 
            // listView1
            // 
            this.listView1.Activation = System.Windows.Forms.ItemActivation.OneClick;
            this.listView1.AllowColumnReorder = true;
            this.listView1.BorderStyle = System.Windows.Forms.BorderStyle.None;
            this.listView1.Columns.AddRange(new System.Windows.Forms.ColumnHeader[] {
            this.Name_LV,
            this.Size_LV,
            this.Type_LV,
            this.Path_LV,
            this.Modified_LV});
            this.listView1.ContextMenu = this.contextMenu1;
            resources.ApplyResources(this.listView1, "listView1");
            this.listView1.FullRowSelect = true;
            this.listView1.Name = "listView1";
            this.listView1.UseCompatibleStateImageBehavior = false;
            this.listView1.View = System.Windows.Forms.View.Details;
            this.listView1.SelectedIndexChanged += new System.EventHandler(this.listView1_SelectedIndexChanged);
            this.listView1.DoubleClick += new System.EventHandler(this.listView1_DoubleClick);
            this.listView1.MouseDown += new System.Windows.Forms.MouseEventHandler(this.listView1_MouseDown);
            // 
            // Name_LV
            // 
            resources.ApplyResources(this.Name_LV, "Name_LV");
            // 
            // Size_LV
            // 
            resources.ApplyResources(this.Size_LV, "Size_LV");
            // 
            // Type_LV
            // 
            resources.ApplyResources(this.Type_LV, "Type_LV");
            // 
            // Path_LV
            // 
            resources.ApplyResources(this.Path_LV, "Path_LV");
            // 
            // Modified_LV
            // 
            resources.ApplyResources(this.Modified_LV, "Modified_LV");
            // 
            // contextMenu1
            // 
            this.contextMenu1.MenuItems.AddRange(new System.Windows.Forms.MenuItem[] {
            this.ctxViewMenuItem,
            this.ctxAddMenuItem,
            this.ctxRemoveMenuItem});
            // 
            // ctxViewMenuItem
            // 
            this.ctxViewMenuItem.Index = 0;
            resources.ApplyResources(this.ctxViewMenuItem, "ctxViewMenuItem");
            this.ctxViewMenuItem.Click += new System.EventHandler(this.ctxViewMenuItem_Click);
            // 
            // ctxAddMenuItem
            // 
            this.ctxAddMenuItem.Index = 1;
            resources.ApplyResources(this.ctxAddMenuItem, "ctxAddMenuItem");
            this.ctxAddMenuItem.Click += new System.EventHandler(this.ctxAddMenuItem_Click);
            // 
            // ctxRemoveMenuItem
            // 
            this.ctxRemoveMenuItem.Index = 2;
            resources.ApplyResources(this.ctxRemoveMenuItem, "ctxRemoveMenuItem");
            this.ctxRemoveMenuItem.Click += new System.EventHandler(this.ctxRemoveMenuItem_Click);
            // 
            // statusBar1
            // 
            this.statusBar1.Cursor = System.Windows.Forms.Cursors.Default;
            resources.ApplyResources(this.statusBar1, "statusBar1");
            this.statusBar1.Name = "statusBar1";
            this.statusBar1.Panels.AddRange(new System.Windows.Forms.StatusBarPanel[] {
            this.statusBarPanel1,
            this.statusBarPanel2,
            this.statusBarPanel3});
            this.statusBar1.ShowPanels = true;
            // 
            // statusBarPanel1
            // 
            this.statusBarPanel1.AutoSize = System.Windows.Forms.StatusBarPanelAutoSize.Spring;
            resources.ApplyResources(this.statusBarPanel1, "statusBarPanel1");
            // 
            // statusBarPanel2
            // 
            resources.ApplyResources(this.statusBarPanel2, "statusBarPanel2");
            // 
            // statusBarPanel3
            // 
            resources.ApplyResources(this.statusBarPanel3, "statusBarPanel3");
            // 
            // CCI_Form
            // 
            resources.ApplyResources(this, "$this");
            this.Controls.Add(this.statusBar1);
            this.Controls.Add(this.panel1);
            this.Controls.Add(this.toolBar1);
            this.Menu = this.mainMenu1;
            this.Name = "CCI_Form";
            this.Layout += new System.Windows.Forms.LayoutEventHandler(this.CCI_Layout);
            this.panel1.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.statusBarPanel1)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.statusBarPanel2)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.statusBarPanel3)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

		}
		#endregion

        [STAThread]
        static void Main() 
		{
			try 
			{
                if (Thread.CurrentThread.CurrentUICulture.Name.ToUpper() == "ZH-CN")
                {
                    SplashScreen.ShowSplashScreen();
                    //SplashScreen.SetStatus(NewteraNameSpace.RELEASE_VERSION);
                    System.Threading.Thread.Sleep(3500);
                }

                DataProviders dpd = new DataProviders();
                dpd.ProcessKill();

				Application.Run(new CCI_Form());

			}
			catch (Exception e)
			{
				string msg = e.Message;
                MessageBox.Show(msg, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
			}
		}

		
		//fader out
		private void CCI_Layout(object sender, System.Windows.Forms.LayoutEventArgs e)
		{
			if( _layoutCalled == false )
			{
				_layoutCalled = true;
				if( SplashScreen.SplashForm != null )
					SplashScreen.SplashForm.Owner = this;
				this.Activate();
				SplashScreen.CloseForm();
			}
		}

        //定义一个DataProviders新的实例dp
        DataProviders dp = new DataProviders();


		private void toolBar1_ButtonClick(object sender, System.Windows.Forms.ToolBarButtonClickEventArgs e)
		{
			// Evaluate the Button property to determine which button was clicked.
			switch(toolBar1.Buttons.IndexOf(e.Button))
			{
				case 0:
					//openFileDialog1.ShowDialog();
					// Insert code to open the file.
					break; 
				case 1:
					//Search File
					SearchCustomFile ();
					break; 
				case 2:
					//Pack File Save File
					 packCustomFile();
					break; 
				case 4:
					//Pack File
					OpenCustomFile();
					break; 
				case 5:
					//UnPack File
					UnpackCustomFile();
					break; 
				case 7:
					AddCustomFile();
					break; 
				case 8:
					RemoveCustomFile();
					break; 
				case 9:
					ViewCustomFile();
					break; 
				case 11:
					showhelp();
					break; 
			}
		}
		//Search custom code file directory
		private string [] InstallPath;
		//private int n_HOME_DIR;
		//private int n_TOOL_DIR;
        private string[] myPath;

        private void SearchCustomFile()
        {
            string path1 = null, path2 = null;
            try
            {
                this.toolBar1.Buttons[2].Enabled = true;
                this.toolBar1.Buttons[5].Enabled = false;
                this.toolBar1.Buttons[7].Enabled = true;
                this.toolBar1.Buttons[8].Enabled = true;
                this.toolBar1.Buttons[9].Enabled = true;
                this.Save_MI.Enabled = true;
                this.Add_MI.Enabled = true;
                this.Remove_MI.Enabled = true;
                this.View_MI.Enabled = true;
                this.Restore_MI.Enabled = false;


                //从注册表里获取newtera TDM 的安装路径
                InstallPath = dp.HKEYPath();

                if (InstallPath[0] == "HOME_DIR")
                {
                    //n_HOME_DIR = InstallPath[1].Length;
                    //n_TOOL_DIR = InstallPath[3].Length;

                    //设置搜索文件目录
                    path1 = InstallPath[1];
                    path2 = InstallPath[3];
                    myPath = new string[] {path1, path2};
                }
                else if (InstallPath[0] == "TOOL_DIR")
                {
                    //n_TOOL_DIR = InstallPath[1].Length;
                    //设置搜索文件目录
                    path1 = InstallPath[1];
                    myPath = new string[] {path1};
                }

                string installer_directoryname = null;
                string installer_directorpath = null;                

                //读取原文档FileList.xml目录文件
                string FileListPath = InstallPath[1] + "FileList.xml";
                string NodeName = "list";

                    if (File.Exists(FileListPath))
                    {
                        dp.ReadXMLFile(statusBarPanel1, FileListPath, NodeName);
                        //清空ListView
                        listView1.Items.Clear();
                        //清空用户文件存放临时数组tempAL
                        dp.tempAL.Clear();
                        dp.tempBL.Clear();
                        dp.tempCL.Clear();
                        dp.tempDL.Clear();

                        //对指定文件夹遍历搜索比较，找出用户添加的文件并在ListView中显示
                        if (InstallPath[0] == "HOME_DIR")//服务器版
                        {
                            //设置路径一维数组
                            //对指定文件夹遍历搜索
                            foreach (string path in myPath)
                            {
                                if (path != InstallPath[3])
                                {
                                    installer_directoryname = InstallPath[0];
                                    installer_directorpath = InstallPath[1];
                                    dp.GetDirectoryFile(listView1, statusBarPanel1, installer_directoryname, installer_directorpath, path);
                                }
                                else
                                {
                                    installer_directoryname = InstallPath[2];
                                    installer_directorpath = InstallPath[3];
                                    dp.GetDirectoryFile(listView1, statusBarPanel1, installer_directoryname, installer_directorpath, path);
                                }
                            }
                        }
                        else if (InstallPath[0] == "TOOL_DIR")//客户版
                        {
                            myPath = new string[] { path1, path2 };
                            //对指定文件夹遍历搜索
                            foreach (string path in myPath)
                            {
                                installer_directoryname = InstallPath[0];
                                installer_directorpath = InstallPath[1];
                                dp.GetDirectoryFile(listView1, statusBarPanel1, installer_directoryname, installer_directorpath, path);
                            }
                        }
                    }
                    else
                    {
                        string msg = "没有足够权限读取" + FileListPath + "文件！请与系统管理员联系。";
                        MessageBox.Show(msg, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    }
            }
            catch (Exception e)
            {
                MessageBox.Show(e.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            //contextMenuItem Enbled
            tag = true;
            contextMenuItem_Enabled();
        }

		//保存文件
		private void packCustomFile()
		{
            //MessageBox.Show(opSystem, "Info", MessageBoxButtons.OK, MessageBoxIcon.Information);
			try
			{
				if (dp.tempAL.Count!=0)//没有用户文件跳过
				{
					SaveFileDialog saveFileDialog1 = new SaveFileDialog();
					saveFileDialog1.InitialDirectory = "c:\\";
					saveFileDialog1.Filter = "CCI File (*.cci)|*.cci";
					saveFileDialog1.FilterIndex = 1 ;
					saveFileDialog1.RestoreDirectory = false ;
                    //For Window7                    
                    //if(opSystem=="Windows 7 / Windows Server 2008")
                    //   saveFileDialog1.AutoUpgradeEnabled = false;
 
					if(saveFileDialog1.ShowDialog() == DialogResult.OK)
					{
						string packFileName=saveFileDialog1.FileName;
						dp.PackFiles(packFileName);	
						statusBarPanel1.Text=MessageResourceManager.GetString("SaveComplete");
						MessageBox.Show(MessageResourceManager.GetString("SaveComplete"),"Info",MessageBoxButtons.OK,MessageBoxIcon.Information);
					}
					saveFileDialog1.Dispose();
				}
				else
				{
					throw new Exception(MessageResourceManager.GetString("NoCustomFile"));
				}
			}
			catch (Exception e)
			{
				MessageBox.Show(e.Message,"Error",MessageBoxButtons.OK,MessageBoxIcon.Error);
			}
		}

		//打开备份文件
		private string packFileName;
		private void OpenCustomFile()
		{
			this.toolBar1.Buttons[5].Enabled=true;
			this.toolBar1.Buttons[2].Enabled=false;
			this.toolBar1.Buttons[7].Enabled=false;
			this.toolBar1.Buttons[8].Enabled=true;
			this.toolBar1.Buttons[9].Enabled=true;
			this.Restore_MI.Enabled=true;
			this.Remove_MI.Enabled=true;
			this.View_MI.Enabled=false;
			this.Add_MI.Enabled=false;
			this.Add_tbbt.Enabled=false;
			
			
			dp.tempAL.Clear();
			dp.tempBL.Clear();
			dp.tempCL.Clear();
			dp.tempDL.Clear();

			try
			{
                //MessageBox.Show(opSystem, "Info", MessageBoxButtons.OK, MessageBoxIcon.Information);

				OpenFileDialog openFileDialog1 = new OpenFileDialog();

				openFileDialog1.InitialDirectory = "c:\\" ;
				openFileDialog1.Filter = "All files (*.*)|*.*|Pack File (*.cci)|*.cci";
				openFileDialog1.FilterIndex = 2 ;
				openFileDialog1.RestoreDirectory = true ;
                //For window7 system
                //if (opSystem == "Windows 7 / Windows Server 2008")
                //     openFileDialog1.AutoUpgradeEnabled = false;

				if(openFileDialog1.ShowDialog() == DialogResult.OK)
				{
					//清空ListView
					listView1.Items.Clear();

					packFileName=openFileDialog1.FileName;
					int _index=packFileName.LastIndexOf(".");
					string packFileNameExtention=packFileName.Substring(_index+1);
					if (packFileNameExtention=="cci")
					{
						dp.OpenFiles(listView1,statusBarPanel1,packFileName);
						//Enbled contextMenuItem
						tag=false;
						contextMenuItem_Enabled();
					}
					else
					{
						throw new Exception(MessageResourceManager.GetString("FileType"));
					}
				}
				//释放对话框
				openFileDialog1.Dispose();
			}
			catch (Exception e)
			{
				MessageBox.Show(e.Message,"Error",MessageBoxButtons.OK,MessageBoxIcon.Error);
			}
		}
		private void UnpackCustomFile()
		{
			try
			{
				dp.UnpackFiles();
				statusBarPanel1.Text=MessageResourceManager.GetString("FileRestored");
				MessageBox.Show(MessageResourceManager.GetString("FileRestored"),"Info",MessageBoxButtons.OK,MessageBoxIcon.Information);
			}
			catch (Exception e)
			{
				MessageBox.Show(e.Message,"Error",MessageBoxButtons.OK,MessageBoxIcon.Error);
			}
		}


		private void menuItem1_Click(object sender, System.EventArgs e)
		{
		  OpenCustomFile();
		}

		private void menuItem2_Click(object sender, System.EventArgs e)
		{
		   packCustomFile();
		}

		private void menuItem3_Click(object sender, System.EventArgs e)
		{
		    SearchCustomFile ();
		}

		private void menuItem6_Click(object sender, System.EventArgs e)
		{
		    UnpackCustomFile();
		}

		private void menuItem5_Click(object sender, System.EventArgs e)
		{
		    Application.Exit();
		}
		//************************************鼠标右键事件、弹出面版*******************************************
		private  bool eventFalg=false;
		private int lvX;
		private int lvY;
		private int FormHeight;
		private int FormWith;
		private const int PopPanelHeight=50;
		private const int PopPanelWith=80;
		private int lvIndex=-1;

		private void listView1_MouseDown(object sender, System.Windows.Forms.MouseEventArgs e)
		{
			FormHeight=this.Height;
			FormWith=this.Width;

			if ((e.X+PopPanelWith)>=FormWith)
			{
				lvX=FormWith-PopPanelWith-18;
			}
			else{lvX=e.X;}
			if ((e.Y+PopPanelHeight)>=(FormHeight-100))
			{
				lvY=FormHeight-PopPanelHeight*3/2-70;
			}
			else{lvY=e.Y;}
			//判断是鼠标那个键
			if (e.Button.ToString()=="Right" && lvIndex==-1)//右键
			{
				eventFalg=false;
				contextMenuItem_Enabled();
			}
			else if (e.Button.ToString()=="Left" && lvIndex==-1)//左键
			{
				eventFalg=false;
				contextMenuItem_Enabled();
			}
		
		}

		private void listView1_SelectedIndexChanged(object sender, System.EventArgs e)
		{

            if (this.listView1.FocusedItem != null)
            {
                lvIndex = this.listView1.FocusedItem.Index;
			    this.statusBarPanel1.Text=this.listView1.FocusedItem.Text;
			    this.statusBarPanel2.Text=this.listView1.FocusedItem.SubItems[1].Text;
			    this.statusBarPanel3.Text=this.listView1.FocusedItem.SubItems[2].Text;
			
			    if (eventFalg){
				    //this.panel2.Visible=false;
			    }
			    else
			    {
				    eventFalg=true;
			    }
			        contextMenuItem_Enabled();
             }
		}


		//Add custom file   
		private void AddCustomFile()
		{
			OpenFileDialog openFileDialog1 = new OpenFileDialog();

			openFileDialog1.InitialDirectory = "c:\\" ;
			openFileDialog1.Filter = "All files (*.*)|*.*";
			openFileDialog1.FilterIndex = 2 ;
			openFileDialog1.Multiselect=true;
			openFileDialog1.RestoreDirectory = true ;

            //MessageBox.Show(opSystem, "Info", MessageBoxButtons.OK, MessageBoxIcon.Information);

            //For Window7
            //if (opSystem == "Windows 7 / Windows Server 2008")
            //        openFileDialog1.AutoUpgradeEnabled = false;

			if(openFileDialog1.ShowDialog() == DialogResult.OK)
			{
				string [] addFileName=openFileDialog1.FileNames;
				bool inPath_HOME_DIR;
				bool inPath_TOOL_DIR;
                bool inPathProgramDIR;

				foreach(string filename in addFileName)
				{
					inPath_HOME_DIR=filename.ToString().StartsWith(InstallPath[1].ToString());	//服务器

					inPath_TOOL_DIR=filename.StartsWith(InstallPath[1]);                         //客户端

                    inPathProgramDIR = filename.StartsWith(InstallPath[3]);                     //Windows程序

				
				//test  跟踪参数::::::::::::::::::::
				//DialogResult retTest=MessageBox.Show(MessageResourceManager.GetString("RemoveCustomFile")+filename+"<>"+InstallPath[1]+"<>"+inPath_HOME_DIR+" ”?","Info",MessageBoxButtons.OKCancel,MessageBoxIcon.Question);
				//end::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::


				DialogResult ret=new DialogResult();

                if (inPath_HOME_DIR || inPath_TOOL_DIR || inPathProgramDIR)
				{
					if(ret!=DialogResult.OK)
					{
						dp.AddCustomeFile(listView1,statusBarPanel1,filename);
					}
				}
				else
				{
					ret=MessageBox.Show(MessageResourceManager.GetString("AddCustomFile"),"Info",MessageBoxButtons.OK,MessageBoxIcon.Warning);
				}
				}
			}
			//释放对话框   
			openFileDialog1.Dispose();
		}
		//Remove custom file
		private void RemoveCustomFile()
		{
			if (lvIndex!=-1 && listView1.Items[lvIndex].Selected)
			{
				string _name=this.listView1.Items[lvIndex].Text;
				DialogResult ret=MessageBox.Show(MessageResourceManager.GetString("RemoveCustomFile")+_name+" ”?","Info",MessageBoxButtons.OKCancel,MessageBoxIcon.Question);
				if(ret==DialogResult.OK)
				{
					dp.RemoveCustomeFile(listView1,statusBarPanel1,lvIndex);
				}
			}
		}
		//View code
		private void ViewCustomFile()
		{
			ViewCode vc=new ViewCode();
			
			if (lvIndex!=-1 && tag)//Look packfile
			{
				vc.FileName=this.listView1.Items[lvIndex].SubItems[3].Text;
				vc.file_load();
			}
			else if (!tag)//Look unpackfile
			{
				//从备份文件里读指定的文件未解决，有一种办法将文件写到硬盘，再读文件；读完后，再将文件删除。
			}
			vc.statusBar1.Text=this.listView1.Items[lvIndex].Text;
			vc.ShowDialog(this);
			vc.Dispose();
		}
		

       //双击文件打开应用程序
		private void listView1_DoubleClick(object sender, System.EventArgs e)
		{
			lvIndex=this.listView1.FocusedItem.Index;
			if (lvIndex!=-1)
			{
				string fullname=this.listView1.Items[lvIndex].SubItems[3].Text;
				System.Diagnostics.Process.Start(fullname);
			}
		}

		private void About_MI_Click(object sender, System.EventArgs e)
		{
		    AboutCCI ac=new AboutCCI();
			ac.X=this.Location.X+this.Width/2;
			ac.Y=this.Location.Y+this.Height/2;
			ac.Show();
			ac.Location_point();
		}
		private void showhelp()
		{
			string helpFile = AppDomain.CurrentDomain.BaseDirectory + "CCI UsersGuide.chm";

			Help.ShowHelp(this, helpFile);
		}

		private void Using_MI_Click(object sender, System.EventArgs e)
		{
		   showhelp();
		}

		private void ctxViewMenuItem_Click(object sender, System.EventArgs e)
		{
			if (lvIndex!=-1){ViewCustomFile();}
		}

		private void ctxAddMenuItem_Click(object sender, System.EventArgs e)
		{
			AddCustomFile();
		}

		private void ctxRemoveMenuItem_Click(object sender, System.EventArgs e)
		{
			if (lvIndex!=-1){RemoveCustomFile();}
		}
		//contextMenuItem enabled
		private void contextMenuItem_Enabled()
		{
			if (lvIndex!=-1){eventFalg=listView1.Items[lvIndex].Selected;}
			if  (this.listView1.Items.Count==0 )//No file
			{
				this.ctxViewMenuItem.Enabled=false;
				this.ctxAddMenuItem.Enabled=false;
				this.ctxRemoveMenuItem.Enabled=false;
			}
			//Search file is not selected.
			//tag用于判断是Search=true 还是Open=false.
			//eventFalg用于判断鼠标左键选中条目后又移出，再在空白处点击，条目有虚线框，此时用户会误认为条目被选中。
			//eventFalg 为true 选中 ,eventFalg 为fasle 未选中  
			else if (this.listView1.Items.Count!=0 && tag && !eventFalg)//Search file is not selected
			{
				this.ctxViewMenuItem.Enabled=false;
				this.ctxAddMenuItem.Enabled=true;
				this.ctxRemoveMenuItem.Enabled=false;
			}
			
			else if (this.listView1.Items.Count!=0 && tag && eventFalg)//Search file is selected 
			{
				this.ctxViewMenuItem.Enabled=true;
				this.ctxAddMenuItem.Enabled=true;
				this.ctxRemoveMenuItem.Enabled=true;
			}

			else if (this.listView1.Items.Count!=0 && !tag && !eventFalg)//Open file is not selected
			{
				this.ctxViewMenuItem.Enabled=false;
				this.ctxAddMenuItem.Enabled=false;
				this.ctxRemoveMenuItem.Enabled=false;
			}
			else if (this.listView1.Items.Count!=0 && !tag && eventFalg)//Open file is selected
			{
				this.ctxViewMenuItem.Enabled=false;
				this.ctxAddMenuItem.Enabled=false;
				this.ctxRemoveMenuItem.Enabled=true;
			}
		}

		private void Add_MI_Click(object sender, System.EventArgs e)
		{
			AddCustomFile();		
		}

		private void Remove_MI_Click(object sender, System.EventArgs e)
		{
			RemoveCustomFile();		
		}
	}
}
