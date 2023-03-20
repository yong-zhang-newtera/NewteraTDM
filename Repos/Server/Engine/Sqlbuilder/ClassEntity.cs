/*
* @(#)ClassEntity.cs
*
* Copyright (c) 2003 Newtera, Inc. All rights reserved.
*
*/
namespace Newtera.Server.Engine.Sqlbuilder
{
    using System;
    using System.Collections;

    using Newtera.Common.MetaData.Schema;
    using Newtera.Common.MetaData.XaclModel;

    /// <summary>
    /// A ClassEntity object represents a class in an object-relational data model. A class
    /// can have other entities associated with it, such as a parent class, children classes,
    /// attributes, and relationships. In addition to that, a ClassEntity object stores
    /// information about how a class is mapped to a relational database table(s).
    /// </summary>
    /// <version>  	1.0.0 08 Jul 2003 </version>
    /// <author> Yong Zhang </author>
    public class ClassEntity : DBEntity
    {
        /* private instance variables */
        private ClassEntity _parent; // It's parent
        private DBEntityCollection _subclasses; // It's direct children
        private DBEntityCollection _attributes; // It's attributes
        private ObjIdEntity _objIdEntity; // It's object id entity
        private ClsIdEntity _clsIdEntity; // It's class id entity
        private AttachmentEntity _attachmentEntity; // It's attachment count entity
        private PermissionEntity _permissionEntity;
        private ReadOnlyEntity _readOnlyEntity;
        private DBEntityCollection _relationships; // It's relationships to other class entities
        private DBEntityCollection _functions; // Functions for the class, such as values of score, avg, count
        private ClassElement _classElement; // The meta model element for this class
        private string _alias; // the class alias
        private bool _isHidden; // whether to hide all attributes from search result
        private ClassEntity _baseClass; // the base class
        private bool _hasImageAttribute = false;

        /// <summary>
        /// Initializes a new instance of the ClassEntity class 
        /// </summary>
        /// <param name="element">schema model element</param>
        public ClassEntity(ClassElement element)
            : base()
        {
            _classElement = element;
            _parent = null;
            _subclasses = null;
            _attributes = null;
            _objIdEntity = new ObjIdEntity();
            _objIdEntity.OwnerClass = this;
            _clsIdEntity = new ClsIdEntity();
            _clsIdEntity.OwnerClass = this;
            _attachmentEntity = new AttachmentEntity();
            _attachmentEntity.OwnerClass = this;
            _permissionEntity = new PermissionEntity();
            _permissionEntity.OwnerClass = this;
            _readOnlyEntity = new ReadOnlyEntity();
            _readOnlyEntity.OwnerClass = this;
            _relationships = null;
            _functions = null;
            _alias = null;
            _isHidden = false;
            _baseClass = this;
        }

        /// <summary>
        /// Get SchemaModel element of ClassEntity object. SchemaModel element provides meta data
        /// information about this class.
        /// </summary>
        /// <value> a ClassElement object</returns>
        public virtual ClassElement SchemaElement
        {
            get
            {
                return _classElement;
            }
        }

        /// <summary>
        /// Gets or sets the base class entity. Base class entity represents the leaf class of
        /// the class entity inheritance tree
        /// </summary>
        /// <value> The base class entity</value>
        public ClassEntity BaseClassEntity
        {
            get
            {
                return _baseClass;
            }
            set
            {
                _baseClass = value;
            }
        }

        /// <summary>
        /// Gets or sets the parent entity. parent class represents the parent class entity to the
        /// current class entity in the inheritance tree
        /// </summary>
        /// <value> The parent class</value>
        public ClassEntity ParentEntity
        {
            get
            {
                return _parent;
            }
            set
            {
                _parent = value;
            }
        }

        /// <summary>
        /// Gets the entity representing object id.
        /// </summary>
        /// <value> object id entity</value>
        public ObjIdEntity ObjIdEntity
        {
            get
            {
                return _objIdEntity;
            }
        }

        /// <summary>
        /// Gets the entity representing class id.
        /// </summary>
        /// <value> class id entity</value>
        public ClsIdEntity ClsIdEntity
        {
            get
            {
                return _clsIdEntity;
            }
        }

