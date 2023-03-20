/*
* @(#)PersonalizeVisitor.cs
*
* Copyright (c) 2003 Newtera, Inc. All rights reserved.
*
*/
namespace Newtera.Server.Engine.Sqlbuilder
{
	using System;
	using System.Collections;
	using System.Xml;

	using Newtera.Common.Core;
    using Newtera.Common.MetaData.DataView;
    using Newtera.Server.Engine.Interpreter;
	using Newtera.Server.Engine.Sqlbuilder.Sql;
	using Newtera.Server.Engine.Vdom;
	using Newtera.Common.MetaData;
	using Newtera.Common.MetaData.Schema;
	using Newtera.Common.MetaData.XaclModel;
	
	/// <summary>
	/// A PersonalizeVisitor visitor personalizes a resulting xml document based on the
	/// permission previlege of an user, by hiding the values of attributes and relationships
	/// to which the user does not have read permission.
	/// </summary>
	/// <version>  	1.0.0 23 Jul 2003 </version>
	/// <author> Yong Zhang </author>
	public class PersonalizeVisitor : EntityVisitor
	{
		/* private instance members */
		private XmlDocument _personalizedDoc;
		private Hashtable _entitiesByXmlObject;
		private MetaDataModel _metaData;
		private ArrayList _encryptedElements;
		private ArrayList _removedAttributes;
        private QueryInfo _queryInfo;
        private Hashtable _questionableClassNames;
        private Interpreter _interpreter;
        private Hashtable _dataViewTable;
        private bool _checkReadPermissionOnly;
        private bool _showEncryptedData;
		
		/// <summary>
		/// Initiating an instance of a PersonalizeVisitor class.
		/// </summary>
		/// <param name="personalizedDoc">the xml document to be personalized.</param>
		/// <param name="entityTable">the hash table for associating xml elements with their entities</param>
        /// <param name="metaData">The meta data</param>
        /// <param name="checkReadPermissionOnly">true to check read permission on attributes only, false check both read or write permission on attributes</param>
        /// <param name="showEncryptedData">Indicate whether to show encrypted data in the result</param>
		public PersonalizeVisitor(XmlDocument personalizedDoc, Hashtable entityTable, MetaDataModel metaData,
            QueryInfo queryInfo, bool checkReadPermissionOnly, bool showEncryptedData)
		{
			_personalizedDoc = personalizedDoc;
			_entitiesByXmlObject = entityTable;
			_metaData = metaData;
			_encryptedElements = new ArrayList();
			_removedAttributes = new ArrayList();
            _queryInfo = queryInfo;
            _checkReadPermissionOnly = checkReadPermissionOnly;
            _showEncryptedData = showEncryptedData;
            _interpreter = null;
            _questionableClassNames = new Hashtable();
            _dataViewTable = new Hashtable();

            // build a hash table for fast access
            foreach (string classId in _queryInfo.QuestionableLeafClassIds)
            {
                ClassElement classElement = (ClassElement) _metaData.SchemaModel.FindClassById(classId);

                _questionableClassNames.Add(classElement.Name, classId);
            }
		}
		
		/// <summary>
		/// Visit a ClassEntity object. Check the principal's write, create, and
		/// delete permissions to each instance in the result document
		/// </summary>
		/// <param name="entity">the class entity object to be visited</param>
		public virtual bool VisitClass(ClassEntity entity)
		{			
			// find the corresponding class element in the document, which could be null    
			string classElementName = entity.Name + SQLElement.ELEMENT_CLASS_NAME_SUFFIX;
			
			XmlElement classElement = _personalizedDoc.DocumentElement[classElementName];
			
			if (classElement != null)
			{
				XaclPermissionFlag flags = 0;

				/*
				* Go through the class instance elements one by one and get the user's
				* permissions of writing, creating, and deleting, save the permissions
				* as an attribute of the instance.
				*/
				XmlNodeList instances = classElement.ChildNodes;
                string leafClassName;
				foreach (XmlElement instanceElement in instances)
				{
                    leafClassName = instanceElement.GetAttribute(XMLSchemaInstanceNameSpace.TYPE, XMLSchemaInstanceNameSpace.URI);

                    // if the instance belongs to a leaf class that the current principle may not
                    // have read permission to all the instances, wen have to search the instance
                    // from the leaf class. If the search return nothing, which means the principle
                    // has no permission to the instance, set the permission flag according
                    if (_questionableClassNames[leafClassName] != null && !HasReadPermissionAtLeafClass(instanceElement, leafClassName))
                    {
                        // search on the leaf class to make sure
                        flags = 0; // no any permissions
                    }
                    else
                    {
                        // has read permission to the instance 
                        flags = PermissionChecker.Instance.GetPermissionFlags(_metaData.XaclPolicy,
                            entity.SchemaElement,
                            XaclActionType.Write | XaclActionType.Create | XaclActionType.Delete | XaclActionType.Upload | XaclActionType.Download,
                            instanceElement);
                        flags = flags | XaclPermissionFlag.GrantRead;
                    }
									
					// set the flags as an attribute of the instance element
					instanceElement.SetAttribute(NewteraNameSpace.PERMISSION, System.Convert.ToByte(flags) + "");

                    // attach the element with the entity that describes it                
					_entitiesByXmlObject[instanceElement] = entity;
					
					// attach the obj id attribute with the entity that describes it
					ObjIdEntity objIdEntity = entity.ObjIdEntity;
					XmlAttribute defaultAttribute = instanceElement.GetAttributeNode(objIdEntity.Name);
					_entitiesByXmlObject[defaultAttribute] = objIdEntity;
					
					// attach the cls id attribute with the entity that describes it
					ClsIdEntity clsIdEntity = entity.ClsIdEntity;
					defaultAttribute = instanceElement.GetAttributeNode(clsIdEntity.Name, XMLSchemaInstanceNameSpace.URI);
					_entitiesByXmlObject[defaultAttribute] = clsIdEntity;
				}
			}
			
			return true;
		}
		
