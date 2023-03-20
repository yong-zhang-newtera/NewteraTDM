/*
* @(#)MetaDataComparator.cs
*
* Copyright (c) 2003 Newtera, Inc. All rights reserved.
*
*/
namespace Newtera.Server.DB.MetaData
{
	using System;
	using Newtera.Common.MetaData;
	using Newtera.Common.MetaData.Schema;
	using Newtera.Server.DB;

	/// <summary>
	/// Compare two MetaDataModel instances and return a comparng result that
	/// stores differences between the MetaDataModel instances. The result is then
	/// used to determine the necessary updating actions to the meta data stored
	/// in the database.
	/// </summary>
	/// <version>  	1.0.1 16 Jan 2013 </version>
	public class MetaDataComparator
	{
		private IDataProvider _dataProvider;
        private bool _nonConditionalCompare;

		/// <summary>
		/// Initializes a new instance of the MetaDataComparator class
		/// </summary>
		/// <param name="dataProvider">Data Provider</param>
		public MetaDataComparator(IDataProvider dataProvider)
		{
			_dataProvider = dataProvider;
            _nonConditionalCompare = true;
		}

        /// <summary>
        /// Initializes a new instance of the MetaDataComparator class
        /// </summary>
        /// <param name="dataProvider">Data Provider</param>
        /// <param name="isNonconditional">True indicating compare all classes between two meta datas, false compare the classes elements that are marked as NeedToAlter </param>
        public MetaDataComparator(IDataProvider dataProvider, bool isNonconditional)
        {
            _dataProvider = dataProvider;
            _nonConditionalCompare = isNonconditional;
        }

		/// <summary>
		/// Compare two MetaDataModel instances and return the comparison results
		/// containing the actions to sync with meta data in database with the new
		/// meta data.
		/// </summary>
		/// <param name="newMetaDataModel">The new meta data</param>
		/// <param name="oldMetaDataModel">The old meta data</param>
		/// <param name="type">Type of the meta data to be compared</param>
		/// <returns>The comparison result</returns>
		public MetaDataCompareResult Compare(MetaDataModel newMetaDataModel,
			MetaDataModel oldMetaDataModel, MetaDataType type)
		{
			MetaDataCompareResult result = new MetaDataCompareResult(newMetaDataModel, oldMetaDataModel);

			if (oldMetaDataModel == null)
			{
				// the new schema is a brand new one, all the elements are considered
				// to be added ones
				ISchemaModelElementVisitor visitor = new FindAdditionVisitor(newMetaDataModel,
					oldMetaDataModel, result, _dataProvider);
				newMetaDataModel.SchemaModel.Accept(visitor);
			}
			else if (newMetaDataModel == null)
			{
				// delete the old schema, all the elements in the schema are
				// considered to be deleted ones
				ISchemaModelElementVisitor visitor = new FindDeletionVisitor(newMetaDataModel,
					oldMetaDataModel, result, _dataProvider);
				oldMetaDataModel.SchemaModel.Accept(visitor);
			}
			else
			{
				// Find added meta model elements and add corresponding actions to the result
				ISchemaModelElementVisitor visitor = new FindAdditionVisitor(newMetaDataModel,
                    oldMetaDataModel, result, _dataProvider, _nonConditionalCompare);
				newMetaDataModel.SchemaModel.Accept(visitor);

				// Find deleted meta model elements and add corresponding actions to the result
				visitor = new FindDeletionVisitor(newMetaDataModel,
                    oldMetaDataModel, result, _dataProvider, _nonConditionalCompare);
				oldMetaDataModel.SchemaModel.Accept(visitor);

				// Find those elements in the new meta data that do not have ID filled
				// but they have the corresponding elements in the old mete data that
				// have IDs. Then copy the IDs from elements in the old schema to
				// those in the new one. This is to handle the situation when a 
				// meta data coming from a meta data editor tool where the meta data
				// has not been refreshed. This step has to be performed before
				// performing the next step of finding alteration.
                visitor = new FindMissingIDVisitor(newMetaDataModel, oldMetaDataModel, _dataProvider);
				newMetaDataModel.SchemaModel.Accept(visitor);

				// Find altered meta model elements and add corresponding actions to the result
				visitor = new FindAlterationVisitor(newMetaDataModel,
                    oldMetaDataModel, result, _dataProvider, _nonConditionalCompare);
				newMetaDataModel.SchemaModel.Accept(visitor);
			}

			return result;
		}
	}
}