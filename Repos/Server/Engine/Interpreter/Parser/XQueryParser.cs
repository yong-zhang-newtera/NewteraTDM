// $ANTLR 2.7.2: "XQuery.g" -> "XQueryParser.cs"$


namespace Newtera.Server.Engine.Interpreter.Parser
{
	// Generate the header common to all output files.
	using System;
	
	using TokenBuffer              = antlr.TokenBuffer;
	using TokenStreamException     = antlr.TokenStreamException;
	using TokenStreamIOException   = antlr.TokenStreamIOException;
	using ANTLRException           = antlr.ANTLRException;
	using LLkParser = antlr.LLkParser;
	using Token                    = antlr.Token;
	using TokenStream              = antlr.TokenStream;
	using RecognitionException     = antlr.RecognitionException;
	using NoViableAltException     = antlr.NoViableAltException;
	using MismatchedTokenException = antlr.MismatchedTokenException;
	using SemanticException        = antlr.SemanticException;
	using ParserSharedInputState   = antlr.ParserSharedInputState;
	using BitSet                   = antlr.collections.impl.BitSet;
	
   // global code stuff that will be included in the source file just before the 'XQueryParser' class below
	using System.Collections;
	using Newtera.Server.Engine.Interpreter;
	using Newtera.Server.Engine.Vdom;

	public 	class XQueryParser : antlr.LLkParser
	{
		public const int EOF = 1;
		public const int NULL_TREE_LOOKAHEAD = 3;
		public const int LITERAL_import = 4;
		public const int Name = 5;
		public const int LITERAL_as = 6;
		public const int COMMA = 7;
		public const int SLASH = 8;
		public const int SLASH_SLASH = 9;
		public const int SLASH_AT = 10;
		public const int DEREFERENCE = 11;
		public const int AT = 12;
		public const int COLON = 13;
		public const int L_BRACKET = 14;
		public const int R_BRACKET = 15;
		public const int DOLLAR = 16;
		public const int LITERAL_return = 17;
		public const int LITERAL_for = 18;
		public const int LITERAL_in = 19;
		public const int LITERAL_let = 20;
		public const int SET_EQUAL_TO = 21;
		public const int LITERAL_where = 22;
		public const int LITERAL_sortby = 23;
		public const int L_PAREN = 24;
		public const int R_PAREN = 25;
		public const int LITERAL_ascending = 26;
		public const int LITERAL_descending = 27;
		public const int LITERAL_or = 28;
		public const int LITERAL_and = 29;
		public const int LITERAL_to = 30;
		public const int NOT_EQUALS = 31;
		public const int EQUALS = 32;
		public const int LESS_THAN = 33;
		public const int GREATER_THAN = 34;
		public const int LESS_THAN_EQUALS = 35;
		public const int GREATER_THAN_EQUALS = 36;
		public const int LITERAL_ni = 37;
		public const int LITERAL_like = 38;
		public const int PLUS = 39;
		public const int MINUS = 40;
		public const int STAR = 41;
		public const int LITERAL_div = 42;
		public const int MOD = 43;
		public const int LITERAL_null = 44;
		public const int INLINE = 45;
		public const int NUMERIC_LITERAL = 46;
		public const int STRING_LITERAL = 47;
		public const int LITERAL_if = 48;
		public const int LITERAL_then = 49;
		public const int LITERAL_else = 50;
		public const int LESS_THAN_SLASH = 51;
		public const int L_BRACE = 52;
		public const int R_BRACE = 53;
		public const int NL = 54;
		public const int CDATA_DATA = 55;
		public const int LETTER = 56;
		public const int DIGIT = 57;
		public const int ESC = 58;
		public const int HEX_DIGIT = 59;
		public const int PREDEFINED_ENTITY_REF = 60;
		public const int WS = 61;
		public const int SEMICOLON = 62;
		public const int D_COLON = 63;
		public const int STAR_COLON = 64;
		public const int D_EQUALS = 65;
		public const int NOT_D_EQUALS = 66;
		public const int BAR = 67;
		public const int QUOTE = 68;
		public const int ESCAPED_QUOTE = 69;
		public const int S_QUOTE = 70;
		public const int ESCAPED_S_QUOTE = 71;
		public const int SLASH_GREATER_THAN = 72;
		
		
   // additional methods and members for the generated 'MyParser' class
	Interpreter _interpreter;
	Stack _stack;
	Stack _contextStack; // Soly used for providing context to the path embedded in a predicate

	public XQueryParser(Interpreter interpreter, XQueryLexer lexer) : base(lexer, 1)
	{
		_interpreter = interpreter;
	}
	
	public IExpr Parse()
	{
		return Parse(null);
	}
	
	public IExpr Parse(IExpr context)
	{
        
		_stack = new Stack();
		_contextStack = new Stack();
		
		if (context != null)
		{
			_contextStack.Push(context);
		}
		
		start();
		
		if (_stack.Count != 1)
		{
			throw new Exception("Parser fatal error: Bad stack size: " + _stack.Count);
		}

		return  (IExpr) _stack.Pop();
	}	
		
		protected void initialize()
		{
			tokenNames = tokenNames_;
		}
		
		
		protected XQueryParser(TokenBuffer tokenBuf, int k) : base(tokenBuf, k)
		{
			initialize();
		}
		
		public XQueryParser(TokenBuffer tokenBuf) : this(tokenBuf,2)
		{
		}
		
