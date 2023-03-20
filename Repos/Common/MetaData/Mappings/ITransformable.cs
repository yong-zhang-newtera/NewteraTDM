/*
* @(#)ITransformable.cs
*
* Copyright (c) 2004 Newtera, Inc. All rights reserved.
*
*/
namespace Newtera.Common.MetaData.Mappings
{
	using System;
	using System.Xml;
	using System.Data;
	using System.Reflection;

	using Newtera.Common.MetaData.DataView;
	using Newtera.Common.MetaData.Mappings.Transform;
	using Newtera.Common.MetaData.Mappings.Generator;

	/// <summary>
	/// Represents a interface for the nodes in Mappings package that can transform
	/// a data row from source to destination.
	/// </summary>
	/// <version> 1.0.0 16 Nov 2004</version>
	/// <author>  Yong Zhang </author>
	public interface ITransformable
	{
		/// <summary>
		/// Gets or sets the information indicating whether the script is enabled
		/// for transformation.
		/// </summary>
		/// <value>true if it is enabled, false otherwise.</value>
		bool ScriptEnabled { get; set; }

		/// <summary>
		/// Gets or sets type of the script language
		/// </summary>
		/// <value>One of ScriptLanguage enum values.</value>
		ScriptLanguage ScriptLanguage { get; set; }

		/// <summary>
		/// Gets or sets class type of the script
		/// </summary>
		/// <value>Class type string</value>
		string ClassType { get; set; }

		/// <summary>
		/// Gets or sets a script of transformation.
		/// </summary>
		/// <value> The string representing transform script.</value>
		string Script { get; set; }
		

		/// <summary>
		/// Perform transformation and return a collection of IAttributeSetter instances that set a source
		/// value to destination attribute.
		/// </summary>
		/// <param name="srcDataRow">The DataRow from source</param>
		/// <param name="srcDataView">The DataViewModel for the source class.</param>
		/// <param name="dstDataSet">The destination DataSet</param>
		/// <param name="dstDataView">The DataViewModel for destination class</param>
		/// <param name="rowIndex">The row index of destination row.</param>
		/// <param name="assembly">The assembly contains transformer classes</param>
		/// <returns>A AttributeSetterCollection instance.</returns>
		AttributeSetterCollection DoTransform(DataRow srcDataRow, DataViewModel srcDataView,
			DataSet dstDataSet, DataViewModel dstDataView, int rowIndex, Assembly assembly);
	}
}