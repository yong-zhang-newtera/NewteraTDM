using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.ComponentModel;
using System.Data;
using System.Text;
using System.IO;
using System.Xml;
using System.Threading;
using System.Collections;
using Word = Microsoft.Office.Interop.Word;

using Newtera.Common.Core;
using Newtera.Common.MetaData;
using Newtera.Common.MetaData.Schema;
using Newtera.Common.MetaData.DataView;
using Newtera.Common.MetaData.DataView.Taxonomy;
using Newtera.Common.MetaData.XaclModel;
using Newtera.Common.MetaData.Principal;
using Newtera.SmartWordUtil;
using SmartWord.CMDataWebService;
using Newtera.WindowsControl;

namespace SmartWord
{
    public class DocDataSourceClient : DocDataSourceBase
    {
        private CMDataService _dataService;
        private ResultDataControl _resultDataControl;
        private int _pageSize = 100;

        public DocDataSourceClient(ResultDataControl resultDataControl)
        {
            _resultDataControl = resultDataControl;
        }

        /// <summary>
        /// Gets or sets name of the selected base class in the data source.
        /// </summary>
        public override string BaseClassName
        {
            get
            {
                return _resultDataControl.CurrentSlide.DataView.BaseClass.Name;
            }
            set
            {
            }
        }

        /// <summary>
        /// Gets or sets the meta data model of the data source.
        /// </summary>
        public override MetaDataModel MetaData
        {
            get
            {
                return _resultDataControl.MetaData;
            }
            set
            {
            }
        }

        /// <summary>
        /// Gets information indicating whether the given class is an inherited class to the base class
        /// </summary>
        /// <param name="className"></param>
        /// <returns></returns>
        public override bool IsInheitedClass(string className)
        {
            bool status = false;

            InstanceView instanceView = GetInstanceView();

            ClassElement baseClassElement = instanceView.DataView.SchemaModel.FindClass(BaseClassName);
            if (baseClassElement.FindParentClass(className) != null)
            {
                status = true;
            }

            return status;
        }

        /// <summary>
        /// Gets an InstanceView representing a data instance that is related to the data instances
        /// selected.
        /// </summary>
        /// <param name="familyNode">The family node that contains the related view node</param>
        /// <param name="viewNode">The view node presenting a related view</param>
        /// <returns>An InstanceView representing a data instance</returns>
        public override InstanceView GetInstanceView(Word.XMLNode familyNode, Word.XMLNode viewNode)
        {
            InstanceView instanceView = null;

            DataViewModel instanceDataView = GetDataView(familyNode, viewNode);
            if (instanceDataView != null)
            {

                string query = instanceDataView.SearchQuery;

                int totalCount = GetCMDataWebService().ExecuteCount(ConnectionStringBuilder.Instance.Create(MetaData.SchemaInfo),
                    query);

                if (totalCount > PopulateLineLimit)
                {
                    string msg = string.Format(Newtera.WindowsControl.MessageResourceManager.GetString("SmartWord.TooManyInstances"), instanceView.DataView.BaseClass.Caption, totalCount, PopulateLineLimit);
                    throw new Exception(msg);
                }

                // get the result in paging mode, the database connection is released at BeginQueryDone method when done
                string queryId = GetCMDataWebService().BeginQuery(ConnectionStringBuilder.Instance.Create(MetaData.SchemaInfo), query, _pageSize);

                DataSet masterDataSet = null;

                DataSet slaveDataSet;
                int currentPageIndex = 0;
                int instanceCount = 0;
                int start, end;
                XmlReader xmlReader;
                XmlNode xmlNode;
                while (instanceCount < totalCount)
                {
                    start = currentPageIndex * _pageSize + 1;
                    end = start + this._pageSize - 1;
                    if (end > totalCount)
                    {
                        end = totalCount;
                    }

                    // invoke the web service synchronously to get data in pages
                    xmlNode = GetCMDataWebService().GetNextResult(ConnectionStringBuilder.Instance.Create(MetaData.SchemaInfo),
                        queryId);

                    if (xmlNode == null)
                    {
                        // end of result
                        break;
                    }

                    slaveDataSet = new DataSet();

                    xmlReader = new XmlNodeReader(xmlNode);
                    slaveDataSet.ReadXml(xmlReader);

                    instanceCount += slaveDataSet.Tables[instanceDataView.BaseClass.Name].Rows.Count;

                    if (masterDataSet == null)
                    {
                        // first page
                        masterDataSet = slaveDataSet;
                        masterDataSet.EnforceConstraints = false;
                    }
                    else
                    {
                        // append to the master dataset
                        AppendDataSet(instanceDataView.BaseClass.Name, masterDataSet, slaveDataSet);
                    }

                    currentPageIndex++;
                }

                if (masterDataSet != null &&
                    !DataSetHelper.IsEmptyDataSet(masterDataSet, instanceDataView.BaseClass.ClassName))
                {
                    instanceView = new InstanceView(instanceDataView, masterDataSet);
                    instanceView.SelectedIndex = 0;
                }
                else
                {
                    instanceView = null;
                }
            }

            return instanceView;
        }

        /// <summary>
        /// Gets an InstanceView representing the selected instance of the base class.
        /// </summary>
        /// <returns>An InstanceView representing the data instance for the specified row</returns>
        public override InstanceView GetInstanceView()
        {
            return GetInstanceView(-1);
        }

        /// <summary>
        /// Gets an InstanceView representing the instance data of
        /// the given row index in the datagrid.
        /// </summary>
        /// <param name="rowIndex">The row index, -1 for the currently selected index.</param>
        /// <returns>An InstanceView representing the data instance for the specified row</returns>
        private InstanceView GetInstanceView(int rowIndex)
        {
            if (rowIndex >= 0)
            {
                _resultDataControl.SetCurrentSlideSelectedRowIndex(rowIndex);
            }

            if (_resultDataControl.CurrentSlide.InstanceView == null)
            {
                if (_resultDataControl.CurrentSlide.InstanceDataView == null)
                {
                    // uses the same dataview of the data slide
                    _resultDataControl.CurrentSlide.InstanceDataView = _resultDataControl.CurrentSlide.DataView;
                }

                _resultDataControl.CurrentSlide.InstanceView = new InstanceView(_resultDataControl.CurrentSlide.InstanceDataView,
                    _resultDataControl.CurrentSlide.DataSet);
            }

            // this assign the selected instance to the InstanceView object
            if (_resultDataControl.CurrentSlide.InstanceView.SelectedIndex != _resultDataControl.CurrentSlide.SelectedRowIndex)
            {
                _resultDataControl.CurrentSlide.InstanceView.SelectedIndex = _resultDataControl.CurrentSlide.SelectedRowIndex;
            }

            return _resultDataControl.CurrentSlide.InstanceView;
        }

        /// <summary>
        /// Get a web service for retrieving data instance
        /// </summary>
        /// <returns></returns>
        private CMDataService GetCMDataWebService()
        {
            if (_dataService == null)
            {
                _dataService = new CMDataService();
            }

            return _dataService;
        }

        /// <summary>
        /// Append a slave DataSet to a master dataset
        /// </summary>
        /// <param name="master">The master DataSet</param>
        /// <param name="slave">The slave DataSet</param>
        private void AppendDataSet(string tableName, DataSet master, DataSet slave)
        {
            DataTable masterTable = master.Tables[tableName];
            DataTable slaveTable = slave.Tables[tableName];

            foreach (DataRow row in slaveTable.Rows)
            {
                masterTable.ImportRow(row);
            }
        }
    }
}