/*
* @(#)QualifierVisitor.cs
*
* Copyright (c) 2003 Newtera, Inc. All rights reserved.
*
*/
namespace Newtera.Server.Engine.Vdom.Dbimp
{
	using System;
	using System.Collections;
	using Newtera.Server.Engine.Interpreter;
	using Newtera.Server.Engine.Sqlbuilder;
	using Newtera.Common.MetaData.Schema;
	using Newtera.Server.Engine.Sqlbuilder.Sql;
	using Newtera.Server.Engine.Vdom;
	using Newtera.Server.DB;

	/// <summary>
	/// A QualifierVisitor visits an Interpreter IExpr object and creates
	/// a condition for a WHERE clause of SQL statement.
	/// </summary>
	/// <version>  	1.0.0 08 Jul 2003 </version>
	/// <author>  		Yong Zhang  </author>
	public class QualifierVisitor : IExprVisitor
	{
        private const string UNKNOWN_ENUM_VALUE = "unknown";

		// Private members
		private VDocument _document;
		private TreeManager _treeManager;
		private Stack _exprStack;
		private Stack _elementStack;
		private Stack _childCountStack;
		private DBEntity _attributeEntity;
		private DBEntity _functionEntity;
		private Range _range;
		private Hashtable _variables;
		private IDataProvider _dataProvider;
		
		/// <summary>
		/// Initiating an instance of QualifierVisitor class.
		/// </summary>
		/// <param name="treeManager">tree manager object</param>
		/// <param name="doc">The owner document</param>
		/// <param name="dataProvider">data provider</param>
		public QualifierVisitor(TreeManager treeManager, VDocument doc, IDataProvider dataProvider)
		{
			_document = doc;
			_treeManager = treeManager;
			_elementStack = new Stack();
			_exprStack = new Stack();
			_childCountStack = new Stack();
			_attributeEntity = null;
			_functionEntity = null;
			_range = null;
			_variables = null;
			_dataProvider = dataProvider;
		}

		/// <summary>
		/// Gets the SQLElement created for the qualifier.
		/// </summary>
		/// <value> the SQLElement representing a condition </value>
		public SQLElement ConditionElement
		{
			get
			{
				// The last element in the element stack is the composit element for the qualifier
				if (_elementStack.Count == 1)
				{
					return (SQLElement) _elementStack.Peek();
				}
				else if (_elementStack.Count == 0)
				{
					return null;
				}
				else
				{
					throw new VDOMException("Failed to get a Condition element since the stack has " + _elementStack.Count + " elements");
				}
			}
		}

		/// <summary>
		/// Gets the range object
		/// </summary>
		/// <value> the range object</value>
		public Range Range
		{
			get
			{
				return _range;
			}
		}

		/// <summary>
		/// Get the hashtable that associates variables with its SearchValue components
		/// </summary>
		/// <value> the hashtable object </returns>
		public Hashtable Variables
		{
			get
			{
				return _variables;
			}
		}
			
		/// <summary>
		/// Clear the visitor for a new iteration.
		/// </summary>
		public void  Clear()
		{
			_elementStack.Clear();
			_exprStack.Clear();
			_childCountStack.Clear();
			_range = null;
		}
		
