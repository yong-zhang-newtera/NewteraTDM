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
    class ExperimentIdPropertyValueEditor: System.Activities.Presentation.PropertyEditing.DialogPropertyValueEditor
    {
        private ResourceDictionary res = new ExperimentIdInlineEditorTemplate().Resources;

        public ExperimentIdPropertyValueEditor()
        {
            this.InlineEditorTemplate =  res["ExperimentIdInlineEditorTemplate"] as DataTemplate;
        }

        public override void ShowDialog(PropertyValue propertyValue, IInputElement commandSource)
        {
            ExperimentIdDialog dialog = new ExperimentIdDialog();
            dialog.ExperimentId = propertyValue.StringValue;


            if (dialog.ShowDialog() == true)
            {
                propertyValue.StringValue = dialog.ExperimentId;
            }
        }
    }
}
