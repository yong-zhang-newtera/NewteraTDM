using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using System.Threading.Tasks;

namespace Newtera.MLActivities.MLConfig
{
    /// <summary>
    /// Class definition for a Machine Learning Command
    /// </summary>
    [Serializable]
    public class MLCommand : MLComponentBase
    {
        public string Action { get; set; }

        public bool Enabled { get; set; }

        public Dictionary<string, string> Variables { get; set; }

        public MLCommand() : base()
        {
            Name = "Command";
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
            bool status = visitor.VisitCommand(this);
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

            ((MLCommand)copy).Action = this.Action;

            ((MLCommand)copy).Variables = new Dictionary<string, string>();
            // write variables
            foreach (string key in this.Variables.Keys)
            {
                ((MLCommand)copy).Variables[key] = this.Variables[key];
            }
        }

        protected override void WriteCNTKTo(StreamWriter writer, int indentLevel)
        {
            // comment
            writer.Write("# ");
            writer.Write(Action);
            writer.WriteLine(" command");

            // write command body
            writer.WriteLine(Action + " = {");
            writer.WriteLine(Indent(indentLevel) + "action=\"" + GetCommandAction(Action) + "\"");
            writer.WriteLine("");

            foreach (IMLComponnet child in this.Children)
            {
                child.WriteTo(writer, indentLevel, MLConfigurationType.CNTK);
                writer.WriteLine("");
            }

            foreach (string key in Variables.Keys)
            {
                writer.WriteLine(Indent(indentLevel) + key + " = " + Variables[key]);
            }

            writer.WriteLine("}");
        }

        private string GetCommandAction(string actionName)
        {
            string action = null;

            switch (actionName)
            {
                case "Train":
                    action = "train";
                    break;

                case "Test":
                    action = "test";
                    break;

                case "Output":
                    action = "write";
                    break;
            }

            return action;
        }
    }

}
