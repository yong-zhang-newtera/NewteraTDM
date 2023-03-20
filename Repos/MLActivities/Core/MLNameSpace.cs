using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Configuration;

namespace Newtera.MLActivities.Core
{
    public class MLNameSpace
    {
        public const string BASE_URL = "ServiceUri";
        public const string HOME_DIR = "HomeDir";
        public const string CNTK_DIR = "CNTKDir";
        public const string ANACONDA_DIR = "AnacondaDir";
        public const string EXPERIMENT_DIR = "Experiments";
        public const string LOG_DIR = "log";
        public const string DATA_DIR = "data";
        public const string NETWORK_TEMPLATE_DIR = "NetworkTemplates";
        public const string WORKFLOW_TEMPLATE_DIR = "Templates";
        public const string HOME_DIR_VAR = "{HOME_DIR}";

        internal static string HomeDir = null;

        internal static string ServerUri = null;

        internal static string CNTKDir = null;

        internal static string AnacondaDir = null;

        public static string GetHomeDir()
        {
            if (MLNameSpace.HomeDir == null)
            {
                //Create the object
                Configuration config = ConfigurationManager.OpenExeConfiguration(ConfigurationUserLevel.None);

                // get uri
                if (config.AppSettings.Settings[MLNameSpace.HOME_DIR] != null)
                {
                    MLNameSpace.HomeDir = config.AppSettings.Settings[MLNameSpace.HOME_DIR].Value.Trim();
                    if (MLNameSpace.HomeDir.EndsWith(@"\"))
                    {
                        MLNameSpace.HomeDir = MLNameSpace.HomeDir.Substring(0, MLNameSpace.HomeDir.Length - 1);
                    }
                }
            }

            return MLNameSpace.HomeDir;
        }

        public static string GetServerUri()
        {
            if (MLNameSpace.ServerUri == null)
            {
                //Create the object
                Configuration config = ConfigurationManager.OpenExeConfiguration(ConfigurationUserLevel.None);

                // get uri
                if (config.AppSettings.Settings[MLNameSpace.BASE_URL] != null)
                {
                    MLNameSpace.ServerUri = config.AppSettings.Settings[MLNameSpace.BASE_URL].Value;
                }
            }

            return MLNameSpace.ServerUri;
        }

        public static string GetCNTKDir()
        {
            if (MLNameSpace.CNTKDir == null)
            {
                //Create the object
                Configuration config = ConfigurationManager.OpenExeConfiguration(ConfigurationUserLevel.None);

                // get uri
                if (config.AppSettings.Settings[MLNameSpace.CNTK_DIR] != null)
                {
                    MLNameSpace.CNTKDir = config.AppSettings.Settings[MLNameSpace.CNTK_DIR].Value;
                }
            }

            return MLNameSpace.CNTKDir;
        }

        public static string GetAnacondaDir()
        {
            if (MLNameSpace.AnacondaDir == null)
            {
                //Create the object
                Configuration config = ConfigurationManager.OpenExeConfiguration(ConfigurationUserLevel.None);

                // get uri
                if (config.AppSettings.Settings[MLNameSpace.ANACONDA_DIR] != null)
                {
                    MLNameSpace.AnacondaDir = config.AppSettings.Settings[MLNameSpace.ANACONDA_DIR].Value;
                }
            }

            return MLNameSpace.AnacondaDir;
        }
    }
}
