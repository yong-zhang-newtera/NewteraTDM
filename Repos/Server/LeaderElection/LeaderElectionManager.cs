/*
* @(#) LeaderElectionManager.cs
*
* Copyright (c) 2019 Newtera, Inc. All rights reserved.
*
*/
namespace Newtera.Server.LeaderElection
{
	using System;
	using System.IO;
	using System.Text;
	using System.Data;
	using System.Collections.Specialized;

    using Newtera.Common.Core;
	using Newtera.Server.DB;
    using Newtera.Server.Engine.Sqlbuilder.Sql;

    /// <summary>
    /// Responsible to participate in the leader election or get the current leader info.
    /// </summary>
    /// <version> 1.0.0	27 Jul 2019 </version>
    public class LeaderElectionManager
	{
        private IDataProvider _dataProvider;
        private string _resourceId;
        private string _myId;
        private string _url;

		/// <summary>
		/// Instantiate an instance of ChartInfoDBRepository class.
		/// </summary>
		public LeaderElectionManager(string resourceId, string resourceUrl)
		{
            _resourceId = resourceId;
            _myId = NewteraNameSpace.ComputerCheckSum;
            _url = resourceUrl;
            _dataProvider = DataProviderFactory.Instance.Create();
		}

        public bool TryAcquire(DateTime expiration)
        {
            expiration = expiration.ToUniversalTime();
            if (expiration < DateTime.UtcNow)
                return false;

            var leader = GetLeaderInfo(_resourceId);
            if (leader != null)
            {
                if (leader.Expiration >= DateTime.UtcNow && leader.OwnerId != _myId)
                {
                    return false;
                }

                leader.OwnerId = this._myId;
                leader.Expiration = expiration;
                leader.URL = this._url;
                UpdateLeaderInfo(leader);

                return true;
            }
            else
            {
                // create the leader record with my id
                InsertLeaderInfo(new LeaderInfo {
                    ResourceId = this._resourceId,
                    OwnerId = this._myId,
                    Expiration = expiration,
                    URL = this._url
                });
            }
  
            return true;
        }

        #region DB access methods

        /// <summary>
        /// Gets a LeaderInfo for a resource.
        /// </summary>
        /// <param name="resourceId">The resource id</param>
        private LeaderInfo GetLeaderInfo(string resourceId)
		{
            IDbConnection con = _dataProvider.Connection;
            IDbCommand cmd = con.CreateCommand();
            string sql = CannedSQLManager.GetCannedSQLManager(_dataProvider).GetSql("GetResourceLeaderInfo");

            try
            {
                sql = sql.Replace(GetParamName("ResourceId", _dataProvider), $"'{resourceId}'");

                cmd.CommandText = sql;

                IDataReader reader = cmd.ExecuteReader();

                LeaderInfo leader = null;
                if (reader.Read())
                {
                    leader = new LeaderInfo();
                    leader.ResourceId = resourceId;
                    leader.OwnerId = reader.GetString(0);
                    leader.Expiration = reader.GetDateTime(1);
                    leader.URL = reader.GetString(2);
                }

                return leader;
            }
            finally
            {
                con.Close();
            }
        }

        /// <summary>
        /// Delete a leader record from database by the given id
        /// </summary>
        /// <param name="resourceId">The resource id.</param>
        private void DeleteLeaderInfo(string resourceId)
		{
			IDbConnection con = _dataProvider.Connection;
			IDbTransaction tran = con.BeginTransaction();
			IDbCommand cmd = con.CreateCommand();
			cmd.Transaction = tran;
			string sql = CannedSQLManager.GetCannedSQLManager(_dataProvider).GetSql("DeleteResourceLeaderInfo");

			try
			{
                sql = sql.Replace(GetParamName("ResourceId", _dataProvider), $"'{resourceId}'");

                cmd.CommandText = sql;

				cmd.ExecuteNonQuery();

				tran.Commit();
			}
			catch (Exception e)
			{
				tran.Rollback();
				throw e;
			}
			finally
			{
				con.Close();
			}
		}

