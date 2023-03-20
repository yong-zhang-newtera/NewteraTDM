/*
* @(#) ImportManager.cs
*
* Copyright (c) 2007 Newtera, Inc. All rights reserved.
*
*/
using System;
using System.Xml;
using System.IO;
using System.Data;
using System.Collections;
using System.Text;
using System.Resources;

using Newtera.Common.Core;
using Newtera.Conveters;
using Newtera.Common.MetaData.Mappings;
using Newtera.Common.MetaData.Mappings.Scripts;
using Newtera.Common.MetaData.Schema;
using Newtera.Common.MetaData.DataView;
using Newtera.Common.MetaData.XaclModel;
using Newtera.Common.MetaData.DataView.Validate;
using Newtera.Server.Engine.Cache;
using Newtera.Server.Engine.XmlGenerator;
using Newtera.ParserGen.Converter;
using Newtera.Data;

namespace Newtera.Import
{
    /// <summary>
    /// The main class that provides the server-side data import functions that can be used
    /// by web-based data importing.
    /// </summary>
    /// <version>1.0.0 21 Apr 2007 </version>
    public class ImportManager : IImportManager
    {
        internal const int CHUNK_SIZE = 100;

        private ResourceManager _resources;

        private int _currentBlock = 0;
        IDataSourceConverter _converter = null;

        /// <summary>
		/// Initiating a ImportManager object
		/// </summary>
        public ImportManager()
		{
            _resources = new ResourceManager(this.GetType());
		}

        /// <summary>
        /// Gets mapping packages of a source data type from a database
        /// </summary>
        /// <param name="connectionString">The connection string indicating a database.</param>
        /// <param name="type">One of the DataSourceType enum values</param>
        /// <returns>A collection of MappingPackage objects, it can be an empty collection.</returns>
        public MappingPackageCollection GetMappingPackages(string connectionString, DataSourceType type)
        {
            MappingPackageCollection mappingPackages;

            using (CMConnection connection = new CMConnection(connectionString))
            {
                connection.Open();

                mappingPackages = connection.MetaDataModel.MappingManager.GetMappingPackages(type);

                return mappingPackages;
            }
        }

        /// <summary>
        /// Gets mapping packages of a source data type for a specific destination class in a database
        /// </summary>
        /// <param name="connectionString">The connection string indicating a database.</param>
        /// <param name="type">One of the DataSourceType enum values</param>
        /// <param name="destinationClassName">Name of the destination class.</param>
        /// <returns>A collection of MappingPackage objects, it can be an empty collection.</returns>
        /// <remarks>Return the packages that have only one destination class and the class name is the same as the specified one.</remarks>
        public MappingPackageCollection GetMappingPackagesByClass(string connectionString, DataSourceType type,
            string destinationClassName)
        {
            MappingPackageCollection mappingPackages;

            using (CMConnection connection = new CMConnection(connectionString))
            {
                connection.Open();

                mappingPackages = connection.MetaDataModel.MappingManager.GetMappingPackagesByClass(type, destinationClassName);

                return mappingPackages;
            }
        }

        /// <summary>
        /// Get a mapping package of a given name
        /// </summary>
        /// <param name="connectionString">The connection string indicating a database</param>
        /// <param name="packageName">The package name</param>
        /// <returns>The mapping package for import</returns>
        public MappingPackage GetMappingPackageByName(string connectionString, string packageName)
        {
            MappingPackage mappingPackage = null;

            using (CMConnection connection = new CMConnection(connectionString))
            {
                connection.Open();

                mappingPackage = connection.MetaDataModel.MappingManager.GetMappingPackage(packageName);

                // make a copy of mapping package since there will be some run-time data to
                // be stored in the package
                mappingPackage = (MappingPackage)mappingPackage.Copy();
            }

            return mappingPackage;
        }

