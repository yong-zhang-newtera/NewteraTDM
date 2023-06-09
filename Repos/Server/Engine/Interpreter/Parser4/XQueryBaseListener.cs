//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated by a tool.
//     ANTLR Version: 4.5.1
//
//     Changes to this file may cause incorrect behavior and will be lost if
//     the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

// Generated from XQuery.g4 by ANTLR 4.5.1

// Unreachable code detected
#pragma warning disable 0162
// The variable '...' is assigned but its value is never used
#pragma warning disable 0219
// Missing XML comment for publicly visible type or member '...'
#pragma warning disable 1591


   // global code stuff that will be included in the source file just before the 'XQueryParser' class below
	using System.Collections;
	using Newtera.Server.Engine.Interpreter;
	using Newtera.Server.Engine.Vdom;


using Antlr4.Runtime.Misc;
using IErrorNode = Antlr4.Runtime.Tree.IErrorNode;
using ITerminalNode = Antlr4.Runtime.Tree.ITerminalNode;
using IToken = Antlr4.Runtime.IToken;
using ParserRuleContext = Antlr4.Runtime.ParserRuleContext;

/// <summary>
/// This class provides an empty implementation of <see cref="IXQueryListener"/>,
/// which can be extended to create a listener which only needs to handle a subset
/// of the available methods.
/// </summary>
[System.CodeDom.Compiler.GeneratedCode("ANTLR", "4.5.1")]
[System.CLSCompliant(false)]
public partial class XQueryBaseListener : IXQueryListener {
	/// <summary>
	/// Enter a parse tree produced by <see cref="XQueryParser.start"/>.
	/// <para>The default implementation does nothing.</para>
	/// </summary>
	/// <param name="context">The parse tree.</param>
	public virtual void EnterStart([NotNull] XQueryParser.StartContext context) { }
	/// <summary>
	/// Exit a parse tree produced by <see cref="XQueryParser.start"/>.
	/// <para>The default implementation does nothing.</para>
	/// </summary>
	/// <param name="context">The parse tree.</param>
	public virtual void ExitStart([NotNull] XQueryParser.StartContext context) { }
	/// <summary>
	/// Enter a parse tree produced by <see cref="XQueryParser.queryModule"/>.
	/// <para>The default implementation does nothing.</para>
	/// </summary>
	/// <param name="context">The parse tree.</param>
	public virtual void EnterQueryModule([NotNull] XQueryParser.QueryModuleContext context) { }
	/// <summary>
	/// Exit a parse tree produced by <see cref="XQueryParser.queryModule"/>.
	/// <para>The default implementation does nothing.</para>
	/// </summary>
	/// <param name="context">The parse tree.</param>
	public virtual void ExitQueryModule([NotNull] XQueryParser.QueryModuleContext context) { }
	/// <summary>
	/// Enter a parse tree produced by <see cref="XQueryParser.importAs"/>.
	/// <para>The default implementation does nothing.</para>
	/// </summary>
	/// <param name="context">The parse tree.</param>
	public virtual void EnterImportAs([NotNull] XQueryParser.ImportAsContext context) { }
	/// <summary>
	/// Exit a parse tree produced by <see cref="XQueryParser.importAs"/>.
	/// <para>The default implementation does nothing.</para>
	/// </summary>
	/// <param name="context">The parse tree.</param>
	public virtual void ExitImportAs([NotNull] XQueryParser.ImportAsContext context) { }
	/// <summary>
	/// Enter a parse tree produced by <see cref="XQueryParser.exprSequence"/>.
	/// <para>The default implementation does nothing.</para>
	/// </summary>
	/// <param name="context">The parse tree.</param>
	public virtual void EnterExprSequence([NotNull] XQueryParser.ExprSequenceContext context) { }
	/// <summary>
	/// Exit a parse tree produced by <see cref="XQueryParser.exprSequence"/>.
	/// <para>The default implementation does nothing.</para>
	/// </summary>
	/// <param name="context">The parse tree.</param>
	public virtual void ExitExprSequence([NotNull] XQueryParser.ExprSequenceContext context) { }
	/// <summary>
	/// Enter a parse tree produced by <see cref="XQueryParser.expression"/>.
	/// <para>The default implementation does nothing.</para>
	/// </summary>
	/// <param name="context">The parse tree.</param>
	public virtual void EnterExpression([NotNull] XQueryParser.ExpressionContext context) { }
	/// <summary>
	/// Exit a parse tree produced by <see cref="XQueryParser.expression"/>.
	/// <para>The default implementation does nothing.</para>
	/// </summary>
	/// <param name="context">The parse tree.</param>
	public virtual void ExitExpression([NotNull] XQueryParser.ExpressionContext context) { }
	/// <summary>
	/// Enter a parse tree produced by <see cref="XQueryParser.pathExpression"/>.
	/// <para>The default implementation does nothing.</para>
	/// </summary>
	/// <param name="context">The parse tree.</param>
	public virtual void EnterPathExpression([NotNull] XQueryParser.PathExpressionContext context) { }
	/// <summary>
	/// Exit a parse tree produced by <see cref="XQueryParser.pathExpression"/>.
	/// <para>The default implementation does nothing.</para>
	/// </summary>
	/// <param name="context">The parse tree.</param>
	public virtual void ExitPathExpression([NotNull] XQueryParser.PathExpressionContext context) { }
	/// <summary>
	/// Enter a parse tree produced by <see cref="XQueryParser.path"/>.
	/// <para>The default implementation does nothing.</para>
	/// </summary>
	/// <param name="context">The parse tree.</param>
	public virtual void EnterPath([NotNull] XQueryParser.PathContext context) { }
	/// <summary>
	/// Exit a parse tree produced by <see cref="XQueryParser.path"/>.
	/// <para>The default implementation does nothing.</para>
	/// </summary>
	/// <param name="context">The parse tree.</param>
	public virtual void ExitPath([NotNull] XQueryParser.PathContext context) { }
	/// <summary>
	/// Enter a parse tree produced by <see cref="XQueryParser.xstep"/>.
	/// <para>The default implementation does nothing.</para>
	/// </summary>
	/// <param name="context">The parse tree.</param>
	public virtual void EnterXstep([NotNull] XQueryParser.XstepContext context) { }
	/// <summary>
	/// Exit a parse tree produced by <see cref="XQueryParser.xstep"/>.
	/// <para>The default implementation does nothing.</para>
	/// </summary>
	/// <param name="context">The parse tree.</param>
	public virtual void ExitXstep([NotNull] XQueryParser.XstepContext context) { }
	/// <summary>
	/// Enter a parse tree produced by <see cref="XQueryParser.basicExpression"/>.
	/// <para>The default implementation does nothing.</para>
	/// </summary>
	/// <param name="context">The parse tree.</param>
	public virtual void EnterBasicExpression([NotNull] XQueryParser.BasicExpressionContext context) { }
	/// <summary>
	/// Exit a parse tree produced by <see cref="XQueryParser.basicExpression"/>.
	/// <para>The default implementation does nothing.</para>
	/// </summary>
	/// <param name="context">The parse tree.</param>
	public virtual void ExitBasicExpression([NotNull] XQueryParser.BasicExpressionContext context) { }
	/// <summary>
	/// Enter a parse tree produced by <see cref="XQueryParser.predicate"/>.
	/// <para>The default implementation does nothing.</para>
	/// </summary>
	/// <param name="context">The parse tree.</param>
	public virtual void EnterPredicate([NotNull] XQueryParser.PredicateContext context) { }
	/// <summary>
	/// Exit a parse tree produced by <see cref="XQueryParser.predicate"/>.
	/// <para>The default implementation does nothing.</para>
	/// </summary>
	/// <param name="context">The parse tree.</param>
	public virtual void ExitPredicate([NotNull] XQueryParser.PredicateContext context) { }
	/// <summary>
	/// Enter a parse tree produced by <see cref="XQueryParser.variable"/>.
	/// <para>The default implementation does nothing.</para>
	/// </summary>
	/// <param name="context">The parse tree.</param>
	public virtual void EnterVariable([NotNull] XQueryParser.VariableContext context) { }
	/// <summary>
	/// Exit a parse tree produced by <see cref="XQueryParser.variable"/>.
	/// <para>The default implementation does nothing.</para>
	/// </summary>
	/// <param name="context">The parse tree.</param>
	public virtual void ExitVariable([NotNull] XQueryParser.VariableContext context) { }
	/// <summary>
	/// Enter a parse tree produced by <see cref="XQueryParser.flwrExpr"/>.
	/// <para>The default implementation does nothing.</para>
	/// </summary>
	/// <param name="context">The parse tree.</param>
	public virtual void EnterFlwrExpr([NotNull] XQueryParser.FlwrExprContext context) { }
	/// <summary>
	/// Exit a parse tree produced by <see cref="XQueryParser.flwrExpr"/>.
	/// <para>The default implementation does nothing.</para>
	/// </summary>
	/// <param name="context">The parse tree.</param>
	public virtual void ExitFlwrExpr([NotNull] XQueryParser.FlwrExprContext context) { }
	/// <summary>
	/// Enter a parse tree produced by <see cref="XQueryParser.flClause"/>.
	/// <para>The default implementation does nothing.</para>
	/// </summary>
	/// <param name="context">The parse tree.</param>
	public virtual void EnterFlClause([NotNull] XQueryParser.FlClauseContext context) { }
	/// <summary>
	/// Exit a parse tree produced by <see cref="XQueryParser.flClause"/>.
	/// <para>The default implementation does nothing.</para>
	/// </summary>
	/// <param name="context">The parse tree.</param>
	public virtual void ExitFlClause([NotNull] XQueryParser.FlClauseContext context) { }
	/// <summary>
	/// Enter a parse tree produced by <see cref="XQueryParser.forClause"/>.
	/// <para>The default implementation does nothing.</para>
	/// </summary>
	/// <param name="context">The parse tree.</param>
	public virtual void EnterForClause([NotNull] XQueryParser.ForClauseContext context) { }
	/// <summary>
	/// Exit a parse tree produced by <see cref="XQueryParser.forClause"/>.
	/// <para>The default implementation does nothing.</para>
	/// </summary>
	/// <param name="context">The parse tree.</param>
	public virtual void ExitForClause([NotNull] XQueryParser.ForClauseContext context) { }
	/// <summary>
	/// Enter a parse tree produced by <see cref="XQueryParser.letClause"/>.
	/// <para>The default implementation does nothing.</para>
	/// </summary>
	/// <param name="context">The parse tree.</param>
	public virtual void EnterLetClause([NotNull] XQueryParser.LetClauseContext context) { }
	/// <summary>
	/// Exit a parse tree produced by <see cref="XQueryParser.letClause"/>.
	/// <para>The default implementation does nothing.</para>
	/// </summary>
	/// <param name="context">The parse tree.</param>
	public virtual void ExitLetClause([NotNull] XQueryParser.LetClauseContext context) { }
	/// <summary>
	/// Enter a parse tree produced by <see cref="XQueryParser.whereClause"/>.
	/// <para>The default implementation does nothing.</para>
	/// </summary>
	/// <param name="context">The parse tree.</param>
	public virtual void EnterWhereClause([NotNull] XQueryParser.WhereClauseContext context) { }
	/// <summary>
	/// Exit a parse tree produced by <see cref="XQueryParser.whereClause"/>.
	/// <para>The default implementation does nothing.</para>
	/// </summary>
	/// <param name="context">The parse tree.</param>
	public virtual void ExitWhereClause([NotNull] XQueryParser.WhereClauseContext context) { }
	/// <summary>
	/// Enter a parse tree produced by <see cref="XQueryParser.sortByClause"/>.
	/// <para>The default implementation does nothing.</para>
	/// </summary>
	/// <param name="context">The parse tree.</param>
	public virtual void EnterSortByClause([NotNull] XQueryParser.SortByClauseContext context) { }
	/// <summary>
	/// Exit a parse tree produced by <see cref="XQueryParser.sortByClause"/>.
	/// <para>The default implementation does nothing.</para>
	/// </summary>
	/// <param name="context">The parse tree.</param>
	public virtual void ExitSortByClause([NotNull] XQueryParser.SortByClauseContext context) { }
	/// <summary>
	/// Enter a parse tree produced by <see cref="XQueryParser.sortSpecList"/>.
	/// <para>The default implementation does nothing.</para>
	/// </summary>
	/// <param name="context">The parse tree.</param>
	public virtual void EnterSortSpecList([NotNull] XQueryParser.SortSpecListContext context) { }
	/// <summary>
	/// Exit a parse tree produced by <see cref="XQueryParser.sortSpecList"/>.
	/// <para>The default implementation does nothing.</para>
	/// </summary>
	/// <param name="context">The parse tree.</param>
	public virtual void ExitSortSpecList([NotNull] XQueryParser.SortSpecListContext context) { }
	/// <summary>
	/// Enter a parse tree produced by <see cref="XQueryParser.sortSpec"/>.
	/// <para>The default implementation does nothing.</para>
	/// </summary>
	/// <param name="context">The parse tree.</param>
	public virtual void EnterSortSpec([NotNull] XQueryParser.SortSpecContext context) { }
	/// <summary>
	/// Exit a parse tree produced by <see cref="XQueryParser.sortSpec"/>.
	/// <para>The default implementation does nothing.</para>
	/// </summary>
	/// <param name="context">The parse tree.</param>
	public virtual void ExitSortSpec([NotNull] XQueryParser.SortSpecContext context) { }
	/// <summary>
	/// Enter a parse tree produced by <see cref="XQueryParser.logicalOrExpression"/>.
	/// <para>The default implementation does nothing.</para>
	/// </summary>
	/// <param name="context">The parse tree.</param>
	public virtual void EnterLogicalOrExpression([NotNull] XQueryParser.LogicalOrExpressionContext context) { }
	/// <summary>
	/// Exit a parse tree produced by <see cref="XQueryParser.logicalOrExpression"/>.
	/// <para>The default implementation does nothing.</para>
	/// </summary>
	/// <param name="context">The parse tree.</param>
	public virtual void ExitLogicalOrExpression([NotNull] XQueryParser.LogicalOrExpressionContext context) { }
	/// <summary>
	/// Enter a parse tree produced by <see cref="XQueryParser.logicalAndExpression"/>.
	/// <para>The default implementation does nothing.</para>
	/// </summary>
	/// <param name="context">The parse tree.</param>
	public virtual void EnterLogicalAndExpression([NotNull] XQueryParser.LogicalAndExpressionContext context) { }
	/// <summary>
	/// Exit a parse tree produced by <see cref="XQueryParser.logicalAndExpression"/>.
	/// <para>The default implementation does nothing.</para>
	/// </summary>
	/// <param name="context">The parse tree.</param>
	public virtual void ExitLogicalAndExpression([NotNull] XQueryParser.LogicalAndExpressionContext context) { }
	/// <summary>
	/// Enter a parse tree produced by <see cref="XQueryParser.rangeExpression"/>.
	/// <para>The default implementation does nothing.</para>
	/// </summary>
	/// <param name="context">The parse tree.</param>
	public virtual void EnterRangeExpression([NotNull] XQueryParser.RangeExpressionContext context) { }
	/// <summary>
	/// Exit a parse tree produced by <see cref="XQueryParser.rangeExpression"/>.
	/// <para>The default implementation does nothing.</para>
	/// </summary>
	/// <param name="context">The parse tree.</param>
	public virtual void ExitRangeExpression([NotNull] XQueryParser.RangeExpressionContext context) { }
	/// <summary>
	/// Enter a parse tree produced by <see cref="XQueryParser.equalityExpression"/>.
	/// <para>The default implementation does nothing.</para>
	/// </summary>
	/// <param name="context">The parse tree.</param>
	public virtual void EnterEqualityExpression([NotNull] XQueryParser.EqualityExpressionContext context) { }
	/// <summary>
	/// Exit a parse tree produced by <see cref="XQueryParser.equalityExpression"/>.
	/// <para>The default implementation does nothing.</para>
	/// </summary>
	/// <param name="context">The parse tree.</param>
	public virtual void ExitEqualityExpression([NotNull] XQueryParser.EqualityExpressionContext context) { }
	/// <summary>
	/// Enter a parse tree produced by <see cref="XQueryParser.relationalExpression"/>.
	/// <para>The default implementation does nothing.</para>
	/// </summary>
	/// <param name="context">The parse tree.</param>
	public virtual void EnterRelationalExpression([NotNull] XQueryParser.RelationalExpressionContext context) { }
	/// <summary>
	/// Exit a parse tree produced by <see cref="XQueryParser.relationalExpression"/>.
	/// <para>The default implementation does nothing.</para>
	/// </summary>
	/// <param name="context">The parse tree.</param>
	public virtual void ExitRelationalExpression([NotNull] XQueryParser.RelationalExpressionContext context) { }
	/// <summary>
	/// Enter a parse tree produced by <see cref="XQueryParser.additiveExpression"/>.
	/// <para>The default implementation does nothing.</para>
	/// </summary>
	/// <param name="context">The parse tree.</param>
	public virtual void EnterAdditiveExpression([NotNull] XQueryParser.AdditiveExpressionContext context) { }
	/// <summary>
	/// Exit a parse tree produced by <see cref="XQueryParser.additiveExpression"/>.
	/// <para>The default implementation does nothing.</para>
	/// </summary>
	/// <param name="context">The parse tree.</param>
	public virtual void ExitAdditiveExpression([NotNull] XQueryParser.AdditiveExpressionContext context) { }
	/// <summary>
	/// Enter a parse tree produced by <see cref="XQueryParser.multiplicativeExpression"/>.
	/// <para>The default implementation does nothing.</para>
	/// </summary>
	/// <param name="context">The parse tree.</param>
	public virtual void EnterMultiplicativeExpression([NotNull] XQueryParser.MultiplicativeExpressionContext context) { }
	/// <summary>
	/// Exit a parse tree produced by <see cref="XQueryParser.multiplicativeExpression"/>.
	/// <para>The default implementation does nothing.</para>
	/// </summary>
	/// <param name="context">The parse tree.</param>
	public virtual void ExitMultiplicativeExpression([NotNull] XQueryParser.MultiplicativeExpressionContext context) { }
	/// <summary>
	/// Enter a parse tree produced by <see cref="XQueryParser.unaryExpression"/>.
	/// <para>The default implementation does nothing.</para>
	/// </summary>
	/// <param name="context">The parse tree.</param>
	public virtual void EnterUnaryExpression([NotNull] XQueryParser.UnaryExpressionContext context) { }
	/// <summary>
	/// Exit a parse tree produced by <see cref="XQueryParser.unaryExpression"/>.
	/// <para>The default implementation does nothing.</para>
	/// </summary>
	/// <param name="context">The parse tree.</param>
	public virtual void ExitUnaryExpression([NotNull] XQueryParser.UnaryExpressionContext context) { }
	/// <summary>
	/// Enter a parse tree produced by <see cref="XQueryParser.primitiveExpression"/>.
	/// <para>The default implementation does nothing.</para>
	/// </summary>
	/// <param name="context">The parse tree.</param>
	public virtual void EnterPrimitiveExpression([NotNull] XQueryParser.PrimitiveExpressionContext context) { }
	/// <summary>
	/// Exit a parse tree produced by <see cref="XQueryParser.primitiveExpression"/>.
	/// <para>The default implementation does nothing.</para>
	/// </summary>
	/// <param name="context">The parse tree.</param>
	public virtual void ExitPrimitiveExpression([NotNull] XQueryParser.PrimitiveExpressionContext context) { }
	/// <summary>
	/// Enter a parse tree produced by <see cref="XQueryParser.functionCall"/>.
	/// <para>The default implementation does nothing.</para>
	/// </summary>
	/// <param name="context">The parse tree.</param>
	public virtual void EnterFunctionCall([NotNull] XQueryParser.FunctionCallContext context) { }
	/// <summary>
	/// Exit a parse tree produced by <see cref="XQueryParser.functionCall"/>.
	/// <para>The default implementation does nothing.</para>
	/// </summary>
	/// <param name="context">The parse tree.</param>
	public virtual void ExitFunctionCall([NotNull] XQueryParser.FunctionCallContext context) { }
	/// <summary>
	/// Enter a parse tree produced by <see cref="XQueryParser.literal"/>.
	/// <para>The default implementation does nothing.</para>
	/// </summary>
	/// <param name="context">The parse tree.</param>
	public virtual void EnterLiteral([NotNull] XQueryParser.LiteralContext context) { }
	/// <summary>
	/// Exit a parse tree produced by <see cref="XQueryParser.literal"/>.
	/// <para>The default implementation does nothing.</para>
	/// </summary>
	/// <param name="context">The parse tree.</param>
	public virtual void ExitLiteral([NotNull] XQueryParser.LiteralContext context) { }
	/// <summary>
	/// Enter a parse tree produced by <see cref="XQueryParser.ifExpr"/>.
	/// <para>The default implementation does nothing.</para>
	/// </summary>
	/// <param name="context">The parse tree.</param>
	public virtual void EnterIfExpr([NotNull] XQueryParser.IfExprContext context) { }
	/// <summary>
	/// Exit a parse tree produced by <see cref="XQueryParser.ifExpr"/>.
	/// <para>The default implementation does nothing.</para>
	/// </summary>
	/// <param name="context">The parse tree.</param>
	public virtual void ExitIfExpr([NotNull] XQueryParser.IfExprContext context) { }
	/// <summary>
	/// Enter a parse tree produced by <see cref="XQueryParser.elementConstructor"/>.
	/// <para>The default implementation does nothing.</para>
	/// </summary>
	/// <param name="context">The parse tree.</param>
	public virtual void EnterElementConstructor([NotNull] XQueryParser.ElementConstructorContext context) { }
	/// <summary>
	/// Exit a parse tree produced by <see cref="XQueryParser.elementConstructor"/>.
	/// <para>The default implementation does nothing.</para>
	/// </summary>
	/// <param name="context">The parse tree.</param>
	public virtual void ExitElementConstructor([NotNull] XQueryParser.ElementConstructorContext context) { }
	/// <summary>
	/// Enter a parse tree produced by <see cref="XQueryParser.elementContents"/>.
	/// <para>The default implementation does nothing.</para>
	/// </summary>
	/// <param name="context">The parse tree.</param>
	public virtual void EnterElementContents([NotNull] XQueryParser.ElementContentsContext context) { }
	/// <summary>
	/// Exit a parse tree produced by <see cref="XQueryParser.elementContents"/>.
	/// <para>The default implementation does nothing.</para>
	/// </summary>
	/// <param name="context">The parse tree.</param>
	public virtual void ExitElementContents([NotNull] XQueryParser.ElementContentsContext context) { }
	/// <summary>
	/// Enter a parse tree produced by <see cref="XQueryParser.attribute"/>.
	/// <para>The default implementation does nothing.</para>
	/// </summary>
	/// <param name="context">The parse tree.</param>
	public virtual void EnterAttribute([NotNull] XQueryParser.AttributeContext context) { }
	/// <summary>
	/// Exit a parse tree produced by <see cref="XQueryParser.attribute"/>.
	/// <para>The default implementation does nothing.</para>
	/// </summary>
	/// <param name="context">The parse tree.</param>
	public virtual void ExitAttribute([NotNull] XQueryParser.AttributeContext context) { }
	/// <summary>
	/// Enter a parse tree produced by <see cref="XQueryParser.enclosedExpr"/>.
	/// <para>The default implementation does nothing.</para>
	/// </summary>
	/// <param name="context">The parse tree.</param>
	public virtual void EnterEnclosedExpr([NotNull] XQueryParser.EnclosedExprContext context) { }
	/// <summary>
	/// Exit a parse tree produced by <see cref="XQueryParser.enclosedExpr"/>.
	/// <para>The default implementation does nothing.</para>
	/// </summary>
	/// <param name="context">The parse tree.</param>
	public virtual void ExitEnclosedExpr([NotNull] XQueryParser.EnclosedExprContext context) { }

	/// <inheritdoc/>
	/// <remarks>The default implementation does nothing.</remarks>
	public virtual void EnterEveryRule([NotNull] ParserRuleContext context) { }
	/// <inheritdoc/>
	/// <remarks>The default implementation does nothing.</remarks>
	public virtual void ExitEveryRule([NotNull] ParserRuleContext context) { }
	/// <inheritdoc/>
	/// <remarks>The default implementation does nothing.</remarks>
	public virtual void VisitTerminal([NotNull] ITerminalNode node) { }
	/// <inheritdoc/>
	/// <remarks>The default implementation does nothing.</remarks>
	public virtual void VisitErrorNode([NotNull] IErrorNode node) { }
}