        /// <summary>
        /// Gets the entity representing attachment count.
        /// </summary>
        /// <value> attachment count entity</value>
        public AttachmentEntity AttachmentEntity
        {
            get
            {
                return _attachmentEntity;
            }
        }

        /// <summary>
        /// Gets the entity representing permission to a class instance.
        /// </summary>
        /// <value> An PermissionEntity object</value>
        public PermissionEntity PermissionEntity
        {
            get
            {
                return _permissionEntity;
            }
        }

        /// <summary>
        /// Gets the entity representing read-only attributes of a class instance.
        /// </summary>
        /// <value> A ReadOnlyEntity object</value>
        public ReadOnlyEntity ReadOnlyEntity
        {
            get
            {
                return _readOnlyEntity;
            }
        }

        /// <summary>
        /// Gets the subclasses collections.
        /// </summary>
        /// <value> The subclass collection.</value>
        public DBEntityCollection Subclasses
        {
            get
            {
                return _subclasses;
            }
        }

        /// <summary>
        /// Gets collection of local attributes.
        /// </summary>
        /// <value> The local attributes.</value>
        public DBEntityCollection LocalAttributes
        {
            get
            {
                return _attributes;
            }
        }

        /// <summary>
        /// Gets the all attributes from the local class or inherited classes,
        /// return null if none exists.
        /// </summary>
        /// <value> The Inherited Attribute list, null if none exists.</value>
        public DBEntityCollection InheritedAttributes
        {
            get
            {
                DBEntityCollection attributes = null;

                ClassEntity currentClass = this;

                while (currentClass != null)
                {
                    DBEntityCollection localAttributes = currentClass.LocalAttributes;

                    if (localAttributes != null)
                    {
                        if (attributes == null)
                        {
                            attributes = new DBEntityCollection();
                        }

                        attributes.AddCollection(localAttributes);
                    }

                    currentClass = currentClass.ParentEntity;
                }

                return attributes;
            }
        }

        /// <summary>
        /// Gets the all relationships from the local class or inherited classes.
        /// </summary>
        /// <returns> The collection of relationships, null if none exists.</returns>
        public DBEntityCollection InheritedRelationships
        {
            get
            {
                DBEntityCollection relationships = null;

                ClassEntity currentClass = this;

                while (currentClass != null)
                {
                    DBEntityCollection localRelationships = currentClass.LocalRelationships;

                    if (localRelationships != null)
                    {
                        if (relationships == null)
                        {
                            relationships = new DBEntityCollection();
                        }

                        relationships.AddCollection(localRelationships);
                    }

                    currentClass = currentClass.ParentEntity;
                }

                return relationships;
            }
        }

        /// <summary>
        /// Gets the local relationships.
        /// </summary>
        /// <value> The collection of relationship objects.</value>
        public DBEntityCollection LocalRelationships
        {
            get
            {
                return _relationships;
            }
        }

        /// <summary>
        /// Gets or stes the class alias
        /// </summary>
        /// <value> the class alias</value>
        public string Alias
        {
            get
            {
                return _alias;
            }
            set
            {
                _alias = value;
            }
        }

        /// <summary>
        /// Gets or sets the information indicating whether the class is hidden.
        /// </summary>
        /// <value> true if it is hidden, false otherwise.</value>
        public bool IsHidden
        {
            get
            {
                return _isHidden;
            }
            set
            {
                _isHidden = value;
            }
        }

        /// <summary>
        /// Gets the table name for the class.
        /// </summary>
        /// <value> table name for the class</value>
        /// <remarks>The table name is provided by schema model element</remarks>
        public string TableName
        {
            get
            {
                return _classElement.TableName;
            }
        }

        /// <summary>
        /// Gets the class size.
        /// </summary>
        /// <value> the class size.</value>
        /// <remarks> The class size may be provided in mete model</remarks>
        public virtual int ClassSize
        {
            get
            {
                int size = 0;
                // TODO
                //size = _classElement.getSize();

                return size;
            }
        }

        /// <summary>
        /// Gets the information indicating whether the class is leaf node
        /// </summary>
        /// <value>true if it is a leaf, false otherwise</value>
        public bool IsLeaf
        {
            get
            {
                return (_subclasses == null ? true : false);
            }
        }