        /// <summary>
        /// Perform the action of importing data from a file into a database using the indicated
        /// mapping package.
        /// </summary>
        /// <param name="connectionString">The connection string indicating a database.</param>
        /// <param name="filePath">The full path of a data file to be imported. </param>
        /// <param name="packageName">The mapping package for importing the file.</param>
        /// <returns>A ImportResult that contains any errors or warings that are taken place during the
        /// import process.</returns>
        public ImportResult ImportFile(string connectionString, string filePath, MappingPackage mappingPackage)
        {
            ImportResult importResult = new ImportResult();
            ImportResultEntry entry;

            importResult.MainMessage = _resources.GetString("VerifyImport");

            if (File.Exists(filePath))
            {
                QueryDataCache.Instance.ClearExpiringObjects(); // clear the key-objId cached previously

                using (CMConnection connection = new CMConnection(connectionString))
                {
                    connection.Open();

                    if (mappingPackage != null)
                    {
                        // check user's permissions to write on all of the destination classes
                        // specified in the mapping package
                        if (HasImportPermission(connection, mappingPackage, importResult))
                        {
                            DataSet sourceDataSet;
                            DataSet destinationDataSet;

                            // a big file could be processed in the paging mode, so run
                            // the importing task in a while loop
                            while (true)
                            {
                                // converting data in a file to a DataSet representing the source data
                                sourceDataSet = ConvertToSourceDataSet(filePath,
                                    mappingPackage, importResult);

                                // check if it reaches the end of file in paging mode or
                                // there are errors during converting data in a file to a DataSet
                                if (sourceDataSet == null || importResult.HasError)
                                {
                                    // it could be the end of reading a file or has some errors,
                                    // return the import result
                                    break;
                                }

                                // converting data in the source DataSet to the destination DataSet using
                                // the mapping package
                                destinationDataSet = ConvertToDestinationDataSet(connection, sourceDataSet,
                                    mappingPackage, importResult, null, null);

                                // check if there are errors during converting data from the source to destination
                                if (importResult.HasError)
                                {
                                    // stop, return the import result
                                    break;
                                }

                                // validate destination DataSet
                                ValidateDestinationDataSet(destinationDataSet, mappingPackage, connection, importResult);

                                // check if there are validating errors
                                if (importResult.HasError)
                                {
                                    // stop, return the import result
                                    break;
                                }

                                // import data in the destination DataSet into database
                                PerformImport(destinationDataSet, connection, mappingPackage, importResult);

                                if (importResult.HasError)
                                {
                                    // stop, return the import result
                                    break;
                                }

                                // if it is a paging mode, continue, otherwise, exit the loop
                                if (!(mappingPackage.IsPaging && _converter.SupportPaging))
                                {
                                    // not paging mode, exit the loop
                                    break;
                                }
                            }

                            // free the resource
                            if (_converter != null)
                            {
                                _converter.Close();
                                _converter = null;
                            }
                        }
                    }
                }
            }
            else
            {
                // the import file does not exist
                entry = new ImportResultEntry(_resources.GetString("InvalidFilePath"), filePath);
                importResult.AddError(entry);
            }

            QueryDataCache.Instance.ClearExpiringObjects(); // clear the key-objId cached during import

            return importResult;
        }

        /// <summary>
        /// Perform the action of importing data from a Dataset into a database using the indicated
        /// mapping package.
        /// </summary>
        /// <param name="connectionString">The connection string indicating a database.</param>
        /// <param name="sourceDataSet">The dataset contains source data to be imported. </param>
        /// <param name="mapingPackage">The mapping package for importing the file.</param>
        /// <param name="overridingValues">The overriding attribute values of destination class</param>
        /// <param name="overridingRelationshipValues">The overriding relationhsip values of destination class</param>
        /// <returns>A ImportResult that contains any errors or warings that are taken place during the
        /// import process.</returns>
        public ImportResult ImportDataSet(string connectionString, DataSet sourceDataSet, MappingPackage mappingPackage,
            DefaultValueCollection overridingValues, DefaultValueCollection overridingRelationshipValues)
        {
            ImportResult importResult = new ImportResult();

            importResult.MainMessage = _resources.GetString("VerifyImport");

            using (CMConnection connection = new CMConnection(connectionString))
            {
                connection.Open();

                QueryDataCache.Instance.ClearExpiringObjects(); // clear the key-objId cached previously

                if (mappingPackage != null)
                {
                    // check user's permissions to write on all of the destination classes
                    // specified in the mapping package
                    if (HasImportPermission(connection, mappingPackage, importResult))
                    {
                        DataSet destinationDataSet;

                        // converting data in the source DataSet to the destination DataSet using
                        // the mapping package
                        destinationDataSet = ConvertToDestinationDataSet(connection, sourceDataSet,
                            mappingPackage, importResult, overridingValues, overridingRelationshipValues);

                        // check if there are errors during converting data from the source to destination
                        if (importResult.HasError)
                        {
                            return importResult;
                        }

                        // validate destination DataSet
                        ValidateDestinationDataSet(destinationDataSet, mappingPackage, connection, importResult);

                        // check if there are validating errors
                        if (importResult.HasError)
                        {
                            return importResult;
                        }

                        // import data in the destination DataSet into database
                        PerformImport(destinationDataSet, connection, mappingPackage, importResult);
                    }
                }
            }

            QueryDataCache.Instance.ClearExpiringObjects(); // clear the key-objId cached during import

            return importResult;
        }

