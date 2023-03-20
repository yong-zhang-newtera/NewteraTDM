/*
* @(#)Instance.cs
*
* Copyright (c) 2003 Newtera, Inc. All rights reserved.
*
*/
namespace Newtera.Server.Engine.Sqlbuilder
{
	using System;
	using System.Xml;
    using System.Collections.Specialized;

	using Newtera.Common.Core;
	using Newtera.Common.MetaData.Schema;
	using Newtera.Server.Engine.Sqlbuilder.Sql;

	/// <summary>
	/// An Instance object store data for a XML instance in a class. 
	/// </summary>
	/// <version>  	1.0.1 18 Jul 2003</version>
	/// <author> Yong Zhang </author>
	public class Instance
	{
		/* private instance variables*/
		private XmlElement _root = null; // XML element for the instance
		
		/// <summary>
		/// Initiating an instance of Instance class
		/// </summary>
		/// <param name="doc">the xml document holding the instance data as root</param>
		public Instance(XmlDocument doc)
		{
			_root = doc.DocumentElement;
		}
		
		/// <summary>
		/// Initiating an instance of Instance class
		/// </summary>
		/// <param name="doc">the xml element representing the instance data.</param>
		public Instance(XmlElement root)
		{
			_root = root;
		}

		/// <summary>
		/// Gets or sets the object id of the instance data.
		/// </summary>
		/// <value> the object id or null if it does not have an object id.</value>
		public string ObjId
		{
			get
			{
				return _root.GetAttribute(SQLElement.OBJ_ID);
			}
			set
			{
				_root.SetAttribute(SQLElement.OBJ_ID, value);
			}
		}

		/// <summary>
		/// Gets the leaf class name of the instance data
		/// </summary>
		/// <value> the class name or null if it does not have an object id.</value>
		public string ClsName
		{
			get
			{
				return _root.GetAttribute(XMLSchemaInstanceNameSpace.TYPE, XMLSchemaInstanceNameSpace.URI);
			}
		}
				
		/// <summary>
		/// Gets the value of an element
		/// </summary>
		/// <param name="name">Name of the element</param>
		/// <returns> the value of element</returns>
		public string GetValue(string name)
		{
			string val = null;

			XmlElement child = _root[name];

			if (child != null)
			{
				if (child.GetAttribute(XMLSchemaInstanceNameSpace.NIL, XMLSchemaInstanceNameSpace.URI) == "true")
				{
					val = SQLElement.VALUE_NULL;
				}
				else
				{
					val = child.InnerText;
				}
			}

			return val;
		}
		
		/// <summary>
		/// Set the value of an element
		/// </summary>
		/// <param name="name">Name of the element</param>
		/// <param name="value">value of element</param>
		public void SetValue(string name, string val)
		{
			XmlElement child = _root[name];

			if (child != null)
			{
				child.InnerText = val;
			}
		}

        /// <summary>
        /// Gets a collection of element names contained in this instance
        /// </summary>
        /// <returns></returns>
        public StringCollection GetElementNames()
        {
            StringCollection attributeNames = new StringCollection();

            foreach (XmlElement child in _root.ChildNodes)
            {
                attributeNames.Add(child.Name);
            }

            return attributeNames;
        }
		
		/// <summary>
		/// Gets information indicating whether the instance contains primary key
		/// values for a relationship
		/// </summary>
		/// <param name="name">the relationship name.</param>
		/// <returns>true if the instance contains primary key value(s) for the given
		/// relationship.
		/// </returns>
		public bool HasPrimaryKeyValues(string name)
		{
			bool status = false;
			XmlElement relationshipElement = _root[name];

			if (relationshipElement != null)
			{
				if (relationshipElement.ChildNodes.Count > 0)
				{
					status = true;
				}
			}
			
			return status;
		}
		
		/// <summary>
		/// Gets the value of a primary key in a relationship.
		/// </summary>
		/// <param name="relationshipName">the name of relatonship</param>
		/// <param name="keyName">the name of primary key</param>
		/// <returns> the found primary key value, null if none found.</returns>
		public string GetPrimaryKeyValue(string relationshipName, string keyName)
		{
			string keyValue = null;
			
			XmlElement relationshipElement = _root[relationshipName];
			if (relationshipElement != null)
			{
				XmlElement keyElement = relationshipElement[keyName];
				if (keyElement != null)
				{
					keyValue = keyElement.InnerText;
				}
			}
			
			return keyValue;
		}
		
		/// <summary>
		/// Get the id of a referenced object
		/// </summary>
		/// <param name="attributeName">the reference attribute name</param>
		/// <returns> the referenced object id or null if it does not have an object id.</returns>
		public string GetReferencedObjId(string attributeName)
		{
			string val = _root.GetAttribute(attributeName);
			if (val == "")
			{
				val = null;
			}

			return val;
		}
		
