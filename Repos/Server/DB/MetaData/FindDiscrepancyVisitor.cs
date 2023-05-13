/*
* @(#)FindDiscrepancyVisitor.cs
*
* Copyright (c) 2005 Newtera, Inc. All rights reserved.
*
*/
namespace Newtera.Server.DB.MetaData
{
	using System;
	using System.Data;

	using Newtera.Common.MetaData;
	using Newtera.Common.MetaData.Schema;
	using Newtera.Server.DB;

	/// <summary>
	/// Traverse the elements in schema model and find any discrepancies
	/// between the elements and its database elements, and create corresponding
	/// actions in the result to fix the discrepancies.
	/// </summary>
	/// <version> 1.0.0 20 Mar 2005 </version>
	/// <author> Yong Zhang</author>
	public class FindDiscrepancyVisitor : ISchemaModelElementVisitor
	{
		private MetaDataModel _metaDataModel;
		private MetaDataCompareResult _result;
		private IDataProvider _dataProvider;
		private DataSet _dataSet;

		/// <summary>
		/// Instantiate an instance of FindDiscrepancyVisitor class
		/// </summary>
		/// <param name="metaDataModel">The meta data model</param>
		/// <param name="result">The compare result</param>
		/// <param name="dataProvider">The DataProvider.</param>
		public FindDiscrepancyVisitor(MetaDataModel metaDataModel,
			MetaDataCompareResult result,
			IDataProvider dataProvider)
		{
			_metaDataModel = metaDataModel;
			_result = result;
			_dataProvider = dataProvider;
			_dataSet = new DataSet();
		}

		/// <summary>
		/// Viste a class element.
		/// </summary>
		/// <param name="element">A ClassElement instance</param>
		public void VisitClassElement(ClassElement element)
		{
			if (element.TableName != null && element.TableName.Length > 0)
			{
				IDbConnection con = _dataProvider.Connection;

				try
				{
					string sql = "select * from " + element.TableName;
				
					IDbCommand cmd = con.CreateCommand();
					cmd.CommandText = sql;

					IDataAdapter adapter = _dataProvider.GetDataAdapter(cmd);

					adapter.FillSchema(_dataSet, SchemaType.Source);

					// adapter add a DataTable called "Table" to the dataset,
					// in order to add multiple tables for all classes in
					// the schema, we have to rename the DataTable with
					// the class name.
					_dataSet.Tables["Table"].TableName = element.TableName;
				}
				catch (Exception)
				{
				}
				finally
				{
					con.Close();
				}
			}
		}

		/// <summary>
		/// Viste a simple attribute element.
		/// </summary>
		/// <param name="element">A SimpleAttributeElement instance</param>
		public void VisitSimpleAttributeElement(SimpleAttributeElement element)
		{
			if (element.ColumnName != null &&
				element.ColumnName.Length > 0 &&
				_dataSet.Tables[element.OwnerClass.TableName] != null)
			{
				// check if the DB column for the element exists in the
				// corresponding table
				DataTable dataTable = _dataSet.Tables[element.OwnerClass.TableName];
				if (dataTable.Columns[element.ColumnName] == null)
				{
					// the corresponding column does not exist,
					// add it the list of an action that creates a DB column
					IMetaDataAction action = new AddSimpleAttributeAction(_metaDataModel, element, _dataProvider);
					_result.AddAddSimpleAttributeAction(action);

					if (element.IsUnique && !element.IsPrimaryKey)
					{
						// when the element is part of primary key, it's been uniquely
						// constrainted.
						action = new AddSimpleAttributeUniqueConstraintAction(_metaDataModel, element, _dataProvider);
						_result.AddAlterSimpleAttributeAction(action);
					}

                    if (element.IsAutoIncrement && !element.HasCustomValueGenerator)
					{
						action = new AddSimpleAttributeAutoIncrementAction(_metaDataModel, element, _dataProvider);
						_result.AddAlterSimpleAttributeAction(action);
					}

                    if (element.IsHistoryEdit && element.DataType == DataType.Text)
                    {
                        // History edit works on the attribute of Text type
                        action = new AddSimpleAttributeHistoryEditAction(_metaDataModel, element, _dataProvider);
                        _result.AddAlterSimpleAttributeAction(action);
                    }

                    if (element.IsRichText && element.DataType == DataType.Text)
                    {
                        // rich text works on the attribute of Text type
                        action = new AddSimpleAttributeRichTextAction(_metaDataModel, element, _dataProvider);
                        _result.AddAlterSimpleAttributeAction(action);
                    }

                    if (element.IsIndexed && !element.IsUnique && !element.IsPrimaryKey)
					{
						// when a element is unique or primary key, it will be indexed automatically
						action = new AddSimpleAttributeIndexAction(_metaDataModel, element, _dataProvider);
						_result.AddAlterSimpleAttributeAction(action);
					}
				}
                else if (element.IsAutoIncrement && !element.HasCustomValueGenerator)
				{
					bool createSequence = false;
					bool createTrigger = false;
					int sequenceStart = 1;
					IDDLGenerator generator = DDLGeneratorManager.Instance.GetDDLGenerator(_dataProvider);

					IDbConnection con = _dataProvider.Connection;
					IDataReader reader = null;
					IDbCommand cmd = con.CreateCommand();

					try
					{
						// check if the sequence exists, if not, create an sequence
						string sequenceName = DBNameComposer.GetSequenceName(element.OwnerClass, element);

						string sql = generator.GetFindSequenceSQL(sequenceName);

						if (sql != null)
						{
							cmd.CommandText = sql;

							reader = cmd.ExecuteReader();

							if (!reader.Read())
							{
								// the sequence does not exist
								createSequence = true;

								// createing a new sequence with the starting number
								// greater than the max of existing values
								sequenceStart = GetSequenceStart(element, cmd);
							}

							reader.Close();
							reader = null;
						}

						// check if the trigger exists, if not, create an trigger
						string triggerName = DBNameComposer.GetTriggerName(element.OwnerClass, element);
					
						sql = generator.GetFindTriggerSQL(triggerName);
						if (sql != null)
						{
							cmd.CommandText = sql;

							reader = cmd.ExecuteReader();

							if (!reader.Read())
							{
								// the trigger does not exist
								createTrigger = true;
							}

							reader.Close();
							reader = null;
						}

						if (createSequence || createTrigger)
						{
							// the trigger does not exist, create an action to create a trigger
							IMetaDataAction action = new AddSimpleAttributeAutoIncrementAction(_metaDataModel, element, _dataProvider, createSequence, createTrigger, sequenceStart);
							_result.AddAlterSimpleAttributeAction(action);
						}
					}
					catch (Exception)
					{
					}
					finally
					{
						if (reader != null && !reader.IsClosed)
						{
							reader.Close();
						}

						con.Close();
					}
				}
			}
		}

