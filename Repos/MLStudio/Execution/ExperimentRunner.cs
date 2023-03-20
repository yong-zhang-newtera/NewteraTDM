using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using System.Threading.Tasks;
using System.Diagnostics;
using System.Reflection;

using Newtera.MLActivities.Core;
using Newtera.MLActivities.MLConfig;
using Newtera.MLStudio.Utilities;

namespace Newtera.MLStudio.Execution
{
    /// <summary>
    /// Runner to run the ML configurations
    /// </summary>
    public class ExperimentRunner
    {
        private bool cleanLogs;
        private bool isAborted;
        private MLExperimentManager experimentManager;

        public ExperimentRunner(MLExperimentManager experimentManager, bool cleanLogs)
        {
            this.experimentManager = experimentManager;
            this.cleanLogs = cleanLogs;
        }

        public string Run(TextWriterHolder writerHolder)
        {
            // kill tensorboard process so that log file can be deleted
            KillTensorboardProcess();

            // create base dirs
            string baseDir = MLNameSpace.GetHomeDir() + @"\" + MLNameSpace.EXPERIMENT_DIR;
            if (!Directory.Exists(baseDir))
            {
                Directory.CreateDirectory(baseDir);
            }

            // base experiment dir
            string experimentDir = baseDir + @"\" + this.experimentManager.Name;
            if (!Directory.Exists(experimentDir))
            {
                Directory.CreateDirectory(experimentDir);
            }

            string baseLogDir = experimentDir + @"\" + MLNameSpace.LOG_DIR;

            if (this.cleanLogs)
            {
                bool keepTrying = true;
                int tries = 3;
                while (keepTrying)
                {
                    // delete the log directory and everything under log directory
                    if (Directory.Exists(baseLogDir))
                    {
                        try
                        {
                            Directory.Delete(baseLogDir, true);

                            keepTrying = false;
                        }
                        catch (Exception)
                        {
                            if (tries > 0)
                            {
                                tries--;
                                // wait for 2 seconds
                                System.Threading.Thread.Sleep(2000);
                            }
                            else
                            {
                                keepTrying = false;
                            }
                        }
                    }
                    else
                    {
                        keepTrying = false;
                    }
                }
            }

            if (!Directory.Exists(baseLogDir))
            {
                // create a log dir for the experiment
                Directory.CreateDirectory(baseLogDir);
            }

            string experimentDataDir = experimentDir + @"\" + MLNameSpace.DATA_DIR;
            if (!Directory.Exists(experimentDataDir))
            {
                // create a data dir for the experiment
                Directory.CreateDirectory(experimentDataDir);
            }

            isAborted = false;

            string evalDataFilePath = null;
 
            if (this.experimentManager.PreProcessing != null)
            {
                evalDataFilePath = experimentDataDir + @"\" + this.experimentManager.PreProcessing.EvalFileName;
                writerHolder.evalData.FilePath = evalDataFilePath; // load the eval file when requested
                writerHolder.rawData.FilePath = this.experimentManager.PreProcessing.GetDataFilePath();

                if (this.experimentManager.PreProcessing.Enabled)
                {
                    RunPreprocessing(writerHolder.output, experimentDataDir, this.experimentManager.PreProcessing);
                }
            }

            if (isAborted)
            {
                return "";
            }

            string configName;
            string modelOutputFileDir = null;
            MLComponentCollection configurations = this.experimentManager.ExecutableConfigurations;
            foreach (MLConfiguration configuration in configurations)
            {
                if (isAborted)
                {
                    break;
                }

                configName = configuration.Name;
                if (!string.IsNullOrEmpty(configuration.BranchName))
                {
                    configName += @"\" + configuration.BranchName;
                }

                writerHolder.output.WriteLine("*** " + DateTime.Now.ToString("s") +  ": Start running configuration " + configName + " ***");
                writerHolder.output.WriteLine(".....");

                modelOutputFileDir = RunConfiguration(experimentDir, baseLogDir, configuration, writerHolder.code, writerHolder.output);

                if (!this.isAborted)
                {
                    writerHolder.output.WriteLine("*** " + DateTime.Now.ToString("s") + ": Finish running configuration " + configName + " ***");
                }
                else
                {
                    writerHolder.output.WriteLine("*** " + DateTime.Now.ToString("s") + ": Abort running configuration " + configName + " ***");
                }

                writerHolder.output.WriteLine("");
            }

            // show the training, test, and evaluation data when requested
            writerHolder.trainData.FilePath = this.experimentManager.TrainDataFile;
            writerHolder.testData.FilePath = this.experimentManager.TestDataFile;
            writerHolder.evalData.FilePath = this.experimentManager.EvalDataFile;

            string modelOutputFilePath = GetModelOutputFilePath(modelOutputFileDir);

            writerHolder.outputData.FilePath = modelOutputFilePath;
            if (this.experimentManager.PostProcessing != null)
            {
                if (this.experimentManager.PostProcessing.Enabled &&
                    !string.IsNullOrEmpty(modelOutputFilePath) &&
                    !string.IsNullOrEmpty(evalDataFilePath))
                {
                    RunPostprocessing(writerHolder.output, writerHolder.resultData, modelOutputFilePath, evalDataFilePath, experimentDataDir, this.experimentManager.PostProcessing);
                }
                else
                {
                    if (string.IsNullOrEmpty(modelOutputFilePath))
                    {
                        writerHolder.output.WriteLine("*** Unable to find the model output file. ***");
                    }
                }
            }

            // start tensorboard process so that webbrowser can display the log(s)
            StartTensorboardProcess(baseLogDir);

            return "";
        }

