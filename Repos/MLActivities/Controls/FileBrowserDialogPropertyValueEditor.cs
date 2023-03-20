using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Forms;
using System.Activities.Presentation.PropertyEditing;

namespace Newtera.MLActivities.Controls
{
    class FileBrowserDialogPropertyValueEditor: System.Activities.Presentation.PropertyEditing.DialogPropertyValueEditor
    {
        private ResourceDictionary res = new FileBrowserInlineEditorTemplate().Resources;

        public FileBrowserDialogPropertyValueEditor()
        {
            this.InlineEditorTemplate =  res["FileBrowserInlineEditorTemplate"] as DataTemplate;
        }

        public override void ShowDialog(PropertyValue propertyValue, IInputElement commandSource)
        {
            OpenFileDialog ofd = new OpenFileDialog();
            ofd.Filter = "Text files (*.txt)|*.txt";
            ofd.Title = "Select Text file";
            ofd.Multiselect = false;

            if (ofd.ShowDialog() == DialogResult.OK)
            {
                propertyValue.StringValue = ofd.FileName;
            }
        }
    }
}
