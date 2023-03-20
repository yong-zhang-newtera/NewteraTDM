using System;
using System.IO;
using System.Data;
using System.Xml;
using System.Collections;
using Microsoft.Win32;



namespace RestoreTool
{
    /// <summary>
    /// GenerationFileList ��ժҪ˵����
    /// </summary>
    public class GenerationFileList
    {

        public GenerationFileList()
        {
            //���÷���GeneraterXMLFile
            //GeneraterXMLFile();
        }
        //static void Main()
        //{
        //GenerationFileList gf=new GenerationFileList();
        //}

        //����Ŀ¼�ļ�
        private ArrayList tempAL = new ArrayList();
        //�õ��û���װĿ¼��(ע�����ڵ���)
        private string directory_name = "Newtera";
        private string[] myPath;
        private string _type;


        public void GeneraterXMLFile()
        {
            string path1 = null, path2 = null;
            //��ע���õ���װĿ¼
            string[] InstallPath = HKEYPath(directory_name);
            _type = InstallPath[0];
            //���������ļ�Ŀ¼
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

            if (InstallPath[0] == "HOME_DIR")//��������
            {
                //��ָ���ļ��б�������
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
            else if (InstallPath[0] == "TOOL_DIR")//�ͻ���
            {
                myPath = new string[] { path1, path2 };
                //��ָ���ļ��б�������
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

        // �ݹ�ʵ������Ŀ¼�µ�������Ŀ¼���ļ�
        public void SearchDirectory(string direct_name, string directory_path, string dir)    //����Ϊָ����Ŀ¼   
        {
            int num = directory_path.Length;

            //��ָ��Ŀ¼����Ŀ¼�²����ļ�,��listBox1���г���Ŀ¼���ļ�   
            DirectoryInfo Dir = new DirectoryInfo(dir);
            try
            {
                foreach (DirectoryInfo d in Dir.GetDirectories())     //������Ŀ¼     
                {
                    SearchDirectory(direct_name, directory_path, Dir + d.ToString() + "\\");
                }

                string _fullname = null;

                foreach (FileInfo file in Dir.GetFiles("*.*"))             //�����ļ�   
                {
                    // tempAL������ļ��� 
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

        //��ע����л�ȡNewtera TDM �İ�װ·��
        private string[] HKEYPath(string nodename)
        {
            RegistryKey RK = Registry.LocalMachine;
            RegistryKey subRK = RK.OpenSubKey("SOFTWARE");
            //��"nodename"�Ӽ� 
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
    }
}
