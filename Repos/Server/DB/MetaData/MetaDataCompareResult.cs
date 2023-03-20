/*
* @(#)MetaDataCompareResult.cs
*
* Copyright (c) 2003 Newtera, Inc. All rights reserved.
*
*/
namespace Newtera.Server.DB.MetaData
{
	using System;
	using Newtera.Common.MetaData;
	using Newtera.Common.MetaData.Schema;

	/// <summary>
	/// Keeps the result of comparing two different MetaDataModel.
	/// </summary>
	/// <version>  	1.0.0 16 Oct 2003 </version>
	/// <author> Yong Zhang</author>
	public class MetaDataCompareResult
	{
		private MetaDataModel _newMetaDataModel;
		private MetaDataModel _oldMetaDataModel;
		private IMetaDataAction _addSchema = null;
		private IMetaDataAction _deleteSchema = null;
		private MetaDataActionCollection _addClasses = null;
		private MetaDataActionCollection _deleteClasses = null;
		private MetaDataActionCollection _alterClasses = null;
		private MetaDataActionCollection _addSimpleAttributes = null;
		private MetaDataActionCollection _deleteSimpleAttributes = null;
		private MetaDataActionCollection _alterSimpleAttributes = null;
		private MetaDataActionCollection _addRelationshipAttributes = null;
		private MetaDataActionCollection _deleteRelationshipAttributes = null;
		private MetaDataActionCollection _alterRelationshipAttributes = null;
		private MetaDataActionCollection _addArrayAttributes = null;
		private MetaDataActionCollection _deleteArrayAttributes = null;
		private MetaDataActionCollection _alterArrayAttributes = null;
        private MetaDataActionCollection _addImageAttributes = null;
        private MetaDataActionCollection _deleteImageAttributes = null;
        private SchemaModelElementCollection _mismatchedEnums = null;

		/// <summary>
		/// Initializes a new instance of the MetaDataCompareResult class
		/// </summary>
		public MetaDataCompareResult(MetaDataModel newMetaDataModel,
			MetaDataModel oldMetaDataModel)
		{
			_newMetaDataModel = newMetaDataModel;
			_oldMetaDataModel = oldMetaDataModel;
			_addClasses = new MetaDataActionCollection();
			_deleteClasses = new MetaDataActionCollection();
			_alterClasses = new MetaDataActionCollection();
			_addSimpleAttributes = new MetaDataActionCollection();
			_deleteSimpleAttributes = new MetaDataActionCollection();
			_alterSimpleAttributes = new MetaDataActionCollection();
			_addRelationshipAttributes = new MetaDataActionCollection();
			_deleteRelationshipAttributes = new MetaDataActionCollection();
			_alterRelationshipAttributes = new MetaDataActionCollection();
			_addArrayAttributes = new MetaDataActionCollection();
			_deleteArrayAttributes = new MetaDataActionCollection();
			_alterArrayAttributes = new MetaDataActionCollection();
            _addImageAttributes = new MetaDataActionCollection();
            _deleteImageAttributes = new MetaDataActionCollection();
            _mismatchedEnums = new SchemaModelElementCollection();
		}

		/// <summary>
		/// Gets the new meta data model in the comparison
		/// </summary>
		public MetaDataModel NewMetaDataModel
		{
			get
			{
				return _newMetaDataModel;
			}
		}

		/// <summary>
		/// Gets the old meta data model in the comparison
		/// </summary>
		public MetaDataModel OldMetaDataModel
		{
			get
			{
				return _oldMetaDataModel;
			}
		}

		/// <summary>
		/// Gets the information indicating whether the result is for adding a schema
		/// </summary>
		/// <value>true if it is adding a schema, false otherwise</value>
		public bool IsAddSchema
		{
			get
			{
				if (_addSchema != null)
				{
					return true;
				}
				else
				{
					return false;
				}
			}
		}

		/// <summary>
		/// Gets the information indicating whether the result is for deleting a schema
		/// </summary>
		/// <value>true if it is deleting a schema, false otherwise</value>
		public bool IsDeleteSchema
		{
			get
			{
				if (_deleteSchema != null)
				{
					return true;
				}
				else
				{
					return false;
				}
			}
		}