		/// <summary>
		/// Viste a relationship attribute element.
		/// </summary>
		/// <param name="element">A RelationshipAttributeElement instance</param>
		public void VisitRelationshipAttributeElement(RelationshipAttributeElement element)
		{
            // check if the DB column for the element exists in the
            // corresponding table
            if (element.ColumnName != null &&
                element.ColumnName.Length > 0 &&
                _dataSet.Tables[element.OwnerClass.TableName] != null)
            {
                if (element.IsForeignKeyRequired && element.IsIndexed)
                {
                    // Add a index if the relationship is many-to-one
                    // or one-to-one and not join manager
                    IMetaDataAction action = new AddRelationshipAttributeIndexAction(_metaDataModel, element, _dataProvider);
                    _result.AddAlterRelationshipAttributeAction(action);
                }
            }
		}

		/// <summary>
		/// Viste an array attribute element.
		/// </summary>
		/// <param name="element">A ArrayAttributeElement instance</param>
		public void VisitArrayAttributeElement(ArrayAttributeElement element)
		{
			// check if the DB column for the element exists in the
			// corresponding table
			if (element.ColumnName != null &&
				element.ColumnName.Length > 0 &&
				_dataSet.Tables[element.OwnerClass.TableName] != null)
			{
				// check if the DB column for the element exists in the
				// corresponding table
				DataTable dataTable = _dataSet.Tables[element.OwnerClass.TableName];
				if (dataTable.Columns[element.ColumnName] == null)
				{
					// the corresponding column does not exist,
					// add it the list of an action that creates a DB column
					IMetaDataAction action = new AddArrayAttributeAction(_metaDataModel, element, _dataProvider);
					_result.AddAddArrayAttributeAction(action);
				}
			}
		}

        /// <summary>
        /// Viste a virtual attribute element.
        /// </summary>
        /// <param name="element">A VirtualAttributeElement instance</param>
        public void VisitVirtualAttributeElement(VirtualAttributeElement element)
        {
        }

        /// <summary>
        /// Viste an image attribute element.
        /// </summary>
        /// <param name="element">A ImageAttributeElement instance</param>
        public void VisitImageAttributeElement(ImageAttributeElement element)
        {
        }

        /// <summary>
        /// Viste an custom page element.
        /// </summary>
        /// <param name="element">A CustomPageElement instance</param>
        public void VisitCustomPageElement(CustomPageElement element)
        {
        }

		/// <summary>
		/// Viste a schema info element.
		/// </summary>
		/// <param name="element">A SchemaInfoElement instance</param>
		public void VisitSchemaInfoElement(SchemaInfoElement element)
		{
		}

		/// <summary>
		/// Viste an enum constraint element.
		/// </summary>
		/// <param name="element">A EnumElement instance</param>
		public void VisitEnumElement(EnumElement element)
		{
		}

		/// <summary>
		/// Viste a range constraint element.
		/// </summary>
		/// <param name="element">A RangeElement instance</param>
		public void VisitRangeElement(RangeElement element)
		{
		}

		/// <summary>
		/// Viste a pattern constraint element.
		/// </summary>
		/// <param name="element">A PatternElement instance</param>
		public void VisitPatternElement(PatternElement element)
		{
		}

		/// <summary>
		/// Viste a list constraint element.
		/// </summary>
		/// <param name="element">A ListElement instance</param>
		public void VisitListElement(ListElement element)
		{
		}

		/// <summary>
		/// Gets the starting number for a sequence that is greater than
		/// the max of existing values of the given attribute
		/// </summary>
		/// <param name="element">The simple attribute element</param>
		/// <returns>The sequence starting number.</returns>
		private int GetSequenceStart(SimpleAttributeElement element, IDbCommand cmd)
		{
			int sequenceStart = 1;

			cmd.CommandText = "SELECT MAX(" + element.ColumnName +  ") FROM " + element.OwnerClass.TableName;

			try
			{
				sequenceStart = System.Convert.ToInt32(cmd.ExecuteScalar().ToString());

				sequenceStart++;
			}
			catch (Exception e)
			{
				sequenceStart = 1;

				throw e;
			}

			return sequenceStart;
		}
	}
}