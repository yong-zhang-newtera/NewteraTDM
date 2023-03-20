/*
* @(#)DBDiscrepancyDetector.cs
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
	/// Compare a schema model definitions with its corresponding database definitions
	/// to detect any discrepancies between them. And then create necessary actions
	/// to fix the discrepancies.
	/// </summary>
	/// <version>  	1.0.0 20 Mar 2005 </version>
	/// <author> Yong Zhang</author>
	public class DBDiscrepancyDetector
	{
		private MetaDataModel _metaDataModel;
		private IDataProvider _dataProvider;

		/// <summary>
		/// Initializes a new instance of the DBDiscrepancyDetector class
		/// </summary>
		/// <param name="metaDataModel">The meta data contains the schema model info.</param>
		/// <param name="dataProvider">Data Provider</param>
		public DBDiscrepancyDetector(MetaDataModel metaDataModel,
			IDataProvider dataProvider)
		{
			_metaDataModel = metaDataModel;
			_dataProvider = dataProvider;
		}

		/// <summary>
		/// Detect any discrepancies bewteen the schema model and its corresponding
		/// database, and generate neccessry actions in the compare result that
		/// are intended to fix the discrepancies.
		/// meta data.
		/// </summary>
		/// <returns>The comparison result</returns>
		public MetaDataCompareResult Detect()
		{
			MetaDataCompareResult result = new MetaDataCompareResult(_metaDataModel, null);

			ISchemaModelElementVisitor visitor = new FindDiscrepancyVisitor(_metaDataModel,
				result, _dataProvider);
			_metaDataModel.SchemaModel.Accept(visitor);

			return result;
		}
	}
}