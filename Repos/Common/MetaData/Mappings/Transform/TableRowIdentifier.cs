/*
* @(#)TableRowIdentifier.cs
*
* Copyright (c) 2003-2014 Newtera, Inc. All rights reserved.
*
*/
namespace Newtera.Common.MetaData.Mappings.Transform
{
	using System;
	using System.Xml;

	using Newtera.Common.MetaData.Mappings;

	/// <summary> 
	/// The base class for all transformers
	/// </summary>
	/// <version> 1.0.0 07 Jan 2014</version>
	public class TableRowIdentifier : TransformerBase
	{
		/// <summary>
		/// Initiate an instance of TableRowIdentifier class
		/// </summary>
		public TableRowIdentifier() : base()
		{
		}


		#region ITransformer interface implementation

		/// <summary>
		/// Gets the type of transformer.
		/// </summary>
		/// <value>One of the NodeType enum values</value>
		public override NodeType TransformType 
		{
			get
			{
				return NodeType.IdentifyRowScript;
			}
		}


		#endregion

	}
}