using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using System.Threading.Tasks;

using Newtera.MLActivities.Core;

namespace Newtera.MLActivities.MLConfig
{
    /// <summary>
    /// Definition for data reader
    /// </summary>
    [Serializable]
    public class MLPreProcessing : MLComponentBase
    {
        public string DataFile { get; set; }

        public string ProgramFile { get; set; }

        public string TrainFileName { get; set; }

        public string TestFileName { get; set; }

        public string EvalFileName { get; set; }

        /// <summary>
        /// input data dimensions
        /// </summary>
        public int FeatureDimension { get; set; }

        /// <summary>
        /// label data  dimensions
        /// </summary>
        public int LabelDimension { get; set; }

        public bool Enabled { get; set; }

        public MLPreProcessing() : base()
        {
            Name = "PreProcessing";
        }

        public string GetDataFilePath()
        {
            if (this.DataFile.Contains(MLNameSpace.HOME_DIR_VAR))
            {
                return this.DataFile.Replace(MLNameSpace.HOME_DIR_VAR, MLNameSpace.GetHomeDir());
            }
            else
            {
                return this.DataFile;
            }
        }

        public string GetProgramFilePath()
        {
            if (this.ProgramFile.Contains(MLNameSpace.HOME_DIR_VAR))
            {
                return this.ProgramFile.Replace(MLNameSpace.HOME_DIR_VAR, MLNameSpace.GetHomeDir());
            }
            else
            {
                return this.ProgramFile;
            }
        }

        /// <summary>
        /// Write Machine Learning Configuration code to a writer
        /// </summary>
        /// <param name="writer"></param>
        /// <param name="configType"></param>
        public override void WriteTo(System.IO.StreamWriter writer, int indentLevel, MLConfigurationType configType)
        {
           
        }

        /// <summary>
        /// Accept a visitor of IMLComponnetVisitor type to visit itself and
        /// let its children to accept the visitor next.
        /// </summary>
        /// <param name="visitor">The visitor</param>
        /// <returns>true to continue travers, false to stop</returns>
        public override bool Accept(IMLComponnetVisitor visitor)
        {
            return visitor.VisitCommonComponent(this);
        }

        /// <summary>
        /// Copy this IMLComponent object
        /// </summary>
        public override void Copy(IMLComponnet copy)
        {
            base.Copy(copy);

            ((MLPreProcessing)copy).DataFile = this.DataFile;
            ((MLPreProcessing)copy).ProgramFile = this.ProgramFile;
            ((MLPreProcessing)copy).TrainFileName = this.TrainFileName;
            ((MLPreProcessing)copy).TestFileName = this.TestFileName;
            ((MLPreProcessing)copy).EvalFileName = this.EvalFileName;
            ((MLPreProcessing)copy).FeatureDimension = this.FeatureDimension;
            ((MLPreProcessing)copy).LabelDimension = this.LabelDimension;
        }
    }

}
