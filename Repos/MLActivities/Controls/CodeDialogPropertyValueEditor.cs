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
    class CodeDialogPropertyValueEditor: System.Activities.Presentation.PropertyEditing.DialogPropertyValueEditor
    {
        private ResourceDictionary res = new CodeInlineEditorTemplate().Resources;

        public CodeDialogPropertyValueEditor()
        {
            this.InlineEditorTemplate =  res["CodeInlineEditorTemplate"] as DataTemplate;
        }

        public override void ShowDialog(PropertyValue propertyValue, IInputElement commandSource)
        {
            CodeEditorDialog dialog = new CodeEditorDialog();
            dialog.Code = propertyValue.StringValue;


            if (dialog.ShowDialog() == true)
            {
                propertyValue.StringValue = dialog.Code;
            }
        }
    }
}