        public void Abort()
        {
            isAborted = true;

            KillConfigRunnerProcess();
        }

        private string RunConfiguration(string experimentDir, string baseLogDir, MLConfiguration configuration, TextEditorWriter code, TextWriter output)
        {
            // create a subdirectory using config's name if non-exist
            string configDir = experimentDir + @"\" + configuration.Name;
            if (!Directory.Exists(configDir))
            {
                Directory.CreateDirectory(configDir);
            }

            string logDir = baseLogDir + @"\" + configuration.Name;
            string relativeLogDirPrefix = @"..\log\";
            string relativeLogDir =  configuration.Name;
            if (!string.IsNullOrEmpty(configuration.BranchName))
            {
                logDir += @"\" + configuration.BranchName; // for diffrent hyper-parameters
                relativeLogDir += @"\" + configuration.BranchName;
                configDir += @"\" + configuration.BranchName;
                relativeLogDirPrefix = @"..\..\log\";
                if (!Directory.Exists(configDir))
                {
                    Directory.CreateDirectory(configDir);
                }
            }

            string runName = CreateLogSubdir(logDir);
            relativeLogDir += @"\" + runName;
            logDir += @"\" + runName;
            if (!Directory.Exists(logDir))
            {
                Directory.CreateDirectory(logDir);
            }

            string configFileName = configuration.Name + ".cntk";
            string configFilePath = configDir + @"\" + configFileName;

            // remove the file if exist
            if (File.Exists(configFilePath))
            {
                File.Delete(configFilePath);
            }

            // write the configuration
            using (StreamWriter sw = new StreamWriter(configFilePath))
            {
                configuration.WriteTo(sw, 0, MLConfigurationType.CNTK);
                sw.Flush();
            }

            // keep the configFilePath in code output object to display the content when requested
            code.FilePath = configFilePath;

            RunMLConfiguration(configDir, configFileName, relativeLogDirPrefix + relativeLogDir, output);

            return configDir;
        }

