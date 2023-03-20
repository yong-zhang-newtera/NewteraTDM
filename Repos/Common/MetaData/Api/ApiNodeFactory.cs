/*
* @(#)ApiNodeFactory.cs
*
* Copyright (c) 2015 Newtera, Inc. All rights reserved.
*
*/
namespace Newtera.Common.MetaData.Api
{
	using System;
	using System.Xml;

	/// <summary>
	/// A singleton class that creates an instance of IApiNode based on a xml element
	/// </summary>
	/// <version>1.0.0 16 Oct 2015 </version>
	public class ApiNodeFactory
	{		
		/// <summary>
		/// Singleton's private instance.
		/// </summary>
		private static ApiNodeFactory theFactory;
		
		static ApiNodeFactory()
		{
			theFactory = new ApiNodeFactory();
		}

		/// <summary>
		/// The private constructor.
		/// </summary>
		private ApiNodeFactory()
		{
		}

		/// <summary>
		/// Gets the NodeFactory instance.
		/// </summary>
		/// <returns> The NodeFactory instance.</returns>
		static public ApiNodeFactory Instance
		{
			get
			{
				return theFactory;
			}
		}
		
		/// <summary>
		/// Creates an instance of IApiNode type based on the xml element
		/// representing the node.
		/// </summary>
		/// <param name="xmlElement">the xml element.</param>
		/// <returns>A IApiNode instance</returns>
        public IApiNode Create(XmlElement xmlElement)
		{
			IApiNode obj = null;

			string elemntName = xmlElement.Name;

			ApiNodeType type = ConvertStringToType(elemntName);

			switch (type)
			{
				case ApiNodeType.ApiGroup:
                    obj = new ApiGroup(xmlElement);
					break;
				case ApiNodeType.Api:
                    obj = new Api(xmlElement);
					break;
				case ApiNodeType.ApiGroupCollection:
                    obj = new ApiGroupCollection(xmlElement);
					break;
				case ApiNodeType.ApiCollection:
                    obj = new ApiCollection(xmlElement);
					break;
			}
			
			return obj;
		}

		/// <summary>
		/// Convert a ApiNodeType value to a string
		/// </summary>
		/// <param name="type">One of ApiNodeType values</param>
		/// <returns>The corresponding string</returns>
		internal static string ConvertTypeToString(ApiNodeType type)
		{
            return Enum.GetName(typeof(ApiNodeType), type);
		}

		/// <summary>
		/// Convert a type string to a ApiNodeType value
		/// </summary>
		/// <param name="str">A type string</param>
		/// <returns>One of ApiNodeType values</returns>
		internal static ApiNodeType ConvertStringToType(string str)
		{
            return (ApiNodeType)Enum.Parse(typeof(ApiNodeType), str);
		}
	}
}