﻿using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.Xml.Linq;
using Microsoft.Office.Tools.Excel;
using Microsoft.VisualStudio.Tools.Applications.Runtime;
using Excel = Microsoft.Office.Interop.Excel;
using Office = Microsoft.Office.Core;

namespace SmartExcel2013
{
    public partial class ThisWorkbook
    {
        private NavigationControl navigationPane;

        private void ThisWorkbook_Startup(object sender, System.EventArgs e)
        {
            // Initialize and load Actions Pane controls.
            CreateActionsPane();
        }

        private void ThisWorkbook_Shutdown(object sender, System.EventArgs e)
        {
        }

        #region VSTO 设计器生成的代码

        /// <summary>
        /// 设计器支持所需的方法 - 不要修改
        /// 使用代码编辑器修改此方法的内容。
        /// </summary>
        private void InternalStartup()
        {
            this.Startup += new System.EventHandler(ThisWorkbook_Startup);
            this.Shutdown += new System.EventHandler(ThisWorkbook_Shutdown);
        }

        #endregion

        /// <summary>
        /// Initializes user controls, adds them to the Actions Pane.  
        /// </summary>
        private void CreateActionsPane()
        {
            // Create the user controls used in the Actions Pane.

            navigationPane = new NavigationControl();

            // Add user controls to the ActionsPane.

            ActionsPane.Controls.Add(navigationPane);

           
        }

    }
}
