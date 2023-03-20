using System;
using System.IO;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MLTimeSeriesProcess
{
    public class Preprocess
    {
        private string frequency;
        private int inputSize;
        private int maxForecastHorizon;

        private static readonly string[] DataTypes = { "TRAIN", "TEST", "EVAL"};

        public string Run(string inputFilePath, string trainFilePath, string testFilePath, string evalFilePath, int featureDim, int labelDim)
        {
            string msg = "ok";

            this.inputSize = featureDim;
            this.maxForecastHorizon = labelDim;

            Dictionary<string, List<double>> timeSeriesDictionary = ReadTimeSeries(inputFilePath);

            ApplyLogFunction(timeSeriesDictionary);

            // generate files for deep learning model
            foreach (string dataType in DataTypes)
            {
                string outputFile = null;
                switch (dataType)
                {
                    case "TRAIN":
                        outputFile = trainFilePath;
                        break;

                    case "TEST":
                        outputFile = testFilePath;
                        break;

                    case "EVAL":
                        outputFile = evalFilePath;
                        break;
                }

                msg = GenerateMLDataFile(timeSeriesDictionary, outputFile, dataType);
            }

            return msg;
        }

        private Dictionary<string, List<double>> ReadTimeSeries(string inputFilePath)
        {
            Dictionary<string, List<double>> timeSeriesDictionary = new Dictionary<string, List<double>>();

            if (File.Exists(inputFilePath))
            {
                using (StreamReader inputFile = new StreamReader(inputFilePath, Encoding.UTF8))
                {
                    // build a dictionary of time series from the input file
                    // Assuming each line is a time series in format of ts{num};{maxPrediction};{frequency};data1;data2;......;dataN
                    string line;
                    while ((line = inputFile.ReadLine()) != null)
                    {
                        int pos = line.IndexOf(';');
                        if (pos > 0)
                        {
                            string key = line.Substring(0, pos);

                            line = line.Substring(pos + 1);
                            pos = line.IndexOf(';');
                            if (pos > 0)
                            {
                                string val = line.Substring(0, pos);
                                int maxForecastHorizon;
                                try
                                {
                                    maxForecastHorizon = int.Parse(val);

                                    if (maxForecastHorizon != this.maxForecastHorizon)
                                    {
                                        continue; // ignore the 6 month forecast in the test data
                                    }

                                    // next one is frequency
                                    line = line.Substring(pos + 1);
                                    pos = line.IndexOf(';');

                                    if (pos > 0)
                                    {
                                        this.frequency = line.Substring(0, pos);
                                    }

                                    line = line.Substring(pos + 1); // rest of line is data
                                }
                                catch (Exception)
                                {
                                    // max forecast column is optional in some data file
                                    this.frequency = val;

                                    line = line.Substring(pos + 1); // rest of line is data
                                }

                                List<double> ts = GetDataList(line);

                                timeSeriesDictionary.Add(key, ts);
                            }
                            else
                            {
                                throw new Exception("A time series of unkown format");
                            }
                        }
                        else
                        {
                            throw new Exception("A time series of unkown format: missing key.");
                        }
                    }
                }
            }
            else
            {
                throw new Exception(inputFilePath + " doesn't exist.");
            }

            return timeSeriesDictionary;
        }

        private void ApplyLogFunction(Dictionary<string, List<double>> timeSeriesDictionary)
        {
            foreach (List<double> timeSeries in timeSeriesDictionary.Values)
            {
                for (int i = 0; i < timeSeries.Count; i++)
                {
                    timeSeries[i] = Math.Log(timeSeries[i]);
                }
            }
        }

        private string GenerateMLDataFile(Dictionary<string, List<double>> timeSeriesDictionary, string outputFilePath, string dataType)
        {
            int row = 1;
            List<double> numbers;
            int n, end;
          
            if (File.Exists(outputFilePath))
            {
                File.Delete(outputFilePath);
            }

            StreamWriter sw;
            string seq;
            using (sw = new StreamWriter(outputFilePath))
            {
                foreach (string key in timeSeriesDictionary.Keys)
                {
                    Console.WriteLine(key);

                    seq = key.Substring(2);

                    numbers = timeSeriesDictionary[key];
                    n = numbers.Count;
                    if (dataType == "TRAIN")
                    {
                        // leave the last maxForecastHorizon numbers for validation
                        n = n - this.maxForecastHorizon;
                        numbers = numbers.GetRange(0, n);
                    }

                    if (dataType == "EVAL")
                    {
                        end = n + 1; // use all data for evaluation
                    }
                    else
                    {
                        end = n - maxForecastHorizon + 1;
                    }

                    List<double> nnVect = numbers;
                    List<double> nnLevels = numbers;
                    double level, val;
                    StringBuilder sb;
                    for (int inn = this.inputSize; inn < end; inn++)
                    {
                        sb = new StringBuilder();
                        level = nnLevels[inn - 1];  // last "trend" point in the input window is the "level" (the value used for the normalization)

                        sb.Append(seq).Append("|features");

                        for (int ii = 0; ii < this.inputSize; ii++)
                        {
                            val = nnVect[inn - this.inputSize + ii] - level;
                            sb.Append(" ").Append(val);
                        }

                        // eval file doesn't need labels
                        if (dataType != "EVAL")
                        {
                            sb.Append(" |labels");

                            for (int ii = 0; ii < this.maxForecastHorizon; ii++)
                            {
                                val = nnVect[inn + ii] - level;

                                sb.Append(" ").Append(val);
                            }
                        }

                        sb.Append(" |#");
                        sb.Append(" " + level);

                        // write a line
                        sw.WriteLine(sb.ToString());
                    }

                    row++;
                }

                sw.Flush();
            }

            return "";
        }

        private List<double> GetDataList(string line)
        {
            string[] values = line.Split(';');

            List<double> numbers = new List<double>();
            double number;
            foreach (string value in values)
            {
                try
                {
                    number = double.Parse(value);
                }
                catch (Exception)
                {
                    number = 0.0;
                }

                numbers.Add(number);
            }

            return numbers;
        }
    }
}
