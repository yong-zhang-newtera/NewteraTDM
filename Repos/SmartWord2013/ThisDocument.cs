using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.IO;
using System.Windows.Forms;
using System.Xml.Linq;
using Microsoft.Office.Tools.Word;
using Microsoft.VisualStudio.Tools.Applications.Runtime;
using Office = Microsoft.Office.Core;
using Word = Microsoft.Office.Interop.Word;

namespace SmartWord2013
{
    public partial class ThisDocument
    {
        #region SmartWord XML Schema constants.

        public const string NameSpaceUri = "http://www.newtera.com/SmartWord.xsd";
        //public const string NameSpaceUri = "";

        internal const string NameSpaceAlias = "SmartWord";

        internal const string SchemaFile = "SmartWord.xsd";

        internal static readonly string NameSpacePath = string.Format(
            "xmlns:{0}='{1}'", NameSpaceAlias, NameSpaceUri);

        internal const string DatabaseAttribute = "dname";
        internal const string TypeAttribute = "etype"; // meta data element type
        internal const string ElementAttribute = "ename";
        internal const string TableAttribute = "tname";
        internal const string TaxonomyAttribute = "xname";
        internal const string PropertyNameAttribute = "pname";
        internal const string ColumnNameAttribute = "cname";
        internal const string PathAttribute = "path";
        internal const string PropertyNodeName = "Property";
        internal const string ArrayNodeName = "Array";
        internal const string ColumnNodeName = "Column";
        internal const string ViewNodeName = "View";
        internal const string FamilyNodeName = "Family";
        internal const string DatabaseNodeName = "Database";
        internal const string ClassType = "c";
        internal const string DataViewType = "d";
        internal const string TaxonType = "t";

        // Use Schema in the same directory as the document.
        internal static string SchemaLocation;

        #endregion

        #region Actions Pane UserControls

        private NavigationControl navigationPane;

        #endregion

        private void ThisDocument_Startup(object sender, System.EventArgs e)
        {
            // Set location based on document path.
            SchemaLocation = System.IO.Path.Combine(Globals.ThisDocument.Path,
                SchemaFile);

            InstallSchema();

            // Initialize and load Actions Pane controls.
            CreateActionsPane();

            // Register delegates for XML node events.
            CreateEventHandlers();
        }

        private void ThisDocument_Shutdown(object sender, System.EventArgs e)
        {
        }

        /// <summary>
        /// Attach the schema to the document.
        /// </summary>
        private void InstallSchema()
        {
            // Use a local copy of the schema location in case we need to 
            // modify when run in DEBUG mode from VS.NET.
            string schemaLocation = SchemaLocation;

            if (!System.IO.File.Exists(schemaLocation))
            {
                // When run from VS.Net the Schema will be in the Debug dir.
                string debugDirectory = System.IO.Path.Combine(
                    System.IO.Path.GetDirectoryName(schemaLocation), "Debug");

                schemaLocation = System.IO.Path.Combine(debugDirectory,
                    System.IO.Path.GetFileName(schemaLocation));
            }

            // Ensure that it now exists in debug mode.  
            // Note: This assertion should not be used in non-Sample code.
            if (!File.Exists(schemaLocation))
            {
                throw new ApplicationException("Couldn't find schema SmartWord.xsd.");
            }

            object nameSpaceUri = (object)NameSpaceUri;
            object alias = (object)NameSpaceAlias;

            Word.XMLNamespace srNamespace =
                Globals.ThisDocument.ThisApplication.XMLNamespaces.Add(
                schemaLocation, ref nameSpaceUri, ref alias, false);

            object doc = Globals.ThisDocument.ThisApplication.ActiveDocument;
            srNamespace.AttachToDocument(ref doc);
        }

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

        /// <summary>
        /// Registers event handlers for XML nodes in the document.
        /// </summary>
        private void CreateEventHandlers()
        {
            ThisApplication.XMLSelectionChange += new
               Word.ApplicationEvents4_XMLSelectionChangeEventHandler(
                  ThisApplication_XMLSelectionChange);
        }