        /// <summary>
        /// Convert data in a file to a DataSet object for importing to destinated classes in database.
        /// </summary>
        /// <param name="connectionString">The connection string indicating a database.</param>
        /// <param name="filePath">The full path of a data file to be imported. </param>
        /// <param name="mappingPackage">The mapping package for importing the file.</param>
        /// <param name="importResult">Object that contains any errors during the conversion</param>
        /// <returns>A DataSet for desination classes.</returns>
        /// <remarks>This method does'nt support paging mode of reading data from file</remarks>
        public DataSet ConvertToDestinationDataSet(string connectionString, string filePath, MappingPackage mappingPackage)
        {
            ImportResult importResult = new ImportResult();
            return ConvertToDestinationDataSet(connectionString, filePath, mappingPackage, importResult);
        }

        /// <summary>
        /// Convert data in a file to a DataSet object for importing to destinated classes in database.
        /// </summary>
        /// <param name="connectionString">The connection string indicating a database.</param>
        /// <param name="filePath">The full path of a data file to be imported. </param>
        /// <param name="mappingPackage">The mapping package for importing the file.</param>
        /// <param name="importResult">Object that contains any errors during the conversion</param>
        /// <returns>A DataSet for desination classes.</returns>
        /// <remarks>This method does'nt support paging mode of reading data from file</remarks>
        public DataSet ConvertToDestinationDataSet(string connectionString, string filePath, MappingPackage mappingPackage, ImportResult importResult)
        {
            ImportResultEntry entry;
            DataSet destinationDataSet = null;

            importResult.MainMessage = _resources.GetString("VerifyImport");

            if (File.Exists(filePath))
            {
                using (CMConnection connection = new CMConnection(connectionString))
                {
                    connection.Open();

                    if (mappingPackage != null)
                    {
                        // check user's permissions to write on all of the destination classes
                        // specified in the mapping package
                        if (HasImportPermission(connection, mappingPackage, importResult))
                        {
                            DataSet sourceDataSet;

                            // converting data in a file to a DataSet representing the source data
                            sourceDataSet = ConvertToSourceDataSet(filePath,
                                mappingPackage, importResult);

                            // check if it reaches the end of file in paging mode or
                            // there are errors during converting data in a file to a DataSet
                            if (sourceDataSet != null && !importResult.HasError)
                            {

                                // converting data in the source DataSet to the destination DataSet using
                                // the mapping package
                                destinationDataSet = ConvertToDestinationDataSet(connection, sourceDataSet,
                                    mappingPackage, importResult, null, null);

                                // validate destination DataSet
                                ValidateDestinationDataSet(destinationDataSet, mappingPackage, connection, importResult);

                                // check if there are validating errors
                                if (importResult.HasError)
                                {
                                    destinationDataSet = null;
                                }
                            }

                            // free the resource
                            if (_converter != null)
                            {
                                _converter.Close();
                                _converter = null;
                            }
                        }
                    }
                }
            }
            else
            {
                // the import file does not exist
                entry = new ImportResultEntry(_resources.GetString("InvalidFilePath"), filePath);
                importResult.AddError(entry);
            }

            return destinationDataSet;
        }

