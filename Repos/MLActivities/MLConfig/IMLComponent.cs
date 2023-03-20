using System;
using System.Collections;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Newtera.MLActivities.MLConfig
{
    /// <summary>
    /// Interface definition for components in ML configuration
    /// </summary>
    public interface IMLComponnet
    {
        string Name { get; set; }

        IList Children { get; set; }

        void WriteTo(System.IO.StreamWriter writer, int indentLevel, MLConfigurationType configType);

        /// <summary>
        /// Accept a visitor of IMLComponnetVisitor type to visit itself and
        /// let its children to accept the visitor next.
        /// </summary>
        /// <param name="visitor">The visitor</param>
        /// <returns>true to continue travers, false to stop</returns>
        bool Accept(IMLComponnetVisitor visitor);
    }
}
