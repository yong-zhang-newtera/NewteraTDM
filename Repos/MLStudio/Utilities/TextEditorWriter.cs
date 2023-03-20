
namespace Newtera.MLStudio.Utilities
{
    using System;
    using System.IO;
    using System.Diagnostics;
    using System.Text;
    using System.Windows.Controls;
    using ICSharpCode.AvalonEdit;

    public class TextEditorWriter
    {
        private string filePath = null;
        private string workflowName = null;
        private TextEditor output = null;

        public TextEditorWriter(TextEditor output, string workflowName)
        {
            this.output = output;
            this.workflowName = workflowName;
        }

        public string FilePath
        {
            get {
                return this.filePath;
            }
            set
            {
                this.filePath = value;
            }
        }

        public bool IsLoaded
        {
            get
            {
                if (!string.IsNullOrEmpty(this.output.Text))
                {
                    return true;
                }
                else
                {
                    return false;
                }
            }
        }

        public void LoadFile()
        {
            // clear the content
            this.output.Clear();

            if (!string.IsNullOrEmpty(this.filePath) &&
                File.Exists(this.filePath))
            {
                this.output.Load(this.filePath);
            }
        }
    }
}