/*
* @(#)MessageResourceManager.cs
*
* Copyright (c) 2006 Newtera, Inc. All rights reserved.
*
*/
namespace Newtera.DataGridActiveX
{
	using System;
	using System.Collections;
	using System.Threading;
	using System.Diagnostics;
	using System.Globalization;
	using System.Reflection;
	using System.Resources;

	/// <summary> 
	/// A utility class that cache the resources for messages displayed
	/// in the UI.
	/// </summary>
	/// <version> 1.0.0 25 Apr 2006 </version>
	/// <author> Yong Zhang </author>
	public sealed class MessageResourceManager
	{
		private static Hashtable _resourcesTable = new Hashtable();
		
		/// <summary>
		/// prevent the class from being instantiated.
		/// </summary>
		private MessageResourceManager()
		{
		}

		/// <summary>
		/// Loads the resources
		/// </summary>
		private static ResourceManager GetResources() 
		{
			string resourcesName = Thread.CurrentThread.CurrentCulture.ToString();
			ResourceManager resources = (ResourceManager) _resourcesTable[resourcesName];

			if (resources == null)
			{
				// get the resource manager for the given culture and save it in
				// the hashtable
				try
				{
					resources = new ResourceManager("Newtera.DataGridActiveX.MessageResourceManager",
						typeof(Newtera.DataGridActiveX.MessageResourceManager).Assembly);

					_resourcesTable[resourcesName] = resources;
				}
				catch (Exception e)
				{
					string msg = e.Message;
				}
			}

			return resources;
		}

		/// <summary>
		/// Gets an object from resources of a default culture
		/// </summary>
		/// <param name="name">The specified name</param>
		/// <returns>An object</returns>
		public static object GetObject(string name)
		{
			return GetObject(null, name);
		}

		/// <summary>
		/// Gets an object from resources of a specified culture
		/// </summary>
		/// <param name="culture">The culture info</param>
		/// <param name="name">The specified name</param>
		/// <returns>An object</returns>
		public static object GetObject(CultureInfo culture, string name)
		{
			ResourceManager resources = GetResources();
			if (resources != null)
			{
				return resources.GetObject(name, culture);
			}

			return null;
		}

		/// <summary>
		/// Gets a string from resources of a default culture
		/// </summary>
		/// <param name="name">The specified name</param>
		/// <returns>A string</returns>
		public static string GetString(string name)
		{
			return GetString(null, name);
		}

		/// <summary>
		/// Gets a string from resources of a specified culture
		/// </summary>
		/// <param name="culture">The culture info</param>
		/// <param name="name">The specified name</param>
		/// <returns>A String</returns>
		public static string GetString(CultureInfo culture, string name)
		{
			ResourceManager resources = GetResources();
			if (resources != null)
			{
				return resources.GetString(name);
			}

			return null;
		}
	}
}