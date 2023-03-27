/*
* @(#)ActionCodeRunner.cs
*
* Copyright (c) 2008 Newtera, Inc. All rights reserved.
*
*/
namespace Newtera.Server.Engine.Workflow
{
	using System;
	using System.Xml;
	using System.IO;
	using System.Text;
	using System.CodeDom;
    using System.Threading;
	using System.CodeDom.Compiler;
	using Microsoft.CSharp;
    using System.Reflection;
    using System.Collections;
	using Newtera.Common.Core;
    using Newtera.Server.DB;
    using Newtera.WFModel;
    using Newtera.Common.MetaData.DataView;
    using Newtera.Common.Wrapper;
    using Newtera.Common.MetaData.Principal;
    using Newtera.Server.UsrMgr;

	/// <summary>
	/// A singleton class that compile and execute the action code specified for workflow task.
	/// </summary>
	/// <version>1.0.0 28 Aug 2008 </version>
	public class ActionCodeRunner
	{
        private const string ActionCodeTemplate = @"
            namespace Newtera.WFTaskCustomActions
            {
                using System;
	            using System.Xml;
                using Newtera.Common.Core;
                using Newtera.Common.Wrapper;
                using Newtera.Common.MetaData.DataView;
                using Newtera.Server.Engine.Workflow;
                
                public class {CLASSNAME} : ActionCodeExecutorBase
                {
                    public override void Execute()
                    {
                        {CODE}
                    }   
                }
            }";

		private CodeDomProvider _csharpProvider = null;
        private Hashtable _assemblyTable;

		/// <summary>
		/// Singleton's private instance.
		/// </summary>
		private static ActionCodeRunner theActionCodeRunner;
		
		static ActionCodeRunner()
		{
			theActionCodeRunner = new ActionCodeRunner();
		}

		/// <summary>
		/// The private constructor.
		/// </summary>
		private ActionCodeRunner()
		{
            _assemblyTable = new Hashtable();
		}

		/// <summary>
		/// Gets the ActionCodeRunner instance.
		/// </summary>
		/// <returns> The ActionCodeRunner instance.</returns>
		static public ActionCodeRunner Instance
		{
			get
			{
				return theActionCodeRunner;
			}
		}

        /// <summary>
        /// Complete the action code by inserting the code into a template and compile
        /// the complete action code.
        /// </summary>
        /// <param name="actionCode">The partial action code</param>
        /// <param name="schemaId">The schema id indicates the schema where the instance class resides</param>
        /// <param name="instanceClassName">The class name of the instance to which the action code is run against</param>
        /// <returns>The error message if there is compilation error, null if the compilation succeeded.</returns>
        public string CompileActionCode(string actionCode, string schemaId, string instanceClassName)
        {
            string errorMsg = null;
            StringBuilder buffer = new StringBuilder();
            // create an unique class name
            string actionId = "CA" + Guid.NewGuid().ToString("N");
            string typeName = "Newtera.WFTaskCustomActions." + actionId;

            // compile the code into an assembly in memory
            string fullPath = System.Reflection.Assembly.GetAssembly(typeof(ActionCodeRunner)).Location;
            string libPath = Path.GetDirectoryName(fullPath);
            // the assembly isn't generated here, therefore, we can use a single name for the class
            string completeCode = GetCompleteCode(actionId, actionCode);
            CompilerResults cr = CompileFromSource(completeCode, libPath);

            if (cr.Errors.Count > 0)
            {
                foreach (CompilerError compilerError in cr.Errors)
                {
                    buffer.Append(compilerError.ErrorText).Append(Environment.NewLine);
                }
            }

            if (buffer.Length > 0)
            {
                errorMsg = buffer.ToString();
            }
            else
            {
                // test run to check the run time errors
                Assembly assembly = cr.CompiledAssembly;
                if (assembly != null)
                {
                    // execute the code
                    IActionCodeExecutor codeExecutor = (IActionCodeExecutor)assembly.CreateInstance(typeName);
                    // set the context
                    codeExecutor.Instance = new ActionCodeTester(schemaId, instanceClassName);
                    try
                    {
                        codeExecutor.Execute();
                    }
                    catch (Exception ex)
                    {
                        errorMsg = ex.Message + "\n" + ex.StackTrace;
                    }
                }
            }

            return errorMsg;
        }

        /// <summary>
        /// Execute the action code provided
        /// </summary>
        /// <param name="actionId">The unique action id</param>
        /// <param name="actionCode">The partial action code</param>
        /// <param name="instanceWrapper">The wrapper of an data instance as execution context</param>
        public void ExecuteActionCode(string actionId, string actionCode, IInstanceWrapper instanceWrapper)
        {
            ExecuteActionCode("Unknown", actionId, actionCode, instanceWrapper, null);
        }

