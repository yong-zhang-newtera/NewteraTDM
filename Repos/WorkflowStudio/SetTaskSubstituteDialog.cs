using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.IO;
using System.Xml;
using System.Drawing;
using System.Text;
using System.Windows.Forms;

using Newtera.WFModel;
using Newtera.WindowsControl;
using Newtera.WinClientCommon;

namespace WorkflowStudio
{
    public partial class SetTaskSubstituteDialog : Form
    {
        private bool _isLockObtained = false;
        private TaskSubstituteModel _model;
        private TreeNode _selectedTreeNode = null;

        public SetTaskSubstituteDialog()
        {
            InitializeComponent();
        }

        #region private methods

        private void DisplayTaskSubstituteModel(IWFModelElement currentElement)
        {
            TreeNode root = CreateTreeNode(_model);
            if (currentElement == _model)
            {
                _selectedTreeNode = root;
            }

            foreach (SubjectEntry subject in _model.SubjectEntries)
            {
                AddSubjectNode(root, subject, currentElement);
            }

            treeView.BeginUpdate();
            treeView.Nodes.Clear();
            treeView.Nodes.Add(root);
            if (_selectedTreeNode != null)
            {
                treeView.SelectedNode = _selectedTreeNode;
            }

            root.Expand();

            treeView.EndUpdate();
        }

        private void AddSubjectNode(TreeNode parent, SubjectEntry subject, IWFModelElement currentElement)
        {
            TreeNode subjectNode = CreateTreeNode(subject);
            parent.Nodes.Add(subjectNode);
            if (subject == currentElement)
            {
                _selectedTreeNode = subjectNode;
            }

            TreeNode substituteNode;
            foreach (SubstituteEntry substitute in subject.SubstituteEntries)
            {
                substituteNode = CreateTreeNode(substitute);
                subjectNode.Nodes.Add(substituteNode);

                if (substitute == currentElement)
                {
                    _selectedTreeNode = substituteNode;
                }
            }
        }

        private TreeNode CreateTreeNode(IWFModelElement element)
        {
            WFModelTreeNode treeNode = new WFModelTreeNode(element);

            switch (element.ElementType)
            {
                case ElementType.TaskSubstituteModel:
                    treeNode.Text = MessageResourceManager.GetString("WorkflowStudio.TaskSubstituteModel");
                    treeNode.ImageIndex = 0;
                    treeNode.SelectedImageIndex = 0;
                    break;

                case ElementType.SubjectEntry:
                    treeNode.Text = element.ToString();
                    treeNode.ImageIndex = 1;
                    treeNode.SelectedImageIndex = 1;
                    break;

                case ElementType.SubstituteEntry:
                    treeNode.Text = element.ToString();
                    treeNode.ImageIndex = 2;
                    treeNode.SelectedImageIndex = 2;
                    break;
            }

            return treeNode;
        }

        private void AddSubjectEntry(TaskSubstituteModel model)
        {
            SelectSubjectDialog dialog = new SelectSubjectDialog();
            if (dialog.ShowDialog() == DialogResult.OK)
            {
                if (!IsSubjectExist(dialog.SelectedSubjectName))
                {
                    SubjectEntry subjectEntry = new SubjectEntry(dialog.SelectedSubjectName,
                        dialog.SelectedSubjectDisplayText);

                    model.AddSubjectEntry(subjectEntry);

                    DisplayTaskSubstituteModel(subjectEntry);
                }
                else
                {
                    MessageBox.Show(MessageResourceManager.GetString("WorkflowStduio.SubjectExist"), "Info",
                        MessageBoxButtons.OK, MessageBoxIcon.Information);
                }
            }
        }

        private bool IsSubjectExist(string subjectName)
        {
            bool status = false;

            foreach (SubjectEntry entry in _model.SubjectEntries)
            {
                if (entry.Name == subjectName)
                {
                    status = true;

                    break;
                }
            }

            return status;
        }

        private void AddSubstituteEntry(SubjectEntry subjectEntry)
        {
            EnterNameDialog dialog = new EnterNameDialog();
            // set default name
            dialog.EnterName = MessageResourceManager.GetString("WorkflowStudio.Rule") + (subjectEntry.SubstituteEntries.Count + 1);
            if (dialog.ShowDialog() == DialogResult.OK)
            {
                SubstituteEntry substituteEntry = new SubstituteEntry(dialog.EnterName);

                subjectEntry.AddSubstituteEntry(substituteEntry);

                DisplayTaskSubstituteModel(substituteEntry);
            }
        }

        private void DeleteSubjectEntry(SubjectEntry subjectEntry)
        {
            _model.RemoveSubjectEntry(subjectEntry);

            DisplayTaskSubstituteModel(_model);
        }

        private void DeleteSubstituteEntry(SubstituteEntry substituteEntry)
        {
            SubjectEntry parentSubjectEntry = null;

            foreach (SubjectEntry subjectEntry in _model.SubjectEntries)
            {
                if (subjectEntry.Contains(substituteEntry))
                {
                    parentSubjectEntry = subjectEntry;
                    subjectEntry.RmoveSubstituteEntry(substituteEntry);
                    break;
                }
            }

            if (parentSubjectEntry != null)
            {
                DisplayTaskSubstituteModel(parentSubjectEntry);
            }
        }