		protected XQueryParser(TokenStream lexer, int k) : base(lexer,k)
		{
			initialize();
		}
		
		public XQueryParser(TokenStream lexer) : this(lexer,2)
		{
		}
		
		public XQueryParser(ParserSharedInputState state) : base(state,2)
		{
			initialize();
		}
		
	public void start() //throws RecognitionException, TokenStreamException
{
		
		
		queryModule();
		if (0==inputState.guessing)
		{
			
					IExpr expr = (IExpr) _stack.Pop();
					_stack.Push(new QueryExpr(_interpreter, expr));
			
		}
	}
	
/** [2] **/
	public void queryModule() //throws RecognitionException, TokenStreamException
{
		
		
		{    // ( ... )*
			for (;;)
			{
				if ((LA(1)==LITERAL_import))
				{
					importAs();
				}
				else
				{
					goto _loop4_breakloop;
				}
				
			}
_loop4_breakloop:			;
		}    // ( ... )*
		expression();
	}
	
	public void importAs() //throws RecognitionException, TokenStreamException
{
		
		
		match(LITERAL_import);
		match(Name);
		match(LITERAL_as);
		match(Name);
	}
	
	public void expression() //throws RecognitionException, TokenStreamException
{
		
		
		basicExpression();
	}
	
	public void exprSequence() //throws RecognitionException, TokenStreamException
{
		
		
			int depth = _stack.Count;
		
		
		expression();
		{    // ( ... )*
			for (;;)
			{
				if ((LA(1)==COMMA))
				{
					match(COMMA);
					expression();
				}
				else
				{
					goto _loop8_breakloop;
				}
				
			}
_loop8_breakloop:			;
		}    // ( ... )*
		if (0==inputState.guessing)
		{
			
					IExpr expr = null;
					if (_stack.Count > depth)
					{
						ExprCollection exprs = new ExprCollection();
						while (_stack.Count > depth)
						{
							expr = (IExpr) _stack.Pop();
							exprs.Insert(0, expr);
						}
						_stack.Push(exprs);
					}
				
		}
	}
	
	public void basicExpression() //throws RecognitionException, TokenStreamException
{
		
		
		switch ( LA(1) )
		{
		case LITERAL_for:
		case LITERAL_let:
		{
			flwrExpr();
			break;
		}
		case LITERAL_if:
		{
			ifExpr();
			break;
		}
		case Name:
		case SLASH:
		case SLASH_SLASH:
		case SLASH_AT:
		case DEREFERENCE:
		case AT:
		case DOLLAR:
		case L_PAREN:
		case LESS_THAN:
		case MINUS:
		case LITERAL_null:
		case INLINE:
		case NUMERIC_LITERAL:
		case STRING_LITERAL:
		{
			logicalOrExpression();
			break;
		}
		default:
		{
			throw new NoViableAltException(LT(1), getFilename());
		}
		 }
	}
	
	public void pathExpression() //throws RecognitionException, TokenStreamException
{
		
		
			bool hasSortBy = false;
		
		
		if ((LA(1)==DOLLAR))
		{
			variable(false);
			path();
			if (0==inputState.guessing)
			{
				
							ExprCollection steps = (ExprCollection) _stack.Pop();
							IExpr context = (IExpr) _stack.Pop();
							_stack.Push(new Path(_interpreter, steps, context));
						
			}
		}
		else if ((LA(1)==Name) && (LA(2)==L_PAREN)) {
			functionCall();
			path();
			if (0==inputState.guessing)
			{
				
							ExprCollection steps = (ExprCollection) _stack.Pop();
							IExpr context = (IExpr) _stack.Pop();
							_stack.Push(new Path(_interpreter, steps, context));        
				
			}
		}
		else if ((tokenSet_0_.member(LA(1))) && (tokenSet_1_.member(LA(2)))) {
			path();
			if (0==inputState.guessing)
			{
				
							ExprCollection steps = (ExprCollection) _stack.Pop();
							Path pathExpr = new Path(_interpreter, steps, null); 
							if (_contextStack.Count > 0)
							{
								IExpr context = (IExpr) _contextStack.Peek();
								pathExpr.Context = context;
							}
							_stack.Push(pathExpr); 			       
					
			}
		}
		else
		{
			throw new NoViableAltException(LT(1), getFilename());
		}
		
	}
	
	public void variable(
		bool isDef
	) //throws RecognitionException, TokenStreamException
{
		
		Token  t = null;
		
		match(DOLLAR);
		t = LT(1);
		match(Name);
		if (0==inputState.guessing)
		{
			
				_stack.Push(new Ident(_interpreter, t.getText(), isDef));
			
		}
	}
	
	public void path() //throws RecognitionException, TokenStreamException
{
		
		
			int depth = _stack.Count;
		
		
		{ // ( ... )+
		int _cnt13=0;
		for (;;)
		{
			if ((tokenSet_0_.member(LA(1))))
			{
				step();
			}
			else
			{
				if (_cnt13 >= 1) { goto _loop13_breakloop; } else { throw new NoViableAltException(LT(1), getFilename());; }
			}
			
			_cnt13++;
		}
_loop13_breakloop:		;
		}    // ( ... )+
		if (0==inputState.guessing)
		{
			
					IExpr step = null;
					if (_stack.Count > depth)
					{
						ExprCollection steps = new ExprCollection();
						while (_stack.Count > depth)
						{
							step = (IExpr) _stack.Pop();
							steps.Insert(0, step);
						}
						_stack.Push(steps);
					}
				
		}
	}
	
