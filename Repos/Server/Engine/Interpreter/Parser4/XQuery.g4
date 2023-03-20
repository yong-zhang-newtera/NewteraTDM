// XQuery grammar file as an input to ANTLR parser generator
grammar XQuery;

options {
    language=CSharp;
}

@header
{
   // global code stuff that will be included in the source file just before the 'XQueryParser' class below
	using System.Collections;
	using Newtera.Server.Engine.Interpreter;
	using Newtera.Server.Engine.Vdom;
}

@parser::members
{
   // additional methods and members for the generated 'MyParser' class
	Interpreter _interpreter;
	Stack _stack;
	Stack _contextStack; // Soly used for providing context to the path embedded in a predicate
	
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
}

start :
    queryModule
    {
		IExpr expr = (IExpr) _stack.Pop();
		_stack.Push(new QueryExpr(_interpreter, expr));
    }
;

/** [2] **/
queryModule :
	importAs* expression
;

importAs :
	'import' Name 'as' Name
;

exprSequence
@init{
	int depth = _stack.Count;
	}
:
	expression (COMMA expression)*
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
;

expression : 
//	  (pathExpression)=>pathExpression 
	basicExpression
;
		
pathExpression
@init{
	bool hasSortBy = false;
	}
:
		variable[false] path
		{
			ExprCollection steps = (ExprCollection) _stack.Pop();
			IExpr context = (IExpr) _stack.Pop();
			_stack.Push(new Path(_interpreter, steps, context));
		}
      | functionCall path
        {   
			ExprCollection steps = (ExprCollection) _stack.Pop();
			IExpr context = (IExpr) _stack.Pop();
			_stack.Push(new Path(_interpreter, steps, context));        
        }         
      | path
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
;


path
@init{
	int depth = _stack.Count;
	}
: xstep+
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
;

xstep
@init{
	int axisId = Axis.CHILD;
	string prefix = null;
	Step stepExpr = null;
	}
:
	(   SLASH {axisId = Axis.CHILD;}
	  | SLASH_SLASH {axisId = Axis.DESCENDANT;}
	  | SLASH_AT {axisId = Axis.ATTRIBUTE;}
	  | DEREFERENCE {axisId = Axis.DEREFERENCE;}
	  | AT {axisId = Axis.ATTRIBUTE;} // appears as begining of a path in a predicate
	)?
	(p=Name {prefix = p.getText();} COLON)?
	t=Name {stepExpr = new Step(_interpreter, t.getText(), prefix, axisId); _contextStack.Push(stepExpr); }
	(predicate {stepExpr.Qualifier = (IExpr) _stack.Pop();})?
	{
		_contextStack.Pop();
		_stack.Push(stepExpr);
	}
;

basicExpression :     
		flwrExpr
      | ifExpr	     
      | logicalOrExpression   
;

predicate:
	L_BRACKET expression R_BRACKET
;

variable[bool isDef]
: DOLLAR t = Name
{
	_stack.Push(new Ident(_interpreter, t.getText(), isDef));
}
;
	
flwrExpr
@init{
	IExpr where = null;
	bool hasWhere = false;
	IExpr sortby = null;
	bool hasSortby = false;
	}
:
	flClause (whereClause {hasWhere = true;})? 'return' primitiveExpression (sortByClause { hasSortby = true;})?
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
;

flClause
@init{
	int depth = _stack.Count;
	FLClause fl = new FLClause(_interpreter);
	}
:
	( forClause | letClause)+
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
;

forClause
:
   'for' variable[true] 'in' expression
   {
		IExpr expr = (IExpr) _stack.Pop();
		IExpr var = (IExpr) _stack.Pop();
		_stack.Push(new For(_interpreter, var, expr));
   }
;

letClause
:
	'let' variable[true] SET_EQUAL_TO expression
	{
		IExpr expr = (IExpr) _stack.Pop();
		IExpr var = (IExpr) _stack.Pop();
		_stack.Push(new Let(_interpreter, var, expr));
	}
;

whereClause
:
	'where' expression
;

sortByClause
:
	'sortby' L_PAREN sortSpecList R_PAREN
	{
		ExprCollection sortbySpecs = (ExprCollection) _stack.Pop();
		_stack.Push(new Sortby(_interpreter, sortbySpecs));
    }
;

sortSpecList
@init{
	int depth = _stack.Count;
	ExprCollection sortSpecs = new ExprCollection();
	}