		/// <summary>
		/// Visit an IExpr object.
		/// </summary>
		/// <param name="expr">the IExpr object</param>
		public bool Visit(IExpr expr)
		{
			bool visitChildren = true;
			
			int childCount;
			SQLElement element = null;
			string val;
			ExprType type = expr.ExprType;
			SearchValue constant;

			switch (type)
			{
				case ExprType.AND: 
					element = new ANDExpression();
					_exprStack.Push(expr);
					_childCountStack.Push(0);
					break;
				
				case ExprType.OR: 
					element = new ORExpression();
					_exprStack.Push(expr);
					_childCountStack.Push(0);
					break;
				
				case ExprType.EQUALS: 
					element = new Condition(SQLElement.OPT_EQUALS);
					_exprStack.Push(expr);
					_childCountStack.Push(0);
					break;
				
				case ExprType.GT: 
					element = new Condition(SQLElement.OPT_GT);
					_exprStack.Push(expr);
					_childCountStack.Push(0);
					break;
				
				case ExprType.GEQ: 
					element = new Condition(SQLElement.OPT_GEQ);
					_exprStack.Push(expr);
					_childCountStack.Push(0);
					break;

                case ExprType.LIKE:
                    element = new Condition(SQLElement.OPT_LIKE);
                    _exprStack.Push(expr);
                    _childCountStack.Push(0);
                    break;
				
				case ExprType.LT: 
					element = new Condition(SQLElement.OPT_LT);
					_exprStack.Push(expr);
					_childCountStack.Push(0);
					break;
				
				case ExprType.LEQ: 
					element = new Condition(SQLElement.OPT_LEQ);
					_exprStack.Push(expr);
					_childCountStack.Push(0);
					break;
				
				case ExprType.NEQ:
                    element = new Condition(SQLElement.OPT_NEQ, _dataProvider);
					_exprStack.Push(expr);
					_childCountStack.Push(0);
					break;
				
				case ExprType.IN: 
					element = new Condition(SQLElement.OPT_IN);
					_exprStack.Push(expr);
					_childCountStack.Push(0);
					break;
				
				case ExprType.NOTIN: 
					element = new Condition(SQLElement.OPT_NOT_IN);
					_exprStack.Push(expr);
					_childCountStack.Push(0);
					break;
				
				case ExprType.TO: 
					if (_range == null)
					{
						_range = new Range();
					}
					_range.From = ((To) expr).GetFromValue();
					_range.To = ((To) expr).GetToValue();
					
					// Push a bogus condition element (1 = 1) onto the stack
					element = new Condition(SQLElement.OPT_EQUALS);
					constant = new SearchValue("1", Newtera.Common.MetaData.Schema.DataType.Integer, _dataProvider);
					((Condition) element).LeftOperand = constant;
					((Condition) element).RightOperand = constant;
					_exprStack.Push(expr);
					_childCountStack.Push(2);
					visitChildren = false;
					break;

				case ExprType.TRUE: 
					// Push a bogus condition element (1 = 1) onto the stack
					element = new Condition(SQLElement.OPT_EQUALS);
					constant = new SearchValue("1", Newtera.Common.MetaData.Schema.DataType.Integer, _dataProvider);
					((Condition) element).LeftOperand = constant;
					((Condition) element).RightOperand = constant;
					_exprStack.Push(expr);
					_childCountStack.Push(1);
					visitChildren = false;
					break;
				
				case ExprType.NOT: 
					element = new NegateCondition();
					_exprStack.Push(expr);
					_childCountStack.Push(0);
					break;
				
				case ExprType.PATH: 
					PathEnumerator pathEnumerator = ((Path) expr).GetAbsolutePathEnumerator();
					
					/*
					* The path refers to an AttributeEntity object which has been
					* created during the prepareNodes call earlier. Find it so that
					* we can use it to create SQLElements for a condition
					*/
					_attributeEntity = _treeManager.FindEntity(pathEnumerator);
					
					if (_attributeEntity is AttributeEntity)
					{
						AttributeEntity attrEntity = (AttributeEntity) _attributeEntity;
						if (attrEntity.IsMultipleChoice)
						{
							// Apply bitwise AND to the search field with the search value (operand)
							// however, the search value is unknown at this point, zero
							// is used as fake search value, the real search value will be
							// set once it is encountered afterwards.
							element = new BitwiseAndFunc(attrEntity.ColumnName, attrEntity.OwnerClass.Alias, "0", _dataProvider);
						}
						else
						{
							element = new SearchFieldName(attrEntity.ColumnName, attrEntity.OwnerClass.Alias, attrEntity.CaseStyle, _dataProvider);
						}
						element.ClassEntity = attrEntity.OwnerClass; // Reference to its owner class
					}
					else if (_attributeEntity is ObjIdEntity || _attributeEntity is ClsIdEntity)
					{
						element = new SearchFieldName(_attributeEntity.ColumnName, _attributeEntity.OwnerClass.Alias, _dataProvider);
						element.ClassEntity = _attributeEntity.OwnerClass; // Reference to its owner class
					}
					else
					{
						throw new VDOMException("Got an invalid path " + pathEnumerator.ToString() + " for the left operand of a qualifier");
					}
					
					// Increase the child count  of a parent by one
					childCount = (System.Int32) _childCountStack.Pop();
					_childCountStack.Push(childCount + 1);
					
					visitChildren = false;
					break;
				
				// Literal
				case ExprType.LITERAL: 
				// Identifier
				case ExprType.IDENT: 
					if (type == ExprType.IDENT)
					{
						val = "0"; // a fake value temporarily
					}
					else
					{
						val = expr.Eval().ToString();
					}
					
					if (_attributeEntity != null)
					{
						if (_attributeEntity is AttributeEntity)
						{
							AttributeEntity attrEntity = (AttributeEntity) _attributeEntity;
							if (attrEntity.IsEnum)
							{
								if (attrEntity.IsMultipleChoice)
								{
									// convert search value from string representation to integer representation
									val = attrEntity.ConvertToEnumInteger(val).ToString();
									// set the converted search value as a bitwise operand to
									// its corresponding search field sitting on top of stack
									BitwiseAndFunc bitwiseAnd = (BitwiseAndFunc) _elementStack.Peek();
									bitwiseAnd.RightOperand = val;
								}
								else
								{
									// convert from display text to its enum value
									val = attrEntity.ConvertToEnumValue(val);
                                    if (string.IsNullOrEmpty(val))
                                    {
                                        // it's an unknown enum value, replace with unknown
                                        val = UNKNOWN_ENUM_VALUE;
                                    }
								}
							}
                            else if (attrEntity.HasInputMask)
                            {
                                val = attrEntity.ConvertToUnmaskedString(val);
                            }
                            else if (attrEntity.IsEncrypted)
                            {
                                val = attrEntity.ConvertToEncrytedString(val);
                            }

							element = new SearchValue(val, attrEntity.Type, attrEntity.CaseStyle, _dataProvider);
						}
						else if (_attributeEntity is ObjIdEntity || _attributeEntity is ClsIdEntity)
						{
							element = new SearchValue(val, _attributeEntity.Type, _dataProvider);
						}
						
						_attributeEntity = null;
					}
					else if (_functionEntity != null)
					{
						element = new SearchValue(val, _functionEntity.Type, _dataProvider);
					}
					else
					{
						throw new VDOMException("Failed to get an entity object for right operand of the qualifier");
					}
					
					if (type == ExprType.IDENT)
					{
						// add the variable and element entry to a hashtable
						if (_variables == null)
						{
							_variables = new Hashtable();
						}
						_variables[expr] = element;

						// if the value of the variable changes, we have to reload
						// the document, therefore, add a listener in the document to the value
						// change event
						((Ident) expr).AddValueChangeHandler(new ValueChangeEventHandler(_document.ValueChangedHandler));
					}
					
					// Increase the child count of a parent by one                
					childCount = (System.Int32) _childCountStack.Pop();
					_childCountStack.Push(childCount + 1);
					
					visitChildren = false;
					break;
				
				case ExprType.COLLECTION: 
					if (_attributeEntity != null)
					{
						// The sequence set is left side of an IN condition
						if (_attributeEntity is AttributeEntity ||
							_attributeEntity is ObjIdEntity)
						{							
							SearchValue searchValue;
							IEnumerator enumerator = ((ExprCollection) expr).GetEnumerator();
							AttributeEntity attrEntity = _attributeEntity as AttributeEntity;

							element = new SearchValueSet();
                            Value exprValue;
							while (enumerator.MoveNext())
							{
                                exprValue = ((IExpr)enumerator.Current).Eval();
                                if (exprValue.DataType.IsCollection)
                                {
                                    ValueCollection exprValues = exprValue.ToCollection();
                                    foreach (Value value in exprValues)
                                    {
                                        searchValue = GetSearchValue(attrEntity, value.ToString());
                                        if (!string.IsNullOrEmpty(searchValue.Value))
                                            ((SearchValueSet)element).Add(searchValue);
                                    }
                                }
                                else
                                {
                                    searchValue = GetSearchValue(attrEntity, exprValue.ToString());
                                    ((SearchValueSet)element).Add(searchValue);
                                }
							}
							
							_attributeEntity = null;
							
							// Increase the child count of a parent by one                
							childCount = (System.Int32) _childCountStack.Pop();
							_childCountStack.Push(childCount + 1);
							
							visitChildren = false;
						}
						else
						{
							throw new VDOMException("Wrong type of left-side expression encountered for in operator");
						}
					}

					break;

				case ExprType.PARENTHESIZED:

					// The sequence represents a parenthesis-enclosed expression
					element = new EnclosedExpression();
					_exprStack.Push(expr);
					_childCountStack.Push(0);
					
					break;
				
				case ExprType.NULL: 
					element = new NullValue();
					
					// Increase the child count of a parent by one                
					childCount = (System.Int32) _childCountStack.Pop();
					_childCountStack.Push(childCount + 1);
					
					visitChildren = false;
					break;
				
				case ExprType.FUNCTION: 
					if (_attributeEntity != null)
					{
						// The function is at right side of a qualifier
						
						/*
						* evaluate the function to get the returned value. Treat the returned
						* value as an string literal as the right-side of the qualifier
						*/
						Value functionVal = expr.Eval();
                        Literal literal;
                        if (!functionVal.DataType.IsCollection)
                        {
                            /*
                             * Returned value is a single value. Treat the returned
                             * value as an string literal as the right-side of the qualifier
                            */
                            literal = new Literal(expr.Interpreter, functionVal.DataType, functionVal);
                            Visit(literal);
                        }
                        else
                        {
                            // returned value is a collection (For example, GetCurrentRolesFunction()), 
                            // create a parenthesized expression that contains a collection of literals as the right-side of the qualifier
                            ExprCollection exprCollection = new ExprCollection();
                            ValueCollection values = functionVal.ToCollection();
                            foreach (Value aValue in values)
                            {
                                literal = new Literal(expr.Interpreter, aValue.DataType, aValue);
                                exprCollection.Add(literal);
                            }

                            //ParenthesizedExpr parenthesizeExpr = new ParenthesizedExpr(expr.Interpreter, exprCollection);

                            Visit(exprCollection);
                        }
					}
					else
					{
						/*
						* function is at left side of a qualifier, which must be an aggregate
						* function
						*/
						IExpr funcImp = ((FunctionCall) expr).FunctionImp;
						if (funcImp is IDBFunction)
						{
							IDBFunction dbBuiltin = (IDBFunction) funcImp;
							// set the environment to the function
							dbBuiltin.TreeManager = _treeManager;
							dbBuiltin.DataProvider = _dataProvider;
							
							/*
							* each IDBFunction overrides restrict method to build
							* an SQL equivalent element for the function.
							*/
							expr.Restrict();
							element = dbBuiltin.SQLElement;
							_functionEntity = dbBuiltin.FunctionEntity;
						}
						else
						{
							throw new InterpreterException("Function " + ((FunctionCall) expr).Name + " is not allowed as part of condition");
						}
						
						// Increase the child count of a parent by one
						if (_childCountStack.Count > 0)
						{
							childCount = (System.Int32) _childCountStack.Pop();
							_childCountStack.Push(childCount + 1);
						}
					}
					
					visitChildren = false;
					break;

				case ExprType.UMINUS:

					val = expr.Eval().ToString();

					if (_attributeEntity != null && _attributeEntity is AttributeEntity)
					{
						AttributeEntity attrEntity = (AttributeEntity) _attributeEntity;
						element = new SearchValue(val, attrEntity.Type, attrEntity.CaseStyle, _dataProvider);
						
						_attributeEntity = null;
					}
					else if (_functionEntity != null)
					{
						element = new SearchValue(val, _functionEntity.Type, _dataProvider);
					}
					else
					{
						throw new VDOMException("Failed to get an entity object for right operand of the qualifier");
					}

					// Increase the child count of a parent by one                
					childCount = (System.Int32) _childCountStack.Pop();
					_childCountStack.Push(childCount + 1);
					
					visitChildren = false;
					break;
				
				default: 
					throw new VDOMException("Unexpected expression of type:" + type);
				
			}
			
			if (element != null)
			{
				_elementStack.Push(element);
				
				// pop the stack if all children elements for a parent are pushed
				PopStack();
			}
			
			return visitChildren;
		}

