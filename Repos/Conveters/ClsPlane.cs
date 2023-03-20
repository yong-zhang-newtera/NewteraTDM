using System;
using System.Collections.Generic;
using System.Text;
using Newtera.ParserGen.Converter;
using System.Data;
using System.IO;
using Newtera.Common.Core;
using System.Text.RegularExpressions;
using System.Collections.Specialized;

namespace Newtera.Conveters
{
    public class  NewteraClsPlane: DataSourceConverterBase
    {
         private DataSet dataSet;
        public List<int> intBiao = new List<int>();
        public List<int> intChannelOne = new List<int>();
        public List<int> intChannelTwo = new List<int>();
        public List<int> intChannelThree = new List<int>();
        public List<int> intChannelFour = new List<int>();
        public Regex r0 = new Regex(@"( )+");//数据集          
        public Regex r1 = new Regex(@"试验");
       

        #region 构造函数
        /// <summary>
        /// 初始化类
        /// </summary>
        public NewteraClsPlane(): base()
        {
            //dataSet = new DataSet();
           
        }
        #endregion

        #region SupportPaging方法
        /// <summary>
        /// 设置是否分块读取数据
        /// </summary>
        public override bool SupportPaging
        {
            get
            {
                return false;
            }
        }
        #endregion

        #region Close方法
        /// <summary>
        /// 关闭的连接
        /// </summary>
        public override void Close()
        {
            this.dataSet = null;
        }
        #endregion