:
	sortSpec (COMMA sortSpec)* 
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
;

sortSpec
@init{
	bool isAscending = true;
	}
:
	pathExpression ('ascending' | 'descending' {isAscending=false;})?
	{
		IExpr path = (IExpr) _stack.Pop();
		_stack.Push(new SortbySpec(_interpreter, path, isAscending));
	}
;

// logical or (or)  (level 11)
logicalOrExpression
@init{
	bool isLogicalOr = false;
	int depth = _stack.Count;
	}
	: logicalAndExpression ('or' {isLogicalOr = true;} logicalAndExpression)*
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
;


// logical and (and)  (level 10)
logicalAndExpression
@init{
	bool isLogicalAnd = false;
	int depth = _stack.Count;
	}
	:	rangeExpression ('and' {isLogicalAnd = true;} rangeExpression)*
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
;
	
rangeExpression
@init{
	bool isRange = false;
	}
:
	equalityExpression ('to' {isRange = true;} equalityExpression )?
	{
		if (isRange)
		{
			IExpr left, right;
			right = (IExpr) _stack.Pop();
			left = (IExpr) _stack.Pop();
			_stack.Push(new To(_interpreter, left, right));
		}		
	}	
;	
	
// equality/inequality (==/!=) (level 6)
equalityExpression
@init{
	bool isEquality = false;
	int type = 0;
	}
	: relationalExpression ((NOT_EQUALS {isEquality = true;} | EQUALS {isEquality = true; type = 1;}) relationalExpression)*	
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
;

// boolean relational expressions (<, >, <=, >=, in, ni, like) (level 5)
relationalExpression
@init{
	bool isRelational = false;
	int type = 0;
	}
	:	additiveExpression
		(	(		LESS_THAN {isRelational = true;}
				|	GREATER_THAN {isRelational = true; type = 1;}
				|	LESS_THAN_EQUALS {isRelational = true; type = 2;}
				|	GREATER_THAN_EQUALS {isRelational = true; type = 3;}
				|	'in' {isRelational = true; type = 4;}
				|	'ni' {isRelational = true; type = 5;}
				|	'like' {isRelational = true; type = 6;}
			)
			additiveExpression
		)*	
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
;
	
// addition/subtraction (level 3)
additiveExpression
@init{
	bool isAdditive = false;
	int type = 0;
	}
	:	multiplicativeExpression ((PLUS {isAdditive = true;} | MINUS {isAdditive = true; type = 1;}) multiplicativeExpression)*
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
	;

// multiplication/division/modulo (level 2)
multiplicativeExpression
@init{
	bool isMultiplicative = false;
	int type = 0;
	}
	: unaryExpression ((STAR {isMultiplicative = true;} | 'div' {isMultiplicative = true; type = 1;} | MOD {isMultiplicative = true; type = 2;}) unaryExpression)*		
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
	;

unaryExpression
	:   MINUS unaryExpression
		{
			IExpr expr = (IExpr) _stack.Pop();
			_stack.Push(new UMinus(_interpreter, expr));
		}
	|	primitiveExpression
	;

// the basic element of an expression
primitiveExpression
	:	literal
	|	'null' { _stack.Push(new Null(_interpreter)); }
	|	t1 = INLINE { _stack.Push(new InlineXml(_interpreter, t1.getText())); }
	|	elementConstructor
	|	functionCall
	|	variable[false]
	|	L_PAREN exprSequence R_PAREN { IExpr expr = (IExpr) _stack.Pop(); _stack.Push(new ParenthesizedExpr(_interpreter, expr)); }
	|   pathExpression
	;

functionCall
@init{
	bool hasArguments = false;
	}
:
	t=Name L_PAREN (exprSequence {hasArguments = true;})? R_PAREN
	{
		ExprCollection arguments = null;
		if (hasArguments)
		{
			arguments = (ExprCollection) _stack.Pop();
		}
		
		_stack.Push(new FunctionCall(_interpreter, t.getText(), arguments));
	}
;

literal
:	
		t1=NUMERIC_LITERAL { _stack.Push(new Literal(_interpreter, t1.getText()));}
	|	t2=STRING_LITERAL { _stack.Push(new Literal(_interpreter, new StringType(), new XString(t2.getText())));}
	;	

