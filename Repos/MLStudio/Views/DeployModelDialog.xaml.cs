using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

using CNTK;

using Newtera.MLActivities.Core;
using Newtera.MLStudio.Utilities;
using Newtera.MLStudio.ViewModel;
using Newtera.MLStudio.WebApi;
using Newtera.MLActivities.MLConfig;

namespace Newtera.MLStudio.Views
{
    /// <summary>
    /// DeployModelDialog.xaml 的交互逻辑
    /// </summary>
    public partial class DeployModelDialog : Window
    {
        private SchemaInfo schemaInfo;
        private MLExperimentManager configurationManager;
        private MLModelServiceStub modelService;
        private MetaDataServiceStub metaDataService;
        private string selectedClassName;
        private string selectedXMLSchemaName;
        private string selectedModelName;
        private string selectedTimeSeriesName;
        private string selectedFrequency;
        private string maxForecastHorizon;
        private JObject rootNode;
        private int currentPage;
        private JArray dataSchemas;
        private JArray xmlSchemaFields;
        private JArray timeSeriesFrequencies;
        private MLComponentCollection executableConfigurations;
        private bool isDateTimeXAxis;

        public DeployModelDialog(MLExperimentManager configurationManager, SchemaInfo schemaInfo)
        {
            this.configurationManager = configurationManager;
            this.schemaInfo = schemaInfo;
            this.selectedClassName = null;
            this.selectedXMLSchemaName = null;
            this.selectedModelName = null;
            this.selectedTimeSeriesName = null;
            this.selectedFrequency = null;
            this.currentPage = 1;
            this.dataSchemas = null;
            this.xmlSchemaFields = null;
            this.maxForecastHorizon = null;
            this.executableConfigurations = null;
            this.isDateTimeXAxis = false;

            this.timeSeriesFrequencies = GetTimeSeriesFrequencies();

            this.modelService = new MLModelServiceStub();
            this.modelService.BaseUri = MLNameSpace.GetServerUri();

            this.metaDataService = new MetaDataServiceStub();
            this.metaDataService.BaseUri = MLNameSpace.GetServerUri();

            InitializeComponent();
        }

        private void Window_ContentRendered(object sender, EventArgs e)
        {
            
        }

        private void listBoxModels_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (this.listBoxModels.SelectedIndex >= 0)
            {
                this.selectedModelName = this.listBoxModels.SelectedItem.ToString();
                this.txtModel.Text = this.selectedModelName;
                this.nextBtn.IsEnabled = true;
            }
        }

        private void Grid_Initialized(object sender, EventArgs e)
        {
            string classTreeStr = this.metaDataService.GetClassTree(ConnectionStringBuilder.Instance.Create(this.schemaInfo));
            this.rootNode = JsonConvert.DeserializeObject<JObject>(classTreeStr);

            TreeViewItem treeRoot = BuildClassTree(this.rootNode);
                
            this.classTree.Items.Add(treeRoot);

            if (this.configurationManager != null)
            {
                this.executableConfigurations = this.configurationManager.ExecutableConfigurations;
                string modelName;
                this.listBoxModels.Items.Clear();
                foreach (MLConfiguration executableConfig in this.executableConfigurations)
                {
                    modelName = executableConfig.Name;
                    if (!string.IsNullOrEmpty(executableConfig.BranchName))
                    {
                        modelName += @"\" + executableConfig.BranchName;
                    }
                    this.listBoxModels.Items.Add(modelName);
                }

                if (this.executableConfigurations.Count > 0)
                {
                    this.listBoxModels.SelectedIndex = 0; // select the first model by default
                }
            }
        }

        private TreeViewItem BuildClassTree(JObject classTreeRoot)
        {
            TreeViewItem root = null;

            root = CreateTreeViewItem(classTreeRoot, true, true);

            AddChildNodes(root, classTreeRoot);

            return root;
        }

