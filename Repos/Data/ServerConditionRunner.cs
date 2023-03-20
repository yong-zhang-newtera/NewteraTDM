/*
* @(#)ServerConditionRunner.cs
*
* Copyright (c) 2003 Newtera, Inc. All rights reserved.
*
*/
namespace Newtera.Data
{
	using System;
	using System.Xml;

    using Newtera.Common.Core;
	using Newtera.Server.Engine.Interpreter;
	using Newtera.Common.MetaData.XaclModel.Processor;

	/// <summary>
	/// Runs a condition expressed in xquery using xquery interpreter
	/// </summary>
	/// <version>  	1.0.0 18 Dec. 2003</version>
	/// <author>  Yong Zhang </author>
	internal class ServerConditionRunner : IConditionRunner
	{
		/// <summary> 
		/// Initiate a new instance of ServerConditionRunner class
		/// </summary>
		public ServerConditionRunner()
		{
		}

		/// <summary>
		/// Gets an information indicating if a condition expressed in xquery is met or
		/// or not.
		/// </summary>
		/// <param name="condition">The condition expressed in xquery</param>
		/// <returns>true if the condition is met, false otherwise</returns>
		public bool IsConditionMet(string condition)
		{
            try
            {
                Interpreter interpreter = new Interpreter();

                // execute the xquery using interpreter
                XmlDocument doc = interpreter.Query(condition);

                string result = doc.DocumentElement["flag"].InnerText;
                if (result == "1")
                {
                    return true;
                }
                else
                {
                    return false;
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
		}
	}
}