        #region 不分块读取数据的方法
        /// <summary>
        /// 数据转换器方法
        /// </summary>
        /// <param name="dataSourceName">转换数据的路径</param>
        /// <returns>数据集</returns>
        public override System.Data.DataSet Convert(string dataSourceName)
        {
            DataSet dataSet = new DataSet();

            string nextline;            
            StreamReader sr = new StreamReader(dataSourceName, Encoding.Default);
            StringCollection result = new StringCollection();
            while ((nextline = sr.ReadLine()) != null)
            {
                if (nextline.Trim() != "")
                {
                    result.Add(nextline.Trim());
                }
            }
            int linehight = result.Count;
            //string[][] stringarray = new string[linehight][];
            string[] linesarray = new string[result.Count];
            result.CopyTo(linesarray, 0);
            //for (int i = 0; i < result.Count; i++)
            //{
            //    Regex r0 = new Regex(@"( )+");
            //    char fengefu = ' ';
            //    string str = r0.Replace(linesarray[i], " ");
            //    stringarray[i] = str.Split(fengefu);
            //}

            ////记录表头的起始位置和结束位置
            //List<int> intBiao = new List<int>();

            ////记录通道的起始位置和结束位置
            //List<int> intChannelOne = new List<int>();
            //List<int> intChannelTwo = new List<int>();
            //List<int> intChannelThree = new List<int>();
            //List<int> intChannelfour = new List<int>();

            int tempInt = 0; //定义位置
            //循环读取数据并把表头的位置和通道位置记录下来
            for (int i = 0; i < linesarray.Length; i++)
            {
                if (linesarray[i].ToString().Contains("试验")) 
                {
                    intBiao.Add(tempInt); 
                }
                else if (linesarray[i].ToString()=="通用参数：")
                {
                    intChannelOne.Add(tempInt); 
                }
                else if (linesarray[i].ToString().Contains("原始数据") && linesarray[i].ToString().Contains("："))
                {
                    intChannelTwo.Add(tempInt);
                }
                else if (linesarray[i].ToString()=="马赫数：")
                {
                    intChannelThree.Add(tempInt);
                }
                else if (linesarray[i].ToString()=="扫描数据：")
                {
                    intChannelFour.Add(tempInt);
                }
                tempInt++;
            }
            intBiao.Add(tempInt); 

            //创建结构化数据组表
            DataTable headerDataTable = dataSet.Tables.Add(NewteraNameSpace.HEADER_TABLE_NAME);  //NewteraNameSpace.HEADER_TABLE_NAME
            //创建结构化数据记录表
            DataTable channelDataTable = dataSet.Tables.Add(NewteraNameSpace.CHANNEL_TABLE_NAME); //NewteraNameSpace.CHANNEL_TABLE_NAME

            //添加结构化数据组表主键
            headerDataTable.Columns.Add(NewteraNameSpace.GROUP_ID);
            //添加表头参数数组  NewteraNameSpace.HEADER_NAME_ARRAY
            headerDataTable.Columns.Add(NewteraNameSpace.HEADER_NAME_ARRAY);
            //添加表头数据数组  NewteraNameSpace.HEADER_VALUE_ARRAY
            headerDataTable.Columns.Add(NewteraNameSpace.HEADER_VALUE_ARRAY);
            //添加通道参数数组   NewteraNameSpace.CHANNEL_NAME_ARRAY
            headerDataTable.Columns.Add(NewteraNameSpace.CHANNEL_NAME_ARRAY);

            //添加结构化数据记录表主键  NewteraNameSpace.RECORD_ID
            channelDataTable.Columns.Add(NewteraNameSpace.CHANNEL_ID);
            //添加结构化数据记录表外键  NewteraNameSpace.RECORD_ID
            channelDataTable.Columns.Add(NewteraNameSpace.GROUP_ID);
            //添加通道数据数组   NewteraNameSpace.CHANNEL_VALUE_ARRAY
            channelDataTable.Columns.Add(NewteraNameSpace.CHANNEL_VALUE_ARRAY);

            // 创建两个DataTable之间的关系
            dataSet.Relations.Add(NewteraNameSpace.GROUP_CHANNEL_RELATION, headerDataTable.Columns[NewteraNameSpace.GROUP_ID],
            channelDataTable.Columns[NewteraNameSpace.GROUP_ID]);
            for (int channelNum = 0; channelNum < 4;channelNum++)
            {
                string headerId;  //结构化数据记录表主键编号
                try
                {
                    DataRow headerRow = ReadHeaderData(linesarray, headerDataTable); //读文件表头数据

                    //开始读取通道参数名并以数组的形式保存在表头记录中
                    ReadChannelNames(linesarray, channelNum, headerRow);

                    //返回结构化数据记录表的主键编号
                    headerId = headerRow[NewteraNameSpace.GROUP_ID].ToString();

                    //读通道数据记录直到文件结束
                    ReadChannelData(linesarray, channelNum, channelDataTable, headerId);
                }
                catch 
                {
                    break;
                }
            }
        
        
            return dataSet;
        }
        #endregion

        /// <summary>
        /// 从文件中读取表头数据,并作为DataTable中的唯一记录
        /// </summary>
        /// <param name="ds">数据集</param>
        /// <param name="startCell">起始列</param>
        /// <param name="endCell">结束列</param>
        /// <param name="headerDataTable">结构化数据组表</param>
        /// <returns></returns>
        public DataRow ReadHeaderData(string[] sArray,DataTable headerDataTable)
        {
            DataRow dataRow = headerDataTable.NewRow();

            //设置主键
            string rowId = null;
            //生成主键
            rowId = CreateRowId(); //随机生成主键
            dataRow[NewteraNameSpace.GROUP_ID] = rowId;

            string strHeader = ""; // 读取表头参数
            string strHeaderData = ""; // 读取表头数据           
            
            //for (int i = 0; i < sArray.Length; i++)
            //{
            //    if (sArray[i].Contains("试验"))
            //    {
            //        strHeader = "";
            //        strHeaderData = "";
            //        string[] temp = r1.Split(sArray[i]);
            //        strHeader += "试验件名称" + ";";
            //        strHeaderData += temp[0].Trim() + ";";
            //        break;
            //    }               
 
            //}

            string[] temp = r1.Split(sArray[intBiao[0]]);
            strHeader += "试验件名称" + ";";
            strHeaderData += temp[0].Trim() + ";";

                //给表头参数行赋值
            if (strHeader.Length > 0)
                dataRow[NewteraNameSpace.HEADER_NAME_ARRAY] = strHeader.Remove(strHeader.Length - 1);

            //给表头数据行赋值
            if (strHeaderData.Length > 0)
                dataRow[NewteraNameSpace.HEADER_VALUE_ARRAY] = strHeaderData.Remove(strHeaderData.Length - 1);

            //添加表头记录行
            headerDataTable.Rows.Add(dataRow);

            return dataRow;
        }