        /// <summary>
        /// Convert data in a file to a DataSet object using the converter specified in a mapping package.
        /// </summary>
        /// <param name="filePath">The full file path</param>
        /// <param name="mappingPackage">The mapping package</param>
        /// <param name="importResult">The import result object</param>
        /// <returns>The converted DataSet, null if there are any errors taking place.</returns>
        public DataSet ConvertToSourceDataSet(string filePath, MappingPackage mappingPackage, ImportResult importResult)
        {
            DataSet dataSet = null;
            ImportResultEntry entry;

            importResult.MainMessage = _resources.GetString("ConvertSource");

            if (mappingPackage.ConverterTypeName != null)
            {
                if (_converter == null)
                {
                    string assemblyDir = GetConverterAssemblyDir();

                    // create the specified converter from the windows client directory
                    _converter = ConverterFactory.Instance.Create(mappingPackage.ConverterTypeName, assemblyDir);
                    _currentBlock = 0;

                    if (_converter == null)
                    {
                        // failed to create the specified converter
                        entry = new ImportResultEntry(_resources.GetString("CreateConverterFailed"), mappingPackage.Name);
                        importResult.AddError(entry);

                        return null;
                    }
                    else if (_converter is DelimitedTextFileConverter)
                    {
                        ((DelimitedTextFileConverter)_converter).RowDelimiter = mappingPackage.TextFormat.RowDelimiter;
                        ((DelimitedTextFileConverter)_converter).ColumnDelimiter = mappingPackage.TextFormat.ColumnDelimiter;
                        ((DelimitedTextFileConverter)_converter).IsFirstRowColumns = mappingPackage.TextFormat.IsFirstRowColumns;
                        ((DelimitedTextFileConverter)_converter).StartingDataRow = mappingPackage.TextFormat.StartingDataRow;
                    }
                }

                if (mappingPackage.IsPaging && _converter.SupportPaging)
                {
                    // import the file in paging mode
                    if (_currentBlock == 0)
                    {
                        // the first block
                        dataSet = _converter.ConvertFirstPage(filePath, mappingPackage.BlockSize);
                    }
                    else
                    {
                        // the rest of blocks
                        dataSet = _converter.ConvertNextPage();
                    }

                    if (dataSet == null)
                    {
                        // reached the end of the file, return null
                        return null;
                    }

                    _currentBlock++;
                }
                else
                {
                    // convert a file as a whole
                    dataSet = _converter.Convert(filePath);
                }

                if (dataSet.Tables.Count == 0 || dataSet.Tables[0].Columns.Count == 0)
                {
                    // failed to convert a file
                    entry = new ImportResultEntry(_resources.GetString("ConvertFileFailed"), mappingPackage.Name);
                    importResult.AddError(entry);

                    return dataSet;
                }
            }
            else
            {
                // the converter name not specified
                entry = new ImportResultEntry(_resources.GetString("InvalidConverter"), mappingPackage.Name);
                importResult.AddError(entry);
            }

            return dataSet;
        }

        /// <summary>
        /// Convert data a DataSet object representing source data into a DataSet object representing
        /// destination data using the mappings specified in a mapping package.
        /// </summary>
        /// <param name="sourceDataSet">a DataSet object representing source data</param>
        /// <param name="mappingPackage">The mapping package</param>
        /// <param name="importResult">The import result object</param>
        /// <param name="overridingValues">The overriding values for destination class</param>
        /// <param name="overridingRelationshipValues">The overriding relationship values for destination class</param>
        /// <returns>a DataSet object representing
        /// destination data, null if there are any errors taking place.</returns>
        private DataSet ConvertToDestinationDataSet(CMConnection connection,
            DataSet sourceDataSet, MappingPackage mappingPackage, ImportResult importResult,
            DefaultValueCollection overridingValues, DefaultValueCollection overridingRelationshipValues)
        {
            DataSet destinationDataSet = null;
            ImportResultEntry entry;

            importResult.MainMessage = _resources.GetString("ConvertDestination");

            string libPath = GetConverterAssemblyDir();

            try
            {
                // gets the detailed data view for the destination class in each of
                // the class mappings
                foreach (ClassMapping classMapping in mappingPackage.CheckedClassMappings)
                {
                    classMapping.DestinationDataView = connection.MetaDataModel.GetDetailedDataView(classMapping.DestinationClassName);
                    if (overridingValues != null)
                    {
                        DefaultValueCollection defaultValues = new DefaultValueCollection();
                        foreach (DefaultValue defaultValue in overridingValues)
                        {
                            if (defaultValue.DestinationClassName == classMapping.DestinationClassName)
                            {
                                defaultValues.Add(defaultValue);
                            }
                        }

                        classMapping.OverridingDefaultValues = defaultValues;
                    }

                    if (overridingRelationshipValues != null)
                    {
                        foreach (DefaultValue defaultValue in overridingRelationshipValues)
                        {
                            if (defaultValue.DestinationClassName == classMapping.DestinationClassName)
                            {
                                classMapping.OverrideRelationshipDefaultValue(defaultValue.DestinationAttributeName, defaultValue.Value);
                            }
                        }
                    }

                    // add missing columns to the data table in case the source file contains a subset
                    // of the parameters required by the mapping.
                    AddMissingColumns(classMapping, sourceDataSet);
                }

                destinationDataSet = mappingPackage.Transform(sourceDataSet, libPath);
            }
            catch (Exception ex)
            {
                // the transform failed
                entry = new ImportResultEntry(_resources.GetString("TransformFailed") + ex.Message, mappingPackage.Name);
                importResult.AddError(entry);
            }

            return destinationDataSet;
        }

