using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using System.Threading.Tasks;

namespace Newtera.MLActivities.MLConfig
{
    /// <summary>
    /// Definition for Logistic Regression Network
    /// </summary>
    [Serializable]
    public class MLSimpleNetworkBuilder : MLComponentBase
    {
        public string InputDimension { get; set; }

        public string OutputDimension { get; set; }

        /// <summary>
        /// Code that descibe the network model
        /// </summary>
        public string HiddenLayers { get; set; }

        /// <summary>
        /// Layer type, Sigmoid, Tanh, and RectifiedLinear
        /// </summary>
        public string LayerTypes { get; set; }

        /// <summary>
        /// Criteria function used to training
        /// </summary>
        public string TrainingCriterion { get; set; }

        /// <summary>
        /// Evaluate function used to evaluate model
        /// </summary>
        public string EvalCriterion { get; set; }

        public bool AddDropoutNodes { get; set; }

        public MLSimpleNetworkBuilder() : base()
        {
            Name = "Network Simple Builder";
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
        /// Copy this IMLComponent object
        /// </summary>
        public override void Copy(IMLComponnet copy)
        {
            base.Copy(copy);
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

        protected override void WriteCNTKTo(StreamWriter writer, int indentLevel)
        {
            writer.WriteLine(Indent(indentLevel) + "# simple network description");
            writer.WriteLine(Indent(indentLevel) + "SimpleNetworkBuilder = {");
            writer.WriteLine("");
            writer.WriteLine(Indent(indentLevel + 1) + "layerSizes = " + this.InputDimension + ":" + this.HiddenLayers + ":" + this.OutputDimension);
            writer.WriteLine(Indent(indentLevel + 1) + "trainingCriterion = " + this.TrainingCriterion);
            writer.WriteLine(Indent(indentLevel + 1) + "evalCriterion = " + this.EvalCriterion);
            writer.WriteLine(Indent(indentLevel + 1) + "layerTypes  = " + this.LayerTypes);
            writer.WriteLine(Indent(indentLevel + 1) + "addDropoutNodes = " + (this.AddDropoutNodes? "true" : "false"));
            writer.WriteLine(Indent(indentLevel) + "}");
        }
    }

}