		/// <summary>
		/// Visit an AttributeEntity object and hide value of the attribute if the user do
		/// not have read permission.
		/// </summary>
		/// <param name="entity">the attribute entity to be visited</param>
		public bool VisitAttribute(AttributeEntity entity)
		{
			// find the attribute's owner class element in the document   
			string classElementName = entity.BaseClassEntity.Name + SQLElement.ELEMENT_CLASS_NAME_SUFFIX;
			XmlElement classElement = _personalizedDoc.DocumentElement[classElementName];
            string readOnlyAttributes;
            bool hasLocalRules = _metaData.XaclPolicy.HasLocalRules(entity.SchemaModelElement);

			if (classElement != null)
			{
				/*
				* Go through the class instance elements one by one and check the user's
				* permission to the attribute.
				*/
				XmlNodeList instances = classElement.ChildNodes;
				foreach (XmlElement instance in instances)
				{
					XmlElement attributeElement = (XmlElement) instance[entity.Name];
					
					/// Check if it has permission to read the value of this attribute.
					/// If not, replace it encrypted string.
					/// 
					/// Note that read permission check for explicit attribute
					/// references in a XQUery have been performed at ExtendBranchVisitor.cs
					/// during parsing of an XQuery. The permission check here is to guard
					/// against implicit references to attributes
                    if ((!_showEncryptedData && entity.IsEncrypted) || !HasReadPermission(instance))
                    {
                        if (attributeElement != null)
                        {
                            // do not encrypt the value of the attributeElement yet since
                            // it may affect the result of evaluating conditions of
                            // XaclRules. Instead, keep it in a list so that we can
                            // encrypt the values at the end of traverse.
                            _encryptedElements.Add(attributeElement);
                        }
                    }
                    else
                    {
                        IXaclObject xaclObject = entity;

                        // If the attribute has rules defined localy,
                        // check write permission using its schema model element rather than
                        // AttributeEntity because AttributeEntity is associated with
                        // the base class, while its schema model element is associated with the
                        // its owner class which may not be the base class. The xacl rules
                        // defined for an attribute are not passed down to the base class.
                        if (hasLocalRules)
                        {
                            xaclObject = entity.SchemaModelElement;

                            if (!PermissionChecker.Instance.HasPermission(_metaData.XaclPolicy,
                                    xaclObject,
                                    XaclActionType.Read,
                                    instance))
                            {
                                if (attributeElement != null)
                                {
                                    // do not encrypt the value of the attributeElement yet since
                                    // it may affect the result of evaluating conditions of
                                    // XaclRules. Instead, keep it in a list so that we can
                                    // encrypt the values at the end of traverse.
                                    _encryptedElements.Add(attributeElement);
                                }

                                /// So the attribute is readonly too, remember attribute name in the instance element's attribute
                                /// so that it will show the attribute as a ReadOnly field on UI.
                                readOnlyAttributes = instance.GetAttribute(NewteraNameSpace.READ_ONLY);
                                if (string.IsNullOrEmpty(readOnlyAttributes))
                                {
                                    readOnlyAttributes = entity.Name;
                                }
                                else if (!ContainsAttribute(readOnlyAttributes, entity.Name))
                                {
                                    readOnlyAttributes += ";" + entity.Name;
                                }

                                instance.SetAttribute(NewteraNameSpace.READ_ONLY, readOnlyAttributes);
                            }
                            else if (!_checkReadPermissionOnly && !PermissionChecker.Instance.HasPermission(_metaData.XaclPolicy,
                                xaclObject,
                                XaclActionType.Write,
                                instance))
                            {
                                /// Check if it has permission to write the value to this attribute.
                                /// If not, remember attribute name in the instance element's attribute
                                /// so that it will show the attribute as a ReadOnly field on UI.
                                readOnlyAttributes = instance.GetAttribute(NewteraNameSpace.READ_ONLY);
                                if (string.IsNullOrEmpty(readOnlyAttributes))
                                {
                                    readOnlyAttributes = entity.Name;
                                }
                                else if (!ContainsAttribute(readOnlyAttributes, entity.Name))
                                {
                                    readOnlyAttributes += ";" + entity.Name;
                                }

                                instance.SetAttribute(NewteraNameSpace.READ_ONLY, readOnlyAttributes);
                            }
                        }

                        // attach the attribute element with the entity that describes it                
                        _entitiesByXmlObject[attributeElement] = entity;
                    }
				}
			}
			
			return true;
		}
		