        private void AddChildNodes(TreeViewItem parentTreeItem, JObject parentNode)
        {
            if (parentNode["children"] != null)
            {
                JArray childNodes = parentNode["children"] as JArray;

                foreach (JObject childNode in childNodes)
                {
                    TreeViewItem childItem = CreateTreeViewItem(childNode, false, false);

                    parentTreeItem.Items.Add(childItem);

                    // add children recursively
                    AddChildNodes(childItem, childNode);
                }
            }
        }

        private TreeViewItem CreateTreeViewItem(JObject node, bool isExpanded, bool isRoot)
        {
            TreeViewItem item = new TreeViewItem();

            item.IsExpanded = isExpanded;

            // create stack panel
            StackPanel stack = new StackPanel();
            stack.Orientation = Orientation.Horizontal;

            // create Image
            Image image = new Image();
            if ((node.GetValue("type") != null &&
                node.GetValue("type").ToString() == "Folder") ||
                isRoot)
            {
                image.Source = new BitmapImage(new Uri("pack://application:,,/Resources/Dialog/folder-closed.png"));
            }
            else
            {
                image.Source = new BitmapImage(new Uri("pack://application:,,/Resources/Dialog/file.png"));
            }

            // Label
            Label lbl = new Label();
            lbl.Content = node["title"];

            // Add into stack
            stack.Children.Add(image);
            stack.Children.Add(lbl);

            // assign stack to header
            item.Header = stack;

            item.DataContext = node;

            return item;
        }

        private void LoadDataSchemas()
        {
            string dataSchemasStr = this.metaDataService.GetXMLSchemas(ConnectionStringBuilder.Instance.Create(this.schemaInfo), this.selectedClassName);
            this.dataSchemas = JsonConvert.DeserializeObject<JArray>(dataSchemasStr);

            this.listBoxSchemas.Items.Clear();
            foreach (JObject dataSchema in this.dataSchemas)
            {
                this.listBoxSchemas.Items.Add(dataSchema.GetValue("title").ToString());
            }
        }

        private void LoadXMLSchemaColumns()
        {
            string fieldsStr = this.modelService.GetTimeSeriesFields(this.schemaInfo.Name, this.selectedClassName, this.selectedXMLSchemaName);
            var fields = JsonConvert.DeserializeObject<JArray>(fieldsStr);
            this.comboBoxSeriesName.Items.Clear();

            this.xmlSchemaFields = new JArray();

            foreach (JObject xmlSchemaField in fields)
            {
                if (!IsDateTimeXAxis(xmlSchemaField))
                {
                    this.comboBoxSeriesName.Items.Add(xmlSchemaField.GetValue("title").ToString());
                    this.xmlSchemaFields.Add(xmlSchemaField);
                }
                else
                {
                    string type = xmlSchemaField["type"].ToString();
                    if (type == "dateTime" || type == "date")
                    {
                        this.isDateTimeXAxis = true;
                    }
                    else
                    {
                        this.isDateTimeXAxis = false;
                    }
                }
            }
        }

        private bool IsDateTimeXAxis(JObject xmlSchemaField)
        {
            bool status = false;
            string val = xmlSchemaField["xaxis"].ToString();

            try
            {
                status = bool.Parse(val);
            }
            catch (Exception)
            {
                status = false;
            }

            return status;
        }

        private void CreateFrequencyComboBoxItems()
        {
            this.comboBoxFrequency.Items.Clear();
            if (this.isDateTimeXAxis)
            {
                foreach (JObject frequency in this.timeSeriesFrequencies)
                    comboBoxFrequency.Items.Add(frequency["title"].ToString());

                this.selectedFrequency = this.timeSeriesFrequencies[0]["name"].ToString();
                comboBoxFrequency.SelectedIndex = 0;
            }
            else
            {
                string item = "None";
                comboBoxFrequency.Items.Add(item);

                this.selectedFrequency = item;
                comboBoxFrequency.SelectedIndex = 0;
            }
        }

