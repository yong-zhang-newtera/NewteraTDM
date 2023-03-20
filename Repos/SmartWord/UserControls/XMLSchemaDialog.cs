using System;
using System.Drawing;
using System.Collections;
using System.Text;
using System.Xml;
using System.Collections.Generic;
using System.ComponentModel;
using System.Windows.Forms;
using Word = Microsoft.Office.Interop.Word;

using Newtera.Common.MetaData;
using Newtera.Common.MetaData.Schema;
using Newtera.Common.MetaData.DataView;
using Newtera.Common.MetaData.XMLSchemaView;
using Newtera.SmartWordUtil;
using Newtera.WinClientCommon;

namespace SmartWord
{
	/// <summary>
	/// Summary description for ChooseClassDialog.
	/// </summary>
	public class XMLSchemaDialog : System.Windows.Forms.Form
	{
		private MetaDataModel _metaData = null;
		private string _selectedXMLSchemaName;
        private XMLSchemaModel _xmlSchemaModel;
        private MetaDataTreeNode _selectedNode;
        private NavigationControl _navigateControl;
        private IMetaDataElement _selectedMetaDataElement = null;

        private object objMissing = System.Type.Missing;

        private System.Windows.Forms.Button cancelButton;
        private System.Windows.Forms.GroupBox groupBox2;
		private System.Windows.Forms.TreeView treeView;
        private ImageList treeViewImageList;
        private Button insertXMLSchemaNodeButton;
		private System.ComponentModel.IContainer components;

		public XMLSchemaDialog()
		{
			//
			// Required for Windows Form Designer support
			//
			InitializeComponent();

			//
			// TODO: Add any constructor code after InitializeComponent call
			//
			_metaData = null;
            _selectedXMLSchemaName = null;
            _xmlSchemaModel = null;
            _selectedNode = null;
            _navigateControl = null;
		}

		/// <summary>
		/// Clean up any resources being used.
		/// </summary>
		protected override void Dispose( bool disposing )
		{
			if( disposing )
			{
				if(components != null)
				{
					components.Dispose();
				}
			}
			base.Dispose( disposing );
		}

		/// <summary>
		/// Gets or sets the meta data model from which to get classes
		/// </summary>
		public MetaDataModel MetaData
		{
			get
			{
				return _metaData;
			}
			set
			{
				_metaData = value;
			}
		}

		/// <summary>
		/// Get the selected xml schema name
		/// </summary>
		public string SelectedXMLSchemaName
		{
			get
			{
				return _selectedXMLSchemaName;
			}
            set
            {
                _selectedXMLSchemaName = value;
            }
		}

        /// <summary>
        /// Gets or sets the navigation control which launches this dialog
        /// </summary>
        public NavigationControl NavigationControl
        {
            get
            {
                return _navigateControl;
            }
            set
            {
                _navigateControl = value;
            }
        }

        /// <summary>
        /// Gets the currently selected xml node in the word doc 
        /// </summary>
        internal Word.XMLNode SelectedXmlNode
        {
            get
            {
                return _navigateControl.SelectedXmlNode;
            }
        }