        private void RunMLConfiguration(string configDir, string configFileName, string logDirPath, TextWriter output)
        {
            Process p = new Process();
            ProcessStartInfo info = new ProcessStartInfo();
            info.FileName = "cmd.exe";
            info.CreateNoWindow = true;
            info.RedirectStandardInput = true;
            info.RedirectStandardError = true;
            //info.RedirectStandardOutput = true;
            info.UseShellExecute = false;

            p.StartInfo = info;
            p.ErrorDataReceived += (sender, args) => DisplayProcessOutput(output, args.Data);
            p.Start();

            p.BeginErrorReadLine();

            string cntkDir = MLNameSpace.GetCNTKDir();
            string anacondaDir = MLNameSpace.GetAnacondaDir();
            string cmd1 = @"cd " + configDir;
            string cmd2 = @"set PATH=" + cntkDir + @"\cntk\cntk;%PATH%";
            string cmd3 = @"set PATH=" + cntkDir + @"\cntk\cntk;%PATH% '" + anacondaDir + @"\Scripts\activate\' '" + anacondaDir + @"\envs\cntk-py35'";
            string cmd4 = @"cntk configFile=" + configFileName + " makeMode=false tensorBoardLogDir=" + logDirPath + " tensorBoardNumMBsToLogResult=10";
            p.StandardInput.WriteLine(cmd1);
            p.StandardInput.WriteLine(cmd2);
            p.StandardInput.WriteLine(cmd3);
            p.StandardInput.WriteLine(cmd4);

            p.StandardInput.Close(); // line added to stop process from hanging on ReadToEnd()
            p.WaitForExit(); //you need this in order to flush the output buffer

            //string outputString = p.StandardOutput.ReadToEnd();

            /*
            using (StreamReader reader = p.StandardError)
            {
                string outputString = reader.ReadToEnd();

                return outputString;
            }
            */
        }

        private string CreateLogSubdir(string logDir)
        {
            int index = 1;

            if (Directory.Exists(logDir))
            {
                string[] subdirs = Directory.GetDirectories(logDir);
                foreach (string subDirName in subdirs)
                {
                    int pos = subDirName.LastIndexOf('-');
                    if (pos > 0)
                    {
                        string numStr = subDirName.Substring(pos + 1);
                        try
                        {
                            int num = Int32.Parse(numStr);
                            if (num >= index)
                            {
                                // add num bu one as index
                                index = num + 1;
                            }
                        }
                        catch (Exception)
                        {
                        }
                    }
                }
            }

            return "run-" + index;
        }

        private void KillTensorboardProcess()
        {
            string[] ProcessName = { "tensorboard" };
            try
            {
                System.Diagnostics.Process[] myProcesses = System.Diagnostics.Process.GetProcesses();
                foreach (System.Diagnostics.Process myProcess in myProcesses)
                {
                    foreach (string pName in ProcessName)
                    {
                        if (pName == myProcess.ProcessName)
                        {
                            myProcess.Kill();
                        }
                    }
                }
            }
            catch (Exception)
            {
            }
        }

        private void KillConfigRunnerProcess()
        {
            string[] ProcessName = { "cntk" };
            try
            {
                System.Diagnostics.Process[] myProcesses = System.Diagnostics.Process.GetProcesses();
                foreach (System.Diagnostics.Process myProcess in myProcesses)
                {
                    foreach (string pName in ProcessName)
                    {
                        if (pName.ToUpper() == myProcess.ProcessName.ToUpper())
                        {
                            myProcess.Kill();
                        }
                    }
                }
            }
            catch (Exception)
            {
            }
        }

        private void StartTensorboardProcess(string baseLogDir)
        {
            try
            {
                Process p = new Process();
                ProcessStartInfo info = new ProcessStartInfo();
                info.FileName = "tensorboard.exe";
                info.CreateNoWindow = true;
                info.RedirectStandardInput = true;
                info.UseShellExecute = false;
                info.Arguments = "--logdir=" + baseLogDir;

                p.StartInfo = info;
                p.Start();

                Newtera.MLStudio.ViewModel.TensorBoard.TheInstance.IsAvailable = true;
            }
            catch (Exception ex)
            {
                // TensorBoard.exe doesn't exist.
                Newtera.MLStudio.ViewModel.TensorBoard.TheInstance.IsAvailable = false;

                Newtera.MLStudio.ViewModel.TensorBoard.TheInstance.ErrorMessage = ex.Message;
            }
        }