		/// <summary>
		/// Visit a RelationshipEntity object, create IDREF or IDREFS attributes to an
		/// instance element.
		/// </summary>
		/// <param name="entity">the relationship entity to be visited.</param>
		public bool VisitRelationship(RelationshipEntity entity)
		{		
			// find the relationship's base class element in the document   
            string classElementName = entity.BaseClassEntity.Name + SQLElement.ELEMENT_CLASS_NAME_SUFFIX;
			XmlElement classElement =  _personalizedDoc.DocumentElement[classElementName];
            string readOnlyAttributes;
            bool hasLocalRules = _metaData.XaclPolicy.HasLocalRules(entity.SchemaModelElement);

			if (classElement != null)
			{
				/*
				* Go through the class instance elements one by one and check the user's
				* permission to the attribute.
				*/
				XmlNodeList instances = classElement.ChildNodes;
				foreach (XmlElement instance in instances)
				{					
                    IXaclObject xaclObject = entity;

                    // If the RelationshipEntity has rules defined localy,
                    // check write permission using its schema model element rather than
                    // RelationshipEntity because RelationshipEntity is associated with
                    // the base class, while its schema model element is associated with the
                    // its owner class which may not be the base class. The xacl rules
                    // defined for an relationship are not passed down to the base class.
                    if (hasLocalRules)
                    {
                        xaclObject = entity.SchemaModelElement;

                        if (!PermissionChecker.Instance.HasPermission(_metaData.XaclPolicy,
                                xaclObject,
                                XaclActionType.Write,
                                instance))
                        {
                            /// Check if it has permission to write the value to this attribute.
                            /// If not, remember attribute name in the instance element's attribute
                            /// so that it will show the attribute as a ReadOnly field on UI.
                            readOnlyAttributes = instance.GetAttribute(NewteraNameSpace.READ_ONLY);
                            if (string.IsNullOrEmpty(readOnlyAttributes))
                            {
                                readOnlyAttributes = entity.Name;
                            }
                            else if (!ContainsAttribute(readOnlyAttributes, entity.Name))
                            {
                                readOnlyAttributes += ";" + entity.Name;
                            }

                            instance.SetAttribute(NewteraNameSpace.READ_ONLY, readOnlyAttributes);
                        }
                    }

                    XmlAttribute relationshipAttribute = instance.GetAttributeNode(entity.Name);

                    if (relationshipAttribute != null)
                    {
					    /// Check if it has permission to read the value of this relationship.
					    /// If not, remove this relationship's value.
					    /// Note that read permission check for explicit attribute
					    /// references in a XQUery have been performed at ExtendBranchVisitor.cs
					    /// during parsing of an XQuery. The permission check here is to guard
					    /// against implicit references to relationships.
					    if (!PermissionChecker.Instance.HasPermission(_metaData.XaclPolicy,
                            xaclObject,
						    XaclActionType.Read,
						    instance))
					    {
						    // do not remove the attribute yet since
						    // it may affect the result of evaluating conditions of
						    // XaclRules. Instead, keep it in a list so that we can
						    // remove it at the end of traverse.
						    _removedAttributes.Add(instance.GetAttributeNode(entity.Name));
					    }
					    else
					    {
						    // attach the element with the entity that describes the element                
						    _entitiesByXmlObject[relationshipAttribute] = entity;
					    }
                    }
			    }
			}
			
			return true;
		}
		
