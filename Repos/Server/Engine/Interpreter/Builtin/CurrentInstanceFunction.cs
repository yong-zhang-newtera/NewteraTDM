/*
*  @(#)CurrentInstanceFunction.cs
*
*  Copyright (c) 2003 Newtera, Inc. All rights reserved.
*/
namespace Newtera.Server.Engine.Interpreter.Builtin
{
	using System;
	using System.Threading;
	using System.Xml;
	using System.Collections;
	using Newtera.Server.Engine.Interpreter;
	using Newtera.Common.MetaData.Principal;
	using Newtera.Server.Engine.Vdom;
	
	/// <summary> 
	/// Returns the current instance. Used for xacl
	/// </summary>
	/// <version>  1.0 24 Aug 2003</version>
	/// <author>  Yong Zhang</author>
	public class CurrentInstanceFunction : FunctionImpBase, ITraceable
	{
		private VDocument _emptyDoc;
		
		/// <summary>
		/// Initiate an instance of the class. The constructor cannot have any
		/// arguments because it is created by Activator.CreateInstance() method.
		/// </summary>
		public CurrentInstanceFunction() : base()
		{
			_emptyDoc = null;
		}

		/// <summary>
		/// Validate the invoking arguments of a function
		/// </summary>
		/// <param name="arguments">The arguments</param>
		/// <exception cref="InterpreterException">Thrown if the number of arguments mismatched or invalid arguments.</exception>
		public override void CheckArgs(ExprCollection arguments)
		{		
		}

		/// <summary> 
		/// Set the document that owns the instance as context node
		/// </summary>
		public override void Prepare()
		{	
			CustomPrincipal principal = Thread.CurrentPrincipal as CustomPrincipal;
			
			if (principal == null || principal.CurrentInstance == null)
			{
				if (_emptyDoc == null)
				{
					// Create an empty document since there is not a real one
					_emptyDoc = DocumentFactory.Instance.Create();
					XmlElement element = _emptyDoc.CreateElement("Empty");
					_emptyDoc.AppendChild(element);
				}
			}
		}
		
		/// <summary> 
		/// Performs function logic in this method using the arguments.
		/// </summary>
		/// <returns> a Value object</returns>
		public override Value Eval()
		{
			return new XNode(Instance);
		}

		/// <summary> 
		/// Gets the instance for evaluation that contains the instance
		/// </summary>
		private XmlElement Instance
		{
			get
			{
				XmlElement instance;
				CustomPrincipal principal = Thread.CurrentPrincipal as CustomPrincipal;
				
				if (principal != null && principal.CurrentInstance != null)
				{
					// get the current instance from user info object
					instance = principal.CurrentInstance;
				}
				else
				{
					// get the bogus instance from empty doc
					instance =  _emptyDoc.DocumentElement;
				}
				
				return instance;
			}
		}

		#region ITraceable Members

		/// <summary>
		/// Trace the owner document of the expression.
		/// </summary>
		/// <returns>The XNode that points to a Xml Document.</returns>
		public Value TraceDocument()
		{
			CustomPrincipal principal = Thread.CurrentPrincipal as CustomPrincipal;
			VDocument doc;
	
			if (principal != null && principal.CurrentInstance != null)
			{
				doc = (VDocument) principal.CurrentInstance.OwnerDocument;
			}
			else
			{
				doc =  _emptyDoc;
			}

			return new XNode(doc);
		}

		public PathEnumerator GetAbsolutePathEnumerator()
		{
			PathEnumerator enumerator = new PathEnumerator();

			// construct a step for the instance
			Step step = new Step(this.Interpreter, Instance.Name, null, Axis.DESCENDANT);
			enumerator.Append(step);

			return enumerator;
		}

		#endregion
	}
}