	public void functionCall() //throws RecognitionException, TokenStreamException
{
		
		Token  t = null;
		
			bool hasArguments = false;
		
		
		t = LT(1);
		match(Name);
		match(L_PAREN);
		{
			switch ( LA(1) )
			{
			case Name:
			case SLASH:
			case SLASH_SLASH:
			case SLASH_AT:
			case DEREFERENCE:
			case AT:
			case DOLLAR:
			case LITERAL_for:
			case LITERAL_let:
			case L_PAREN:
			case LESS_THAN:
			case MINUS:
			case LITERAL_null:
			case INLINE:
			case NUMERIC_LITERAL:
			case STRING_LITERAL:
			case LITERAL_if:
			{
				exprSequence();
				if (0==inputState.guessing)
				{
					hasArguments = true;
				}
				break;
			}
			case R_PAREN:
			{
				break;
			}
			default:
			{
				throw new NoViableAltException(LT(1), getFilename());
			}
			 }
		}
		match(R_PAREN);
		if (0==inputState.guessing)
		{
			
					ExprCollection arguments = null;
					if (hasArguments)
					{
						arguments = (ExprCollection) _stack.Pop();
					}
					
					_stack.Push(new FunctionCall(_interpreter, t.getText(), arguments));
				
		}
	}
	
	public void step() //throws RecognitionException, TokenStreamException
{
		
		Token  p = null;
		Token  t = null;
		
			int axisId = Axis.CHILD;
			string prefix = null;
			Step stepExpr = null;
		
		
		{
			switch ( LA(1) )
			{
			case SLASH:
			{
				match(SLASH);
				if (0==inputState.guessing)
				{
					axisId = Axis.CHILD;
				}
				break;
			}
			case SLASH_SLASH:
			{
				match(SLASH_SLASH);
				if (0==inputState.guessing)
				{
					axisId = Axis.DESCENDANT;
				}
				break;
			}
			case SLASH_AT:
			{
				match(SLASH_AT);
				if (0==inputState.guessing)
				{
					axisId = Axis.ATTRIBUTE;
				}
				break;
			}
			case DEREFERENCE:
			{
				match(DEREFERENCE);
				if (0==inputState.guessing)
				{
					axisId = Axis.DEREFERENCE;
				}
				break;
			}
			case AT:
			{
				match(AT);
				if (0==inputState.guessing)
				{
					axisId = Axis.ATTRIBUTE;
				}
				break;
			}
			case Name:
			{
				break;
			}
			default:
			{
				throw new NoViableAltException(LT(1), getFilename());
			}
			 }
		}
		{
			if ((LA(1)==Name) && (LA(2)==COLON))
			{
				p = LT(1);
				match(Name);
				if (0==inputState.guessing)
				{
					prefix = p.getText();
				}
				match(COLON);
			}
			else if ((LA(1)==Name) && (tokenSet_2_.member(LA(2)))) {
			}
			else
			{
				throw new NoViableAltException(LT(1), getFilename());
			}
			
		}
		t = LT(1);
		match(Name);
		if (0==inputState.guessing)
		{
			stepExpr = new Step(_interpreter, t.getText(), prefix, axisId); _contextStack.Push(stepExpr);
		}
		{
			switch ( LA(1) )
			{
			case L_BRACKET:
			{
				predicate();
				if (0==inputState.guessing)
				{
					stepExpr.Qualifier = (IExpr) _stack.Pop();
				}
				break;
			}
			case EOF:
			case Name:
			case COMMA:
			case SLASH:
			case SLASH_SLASH:
			case SLASH_AT:
			case DEREFERENCE:
			case AT:
			case R_BRACKET:
			case LITERAL_return:
			case LITERAL_for:
			case LITERAL_in:
			case LITERAL_let:
			case LITERAL_where:
			case LITERAL_sortby:
			case R_PAREN:
			case LITERAL_ascending:
			case LITERAL_descending:
			case LITERAL_or:
			case LITERAL_and:
			case LITERAL_to:
			case NOT_EQUALS:
			case EQUALS:
			case LESS_THAN:
			case GREATER_THAN:
			case LESS_THAN_EQUALS:
			case GREATER_THAN_EQUALS:
			case LITERAL_ni:
			case LITERAL_like:
			case PLUS:
			case MINUS:
			case STAR:
			case LITERAL_div:
			case MOD:
			case LITERAL_else:
			case R_BRACE:
			{
				break;
			}
			default:
			{
				throw new NoViableAltException(LT(1), getFilename());
			}
			 }
		}
		if (0==inputState.guessing)
		{
			
					_contextStack.Pop();
					_stack.Push(stepExpr);
				
		}
	}
	
	public void predicate() //throws RecognitionException, TokenStreamException
{
		
		
		match(L_BRACKET);
		expression();
		match(R_BRACKET);
	}
	
