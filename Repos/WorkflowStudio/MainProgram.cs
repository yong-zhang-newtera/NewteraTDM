using System;
using System.Threading;
using System.Collections.Generic;
using System.Windows.Forms;
using System.Globalization;

namespace WorkflowStudio
{
    static class MainProgram
    {
        /// <summary>
        /// 应用程序的主入口点。
        /// </summary>
        [STAThread]
        static void Main(string[] args)
        {
            string language = (args.Length > 0) ? args[0] : null;
            if (language != null)
            {
                Thread.CurrentThread.CurrentUICulture = new CultureInfo(language);
            }

            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);
            Application.Run(new WorkflowStudioApp());
        }
    }
}