        /// <summary>
        /// Gets the information indicating whether the class is root node
        /// </summary>
        public bool IsRoot
        {
            get
            {
                return (_parent == null ? true : false);
            }
        }

        /// <summary>
        /// Gets the class name
        /// </summary>
        /// <value> the class name</value>
        public override string Name
        {
            get
            {
                return _classElement.Name;
            }
        }

        /// <summary>
        /// Gets the data type of the entity.
        /// </summary>
        /// <value>DataType.Unknown</value>
        /// <remarks>Class entity represents a set of data, therefore, does not have a primitive type</remarks>
        public override DataType Type
        {
            get
            {

                return DataType.Unknown;
            }
        }

        /// <summary>
        /// Gets the DB name for the class entity.
        /// </summary>
        /// <value>
        /// String.Empty
        /// </returns>
        public override string ColumnName
        {
            get
            {
                return String.Empty;
            }
        }

        /// <summary>
        /// Gets the information indicating whether the class has image attributes
        /// </summary>
        public bool HasImageAttributes
        {
            get
            {
                return _hasImageAttribute;
            }
        }

        /// <summary>
        /// Accept a EntityVistor to visit itself, parent, children, attribute and
        /// relationship entities.
        /// </summary>
        /// <param name="visitor">the visiting visitor</param>
        public override void Accept(EntityVisitor visitor)
        {
            // Visit itself
            if (visitor.VisitClass(this))
            {
                // visit its parent
                if (_parent != null)
                {
                    _parent.Accept(visitor);
                }

                // visit its attributes
                if (_attributes != null)
                {
                    foreach (DBEntity attribute in _attributes)
                    {
                        attribute.Accept(visitor);
                    }
                }

                // visit its relationships
                if (_relationships != null)
                {
                    foreach (DBEntity relationship in _relationships)
                    {
                        relationship.Accept(visitor);
                    }
                }

                // visit its functions
                if (_functions != null)
                {
                    foreach (DBEntity function in _functions)
                    {
                        function.Accept(visitor);
                    }
                }

                // visit linked classes at end so that the attributes and relationships of the same class with appeared in SQL statment togetther
                if (_relationships != null)
                {
                    foreach (RelationshipEntity relationship in _relationships)
                    {
                        relationship.LinkedClass.Accept(visitor);
                    }
                }
            }
        }

        /// <summary>
        /// Add a class as a direct subclass to the current class.
        /// </summary>
        /// <param name="subclass">The subclass to be added.</param>
        public void AddSubclass(ClassEntity subclass)
        {
            // make this class as the parent to the subclass.
            subclass.ParentEntity = this;

            // add to the subclasses
            if (_subclasses == null)
            {
                _subclasses = new DBEntityCollection();
            }

            _subclasses.Add(subclass);
        }

        /// <summary>
        /// Add an attribute to the current class.
        /// </summary>
        /// <param name="attribute">The attribute to be added.</param>
        public void AddAttribute(AttributeEntity attribute)
        {
            // make this class as the owner class.
            attribute.OwnerClass = this;

            // pass the base class entity to the attribute so that permission check will work correctly
            attribute.BaseClassEntity = this.BaseClassEntity;

            // add to the attribute list
            if (_attributes == null)
            {
                _attributes = new DBEntityCollection();
            }

            _attributes.Add(attribute);
        }

        /// <summary>
        /// Gets the attribute for a given name from this local class only,
        /// return null if the name doesn't match.
        /// </summary>
        /// <param name="name">the attribute name</param>
        /// <returns> The AttributeEntity object.</returns>
        public AttributeEntity GetLocalAttribute(string name)
        {
            AttributeEntity found = null;

            if (_attributes != null)
            {
                foreach (AttributeEntity attribute in _attributes)
                {
                    if (attribute.Name == name)
                    {
                        found = attribute;
                        break;
                    }
                }
            }

            return found;
        }