	public void flwrExpr() //throws RecognitionException, TokenStreamException
{
		
		
			IExpr where = null;
			bool hasWhere = false;
			IExpr sortby = null;
			bool hasSortby = false;
		
		
		flClause();
		{
			switch ( LA(1) )
			{
			case LITERAL_where:
			{
				whereClause();
				if (0==inputState.guessing)
				{
					hasWhere = true;
				}
				break;
			}
			case LITERAL_return:
			{
				break;
			}
			default:
			{
				throw new NoViableAltException(LT(1), getFilename());
			}
			 }
		}
		match(LITERAL_return);
		primitiveExpression();
		{
			switch ( LA(1) )
			{
			case LITERAL_sortby:
			{
				sortByClause();
				if (0==inputState.guessing)
				{
					hasSortby = true;
				}
				break;
			}
			case EOF:
			case COMMA:
			case R_BRACKET:
			case LITERAL_return:
			case LITERAL_for:
			case LITERAL_let:
			case LITERAL_where:
			case R_PAREN:
			case LITERAL_else:
			case R_BRACE:
			{
				break;
			}
			default:
			{
				throw new NoViableAltException(LT(1), getFilename());
			}
			 }
		}
		if (0==inputState.guessing)
		{
			
					if (hasSortby)
					{
						sortby = (IExpr) _stack.Pop();
					}
					
					IExpr returnExpr = (IExpr) _stack.Pop();
					
					if (hasWhere)
					{
						where = (IExpr) _stack.Pop();
					}
					
					IExpr fl = (IExpr) _stack.Pop();
					
					_stack.Push(new Flwr(_interpreter, fl, where, returnExpr, sortby));
				
		}
	}
	
/** [17] **/
	public void ifExpr() //throws RecognitionException, TokenStreamException
{
		
		
		match(LITERAL_if);
		match(L_PAREN);
		expression();
		match(R_PAREN);
		match(LITERAL_then);
		expression();
		match(LITERAL_else);
		expression();
		if (0==inputState.guessing)
		{
			
					IExpr elseClause = (IExpr) _stack.Pop();
					IExpr thenClause = (IExpr) _stack.Pop();
					IExpr condition = (IExpr) _stack.Pop();
					_stack.Push(new If(_interpreter, condition, thenClause, elseClause));
				
		}
	}
	
	public void logicalOrExpression() //throws RecognitionException, TokenStreamException
{
		
		
			bool isLogicalOr = false;
			int depth = _stack.Count;
		
		
		logicalAndExpression();
		{    // ( ... )*
			for (;;)
			{
				if ((LA(1)==LITERAL_or))
				{
					match(LITERAL_or);
					if (0==inputState.guessing)
					{
						isLogicalOr = true;
					}
					logicalAndExpression();
				}
				else
				{
					goto _loop38_breakloop;
				}
				
			}
_loop38_breakloop:			;
		}    // ( ... )*
		if (0==inputState.guessing)
		{
			
					if (isLogicalOr)
					{
						IExpr left, right;
						right = (IExpr) _stack.Pop();
						left = (IExpr) _stack.Pop();
						right = new Or(_interpreter, left, right);
						while (_stack.Count > depth)
						{
							left = (IExpr) _stack.Pop();
							right = new Or(_interpreter, left, right);
						}
						
						_stack.Push(right);
					}		
				
		}
	}
	
	public void flClause() //throws RecognitionException, TokenStreamException
{
		
		
			int depth = _stack.Count;
			FLClause fl = new FLClause(_interpreter);
		
		
		{ // ( ... )+
		int _cnt26=0;
		for (;;)
		{
			switch ( LA(1) )
			{
			case LITERAL_for:
			{
				forClause();
				break;
			}
			case LITERAL_let:
			{
				letClause();
				break;
			}
			default:
			{
				if (_cnt26 >= 1) { goto _loop26_breakloop; } else { throw new NoViableAltException(LT(1), getFilename());; }
			}
			break; }
			_cnt26++;
		}
_loop26_breakloop:		;
		}    // ( ... )+
		if (0==inputState.guessing)
		{
			
					IExpr expr = null;
					if (_stack.Count > depth)
					{
						while (_stack.Count > depth)
						{
							expr = (IExpr) _stack.Pop();
							fl.InsertExpr(0, expr);
						}
						_stack.Push(fl);
					}		
				
		}
	}
	
	public void whereClause() //throws RecognitionException, TokenStreamException
{
		
		
		match(LITERAL_where);
		expression();
	}
	
	public void primitiveExpression() //throws RecognitionException, TokenStreamException
{
		
		Token  t1 = null;
		
		switch ( LA(1) )
		{
		case NUMERIC_LITERAL:
		case STRING_LITERAL:
		{
			literal();
			break;
		}
		case LITERAL_null:
		{
			match(LITERAL_null);
			if (0==inputState.guessing)
			{
				_stack.Push(new Null(_interpreter));
			}
			break;
		}
		case INLINE:
		{
			t1 = LT(1);
			match(INLINE);
			if (0==inputState.guessing)
			{
				_stack.Push(new InlineXml(_interpreter, t1.getText()));
			}
			break;
		}
		case LESS_THAN:
		{
			elementConstructor();
			break;
		}
		case L_PAREN:
		{
			match(L_PAREN);
			exprSequence();
			match(R_PAREN);
			if (0==inputState.guessing)
			{
				IExpr expr = (IExpr) _stack.Pop(); _stack.Push(new ParenthesizedExpr(_interpreter, expr));
			}
			break;
		}
		default:
			bool synPredMatched63 = false;
			if (((tokenSet_3_.member(LA(1))) && (tokenSet_4_.member(LA(2)))))
			{
				int _m63 = mark();
				synPredMatched63 = true;
				inputState.guessing++;
				try {
					{
						pathExpression();
					}
				}
				catch (RecognitionException)
				{
					synPredMatched63 = false;
				}
				rewind(_m63);
				inputState.guessing--;
			}
			if ( synPredMatched63 )
			{
				pathExpression();
			}
			else if ((LA(1)==Name) && (LA(2)==L_PAREN)) {
				functionCall();
			}
			else if ((LA(1)==DOLLAR) && (LA(2)==Name)) {
				variable(false);
			}
		else
		{
			throw new NoViableAltException(LT(1), getFilename());
		}
		break; }
	}
	