        private JArray GetTimeSeriesFrequencies()
        {
            JArray frequencies = new JArray();

            JObject frequency = new JObject();
            frequency["title"] = "Every Second";
            frequency["name"] = "Second";
            frequencies.Add(frequency);

            frequency = new JObject();
            frequency["title"] = "Every Minute";
            frequency["name"] = "Minute";
            frequencies.Add(frequency);

            frequency = new JObject();
            frequency["title"] = "Every Hour";
            frequency["name"] = "Hour";
            frequencies.Add(frequency);

            frequency = new JObject();
            frequency["title"] = "Every Day";
            frequency["name"] = "Day";
            frequencies.Add(frequency);

            frequency = new JObject();
            frequency["title"] = "Every Month";
            frequency["name"] = "Month";
            frequencies.Add(frequency);

            return frequencies;
        }

        private void btnNext_Click(object sender, RoutedEventArgs e)
        {
            switch (this.currentPage)
            {
                case 1:

                    // check if the dnn model file exists
                    MLConfiguration mlConfig = this.executableConfigurations[this.listBoxModels.SelectedIndex] as MLConfiguration;

                    if (mlConfig != null)
                    {
                        string modelFilePath = mlConfig.ModelFilePath;
                        if (File.Exists(modelFilePath))
                        {
                            // go to page 2 (select a class)
                            this.Page1.Visibility = Visibility.Hidden;

                            this.Page2.Visibility = Visibility.Visible;

                            this.prevBtn.Visibility = Visibility.Visible;
                            this.nextBtn.IsEnabled = false;
                            this.currentPage = 2;
                        }
                        else
                        {
                            MessageBox.Show("Model '" + this.selectedModelName + "' has not been generated, please execute run command first.");
                        }
                    }

                    break;

                case 2:

                    // go to page 3 (select xml schema)
                    this.Page2.Visibility = Visibility.Hidden;

                    this.Page3.Visibility = Visibility.Visible;

                    LoadDataSchemas(); // load xml schemas for the selected class name

                    this.nextBtn.IsEnabled = false;

                    this.currentPage = 3;

                    break;

                case 3:

                    // go to page 4 (model type)
                    this.Page3.Visibility = Visibility.Hidden;

                    this.Page4.Visibility = Visibility.Visible;
 
                    this.nextBtn.IsEnabled = true;

                    this.currentPage = 4;

                    LoadXMLSchemaColumns(); // show columns of the selected xml schemas

                    CreateFrequencyComboBoxItems(); // create frequency combobox

                    break;

                case 4:

                    this.Page4.Visibility = Visibility.Hidden;

                    if (this.rTimeSeriesForecast.IsChecked == true)
                    {
                        // goto page 5 (time series model specs)
                        this.Page5.Visibility = Visibility.Visible;

                        this.nextBtn.IsEnabled = false;

                        this.currentPage = 5;

                        this.maxForecastHorizon = GetMaxForecastHorizon();
                        this.txtMaxForecastHorizon.Text = this.maxForecastHorizon;

                    }
                    else
                    {
                        // goto page 6 (classification model specs
                        this.Page6.Visibility = Visibility.Visible;

                        this.nextBtn.IsEnabled = false;

                        this.currentPage = 6;
                    }

                    break;
            }
        }

        private string GetMaxForecastHorizon()
        {
            int dimension = 0;

            if (this.listBoxModels.SelectedIndex >= 0)
            {
                MLConfiguration mlConfig = this.executableConfigurations[this.listBoxModels.SelectedIndex] as MLConfiguration;

                string modelFilePath = mlConfig.ModelFilePath;

                Function modelFunc = Function.Load(modelFilePath, DeviceDescriptor.CPUDevice);

                // get output info
                IList<Variable> outputVars = modelFunc.Outputs;
                foreach (Variable outputVar in outputVars)
                {
                    // output vars can be err etc.
                    if (outputVar.Shape.TotalSize > dimension)
                    {
                        // assume the largest diemnsion is output dimension
                        dimension = outputVar.Shape.TotalSize;
                    }
                }

                return dimension.ToString();
            }
            else
            {
                return null;
            }
        }

