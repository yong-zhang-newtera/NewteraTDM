/*
* @(#)IImportManager.cs
*
* Copyright (c) 2007 Newtera, Inc. All rights reserved.
*
*/
namespace Newtera.Server.Engine.XmlGenerator
{
	using System;
    using System.Data;
	using System.Collections;

    using Newtera.Common.MetaData.Mappings;

	/// <summary>
	/// Represents an interface for import mamager that imports data from file using mapping package
	/// </summary>
	/// <version> 1.0.0 14 Oct 2014</version>
	public interface IImportManager
	{
        /// <summary>
        /// Convert data in a file to a DataSet object for importing to destinated classes in database.
        /// </summary>
        /// <param name="connectionString">The connection string indicating a database.</param>
        /// <param name="filePath">The full path of a data file to be imported. </param>
        /// <param name="mappingPackage">The mapping package for importing the file.</param>
        /// <param name="importResult">Object that contains any errors during the conversion</param>
        /// <returns>A DataSet for desination classes.</returns>
        /// <remarks>This method does'nt support paging mode of reading data from file</remarks>
        DataSet ConvertToDestinationDataSet(string connectionString, string filePath, MappingPackage mappingPackage);
	}
}