	public void sortByClause() //throws RecognitionException, TokenStreamException
{
		
		
		match(LITERAL_sortby);
		match(L_PAREN);
		sortSpecList();
		match(R_PAREN);
		if (0==inputState.guessing)
		{
			
					ExprCollection sortbySpecs = (ExprCollection) _stack.Pop();
					_stack.Push(new Sortby(_interpreter, sortbySpecs));
			
		}
	}
	
	public void forClause() //throws RecognitionException, TokenStreamException
{
		
		
		match(LITERAL_for);
		variable(true);
		match(LITERAL_in);
		expression();
		if (0==inputState.guessing)
		{
			
					IExpr expr = (IExpr) _stack.Pop();
					IExpr var = (IExpr) _stack.Pop();
					_stack.Push(new For(_interpreter, var, expr));
			
		}
	}
	
	public void letClause() //throws RecognitionException, TokenStreamException
{
		
		
		match(LITERAL_let);
		variable(true);
		match(SET_EQUAL_TO);
		expression();
		if (0==inputState.guessing)
		{
			
					IExpr expr = (IExpr) _stack.Pop();
					IExpr var = (IExpr) _stack.Pop();
					_stack.Push(new Let(_interpreter, var, expr));
				
		}
	}
	
	public void sortSpecList() //throws RecognitionException, TokenStreamException
{
		
		
			int depth = _stack.Count;
			ExprCollection sortSpecs = new ExprCollection();
		
		
		sortSpec();
		{    // ( ... )*
			for (;;)
			{
				if ((LA(1)==COMMA))
				{
					match(COMMA);
					sortSpec();
				}
				else
				{
					goto _loop33_breakloop;
				}
				
			}
_loop33_breakloop:			;
		}    // ( ... )*
		if (0==inputState.guessing)
		{
			
					IExpr spec = null;
					if (_stack.Count > depth)
					{
						while (_stack.Count > depth)
						{
							spec = (IExpr) _stack.Pop();
							sortSpecs.Insert(0, spec);
						}
						_stack.Push(sortSpecs);
					}		
				
		}
	}
	
	public void sortSpec() //throws RecognitionException, TokenStreamException
{
		
		
			bool isAscending = true;
		
		
		pathExpression();
		{
			switch ( LA(1) )
			{
			case LITERAL_ascending:
			{
				match(LITERAL_ascending);
				break;
			}
			case LITERAL_descending:
			{
				match(LITERAL_descending);
				if (0==inputState.guessing)
				{
					isAscending=false;
				}
				break;
			}
			case COMMA:
			case R_PAREN:
			{
				break;
			}
			default:
			{
				throw new NoViableAltException(LT(1), getFilename());
			}
			 }
		}
		if (0==inputState.guessing)
		{
			
					IExpr path = (IExpr) _stack.Pop();
					_stack.Push(new SortbySpec(_interpreter, path, isAscending));
				
		}
	}
	
	public void logicalAndExpression() //throws RecognitionException, TokenStreamException
{
		
		
			bool isLogicalAnd = false;
			int depth = _stack.Count;
		
		
		rangeExpression();
		{    // ( ... )*
			for (;;)
			{
				if ((LA(1)==LITERAL_and))
				{
					match(LITERAL_and);
					if (0==inputState.guessing)
					{
						isLogicalAnd = true;
					}
					rangeExpression();
				}
				else
				{
					goto _loop41_breakloop;
				}
				
			}
_loop41_breakloop:			;
		}    // ( ... )*
		if (0==inputState.guessing)
		{
			
					if (isLogicalAnd)
					{
						IExpr left, right;
						right = (IExpr) _stack.Pop();
						left = (IExpr) _stack.Pop();
						right = new And(_interpreter, left, right);
						while (_stack.Count > depth)
						{
							left = (IExpr) _stack.Pop();
							right = new And(_interpreter, left, right);
						}
						
						_stack.Push(right);
					}		
				
		}
	}
	
	public void rangeExpression() //throws RecognitionException, TokenStreamException
{
		
		
			bool isRange = false;
		
		
		equalityExpression();
		{
			switch ( LA(1) )
			{
			case LITERAL_to:
			{
				match(LITERAL_to);
				if (0==inputState.guessing)
				{
					isRange = true;
				}
				equalityExpression();
				break;
			}
			case EOF:
			case COMMA:
			case R_BRACKET:
			case LITERAL_return:
			case LITERAL_for:
			case LITERAL_let:
			case LITERAL_where:
			case R_PAREN:
			case LITERAL_or:
			case LITERAL_and:
			case LITERAL_else:
			case R_BRACE:
			{
				break;
			}
			default:
			{
				throw new NoViableAltException(LT(1), getFilename());
			}
			 }
		}
		if (0==inputState.guessing)
		{
			
					if (isRange)
					{
						IExpr left, right;
						right = (IExpr) _stack.Pop();
						left = (IExpr) _stack.Pop();
						_stack.Push(new To(_interpreter, left, right));
					}		
				
		}
	}
	
