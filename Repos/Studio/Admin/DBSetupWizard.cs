/*
* @(#)DBSetupWizard.cs
*
* Copyright (c) 2003 Newtera, Inc. All rights reserved.
*
*/
namespace Newtera.Studio
{
	using System;
	using System.Xml;
	using System.Windows.Forms;

	using Newtera.Common.Core;
    using Newtera.WinClientCommon;

    /// <summary>
    /// A class that controls the flow of a wizard to set up database tablespace
    /// and system tables that are required by Newtera Enterprise Catalog Management
    /// Software.
    /// </summary>
    /// <version>  1.0.0 2 Feb 2004 </version>
    /// <author> Yong Zhang </author>
    public class DBSetupWizard
	{
		private DBSetupWelcom _welcomeDialog;
		private DBSetupDatabaseType _dbTypeDialog;
		private DBSetupTNSName _tnsNameDialog;
		private DBSetupDataSourceName _dsNameDialog;
		private DBSetupDBAAccount _createTablespaceDialog;
		private DBSetupSchema _updateSchemaDialog;
		private ImageStoreSetup _imageStoreSetupDialog;
		private DBSetupComplete _setupCompleteDialog;

		private string _databaseType;
		private string _dataSource;
		private string _dbaUser;
		private string _dbaPassword;
		private string _dataFileDir;
		private string _imageBaseURL;
		private string _imageBasePath;

		/// <summary>
		/// Initiate an instance of a DBSetupWizard
		/// </summary>
		public DBSetupWizard()
		{
			_welcomeDialog = null;
			_dbTypeDialog = null;
			_tnsNameDialog = null;
			_createTablespaceDialog = null;
			_updateSchemaDialog = null;
			_imageStoreSetupDialog = null;
			_setupCompleteDialog = null;

			_databaseType = null;
			_dataSource = null;
			_dbaUser = null;
			_dbaPassword = null;
			_dataFileDir = null;
			_imageBaseURL = null;
			_imageBasePath = null;
		}

		/// <summary>
		/// Gets or sets a string that represents database type
		/// </summary>
		/// <value>A database type string, for example, "Oracle"</value>
		public string DatabaseType
		{
			get
			{
				return _databaseType;
			}
			set
			{
				_databaseType = value;
			}
		}

		/// <summary>
		/// Gets or sets a string that represents data source
		/// </summary>
		/// <value>A data source string, for example, "Newtera"</value>
		public string DataSource
		{
			get
			{
				return _dataSource;
			}
			set
			{
				_dataSource = value;
			}
		}

		/// <summary>
		/// Gets or sets ID of a DBA user
		/// </summary>
		/// <value>An ID of DBA user</value>
		public string DBAUserID
		{
			get
			{
				return _dbaUser;
			}
			set
			{
				_dbaUser = value;
			}
		}

		/// <summary>
		/// Gets or sets Password of a DBA user
		/// </summary>
		/// <value>An Password of DBA user</value>
		public string DBAUserPassword
		{
			get
			{
				return _dbaPassword;
			}
			set
			{
				_dbaPassword = value;
			}
		}

		/// <summary>
		/// Gets or sets string represents a data file directory
		/// </summary>
		/// <value>Data File Directory string</value>
		public string DataFileDir
		{
			get
			{
				return _dataFileDir;
			}
			set
			{
				_dataFileDir = value;
			}
		}

		/// <summary>
		/// Gets or sets string represents a base url for getting catalog images
		/// </summary>
		/// <value>A image base url string</value>
		public string ImageBaseURL
		{
			get
			{
				return _imageBaseURL;
			}
			set
			{
				_imageBaseURL = value;
			}
		}

		/// <summary>
		/// Gets or sets string represents a base path for storing catalog images
		/// </summary>
		/// <value>A image base path string</value>
		public string ImageBasePath
		{
			get
			{
				return _imageBasePath;
			}
			set
			{
				_imageBasePath = value;
			}
		}

