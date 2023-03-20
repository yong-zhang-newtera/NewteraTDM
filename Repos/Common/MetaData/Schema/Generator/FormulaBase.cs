/*
* @(#)FormulaBase.cs
*
* Copyright (c) 2003-2007 Newtera, Inc. All rights reserved.
*
*/
namespace Newtera.Common.MetaData.Schema.Generator
{
	using System;
	using System.Xml;
	using System.Collections.Specialized;

	using Newtera.Common.Wrapper;
    using Newtera.Common.MetaData.Schema;

	/// <summary> 
	/// The base class for formula used by virtual attributes
	/// </summary>
	/// <version> 1.0.0 26 May 2006</version>
	public abstract class FormulaBase : IFormula
	{
		/// <summary>
		/// Initiate an instance of FormulaBase class
		/// </summary>
		public FormulaBase()
		{
		}

		#region IFormula interface implementation

        /// <summary>
        /// Execute the formula to generate a value of the virtual attribute.
        /// </summary>
        /// <param name="instance">A wrapped instance of the current row.</param>
        /// <param name="context">The execution context.</param>
        /// <returns>The result of evaluating a formula.</returns>
        public string Execute(IInstanceWrapper instance, ExecutionContext context)
        {
            return Eval(instance, context);
        }

		#endregion

        /// <summary>
        /// To be overrided by the subclass.
        /// </summary>
        /// <param name="instance">A wrapped instance of the current row.</param>
        /// <param name="context">The execution context.</param>
        /// <returns>The result of evaluating a formula.</returns>
        public abstract string Eval(IInstanceWrapper instance, ExecutionContext context);
	}
}