        // Get a SQLBuilder SearchValue object
        private SearchValue GetSearchValue(AttributeEntity attrEntity, string val)
        {
            SearchValue searchValue = null;
            if (attrEntity != null)
            {
                if (attrEntity.IsEnum)
                {
                    if (attrEntity.IsMultipleChoice)
                    {
                        // convert search value from string representation to integer representation
                        val = attrEntity.ConvertToEnumInteger(val).ToString();
                    }
                    else
                    {
                        // convert from display text to its enum value
                        val = attrEntity.ConvertToEnumValue(val);
                    }
                }
                else if (attrEntity.HasInputMask)
                {
                    // unmask the string
                    val = attrEntity.ConvertToUnmaskedString(val);
                }
                else if (attrEntity.IsEncrypted)
                {
                    val = attrEntity.ConvertToEncrytedString(val);
                }

                // left parameter represents an AttributeEntity object
                searchValue = new SearchValue(val, attrEntity.Type, attrEntity.CaseStyle, _dataProvider);
            }
            else
            {
                // left parameter represents an ObjIdEntity object
                searchValue = new SearchValue(val, _attributeEntity.Type, _dataProvider);
            }

            return searchValue;
        }
		
		/// <summary>
		/// Pop the stack if all child elements for a parent are pushed.
		/// </summary>
		private void PopStack()
		{
			SQLElement relOperator, leftOperand, rightOperand;
			
			if (_exprStack.Count > 0)
			{
				IExpr parent = (IExpr) _exprStack.Peek();
				int childCount = ((ITraversable) parent).ChildCount;
				
				if (childCount == (System.Int32) _childCountStack.Peek())
				{
					/*
					* All the children element for the current parent have been
					* created. Pop it out from the stack
					*/
					_exprStack.Pop();
					_childCountStack.Pop();
					
					switch (parent.ExprType)
					{
						case ExprType.AND: 
						// And operator
						case ExprType.OR: 
							rightOperand = (SQLElement) _elementStack.Pop();
							leftOperand = (SQLElement) _elementStack.Pop();
							relOperator = (SQLElement) _elementStack.Peek();
							relOperator.Add(leftOperand);
							relOperator.Add(rightOperand);
							break;
						
						case ExprType.PARENTHESIZED: 
							rightOperand = (SQLElement) _elementStack.Pop();
							relOperator = (SQLElement) _elementStack.Peek();
							((EnclosedExpression) relOperator).Expression = rightOperand;
							break;
						
						case ExprType.NOT: 
							rightOperand = (SQLElement) _elementStack.Pop();
							relOperator = (SQLElement) _elementStack.Peek();
							((NegateCondition) relOperator).Condition = rightOperand;
							break;
						
						case ExprType.EQUALS: 
						// = operator
						case ExprType.GT: 
						// > operator
						case ExprType.GEQ: 
						// >= operator
                        case ExprType.LIKE:
                        // like operator
						case ExprType.LT: 
						// < operator
						case ExprType.LEQ: 
						// =< operator
						case ExprType.NEQ: 
						// != operator
						case ExprType.IN: 
						// in operator
						case ExprType.NOTIN: 
							rightOperand = (SQLElement) _elementStack.Pop();
							leftOperand = (SQLElement) _elementStack.Pop();
							relOperator = (SQLElement) _elementStack.Peek();
							((Condition) relOperator).LeftOperand = leftOperand;
							((Condition) relOperator).RightOperand = rightOperand;
							
							if (_functionEntity != null && _functionEntity is AggregateFuncEntity)
							{
								/*
								* this is a condition in which an aggregate function is
								* part of. This condition will not be part of WHERE clause.
								* It is a HAVING clause in function SQL. Pop the condition
								* out of stack, save it in the function entity, and push a
								* true condition to the stack instead.
								*/
								((AggregateFuncEntity) _functionEntity).Condition = (SQLElement) _elementStack.Pop();
								
								_elementStack.Push(TrueCondition);
								
								_functionEntity = null; // reset the variable
							}
							break;
						
						default: 
							break;
						
					}
					
					// increase the child count of the current parent by one
					if (_childCountStack.Count > 0)
					{
						childCount = (System.Int32) _childCountStack.Pop();
						_childCountStack.Push(childCount + 1);
					}
					
					PopStack(); // do it recursively
				}
			}
		}
		
		/// <summary>
		/// Check if the name is a valid name for class list element.
		/// </summary>
		/// <param name="name">the name of the class list element.</param>
		/// <returns> true if it a valid class list name, false otherwise</returns>
		private bool IsValidClassListName(string name)
		{
			bool status = false;
            if (name.EndsWith(SQLElement.ELEMENT_CLASS_NAME_SUFFIX))
            {
                status = true;
            }

			return status;
		}
		
		/// <summary>
		/// Gets a condition that evaluate true.
		/// </summary>
		/// <value> Condition element returns true.</returns>
		private SQLElement TrueCondition
		{
			get
			{
				SQLElement leftOp = new SearchValue("1", Newtera.Common.MetaData.Schema.DataType.Integer, _dataProvider);
				SQLElement rightOp = new SearchValue("1", Newtera.Common.MetaData.Schema.DataType.Integer, _dataProvider);
				
				return new Condition(leftOp, "=", rightOp);
			}
		}
	}
}