using System;
using System.Data;
using System.Collections.Generic;
using System.Text;
using System.Xml;
using System.IO;
using System.Text.RegularExpressions;
using System.Collections.Specialized;

using Newtera.Common.Core;
using Newtera.ParserGen.Converter;

namespace Newtera.Conveters
{
    public class HeaderChannelDataConverter : DataSourceConverterBase
    {
        private const string NameValueExp = @"\s*[^:]+:\s*[^\s]*";

        private Regex _nameValueExp;
        private bool _endOfFile;
 
        public HeaderChannelDataConverter() : base()
        {
            // Compile regular expression to find "name = value" or "name : value" pairs
            _nameValueExp = new Regex(HeaderChannelDataConverter.NameValueExp);

            _endOfFile = false;
        }

        public override DataSet Convert(string dataSourceName)
        {
            DataSet dataSet = new DataSet();
            string headerId;

            DataTable headerDataTable = dataSet.Tables.Add(NewteraNameSpace.HEADER_TABLE_NAME);
            headerDataTable.Columns.Add(NewteraNameSpace.GROUP_ID); // 表头的ＩＤ
            headerDataTable.Columns.Add(NewteraNameSpace.HEADER_NAME_ARRAY); // 表头参数数组
            headerDataTable.Columns.Add(NewteraNameSpace.HEADER_VALUE_ARRAY); // 表头参数值数组
            headerDataTable.Columns.Add(NewteraNameSpace.CHANNEL_NAME_ARRAY); // 通道参数名数组

            DataTable channelDataTable = dataSet.Tables.Add(NewteraNameSpace.CHANNEL_TABLE_NAME);
            channelDataTable.Columns.Add(NewteraNameSpace.RECORD_ID); // 主键键
            channelDataTable.Columns.Add(NewteraNameSpace.GROUP_ID); // 外键
            channelDataTable.Columns.Add(NewteraNameSpace.CHANNEL_VALUE_ARRAY); // 通道参数值数组

            // 创建两个DataTable之间的关系
            dataSet.Relations.Add(NewteraNameSpace.GROUP_CHANNEL_RELATION, headerDataTable.Columns[NewteraNameSpace.GROUP_ID],
                channelDataTable.Columns[NewteraNameSpace.GROUP_ID]);

            // open the file as stream reader
            using (StreamReader reader = new StreamReader(dataSourceName, Encoding.Default))
            {
                while (!_endOfFile)
                {
                    // 从文件中读取文件表头数据并形成组记录
                    DataRow headerRow = ReadHeaderData(reader, headerDataTable); // 读文件表头数据

                    // 逃过中间数据直到通道参数行
                    string line = reader.ReadLine();
                    while (line != null && line.Trim().Length > 0)
                    {
                        line = reader.ReadLine();
                    }

                    line = reader.ReadLine(); // 再跳过一行

                    // 开始读取通道参数名并以数组的形式保存在表头记录中
                    ReadChannelNames(reader, headerRow);

                    // 读取通道数据记录直到文件结束
                    headerId = headerRow[NewteraNameSpace.GROUP_ID].ToString();
                    ReadChannelData(reader, channelDataTable, headerId); // 读通道数据记录
                }
            }
            
            return dataSet;
        }

        /// <summary>
        /// 从文件中读取表头数据,并作为DataTable中的唯一记录
        /// </summary>
        private DataRow ReadHeaderData(StreamReader reader, DataTable headerDataTable)
        {
            string rowData = GetHeaderRowData(reader);

            NameValueCollection columnValues = GetColumnValues(rowData);

            // 添加表头记录
            return CreateHeaderRow(headerDataTable, columnValues);
        }

        /// <summary>
        /// 在DataTable添加一行,并返回生成的ID值
        /// </summary>
        private DataRow CreateHeaderRow(DataTable dataTable, NameValueCollection columnValues)
        {
            DataRow dataRow = dataTable.NewRow();
            StringBuilder namesBuilder = new StringBuilder();
            StringBuilder valuesBuilder = new StringBuilder();

            string rowId = null;

            // 设置主键
            rowId = CreateRowId();
            dataRow[NewteraNameSpace.GROUP_ID] = rowId;

            // 创建表头参数名和参数值数组
            int index = 0;
            foreach (string key in columnValues.Keys)
            {
                if (index > 0)
                {
                    namesBuilder.Append(";");
                    valuesBuilder.Append(";");
                }

                namesBuilder.Append(key);
                valuesBuilder.Append(columnValues[key]);

                index++;
            }

            dataRow[NewteraNameSpace.HEADER_NAME_ARRAY] = namesBuilder.ToString();
            dataRow[NewteraNameSpace.HEADER_VALUE_ARRAY] = valuesBuilder.ToString();

            dataTable.Rows.Add(dataRow);

            return dataRow;
        }

