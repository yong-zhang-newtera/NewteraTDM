/*
* @(#)ImportPackageListHandler.cs
*
* Copyright (c) 2014 Newtera, Inc. All rights reserved.
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
    using Newtera.Common.MetaData.Mappings;

	/// <summary>
	/// Represents an handler that retrieve a list of predefined import packages from meta data model
	/// </summary>
	/// <version> 1.0.0 11 Feb 2014 </version>
	public class ImportPackageListHandler : IListHandler
	{
		/// <summary>
        /// Get a list of import packages defined for a schema.
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
                if (context != null)
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
                        MappingManager mappingManager = ((MetaDataModel)current).MappingManager;

                        MappingPackageCollection packages = mappingManager.GetMappingPackages();

                        EnumValue enumValue;
                        foreach (MappingPackage package in packages)
                        {
                            enumValue = new EnumValue();
                            enumValue.Value = package.Name;
                            enumValue.DisplayText = package.Name;
                            values.Add(enumValue);
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