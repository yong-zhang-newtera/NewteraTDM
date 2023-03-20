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
    public class MLNetworkBuilder : MLComponentBase
    {
        /// <summary>
        /// Code that descibe the network model
        /// </summary>
        public string NetworkBuilderCode { get; set; }


        public MLNetworkBuilder() : base()
        {
            Name = "Network Builder";
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
            writer.WriteLine(Indent(indentLevel) + "# network description");
            writer.WriteLine(Indent(indentLevel) + "BrainScriptNetworkBuilder = {");

            string[] lines = this.NetworkBuilderCode.Split(new string[] { "\r\n", "\n" }, StringSplitOptions.None);
            for (int i = 0; i < lines.Length; i++)
            {
                writer.WriteLine(Indent(indentLevel + 1) + lines[i]);
            }
            writer.WriteLine("");
            
            writer.WriteLine(Indent(indentLevel) + "}");
        }
    }

}
