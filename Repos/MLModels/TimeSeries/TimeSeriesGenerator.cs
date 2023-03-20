/*
* @(#)TimeSeriesGenerator.cs
*
* Copyright (c) 2016 Newtera, Inc. All rights reserved.
*
*/
namespace Newtera.MLServer.TimeSeries
{
	using System;
	using System.Xml;
    using System.Data;
    using System.Text;
    using System.IO;
    using System.Linq;
    using System.Xml.Linq;
    using System.Threading;
    using System.Collections;
    using System.Collections.Specialized;
    using System.Collections.Generic;
    using System.Runtime.Remoting;

    using Deedle;
    using Newtonsoft.Json;

    using Newtera.Common.Core;
    using Newtera.Common.MetaData;
	using Newtera.Common.MetaData.XMLSchemaView;
    using Newtera.Common.MetaData.DataView;
    using Newtera.Server.Engine.XmlGenerator;

    /// <summary>
    /// Generate a XmlDocument for the data instance(s) based on a given XMLSchemaModel object using the mappings defined for the classes involved
    /// </summary>
    /// <version>  	1.0.0 27 Aug 2014 </version>
    public class TimeSeriesGenerator
    {
        public static string TIME_SERIES_FILE = "timeseries.csv";
        public static string TIME_SERIES_LOG_FILE = "log.txt";
        private const string TEST_ITEM_FILE_BASE_DIR_ATTRIBUTE = "fileBaseDir";
        private XmlDocGenerator _xmlDocGenerator;
        private XMLSchemaModel _xmlSchemaModel;
        private string _fileBaseDir;

        /// <summary>
        /// Initiating an instance of TimeSeriesGenerator
        /// </summary>
        /// <param name="metaData">The meta data model</param>
        /// <param name="connectionStr">The connection string for the db</param>
        /// <param name="xmlSchemaModel">The xml schema model</param>
        /// <param name="baseInstanceView">The root instance data view</param>
        /// <param name="baseInstanceCount">The root instance count</param>
        public TimeSeriesGenerator(MetaDataModel metaData, string connectionStr, XMLSchemaModel xmlSchemaModel, InstanceView baseInstanceView, int baseInstanceCount)
        {
            this._xmlSchemaModel = xmlSchemaModel;
            this._fileBaseDir = GetUserFilePath(baseInstanceView, TEST_ITEM_FILE_BASE_DIR_ATTRIBUTE);

            this._xmlDocGenerator = new XmlDocGenerator(metaData, connectionStr, xmlSchemaModel, baseInstanceView, baseInstanceCount);
        }

        public DataTable GetTimeSeries()
        {
            return GetTimeSeries(null, null, TimeSeriesFrequency.None, TimeSeriesOperation.None, -1, 0, false); // get all time series
        }

        public DataTable GetTimeSeries(string category, string fieldName, TimeSeriesFrequency frequency, TimeSeriesOperation op, int startRow, int pageSize, bool reload)
        {
            try
            {
                if (IsCachedTimeSeriesOutdated() || reload)
                {
                    BuildCachedTimeSeries();
                }

                return LoadTimeSeries(category, fieldName, frequency, op, startRow, pageSize);
            }
            catch (Exception ex)
            {
                ErrorLog.Instance.WriteLine(ex.Message + @"\n" + ex.StackTrace);
                DataTable dt = new DataTable();

                return dt;
            }
        }

        public IList<string> GetTimeSeriesCategories()
        {
            try
            {
                if (IsCachedTimeSeriesOutdated())
                {
                    BuildCachedTimeSeries();
                }

                return GetCategories();
            }
            catch (Exception ex)
            {
                ErrorLog.Instance.WriteLine(ex.Message + @"\n" + ex.StackTrace);
                return null;
            }
        }

        public int GetTimeSeriesPointCount(string category)
        {
            try
            {
                if (IsCachedTimeSeriesOutdated())
                {
                    BuildCachedTimeSeries();
                }

                return GetCount(category);
            }
            catch (Exception ex)
            {
                ErrorLog.Instance.WriteLine(ex.Message + @"\n" + ex.StackTrace);
                return 0;
            }
        }