        /// <summary>
        /// Find the attribute for a given name from the local class or inherited classes,
        /// return null if the name doesn't match.
        /// </summary>
        /// <param name="name">the attribute name.</param>
        /// <returns> The AttributeEntity object.</returns>
        public AttributeEntity GetInheritedAttribute(string name)
        {
            AttributeEntity found = null;

            ClassEntity currentClass = this;

            while (currentClass != null)
            {
                AttributeEntity attribute = currentClass.GetLocalAttribute(name);

                // Failed to find the attribute in current class, search its parent
                if (attribute != null)
                {
                    found = attribute;
                    break;
                }
                else
                {
                    currentClass = currentClass.ParentEntity;
                }
            }

            return found;
        }

        /// <summary>
        /// Gets the relationship for a given name from this local class only,
        /// return null if the name doesn't match.
        /// </summary>
        /// <param name="name">the relationship name.</param>
        /// <returns> The RelationshipEntity object.</returns>
        public RelationshipEntity GetLocalRelationship(string name)
        {
            RelationshipEntity found = null;

            if (_relationships != null)
            {
                foreach (RelationshipEntity relationship in _relationships)
                {
                    if (relationship.Name == name)
                    {
                        found = relationship;
                        break;
                    }
                }
            }

            return found;
        }

        /// <summary>
        /// Find the relationship for a given name from the local class or inherited classes,
        /// return null if the name doesn't match.
        /// </summary>
        /// <param name="name">the relationship name.</param>
        /// <returns> The RelationshipEntity object.</returns>
        public RelationshipEntity GetInheritedRelationship(string name)
        {
            RelationshipEntity found = null;

            ClassEntity currentClass = this;

            while (currentClass != null)
            {
                RelationshipEntity relationship = currentClass.GetLocalRelationship(name);

                // Failed to find the relationship in current class, search its parent
                if (relationship != null)
                {
                    found = relationship;
                    break;
                }
                else
                {
                    currentClass = currentClass.ParentEntity;
                }
            }

            return found;
        }

        /// <summary>
        /// Create an attribute of a given name and add it to this local class. If failed
        /// to find the attribute of the given name at this class, return a null object.
        /// </summary>
        /// <param name="name">the attribute name</param>
        /// <returns> the created AttributeEntity object.</returns>
        public AttributeEntity CreateLocalAttribute(string name)
        {
            AttributeEntity created = GetLocalAttribute(name);

            // do not create the attribute if it exists
            if (created == null)
            {
                SchemaModelElementCollection attributeElements = _classElement.SimpleAttributes;

                foreach (SchemaModelElement element in attributeElements)
                {
                    if (element.Name == name)
                    {
                        created = new SimpleAttributeEntity((SimpleAttributeElement)element);
                        AddAttribute(created);
                        break;
                    }
                }

                if (created == null)
                {
                    attributeElements = _classElement.ArrayAttributes;

                    foreach (SchemaModelElement element in attributeElements)
                    {
                        if (element.Name == name)
                        {
                            created = new ArrayAttributeEntity((ArrayAttributeElement)element);
                            AddAttribute(created);
                            break;
                        }
                    }
                }

                if (created == null)
                {
                    attributeElements = _classElement.VirtualAttributes;

                    foreach (SchemaModelElement element in attributeElements)
                    {
                        if (element.Name == name)
                        {
                            created = new VirtualAttributeEntity((VirtualAttributeElement)element);
                            AddAttribute(created);
                            break;
                        }
                    }
                }

                if (created == null)
                {
                    attributeElements = _classElement.ImageAttributes;

                    foreach (SchemaModelElement element in attributeElements)
                    {
                        if (element.Name == name)
                        {
                            created = new ImageAttributeEntity((ImageAttributeElement)element);
                            AddAttribute(created);
                            _hasImageAttribute = true;
                            break;
                        }
                    }
                }
            }

            return created;
        }

        /// <summary>
        /// Create a relationship of a given name and add it to this local class.
        /// If failed to find the relationship of the given name at this local class,
        /// return a null object.
        /// </summary>
        /// <param name="name">the relationship name</param>
        /// <returns> the created RelationshipEntity object</returns>
        public RelationshipEntity CreateLocalRelationship(string name)
        {
            return CreateLocalRelationship(name, true);
        }