		/// <summary>
		/// Gets or sets the action that addes a new schema
		/// </summary>
		public IMetaDataAction AddSchema
		{
			get
			{
				return _addSchema;
			}
			set
			{
				_addSchema = value;
			}
		}

		/// <summary>
		/// Gets or sets the action that deletes an existing schema
		/// </summary>
		public IMetaDataAction DeleteSchema
		{
			get
			{
				return _deleteSchema;
			}
			set
			{
				_deleteSchema = value;
			}
		}

		/// <summary>
		/// Gets actions that add classes to the meta data model in database
		/// </summary>
		public MetaDataActionCollection AddClasses
		{
			get
			{
				return _addClasses;
			}
		}

		/// <summary>
		/// Gets the actions that deletes classes from the meta data model database
		/// </summary>
		public MetaDataActionCollection DeleteClasses
		{
			get
			{
				return _deleteClasses;
			}
		}

		/// <summary>
		/// Gets the actions that alters classes in the meta data model database
		/// </summary>
		public MetaDataActionCollection AlterClasses
		{
			get
			{
				return _alterClasses;
			}
		}

		/// <summary>
		/// Gets the actions that addes simple attributes to the meta data model database
		/// </summary>
		public MetaDataActionCollection AddSimpleAttributes
		{
			get
			{
				return _addSimpleAttributes;
			}
		}

		/// <summary>
		/// Gets the actions that delete simple attributes from the meta data model database
		/// </summary>
		public MetaDataActionCollection DeleteSimpleAttributes
		{
			get
			{
				return _deleteSimpleAttributes;
			}
		}

		/// <summary>
		/// Gets the actions that alter simple attributes in the meta data model database
		/// </summary>
		public MetaDataActionCollection AlterSimpleAttributes
		{
			get
			{
				return _alterSimpleAttributes;
			}
		}

		/// <summary>
		/// Gets the actions that added relationship attributes to the meta data model database
		/// </summary>
		public MetaDataActionCollection AddRelationshipAttributes
		{
			get
			{
				return _addRelationshipAttributes;
			}
		}

		/// <summary>
		/// Gets the actions that delete relationship attributes from the meta data model database
		/// </summary>
		public MetaDataActionCollection DeleteRelationshipAttributes
		{
			get
			{
				return _deleteRelationshipAttributes;
			}
		}

		/// <summary>
		/// Gets the actions that alter relationship attributes in the meta data model database
		/// </summary>
		public MetaDataActionCollection AlterRelationshipAttributes
		{
			get
			{
				return _alterRelationshipAttributes;
			}
		}

		/// <summary>
		/// Gets the actions that addes array attributes to the meta data model database
		/// </summary>
		public MetaDataActionCollection AddArrayAttributes
		{
			get
			{
				return _addArrayAttributes;
			}
		}

		/// <summary>
		/// Gets the actions that delete array attributes from the meta data model database
		/// </summary>
		public MetaDataActionCollection DeleteArrayAttributes
		{
			get
			{
				return _deleteArrayAttributes;
			}
		}

		/// <summary>
		/// Gets the actions that alter array attributes in the meta data model database
		/// </summary>
		public MetaDataActionCollection AlterArrayAttributes
		{
			get
			{
				return _alterArrayAttributes;
			}
		}

        /// <summary>
        /// Gets the actions that addes image attributes to the meta data model database
        /// </summary>
        public MetaDataActionCollection AddImageAttributes
        {
            get
            {
                return _addImageAttributes;
            }
        }

        /// <summary>
        /// Gets the actions that delete image attributes from the meta data model database
        /// </summary>
        public MetaDataActionCollection DeleteImageAttributes
        {
            get
            {
                return _deleteImageAttributes;
            }
        }

        /// <summary>
        /// Gets a collection of mismatched enum elements
        /// </summary>
        public SchemaModelElementCollection MismatchedEnumElements
        {
            get
            {
                return _mismatchedEnums;
            }
        }

