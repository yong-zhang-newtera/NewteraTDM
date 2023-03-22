/*
* @(#)NewteraNameSpace.cs
*
* Copyright (c) 2003 Newtera, Inc. All rights reserved.
*
*/
namespace Newtera.Common.Core
{
	using System;
    using System.Web.SessionState;
    using System.IO;
    using System.Web;
	using System.Configuration;

    using Newtera.Registry;

	/// <summary>
	/// The NewteraNameSpace class is a centralized place where keywords belong to 
	/// Newtera name space are defined as constants. Application will refer to the 
	/// constants rather than directly to the keywords. Therefore, if any changes 
	/// of keywords in Newtera name space won't affect the application code.
	/// </summary>
	/// <version>      1.0.0 26 Jun 2003
	/// </version>
	public class NewteraNameSpace
	{
        private const string RegCode = "1KF6M-RNSH5-B1SG6-LR3AU-K9LFM"; // getdiskserial license

		/// <summary>
		/// The computer checksum
		/// </summary>
		internal static string COMPUTER_ID = null;

        internal static string SERVER_BASE_URL = null;

        private const char MACHINE_ID_SEPARATOR = '-';

        /// <summary>
        /// The software release version, which needs to be updated for each release
        /// </summary>
        public const string RELEASE_VERSION = "8.0.0";

        /// <summary>
        /// Maximum number of attributes per class
        /// </summary>
        public const int ATTRIBUTES_PER_CLASS = 1000;


        /// <summary>
        /// The constant for None value
        /// </summary>
        public const string NONE = "None";

		/// <summary>
		/// The constant definition representing the database column name for
		/// internally created obj id.
		/// </summary>
		public const string OBJ_ID = "obj_id";

        // database connection string used by a request
        public const string CURRENT_CONNECTION = "CurrentConnection";

        /// <summary>
        /// The constant definition representing the CM super user role
        /// </summary>
        // cm super user role
        public const string CM_SUPER_USER_ROLE = "cm_super_user";
		/// <summary>
		/// The constant definition representing the TYPE keyword
		/// </summary>
		public const string TYPE = "type";
		/// <summary>
		/// The constant definition representing suffix of a List keyword
		/// </summary>
		public const string LIST_SUFFIX = "List";
		/// <summary>
		/// The constant definition representing attachment keyword
		/// </summary>
		public const string ATTACHMENTS = "attachments";
		/// <summary>
		/// The constant definition representing permission keyword
		/// </summary>
		public const string PERMISSION = "permission";
        /// <summary>
        /// The constant definition representing read-only columns
        /// </summary>
        public const string READ_ONLY = "read_only";

		/// <summary>
		/// The object id xsd:attribute element's name
		/// </summary>
		public const string OBJ_ID_ATTRIBUTE = "@obj_id";

		/// <summary>
		/// The constant definition representing the name of root table
		/// </summary>
		public const string CM_ROOT_TABLE = "CM_ROOT";

        /// <summary>
        /// Attribute prefix
        /// </summary>
        public const string ATTRIBUTE_SUFFIX = @"~";

		/// <summary>
		/// The short name for Design Studio client
		/// </summary>
		public const string DESIGN_STUDIO_NAME = "DS";
        public const string WORKFLOW_STUDIO_NAME = "WS";
        public const string SMART_WORD_NAME = "SW";
        public const string SMART_EXCEL_NAME = "SE";

		/// <summary>
		/// LICENSE_KEY definition for Web.Config
		/// </summary>
		//public const string LICENSE_KEY = @"LicenseKey";

		internal const string TIGHT_REFERENCED = "tightReferenced";
		internal const string LOOSE_REFERENCED = "looseReferenced";
		internal const string OWNED = "owned";
		
		internal const string ONE_TO_ONE = "oneToOne";
		internal const string ONE_TO_MANY = "oneToMany";
		internal const string MANY_TO_ONE = "manyToOne";
		internal const string MANY_TO_MANY = "manyToMany";
		
