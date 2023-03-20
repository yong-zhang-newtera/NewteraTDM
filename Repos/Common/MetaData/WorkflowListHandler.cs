/*
* @(#)WorkflowListHandler.cs
*
* Copyright (c) 2012 Newtera, Inc. All rights reserved.
*
*/
namespace Newtera.Common.MetaData
{
	using System;
    using System.IO;
    using System.Threading;
	using System.Collections.Specialized;

    using Newtera.Common.Core;
	using Newtera.Common.MetaData.Schema;
	using Newtera.Common.MetaData.DataView;
	using Newtera.Common.MetaData.XaclModel;
    using Newtera.Common.MetaData.Principal;

	/// <summary>
	/// Represents an handler that retrieve a list of worklfow defined for a class.
	/// </summary>
	/// <version> 1.0.0 25 Sept 2012 </version>
	public class WorkflowListHandler : IListHandler
	{
		/// <summary>
        /// Get a list of workflows for a class.
		/// </summary>
		/// <param name="context">The SchemaModelElement object constraint by the list values</param>
        /// <param name="query">A query that returns the result, can be null</param>
        /// <param name="parameter">The parameter value defined for the handler, could be null</param>
        /// <param name="filterValue">Any filter that is used to get a list, can be null</param>
        /// <param name="textField">The field that is used to get enum text, can be null</param>
        /// <param name="valueField">The field that is used to get enum value, can be null</param>
        /// <returns>A collection of EnumValue object</returns>
        public EnumValueCollection GetValues(SchemaModelElement context, string query, string parameter, string filterValue, string textField, string valueField)
		{
			EnumValueCollection values = new EnumValueCollection();

            // get the CustomPrincipal from the thread
            CustomPrincipal principal = Thread.CurrentPrincipal as CustomPrincipal;

            if (principal != null)
            {
                IServerProxy serverProxy = principal.ServerProxy;

                if (context != null)
                {
                    // get project id and class name
                    if (!string.IsNullOrEmpty(parameter))
                    {
                        // parameter format: projectName=XXX&projectVersion=1.0&className=YYY
                        string[] pairs = parameter.Split('&');
                        int pos;
                        string name, val;
                        string projectName = null;
                        string projectVersion = null;
                        string className = null;
                        foreach (string pair in pairs)
                        {
                            pos = pair.IndexOf("=");
                            name = pair.Substring(0, pos);
                            val = pair.Substring(pos + 1);

                            if (name.ToUpper() == "PROJECTNAME")
                            {
                                projectName = val;
                            }
                            else if (name.ToUpper() == "PROJECTVERSION")
                            {
                                projectVersion = val;
                            }
                            else if (name.ToUpper() == "CLASSNAME")
                            {
                                className = val;
                            }
                        }

                        // get the MetaDataModel object which is the root of
                        // the IXaclObject
                        IXaclObject current = (IXaclObject)context;
                        while (current.Parent != null)
                        {
                            current = current.Parent;
                        }

                        if (current is MetaDataModel)
                        {
                            string schemaId = ((MetaDataModel)current).SchemaInfo.NameAndVersion;

                            string[] workflowNames = serverProxy.GetWorkflowNames(projectName, projectVersion, schemaId, className);

                            EnumValue enumValue;
                            foreach (string workflowName in workflowNames)
                            {
                                enumValue = new EnumValue();
                                enumValue.Value = workflowName;
                                enumValue.DisplayText = workflowName;
                                values.Add(enumValue);
                            }
                        }
                    }
                }
            }

			return values;
		}

		/// <summary>
		/// Gets information indicating whether a given data view name is valid
		/// </summary>
		/// <param name="val">The given data view name</param>
		/// <param name="context">The SchemaModelElement object constraint by the list values</param>
		/// <returns>true if the value is valid, false, otherwise.</returns>
		public bool IsValueValid(string val, SchemaModelElement context)
		{
			return false;
		}
	}
}