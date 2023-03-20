// $ANTLR 2.7.5 (20050201): "JustFax.g" -> "WildcardLexer.cs"$


namespace Newtera.ParserGen.Util
{
	// Generate header specific to lexer CSharp file
	using System;
	using Stream                          = System.IO.Stream;
	using TextReader                      = System.IO.TextReader;
	using Hashtable                       = System.Collections.Hashtable;
	using Comparer                        = System.Collections.Comparer;
	
	using TokenStreamException            = antlr.TokenStreamException;
	using TokenStreamIOException          = antlr.TokenStreamIOException;
	using TokenStreamRecognitionException = antlr.TokenStreamRecognitionException;
	using CharStreamException             = antlr.CharStreamException;
	using CharStreamIOException           = antlr.CharStreamIOException;
	using ANTLRException                  = antlr.ANTLRException;
	using CharScanner                     = antlr.CharScanner;
	using InputBuffer                     = antlr.InputBuffer;
	using ByteBuffer                      = antlr.ByteBuffer;
	using CharBuffer                      = antlr.CharBuffer;
	using Token                           = antlr.Token;
	using IToken                          = antlr.IToken;
	using CommonToken                     = antlr.CommonToken;
	using SemanticException               = antlr.SemanticException;
	using RecognitionException            = antlr.RecognitionException;
	using NoViableAltForCharException     = antlr.NoViableAltForCharException;
	using MismatchedCharException         = antlr.MismatchedCharException;
	using TokenStream                     = antlr.TokenStream;
	using LexerSharedInputState           = antlr.LexerSharedInputState;
	using BitSet                          = antlr.collections.impl.BitSet;
	
	public 	class WildcardLexer : antlr.CharScanner	, TokenStream
	{
		public const int EOF = 1;
		public const int NULL_TREE_LOOKAHEAD = 3;
		
		public WildcardLexer(Stream ins) : this(new ByteBuffer(ins))
		{
		}
		
		public WildcardLexer(TextReader r) : this(new CharBuffer(r))
		{
		}
		
		public WildcardLexer(InputBuffer ib) : this(new LexerSharedInputState(ib))
		{
		}
		
		public WildcardLexer(LexerSharedInputState state) : base(state)
		{
			initialize();
		}
		private void initialize()
		{
			caseSensitiveLiterals = true;
			setCaseSensitive(true);
            literals = new Hashtable(100, (float)0.4);
		}
		
		override public IToken nextToken()			//throws TokenStreamException
		{
			IToken theRetToken = null;
			tryAgain:
			for (;;)
			{
				int _ttype = Token.INVALID_TYPE;
				resetText();
				try     // for char stream error handling
				{
					try     // for lexical error handling
					{
						
						if (cached_LA1==EOF_CHAR)
						{ 
							uponEOF();
							returnToken_ = makeToken(Token.EOF_TYPE);
						}
						else if (((cached_LA1 >= '\u0000' && cached_LA1 <= '\ufffe')))
						{
							mWILDCARD(true);
							theRetToken = returnToken_;
						}
						else
						{
							throw new NoViableAltForCharException(cached_LA1, getFilename(), getLine(), getColumn());
						}

						if ( null==returnToken_ ) goto tryAgain;
						_ttype = returnToken_.Type;
						returnToken_.Type = _ttype;
						return returnToken_;
					}
					catch (RecognitionException e) {
							throw new TokenStreamRecognitionException(e);
					}
				}
				catch (CharStreamException cse) {
					if ( cse is CharStreamIOException ) {
						throw new TokenStreamIOException(((CharStreamIOException)cse).io);
					}
					else {
						throw new TokenStreamException(cse.Message);
					}
				}
			}
		}
		
	
		protected void mWILDCARD(bool _createToken) //throws RecognitionException, CharStreamException, TokenStreamException
		{
			int _ttype; IToken _token=null; int _begin=text.Length;
			_ttype = 2;
			
			/*
			if ((cached_LA1=='\r'))
			{
				match("\r\n");
				if (0==inputState.guessing)
				{
					newline();
				}
			}
			else */
			if (((cached_LA1 >= '\u0000' && cached_LA1 <= '\ufffe'))) {
				matchNot(EOF/*_CHAR*/);
			}

			if (_createToken && (null == _token))
			{
				_token = makeToken(_ttype);
				//_token.setText(text.ToString(_begin, text.Length-_begin));
				_token.setText("");
			}
			returnToken_ = _token;
		}
	}
}
