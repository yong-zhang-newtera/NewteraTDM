namespace antlr
{
	/*ANTLR Translator Generator
	* Project led by Terence Parr at http://www.jGuru.com
	* Software rights: http://www.antlr.org/RIGHTS.html
	*
	* $Id: $
	*/

	//
	// ANTLR C# Code Generator by Micheal Jordan
	//                            Kunle Odutola       : kunle UNDERSCORE odutola AT hotmail DOT com
	//                            Anthony Oguntimehin
	//
	// With many thanks to Eric V. Smith from the ANTLR list.
	//
	
	using System;
	
	public class ANTLRPanicException : ANTLRException 
	{
		public ANTLRPanicException() : base() 
		{
		}

		public ANTLRPanicException(string s) : base(s)
		{
		}

		public ANTLRPanicException(string s, Exception inner) : base(s, inner)
		{
		}
	}
}
