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
    /// DataProviders ��ժҪ˵����
    /// </summary>
    /// 
    public class DataProviders
    {
        #region ˽����
        private ArrayList listAL = new ArrayList();//ԭʼ�ļ��б�New Web.config file�������á����ļ��б�
        public ArrayList tempAL = new ArrayList();
        public ArrayList tempBL = new ArrayList();
        public ArrayList tempCL = new ArrayList();
        public ArrayList tempDL = new ArrayList();
        private ArrayList fileinfolist = new ArrayList();
        private string[,] OrdinalList;//ɸѡԭʼ�ļ�
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
        //ע���ڵ���
        private const string HKEY_LOCALNODE__MACHINE_NODE = "Newtera";
        //��ע����л�ȡnewtera TDM �İ�װ·�� 
        private string[] HKEY_KeyValue;
        private string[] HKEY_KeyName;
        //CCI�ָ�ʱ���ϲ���ȫ���Ǵ�����ĵ����ļ����������ļ�(���û�ԭ�ĸ��ǰ�װ�ļ�����)
        private static string copyFileFolder = "Config";
        private static string[] filterFile ={ "web.config","Studio.exe.config","WorkflowStudio.exe.config","SmartWord.dll.config", "SmartWord.doc", "web.sitemap","web_TDP.sitemap"};

        //Newtera TDM�������ļ�,��Щ�ĵ�����Ҫ����,Cpnfig�ļ����������ļ�����Ҫ�����ݣ����ڰ�װTDM����FileList������Config�ļ�������ļ���
        private static string[] configFile = new string[] {"web.config", "Studio.exe.config","WorkflowStudio.exe.config","SmartWord.dll.config","SmartWord.doc", 
              "web_TDP.sitemap","web.sitemap","logo.png","logo.jpg"};

        //����sitemap��չ�����ļ�ȫ������
        private static string[] customFilesInExt = new string[] { ".sitemap" };

        //CCI����ʱ��Ҫ�ų����ļ����ļ���
        //�ų����漸���ļ���������
        private static string[] excludeFiles = { "catalog_sqls_oracle.xml", "catalog_sqls_sqlserver.xml", "license.rtf", "FileList.xml", "Thumbs.db" };
        //�ų�attachments ,temp�ļ�������ļ���ͼƬ�����б�
        private static string[] excludeFileFolders = { "attachments", "temp" };

        //CCI�ָ�ʱ��Ҫ�ų����ļ�,��Ϊǰ�汾�����ȥ�ļ�
        private static string[] excludePackFiles = { "RestoreTool.dll", "RestoreTool.resources.dll", "RestoreTool.exe", "ICSharpCode.SharpZipLib.dll" };

        #endregion

        public DataProviders()
        {
            //
            // TODO: �ڴ˴���ӹ��캯���߼�
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
            //��"myName"�Ӽ� 
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
        /// inetpubСдת����дInetpub
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
                //��ע������ȡnewtera TDM �İ�װ·��
                InstallPath = HKEYPath();

                if (InstallPath[0] == "HOME_DIR")
                {

                    //���������ļ�Ŀ¼
                    path1 = InstallPath[1];
                    path2 = InstallPath[3];
                    myPath = new string[] { path1, path2 };
                }
                else if (InstallPath[0] == "TOOL_DIR")
                {
                    //n_TOOL_DIR = InstallPath[1].Length;
                    //���������ļ�Ŀ¼
                    path1 = InstallPath[1];
                    myPath = new string[] { path1 };
                }

                string installer_directoryname = null;
                string installer_directorpath = null;

                //��ȡԭ�ĵ�FileList.xmlĿ¼�ļ�
                string FileListPath = InstallPath[1] + "FileList.xml";
                string NodeName = "list";

                if (File.Exists(FileListPath))
                {
                    ReadXMLFile(FileListPath, NodeName);
                    //���ListView
                    //mylistView1.Items.Clear();
                    //����û��ļ������ʱ����tempAL
                    tempAL.Clear();
                    //tempBL.Clear();
                    //tempCL.Clear();
                    //tempDL.Clear();

                    //��ָ���ļ��б��������Ƚϣ��ҳ��û���ӵ��ļ�����ListView����ʾ
                    if (InstallPath[0] == "HOME_DIR")//��������
                    {
                        //����·��һά����
                        //��ָ���ļ��б�������
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
                    else if (InstallPath[0] == "TOOL_DIR")//�ͻ���
                    {
                        myPath = new string[] { path1, path2 };
                        //��ָ���ļ��б�������
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
                    string msg = "û���㹻Ȩ�޶�ȡ" + FileListPath + "�ļ�������ϵͳ����Ա��ϵ��";
                    MessageBox.Show(msg, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
            catch (Exception e)
            {
                MessageBox.Show(e.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        /// <summary>
        /// �ݹ�ʵ������Ŀ¼�µ�������Ŀ¼���ļ�
        /// ����Ϊָ����Ŀ¼ 
        ///  /// <jparment=System.Windows.Forms.ListView>
        /// </summary>
        private void SearchDirectory(System.Windows.Forms.ListView listView1, string installer_directoryname, string installer_directorpath, string path)
        {
            int k = 0;
            bool flag = true;
            int _n = 0;

            //��ָ��Ŀ¼����Ŀ¼�²����ļ�   
            DirectoryInfo Dir = new DirectoryInfo(path);
            try
            {
                //������Ŀ¼   
                foreach (DirectoryInfo d in Dir.GetDirectories())
                {
                    //�ų�attachments ,temp�ļ�������ļ���ͼƬ�����б��ݡ�
                    if (d.ToString().ToLower() != excludeFileFolders[0].ToLower() && d.ToString().ToLower() != excludeFileFolders[1].ToLower())  
                           SearchDirectory(listView1, installer_directoryname, installer_directorpath, path + d.ToString() + "\\");
                }

                //�����û��ƶ��ļ� 
                FindCustomFiles(Dir,listView1,path,installer_directorpath,flag,_n,k);
            }
            catch (Exception e)
            {
                MessageBox.Show(e.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }

        }

        /// <summary>
        /// �ݹ�ʵ������Ŀ¼�µ�������Ŀ¼���ļ�
        /// ����Ϊָ����Ŀ¼ 
        /// <jparment=installer_directoryname>
        /// <jparment=installer_directorpath>
        /// <jparment=path>
        /// </summary>
        private void SearchDirectory(string installer_directoryname, string installer_directorpath, string path)
        {
            int k = 0;
            bool flag = true;
            int _n = 0;

            //��ָ��Ŀ¼����Ŀ¼�²����ļ�   
            DirectoryInfo Dir = new DirectoryInfo(path);
            try
            {
                //������Ŀ¼   
                foreach (DirectoryInfo d in Dir.GetDirectories())
                {
                    //�ų�attachments ,temp�ļ�������ļ���ͼƬ�����б��ݡ�
                    if (d.ToString().ToLower() != excludeFileFolders[0].ToLower() && d.ToString().ToLower() != excludeFileFolders[1].ToLower())
                        SearchDirectory(installer_directoryname, installer_directorpath, path + d.ToString() + "\\");
                }

                //�����û��ƶ��ļ� 
                FindCustomFiles(Dir,path, installer_directorpath, flag, _n, k);
            }
            catch (Exception e)
            {
                MessageBox.Show(e.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }

        }

        /// <summary>
        /// ����ָ��Ŀ¼�µ��������ļ�
        /// ����Ϊָ����Ŀ¼ 
        /// </summary>
        private void FindCustomFiles(DirectoryInfo Dir, string path, string installer_directorpath, bool flag, int _n, int k)
        {
            foreach (FileInfo file in Dir.GetFiles("*.*"))
            {
                if (flag)
                {

                    //����һά����
                    tempAL.Add(file.Name);
                    tempAL.Add(file.FullName);
                }
                k++;
            }
        }


        /// <summary>
        /// �����û��ƶ��ļ� 
        /// <partment=ListView>
        /// </summary>
        private void FindCustomFiles(DirectoryInfo Dir, ListView listView1, string path, string installer_directorpath, bool flag, int _n, int k)
        {
            foreach (FileInfo file in Dir.GetFiles("*.*"))
            {
                //�ų�attachments ,temp�ļ�������ļ���ͼƬ�����б��ݡ�
                if (Dir.Name.ToLower() != excludeFileFolders[0].ToLower() && Dir.Name.ToLower() != excludeFileFolders[1].ToLower())  
                {
                    ListViewItem lvi = new ListViewItem();
                    //��ÿ���ļ���Ϊ���������û��ļ�
                    string tempPath = null;
                    for (int i = 0; i < num; i++)
                    {
                        tempPath = OrdinalList[i, 1];
                        //��ָ�����ļ�������
                        if (tempPath.IndexOf(path) == 0)
                        {
                            //ɸѡ��Newtera TDMԭʼ�ļ��Ͳ���Ҫ���ݵ��ļ�
                            bool isExcFile = false;
                            foreach (string orgFile in excludeFiles)
                            {
                                if (file.Name == orgFile)
                                {
                                    isExcFile = true;
                                    break;
                                }      
                            }
                            //ɸѡ��Newtera TDMԭʼ�ļ��Ͳ���Ҫ���ݵ��ļ�
                            if (String.Equals(file.Name, OrdinalList[i, 0]) || file.Extension == "db" || isExcFile)
                            {
                                //flag = false;
                                //break;
                                //ָ��Ҫ���ݵ��ļ������ļ���Filelist���ڡ�
                                bool isExtensionSameFile = false;

                                foreach (string extName in customFilesInExt)
                                {
                                    if (file.Extension == extName)
                                    {
                                        isExtensionSameFile = true;
                                        break;
                                    }
                                }

                                if (isExtensionSameFile)//ָ��Ҫ���ݵ��ļ������ļ���Filelist���ڡ�
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
                        //�����ļ�����
                        int loca = file.Name.LastIndexOf(".");
                        if (loca == -1)
                        {
                            lvi.Text = file.Name;
                        }
                        else
                        {
                            lvi.Text = file.Name.Substring(0, loca);
                        }

                        //��Tag���Ա����ļ�����ȫ·��
                        //lvi.Tag=file.FullName;
                        //�����ļ���С
                        lvi.SubItems.Add(file.Length.ToString());
                        //�����ļ�����
                        if (loca == -1)
                        {
                            lvi.SubItems.Add("");
                        }
                        else
                        {
                            lvi.SubItems.Add(file.Extension.Substring(1));
                        }

                        //�����ļ�·��
                        lvi.SubItems.Add(file.FullName);
                        //�����ļ���������ʱ��
                        lvi.SubItems.Add(file.LastWriteTime.ToString());
                        listView1.Items.Add(lvi);
                        //����һά����
                        tempAL.Add(file.Name);
                        tempAL.Add(file.FullName);
                        //��ȡ�ļ������·���������ڴ������
                        //���ʱ�������Ϳͻ�����ʾ˵����
                        //2035��1��1   35��ʾ��װĿ¼�ַ�������1Ҳ��ʾ�������棬1��ʾweb,2��ʾapp��2��ʾ�ͻ���
                        _n = installer_directorpath.Length;
                        _installer_directoryname = file.FullName.Substring(0, _n);

                        // MessageBox.Show(_installer_directoryname, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);

                        if (installerDirectory.ToLower() == "HOME_DIR".ToLower())//��������
                        {
                            if (_installer_directoryname.ToLower() == HKEY_KeyValue[1].ToLower())//web view
                            {
                                //�滻·��
                                tempBL.Add("20" + HKEY_KeyValue[1].Length + ",1,1");
                            }
                            else if (_installer_directoryname.ToLower() == HKEY_KeyValue[3].ToLower())//Ӧ�ó���
                            {
                                //�滻·��
                                tempBL.Add("20" + HKEY_KeyValue[3].Length + ",1,2");
                            }
                        }
                        else if (installerDirectory.ToLower() == "TOOL_DIR".ToLower())//�ͻ���
                        {
                            if (_installer_directoryname.ToLower() == HKEY_KeyValue[1].ToLower())
                            {
                                //�滻·�� 
                                tempBL.Add("20" + HKEY_KeyValue[1].Length + ",2,1");
                            }
                        }
                    }
                    k++;
                }
            }
        }

        /// <summary>
        /// �ҳ��û��ļ�
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

                //�����ļ�ͳ����Ϣ
                int totalCount = System.Convert.ToInt16(tempAL.Count.ToString()) / 2;
                string msg = MessageResourceManager.GetString("TotalFiles");
                msg = String.Format(msg, totalCount);
                //MessageBox.Show(msg,"info",MessageBoxButtons.OK,MessageBoxIcon.Information);
                statusBarPanel1.Text = msg;
            }
            catch (Exception e)
            {
                //���ó�����Ϣ
                string FileMsg = e.Message.ToString();
                //statusBarPanel1.Text=FileMsg;
            }
        }

        /// <summary>
        /// �ҳ��û��ļ�  
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
                //���ó�����Ϣ
                string FileMsg = e.Message.ToString();
                //statusBarPanel1.Text=FileMsg;
            }
        }

        /// <summary>
        /// Add a customer file��
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
                //�õ���Ҫ��ʾ�����б��Ŀ¼��Ϣ
                DirectoryInfo myDir = new DirectoryInfo(path);
                //�õ�ָ��Ŀ¼�е�ָ���ļ�
                FileInfo[] files = myDir.GetFiles(filename);
                //�ж��б����Ƿ�����ͬ���ļ��������˳���
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
                    //�����ļ�����
                    int loca = files[0].Name.IndexOf(".");

                    if (loca == -1)
                    {
                        lvi.Text = files[0].Name;
                    }
                    else
                    {
                        lvi.Text = files[0].Name.Substring(0, loca);
                    }


                    //��Tag���Ա����ļ�����ȫ·��
                    //lvi.Tag=file.FullName;
                    //�����ļ���С
                    lvi.SubItems.Add(files[0].Length.ToString());
                    //�����ļ�����
                    // lvi.SubItems.Add(files[0].Extension.Substring(1));
                    if (loca == -1)
                    {
                        lvi.SubItems.Add("");
                    }
                    else
                    {
                        lvi.SubItems.Add(files[0].Extension.Substring(1));
                    }
                    //�����ļ�·��
                    lvi.SubItems.Add(files[0].FullName);
                    //�����ļ���������ʱ��
                    lvi.SubItems.Add(files[0].LastWriteTime.ToString());
                    listView1.Items.Add(lvi);
                    //����һά����
                    tempAL.Add(files[0].Name);
                    tempAL.Add(files[0].FullName);

                    if (installerDirectory.ToLower() == "HOME_DIR".ToLower())//��������
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
                        else if (version.ToLower() == "app".ToLower())//Ӧ�ó���
                        {
                            tempBL.Add("20" + HKEY_KeyValue[3].Length + ",1,2");
                        }
                    }
                    else if (installerDirectory.ToLower() == "TOOL_DIR".ToLower())//�ͻ���
                    {
                        _n = files[0].FullName.IndexOf(HKEY_KeyValue[1]);
                        if (_n == 0)
                        {
                            tempBL.Add("20" + HKEY_KeyValue[1].Length + ",2,1");
                        }
                    }
                }
                //�����ļ�ͳ����Ϣ
                int totalCount = System.Convert.ToInt16(tempAL.Count.ToString()) / 2;
                string msg = MessageResourceManager.GetString("TotalFiles");
                msg = String.Format(msg, totalCount);
                //MessageBox.Show(msg,"info",MessageBoxButtons.OK,MessageBoxIcon.Information);
                statusBarPanel1.Text = msg;
            }
            catch (Exception e)
            {
                //���ó�����Ϣ
                string FileMsg = e.Message.ToString();
                //statusBarPanel1.Text=FileMsg;
                MessageBox.Show(e.Message);
            }
        }
        /// <summary>
        /// Remove a customer file��
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
                    tempDL.Remove(filename);//���ListView files, ��ʾ�����ͻָ����ļ���

                    listView1.Items[indexfile].Remove();
                }
                //�����ļ�ͳ����Ϣ

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
                //���ó�����Ϣ
                string FileMsg = e.Message.ToString();
                //statusBarPanel1.Text=FileMsg;
                MessageBox.Show(e.Message);
            }
        }

        /// <summary>
        /// Insert,Update ,Add  XML �ӽڵ�,RemoveChild XML child node
        /// </summary>
        public void WriteFileXML(string pathfile, string _nodename, ArrayList myList, string flag)
        {
            XmlNode currNode;
            XmlDocument Xmldoc = new XmlDocument();

            //�Ӱ�װĿ¼���ȡ�µ�XML�ļ�
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
        /// �õ��û���װĿ¼��
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

            //������д��xml �ڵ�(file��name,path)
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
        /// �� XML�ļ� 
        /// </summary>
        public void ReadXMLFile(System.Windows.Forms.StatusBarPanel statusBarPanel1, string filiPath, string nodeName)
        {
            try
            {
                statusBarPanel1.Text = "Load file " + filiPath;
                // ���ļ����� XML
                XmlDocument myXmlDocument = new XmlDocument();
                myXmlDocument.Load(filiPath);
                //���� XmlNodeReader ����ʾȫ����¼
                XmlNodeReader myXmlNodeReader = new XmlNodeReader(myXmlDocument.SelectSingleNode(nodeName));
                listAL = FormatXml(myXmlNodeReader);

                //��ԭĿ¼�ļ�(TDM�İ�װ�ļ�����Ҫ���Ƚϵ��ļ�)��һάת��Ϊ��ά
                OrdinalList = convertToArray(listAL);
                num = OrdinalList.Length / 2;
            }
            catch (Exception e)
            {
                //���ó�����Ϣ
                //string FileMsg = e.Message.ToString();
                MessageBox.Show(e.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }
        /// <summary>
        /// �� XML�ļ� 
        /// </summary>
        public void ReadXMLFile(string filiPath, string nodeName)
        {
            try
            {
                // ���ļ����� XML
                XmlDocument myXmlDocument = new XmlDocument();
                myXmlDocument.Load(filiPath);
                //���� XmlNodeReader ����ʾȫ����¼
                XmlNodeReader myXmlNodeReader = new XmlNodeReader(myXmlDocument.SelectSingleNode(nodeName));
                listAL = FormatXml(myXmlNodeReader);

                //��ԭĿ¼�ļ�(TDM�İ�װ�ļ�����Ҫ���Ƚϵ��ļ�)��һάת��Ϊ��ά
                OrdinalList = convertToArray(listAL);
                num = OrdinalList.Length / 2;
            }
            catch (Exception e)
            {
                //���ó�����Ϣ
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
        /// �滻·��
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
                            xmlAl.Add(reader.Value);//��ȡ�ļ������ڱȽ�
                        }
                        else//�滻·��
                        {
                            _installer_directoryname = reader.Value.Substring(0, num);
                            if (installerDirectory.ToLower() == "HOME_DIR".ToLower())//��������
                            {
                                if (_installer_directoryname.ToLower() == HKEY_KeyValue[0].ToLower())
                                {
                                    _installer_directorypath = HKEY_KeyValue[1] + reader.Value.Substring(num + 1);//�滻·��
                                    xmlAl.Add(_installer_directorypath);
                                }
                                else if (_installer_directoryname.ToLower() == HKEY_KeyValue[2].ToLower())
                                {
                                    _installer_directorypath = HKEY_KeyValue[3] + reader.Value.Substring(num + 1);//�滻·��
                                    xmlAl.Add(_installer_directorypath);
                                }
                            }
                            else if (installerDirectory.ToLower() == "TOOL_DIR".ToLower())//�ͻ���
                            {
                                if (_installer_directoryname.ToLower() == HKEY_KeyValue[0].ToLower())
                                {
                                    _installer_directorypath = HKEY_KeyValue[1] + reader.Value.Substring(num + 1);//�滻·��
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
        /// �� ArrayList һά����ת��Ϊ��ά���� string [,]
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
                    //����Newtera TDM�������ļ�
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
                        //���˵����ļ�����Ϊ�յ��ļ���
                        myArray.SetValue("", i, j);
                    }
                    k++;
                }
            }
            return myArray;
        }

        /// <summary>
        /// �õ��û��ļ�·��һά�б�
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

                    //������
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
                //��ע������ȡ�ְ�װ·��
                REG_Values = HKEYPath();
				
				//����ָ���ļ��ļ����Ƴ�����ļ��в���Ҫ�ָ����ļ���
                SearchTheDirectorInfiles();

                packFileName = _packFileName;
                s = new ZipInputStream(File.OpenRead(_packFileName));
                //���Ա����ļ���Zip���
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

                    //�����ַ����ж��������ļ���
                    _index = System.Convert.ToInt16(theEntry.DateTime.Date.Year.ToString().Substring(2, 2));
                    //�滻·��
                    newFilePath = InstallerFilePath(theEntry, REG_Values, directoryName, fileName, _index);

                    //�Ƴ�����ļ��в���Ҫ�ָ����ļ�
                    bool isUnPack = true;
                    #region �Ƴ�����ļ��в���Ҫ�ָ����ļ�������
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
                        #region ����ͼ����ʾ��Ҫ�ָ����ļ�
                        //Display file in ListView					
                        ListViewItem lvi = new ListViewItem();
                        //�����ļ�����
                        int loca = fileName.LastIndexOf(".");

                        if (loca == -1)
                        {
                            lvi.Text = fileName;
                        }
                        else
                        {
                            lvi.Text = fileName.Substring(0, loca);
                        }

                        //�����ļ���С
                        lvi.SubItems.Add(size);
                        //�����ļ�����
                        if (loca == -1)
                        {
                            lvi.SubItems.Add("");
                        }
                        else
                        {
                            lvi.SubItems.Add(fileExtension.Substring(1));
                        }
                        //�����ļ�·��
                        lvi.SubItems.Add(newFilePath);
                        //�����ļ���������ʱ��
                        lvi.SubItems.Add("");
                        listView1.Items.Add(lvi);
                        //////////////////// end ///////////////////////////////
                        tempDL.Add(fileName);
                        tempBL.Add("");
                        //�����ļ�ͳ����Ϣ
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
        /// �ָ����ݵ��ļ�
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
                    //�û����ݵ��ļ�,fileName=pack_"ʵ���ļ���"
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

                    //�����ļ�,�����κβ�����ֱ�����û��ļ�������ͬ�İ�װ�ļ�
                    if (filefolder == copyFileFolder)
                    {
                        isFilterFile = true;//�����ļ����������ļ����û��ļ�����
                    }
                    else
                    {
                        foreach (string filterName in filterFile)
                        {
                            if (fileName == filterName)
                                isFilterFile = true;
                        }
                    }

                    //ʹ���ֶ����߰�ť���ߵ��ļ�����
                    bool isExcludePackFile = true;
                    foreach (string _filename in tempCL)
                    {
                        if (fileName == _filename)
                        {
                            isExcludePackFile = false;
                            break;
                        }
                    }

                    #region ʹ������Ŀ¼���жԱ��ļ����ݵ�Ŀ¼��ָ����Ŀ¼��һ��ʱ���ټ����չ����dll���������㣬�ļ������лָ�����Ŀ¼�ԱȲ��ɹ���
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

                    //���������������ļ��������Ǵ���!isFilterFile �����ļ���
                    // if ((fileType.ToLower() == "config" || fileType.ToLower() == "xml") && !isFilterFile && File.Exists(newFilePath))
                    if ((fileType.ToLower() == "config" || fileType.ToLower() == "xml") && !isFilterFile && isExcludePackFile)
                    {
                        //�����ַ����ж��������ļ���
                        _index = System.Convert.ToInt16(theEntry.DateTime.Date.Year.ToString().Substring(2, 2));

                        isOverrideFile = false;//������ԭ�ļ�
                        WriteXMLFileToDisk(theEntry, REG_Values, directoryName, fileName, _index, isOverrideFile);

                        //ConvertEncodingToUTF8(REG_Values, directoryName, fileName, _index, isOverrideFile);

                        newFilePath = InstallerFilePath(theEntry, REG_Values, directoryName, fileName, _index);

                        //�ϲ�XML�ĵ�
                        MergerCustomCodeFiles mcc = new MergerCustomCodeFiles();
                        mcc.MergerXMLFile(newFilePath);

                        #region ����XML�ĵ��ڵ�
                        //string rootPath = REG_Values[1].ToString();
                        //if (REG_Values[0].ToString() == "HOME_DIR" && REG_Values[2].ToString() == "TOOL_DIR")//�������汾
                        //{
                        //    MergerCustomCodeFiles rcc = new MergerCustomCodeFiles();
                        //    rcc.RemoveXMLNode(newFilePath, rootPath);
                        //}
                        #endregion

                        flag = false;
                    }
                    else
                    {
                        //�û������ļ������°�װ���ļ�
                        flag = true;
                        _index = System.Convert.ToInt16(theEntry.DateTime.Date.Year.ToString().Substring(2, 2));//�����ַ����ж��������ļ���

                        //�ų����ߵ��ļ� in tempCL  ʹ���ֶ����߰�ť���ߵ��ļ�����
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
                    //�����°�װ���ļ�
                    if (flag)
                    {
                        isOverrideFile = true;
                        WriteXMLFileToDisk(theEntry, REG_Values, directoryName, fileName, _index, isOverrideFile);//�����°�װ���ļ�(��config xml��)

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
        //�����ݵ��ļ�д��ָ��Ŀ¼����ļ���
        //Save packXML file to the disk
        /// </summary>
        private void WriteXMLFileToDisk(ZipEntry theEntry, string[] REG_Values, string directoryName, string fileName, int _index, bool isOverrideFile)
        {
            //�Ƿ�Ҫ����ԭ�ļ�
            if (!isOverrideFile)
            {
                fileName = "pack_" + fileName;
            }

            //�滻·��
            packFilePath = InstallerFilePath(theEntry, REG_Values, directoryName, fileName, _index);

            try
            {

                //Unpack file and write file
                //�����ݵ��ļ�д��ָ��Ŀ¼����ļ���
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
        /// ���ļ���UTF8�ı����ʽ���̡�
        /// ת������ ANSI CONVERT TO UTF8
        /// </summary>
        private void ConvertEncodingToUTF8(string[] REG_Values, string directoryName, string fileName, int _index, bool isOverrideFile)
        {
            string tempFile = null;
            string tempFilePath = null;

            //�Ƿ�Ҫ����ԭ�ļ�
            if (!isOverrideFile)
            {
                fileName = "pack_" + fileName;
            }

            tempFile = "Temp_" + fileName;

            //�滻·��
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
            if (REG_Values[0].ToString() == "HOME_DIR" && REG_Values[2].ToString() == "TOOL_DIR")//�������汾
            {
                //20AA��B��C   AA=35��ʾ��װĿ¼�ַ�������B=1��ʾ��������\B=2��ʾ�ͻ��棬C=1��ʾweb,C=2��ʾapp��
                if (theEntry.DateTime.Date.Month.ToString() == "1")//�ж�ѡ����ļ��Ƿ��Ƿ�������
                {
                    if (theEntry.DateTime.Date.Day.ToString() == "1")//web
                    {
                        if (directoryName.Length <= _index)
                        {
                            newPath = REG_Values[1].ToString() + fileName;//���������ļ���
                        }
                        else
                        {
                            //int n = directoryName.LastIndexOf("\\");
                            subFloder = directoryName.Substring(_index);
                            newPath = REG_Values[1].ToString() + subFloder + "\\" + fileName;//�������ļ���
                        }
                    }
                    else if (theEntry.DateTime.Date.Day.ToString() == "2")//app
                    {
                        if (directoryName.Length <= _index)
                        {
                            newPath = REG_Values[3].ToString() + fileName;//���������ļ���
                        }
                        else
                        {
                            newPath = REG_Values[3].ToString() + directoryName.Substring(_index) + "\\" + fileName;//�������ļ���
                        }
                    }
                }
                else // 
                {
                    throw new Exception(MessageResourceManager.GetString("NoServer"));
                }
            }
            else if (REG_Values[0].ToString() == "TOOL_DIR")//�ͻ��汾
            {
                if (theEntry.DateTime.Date.Month.ToString() == "2")//�ж�ѡ����ļ��Ƿ��Ƿ�������
                {
                    if (directoryName.Length <= _index)
                    {
                        newPath = REG_Values[1].ToString() + fileName;//���������ļ���
                    }
                    else
                    {
                        newPath = REG_Values[1].ToString() + directoryName.Substring(_index) + "\\" + fileName;//�������ļ���
                    }
                }
                else
                {
                    throw new Exception(MessageResourceManager.GetString("NoClient"));
                }
            }
            return newPath;
        }

        #region ========================û��ʹ�õ��ķ���=========================

        /// <summary>
        //��ȡXML�ļ��Ƚ��ӽڵ����ݣ�������
        /// </summary>   
        private void updataXMLFile(ZipInputStream s, string filepath)
        {
            StringReader stream;
            XmlTextReader reader = null;
            string myString = null;

            //�ж�TDM Application�°�װĿ¼���ļ��Ƿ����
            if (File.Exists(filepath))
            {
                try
                {
                    //��ȡpack���ļ�ԭ�����ļ�
                    StreamReader strReader = new StreamReader(s);
                    myString = strReader.ReadToEnd();
                    stream = new StringReader(myString);

                    // �����м��� XmlTextReader
                    reader = new XmlTextReader(stream);

                    //�õ�����XML�ļ��ӽ�㼯��
                    //fileinfolist = myFormatXML(reader, "");
                }
                catch (Exception e)
                {
                    MessageBox.Show(e.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
                finally
                {
                    // ͨ�� XmlTextReader ���
                    if (reader != null)
                        reader.Close();
                    myString = "";
                }

                //��ȡ�ļ�
                try
                {
                    listAL.Clear();
                    // ���ļ����� XML

                    reader = new XmlTextReader(filepath);

                    //�Ӱ�װĿ¼��ȡ�°�װXML�ļ�����ȡ���������ӽ�㡣
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
                    //���½������
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
        /// ��ȡpack���ļ�����ΪXML�ļ���Ӳ�̡�
        /// ��Ӳ�̶�ȡPackXML�ļ����°�װ����ͬ�ļ�����XML�ļ����ȽϽڵ����ݣ�
        /// ���в���͸��½ڵ����ݡ�
        /// </summary>   
        private void modifyPackCustomFile(ZipInputStream s, string filepath)
        {
            StringReader stream;
            XmlTextReader reader = null;
            string myString = null;

            //�ж�TDM Application�°�װĿ¼���ļ��Ƿ����
            if (File.Exists(filepath))
            {
                try
                {
                    //��ȡpack���ļ�ԭ�����ļ�
                    StreamReader strReader = new StreamReader(s);
                    myString = strReader.ReadToEnd();
                    stream = new StringReader(myString);

                    // �����м��� XmlTextReader
                    reader = new XmlTextReader(stream);

                    //�õ�����XML�ļ��ӽ�㼯��
                    fileinfolist = myFormatXML(reader, "");
                }
                catch (Exception e)
                {
                    MessageBox.Show(e.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
                finally
                {
                    // ͨ�� XmlTextReader ���
                    if (reader != null)
                        reader.Close();
                    myString = "";
                }

                //��ȡ�ļ�
                try
                {
                    listAL.Clear();
                    // ���ļ����� XML

                    reader = new XmlTextReader(filepath);

                    //�Ӱ�װĿ¼��ȡ�°�װXML�ļ�����ȡ���������ӽ�㡣
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
                    //���½������
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
        //��ȡweb.config�ļ��Ƚ��ӽڵ����ݣ�������
        /// </summary>
        private void CompareFileContent()
        {
            int k = 0;
            bool b = true;
            string key = null;
            string[] nodeName = new string[] { null };

            //�����°�װ��Web.config
            foreach (MyFileInfo m in listAL)
            {
                key = m.Key;

                //��������Web.config
                foreach (MyFileInfo n in fileinfolist)
                {
                    if (key == n.Key)
                    {
                        foreach (string childNodeName in nodeName)
                        {
                            if (key == childNodeName)
                            {
                                n.Values = m.Values;//����Web.config��valueֵ���Ǳ���config valueֵ
                                break;
                            }
                        }
                        b = true;
                        break;
                    }
                    else
                    {
                        b = false;//�ҳ����µ�Web.config����ӵ��½ڵ㣬��������Web.config�Ƚ�������ͬ�Ľڵ㣬û�о�������ģ���b=false��
                    }
                }
                if (!b) fileinfolist.Insert(k, m);//���µ�Web.config����ӵ��½ڵ㣬���뵽����Web.configͬһλ�ýڵ�֮��
                k++;
            }
        }

        /// <summary>
        ////���ڵ��������,�滻�µİ�װ·��
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
                //���ڵ��������
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
                                            _values = "http://localhost/" + dirname + "/images/items/";//�滻�µİ�װ·��
                                        }
                                        if (_key == "ImageBasePath")
                                        {
                                            _values = HKEY_KeyValue[3] + "images\\items\\";//�滻�µİ�װ·��
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
        /// ����Web����
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

        // ��ȡ�������˲���ϵͳ
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
        /// Intת����Datetime
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