        private void AddEntry()
        {
            if (this.treeView.SelectedNode != null)
            {
                WFModelTreeNode node = (WFModelTreeNode)this.treeView.SelectedNode;
                switch (node.WFModelElement.ElementType)
                {
                    case ElementType.TaskSubstituteModel:
                        AddSubjectEntry((TaskSubstituteModel)node.WFModelElement);
                        break;

                    case ElementType.SubjectEntry:
                        AddSubstituteEntry((SubjectEntry)node.WFModelElement);
                        break;

                    case ElementType.SubstituteEntry:
                        break;
                }
            }
        }

        private void DeleteEntry()
        {
            if (this.treeView.SelectedNode != null)
            {
                WFModelTreeNode node = (WFModelTreeNode)this.treeView.SelectedNode;
                switch (node.WFModelElement.ElementType)
                {
                    case ElementType.TaskSubstituteModel:
                        break;

                    case ElementType.SubjectEntry:
                        DeleteSubjectEntry((SubjectEntry)node.WFModelElement);
                        break;

                    case ElementType.SubstituteEntry:
                        DeleteSubstituteEntry((SubstituteEntry)node.WFModelElement);
                        break;
                }
            }
        }

        #endregion

        private void addButton_Click(object sender, EventArgs e)
        {
            AddEntry();
        }

        private void deleteButton_Click(object sender, EventArgs e)
        {
            DeleteEntry();
        }

        private void treeView_AfterSelect(object sender, TreeViewEventArgs e)
        {
            // Get the selected node
			WFModelTreeNode node = (WFModelTreeNode) e.Node;

            if (node != null)
            {
                if (node.WFModelElement.ElementType == ElementType.SubstituteEntry)
                {
                    this.propertyGrid.SelectedObject = node.WFModelElement;

                    this.addButton.Enabled = false;
                    this.addToolStripMenuItem.Enabled = false;
                    this.deleteButton.Enabled = true;
                    this.deleteToolStripMenuItem.Enabled = true;
                }
                else if (node.WFModelElement.ElementType == ElementType.TaskSubstituteModel)
                {
                    this.propertyGrid.SelectedObject = null;
                    this.addButton.Enabled = true;
                    this.addToolStripMenuItem.Enabled = true;
                    this.deleteButton.Enabled = false;
                    this.deleteToolStripMenuItem.Enabled = false;
                }
                else if (node.WFModelElement.ElementType == ElementType.SubjectEntry)
                {
                    this.propertyGrid.SelectedObject = null;
                    this.addButton.Enabled = true;
                    this.addToolStripMenuItem.Enabled = true;
                    this.deleteButton.Enabled = true;
                    this.deleteToolStripMenuItem.Enabled = true;
                }
            }
        }

        private void SetTaskSubstituteDialog_Load(object sender, EventArgs e)
        {
            // Change the cursor to indicate that we are waiting
            Cursor.Current = Cursors.WaitCursor;

            // get model to display first
            try
            {
                WorkflowModelServiceStub service = new WorkflowModelServiceStub();

                string xml = service.GetTaskSubstituteModel(ConnectionStringBuilder.Instance.Create());

                _model = new TaskSubstituteModel();

                if (!string.IsNullOrEmpty(xml))
                {
                    StringReader reader = new StringReader(xml);
                    _model.Read(reader);
                }

                DisplayTaskSubstituteModel(_model);

                // try to get the lock
                try
                {
                    service.LockTaskSubstituteModel(ConnectionStringBuilder.Instance.Create());

                    _isLockObtained = true;
                }
                catch (Exception)
                {
                    // unable to lock the model, check if want to unlock it forcefully
                    if (MessageBox.Show(MessageResourceManager.GetString("WorkflowStudio.TaskSubstituteLocked"), "Confirm",
                        MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.Yes)
                    {
                        service.UnlockTaskSubstituteModel(ConnectionStringBuilder.Instance.Create(), true);
                    }

                    this.okButton.Enabled = false;

                    _isLockObtained = false;
                }

            }
            finally
            {
                Cursor.Current = this.Cursor;
            }
        }

        private void SetTaskSubstituteDialog_FormClosing(object sender, FormClosingEventArgs e)
        {
            if (_isLockObtained)
            {
                // release the lock
                WorkflowModelServiceStub service = new WorkflowModelServiceStub();

                service.UnlockTaskSubstituteModel(ConnectionStringBuilder.Instance.Create(), false);
            }
        }

        private void okButton_Click(object sender, EventArgs e)
        {
            Cursor.Current = Cursors.WaitCursor;

            try
            {
                // save the task substitute model to database
                WorkflowModelServiceStub service = new WorkflowModelServiceStub();

                StringBuilder builder = new StringBuilder();
                StringWriter writer = new StringWriter(builder);
                _model.Write(writer);
                string xml = builder.ToString();

                service.UpdateTaskSubstituteModel(ConnectionStringBuilder.Instance.Create(),
                    xml);
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
                this.DialogResult = DialogResult.None;
            }
            finally
            {
                Cursor.Current = this.Cursor;
            }
        }

        private void addToolStripMenuItem_Click(object sender, EventArgs e)
        {
            AddEntry();
        }

        private void deleteToolStripMenuItem_Click(object sender, EventArgs e)
        {
            DeleteEntry();
        }
    }

    internal class WFModelTreeNode : TreeNode
    {
        private IWFModelElement _element;

        public WFModelTreeNode(IWFModelElement element)
        {
            _element = element;
        }

        public IWFModelElement WFModelElement
        {
            get
            {
                return _element;
            }
        }
    }
}