        /// <summary>
        /// When a user moves from one child node to another of the same type
        /// the ContextEnter & Leave events will not fire.  SelectionChange 
        /// occurs whenever an XmlNode is selected.  This method checks if the
        /// oldNode and newNode are the same to determine if it should force a 
        /// context change on repeating elements.
        /// </summary>
        /// <param name="Sel"></param>
        /// <param name="OldXMLNode"></param>
        /// <param name="NewXMLNode"></param>
        /// <param name="Reason"></param>
        /// <remarks>
        /// This is only needed for repeating elements.
        /// </remarks>
        void ThisApplication_XMLSelectionChange(Word.Selection Sel,
           Word.XMLNode OldXMLNode,
           Word.XMLNode NewXMLNode, ref int Reason)
        {
            string elementName = null;
            string propertyName = null;
            string database = null;
            string elementType = null;
            string taxonomyName = null;

            if (NewXMLNode != null)
            {
                switch (NewXMLNode.BaseName)
                {
                    case ThisDocument.DatabaseNodeName:
                        database = GetAttributeValue(NewXMLNode, DatabaseAttribute);

                        this.navigationPane.SelectDatabase(NewXMLNode, database);

                        break;

                    case ThisDocument.FamilyNodeName:
                        elementName = GetAttributeValue(NewXMLNode, ElementAttribute);
                        elementType = GetAttributeValue(NewXMLNode, TypeAttribute);
                        taxonomyName = GetAttributeValue(NewXMLNode, TaxonomyAttribute);
                        database = GetAttributeValue(NewXMLNode.ParentNode, DatabaseAttribute);

                        // select the base view for the family
                        this.navigationPane.SelectElement(NewXMLNode, database, elementName, elementType, taxonomyName);

                        break;

                    case ThisDocument.ViewNodeName:
                        elementName = GetAttributeValue(NewXMLNode, ElementAttribute);
                        elementType = GetAttributeValue(NewXMLNode, TypeAttribute);
                        taxonomyName = GetAttributeValue(NewXMLNode, TaxonomyAttribute);
                        database = GetAttributeValue(NewXMLNode.ParentNode, DatabaseAttribute);

                        this.navigationPane.SelectElement(NewXMLNode, database, elementName, elementType, taxonomyName);

                        break;

                    case ThisDocument.PropertyNodeName:
                        propertyName = GetAttributeValue(NewXMLNode, PropertyNameAttribute);
                        elementName = GetAttributeValue(NewXMLNode.ParentNode, ElementAttribute);
                        elementType = GetAttributeValue(NewXMLNode.ParentNode, TypeAttribute);
                        taxonomyName = GetAttributeValue(NewXMLNode.ParentNode, TaxonomyAttribute);
                        database = GetAttributeValue(NewXMLNode.ParentNode.ParentNode, DatabaseAttribute);

                        this.navigationPane.SelectAttribute(NewXMLNode, database, elementName, elementType, taxonomyName, propertyName);

                        break;

                    case ThisDocument.ArrayNodeName:
                        propertyName = GetAttributeValue(NewXMLNode, PropertyNameAttribute);
                        elementName = GetAttributeValue(NewXMLNode.ParentNode, ElementAttribute);
                        elementType = GetAttributeValue(NewXMLNode.ParentNode, TypeAttribute);
                        taxonomyName = GetAttributeValue(NewXMLNode.ParentNode, TaxonomyAttribute);
                        database = GetAttributeValue(NewXMLNode.ParentNode.ParentNode, DatabaseAttribute);

                        this.navigationPane.SelectAttribute(NewXMLNode, database, elementName, elementType, taxonomyName, propertyName);

                        break;
                }
            }
            else
            {
                this.navigationPane.Clear();
            }
        }

        private string GetAttributeValue(Word.XMLNode xmlNode, string attributeName)
        {
            string val = null;
            foreach (Word.XMLNode attr in xmlNode.Attributes)
            {
                if (attr.BaseName == attributeName)
                {
                    val = attr.NodeValue;
                }
            }

            return val;
        }

        private void ThisDocument_SelectionChange(object sender, Microsoft.Office.Tools.Word.SelectionEventArgs e)
        {
            Word.Range selectedRange = e.Selection.Range;

            if (selectedRange != null)
            {
                this.navigationPane.SelectedRange = selectedRange;
                this.navigationPane.EnableInsertButton();
            }
        }

        #region VSTO 设计器生成的代码

        /// <summary>
        /// 设计器支持所需的方法 - 不要修改
        /// 使用代码编辑器修改此方法的内容。
        /// </summary>
        private void InternalStartup()
        {
            this.Startup += new System.EventHandler(ThisDocument_Startup);
            this.Shutdown += new System.EventHandler(ThisDocument_Shutdown);
            this.SelectionChange += new Microsoft.Office.Tools.Word.SelectionEventHandler(this.ThisDocument_SelectionChange);
        }

        #endregion
    }
}