        /// <summary>
        /// 从文件中读取通道参数名
        /// </summary>
        /// <param name="ds">数据集</param>
        /// <param name="startCell">起始列</param>
        /// <param name="endCell">结束列</param>
        /// <param name="headerRow"></param>
        public void ReadChannelNames(string[] sArray, int channelNum, DataRow headerRow)
        {
            char fengefu = ' ';
            string channelNames = "试验时间" + ";";      // 读取通道参数 

            string dataLine = r0.Replace(sArray[intBiao[0] + 3], " ");
            string[] dataLineCol = dataLine.Split(fengefu);
            for (int i = 1; i < dataLineCol.Length - 1; i++)
            {
                string headBlock = dataLineCol[i].Trim();
                if (headBlock.Contains("："))
                {
                    string data = headBlock.Trim();
                    int a = data.IndexOf("：");
                    int b = data.Length;
                    channelNames += data.Substring(0, a) + ";";
                }

            }

            if (channelNum == 0)
            {
                
                for (int i = intChannelOne[0]+1; i < intChannelTwo[0]; i=i+1)
                {
                    string strLine = r0.Replace(sArray[i], " ");
                    //if (strLine != "特有参数：")
                    //{                        
                    //    string[] strLineCol = strLine.Split(fengefu);
                    //    for (int j = 0; j < strLineCol.Length; j++)
                    //    {
                    //        channelNames += strLineCol[j] + ";";
                    //    }
                    //}
                    //else
                    //{
                    //    i--;
                    //}

                    string[] strLineCol = strLine.Split(fengefu);

                    if (!strLineCol[0].Contains("："))
                    {
                        if (!IsNumber(strLineCol[0]))
                        {
                            for (int j = 0; j < strLineCol.Length; j++)
                            {
                                channelNames += strLineCol[j] + ";";
                            }
                        }                        
                    }                                  
                }
 
            }
            else if (channelNum == 1)
            {
                
                for (int i = intChannelTwo[0] + 1; i < intChannelThree[0]; i = i + 1)
                {
                    string strLine = r0.Replace(sArray[i], " ");
                    string[] strLineCol = strLine.Split(fengefu);

                    string strNextLine = "";                   

                    for (int j = 1; j < strLineCol.Length; j++)
                    {
                        string str = strLineCol[0] + "-" + j;
                        channelNames += str + ";";
                    }

                    int n = i;
                    for (int k = i + 1; k < intChannelThree[0]; k++)
                    {
                        
                        strNextLine = r0.Replace(sArray[k], " ");
                        string[] strNextLineCol = strNextLine.Split(fengefu);

                        if (IsNumber(strNextLineCol[0]))
                        {
                            for (int j = 0; j < strNextLineCol.Length; j++)
                            {
                                string str = strLineCol[0] + "-" + (j + (k - n) * (strLineCol.Length-1)+1);
                                channelNames += str + ";";
                            }
                            i++;
                        }

                        else
                        {
                            break;
                        } 
                    }

                } 
            }
            else if (channelNum == 2)
            {
                
                for (int i = intChannelThree[0] + 1; i < intChannelFour[0]; i = i + 1)
                {
                    string strLine = r0.Replace(sArray[i], " ");
                    string[] strLineCol = strLine.Split(fengefu);

                    string strNextLine = "";

                    for (int j = 1; j < strLineCol.Length; j++)
                    {
                        string str = strLineCol[0] + "-" + j;
                        channelNames += str + ";";
                    }

                    int n = i;
                    for (int k = i + 1; k < intChannelFour[0]; k++)
                    {

                        strNextLine = r0.Replace(sArray[k], " ");
                        string[] strNextLineCol = strNextLine.Split(fengefu);

                        if (IsNumber(strNextLineCol[0]))
                        {
                            for (int j = 0; j < strNextLineCol.Length; j++)
                            {
                                string str = strLineCol[0] + "-" + (j + (k - n) * (strLineCol.Length - 1) + 1);
                                channelNames += str + ";";
                            }
                            i++;
                        }

                        else
                        {
                            break;
                        }
                    }

                }  
            }
            else if (channelNum == 3)
            {
                
                for (int i = intChannelFour[0] + 1; i < intBiao[1]; i = i + 1)
                {
                    string strLine = r0.Replace(sArray[i], " ");                    
                    string[] strLineCol = strLine.Split(fengefu);
                    string strNextLine = "";
                    if (!IsNumber(strLine))
                    {
                        string strName = strLine.Split('(')[0];
                        int n = i;

                        int l = r0.Replace(sArray[i + 1], " ").Split(fengefu).Length;
                        for (int k = i + 1; k < intBiao[1]; k++)
                        {
                            strNextLine = r0.Replace(sArray[k], " ");
                            string[] strNextLineCol = strNextLine.Split(fengefu);
                            if (IsNumber(strNextLineCol[0]))
                            {
                                for (int j = 0; j < strNextLineCol.Length; j++)
                                {

                                    string str = strName + "-" + (j + (k - n-1) * l + 1);
                                    channelNames += str + ";";
                                }
                                i++;
                            }

                            else
                            {
                                break;
                            }
                        }
 
                    }
                }
 
            }
            //给通道参数行赋值
            headerRow[NewteraNameSpace.CHANNEL_NAME_ARRAY] = channelNames.Remove(channelNames.Length - 1);
        }

