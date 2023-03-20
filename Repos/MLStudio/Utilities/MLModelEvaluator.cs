using System;
using System.Collections.Generic;
using System.Linq;
using System.IO;
using System.Text;
using System.Threading.Tasks;
using CNTK;

namespace Newtera.MLStudio.Utilities
{
    /// <summary>
    /// Evaluate the model
    /// </summary>
    public class MLModelEvaluator
    {
        /// <summary>
        /// The example shows
        /// - how to load model.
        /// - how to prepare input data for a single sample.
        /// - how to prepare input and output data map.
        /// - how to evaluate a model.
        /// - how to retrieve evaluation result and retrieve output data in dense format.
        /// </summary>
        /// <param name="modelFilePath">the model file path</param>
        /// <param name="inputValues">Semi-colon separated values</param>
        /// <param name="device">Specify on which device to run the evaluation.</param>
        public static Dictionary<Variable, Value> EvaluateModel(string modelFilePath, string features, string labels, DeviceDescriptor device)
        {
            try
            {
                // Load the model.
                ThrowIfFileNotExist(modelFilePath, string.Format("Error: The model '{0}' does not exist. Please follow instructions in README.md in <CNTK>/Examples/Image/Classification/ResNet to create the model.", modelFilePath));

                Function modelFunc = Function.Load(modelFilePath, device);

                var inputDataMap = new Dictionary<Variable, Value>();

                // Get input variable. The model has only one single input.
                // The same way described above for output variable can be used here to get input variable by name.
                IList<Variable> inputVars = modelFunc.Arguments;
                IList<float> batch;
                foreach (Variable inputVar in inputVars)
                {
                    if (inputVar.Name == "features")
                    {
                        batch = CreateFloatBatch(features);
                        inputDataMap.Add(inputVar, Value.CreateBatch<float>(inputVar.Shape, batch, device));
                        //Value.CreateDenseDouble(inputVar.Shape, 
                    }
                    else if (inputVar.Name == "labels")
                    {
                        batch = CreateFloatBatch(labels);
                        inputDataMap.Add(inputVar, Value.CreateBatch<float>(inputVar.Shape, batch, device));
                    }
                }

                var outputDataMap = new Dictionary<Variable, Value>();

                // The model has only one output.
                // If the model have more than one output, use the following way to get output variable by name.
                // Variable outputVar = modelFunc.Outputs.Where(variable => string.Equals(variable.Name, outputName)).Single();
                IList<Variable> outputVars = modelFunc.Outputs;
                foreach (Variable outputVar in outputVars)
                {
                    outputDataMap.Add(outputVar, null);
                }

                // Start evaluation on the device
                modelFunc.Evaluate(inputDataMap, outputDataMap, device);

                return outputDataMap;
            }
            catch (Exception ex)
            {
                Console.WriteLine("Error: {0}\nCallStack: {1}\n Inner Exception: {2}", ex.Message, ex.StackTrace, ex.InnerException != null ? ex.InnerException.Message : "No Inner Exception");

                throw ex;
            }
        }

        /// <summary>
        /// Evaluate time series model
        /// - how to load model.
        /// - how to prepare input data for a single sample.
        /// - how to prepare input and output data map.
        /// - how to evaluate a model.
        /// - how to retrieve evaluation result and retrieve output data in dense format.
        /// </summary>
        public static IList<IList<float>> EvaluateTimeSeriesModel(string modelFilePath, List<string> featureList, DeviceDescriptor device)
        {
            try
            {
                IList<IList<float>> sequenceList;

                // Load the model.
                ThrowIfFileNotExist(modelFilePath, string.Format("Error: The model '{0}' does not exist. Please follow instructions in README.md in <CNTK>/Examples/Image/Classification/ResNet to create the model.", modelFilePath));

                Function modelFunc = Function.Load(modelFilePath, device);
                int labelDimension = 0;

                // Build a batch that includes all fearure values
                IList<float> batch = new List<float>();
                foreach (string features in featureList)
                {
                    AddFeaturesToBatch(batch, features);
                }

                var inputDataMap = new Dictionary<Variable, Value>();
                var outputDataMap = new Dictionary<Variable, Value>();

                IList<Variable> inputVars = modelFunc.Arguments;
                foreach (Variable inputVar in inputVars)
                {
                    if (inputVar.Name == "features")
                    {
                        inputDataMap.Add(inputVar, Value.CreateSequence<float>(inputVar.Shape, batch, device)); // Create a Sequnece as value as model input
                    }
                    else if (inputVar.Name == "labels")
                    {
                        // label input is required input, therefore create a fake label batch
                        labelDimension = inputVar.Shape.TotalSize;
                    }
                }

                IList<Variable> outputVars = modelFunc.Outputs;
                foreach (Variable outputVar in outputVars)
                {
                    if (outputVar.Shape.TotalSize == labelDimension)
                    {
                        outputDataMap.Add(outputVar, null);
                    }
                }

                // Start evaluation on the device
                modelFunc.Evaluate(inputDataMap, outputDataMap, device);

                var sequence = GetForecastSequence(outputDataMap, labelDimension);

                // the output of the model is a whole sequence, we need to break it into multiple sequences of LabelDimension size
                // since it is what the post-processing expects
                sequenceList = GetSequenceList(sequence, labelDimension);

                return sequenceList;
            }
            catch (Exception ex)
            {
                Console.WriteLine("Error: {0}\nCallStack: {1}\n Inner Exception: {2}", ex.Message, ex.StackTrace, ex.InnerException != null ? ex.InnerException.Message : "No Inner Exception");

                throw ex;
            }
        }