        /// <summary>
        /// Execute the action code provided
        /// </summary>
        /// <param name="actionId">The unique action id</param>
        /// <param name="actionCode">The unique action id</param>
        /// <param name="instanceWrapper">The wrapper of an data instance as execution context</param>
        /// <param name="propertyName">The property that cause to execute the action, for example, callback function</param>
        public void ExecuteActionCode(string actionId, string actionCode, IInstanceWrapper instanceWrapper, string propertyName)
        {
            ExecuteActionCode("Unknown", actionId, actionCode, instanceWrapper, propertyName);
        }

        /// <summary>
        /// Execute the action code provided
        /// </summary>
        /// <param name="client">The client that executes the action code</param>
        /// <param name="actionId">The unique action id</param>
        /// <param name="actionCode">The partial action code</param>
        /// <param name="instanceWrapper">The wrapper of an data instance as execution context</param>
        public void ExecuteActionCode(string client, string actionId, string actionCode, IInstanceWrapper instanceWrapper)
        {
            ExecuteActionCode(client, actionId, actionCode, instanceWrapper, null);
        }

        /// <summary>
        /// Execute the action code provided
        /// </summary>
        /// <param name="client">The client that executes the action code</param>
        /// <param name="actionId">The unique action id</param>
        /// <param name="actionCode">The partial action code</param>
        /// <param name="instanceWrapper">The wrapper of an data instance as execution context</param>
        /// <param name="propertyName">The property that cause to execute the action, for example, callback function</param>
        public void ExecuteActionCode(string client, string actionId, string actionCode, IInstanceWrapper instanceWrapper, string propertyName)
        {
            Assembly assembly = null;
            string typeName = "Newtera.WFTaskCustomActions." + actionId;

            lock (this)
            {
                // the assembly for the action code may have been created
                assembly = (Assembly)_assemblyTable[typeName];

                if (assembly == null)
                {
                    // compile the code into an assembly in memory
                    string libPath = NewteraNameSpace.GetAppHomeDir() + @"bin\";
                    // use the unique action id as the class name
                    string completeCode = GetCompleteCode(actionId, actionCode);
                    CompilerResults cr = CompileFromSource(completeCode, libPath);

                    if (cr.Errors.Count > 0)
                    {
                        throw new Exception("Errors accure while compiling action code");
                    }
                    else
                    {
                        assembly = cr.CompiledAssembly;
                        _assemblyTable.Add(typeName, assembly);
                    }
                }
            }

            if (assembly != null)
            {
                try
                {
                    // execute the code
                    IActionCodeExecutor codeExecutor = (IActionCodeExecutor)assembly.CreateInstance(typeName);
                    // set the context
                    codeExecutor.Instance = instanceWrapper;
                    codeExecutor.Client = client;
                    codeExecutor.Property = propertyName;
                    codeExecutor.Execute();
                }
                catch (Exception ex)
                {
                    throw ex;
                }
            }
        }

        /// <summary>
        /// Clear the assemblies cached
        /// </summary>
        public void ClearAssemblies()
        {
            lock (this)
            {
                _assemblyTable.Clear();
            }
        }

        /// <summary>
        /// Get a complete action code by inserting the partial code to a template
        /// </summary>
        /// <param name="partialCode">The partial code</param>
        /// <returns>The complete code ready to compile</returns>
        private string GetCompleteCode(string className, string partialCode)
        {
            string completeCode = ActionCodeTemplate.Replace("{CODE}", partialCode);
            completeCode = completeCode.Replace("{CLASSNAME}", className);

            return completeCode;
        }
		