        private void btnPrev_Click(object sender, RoutedEventArgs e)
        {
            switch (this.currentPage)
            {
                case 1:

                    break;

                case 2:

                    // go to page 1
                    this.Page2.Visibility = Visibility.Hidden;
                    this.Page1.Visibility = Visibility.Visible;
                    this.prevBtn.Visibility = Visibility.Hidden;
                    this.nextBtn.IsEnabled = true;

                    this.currentPage = 1;

                    break;

                case 3:

                    // go to page 2
                    this.Page3.Visibility = Visibility.Hidden;
                    this.Page2.Visibility = Visibility.Visible;
                    
                    this.nextBtn.IsEnabled = true;

                    this.currentPage = 2;
                    break;

                case 4:

                    // go to page 3
                    this.Page4.Visibility = Visibility.Hidden;
                    this.Page3.Visibility = Visibility.Visible;

                    this.nextBtn.IsEnabled = true;

                    this.currentPage = 3;
                    break;

                case 5:

                    // go to page 4
                    this.Page5.Visibility = Visibility.Hidden;
                    this.Page4.Visibility = Visibility.Visible;

                    this.nextBtn.IsEnabled = true;

                    this.currentPage = 4;
                    break;

                case 6:

                    // go to page 4
                    this.Page6.Visibility = Visibility.Hidden;
                    this.Page4.Visibility = Visibility.Visible;

                    this.nextBtn.IsEnabled = true;

                    this.currentPage = 4;
                    break;
            }
        }

        private void classTree_SelectedItemChanged(object sender, RoutedPropertyChangedEventArgs<object> e)
        {
            var item = (e.NewValue as TreeViewItem);

            if (item != null)
            {
                JObject node = item.DataContext as JObject;

                string nodeType = "None";
                if (node.GetValue("type") != null)
                {
                    nodeType = node.GetValue("type").ToString();
                }

                if (nodeType != "Folder" &&
                    node != this.rootNode)
                {
                    txtClass.Text = node["title"].ToString();
                    this.selectedClassName = node["name"].ToString();
                    this.nextBtn.IsEnabled = true;
                }
            }
        }

