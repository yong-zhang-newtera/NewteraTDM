using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Newtera.MLServer.Evaluate
{
    /// <summary>
    /// Descriptive information of a machine learning model
    /// </summary>
    public class MLModelInfo
    {
        private IList<MLVariableInfo> inputVariables;
        private IList<MLVariableInfo> outputVariables;

        public MLModelInfo()
        {
            inputVariables = new List<MLVariableInfo>();
            outputVariables = new List<MLVariableInfo>();
        }

        /// <summary>
        /// Model name
        /// </summary>
        public string Name {get; set;}

        /// <summary>
        /// Model description
        /// </summary>
        public string Description { get; set; }

        /// <summary>
        /// Model input variables
        /// </summary>
        public IList<MLVariableInfo> InputVariables
        {
            get
            {
                return inputVariables;
            }
        }

        /// <summary>
        /// Model output variables
        /// </summary>
        public IList<MLVariableInfo> OutputVariables
        {
            get
            {
                return outputVariables;
            }
        }
    }

    public class MLVariableInfo
    {
        public string Name { get; set; }

        public string DataType { get; set; }

        public int Dimension { get; set; }
    }
}
