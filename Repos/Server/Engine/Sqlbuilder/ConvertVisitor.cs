/*
* @(#)ConvertVisitor.cs
*
* Copyright (c) 2003 Newtera, Inc. All rights reserved.
*
*/
namespace Newtera.Server.Engine.Sqlbuilder
{
	using System;
	using System.Xml;
	using System.Data;
    using System.Reflection;
	using System.Collections;
    using System.CodeDom.Compiler;
    using System.Data.OracleClient;

	using Newtera.Common.Core;
    using Newtera.Common.Wrapper;
	using Newtera.Server.DB;
	using Newtera.Server.Engine.Sqlbuilder.Sql;
	using Newtera.Common.MetaData;
	using Newtera.Common.MetaData.Schema;
    using Newtera.Common.MetaData.Schema.Generator;
    using Newtera.Common.MetaData.DataView;
	using Newtera.Server.Engine.Vdom;
    using Newtera.Server.Engine.Cache;

	/// <summary>
	/// A ConvertVisitor that converts the two-dimentional search results into a xml doc.
	/// </summary>
	/// <version>  	1.0.0 08 Jul 2003 </version>
	/// <author> Yong Zhang </author>
	class ConvertVisitor : EntityVisitor
	{
		/* private instance members */
		private VDocument _doc;
		private IDataReader _dataReader;
		private Hashtable _classElements;
		private Hashtable _instancesByClass;
		private Hashtable _instancesByObjID;
        private Hashtable _instancesByVirtualAttribute;
        private Hashtable _formulaTable;
        private Hashtable _instanceViewTable;
		private MetaDataModel _metaData;
		private IDataProvider _dataProvider;
        private bool _omitArrayData;
        private bool _delayCalculateVirtualValues;
		
		/// <summary>
		/// Initiating an instance of ConvertVisitor class.
		/// </summary>
		/// <param name="doc">the xml document</param>
		/// <param name="dataReader">the result data reader</param>
		/// <param name="metaData">the meta model</param>
        /// <param name="omitArrayData">Whether to omit array data</param>
		public ConvertVisitor(VDocument doc, IDataReader dataReader,
			MetaDataModel metaData, IDataProvider dataProvider, bool omitArrayData, bool delayCalculateVirtualValues)
		{
			_doc = doc;
			_dataReader = dataReader;
			_classElements = new Hashtable();
			_instancesByClass = new Hashtable();
			_instancesByObjID = new Hashtable();
            _instancesByVirtualAttribute = new Hashtable();
            _instanceViewTable = new Hashtable();
            _formulaTable = new Hashtable();
			_metaData = metaData;
			_dataProvider = dataProvider;
            _omitArrayData = omitArrayData;
            _delayCalculateVirtualValues = delayCalculateVirtualValues;
        }

		/// <summary>
		/// Gets the data reader for result.
		/// </summary>
		/// <value> The data reader</value>
		public IDataReader DataReader
		{
			get
			{
				return _dataReader;
			}
		}
		
		/// <summary>
		/// Clear up the internal states so that it gets ready to visit a new row
		/// in a query result set.
		/// </summary>
		public void  Clear()
		{
			_instancesByClass.Clear();
            _instancesByVirtualAttribute.Clear();
		}
		