		#region Windows Form Designer generated code
		/// <summary>
		/// Required method for Designer support - do not modify
		/// the contents of this method with the code editor.
		/// </summary>
		private void InitializeComponent()
		{
            this.components = new System.ComponentModel.Container();
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(XMLSchemaDialog));
            this.cancelButton = new System.Windows.Forms.Button();
            this.groupBox2 = new System.Windows.Forms.GroupBox();
            this.treeView = new System.Windows.Forms.TreeView();
            this.treeViewImageList = new System.Windows.Forms.ImageList(this.components);
            this.insertXMLSchemaNodeButton = new System.Windows.Forms.Button();
            this.groupBox2.SuspendLayout();
            this.SuspendLayout();
            // 
            // cancelButton
            // 
            this.cancelButton.AccessibleDescription = null;
            this.cancelButton.AccessibleName = null;
            resources.ApplyResources(this.cancelButton, "cancelButton");
            this.cancelButton.BackgroundImage = null;
            this.cancelButton.DialogResult = System.Windows.Forms.DialogResult.Cancel;
            this.cancelButton.Font = null;
            this.cancelButton.Name = "cancelButton";
            this.cancelButton.Click += new System.EventHandler(this.cancelButton_Click);
            // 
            // groupBox2
            // 
            this.groupBox2.AccessibleDescription = null;
            this.groupBox2.AccessibleName = null;
            resources.ApplyResources(this.groupBox2, "groupBox2");
            this.groupBox2.BackgroundImage = null;
            this.groupBox2.Controls.Add(this.treeView);
            this.groupBox2.Font = null;
            this.groupBox2.Name = "groupBox2";
            this.groupBox2.TabStop = false;
            // 
            // treeView
            // 
            this.treeView.AccessibleDescription = null;
            this.treeView.AccessibleName = null;
            resources.ApplyResources(this.treeView, "treeView");
            this.treeView.BackgroundImage = null;
            this.treeView.Font = null;
            this.treeView.FullRowSelect = true;
            this.treeView.ImageList = this.treeViewImageList;
            this.treeView.ItemHeight = 16;
            this.treeView.Name = "treeView";
            this.treeView.AfterSelect += new System.Windows.Forms.TreeViewEventHandler(this.treeView_AfterSelect);
            // 
            // treeViewImageList
            // 
            this.treeViewImageList.ImageStream = ((System.Windows.Forms.ImageListStreamer)(resources.GetObject("treeViewImageList.ImageStream")));
            this.treeViewImageList.TransparentColor = System.Drawing.Color.Transparent;
            this.treeViewImageList.Images.SetKeyName(0, "");
            this.treeViewImageList.Images.SetKeyName(1, "");
            this.treeViewImageList.Images.SetKeyName(2, "");
            this.treeViewImageList.Images.SetKeyName(3, "");
            this.treeViewImageList.Images.SetKeyName(4, "");
            this.treeViewImageList.Images.SetKeyName(5, "");
            this.treeViewImageList.Images.SetKeyName(6, "");
            this.treeViewImageList.Images.SetKeyName(7, "");
            this.treeViewImageList.Images.SetKeyName(8, "");
            this.treeViewImageList.Images.SetKeyName(9, "");
            this.treeViewImageList.Images.SetKeyName(10, "");
            this.treeViewImageList.Images.SetKeyName(11, "");
            this.treeViewImageList.Images.SetKeyName(12, "");
            this.treeViewImageList.Images.SetKeyName(13, "");
            this.treeViewImageList.Images.SetKeyName(14, "");
            this.treeViewImageList.Images.SetKeyName(15, "virtualproperty.GIF");
            this.treeViewImageList.Images.SetKeyName(16, "imageicon.gif");
            // 
            // insertXMLSchemaNodeButton
            // 
            this.insertXMLSchemaNodeButton.AccessibleDescription = null;
            this.insertXMLSchemaNodeButton.AccessibleName = null;
            resources.ApplyResources(this.insertXMLSchemaNodeButton, "insertXMLSchemaNodeButton");
            this.insertXMLSchemaNodeButton.BackgroundImage = null;
            this.insertXMLSchemaNodeButton.Font = null;
            this.insertXMLSchemaNodeButton.Name = "insertXMLSchemaNodeButton";
            this.insertXMLSchemaNodeButton.UseVisualStyleBackColor = true;
            this.insertXMLSchemaNodeButton.Click += new System.EventHandler(this.insertXMLSchemaNodeButton_Click);
            // 
            // XMLSchemaDialog
            // 
            this.AccessibleDescription = null;
            this.AccessibleName = null;
            resources.ApplyResources(this, "$this");
            this.BackgroundImage = null;
            this.Controls.Add(this.insertXMLSchemaNodeButton);
            this.Controls.Add(this.groupBox2);
            this.Controls.Add(this.cancelButton);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.SizableToolWindow;
            this.Icon = null;
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "XMLSchemaDialog";
            this.Load += new System.EventHandler(this.ChooseClassDialog_Load);
            this.groupBox2.ResumeLayout(false);
            this.ResumeLayout(false);

		}
		#endregion

		#region Controller code