		/* Keywords definitions */
		internal const string PREFIX = "psd";
		internal const string ID = "id";
		internal const string BASE = "base";
		internal const string NAME = "name";
		internal const string VERSION = "version";
		internal const string MAPPING_METHOD = "mappingMethod";
        internal const string MODIFIED_TIME = "modifiedTime";
		internal const string DESCRIPTION = "description";
		internal const string CAPTION = "displayName";
		internal const string OWNERSHIP = "ownership";
		internal const string AUTO_INCREMENT = "autoIncrement";
		internal const string UNIQUE = "unique";
		internal const string BROWSABLE = "browsable";
        internal const string JUNCTION = "junction";
		internal const string KEY = "key";
		internal const string KEY_REF = "keyref";
		internal const string CASE_STYLE = "caseStyle";
		internal const string JOIN_MANAGER = "joinManager";
        internal const string OWNED_RELATIONSHIP = "ownedRelationship";
		internal const string REF_TYPE = "refType";
		internal const string REF_CLASS = "refClass";
		internal const string REF_ATTR = "refAttr";
		internal const string POSITION = "order";
		internal const string MULTILINE = "multiLine";
		internal const string LIST_HANDLER = "listHandler";
		internal const string MULTI_SELECTION = "multiSelect";
		internal const string INDEX = "index";
		internal const string COLUMN_NAME = "colName";
		internal const string TABLE_NAME = "tableName";
		internal const string SECTION = "section";
		internal const string CATEGORY = "category";
		internal const string LARGE_IMAGE = "largeImage";
		internal const string MEDIAN_IMAGE = "medianImage";
		internal const string SMALL_IMAGE = "smallImage";
		internal const string ENABLED = "enabled";
		internal const string TEXT = "text";
		internal const string ROWS = "rows";
		internal const string DIMENSION = "dimension";
		internal const string ELEMENT_TYPE = "elementType";
		internal const string COLUMN_COUNT = "columns";
		internal const string COLUMN_TITLES = "colTitles";
        internal const string SINGLE_ROW = "singleRow";
		internal const string ARRAY_SIZE = "size";
		internal const string USAGE = "usage";
        internal const string OPERATOR = "operator";
		internal const string ERROR_MESSAGE = "errorMsg";
        internal const string CODE = "code";
        internal const string CLASS_TYPE = "classtype";
        internal const string DISPLAY_MODE = "displayMode";
        internal const string SORT_ATTRIBUTE = "sortAttribute";
        internal const string SORT_DIRECTION = "sortDirection";
        internal const string VALUE_GENERATOR = "valueGenerator";
        internal const string LIST_PARAMETER = "parameter";
        internal const string LIST_CONDITIONAL_QUERY = "conditionalQuery";
        internal const string LIST_NONCONDITIONAL_QUERY = "nonConditionalQuery";
        internal const string LIST_TEXT = "textField";
        internal const string LIST_VALUE = "valueField";
        internal const string HEIGHT = "height";
        internal const string WIDTH = "width";
        internal const string THEIGHT = "tHeight";
        internal const string TWIDTH = "tWidth";
        internal const string INLINE_EDIT = "inlineEdit";
        internal const string MANUAL_UPDATE = "manualUpdate";
        internal const string CLASS_PAGE_URL = "classPageUrl";
        internal const string INSTANCE_PAGE_URL = "instancePageUrl";
        internal const string SHOW_AS_PROGRESS_BAR = "progressBar";
        internal const string SHOW_AS_HTML = "isHtml";
        internal const string SHOW_UPDATE_HISTORY = "updateHistory";
        internal const string SHOW_RELATED_CLASSES = "showRelatedCls";
        internal const string CUSTOM_PAGES = "customPages";
        internal const string URL = "url";
        internal const string QUERY_STRING = "queryStr";
        internal const string RELATED_CLASS = "relatedCls";
        internal const string INITIALIZATION = "initialize";
        internal const string BEFORE_UPDATE = "beforeUpdate";
        internal const string BEFORE_INSERT = "beforeInsert";
        internal const string CALLBACK_FUNCTION = "callBack";
        internal const string CASCADE_ATTRIBUTE = "cascadeAttribute";
        internal const string PARENT_ATTRIBUTE = "parentAttribute";
        internal const string LIST_STYLE = "listStyle";
        internal const string INPUT_MASK = "inputMask";
        internal const string DISPLAY_FORMAT = "displayFormat";
        internal const string KEYWORD_FORMAT = "keywordFormat";
        internal const string DATA_SOURCE = "dataSource";
        internal const string IS_ENCRYPTED = "encrypted";
        internal const string MATCH_CONDITION = "matchCondition";
        internal const string VISIBLE_CONDITION = "visibleCondition";
        internal const string INDEXED_TIME = "indexedTime";
        internal const string CONSTRAINT_USAGE = "constraintUsage";
        internal const string UI_CREATOR = "creator";
        internal const string INVOKE_CALLBACK = "invokeCallback";
        internal const string DATA_VIEW = "dataview";

