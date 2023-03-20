using System;
using System.Drawing;
using System.Data;
using System.Xml;
using System.Collections;
using System.ComponentModel;
using System.Windows.Forms;

using Newtera.Common.MetaData;
using Newtera.Common.MetaData.Schema;
using Newtera.Common.MetaData.DataView;
using Newtera.Common.MetaData.DataView.Validate;
using Newtera.Common.MetaData.Rules;
using Newtera.WindowsControl;
using Newtera.WinClientCommon;

namespace Newtera.Studio
{
	/// <summary>
	/// Summary description for NewInstanceDialog.
	/// </summary>
	public class NewInstanceDialog : System.Windows.Forms.Form
	{
		private InstanceView _instanceView;
        private MetaDataModel _metaData;

		private System.Windows.Forms.Label classCaptionLabel;
		private System.Windows.Forms.Button okButton;
		private System.Windows.Forms.Button cancelButton;
		private System.Windows.Forms.PropertyGrid instancePropertyGrid;
		private System.Windows.Forms.Button validateButton;
		/// <summary>
		/// Required designer variable.
		/// </summary>
		private System.ComponentModel.Container components = null;

		public NewInstanceDialog(MetaDataModel metaData)
		{
			//
			// Required for Windows Form Designer support
			//
			InitializeComponent();

			//
			// TODO: Add any constructor code after InitializeComponent call
			//
			_instanceView = null;
            _metaData = metaData;
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
		/// Gets or sets the instance view of the new instance
		/// </summary>
		public InstanceView InstanceView
		{
			get
			{
				return _instanceView;
			}
			set
			{
				_instanceView = value;
			}
		}

        /// <summary>
        /// Gets or sets the meta data model of the instance
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

		#region Windows Form Designer generated code
		/// <summary>
		/// Required method for Designer support - do not modify
		/// the contents of this method with the code editor.
		/// </summary>
		private void InitializeComponent()
		{
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(NewInstanceDialog));
            this.classCaptionLabel = new System.Windows.Forms.Label();
            this.okButton = new System.Windows.Forms.Button();
            this.cancelButton = new System.Windows.Forms.Button();
            this.instancePropertyGrid = new System.Windows.Forms.PropertyGrid();
            this.validateButton = new System.Windows.Forms.Button();
            this.SuspendLayout();
            // 
            // classCaptionLabel
            // 
            resources.ApplyResources(this.classCaptionLabel, "classCaptionLabel");
            this.classCaptionLabel.Name = "classCaptionLabel";
            // 
            // okButton
            // 
            resources.ApplyResources(this.okButton, "okButton");
            this.okButton.DialogResult = System.Windows.Forms.DialogResult.OK;
            this.okButton.Name = "okButton";
            this.okButton.Click += new System.EventHandler(this.okButton_Click);
            // 
            // cancelButton
            // 
            resources.ApplyResources(this.cancelButton, "cancelButton");
            this.cancelButton.DialogResult = System.Windows.Forms.DialogResult.Cancel;
            this.cancelButton.Name = "cancelButton";
            // 
            // instancePropertyGrid
            // 
            resources.ApplyResources(this.instancePropertyGrid, "instancePropertyGrid");
            this.instancePropertyGrid.CausesValidation = false;
            this.instancePropertyGrid.LineColor = System.Drawing.SystemColors.ScrollBar;
            this.instancePropertyGrid.Name = "instancePropertyGrid";
            this.instancePropertyGrid.Validating += new System.ComponentModel.CancelEventHandler(this.InstanceProperty_Validating);
            // 
            // validateButton
            // 
            resources.ApplyResources(this.validateButton, "validateButton");
            this.validateButton.Name = "validateButton";
            this.validateButton.Click += new System.EventHandler(this.validateButton_Click);
            // 
            // NewInstanceDialog
            // 
            resources.ApplyResources(this, "$this");
            this.Controls.Add(this.validateButton);
            this.Controls.Add(this.instancePropertyGrid);
            this.Controls.Add(this.cancelButton);
            this.Controls.Add(this.okButton);
            this.Controls.Add(this.classCaptionLabel);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.SizableToolWindow;
            this.Name = "NewInstanceDialog";
            this.Load += new System.EventHandler(this.NewInstanceDialog_Load);
            this.ResumeLayout(false);

		}
		#endregion

