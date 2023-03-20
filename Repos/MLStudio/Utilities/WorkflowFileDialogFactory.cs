
namespace Newtera.MLStudio.Utilities
{
    using Microsoft.Win32;

    using Newtera.MLActivities.Core;

    public class WorkflowFileDialogFactory
    {
        public static SaveFileDialog CreateSaveFileDialog(string defaultFilename)
        {
            var fileDialog = new SaveFileDialog();
            fileDialog.DefaultExt = "xaml";
            fileDialog.FileName = defaultFilename;
            fileDialog.Filter = "xaml files (*.xaml,*.xamlx)|*.xaml;*.xamlx;|All files (*.*)|*.*";
            return fileDialog;
        }

        public static OpenFileDialog CreateOpenFileDialog()
        {
            var fileDialog = new OpenFileDialog();
            fileDialog.DefaultExt = "xaml";
            fileDialog.Filter = "xaml files (*.xaml,*.xamlx)|*.xaml;*.xamlx;|All files (*.*)|*.*";
            return fileDialog;
        }

        public static OpenFileDialog CreateNewFileDialog()
        {
            var fileDialog = new OpenFileDialog();
            fileDialog.DefaultExt = "xaml";
            fileDialog.InitialDirectory = MLNameSpace.GetHomeDir() +  @"\" + MLNameSpace.WORKFLOW_TEMPLATE_DIR;
            fileDialog.Filter = "xaml files (*.xaml,*.xamlx)|*.xaml;*.xamlx;|All files (*.*)|*.*";
            return fileDialog;
        }

        public static OpenFileDialog CreateAddReferenceDialog()
        {
            var fileDialog = new OpenFileDialog();
            fileDialog.DefaultExt = "dll";
            fileDialog.Filter = "assembly files (*.dll)|*.dll;|All files (*.*)|*.*";
            return fileDialog;
        }
    }
}