        private bool IsCachedTimeSeriesOutdated()
        {
            bool status = false;
            string cachedFile = Path.Combine(_fileBaseDir, TimeSeriesGenerator.TIME_SERIES_FILE);
            string logFile = Path.Combine(_fileBaseDir, TimeSeriesGenerator.TIME_SERIES_LOG_FILE);
            if (File.Exists(cachedFile))
            {
                // go through the files in the directory to make sure the cached file contains contents of all data files
                Dictionary<string, string> logDictionary = ReadLogs();

                string[] dataFiles = Directory.GetFiles(_fileBaseDir, "*.*", SearchOption.AllDirectories);
                // check if a file has been added
                foreach (string dataFile in dataFiles)
                {
                    if (dataFile != cachedFile &&
                        dataFile != logFile &&
                        !logDictionary.ContainsKey(dataFile))
                    {
                        status = true; // the log doesn't contains the file, time series is outdated
                        break;
                    }
                }

                // check if a file has been deleted
                foreach (string key in logDictionary.Keys)
                {
                    if (!dataFiles.Contains<string>(key))
                    {
                        status = true; // a file has been deleted
                        break;
                    }
                }
            }
            else
            {
                status = true; // no cached file
            }

            return status;
        }

        private void BuildCachedTimeSeries()
        {
            string cachedFile = Path.Combine(_fileBaseDir, TimeSeriesGenerator.TIME_SERIES_FILE);
            if (File.Exists(cachedFile))
            {
                File.Delete(cachedFile);
            }

            XMLSchemaElement xAxisElement = this._xmlSchemaModel.GetXAxisElement();

            XmlDocument doc = this._xmlDocGenerator.Create();

            if (doc != null &&
                doc.DocumentElement != null &&
                doc.DocumentElement.ChildNodes.Count > 0)
            {

                var df = GetDataFrame(doc); // convert to Deedle Data Frame

                if (df != null)
                {
                    FileStream fileStream = new FileStream(cachedFile, FileMode.Create, FileAccess.Write, FileShare.None);
                    using (StreamWriter outputFile = new StreamWriter(fileStream, Encoding.UTF8))
                    {
                        if (xAxisElement != null && IsDateTimeType(xAxisElement))
                        {
                            var sortedDf = df.IndexRows<DateTime>(xAxisElement.Caption).SortRowsByKey();

                            WriteHeader(outputFile, sortedDf, xAxisElement.Caption);
                            WriteRows(outputFile, sortedDf);
                        }
                        else
                        {
                            WriteHeader(outputFile, df);
                            WriteRows(outputFile, df);
                        }
                    }


                    WriteLogs(); // remember the data files in a log
                }
            }
        }

        private Frame<int, string> GetDataFrame(XmlDocument doc)
        {
            XmlDocument flattenXmlDoc = new XmlDocument();
            XmlElement root = flattenXmlDoc.CreateElement("Records");
            flattenXmlDoc.AppendChild(root);

            // get a flatten xml document
            List<KeyValuePair<int, String>> result = FlattenXML(doc.DocumentElement, "", 0);
            var records = result.Where(c => c.Key == result.Max(b => b.Key)).Select(b => b.Value.Substring(0, b.Value.Length - 1)).ToArray();

            if (records != null && records.Length > 0)
            {
                // convert to deedle data frame
                var rows = new List<KeyValuePair<int, Series<string, object>>>();
                for (int i = 0; i < records.Length; i++)
                {
                    // Build each row using series builder & return 
                    // KeyValue representing row key with row data
                    string recordStr = records[i];
                    string[] valuePairs = recordStr.Split(';');

                    var sb = new SeriesBuilder<string>();

                    for (int j = 0; j < valuePairs.Length; j++)
                    {
                        string valuePair = valuePairs[j];
                        int pos = valuePair.IndexOf(':');
                        if (pos > 0)
                        {
                            string name = valuePair.Substring(0, pos);
                            string val = valuePair.Substring(pos + 1);

                            sb.Add(name, val);
                        }
                    }

                    rows.Add(KeyValue.Create(i, sb.Series));
                }

                // Turn sequence of row information into data frame
                var df = Frame.FromRows(rows);

                return df;
            }
            else
            {
                return null;
            }
        }