        /// <summary>
        /// Create a relationship of a given name and add it to this local class.
        /// If failed to find the relationship of the given name at this local class,
        /// return a null object.
        /// </summary>
        /// <param name="name">the relationship name.</param>
        /// <param name="addToClass">whether to add the created relationship to the class.</param>
        /// <returns> the created RelationshipEntity object.</returns>
        public RelationshipEntity CreateLocalRelationship(string name, bool addToClass)
        {
            RelationshipEntity created = null;

            // try to see if it is already created. If it does, just return it
            if (addToClass)
            {
                created = GetLocalRelationship(name);
            }

            // do not create the attribute if it exists
            if (created == null)
            {
                SchemaModelElementCollection relationshipElements = _classElement.RelationshipAttributes;

                foreach (SchemaModelElement element in relationshipElements)
                {
                    if (element.Name == name)
                    {
                        created = new RelationshipEntity((RelationshipAttributeElement)element);
                        if (addToClass)
                        {
                            AddRelationship(created);
                        }
                    }
                }
            }

            return created;
        }

        /// <summary>
        /// Create an inherited attribute of a given name which may belong to this local
        /// class or one of its inherited classes. The created attribute is added to its
        /// owning class. If failed to find the attribute of the given name at this
        /// local class and any of its inherited classes, return a null object.
        /// </summary>
        /// <param name="name">the attribute name</param>
        /// <returns> the created AttributeEntity object</returns>
        public AttributeEntity CreateInheritedAttribute(string name)
        {
            ClassEntity currentClass = this;
            AttributeEntity created = null;

            while (currentClass != null)
            {
                created = currentClass.CreateLocalAttribute(name);
                if (created != null)
                {
                    break;
                }
                else
                {
                    currentClass = currentClass.ParentEntity;
                }
            }

            return created;
        }

        /// <summary>
        /// Create an inherited relationship of a given name which may belong to this local
        /// class or one of its inherited classes. The created relationship is added to its
        /// owning class. If failed to find the relationship of the given name at this
        /// local class and any of its inherited classes, return a null object.
        /// </summary>
        /// <param name="name">the relationship name.</param>
        /// <returns> the created RelationshipEntity object.</returns>
        public RelationshipEntity CreateInheritedRelationship(string name)
        {
            return CreateInheritedRelationship(name, true);
        }

        /// <summary>
        /// Create an inherited relationship of a given name which may belong to this local
        /// class or one of its inherited classes. The created relationship is added to its
        /// owning class. If failed to find the relationship of the given name at this
        /// local class and any of its inherited classes, return a null object.
        /// </summary>
        /// <param name="name">the relationship name</param>
        /// <param name="addToClass">whether to add the created relationship to the class</param>
        /// <returns> the created RelationshipEntity object.</returns>
        public RelationshipEntity CreateInheritedRelationship(string name, bool addToClass)
        {
            ClassEntity currentClass = this;
            RelationshipEntity created = null;

            while (currentClass != null)
            {
                created = currentClass.CreateLocalRelationship(name, addToClass);
                if (created != null)
                {
                    break;
                }
                else
                {
                    currentClass = currentClass.ParentEntity;
                }
            }

            return created;
        }

        /// <summary>
        /// Create and add all attributes of this local class.
        /// </summary>
        public void CreateLocalAttributes()
        {
            SchemaModelElementCollection attributeElements = _classElement.SimpleAttributes;

            foreach (SchemaModelElement element in attributeElements)
            {
                if (!HasAttribute(element.Name))
                {
                    AddAttribute(new SimpleAttributeEntity((SimpleAttributeElement)element));
                }
            }

            attributeElements = _classElement.ArrayAttributes;

            foreach (SchemaModelElement element in attributeElements)
            {
                if (!HasAttribute(element.Name))
                {
                    AddAttribute(new ArrayAttributeEntity((ArrayAttributeElement)element));
                }
            }

            attributeElements = _classElement.VirtualAttributes;

            foreach (SchemaModelElement element in attributeElements)
            {
                if (!HasAttribute(element.Name))
                {
                    AddAttribute(new VirtualAttributeEntity((VirtualAttributeElement)element));
                }
            }

            attributeElements = _classElement.ImageAttributes;

            foreach (SchemaModelElement element in attributeElements)
            {
                if (!HasAttribute(element.Name))
                {
                    AddAttribute(new ImageAttributeEntity((ImageAttributeElement)element));
                    _hasImageAttribute = true;
                }
            }
        }

