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
        private DataSet dataSet;      //���ݼ�  
        private ExcelHelper excelHelp;  //��ʼ����ȡExcel��

        
        #region ���캯��
        /// <summary>
        /// ��ʼ����
        /// </summary>
        public ExcelTreeConverter()
        {
            dataSet = new DataSet();
            excelHelp = new ExcelHelper();
        }
        #endregion

        #region SupportPaging����
        /// <summary>
        /// �����Ƿ�ֿ��ȡ����
        /// </summary>
        public override bool SupportPaging
        {
            get
            {
                return false;
            }
        }
        #endregion

        #region Close����
        /// <summary>
        /// �رյ�����
        /// </summary>
        public override void Close()
        {
            this.dataSet = null;
        }
        #endregion

        #region ���ֿ��ȡ���ݵķ���
        /// <summary>
        /// ����ת��������
        /// </summary>
        /// <param name="dataSourceName">ת�����ݵ�·��</param>
        /// <returns>���ݼ�</returns>
        public override System.Data.DataSet Convert(string dataSourceName)
        {
            DataSet dataSet = new DataSet();

            //�����㲿��������¼��  NewteraNameSpace.TABLE_DATA
            DataTable inputDataTable = dataSet.Tables.Add("NewteraNameSpace.TABLE_DATA");

            //��������������
            inputDataTable.Columns.Add(NewteraNameSpace.GROUP_ID);

            inputDataTable.Columns.Add("PartNumber");

            inputDataTable.Columns.Add("PartName");

            inputDataTable.Columns.Add("Qty");
            inputDataTable.Columns.Add("type");
            inputDataTable.Columns.Add("Version");
            inputDataTable.Columns.Add("Children");


            List<string> tableList = new List<string>();
            tableList = excelHelp.GetSheetNames(dataSourceName);  //��ȡ�б��������Sheet

            if (tableList.Count > 0)
            {
                try
                {
                    ReadHeaderData(inputDataTable, tableList[0], dataSourceName); //��ȡ�ļ�����
                }
                catch (Exception exp)
                {
                    Console.WriteLine(exp.ToString());  //����쳣
                }
            }
            else
            {
                return null;
            }

            return dataSet;  //�������ݼ�
        }
        #endregion

        /// <summary>
        /// ���ļ��ж�ȡ��ͷ����,����ΪDataTable�е�Ψһ��¼
        /// </summary>
        /// <param name="headerDataTable">�ṹ���������</param>
        /// <param name="dataSourceName">�ļ�·��</param>
        /// <returns></returns>
        private void ReadHeaderData(DataTable headerDataTable, string sheetName, string dataSourceName)
        {
            DataRow dataRow = null; //����������
            Hashtable hashTID = new Hashtable();
            //������ʱ�洢Excel���ݵ����ݼ�
            DataSet dataSetExcel = new DataSet();
            //��ȡ���ݲ��洢��dataSetExcel���ݼ���
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
                    //�ܱ�������
                    dataRow = headerDataTable.NewRow();
                    string groupId = CreateRowId();
                    dataRow[NewteraNameSpace.GROUP_ID] = groupId;
                    headerDataTable.Rows.Add(dataRow);   //�������������
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
        /// ����һ���м�¼�ɣ�ֵ
        /// </summary>
        private string CreateRowId()
        {
            string id = "R_" + Guid.NewGuid();
            return id;
        }
    }
}