	public void equalityExpression() //throws RecognitionException, TokenStreamException
{
		
		
			bool isEquality = false;
			int type = 0;
		
		
		relationalExpression();
		{    // ( ... )*
			for (;;)
			{
				if ((LA(1)==NOT_EQUALS||LA(1)==EQUALS))
				{
					{
						switch ( LA(1) )
						{
						case NOT_EQUALS:
						{
							match(NOT_EQUALS);
							if (0==inputState.guessing)
							{
								isEquality = true;
							}
							break;
						}
						case EQUALS:
						{
							match(EQUALS);
							if (0==inputState.guessing)
							{
								isEquality = true; type = 1;
							}
							break;
						}
						default:
						{
							throw new NoViableAltException(LT(1), getFilename());
						}
						 }
					}
					relationalExpression();
				}
				else
				{
					goto _loop47_breakloop;
				}
				
			}
_loop47_breakloop:			;
		}    // ( ... )*
		if (0==inputState.guessing)
		{
			
					if (isEquality)
					{
						IExpr left, right;
						right = (IExpr) _stack.Pop();
						left = (IExpr) _stack.Pop();
						switch(type)
						{
							case 0:
								_stack.Push(new NotEquals(_interpreter, left, right));
								break;
							case 1:
								_stack.Push(new Equals(_interpreter, left, right));
								break;
						}				
					}	
				
		}
	}
	
	public void relationalExpression() //throws RecognitionException, TokenStreamException
{
		
		
			bool isRelational = false;
			int type = 0;
		
		
		additiveExpression();
		{    // ( ... )*
			for (;;)
			{
				if ((tokenSet_5_.member(LA(1))))
				{
					{
						switch ( LA(1) )
						{
						case LESS_THAN:
						{
							match(LESS_THAN);
							if (0==inputState.guessing)
							{
								isRelational = true;
							}
							break;
						}
						case GREATER_THAN:
						{
							match(GREATER_THAN);
							if (0==inputState.guessing)
							{
								isRelational = true; type = 1;
							}
							break;
						}
						case LESS_THAN_EQUALS:
						{
							match(LESS_THAN_EQUALS);
							if (0==inputState.guessing)
							{
								isRelational = true; type = 2;
							}
							break;
						}
						case GREATER_THAN_EQUALS:
						{
							match(GREATER_THAN_EQUALS);
							if (0==inputState.guessing)
							{
								isRelational = true; type = 3;
							}
							break;
						}
						case LITERAL_in:
						{
							match(LITERAL_in);
							if (0==inputState.guessing)
							{
								isRelational = true; type = 4;
							}
							break;
						}
						case LITERAL_ni:
						{
							match(LITERAL_ni);
							if (0==inputState.guessing)
							{
								isRelational = true; type = 5;
							}
							break;
						}
						case LITERAL_like:
						{
							match(LITERAL_like);
							if (0==inputState.guessing)
							{
								isRelational = true; type = 6;
							}
							break;
						}
						default:
						{
							throw new NoViableAltException(LT(1), getFilename());
						}
						 }
					}
					additiveExpression();
				}
				else
				{
					goto _loop51_breakloop;
				}
				
			}
_loop51_breakloop:			;
		}    // ( ... )*
		if (0==inputState.guessing)
		{
			
					if (isRelational)
					{
						IExpr left, right;
						right = (IExpr) _stack.Pop();
						left = (IExpr) _stack.Pop();
						switch(type)
						{
							case 0:
								_stack.Push(new LessThan(_interpreter, left, right));
								break;
							case 1:
								_stack.Push(new GreaterThan(_interpreter, left, right));
								break;
							case 2:
								_stack.Push(new LEquals(_interpreter, left, right));
								break;
							case 3:
								_stack.Push(new GEquals(_interpreter, left, right));
								break;
							case 4:
								_stack.Push(new In(_interpreter, left, right));
								break;
							case 5:
								_stack.Push(new NotIn(_interpreter, left, right));
								break;	
							case 6:
								_stack.Push(new Like(_interpreter, left, right));
								break;																				
						}				
					}	
				
		}
	}
	
	public void additiveExpression() //throws RecognitionException, TokenStreamException
{
		
		
			bool isAdditive = false;
			int type = 0;
		
		
		multiplicativeExpression();
		{    // ( ... )*
			for (;;)
			{
				if ((LA(1)==PLUS||LA(1)==MINUS))
				{
					{
						switch ( LA(1) )
						{
						case PLUS:
						{
							match(PLUS);
							if (0==inputState.guessing)
							{
								isAdditive = true;
							}
							break;
						}
						case MINUS:
						{
							match(MINUS);
							if (0==inputState.guessing)
							{
								isAdditive = true; type = 1;
							}
							break;
						}
						default:
						{
							throw new NoViableAltException(LT(1), getFilename());
						}
						 }
					}
					multiplicativeExpression();
				}
				else
				{
					goto _loop55_breakloop;
				}
				
			}
_loop55_breakloop:			;
		}    // ( ... )*
		if (0==inputState.guessing)
		{
			
					if (isAdditive)
					{
						IExpr left, right;
						right = (IExpr) _stack.Pop();
						left = (IExpr) _stack.Pop();
						switch(type)
						{
							case 0:
								_stack.Push(new Plus(_interpreter, left, right));
								break;
							case 1:
								_stack.Push(new Substract(_interpreter, left, right));
								break;																			
						}				
					}
				
		}
	}
	