		/// <summary>
		/// Compile action code
		/// </summary>
        /// <param name="completeActionCode">The complete action code in C#</param>
		/// <param name="libPath">The lib path where to find refrenced assemblies</param>
		/// <returns></returns>
		private CompilerResults CompileFromSource(string completeActionCode, string libPath)
		{
            CodeDomProvider compiler = this.GetProvider();

			//ICodeCompiler compiler = provider.CreateCompiler();

			// Configure a CompilerParameters that links System.dll and 
			// produces a file name based on the specified source file name.
			CompilerParameters cp = new CompilerParameters();
			
			// Indicate that a dll rather than an executable should be generated.
			cp.GenerateExecutable = false;

			// Sets filename of the assembly file to generate.
			//cp.OutputAssembly = NewteraNameSpace.TRANSFORMER_ASSEMBLY;

			// Adds an assembly reference.
			cp.ReferencedAssemblies.Add("System.dll");
			cp.ReferencedAssemblies.Add("System.XML.dll");
			cp.ReferencedAssemblies.Add("System.Data.dll");
			if (libPath.EndsWith(@"\"))
			{
				cp.ReferencedAssemblies.Add(libPath + "Newtera.Common.dll");
                cp.ReferencedAssemblies.Add(libPath + "Newtera.Server.dll");
			}
			else
			{
				cp.ReferencedAssemblies.Add(libPath + @"\Newtera.Common.dll");
                cp.ReferencedAssemblies.Add(libPath + @"\Newtera.Server.dll");
			}

			cp.CompilerOptions = "/t:library ";

			// Sets whether to generate the assembly in memory.
			cp.GenerateInMemory = true;

			// Sets the warning level at which 
			// the compiler should abort compilation
			// if a warning of this level occurrs.
			cp.WarningLevel = 3;

			// Sets whether to treat all warnings as errors.
			cp.TreatWarningsAsErrors = false;

			// Invoke compilation.
            CompilerResults cr = compiler.CompileAssemblyFromSource(cp, completeActionCode);

			// Return the results of compilation.
			return cr;
		}

		/// <summary>
		/// Gets the provider for the C#
		/// </summary>
		/// <returns>CodeDomProvider instance</returns>
		public CodeDomProvider GetProvider()
		{
            if (_csharpProvider == null)
            {
                _csharpProvider = new CSharpCodeProvider();
            }

            return _csharpProvider;
		}
	}

    public abstract class ActionCodeExecutorBase : IActionCodeExecutor
    {
        protected IInstanceWrapper _instance = null;
        protected string _client = null;
        protected string _property = null;

        /// <summary>
        /// Gets the connection string to the current database
        /// </summary>
        public string CurrentConnectionString
        {
            get
            {
                CustomPrincipal customPrincipal = Thread.CurrentPrincipal as CustomPrincipal;
                if (customPrincipal != null &&
                    customPrincipal.IsServerSide)
                {
                    // The client is a web application or web service

                    return customPrincipal.GetUserDataString(NewteraNameSpace.CURRENT_CONNECTION);
                }
                else
                {
                    return null;
                }
            }
        }

        /// <summary>
        /// Gets the login id of the current user
        /// </summary>
        public string CurrentLoginID
        {
            get
            {
                string loginId = null;

                // do not return a user id for test run
                if (_instance != null && !(_instance is ActionCodeTester))
                {
                    CustomPrincipal principal = null;

                    principal = (CustomPrincipal)Thread.CurrentPrincipal;
                    loginId = principal.Identity.Name;
                }

                return loginId;
            }
        }

        /// <summary>
        /// Gets the display name of the current user
        /// </summary>
        public string CurrentUser
        {
            get
            {
                string userName = null;

                // do not return a user name for test run
                if (_instance != null && !(_instance is ActionCodeTester))
                {
                    CustomPrincipal principal = null;

                    principal = (CustomPrincipal)Thread.CurrentPrincipal;
                    if (principal != null)
                    {
                        userName = principal.DisplayText;
                    }
                }

                return userName;
            }
        }

        /// <summary>
        /// Gets roles of the current user
        /// </summary>
        public string[] CurrentRoles
        {
            get
            {
                string[] roles = new String[0];

                // do not return a role array for test run
                if (_instance != null && !(_instance is ActionCodeTester))
                {
                    CustomPrincipal principal = null;

                    principal = (CustomPrincipal)Thread.CurrentPrincipal;
                    if (principal != null)
                    {
                        string userId = principal.Identity.Name;
                        if (!string.IsNullOrEmpty(userId))
                        {
                            IUserManager userManager = new ServerSideUserManager();
                            roles = userManager.GetRoles(userId);
                        }
                    }
                }

                return roles;
            }
        }

        /// <summary>
        /// Gets units of the current user
        /// </summary>
        public string[] CurrentUnits
        {
            get
            {
                string[] roles = new String[0];

                // do not return a role array for test run
                if (_instance != null && !(_instance is ActionCodeTester))
                {
                    CustomPrincipal principal = null;

                    principal = (CustomPrincipal)Thread.CurrentPrincipal;
                    if (principal != null)
                    {
                        string userId = principal.Identity.Name;
                        if (!string.IsNullOrEmpty(userId))
                        {
                            IUserManager userManager = new ServerSideUserManager();
                            roles = userManager.GetRoles(userId, "Unit");
                        }
                    }
                }

                return roles;
            }
        }

        /// <summary>
        /// Gets functions of the current user
        /// </summary>
        public string[] CurrentFunctions
        {
            get
            {
                string[] roles = new String[0];

                // do not return a role array for test run
                if (_instance != null && !(_instance is ActionCodeTester))
                {
                    CustomPrincipal principal = null;

                    principal = (CustomPrincipal)Thread.CurrentPrincipal;
                    if (principal != null)
                    {
                        string userId = principal.Identity.Name;
                        if (!string.IsNullOrEmpty(userId))
                        {
                            IUserManager userManager = new ServerSideUserManager();
                            roles = userManager.GetRoles(userId, "Function");
                        }
                    }
                }

                return roles;
            }
        }

        /// <summary>
        /// Gets roles string of the current user
        /// </summary>
        public string CurrentRoleString
        {
            get
            {
                StringBuilder builder = new StringBuilder();

                // do not return a role array for test run
                if (_instance != null && !(_instance is ActionCodeTester))
                {
                    CustomPrincipal principal = null;

                    principal = (CustomPrincipal)Thread.CurrentPrincipal;
                    if (principal != null)
                    {
                        string userId = principal.Identity.Name;
                        if (!string.IsNullOrEmpty(userId))
                        {
                            IUserManager userManager = new ServerSideUserManager();
                            string[] roles = userManager.GetRoles(userId);

                            if (roles != null)
                            {
                                foreach (string role in roles)
                                {
                                    if (builder.Length > 0)
                                    {
                                        builder.Append(";").Append(role);
                                    }
                                    else
                                    {
                                        builder.Append(role);
                                    }
                                }
                            }
                        }
                    }
                }

                return builder.ToString();
            }
        }

        /// <summary>
        /// Gets units string of the current user
        /// </summary>
        public string CurrentUnitString
        {
            get
            {
                StringBuilder builder = new StringBuilder();

                // do not return a role array for test run
                if (_instance != null && !(_instance is ActionCodeTester))
                {
                    CustomPrincipal principal = null;

                    principal = (CustomPrincipal)Thread.CurrentPrincipal;
                    if (principal != null)
                    {
                        string userId = principal.Identity.Name;
                        if (!string.IsNullOrEmpty(userId))
                        {
                            IUserManager userManager = new ServerSideUserManager();
                            string[] roles = userManager.GetRoles(userId, "Unit");

                            if (roles != null)
                            {
                                foreach (string role in roles)
                                {
                                    if (builder.Length > 0)
                                    {
                                        builder.Append(";").Append(role);
                                    }
                                    else
                                    {
                                        builder.Append(role);
                                    }
                                }
                            }
                        }
                    }
                }

                return builder.ToString();
            }
        }

        /// <summary>
        /// Gets functions string of the current user
        /// </summary>
        public string CurrentFunctionString
        {
            get
            {
                StringBuilder builder = new StringBuilder();

                // do not return a role array for test run
                if (_instance != null && !(_instance is ActionCodeTester))
                {
                    CustomPrincipal principal = null;

                    principal = (CustomPrincipal)Thread.CurrentPrincipal;
                    if (principal != null)
                    {
                        string userId = principal.Identity.Name;
                        if (!string.IsNullOrEmpty(userId))
                        {
                            IUserManager userManager = new ServerSideUserManager();
                            string[] roles = userManager.GetRoles(userId, "Function");

                            if (roles != null)
                            {
                                foreach (string role in roles)
                                {
                                    if (builder.Length > 0)
                                    {
                                        builder.Append(";").Append(role);
                                    }
                                    else
                                    {
                                        builder.Append(role);
                                    }
                                }
                            }
                        }
                    }
                }

                return builder.ToString();
            }
        }

        /// <summary>
        /// Gets data of a given parameter of the current user's data, such as email, telphone, address
        /// </summary>
        public string GetCurrentUserData(string parameterName)
        {
            string parameterValue = "";

            // do not return a role array for test run
            if (_instance != null && !(_instance is ActionCodeTester))
            {
                CustomPrincipal principal = null;

                principal = (CustomPrincipal)Thread.CurrentPrincipal;
                if (principal != null)
                {
                    string userId = principal.Identity.Name;
                    if (!string.IsNullOrEmpty(userId))
                    {
                        IUserManager userManager = new ServerSideUserManager();
                        parameterValue = userManager.GetUserData(userId, parameterName);
                    }
                }
            }

            return parameterValue;
        }

        /// <summary>
        /// Gets the current date
        /// </summary>
        public string CurrentDate
        {
            get
            {
                return DateTime.Now.ToShortDateString();
            }
        }

        /// <summary>
        /// Gets current time
        /// </summary>
        public string CurrentTime
        {
            get
            {
                return DateTime.Now.ToShortTimeString();
            }
        }

        /// <summary>
        /// Gets current date time
        /// </summary>
        public string CurrentDateTime
        {
            get
            {
                return DateTime.Now.ToString("s");
            }
        }

        /// <summary>
        /// Gets a parameter's value from the user's session
        /// </summary>
        public string GetSessionParameterValue(string parameterName)
        {
            CustomPrincipal customPrincipal = Thread.CurrentPrincipal as CustomPrincipal;

            if (customPrincipal != null &&
                customPrincipal.IsServerSide)
            {
                // The client is a web application or web service

                return customPrincipal.GetUserDataString(parameterName);
            }
            else
            {
                return null;
            }
        }

        /// <summary>
        /// Gets a value of an attribute from the database.
        /// </summary>
        public string GetAttributeStringValueFromDB(IInstanceWrapper instance, string attributeName)
        {
            string attributeValue = "";

            if (instance.WrappedInstance != null &&
                instance.WrappedInstance.InstanceData != null &&
                !string.IsNullOrEmpty(instance.WrappedInstance.InstanceData.PrimaryKeyValues))
            {
                try
                {
                    NewteraDataService dataService = new NewteraDataService();

                    InstanceView instanceView = dataService.GetInstanceView(instance.SchemaId, instance.OwnerClassName, instance.WrappedInstance.InstanceData.PrimaryKeyValues);

                    if (instanceView != null)
                    {
                        attributeValue = instanceView.InstanceData.GetAttributeStringValue(attributeName);
                    }
                }
                catch (Exception ex)
                {
                    // non-critical call, ignore the error
                    ErrorLog.Instance.WriteLine("Failed in calling GetAttributeStringValueFromDB with error " + ex.Message);
                }
            }

            return attributeValue;
        }

        /// <summary>
        /// Gets the current state of the workflow instance that is bound to the given instance.
        /// </summary>
        public string GetCurrentWorkflowStateName(IInstanceWrapper instance)
        {
            string stateName = "";

            // get the binding workflow instance info if there exists one
            WorkflowModelAdapter adapter = new WorkflowModelAdapter();
            WorkflowInstanceBindingInfo bindingInfo = adapter.GetBindingInfoByObjId(instance.ObjId);

            if (bindingInfo != null)
            {
                // Get binding workflow tracking state infos
                NewteraTrackingWorkflowInstanceCollection trackingWorkflowInstances = adapter.GetTrackingWorkflowInstancesByWorkflowInstanceId(bindingInfo.WorkflowInstanceId);
                Guid workflowInstanceId = new Guid(bindingInfo.WorkflowInstanceId);

                foreach (NewteraTrackingWorkflowInstance trackingWorkflowInstance in trackingWorkflowInstances)
                {
                    foreach (NewteraActivityTrackingRecord activityRecord in trackingWorkflowInstance.ActivityEvents)
                    {
                        // make sure the activity is a StateActivity and is in Executing Status
                        if (activityRecord.ExecutionStatus == System.Workflow.ComponentModel.ActivityExecutionStatus.Executing &&
                             activityRecord.TypeName == "StateActivity")
                        {
                            stateName = activityRecord.QualifiedName;
                            break;
                        }
                    }
                }
            }

            return stateName;
        }

        /// <summary>
        /// Gets the sequence number generator
        /// </summary>
        /// <param name="className">A name of the class where the sequence number is stored</param>
        /// <param name="keyName">A name of the column which contains a key identifying the instance where the sequence number is stored</param>
        /// <param name="keyValue">The key value which identifies the instance that contain the sequence number</param>
        /// <param name="sequenceNumberColumn">A name of the column which stores the sequence number</param>
        /// <returns>A SequenceNumberGenerator instance.</returns>
        public SequenceNumberGenerator GetSequenceNumberGenerator(string className, string keyName, string keyValue, string sequenceNumberColumn)
        {
            return SequenceNumberGeneratorFactory.Instance.Create(className, keyName, keyValue, sequenceNumberColumn);
        }

        #region IActionCodeExecutor
        public IInstanceWrapper Instance
        {
            get
            {
                return _instance;
            }
            set
            {
                _instance = value;
            }
        }

        public string Client
        {
            get
            {
                return _client;
            }
            set
            {
                _client = value;
            }
        }

        /// <summary>
        /// Gets or sets the name of the propety that invokes the callback function
        /// </summary>
        public string Property
        {
            get
            {
                return _property;
            }
            set
            {
                _property = value;
            }
        }

        public abstract void Execute();

        #endregion
    }
}