        private List<KeyValuePair<int, String>> FlattenXML(XmlElement node, String parent, int level)
        {
            List<KeyValuePair<int, String>> result = new List<KeyValuePair<int, String>>();
            String detail = "";

            if (node.InnerText == node.InnerXml && node.InnerText != "")
            {
                // xmldocgenerator has encoded the element name, we need to deconde it
                parent += XmlConvert.DecodeName(node.Name) + ":" + node.InnerText + ";";
            }

            foreach (XmlElement child in node.ChildNodes)
            {
                if (child.InnerText == child.InnerXml && child.InnerText != "")
                {
                    detail += XmlConvert.DecodeName(child.Name) + ":" + child.InnerText + ";";
                    level++;
                }

                if (child.FirstChild != child.LastChild)
                {
                    List<KeyValuePair<int, String>> childResult = FlattenXML(child, parent + detail, level);
                    result.AddRange(childResult);
                }
            }
            result.Add(new KeyValuePair<int, String>(level, parent + detail));
            return result;
        }

        private void WriteHeader(StreamWriter outputFile, Frame<DateTime, string> df, string keyColumnName)
        {
            StringBuilder sb = new StringBuilder();

            sb.Append(keyColumnName);

            for (int i = 0; i < df.ColumnCount; i++)
            {
                if (sb.Length > 0)
                {
                    sb.Append(',');
                }

                sb.Append(df.Columns.GetKeyAt(i));
            }

            outputFile.WriteLine(sb.ToString());
        }

        private void WriteHeader(StreamWriter outputFile, Frame<int, string> df)
        {
            StringBuilder sb = new StringBuilder();

            for (int i = 0; i < df.ColumnCount; i++)
            {
                if (sb.Length > 0)
                {
                    sb.Append(',');
                }

                sb.Append(df.Columns.GetKeyAt(i));
            }

            outputFile.WriteLine(sb.ToString());
        }

        private void WriteRows(StreamWriter outputFile, Frame<DateTime, string> df)
        {
            StringBuilder sb;
            foreach (var row in df.Rows.ObservationsAll)
            {
                sb = new StringBuilder();
                sb.Append(row.Key.ToString());
                for (int j = 0; j < df.ColumnCount; j++)
                {
                    if (sb.Length > 0)
                    {
                        sb.Append(',');
                    }

                    if (row.Value.Value != null &&
                        row.Value.Value.GetAt(j) != null)
                    {
                        sb.Append(row.Value.Value.GetAt(j).ToString());
                    }
                    else
                    {
                        sb.Append("");
                    }
                }

                outputFile.WriteLine(sb.ToString());
            }
        }

        private void WriteRows(StreamWriter outputFile, Frame<int, string> df)
        {
            StringBuilder sb;
            foreach (var row in df.Rows.ObservationsAll)
            {
                sb = new StringBuilder();
                
                for (int j = 0; j < df.ColumnCount; j++)
                {
                    if (sb.Length > 0)
                    {
                        sb.Append(',');
                    }

                    if (row.Value.Value != null &&
                        row.Value.Value.GetAt(j) != null)
                    {
                        sb.Append(row.Value.Value.GetAt(j).ToString());
                    }
                    else
                    {
                        sb.Append("");
                    }
                }

                outputFile.WriteLine(sb.ToString());

            }
        }

        private Dictionary<string, string> ReadLogs()
        {
            string logFile = Path.Combine(_fileBaseDir, TimeSeriesGenerator.TIME_SERIES_LOG_FILE);
            Dictionary<string, string> logDictionary = new Dictionary<string, string>();

            if (File.Exists(logFile))
            {
                using (StreamReader inputFile = new StreamReader(logFile, Encoding.UTF8))
                {
                    // build a log file
                    string line;
                    while ((line = inputFile.ReadLine()) != null)
                    {
                        int pos = line.IndexOf(',');
                        string key = line.Substring(0, pos);
                        string val = line.Substring(pos + 1);

                        logDictionary.Add(key, val);
                    }
                }
            }

            return logDictionary;
        }

        private void WriteLogs()
        {
            string logFile = Path.Combine(_fileBaseDir, TimeSeriesGenerator.TIME_SERIES_LOG_FILE);
            string cachedFilePath = Path.Combine(_fileBaseDir, TimeSeriesGenerator.TIME_SERIES_FILE);

            if (File.Exists(logFile))
            {
                File.Delete(logFile);
            }

            FileStream fileStream = new FileStream(logFile, FileMode.Create, FileAccess.Write, FileShare.None);
            using (StreamWriter outputFile = new StreamWriter(fileStream, Encoding.UTF8))
            {
                // build a log file
                string[] dataFiles = Directory.GetFiles(_fileBaseDir, "*.*", SearchOption.AllDirectories);
                foreach (string dataFile in dataFiles)
                {
                    if (dataFile != cachedFilePath &&
                        dataFile != logFile)
                    {
                        outputFile.WriteLine(dataFile + "," + "yes");
                    }
                }
            }
        }

