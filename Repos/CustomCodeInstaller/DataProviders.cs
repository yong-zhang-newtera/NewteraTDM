using System;
using System.IO;
using System.Data;
using System.Xml;
using System.Text;
using System.Collections;
using Microsoft.Win32;
using System.Windows.Forms;

using ICSharpCode.SharpZipLib.Checksums;
using ICSharpCode.SharpZipLib.Zip;
using ICSharpCode.SharpZipLib.GZip;

using FileInfomation;



namespace RestoreTool
{
    /// <summary>
    /// DataProviders 的摘要说明。
    /// </summary>
    /// 
    public class DataProviders
    {
        #region 私有域
        private ArrayList listAL = new ArrayList();//原始文件列表、New Web.config file　更新用、新文件列表
        public ArrayList tempAL = new ArrayList();
        public ArrayList tempBL = new ArrayList();
        public ArrayList tempCL = new ArrayList();
        public ArrayList tempDL = new ArrayList();
        private ArrayList fileinfolist = new ArrayList();
        private string[,] OrdinalList;//筛选原始文件
        private int num;
        private string packFileName;
        private string _installer_directoryname;
        private string _installer_directorypath;
        private string _installerDirectory;
        private string packFilePath;
        private string newFilePath;
        private string[] InstallPath;
        private string[] myPath;
        //private System.Windows.Forms.StatusBarPanel myStatusBarPanel;
        //private System.Windows.Forms.ListView mylistView1;
        //注册表节点名
        private const string HKEY_LOCALNODE__MACHINE_NODE = "Newtera";
        //从注册表中获取newtera TDM 的安装路径 
        private string[] HKEY_KeyValue;
        private string[] HKEY_KeyName;
        //CCI恢复时不合并完全覆盖处理的文档和文件夹里所有文件(用用户原文覆盖安装文件处理)
        private static string copyFileFolder = "Config";
        private static string[] filterFile ={ "web.config","Studio.exe.config","WorkflowStudio.exe.config","SmartWord.dll.config", "SmartWord.doc", "web.sitemap","web_TDP.sitemap"};

        //Newtera TDM的配置文件,这些文档都需要备份,Cpnfig文件夹里所有文件都需要做备份，放在安装TDM生成FileList不生成Config文件夹里的文件。
        private static string[] configFile = new string[] {"web.config", "Studio.exe.config","WorkflowStudio.exe.config","SmartWord.dll.config","SmartWord.doc", 
              "web_TDP.sitemap","web.sitemap","logo.png","logo.jpg"};

        //含有sitemap扩展名的文件全部备份
        private static string[] customFilesInExt = new string[] { ".sitemap" };

        //CCI搜索时需要排除的文件和文件夹
        //排除下面几个文件不做备份
        private static string[] excludeFiles = { "catalog_sqls_oracle.xml", "catalog_sqls_sqlserver.xml", "license.rtf", "FileList.xml", "Thumbs.db" };
        //排除attachments ,temp文件夹里的文件对图片不进行备
        private static string[] excludeFileFolders = { "attachments", "temp" };

        //CCI恢复时需要排除的文件,因为前版本打包进去文件
        private static string[] excludePackFiles = { "RestoreTool.dll", "RestoreTool.resources.dll", "RestoreTool.exe", "ICSharpCode.SharpZipLib.dll" };

        #endregion

        public DataProviders()
        {
            //
            // TODO: 在此处添加构造函数逻辑
            //
        }

        public string[] HKEYPath()
        {
            RegistryKey RK = Registry.LocalMachine;
            RegistryKey subRK;

            MyToDate();

            if (Is64BitMode())
            {
                //subRK = RK.OpenSubKey("SOFTWARE\\Wow6432Node");
                subRK = RK.OpenSubKey("SOFTWARE");
            }
            else
            {
                subRK = RK.OpenSubKey("SOFTWARE");
            }
            //打开"myName"子键 
            RegistryKey node = subRK.OpenSubKey(HKEY_LOCALNODE__MACHINE_NODE);
            if (node != null)
            {
                HKEY_KeyName = node.GetValueNames();
                HKEY_KeyValue = new string[HKEY_KeyName.Length * 2];
                int i = 0;
                foreach (string kn in HKEY_KeyName)
                {
                    HKEY_KeyValue.SetValue(kn, i);
                    i++;
                    string myPath = node.GetValue(kn).ToString();
                    myPath = convertChart(myPath);
                    //HKEY_KeyValue.SetValue(node.GetValue(kn),i);
                    HKEY_KeyValue.SetValue(myPath, i);
                    i++;
                }
            }
            else
            {
                throw new Exception(MessageResourceManager.GetString("NeweraTDM_Application"));
            }

            return HKEY_KeyValue;
        }

