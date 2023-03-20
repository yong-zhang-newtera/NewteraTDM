/*
* @(#)WinClientUtil.cs
*
* Copyright (c) 2011 Newtera, Inc. All rights reserved.
*
*/
namespace Newtera.WinClientCommon
{
	using System;
	using System.Collections;

	using Newtera.Common.Core;
	using Newtera.Common.MetaData;

	/// <summary>
	/// An utility class for windows client
	/// </summary>
	/// <version>  	1.0.0 04 Jan 2010 </version>
	public class WinClientUtil
	{
		/// <summary>
        ///  Get a message of original exception from a soap exception.
		/// </summary>
		/// <returns> The message of original exception from a soap exception.</returns>
		static public string GetOriginalMessage(string soapErrorMessage)
		{
            string msg = soapErrorMessage;

            int pos = msg.IndexOf("--->");
            if (pos > 0)
            {
                msg = msg.Substring(pos + 4);
                pos = msg.IndexOf(" at");
                if (pos > 0)
                {
                    msg = msg.Substring(0, pos);
                }
            }

            return msg;
		}
	}
}