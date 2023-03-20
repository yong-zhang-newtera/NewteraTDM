using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;
using System.Diagnostics;
using System.Reflection;

using CNTK;

using Newtera.MLActivities.Core;
using Newtera.MLStudio.Utilities;
using Newtera.MLActivities.MLConfig;

namespace Newtera.MLStudio.Views
{
    /// <summary>
    /// EvaluateInfoDialog.xaml 的交互逻辑
    /// </summary>
    public partial class EvaluateInfoDialog : Window
    {
        private const string INPUT_DATA_FILE = "Input_Data.txt";
        private const string MODEL_OUTPUT_LABEL_FILE = "Model_Output.txt";

        private MLExperimentManager experimentManager;
        private string experimentDir;
        public EvaluateInfoDialog(MLExperimentManager experimentManager)
        {
            InitializeComponent();

            this.experimentManager = experimentManager;

            this.experimentDir = MLNameSpace.GetHomeDir() + @"\" + MLNameSpace.EXPERIMENT_DIR + @"\" + this.experimentManager.Name;
            if (Directory.Exists(this.experimentDir))
            {
                string configName = null;
                MLComponentCollection configurations = this.experimentManager.ExecutableConfigurations;
                foreach (MLConfiguration configuration in configurations)
                {
                    configName = configuration.Name;
                    break;
                }

                if (!string.IsNullOrEmpty(configName))
                {
                    string modelFile = this.experimentDir + @"\" + configName + @"\model.dnn";
                    if (File.Exists(modelFile))
                    {
                        txtModelFile.Text = modelFile;
                        //txtFeatures.Text = @"ts1;monthly;855.144865868006;847.292061506786;824.731660406105;866.64540600304;928.333300035969;927.649280359846;901.072254821959;940.843420025213;954.740175823436;960.653429083165;982.375013296118;899.664922132408;993.954018188677;1009.38595792977;982.587022405312;928.385610618689;970.463200723922;945.712082400429;1101.38819519303;1036.44376151694;958.88806908289;1018.38727368794;1033.03583309946;1056.03112280476;1028.72942599475;1071.88505018723;1068.11569905266;1036.9075131888;1086.45344539621;1085.11331046227;1119.35713814429;1123.74215042115;1077.81269065316;1127.47995780678;1176.46472562497;1019.81624257261;1122.43214034554;1106.14375392939;1137.01870344617;1098.78831965699;1138.25310787545;1175.60115758247;1138.20956338146;1203.36892983547;1186.00010652102;1221.83247807841;1185.25616276843;1198.46020610794;1124.843862246;1176.35929728173;1203.81528906175;1270.66492178852;1213.84772111947;1276.12974705234;1291.04501075437;1281.34541848622;1288.15368786252;1306.7916532282;1323.05465455383;1300.74402148183;1294.26849512986;1278.74608936281;1267.80879733955;1313.73280951287;1353.05068115347;1352.43596163822;1370.48371974105;1311.1353095457;1329.41560815028;1285.0196036247;1433.78032273718;1457.5479556959;1333.90279914662;1364.56173548464;1373.64679679303;1415.95127588851;1393.59145842546;1376.09087888614;1464.88469438441;1410.01374186609;1450.58864155722;1376.84312096406;1459.91759014253;1409.39298459532;1429.07831741491;1489.34175229004;1499.39634619075;1530.4928662473;1464.55665878826;1455.05074488022;1352.16118539168;1434.49757681188;1540.61872139584;1529.90069057903;1520.13560167111;1537.21195856669;1554.21333358302;1562.26051934629;1562.59924164802;1655.36384538449;1522.98878747559;1626.02824335177;1524.9635718672;1611.94304453833;1600.2298640262;1665.00092848864;1554.96685838367;1592.16733774914";
                        txtFeatures.Text = "";
                        txtLabels.Text = "";
                    }
                    else
                    {
                        MessageBox.Show("Model file " + modelFile + " doesn't exist");
                    }
                }
                else
                {
                    MessageBox.Show("No configuration found");
                }
            }
            else
            {
                MessageBox.Show("Directory " + this.experimentDir + " doesn't exist");
            }
        }