		/// <summary>
		/// validate the instance data
		/// </summary>
		/// <returns>True if the data is valid, false otherwise.</returns>
		private bool ValidateData()
		{
			bool status = true;
            DataValidateErrorDialog dialog;

            // validate the instance data
            DataViewValidateResult validateResult = _instanceView.DataView.ValidateData();

			if (validateResult.HasError)
			{
				status = false;
				dialog = new DataValidateErrorDialog();
				dialog.Entries = validateResult.Errors;
				dialog.ShowDialog();
			}
            else if (validateResult.HasDoubt)
            {
                // determine if the doubts are errors or not
                ValidateDoubts(validateResult);

                // ValidateDoubts method call may turn doubts into an errors
                if (validateResult.HasError)
                {
                    status = false;
                    dialog = new DataValidateErrorDialog();
                    dialog.Entries = validateResult.Errors;
                    dialog.ShowDialog();
                }
            }

            if (status)
            {
                // finally verify the instance data using validating rules if exist.
                ClassElement classElement = _metaData.SchemaModel.FindClass(_instanceView.DataView.BaseClass.Name);
                RuleCollection validatingRules = _metaData.RuleManager.GetPrioritizedRules(classElement).Rules;
                if (validatingRules != null && validatingRules.Count > 0)
                {
                    ValidateUsingRules(_instanceView.DataView, validateResult, validatingRules, classElement);
                    if (validateResult.HasError)
                    {
                        status = false;
                        dialog = new DataValidateErrorDialog();
                        dialog.Entries = validateResult.Errors;
                        dialog.ShowDialog();
                    }
                }
            }

			return status;
		}

        /// <summary>
        /// validate the doubts raised by validating data process
        /// </summary>
        /// <param name="validateResult"></param>
        private void ValidateDoubts(DataViewValidateResult validateResult)
        {
            foreach (DataValidateResultEntry doubt in validateResult.Doubts)
            {
                if (doubt.EntryType == Newtera.Common.MetaData.DataView.Validate.EntryType.PrimaryKey)
                {
                    if (IsPKValueExists())
                    {
                        // the primary key value exists, change the doubt into error
                        doubt.EntryType = Newtera.Common.MetaData.DataView.Validate.EntryType.Error;
                        validateResult.AddError(doubt);
                    }
                }
                else if (doubt.EntryType == Newtera.Common.MetaData.DataView.Validate.EntryType.UniqueValue)
                {
                    if (!IsValueUnique())
                    {
                        // the value isn't unique, change the doubt into error
                        doubt.EntryType = Newtera.Common.MetaData.DataView.Validate.EntryType.Error;
                        validateResult.AddError(doubt);
                    }
                }
                else if (doubt.EntryType == Newtera.Common.MetaData.DataView.Validate.EntryType.UniqueReference)
                {
                    if (!IsReferenceUnique(doubt.DataViewElement))
                    {
                        // the reference isn't unique, change the doubt into error
                        doubt.EntryType = Newtera.Common.MetaData.DataView.Validate.EntryType.Error;
                        validateResult.AddError(doubt);
                    }
                }
                else if (doubt.EntryType == Newtera.Common.MetaData.DataView.Validate.EntryType.UniqueValues)
                {
                    if (!IsCombinedValuesUnique(doubt.ClassName))
                    {
                        // the combination of values isn't unique, change the doubt into error
                        doubt.EntryType = Newtera.Common.MetaData.DataView.Validate.EntryType.Error;
                        validateResult.AddError(doubt);
                    }
                }
            }
        }

        /// <summary>
        /// Gets the information indicating whether the primary key value are already used by
        /// another instance
        /// </summary>
        /// <returns>True if it's been used, false otherwise</returns>
        private bool IsPKValueExists()
        {
            bool status = false;

            CMDataServiceStub dataService = new CMDataServiceStub();

            string query = _instanceView.DataView.GetInstanceByPKQuery();
            if (query != null)
            {

                // invoke the web service synchronously
                XmlNode xmlNode = dataService.ExecuteQuery(ConnectionStringBuilder.Instance.Create(_metaData.SchemaInfo),
                    query);

                DataSet ds = new DataSet();

                XmlReader xmlReader = new XmlNodeReader(xmlNode);
                ds.ReadXml(xmlReader);

                // if the result isn't empty, the instance with the same primary key value exists
                if (!DataSetHelper.IsEmptyDataSet(ds, _instanceView.DataView.BaseClass.ClassName))
                {
                    status = true;
                }
            }

            return status;
        }

        /// <summary>
        /// Gets the information indicating whether the indicated value are unique among the
        /// same class.
        /// </summary>
        /// <returns>True if it is unque, false otherwise</returns>
        private bool IsValueUnique()
        {
            bool status = true;

            // to be implemented

            return status;
        }

