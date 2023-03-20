using System;
using System.Collections.Generic;
using System.Linq;
using System.IO;
using System.Text;
using System.Threading.Tasks;

namespace Newtera.MLActivities.MLConfig
{
    /// <summary>
    /// Definition for Stochastic gradient descent
    /// </summary>
    [Serializable]
    public class MLSGD : MLComponentBase
    {
        public string EpochSize { get; set; }

        public string MinibatchSize { get; set; }

        public string LearningRatesPerSample { get; set; }

        public string MaxEpochs { get; set; }

        public string MomentumAsTimeConstant { get; set; }

        public string NumMBsToShowResult { get; set; }

        public MLSGD() : base()
        {
            Name = "SGD";
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
            return visitor.VisitSGD(this);
        }

        /// <summary>
        /// Copy this IMLComponent object
        /// </summary>
        public override void Copy(IMLComponnet copy)
        {
            base.Copy(copy);

            ((MLSGD)copy).EpochSize = this.EpochSize;
            ((MLSGD)copy).MinibatchSize = this.MinibatchSize;
            ((MLSGD)copy).LearningRatesPerSample = this.LearningRatesPerSample;
            ((MLSGD)copy).MaxEpochs = this.MaxEpochs;
        }

        protected override void WriteCNTKTo(StreamWriter writer, int indentLevel)
        {
            writer.WriteLine(Indent(indentLevel) + "# configuration parameters of the SGD procedure");

            writer.WriteLine(Indent(indentLevel) + "SGD = {");
            writer.WriteLine(Indent(indentLevel + 1) + "epochSize = " + this.EpochSize + "                 # =0 means size of the training set");
            writer.WriteLine(Indent(indentLevel + 1) + "minibatchSize = " + this.MinibatchSize);
            writer.WriteLine(Indent(indentLevel + 1) + "learningRatesPerSample = " + this.LearningRatesPerSample + "  # gradient contribution from each sample");
            writer.WriteLine(Indent(indentLevel + 1) + "maxEpochs = " + this.MaxEpochs);
            if (!string.IsNullOrEmpty(this.MomentumAsTimeConstant))
            {
                writer.WriteLine(Indent(indentLevel + 1) + "momentumAsTimeConstant = " + this.MomentumAsTimeConstant);
            }
            if (!string.IsNullOrEmpty(this.NumMBsToShowResult))
            {
                writer.WriteLine(Indent(indentLevel + 1) + "numMBsToShowResult = " + this.NumMBsToShowResult);
            }
            //writer.WriteLine(Indent(indentLevel + 1) + "tensorBoardLogDir = \"log\"");
            //writer.WriteLine(Indent(indentLevel + 1) + "tensorBoardNumMBsToLogResult = 0");
            writer.WriteLine(Indent(indentLevel) + "}");
        }
    }

}