        private DataTable LoadTimeSeries(string category, string fieldName, TimeSeriesFrequency frequency, TimeSeriesOperation op, int startRow, int pageSize)
        {
            string cachedFilePath = Path.Combine(_fileBaseDir, TimeSeriesGenerator.TIME_SERIES_FILE);
            if (File.Exists(cachedFilePath))
            {
                var dataFrame = Frame.ReadCsv(cachedFilePath);

                if (!string.IsNullOrEmpty(category))
                {
                    dataFrame = GetFilteredDataFrame(dataFrame, category);
                }

                if (string.IsNullOrEmpty(fieldName))
                {
                    // get all time series from the file
                    if (startRow >= 0)
                    {
                        if (dataFrame.RowCount >= startRow + pageSize)
                        {
                            dataFrame = dataFrame.GetAddressRange(RangeRestriction<long>.NewFixed(startRow, startRow + pageSize - 1));
                        }
                        else
                        {
                            dataFrame = dataFrame.GetAddressRange(RangeRestriction<long>.NewFixed(startRow, dataFrame.RowCount - 1));
                        }
                    }

                    DataTable dt = dataFrame.ToDataTable(new string[1] { "Index" });

                    // fix the row index to start from 1
                    dt = FixRowsIndex(dt, startRow);

                    return dt;
                }
                else
                {
                    // get the specified time series
                    DataTable dt = new DataTable();
                    dt.Columns.Add("Index");
                    dt.Columns.Add(fieldName);
 
                    XMLSchemaElement xAxisElement = this._xmlSchemaModel.GetXAxisElement();
                    if (xAxisElement != null &&
                        IsDateTimeType(xAxisElement))
                    {
                        // resample the time series based on the frequency
                        var timeIndexedDf = dataFrame.IndexRows<DateTime>(xAxisElement.Caption).SortRowsByKey();

                        Series<DateTime, double> timeIndexedSeries = timeIndexedDf.GetColumn<double>(fieldName);
                        // has time axis
                        Series<string, double> resampledSeries;
                        string grouping = "yyyy-MM-dd HH:mm:ss";
                        switch (frequency)
                        {
                            case TimeSeriesFrequency.Second:

                                grouping = "yyyy-MM-dd HH:mm:ss";

                                break;

                            case TimeSeriesFrequency.Minute:

                                grouping = "yyyy-MM-dd hh:mm";

                                break;

                            case TimeSeriesFrequency.Hour:

                                grouping = "yyyy-MM-dd hh";

                                break;

                            case TimeSeriesFrequency.Day:

                                grouping = "yyyy-MM-dd";

                                break;

                            case TimeSeriesFrequency.Month:

                                grouping = "yyyy-MM";

                                break;

                            default:

                                // second
                                grouping = "yyyy-MM-dd HH:mm:ss";
                                break;

                        }

                        switch (op)
                        {
                            case TimeSeriesOperation.Min:

                                resampledSeries = timeIndexedSeries.ResampleEquivalence(x => x.ToString(grouping), s => s.Min());

                                break;

                            case TimeSeriesOperation.Max:

                                resampledSeries = timeIndexedSeries.ResampleEquivalence(x => x.ToString(grouping), s => s.Max());

                                break;

                            case TimeSeriesOperation.Median:

                                resampledSeries = timeIndexedSeries.ResampleEquivalence(x => x.ToString(grouping), s => s.Median());

                                break;

                            case TimeSeriesOperation.FirstValue:

                                resampledSeries = timeIndexedSeries.ResampleEquivalence(x => x.ToString(grouping), s => s.FirstValue());

                                break;

                            case TimeSeriesOperation.LastValue:

                                resampledSeries = timeIndexedSeries.ResampleEquivalence(x => x.ToString(grouping), s => s.LastValue());

                                break;

                            case TimeSeriesOperation.Mean:

                                resampledSeries = timeIndexedSeries.ResampleEquivalence(x => x.ToString(grouping), s => s.Mean());

                                break;

                            default:

                                // second
                                resampledSeries = timeIndexedSeries.ResampleEquivalence(x => x.ToString(grouping), s => s.Median());

                                break;

                        }

                        resampledSeries = resampledSeries.SortByKey();

                        foreach (KeyValuePair<string, double> kvp in resampledSeries.Observations)
                        {
                            DataRow row = dt.NewRow();
                            row[0] = kvp.Key;
                            row[1] = kvp.Value;
                            dt.Rows.Add(row);
                        }
                    }
                    else
                    {
                        // no time axis
                        Series<int, string> series = dataFrame.GetColumn<string>(fieldName);
                        int index = 0;
                        foreach (KeyValuePair<int, string> kvp in series.Observations)
                        {
                            DataRow row = dt.NewRow();
                            row[0] = (index++).ToString();
                            row[1] = kvp.Value;
                            dt.Rows.Add(row);
                        }
                    }

                    return dt;
                }
            }
            else
            {
                return null; 
            }
        }

