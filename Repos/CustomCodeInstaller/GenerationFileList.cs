using System;
using System.IO;
using System.Data;
using System.Xml;
using System.Collections;
using Microsoft.Win32;



namespace RestoreTool
{
    /// <summary>
    /// GenerationFileList 的摘要说明。
    /// </summary>
    public class GenerationFileList
    {

        public GenerationFileList()
        {
            //调用方法GeneraterXMLFile
            //GeneraterXMLFile();
        }
        //static void Main()
        //{
        //GenerationFileList gf=new GenerationFileList();
        //}

        //生成目录文件
        private ArrayList tempAL = new ArrayList();
        //得到用户安装目录名(注册表里节点名)
        private string directory_name = "Newtera";
        private string[] myPath;
        private string _type;


        public void GeneraterXMLFile()
        {
            string path1 = null, path2 = null;
            //从注册表得到安装目录
            string[] InstallPath = HKEYPath(directory_name);
            _type = InstallPath[0];
            //设置搜索文件目录
            if (InstallPath[0] == "HOME_DIR" && InstallPath[2] == "TOOL_DIR")
            {
                path1 = InstallPath[1];
                path2 = InstallPath[3];
                myPath = new string[] {path1, path2};
            }
            else if (InstallPath[0] == "TOOL_DIR")
            {
                path1 = InstallPath[1];
                myPath = new string[] {path1};
            }

            string installer_directoryname = null;
            string installer_directorpath = null;

            if (InstallPath[0] == "HOME_DIR")//服务器版
            {
                //对指定文件夹遍历搜索
                foreach (string path in myPath)
                {
                    if (path != InstallPath[3])
                    {
                        installer_directoryname = InstallPath[0];
                        installer_directorpath = InstallPath[1];
                        SearchDirectory(installer_directoryname, installer_directorpath, path);
                    }
                    else
                    {
                        installer_directoryname = InstallPath[2];
                        installer_directorpath = InstallPath[3];
                        SearchDirectory(installer_directoryname, installer_directorpath, path);
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
                    SearchDirectory(installer_directoryname, installer_directorpath, path);
                }
            }
            string filename = InstallPath[1] + "FileList.xml";
            WriteFileXML(filename);
            tempAL.Clear();
        }

        // 递归实现搜索目录下的所有子目录和文件
        public void SearchDirectory(string direct_name, string directory_path, string dir)    //参数为指定的目录   
        {
            int num = directory_path.Length;

            //在指定目录及子目录下查找文件,在listBox1中列出子目录及文件   
            DirectoryInfo Dir = new DirectoryInfo(dir);
            try
            {
                foreach (DirectoryInfo d in Dir.GetDirectories())     //查找子目录     
                {
                    SearchDirectory(direct_name, directory_path, Dir + d.ToString() + "\\");
                }

                string _fullname = null;

                foreach (FileInfo file in Dir.GetFiles("*.*"))             //查找文件   
                {
                    // tempAL中填加文件名 
                    _fullname = direct_name + "\\" + file.FullName.Substring(num);
                    tempAL.Add(file.Name);
                    tempAL.Add(_fullname);
                }
            }
            catch (Exception e)
            {
                Console.Write(e.Message.ToString());
            }
        }

        //从注册表中获取Newtera TDM 的安装路径
        private string[] HKEYPath(string nodename)
        {
            RegistryKey RK = Registry.LocalMachine;
            RegistryKey subRK = RK.OpenSubKey("SOFTWARE");
            //打开"nodename"子键 
            RegistryKey node = subRK.OpenSubKey(nodename);
            string[] KeyName = node.GetValueNames();
            string[] KeyValue = new string[KeyName.Length * 2];
            int i = 0;
            foreach (string kn in KeyName)
            {
                KeyValue.SetValue(kn, i);
                i++;
                KeyValue.SetValue(node.GetValue(kn), i);
                i++;
            }
            return KeyValue;
        }
        //Write xml file
        public void WriteFileXML(string filename)
        {
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
            writer.WriteAttributeString("type", _type);

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
    }
}
