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
    /// 合并XML文档
    /// 将PackXML文件与新安装的XML文件进行比较子节点内容，若值不同做插入、替换。
    /// 
    ///     用户文件节点         新文件节点           比较值         是否插入       是否替换
    /// -------------------------------------------------------------------------------------
    ///    没有修改该节点       没有修改该节点         相同             否             否
    ///    改过该节点           没有修改该节点         不同             过滤           过滤
    ///    没有修改该节点       修改过该节点           不同             否             是
    ///    没有该节点           新添节点               无               是             否
    ///    新添节点             没有该节点             无               否             否
    ///    修改过该节点         修改过该节点           不同             不确定         不确定
    /// -------------------------------------------------------------------------------------
    /// 1.替换节点的判断：
    ///   程序遇到节点属性名为assembly=,(name= type=),(key=、value=)，比较其属性值若不同则作出替换处理。
    /// 2.节点新增的判断：
    ///   除了节点属性名为assembly以外，比较结果没有相同的节点，作为新增节点作插入处理。
    ///   插入新添的XML节点必须满足条件：
    ///   a.父结点相同、b.两个节点所处的层次(树形节点)位置一致。
    /// 3.过滤节点："authorizatio"、"deny users="*""、"allow users="?" "
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
        /// 合并XML文件
        /// newFilePath和packFilePath为XML文件路径
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
                //得到packXmldocXML文档第一个Element节点
                packCurrNode = GetRootElement(packXmldoc);
                //得到newXmldoc文档第一个Element节点
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

                //将合并后的CustomCode文件存盘
                packXmldoc.Save(newFilePath);

                //删除文件
                if (File.Exists(packFilePath))
                    File.Delete(packFilePath);
            }
            else
            {
                //读取XML文件失败，保留用户备份XMl文件
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

            // 从流中加载 XmlTextReader
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
        /// 移走XML文件节点
        /// newFilePath和packFilePath为XML文件路径
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
                //得到newXmldoc文档第一个Element节点
                newCurrNode = GetRootElement(newXmldoc);
                //得到packXmldocXML文档第一个Element节点
                packCurrNode = GetRootElement(packXmldoc);

                //删除节点文件含有要删除的子节点，进入搜索比较
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

                    //将移走节点后的CustomCode文件存盘
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
                //确定newXmldoc文档第一个Element节点
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

            //从安装目录里读取备份XML文件 
            try
            {
                //被比较文件
                newXmldoc.Load(packFilePath);
                //基准文件
                packXmldoc.Load(newFilePath);
            }
            catch
            {
                isLoad = false;
            }
            return isLoad;
        }

        /// <summary>
        /// 以新的文件newXmldoc为基准进行递归搜索
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
                isNewAttributes = true;//假设有新的属性值
                isHasNewNode = true; //所取的每个childNode都假设是新添的子节点
                refParentNode = packCurrNode;
                currParentNode = newCurrNode;
                #endregion

                //调用 CompareXMLNode方法遍历Xml文件所有节点进行比较。
                CompareXMLNode(childNode, packCurrNode);

                //isNextNode为false 表示进行过新节点插入操作,不再进入子节点循环
                if (isNextNode)
                {
                    newChildNodePostion++;//节点位置
                    SearchXMLNode(childNode, packCurrNode);     //递归循环               
                }
                indexNewXmlNode++;
            }
            indexNewXmlNode = 0;
            newChildNodePostion = 0;
        }

        /// <summary>
        /// 将两个XML文件的节点进行比较。
        /// </summary>
        private void CompareXMLNode(XmlNode newCurrNode, XmlNode packCurrNode)
        {
            foreach (XmlNode childNode in packCurrNode)
            {
                //若PackXML与NewXML节点名和属性值相同则中断遍历循环，进入下级子节点循环。
                if (newCurrNode.Name == childNode.Name || isRemoveXMLNode) //同一级树形节点循环比较
                {
                    //比较Xml节点和属性值是否相同,判断是否进入子循环
                    bool isIntoChildNodeCircle = CompareXmlNodeType(childNode, newCurrNode, packCurrNode);

                    currPackChildNode = childNode;

                    //isIntoChildNodeCircle返回true进入newCurrNode节点子节点循环
                    if (isIntoChildNodeCircle)
                        SearchXMLNode(newCurrNode, childNode); //递归循环

                    if (!isNewAttributes) //两节点属性值相同中断循环
                    {
                        isNextNode = false;
                        break;
                    }
                }

                //PackFile进入子节点递归循环，确保比较是在同一级别节点内进行和新节点插入操作
                if (newCurrNode.ParentNode != null && packCurrNode.ParentNode != null)
                {
                    if (newCurrNode.ParentNode.Name == packCurrNode.ParentNode.Name || isRemoveXMLNode)
                    {
                        packChildNodePostion++;
                        CompareXMLNode(newCurrNode, childNode); //同一个父节点里遍历子节点循环
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
                    //替换XML节点
                    ReplaceXMLNode(packXmldoc, newCurrNode);
                }
                else if (isInser && !isFilter && !isReplace)
                {
                    //插入新添的XML节点
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
        /// 过滤节点 "authorization", "allow", "deny"，对这几个节点值不做插入。
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
        /// 是否是要插入的节点
        /// <return>return bool</return>
        /// </summary>
        private bool isInsertNode(XmlNode newCurrNode)
        {
            //插入新添的XML节点必须满足条件：
            //1。父结点相同、2。每个节点都搜索一遍、3。有新的节点、4。两个节点所处的位置一致。
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
        /// 检查新节点是更新的节点,以下是要比较的实例
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
                //范例<add assembly="System.DirectoryServices, Version=2.0.0.0, Culture=neutral, PublicKeyToken=B03F5F7F11D50A3A" />           
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
                    //范例<pages theme="BlueTheme">
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
        /// 检查是否是新的属性值
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
                //检查child.InnerText这一类型节点.
                //例如：<value>http://localhost/Newtera/WebService/AdminWebService.asmx</value>                
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

            //节点属性值相同则中断CompareXMLNode遍历循环。
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
                        //当前节点没有属性值
                        if (newCurrNode.Attributes.Count == 0 && childNode.Attributes.Count == 0)
                        {
                            isHasNewNode = false;
                            isIntoChildCircle = true;
                        }
                        else
                        //当前节点有属性值，进行属性值比较。
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
                                        //isNewAttributes为false表明进行过一次插入计算,不再进入子循环
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
                    //比较最里层子节点<add name="" />格式
                    else if (newCurrNode.Attributes != null && childNode.Attributes != null &&
                              !newCurrNode.HasChildNodes)
                    {
                        if (newCurrNode.Attributes.Count > 0)
                        {
                            //当前节点有属性值，进行属性值比较。
                            for (int i = 0; i < newCurrNode.Attributes.Count; i++)
                            {
                                if (newCurrNode.Attributes.Item(i) != null && childNode.Attributes.Item(i) != null)
                                {
                                    if (newCurrNode.Attributes.Item(i).Name == childNode.Attributes.Item(i).Name &&
                                      newCurrNode.Attributes.Item(i).Value == childNode.Attributes.Item(i).Value)
                                    {
                                        //循环比较到最后一个属性值
                                        if (i == newCurrNode.Attributes.Count - 1)
                                        {
                                            isHasNewNode = false; //No New Node
                                            isIntoChildCircle = false;
                                        }
                                    }
                                    else
                                    {
                                        //判断该节点是否是替换类型操作,返回true替换节点.                                     
                                        if (i == 0)
                                        {
                                            //遍历指定替换节点属性名
                                            //范例<pages theme="BlueTheme">   
                                            //节点类型<add assembly="System.DirectoryServices, Version=2.0.0.0, Culture=neutral, PublicKeyToken=B03F5F7F11D50A3A" />
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
                                            //节点类型<add key="EmailPassword" value="123456" />
                           
                                            //遍历指定替换节点属性名
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
                            //当前节点没有属性值(例如：<appSettings/>)，进行OuterXml值比较。
                            if (newCurrNode.OuterXml == childNode.OuterXml)
                            {
                                isHasNewNode = false; //No New Node
                                isIntoChildCircle = false;
                            }
                        }
                    }

                    break;
                case XmlNodeType.Text:
                    //比较child.InnerText这一类型节点.
                    //例如:<value>http://localhost/Newtera/WebService/AdminWebService.asmx</value>
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
        /// isHasNewNode 确认是新添的节点
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
                            //用于类似<add name="" />格式
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
                            //用于类似<add name="" />格式
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
        /// 重设布尔值
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
        /// Xml文档读取是否成功
        /// </summary>
        public bool IsLoadXmlFile
        {
            get { return isSuccessLoadXmlFile; }
            set { isSuccessLoadXmlFile = value; }
        }
    }
}
