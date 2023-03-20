/*
* @(#)GlobalSettings.cs
*
* Copyright (c) 2003 Newtera, Inc. All rights reserved.
*
*/
namespace Newtera.Common.MetaData
{
	using System;
	using System.Resources;
	using System.Collections;
	using System.Collections.Specialized;
	using System.Threading;
	using System.Reflection;
	using System.Reflection.Emit;
    using System.Web;

	using Newtera.Common.Core;

	/// <summary>
	/// A singleton class that global settings
	/// </summary>
	/// <version>  	1.0.0 28 Nov 2011 </version>
	public class GlobalSettings
	{
		/// <summary>
		/// Singleton's private instance.
		/// </summary>
		private static GlobalSettings theInstance;

        private bool _isWindowClient;
		
		static GlobalSettings()
		{
            theInstance = new GlobalSettings();
		}

		/// <summary>
		/// The private constructor.
		/// </summary>
		private GlobalSettings()
		{
            _isWindowClient = false;
		}

		/// <summary>
		/// Gets the GlobalSettings instance.
		/// </summary>
		/// <returns> The GlobalSettings instance.</returns>
		static public GlobalSettings Instance
		{
			get
			{
                return theInstance;
			}
		}

        /// <summary>
        /// gets information indicating whether the host is a window client
        /// </summary>
        /// <returns>True if the client is a window client, false otherwise</returns>
        public bool IsWindowClient
        {
            get
            {
                return _isWindowClient;
            }
            set
            {
                _isWindowClient = value;
            }
        }
	}
}