        /// <summary>
        /// 从文件中读取通道参数名
        /// </summary>
        private void ReadChannelNames(StreamReader reader, DataRow headerRow)
        {
            string parameterLine = reader.ReadLine(); // 读取通道参数
            string uomLine = reader.ReadLine(); // 读取通道测量单位

            // 首先填充参数名称和单位
            string[] parameters = parameterLine.Split('\t'); // 名称由制表符隔开
            string[] uoms = uomLine.Split('\t'); //单位由制表符隔开
            StringBuilder channelNames = new StringBuilder();
            for (int index = 0; index < parameters.Length; index++)
            {
                if (index > 0)
                {
                    channelNames.Append(";");
                }

                channelNames.Append(parameters[index]).Append("(").Append(uoms[index]).Append(")");
            }

            headerRow[NewteraNameSpace.CHANNEL_NAME_ARRAY] = channelNames.ToString();
        }


        /// <summary>
        /// 读取通道数据记录
        /// </summary>
        private void ReadChannelData(StreamReader reader, DataTable channelDataTable, string groupId)
        {
            DataRow dataRow;
 
            // 读取通道数据记录
            string line;
            string[] values;
            StringBuilder channelValues;
            while ((line = reader.ReadLine()) != null)
            {
                if (line.Trim().Length > 0)
                {
                    dataRow = channelDataTable.NewRow();
                    channelValues = new StringBuilder();

                    // 设置外键
                    dataRow[NewteraNameSpace.GROUP_ID] = groupId;
                    dataRow[NewteraNameSpace.RECORD_ID] = CreateRowId();

                    // add the data row to the data table
                    channelDataTable.Rows.Add(dataRow);

                    values = line.Split('\t'); // 参数值由制表符隔开
                    for (int index = 0; index < values.Length; index++)
                    {
                        if (index > 0)
                        {
                            channelValues.Append(";");
                        }

                        channelValues.Append(values[index]);
                    }

                    dataRow[NewteraNameSpace.CHANNEL_VALUE_ARRAY] = channelValues.ToString();
                }
                else
                {
                    // 空行表示通道数据结束
                    break;
                }
            }

            if (line == null)
            {
                _endOfFile = true; //文件结束
            }
        }

        /// <summary>
        /// 读取文件的表头数据并拼接为一个字符串
        /// </summary>
        private string GetHeaderRowData(StreamReader reader)
        {
            StringBuilder builder = new StringBuilder();

            bool firstRow = true;
            bool hasData = false;

            // 每次读一行
            string line = null;
            while ((line = reader.ReadLine()) != null)
            {
                if (line.Trim().Length > 0 && firstRow)
                {
                    // 读到第一行
                    hasData = true;
                    firstRow = false;
                }
                else if (line.Trim().Length == 0)
                {
                    // 读到空行, 文件数据结束
                    line = null;
                }

                if (line != null)
                {
                    builder.Append(line.Trim()).Append(" ");
                }
                else if (!firstRow)
                {
                    break; // 读完表头
                }
            }

            if (hasData)
            {
                return builder.ToString();
            }
            else
            {
                return null; // 读到文件结尾
            }
        }

        /// <summary>
        /// 解析"Name:Value"的字符串,并返回NameValueCollection
        /// </summary>
        private NameValueCollection GetColumnValues(string rowData)
        {
            NameValueCollection columnValues = new NameValueCollection();

            // 使用正则表达式解析
            if (rowData != null)
            {
                MatchCollection matches = _nameValueExp.Matches(rowData);
                foreach (Match match in matches)
                {
                    int pos = match.Value.IndexOf(":");

                    if (pos > 0)
                    {
                        // 提取Name和Value
                        string key = match.Value.Substring(0, pos).Trim();
                        string val = match.Value.Substring(pos + 1).Trim();

                        bool added = false;
                        string next = "";
                        int colId = 0;
                        while (!added)
                        {
                            //生成一个唯一的字段名,去除特殊字符
                            string columnName = key + next;
                            columnName = columnName.Replace("#", "");
                            columnName = columnName.Replace("'", "");
                            columnName = columnName.Replace("&", "");

                            // 检查是否有重名
                            if (columnValues[columnName] == null)
                            {
                                columnValues.Add(columnName, val);
                                added = true;
                            }
                            else
                            {
                                //如果有重名,加一个序号以区别.
                                colId++;
                                next = "_" + colId.ToString();
                            }
                        }
                    }
                }
            }

            return columnValues;
        }

        /// <summary>
        /// 生成一个行记录ＩＤ值
        /// </summary>
        private string CreateRowId()
        {
            string id = "R_" + Guid.NewGuid();

            return id;
        }
    }
}
