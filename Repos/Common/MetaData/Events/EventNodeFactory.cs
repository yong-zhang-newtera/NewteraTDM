/*
* @(#)EventNodeFactory.cs
*
* Copyright (c) 2006 Newtera, Inc. All rights reserved.
*
*/
namespace Newtera.Common.MetaData.Events
{
	using System;
	using System.Xml;

	/// <summary>
	/// A singleton class that creates an instance of IEventNode based on a xml element
	/// </summary>
	/// <version>1.0.0 22 Dec 2006 </version>
	public class EventNodeFactory
	{		
		/// <summary>
		/// Singleton's private instance.
		/// </summary>
		private static EventNodeFactory theFactory;
		
		static EventNodeFactory()
		{
			theFactory = new EventNodeFactory();
		}

		/// <summary>
		/// The private constructor.
		/// </summary>
		private EventNodeFactory()
		{
		}

		/// <summary>
		/// Gets the NodeFactory instance.
		/// </summary>
		/// <returns> The NodeFactory instance.</returns>
		static public EventNodeFactory Instance
		{
			get
			{
				return theFactory;
			}
		}
		
		/// <summary>
		/// Creates an instance of IEventNode type based on the xml element
		/// representing the node.
		/// </summary>
		/// <param name="xmlElement">the xml element.</param>
		/// <returns>A IEventNode instance</returns>
		public IEventNode Create(XmlElement xmlElement)
		{
			IEventNode obj = null;

			string elemntName = xmlElement.Name;

			EventNodeType type = ConvertStringToType(elemntName);

			switch (type)
			{
				case EventNodeType.EventGroup:
                    obj = new EventGroup(xmlElement);
					break;
				case EventNodeType.EventDef:
                    obj = new EventDef(xmlElement);
					break;
				case EventNodeType.EventGroupCollection:
                    obj = new EventGroupCollection(xmlElement);
					break;
				case EventNodeType.EventCollection:
                    obj = new EventCollection(xmlElement);
					break;
			}
			
			return obj;
		}

		/// <summary>
		/// Convert a EventNodeType value to a string
		/// </summary>
		/// <param name="type">One of EventNodeType values</param>
		/// <returns>The corresponding string</returns>
		internal static string ConvertTypeToString(EventNodeType type)
		{
            return Enum.GetName(typeof(EventNodeType), type);
		}

		/// <summary>
		/// Convert a type string to a EventNodeType value
		/// </summary>
		/// <param name="str">A type string</param>
		/// <returns>One of EventNodeType values</returns>
		internal static EventNodeType ConvertStringToType(string str)
		{
            return (EventNodeType)Enum.Parse(typeof(EventNodeType), str);
		}
	}
}