/*
* @(#)BatteryCycleLifeMatFileConverter.cs
*
*/
namespace Newtera.Conveters
{
	using System;
	using System.IO;
	using System.Xml;
	using System.Data;

    using csmatio.io;
    using csmatio.types;

	using Newtera.ParserGen.Converter;

	/// <summary> 
	/// The class implements a converter for battery cycle life data in matlab format.
	/// </summary>
	internal class BatteryCycleLifeMatFileConverter : DataSourceConverterBase
	{
		/// <summary>
		/// Initiate an instance of MatlabFileConverter class
		/// </summary>
		public BatteryCycleLifeMatFileConverter() : base()
		{
		}

		/// <summary>
		/// Initiate an instance of MatlabFileConverter class
		/// </summary>
		/// <param name="name">The parser name</param>
		public BatteryCycleLifeMatFileConverter(string name) : base(name)
		{
		}

		/// <summary>
		/// Overriding the method to implement operation of converting an xml file. 
		/// </summary>
		/// <param name="dataSourceName">The data source name that identifies an xml file</param>
		/// <returns>A DataSet instance that contains DataTable(s) for the converted data.</returns>
		public override DataSet Convert(string dataSourceName)
		{
			// The DataSet to return
			DataSet dataSet = new DataSet(dataSourceName);
            int cycleCount = 0;

            MatFileReader mfr = new MatFileReader(dataSourceName);
            MLStructure mlStruct = null;
            foreach (string key in mfr.Content.Keys)
            {
                mlStruct = mfr.Content[key] as MLStructure;
            }

            if (mlStruct != null)
            {
                DataTable dataTable = dataSet.Tables.Add(mlStruct.Name);

                MLStructure cycleStruct = mlStruct["cycle", 0] as MLStructure;
 
                // add colum name
                dataTable.Columns.Add("Cycle");
                dataTable.Columns.Add("Capacity");
                dataTable.Columns.Add("Voltage_measured");
                dataTable.Columns.Add("Current_measured");
                dataTable.Columns.Add("Temperature_measured");
                dataTable.Columns.Add("Time");
                dataTable.Columns.Add("SampleId");

                string[] items;
                int columnSize = 7;
                for (int i = 0; i < cycleStruct.Size; ++i)
                {
                    MLChar mlType = cycleStruct["type", i] as MLChar;
                    string type = mlType.GetString(0);
                    if (type == "discharge")
                    {
                        MLDouble mlTemp = (MLDouble)cycleStruct["ambient_temperature", i];
                        double temp = mlTemp.GetReal(0);
                        MLDouble mlTime = cycleStruct["time", i] as MLDouble;
                        double[] timeArray = mlTime.GetArray()[0];
                        string timeStamp = "";
                        for (int j = 0; j < timeArray.Length; j++)
                        {
                            timeStamp += timeArray[j] + "-";
                        }

                        MLStructure mlData = cycleStruct["data", i] as MLStructure;
                        items = new string[columnSize];
                        int index = 0;

                        items[index++] = cycleCount.ToString(); // Cycle number

                        try
                        {
                            MLDouble mlCapacity = mlData["Capacity", 0] as MLDouble;

                            if (mlCapacity != null)
                            {
                                double capacity = mlCapacity.GetArray()[0][0];
                                items[index++] = capacity.ToString(); // Capacity
                            }
                        }
                        catch (Exception)
                        {
                        }

                        try
                        {
                            MLDouble mlVoltageMeasured = mlData["Voltage_measured", 0] as MLDouble;
                            double[]  voltageMeasured = mlVoltageMeasured.GetArray()[0];

                            items[index++] = voltageMeasured[voltageMeasured.Length - 1].ToString(); // last voltage measured
                        }
                        catch (Exception)
                        {

                        }

                        try
                        {
                            MLDouble mlCurrentMeasured = mlData["Current_measured", 0] as MLDouble;
                            double[]  currentMeasured = mlCurrentMeasured.GetArray()[0];
                            items[index++] = currentMeasured[currentMeasured.Length - 1].ToString(); // last current measured
                        }
                        catch (Exception)
                        {

                        }

                        try
                        {
                            MLDouble mlTemperatureMeasured = mlData["Temperature_measured", 0] as MLDouble;
                            double[] temperatureMeasured = mlTemperatureMeasured.GetArray()[0];
                            items[index++] = temperatureMeasured[temperatureMeasured.Length - 1].ToString(); // last temparature measured
                        }
                        catch (Exception)
                        {

                        }

                        try
                        {
                            MLDouble mlTimeMeasured = mlData["Time", 0] as MLDouble;
                            double[] timeMeasured = mlTimeMeasured.GetArray()[0];
                            items[index++] = timeMeasured[timeMeasured.Length - 1].ToString(); // last temparature measured
                        }
                        catch (Exception)
                        {
                        }

                        // add battery name as sample id
                        items[index++] = mlStruct.Name;

                        // add a row
                        dataTable.Rows.Add(items);

                        cycleCount++;
                    }
                }
            }


            return dataSet;
		}
	}
}