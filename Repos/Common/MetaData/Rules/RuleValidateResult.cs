/*
* @(#)RuleValidateResult.cs
*
* Copyright (c) 2003-2007 Newtera, Inc. All rights reserved.
*
*/
namespace Newtera.Common.MetaData.Rules
{
	using System;
	using System.Xml;
	using System.Collections;

	/// <summary> 
	/// The class that represents information as result of validating a rule
	/// </summary>
	/// <version> 1.0.0 18 Oct 2007</version>
    public class RuleValidateResult
	{
        private string _message = null;
	
		/// <summary>
		/// Initiate an instance of RuleValidateResult class
		/// </summary>
        /// <param name="message">Validating result message</param>
		public RuleValidateResult(string message)
		{
            _message = message;
		}

        /// <summary>
        /// Gets information indicating whether the result represents an error.
        /// </summary>
        /// <value>true if it has an error, false otherwise.</value>
        public bool HasError
        {
            get
            {
                if (string.IsNullOrEmpty(_message))
                {
                    return false;
                }
                else
                {
                    return true;
                }
            }
        }

        /// <summary>
        /// Gets validating message.
        /// </summary>
        /// <value>The validating message string, or null.</value>
        public string Message
        {
            get
            {
                return _message;
            }
        }
	}
}