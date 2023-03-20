/*
* @(#)GetterFactory.cs
*
* Copyright (c) 2003-2004 Newtera, Inc. All rights reserved.
*
*/
namespace Newtera.Common.MetaData.Mappings.Transform
{
	using System;
	using System.Data;
	using System.Xml;

	using Newtera.Common.MetaData.DataView;

	/// <summary>
	/// A singleton class that creates an instance of IAttributeGetter
	/// </summary>
	/// <version>1.0.0 20 Jan 2005 </version>
	/// <author> Yong Zhang </author>
	public class GetterFactory
	{		
		/// <summary>
		/// Singleton's private instance.
		/// </summary>
		private static GetterFactory theFactory;
		
		static GetterFactory()
		{
			theFactory = new GetterFactory();
		}

		/// <summary>
		/// The private constructor.
		/// </summary>
		private GetterFactory()
		{
		}

		/// <summary>
		/// Gets the GetterFactory instance.
		/// </summary>
		/// <returns> The GetterFactory instance.</returns>
		static public GetterFactory Instance
		{
			get
			{
				return theFactory;
			}
		}
		
		/// <summary>
		/// Creates an instance of IAttributeGetter type
		/// </summary>
		/// <param name="type">One of GetterType enum</param>
		/// <param name="dataRow">The DataRow instance</param>
		/// <param name="attributeName">The attribute name</param>
		/// <param name="dataView">The DataViewModel instance</param>
		/// <returns>A IAttributeGetter instance</returns>
		public IAttributeGetter Create(GetterType type, DataRow dataRow, string attributeName,
			DataViewModel dataView)
		{
			IAttributeGetter obj = null;

			switch (type)
			{
				case GetterType.SimpleAttributeGetter:
					obj = new SimpleAttributeGetter(dataRow, dataView, attributeName);
					break;
				case GetterType.ArrayDataCellGetter:
					int row = 0;
					int col = 0;
					string arrayAttributeName;
					int pos = attributeName.IndexOf("_");
					if (pos > 0)
					{
						arrayAttributeName = attributeName.Substring(0, pos);
						// the index str is rowIndex_colIndex, for example 9_12
						string indexStr = attributeName.Substring(pos + 1);
						pos = indexStr.IndexOf("_");
						if (pos > 0 && (pos + 1) < indexStr.Length)
						{
							row = int.Parse(indexStr.Substring(0, pos));
							col = int.Parse(indexStr.Substring(pos + 1));
						}
					}
					else
					{
						arrayAttributeName = attributeName;
					}
					obj = new ArrayDataCellGetter(dataRow, dataView, arrayAttributeName, row, col);
					break;
				case GetterType.PrimaryKeyGetter:
					throw new MappingException("Mapping source PK is not supported.");
			}
			
			return obj;
		}
	}
}