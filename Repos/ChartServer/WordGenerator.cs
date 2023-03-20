using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.ComponentModel;
using System.Data;
using System.Text;
using System.IO;
using System.Xml;
using System.Threading;
using System.Collections;
using Word = Microsoft.Office.Interop.Word;

using Newtera.Common.Core;
using Newtera.Common.MetaData;
using Newtera.Common.MetaData.Schema;
using Newtera.Common.MetaData.DataView;
using Newtera.Common.MetaData.DataView.Taxonomy;
using Newtera.Common.MetaData.XaclModel;
using Newtera.Common.MetaData.Principal;
using Newtera.Data;
using Newtera.SmartWordUtil;

namespace Newtera.ChartServer
{
    public class WordGenerator
    {
        private object missing = System.Reflection.Missing.Value;
        private object notTrue = false;
        private const string DatabaseNodeName = "Database";

        private Word.ApplicationClass objApp = null;
        private Word.Document objDocLast = null;

        public WordGenerator()
        {
        }

        /// <summary>
        /// Generates a word doc by filling a word template with a base instance view object.
        /// </summary>
        /// <param name="templateFilePath">path of the word template file</param>
        /// <param name="destinationFilePath">path of the generated word file</param>
        /// <param name="connectionStr">The database connection string</param>
        /// <param name="baseInstanceId">The base instance id</param>
        /// <param name="baseClassName">The base class name</param>
        public void Generate(object templateFilePath, object destinationFilePath, string connectionStr,
            string baseClassName, string baseInstanceId)
        {
            Word.ApplicationClass wordApp = new Word.ApplicationClass();
            Word.Document wordDoc = null;

            if (!File.Exists(templateFilePath.ToString()))
            {
                throw new Exception(templateFilePath + " does not exist.");
            }

            try
            {
                // tell word not to show itself
                wordApp.Visible = false;

                // open the word template
                wordDoc = wordApp.Documents.Add(ref templateFilePath,
                    ref missing, ref missing, ref missing);

                //wordDoc.Activate();

                if (wordDoc.XMLNodes != null && wordDoc.XMLNodes.Count > 0)
                {
                    Word.XMLNode databaseNode = null;

                    foreach (Word.XMLNode node in wordDoc.XMLNodes)
                    {
                        if (node.BaseName == DatabaseNodeName)
                        {
                            databaseNode = node;
                            break;
                        }
                    }

                    if (databaseNode != null)
                    {
                        MetaDataModel metaData;
                        using (CMConnection connection = new CMConnection(connectionStr))
                        {
                            connection.Open();

                            metaData = connection.MetaDataModel;
                        }

                        IDocDataSource dataSource = new DocDataSourceServer(connectionStr, baseClassName, baseInstanceId); // create a data source
                        dataSource.MetaData = metaData;

                        WordPopulator populator = new WordPopulator(dataSource);

                        populator.PopulateToSelectedNode(databaseNode); // populate the word doc
                    }
                }

                // save as the destination file
                wordDoc.SaveAs(ref destinationFilePath,
                    ref missing, ref missing, ref missing,
                    ref missing, ref missing, ref missing,
                    ref missing, ref missing, ref missing,
                    ref missing, ref missing, ref missing,
                    ref missing, ref missing, ref missing);
            }
            finally
            {
                if (wordDoc != null)
                {
                    wordDoc.Close(ref missing, ref missing, ref missing);
                }

                wordApp.Application.Quit(ref notTrue, ref missing, ref missing);
            }
        }

        /// <summary>
        /// Generates a word doc by inserting files stored in a given directory.in a template file
        /// </summary>
        /// <param name="fileDir">directory of the word template files</param>
        /// <param name="templateFilePath">Template file path</param>
        /// <param name="destinationFilePath">path of the generated word file</param>
        public void InsertMerge(string fileDir, string templateFilePath, string destinationFilePath)
        {
            objApp = new Word.ApplicationClass();

            // tell word not to show itself
            objApp.Visible = false;

            string[] arrFiles = Directory.GetFiles(fileDir);

            // sort the file names based on the number at the end of each file name, file name is in form of "File_N", in which  N is a number
            arrFiles = SortFileNames(arrFiles);
            InsertMerge(templateFilePath, arrFiles, destinationFilePath);
        }

