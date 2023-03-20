using System;
using System.IO;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Newtera.MLStudio.Utilities
{
    public class TextWriterHolder
    {
        public TextWriter output { get; set; }
        public TextEditorWriter code { get; set; }

        public TextEditorWriter rawData { get; set; }
        public TextEditorWriter trainData { get; set; }
        public TextEditorWriter testData { get; set; }
        public TextEditorWriter evalData { get; set; }
        public TextEditorWriter outputData { get; set; }
        public TextEditorWriter resultData { get; set; }
    }
}