        /// <summary>
        /// inetpub小写转换大写Inetpub
        /// </summary>
        private string convertChart(string strpath)
        {
            string path = null;

            //if (strpath=="C:\\Inetpub\\wwwroot\\Newtera\\"){strpath="C:\\inetpub\\wwwroot\\Newtera\\";}

            int n = strpath.IndexOf("inetpub");
            if (n > 0)
            {
                path = strpath.Substring(0, n) + "Inetpub" + strpath.Substring(n + 7);
            }
            else
            {
                path = strpath;
            }
            return path;
        }

      
        private void SearchTheDirectorInfiles()
        {
            string path1 = null, path2 = null;
            try
            {
                //从注册表里获取newtera TDM 的安装路径
                InstallPath = HKEYPath();

                if (InstallPath[0] == "HOME_DIR")
                {

                    //设置搜索文件目录
                    path1 = InstallPath[1];
                    path2 = InstallPath[3];
                    myPath = new string[] { path1, path2 };
                }
                else if (InstallPath[0] == "TOOL_DIR")
                {
                    //n_TOOL_DIR = InstallPath[1].Length;
                    //设置搜索文件目录
                    path1 = InstallPath[1];
                    myPath = new string[] { path1 };
                }

                string installer_directoryname = null;
                string installer_directorpath = null;

                //读取原文档FileList.xml目录文件
                string FileListPath = InstallPath[1] + "FileList.xml";
                string NodeName = "list";

                if (File.Exists(FileListPath))
                {
                    ReadXMLFile(FileListPath, NodeName);
                    //清空ListView
                    //mylistView1.Items.Clear();
                    //清空用户文件存放临时数组tempAL
                    tempAL.Clear();
                    //tempBL.Clear();
                    //tempCL.Clear();
                    //tempDL.Clear();

                    //对指定文件夹遍历搜索比较，找出用户添加的文件并在ListView中显示
                    if (InstallPath[0] == "HOME_DIR")//服务器版
                    {
                        //设置路径一维数组
                        //对指定文件夹遍历搜索
                        foreach (string path in myPath)
                        {
                            if (path == InstallPath[3])
                            {
                                installer_directoryname = InstallPath[0];
                                installer_directorpath = InstallPath[3];
                                GetDirectoryFile(installer_directoryname, installer_directorpath, path);
                            }
                        }
                    }
                    else if (InstallPath[0] == "TOOL_DIR")//客户版
                    {
                        myPath = new string[] { path1, path2 };
                        //对指定文件夹遍历搜索
                        foreach (string path in myPath)
                        {
                            if (path == InstallPath[3])
                            {
                                installer_directoryname = InstallPath[0];
                                installer_directorpath = InstallPath[1];
                                GetDirectoryFile(installer_directoryname, installer_directorpath, path);
                            }
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
        }

        /// <summary>
        /// 递归实现搜索目录下的所有子目录和文件
        /// 参数为指定的目录 
        ///  /// <jparment=System.Windows.Forms.ListView>
        /// </summary>
        private void SearchDirectory(System.Windows.Forms.ListView listView1, string installer_directoryname, string installer_directorpath, string path)
        {
            int k = 0;
            bool flag = true;
            int _n = 0;

            //在指定目录及子目录下查找文件   
            DirectoryInfo Dir = new DirectoryInfo(path);
            try
            {
                //查找子目录   
                foreach (DirectoryInfo d in Dir.GetDirectories())
                {
                    //排除attachments ,temp文件夹里的文件对图片不进行备份。
                    if (d.ToString().ToLower() != excludeFileFolders[0].ToLower() && d.ToString().ToLower() != excludeFileFolders[1].ToLower())  
                           SearchDirectory(listView1, installer_directoryname, installer_directorpath, path + d.ToString() + "\\");
                }

                //查找用户制定文件 
                FindCustomFiles(Dir,listView1,path,installer_directorpath,flag,_n,k);
            }
            catch (Exception e)
            {
                MessageBox.Show(e.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }

        }

        /// <summary>
        /// 递归实现搜索目录下的所有子目录和文件
        /// 参数为指定的目录 
        /// <jparment=installer_directoryname>
        /// <jparment=installer_directorpath>
        /// <jparment=path>
        /// </summary>
        private void SearchDirectory(string installer_directoryname, string installer_directorpath, string path)
        {
            int k = 0;
            bool flag = true;
            int _n = 0;

            //在指定目录及子目录下查找文件   
            DirectoryInfo Dir = new DirectoryInfo(path);
            try
            {
                //查找子目录   
                foreach (DirectoryInfo d in Dir.GetDirectories())
                {
                    //排除attachments ,temp文件夹里的文件对图片不进行备份。
                    if (d.ToString().ToLower() != excludeFileFolders[0].ToLower() && d.ToString().ToLower() != excludeFileFolders[1].ToLower())
                        SearchDirectory(installer_directoryname, installer_directorpath, path + d.ToString() + "\\");
                }

                //查找用户制定文件 
                FindCustomFiles(Dir,path, installer_directorpath, flag, _n, k);
            }
            catch (Exception e)
            {
                MessageBox.Show(e.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }

        }

        /// <summary>
        /// 搜索指定目录下的所有子文件
        /// 参数为指定的目录 
        /// </summary>
        private void FindCustomFiles(DirectoryInfo Dir, string path, string installer_directorpath, bool flag, int _n, int k)
        {
            foreach (FileInfo file in Dir.GetFiles("*.*"))
            {
                if (flag)
                {

                    //设置一维数组
                    tempAL.Add(file.Name);
                    tempAL.Add(file.FullName);
                }
                k++;
            }
        }


        /// <summary>
        /// 查找用户制定文件 
        /// <partment=ListView>
        /// </summary>
        private void FindCustomFiles(DirectoryInfo Dir, ListView listView1, string path, string installer_directorpath, bool flag, int _n, int k)
        {
            foreach (FileInfo file in Dir.GetFiles("*.*"))
            {
                //排除attachments ,temp文件夹里的文件对图片不进行备份。
                if (Dir.Name.ToLower() != excludeFileFolders[0].ToLower() && Dir.Name.ToLower() != excludeFileFolders[1].ToLower())  
                {
                    ListViewItem lvi = new ListViewItem();
                    //以每个文件夹为区域搜索用户文件
                    string tempPath = null;
                    for (int i = 0; i < num; i++)
                    {
                        tempPath = OrdinalList[i, 1];
                        //按指定的文件夹搜索
                        if (tempPath.IndexOf(path) == 0)
                        {
                            //筛选出Newtera TDM原始文件和不需要备份的文件
                            bool isExcFile = false;
                            foreach (string orgFile in excludeFiles)
                            {
                                if (file.Name == orgFile)
                                {
                                    isExcFile = true;
                                    break;
                                }      
                            }
                            //筛选出Newtera TDM原始文件和不需要备份的文件
                            if (String.Equals(file.Name, OrdinalList[i, 0]) || file.Extension == "db" || isExcFile)
                            {
                                //flag = false;
                                //break;
                                //指定要备份的文件，该文件在Filelist存在。
                                bool isExtensionSameFile = false;

                                foreach (string extName in customFilesInExt)
                                {
                                    if (file.Extension == extName)
                                    {
                                        isExtensionSameFile = true;
                                        break;
                                    }
                                }

                                if (isExtensionSameFile)//指定要备份的文件，该文件在Filelist存在。
                                {
                                    flag = true;
                                    break;
                                }

                                flag = false;
                                break;
                            }
                            else
                            {
                                flag = true;
                            }
                        }
                    }
                    if (flag)
                    {
                        //设置文件名称
                        int loca = file.Name.LastIndexOf(".");
                        if (loca == -1)
                        {
                            lvi.Text = file.Name;
                        }
                        else
                        {
                            lvi.Text = file.Name.Substring(0, loca);
                        }

                        //用Tag属性保存文件的完全路径
                        //lvi.Tag=file.FullName;
                        //设置文件大小
                        lvi.SubItems.Add(file.Length.ToString());
                        //设置文件类型
                        if (loca == -1)
                        {
                            lvi.SubItems.Add("");
                        }
                        else
                        {
                            lvi.SubItems.Add(file.Extension.Substring(1));
                        }

                        //设置文件路径
                        lvi.SubItems.Add(file.FullName);
                        //设置文件最后更改问时间
                        lvi.SubItems.Add(file.LastWriteTime.ToString());
                        listView1.Items.Add(lvi);
                        //设置一维数组
                        tempAL.Add(file.Name);
                        tempAL.Add(file.FullName);
                        //获取文件与相对路径名名用于打包备份
                        //打包时服务器和客户机标示说明：
                        //2035，1，1   35表示安装目录字符总数，1也表示服务器版，1表示web,2表示app。2表示客户版
                        _n = installer_directorpath.Length;
                        _installer_directoryname = file.FullName.Substring(0, _n);

                        // MessageBox.Show(_installer_directoryname, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);

                        if (installerDirectory.ToLower() == "HOME_DIR".ToLower())//服务器版
                        {
                            if (_installer_directoryname.ToLower() == HKEY_KeyValue[1].ToLower())//web view
                            {
                                //替换路径
                                tempBL.Add("20" + HKEY_KeyValue[1].Length + ",1,1");
                            }
                            else if (_installer_directoryname.ToLower() == HKEY_KeyValue[3].ToLower())//应用程序
                            {
                                //替换路径
                                tempBL.Add("20" + HKEY_KeyValue[3].Length + ",1,2");
                            }
                        }
                        else if (installerDirectory.ToLower() == "TOOL_DIR".ToLower())//客户版
                        {
                            if (_installer_directoryname.ToLower() == HKEY_KeyValue[1].ToLower())
                            {
                                //替换路径 
                                tempBL.Add("20" + HKEY_KeyValue[1].Length + ",2,1");
                            }
                        }
                    }
                    k++;
                }
            }
        }

        /// <summary>
        /// 找出用户文件
        /// <Parment=System.Windows.Forms.ListView>
        /// <Parment= System.Windows.Forms.StatusBarPanel>
        /// </summary>
        public void GetDirectoryFile(System.Windows.Forms.ListView listView1,
            System.Windows.Forms.StatusBarPanel statusBarPanel1,
            string installer_directoryname, string installer_directorpath, string path)
        {
            try
            {
                SearchDirectory(listView1, installer_directoryname, installer_directorpath, path);

                //设置文件统计信息
                int totalCount = System.Convert.ToInt16(tempAL.Count.ToString()) / 2;
                string msg = MessageResourceManager.GetString("TotalFiles");
                msg = String.Format(msg, totalCount);
                //MessageBox.Show(msg,"info",MessageBoxButtons.OK,MessageBoxIcon.Information);
                statusBarPanel1.Text = msg;
            }
            catch (Exception e)
            {
                //设置出错信息
                string FileMsg = e.Message.ToString();
                //statusBarPanel1.Text=FileMsg;
            }
        }

        /// <summary>
        /// 找出用户文件  
        /// <parment=installer_directoryname>
        /// <parment=installer_directorpath>
        /// <parment=path>
        /// </summary>
        public void GetDirectoryFile(string installer_directoryname, string installer_directorpath, string path)
        {
            try
            {
                SearchDirectory(installer_directoryname, installer_directorpath, path);
            }
            catch (Exception e)
            {
                //设置出错信息
                string FileMsg = e.Message.ToString();
                //statusBarPanel1.Text=FileMsg;
            }
        }

        /// <summary>
        /// Add a customer file。
        /// </summary>
        public void AddCustomeFile(System.Windows.Forms.ListView listView1, System.Windows.Forms.StatusBarPanel statusBarPanel1, string addFileName)
        {
            int li = addFileName.LastIndexOf("\\");
            int _n = 0;
            string path = addFileName.Substring(0, li);
            string filename = addFileName.Substring(li + 1);
            //string _installer_directoryname=null;
            string version = null;
            bool flag = true;
            try
            {
                //得到需要显示内容列表的目录信息
                DirectoryInfo myDir = new DirectoryInfo(path);
                //得到指定目录中的指定文件
                FileInfo[] files = myDir.GetFiles(filename);
                //判断列表里是否有相同的文件如有则退出。
                foreach (string fn in tempAL)
                {
                    if (fn.ToLower() == addFileName.ToLower())
                    {
                        flag = false;
                        break;
                    }
                    else
                    {
                        flag = true;
                    }
                }
                if (flag)
                {
                    ListViewItem lvi = new ListViewItem();
                    //设置文件名称
                    int loca = files[0].Name.IndexOf(".");

                    if (loca == -1)
                    {
                        lvi.Text = files[0].Name;
                    }
                    else
                    {
                        lvi.Text = files[0].Name.Substring(0, loca);
                    }


                    //用Tag属性保存文件的完全路径
                    //lvi.Tag=file.FullName;
                    //设置文件大小
                    lvi.SubItems.Add(files[0].Length.ToString());
                    //设置文件类型
                    // lvi.SubItems.Add(files[0].Extension.Substring(1));
                    if (loca == -1)
                    {
                        lvi.SubItems.Add("");
                    }
                    else
                    {
                        lvi.SubItems.Add(files[0].Extension.Substring(1));
                    }
                    //设置文件路径
                    lvi.SubItems.Add(files[0].FullName);
                    //设置文件最后更改问时间
                    lvi.SubItems.Add(files[0].LastWriteTime.ToString());
                    listView1.Items.Add(lvi);
                    //设置一维数组
                    tempAL.Add(files[0].Name);
                    tempAL.Add(files[0].FullName);

                    if (installerDirectory.ToLower() == "HOME_DIR".ToLower())//服务器版
                    {
                        for (int i = 0; i < 4; i++)
                        {
                            _n = files[0].FullName.IndexOf(HKEY_KeyValue[i]);
                            if (_n == 0 && i == 1)//web ..\Inetpub\wwwroot\
                            {
                                version = "web";
                                break;
                            }
                            else if (_n == 0 && i == 3)//app ..\Program Files\Newtera
                            {
                                version = "app";
                                break;
                            }
                        }
                        if (version.ToLower() == "web".ToLower())//web view
                        {
                            tempBL.Add("20" + HKEY_KeyValue[1].Length + ",1,1");
                        }
                        else if (version.ToLower() == "app".ToLower())//应用程序
                        {
                            tempBL.Add("20" + HKEY_KeyValue[3].Length + ",1,2");
                        }
                    }
                    else if (installerDirectory.ToLower() == "TOOL_DIR".ToLower())//客户版
                    {
                        _n = files[0].FullName.IndexOf(HKEY_KeyValue[1]);
                        if (_n == 0)
                        {
                            tempBL.Add("20" + HKEY_KeyValue[1].Length + ",2,1");
                        }
                    }
                }
                //设置文件统计信息
                int totalCount = System.Convert.ToInt16(tempAL.Count.ToString()) / 2;
                string msg = MessageResourceManager.GetString("TotalFiles");
                msg = String.Format(msg, totalCount);
                //MessageBox.Show(msg,"info",MessageBoxButtons.OK,MessageBoxIcon.Information);
                statusBarPanel1.Text = msg;
            }
            catch (Exception e)
            {
                //设置出错信息
                string FileMsg = e.Message.ToString();
                //statusBarPanel1.Text=FileMsg;
                MessageBox.Show(e.Message);
            }
        }
        /// <summary>
        /// Remove a customer file。
        /// </summary>
        private int totalCount;
        public void RemoveCustomeFile(System.Windows.Forms.ListView listView1, System.Windows.Forms.StatusBarPanel statusBarPanel1, int indexfile)
        {
            try
            {
                if (listView1.Items[indexfile].Selected)
                {
                    string filename = listView1.Items[indexfile].Text + "." + listView1.Items[indexfile].SubItems[2].Text;
                    string filepath = listView1.Items[indexfile].SubItems[3].Text;
                    //Remove file forom TempAL and TempBL
                    tempBL.RemoveAt(indexfile);
                    tempAL.Remove(filename);
                    tempAL.Remove(filepath);
                    tempCL.Add(filename);
                    tempDL.Remove(filename);//存放ListView files, 显示搜索和恢复的文件。

                    listView1.Items[indexfile].Remove();
                }
                //设置文件统计信息

                if (tempAL.Count != 0 && tempDL.Count == 0)
                {
                    totalCount = System.Convert.ToInt16(tempAL.Count.ToString()) / 2;
                }
                else if (tempDL.Count != 0 && tempAL.Count == 0)
                {
                    totalCount = System.Convert.ToInt16(tempDL.Count.ToString());
                }
                string msg = MessageResourceManager.GetString("TotalFiles");
                msg = String.Format(msg, totalCount);
                //MessageBox.Show(msg,"info",MessageBoxButtons.OK,MessageBoxIcon.Information);
                statusBarPanel1.Text = msg;
            }
            catch (Exception e)
            {
                //设置出错信息
                string FileMsg = e.Message.ToString();
                //statusBarPanel1.Text=FileMsg;
                MessageBox.Show(e.Message);
            }
        }

        /// <summary>
        /// Insert,Update ,Add  XML 子节点,RemoveChild XML child node
        /// </summary>
        public void WriteFileXML(string pathfile, string _nodename, ArrayList myList, string flag)
        {
            XmlNode currNode;
            XmlDocument Xmldoc = new XmlDocument();

            //从安装目录里读取新的XML文件
            Xmldoc.Load(pathfile);
            //::: Get menu node num:::::::::::::::::::::::::::::::::::::::::::::::::::::::::::
            int nodeCount = Xmldoc.ChildNodes.Item(1).ChildNodes.Count;

            string nodeName;
            int nodeNum = 0;

            for (int i = 0; i < nodeCount; i++)
            {
                nodeName = Xmldoc.ChildNodes.Item(1).ChildNodes.Item(i).Name;
                if (nodeName == _nodename)
                {
                    nodeNum = i;
                    break;
                }
            }

            //::::::::::::::Write contents::::::::::::::::::::::::::::::::::: 
            currNode = Xmldoc.ChildNodes.Item(1).ChildNodes.Item(nodeNum);

            string Insstr = null;
            XmlDocumentFragment DocFrag = Xmldoc.CreateDocumentFragment();
            XmlDocumentFragment newFrag = Xmldoc.CreateDocumentFragment();

            switch (flag)
            {
                case "insert":
                    //DocFrag.InnerXml ="<menuitem label=\""+ Insstr+"\" ></menuitem>";
                    //currNode.AppendChild(DocFrag);
                    break;
                case ("delete"):
                    //currNode.RemoveChild(currNode.ChildNodes.Item(newsNum));
                    break;
                case ("update"):
                    //XmlDocumentFragment newFrag=Xmldoc.CreateDocumentFragment();
                    //newFrag.InnerXml ="<menuitem label=\""+ Insstr+"\" ></menuitem>";
                    //Replace the title element.
                    //currNode.ReplaceChild(newFrag, currNode.ChildNodes.Item(nodeNum));
                    break;
                default:
                    //Remove all node
                    currNode.RemoveAll();
                    //insert node
                    foreach (MyFileInfo m in myList)
                    {
                        if (m.Comment == null)
                        {
                            Insstr = "<add key=\"" + m.Key + "\" value=\"" + m.Values + "\" />";
                        }
                        else
                        {
                            Insstr = "<!--" + m.Comment + " -->" + "<add key=\"" + m.Key + "\" value=\"" + m.Values + "\" />";
                        }
                        DocFrag.InnerXml = Insstr;
                        currNode.AppendChild(DocFrag);
                    }
                    break;
            }

            Xmldoc.Save(pathfile);
        }

        /// <summary>
        /// Write xml file
        /// 得到用户安装目录名
        /// </summary>
        public void WriteFileXML(string filename)
        {
            string directory_name = "HOM_DIR";
            //string filename =HKEY_KeyValue[1]+"Web.config";
            XmlTextWriter writer = null;

            writer = new XmlTextWriter(filename, null);
            //Use indenting for readability.
            writer.Formatting = Formatting.Indented;
            //Write the XML delcaration. 
            writer.WriteStartDocument();
            //Write a root element.
            writer.WriteStartElement("list");
            //Write the genre attribute.
            writer.WriteAttributeString("genre", "Custom code file");
            writer.WriteAttributeString("directory", directory_name);

            //将数组写入xml 节点(file→name,path)
            int m = tempAL.Count / 2;
            int n = 2;
            int k = 0;
            for (int i = 0; i < m; i++)
            {
                //Write a root element.
                writer.WriteStartElement("file");
                for (int j = 0; j < n; j++)
                {
                    //Write the name.
                    if (j == 0) { writer.WriteElementString("name", tempAL[k].ToString()); }
                    //Write the path.
                    if (j == 1) { writer.WriteElementString("path", tempAL[k].ToString()); }
                    k++;
                }
                //end node
                writer.WriteEndElement();
            }
            writer.WriteEndDocument();
            //Write the XML to file and close the writer.
            writer.Flush();
            writer.Close();
        }

        /// <summary>
        /// 读 XML文件 
        /// </summary>
        public void ReadXMLFile(System.Windows.Forms.StatusBarPanel statusBarPanel1, string filiPath, string nodeName)
        {
            try
            {
                statusBarPanel1.Text = "Load file " + filiPath;
                // 从文件加载 XML
                XmlDocument myXmlDocument = new XmlDocument();
                myXmlDocument.Load(filiPath);
                //创建 XmlNodeReader 以显示全部纪录
                XmlNodeReader myXmlNodeReader = new XmlNodeReader(myXmlDocument.SelectSingleNode(nodeName));
                listAL = FormatXml(myXmlNodeReader);

                //将原目录文件(TDM的安装文件，需要做比较的文件)由一维转换为二维
                OrdinalList = convertToArray(listAL);
                num = OrdinalList.Length / 2;
            }
            catch (Exception e)
            {
                //设置出错信息
                //string FileMsg = e.Message.ToString();
                MessageBox.Show(e.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }
        /// <summary>
        /// 读 XML文件 
        /// </summary>
        public void ReadXMLFile(string filiPath, string nodeName)
        {
            try
            {
                // 从文件加载 XML
                XmlDocument myXmlDocument = new XmlDocument();
                myXmlDocument.Load(filiPath);
                //创建 XmlNodeReader 以显示全部纪录
                XmlNodeReader myXmlNodeReader = new XmlNodeReader(myXmlDocument.SelectSingleNode(nodeName));
                listAL = FormatXml(myXmlNodeReader);

                //将原目录文件(TDM的安装文件，需要做比较的文件)由一维转换为二维
                OrdinalList = convertToArray(listAL);
                num = OrdinalList.Length / 2;
            }
            catch (Exception e)
            {
                //设置出错信息
                //string FileMsg = e.Message.ToString();
                MessageBox.Show(e.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        public string installerDirectory
        {
            get
            {
                return _installerDirectory;
            }
            set
            {
                _installerDirectory = value;
            }
        }


        /// <summary>
        /// 替换路径
        /// </summary>
        private ArrayList FormatXml(XmlNodeReader reader)
        {
            ArrayList xmlAl = new ArrayList();
            int num = 0;
            while (reader.Read())
            {
                switch (reader.NodeType)
                {
                    case XmlNodeType.Element:
                        while (reader.MoveToNextAttribute())
                        {
                            if (reader.Name == "type") { installerDirectory = reader.Value; }
                        }
                        break;
                    case XmlNodeType.Text:
                        num = reader.Value.IndexOf("\\");
                        if (num == -1)
                        {
                            xmlAl.Add(reader.Value);//获取文件名用于比较
                        }
                        else//替换路径
                        {
                            _installer_directoryname = reader.Value.Substring(0, num);
                            if (installerDirectory.ToLower() == "HOME_DIR".ToLower())//服务器版
                            {
                                if (_installer_directoryname.ToLower() == HKEY_KeyValue[0].ToLower())
                                {
                                    _installer_directorypath = HKEY_KeyValue[1] + reader.Value.Substring(num + 1);//替换路径
                                    xmlAl.Add(_installer_directorypath);
                                }
                                else if (_installer_directoryname.ToLower() == HKEY_KeyValue[2].ToLower())
                                {
                                    _installer_directorypath = HKEY_KeyValue[3] + reader.Value.Substring(num + 1);//替换路径
                                    xmlAl.Add(_installer_directorypath);
                                }
                            }
                            else if (installerDirectory.ToLower() == "TOOL_DIR".ToLower())//客户版
                            {
                                if (_installer_directoryname.ToLower() == HKEY_KeyValue[0].ToLower())
                                {
                                    _installer_directorypath = HKEY_KeyValue[1] + reader.Value.Substring(num + 1);//替换路径
                                    xmlAl.Add(_installer_directorypath);
                                }
                            }
                        }
                        break;
                    case XmlNodeType.Whitespace:
                        //Format (reader, "Whitespace");
                        break;
                }
            }
            return xmlAl;
        }

        /// <summary>
        /// 将 ArrayList 一维数组转换为二维数组 string [,]
        /// </summary>
        private string[,] convertToArray(ArrayList myAL)
        { 
            bool Flag = false;
            int m = myAL.Count / 2;
            int n = 2;
            string[,] myArray = new string[m, n];
            int k = 0;
            for (int i = 0; i < m; i++)
            {
                for (int j = 0; j < n; j++)
                {
                    //过滤Newtera TDM的配置文件
                    foreach (string FileName in configFile)
                    {
                        if (myAL[k].ToString().ToLower() == FileName.ToLower() )
                        {
                            Flag = false;
                            break;
                        }
                        else
                        {
                            Flag = true;
                        }
                    }
                    if (Flag)
                    {
                        myArray.SetValue(myAL[k], i, j);
                    }
                    else
                    {
                        //过滤掉的文件设置为空的文件名
                        myArray.SetValue("", i, j);
                    }
                    k++;
                }
            }
            return myArray;
        }

        /// <summary>
        /// 得到用户文件路径一维列表
        /// </summary>
        private string[] GetCustomFile()
        {
            int m = tempAL.Count;
            string[] myArray = new string[m / 2];
            int k = 0;
            for (int i = 0; i < m; i++)
            {
                if ((i % 2) == 1) { myArray.SetValue(tempAL[i], k); k++; }
            }
            return myArray;
        }
        /// <summary>
        /// Pack the files into a zipped file
        /// </summary>
        public void PackFiles(string _packFileName)
        {
            ZipEntry entry;
            FileStream fs = null;
            ZipOutputStream s = null;

            try
            {
                s = new ZipOutputStream(File.Create(_packFileName));
                s.SetLevel(6); // 0 - store only to 9 - means best compression
                string[] filenames = GetCustomFile();
                int index = 0;
                foreach (string file in filenames)
                {


                    fs = File.OpenRead(file);

                    byte[] buffer = new byte[fs.Length];
                    fs.Read(buffer, 0, buffer.Length);
                    entry = new ZipEntry(file);

                    //entry.DateTime = DateTime.Now;
                    //FileInfo fileInfo = new FileInfo(file);
                    //entry.DateTime =new DateTime(2006,1,1);
                    entry.DateTime = System.Convert.ToDateTime(tempBL[index]);

                    //调试用
                    //MessageBox.Show(Convert.ToString(index), "Info", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    //MessageBox.Show(tempBL.Count.ToString(), "Info", MessageBoxButtons.OK, MessageBoxIcon.Information);

                    // set Size because the information
                    // about the size should be stored in the header
                    // if it is not set it is automatically written in the footer.
                    entry.Size = fs.Length;
                    fs.Close();
                    fs = null;

                    s.PutNextEntry(entry);

                    s.Write(buffer, 0, buffer.Length);


                    index++;
                }
            }
            finally
            {
                if (fs != null)
                {
                    fs.Close();
                }

                if (s != null)
                {
                    s.Finish();
                    s.Close();
                }
            }
        }

        /// <summary>
        /// Unpack the files into a directory
        /// </summary>
        /// 
        private FileStream streamWriter = null;
        private ZipInputStream s = null;
        private ZipEntry theEntry;
        private string[] REG_Values;

        public void OpenFiles(System.Windows.Forms.ListView listView1,
            System.Windows.Forms.StatusBarPanel statusBarPanel1, string _packFileName)
        {
            int _index = 0;
            try
            {                
                //从注册表里获取现安装路径
                REG_Values = HKEYPath();
				
				//搜索指定文件文件，移除打包文件中不需要恢复的文件。
                SearchTheDirectorInfiles();

                packFileName = _packFileName;
                s = new ZipInputStream(File.OpenRead(_packFileName));
                //来自备份文件、Zip软件
                while ((theEntry = s.GetNextEntry()) != null)
                {
                    string directoryName = Path.GetDirectoryName(theEntry.Name);
                    string fileName = Path.GetFileName(theEntry.Name);
                    string fileExtension = Path.GetExtension(theEntry.Name);
                    string fileFullPath = Path.GetFullPath(theEntry.Name);
                    string size = System.Convert.ToString(theEntry.Size);
                    string _date = theEntry.DateTime.ToShortDateString();


                    // create directory
                    if (!Directory.Exists(directoryName))
                    {
                        Directory.CreateDirectory(directoryName);
                    }

                    //计算字符数判断有无子文件夹
                    _index = System.Convert.ToInt16(theEntry.DateTime.Date.Year.ToString().Substring(2, 2));
                    //替换路径
                    newFilePath = InstallerFilePath(theEntry, REG_Values, directoryName, fileName, _index);

                    //移除打包文件中不需要恢复的文件
                    bool isUnPack = true;
                    #region 移除打包文件中不需要恢复的文件不用了
                    //foreach (string installFilePath in tempAL)
                    //{
                    //    if (installFilePath.ToLower().IndexOf(excludePackFileFolders.ToLower()) == 0 && fileExtension == ".dll")
                    //    {
                    //        isUnPack = false;
                    //        break;
                    //    }
                    //    else if (installFilePath.ToLower().IndexOf(excludePackFileFolders.ToLower()) == 0 && fileExtension == ".exe")
                    //    {
                    //        isUnPack = false;
                    //        break;
                    //    }
                    //}
                    #endregion

                    if (isUnPack)
                    {
                        #region 在视图中显示需要恢复的文件
                        //Display file in ListView					
                        ListViewItem lvi = new ListViewItem();
                        //设置文件名称
                        int loca = fileName.LastIndexOf(".");

                        if (loca == -1)
                        {
                            lvi.Text = fileName;
                        }
                        else
                        {
                            lvi.Text = fileName.Substring(0, loca);
                        }

                        //设置文件大小
                        lvi.SubItems.Add(size);
                        //设置文件类型
                        if (loca == -1)
                        {
                            lvi.SubItems.Add("");
                        }
                        else
                        {
                            lvi.SubItems.Add(fileExtension.Substring(1));
                        }
                        //设置文件路径
                        lvi.SubItems.Add(newFilePath);
                        //设置文件最后更改问时间
                        lvi.SubItems.Add("");
                        listView1.Items.Add(lvi);
                        //////////////////// end ///////////////////////////////
                        tempDL.Add(fileName);
                        tempBL.Add("");
                        //设置文件统计信息
                        int totalCount = System.Convert.ToInt16(tempDL.Count.ToString());
                        string msg = MessageResourceManager.GetString("TotalFiles");
                        msg = String.Format(msg, totalCount);
                        //MessageBox.Show(msg,"info",MessageBoxButtons.OK,MessageBoxIcon.Information);
                        statusBarPanel1.Text = msg;
                        #endregion
                    }
                }
            }
            catch (Exception e)
            {
                MessageBox.Show(e.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            finally
            {
                if (s != null) { s.Close(); }
            }
        }


        /// <summary>
        /// 恢复备份的文件
        /// </summary>
        public void UnpackFiles()
        {
            int _index = 0;
            bool flag = true;
            bool isOverrideFile;

            try
            {
                s = new ZipInputStream(File.OpenRead(packFileName));

                while ((theEntry = s.GetNextEntry()) != null)
                {
                    string directoryName = Path.GetDirectoryName(theEntry.Name);
                    //用户备份的文件,fileName=pack_"实际文件名"
                    string fileName = Path.GetFileName(theEntry.Name);

                    // create directory
                    if (!Directory.Exists(directoryName))
                    {
                        Directory.CreateDirectory(directoryName);
                    }

                    int post = fileName.LastIndexOf(".") + 1;
                    string fileType = fileName.Substring(post);

                    int post2 = directoryName.LastIndexOf("\\");
                    string filefolder = directoryName.Substring(post2 + 1);

                    bool isFilterFile = false;

                    //过滤文件,不做任何操作，直接用用户文件覆盖相同的安装文件
                    if (filefolder == copyFileFolder)
                    {
                        isFilterFile = true;//整个文件夹里所有文件用用户文件覆盖
                    }
                    else
                    {
                        foreach (string filterName in filterFile)
                        {
                            if (fileName == filterName)
                                isFilterFile = true;
                        }
                    }

                    //使用手动移走按钮移走的文件集合
                    bool isExcludePackFile = true;
                    foreach (string _filename in tempCL)
                    {
                        if (fileName == _filename)
                        {
                            isExcludePackFile = false;
                            break;
                        }
                    }

                    #region 使用两个目录进行对比文件备份的目录和指定的目录相一致时，再检查扩展名是dll，条件满足，文件不进行恢复；但目录对比不成功。
                    //int loca = theEntry.Name.LastIndexOf("\\");
                    //string filepathName = theEntry.Name.Substring(0, loca);

                    //if (directoryName.ToLower() == excludePackFileFolders.ToLower())
                    //    MessageBox.Show(directoryName.ToLower(), "Info", MessageBoxButtons.OK, MessageBoxIcon.Information);

                    //if (directoryName.ToLower() == excludePackFileFolders.ToLower() && fileType == "dll")
                    //{
                    //    isExcludePackFile = false;
                    //}
                    //else if (directoryName.ToLower() == excludePackFileFolders.ToLower() && fileType == "exe")
                    //{
                    //    isExcludePackFile = false;
                    //}
                    #endregion

                    foreach (string excludeFile in excludePackFiles)
                    {
                        if (fileName == excludeFile)
                        {
                            isExcludePackFile = false;
                            break;
                        }
                    }

                    //满足下列条件的文件不做覆盖处理（!isFilterFile 过滤文件否）
                    // if ((fileType.ToLower() == "config" || fileType.ToLower() == "xml") && !isFilterFile && File.Exists(newFilePath))
                    if ((fileType.ToLower() == "config" || fileType.ToLower() == "xml") && !isFilterFile && isExcludePackFile)
                    {
                        //计算字符数判断有无子文件夹
                        _index = System.Convert.ToInt16(theEntry.DateTime.Date.Year.ToString().Substring(2, 2));

                        isOverrideFile = false;//不覆盖原文件
                        WriteXMLFileToDisk(theEntry, REG_Values, directoryName, fileName, _index, isOverrideFile);

                        //ConvertEncodingToUTF8(REG_Values, directoryName, fileName, _index, isOverrideFile);

                        newFilePath = InstallerFilePath(theEntry, REG_Values, directoryName, fileName, _index);

                        //合并XML文档
                        MergerCustomCodeFiles mcc = new MergerCustomCodeFiles();
                        mcc.MergerXMLFile(newFilePath);

                        #region 移走XML文档节点
                        //string rootPath = REG_Values[1].ToString();
                        //if (REG_Values[0].ToString() == "HOME_DIR" && REG_Values[2].ToString() == "TOOL_DIR")//服务器版本
                        //{
                        //    MergerCustomCodeFiles rcc = new MergerCustomCodeFiles();
                        //    rcc.RemoveXMLNode(newFilePath, rootPath);
                        //}
                        #endregion

                        flag = false;
                    }
                    else
                    {
                        //用户备份文件覆盖新安装的文件
                        flag = true;
                        _index = System.Convert.ToInt16(theEntry.DateTime.Date.Year.ToString().Substring(2, 2));//计算字符数判断有无子文件夹

                        //排除移走的文件 in tempCL  使用手动移走按钮移走的文件集合
                        foreach (string _filename in tempCL)
                        {
                            if (fileName == _filename)
                            {
                                flag = false;
                                break;
                            }
                        }

                        foreach (string excludeFile in excludePackFiles)
                        {
                            if (fileName == excludeFile)
                            {
                                flag = false;
                                break;
                            }
                        }
                    }
                    //覆盖新安装的文件
                    if (flag)
                    {
                        isOverrideFile = true;
                        WriteXMLFileToDisk(theEntry, REG_Values, directoryName, fileName, _index, isOverrideFile);//覆盖新安装的文件(除config xml外)

                        //ConvertEncodingToUTF8(REG_Values, directoryName, fileName, _index, isOverrideFile);
                    }
                }
            }
            finally
            {
                if (streamWriter != null)
                {
                    streamWriter.Close();
                }

                if (s != null)
                {
                    s.Close();
                }
            }
        }

        /// <summary>
        //将备份的文件写入指定目录里的文件。
        //Save packXML file to the disk
        /// </summary>
        private void WriteXMLFileToDisk(ZipEntry theEntry, string[] REG_Values, string directoryName, string fileName, int _index, bool isOverrideFile)
        {
            //是否要覆盖原文件
            if (!isOverrideFile)
            {
                fileName = "pack_" + fileName;
            }

            //替换路径
            packFilePath = InstallerFilePath(theEntry, REG_Values, directoryName, fileName, _index);

            try
            {

                //Unpack file and write file
                //将备份的文件写入指定目录里的文件。
                if (packFilePath != String.Empty)
                {
                    streamWriter = File.Create(packFilePath);

                    int size = 2048;
                    byte[] data = new byte[2048];
                    while (true)
                    {
                        size = s.Read(data, 0, data.Length);
                        if (size > 0)
                        {
                            streamWriter.Write(data, 0, size);

                        }
                        else
                        {
                            break;
                        }
                    }
  
                }
            }
            catch (Exception)
            {
            }
            finally
            {
                if (streamWriter != null)
                {
                    streamWriter.Close();
                    streamWriter.Dispose();
                    streamWriter = null;
                }
            }
        }

        /// <summary>
        /// 将文件以UTF8的编码格式存盘。
        /// 转换编码 ANSI CONVERT TO UTF8
        /// </summary>
        private void ConvertEncodingToUTF8(string[] REG_Values, string directoryName, string fileName, int _index, bool isOverrideFile)
        {
            string tempFile = null;
            string tempFilePath = null;

            //是否要覆盖原文件
            if (!isOverrideFile)
            {
                fileName = "pack_" + fileName;
            }

            tempFile = "Temp_" + fileName;

            //替换路径
            packFilePath = InstallerFilePath(theEntry, REG_Values, directoryName, fileName, _index);
            tempFilePath = InstallerFilePath(theEntry, REG_Values, directoryName, tempFile, _index);

            //File.Create(tempFilePath);

            using (FileStream fs = new FileStream(tempFilePath, FileMode.Create, FileAccess.ReadWrite))
            {
                using (StreamWriter w = new StreamWriter(fs, Encoding.UTF8))
                {
                    StreamReader sr = new StreamReader(packFilePath);

                    char[] c = new char[2048];
                    int size = 2048;

                    while (true)
                    {
                        size = sr.Read(c, 0, c.Length);

                        if (size > 0)
                        {
                            w.Write(c, 0, size);

                        }
                        else
                        {
                            break;
                        }
                    }
                    w.Close();
                    sr.Close();
                    w.Dispose();
                    sr.Dispose();
                }
                fs.Close();
                fs.Dispose();            }

            if (File.Exists(packFilePath))
                File.Delete(packFilePath);
            if (File.Exists(tempFilePath))
                File.Move(tempFilePath, packFilePath);
        }

        /// <summary>
        //Replace installer path
        /// </summary>
        private string InstallerFilePath(ZipEntry theEntry, string[] REG_Values, string directoryName, string fileName, int _index)
        {
            string subFloder = null;
            string newPath = null;
            if (REG_Values[0].ToString() == "HOME_DIR" && REG_Values[2].ToString() == "TOOL_DIR")//服务器版本
            {
                //20AA，B，C   AA=35表示安装目录字符总数，B=1表示服务器版\B=2表示客户版，C=1表示web,C=2表示app。
                if (theEntry.DateTime.Date.Month.ToString() == "1")//判断选择的文件是否是服务器版
                {
                    if (theEntry.DateTime.Date.Day.ToString() == "1")//web
                    {
                        if (directoryName.Length <= _index)
                        {
                            newPath = REG_Values[1].ToString() + fileName;//不含有子文件夹
                        }
                        else
                        {
                            //int n = directoryName.LastIndexOf("\\");
                            subFloder = directoryName.Substring(_index);
                            newPath = REG_Values[1].ToString() + subFloder + "\\" + fileName;//含有子文件夹
                        }
                    }
                    else if (theEntry.DateTime.Date.Day.ToString() == "2")//app
                    {
                        if (directoryName.Length <= _index)
                        {
                            newPath = REG_Values[3].ToString() + fileName;//不含有子文件夹
                        }
                        else
                        {
                            newPath = REG_Values[3].ToString() + directoryName.Substring(_index) + "\\" + fileName;//含有子文件夹
                        }
                    }
                }
                else // 
                {
                    throw new Exception(MessageResourceManager.GetString("NoServer"));
                }
            }
            else if (REG_Values[0].ToString() == "TOOL_DIR")//客户版本
            {
                if (theEntry.DateTime.Date.Month.ToString() == "2")//判断选择的文件是否是服务器版
                {
                    if (directoryName.Length <= _index)
                    {
                        newPath = REG_Values[1].ToString() + fileName;//不含有子文件夹
                    }
                    else
                    {
                        newPath = REG_Values[1].ToString() + directoryName.Substring(_index) + "\\" + fileName;//含有子文件夹
                    }
                }
                else
                {
                    throw new Exception(MessageResourceManager.GetString("NoClient"));
                }
            }
            return newPath;
        }

        #region ========================没有使用到的方法=========================

        /// <summary>
        //读取XML文件比较子节点内容，更换。
        /// </summary>   
        private void updataXMLFile(ZipInputStream s, string filepath)
        {
            StringReader stream;
            XmlTextReader reader = null;
            string myString = null;

            //判断TDM Application新安装目录里文件是否存在
            if (File.Exists(filepath))
            {
                try
                {
                    //读取pack流文件原备份文件
                    StreamReader strReader = new StreamReader(s);
                    myString = strReader.ReadToEnd();
                    stream = new StringReader(myString);

                    // 从流中加载 XmlTextReader
                    reader = new XmlTextReader(stream);

                    //得到备份XML文件子结点集合
                    //fileinfolist = myFormatXML(reader, "");
                }
                catch (Exception e)
                {
                    MessageBox.Show(e.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
                finally
                {
                    // 通过 XmlTextReader 完成
                    if (reader != null)
                        reader.Close();
                    myString = "";
                }

                //读取文件
                try
                {
                    listAL.Clear();
                    // 从文件加载 XML

                    reader = new XmlTextReader(filepath);

                    //从安装目录读取新安装XML文件，获取父结点里的子结点。
                    listAL = myFormatXML(reader, "");
                }
                catch (Exception e)
                {
                    MessageBox.Show(e.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
                finally
                {
                    if (reader != null)
                        reader.Close();
                }
                try
                {
                    //compare two file content fileinfolist with (pack Web.config) and listAL(new Web.config)
                    //更新结点内容
                    CompareFileContent();

                    string flag = null;
                    string nodeName = null;
                    if (fileinfolist.Count != 0)
                        WriteFileXML(filepath, nodeName, fileinfolist, flag);

                }
                catch (Exception e)
                {
                    MessageBox.Show(e.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
            else
            {
                MessageBox.Show(MessageResourceManager.GetString("NoCustomFile"), "Warning", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            }
        }

        /// <summary>
        /// 读取pack流文件，存为XML文件到硬盘。
        /// 从硬盘读取PackXML文件和新安装的相同文件名的XML文件，比较节点内容，
        /// 进行插入和更新节点内容。
        /// </summary>   
        private void modifyPackCustomFile(ZipInputStream s, string filepath)
        {
            StringReader stream;
            XmlTextReader reader = null;
            string myString = null;

            //判断TDM Application新安装目录里文件是否存在
            if (File.Exists(filepath))
            {
                try
                {
                    //读取pack流文件原备份文件
                    StreamReader strReader = new StreamReader(s);
                    myString = strReader.ReadToEnd();
                    stream = new StringReader(myString);

                    // 从流中加载 XmlTextReader
                    reader = new XmlTextReader(stream);

                    //得到备份XML文件子结点集合
                    fileinfolist = myFormatXML(reader, "");
                }
                catch (Exception e)
                {
                    MessageBox.Show(e.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
                finally
                {
                    // 通过 XmlTextReader 完成
                    if (reader != null)
                        reader.Close();
                    myString = "";
                }

                //读取文件
                try
                {
                    listAL.Clear();
                    // 从文件加载 XML

                    reader = new XmlTextReader(filepath);

                    //从安装目录读取新安装XML文件，获取父结点里的子结点。
                    listAL = myFormatXML(reader, "");
                }
                catch (Exception e)
                {
                    MessageBox.Show(e.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
                finally
                {
                    if (reader != null)
                        reader.Close();
                }
                try
                {
                    //compare two file content fileinfolist with (pack Web.config) and listAL(new Web.config)
                    //更新结点内容
                    CompareFileContent();

                    string flag = null;
                    string nodeName = null;
                    if (fileinfolist.Count != 0)
                        WriteFileXML(filepath, nodeName, fileinfolist, flag);

                }
                catch (Exception e)
                {
                    MessageBox.Show(e.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
            else
            {
                MessageBox.Show(MessageResourceManager.GetString("NoCustomFile"), "Warning", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            }
        }

        /// <summary>
        ///compare two file content fileinfolist with (pack Web.config) and listAL(new Web.config)
        //读取web.config文件比较子节点内容，更换。
        /// </summary>
        private void CompareFileContent()
        {
            int k = 0;
            bool b = true;
            string key = null;
            string[] nodeName = new string[] { null };

            //遍历新安装的Web.config
            foreach (MyFileInfo m in listAL)
            {
                key = m.Key;

                //遍历备份Web.config
                foreach (MyFileInfo n in fileinfolist)
                {
                    if (key == n.Key)
                    {
                        foreach (string childNodeName in nodeName)
                        {
                            if (key == childNodeName)
                            {
                                n.Values = m.Values;//将新Web.config的value值覆盖备份config value值
                                break;
                            }
                        }
                        b = true;
                        break;
                    }
                    else
                    {
                        b = false;//找出在新的Web.config中添加的新节点，遍历备份Web.config比较有无相同的节点，没有就是新添的，设b=false。
                    }
                }
                if (!b) fileinfolist.Insert(k, m);//将新的Web.config中添加的新节点，插入到备份Web.config同一位置节点之后。
                k++;
            }
        }

        /// <summary>
        ////读节点里的内容,替换新的安装路径
        /// </summary>  
        private ArrayList myFormatXML(XmlReader reader, string nodeName)
        {
            ArrayList myList = new ArrayList();
            string _comment = null;
            string _key = null;
            string _values = null;
            string path = null;
            bool flag = false;

            if (Is64BitMode())
            {
                //path = "C:\\Program Files (x86)\\";
                path = "C:\\Program Files\\";
            }
            else
            {
                path = "C:\\Program Files\\";
            }

            int k = path.Length;
            int l = HKEY_KeyValue[3].Length;
            string dirname = HKEY_KeyValue[3].Substring(k, l - k - 1);

            while (reader.Read())
            {
                //读节点里的内容
                if (reader.Name == nodeName) flag = true;

                switch (reader.NodeType)
                {
                    case XmlNodeType.Comment:
                        if (flag) _comment = reader.Value;
                        break;
                    case XmlNodeType.Element:
                        if (flag)
                        {
                            while (reader.MoveToNextAttribute())
                            {
                                switch (reader.Name)
                                {
                                    case "key":
                                        _key = reader.Value;
                                        break;
                                    case "value":
                                        if (_key == "ImageBaseURL")
                                        {
                                            _values = "http://localhost/" + dirname + "/images/items/";//替换新的安装路径
                                        }
                                        if (_key == "ImageBasePath")
                                        {
                                            _values = HKEY_KeyValue[3] + "images\\items\\";//替换新的安装路径
                                        }

                                        myList.Add(new MyFileInfo(_key, _values, _comment));
                                        break;
                                }
                            }
                        }
                        break;
                }
            }
            flag = false;
            return myList;
        }

        #endregion

        /// <summary>
        /// Gets information indicating whether it is a 64 bits system
        /// </summary>
        /// <returns>true if it is 64, false otherwise</returns>
        public bool Is64BitMode()
        {
            return System.Runtime.InteropServices.Marshal.SizeOf(typeof(IntPtr)) == 8;
        }

        /// <summary>
        /// Process.Kill Method
        /// 结束Web进程
        /// </summary>
        public void ProcessKill()
        {
            string[] ProcessName ={ "aspnet_wp", "w3wp"};
            try
            {
                System.Diagnostics.Process[] myProcesses = System.Diagnostics.Process.GetProcesses();
                foreach (System.Diagnostics.Process myProcess in myProcesses)
                {
                    foreach (string pName in ProcessName)
                    {
                        if (pName == myProcess.ProcessName)
                        {
                            myProcess.Kill();

                        }
                    }                    
                }
            }
            catch (Exception e)
            {
                MessageBox.Show(e.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        // 获取服务器端操作系统
        public static string GetServerSystem()
        {
            OperatingSystem os = System.Environment.OSVersion;
            string osName = "UNKNOWN";
            switch (os.Platform)
            {
                case PlatformID.Win32Windows:
                    switch (os.Version.Minor)
                    {
                        case 0: osName = "Windows 95"; break;
                        case 10: osName = "Windows 98"; break;
                        case 90: osName = "Windows ME"; break;
                    }
                    break;
                case PlatformID.Win32NT:
                    switch (os.Version.Major)
                    {
                        case 3: osName = "Windws NT 3.51"; break;
                        case 4: osName = "Windows NT 4"; break;
                        case 5: if (os.Version.Minor == 0)
                            {
                                osName = "Windows 2000";
                            }
                            else if (os.Version.Minor == 1)
                            {
                                osName = "Windows XP";
                            }
                            else if (os.Version.Minor == 2)
                            {
                                osName = "Windows Server 2003";
                            }
                            break;
                        case 6: if (os.Version.Minor == 0)
                            {
                                osName = "Windows Vista / Windows Server 2008";
                            }
                            else if (os.Version.Minor == 1)
                            {
                                osName = "Windows 7 / Windows Server 2008";
                            }
                            break;
                    }
                      break;
                    }
            return osName;
                   
            }


        /// <summary>
        /// Int转换成Datetime
        /// </summary>
        private void MyToDate()
        {
            //DateTime dateBegin;
            int expriseDay =653;
            DateTime dtDtae = System.DateTime.Now; ;
            //dtDtae.AddDays(expriseDay);
            DateTime myDate = dtDtae.AddDays(expriseDay);
        }

        public string DateFormat
        {
            get
            {
                return "yyyy-MM-dd";
            }
        }
        public string DateTimeFormat
        {
            get
            {
                return "yyyy-MM-dd HH:mm:ss";
            }
        }


    }
}