using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Newtera.MLActivities.Core;

namespace Newtera.MLActivities.MLConfig
{
    /// <summary>
    /// Definition for a Machine Learning Configuration
    /// </summary>
    [Serializable]
    public class MLConfiguration : MLComponentBase
    {
        private MLExperimentManager experimentManager;
        private int labelDimension;
        private int featureFimension;

        // sub name of configuration
        public string BranchName { get; set; }

        /// <summary>
        /// get full path of the dnn model file
        /// </summary>
        public string ModelFilePath
        {
            get
            {
                string path = MLNameSpace.GetHomeDir() + @"\" + MLNameSpace.EXPERIMENT_DIR + @"\" + this.experimentManager.Name + @"\" + this.Name + @"\model.dnn";
                return path;
            }
        }

        public string PreprocessProgramPath
        {
            get
            {
                return this.experimentManager.PreProcessing.ProgramFile;
            }
        }

        public string PostprocessProgramPath
        {
            get
            {
                return this.experimentManager.PostProcessing.ProgramFile;
            }
        }

        /// <summary>
        /// get model's label dimension
        /// </summary>
        public int LabelDimension {
            get
            {
                return this.labelDimension;
            }
        }

        /// <summary>
        /// Get model's feature dimenison
        /// </summary>
        public int FeatureDimension
        {
            get
            {
                return this.featureFimension;
            }
        }

        public Dictionary<string, string> Variables { get; set; }
        public MLConfiguration(MLExperimentManager experimentManager) : base()
        {
            Name = "MLConfiguration";
            BranchName = null;
            this.experimentManager = experimentManager;

            Variables = new Dictionary<string, string>();
        }

        /// <summary>
        /// Write Machine Learning Configuration code to a writer
        /// </summary>
        /// <param name="writer"></param>
        /// <param name="configType"></param>
        public override void WriteTo(System.IO.StreamWriter writer, int indentLevel, MLConfigurationType configType)
        {
            if (configType == MLConfigurationType.CNTK)
            {
                WriteCNTKTo(writer, indentLevel);
            }
            else if (configType == MLConfigurationType.TensorFlow)
            {
                WriteTensorFlowTo(writer, indentLevel);
            }
        }

        /// <summary>
        /// Accept a visitor of IMLComponnetVisitor type to visit itself and
        /// let its children to accept the visitor next.
        /// </summary>
        /// <param name="visitor">The visitor</param>
        /// <returns>true to continue travers, false to stop</returns>
        public override bool Accept(IMLComponnetVisitor visitor)
        {
            bool status = visitor.VisitConfiguration(this);
            // top down traverse the tree
            if (status)
            {
                foreach (IMLComponnet component in Children)
                {
                    status = component.Accept(visitor);
                    if (!status)
                    {
                        break;
                    }
                }
            }

            return status;
        }

        /// <summary>
        /// Copy this IMLComponent object
        /// </summary>
        public override void Copy(IMLComponnet copy)
        {
            base.Copy(copy);

            ((MLConfiguration)copy).Variables = new Dictionary<string, string>();
            // write variables
            foreach (string key in this.Variables.Keys)
            {
                ((MLConfiguration)copy).Variables[key] = this.Variables[key];
            }
        }

        protected override void WriteCNTKTo(StreamWriter writer, int indentLevel)
        {
            writer.WriteLine("# CNTK Machine Learning Configuration");
            writer.WriteLine("# commands to run");
            writer.Write("command=");
            int index = 0;
            foreach (IMLComponnet component in this.Children)
            {
                if (component is MLCommand && ((MLCommand) component).Enabled)
                {
                    if (index > 0)
                    {
                        writer.Write(":");
                    }
                    writer.Write(((MLCommand) component).Action);
                    index++;
                }
            }
            writer.WriteLine("");

            // write variables
            foreach (string key in this.Variables.Keys)
            {
                writer.WriteLine(key + " = " + this.Variables[key]);
            }

            // Write command configuration
            foreach (IMLComponnet component in this.Children)
            {
                if (component is MLCommand)
                {
                    component.WriteTo(writer, indentLevel + 1, MLConfigurationType.CNTK);
                    writer.WriteLine();
                }
            }

            // write feature dimension and label dimension from the train_data_file which is obtained from the reader activty in the Train command
            // must run this function after the write command configuration since the Train_Data_File value is set as result of that
            WriteDimensions(writer, experimentManager);
        }

        private void WriteDimensions(StreamWriter writer, MLExperimentManager experimentManager)
        {
            string FeatureHeader = "|features ";
            string LabelHeader = "|labels ";

            if (!string.IsNullOrEmpty(experimentManager.TrainDataFile))
            {
                using (StreamReader sr = new StreamReader(experimentManager.TrainDataFile))
                {
                    string line = sr.ReadLine();
                    if (!string.IsNullOrEmpty(line))
                    {
                        int pos = line.IndexOf(FeatureHeader);
                        if (pos >= 0)
                        {
                            string featureData = line.Substring(pos + FeatureHeader.Length);
                            this.featureFimension = GetNumberCount(featureData);

                            writer.WriteLine("dimension = " + this.featureFimension);
                        }
                        else
                        {
                            throw new Exception(experimentManager.TrainDataFile + " is an unknown format. Missing '|features'.");
                        }

                        pos = line.IndexOf(LabelHeader);
                        if (pos >= 0)
                        {
                            string labelData = line.Substring(pos + LabelHeader.Length);
                            this.labelDimension = GetNumberCount(labelData);
                            writer.WriteLine("labelDimension = " + this.labelDimension);
                        }
                        else
                        {
                            throw new Exception(experimentManager.TrainDataFile + " is an unknown format. Missing '|labels'.");
                        }
                    }
                    else
                    {
                        throw new Exception(experimentManager.TrainDataFile + " is empty.");
                    }
                }
            }
            else
            {
                throw new Exception("TrainDataFile is empty");
            }
        }

        private int GetNumberCount(string line)
        {
            int count = 0;

            string[] items = line.Split(' '); // items are separated by space
            for (int i = 0; i < items.Length; i++)
            {
                // count the items that are number, stop if it reaches a non-number
                try
                {
                    double.Parse(items[i]);

                    count++;
                }
                catch (Exception)
                {
                    break;
                }
            }

            return count;
        }
    }

}