        public bool IsAdditionOnly
        {
            get
            {
                bool status = true;

                if (_deleteClasses.Count > 0)
                {
                    status = false;
                }
                else if (_deleteSimpleAttributes.Count > 0)
                {
                    status = false;
                }
                else if (_deleteRelationshipAttributes.Count > 0)
                {
                    status = false;
                }
                else if (_deleteArrayAttributes.Count > 0)
                {
                    status = false;
                }
                else if (_deleteImageAttributes.Count > 0)
                {
                    status = false;
                }

                return status;
            }
        }

        public bool HasUpdateActions
        {
            get
            {
                bool status = false;

                if (_alterClasses.Count > 0)
                {
                    status = true;
                }
                else if (_alterSimpleAttributes.Count > 0)
                {
                    status = true;
                }
                else if (_alterArrayAttributes.Count > 0)
                {
                    status = true;
                }
                else if (_alterRelationshipAttributes.Count > 0)
                {
                    status = true;
                }
                else if (_deleteClasses.Count > 0)
                {
                    status = true;
                }
                else if (_deleteSimpleAttributes.Count > 0)
                {
                    status = true;
                }
                else if (_deleteRelationshipAttributes.Count > 0)
                {
                    status = true;
                }
                else if (_deleteArrayAttributes.Count > 0)
                {
                    status = true;
                }
                else if (_deleteImageAttributes.Count > 0)
                {
                    status = true;
                }
                else if (_addClasses.Count > 0)
                {
                    status = true;
                }
                else if (_addSimpleAttributes.Count > 0)
                {
                    status = true;
                }
                else if (_addRelationshipAttributes.Count > 0)
                {
                    status = true;
                }
                else if (_addArrayAttributes.Count > 0)
                {
                    status = true;
                }
                else if (_addImageAttributes.Count > 0)
                {
                    status = true;
                }
                else if (_addSchema != null)
                {
                    status = true;
                }
                else if (_deleteSchema != null)
                {
                    status = true;
                }

                return status;
            }
        }

        /// <summary>
        /// Gets the info of the first element
        /// </summary>
        public string DeleteElementInfo
        {
            get
            {
                string msg = "Unknown";

                try
                {
                    if (_deleteClasses.Count > 0)
                    {
                        msg = "Class:" + _deleteClasses[0].SchemaModelElement.Caption;
                    }
                    else if (_deleteSimpleAttributes.Count > 0)
                    {
                        ClassElement ownerClass = ((SimpleAttributeElement)_deleteSimpleAttributes[0].SchemaModelElement).OwnerClass;
                        if (ownerClass != null)
                        {
                            msg = "Class:" + ownerClass.Caption + " Simple Attribute:" + _deleteSimpleAttributes[0].SchemaModelElement.Caption;
                        }
                        else
                        {
                            msg = "Simple Attribute:" + _deleteSimpleAttributes[0].SchemaModelElement.Caption;
                        }
                    }
                    else if (_deleteRelationshipAttributes.Count > 0)
                    {
                        ClassElement ownerClass = ((RelationshipAttributeElement)_deleteRelationshipAttributes[0].SchemaModelElement).OwnerClass;
                        if (ownerClass != null)
                        {
                            msg = "Class:" + ownerClass.Caption + " Relationship Attribute:" + _deleteRelationshipAttributes[0].SchemaModelElement.Caption; ;
                        }
                        else
                        {
                            msg = "Relationship Attribute:" + _deleteRelationshipAttributes[0].SchemaModelElement.Caption;
                        }
                    }
                    else if (_deleteArrayAttributes.Count > 0)
                    {
                        ClassElement ownerClass = ((ArrayAttributeElement)_deleteArrayAttributes[0].SchemaModelElement).OwnerClass;
                        if (ownerClass != null)
                        {
                            msg = "Class:" + ownerClass.Caption + " Array Attribute:" + _deleteArrayAttributes[0].SchemaModelElement.Caption;
                        }
                        else
                        {
                            msg = "Array Attribute:" + _deleteArrayAttributes[0].SchemaModelElement.Caption;
                        }
                    }
                    else if (_deleteImageAttributes.Count > 0)
                    {
                        ClassElement ownerClass = ((ImageAttributeElement)_deleteImageAttributes[0].SchemaModelElement).OwnerClass;

                        if (ownerClass != null)
                        {

                            msg = "Class:" + ownerClass.Caption + " Image Attribute:" + _deleteImageAttributes[0].SchemaModelElement.Caption;
                        }
                        else
                        {
                            msg = "Image Attribute:" + _deleteImageAttributes[0].SchemaModelElement.Caption;
                        }
                    }
                }
                catch (Exception)
                {
                }

                return msg;
            }
        }