        private string[] SortFileNames(string[] array)
        {
            int i, j;
            string temp;
            bool exchange;

            for (i = 0; i < array.Length; i++)
            {
                exchange = false;

                for (j = array.Length - 2; j >= i; j--)
                {
                    if (System.String.CompareOrdinal(array[j + 1], array[j]) < 0)
                    {
                        temp = array[j + 1];
                        array[j + 1] = array[j];
                        array[j] = temp;

                        exchange = true;
                    }
                }

                if (!exchange)
                {
                    break;
                }
            }

            return array;
        }

        /// <summary>
        /// Insert individual files into a single file
        /// </summary>
        /// <param name="tempDoc">template file</param>
        /// <param name="arrCopies">individual file path</param>
        /// <param name="outDoc">output file</param>
        private void InsertMerge(string tempDoc, string[] arrCopies, string outDoc)
        {
            object objFalse = false;
            object confirmConversion = false;
            object link = false;
            object attachment = false;
            object pBreak = (int)Word.WdBreakType.wdSectionBreakNextPage;

            try
            {
                // open template file
                Open(tempDoc);

                foreach (string strCopy in arrCopies)
                {
                    objApp.Selection.InsertFile(strCopy,
                        ref missing,
                        ref confirmConversion,
                        ref link,
                        ref attachment
                        );

                    // next doc is inserted into a new page
                    objApp.Selection.InsertBreak(ref pBreak);

                }

                // Save to output file
                SaveAs(outDoc);

                foreach (Word.Document objDocument in objApp.Documents)
                {
                    objDocument.Close(ref objFalse,     //SaveChanges 
                        ref missing,   //OriginalFormat 
                        ref missing    //RouteDocument  
                        );
                }
            }
            finally
            {
                objApp.Application.Quit(ref notTrue,     //SaveChanges  
                    ref missing,     //OriginalFormat 
                    ref missing      //RoutDocument  
                    );

                objApp = null;
            }
        }

        // Save to the  output file
        private void SaveAs(string outDoc)
        {
            object objOutDoc = outDoc;
            objDocLast.SaveAs(ref objOutDoc,      //FileName 
                ref missing,     //FileFormat
                ref missing,     //LockComments  
                ref missing,     //PassWord  
                ref missing,     //AddToRecentFiles 
                ref missing,     //WritePassword 
                ref missing,     //ReadOnlyRecommended  
                ref missing,     //EmbedTrueTypeFonts   
                ref missing,     //SaveNativePictureFormat  
                ref missing,     //SaveFormsData  
                ref missing,     //SaveAsAOCELetter,  
                ref missing,     //Encoding   
                ref missing,     //InsertLineBreaks  
                ref missing,     //AllowSubstitutions    
                ref missing,     //LineEnding    
                ref missing      //AddBiDiMarks  
                );
        }

        // Open a template file
        private void Open(string templateDoc)
        {
            object objTempDoc = templateDoc;

            objDocLast = objApp.Documents.Open(ref objTempDoc,    //FileName
                ref missing,   //ConfirmVersions 
                ref missing,   //ReadOnly
                ref missing,   //AddToRecentFiles
                ref missing,   //PasswordDocument
                ref missing,   //PasswordTemplate
                ref missing,   //Revert
                ref missing,   //WritePasswordDocument
                ref missing,   //WritePasswordTemplate 
                ref missing,   //Format
                ref missing,   //Enconding 
                ref missing,   //Visible 
                ref missing,   //OpenAndRepair
                ref missing,   //DocumentDirection 
                ref missing,   //NoEncodingDialog 
                ref missing    //XMLTransform 
                );

            //objDocLast.Activate();
        }
    }
}