        /// <summary>
        /// Create and add all relationships of this local class.
        /// </summary>
        public void CreateLocalRelationships()
        {
            SchemaModelElementCollection relationshipElements = _classElement.RelationshipAttributes;

            foreach (SchemaModelElement element in relationshipElements)
            {
                if (!HasRelationship(element.Name))
                {
                    AddRelationship(new RelationshipEntity((RelationshipAttributeElement)element));
                }
            }
        }

        /// <summary>
        /// Create primary keys of this local class if there are any. Return null if
        /// this class does not have any primary keys.
        /// </summary>
        /// <returns> the created primary keys or null
        /// 
        /// </returns>
        public DBEntityCollection CreateLocalPrimaryKeys()
        {

            SchemaModelElementCollection attributeElements = _classElement.SimpleAttributes;
            DBEntityCollection keys = null;
            AttributeEntity key;

            foreach (SchemaModelElement element in attributeElements)
            {
                if (((SimpleAttributeElement)element).IsPrimaryKey)
                {
                    // get it or create it
                    if (!HasAttribute(element.Name))
                    {
                        key = new SimpleAttributeEntity((SimpleAttributeElement)element);
                        AddAttribute(key);
                    }
                    else
                    {
                        key = GetLocalAttribute(element.Name);
                    }

                    if (keys == null)
                    {
                        keys = new DBEntityCollection();
                    }

                    keys.Add(key);
                }
            }

            return keys;
        }

        /// <summary>
        /// Creates primary key attributes of the class, assuming all primary keys are regular
        /// attributes, not relationships. The primary keys can be inheried its ancestor class
        /// class or one of its inherited classes.
        /// </summary>
        /// <returns> the created primary key list</returns>
        public DBEntityCollection CreatePrimaryKeys()
        {
            ClassEntity currentClass = this;
            DBEntityCollection keys = new DBEntityCollection();

            while (currentClass != null)
            {
                DBEntityCollection localPKs = currentClass.CreateLocalPrimaryKeys();
                if (localPKs != null)
                {
                    keys.AddCollection(localPKs);
                }

                // Primary keys may distribute in more than one class in a hierarchy
                currentClass = currentClass.ParentEntity;
            }

            return keys;
        }

        /// <summary>
        /// Creates the inherited classes to this class without any attributes and
        /// relationships
        /// </summary>
        public void CreateEmptyAncestorClasses()
        {
            ClassEntity childClass = this;
            ClassElement parentElement = _classElement.ParentClass;
            while (parentElement != null)
            {
                // do nothing if the parent class exists
                ClassEntity parentClass = childClass.ParentEntity;
                if (parentClass == null)
                {
                    parentClass = new ClassEntity(parentElement);
                    parentClass.BaseClassEntity = this;
                    childClass.ParentEntity = parentClass;
                    parentClass.AddSubclass(childClass);
                }

                childClass = parentClass;
                parentElement = parentElement.ParentClass;
            }
        }

        /// <summary>
        /// Create a full-blown class object with its inherited classes, all with
        /// complete set of attributes and relationships.
        /// </summary>
        public void MakeFullBlown()
        {
            // create its ancestor (inherited) classes first
            CreateEmptyAncestorClasses();

            // create attributes and relationships for this local class and its ancestor
            ClassEntity currentClass = this;
            while (currentClass != null)
            {
                currentClass.CreateLocalAttributes();
                currentClass.CreateLocalRelationships();

                currentClass = currentClass.ParentEntity;
            }
        }

        /// <summary>
        /// Get information whether the class has a given attribute.
        /// </summary>
        /// <returns> true if it has, false otherwise.</returns>
        public bool HasAttribute(string name)
        {
            return (GetLocalAttribute(name) != null ? true : false);
        }

        /// <summary>
        /// Get information whether the class has a given relationship.
        /// </summary>
        /// <returns> true if it has, false otherwise.</returns>
        public bool HasRelationship(string name)
        {
            return (GetLocalRelationship(name) != null ? true : false);
        }

        /// <summary>
        /// Get information whether the class has any attributes.
        /// </summary>
        /// <returns>true if the class has attributes, false otherwise.</returns>
        public bool HasLocalAttributes()
        {
            return (_attributes != null && _attributes.Count > 0 ? true : false);
        }

