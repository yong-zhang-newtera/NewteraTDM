/*
* @(#)GetAttributeValueFunction.cs
*
* Copyright (c) 2003-2008 Newtera, Inc. All rights reserved.
*
*/
namespace Newtera.Server.Engine.Interpreter.Builtin
{
	using System;
	using System.Xml;
    using System.Text;
    using System.Threading;
    using System.Security.Principal;
	using System.Collections;

	using Newtera.Server.Engine.Interpreter;
    using Newtera.Server.UsrMgr;
    using Newtera.Server.DB;
    using Newtera.WFModel;

	/// 
	/// <summary>
	/// Gets value of an attribute of an existing data instance in a specified class of a specified schema,
    /// empty string if such data instance does not exist.
	/// </summary>
	/// <version>  1.0.0 06 May 2008</version>
	public class GetAttributeValueFunction : FunctionImpBase
	{
        private IPrincipal _superUser = null;

		/// <summary>
		/// Initiate an instance of the class. The constructor cannot have any
		/// arguments because it is created by Activator.CreateInstance() method.
		/// </summary>
		public GetAttributeValueFunction() : base()
		{
            CMUserManager userMgr = new CMUserManager();
            _superUser = userMgr.SuperUser;
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
		/// Performs function logic in this method using the arguments.
		/// </summary>
		/// <returns> a Value object</returns>
		public override Value Eval()
		{
            string val = "";

            if (_arguments.Count == 5)
            {
                string schemaName = ((IExpr)_arguments[0]).Eval().ToString();
                string schemaVersion = ((IExpr)_arguments[1]).Eval().ToString();
                string className = ((IExpr)_arguments[2]).Eval().ToString();
                string attributeName = ((IExpr)_arguments[3]).Eval().ToString();
                string objId = ((IExpr)_arguments[4]).Eval().ToString();

                if (!string.IsNullOrEmpty(objId))
                {
                    StringBuilder queryBuilder = new StringBuilder();
                    queryBuilder.Append("for $i in document(\"db://");
                    queryBuilder.Append(schemaName).Append(".xml");
                    if (schemaVersion != null && schemaVersion != "1.0")
                    {
                        queryBuilder.Append("?Version=").Append(schemaVersion);
                    }
                    queryBuilder.Append("\")/");
                    queryBuilder.Append(className).Append("List/").Append(className);
                    queryBuilder.Append(" where $i/@obj_id=\"");
                    queryBuilder.Append(objId);
                    queryBuilder.Append("\" ");
                    queryBuilder.Append("return $i/").Append(attributeName);

                    string query = queryBuilder.ToString();

                    IPrincipal originalPrincipal = Thread.CurrentPrincipal;

                    try
                    {
                        // execute the query as a super user
                        Thread.CurrentPrincipal = _superUser;

                        Interpreter interpreter = new Interpreter();
                        XmlDocument doc = interpreter.Query(query);

                        val = doc.DocumentElement.InnerText;
                    }
                    finally
                    {
                        // attach the original principal to the thread
                        Thread.CurrentPrincipal = originalPrincipal;
                    }
                }
            }

            return new XString(val);
		}
	}
}