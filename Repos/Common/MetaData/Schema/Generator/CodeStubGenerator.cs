/*
* @(#)CodeStubGenerator.cs
*
* Copyright (c) 2003-2007 Newtera, Inc. All rights reserved.
*
*/
namespace Newtera.Common.MetaData.Schema.Generator
{
	using System;
	using System.Xml;
	using System.IO;
	using System.Text;
	using System.CodeDom;
	using System.CodeDom.Compiler;
	using Microsoft.CSharp;
	using Microsoft.VisualBasic;
	using Microsoft.JScript;

	using Newtera.Common.Core;
	using Newtera.Common.MetaData.Schema;
    using Newtera.Common.Wrapper;
    using Newtera.Common.MetaData.DataView;

	/// <summary>
	/// A singleton class that generate code stub for a virtual attribute.
	/// </summary>
	/// <version>1.0.0 26 May 2007 </version>
	public class CodeStubGenerator
	{
		private CodeDomProvider _csharpProvider = null;
		private CodeDomProvider _vbProvider = null;
		private CodeDomProvider _jscriptProvider = null;

		/// <summary>
		/// Singleton's private instance.
		/// </summary>
		private static CodeStubGenerator theGenerator;
		
		static CodeStubGenerator()
		{
			theGenerator = new CodeStubGenerator();
		}

		/// <summary>
		/// The private constructor.
		/// </summary>
		private CodeStubGenerator()
		{
		}

		/// <summary>
		/// Gets the CodeStubGenerator instance.
		/// </summary>
		/// <returns> The CodeStubGenerator instance.</returns>
		static public CodeStubGenerator Instance
		{
			get
			{
				return theGenerator;
			}
		}
		
		/// <summary>
		/// Generate code graph using System.CodeDom type.
		/// </summary>
		/// <param name="className">The class name of code.</param>
		/// <returns>A CodeCompileUnit instance</returns>
		public CodeCompileUnit Generate(string className)
		{
            string baseTypeName = "FormulaBase";
			string theMethodName = "Eval";
            string returnType = "System.String";

			// Create a new CodeCompileUnit to contain the program graph
			CodeCompileUnit compileUnit = new CodeCompileUnit();

			// Declare a new namespace called Newtera.Formula.
            CodeNamespace formulaNS = new CodeNamespace(NewteraNameSpace.FORMULA_NAME_SPACE);

			// Add the new namespace to the compile unit.
            compileUnit.Namespaces.Add(formulaNS);

			// Add the new namespace import for the related namespace.
            formulaNS.Imports.Add(new CodeNamespaceImport("System"));
            formulaNS.Imports.Add(new CodeNamespaceImport("System.Xml"));
            formulaNS.Imports.Add(new CodeNamespaceImport("System.Collections"));
            formulaNS.Imports.Add(new CodeNamespaceImport("Newtera.Common.Wrapper"));
            formulaNS.Imports.Add(new CodeNamespaceImport("Newtera.Common.MetaData.Schema"));
            formulaNS.Imports.Add(new CodeNamespaceImport("Newtera.Common.MetaData.Schema.Generator")); 

			// Declare a new type using the given name.
			CodeTypeDeclaration theClass = new CodeTypeDeclaration(className);

			// set the base type
			if (baseTypeName != null)
			{
				theClass.BaseTypes.Add(baseTypeName);
			}
			
			// Add the new type to the namespace's type collection.
            formulaNS.Types.Add(theClass);
    
			// Defines a override method that performs the actual evaluation
			CodeMemberMethod formulaMethod = new CodeMemberMethod();
            formulaMethod.Attributes = MemberAttributes.Public | MemberAttributes.Override;
            formulaMethod.Name = theMethodName;
            formulaMethod.ReturnType = new CodeTypeReference(returnType);
            // add parameters and return statement
            formulaMethod.Parameters.Add(new CodeParameterDeclarationExpression("IInstanceWrapper", "instance"));
            formulaMethod.Parameters.Add(new CodeParameterDeclarationExpression("ExecutionContext", "context"));
            formulaMethod.Statements.Add(new CodeMethodReturnStatement(new CodeArgumentReferenceExpression("\"\"")));

			// Add the override method to the type's members collection
            theClass.Members.Add(formulaMethod);
			
			return compileUnit;
		}

		/// <summary>
		/// Compile a script of a language type
		/// </summary>
		/// <param name="language">One of the ScriptLanguage enum values</param>
		/// <param name="script">The script</param>
        /// <param name="libPath">The path where to find the referenced assemblies</param>
        /// <returns>CompilerResults object</returns>
		public CompilerResults CompileFromSource(ScriptLanguage language, string script, string libPath)
		{
            CodeDomProvider compiler = this.GetProvider(language);

			//ICodeCompiler compiler = provider.CreateCompiler();

			// Configure a CompilerParameters that links System.dll and 
			// produces a file name based on the specified source file name.
			CompilerParameters cp = new CompilerParameters();
			
			// Indicate that a dll rather than an executable should be generated.
			cp.GenerateExecutable = false;

			// Adds an assembly reference.
			cp.ReferencedAssemblies.Add("System.dll");
			cp.ReferencedAssemblies.Add("System.XML.dll");
			cp.ReferencedAssemblies.Add("System.Data.dll");
			cp.ReferencedAssemblies.Add("Microsoft.JScript.dll");
            string libName = libPath + "Newtera.Common.dll";
            if (!File.Exists(libName))
            {
                // It is in debug mode, get it from the current assembly's code base
                Type type = this.GetType();
                libName = type.Assembly.Location;
            }
            cp.ReferencedAssemblies.Add(libName);

			cp.CompilerOptions = "/t:library ";

			// Sets whether to generate the assembly in memory.
			cp.GenerateInMemory = true;

			// Sets the warning level at which 
			// the compiler should abort compilation
			// if a warning of this level occurrs.
			cp.WarningLevel = 3;

			// Sets whether to treat all warnings as errors.
			cp.TreatWarningsAsErrors = false;
			
			/*
			switch (language)
			{
				case ScriptLanguage.CSharp:
				case ScriptLanguage.JScript:
					cp.CompilerOptions += "/lib:" + libPath + " ";
					break;
				case ScriptLanguage.VBScript:
					cp.CompilerOptions += @"/libpath:" + libPath;
					break;
			}
			*/

			// Invoke compilation.
			CompilerResults cr = compiler.CompileAssemblyFromSource(cp, script);

			// Return the results of compilation.
			return cr;
		}

		/// <summary>
		/// Gets the provider for the selected language
		/// </summary>
		/// <returns>CodeDomProvider instance</returns>
		public CodeDomProvider GetProvider(ScriptLanguage language)
		{
			CodeDomProvider provider;
			switch(language)
			{
				case ScriptLanguage.CSharp:
					if (_csharpProvider == null)
					{
						_csharpProvider = new CSharpCodeProvider();
					}

					provider = _csharpProvider;

					break;
				case ScriptLanguage.VBScript:

					if (_vbProvider == null)
					{
						_vbProvider = new VBCodeProvider();
					}

					provider = _vbProvider;

					break;
				case ScriptLanguage.JScript:
					if (_jscriptProvider == null)
					{
						_jscriptProvider = new JScriptCodeProvider();
					}

					provider = _jscriptProvider;

					break;
				default:
					if (_csharpProvider == null)
					{
						_csharpProvider = new CSharpCodeProvider();
					}

					provider = _csharpProvider;
					break;
			}

			return provider;
		}
	}
}