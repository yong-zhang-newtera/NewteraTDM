/*
* @(#)GetInsertSQLActionsExecutor.cs
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
    using System.Text;
	using System.Collections.Specialized;
	using System.Data;
    using System.Resources;

    using Newtera.Common.Core;
	using Newtera.Server.Engine.Vdom;
	using Newtera.Common.MetaData;
	using Newtera.Common.MetaData.Schema;
	using Newtera.Server.Engine.Sqlbuilder;
	using Newtera.Server.Engine.Sqlbuilder.Sql;
	using Newtera.Server.Engine.Interpreter;
	using Newtera.Server.DB;

	/// <summary>
	/// This class creates a collection of SQL Actions for inserting a data instance, but it
    /// doesn't execute the actions, therefore, cause no changes to the database. The SQL Actions
    /// will be used as templates for inserting other data instances to the database.
	/// </summary>
	/// <version>  	1.0.0 03 Aug 2008 </version>
	public class GetInsertSQLActionsExecutor : Executor
	{
		private KeyGenerator _idGenerator; // the obj id generator
		private Interpreter _interpreter;
        private VDocument _doc;

		/// <summary>
		/// Initiating an instance of GetInsertSQLActionsExecutor class
		/// </summary>
		/// <param name="metaData">the meta data model </param>
		/// <param name="dataProvider">the database provider</param>
		/// <param name="builder">the sql builder </param>
		/// <param name="doc">the document to which to insert the instance</param>
		/// <param name="interpreter">the interpreter to run the xqueries. </param>
		public GetInsertSQLActionsExecutor(MetaDataModel metaData, IDataProvider dataProvider, SQLBuilder builder, VDocument doc, Interpreter interpreter):base(metaData, dataProvider, builder)
		{
			_idGenerator = KeyGeneratorFactory.Instance.Create(KeyGeneratorType.ObjId, metaData.SchemaInfo);
			_interpreter = interpreter;
            _doc = doc;
		}
		
		/// <summary>
		/// Gets SQL Actions created for inserting a data instance
		/// </summary>
		/// <param name="instanceNodes">the instances to be inserted.</param>
		/// <returns>
		/// A collection of SQLAction objects
		/// </returns>
        public SQLActionCollection Execute(ValueCollection instanceNodes)
		{
            SQLActionCollection sqlActions = null;
			ClassEntity baseClass;
			
            if (instanceNodes.Count > 0)
            {
                XNode node = (XNode) instanceNodes[0];
				Instance instance = new Instance((XmlElement) node.ToNode());
				
				baseClass = CreateFullBlownClass(instance);
				
				// Assign obj_id as a variable
                instance.ObjId = GetVariable(NewteraNameSpace.OBJ_ID);

                /*
                 * mark the relationships with variables in the SQL so that they can be replaced
                 * with obj_id(s) of referenced instances when execution take place
                 */
                DBEntityCollection relationships = baseClass.InheritedRelationships;
                if (relationships != null)
                {
                    for (int i = 0; i < relationships.Count; i++)
                    {
                        RelationshipEntity relationship = (RelationshipEntity)relationships[i];

                        // do this only for the forward relationship
                        if (relationship.Direction == RelationshipDirection.Forward)
                        {
                            instance.SetReferencedObjId(relationship.Name, GetVariable(relationship.Name));
                        }
                    }
                }

                StringBuilder actionData = new StringBuilder();

                // generate sql templates
                sqlActions = _builder.GenerateInserts(baseClass, instance, true, actionData);

                string sql;
                foreach (SQLAction action in sqlActions)
                {
                    if (action.Type == SQLActionType.Insert)
                    {
                        sql = action.Statement.ToSQL();
                        action.SQLTemplate = sql;
                    }
                }
			}

            return sqlActions;
		}

        private string GetVariable(string attributeName)
        {
            return "{" + attributeName + "}";
        }
	}
}