	public void multiplicativeExpression() //throws RecognitionException, TokenStreamException
{
		
		
			bool isMultiplicative = false;
			int type = 0;
		
		
		unaryExpression();
		{    // ( ... )*
			for (;;)
			{
				if (((LA(1) >= STAR && LA(1) <= MOD)))
				{
					{
						switch ( LA(1) )
						{
						case STAR:
						{
							match(STAR);
							if (0==inputState.guessing)
							{
								isMultiplicative = true;
							}
							break;
						}
						case LITERAL_div:
						{
							match(LITERAL_div);
							if (0==inputState.guessing)
							{
								isMultiplicative = true; type = 1;
							}
							break;
						}
						case MOD:
						{
							match(MOD);
							if (0==inputState.guessing)
							{
								isMultiplicative = true; type = 2;
							}
							break;
						}
						default:
						{
							throw new NoViableAltException(LT(1), getFilename());
						}
						 }
					}
					unaryExpression();
				}
				else
				{
					goto _loop59_breakloop;
				}
				
			}
_loop59_breakloop:			;
		}    // ( ... )*
		if (0==inputState.guessing)
		{
			
					if (isMultiplicative)
					{
						IExpr left, right;
						right = (IExpr) _stack.Pop();
						left = (IExpr) _stack.Pop();
						switch(type)
						{
							case 0:
								_stack.Push(new Times(_interpreter, left, right));
								break;
							case 1:
								_stack.Push(new Divide(_interpreter, left, right));
								break;
							case 2:
								_stack.Push(new Mod(_interpreter, left, right));
								break;																							
						}				
					}
				
		}
	}
	
	public void unaryExpression() //throws RecognitionException, TokenStreamException
{
		
		
		switch ( LA(1) )
		{
		case MINUS:
		{
			match(MINUS);
			unaryExpression();
			if (0==inputState.guessing)
			{
				
							IExpr expr = (IExpr) _stack.Pop();
							_stack.Push(new UMinus(_interpreter, expr));
						
			}
			break;
		}
		case Name:
		case SLASH:
		case SLASH_SLASH:
		case SLASH_AT:
		case DEREFERENCE:
		case AT:
		case DOLLAR:
		case L_PAREN:
		case LESS_THAN:
		case LITERAL_null:
		case INLINE:
		case NUMERIC_LITERAL:
		case STRING_LITERAL:
		{
			primitiveExpression();
			break;
		}
		default:
		{
			throw new NoViableAltException(LT(1), getFilename());
		}
		 }
	}
	
	public void literal() //throws RecognitionException, TokenStreamException
{
		
		Token  t1 = null;
		Token  t2 = null;
		
		switch ( LA(1) )
		{
		case NUMERIC_LITERAL:
		{
			t1 = LT(1);
			match(NUMERIC_LITERAL);
			if (0==inputState.guessing)
			{
				_stack.Push(new Literal(_interpreter, t1.getText()));
			}
			break;
		}
		case STRING_LITERAL:
		{
			t2 = LT(1);
			match(STRING_LITERAL);
			if (0==inputState.guessing)
			{
				_stack.Push(new Literal(_interpreter, new StringType(), new XString(t2.getText())));
			}
			break;
		}
		default:
		{
			throw new NoViableAltException(LT(1), getFilename());
		}
		 }
	}
	
/** [52] **/
	public void elementConstructor() //throws RecognitionException, TokenStreamException
{
		
		Token  t = null;
		
			int depth = _stack.Count;
			ExprCollection attributes = null;
		
		
		match(LESS_THAN);
		t = LT(1);
		match(Name);
		{    // ( ... )*
			for (;;)
			{
				if ((LA(1)==Name||LA(1)==L_BRACE))
				{
					attribute();
				}
				else
				{
					goto _loop70_breakloop;
				}
				
			}
_loop70_breakloop:			;
		}    // ( ... )*
		match(GREATER_THAN);
		elementContents();
		match(LESS_THAN_SLASH);
		match(Name);
		match(GREATER_THAN);
		if (0==inputState.guessing)
		{
			
					IExpr content = (IExpr) _stack.Pop();
			
					if (_stack.Count > depth)
					{
						IExpr attr;
						attributes = new ExprCollection();
						while (_stack.Count > depth)
						{
							attr = (IExpr) _stack.Pop();
							attributes.Insert(0, attr);
						}
					}
					
					_stack.Push(new ElementNode(_interpreter, t.getText(), attributes, content));
				
		}
	}
	
	public void attribute() //throws RecognitionException, TokenStreamException
{
		
		Token  t = null;
		
		switch ( LA(1) )
		{
		case Name:
		{
			t = LT(1);
			match(Name);
			match(EQUALS);
			literal();
			if (0==inputState.guessing)
			{
				
						IExpr attrValue = (IExpr) _stack.Pop();
						_stack.Push(new AttributeNode(_interpreter, t.getText(), attrValue));
					
			}
			break;
		}
		case L_BRACE:
		{
			enclosedExpr();
			break;
		}
		default:
		{
			throw new NoViableAltException(LT(1), getFilename());
		}
		 }
	}
	
