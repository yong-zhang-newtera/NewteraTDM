using System;
using System.IO;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using System.Xml;
using Microsoft.Win32;
using System.Windows.Forms;

using FileInfomation;

namespace RestoreTool
{
    /// <summary>
    /// �ϲ�XML�ĵ�
    /// ��PackXML�ļ����°�װ��XML�ļ����бȽ��ӽڵ����ݣ���ֵ��ͬ�����롢�滻��
    /// 
    ///     �û��ļ��ڵ�         ���ļ��ڵ�           �Ƚ�ֵ         �Ƿ����       �Ƿ��滻
    /// -------------------------------------------------------------------------------------
    ///    û���޸ĸýڵ�       û���޸ĸýڵ�         ��ͬ             ��             ��
    ///    �Ĺ��ýڵ�           û���޸ĸýڵ�         ��ͬ             ����           ����
    ///    û���޸ĸýڵ�       �޸Ĺ��ýڵ�           ��ͬ             ��             ��
    ///    û�иýڵ�           ����ڵ�               ��               ��             ��
    ///    ����ڵ�             û�иýڵ�             ��               ��             ��
    ///    �޸Ĺ��ýڵ�         �޸Ĺ��ýڵ�           ��ͬ             ��ȷ��         ��ȷ��
    /// -------------------------------------------------------------------------------------
    /// 1.�滻�ڵ���жϣ�
    ///   ���������ڵ�������Ϊassembly=,(name= type=),(key=��value=)���Ƚ�������ֵ����ͬ�������滻����
    /// 2.�ڵ��������жϣ�
    ///   ���˽ڵ�������Ϊassembly���⣬�ȽϽ��û����ͬ�Ľڵ㣬��Ϊ�����ڵ������봦��
    ///   ���������XML�ڵ��������������
    ///   a.�������ͬ��b.�����ڵ������Ĳ��(���νڵ�)λ��һ�¡�
    /// 3.���˽ڵ㣺"authorizatio"��"deny users="*""��"allow users="?" "
    /// </summary>  
    public struct MergerCustomCodeFiles
    {
        private bool isHasNewNode;
        private bool isNextNode;
        private bool isInser;
        private bool isFilter;
        private bool isReplace;
        private bool isNewAttributes;
        private bool isMergerXMLNode;
        private bool isRemoveXMLNode;
        private bool isSuccessLoadXmlFile;
        private XmlNode refChildNode;
        private XmlNode refParentNode;
        private XmlNode currParentNode;
        private XmlNode currPackChildNode;
        private XmlDocument newXmldoc;
        private XmlDocument packXmldoc;
        private int indexNewXmlNode;
        private int newChildNodePostion;
        private int packChildNodePostion;
        private XmlNode currPackNode;
        private static string[] filterNode ={ "authorization", "allow", "deny" };
        private static string[] nodeAttributesName = { "value", "theme" };
        //private static string[] nodeAttributesName = { "assembly" ,"type","value","theme"};
        //private static string[] replaceParentNode = { "appSettings", "AlgorithmTypes", "ChartFormats" ,"ExportTypes","FileTypes"};
        private static string httpHead = "http://";
        private const string removeNodeFile = "bin\\DeleteXMLNode.xml";