        /// <summary>
        /// Add the missing columns to the DataTable as specified in the class mapping
        /// </summary>
        /// <param name="classMapping">The class mapping</param>
        /// <param name="srcDataSet">The source dataset</param>
        /// <returns>True if there are missing columns, false, otherwise</returns>
        private bool AddMissingColumns(ClassMapping classMapping, DataSet srcDataSet)
        {
            bool status = false;
            DataTable srcTable = srcDataSet.Tables[classMapping.SourceClassIndex];

            if (srcTable != null)
            {
                foreach (IMappingNode mapping in classMapping.AttributeMappings)
                {
                    switch (mapping.NodeType)
                    {
                        case Newtera.Common.MetaData.Mappings.NodeType.ArrayDataCellMapping:
                        case Newtera.Common.MetaData.Mappings.NodeType.AttributeMapping:
                        case Newtera.Common.MetaData.Mappings.NodeType.PrimaryKeyMapping:

                            AttributeMapping attrMapping = (AttributeMapping)mapping;

                            if (srcTable.Columns[attrMapping.SourceAttributeName] == null)
                            {
                                srcTable.Columns.Add(attrMapping.SourceAttributeName);
                                status = true;
                            }

                            break;

                        case Newtera.Common.MetaData.Mappings.NodeType.OneToManyMapping:
                        case Newtera.Common.MetaData.Mappings.NodeType.ManyToOneMapping:
                        case Newtera.Common.MetaData.Mappings.NodeType.ManyToManyMapping:

                            MultiAttributeMappingBase multiMapping = (MultiAttributeMappingBase)mapping;

                            foreach (InputOutputAttribute inputAttribute in multiMapping.InputAttributes)
                            {
                                if (srcTable.Columns[inputAttribute.AttributeName] == null)
                                {
                                    srcTable.Columns.Add(inputAttribute.AttributeName);
                                    status = true;
                                }
                            }

                            break;
                    }
                }
            }

            return status;
        }

        /// <summary>
        /// Validate a DataSet object representing destination data using the validating rules of
        /// a database schema.
        /// </summary>
        /// <param name="dataSet">a DataSet object representing destination data</param>
        /// <param name="connection">A connection to the database</param>
        /// <param name="importResult">The import result object</param>
        private void ValidateDestinationDataSet(DataSet dataSet, MappingPackage mappingPackage, CMConnection connection, ImportResult importResult)
        {
            ImportResultEntry entry;
            int instanceCount;
            InstanceData instanceData;

            importResult.MainMessage = _resources.GetString("ValidatingData");

            foreach (ClassMapping classMapping in mappingPackage.CheckedClassMappings)
            {
                instanceCount = dataSet.Tables[classMapping.DestinationClassName].Rows.Count;

                instanceData = new InstanceData(classMapping.DestinationDataView,
                    dataSet, true);

                // validate the instances in the destination class
                for (int i = 0; i < instanceCount; i++)
                {
                    // setting the row index will cause the instance data for
                    // the row copied to the result attributes of the data view model
                    instanceData.RowIndex = i;

                    DataViewValidateResult result = classMapping.DestinationDataView.ValidateData();

                    // verify the doubts here
                    if (result.HasDoubt)
                    {
                        // turn the doubts into errors if any doubts turn out to be errors
                        //ValidateDoubts(result, classMapping.DestinationDataView, connection);
                    }

                    if (result.HasError)
                    {
                        foreach (DataValidateResultEntry validateResult in result.Errors)
                        {
                            // convert the result entry type
                            entry = new ImportResultEntry(validateResult.Message, validateResult.Source);
                            importResult.AddError(entry);
                        }
                    }
                }
            }
        }

