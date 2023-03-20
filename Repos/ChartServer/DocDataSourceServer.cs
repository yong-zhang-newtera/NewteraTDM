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
using Newtera.Data;
using Newtera.SmartWordUtil;

namespace Newtera.ChartServer
{
    public class DocDataSourceServer : DocDataSourceBase
    {
        private string _connectionStr;
        private string _baseInstanceId;
        private string _baseClassName;
        private InstanceView _baseInstanceView;
        private Hashtable _relatedInstanceViewTable;

        public DocDataSourceServer(string connectionStr, string baseClassName, string baseInstanceId)
        {
            _connectionStr = connectionStr;
            _baseClassName = baseClassName;
            _baseInstanceId = baseInstanceId;
            _baseInstanceView = null;
            _relatedInstanceViewTable = new Hashtable();
        }

        /// <summary>
        /// Gets or sets name of the selected base class in the data source.
        /// </summary>
        public override string BaseClassName
        {
            get
            {
                return _baseClassName;
            }
            set
            {
                _baseClassName = value;
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

            using (CMConnection connection = new CMConnection(_connectionStr))
            {
                connection.Open();

                ClassElement baseClassElement = connection.MetaDataModel.SchemaModel.FindClass(_baseClassName);
                if (baseClassElement.FindParentClass(className) != null)
                {
                    status = true;
                }
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

            string path = GetAttributeValue(viewNode, WordPopulator.PathAttribute, false);
            //  try to get the instanceView from the hash table for sake of performance
            instanceView = (InstanceView)_relatedInstanceViewTable[path];
            if (instanceView == null)
            {
                DataViewModel instanceDataView = GetDataView(familyNode, viewNode);
                if (instanceDataView != null)
                {
                    using (CMConnection connection = new CMConnection(_connectionStr))
                    {
                        connection.Open();
                        string query = instanceDataView.SearchQuery;

                        CMCommand cmd = connection.CreateCommand();
                        cmd.CommandText = query;

                        int totalCount = cmd.ExecuteCount();

                        if (totalCount > PopulateLineLimit)
                        {
                            string msg = string.Format(Newtera.SmartWordUtil.MessageResourceManager.GetString("SmartWord.TooManyInstances"), instanceView.DataView.BaseClass.Caption, totalCount, PopulateLineLimit);
                            throw new Exception(msg);
                        }

                        if (totalCount > 0)
                        {
                            cmd = connection.CreateCommand();
                            cmd.PageSize = totalCount;
                            cmd.CommandText = query;

                            // Since the result will be displayed on DataGridView, we don't need to check
                            // write permissions on each attribute, use CMCommandBehavior.CheckReadPermissionOnly
                            XmlReader reader = cmd.ExecuteXMLReader(CMCommandBehavior.CheckReadPermissionOnly);
                            DataSet ds = new DataSet();
                            ds.ReadXml(reader);

                            instanceView = new InstanceView(instanceDataView, ds);
                        }
                        else
                        {
                            instanceView = new InstanceView(instanceDataView);
                        }
                    }
                }

                _relatedInstanceViewTable[path] = instanceView;
            }

            return instanceView;
        }

        /// <summary>
        /// Gets an InstanceView representing the selected instance of the base class.
        /// </summary>
        /// <returns>An InstanceView representing the data instance for the specified row</returns>
        public override InstanceView GetInstanceView()
        {
            if (_baseInstanceView == null || _baseInstanceView.InstanceData.ObjId != _baseInstanceId)
            {
                using (CMConnection connection = new CMConnection(_connectionStr))
                {
                    connection.Open();

                    DataViewModel instanceDataView = connection.MetaDataModel.GetDetailedDataView(BaseClassName);

                    string query = instanceDataView.GetInstanceQuery(_baseInstanceId);

                    CMCommand cmd = connection.CreateCommand();
                    cmd.CommandText = query;

                    // Since the result will be displayed on DataGridView, we don't need to check
                    // write permissions on each attribute, use CMCommandBehavior.CheckReadPermissionOnly
                    XmlReader reader = cmd.ExecuteXMLReader(CMCommandBehavior.CheckReadPermissionOnly);
                    DataSet ds = new DataSet();
                    ds.ReadXml(reader);

                    _baseInstanceView = new InstanceView(instanceDataView, ds);
                }
            }

            return _baseInstanceView;
        }
    }
}