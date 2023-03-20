/*
* @(#)DeleteImageAttributeAction.cs
*
* Copyright (c) 2008 Newtera, Inc. All rights reserved.
*
*/
namespace Newtera.Server.DB.MetaData
{
	using System;
	using System.Data;
    using System.IO;

    using Newtera.Common.Core;
	using Newtera.Common.MetaData;
	using Newtera.Common.MetaData.Schema;
	using Newtera.Server.DB;

	/// <summary>
	/// Delete an image attribute from the database. Action of deleting an image attribute is
    /// the same as that of deleting a simple attribute
	/// </summary>
	/// <version> 1.0.0 04 Jul 2008 </version>
	public class DeleteImageAttributeAction : MetaDataActionBase
	{
		/// <summary>
		/// Instantiate an instance of DeleteImageAttributeAction
		/// </summary>
		/// <param name="metaDataModel">The meta data model of the action</param>
		public DeleteImageAttributeAction(MetaDataModel metaDataModel,
			SchemaModelElement element,
			IDataProvider dataProvider) : base(metaDataModel, element, dataProvider)
		{
		}

		/// <summary>
		/// Gets the action type
		/// </summary>
		/// <value>One of MetaDataActionType values</value>
		public override MetaDataActionType ActionType
		{
			get
			{
				return MetaDataActionType.DeleteImageAttribute;
			}
		}

		/// <summary>
		/// Prepare the action for deleting a simple attribute from database
		/// </summary>
		/// <param name="con">Database connection</param>
		/// <param name="trans">The transaction object</param>
		public override void Prepare(IDbConnection con, IDbTransaction trans) 
		{
            IDbCommand cmd = con.CreateCommand();
            cmd.Transaction = trans;

            string sql = CannedSQLManager.GetCannedSQLManager(_dataProvider).GetSql("DelSimpleAttributeDML");
            DMLGenerator dmlGenerator = new DMLGenerator(_dataProvider);
            sql = dmlGenerator.GetDelSimpleAttributeDML(sql, SchemaModelElement.ID);

            cmd.CommandText = sql;

            if (_log != null)
            {
                _log.Append(sql, LogType.DML);
            }

            cmd.ExecuteNonQuery();

            sql = CannedSQLManager.GetCannedSQLManager(_dataProvider).GetSql("DelAttributeDML");
            sql = dmlGenerator.GetDelAttributeDML(sql, SchemaModelElement.ID);

            cmd.CommandText = sql;

            if (_log != null)
            {
                _log.Append(sql, LogType.DML);
            }

            cmd.ExecuteNonQuery();
		}

		/// <summary>
		/// Peform the action of deleting a simple attribute from database
		/// </summary>
		/// <param name="con">The database connection for executing the action</param>
		public override void Do(IDbConnection con)
		{
            ImageAttributeElement attribute = (ImageAttributeElement)SchemaModelElement;

            if (!SkipDDLExecution)
            {
                IDbCommand cmd = con.CreateCommand();
                IDDLGenerator generator = DDLGeneratorManager.Instance.GetDDLGenerator(_dataProvider);

                string ddl = generator.GetDelColumnDDL(attribute.OwnerClass.TableName,
                    attribute.ColumnName);

                cmd.CommandText = ddl;

                if (_log != null)
                {
                    _log.Append(ddl, LogType.DDL);
                }

                cmd.ExecuteNonQuery();
            }

            // Delete all the images associated with the attribute
            // image file name has pattern of class-attribte-objId.*
            string pattern = attribute.OwnerClass.Name + "-" + attribute.Name + "-*.*";
            // Note, unlike attachment file, image files are stored at base dir
            string[] fileNames = Directory.GetFiles(NewteraNameSpace.GetAttachmentDir(),
                pattern);
            foreach (string fileName in fileNames)
            {
                try
                {
                    File.Delete(fileName);
                }
                catch (Exception)
                {
                }
            }
		}
	}
}