		/// <summary>
		/// Insert a leader record into database.
		/// </summary>
		/// <param name="leaderInfo">The leader information that describes the local resource.</param>
		private void InsertLeaderInfo(LeaderInfo leader)
		{
			IDbConnection con = _dataProvider.Connection;
			IDbTransaction tran = con.BeginTransaction();
			IDbCommand cmd = con.CreateCommand();
			cmd.Transaction = tran;
			string sql = CannedSQLManager.GetCannedSQLManager(_dataProvider).GetSql("AddResourceLeaderInfo");

			try
			{
                sql = sql.Replace(GetParamName("ResourceId", _dataProvider), $"'{leader.ResourceId}'");
                sql = sql.Replace(GetParamName("OwnerId", _dataProvider), $"'{leader.OwnerId}'");
                SymbolLookup lookup = SymbolLookupFactory.Instance.Create(_dataProvider.DatabaseType);
                string expiration = lookup.GetTimestampFunc(leader.Expiration.ToString("s"), LocaleInfo.Instance.DateTimeFormat);
                sql = sql.Replace(GetParamName("Expiration", _dataProvider), expiration);
				sql = sql.Replace(GetParamName("URL", _dataProvider), $"'{leader.URL}'");
				
				cmd.CommandText = sql;
				
				cmd.ExecuteNonQuery();

				tran.Commit();
			}
			catch (Exception e)
			{
				tran.Rollback();
				throw e;
			}
			finally
			{
				con.Close();
			}
		}

		/// <summary>
		/// Update the record in database given a resource id.
		/// </summary>
		/// <param name="leaderInfo">The leader information to update.</param>
		private void UpdateLeaderInfo(LeaderInfo leader)
		{
			IDbConnection con = _dataProvider.Connection;
			IDbTransaction tran = con.BeginTransaction();
			IDbCommand cmd = con.CreateCommand();
			cmd.Transaction = tran;
			string sql = CannedSQLManager.GetCannedSQLManager(_dataProvider).GetSql("UpdateResourceLeaderInfo");

			try
			{
				sql = sql.Replace(GetParamName("ResourceId", _dataProvider), $"'{leader.ResourceId}'");
                sql = sql.Replace(GetParamName("OwnerId", _dataProvider), $"'{leader.OwnerId}'");
                SymbolLookup lookup = SymbolLookupFactory.Instance.Create(_dataProvider.DatabaseType);
                string expiration = lookup.GetTimestampFunc(leader.Expiration.ToString("s"), LocaleInfo.Instance.DateTimeFormat);
                sql = sql.Replace(GetParamName("Expiration", _dataProvider), expiration);
                sql = sql.Replace(GetParamName("URL", _dataProvider), $"'{leader.URL}'");

                cmd.CommandText = sql;
				
				cmd.ExecuteNonQuery();

				tran.Commit();
			}
			catch (Exception e)
			{
				tran.Rollback();
				throw e;
			}
			finally
			{
				con.Close();
			}
		}

		/// <summary>
		/// Get the appropriate parameter name for the specific database type
		/// </summary>
		/// <param name="name">The bare parameter name.</param>
		/// <param name="dataProvider">The data provider.</param>
		/// <returns>The parameter name</returns>
		private string GetParamName(string name, IDataProvider dataProvider)
		{
			string param;

			switch (dataProvider.DatabaseType)
			{
				case DatabaseType.Oracle:
					param = ":" + name;
					break;
				case DatabaseType.SQLServer:
                case DatabaseType.SQLServerCE:
					param = "@" + name;
					break;
				default:
					param = ":" + name;
					break;
			}

			return param;
		}

        #endregion
    }

    internal class LeaderInfo
    {
        public string ResourceId { get; set; }
        public string OwnerId { get; set; }
        public DateTime Expiration { get; set; }
        public string URL { get; set; }
    }
}