        /// <summary>
        /// Perform importing data in a DataSet object representing destination data into
        /// the database.
        /// </summary>
        /// <param name="dataSet">a DataSet object representing destination data</param>
        /// <param name="connection">A connection to the database</param>
        /// <param name="mappingPackage">The mapping package</param>
        /// <param name="importResult">The import result object</param>
        private void PerformImport(DataSet dataSet, CMConnection connection, MappingPackage mappingPackage, ImportResult importResult)
        {
            ImportResultEntry entry;
            importResult.MainMessage = _resources.GetString("ImportData");
            bool generateUpdateQuery = mappingPackage.ModifyExistingInstances;
            InstanceIdentifier identifier = mappingPackage.InstanceIdentifier;

            ScriptManager scriptManager = new ScriptManager();
            StringBuilder builder = null;

            CMCommand command = connection.CreateCommand();

            foreach (ClassMapping classMapping in mappingPackage.CheckedClassMappings)
            {
                int currentChunkIndex = 0;
                int actual;

                while ((actual = GenerateScripts(dataSet, scriptManager, classMapping, currentChunkIndex, generateUpdateQuery, identifier)) > 0)
                {
                    // convert the import scripts to a xml string to be sent to
                    // the server
                    builder = new StringBuilder();
                    StringWriter writer = new StringWriter(builder);
                    scriptManager.Write(writer);

                    // run the import script using the CMCommand
                    string xml = command.ExecuteScripts(builder.ToString(), _currentBlock + currentChunkIndex);

                    // read the xml that contains script execution status and detail messages
                    StringReader reader = new StringReader(xml);
                    ScriptManager scriptResult = new ScriptManager();
                    scriptResult.Read(reader);

                    // If there are errors, stop the importing process
                    if (!scriptResult.IsSucceeded)
                    {
                        // build import error entries for errors
                        foreach (ClassScript classScript in scriptResult.ClassScripts)
                        {
                            int row = 1;

                            foreach (InstanceScript instanceScript in classScript.InstanceScripts)
                            {
                                if (!instanceScript.IsSucceeded)
                                {
                                    entry = new ImportResultEntry(instanceScript.Message, _resources.GetString("Row") + row.ToString());
                                    importResult.AddError(entry);
                                }

                                row++;
                            }
                        }

                        break;
                    }

                    currentChunkIndex++; // increase to next chunk
                    scriptManager = new ScriptManager();
                }
            }
        }