        /// <summary>
        /// Insert a corresponding XML node into the Word Document according to the
        /// attached xml schema and current position
        /// </summary>
        private void InsertXMLNode()
        {
            Word.XMLNode xmlNode = null;
            Word.XMLNode attributeNode;

            if (SelectedXmlNode != null)
            {
                if (SelectedXmlNode.BaseName == ThisDocument.DatabaseNodeName)
                {
                    bool containTable = false;
                    int numOfTables = 0;
                    Word.Range range = (Word.Range)NavigationControl._selectedRange;
                    // make sure that the class node does not contains more than one table
                    numOfTables = range.Tables.Count;
                    if (numOfTables == 1)
                    {
                        // find the table contained by the class node
                        //Remember: arrays through interop in Word are 1 based.
                        Word.Table table = range.Tables[1];
                        // make sure the table only has two rows
                        if (table.Rows.Count < 2)
                        {
                            MessageBox.Show(Newtera.WindowsControl.MessageResourceManager.GetString("SmartWord.InvalidTable"),
                                "Error",
                                MessageBoxButtons.OK,
                                MessageBoxIcon.Error);
                            return;
                        }

                        containTable = true;
                    }

                    // ask for user whether to insert a view or a family
                    SelectViewOrFamilyDialog dialog = new SelectViewOrFamilyDialog();
                    string nodeName = ThisDocument.ViewNodeName;
                    if (dialog.ShowDialog() == DialogResult.Cancel)
                    {
                        return;
                    }
                    else
                    {
                        nodeName = dialog.InsertNodeName;
                    }

                    // insert a class node or family node
                    foreach (Word.XMLChildNodeSuggestion nodeSuggestion in SelectedXmlNode.ChildNodeSuggestions)
                    {
                        if (nodeName == ThisDocument.ViewNodeName && nodeSuggestion.BaseName == ThisDocument.ViewNodeName)
                        {
                            if (numOfTables > 1)
                            {
                                // A class node can only contain one table
                                MessageBox.Show(Newtera.WindowsControl.MessageResourceManager.GetString("SmartWord.TooManyTables"),
                                    "Error",
                                    MessageBoxButtons.OK,
                                    MessageBoxIcon.Error);
                                break;
                            }
                            else
                            {
                                // insert a view node only
                                xmlNode = nodeSuggestion.Insert(ref NavigationControl._selectedRange);
                                SetViewNodeAttributes(xmlNode, containTable);
                            }
                            break;
                        }
                        else if (nodeSuggestion.BaseName == ThisDocument.FamilyNodeName)
                        {
                            // insert a family node
                            xmlNode = nodeSuggestion.Insert(ref NavigationControl._selectedRange);
                            SetViewNodeAttributes(xmlNode, containTable);
                            break;
                        }
                    }
                }
                else if (SelectedXmlNode.BaseName == ThisDocument.FamilyNodeName)
                {
                    // make sure the view inserting into family node is the same as the base class associated with
                    // the family node or is related to the base class
                    // of the family through some relationship chain
                    string baseClassCaption = "";
                    Stack<string> path = new Stack<string>();
                    
                    bool containTable = false;
                    Word.Range range = (Word.Range)NavigationControl._selectedRange;
                    // make sure that the class node does not contains more than one table
                    if (range.Tables.Count == 1)
                    {
                        // find the table contained by the class node
                        //Remember: arrays through interop in Word are 1 based.
                        Word.Table table = range.Tables[1];
                        // make sure the table only has two rows
                        if (table.Rows.Count == 2)
                        {
                            /*
                            MessageBox.Show(Newtera.WindowsControl.MessageResourceManager.GetString("SmartWord.InvalidTable"),
                                "Error",
                                MessageBoxButtons.OK,
                                MessageBoxIcon.Error);
                            return;
                             */
                            containTable = true;
                        }
                    }
                    else if (range.Tables.Count > 1)
                    {
                        MessageBox.Show(Newtera.WindowsControl.MessageResourceManager.GetString("SmartWord.TooManyTables"),
                            "Error",
                            MessageBoxButtons.OK,
                            MessageBoxIcon.Error);
                        return;
                    }

                    // ask for user whether to insert a view or a property
                    string nodeName = ThisDocument.ViewNodeName;

                    SelectViewOrPropertyDialog dialog = new SelectViewOrPropertyDialog();
                    if (dialog.ShowDialog() == DialogResult.Cancel)
                    {
                        return;
                    }
                    else
                    {
                        nodeName = dialog.InsertNodeName;
                    }

                    foreach (Word.XMLChildNodeSuggestion nodeSuggestion in SelectedXmlNode.ChildNodeSuggestions)
                    {
                        if (nodeSuggestion.BaseName == ThisDocument.ViewNodeName)
                        {
                            // insert a view node
                            xmlNode = nodeSuggestion.Insert(ref NavigationControl._selectedRange);
                            SetViewNodeAttributes(xmlNode, containTable, path);

                            if (nodeName == ThisDocument.PropertyNodeName)
                            {
                                IMetaDataElement property = _selectedNode.MetaDataElement;
                                // do not insert relationship attribute
                                if (!(property is RelationshipAttributeElement))
                                {
                                    // in order to insert array or property node inside the view node
                                    // we need to change the selected range
                                    NavigationControl._selectedRange = xmlNode.Range;
                                    
                                    // insert a Property tag
                                    InsertPropertyNode(xmlNode, property);
                                }
                            }
                        }
                    }
                }
                else if (SelectedXmlNode.BaseName == ThisDocument.ViewNodeName)
                {
                    // insert attribute nodes
                    IMetaDataElement attribute = _selectedNode.MetaDataElement;
                    // do not insert relationship attribute
                    if (!(attribute is RelationshipAttributeElement))
                    {
                        // insert a Property tag
                        InsertPropertyNode(SelectedXmlNode, attribute);
                    }
                }
                else if (SelectedXmlNode.BaseName == ThisDocument.PropertyNodeName ||
                    SelectedXmlNode.BaseName == ThisDocument.ArrayNodeName)
                {
                }
            }
            else
            {
                // there is no currently selected node, insert a Database node if it doesn't exist.
                if (Globals.ThisDocument.XMLNodes.Count == 0)
                {
                    foreach (Word.XMLChildNodeSuggestion nodeSuggestion in Globals.ThisDocument.ChildNodeSuggestions)
                    {
                        xmlNode = nodeSuggestion.Insert(ref NavigationControl._selectedRange);
                    }
                    if (xmlNode != null)
                    {
                        attributeNode = xmlNode.Attributes.Add(ThisDocument.DatabaseAttribute, ThisDocument.NameSpaceUri, ref objMissing);
                        attributeNode.NodeValue = this._metaData.SchemaInfo.NameAndVersion;
                    }
                }
            }
        }