		/// <summary>
		/// Visit a ClassEntity object. Create a xml element for the class and a xml
		/// element for an instance
		/// </summary>
		/// <param name="entity">the class entity object to be visited.</param>
		public bool VisitClass(ClassEntity entity)
		{	
			/*
			* Create a parent element for each class if it is the first time the class
			* is visted and the class has attribues or relationships
			*/
			if (!IsClassExist(entity) && (entity.HasInheritedAttributes() || entity.HasInheritedRelationships()))
			{
				XmlElement classElement = _doc.CreateElement(entity.Name + SQLElement.ELEMENT_CLASS_NAME_SUFFIX);

                if (!_doc.HasDuplicatedRootElements && HasDuplicatedChild(_doc.DocumentElement, classElement))
                {
                    _doc.HasDuplicatedRootElements = true;
                }

				// add it as child of root element
				_doc.DocumentElement.AppendChild(classElement);
				
				_classElements[entity] = classElement;
			}
			
			/*
			* Since outer join is used for many-to-one relatioship in a SQL, the result set
			* may have null values for a class. If result set contains instance,
			* create a XML element for an instance of this class only if the class
			* is the bottom class of the instance and the instance hasn't been created
			*/
			if (ContainsInstance(entity) && GetInstanceElement(entity) == null && (entity.HasInheritedAttributes() || entity.HasInheritedRelationships()))
			{
				
				string objId, clsId, attachments;
				
				XmlElement classElement = GetClassElement(entity);
				
				// instance element has the same name as the class name
				XmlElement instanceElement = _doc.CreateElement(entity.Name);
								
				// add instance element as child of class element
				classElement.AppendChild(instanceElement);
				
				try
				{
					/*
					* Create and add xml attributes for obj_id & cls_id to the instance element.
					* each class entity keeps objIdEntity & clsIdEntity objects.
					*/
					ObjIdEntity objIdEntity = entity.ObjIdEntity;
					objId = System.Convert.ToString(_dataReader.GetValue(objIdEntity.ColumnIndex - 1));
					XmlAttribute defaultAttribute = _doc.CreateAttribute(objIdEntity.Name);
					defaultAttribute.Value = objId;
					instanceElement.SetAttributeNode(defaultAttribute);
					
					ClsIdEntity clsIdEntity = entity.ClsIdEntity;
					clsId = System.Convert.ToString(_dataReader.GetValue(clsIdEntity.ColumnIndex - 1));
					clsId = ConvertToClassName(clsId);
					defaultAttribute = _doc.CreateAttribute(XMLSchemaInstanceNameSpace.PREFIX, clsIdEntity.Name, XMLSchemaInstanceNameSpace.URI);
					defaultAttribute.Value = clsId;
					instanceElement.SetAttributeNode(defaultAttribute);					
					
					AttachmentEntity attachmentEntity = entity.AttachmentEntity;
					attachments = System.Convert.ToString(_dataReader.GetValue(attachmentEntity.ColumnIndex - 1));
					if (attachments == null)
					{
						attachments = "0";
					}

					// permission attribute is not set at this point, it will be
					// set by PersonizationVisitor later

					defaultAttribute = _doc.CreateAttribute(attachmentEntity.Name);
					defaultAttribute.Value = attachments;
					instanceElement.SetAttributeNode(defaultAttribute);
				}
				catch (Exception e)
				{
					throw new SQLBuilderException("Failed to get value from query result", e);
				}
				
				/*
				* Because a row in query result set may contain values for multiple
				* instances in different classes. we keep the instance element in
				* a hash table to make it easy to add children elements for attribute
				* values.
				*/
				_instancesByClass[entity] = instanceElement;
				
				/*
				* put the instance in a hash table by its obj id to avoid to create
				* mutiple copies of the same instance.
				*/
				_instancesByObjID[objId] = instanceElement;
			}
			
			return true;
		}
		