        /// <summary>
        /// 读取通道数据记录
        /// </summary>
        /// <param name="ds">数据集</param>
        /// <param name="startCell">起始列</param>
        /// <param name="endCell">结束列</param>
        /// <param name="channelDataTable">结构化数据数据记录表</param>
        /// <param name="groupId">外键编号</param>
        public void ReadChannelData(string[] sArray, int channelNum, DataTable channelDataTable, string groupId)
        {
            char fengefu = ' ';
            DataRow dataRow = null; //定义行数据

            ////读取通道数据总行数记录
            //int rowsData = ds.Tables[0].Rows.Count;

            string line = "";//通道数据
            for (int c = 0; c < intChannelOne.Count; c++)
            {
                //从数据集里获取通道数据
                line = "";
                string time = sArray[intBiao[c] + 2].ToString();
                line += time + ";";
                string dataLine = r0.Replace(sArray[intBiao[c] + 3], " ");
                string[] dataLineCol = dataLine.Split(fengefu);
                for (int i = 1; i < dataLineCol.Length - 1; i++)
                {
                    string headBlock = dataLineCol[i].Trim();
                    if (headBlock.Contains("："))
                    {
                        string data = headBlock.Trim();
                        int a = data.IndexOf("：");
                        int b = data.Length;
                        line += data.Substring(a + 1, b - a - 1) + ";";
                    }
 
                }


                if (channelNum == 0)
                {

                    for (int i = intChannelOne[c] + 2; i < intChannelTwo[c]; i++)
                    {
                        string strLine = r0.Replace(sArray[i], " ");
                        string[] strLineCol = strLine.Split(fengefu);

                        if (!strLineCol[0].Contains("："))
                        {
                            if (IsNumber(strLineCol[0]))
                            {
                                for (int j = 0; j < strLineCol.Length; j++)
                                {
                                    line += strLineCol[j] + ";";
                                }
                            }
                        }

                    }

                }
                else if (channelNum == 1)
                {
                    for (int i = intChannelTwo[c] + 1; i < intChannelThree[c]; i = i + 1)
                    {
                        string strLine = r0.Replace(sArray[i], " ");
                        string[] strLineCol = strLine.Split(fengefu);

                        string strNextLine = "";

                        for (int j = 1; j < strLineCol.Length; j++)
                        {
                            string str = strLineCol[j];
                            line += str + ";";
                        }

                        int n = i;
                        for (int k = i + 1; k < intChannelThree[c]; k++)
                        {

                            strNextLine = r0.Replace(sArray[k], " ");
                            string[] strNextLineCol = strNextLine.Split(fengefu);

                            if (IsNumber(strNextLineCol[0]))
                            {
                                for (int j = 0; j < strNextLineCol.Length; j++)
                                {
                                    string str = strNextLineCol[j];
                                    line += str + ";";
                                }
                                i++;
                            }

                            else
                            {
                                break;
                            }
                        }

                    }
                }
                else if (channelNum == 2)
                {
                    for (int i = intChannelThree[c] + 1; i < intChannelFour[c]; i = i + 1)
                    {
                        string strLine = r0.Replace(sArray[i], " ");
                        string[] strLineCol = strLine.Split(fengefu);

                        string strNextLine = "";

                        for (int j = 1; j < strLineCol.Length; j++)
                        {
                            string str = strLineCol[j];
                            line += str + ";";
                        }

                        int n = i;
                        for (int k = i + 1; k < intChannelFour[c]; k++)
                        {

                            strNextLine = r0.Replace(sArray[k], " ");
                            string[] strNextLineCol = strNextLine.Split(fengefu);

                            if (IsNumber(strNextLineCol[0]))
                            {
                                for (int j = 0; j < strNextLineCol.Length; j++)
                                {
                                    string str = strNextLineCol[j];
                                    line += str + ";";
                                }
                                i++;
                            }

                            else
                            {
                                break;
                            }
                        }

                    }
                }
                else if (channelNum == 3)
                {
                    for (int i = intChannelFour[c] + 1; i < intBiao[c + 1]; i = i + 1)
                    {
                        string strLine = r0.Replace(sArray[i], " ");
                        string[] strLineCol = strLine.Split(fengefu);
                        string strNextLine = "";
                        if (!IsNumber(strLine))
                        {
                            //string strName = strLine.Split('(')[0];
                            //int n = i;

                            //int l = r0.Replace(sArray[i + 1], " ").Split(fengefu).Length;
                            for (int k = i + 1; k < intBiao[c + 1]; k++)
                            {
                                strNextLine = r0.Replace(sArray[k], " ");
                                string[] strNextLineCol = strNextLine.Split(fengefu);
                                if (IsNumber(strNextLineCol[0]))
                                {
                                    for (int j = 0; j < strNextLineCol.Length; j++)
                                    {

                                        string str = strNextLineCol[j];
                                        line += str + ";";
                                    }
                                    i++;
                                }

                                else
                                {
                                    break;
                                }
                            }

                        }
                    }

                }

                if (line.Replace(";", "").Trim().Length > 0)  //判断行是否为空，如果为空就代表数据读取结束
                {
                    dataRow = channelDataTable.NewRow();
                    // 设置外键
                    dataRow[NewteraNameSpace.GROUP_ID] = groupId;
                    dataRow[NewteraNameSpace.CHANNEL_ID] = CreateRowId();
                    // 添加行到表里
                    channelDataTable.Rows.Add(dataRow);
                    dataRow[NewteraNameSpace.CHANNEL_VALUE_ARRAY] = line.Remove(line.Length - 1);
                }
                else
                {
                    break;
                }
            }
        }


        /// <summary>
        /// 生成一个行记录ＩＤ值
        /// </summary>
        private string CreateRowId()
        {
            string id = "R_" + Guid.NewGuid();
            return id;
        }

        public bool IsNumber(string strData)
        {
            try
            {
                double f = System.Convert.ToDouble(strData);
                return true;
            }
            catch
            {
                return false;
            }                
        }
    }
}

    