        // Insert a Property XML Tag
        private void InsertPropertyNode(Word.XMLNode parentXmlNode, IMetaDataElement attribute)
        {
            Word.XMLNode xmlNode = null;
            Word.XMLNode attributeNode;

            foreach (Word.XMLChildNodeSuggestion nodeSuggestion in parentXmlNode.ChildNodeSuggestions)
            {
                if (nodeSuggestion.BaseName == ThisDocument.PropertyNodeName)
                {
                    xmlNode = nodeSuggestion.Insert(ref NavigationControl._selectedRange);
                    break;
                }
            }
            if (xmlNode != null)
            {
                attributeNode = xmlNode.Attributes.Add(ThisDocument.PropertyNameAttribute, ThisDocument.NameSpaceUri, ref objMissing);
                attributeNode.NodeValue = attribute.Name;
            }
        }

        private void SetViewNodeAttributes(Word.XMLNode xmlNode, bool containTable)
        {
            SetViewNodeAttributes(xmlNode, containTable, null);
        }

        private void SetViewNodeAttributes(Word.XMLNode xmlNode, bool containTable, Stack<string> path)
        {
            Word.XMLNode attributeNode;

            if (xmlNode != null)
            {
                attributeNode = xmlNode.Attributes.Add(ThisDocument.ElementAttribute, ThisDocument.NameSpaceUri, ref objMissing);
                attributeNode.NodeValue = this._selectedNode.Name;
                if (containTable)
                {
                    // remember the table name as an attribute
                    attributeNode = xmlNode.Attributes.Add(ThisDocument.TableAttribute, ThisDocument.NameSpaceUri, ref objMissing);
                    attributeNode.NodeValue = this._selectedNode.Name;
                }

                // remember the path in the attribute
                if (path != null && path.Count > 0)
                {
                    StringBuilder builder = new StringBuilder();
                    while (path.Count > 0)
                    {
                        if (builder.Length > 0)
                        {
                            builder.Append(";");
                        }

                        builder.Append(path.Pop());
                    }

                    attributeNode = xmlNode.Attributes.Add(ThisDocument.PathAttribute, ThisDocument.NameSpaceUri, ref objMissing);
                    attributeNode.NodeValue = builder.ToString();
                }

                // a class node can represent several kinds of meta data elements,
                // remember the type of the element in an attribute
                attributeNode = xmlNode.Attributes.Add(ThisDocument.TypeAttribute, ThisDocument.NameSpaceUri, ref objMissing);
                if (_selectedMetaDataElement is ClassElement)
                {
                    attributeNode.NodeValue = ThisDocument.ClassType;
                }
            }
        }

        /// <summary>
        /// Gets the information indicating whether the element is the root element of an xml schema
        /// </summary>
        /// <param name="element">the element</param>
        /// <returns>true if the element is a root element, false otherwise</returns>
        private bool IsXMLSchemaRootElement(IMetaDataElement element)
        {
            bool status = false;

            if (_xmlSchemaModel.RootElement == element)
            {
                status = true;
            }

            return status;
        }