		internal const string ADD = "add";
		internal const string DELETE = "del";
		internal const string MODIFY = "mod";

		internal const string FULL_TEXT = "fullText";
		internal const string DATASTORE = "datastore";
		internal const string GOOD_FOR_FULL_TEXT = "forfullText";
        internal const string GOOD_FOR_SUGGESTER = "forSuggester";

        internal const string HISTORY_EDIT = "historyEdit";
        internal const string RICH_TEXT = "richText";

        internal const string FILTER = "filter";
		internal const string FILE_TYPE = "fileType";
		
		// for the name of "xsd:key"
		internal const string PRIMARY_KEY_SUFFIX = "PK";
		// For the name of "xsd:keyref"
		internal const string KEY_REF_SUFFIX = "FK";
		internal const string UNIQUE_SUFFIX = "UQ";

		// The attribute name for validation
		internal const string CLASS_NAME = "className";
		internal const string ALL_CLASSES = "allClasses";
		internal const string REF_ROOT_CLASS = "refRootClass";

		// definitions of case style
		internal const string CASE_INSENSITIVE = "caseInsensitive";
		internal const string CASE_SENSITIVE = "caseSensitive";
		internal const string CASE_UPPER = "caseUpper";
		internal const string CASE_LOWER = "caseLower";

		// Data store definitions
		internal const string DATASTORE_COLUMN = "column";
		internal const string DATASTORE_FILE	= "file";

		internal const string URI = "http://www.newtera.com";

		// Assembly name for compiled transformation script
		internal const string TRANSFORMER_ASSEMBLY = "Newtera.Transformers.dll";
		internal const string TRANSFORMER_NAME_SPACE = "Newtera.Transformers";

        // Assembly name for compiled formula of virtual attributes
        public const string FORMULA_NAME_SPACE = "Newtera.Formula";

        // AppSettings definitions
        public const string BASE_URL = "BaseURL";
        public const string CURRENT_CULTURE = "CurrentCulture";
        public const string STATIC_FILES_ROOT = "StaticFilesRoot";
        public const string ATTACHMENTS_DIR_KEY = "AttachmentBasePath";
		public const string ATTACHMENTS_DIR = @"attachments\";
        public const string USER_ICON_DIR = @"styles\custom\icons\";
        public const string USER_IMAGES_DIR = @"styles\custom\images\";
        public const string USER_IMAGE_BASE_URL = @"styles/custom/";
        public const string FORM_TEMPLATES_DIR = @"Templates\Forms\";
        public const string REPORT_TEMPLATES_DIR = @"Templates\Reports\";
        public const string ATTACHMENTS_URL = @"~/attachments/";
        public const string USER_FILES_DIR_KEY = "UserFileBasePath";
        public const string USER_FILES_DIR = @"UserFiles";
        public const string SERVER_CONFIG_FILE = "EbaasServer.exe.config";
        // Elastic Search URL
        public const string ELASTIC_SEARCH_URL_KEY = "ElasticsearchURL";

