/*
* @(#)ManyToManyTransformer.cs
*
* Copyright (c) 2003-2004 Newtera, Inc. All rights reserved.
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
	/// <version> 1.0.0 22 Nov 2004</version>
	/// <author>Yong Zhang</author>
	public class ManyToManyTransformer : TransformerBase
	{
		/// <summary>
		/// Initiate an instance of ManyToManyTransformer class
		/// </summary>
		public ManyToManyTransformer() : base()
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
				return NodeType.ManyToManyMapping;
			}
		}

		#endregion

	}
}