        private void btnDialogEvaluate_Click(object sender, RoutedEventArgs e)
        {
            string modelFilePath = txtModelFile.Text;
            string features = txtFeatures.Text;
            string labels = txtLabels.Text;

            List<string> featureList = null;

            if (!string.IsNullOrEmpty(this.experimentManager.PreProcessing.GetDataFilePath()))
            {
                // Assume input data is a raw data which will be processed to produce feature data
                featureList = GetFeaturesFromPreprocess();
            }
            else
            {
                // no preprocessing, assume the input data is features already
                featureList = GetFeaturesFromTextBox();
            }

            IList<IList<float>> forecastValues = MLModelEvaluator.EvaluateTimeSeriesModel(modelFilePath, featureList, DeviceDescriptor.CPUDevice);

            if (this.experimentManager.PostProcessing != null)
            {
                ShowForecastValues(GetLabelsFromPostprocess(forecastValues));
            }
            else
            {
                // no postprocessing, show forecastLableList
                ShowForecastValues(forecastValues);
            }
        }

        private void Window_ContentRendered(object sender, EventArgs e)
        {
            //txtAnswer.SelectAll();
            //txtAnswer.Focus();
        }

        private void ShowOutputValues(Dictionary<Variable, Value> outputMap)
        {
            foreach (Variable outputVar in outputMap.Keys)
            {
                if (outputVar.Shape.TotalSize > 1)
                {
                    var outputVal = outputMap[outputVar];

                    var outputData = outputVal.GetDenseData<float>(outputVar);

                    var val = outputData[0][0];

                    break;
                }
            }
        }

        private void ShowForecastValues(IList<IList<float>> forecastLabelList)
        {
            StringBuilder sb = new StringBuilder();

            foreach (IList<float> forecastLabels in forecastLabelList)
            {
                foreach (float label in forecastLabels)
                {
                    if (sb.Length > 0)
                    {
                        sb.Append(" ");
                    }
                    sb.Append(label);
                }

                sb.Append(Environment.NewLine);
            }

            this.txtLabels.Text = sb.ToString();
        }

        private List<string> GetFeaturesFromTextBox()
        {
            List<string> featureList = new List<string>();

            if (!string.IsNullOrEmpty(this.txtFeatures.Text))
            {
                using (StringReader reader = new StringReader(this.txtFeatures.Text))
                {
                    string line;
                    while ((line = reader.ReadLine()) != null)
                    {
                        featureList.Add(line);
                    }
                }
            }

            return featureList;
        }