		/// <summary>
		/// Add an AddClass action
		/// </summary>
		public void AddAddClassAction(IMetaDataAction action)
		{
			_addClasses.Add(action);
		}

		/// <summary>
		/// Add an DeleteClass action
		/// </summary>
		public void AddDeleteClassAction(IMetaDataAction action)
		{
			_deleteClasses.Add(action);
		}

		/// <summary>
		/// Add an AlterClassInheritance action
		/// </summary>
		public void AddAlterClassAction(IMetaDataAction action)
		{
			_alterClasses.Add(action);
		}

		/// <summary>
		/// Add an AddSimpleAttribute action
		/// </summary>
		public void AddAddSimpleAttributeAction(IMetaDataAction action)
		{
			_addSimpleAttributes.Add(action);
		}

		/// <summary>
		/// Add a DeleteSimpleAttribute action
		/// </summary>
		public void AddDeleteSimpleAttributeAction(IMetaDataAction action)
		{
			_deleteSimpleAttributes.Add(action);
		}

		/// <summary>
		/// Add an AlterSimpleAttribute action
		/// </summary>
		public void AddAlterSimpleAttributeAction(IMetaDataAction action)
		{
			_alterSimpleAttributes.Add(action);
		}

		/// <summary>
		/// Add an AddRelationshipAttribute action
		/// </summary>
		public void AddAddRelationshipAttributeAction(IMetaDataAction action)
		{
			_addRelationshipAttributes.Add(action);
		}

		/// <summary>
		/// Add a DeleteRelationshipAttribute action
		/// </summary>
		public void AddDeleteRelationshipAttributeAction(IMetaDataAction action)
		{
			_deleteRelationshipAttributes.Add(action);
		}

		/// <summary>
		/// Add an AlterRelationshipAttribute action
		/// </summary>
		public void AddAlterRelationshipAttributeAction(IMetaDataAction action)
		{
			_alterRelationshipAttributes.Add(action);
		}

		/// <summary>
		/// Add an AddArrayAttribute action
		/// </summary>
		public void AddAddArrayAttributeAction(IMetaDataAction action)
		{
			_addArrayAttributes.Add(action);
		}

		/// <summary>
		/// Add a DeleteArrayAttribute action
		/// </summary>
		public void AddDeleteArrayAttributeAction(IMetaDataAction action)
		{
			_deleteArrayAttributes.Add(action);
		}

		/// <summary>
		/// Add an AlterArrayAttribute action
		/// </summary>
		public void AddAlterArrayAttributeAction(IMetaDataAction action)
		{
			_alterArrayAttributes.Add(action);
		}

        /// <summary>
        /// Add an AddImageAttribute action
        /// </summary>
        public void AddAddImageAttributeAction(IMetaDataAction action)
        {
            _addImageAttributes.Add(action);
        }

        /// <summary>
        /// Add a DeleteImageAttribute action
        /// </summary>
        public void AddDeleteImageAttributeAction(IMetaDataAction action)
        {
            _deleteImageAttributes.Add(action);
        }

        /// <summary>
        /// Add an EnumElement to the mismatched list
        /// </summary>
        public void AddMismatchedEnumElement(EnumElement enumElement)
        {
            _mismatchedEnums.Add(enumElement);
        }
	}
}