        /// <summary>
        /// �ϲ�XML�ļ�
        /// newFilePath��packFilePathΪXML�ļ�·��
        /// </summary>
        public void MergerXMLFile(string newFilePath)
        {
            #region set value
            isMergerXMLNode = true;
            isRemoveXMLNode = false;
            int post = newFilePath.LastIndexOf("\\");
            string packFilePath = newFilePath.Substring(0, post + 1) + "pack_" + newFilePath.Substring(post + 1);

            XmlNode newCurrNode = null;
            XmlNode packCurrNode = null;
            #endregion

            isSuccessLoadXmlFile = isLoadXMLFile(newFilePath, packFilePath);

            if (isSuccessLoadXmlFile)
            {
                //�õ�packXmldocXML�ĵ���һ��Element�ڵ�
                packCurrNode = GetRootElement(packXmldoc);
                //�õ�newXmldoc�ĵ���һ��Element�ڵ�
                newCurrNode = GetRootElement(newXmldoc);

                //Search XML Files
                if (newCurrNode != null && packCurrNode != null)
                {
                    SearchXMLNode(newCurrNode, packCurrNode);
                }
                else
                {
                    string msg = "newCurrNode=" + newCurrNode.ToString() + " " + "packCurrNode=" + packCurrNode.ToString();
                    MessageBox.Show(msg, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }

                //���ϲ����CustomCode�ļ�����
                packXmldoc.Save(newFilePath);

                //ɾ���ļ�
                if (File.Exists(packFilePath))
                    File.Delete(packFilePath);
            }
            else
            {
                //��ȡXML�ļ�ʧ�ܣ������û�����XMl�ļ�
                if (File.Exists(newFilePath))
                    File.Delete(newFilePath);

                File.Move(packFilePath, newFilePath);
            }
        }


        private void ReadXML(string fullPath)
        {
            //FileStream fs = File.Open(fullPath, FileMode.Open);
            //StreamReader strReader = new StreamReader(fs);

            StreamReader strReader = new StreamReader(fullPath, Encoding.Default);

            Encoding ed = strReader.CurrentEncoding;
            String myString = strReader.ReadToEnd();
            StringReader stream = new StringReader(myString);

            // �����м��� XmlTextReader
            XmlTextReader reader = new XmlTextReader(stream);

            strReader.Close();
            stream.Close();
            reader.Close();                
       }

        private void WriteFile(string packFilePath, string newFilePath)
        {
            // Create a temporary file, and put some data into it.
            //using (FileStream fs = File.Open(path, FileMode.Open, FileAccess.Write, FileShare.None))
            //{
            //    Byte[] info = new UTF8Encoding(true).GetBytes("");
            //    // Add some information to the file.
            //    fs.Write(info, 0, info.Length);
            //    fs.close();
            //}

            // Open the stream and read it back.
            using (FileStream fs = File.Open(packFilePath, FileMode.Open))
            {
                FileStream streamWriter = null;
                streamWriter = File.Create(newFilePath);

                byte[] b = new byte[1024];

                while (fs.Read(b, 0, b.Length) > 0)
                {
                    //Console.WriteLine(temp.GetString(b));
                    streamWriter.Write(b, 0, b.Length);
                }

                fs.Close();
                streamWriter.Close();
            }
        }

        private void SavePackXML(string packFilePath, string newFilePath)
        {
            FileStream streamWriter = null;
            StreamReader sr = null;
            sr = new StreamReader(newFilePath);

            streamWriter = File.Create(packFilePath);

            char[] c = new char[2048];

            int size = 2048;
            byte[] data = new byte[2048];
            while (true)
            {
                size = sr.Read(c, 0, c.Length);
                if (size > 0)
                {
                    streamWriter.Write(data, 0, size);

                }
                else
                {
                    break;
                }
            }

            streamWriter.Close();
            streamWriter = null;
            File.Delete(packFilePath);
        }

        /// <summary>
        /// ����XML�ļ��ڵ�
        /// newFilePath��packFilePathΪXML�ļ�·��
        /// </summary>
        public void RemoveXMLNode(string newFilePath, string xmlNodeFilePath)
        {
            #region set value
            isMergerXMLNode = false;
            isRemoveXMLNode = true;
            xmlNodeFilePath = xmlNodeFilePath + removeNodeFile;
            XmlNode newCurrNode = null;
            XmlNode packCurrNode = null;
            #endregion

            if (isSuccessLoadXmlFile)
            {
                //�õ�newXmldoc�ĵ���һ��Element�ڵ�
                newCurrNode = GetRootElement(newXmldoc);
                //�õ�packXmldocXML�ĵ���һ��Element�ڵ�
                packCurrNode = GetRootElement(packXmldoc);

                //ɾ���ڵ��ļ�����Ҫɾ�����ӽڵ㣬���������Ƚ�
                if (newCurrNode.HasChildNodes)
                {
                    if (newCurrNode != null && packCurrNode != null)
                    {
                        //Search XML Files
                        SearchXMLNode(newCurrNode, packCurrNode);
                    }
                    else
                    {
                        string msg = "newCurrNode=" + newCurrNode.ToString() + " " + "packCurrNode=" + packCurrNode.ToString();
                        MessageBox.Show(msg, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    }

                    //�����߽ڵ���CustomCode�ļ�����
                    packXmldoc.Save(newFilePath);
                }
            }
        }

        /// <summary>
        /// Get root fist element of xml file
        /// </summary>
        private XmlNode GetRootElement(XmlDocument xmlDoc)
        {
            XmlNode rootNode = null;

            if (xmlDoc.HasChildNodes)
            {
                //ȷ��newXmldoc�ĵ���һ��Element�ڵ�
                for (int i = 0; i < xmlDoc.ChildNodes.Count; i++)
                {
                    if (xmlDoc.ChildNodes.Item(i).NodeType == XmlNodeType.Element)
                    {
                        rootNode = xmlDoc.ChildNodes.Item(i);
                    }
                }
            }
            return rootNode;
        }

        /// <summary>
        /// Load XML file
        /// </summary>
        public bool isLoadXMLFile(string newFilePath, string packFilePath)
        {
            bool isLoad = true;
            newXmldoc = new XmlDocument();
            packXmldoc = new XmlDocument();

            //�Ӱ�װĿ¼���ȡ����XML�ļ� 
            try
            {
                //���Ƚ��ļ�
                newXmldoc.Load(packFilePath);
                //��׼�ļ�
                packXmldoc.Load(newFilePath);
            }
            catch
            {
                isLoad = false;
            }
            return isLoad;
        }

        /// <summary>
        /// ���µ��ļ�newXmldocΪ��׼���еݹ�����
        /// </summary>
        private void SearchXMLNode(XmlNode newCurrNode, XmlNode packCurrNode)
        {
            isNextNode = true;
            refChildNode = packCurrNode;

            foreach (XmlNode childNode in newCurrNode)
            {
                #region set value
                isFilter = false;
                isInser = false;
                isReplace = false;
                isNewAttributes = true;//�������µ�����ֵ
                isHasNewNode = true; //��ȡ��ÿ��childNode��������������ӽڵ�
                refParentNode = packCurrNode;
                currParentNode = newCurrNode;
                #endregion

                //���� CompareXMLNode��������Xml�ļ����нڵ���бȽϡ�
                CompareXMLNode(childNode, packCurrNode);

                //isNextNodeΪfalse ��ʾ���й��½ڵ�������,���ٽ����ӽڵ�ѭ��
                if (isNextNode)
                {
                    newChildNodePostion++;//�ڵ�λ��
                    SearchXMLNode(childNode, packCurrNode);     //�ݹ�ѭ��               
                }
                indexNewXmlNode++;
            }
            indexNewXmlNode = 0;
            newChildNodePostion = 0;
        }

        /// <summary>
        /// ������XML�ļ��Ľڵ���бȽϡ�
        /// </summary>
        private void CompareXMLNode(XmlNode newCurrNode, XmlNode packCurrNode)
        {
            foreach (XmlNode childNode in packCurrNode)
            {
                //��PackXML��NewXML�ڵ���������ֵ��ͬ���жϱ���ѭ���������¼��ӽڵ�ѭ����
                if (newCurrNode.Name == childNode.Name || isRemoveXMLNode) //ͬһ�����νڵ�ѭ���Ƚ�
                {
                    //�Ƚ�Xml�ڵ������ֵ�Ƿ���ͬ,�ж��Ƿ������ѭ��
                    bool isIntoChildNodeCircle = CompareXmlNodeType(childNode, newCurrNode, packCurrNode);

                    currPackChildNode = childNode;

                    //isIntoChildNodeCircle����true����newCurrNode�ڵ��ӽڵ�ѭ��
                    if (isIntoChildNodeCircle)
                        SearchXMLNode(newCurrNode, childNode); //�ݹ�ѭ��

                    if (!isNewAttributes) //���ڵ�����ֵ��ͬ�ж�ѭ��
                    {
                        isNextNode = false;
                        break;
                    }
                }

                //PackFile�����ӽڵ�ݹ�ѭ����ȷ���Ƚ�����ͬһ����ڵ��ڽ��к��½ڵ�������
                if (newCurrNode.ParentNode != null && packCurrNode.ParentNode != null)
                {
                    if (newCurrNode.ParentNode.Name == packCurrNode.ParentNode.Name || isRemoveXMLNode)
                    {
                        packChildNodePostion++;
                        CompareXMLNode(newCurrNode, childNode); //ͬһ�����ڵ�������ӽڵ�ѭ��
                    }
                }
            }

            #region Insert  Replace Remove of child Node isHasNewNode
            packChildNodePostion = 0;
            isInser = isInsertNode(newCurrNode);
            isFilter = isFilterNode(newCurrNode);
            if (isMergerXMLNode)
            {
                if (isReplace && !isFilter)
                {
                    //�滻XML�ڵ�
                    ReplaceXMLNode(packXmldoc, newCurrNode);
                }
                else if (isInser && !isFilter && !isReplace)
                {
                    //���������XML�ڵ�
                    ChooseNodeType(packXmldoc, newCurrNode, packCurrNode, refChildNode);
                }
            }
            else if (isRemoveXMLNode)
            {
                if (!isHasNewNode)
                    ChooseNodeType(packXmldoc, newCurrNode, packCurrNode, refChildNode);
            }
            #endregion
        }

        /// <summary>
        /// ���˽ڵ� "authorization", "allow", "deny"�����⼸���ڵ�ֵ�������롣
        /// <return>return bool</return>
        /// </summary>
        private bool isFilterNode(XmlNode newCurrNode)
        {
            if (newCurrNode.ParentNode != null)
            {
                foreach (string nodeName in filterNode)
                {
                    if (newCurrNode.ParentNode.Name == nodeName ||
                        newCurrNode.Name == nodeName)
                    {
                        return isFilter = true;
                    }
                }
            }
            return isFilter;
        }

        /// <summary>
        /// �Ƿ���Ҫ����Ľڵ�
        /// <return>return bool</return>
        /// </summary>
        private bool isInsertNode(XmlNode newCurrNode)
        {
            //���������XML�ڵ��������������
            //1���������ͬ��2��ÿ���ڵ㶼����һ�顢3�����µĽڵ㡢4�������ڵ�������λ��һ�¡�
            if (newCurrNode.ParentNode != null && refParentNode != null)
            {
                if (newCurrNode.ParentNode.Name == refParentNode.Name &&
                    isHasNewNode &&
                    newChildNodePostion == packChildNodePostion)
                {
                    isInser = true;
                }
                else
                {
                    isInser = false;
                }
            }
            return isInser;
        }

        /// <summary>
        /// ����½ڵ��Ǹ��µĽڵ�,������Ҫ�Ƚϵ�ʵ��
        /// <add assembly="System.DirectoryServices, Version=2.0.0.0, Culture=neutral, PublicKeyToken=B03F5F7F11D50A3A" />
        /// <add name="ScriptModule" type="System.Web.Handlers.ScriptModule, System.Web.Extensions, Version=1.0.61025.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35" />
        /// <return>return bool</return>
        /// </summary>
        private bool isReplaceNode(XmlNode newCurrNode, XmlNode childNode, int itemIndex)
        {
            #region Set Value
            int currPost = -1;
            int packPost = -1;
            string strHead = null;
            string strLast = null;
            string strPackHead = null;
            string strPackLast = null;
            #endregion

            if (!String.IsNullOrEmpty(newCurrNode.Attributes.Item(itemIndex).Value))
                currPost = newCurrNode.Attributes.Item(itemIndex).Value.IndexOf(",");

            if (!String.IsNullOrEmpty(childNode.Attributes.Item(itemIndex).Value))
                packPost = childNode.Attributes.Item(itemIndex).Value.IndexOf(",");

            try
            {
                //����<add assembly="System.DirectoryServices, Version=2.0.0.0, Culture=neutral, PublicKeyToken=B03F5F7F11D50A3A" />           
                if (currPost >= 0 && packPost >= 0)
                {
                    strHead = newCurrNode.Attributes.Item(itemIndex).Value.Substring(0, currPost).Trim();
                    strLast = newCurrNode.Attributes.Item(itemIndex).Value.Substring(currPost + 1).Trim();
                    strPackHead = childNode.Attributes.Item(itemIndex).Value.Substring(0, packPost).Trim();
                    strPackLast = childNode.Attributes.Item(itemIndex).Value.Substring(packPost + 1).Trim();

                    if (newCurrNode.Attributes.Item(itemIndex).Name == childNode.Attributes.Item(itemIndex).Name &&
                        newCurrNode.Attributes.Item(itemIndex).Value == childNode.Attributes.Item(itemIndex).Value)
                    {
                        isReplace = false;
                    }
                    else if (!string.IsNullOrEmpty(strHead) && !string.IsNullOrEmpty(strLast) &&
                                 !string.IsNullOrEmpty(strPackHead) && !string.IsNullOrEmpty(strPackLast))
                    {
                        if (newCurrNode.Attributes.Item(itemIndex).Name == childNode.Attributes.Item(itemIndex).Name &&
                            strHead == strPackHead)
                        {
                            if (strLast == strPackLast)
                            {
                                isReplace = false;
                            }
                            else
                            {
                                currPackNode = childNode;
                                isReplace = true;
                            }
                        }
                    }
                }
                else
                {
                    //����<pages theme="BlueTheme">
                    if (newCurrNode.Attributes.Item(itemIndex).Value != childNode.Attributes.Item(itemIndex).Value)
                    {
                        currPackNode = childNode;
                        isReplace = true;
                    }
                    else
                    {
                        isReplace = false;
                    }
                }
            }
            catch (Exception e)
            {
                MessageBox.Show(e.Message, "ReplaceNodeError", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }

            return isReplace;
        }

        /// <summary>
        /// ����Ƿ����µ�����ֵ
        /// <return>return bool</return>
        /// </summary>
        private bool isNewAttributesValue(XmlNode newCurrNode, XmlNode childNode)
        {
            int post = newCurrNode.InnerText.LastIndexOf("/") + 1;
            string fileName = null;

            if (post >= 0)
                fileName = newCurrNode.InnerText.Substring(post);

            if (newCurrNode.InnerText != null && childNode.InnerText != null)
            {
                //���child.InnerText��һ���ͽڵ�.
                //���磺<value>http://localhost/Newtera/WebService/AdminWebService.asmx</value>                
                if (newCurrNode.InnerText.StartsWith(httpHead))
                {
                    if (childNode.InnerText.EndsWith(fileName))
                    {
                        isNewAttributes = false;
                    }
                }
            }
            return isNewAttributes;
        }

        /// <summary>
        /// Choose a type of compare node
        /// comment and element compare
        /// </summary>
        private bool CompareXmlNodeType(XmlNode childNode,
                                        XmlNode newCurrNode,
                                        XmlNode packCurrNode)
        {
            bool isIntoChildCircle = false;

            //�ڵ�����ֵ��ͬ���ж�CompareXMLNode����ѭ����
            switch (childNode.NodeType)
            {
                case XmlNodeType.Comment:
                    if (newCurrNode.Value == childNode.Value)
                    {
                        isHasNewNode = false;
                        isIntoChildCircle = false;
                    }
                    break;
                case XmlNodeType.Element:
                    if (newCurrNode.Attributes != null && childNode.Attributes != null &&
                        newCurrNode.HasChildNodes)
                    {
                        //��ǰ�ڵ�û������ֵ
                        if (newCurrNode.Attributes.Count == 0 && childNode.Attributes.Count == 0)
                        {
                            isHasNewNode = false;
                            isIntoChildCircle = true;
                        }
                        else
                        //��ǰ�ڵ�������ֵ����������ֵ�Ƚϡ�
                        {
                            for (int i = 0; i < newCurrNode.Attributes.Count; i++)
                            {
                                //string str = null;
                                //if (newCurrNode.Attributes.Item(i).Value == "custom")
                                //    str = newCurrNode.Attributes.Item(i).Value;

                                if (newCurrNode.Attributes.Item(i) != null && childNode.Attributes.Item(i) != null)
                                {
                                    if (newCurrNode.Attributes.Item(i).Name == childNode.Attributes.Item(i).Name &&
                                      newCurrNode.Attributes.Item(i).Value == childNode.Attributes.Item(i).Value)
                                    {
                                        //isNewAttributesΪfalse�������й�һ�β������,���ٽ�����ѭ��
                                        if (isNewAttributes)
                                        {
                                            isHasNewNode = false;
                                            isIntoChildCircle = true;
                                        }
                                    }
                                    else
                                    {
                                        isIntoChildCircle = false;
                                        isHasNewNode = true;
                                        break;
                                    }
                                }
                            }
                        }
                    }
                    //�Ƚ�������ӽڵ�<add name="" />��ʽ
                    else if (newCurrNode.Attributes != null && childNode.Attributes != null &&
                              !newCurrNode.HasChildNodes)
                    {
                        if (newCurrNode.Attributes.Count > 0)
                        {
                            //��ǰ�ڵ�������ֵ����������ֵ�Ƚϡ�
                            for (int i = 0; i < newCurrNode.Attributes.Count; i++)
                            {
                                if (newCurrNode.Attributes.Item(i) != null && childNode.Attributes.Item(i) != null)
                                {
                                    if (newCurrNode.Attributes.Item(i).Name == childNode.Attributes.Item(i).Name &&
                                      newCurrNode.Attributes.Item(i).Value == childNode.Attributes.Item(i).Value)
                                    {
                                        //ѭ���Ƚϵ����һ������ֵ
                                        if (i == newCurrNode.Attributes.Count - 1)
                                        {
                                            isHasNewNode = false; //No New Node
                                            isIntoChildCircle = false;
                                        }
                                    }
                                    else
                                    {
                                        //�жϸýڵ��Ƿ����滻���Ͳ���,����true�滻�ڵ�.                                     
                                        if (i == 0)
                                        {
                                            //����ָ���滻�ڵ�������
                                            //����<pages theme="BlueTheme">   
                                            //�ڵ�����<add assembly="System.DirectoryServices, Version=2.0.0.0, Culture=neutral, PublicKeyToken=B03F5F7F11D50A3A" />
                                            foreach (string attributesName in nodeAttributesName)
                                            {
                                                if (newCurrNode.Attributes.Item(i).Name == attributesName)
                                                {
                                                    isReplace = isReplaceNode(newCurrNode, childNode, i);
                                                    isIntoChildCircle = false;
                                                }
                                            }
                                        }
                                        else
                                        {
                                            //�ڵ�����<add key="EmailPassword" value="123456" />
                           
                                            //����ָ���滻�ڵ�������
                                            foreach (string attributesName in nodeAttributesName)
                                            {
                                                if (newCurrNode.Attributes.Item(i).Name == attributesName)
                                                {
                                                    isReplace = true;
                                                    currPackNode = childNode;
                                                    break;
                                                }
                                            }
                                        }
                                        break;
                                    }
                                }
                            }
                        }
                        else
                        {
                            //��ǰ�ڵ�û������ֵ(���磺<appSettings/>)������OuterXmlֵ�Ƚϡ�
                            if (newCurrNode.OuterXml == childNode.OuterXml)
                            {
                                isHasNewNode = false; //No New Node
                                isIntoChildCircle = false;
                            }
                        }
                    }

                    break;
                case XmlNodeType.Text:
                    //�Ƚ�child.InnerText��һ���ͽڵ�.
                    //����:<value>http://localhost/Newtera/WebService/AdminWebService.asmx</value>
                    if (!string.IsNullOrEmpty(newCurrNode.InnerText) &&
                        !string.IsNullOrEmpty(childNode.InnerText))
                    {
                        isNewAttributes = isNewAttributesValue(newCurrNode, childNode);

                        if (!isNewAttributes)
                        {
                            isHasNewNode = false; //No New Attributes
                            isIntoChildCircle = false;
                        }
                    }

                    break;
                case XmlNodeType.Document:
                    break;
                case XmlNodeType.None:
                    break;
            }
            return isIntoChildCircle;
        }

        /// <summary>
        /// Choose a type of insert XML node
        /// isHasNewNode ȷ��������Ľڵ�
        /// </summary>
        private void ChooseNodeType(XmlDocument packXmldoc, XmlNode newCurrNode,
                                 XmlNode packCurrNode, XmlNode refChildNode)
        {
            switch (newCurrNode.NodeType)
            {
                case XmlNodeType.Comment:
                    if (refParentNode.Name == packCurrNode.Name)
                    {
                        InsertAndRemoveXMLNode(packXmldoc, newCurrNode, refChildNode);
                        isHasNewNode = false;
                    }
                    else
                    {
                        InsertAndRemoveXMLNode(packXmldoc, newCurrNode, refParentNode);
                        isHasNewNode = false;
                    }
                    break;
                case XmlNodeType.Element:
                    if (indexNewXmlNode > refChildNode.ChildNodes.Count - 1)
                    {
                        InsertAndRemoveXMLNode(packXmldoc, newCurrNode, refChildNode.ChildNodes.Item(refChildNode.ChildNodes.Count - 1));
                        isHasNewNode = false;
                    }
                    else
                    {
                        InsertAndRemoveXMLNode(packXmldoc, newCurrNode, refChildNode);
                        isHasNewNode = false;
                    }

                    break;
                case XmlNodeType.Text:
                    if (!string.IsNullOrEmpty(newCurrNode.InnerText))
                        InsertAndRemoveXMLNode(packXmldoc, newCurrNode, refParentNode);

                    isHasNewNode = false;

                    break;
                case XmlNodeType.Document:
                    break;
                case XmlNodeType.None:
                    break;
            }
        }

        /// <summary>
        /// Replace XML child node
        /// </summary>
        private void ReplaceXMLNode(XmlDocument packXmldoc, XmlNode newXMLNode)
        {
            try
            {
                XmlNode root = packXmldoc.DocumentElement;

                switch (newXMLNode.NodeType)
                {
                    case XmlNodeType.Comment:
                        //Create a comment.
                        string comData = newXMLNode.InnerText;
                        XmlComment newComment = packXmldoc.CreateComment(comData);
                        //newComment = packXmldoc.CreateComment(comData);

                        XmlDocumentFragment newFrag = packXmldoc.CreateDocumentFragment();
                        newFrag.InnerXml = packXmldoc.CreateComment(comData).OuterXml;

                        if (currPackNode.HasChildNodes)
                        {
                            //currPackNode.AppendChild(newFrag);
                        }
                        else
                        {
                            //currPackNode.ParentNode.AppendChild(newFrag);
                        }
                        break;
                    case XmlNodeType.Element:
                        //Create a new node.
                        string newNodeName = newXMLNode.Name;
                        XmlElement elem = packXmldoc.CreateElement(newNodeName);

                        if (!String.IsNullOrEmpty(newXMLNode.InnerXml))
                        {
                            elem.InnerXml = newXMLNode.InnerXml;
                            //refParentNode.AppendChild(elem);
                            refParentNode.ReplaceChild(elem, refChildNode);
                        }
                        else if (!String.IsNullOrEmpty(newXMLNode.OuterXml))
                        {
                            //��������<add name="" />��ʽ
                            elem.InnerXml = newXMLNode.OuterXml;
                            XmlDocumentFragment formatFrag = packXmldoc.CreateDocumentFragment();
                            formatFrag.InnerXml = newXMLNode.OuterXml;
                            //refParentNode.AppendChild(formatFrag);
                            refParentNode.ReplaceChild(formatFrag, currPackNode);
                            //remove current node form newXMLFile
                            //currParentNode.RemoveChild(newXMLNode);
                        }
                        break;
                }

                ResetActive();
            }
            catch (Exception e)
            {
                MessageBox.Show(e.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }


        /// <summary>
        /// Insert XML child node
        /// </summary>
        private void InsertAndRemoveXMLNode(XmlDocument packXmldoc, XmlNode newXMLNode, XmlNode refChildNode)
        {
            try
            {
                string newNodeName = null;
                XmlElement elem = null;
                XmlDocumentFragment formatFrag = null;
                XmlNode root = packXmldoc.DocumentElement;

                switch (newXMLNode.NodeType)
                {
                    case XmlNodeType.Comment:
                        //Create a comment.
                        string comData = newXMLNode.InnerText;
                        XmlComment newComment = packXmldoc.CreateComment(comData);
                        //newComment = packXmldoc.CreateComment(comData);

                        XmlDocumentFragment newFrag = packXmldoc.CreateDocumentFragment();
                        newFrag.InnerXml = packXmldoc.CreateComment(comData).OuterXml;

                        if (refChildNode.HasChildNodes)
                        {
                            if (isMergerXMLNode)
                                refChildNode.AppendChild(newFrag);
                            if (isRemoveXMLNode && currParentNode != null)
                                refParentNode.RemoveChild(currPackChildNode);
                        }
                        else
                        {
                            if (isMergerXMLNode && refParentNode.ParentNode != null)
                                refChildNode.ParentNode.AppendChild(newFrag);
                            if (isRemoveXMLNode && currParentNode != null)
                                refParentNode.ParentNode.RemoveChild(currPackChildNode);
                        }
                        break;
                    case XmlNodeType.Element:
                        //Create a new node.
                        newNodeName = newXMLNode.Name;
                        elem = packXmldoc.CreateElement(newNodeName);

                        if (!String.IsNullOrEmpty(newXMLNode.InnerXml) &&
                            !newXMLNode.HasChildNodes)
                        {
                            elem.InnerXml = newXMLNode.InnerXml;

                            if (isMergerXMLNode)
                                refParentNode.AppendChild(elem);
                            if (isRemoveXMLNode && currParentNode != null)
                                refParentNode.RemoveChild(currPackChildNode);
                        }
                        else if (!String.IsNullOrEmpty(newXMLNode.OuterXml))
                        {
                            //��������<add name="" />��ʽ
                            elem.InnerXml = newXMLNode.OuterXml;
                            formatFrag = packXmldoc.CreateDocumentFragment();
                            formatFrag.InnerXml = newXMLNode.OuterXml;

                            if (isMergerXMLNode)
                                refParentNode.AppendChild(formatFrag);
                            if (isRemoveXMLNode && currParentNode != null && currPackChildNode.ParentNode != null &&
                                currParentNode.Name == currPackChildNode.ParentNode.Name)
                                currPackChildNode.ParentNode.RemoveChild(currPackChildNode);
                        }
                        break;
                    case XmlNodeType.Text:
                        newNodeName = refParentNode.Name;
                        elem = packXmldoc.CreateElement(newNodeName);
                        elem.InnerText = newXMLNode.InnerText;

                        if (isMergerXMLNode && refParentNode.ParentNode != null)
                            refParentNode.ParentNode.AppendChild(elem);
                        if (isRemoveXMLNode && currParentNode != null)
                            refParentNode.ParentNode.RemoveChild(currPackChildNode);

                        break;
                }

                ResetActive();
            }
            catch (Exception e)
            {
                MessageBox.Show(e.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        /// <summary>
        /// ���貼��ֵ
        /// </summary>
        private void ResetActive()
        {
            isNextNode = false;
            isInser = false;
            isReplace = false;
            isNewAttributes = false;
            isHasNewNode = false;
        }

        /// <summary>
        /// Xml�ĵ���ȡ�Ƿ�ɹ�
        /// </summary>
        public bool IsLoadXmlFile
        {
            get { return isSuccessLoadXmlFile; }
            set { isSuccessLoadXmlFile = value; }
        }
    }
}