		/// <summary>
		/// Launch the DB Setup Wizard
		/// </summary>
		public void Launch()
		{
			if (_welcomeDialog == null)
			{
				_welcomeDialog = new DBSetupWelcom();
			}

			if (_welcomeDialog.ShowDialog() == DialogResult.OK)
			{
				ChooseDatabaseType();
			}
		}

		/// <summary>
		/// Show a dialog for choosing a database type.
		/// </summary>
		private void ChooseDatabaseType()
		{
			if (_dbTypeDialog == null)
			{
				_dbTypeDialog = new DBSetupDatabaseType();
				_dbTypeDialog.Wizard = this;
			}

			if (_dbTypeDialog.ShowDialog() == DialogResult.OK)
			{
				if (_dbTypeDialog.ButtonTag == WizardButtonTag.Back)
				{
					Launch();
				}
				else if (_dbTypeDialog.ButtonTag == WizardButtonTag.Next)
				{
					if (_dbTypeDialog.DataBaseType == "Oracle")
					{
						GetTNSName();
					}
					else if (_dbTypeDialog.DataBaseType == "MySql")
					{
						GetDataSourceName("MySql", "localhost", true);
					}
					else if (_dbTypeDialog.DataBaseType == "SQLServer")
					{
						GetDataSourceName("SQLServer", "localhost", true);
					}
                    else if (_dbTypeDialog.DataBaseType == "SQLServerCe")
                    {
                        GetDataSourceName("SQLServerCe", "newtera.sdf", false);
                    }
                }
			}
		}

		/// <summary>
		/// Get the TNS name to connection an Oracle database.
		/// </summary>
		private void GetTNSName()
		{
			if (_tnsNameDialog == null)
			{
				_tnsNameDialog = new DBSetupTNSName();
				_tnsNameDialog.Wizard = this;
			}
			
			if (_tnsNameDialog.ShowDialog() == DialogResult.OK)
			{
				if (_tnsNameDialog.ButtonTag == WizardButtonTag.Back)
				{
					ChooseDatabaseType();
				}
				else if (_tnsNameDialog.ButtonTag == WizardButtonTag.Next)
				{
					if (_tnsNameDialog.NeedCreateTableSpace)
					{
						CreateTableSpace();
					}
					else
					{
						UpdateSchema();
					}
				}
			}
		}

		/// <summary>
		/// Get the data source name that identifies a SQL Server.
		/// </summary>
		private void GetDataSourceName(string databaseType, string defaultName, bool editable)
		{
			if (_dsNameDialog == null)
			{
				_dsNameDialog = new DBSetupDataSourceName();
				_dsNameDialog.Wizard = this;
                _dsNameDialog.DataSourceName = defaultName;
                _dsNameDialog.Editable = editable;
			}
			
			if (_dsNameDialog.ShowDialog() == DialogResult.OK)
			{
				if (_dsNameDialog.ButtonTag == WizardButtonTag.Back)
				{
					ChooseDatabaseType();
				}
				else if (_dsNameDialog.ButtonTag == WizardButtonTag.Next)
				{
					if (_dsNameDialog.NeedCreateDatabase)
					{
						CreateDatabase(databaseType);
					}
					else
					{
						UpdateSchema();
					}
				}
			}
		}

        /// <summary>
        /// Create a Oracle table space for Newtera catalog data
        /// </summary>
        private void CreateTableSpace()
		{
			if (_createTablespaceDialog == null)
			{
				_createTablespaceDialog = new DBSetupDBAAccount();
				_createTablespaceDialog.Wizard = this;
			}

			if (_createTablespaceDialog.ShowDialog() == DialogResult.OK)
			{
				if (_createTablespaceDialog.ButtonTag == WizardButtonTag.Back)
				{
					GetTNSName();
				}
				else if (_createTablespaceDialog.ButtonTag == WizardButtonTag.Next)
				{
					UpdateSchema();
				}
			}
		}