        /// <summary>
        /// Generate data import scripts for the given class and given chunk index
        /// </summary>
        /// <param name="scriptManager">The script manager</param>
        /// <param name="classMapping">The class mapping</param>
        /// <returns>Actual number of rows that have been generated in scripts</returns>
        private int GenerateScripts(DataSet dataSet, ScriptManager scriptManager, ClassMapping classMapping, int currentChunkIndex, bool generateUpdateQuery,
            InstanceIdentifier instanceIdentifier)
        {
            ClassScript classScript;
            InstanceScript instanceScript;
            string query;

            int instanceCount = GetInstanceCount(classMapping.DestinationClassName, dataSet);
            int start = currentChunkIndex * ImportManager.CHUNK_SIZE;

            InstanceData instanceData = new InstanceData(classMapping.DestinationDataView,
                dataSet, true);

            classScript = new ClassScript(classMapping.DestinationClassName);
            scriptManager.AddClassScript(classScript);

            int end = start + ImportManager.CHUNK_SIZE;
            if (end > instanceCount)
            {
                end = instanceCount;
            }

            for (int i = start; i < end; i++)
            {
                // Set the row index to InstanceData instance will cause
                // it to copy values of the DataRow of the DataSet to the
                // contained DataViewModel instance
                instanceData.RowIndex = i;

                instanceScript = new InstanceScript();

                // build the insert query using the DataViewModel instance
                query = classMapping.DestinationDataView.InsertQuery;
                instanceScript.InsertQuery = query;

                if (generateUpdateQuery)
                {
                    if (instanceIdentifier == InstanceIdentifier.PrimaryKeys)
                    {
                        // build the search query that can retrieve the instance
                        // based on primary key(s)
                        query = classMapping.DestinationDataView.GetInstanceByPKQuery();
                    }
                    else
                    {
                        // build the search query that can retrieve the instance
                        // based on unique key(s)
                        query = classMapping.DestinationDataView.GetInstanceByUniqueKeysQuery(classMapping.DestinationDataView.BaseClass.ClassName);
                    }

                    instanceScript.SearchQuery = query;

                    // build the update query using the DataViewModel instance
                    // since the obj_id is unknown at this time, we will place
                    // a variable @obj_id in the update query for the time being,
                    // the variable will be replaced at server side with an obj_id
                    // retrieve by the GetInstanceByPKQuery.
                    classMapping.DestinationDataView.CurrentObjId = InstanceScript.OBJ_ID;
                    query = classMapping.DestinationDataView.UpdateQuery;
                    instanceScript.UpdateQuery = query;

                }

                classScript.InstanceScripts.Add(instanceScript);
            }

            return (end - start);
        }

        /// <summary>
        /// Gets number of instances in a DataSet for a given class.
        /// </summary>
        /// <param name="className">The class name</param>
        /// <param name="dataSet">The DataSet instance</param>
        /// <returns>The number of instances</returns>
        private int GetInstanceCount(string className, DataSet dataSet)
        {
            DataTable dataTable = dataSet.Tables[className];

            if (dataTable != null)
            {
                return dataTable.Rows.Count;
            }
            else
            {
                return 0;
            }
        }

        /// <summary>
        /// Gets the information indicating whether the user has permission to import data
        /// to all of the destination classes in the mapping package.
        /// </summary>
        /// <param name="connection">The CMConnection</param>
        /// <param name="mappingPackage">The mapping package.</param>
        /// <param name="importResult">The import result to put errors</param>
        /// <returns>true if user has permission to import data to all of the destination classes in the mapping package,
        /// false if the user doesn't have permission to import data to some of the destination classes in the mapping package.</returns>
        private bool HasImportPermission(CMConnection connection,
            MappingPackage mappingPackage, ImportResult importResult)
        {
            bool status = true;
            ClassElement destinationClassElement;
            ImportResultEntry entry;

            foreach (ClassMapping classMapping in mappingPackage.CheckedClassMappings)
            {
                destinationClassElement = connection.MetaDataModel.SchemaModel.FindClass(classMapping.DestinationClassName);
                if (destinationClassElement != null)
                {
                    if (!PermissionChecker.Instance.HasPermission(connection.MetaDataModel.XaclPolicy,
                            destinationClassElement, XaclActionType.Create))
                    {
                        entry = new ImportResultEntry(_resources.GetString("NoCreatePermission"), destinationClassElement.Caption);
                        importResult.AddError(entry);
                        status = false;
                    }
                }
                else
                {
                    entry = new ImportResultEntry(_resources.GetString("UnknownDestinationClass"), classMapping.DestinationClassName);
                    importResult.AddError(entry);
                    status = false;
                    break;
                }
            }

            return status;
        }

