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
using System.Configuration;

using Newtera.MLActivities.Core;
using Newtera.MLStudio.WebApi;
using Newtera.MLStudio.Utilities;

namespace Newtera.MLStudio.Views
{
    /// <summary>
    /// NewWorkflowDialog.xaml 的交互逻辑
    /// </summary>
    public partial class NewWorkflowDialog : Window
    {
        private string fileName;

        public NewWorkflowDialog()
        {
            this.fileName = null;

            InitializeComponent();

        }

        public string FileName {
            get
            {
                return this.fileName;
            }
        }

        private void btnDialogOK_Click(object sender, RoutedEventArgs e)
        {
            if (string.IsNullOrEmpty(this.fileName))
            {
                MessageBox.Show("Please select a template");

                this.DialogResult = false;
            }
            else
            {
                this.DialogResult = true;
            }
        }

        private void Window_ContentRendered(object sender, EventArgs e)
        {
            List<WorkflowTemplate> items = new List<WorkflowTemplate>();

            string templateDir = MLNameSpace.GetHomeDir() + @"\" + MLNameSpace.WORKFLOW_TEMPLATE_DIR;
            DirectoryInfo dir = new DirectoryInfo(templateDir);
            FileInfo[] files = dir.GetFiles("*.xaml").OrderBy(p => p.CreationTime).ToArray(); //Getting templates files sorted by date

            string title;
            foreach (FileInfo file in files)
            {
                int pos = file.Name.IndexOf('.');
                if (pos > 0)
                {
                    title = file.Name.Substring(0, pos);
                }
                else
                {
                    title = file.Name;
                }

                items.Add(new WorkflowTemplate() { Image = LoadImage("new.png"), Title = title, FileName = file.Name });
            }
            lsvFileList.ItemsSource = items;
        }

        // for this code image needs to be a project resource
        private BitmapImage LoadImage(string filename)
        {
            return new BitmapImage(new Uri("pack://application:,,,/Resources/Dialog/" + filename));
        }

        private void lsvFileList_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (this.lsvFileList.SelectedIndex >= 0)
            {
                this.fileName = MLNameSpace.GetHomeDir() + @"\" + MLNameSpace.WORKFLOW_TEMPLATE_DIR + @"\" + ((WorkflowTemplate)this.lsvFileList.SelectedItem).FileName;

                this.btnDialogOK.IsEnabled = true;
            }
        }
    }

    public class WorkflowTemplate
    {
        public BitmapImage Image { get; set; }
        public string Title { get; set; }

        public string FileName { get; set; }
    }
}