		/// <summary>
		/// Set the id of a referenced object.
		/// </summary>
		/// <param name="attributeName">the reference attribute name</param>
		/// <param name="objId">the id of referenced object.</param>
		public void SetReferencedObjId(string attributeName, string objId)
		{
			_root.SetAttribute(attributeName, objId);
		}
		
		/// <summary>
		/// Gets information indicating whether this Instance contains data a class
		/// </summary>
		/// <param name="classEntity">The ClassEntity object</param>
		/// <returns>true if it contains data for the class, false otherwise.</returns>
		public bool ContainsDataFor(ClassEntity classEntity)
		{
			return ContainsDataFor(classEntity, false);
		}

		/// <summary>
		/// Gets information indicating whether this Instance contains data a class
		/// </summary>
		/// <param name="classEntity">The ClassEntity object</param>
		/// <param name="checkOversizeArray">Indicating whether to check the attribute that is an over-sized array.</param>
		/// <returns>true if it contains data for the class, false otherwise.</returns>
		public bool ContainsDataFor(ClassEntity classEntity, bool checkOversizeArray)
		{
			bool hasData = false;
			
			// go through the attribute list
			if (classEntity.HasLocalAttributes())
			{
				DBEntityCollection attributes = classEntity.LocalAttributes;
				foreach (AttributeEntity attribute in attributes)
				{
					if (attribute is ArrayAttributeEntity &&
						((ArrayAttributeEntity) attribute).ArraySize == ArraySizeType.OverSize)
					{
						if (checkOversizeArray)
						{
							hasData = ContainsDataFor(attribute);
						}
					}
					else
					{
						hasData = ContainsDataFor(attribute);
					}

					if (hasData)
					{
						break;
					}
				}
			}
			
			if (!hasData && classEntity.HasLocalRelationships())
			{
				DBEntityCollection relationships = classEntity.LocalRelationships;
				
				// go through relationship list
				for (int i = 0; i < relationships.Count; i++)
				{
					foreach (RelationshipEntity relationship in relationships)
					{
						hasData = ContainsDataFor(relationship);
						if (hasData)
						{
							break;
						}
					}
				}
			}
			
			return hasData;
		}
		
		/// <summary>
		/// Gets information indicating whether this Instance contains data a attribute. 
		/// </summary>
		/// <param name="attributeEntity">The AttributeEntity object</param>
		/// <returns> true if it contains data for the attribute, false otherwise</returns>
		public bool ContainsDataFor(AttributeEntity attributeEntity)
		{
			bool hasData = false;
			
			string val = this.GetValue(attributeEntity.Name);
			if (val != null)
			{
				hasData = true;
			}

			return hasData;
		}
		
		/// <summary>
		/// Gets the information indicating whther the Instance object contains data for the given relationship
		/// </summary>
		/// <param name="relationshipEntity">The RelationshipEntity object</param>
		/// <returns>true if the Instance object contains data for the given relationship</returns>
		public bool ContainsDataFor(RelationshipEntity relationshipEntity)
		{
			bool hasData = false;
			
			/*
			* An instance may contain either an obj_id value or primary key value(s)
			* for a relationship attribute. If the obj_id appears in the instance, it
			* take precedence.
			*/
			string val = GetReferencedObjId(relationshipEntity.Name);
			if (val != null)
			{
				hasData = true;
			}
			else
			{
				XmlElement child = _root[relationshipEntity.Name];
				/*
				* check if instance has a child for the relationship.
				*/
				if (child != null)
				{
					hasData = true;
				}
			}
			
			return hasData;
		}
		
		/// <summary>
		///  Get the information indicating whether an attribute or relationship has null value.
		/// </summary>
		/// <param name="name">the name of the attribute or relationship</param>
		/// <returns>true if the attribute or relationship of the given name has null value,
		/// otherwise, return false. 
		/// </returns>
		public bool IsNullValue(string name)
		{
			bool isNullValue = false;
			XmlElement child = _root[name];
			
			// for example, <Price xsi:nil = true>
			if (child != null && child.GetAttribute(XMLSchemaInstanceNameSpace.NIL, XMLSchemaInstanceNameSpace.URI) != "")
			{
				isNullValue = true;
			}
			
			return isNullValue;
		}

		/// <summary>
		/// Gets a direct child of a xmlElement
		/// </summary>
		/// <param name="parent">the parent xml element</param>
		/// <param name="childName">name of child element</param>
		/// <returns>the matched child element, null if not found</returns>
		private XmlElement GetChild(XmlElement parent, string childName)
		{
			XmlElement found = null;

			foreach (XmlNode child in parent.ChildNodes)
			{
				if (child.Name == childName)
				{
					found = (XmlElement) child;

					break;
				}
			}

			return found;
		}
	}
}