/*
* @(#)MasterServerUrlResolver.cs
*
* Copyright (c) 2018 Newtera, Inc. All rights reserved.
*
*/
namespace Newtera.MasterServerClient
{
	using System;
	using System.IO;
	using System.Xml;
	using System.Web;
	using System.Threading;
	using System.Text;
	using System.Reflection;
	using System.Collections.Specialized;

	using Newtera.Common.Config;

	/// <summary> 
	/// A class that .
	/// </summary>
	/// <version> 1.0.0 4 Dec 2018 </version>
	public class MasterServerResolver
	{
        private string _serverKey;
        private string _serverUrl;

        /// <summary>
        /// Singleton's private instance.
        /// </summary>
        private static MasterServerResolver theInstance;

        static MasterServerResolver()
        {
            theInstance = new MasterServerResolver();
        }

        /// <summary>
        /// The private constructor.
        /// </summary>
        private MasterServerResolver()
        {
            this._serverKey = "";
            this._serverUrl = "";
        }

        /// <summary>
        /// Gets the MasterServerUrlResolver instance.
        /// </summary>
        /// <returns> The MasterServerUrlResolver instance.</returns>
        static public MasterServerResolver Instance
        {
            get
            {
                return theInstance;
            }
        }

        public string MasterServerUrl
        {
            get
            {
                return _serverUrl;
            }
        }
	}
}