        /// <summary>
        /// Gets the information indicating whether the element is a complex element of an xml schema
        /// </summary>
        /// <param name="element">the element</param>
        /// <returns>true if the element is a complex element, false otherwise</returns>
        private bool IsXMLSchemaComplexElement(IMetaDataElement element)
        {
            bool status = false;

            foreach (XMLSchemaComplexType ct in _xmlSchemaModel.ComplexTypes)
            {
                if (ct == element)
                {
                    status = true;
                    break;
                }
            }

            return status;
        }

        /// <summary>
        /// Gets the information indicating whether the element is a simple element of an xml schema
        /// </summary>
        /// <param name="element">the element</param>
        /// <returns>true if the element is a simple element, false otherwise</returns>
        private bool IsXMLSchemaSimpleElement(IMetaDataElement element)
        {
            bool status = false;

            foreach (XMLSchemaComplexType ct in _xmlSchemaModel.ComplexTypes)
            {
                foreach (XMLSchemaElement ele in ct.Elements)
                {
                    if (ele == element)
                    {
                        status = true;
                        break;
                    }
                }

                if (status)
                {
                    break;
                }
            }

            return status;
        }

		#endregion

		private void treeView_AfterSelect(object sender, System.Windows.Forms.TreeViewEventArgs e)
		{
			// Get the selected node
			_selectedNode = (MetaDataTreeNode) e.Node;

            if (_selectedNode != null)
            {
                this.insertXMLSchemaNodeButton.Text = this.NavigationControl.InsertXMLNodeButtonText;

                if (SelectedXmlNode != null)
                {
                    this.insertXMLSchemaNodeButton.Enabled = false;

                    if (SelectedXmlNode.BaseName == ThisDocument.DatabaseNodeName)
                    {
                        if (IsXMLSchemaRootElement(_selectedNode.MetaDataElement))
                        {
                            this.insertXMLSchemaNodeButton.Enabled = true;
                        }
                    }
                    else if (SelectedXmlNode.BaseName == ThisDocument.FamilyNodeName)
                    {
                        if (IsXMLSchemaComplexElement(_selectedNode.MetaDataElement))
                        {
                            this.insertXMLSchemaNodeButton.Enabled = true;
                        }
                    }
                    else if (SelectedXmlNode.BaseName == ThisDocument.ViewNodeName)
                    {
                        if (IsXMLSchemaSimpleElement(_selectedNode.MetaDataElement))
                        {
                            this.insertXMLSchemaNodeButton.Enabled = true;
                        }
                    }
                }
                else
                {
                    this.insertXMLSchemaNodeButton.Enabled = true;
                }
            }
            else
            {
                this.insertXMLSchemaNodeButton.Text = this.NavigationControl.InsertXMLNodeButtonText;
                this.insertXMLSchemaNodeButton.Enabled = false;
            }
		}

		private void ChooseClassDialog_Load(object sender, System.EventArgs e)
		{
			if (_metaData != null && _selectedXMLSchemaName != null)
			{
                _xmlSchemaModel = (XMLSchemaModel)_metaData.XMLSchemaViews[_selectedXMLSchemaName];

				MetaDataTreeBuilder builder = new MetaDataTreeBuilder();

                // create a new tree node with children nodes
                MetaDataTreeNode root = (MetaDataTreeNode)builder.BuildXMLSchemaTree(_xmlSchemaModel);

				treeView.BeginUpdate();
				treeView.Nodes.Clear();
				treeView.Nodes.Add(root);

				// expand the node
				if (treeView.Nodes.Count > 0)
				{
                    if (treeView.Nodes[0].Nodes.Count > 0)
                    {
                        // expands to the first child node
                        treeView.Nodes[0].Expand();
                        treeView.Nodes[0].Nodes[0].Expand();
                    }
                    else
                    {
                        // expand the root node
                        treeView.Nodes[0].Expand();
                    }
				}

				treeView.EndUpdate();
			}		
		}

        private void insertXMLSchemaNodeButton_Click(object sender, EventArgs e)
        {
            if (_selectedNode != null)
            {
                // attach a custom principal to the thread
                _navigateControl.AttachCustomPrincipal();

                InsertXMLNode();
            }
        }

        private void cancelButton_Click(object sender, EventArgs e)
        {
            this.Close();
        }
	}
}
