using System;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Newtera.MLActivities.MLConfig
{
    /// <summary>
    /// Managing a collection of Machine Learning Configurations
    /// </summary>
    public class MLExperimentManager
    {
        private MLComponentCollection executableConfigurations;

        public string Name { get; set; }

        public MLPreProcessing PreProcessing { get; set; }

        public MLPostProcessing PostProcessing { get; set; }

        public MLComponentCollection Configurations { get; set; }
        public string TrainDataFile { get; set; }
        public string TestDataFile { get; set; }
        public string EvalDataFile { get; set; }
        public MLExperimentManager()
        {
            Configurations = new MLComponentCollection();

            executableConfigurations = null;

            PreProcessing = null;
            PostProcessing = null;

            TrainDataFile = null;
            TestDataFile = null;
            EvalDataFile = null;
        }

        public void Reset()
        {
            executableConfigurations = null;
        }

        public MLComponentCollection ExecutableConfigurations
        {
            get
            {
                if (executableConfigurations == null)
                {
                    executableConfigurations = new MLComponentCollection();

                    ConfigurationCountVisitor configurationCountVisitor;
                    ReplaceCollectionNodeVisitor replaceCollectionNodeVisitor;

                    foreach (MLConfiguration mlConfiguration in Configurations)
                    {
                        configurationCountVisitor = new ConfigurationCountVisitor();
                        mlConfiguration.Accept(configurationCountVisitor);

                        if (configurationCountVisitor.ExecutableConfigurationCount > 0)
                        {
                            // the configuration contains a collection of child compoment
                            // create copies of the configuration, and replace the collection with one of the child component
                            // for each copy
                            for (int i = 0; i < configurationCountVisitor.ExecutableConfigurationCount; i++)
                            {
                                MLConfiguration clonedConfiguration = MLComponnetVisitorBase.DeepClone<MLConfiguration>(mlConfiguration);

                                replaceCollectionNodeVisitor = new ReplaceCollectionNodeVisitor(i);

                                // replace the collection node with a child component of given index
                                clonedConfiguration.Accept(replaceCollectionNodeVisitor);

                                executableConfigurations.Add(clonedConfiguration);
                            }
                        }
                        else
                        {
                            // the configuration doesn't contain a collection of child compoment
                            // no need to split the congiguration
                            executableConfigurations.Add(mlConfiguration);
                        }
                    }
                }

                return executableConfigurations;
            }
        }
    }

}
