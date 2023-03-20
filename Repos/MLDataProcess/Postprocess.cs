using System;
using System.IO;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MLTimeSeriesProcess
{
    public class Postprocess
    {
        public string Run(string forecatFilePath, string evalFilePath, string outputFilePath)
        {
            // forecast and eval files are inputs to the post processing
            List<List<string>> forecastSeriesList = ReadSpaceSeparatedFile(forecatFilePath);
            List<List<string>> evalSeriesList = ReadSpaceSeparatedFile(evalFilePath);

            return GenerateOutputFile(forecastSeriesList, evalSeriesList, outputFilePath);
        }

        private List<List<string>> ReadSpaceSeparatedFile(string inputFilePath)
        {
            List<List<string>> seriesList = new List<List<string>>();

            if (File.Exists(inputFilePath))
            {
                using (StreamReader inputFile = new StreamReader(inputFilePath, Encoding.UTF8))
                {
                    // Assuming each line is a series in format of item1 item2 .... itemN
                    string line;
                    List<string> series;
                    while ((line = inputFile.ReadLine()) != null)
                    {
                        series = new List<string>();
                        string[] items = line.Split(' ');
                        
                        foreach (string item in items)
                        {
                            series.Add(item);
                        }

                        seriesList.Add(series);
                    }
                }
            }
            else
            {
                throw new Exception(inputFilePath + " doesn't exist.");
            }

            return seriesList;
        }

        private string GenerateOutputFile(List<List<string>> forecastSeriesList, List<List<string>> evalSeriesList,
            string outputFilePath)
        {
            if (File.Exists(outputFilePath))
            {
                File.Delete(outputFilePath);
            }

            StreamWriter sw;
            int rows = forecastSeriesList.Count; // rows of forecast file and eval file, both of them should have same length
            List<string> forecastSeries, evalSeries;
            double level;
            string tsNum;
            string currentTSNum = null;
            List<string> lastForecastSeries = null;
            double lastLevel = 0.0;
            using (sw = new StreamWriter(outputFilePath))
            {
                for (int i = 0; i < rows; i++)
                {
                    forecastSeries = forecastSeriesList[i];
                    evalSeries = evalSeriesList[i];

                    // eval series is in format of {num}|features data1 data2 ... dataN |# level
                    // the last item in the eval series is the level
                    int pos = evalSeries[0].IndexOf('|');
                    tsNum = evalSeries[0].Substring(0, pos); // time series seq number
                    level = double.Parse(evalSeries.Last<string>());

                    if (string.IsNullOrEmpty(currentTSNum))
                    {
                        currentTSNum = tsNum;
                    }

                    // only write out the last forecast record of a time series
                    if (tsNum != currentTSNum)
                    {
                        WriteForecast(sw, currentTSNum, lastForecastSeries, lastLevel);
                        currentTSNum = tsNum;
                    }
                    else
                    {
                        lastForecastSeries = forecastSeries;
                        lastLevel = level;
                    }
                }

                // write last record
                WriteForecast(sw, currentTSNum, lastForecastSeries, lastLevel);

                sw.Flush();
            }

            return "";
        }

        private void WriteForecast(StreamWriter sw, string currentTSNum, List<string> lastForecast, double lastLevel)
        {
            Console.WriteLine(currentTSNum);

            StringBuilder sb = new StringBuilder();

            List<double> denormalizedSeries = new List<double>();
            foreach (string val in lastForecast)
            {
                double number = double.Parse(val) + lastLevel;
                denormalizedSeries.Add(number);
            }

            ApplyExpFunction(denormalizedSeries);

            sb.Append("ts" + currentTSNum);
            
            foreach (double number in denormalizedSeries)
            {
                sb.Append(" " + number);
            }

            sw.WriteLine(sb.ToString());
        }

        private void ApplyExpFunction(List<double> denormalizedSeries)
        {
            for (int i = 0; i <  denormalizedSeries.Count; i++)
            {
                denormalizedSeries[i] = Math.Exp(denormalizedSeries[i]);
            }
        }
    }
}