	public void elementContents() //throws RecognitionException, TokenStreamException
{
		
		
			int depth = _stack.Count;
		
		
		switch ( LA(1) )
		{
		case NUMERIC_LITERAL:
		case STRING_LITERAL:
		{
			literal();
			break;
		}
		case LESS_THAN:
		case L_BRACE:
		{
			{ // ( ... )+
			int _cnt73=0;
			for (;;)
			{
				switch ( LA(1) )
				{
				case LESS_THAN:
				{
					elementConstructor();
					break;
				}
				case L_BRACE:
				{
					enclosedExpr();
					break;
				}
				default:
				{
					if (_cnt73 >= 1) { goto _loop73_breakloop; } else { throw new NoViableAltException(LT(1), getFilename());; }
				}
				break; }
				_cnt73++;
			}
_loop73_breakloop:			;
			}    // ( ... )+
			if (0==inputState.guessing)
			{
				
							IExpr expr = null;
							if (_stack.Count > depth)
							{
								ExprCollection exprs = new ExprCollection();
								while (_stack.Count > depth)
								{
									expr = (IExpr) _stack.Pop();
									exprs.Insert(0, expr);
								}
								_stack.Push(exprs);
							}
						
			}
			break;
		}
		default:
		{
			throw new NoViableAltException(LT(1), getFilename());
		}
		 }
	}
	
/** [59] **/
	public void enclosedExpr() //throws RecognitionException, TokenStreamException
{
		
		
		match(L_BRACE);
		exprSequence();
		match(R_BRACE);
	}
	
	private void initializeFactory()
	{
	}
	
	public static readonly string[] tokenNames_ = new string[] {
		@"""<0>""",
		@"""EOF""",
		@"""<2>""",
		@"""NULL_TREE_LOOKAHEAD""",
		@"""import""",
		@"""Name""",
		@"""as""",
		@"""COMMA""",
		@"""SLASH""",
		@"""SLASH_SLASH""",
		@"""SLASH_AT""",
		@"""DEREFERENCE""",
		@"""AT""",
		@"""COLON""",
		@"""L_BRACKET""",
		@"""R_BRACKET""",
		@"""DOLLAR""",
		@"""return""",
		@"""for""",
		@"""in""",
		@"""let""",
		@"""SET_EQUAL_TO""",
		@"""where""",
		@"""sortby""",
		@"""L_PAREN""",
		@"""R_PAREN""",
		@"""ascending""",
		@"""descending""",
		@"""or""",
		@"""and""",
		@"""to""",
		@"""NOT_EQUALS""",
		@"""EQUALS""",
		@"""LESS_THAN""",
		@"""GREATER_THAN""",
		@"""LESS_THAN_EQUALS""",
		@"""GREATER_THAN_EQUALS""",
		@"""ni""",
		@"""like""",
		@"""PLUS""",
		@"""MINUS""",
		@"""STAR""",
		@"""div""",
		@"""MOD""",
		@"""null""",
		@"""INLINE""",
		@"""NUMERIC_LITERAL""",
		@"""STRING_LITERAL""",
		@"""if""",
		@"""then""",
		@"""else""",
		@"""LESS_THAN_SLASH""",
		@"""L_BRACE""",
		@"""R_BRACE""",
		@"""NL""",
		@"""CDATA_DATA""",
		@"""LETTER""",
		@"""DIGIT""",
		@"""ESC""",
		@"""HEX_DIGIT""",
		@"""PREDEFINED_ENTITY_REF""",
		@"""WS""",
		@"""SEMICOLON""",
		@"""D_COLON""",
		@"""STAR_COLON""",
		@"""D_EQUALS""",
		@"""NOT_D_EQUALS""",
		@"""BAR""",
		@"""QUOTE""",
		@"""ESCAPED_QUOTE""",
		@"""S_QUOTE""",
		@"""ESCAPED_S_QUOTE""",
		@"""SLASH_GREATER_THAN"""
	};
	
	private static long[] mk_tokenSet_0_()
	{
		long[] data = { 7968L, 0L};
		return data;
	}
	public static readonly BitSet tokenSet_0_ = new BitSet(mk_tokenSet_0_());
	private static long[] mk_tokenSet_1_()
	{
		long[] data = { 10150691328688034L, 0L};
		return data;
	}
	public static readonly BitSet tokenSet_1_ = new BitSet(mk_tokenSet_1_());
	private static long[] mk_tokenSet_2_()
	{
		long[] data = { 10150691328679842L, 0L};
		return data;
	}
	public static readonly BitSet tokenSet_2_ = new BitSet(mk_tokenSet_2_());
	private static long[] mk_tokenSet_3_()
	{
		long[] data = { 73504L, 0L};
		return data;
	}
	public static readonly BitSet tokenSet_3_ = new BitSet(mk_tokenSet_3_());
	private static long[] mk_tokenSet_4_()
	{
		long[] data = { 10150691144138658L, 0L};
		return data;
	}
	public static readonly BitSet tokenSet_4_ = new BitSet(mk_tokenSet_4_());
	private static long[] mk_tokenSet_5_()
	{
		long[] data = { 541166403584L, 0L};
		return data;
	}
	public static readonly BitSet tokenSet_5_ = new BitSet(mk_tokenSet_5_());
	
}
}
