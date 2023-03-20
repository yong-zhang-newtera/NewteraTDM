/*
* @(#)EditSQLExecutor.cs
*
* Copyright (c) 2008 Newtera, Inc. All rights reserved.
*
*/
namespace Newtera.Server.Engine.Vdom.Dbimp
{
	using System;
	using System.Threading;
	using System.Xml;
	using System.Collections;
	using System.Collections.Specialized;
	using System.Data;
    using System.Resources;

	using Newtera.Server.Engine.Vdom;
	using Newtera.Common.MetaData;
	using Newtera.Common.MetaData.Schema;
	using Newtera.Server.Engine.Sqlbuilder;
	using Newtera.Server.Engine.Sqlbuilder.Sql;
	using Newtera.Server.DB;

	/// <summary>
	/// This class executes the executable sql statements contains in a collection of SQLAction
    /// objects
	/// </summary>
	/// <version>  	1.0.0 03 Aug 2008 </version>
	public class EditSQLExecutor : Executor
	{
        private IDbConnection _con = null;
        private IDbTransaction _tran = null;
        private IDbCommand _cmd = null;

		/// <summary>
		/// Initiating an instance of EditSQLExecutor class
		/// </summary>
		/// <param name="metaData">the meta data model </param>
		/// <param name="dataProvider">the database provider</param>
		public EditSQLExecutor(MetaDataModel metaData, IDataProvider dataProvider):base(metaData, dataProvider, null)
		{
            _con = _dataProvider.Connection;
            _tran = _con.BeginTransaction();
            _cmd = _con.CreateCommand();
            _cmd.Transaction = _tran;
		}
		
		/// <summary>
		/// Execute sql statements provided by the SQLAction objects
		/// </summary>
        /// <param name="sqlActions">the instances to be inserted.</param>
		public void Execute(SQLActionCollection sqlActions)
		{			
			/*
			* Execute the insert starting from root. Database will throw
			* an error if trying to insert from bottom
			*/	
			string sql;
            foreach (SQLAction action in sqlActions)
			{
				if (action.Type == SQLActionType.Insert)
				{
					sql = action.ExecutableSQL;
					_cmd.CommandText = sql;
					_cmd.ExecuteNonQuery();
				}
			}

			// write over-size array data to the corresponding CLOB columns
			// this has to be executed after the record(s) have been created
			IClobDAO clobDAO = ClobDAOFactory.Instance.Create(this._dataProvider);
            foreach (SQLAction action in sqlActions)
			{
				if (action.Type == SQLActionType.WriteClob)
				{
					// Use the database-specific ClobDAO to write to a clob
					clobDAO.WriteClob(_cmd, action.Data, action.TableName,
						action.ColumnName, action.ObjId);
				}
			}
		}

        public void CommitChanges()
        {
            if (_tran != null)
            {
                _tran.Commit();
            }
        }

        public void RollbackChanges()
        {
            if (_tran != null)
            {
                _tran.Rollback();
            }
        }

        public void Close()
        {
            if (_con != null)
            {
                _con.Close();
            }
        }
	}
}