/** [17] **/
ifExpr
:
	'if' L_PAREN expression R_PAREN 'then' expression 'else' expression
	{
		IExpr elseClause = (IExpr) _stack.Pop();
		IExpr thenClause = (IExpr) _stack.Pop();
		IExpr condition = (IExpr) _stack.Pop();
		_stack.Push(new If(_interpreter, condition, thenClause, elseClause));
	}
;

/** [52] **/
elementConstructor
@init{
	int depth = _stack.Count;
	ExprCollection attributes = null;
	}
:
	LESS_THAN t=Name (attribute)* GREATER_THAN elementContents LESS_THAN_SLASH Name GREATER_THAN
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
;

elementContents
@init{
	int depth = _stack.Count;
	}
:
		literal
	 | (elementConstructor | enclosedExpr)+
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
;

attribute
:
	t=Name EQUALS literal
	{
		IExpr attrValue = (IExpr) _stack.Pop();
		_stack.Push(new AttributeNode(_interpreter, t.getText(), attrValue));
	}
	|
	enclosedExpr
;

/** [59] **/
enclosedExpr :
	L_BRACE exprSequence R_BRACE
;

//****** Lexer ********************


fragment NL
    :'\r'? '\n' ;

fragment CDATA_DATA
    : 
        (NL
        | .
        )*?
    ;
    
// Character Set
STRING_LITERAL
	:	STRING_QUOTE (ESC|~('"'|'\\'))* STRING_QUOTE
	;

STRING_QUOTE
    : '"' -> skip;
	
NUMERIC_LITERAL:
		((('.' ('0'..'9')+) | (('0'..'9')+('.' ('0'..'9')*)?))('e' | 'E' ('+' | '-')?('0'..'9')+)?)
;

fragment LETTER
	: 'a'..'z' 
	| 'A'..'Z'
;

fragment DIGIT
	:	'0'..'9'
;

Name
	:	('a'..'z'|'A'..'Z'|'_') ('a'..'z'|'A'..'Z'|'_'|'0'..'9')*
	;


INLINE:
		INLINE_LEFT_BRACKET CDATA_DATA INLINE_RIGHT_BRACKET
;

INLINE_LEFT_BRACKET:
     '[[' -> skip;

INLINE_RIGHT_BRACKET:
     ']]' -> skip;

fragment
ESC
	:	'\\'
		(	'n'
		|	'r'
		|	't'
		|	'b'
		|	'f'
		|	'"'
		|	'\''
		|	'\\'
		|	('u')+ HEX_DIGIT HEX_DIGIT HEX_DIGIT HEX_DIGIT
		|	'0'..'3'
			(
				'0'..'7'
				(
					'0'..'7'
				)?
			)?
		|	'4'..'7'
			(
				'0'..'7'
			)?
		)
	;
	
// hexadecimal digit (again, note it's fragment!)
fragment
HEX_DIGIT
	:	('0'..'9'|'A'..'F'|'a'..'f')
	;
		
// ENTITIES
PREDEFINED_ENTITY_REF:
		'&' ('lt' | 'gt' | 'amp' | 'quot' | 'apos') ';';

// Whitespace -- ignored
WS	:	(	' '
		|	'\t'
		|	'\f'
		// handle newlines
		| (	'\r\n'  // Evil DOS
			|	'\r'    // Macintosh
			|	'\n'    // Unix (the right way)
			)
		)+
		-> skip
	;

// interpunction
SEMICOLON:			';';
EQUALS:				'=';
L_PAREN:			'(';
R_PAREN:			')';
COMMA:				',';
L_BRACKET:			'[';
R_BRACKET:			']';
COLON:				':';
D_COLON:			'::';
L_BRACE:			'{';
R_BRACE:			'}';
DEREFERENCE:		'=>';
AT:					'@';
SLASH_AT:			'/@';
STAR:				'*';
SLASH:				'/';
SLASH_SLASH:		'//';
MOD:				'%';
STAR_COLON:			'*:';
NOT_EQUALS:			'!=';
D_EQUALS:			'==';
NOT_D_EQUALS:		'!==';
LESS_THAN:			'<';
GREATER_THAN:		'>';
LESS_THAN_EQUALS:	'<=';
GREATER_THAN_EQUALS:	'>=';
PLUS:				'+';
MINUS:				'-';
BAR:				'|';
SET_EQUAL_TO:		':=';
QUOTE:				'"';
ESCAPED_QUOTE:		'\\\"';
S_QUOTE:			'\'';
SLASH_GREATER_THAN:	'/>';
LESS_THAN_SLASH:	'</';
DOLLAR:				'$';
