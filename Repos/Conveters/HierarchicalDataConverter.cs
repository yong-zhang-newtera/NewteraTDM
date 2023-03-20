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
    public class HierarchicalDataConverter : DataSourceConverterBase
    {
        private const string NAME_VALUE_EXP = @"\s*[^:]+:\s*[^\s]*";

        private Regex _nameValueExp;
        private bool _endOfFile;
        private bool _groupColumnCreated;
        private bool _channelColumnCreated;

        public HierarchicalDataConverter() : base()
        {
            // Compile regular expression to find "name = value" or "name : value" pairs
            _nameValueExp = new Regex(HierarchicalDataConverter.NAME_VALUE_EXP);
            _endOfFile = false;
            _groupColumnCreated = false;
            _channelColumnCreated = false;
        }

        public override DataSet Convert(string dataSourceName)
        {
            DataSet dataSet = new DataSet();
            string fileId;

            DataTable fileDataTable = dataSet.Tables.Add("File");
            fileDataTable.Columns.Add(NewteraNameSpace.FILE_ID); 

            DataTable groupDataTable = dataSet.Tables.Add("Group");
            groupDataTable.Columns.Add(NewteraNameSpace.GROUP_ID); // 组记录的ＩＤ
            groupDataTable.Columns.Add(NewteraNameSpace.FILE_ID); // 外键

            DataTable channelDataTable = dataSet.Tables.Add("Channel");
            channelDataTable.Columns.Add(NewteraNameSpace.CHANNEL_ID); // 外键
            channelDataTable.Columns.Add(NewteraNameSpace.GROUP_ID); // 外键

            // 创建DataTable之间的关系
            dataSet.Relations.Add(NewteraNameSpace.FILE_GROUP_RELATION, fileDataTable.Columns[NewteraNameSpace.FILE_ID],
                groupDataTable.Columns[NewteraNameSpace.FILE_ID]);
            dataSet.Relations.Add(NewteraNameSpace.GROUP_CHANNEL_RELATION, groupDataTable.Columns[NewteraNameSpace.GROUP_ID],
                channelDataTable.Columns[NewteraNameSpace.GROUP_ID]);

            // open the file as stream reader
            using (StreamReader reader = new StreamReader(dataSourceName, Encoding.Default))
            {
                // 从文件中读取文件级描述数据
                fileId = ReadFileData(reader, fileDataTable); // 读文件描述数据记录

                while (!_endOfFile)
                {
                    ReadGroupData(reader, groupDataTable, channelDataTable, fileId); // 读数据组数据
                }
            }
            
            return dataSet;
        }

        /// <summary>
        /// 从文件中读取文件描述数据,并作为DataTable中的唯一记录
        /// </summary>
        private string ReadFileData(StreamReader reader, DataTable fileDataTable)
        {
            string rowData = GetFileRowData(reader);

            NameValueCollection columnValues = GetColumnValues(rowData);

            // 在DataTable中定义字段名称
            CreateColumns(fileDataTable, columnValues);

            // 添加文件数据记录
            string fileId = AddDataRow(NewteraNameSpace.FILE_ID, fileDataTable, columnValues, null, null);

            return fileId;
        }

        /// <summary>
        /// 从文件中读取组，及组的通道数据
        /// </summary>
        private void ReadGroupData(StreamReader reader, DataTable groupDataTable, DataTable channelDataTable, string fileId)
        {
            NameValueCollection columnValues = GetGroupRowData(reader);

            // 在DataTable中定义组的字段名称
            if (!_groupColumnCreated)
            {
                CreateColumns(groupDataTable, columnValues);
                _groupColumnCreated = true;
            }

            // 添加组数据记录
            string groupId = AddDataRow(NewteraNameSpace.GROUP_ID, groupDataTable, columnValues, NewteraNameSpace.FILE_ID, fileId);

            // 读取组的通道数据
            ReadChannelsData(reader, channelDataTable, groupId);
        }

        /// <summary>
        /// 从文件中读取组的通道数据
        /// </summary>
        private void ReadChannelsData(StreamReader reader, DataTable channelDataTable, string groupId)
        {
            if (!_channelColumnCreated)
            {
                // 创建DataTable的字段
                channelDataTable.Columns.Add(NewteraNameSpace.CHANNEL_NAME);//名称
                channelDataTable.Columns.Add(NewteraNameSpace.CHANNEL_UNIT);　// 单位
                channelDataTable.Columns.Add(NewteraNameSpace.CHANNEL_DATA); // 数据
                _channelColumnCreated = true;
            }

            //　跳过前两行
            reader.ReadLine();
            reader.ReadLine();

            string parameterLine = reader.ReadLine();
            string uomLine = reader.ReadLine();

            // 首先填充参数名称和单位
            string[] parameters = parameterLine.Split('\t'); // 名称由制表符隔开
            string[] uoms = uomLine.Split('\t'); //单位由制表符隔开
            DataRow dataRow;
            List<DataRow> createdDataRows = new List<DataRow>();
            for (int row = 0; row < parameters.Length; row++)
            {
                dataRow = channelDataTable.NewRow();

                // 设置外键
                dataRow[NewteraNameSpace.GROUP_ID] = groupId;
                dataRow[NewteraNameSpace.CHANNEL_ID] = CreateRowId();
                dataRow[NewteraNameSpace.CHANNEL_NAME] = parameters[row];
                dataRow[NewteraNameSpace.CHANNEL_UNIT] = uoms[row];
                dataRow[NewteraNameSpace.CHANNEL_DATA] = "";

                // add the data row to the data table
                channelDataTable.Rows.Add(dataRow);
                createdDataRows.Add(dataRow);
            }

            // 填充通道数据
            string line;
            string[] values;
            while ((line = reader.ReadLine()) != null)
            {
                if (line.Trim().Length > 0)
                {
                    values = line.Split('\t'); // 参数值由制表符隔开
                    string val;
                    for (int row = 0; row < values.Length; row++)
                    {
                        // 形成一个由分号隔开的数组
                        val = createdDataRows[row][NewteraNameSpace.CHANNEL_DATA].ToString();
                        if (val.Length == 0)
                        {
                            createdDataRows[row][NewteraNameSpace.CHANNEL_DATA] = values[row];
                        }
                        else
                        {
                            createdDataRows[row][NewteraNameSpace.CHANNEL_DATA] = val + ";" + values[row];
                        }
                    }
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
        /// 读取文件描述数据字符并拼接为一个字符串
        /// </summary>
        private string GetFileRowData(StreamReader reader)
        {
            StringBuilder builder = new StringBuilder();

            bool foundRow = false;

            // 每次读一行
            string line = null;
            while ((line = reader.ReadLine()) != null)
            {
                if (line.StartsWith("文件名称:"))
                {
                    // 读到第一行
                    foundRow = true;
                }
                else if (line.Trim().Length == 0)
                {
                    // 读到空行, 文件数据结束
                    line = null;
                }

                if (foundRow && line != null)
                {
                    builder.Append(line.Trim()).Append(" ");
                }
                else
                {
                    break;
                }
            }

            if (foundRow)
            {
                return builder.ToString();
            }
            else
            {
                return null; // 读到文件结尾
            }
        }

        /// <summary>
        /// 读取组描述数据字符并拼接为一个字符串
        /// </summary>
        private NameValueCollection GetGroupRowData(StreamReader reader)
        {
            reader.ReadLine(); // 跳过一行
            string nameLine = reader.ReadLine(); // 读名称行
            string valueLine = reader.ReadLine(); // 读数值行
           
            // 形成名称值组合
            string[] names = nameLine.Split('\t'); // 名称由制表符隔开
            string[] values = valueLine.Split('\t'); //值由制表符隔开

            NameValueCollection columnValues = new NameValueCollection();
            for (int i = 0; i < names.Length; i++)
            {
                bool added = false;
                string next = "";
                int colId = 0;
                while (!added)
                {
                    //生成一个唯一的字段名,去除特殊字符
                    string columnName = names[i] + next;
                    columnName = columnName.Replace("#", "");
                    columnName = columnName.Replace("'", "");
                    columnName = columnName.Replace("&", "");

                    // 检查是否有重名
                    if (columnValues[columnName] == null)
                    {
                        columnValues.Add(columnName, values[i]);
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

            return columnValues;
        }

        /// <summary>
        /// 解析"Name:Value"的字符串,并返回NameValueCollection
        /// </summary>
        private NameValueCollection GetColumnValues(string rowData)
        {
            NameValueCollection columnValues = new NameValueCollection();

            // 使用正则表达式解析
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

            return columnValues;
        }

        /// <summary>
        /// 在DataTable中定义字段名称
        /// </summary>
        private void CreateColumns(DataTable dataTable, NameValueCollection columnValues)
        {
            for (int i = 0; i < columnValues.Count; i++)
            {
                string col = columnValues.GetKey(i);
                bool added = false;
                string next = "";
                int colId = 0;
                while (!added)
                {
                    //生成一个唯一的字段名,去除特殊字符
                    string columnName = col + next;
                    columnName = columnName.Replace("#", "");
                    columnName = columnName.Replace("'", "");
                    columnName = columnName.Replace("&", "");

                    // 检查是否有重名
                    if (!dataTable.Columns.Contains(columnName))
                    {
                        dataTable.Columns.Add(columnName);
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

        /// <summary>
        /// 在DataTable添加一行,并返回生成的ID值
        /// </summary>
        private string AddDataRow(string idColumnName, DataTable dataTable, NameValueCollection columnValues, string fkColumnName, string fkValue)
        {
            DataRow dataRow = dataTable.NewRow();

            string rowId = null;

            if (idColumnName != null)
            {
                // 设置主键
                rowId = CreateRowId();
                dataRow[idColumnName] = rowId;
            }

            // 设置外键
            if (fkColumnName != null)
            {
                dataRow[fkColumnName] = fkValue;
            }

            // fill the data row with data        
            foreach (string key in columnValues.Keys)
            {
                dataRow[key] = columnValues[key];
            }

            // add the data row to the data table
            dataTable.Rows.Add(dataRow);

            return rowId;
        }

        /// <summary>
        /// Add columns to the DataTable
        /// </summary>
        private void AddColumns(string firstLine, DataTable dataTable)
        {
            string[] columns = firstLine.Split(new Char[] { ' ', '\t'});

            foreach (string col in columns)
            {
                bool added = false;
                string next = "";
                int i = 0;
                while (!added)
                {
                    //生成一个唯一的字段名,去除特殊字符
                    string columnName = col + next;
                    columnName = columnName.Replace("#", "");
                    columnName = columnName.Replace("'", "");
                    columnName = columnName.Replace("&", "");

                    //See if the column already exists
                    if (!dataTable.Columns.Contains(columnName))
                    {
                        //if it doesn't then we add it here and mark it as added
                        dataTable.Columns.Add(columnName);
                        added = true;
                    }
                    else
                    {
                        //if it did exist then we increment the sequencer and try again.
                        i++;
                        next = "_" + i.ToString();
                    }
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
    }
}