        /// <summary>
        /// Gets the information indicating whether a combination of values is unique among the
        /// same class.
        /// </summary>
        /// <param name="className">The name of the class uniquely constrainted.</param>
        /// <returns>True if it is unque, false otherwise</returns>
        private bool IsCombinedValuesUnique(string className)
        {
            bool status = true;

            CMDataServiceStub dataService = new CMDataServiceStub();

            string query = _instanceView.DataView.GetInstanceByUniqueKeysQuery(className);
            if (query != null)
            {

                // invoke the web service synchronously
                XmlNode xmlNode = dataService.ExecuteQuery(ConnectionStringBuilder.Instance.Create(_metaData.SchemaInfo),
                    query);

                DataSet ds = new DataSet();

                XmlReader xmlReader = new XmlNodeReader(xmlNode);
                ds.ReadXml(xmlReader);

                // if the result isn't empty, the instance with the same unique key value exists
                if (!DataSetHelper.IsEmptyDataSet(ds, _instanceView.DataView.BaseClass.ClassName))
                {
                    status = false;
                }
            }

            return status;
        }

        /// <summary>
        /// Gets the information indicating whether a reference to an instance is unique among the
        /// same class.
        /// </summary>
        /// <param name="element">IDataViewElement representing a relationship element</param>
        /// <returns>True if the reference is unque, false otherwise</returns>
        private bool IsReferenceUnique(IDataViewElement element)
        {
            bool status = true;

            DataRelationshipAttribute relationshipAttribute = element as DataRelationshipAttribute;

            if (relationshipAttribute != null && relationshipAttribute.HasValue)
            {
                CMDataServiceStub dataService = new CMDataServiceStub();

                string query = _instanceView.DataView.GetInstancesQuery(element);
                if (query != null)
                {

                    // invoke the web service synchronously
                    XmlNode xmlNode = dataService.ExecuteQuery(ConnectionStringBuilder.Instance.Create(_metaData.SchemaInfo),
                        query);

                    DataSet ds = new DataSet();

                    XmlReader xmlReader = new XmlNodeReader(xmlNode);
                    ds.ReadXml(xmlReader);

                    // if the result isn't empty, there are instances that have reference to the same instance
                    if (!DataSetHelper.IsEmptyDataSet(ds, _instanceView.DataView.BaseClass.ClassName))
                    {
                        status = false;
                    }
                }
            }

            return status;
        }

        /// <summary>
        /// Validate the instance data using the validating rules
        /// </summary>
        /// <param name="dataView">The data view that holds an instance data to be validated against.</param>
        /// <param name="validateResult">The validating results.</param>
        /// <param name="rules">The validating rules.</param>
        /// <param name="classElement">The rule owner class.</param>
        private void ValidateUsingRules(DataViewModel dataView, DataViewValidateResult validateResult, RuleCollection rules, ClassElement classElement)
        {
            DataValidateResultEntry entry = null;
            string message;
            string query;

            CMDataServiceStub dataService = new CMDataServiceStub();

            foreach (RuleDef ruleDef in rules)
            {
                // generating a validating query based on the rule definition
                query = dataView.GetRuleValidatingQuery(ruleDef);

                // invoke the web service synchronously
                message = dataService.ExecuteValidatingQuery(ConnectionStringBuilder.Instance.Create(_metaData.SchemaInfo), query);

                RuleValidateResult result = new RuleValidateResult(message);
                if (result.HasError)
                {
                    entry = new DataValidateResultEntry(result.Message, classElement.Caption, null);
                    validateResult.AddError(entry);
                }
            }
        }

		private void NewInstanceDialog_Load(object sender, System.EventArgs e)
		{
			if (_instanceView != null)
			{
				this.classCaptionLabel.Text = _instanceView.DataView.BaseClass.Caption;

				this.instancePropertyGrid.SelectedObject = _instanceView;
			}
		}

		private void InstanceProperty_Validating(object sender, System.ComponentModel.CancelEventArgs e)
		{
			this.instancePropertyGrid.CausesValidation = false;
			if (!ValidateData())
			{
				e.Cancel = true;
			}		
		}

		private void okButton_Click(object sender, System.EventArgs e)
		{
			// validate the data in instance property grid
            if (!this.ValidateData())
            {
                this.DialogResult = DialogResult.None;
                return;
            }
		}

		private void validateButton_Click(object sender, System.EventArgs e)
		{
			if (this.ValidateData())
			{
				MessageBox.Show(MessageResourceManager.GetString("DataViewer.ValidData"),
					"Info",
					MessageBoxButtons.OK,
					MessageBoxIcon.Information);
			}
		}
	}
}
