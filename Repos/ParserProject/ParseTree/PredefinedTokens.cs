/*
* @(#)PredefinedTokens.cs
*
* Copyright (c) 2005 Newtera, Inc. All rights reserved.
*
*/
namespace Newtera.ParserGen.ParseTree
{
	using System;
	using System.Xml;

	/// <summary>
	/// Provides a set of predefined tokens
	/// interface.
	/// </summary>
	/// <version> 1.0.0 28 Nov 2005</version>
	/// <author> Yong Zhang </author>
	public class PredefinedTokens
	{
		public static string EMPTY_LINE = "EMPTY_LINE";

		public static string[] PREDEFINED_TOKENS = new string[] {
			"UNKNOWN",
			"ALPHA",
			"ALPHA_",
			"ALPHANUMERIC",
			"NUMERIC",
			"IDENTIFIER",
			"REAL",
			"QUOTEDSTRING",
			"WHITESPACE",
			"EMPTYLINE"
		};
	}
}