        // import data related definition
        public const string FILE_ID = "FileId";
        public const string GROUP_ID = "GroupId";
        public const string CHANNEL_ID = "ChannelId";
        public const string RECORD_ID = "RecordId";
        public const string FILE_GROUP_RELATION = "fileGroups";
        public const string GROUP_CHANNEL_RELATION = "groupChannels";
        public const string CHANNEL_NAME = "Name";
        public const string CHANNEL_UNIT = "Unit";
        public const string CHANNEL_DATA = "Data";

        public const string HEADER_TABLE_NAME = "Header";
        public const string CHANNEL_TABLE_NAME = "Channel";
        public const string HEADER_NAME_ARRAY = "HeaderNames";
        public const string HEADER_VALUE_ARRAY = "HeaderValues";
        public const string CHANNEL_NAME_ARRAY = "ChannelNames";
        public const string CHANNEL_VALUE_ARRAY = "ChannelValues";

		/// <summary>
		/// Gets the base directory where the web application resides
		/// </summary>
		/// <returns>An absulute directory</returns>
		public static string GetAppHomeDir()
		{
            XmlRegistryKey rootKey;
            XmlRegistry theRegistry = XmlRegistryManager.Instance;

            rootKey = theRegistry.RootKey;

            //get the data from a specified item in that key    
            //Note that if the item "HOME_DIR" does not exist, then create the key
            XmlRegistryKey homeKey = rootKey.GetSubKey("HOME_DIR", true);

            String dir = (String)homeKey.GetStringValue();

            if (!string.IsNullOrEmpty(dir) && !dir.EndsWith(@"\"))
            {
                dir += @"\";
            }

            return dir;
		}

        public static string GetStaticFilesDir()
        {
            XmlRegistryKey rootKey;
            XmlRegistry theRegistry = XmlRegistryManager.Instance;

            rootKey = theRegistry.RootKey;

            //get the data from a specified item in that key    
            //Note that if the item "HOME_DIR" does not exist, then create the key
            XmlRegistryKey dirKey = rootKey.GetSubKey(STATIC_FILES_ROOT, true);

            String dir = (String)dirKey.GetStringValue();

            if (!string.IsNullOrEmpty(dir) && !dir.EndsWith(@"\"))
            {
                dir += @"\";
            }

            return dir;
        }

        /// <summary>
        /// Return the directory where the user-defined icons are located on the server side
        /// </summary>
        /// <returns>A directory string.</returns>
        /// <remarks> REMOVE</remarks>
        public static string GetUserIconDir()
        {
            // WinForm client
            string homeDir = GetAppHomeDir();
            if (homeDir.EndsWith(@"\"))
            {
                return homeDir + USER_ICON_DIR;
            }
            else
            {
                return homeDir + @"\" + USER_ICON_DIR;
            }
        }

        /// <summary>
        /// Return the directory where the user's images are located on the server side
        /// </summary>
        /// <returns>A directory string.</returns>
        public static string GetUserImageDir()
        {
            // WinForm client
            string homeDir = GetAppHomeDir();
            if (homeDir.EndsWith(@"\"))
            {
                return homeDir + USER_IMAGES_DIR;
            }
            else
            {
                return homeDir + @"\" + USER_IMAGES_DIR;
            }
        }

        /// <summary>
        /// Gets the base directory where the windows tools reside
        /// </summary>
        /// <returns>An absulute directory</returns>
        public static string GetAppToolDir()
        {
            XmlRegistryKey rootKey;
            XmlRegistry theRegistry = XmlRegistryManager.Instance;

            rootKey = theRegistry.RootKey;

            //get the data from a specified item in that key    
            //Note that if the item "TOOL_DIR" does not exist, then create the key
            XmlRegistryKey toolKey = rootKey.GetSubKey("TOOL_DIR", true);

            String dir = toolKey.GetStringValue();
            if (string.IsNullOrEmpty(dir))
            {
                dir = GetToolDirStr();
                toolKey.SetValue(dir);
            }

            if (!string.IsNullOrEmpty(dir) && !dir.EndsWith(@"\"))
            {
                dir += @"\";
            }

            return dir;
        }

		/// <summary>
		/// Get an unique checksum for the current computer
		/// </summary>
		public static string ComputerCheckSum
		{
			get
			{
				if (NewteraNameSpace.COMPUTER_ID == null)
				{
                    string uniqueMachineId = null;

                    // if the GetSerialNumber failed to get an unqiue number
                    // use the old way to get the machine id. The problem with the old
                    // way of getting a machine id is that it may change after the Operating
                    // system is reinstalled
                    if (string.IsNullOrEmpty(uniqueMachineId))
                    {
                        GetMachineInfo mInfo = new GetMachineInfo();

                        // get the processor id
                        string cpuId = mInfo.GetCPUId();

                        // Gets hard disk number;
                        string hardDiskSerial;
                        try
                        {
                            hardDiskSerial = mInfo.GetVolumeSerial("C");
                        }
                        catch (Exception)
                        {
                            hardDiskSerial = "";
                        }

                        uniqueMachineId = cpuId + hardDiskSerial;
                    }

                    if (!string.IsNullOrEmpty(uniqueMachineId))
                    {
                        int hash = uniqueMachineId.GetHashCode();
                        hash = Math.Abs(hash % 100000);
                        NewteraNameSpace.COMPUTER_ID = hash.ToString();
                    }
                    else
                    {
                        // get it from the registry
                        XmlRegistryKey rootKey;
                        XmlRegistry theRegistry = XmlRegistryManager.Instance;

                        rootKey = theRegistry.RootKey;

                        //get the data from a specified item in that key    
                        //Note that if the item "MACHINE_ID" does not exist, then create the key
                        XmlRegistryKey idKey = rootKey.GetSubKey("MACHINE_ID", true);
                        int hash = 0;
                        uniqueMachineId = idKey.GetStringValue();
                        if (string.IsNullOrEmpty(uniqueMachineId))
                        {
                            uniqueMachineId = "id" + Guid.NewGuid().ToString(); // generate an unique id
                            hash = uniqueMachineId.GetHashCode();
                            hash = Math.Abs(hash % 100000);
                            uniqueMachineId = hash.ToString();

                            idKey.SetValue(uniqueMachineId); // save it to registry
                        } 

                        NewteraNameSpace.COMPUTER_ID = uniqueMachineId;
                    }
				}

                return NewteraNameSpace.COMPUTER_ID;
			}
		}

        /// <summary>
        /// Get the directory for storing attachment files
        /// </summary>
        public static string GetAttachmentDir() 
		{
			string dir = ConfigurationManager.AppSettings[NewteraNameSpace.ATTACHMENTS_DIR_KEY];

			if (dir != null && dir.Trim().Length > 0)
			{
				if (!dir.EndsWith(@"\"))
				{
					dir += @"\";
				}
			}
			else
			{
                // use default attachment directory
                dir = NewteraNameSpace.GetAppHomeDir() + NewteraNameSpace.ATTACHMENTS_DIR;
			}

			return dir;
		}

        /// <summary>
        /// Get the directory for storing user files
        /// </summary>
        public static string GetUserFilesDir()
        {
            string dir = ConfigurationManager.AppSettings[NewteraNameSpace.USER_FILES_DIR_KEY];

            if (dir != null && dir.Trim().Length > 0)
            {
                if (!dir.EndsWith(@"\"))
                {
                    dir += @"\";
                }
            }
            else
            {
                dir = NewteraNameSpace.GetAppHomeDir() + NewteraNameSpace.USER_FILES_DIR + @"\";
            }

            return dir;
        }

        /// <summary>
        /// Get the sub-directory for storing attachment files
        /// </summary>
        /// <param name="createTime"></param>
        /// <returns>The subdir for attachments</returns>
        public static string GetAttachmentSubDir(DateTime createTime)
        {
            string baseDir = GetAttachmentDir();

            string subDir = baseDir + createTime.Year.ToString() + "-" + createTime.Month.ToString() + @"\";

            if (!Directory.Exists(subDir))
            {
                // create the subdir
                Directory.CreateDirectory(subDir);
            }

            return subDir;
        }

        public static string GetReportTemplateBaseDir(string schemaId)
        {
            // for web client
            string rootDir = NewteraNameSpace.GetAppHomeDir();
            if (rootDir.EndsWith(@"\"))
            {
                return rootDir + REPORT_TEMPLATES_DIR + schemaId + @"\";
            }
            else
            {
                return rootDir + @"\" + REPORT_TEMPLATES_DIR + schemaId + @"\";
            }
        }

        public static string GetReportTemplateDir(string schemaId, string baseClassName)
        {
            // for web client
            string rootDir = NewteraNameSpace.GetAppHomeDir();
            if (rootDir.EndsWith(@"\"))
            {
                return rootDir + REPORT_TEMPLATES_DIR + schemaId + @"\" + baseClassName + @"\";
            }
            else
            {
                return rootDir + @"\" + REPORT_TEMPLATES_DIR + schemaId + @"\" + baseClassName + @"\";
            }
        }

        public static string GetFormTemplateBaseDir(string schemaId)
        {
            // for web client
            string rootDir = NewteraNameSpace.GetAppHomeDir();
            if (rootDir.EndsWith(@"\"))
            {
                return rootDir + FORM_TEMPLATES_DIR + schemaId + @"\";
            }
            else
            {
                return rootDir + @"\" + FORM_TEMPLATES_DIR + schemaId + @"\";
            }
        }

        public static string GetFormTemplateDir(string schemaId, string baseClassName)
        {
            // for web client
            string rootDir = NewteraNameSpace.GetAppHomeDir();
            if (rootDir.EndsWith(@"\"))
            {
                return rootDir + FORM_TEMPLATES_DIR + schemaId + @"\" + baseClassName + @"\";
            }
            else
            {
                return rootDir + @"\" + FORM_TEMPLATES_DIR + schemaId + @"\" + baseClassName + @"\";
            }
        }

        /// <summary>
        /// Get the information indicating whether the attachment direcotyr is at the default location
        /// </summary>
        public static bool IsDefaultAttachmentDir()
        {
            bool status = true;

            string dir = ConfigurationManager.AppSettings[NewteraNameSpace.ATTACHMENTS_DIR_KEY];

            if (dir != null && dir.Trim().Length > 0)
            {
                status = false;
            }
           
            return status;
        }

        // get an unique image id
        public static string GetImageId(string className, string attributeName, string objId, string suffix)
        {
            return className + "-" + attributeName + "-" + objId + "." + suffix;
        }

        // Get the image info from the image id which has a pattern of class_attribute_instanceId.(gif, jpg, bmp, or png)
        public static void GetImageInfo(string imageId, out string objId, out string attributeName, out string className, out string suffix)
        {
            objId = null;
            attributeName = null;
            className = null;
            suffix = null;
            if (!string.IsNullOrEmpty(imageId))
            {
                int pos = imageId.IndexOf('-');
                if (pos > 0)
                {
                    className = imageId.Substring(0, pos);
                    imageId = imageId.Substring(pos + 1);
                }
                else
                {
                    return;
                }

                pos = imageId.IndexOf('-');
                if (pos > 0)
                {
                    attributeName = imageId.Substring(0, pos);
                    imageId = imageId.Substring(pos + 1);
                }
                else
                {
                    return;
                }

                pos = imageId.IndexOf('.');
                if (pos > 0)
                {
                    objId = imageId.Substring(0, pos);
                    suffix = imageId.Substring(pos + 1);
                }
            }
        }

        /// <summary>
        /// Gets the information indicating whether the current version is greater than the version
        /// provided
        /// </summary>
        /// <param name="version">The provided</param>
        /// <returns>true if the current version is greater, false otherwise.</returns>
        public static bool IsVersionGreaterThan(string version)
        {
            bool status = false;

            try
            {
                // only compare the first two digits in the version string which is in form
                // of x.x.x where x is a numnber
                string[] currentVersionNumbers = RELEASE_VERSION.Split('.');
                string[] comparingVersionNumbers = version.Split('.');

                int num1, num2;
                num1 = Int32.Parse(currentVersionNumbers[0]);
                num2 = Int32.Parse(comparingVersionNumbers[0]);
                if (num1 > num2)
                {
                    status = true;
                }
                else if (num1 == num2)
                {
                    // compare the second digits
                    num1 = Int32.Parse(currentVersionNumbers[1]);
                    num2 = Int32.Parse(comparingVersionNumbers[1]);
                    if (num1 > num2)
                    {
                        status = true;
                    }
                }
            }
            catch (Exception)
            {
                status = false;
            }

            return status;
        }

        /// <summary>
        /// Get the server base uri from the config file
        /// </summary>
        public static string ServerBaseUri()
        {
            if (string.IsNullOrEmpty(SERVER_BASE_URL))
            {
                string uri = ConfigurationManager.AppSettings[NewteraNameSpace.BASE_URL];

                if (!string.IsNullOrEmpty(uri))
                {
                    SERVER_BASE_URL = uri;
                }
                else
                {
                    // check the server config file
                    string rootDir = NewteraNameSpace.GetAppHomeDir();
                    string configFile;
                    if (rootDir.EndsWith(@"\"))
                    {
                        configFile = rootDir + @"bin\" + SERVER_CONFIG_FILE;
                    }
                    else
                    {
                        configFile = rootDir + @"\bin\" + SERVER_CONFIG_FILE;
                    }

                    if (File.Exists(configFile))
                    {
                        Newtera.Common.Config.AppConfig appConfig = new Config.AppConfig(configFile);
                        uri = appConfig.GetAppSetting(NewteraNameSpace.BASE_URL);

                        if (!string.IsNullOrEmpty(uri))
                        {
                            SERVER_BASE_URL = uri;
                        }
                    }
                }

                if (string.IsNullOrEmpty(SERVER_BASE_URL))
                {
                    SERVER_BASE_URL = "http://localhost:8080"; // default
                }
            }

            return SERVER_BASE_URL;
        }

        public static string GetServerRootDir()
        {
            var path = System.Web.Hosting.HostingEnvironment.MapPath("~");
            if (path == null)
            {
                var uriPath = Path.GetDirectoryName(System.Reflection.Assembly.GetExecutingAssembly().GetName().CodeBase);
                path = new Uri(uriPath).LocalPath;

                // remove \bin if exist
                int pos = path.LastIndexOf(@"\bin");
                if (pos > 0)
                {
                    path = path.Substring(0, pos);
                }

                if (!path.EndsWith(@"\"))
                {
                    path += @"\";
                }
            }

            return path;
        }

        private static string GetToolDirStr()
        {
            string dir = GetServerRootDir();

            if (Directory.Exists(@"C:\Newtera\Ebaas\Repos"))
            {
                // Hack, it is running under development environment, set a special dir
                dir = @"C:\Newtera\Ebaas\Repos\Studio";
            }

            return dir;
        }
    }
}