		/// <summary>
		/// Visit an AttributeEntity object and create an XML element for the attribute
		/// value.
		/// </summary>
		/// <param name="entity">the attribute entity to be visited.</param>
		public bool VisitAttribute(AttributeEntity entity)
		{
			try
			{
				/*
				* The result set may not have an instance for the attribute owner class.
				*/
				XmlElement instanceElement = GetInstanceElement(entity.OwnerClass);

				if (instanceElement != null)
				{					
					XmlElement attributeElement = instanceElement[entity.Name];
					if (attributeElement == null)
					{
						attributeElement =  _doc.CreateElement(entity.Name);
						string val;

                        if (entity is SimpleAttributeEntity)
                        {
                            if (entity.IsHistoryEdit || entity.IsRichText)
                            {
                                // the value of history edit is stored in clob column,
                                // read the value using a ClobDAO
                                IClobDAO clobDAO = ClobDAOFactory.Instance.Create(_dataProvider);
                                val = clobDAO.ReadClobAsText(_dataReader, entity.ColumnIndex - 1);

                                if (!string.IsNullOrEmpty(val))
                                {
                                    attributeElement.InnerText = val;
                                }
                                else
                                {
                                    attributeElement.InnerText = "";
                                }
                            }
                            else if (entity.IsEnum)
                            {
                                if (entity.IsMultipleChoice)
                                {
                                    // we need to convert an integer whose bits representing
                                    // multiple enum values to a concatenated enum string
                                    int dbVal = 0;
                                    if (!_dataReader.IsDBNull(entity.ColumnIndex - 1))
                                    {
                                        try
                                        {
                                            dbVal = _dataReader.GetInt32(entity.ColumnIndex - 1);
                                        }
                                        catch (Exception)
                                        {
                                            // The existing value may not be an integer, therefore,
                                            // set it to 0
                                            dbVal = 0;
                                        }
                                    }

                                    val = entity.ConvertToEnumString(dbVal);
                                    attributeElement.InnerText = val;
                                }
                                else
                                {
                                    // convert the enum value to the display text
                                    val = null;
                                    if (!_dataReader.IsDBNull(entity.ColumnIndex - 1))
                                    {
                                        val = System.Convert.ToString(GetDBValue(entity.ColumnIndex - 1));
                                    }

                                    if (val != null && val.Length > 0)
                                    {
                                        try
                                        {
                                            attributeElement.InnerText = entity.ConvertToEnumDisplayText(val);
                                        }
                                        catch (MissingFieldException)
                                        {
                                            // check if the enum value is one of the user name
                                            string[] userData = UserDataCache.Instance.GetUserData(val);
                                            if (userData != null)
                                            {
                                                // val is user name, convert it to display name
                                                attributeElement.InnerText = GetUserDisplayText(val, userData);
                                            }
                                            else
                                            {
                                                attributeElement.InnerText = val;
                                            }
                                        }
                                    }
                                    else
                                    {
                                        attributeElement.InnerText = "";
                                    }
                                }
                            }
                            else if (entity.HasInputMask)
                            {
                                // convert a string to the masked string
                                val = null;
                                if (!_dataReader.IsDBNull(entity.ColumnIndex - 1))
                                {
                                    val = System.Convert.ToString(GetDBValue(entity.ColumnIndex - 1));
                                }

                                if (!string.IsNullOrEmpty(val))
                                {
                                    attributeElement.InnerText = entity.ConvertToMaskedString(val);
                                }
                                else
                                {
                                    attributeElement.InnerText = "";
                                }
                            }
                            else if (entity.IsEncrypted)
                            {
                                // decrypt a string
                                val = null;
                                if (!_dataReader.IsDBNull(entity.ColumnIndex - 1))
                                {
                                    val = System.Convert.ToString(GetDBValue(entity.ColumnIndex - 1));
                                }

                                if (!string.IsNullOrEmpty(val))
                                {
                                    attributeElement.InnerText = entity.ConvertToDecryptedString(val);
                                }
                                else
                                {
                                    attributeElement.InnerText = "";
                                }
                            }
                            else if (entity.HasDisplayFormat)
                            {
                                // format the value
                                val = null;
                                if (!_dataReader.IsDBNull(entity.ColumnIndex - 1))
                                {
                                    val = System.Convert.ToString(GetDBValue(entity.ColumnIndex - 1));
                                }

                                if (!string.IsNullOrEmpty(val))
                                {
                                    attributeElement.InnerText = entity.FormatValue(val); 
  
                                }
                                else
                                {
                                    attributeElement.InnerText = "";
                                }
                            }
                            else
                            {
                                val = null;
                                if (!_dataReader.IsDBNull(entity.ColumnIndex - 1))
                                {
                                    val = System.Convert.ToString(GetDBValue(entity.ColumnIndex - 1));
                                }

                                if (val != null && val.Length > 0)
                                {
                                    // convert data base value to its localized version based on current user culture
                                    attributeElement.InnerText = ToLocalizedValue(entity, val);
                                }
                                else
                                {
                                    attributeElement.InnerText = "";
                                }
                            }
						}
						else if (entity is ArrayAttributeEntity)
						{
							val = "";
							if (entity.IsReferenced && !_omitArrayData)
							{
								if (!_dataReader.IsDBNull(entity.ColumnIndex - 1))
								{
									if (((ArrayAttributeEntity) entity).ArraySize == ArraySizeType.OverSize)
									{
										// the array value is stored in clob column,
										// read the value using a ClobDAO
										IClobDAO clobDAO = ClobDAOFactory.Instance.Create(_dataProvider);
										val = clobDAO.ReadClobAsText(_dataReader, entity.ColumnIndex - 1);
									}
									else
									{
										// array data is stored in a text column
                                        val = System.Convert.ToString(GetDBValue(entity.ColumnIndex - 1));
									}
								}
							}

							attributeElement.InnerText = val;
						}
                        else if (entity is VirtualAttributeEntity)
                        {
                            // value of a VirtualAttribute is generated using a program that
                            // user has defined. The program may reference values of other
                            // attributes of this instance. Therefore, we postpone calling the
                            // program until values of all attributes have been converted
                            this._instancesByVirtualAttribute[entity] = instanceElement;
                        }
                        else
                        {
                            // default
                            val = null;
                            if (!_dataReader.IsDBNull(entity.ColumnIndex - 1))
                            {
                                val = System.Convert.ToString(GetDBValue(entity.ColumnIndex - 1));
                            }

                            if (val != null && val.Length > 0)
                            {
                                // convert data base value to its localized version based on current user culture
                                attributeElement.InnerText = ToLocalizedValue(entity, val);
                            }
                            else
                            {
                                attributeElement.InnerText = "";
                            }
                        }
						
						instanceElement.AppendChild(attributeElement);
					}
				}
			}
			catch (Exception e)
			{
				throw new SQLBuilderException("Failed to get a value for column " + entity.Name + " of the class " + entity.OwnerClass.Name, e);
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
			try
			{
				/*
				* The result set may not have an instance for the attribute owner class.
				*/
				XmlElement instanceElement = GetInstanceElement(entity.OwnerClass);

				if (instanceElement != null)
				{
					XmlAttribute relationshipAttribute;
					if (entity.Direction == RelationshipDirection.Forward)
					{
						/*
						* The attribute for the relationship could have been created when processing
						* a previous row in the result set
						*/
						relationshipAttribute = instanceElement.GetAttributeNode(entity.Name);
						if (relationshipAttribute == null)
						{
							// Add IDREF attribute to the instance
							string referencedId = null;
							if (!_dataReader.IsDBNull(entity.ColumnIndex - 1))
							{
                                referencedId = System.Convert.ToString(GetDBValue(entity.ColumnIndex - 1));
							}

							if (referencedId != null)
							{
								/*
								* if the referenced id is null, it doesn't appear at all
								*/
								relationshipAttribute = _doc.CreateAttribute(entity.Name);
								relationshipAttribute.Value = referencedId;
								instanceElement.SetAttributeNode(relationshipAttribute);
							}
						}
					}
					else
					{
						/*
						* this backward relationship could represent one-to-many
						* relationship, hence an attribute of IDREFS may have already
						* created when visiting a row previously.
						*/
						relationshipAttribute = instanceElement.GetAttributeNode(entity.Name);
						
						/*
						* Find the objetc id of referenced instance which may not appear in
						* the result set.
						*/
						int objIdIndex = entity.LinkedClass.ObjIdEntity.ColumnIndex;
						if (objIdIndex > 0)
						{
							// object id of referenced instance exists in result set                                
							string referencedObjId = null;
							if (!_dataReader.IsDBNull(objIdIndex - 1))
							{
                                referencedObjId = System.Convert.ToString(GetDBValue(objIdIndex - 1));
							}

							if (referencedObjId != null)
							{
								if (relationshipAttribute == null)
								{
									// it is the first time, create it with the obj_id of referenced instance									
									relationshipAttribute =  _doc.CreateAttribute(entity.Name);
									relationshipAttribute.Value = referencedObjId;
									instanceElement.SetAttributeNode(relationshipAttribute);
								}
								else
								{									
									// append the obj_id of referenced instance to the existing attribute
									string attributeValue = relationshipAttribute.Value;
									relationshipAttribute.Value = attributeValue + " " + referencedObjId;
								}
							}
						}
					}
				}
			}
			catch (Exception e)
			{
				throw new SQLBuilderException(e.Message, e);
			}
			
			return true;
		}
		
		/// <summary>
		/// Visit a DBEntity object representing an function such as count, avg, min, max, sum.
		/// </summary>
		/// <param name="entity">the function entity to be visited.</param>
		public bool VisitFunction(AggregateFuncEntity entity)
		{
			XmlElement functionElement;
			string val;
			
			try
			{
				if (entity.OwnerClass != null)
				{
					// The function element is added to an instance of a class
					XmlElement instanceElement = GetInstanceElement(entity.OwnerClass);

					// Result set may not have an instance for the owner class, check it
					if (instanceElement != null)
					{
						functionElement = instanceElement[entity.Name];
						if (functionElement == null)
						{
							functionElement =  _doc.CreateElement(entity.Name);
							
							val = null;
							if (!_dataReader.IsDBNull(entity.ColumnIndex - 1))
							{
                                val = System.Convert.ToString(GetDBValue(entity.ColumnIndex - 1));
							}
							if (val == null)
							{
								val = SQLElement.NULL_STRING;
							}
							
							functionElement.InnerText = val;
							instanceElement.AppendChild(functionElement);
						}
					}
				}
				else
				{
					// create a function element and add it to root of document
					functionElement = _doc.CreateElement(entity.Name);
					val = null;
					if (!_dataReader.IsDBNull(entity.ColumnIndex - 1))
					{
                        val = System.Convert.ToString(GetDBValue(entity.ColumnIndex - 1));
					}
					if (val == null)
					{
						val = SQLElement.NULL_STRING;
					}
					
					functionElement.InnerText = val;
					_doc.DocumentElement.AppendChild(functionElement);
				}
			}
			catch (Exception e)
			{
				throw new SQLBuilderException("Failed to get function value", e);
			}
			
			return true;
		}
		
		/// <summary>
		/// Visits a SchemaEntity object representing a query schema.
		/// </summary>
		/// <param name="entity">the schema entity to be visited.</param>
		public bool VisitSchema(SchemaEntity entity)
		{
			return true;
		}
		
		/// <summary>
		/// Visits a ScoreEntity object.
		/// </summary>
		/// <param name="entity">the score entity to be visited.</param>
		public bool VisitScore(ScoreEntity entity)
		{
			try
			{
				// Result set may not have an instance for the owner class, check it
				XmlElement instanceElement = GetInstanceElement(entity.OwnerClass);

				if (instanceElement != null)
				{
					XmlElement functionElement = instanceElement[entity.Name];
					if (functionElement == null)
					{
						functionElement = _doc.CreateElement(entity.Name);
						
						string val = null;
						if (!_dataReader.IsDBNull(entity.ColumnIndex - 1))
						{
                            val = System.Convert.ToString(GetDBValue(entity.ColumnIndex - 1));
						}
						if (val == null)
						{
							val = SQLElement.NULL_STRING;
						}
						functionElement.InnerText = val;
						instanceElement.AppendChild(functionElement);
					}
				}
			}
			catch (Exception e)
			{
				throw new SQLBuilderException("Failed to get score value", e);
			}
			
			return true;
		}

        /// <summary>
        /// Generate values of virtual attributes found during the prior visit.
        /// </summary>
        public void GenerateVirtualAttributeValues(ExecutionContext context)
        {
            XmlElement instanceElement;
            XmlElement attributeElement;

            foreach (VirtualAttributeEntity entity in _instancesByVirtualAttribute.Keys)
            {
                if (entity.IsBrowsable)
                {
                    // run this code only when attribute is browsable
                    instanceElement = (XmlElement)_instancesByVirtualAttribute[entity];
                    attributeElement = instanceElement[entity.Name];
                    attributeElement.InnerText = GenerateValue(instanceElement, entity, context);
                }
            }
        }

        /// <summary>
        /// Generate value of a virtual attribute based on an instance element
        /// </summary>
        /// <param name="instanceElement">The xml instance element</param>
        /// <param name="attributeEntity">The attribute entity</param>
        /// <param name="context">Server execution context</param>
        /// <returns>The generated value.</returns>
        private string GenerateValue(XmlElement instanceElement, VirtualAttributeEntity attributeEntity, ExecutionContext context)
        {
            string val = null;
            if (!this._doc.IsNestedQuery &&
                !this._doc.IsForCount)
            {
                IFormula formula = (IFormula)_formulaTable[attributeEntity];

                if (formula == null)
                {
                    VirtualAttributeElement virtualAttributeElement = (VirtualAttributeElement)attributeEntity.SchemaModelElement;
                    // create a formula instance from the source code
                    formula = virtualAttributeElement.CreateFormula();

                    // keep it in the hashtable for reuse
                    _formulaTable[attributeEntity] = formula;
                }

                try
                {
                    InstanceView instanceView = (InstanceView)_instanceViewTable[attributeEntity];
                    if (instanceView == null)
                    {
                        DataViewModel dataView = _metaData.GetDetailedDataView(attributeEntity.OwnerClass.Name);
                        instanceView = new InstanceView(dataView);
                        // keep the instanceView in a hashtable for sake of performance
                        _instanceViewTable[attributeEntity] = instanceView;
                    }

                    InstanceElementWrapper wrapper = new InstanceElementWrapper(this._metaData, this._doc, instanceElement, instanceView);
                    context.Attribute = (VirtualAttributeElement)attributeEntity.SchemaModelElement;

                    VirtualAttributeValueGeneratorContext vContext = new VirtualAttributeValueGeneratorContext(formula, wrapper);

                    if (_delayCalculateVirtualValues)
                    {
                        // Delay generation of the virtual value until the attribute is selected as part of final output
                        // to improve the performance
                        val = "virtual_" + Guid.NewGuid(); // set an unique id

                        _doc.SetVirtualValueGeneratorContext(val, vContext);
                    }
                    else
                    {
                        val = formula.Execute(wrapper, context);
                    }
                }
                catch (Exception)
                {
                    val = "";
                    //throw new SQLBuilderException("Encounter an error while generating value for a virtual attribute " + attributeEntity.SchemaModelElement.Caption + ":" + ex.Message);
                }
            }
            else
            {
                val = "";
            }

            return val;
        }

		/// <summary>
		/// Verify if a xml element has been created for the class entity
		/// </summary>
		/// <param name="entity">the entity to be verified</param>
		/// <returns> true if an xml element has been created, false otherwise.</returns>
		private bool IsClassExist(ClassEntity entity)
		{
			return (GetClassElement(entity) != null? true : false);
		}
		
		/// <summary>
		/// Gets the xml element from the document that represents a class.
		/// </summary>
		/// <param name="entity">the class entity object.</param>
		/// <returns> the xml element for the class, null if it doesn't exist.</returns>
		private XmlElement GetClassElement(ClassEntity entity)
		{
			XmlElement found = null;
			
			ICollection keys = _classElements.Keys;
			
			foreach (ClassEntity key in keys)
			{
				/*
				* The key represents the bottom class for the instance, therefore,
				* if entity is the same as the key or the entity is an ancestor class
				* of the key, return the instance element held for the key.
				*/
				if (key == entity || key.IsChildOf(entity))
				{
					found = (XmlElement) _classElements[key];
					break;
				}
			}
			
			return found;
		}
		
		/// <summary>
		/// Gets the xml element from the document that represents an instance in a class.
		/// </summary>
		/// <param name="entity">the class entity object.</param>
		/// <returns>
		/// the xml instance element, null if it doesn't exist.
		/// </returns>
		private XmlElement GetInstanceElement(ClassEntity entity)
		{
			XmlElement found = null;
			
			/*
			* First, try to see if the instance for the obj id has been created before
			*/
			try
			{
				int colIndex = entity.ObjIdEntity.ColumnIndex;
				if (colIndex > 0)
				{
					string objId = null;
					if (!_dataReader.IsDBNull(colIndex - 1))
					{
						objId = System.Convert.ToString(_dataReader.GetValue(colIndex - 1));
					}

					if (objId != null)
					{
						found = (XmlElement) _instancesByObjID[objId];
					}
				}
			}
			catch (Exception e)
			{
				throw new SQLBuilderException(e.Message, e);
			}
			
			if (found == null)
			{
				
				ICollection keys = _instancesByClass.Keys;
				
				foreach (ClassEntity key in keys)
				{
					/*
					* The key represents the bottom class for the instance, therefore,
					* if entity is the same as the key or the entity is an ancestor class
					* of the key, return the instance element held for the key.
					*/
					if (key == entity || key.IsChildOf(entity))
					{
						found = (XmlElement) _instancesByClass[key];
						break;
					}
				}
			}
			else if (_instancesByClass[entity] == null)
			{
				_instancesByClass[entity] = found;
			}
			
			return found;
		}
		
		/// <summary>
		/// Gets the information indicating whether current row of the result set contains an instance for the class. 
		/// </summary>
		/// <param name="entity">the class entity object.</param>
		/// <returns>true if current row of the result set contains an instance for the class,
		/// otherwise, return false.
		/// </returns>
		private bool ContainsInstance(ClassEntity entity)
		{
			bool status = false;
			
			/*
			* If the object id is null for the class, we can be sure that the result set
			* does not contain an instance for the class.
			*/
			try
			{
				int colIndex = entity.ObjIdEntity.ColumnIndex;
				if (colIndex > 0 && !_dataReader.IsDBNull(colIndex - 1))
				{
					status = true;
				}
			}
			catch (Exception e)
			{
				throw new SQLBuilderException(e.Message, e);
			}
			
			return status;
		}
		
		/// <summary>
		/// Converts a classId to the class name.
		/// </summary>
		/// <param name="clsId">the number representation of a class id.</param>
		/// <returns> the converted class name</returns>
		private string ConvertToClassName(string clsId)
		{
			if (clsId == null)
			{
				throw new SQLBuilderException("Missing value for CID column");
			}
			
			ClassElement classElement = (ClassElement) _metaData.SchemaModel.FindClassById(clsId);
			
			if (classElement == null)
			{
				throw new SQLBuilderException("Failed to find a class with ID: " + clsId);
			}
			else
			{
				return classElement.Name;
			}
		}

        private bool HasDuplicatedChild(XmlElement root, XmlNode child)
        {
            bool status = false;

            foreach (XmlNode node in root.ChildNodes)
            {
                if (node.Name == child.Name)
                {
                    status = true;
                    break;
                }
            }

            return status;
        }

		/// <summary>
		/// Convert the value from db version to its localized version based on
		/// locale info
		/// </summary>
		/// <param name="entity">The attribute entity</param>
		/// <param name="dbValue">The db value</param>
		/// <returns>The localized value</returns>
		private string ToLocalizedValue(AttributeEntity entity, string dbValue)
		{
			string localizedValue = dbValue;
            
			switch (entity.SchemaModelElement.DataType)
			{
				case DataType.Boolean:
					if (dbValue == "0")
					{
						localizedValue = LocaleInfo.Instance.False;
					}
					else if (dbValue == "1")
					{
						localizedValue = LocaleInfo.Instance.True;
					}

					break;
				case DataType.Date:
                    DateTime date = DateTime.Parse(dbValue);
                    localizedValue = date.ToShortDateString();
					break;
				case DataType.DateTime:
                    DateTime dt = DateTime.Parse(dbValue);
                    localizedValue = dt.ToString("s");
					break;
			}

			return localizedValue;
		}

        /// <summary>
        /// Get a database value for a column from the data reader
        /// </summary>
        /// <param name="index">column index</param>
        /// <returns>The column value</returns>
        private object GetDBValue(int colIndex)
        {
            object val = null;
            try
            {
                val = _dataReader.GetValue(colIndex);
            }
            catch (Exception ex)
            {
                // may get an overflow exception from Oracle Reader, try the oracle
                // specific method
                if (_dataReader is OracleDataReader)
                {
                    val = ((OracleDataReader)_dataReader).GetOracleValue(colIndex);
                }
                else
                {
                    // other problem, rethrow the exception
                    throw ex;
                }
            }

            return val;
        }

        /// <summary>
        /// Get user display text from user data
        /// </summary>
        /// <param name="user"></param>
        /// <param name="userData"></param>
        /// <returns></returns>
        private string GetUserDisplayText(string user, string[] userData)
        {
            string displayText;
            if (string.IsNullOrEmpty(userData[0]) &&
                string.IsNullOrEmpty(userData[1]))
            {
                displayText = user;
            }
            else
            {
                displayText = GetFormatedName(userData[0], userData[1]);
            }

            return displayText;
        }

        public string GetFormatedName(string lastName, string firstName)
        {
            string displayFormat = LocaleInfo.Instance.PersonNameFormat;

            if (!string.IsNullOrEmpty(lastName))
            {
                lastName = lastName.Trim();
            }
            else
            {
                lastName = "";
            }

            if (!string.IsNullOrEmpty(firstName))
            {
                firstName = firstName.Trim();
            }
            else
            {
                firstName = "";
            }

            return string.Format(displayFormat, lastName, firstName);
        }

	}
}