        /// <summary>
        /// A utility that get the directory where the converter assemblies reside.
        /// </summary>
        /// <returns>A directory string</returns>
        private string GetConverterAssemblyDir()
        {
            string dir = NewteraNameSpace.GetAppToolDir();

            if (!dir.EndsWith(@"\"))
            {
                dir += @"\bin\";
            }
            else
            {
                dir += @"bin\";
            }

            // test if it is in production mode or debug mode
            if (!File.Exists(dir + @"Newtera.Common.dll"))
            {
                // it is the debug mode, add Debug directory at the end of dir
                dir += @"Debug\";
            }

            return dir;
        }

        /// <summary>
        /// validate the doubts raised by validating data process
        /// </summary>
        /// <param name="validateResult"></param>
        /// <param name="destinationDataView"></param>
        /// <param name="connection">The database connection to run the validating query</param>
        private void ValidateDoubts(DataViewValidateResult validateResult, DataViewModel destinationDataView,
            CMConnection connection)
        {
            foreach (DataValidateResultEntry doubt in validateResult.Doubts)
            {
                if (doubt.EntryType == Newtera.Common.MetaData.DataView.Validate.EntryType.PrimaryKey)
                {
                    if (IsPKValueExists(destinationDataView, connection))
                    {
                        // the primary key value exists, change the doubt into error
                        doubt.EntryType = Newtera.Common.MetaData.DataView.Validate.EntryType.Error;
                        validateResult.AddError(doubt);
                    }
                }
                else if (doubt.EntryType == Newtera.Common.MetaData.DataView.Validate.EntryType.UniqueValue)
                {
                    if (!IsValueUnique(destinationDataView, connection))
                    {
                        // the value isn't unique, change the doubt into error
                        doubt.EntryType = Newtera.Common.MetaData.DataView.Validate.EntryType.Error;
                        validateResult.AddError(doubt);
                    }
                }
                else if (doubt.EntryType == Newtera.Common.MetaData.DataView.Validate.EntryType.UniqueValues)
                {
                    if (!IsCombinedValuesUnique(destinationDataView, doubt.ClassName, connection))
                    {
                        // the combination of values isn't unique, change the doubt into error
                        doubt.EntryType = Newtera.Common.MetaData.DataView.Validate.EntryType.Error;
                        validateResult.AddError(doubt);
                    }
                }
            }
        }

        /// <summary>
        /// Gets the information indicating whether the primary key value are already used by
        /// another instance in the database
        /// </summary>
        /// <param name="destinationDataView">The data view</param>
        /// <param name="connection">The database connection</param>
        /// <returns>True if it's been used, false otherwise</returns>
        private bool IsPKValueExists(DataViewModel destinationDataView, CMConnection connection)
        {
            bool status = false;

            string query = destinationDataView.GetInstanceByPKQuery();
            if (query != null)
            {
                CMCommand cmd = connection.CreateCommand();
                cmd.CommandText = query;

                XmlReader reader = cmd.ExecuteXMLReader();
                DataSet ds = new DataSet();
                ds.ReadXml(reader);

                // if the result isn't empty, the instance with the same primary key value exists
                if (!DataSetHelper.IsEmptyDataSet(ds, destinationDataView.BaseClass.ClassName))
                {
                    status = true;
                }
            }

            return status;
        }

        /// <summary>
        /// Gets the information indicating whether the indivated value are unique among the
        /// same class.
        /// </summary>
        /// <param name="destinationDataView">The data view</param>
        /// <param name="connection">The database connection</param>
        /// <returns>True if it is unque, false otherwise</returns>
        private bool IsValueUnique(DataViewModel destinationDataView, CMConnection connection)
        {
            bool status = true;

            // to be implemented

            return status;
        }

        /// <summary>
        /// Gets the information indicating whether a combination of values is unique among the
        /// same class.
        /// </summary>
        /// <param name="destinationDataView">The data view</param>
        /// <param name="className">The name of the class uniquely constrainted.</param>
        /// <param name="connection">The database connection</param>
        /// <returns>True if it is unque, false otherwise</returns>
        private bool IsCombinedValuesUnique(DataViewModel destinationDataView, string className, CMConnection connection)
        {
            bool status = true;

            string query = destinationDataView.GetInstanceByUniqueKeysQuery(className);
            if (query != null)
            {
                CMCommand cmd = connection.CreateCommand();
                cmd.CommandText = query;

                XmlReader reader = cmd.ExecuteXMLReader();
                DataSet ds = new DataSet();
                ds.ReadXml(reader);

                // if the result isn't empty, the instance with the same primary key value exists
                if (!DataSetHelper.IsEmptyDataSet(ds, destinationDataView.BaseClass.ClassName))
                {
                    status = false;
                }
            }

            return status;
        }
    }
}