        private static IList<float> CreateFeatureBatch(string line)
        {
            IList<float> batch = new List<float>();
            string data = line; 
            if (!string.IsNullOrEmpty(data))
            {
                int pos = data.IndexOf("features");
                if (pos >= 0)
                {
                    data = data.Substring(pos + 9); // skip the last space too
                    pos = data.IndexOf("|#");
                    if (pos > 0)
                    {
                        // remove the trailing comment
                        data = data.Substring(0, pos - 1);
                    }

                    string[] array = data.Split(' ');
                    for (int i = 0; i < array.Length; i++)
                    {
                        float num = 0;
                        try
                        {
                            num = float.Parse(array[i]);
                            batch.Add(num);
                        }
                        catch (Exception)
                        {
                        }
                    }
                }
            }

            return batch;
        }

        private static IList<IList<float>> GetSequenceList(IList<float> bigSequence, int labelDimension)
        {
            IList<IList<float>> sequenceList = new List<IList<float>>();

            int count = 0;
            IList<float> sequence = null;
            bool isAdded = false;
            foreach (float val in bigSequence)
            {
                if (count == 0)
                {
                    isAdded = false;
                    sequence = new List<float>();
                }

                sequence.Add(val);

                count++;

                if (count == labelDimension)
                {
                    // start a new sequence
                    count = 0;

                    sequenceList.Add(sequence);
                    isAdded = true;
                }
            }

            if (!isAdded)
            {
                sequenceList.Add(sequence);
            }

            return sequenceList;
        }

        private static void AddFeaturesToBatch(IList<float> batch, string line)
        {
            string data = line;
            if (!string.IsNullOrEmpty(data))
            {
                int pos = data.IndexOf("features");
                if (pos >= 0)
                {
                    data = data.Substring(pos + 9); // skip the last space too
                    pos = data.IndexOf("|#");
                    if (pos > 0)
                    {
                        // remove the trailing comment
                        data = data.Substring(0, pos - 1);
                    }

                    string[] array = data.Split(' ');
                    for (int i = 0; i < array.Length; i++)
                    {
                        float num = 0;
                        try
                        {
                            num = float.Parse(array[i]);
                            batch.Add(num);
                        }
                        catch (Exception)
                        {
                        }
                    }
                }
            }
        }

        internal static IList<float> CreateFloatBatch(string data)
        {
            IList<float> batch = new List<float>();
            if (!string.IsNullOrEmpty(data))
            {
                string[] array = data.Split(' ');
                for (int i = 0; i < array.Length; i++)
                {
                    float num = 0;
                    try
                    {
                        num = float.Parse(array[i]);
                        batch.Add(num);
                    }
                    catch (Exception)
                    {
                    }
                }
            }

            return batch;
        }

        private static IList<float> GetForecastSequence(Dictionary<Variable, Value> outputMap, int labelDimension)
        {
            IList<float> sequence = null;

            foreach (Variable outputVar in outputMap.Keys)
            {
                if (outputVar.Shape.TotalSize == labelDimension)
                {
                    var outputVal = outputMap[outputVar];

                    var outputData = outputVal.GetDenseData<float>(outputVar);

                    sequence = outputData[0];

                    break;
                }
            }

            return sequence;
        }

        /// <summary>
        /// Checks whether the file exists. If not, write the error message on the console and throw FileNotFoundException.
        /// </summary>
        /// <param name="filePath">The file to check.</param>
        /// <param name="errorMsg">The message to write on console if the file does not exist.</param>
        internal static void ThrowIfFileNotExist(string filePath, string errorMsg)
        {
            if (!File.Exists(filePath))
            {
                if (!string.IsNullOrEmpty(errorMsg))
                {
                    Console.WriteLine(errorMsg);
                }

                throw new FileNotFoundException(string.Format("File '{0}' not found.", filePath));
            }
        }

        /// <summary>
        /// Print out the evalaution results.
        /// </summary>
        /// <typeparam name="T">The data value type</typeparam>
        /// <param name="sampleSize">The size of each sample.</param>
        /// <param name="outputBuffer">The evaluation result data.</param>
        private static void PrintOutput<T>(int sampleSize, IList<IList<T>> outputBuffer)
        {
            Console.WriteLine("The number of sequences in the batch: " + outputBuffer.Count);

            int seqNo = 0;
            int outputSampleSize = sampleSize;

            foreach (var seq in outputBuffer)
            {
                if (seq.Count % outputSampleSize != 0)
                {
                    throw new ApplicationException("The number of elements in the sequence is not a multiple of sample size");
                }

                Console.WriteLine(String.Format("Sequence {0} contains {1} samples.", seqNo++, seq.Count / outputSampleSize));
                int i = 0;
                int sampleNo = 0;
                foreach (var element in seq)
                {
                    if (i++ % outputSampleSize == 0)
                    {
                        Console.Write(String.Format("    sample {0}: ", sampleNo));
                    }

                    Console.Write(element);

                    if (i % outputSampleSize == 0)
                    {
                        Console.WriteLine(".");

                        sampleNo++;
                    }
                    else
                    {
                        Console.Write(",");
                    }
                }
            }
        }
    }
}