		/// <summary>
		/// To be implemented for SQL Server
		/// </summary>
		private void CreateDatabase(string dataBaseType)
		{
			switch (dataBaseType)
            {
				case "MySql":
					CreateMySqlDatabase();

					UpdateSchema();
					break;

				case "SQLServer":
					DBSetupError errorDialog = new DBSetupError();
					errorDialog.ShowDialog();
					break;

				case "SQLServerCe":
					_dsNameDialog.CreateDatabase();

					UpdateSchema();
					break;
			}
		}

		private void CreateMySqlDatabase()
		{
			try
			{
				var service = new AdminServiceStub();
				// invoke the admin web service synchronously
				service.CreateTablespace(this.DatabaseType, this.DataSource,
					this.DBAUserID, this.DBAUserPassword, string.Empty);
			}
			catch (Exception ex)
			{
				MessageBox.Show(ex.Message, "Server Error",
					MessageBoxButtons.OK,
					MessageBoxIcon.Error);
			}
		}

		/// <summary>
		/// Create or update the DB schema used by Newtera Catalog
		/// </summary>
		private void UpdateSchema()
		{
			if (_updateSchemaDialog == null)
			{
				_updateSchemaDialog = new DBSetupSchema();
				_updateSchemaDialog.Wizard = this;
			}

			if (_updateSchemaDialog.ShowDialog() == DialogResult.OK)
			{
				if (_updateSchemaDialog.ButtonTag == WizardButtonTag.Back)
				{
					CreateTableSpace();
				}
				else if (_updateSchemaDialog.ButtonTag == WizardButtonTag.Next)
				{
                    UpdateServerConfig(); // 7.0 version doesn't need to setup image store
                    //EnterImageStoreParameters();
				}
			}
		}

		/// <summary>
		/// Show a dialog for entering image store parameters.
		/// </summary>
		private void EnterImageStoreParameters()
		{
			if (_imageStoreSetupDialog == null)
			{
				_imageStoreSetupDialog = new ImageStoreSetup();
				_imageStoreSetupDialog.Wizard = this;
			}

			if (_imageStoreSetupDialog.ShowDialog() == DialogResult.OK)
			{
				if (_imageStoreSetupDialog.ButtonTag == WizardButtonTag.Next)
				{
					UpdateServerConfig();
				}
			}
		}

		/// <summary>
		/// Show a dialog to update the server config file.
		/// </summary>
		private void UpdateServerConfig()
		{
			DBSetupServerConfig	updateServerConfigDialog = new DBSetupServerConfig();
			updateServerConfigDialog.Wizard = this;

			if (updateServerConfigDialog.ShowDialog() == DialogResult.OK)
			{
				if (updateServerConfigDialog.ButtonTag == WizardButtonTag.Next)
				{
					SetupAppSchemas();
				}
			}
		}

		/// <summary>
		/// Show a dialog to load application schemas to the server.
		/// </summary>
		private void SetupAppSchemas()
		{
			DBSetupAppSchemas setupAppSchemasDialog = new DBSetupAppSchemas();
			setupAppSchemasDialog.Wizard = this;

			if (setupAppSchemasDialog.ShowDialog() == DialogResult.OK)
			{
				if (setupAppSchemasDialog.ButtonTag == WizardButtonTag.Next)
				{
					SetupComplete();
				}
			}
		}

		/// <summary>
		/// Inform the user that DB setup has been completed.
		/// </summary>
		private void SetupComplete()
		{
			if (_setupCompleteDialog == null)
			{
				_setupCompleteDialog = new DBSetupComplete();
				_setupCompleteDialog.Wizard = this;
			}

			_setupCompleteDialog.ShowDialog();
		}
	}


	internal enum WizardButtonTag
	{
		None = 0,
		Cancel,
		Back,
		Next,
		Finish
	}
}