/*
* @(#)ElementFactory.cs
*
* Copyright (c) 2006 Newtera, Inc. All rights reserved.
*
*/
namespace Newtera.WFModel
{
	using System;
	using System.Xml;

	/// <summary>
	/// A singleton class that creates an instance of IWFModelElement based on a xml element
	/// </summary>
	/// <version>  	1.0.0 8 Dec 2006 </version>
	public class ElementFactory
	{		
		/// <summary>
		/// Singleton's private instance.
		/// </summary>
		private static ElementFactory theFactory;
		
		static ElementFactory()
		{
			theFactory = new ElementFactory();
		}

		/// <summary>
		/// The private constructor.
		/// </summary>
		private ElementFactory()
		{
		}

		/// <summary>
		/// Gets the ElementFactory instance.
		/// </summary>
		/// <returns> The ElementFactory instance.</returns>
		static public ElementFactory Instance
		{
			get
			{
				return theFactory;
			}
		}
		
		/// <summary>
		/// Creates an instance of IWFModelElement type based on the xml element
		/// representing the element.
		/// </summary>
		/// <param name="xmlElement">the xml element.</param>
		/// <returns>A IWFModelElement instance</returns>
		public IWFModelElement Create(XmlElement xmlElement)
		{
			IWFModelElement obj = null;

			if (xmlElement != null)
			{
				string elemntName = xmlElement.Name;

				ElementType type = ConvertStringToType(elemntName);

				switch (type)
				{
					case ElementType.Workflow:
						obj = new WorkflowModel(xmlElement);
						break;
                    case ElementType.Workflows:
                        obj = new WorkflowModelCollection(xmlElement);
                        break;
                    case ElementType.ActivityTrackingRecord:
                        obj = new NewteraActivityTrackingRecord(xmlElement);
                        break;
                    case ElementType.ActivityTrackingRecordCollection:
                        obj = new NewteraActivityTrackingRecordCollection(xmlElement);
                        break;
                    case ElementType.TrackingInstance:
                        obj = new NewteraTrackingWorkflowInstance(xmlElement);
                        break;
                    case ElementType.TrackingInstances:
                        obj = new NewteraTrackingWorkflowInstanceCollection(xmlElement);
                        break;
                    case ElementType.SubjectEntry:
                        obj = new SubjectEntry(xmlElement);
                        break;
                    case ElementType.SubjectEntries:
                        obj = new SubjectEntryCollection(xmlElement);
                        break;
                    case ElementType.SubstituteEntry:
                        obj = new SubstituteEntry(xmlElement);
                        break;
                    case ElementType.SubstituteEntries:
                        obj = new SubstituteEntryCollection(xmlElement);
                        break;
				}
			}

			return obj;
		}

		/// <summary>
		/// Convert a ElementType value to a string
		/// </summary>
		/// <param name="type">One of ElementType values</param>
		/// <returns>The corresponding string</returns>
		internal static string ConvertTypeToString(ElementType type)
		{
			return Enum.GetName(typeof(ElementType), type);
		}

		/// <summary>
		/// Convert a type string to a ElementType value
		/// </summary>
		/// <param name="str">A type string</param>
		/// <returns>One of ElementType values</returns>
		internal static ElementType ConvertStringToType(string str)
		{
			ElementType type;

            try
            {
                type = (ElementType)Enum.Parse(typeof(ElementType), str);
            }
            catch
            {
                type = ElementType.Unknown;
            }

			return type;
		}
	}
}