        private void listBoxSchemas_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (this.listBoxSchemas.SelectedIndex >= 0)
            {
                JObject dataSchema = this.dataSchemas[this.listBoxSchemas.SelectedIndex] as JObject;

                this.txtSchema.Text = dataSchema.GetValue("title").ToString();
                this.selectedXMLSchemaName = dataSchema.GetValue("name").ToString();

                this.nextBtn.IsEnabled = true;
            }
            else
            {
                this.txtSchema.Text = "";
                this.selectedXMLSchemaName = null;

                this.nextBtn.IsEnabled = false;
            }
        }

        private void comboBoxSeriesName_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (this.comboBoxSeriesName.SelectedIndex >= 0)
            {
                this.selectedTimeSeriesName = this.xmlSchemaFields[this.comboBoxSeriesName.SelectedIndex]["name"].ToString();
                this.btnPublishTimeSeries.IsEnabled = true;

                if (!string.IsNullOrEmpty(this.schemaInfo.Name) &&
                    !string.IsNullOrEmpty(this.selectedClassName) &&
                    !string.IsNullOrEmpty(this.selectedXMLSchemaName))
                {
                    JArray modelInfos = this.modelService.GetTimeSeriesModelInfos(this.schemaInfo.Name, this.selectedClassName, this.selectedXMLSchemaName,
                            this.selectedTimeSeriesName, this.selectedFrequency, this.maxForecastHorizon);
                    if (modelInfos.Count > 0)
                    {
                        this.checkHasPublished.IsChecked = true;
                        this.btnUnpublishTimeSeries.IsEnabled = true;
                    }
                    else
                    {
                        this.checkHasPublished.IsChecked = false;
                        this.btnUnpublishTimeSeries.IsEnabled = false;
                    }
                }
            }
        }

        private void comboBoxFrequency_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (this.comboBoxFrequency.SelectedIndex >= 0)
            {
                if (this.isDateTimeXAxis)
                {
                    this.selectedFrequency = this.timeSeriesFrequencies[this.comboBoxFrequency.SelectedIndex]["name"].ToString();
                }

                if (!string.IsNullOrEmpty(this.schemaInfo.Name) &&
                     !string.IsNullOrEmpty(this.selectedClassName) &&
                     !string.IsNullOrEmpty(this.selectedXMLSchemaName))
                {
                    JArray modelInfos = this.modelService.GetTimeSeriesModelInfos(this.schemaInfo.Name, this.selectedClassName, this.selectedXMLSchemaName,
                        this.selectedTimeSeriesName, this.selectedFrequency, this.maxForecastHorizon);
                    if (modelInfos.Count > 0)
                    {
                        this.checkHasPublished.IsChecked = true;
                        this.btnUnpublishTimeSeries.IsEnabled = true;
                    }
                    else
                    {
                        this.checkHasPublished.IsChecked = false;
                        this.btnUnpublishTimeSeries.IsEnabled = false;
                    }
                }
            }
        }

        private void btnPublishTimeSeries_Click(object sender, RoutedEventArgs e)
        {
            bool exist = false;
            MLConfiguration mlConfig = this.executableConfigurations[this.listBoxModels.SelectedIndex] as MLConfiguration;

            string modelFilePath = mlConfig.ModelFilePath;
            string preprocessProgramPath = mlConfig.PreprocessProgramPath;
            if (!string.IsNullOrEmpty(preprocessProgramPath) &&
                preprocessProgramPath.Contains(MLNameSpace.HOME_DIR_VAR))
            {
                preprocessProgramPath = preprocessProgramPath.Replace(MLNameSpace.HOME_DIR_VAR, MLNameSpace.GetHomeDir());
            }

            string postprocessProgramPath = mlConfig.PostprocessProgramPath;
            if (!string.IsNullOrEmpty(postprocessProgramPath) &&
                postprocessProgramPath.Contains(MLNameSpace.HOME_DIR_VAR))
            {
                postprocessProgramPath = postprocessProgramPath.Replace(MLNameSpace.HOME_DIR_VAR, MLNameSpace.GetHomeDir());
            }

            if (!exist)
            {
                if (this.checkModel.IsChecked == true)
                {
                    this.modelService.PublishTimeSeriesModel(this.schemaInfo.Name, this.selectedClassName, this.selectedXMLSchemaName,
                        this.selectedTimeSeriesName, this.selectedFrequency, this.maxForecastHorizon, modelFilePath);
                }

                if (!string.IsNullOrEmpty(preprocessProgramPath) &&
                    this.checkPreprocess.IsChecked == true)
                {
                    // publish the preprocess program
                    this.modelService.PublishPreprocessProgram(this.schemaInfo.Name, this.selectedClassName, this.selectedXMLSchemaName,
                        this.selectedTimeSeriesName, this.selectedFrequency, this.maxForecastHorizon, preprocessProgramPath);
                }

                if (!string.IsNullOrEmpty(postprocessProgramPath) &&
                    this.checkPostprocess.IsChecked == true)
                {
                    // publish the postprocess program
                    this.modelService.PublishPostprocessProgram(this.schemaInfo.Name, this.selectedClassName, this.selectedXMLSchemaName,
                        this.selectedTimeSeriesName, this.selectedFrequency, this.maxForecastHorizon, postprocessProgramPath);
                }

                this.checkHasPublished.IsChecked = true;
                this.btnUnpublishTimeSeries.IsEnabled = true;

                MessageBox.Show("The model has been published to the server");
            }
        }

        private void btnUnpublishTimeSeries_Click(object sender, RoutedEventArgs e)
        {
            this.modelService.DeleteTimeSeriesModel(this.schemaInfo.Name, this.selectedClassName, this.selectedXMLSchemaName,
                this.selectedTimeSeriesName, this.selectedFrequency, this.maxForecastHorizon);

            this.checkHasPublished.IsChecked = false;
            this.btnUnpublishTimeSeries.IsEnabled = false;

            MessageBox.Show("The model has been unpublished from the server");
        }

        private void btnPublishClassification_Click(object sender, RoutedEventArgs e)
        {

        }

        private void btnUnpublishClassification_Click(object sender, RoutedEventArgs e)
        {

        }
    }
}