        private List<string> GetFeaturesFromPreprocess()
        {
            List<string> featureList = new List<string>();
            string experimentDataDir = this.experimentDir + @"\" + MLNameSpace.DATA_DIR;

            if (!string.IsNullOrEmpty(this.txtFeatures.Text))
            {
                System.IO.File.WriteAllText(experimentDataDir + @"\" + INPUT_DATA_FILE, this.txtFeatures.Text);

                featureList = RunPreprocessing(experimentDataDir, this.experimentManager.PreProcessing);
            }

            return featureList;
        }

        private List<string> RunPreprocessing(string experimentDataDir, MLPreProcessing preprocessing)
        {
            if (preprocessing.GetProgramFilePath().EndsWith(".py"))
            {
                return RunPythonPreprocessProgram(experimentDataDir, preprocessing);
            }
            else if (preprocessing.GetProgramFilePath().EndsWith(".dll"))
            {
                return RunDotNetPreprocessProgram(experimentDataDir, preprocessing);
            }
            else
            {
                throw new Exception(preprocessing.GetProgramFilePath() + " is an unknown type of program.");
            }
        }

        /// <summary>
        /// Run python preprocess program
        /// </summary>
        /// <param name="experimentDataDir"></param>
        /// <param name="preprocessing"></param>
        private List<string> RunPythonPreprocessProgram(string experimentDataDir, MLPreProcessing preprocessing)
        {
            Process p = new Process();
            ProcessStartInfo start = new ProcessStartInfo();
            string inputDataFile = experimentDataDir + @"\" + INPUT_DATA_FILE;
            start.FileName = MLNameSpace.GetAnacondaDir() + @"\python.exe";
            string trainDataPath = experimentDataDir + @"\" + preprocessing.TrainFileName;
            string testDataPath = experimentDataDir + @"\" + preprocessing.TestFileName;
            string evalDataPath = experimentDataDir + @"\" + preprocessing.EvalFileName;
            start.Arguments = string.Format("{0} {1} {2} {3} {4}", preprocessing.GetProgramFilePath(), inputDataFile, trainDataPath, testDataPath, evalDataPath);
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
        private List<string> RunDotNetPreprocessProgram(string experimentDataDir, MLPreProcessing preprocessing)
        {
            string inputFilePath = experimentDataDir + @"\" + INPUT_DATA_FILE;
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

            return GetProgramOutputData(evalDataPath);
        }

        private IList<IList<float>> GetLabelsFromPostprocess(IList<IList<float>> forecastLableList)
        {
            List<string> labelList = new List<string>();
            string experimentDataDir = this.experimentDir + @"\" + MLNameSpace.DATA_DIR;
            string modelOutputFile = experimentDataDir + @"\" + MODEL_OUTPUT_LABEL_FILE;
            string evalDataFilePath = experimentDataDir + @"\" + this.experimentManager.PreProcessing.EvalFileName;

            using (StreamWriter sw = new StreamWriter(modelOutputFile))
            {
                foreach (IList<float> forecastLabels in forecastLableList)
                {
                    int index = 0;
                    foreach (float label in forecastLabels)
                    {
                        if (index > 0)
                        {
                            sw.Write(" ");
                        }
                        sw.Write(label);

                        index++;
                    }

                    sw.WriteLine();
                }
            }

            labelList = RunPostprocessing(modelOutputFile, evalDataFilePath, experimentDataDir, this.experimentManager.PostProcessing);

            IList<IList<float>> batchList = ConvertToFloatBatch(labelList);

            return batchList;
        }

        /// <summary>
        /// Run the postprocessing
        /// </summary>
        private List<string> RunPostprocessing(string modelOutputFile, string evalDataFilePath, string experimentDataDir, MLPostProcessing postprocessing)
        {
            if (postprocessing.GetProgramFilePath().EndsWith(".py"))
            {
                return RunPythonPostprocessing(modelOutputFile, evalDataFilePath, experimentDataDir, postprocessing);
            }
            else if (postprocessing.GetProgramFilePath().EndsWith(".dll"))
            {
                return RunDotNetPostprocessProgram(modelOutputFile, evalDataFilePath, experimentDataDir, postprocessing);
            }
            else
            {
                throw new Exception(postprocessing.GetProgramFilePath() + " is an unknown type of program.");
            }
        }

        /// <summary>
        /// Run the post processing python script
        /// </summary>
        private List<string> RunPythonPostprocessing(string modelOutputFilePath, string evalDataFilePath, string experimentDataDir, MLPostProcessing postprocessing)
        {
            List<string> forecastLabelList = new List<string>();

            Process p = new Process();
            ProcessStartInfo start = new ProcessStartInfo();
            start.FileName = MLNameSpace.GetAnacondaDir() + @"\python.exe";
            string outputDataPath = experimentDataDir + @"\" + postprocessing.OutputFileName;

            start.Arguments = string.Format("{0} {1} {2} {3}", postprocessing.GetProgramFilePath(), evalDataFilePath, modelOutputFilePath, outputDataPath);
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
        private List<string> RunDotNetPostprocessProgram(string modelOutputFilePath, string evalDataFilePath, string experimentDataDir, MLPostProcessing postprocessing)
        {
            List<string> forecastLabelList = new List<string>();

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
            parameters[0] = modelOutputFilePath;            // model output data file path parameter
            parameters[1] = evalDataFilePath;   // evaluation data file path parameter
            parameters[2] = outputDataPath;   // post process output file path parameter

            // Invoke method 'string Run(int count, string text)'
            var result = methodInfo.Invoke(instance, parameters);

            return GetProgramOutputData(outputDataPath);
        }

        private IList<IList<float>> ConvertToFloatBatch(List<string> textList)
        {
            IList<IList<float>> batchList = new List<IList<float>>();

            foreach (string textLine in textList)
            {
                List<float> batch = new List<float>();
                string[] items = textLine.Split(' ');
                foreach (string item in items)
                {
                    try
                    {
                        float number = float.Parse(item);

                        batch.Add(number);
                    }
                    catch (Exception)
                    {

                    }
                }

                batchList.Add(batch);
            }

            return batchList;
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
    }
}