        /// <summary>
        /// Run the preprocessing
        /// </summary>
        /// <param name="output"></param>
        /// <param name="experimentDataDir"></param>
        /// <param name="preprocessing"></param>
        private void RunPreprocessing(TextWriter output, string experimentDataDir, MLPreProcessing preprocessing)
        {
            output.WriteLine("*** " + DateTime.Now.ToString("s") + ": Start running data preprocessing***");
            output.WriteLine(".....");

            if (preprocessing.GetProgramFilePath().EndsWith(".py"))
            {
                RunPythonPreprocessProgram(output, experimentDataDir, preprocessing);
            }
            else if (preprocessing.GetProgramFilePath().EndsWith(".dll"))
            {
                RunDotNetPreprocessProgram(output, experimentDataDir, preprocessing);
            }
            else
            {
                throw new Exception(preprocessing.GetProgramFilePath() + " is an unknown type of program.");
            }

            output.WriteLine("*** " + DateTime.Now.ToString("s") + ": Finish running data preprocessing***");
            output.WriteLine();
        }

        /// <summary>
        /// Run python preprocess program
        /// </summary>
        /// <param name="output"></param>
        /// <param name="experimentDataDir"></param>
        /// <param name="preprocessing"></param>
        private void RunPythonPreprocessProgram(TextWriter output, string experimentDataDir, MLPreProcessing preprocessing)
        {
            Process p = new Process();
            ProcessStartInfo start = new ProcessStartInfo();
            start.FileName = MLNameSpace.GetAnacondaDir() +  @"\python.exe";
            string trainDataPath = experimentDataDir + @"\" + preprocessing.TrainFileName;
            string testDataPath = experimentDataDir + @"\" + preprocessing.TestFileName;
            string evalDataPath = experimentDataDir + @"\" + preprocessing.EvalFileName;
            start.Arguments = string.Format("{0} {1} {2} {3} {4}", preprocessing.GetProgramFilePath(), preprocessing.GetDataFilePath(), trainDataPath, testDataPath, evalDataPath);
            start.UseShellExecute = false;
            start.CreateNoWindow = true;
            start.RedirectStandardOutput = true;
            start.RedirectStandardError = true;

            p.StartInfo = start;
            p.OutputDataReceived += (sender, args) => DisplayProcessOutput(output, args.Data);
            p.ErrorDataReceived += (sender, args) => DisplayProcessOutput(output, args.Data);

            p.Start();

            p.BeginOutputReadLine();
            p.BeginErrorReadLine();

            p.WaitForExit(); //you need this in order to flush the output buffer
        }

        /// <summary>
        /// Run a preprocess program in dll
        /// </summary>
        /// <param name="output"></param>
        /// <param name="experimentDataDir"></param>
        /// <param name="preprocessing"></param>
        private void RunDotNetPreprocessProgram(TextWriter output, string experimentDataDir, MLPreProcessing preprocessing)
        {
            string inputFilePath = preprocessing.GetDataFilePath();
            string trainDataPath = experimentDataDir + @"\" + preprocessing.TrainFileName;
            string testDataPath = experimentDataDir + @"\" + preprocessing.TestFileName;
            string evalDataPath = experimentDataDir + @"\" + preprocessing.EvalFileName;

            // Load assembly "MyAssembly.dll" from file path. Specify that we will be using class MyAssembly.Preprocess
            Assembly assembly = Assembly.LoadFrom(preprocessing.GetProgramFilePath());
            string assemblyName = preprocessing.GetProgramFilePath();
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
            parameters[4] = preprocessing.FeatureDimension;
            parameters[5] = preprocessing.LabelDimension;

            // Invoke method 'string Run(int count, string text)'
            var result = methodInfo.Invoke(instance, parameters);
        }