        /// <summary>
        /// Get information whether the class has any relationship.
        /// </summary>
        /// <returns>true if the class has relationships, false otherwise.</returns>
        public bool HasLocalRelationships()
        {
            return (_relationships != null && _relationships.Count > 0 ? true : false);
        }

        /// <summary>
        /// Get information whether the class has any inherited attributes.
        /// </summary>
        /// <returns>true if the class has inheriited attributes, false otherwise.</returns>
        public bool HasInheritedAttributes()
        {
            bool status = false;
            ClassEntity currentClass = this;

            while (currentClass != null)
            {
                if (currentClass.HasLocalAttributes())
                {
                    status = true;
                    break;
                }
                else
                {
                    currentClass = currentClass.ParentEntity;
                }
            }

            return status;
        }

        /// <summary>
        /// Get information whether the class has any inherited relationships.
        /// </summary>
        /// <returns>true if the class has inheriited relationships, false otherwise.</returns>
        public bool HasInheritedRelationships()
        {
            bool status = false;
            ClassEntity currentClass = this;

            while (currentClass != null)
            {
                if (currentClass.HasLocalRelationships())
                {
                    status = true;
                    break;
                }
                else
                {
                    currentClass = currentClass.ParentEntity;
                }
            }

            return status;
        }

        /// <summary>
        /// Add a relationship to the current class.
        /// </summary>
        /// <param name="relationship">The relationship to be added.</param>
        public void AddRelationship(RelationshipEntity relationship)
        {
            // make this class as the owner class.
            relationship.OwnerClass = this;

            // pass the base class entity to the relationship so that permission check will work correctly
            relationship.BaseClassEntity = this.BaseClassEntity;

            // add to the relationship list
            if (_relationships == null)
            {
                _relationships = new DBEntityCollection();
            }

            _relationships.Add(relationship);
        }

        /// <summary>
        /// Add a function to the current class.
        /// </summary>
        /// <param name="function">The function to be added. </param>
        public void AddFunction(DBEntity function)
        {
            // make this class as the owner class.
            function.OwnerClass = this;

            // add to the function list
            if (_functions == null)
            {
                _functions = new DBEntityCollection();
            }
            _functions.Add(function);
        }

        /// <summary>
        /// Gets the function for a given name from this local class only,
        /// return null if the name doesn't match.
        /// </summary>
        /// <param name="name">the name of function.</param>
        /// <returns> The DBEntity object.</returns>
        public DBEntity GetFunction(string name)
        {
            DBEntity found = null;

            if (_functions != null)
            {
                foreach (DBEntity function in _functions)
                {
                    if (function.Name == name)
                    {
                        found = function;
                        break;
                    }
                }
            }

            return found;
        }

        /// <summary>
        /// Gets the information indicating whether this class is a child of the given class entity
        /// </summary>
        /// <param name="other">the class entity to be verified as an ancestor of this class entity.</param>
        /// <returns> true if it is child of given class, false otherwise.</returns>
        public bool IsChildOf(ClassEntity other)
        {
            bool status = false;

            ClassEntity current = this;
            while (!current.IsRoot)
            {
                if (current.ParentEntity == other)
                {
                    status = true;
                    break;
                }
                else
                {
                    current = current.ParentEntity;
                }
            }

            return status;
        }

        #region IXaclObject Members

        /// <summary>
        /// Return a xpath representation of the AttributeEntity
        /// </summary>
        /// <returns>a xapth representation</returns>
        public override string ToXPath()
        {
            if (_xpath == null)
            {
                _xpath = this.SchemaElement.ToXPath();
            }

            return _xpath;
        }

        /// <summary>
        /// Return a  parent of the SchemaModelElement
        /// </summary>
        /// <returns>The parent of the SchemaModelElement</returns>
        public override IXaclObject Parent
        {
            get
            {
                return null; // not appropriate
            }
        }

        /// <summary>
        /// Return a  of children of the SchemaModelElement
        /// </summary>
        /// <returns>The collection of IXaclObject nodes</returns>
        public override IEnumerator GetChildren()
        {
            // return an empty enumerator
            ArrayList children = new ArrayList();
            return children.GetEnumerator();
        }

        #endregion
    }
}
