/*
* @(#) DataInstanceGetterFactory.cs
*
* Copyright (c) 2014 Newtera, Inc. All rights reserved.
*
*/
namespace Newtera.Server.Engine.Workflow
{
	using System;
	using System.Collections;
	using System.Data;
    using System.Text.RegularExpressions;
    using System.Runtime.Remoting;

	using Newtera.Common.Core;

	/// <summary>
    /// A factory for all DataInstanceGetter instance.
	/// </summary>
	/// <version> 	1.0.0	16 June 2014 </version>
	public class DataInstanceGetterFactory
	{
		// Static factory object, all invokers will use this factory object.
		private static DataInstanceGetterFactory theFactory;

        private static Hashtable _getters;

		/// <summary>
		/// Private constructor.
		/// </summary>
		private DataInstanceGetterFactory()
		{
            _getters = new Hashtable();
		}

		/// <summary>
		/// Gets the DataInstanceGetterFactory instance.
		/// </summary>
		/// <returns> The DataInstanceGetterFactory instance.</returns>
		static public DataInstanceGetterFactory Instance
		{
			get
			{
				return theFactory;
			}
		}

        /// <summary>
        /// Creates a default DataInstanceGetter.
        /// </summary>
        /// <returns>A IDataInstanceGetter instance.</returns>
        public IDataInstanceGetter Create()
        {
            IDataInstanceGetter getter = new DBInstanceGetter();

            return getter;
        }

        /// <summary>
        /// Creates a specific DataInstanceGetter.
		/// </summary>
        /// <param name="gettterLibName">an example of getter lib name is: Newtera.WebControls.WebServiceInstanceGetter, Newtera.WebControls</param>
        /// <returns>A IDataInstanceGetter instance.</returns>
        public IDataInstanceGetter Create(string gettterLibName)
        {
            IDataInstanceGetter getter = null;

            if (gettterLibName != null)
            {
                int index = gettterLibName.IndexOf(",");
                string assemblyName = null;
                string className;

                if (index > 0)
                {
                    className = gettterLibName.Substring(0, index).Trim();
                    assemblyName = gettterLibName.Substring(index + 1).Trim();
                }
                else
                {
                    className = gettterLibName.Trim();
                }

                try
                {

                    ObjectHandle obj = Activator.CreateInstance(assemblyName, className);
                    getter = (IDataInstanceGetter)obj.Unwrap();
                }
                catch
                {
                    getter = null;
                }
            }

            return getter;
        }

		static DataInstanceGetterFactory()
		{
			// Initializing the factory.
			{
				theFactory = new DataInstanceGetterFactory();
			}
		}
	}
}