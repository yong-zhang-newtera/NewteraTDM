/*
* @(#)MismatchedTokenLengthException.cs
*
* Copyright (c) 2006 Newtera, Inc. All rights reserved.
*
*/
using System;
using System.Text;

using antlr;

namespace Newtera.ParserGen.Util
{
	/// <summary>
	/// Represents a LengthConstraint component in GDL.
	/// </summary>
	/// <version> 1.0.0 02 Jan 2006 </version>
	/// <author> Yong Zhang</author>
	[Serializable]
	public class MismatchedTokenLengthException : RecognitionException
	{
		// The token that was encountered
		public IToken token;
		public int minLength;
		public int maxLength;
		
		// Expected token with length constraint
		public MismatchedTokenLengthException(IToken token_, int minLen, int maxLen, string fileName_) :
					base("Mismatched Token Length", fileName_, token_.getLine(), token_.getColumn())
		{
			token = token_;
			minLength = minLen;
			maxLength = maxLen;
		}
		
		/*
		* Returns a clean error message (no line number/column information)
		*/
		override public string Message
		{
			get 
			{
				string maxLenStr = "unbound";
				if (maxLength >=0)
				{
					maxLenStr = maxLength.ToString();
				}

				StringBuilder sb = new StringBuilder();
	
				sb.Append("the token \'" + token.getText() + "\' does not meet the length constraint in the range of " + this.minLength + " and " + maxLenStr + ".");
				
				return sb.ToString();
			}
		}
	}
}