/*
* @(#)TimeSeriesForecaster.cs
*
* Copyright (c) 2017 Newtera, Inc. All rights reserved.
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
    using System.Threading.Tasks;
    using System.Diagnostics;
    using System.Reflection;

    using CNTK;

    using Deedle;
    using Newtonsoft.Json;
    using Newtonsoft.Json.Linq;

    using Newtera.Common.Core;
    using Newtera.MLServer.Evaluate;

    /// <summary>
    /// Time Series forecast
    /// </summary>
    /// <version>  	1.0.0 11 May 2017 </version>
    public class TimeSeriesForecaster
    {
        private const string MODEL_FILE = "model.dnn";
        private const string DATA_DIR = "data";
        private const string INPUT_DATA_FILE = "Input_Data.txt";
        private const string MODEL_OUTPUT_LABEL_FILE = "Model_Output.txt";
        private const string TrainFileName = "Train_Data.txt";
        private const string TestFileName = "Test_Data.txt";
        private const string EvalFileName = "Eval_Data.txt";
        private const string OutputFileName = "Output_Data.txt";

        private string workingDir = null;

        /// <summary>
        /// Initiating an instance of TimeSeriesForecaster
        /// </summary>
        public TimeSeriesForecaster()
        {

        }

        public async Task<JToken> GetForecastTimeSeries(string modelFileDir, string fieldName, string frequency, string preprocess, string postprocess, JArray timeSeries)
        {
            JToken forecated = null;

            await Task.Factory.StartNew(() =>
            {
                forecated = CreateForecast(modelFileDir, fieldName, frequency, preprocess, postprocess, timeSeries);
            });

            return forecated;
        }

        private JToken CreateForecast(string modelFileDir, string fieldName, string frequency, string preprocess, string postprocess, JArray timeSeries)
        {
            JArray forecast;

            // create a working directory to keep the temp files generated for this run
            try
            {
                this.workingDir = CreateWorkingDirectory(modelFileDir);

                string modelFilePath = modelFileDir + @"\" + MODEL_FILE;
                int featureDimension = 0;
                int labelDimension = 0;
                GetModelDimensions(modelFilePath, out featureDimension, out labelDimension);

                if (timeSeries.Count < featureDimension)
                {
                    throw new Exception("The length of the time series is less than input dimension " + featureDimension);
                }

                TimeSeriesFrequency frequencyType = TimeSeriesFrequency.Second;
                try
                {
                    frequencyType = (TimeSeriesFrequency)Enum.Parse(typeof(TimeSeriesFrequency), frequency);
                }
                catch (Exception)
                {
                }

                List<string> featureList = null;

                if (!string.IsNullOrEmpty(preprocess))
                {
                    // Assume input data is a raw data which will be processed to produce feature data
                    featureList = GetFeaturesFromPreprocess(modelFileDir, preprocess, timeSeries, fieldName, frequency, featureDimension, labelDimension);
                }
                else
                {
                    // no preprocessing, assume the input data is features already
                    featureList = GetFeaturesWithoutPreprocess(timeSeries, fieldName, frequency);
                }

                IList<IList<float>> sequenceList = CNTKModelEvaluator.EvaluateTimeSeriesModel(modelFilePath, featureList, DeviceDescriptor.CPUDevice);
                IList<string> forecastLableList = null;
                if (!string.IsNullOrEmpty(postprocess))
                {
                    forecastLableList = GetLabelsFromPostprocess(modelFileDir, postprocess, sequenceList);
                }

                // convert to forecast time series
                if (forecastLableList != null && forecastLableList.Count == 1)
                {
                    forecast = ConvertToTimeSeries(forecastLableList[0], timeSeries, fieldName, frequencyType);
                }
                else
                {
                    forecast = new JArray();
                }

                return forecast;
            }
            finally
            {
                // delete the dir
                if (Directory.Exists(this.workingDir))
                {
                    Directory.Delete(this.workingDir, true);
                }
            }
        }

        private List<string> GetFeaturesFromPreprocess(string modelFileDir, string preprocess, JArray timeSeries, string fieldName, string frequency, int featureDimension, int labelDimension)
        {
            List<string> featureList = new List<string>();
 
            string featureStr = ConvertToFeatures(timeSeries, fieldName, frequency);

            System.IO.File.WriteAllText(this.workingDir + @"\" + INPUT_DATA_FILE, featureStr);

            featureList = RunPreprocessing(this.workingDir, preprocess, featureDimension, labelDimension);

            return featureList;
        }

        private List<string> GetFeaturesWithoutPreprocess(JArray timeSeries, string fieldName, string frequency)
        {
            List<string> featureList = new List<string>();

            string featureStr = ConvertToFeatures(timeSeries, fieldName, frequency);

            featureList.Add(featureStr);

            return featureList;
        }

        private string ConvertToFeatures(JArray timeSeries, string fieldName, string frequency)
        {
            StringBuilder sb = new StringBuilder();
            sb.Append("ts1").Append(";").Append(frequency);
            foreach (JObject point in timeSeries)
            {
                sb.Append(";");
                sb.Append(point[fieldName].ToString()); // index 0 is index, index 1 is y value
            }

            return sb.ToString();
        }

        private List<string> RunPreprocessing(string workingDir, string preprocess, int featureDimension, int labelDimension)
        {
            if (preprocess.EndsWith(".py"))
            {
                return RunPythonPreprocessProgram(workingDir, preprocess);
            }
            else if (preprocess.EndsWith(".dll"))
            {
                return RunDotNetPreprocessProgram(workingDir, preprocess, featureDimension, labelDimension);
            }
            else
            {
                throw new Exception(preprocess + " is an unknown type of program.");
            }
        }

        private List<string> GetLabelsFromPostprocess(string modelFileDir, string postprocess, IList<IList<float>> sequenceList)
        {
            List<string> labelList = new List<string>();
            string modelOutputFile = this.workingDir + @"\" + MODEL_OUTPUT_LABEL_FILE;
            string evalDataFilePath = this.workingDir + @"\" + EvalFileName;
            bool newSequence;
            using (StreamWriter sw = new StreamWriter(modelOutputFile))
            {
                foreach (IList<float> sequence in sequenceList)
                {
                    newSequence = true;
                    foreach (float val in sequence)
                    {
                        if (!newSequence)
                        {
                            sw.Write(" ");
                        }

                        sw.Write(val);
                        newSequence = false;
                    }
                    sw.WriteLine();
                }
            }

            labelList = RunPostprocessing(modelOutputFile, evalDataFilePath, this.workingDir, postprocess);

            labelList = RemoveNonNumbers(labelList);

            return labelList;
        }

        /// <summary>
        /// Run the postprocessing
        /// </summary>
        private List<string> RunPostprocessing(string modelOutputFile, string evalDataFilePath, string workingDir, string postprocess)
        {
            if (postprocess.EndsWith(".py"))
            {
                return RunPythonPostprocessing(modelOutputFile, evalDataFilePath, workingDir, postprocess);
            }
            else if (postprocess.EndsWith(".dll"))
            {
                return RunDotNetPostprocessProgram(modelOutputFile, evalDataFilePath, workingDir, postprocess);
            }
            else
            {
                throw new Exception(postprocess + " is an unknown type of program.");
            }
        }

        /// <summary>
        /// Run python preprocess program
        /// </summary>
        /// <param name="experimentDataDir"></param>
        /// <param name="preprocessing"></param>
        private List<string> RunPythonPreprocessProgram(string experimentDataDir, string preprocess)
        {
            Process p = new Process();
            ProcessStartInfo start = new ProcessStartInfo();
            string inputDataFile = experimentDataDir + @"\" + INPUT_DATA_FILE;
            start.FileName = GetAnacondaDir() + @"\python.exe";
            string trainDataPath = experimentDataDir + @"\" + TrainFileName;
            string testDataPath = experimentDataDir + @"\" + TestFileName;
            string evalDataPath = experimentDataDir + @"\" + EvalFileName;
            start.Arguments = string.Format("{0} {1} {2} {3} {4}", preprocess, inputDataFile, trainDataPath, testDataPath, evalDataPath);
            start.UseShellExecute = false;
            start.CreateNoWindow = true;
            start.RedirectStandardOutput = true;
            start.RedirectStandardError = true;

            p.StartInfo = start;
            p.Start();

            p.BeginOutputReadLine();
            p.BeginErrorReadLine();

            p.WaitForExit(); //you need this in order to flush the output buffer

            return GetProgramOutputData(evalDataPath);
        }

        /// <summary>
        /// Run a preprocess program in dll
        /// </summary>
        /// <param name="experimentDataDir"></param>
        /// <param name="preprocessing"></param>
        private List<string> RunDotNetPreprocessProgram(string experimentDataDir, string preprocessing, int featureDimension, int labelDimension)
        {
            string inputFilePath = experimentDataDir + @"\" + INPUT_DATA_FILE;
            string trainDataPath = experimentDataDir + @"\" + TrainFileName;
            string testDataPath = experimentDataDir + @"\" + TestFileName;
            string evalDataPath = experimentDataDir + @"\" + EvalFileName;

            // Load assembly "MyAssembly.dll" from file path. Specify that we will be using class MyAssembly.Preprocess
            Assembly assembly = Assembly.LoadFrom(preprocessing);
            string assemblyName = preprocessing;
            int pos = assemblyName.LastIndexOf(@"\");
            if (pos > 0)
            {
                assemblyName = assemblyName.Substring(pos + 1);
            }
            pos = assemblyName.LastIndexOf('.');
            assemblyName = assemblyName.Substring(0, pos);
            string className = assemblyName + ".Preprocess";
            Type classType = assembly.GetType(className);

            // We will be invoking a method: 'public string Run(string inputFilePath, string trainFilePath, string testFilePath, string evalFilePath, int featureDim, int labelDim)'
            var methodInfo = classType.GetMethod("Run", new Type[] { typeof(string), typeof(string), typeof(string), typeof(string), typeof(int), typeof(int) });
            if (methodInfo == null)
            {
                // never throw generic Exception - replace this with some other exception type
                throw new Exception(className + " doesn't have a Run method.");
            }

            // Create instance of MyAssembly.Preprocess.
            var instance = Activator.CreateInstance(classType);

            // Specify parameters for the method we will be invoking: 'string Run(string inputFilePath, string trainFilePath, string testFilePath, string evalFilePath, int featureDim, int labelDim)'
            object[] parameters = new object[6];
            parameters[0] = inputFilePath;            // inputFilePath parameter
            parameters[1] = trainDataPath;   // trainDataPath parameter
            parameters[2] = testDataPath;   // testDataPath parameter
            parameters[3] = evalDataPath;   // evalDataPath parameter
            parameters[4] = featureDimension;
            parameters[5] = labelDimension;

            // Invoke method 'string Run(int count, string text)'
            var result = methodInfo.Invoke(instance, parameters);

            return GetProgramOutputData(evalDataPath);
        }

        /// <summary>
        /// Run the post processing python script
        /// </summary>
        private List<string> RunPythonPostprocessing(string modelOutputFilePath, string evalDataFilePath, string experimentDataDir, string postprocess)
        {
            List<string> forecastLabelList = new List<string>();

            Process p = new Process();
            ProcessStartInfo start = new ProcessStartInfo();
            start.FileName = GetAnacondaDir() + @"\python.exe";
            string outputDataPath = experimentDataDir + @"\" + OutputFileName;

            start.Arguments = string.Format("{0} {1} {2} {3}", postprocess, evalDataFilePath, modelOutputFilePath, outputDataPath);
            start.UseShellExecute = false;
            start.CreateNoWindow = true;
            start.RedirectStandardOutput = true;
            start.RedirectStandardError = true;

            p.StartInfo = start;

            p.Start();

            p.BeginOutputReadLine();
            p.BeginErrorReadLine();

            p.WaitForExit(); //you need this in order to flush the output buffer

            return GetProgramOutputData(outputDataPath);
        }

        /// <summary>
        /// Run a post processing program in dll
        /// </summary>
        /// <param name="output">The console output</param>
        /// <param name="resultData">Writer for post processing result</param>
        /// <param name="modelOutputFilePath">The path of the model ouput file which is the input for post-processing</param>
        /// <param name="evalDataFilePath">The path of the evaluation data file generated by the preprocessing script</param>
        /// <param name="experimentDataDir">The directory where the post processing script writes it output file</param>
        /// <param name="postprocessing">The post processing object</param>
        private List<string> RunDotNetPostprocessProgram(string modelOutputFilePath, string evalDataFilePath, string experimentDataDir, string postprocess)
        {
            List<string> forecastLabelList = new List<string>();

            string outputDataPath = experimentDataDir + @"\" + OutputFileName;

            // Load assembly "MyAssembly.dll" from file path. Specify that we will be using class MyAssembly.Postprocess
            Assembly assembly = Assembly.LoadFrom(postprocess);
            string assemblyName = postprocess;
            int pos = assemblyName.LastIndexOf(@"\");
            if (pos > 0)
            {
                assemblyName = assemblyName.Substring(pos + 1);
            }
            pos = assemblyName.LastIndexOf('.');
            assemblyName = assemblyName.Substring(0, pos);
            string className = assemblyName + ".Postprocess";
            Type classType = assembly.GetType(className);

            // We will be invoking a method: 'public string Run(string forecatFilePath, string evalFilePath, string outputFilePath)'
            var methodInfo = classType.GetMethod("Run", new Type[] { typeof(string), typeof(string), typeof(string) });
            if (methodInfo == null)
            {
                // never throw generic Exception - replace this with some other exception type
                throw new Exception(className + " doesn't have a Run method.");
            }

            // Create instance of MyAssembly.Preprocess.
            var instance = Activator.CreateInstance(classType);

            // Specify parameters for the method we will be invoking: 'string Run(string forecatFilePath, string evalFilePath, string outputFilePath)'
            object[] parameters = new object[3];
            parameters[0] = modelOutputFilePath;            // model output data file path parameter
            parameters[1] = evalDataFilePath;   // evaluation data file path parameter
            parameters[2] = outputDataPath;   // post process output file path parameter

            // Invoke method 'string Run(int count, string text)'
            var result = methodInfo.Invoke(instance, parameters);

            return GetProgramOutputData(outputDataPath);
        }

        private List<string> GetProgramOutputData(string outputDataPath)
        {
            List<string> dataList = new List<string>();

            if (File.Exists(outputDataPath))
            {
                using (StreamReader sr = new StreamReader(outputDataPath, Encoding.UTF8))
                {
                    string line;
                    while ((line = sr.ReadLine()) != null)
                    {
                        dataList.Add(line);
                    }
                }
            }
            return dataList;
        }

        private List<string> RemoveNonNumbers(List<string> textList)
        {
            List<string> dataList = new List<string>();

            foreach (string textLine in textList)
            {
                StringBuilder sb = new StringBuilder();
                string[] items = textLine.Split(' ');
                foreach (string item in items)
                {
                    try
                    {
                        float number = float.Parse(item);

                        if (sb.Length > 0)
                        {
                            sb.Append(' ');
                        }

                        sb.Append(number);
                    }
                    catch (Exception)
                    {

                    }
                }

                dataList.Add(sb.ToString());
            }

            return dataList;
        }

        private string GetAnacondaDir()
        {
            return "";
        }

        private JArray ConvertToTimeSeries(string forecastLables, JArray feedTimeSeries, string fieldName, TimeSeriesFrequency frequencyType)
        {
            JArray forecastTimeSeries = new JArray();

            string[] labels = forecastLables.Split(' ');
            JObject point;
            for (int i = 0; i < labels.Length; i++)
            {
                point = new JObject();
                point["Index"] = GetXValue(feedTimeSeries, frequencyType, i);
                point[fieldName] = labels[i];
                forecastTimeSeries.Add(point);
            }

            return forecastTimeSeries;
        }

        private void GetModelDimensions(string modelFilePath, out int featureDimension, out int labelDimension)
        {
            featureDimension = 0;
            labelDimension = 0;

            Function modelFunc = Function.Load(modelFilePath, DeviceDescriptor.CPUDevice);

            IList<Variable> inputVars = modelFunc.Arguments;
            foreach (Variable inputVar in inputVars)
            {
                if (inputVar.Name == "features")
                {
                    featureDimension = inputVar.Shape.TotalSize;
                }
                else if (inputVar.Name == "labels")
                {
                    labelDimension = inputVar.Shape.TotalSize;
                }
            }
        }

        private string CreateWorkingDirectory(string baseDir)
        {
            string dirName = baseDir + @"\" + Guid.NewGuid().ToString();

            if (!Directory.Exists(dirName))
            {
                Directory.CreateDirectory(dirName);
            }

            return dirName;
        }

        private string GetXValue(JArray feedTimeSeries, TimeSeriesFrequency frequency, int i)
        {
            JObject lastPoint = feedTimeSeries[feedTimeSeries.Count - 1] as JObject;

            string lastX = lastPoint.GetValue("Index").ToString();
            string nextX = lastX;
            int lastXInt = -1;

            try
            {
                lastXInt = int.Parse(lastX) + i;
            }
            catch (Exception)
            {
                lastXInt = -1;
            }

            if (lastXInt < 0)
            {
                try
                {
                    DateTime timeStamp;

                    switch (frequency)
                    {
                        case TimeSeriesFrequency.Second:
                            timeStamp = DateTime.Parse(lastX);
                            timeStamp = timeStamp.AddSeconds(i);

                            nextX = timeStamp.ToString("yyyy-MM-dd HH:mm:ss");
                            break;

                        case TimeSeriesFrequency.Minute:
                            lastX += ":00"; // make it valid date time format
                            timeStamp = DateTime.Parse(lastX);
                            timeStamp = timeStamp.AddMinutes(i);
                            nextX = timeStamp.ToString("yyyy-MM-dd HH:mm");
                            break;

                        case TimeSeriesFrequency.Hour:
                            lastX += ":00:00"; // make it valid date time format
                            timeStamp = DateTime.Parse(lastX);
                            timeStamp = timeStamp.AddHours(i);
                            nextX = timeStamp.ToString("yyyy-MM-dd HH");
                            break;

                        case TimeSeriesFrequency.Day:
                            lastX += " 00:00:00"; // make it valid date time format
                            timeStamp = DateTime.Parse(lastX);
                            timeStamp = timeStamp.AddDays(i);
                            nextX = timeStamp.ToString("yyyy-MM-dd");
                            break;

                        case TimeSeriesFrequency.Month:
                            lastX += "-01 00:00:00"; // make it valid date time format
                            timeStamp = DateTime.Parse(lastX);
                            timeStamp = timeStamp.AddMonths(i);
                            nextX = timeStamp.ToString("yyyy-MM");
                            break;

                        default:
                            timeStamp = DateTime.Parse(lastX);
                            timeStamp = timeStamp.AddSeconds(i);

                            nextX = timeStamp.ToString("yyyy-MM-dd HH:mm:ss");
                            break;
                    }
                }
                catch (Exception ex)
                {
                    ErrorLog.Instance.WriteLine(lastX + ", error: " + ex.Message);
                    nextX = lastX;
                }
            }
            else
            {
                nextX = lastXInt.ToString();
            }

            return nextX;
        }
    }
}