/*
* @(#)SubscriberNodeFactory.cs
*
* Copyright (c) 2013 Newtera, Inc. All rights reserved.
*
*/
namespace Newtera.Common.MetaData.Subscribers
{
	using System;
	using System.Xml;

	/// <summary>
	/// A singleton class that creates an instance of ISubscriberNode based on a xml element
	/// </summary>
	/// <version>1.0.0 16 Sept 2013 </version>
	public class SubscriberNodeFactory
	{		
		/// <summary>
		/// Singleton's private instance.
		/// </summary>
		private static SubscriberNodeFactory theFactory;
		
		static SubscriberNodeFactory()
		{
			theFactory = new SubscriberNodeFactory();
		}

		/// <summary>
		/// The private constructor.
		/// </summary>
		private SubscriberNodeFactory()
		{
		}

		/// <summary>
		/// Gets the NodeFactory instance.
		/// </summary>
		/// <returns> The NodeFactory instance.</returns>
		static public SubscriberNodeFactory Instance
		{
			get
			{
				return theFactory;
			}
		}
		
		/// <summary>
		/// Creates an instance of ISubscriberNode type based on the xml element
		/// representing the node.
		/// </summary>
		/// <param name="xmlElement">the xml element.</param>
		/// <returns>A ISubscriberNode instance</returns>
        public ISubscriberNode Create(XmlElement xmlElement)
		{
			ISubscriberNode obj = null;

			string elemntName = xmlElement.Name;

			SubscriberNodeType type = ConvertStringToType(elemntName);

			switch (type)
			{
				case SubscriberNodeType.SubscriberGroup:
                    obj = new SubscriberGroup(xmlElement);
					break;
				case SubscriberNodeType.Subscriber:
                    obj = new Subscriber(xmlElement);
					break;
				case SubscriberNodeType.SubscriberGroupCollection:
                    obj = new SubscriberGroupCollection(xmlElement);
					break;
				case SubscriberNodeType.SubscriberCollection:
                    obj = new SubscriberCollection(xmlElement);
					break;
			}
			
			return obj;
		}

		/// <summary>
		/// Convert a SubscriberNodeType value to a string
		/// </summary>
		/// <param name="type">One of SubscriberNodeType values</param>
		/// <returns>The corresponding string</returns>
		internal static string ConvertTypeToString(SubscriberNodeType type)
		{
            return Enum.GetName(typeof(SubscriberNodeType), type);
		}

		/// <summary>
		/// Convert a type string to a SubscriberNodeType value
		/// </summary>
		/// <param name="str">A type string</param>
		/// <returns>One of SubscriberNodeType values</returns>
		internal static SubscriberNodeType ConvertStringToType(string str)
		{
            return (SubscriberNodeType)Enum.Parse(typeof(SubscriberNodeType), str);
		}
	}
}