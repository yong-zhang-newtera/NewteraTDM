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

using Newtera.Common.Core;
using Newtera.Common.MetaData;
using Newtera.Common.MetaData.Schema;
using Newtera.Common.MetaData.DataView;
using Newtera.Common.MetaData.DataView.Taxonomy;
using Newtera.Common.MetaData.XaclModel;
using Newtera.Common.MetaData.Principal;
using Newtera.SmartWordUtil;
using SmartExcel.CMDataWebService;
using Newtera.WindowsControl;

namespace SmartExcel
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