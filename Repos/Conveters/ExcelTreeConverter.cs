using System;
using System.Collections.Generic;
using System.Text;
using Newtera.ParserGen.Converter;
using System.Data;
using System.IO;
using Newtera.Common.Core;
using System.Collections;
using Newtera.Common.MetaData.Schema;

namespace Newtera.Conveters
{
    class ExcelTreeConverter : DataSourceConverterBase
    {
        private DataSet dataSet;      //数据集  
        private ExcelHelper excelHelp;  //初始化读取Excel类

        
        #region 构造函数
        /// <summary>
        /// 初始化类
        /// </summary>
        public ExcelTreeConverter()
        {
            dataSet = new DataSet();
            excelHelp = new ExcelHelper();
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

            //创建零部件测量记录表  NewteraNameSpace.TABLE_DATA
            DataTable inputDataTable = dataSet.Tables.Add("NewteraNameSpace.TABLE_DATA");

            //添加数据组表主键
            inputDataTable.Columns.Add(NewteraNameSpace.GROUP_ID);

            inputDataTable.Columns.Add("PartNumber");

            inputDataTable.Columns.Add("PartName");

            inputDataTable.Columns.Add("Qty");
            inputDataTable.Columns.Add("type");
            inputDataTable.Columns.Add("Version");
            inputDataTable.Columns.Add("Children");


            List<string> tableList = new List<string>();
            tableList = excelHelp.GetSheetNames(dataSourceName);  //获取列表里的所有Sheet

            if (tableList.Count > 0)
            {
                try
                {
                    ReadHeaderData(inputDataTable, tableList[0], dataSourceName); //读取文件数据
                }
                catch (Exception exp)
                {
                    Console.WriteLine(exp.ToString());  //输出异常
                }
            }
            else
            {
                return null;
            }

            return dataSet;  //返回数据集
        }
        #endregion

        /// <summary>
        /// 从文件中读取表头数据,并作为DataTable中的唯一记录
        /// </summary>
        /// <param name="headerDataTable">结构化数据组表</param>
        /// <param name="dataSourceName">文件路径</param>
        /// <returns></returns>
        private void ReadHeaderData(DataTable headerDataTable, string sheetName, string dataSourceName)
        {
            DataRow dataRow = null; //定义行数据
            Hashtable hashTID = new Hashtable();
            //创建暂时存储Excel数据的数据集
            DataSet dataSetExcel = new DataSet();
            //读取数据并存储到dataSetExcel数据集里
            dataSetExcel = excelHelp.GetDataSet(dataSourceName, sheetName);
            int upId = 0;

            bool isFirst = true;
            foreach (DataRow dataNowRow in dataSetExcel.Tables[0].Rows)
            {
                if (isFirst)
                {
                    isFirst = false;
                    continue;
                }
                if (dataNowRow[0].ToString().Trim() != "ID")
                {
                    //能表创建新行
                    dataRow = headerDataTable.NewRow();
                    string groupId = CreateRowId();
                    dataRow[NewteraNameSpace.GROUP_ID] = groupId;
                    headerDataTable.Rows.Add(dataRow);   //往表里添加数据
                    dataRow["PartNumber"] = dataNowRow[1].ToString().Trim();
                    dataRow["PartName"] = dataNowRow[3].ToString().Trim();
                    dataRow["Qty"] = dataNowRow[4].ToString().Trim();
                    dataRow["type"] = dataNowRow[5].ToString().Trim();
                    dataRow["Version"] = dataNowRow[2].ToString().Trim();
                    upId = int.Parse(dataNowRow[0].ToString());

                    if (upId == 0)
                    {
                        dataRow["Children"] = "";
                    }
                    else
                    {
                        dataRow["Children"] = hashTID[upId - 1];
                    }

                    if (hashTID.ContainsKey(upId))
                    {
                        hashTID[upId] = groupId;
                    }
                    else
                    {
                        hashTID.Add(upId, groupId);
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