        private int GetCount(string category)
        {
            string cachedFilePath = Path.Combine(_fileBaseDir, TimeSeriesGenerator.TIME_SERIES_FILE);
            if (File.Exists(cachedFilePath))
            {
                var dataFrame = Frame.ReadCsv(cachedFilePath);

                if (!string.IsNullOrEmpty(category))
                {
                    dataFrame = GetFilteredDataFrame(dataFrame, category);
                }

                return dataFrame.RowCount;
            }
            else
            {
                return 0;
            }
        }

        private DataTable FixRowsIndex(DataTable dt, int from)
        {
            for (int i = 0; i < dt.Rows.Count; i++)
            {
                dt.Rows[i]["Index"] = i + from +  1;
            }
            return dt;
        }

        private IList<string> GetCategories()
        {
            XMLSchemaElement cAxisElement = this._xmlSchemaModel.GetCategoryAxisElement();
            if (cAxisElement != null)
            {
                string cachedFilePath = Path.Combine(_fileBaseDir, TimeSeriesGenerator.TIME_SERIES_FILE);
                if (File.Exists(cachedFilePath))
                {
                    Hashtable existings = new Hashtable();
                    List<string> categories = new List<string>();

                    var dataFrame = Frame.ReadCsv(cachedFilePath);

                    IEnumerable<string> values = dataFrame.Rows.Select(kvp => kvp.Value.GetAs<string>(cAxisElement.Caption)).Values;

                    foreach (string category in values)
                    {
                        if (existings[category] == null)
                        {
                            categories.Add(category);
                            existings[category] = "1";
                        }
                    }

                    return categories;
                }
                else
                {
                    throw new Exception("Unable to find file at " + cachedFilePath);
                }
            }
            else
            {
                return null;
            }
        }

        private Frame<int, string> GetFilteredDataFrame(Frame<int, string> df, string category)
        {
            XMLSchemaElement cAxisElement = this._xmlSchemaModel.GetCategoryAxisElement();
            if (cAxisElement != null)
            {
                var categoryRows = df.Rows.Where(kvp =>
                      kvp.Value.GetAs<string>(cAxisElement.Caption) == category);

                // Transform row collection into a new data frame
                var categoryFrame = Frame.FromRows(categoryRows);
                return categoryFrame;
            }
            else
            {
                return df;
            }
        }

        private string GetUserFilePath(InstanceView baseInstanceView, string baseDirAttribute)
        {
            string path = NewteraNameSpace.GetUserFilesDir(); ;

            string itemFilePath = baseInstanceView.InstanceData.GetAttributeStringValue(baseDirAttribute);
            if (!string.IsNullOrEmpty(itemFilePath) && itemFilePath.Length > 0)
            {
                // fix the path
                if (itemFilePath.StartsWith(@"\"))
                {
                    itemFilePath = itemFilePath.Substring(1); // remove the \ at beginning since the base path ends with one
                }

                if (!itemFilePath.EndsWith(@"\"))
                {
                    itemFilePath += @"\";
                }

                path = path + itemFilePath;
            }
            else
            {
                path = null;
            }

            return path;
        }

        private bool IsDateTimeType(XMLSchemaElement xAxisElement)
        {
            bool status = false;

            if (!string.IsNullOrEmpty(xAxisElement.ElementType) &&
                (xAxisElement.ElementType == "date" ||
                xAxisElement.ElementType == "dateTime"))
            {
                status = true;
            }

            return status;
        }
    }
}