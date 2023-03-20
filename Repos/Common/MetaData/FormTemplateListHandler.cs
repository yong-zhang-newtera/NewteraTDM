/*
* @(#)FormTemplateListHandler.cs
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
	/// Represents an handler that retrieve a list of form templates for a class.
	/// </summary>
	/// <version> 1.0.0 25 June 2012 </version>
	public class FormTemplateListHandler : IListHandler
	{
		/// <summary>
        /// Get a list of html templates for a class.
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
                    // get directory of the word templates
                    if (!string.IsNullOrEmpty(parameter))
                    {
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

                            string[] files = serverProxy.GetFormTemplatesFileNames(schemaId, parameter);

                            EnumValue enumValue;
                            foreach (string fileName in files)
                            {
                                FileInfo fileInfo = new FileInfo(fileName);
                                enumValue = new EnumValue();
                                enumValue.Value = fileInfo.Name.GetHashCode().ToString();
                                enumValue.DisplayText = fileInfo.Name;
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