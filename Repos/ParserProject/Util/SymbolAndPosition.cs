/*
* @(#)SymbolAndPosition.cs
*
* Copyright (c) 2005 Newtera, Inc. All rights reserved.
*
*/
namespace Newtera.ParserGen.Util
{
	/// <summary>
	/// Struct that stores a symbol and its position in the grammar
	/// </summary>
	public class SymbolAndPosition
	{
		public string Symbol;
		public string AntlrSymbol;
		public int Position;
		public int Length;
		public bool IsStart;
		public bool IsTerminal;
		public bool IsHidden = false;
		public bool HideLiterals = false;
	}
}