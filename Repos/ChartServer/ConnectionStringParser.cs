/*
* @(#)ConnectionStringParser.cs
*
* Copyright (c) 2006 Newtera, Inc. All rights reserved.
*
*/
namespace Newtera.ChartServer
{
	using System;
	using System.Xml;
	using System.Collections;
	using System.Text.RegularExpressions;

	/// <summary>
	/// Parsing a connection string
	/// </summary>
	/// <version> 1.0.0 28 April 2006</version>
	public class ConnectionStringParser
	{
		private const string USER_ID = "USER_ID";
		private const string SCHEMA_NAME = "SCHEMA_NAME";
		private const string SCHEMA_VERSION = "SCHEMA_VERSION";

		Hashtable _properties;

		public ConnectionStringParser(string connectionStr)
		{
			_properties = new Hashtable();

			// Compile regular expression to find "name = value" pairs
			Regex regex = new Regex(@"\w+\s*=\s*[^;]*");

			MatchCollection matches = regex.Matches(connectionStr);
			foreach (Match match in matches)
			{
				int pos = match.Value.IndexOf("=");
				string key = match.Value.Substring(0, pos).TrimEnd();
				string val = match.Value.Substring(pos + 1).TrimStart();
				_properties[key] = val;
			}
		}

		public string SchemaName
		{
			get
			{
				return (string) _properties[SCHEMA_NAME];
			}
		}

		public string Version
		{
			get
			{
				return (string) _properties[SCHEMA_VERSION];
			}
		}

		public string UserName
		{
			get
			{
				return (string) _properties[USER_ID];
			}
		}
	}
}