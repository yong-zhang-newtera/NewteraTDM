using System;
using System.Collections;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Newtera.MLActivities.MLConfig
{
    /// <summary>
    /// Main class of a Machine Learning Configuration
    /// </summary>
    [Serializable]
    public abstract class MLComponentBase: IMLComponnet
    {
        public string Name { get; set; }

        public IList Children { get; set; }

        public MLComponentBase()
        {
            Children = new MLComponentCollection();
        }

        /// <summary>
        /// Write Machine Learning Configuration code to a writer
        /// </summary>
        /// <param name="writer"></param>
        /// <param name="configType"></param>
        public abstract void WriteTo(System.IO.StreamWriter writer, int indentLevel, MLConfigurationType configType);

        /// <summary>
        /// Accept a visitor of IMLComponnetVisitor type to visit itself and
        /// let its children to accept the visitor next.
        /// </summary>
        /// <param name="visitor">The visitor</param>
        /// <returns>true to continue travers, false to stop</returns>
        public abstract bool Accept(IMLComponnetVisitor visitor);

        /// <summary>
        /// Copy this IMLComponent object
        /// </summary>
        public virtual void Copy(IMLComponnet copy)
        {
            copy.Name = this.Name;
            copy.Children = new MLComponentCollection();
        }

        protected virtual void WriteCNTKTo(System.IO.StreamWriter writer, int indentLevel)
        {
            // overrided by subclass
        }

        protected virtual void WriteTensorFlowTo(System.IO.StreamWriter writer, int indentLevel)
        {
            // overrided by subclass
        }

        protected string Indent(int indentLevel)
        {
            string indent = new string(' ', indentLevel * 4);
            return indent;
        }
    }
}