		/// <summary>
		/// Visit a DBEntity object representing an function such as count, avg, min, max, sum.
		/// </summary>
		/// <param name="entity">the function entity to be visited.</param>
		public bool VisitFunction(AggregateFuncEntity entity)
		{		
			if (entity.OwnerClass != null)
			{
				
				// find the relationship's owner class element in the document   
				string classElementName = entity.OwnerClass.Name + SQLElement.ELEMENT_CLASS_NAME_SUFFIX;
				XmlElement classElement = _personalizedDoc.DocumentElement[classElementName];
				
				if (classElement != null)
				{
					XmlNodeList instances = classElement.ChildNodes;
					foreach (XmlElement instance in instances)
					{
						XmlElement functionElement = (XmlElement) instance[entity.Name];
						
						// attach the function element with the entity that describes it 
						if (functionElement != null)
						{
							_entitiesByXmlObject[functionElement] = entity;
						}
					}
				}
			}
			else
			{
				XmlElement functionElement = _personalizedDoc.DocumentElement[entity.Name];
				
				// attach the element with the entity that describes the element 
				if (functionElement != null)
				{
					_entitiesByXmlObject[functionElement] = entity;
				}
			}
			
			return true;
		}
		
		/// <summary>
		/// Visit a SchemaEntity object representing a query schema.
		/// </summary>
		/// <param name="entity">the schema entity to be visited.</param>
		public bool VisitSchema(SchemaEntity entity)
		{
			return true;
		}
		
		/// <summary>
		/// Visit a ScoreEntity object.
		/// </summary>
		/// <param name="entity">the score entity to be visited.</param>
		public bool VisitScore(ScoreEntity entity)
		{		
			// find the relationship's owner class element in the document   
			string classElementName = entity.OwnerClass.Name + SQLElement.ELEMENT_CLASS_NAME_SUFFIX;
			XmlElement classElement = _personalizedDoc.DocumentElement[classElementName];
			
			if (classElement != null)
			{
				XmlNodeList instances = classElement.ChildNodes;
				foreach (XmlElement instance in instances)
				{
					XmlElement functionElement = (XmlElement) instance[entity.Name];
					
					if (functionElement != null)
					{
						// attach the function element with the entity that describes it                
						_entitiesByXmlObject[functionElement] = entity;
					}
				}
			}
			
			return true;
		}

		/// <summary>
		/// Commit the changes to the document as the result of personalization
		/// based on an user's permission.
		/// </summary>
		public void CommitChanges()
		{
			foreach (XmlElement element in _encryptedElements)
			{
				// replace the value with an encrypted value
				element.InnerText = SQLElement.ENCRYPTED_VALUE;
			}

			foreach (XmlAttribute attribute in _removedAttributes)
			{
				XmlElement ownerElement = attribute.OwnerElement;
				ownerElement.RemoveAttributeNode(attribute);
			}
		}

        /// <summary>
        /// Gets information indicating whether an instance has been granted a read permission
        /// </summary>
        /// <param name="instanceElement">The xml instance element</param>
        /// <returns>true if it has read permission, false otherwise</returns>
        private bool HasReadPermission(XmlElement instanceElement)
        {
            bool status = true;

            string permissionStr = instanceElement.GetAttribute(NewteraNameSpace.PERMISSION);

            if (permissionStr == "0")
            {
                status = false;
            }

            return status;
        }

        /// <summary>
        /// Gets the information indicates whether the current principle has a read permission to
        /// an data instance at a given leaf class
        /// </summary>
        /// <param name="instanceElement">The xml instance element</param>
        /// <param name="leafClassName">The leaf class name</param>
        /// <returns>true if the principle has read permission, false otherwise</returns>
        private bool HasReadPermissionAtLeafClass(XmlElement instanceElement, string leafClassName)
        {
            bool status = true;

            DataViewModel instanceDataView = (DataViewModel)_dataViewTable[leafClassName];
            if (instanceDataView == null)
            {
                instanceDataView = _metaData.GetDetailedDataView(leafClassName);
                _dataViewTable[leafClassName] = instanceDataView;
            }

            string query = instanceDataView.GetInstanceQuery(instanceElement.GetAttribute(SQLElement.OBJ_ID));

            if (_interpreter == null)
            {
                _interpreter = new Interpreter();
            }
            else
            {
                _interpreter.Reset();
            }

            XmlDocument doc = _interpreter.Query(query);

            if (doc.DocumentElement.ChildNodes.Count == 0)
            {
                status = false;
            }

            return status;
        }

        private bool ContainsAttribute(string readOnlyAttributesStr, string attributeName)
        {
            bool status = false;

            string[] readOnlyAttributes = readOnlyAttributesStr.Split(';');

            foreach (string readOnlyAttribute in readOnlyAttributes)
            {
                if (readOnlyAttribute == attributeName)
                {
                    status = true;
                    break;
                }
            }

            return status;
        }
	}
}