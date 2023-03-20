using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading.Tasks;
//using Newtonsoft.Json;
//using Newtonsoft.Json.Linq;

using Newtera.Common.Core;
using Newtera.Common.MetaData;
using Newtera.Common.MetaData.Schema;
using Newtera.Common.MetaData.Principal;

namespace Newtera.WinClientCommon
{
    public class ActiveXControlServiceStub : WebApiServiceBase
    {
        public ActiveXControlServiceStub()
        {
        }

        public string GetDataGridViewForClass(string connectionStr, string className)
        {
            return "";
        }

        public string GetDataGridViewForTaxon(string connectionStr, string taxonomyName, string taxonName)
        {
            return "";
        }

        public string GetDataGridViewForRelatedClass(string connectionStr, string className, string relatedClassAlias, string relatedClassName, string objId)
        {
            return "";
        }

        public string GetDataGridViewForArray(string connectionStr, string className, string arrayName)
        {
            return "";
        }

        public string GetXQueryForDataGridView(string connectionStr, string xml)
        {
            return "";
        }

        public string BeginQuery(string connectionStr, string query, int pageSize)
        {
            return "";
        }

        public System.Xml.XmlNode GetNextResult(string connectionStr, string queryId)
        {
            return null;
        }

        public void EndQuery(string queryId)
        {
  
        }

        public string GetArrayData(string connectionStr, string className, string arrayName, string instanceId)
        {
            return "";
        }

        public bool IsDBA(string connectionStr)
        {
            return false;
        }

        public void SaveWorkingChartInfo(string connectionStr, string chartType, string xml, int blockNum)
        {
     
        }

        public void SaveNamedChartInfo(string connectionStr, string name, string desc, string chartType, string xml, int blockNum)
        {
  
        }

        public void SaveNamedChartTemplate(string connectionStr, string className, string name, string desc, string chartType, string xml)
        {
 
        }

        public bool IsChartNameUnique(string connectionStr, string name)
        {
            return false;
        }

        public bool IsTemplateNameUnique(string connectionStr, string className, string name)
        {
            return true;
        }

        public string GetChartInfos(string connectionStr)
        {
            return "";
        }

        public string GetChartTemplates(string connectionStr, string className)
        {
            return "";
        }

        public string GetChartDefXmlById(string chartId, int blockNum)
        {
            return "";
        }

        public string GetTemplateDefXmlById(string templateId)
        {
            return "";
        }

        public void DeleteChartTemplateById(string templateId)
        {
  
        }

        public string GetExportTypesInXml()
        {
            return "";
        }

        public string GetChartFormatsInXml()
        {
            return "";
        }

        public string GetAlgorithmTypesInXml()
        {
            return "";
        }

        public void ExportDataToFile(string connectionStr, string dataGridViewXml, string fileName, string exportTypeName, string selectedLines)
        {
  
        }

        public void ExportArrayDataToFile(string connectionStr, string className, string arrayName, string instanceId, string fileName, string exportTypeName, string selectedLines)
        {
  
        }

        public string RunAlgorithmOnClassData(string connectionStr, string dataGridViewXml, string algorithmName, string selectedLines)
        {
            return "";
        }

        public string RunAlgorithmOnArrayData(string connectionStr, string className, string arrayName, string instanceId, string algorithmName, string selectedCols, string selectedRows)
        {
            return "";
        }

        public string GetClassDataForPivotGrid(string connectionStr, string dataGridViewXml)
        {
            return "";
        }

        public string GetArrayDataForPivotGrid(string connectionStr, string className, string arrayName, string instanceId)
        {
            return "";
        }

        public string[] GetImageNames()
        {
            return null;
        }

        public byte[] GetImageBytes(string imageName)
        {
            return null;
        }

        public string UpdateData(string connectionStr, string className, string xml)
        {
            return "";
        }


    }
}