        /// <summary>
        /// Run the postprocessing
        /// </summary>
        /// <param name="output"></param>
        /// <param name="experimentDataDir"></param>
        /// <param name="preprocessing"></param>
        private void RunPostprocessing(TextWriter output, TextEditorWriter resultData, string modelOutputFilePath, string evalDataFilePath, string experimentDataDir, MLPostProcessing postprocessing)
        {
            output.WriteLine("*** " + DateTime.Now.ToString("s") + ": Start running data postprocessing***");
            output.WriteLine(".....");

            if (postprocessing.GetProgramFilePath().EndsWith(".py"))
            {
                RunPythonPostprocessing(output, resultData, modelOutputFilePath, evalDataFilePath, experimentDataDir, postprocessing);
            }
            else if (postprocessing.GetProgramFilePath().EndsWith(".dll"))
            {
                RunDotNetPostprocessProgram(output, resultData, modelOutputFilePath, evalDataFilePath, experimentDataDir, postprocessing);
            }
            else
            {
                throw new Exception(postprocessing.GetProgramFilePath() + " is an unknown type of program.");
            }

            output.WriteLine("*** " + DateTime.Now.ToString("s") + ": Finish running data postprocessing***");
            output.WriteLine();
        }

        /// <summary>
        /// Run the post processing python script
        /// </summary>
        /// <param name="output">The console output</param>
        /// <param name="resultData">Writer for post processing result</param>
        /// <param name="modelOutputFilePath">The path of the model ouput file which is the input for post-processing</param>
        /// <param name="evalDataFilePath">The path of the evaluation data file generated by the preprocessing script</param>
        /// <param name="experimentDataDir">The directory where the post processing script writes it output file</param>
        /// <param name="postprocessing">The post processing object</param>
        private void RunPythonPostprocessing(TextWriter output, TextEditorWriter resultData, string modelOutputFilePath, string evalDataFilePath, string experimentDataDir, MLPostProcessing postprocessing)
        {
            Process p = new Process();
            ProcessStartInfo start = new ProcessStartInfo();
            start.FileName = MLNameSpace.GetAnacondaDir() + @"\python.exe";
            string outputDataPath = experimentDataDir + @"\" + postprocessing.OutputFileName;
            resultData.FilePath = outputDataPath; // load the result file when requested

            start.Arguments = string.Format("{0} {1} {2} {3}", postprocessing.GetProgramFilePath(), evalDataFilePath, modelOutputFilePath, outputDataPath);
            start.UseShellExecute = false;
            start.CreateNoWindow = true;
            start.RedirectStandardOutput = true;
            start.RedirectStandardError = true;

            p.StartInfo = start;
            p.OutputDataReceived += (sender, args) => DisplayProcessOutput(output, args.Data);
            p.ErrorDataReceived += (sender, args) => DisplayProcessOutput(output, args.Data);

            p.Start();

            p.BeginOutputReadLine();
            p.BeginErrorReadLine();

            p.WaitForExit(); //you need this in order to flush the output buffer
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
        private void RunDotNetPostprocessProgram(TextWriter output, TextEditorWriter resultData, string modelOutputFilePath, string evalDataFilePath, string experimentDataDir, MLPostProcessing postprocessing)
        {
            string outputDataPath = experimentDataDir + @"\" + postprocessing.OutputFileName;

            // Load assembly "MyAssembly.dll" from file path. Specify that we will be using class MyAssembly.Postprocess
            Assembly assembly = Assembly.LoadFrom(postprocessing.GetProgramFilePath());
            string assemblyName = postprocessing.GetProgramFilePath();
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
            parameters[0] = modelOutputFilePath;            // forecatFilePath parameter
            parameters[1] = evalDataFilePath;   // evalDataFilePath parameter
            parameters[2] = outputDataPath;   // outputFilePath parameter

            // Invoke method 'string Run(int count, string text)'
            var result = methodInfo.Invoke(instance, parameters);
        }

        private string GetModelOutputFilePath(string modelOutputDir)
        {
            string outputFilePath = null;

            DirectoryInfo dir = new DirectoryInfo(modelOutputDir);
            FileInfo[] files = dir.GetFiles();

            foreach (FileInfo file in files)
            {
                if (file.Name.StartsWith("output"))
                {
                    // output file, for example, output.txt.z
                    outputFilePath = modelOutputDir + @"\" + file.Name;
                    break;
                }
            }

            return outputFilePath;
        }

        void DisplayProcessOutput(TextWriter output, string outputString)
        {
            if (!string.IsNullOrEmpty(outputString))
            {
                output.WriteLine(outputString);
            }
        }
    }
}
