/*
* @(#)AssemblyProcessor.cs
*
* Copyright (c) 2003 Newtera, Inc. All rights reserved.
*
*/

namespace AppDomainAssemblyLoader
{
	using System;
	using System.Data;
	using System.Reflection;
	using System.Runtime.Remoting;

	using Newtera.ParserGen.ParseTree;
	using Newtera.ParserGen.Converter;

	/// <summary> 
	/// Use an separate AppDomain to load an assembly of text converter,
	/// so that when an AppDomain is unloaded, the assembly that it has loaded
	/// is also unloaded. This allows a dynamically updatable system to be implemented.
	/// </summary>
	/// <version>  	1.0.0 25 Nov 2005 </version>
	/// <author> Yong Zhang </author>
	public class AssemblyProcessor : MarshalByRefObject
	{
		private Assembly _assembly;
		private DataSet _dataSet;
		private IParseTreeNode _parseTree;

		/// <summary>
		/// Instantiate an AssemblyProcessor instance
		/// </summary>
		public AssemblyProcessor()
		{
		}

		/// <summary>
		/// Load the given assembly into an AppDomain
		/// </summary>
		/// <param name="assemblyName"></param>
		public void Load(string assemblyName)
		{
			// load the assembly into an app domain
			_assembly = Assembly.Load(assemblyName);
		}

		/// <summary>
		/// Gets the parse data in form of DataSet
		/// </summary>
		public DataSet ParseDataSet
		{
			get
			{
				return _dataSet;
			}
		}

		/// <summary>
		/// Gets the parse tree
		/// </summary>
		public IParseTreeNode ParseTree
		{
			get
			{
				return _parseTree;
			}
		}

		/// <summary>
		/// Call the data converter to parse a sample file and get the parsed data as
		/// DataSet.
		/// </summary>
		/// <param name="className">The class name.</param>
		/// <param name="sampleFile">The sample file name.</param>
		public void ParseSample(string className, string sampleFile)
		{
			Type classType = _assembly.GetType(className);

			IDataSourceConverter converter = (IDataSourceConverter) Activator.CreateInstance(classType);

			_dataSet = converter.Convert(sampleFile);

			if (converter is ITextParser)
			{
				_parseTree = ((ITextParser) converter).ParseTreeRoot;
			}
		}
	}
}


