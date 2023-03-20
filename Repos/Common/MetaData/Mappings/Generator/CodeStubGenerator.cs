/*
* @(#)CodeStubGenerator.cs
*
* Copyright (c) 2003-2004 Newtera, Inc. All rights reserved.
*
*/
namespace Newtera.Common.MetaData.Mappings.Generator
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
	using Newtera.Common.MetaData.Mappings;

	/// <summary>
	/// A singleton class that generate code stub for a transformer.
	/// </summary>
	/// <version>1.0.0 22 Nov 2004 </version>
	/// <author> Yong Zhang </author>
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
		/// <param name="className">The class name of transformer.</param>
		/// <param name="nodeType">One of NodeTypeEnum values</param>
		/// <returns>A CodeCompileUnit instance</returns>
		public CodeCompileUnit Generate(string className, NodeType nodeType)
		{
			string baseTypeName = null;
			string theMethodName = null;
			string returnType = null;
			switch (nodeType)
			{
				case NodeType.AttributeMapping:
					baseTypeName = "OneToOneTransformer";
					theMethodName = "OneToOneTransform";
					returnType = "System.String";
					break;

				case NodeType.ManyToOneMapping:
					baseTypeName = "ManyToOneTransformer";
					theMethodName = "ManyToOneTransform";
					returnType = "System.String";
					break;

				case NodeType.OneToManyMapping:
					baseTypeName = "OneToManyTransformer";
					theMethodName = "OneToManyTransform";
					break;

				case NodeType.ManyToManyMapping:
					baseTypeName = "ManyToManyTransformer";
					theMethodName = "ManyToManyTransform";
					break;

                case NodeType.SelectRowScript:
                    baseTypeName = "TableRowSelector";
                    theMethodName = "IsRowSelected";
                    returnType = "System.Boolean";
                    break;

                case NodeType.IdentifyRowScript:
                    baseTypeName = "TableRowIdentifier";
                    theMethodName = "IdentifyRow";
                    returnType = "System.Int32";
                    break;
			}

			// Create a new CodeCompileUnit to contain the program graph
			CodeCompileUnit compileUnit = new CodeCompileUnit();

			// Declare a new namespace called Newtera.Transformer.
			CodeNamespace transformer = new CodeNamespace(NewteraNameSpace.TRANSFORMER_NAME_SPACE);
			// Add the new namespace to the compile unit.
			compileUnit.Namespaces.Add(transformer);

			// Add the new namespace import for the related namespace.
			transformer.Imports.Add(new CodeNamespaceImport("System"));            
			transformer.Imports.Add(new CodeNamespaceImport("System.Collections.Specialized"));
            transformer.Imports.Add(new CodeNamespaceImport("Newtera.Common.Core"));
			transformer.Imports.Add(new CodeNamespaceImport("Newtera.Common.MetaData.Mappings.Transform"));            

			// Declare a new type using the given name.
			CodeTypeDeclaration theClass = new CodeTypeDeclaration(className);

			// set the base type
			if (baseTypeName != null)
			{
				theClass.BaseTypes.Add(baseTypeName);
			}
			
			// Add the new type to the namespace's type collection.
			transformer.Types.Add(theClass);
    
			// Defines a override method that performs the actual transformation
			CodeMemberMethod transformMethod = new CodeMemberMethod();
            transformMethod.Attributes = MemberAttributes.Public | MemberAttributes.Override;
			transformMethod.Name = theMethodName;
			if (returnType != null)
			{
				transformMethod.ReturnType = new CodeTypeReference(returnType);
			}

			switch (nodeType)
			{
				case NodeType.AttributeMapping:
					transformMethod.Parameters.Add(new CodeParameterDeclarationExpression("System.String", "src"));
					transformMethod.Statements.Add(new CodeMethodReturnStatement(new CodeArgumentReferenceExpression("src")));
					break;

				case NodeType.ManyToOneMapping:
					transformMethod.Parameters.Add(new CodeParameterDeclarationExpression("NameValueCollection", "srcValues"));
					transformMethod.Statements.Add(new CodeMethodReturnStatement( new CodePrimitiveExpression(null)));
					break;

				case NodeType.OneToManyMapping:
					transformMethod.Parameters.Add(new CodeParameterDeclarationExpression("System.String", "src"));
					transformMethod.Parameters.Add(new CodeParameterDeclarationExpression("NameValueCollection", "dstValues"));
					break;

				case NodeType.ManyToManyMapping:
					transformMethod.Parameters.Add(new CodeParameterDeclarationExpression("NameValueCollection", "srcValues"));
					transformMethod.Parameters.Add(new CodeParameterDeclarationExpression("NameValueCollection", "dstValues"));
					break;

                case NodeType.SelectRowScript:
                    transformMethod.Parameters.Add(new CodeParameterDeclarationExpression("System.Data.DataTable", "srcTable"));
                    transformMethod.Parameters.Add(new CodeParameterDeclarationExpression("System.Int32", "rowIndex"));
                    transformMethod.Statements.Add(new CodeMethodReturnStatement(new CodePrimitiveExpression(true)));
                    break;

                case NodeType.IdentifyRowScript:
                    transformMethod.Parameters.Add(new CodeParameterDeclarationExpression("System.Data.DataTable", "srcTable"));
                    transformMethod.Parameters.Add(new CodeParameterDeclarationExpression("System.Int32", "srcRowIndex"));
                    transformMethod.Parameters.Add(new CodeParameterDeclarationExpression("System.Data.DataTable", "dstTable"));
                    transformMethod.Statements.Add(new CodeMethodReturnStatement(new CodePrimitiveExpression(-1)));
                    break;
			}
			
			// Add the override method to the type's members collection
			theClass.Members.Add(transformMethod);
			
			return compileUnit;
		}

		/// <summary>
		/// Compile a script of a language type
		/// </summary>
		/// <param name="language">One of the ScriptLanguage enum values</param>
		/// <param name="script">The script</param>
		/// <param name="libPath">The lib path where to find refrenced assemblies</param>
		/// <returns></returns>
		public CompilerResults CompileFromSource(ScriptLanguage language, string script, string libPath)
		{
            CodeDomProvider compiler = this.GetProvider(language);

			//ICodeCompiler compiler = provider.CreateCompiler();

			// Configure a CompilerParameters that links System.dll and 
			// produces a file name based on the specified source file name.
			CompilerParameters cp = new CompilerParameters();
			
			// Indicate that a dll rather than an executable should be generated.
			cp.GenerateExecutable = false;

			// Sets filename of the assembly file to generate.
			//cp.OutputAssembly = NewteraNameSpace.TRANSFORMER_ASSEMBLY;

			// Adds an assembly reference.
			cp.ReferencedAssemblies.Add("System.dll");
			cp.ReferencedAssemblies.Add("System.XML.dll");
			cp.ReferencedAssemblies.Add("System.Data.dll");
			cp.ReferencedAssemblies.Add("Microsoft.JScript.dll");
			if (libPath.EndsWith(@"\"))
			{
				cp.ReferencedAssemblies.Add(libPath + "Newtera.Common.dll");
			}
			else
			{
				cp.ReferencedAssemblies.Add(libPath + @"\Newtera.Common.dll");
			}

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