/*
* @(#)DisplaySettingVisitor.cs
*
* Copyright (c) 2005 Newtera, Inc. All rights reserved.
*
*/
namespace Newtera.ParserGen.ParseTree
{
	using System;
	using System.Data;
	using System.Collections;

	using Newtera.ParserGen.Util;

	/// <summary>
	/// Represents a visitor that traverse a parse tree and
	/// assign diaplay related info to the parse tree. 
	/// </summary>
	/// <version> 1.0.0 03 Jan 2006 </version>
	/// <author> Yong Zhang</author>
	public class DisplaySettingVisitor : IParseTreeNodeVisitor
	{
		private Hashtable _nonterminals = null;
		private Hashtable _predefines = null;

		/// <summary>
		/// Instantiate an instance of DisplaySettingVisitor class
		/// </summary>
		/// <param name="nonterminals">The hashtable of nonterminals.</param>
		/// <param name="predefines">The hsahtable of predefines</param>
		public DisplaySettingVisitor(Hashtable nonterminals, Hashtable predefines)
		{
			_nonterminals = nonterminals;
			_predefines = predefines;
		}

		/// <summary>
		/// Viste a rule node.
		/// </summary>
		/// <param name="node">A ParseTreeRuleNode instance</param>
		/// <returns>true to contibute visiting nested nodes, false to stop</returns>
		public bool VisitRule(ParseTreeRuleNode node)
		{
			SymbolAndPosition sp;
			if (_nonterminals.ContainsKey(node.Name))
			{
				sp = (SymbolAndPosition) _nonterminals[node.Name];
				node.Caption = sp.Symbol; // set original symbol name as caption
				node.IsTerminal = sp.IsTerminal;
				node.IsHidden = sp.IsHidden;
			}
			return true;
		}

		/// <summary>
		/// Viste a token node.
		/// </summary>
		/// <param name="node">A ParseTreeTokenNode instance</param>
		/// <returns>true to contibute visiting nested nodes, false to stop</returns>
		public bool VisitToken(ParseTreeTokenNode node)
		{
			SymbolAndPosition sp;
			if (_predefines.ContainsKey(node.Name))
			{
				sp = (SymbolAndPosition) _predefines[node.Name];
			
				node.Caption = sp.Symbol; // set original symbol name as caption
			}

			return true;
		}

		/// <summary>
		/// Viste a literal node.
		/// </summary>
		/// <param name="node">A ParseTreeLiteralNode instance</param>
		/// <returns>true to contibute visiting nested nodes, false to stop</returns>
		public bool VisitLiteral(ParseTreeLiteralNode node)
		{
			if (node.Parent != null && node.Parent is ParseTreeRuleNode)
			{
				// check the HideLiteral attribute of the parent rule node
				if (_nonterminals.ContainsKey(node.Parent.Name))
				{
					SymbolAndPosition sp = (SymbolAndPosition) _nonterminals[node.Parent.Name];
					if (sp.HideLiterals)
					{
						node.IsHidden = true;
					}
					else
					{
						node.IsHidden = false;
					}
				}
			}
			return true;
		}
	}
}