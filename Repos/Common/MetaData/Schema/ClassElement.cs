/*
* @(#)ClassElement.cs
*
* Copyright (c) 2003 Newtera, Inc. All rights reserved.
*
*/
namespace Newtera.Common.MetaData.Schema
{
	using System;
	using System.Collections;
	using System.Xml;
	using System.Xml.Schema;
	using System.ComponentModel;
	using System.Drawing.Design;
    using System.Security;

	using Newtera.Common.Core;
	using Newtera.Common.MetaData.XaclModel;

	/// <summary>
	/// The ClassElement represents a class in a schema model
	/// </summary>
	/// <example>
	/// </example>
	/// 
	/// <version>1.0.1 26 Jun 2003
	/// </version>
	/// <author> Yong Zhang </author>
	public class ClassElement : SchemaModelElement
	{
        public const string CLASS_INITIALIZATION_CODE = "InitializationCode";
        public const string CLASS_BEFORE_INSERT_CODE = "BeforeInsertCode";
        public const string CLASS_BEFORE_UPDATE_CODE = "BeforeUpdateCode";
        public const string CLASS_CALLBACK_CODE = "CallbackCode";

        /// <summary>
        /// Category changed event
        /// </summary>
        public event EventHandler CategoryChanged;

        private string _category = null;

		private ClassElement _parentClass = null;
		
		private SchemaModelElementCollection _subclasses;

        private bool _needToAlter;
		
		// the simple attributes in this class.
		private SchemaModelElementCollection _simpleAttributes;

		// the relationship attributes in this class.
		private SchemaModelElementCollection _relationshipAttributes;

		// the array attributes in this class.
		private SchemaModelElementCollection _arrayAttributes;

        // the virtual attributes in this class.
        private SchemaModelElementCollection _virtualAttributes;

        // the image attributes in this class.
        private SchemaModelElementCollection _imageAttributes;

		// the primary keys of this class
		private SchemaModelElementCollection _primaryKeys;

        // the unique constraint keys of this class
        private SchemaModelElementCollection _uniqueKeys;

        // the name of attribute used for default sorting.
        private string _sortAttributeName;

        private SortDirection _sortDirection;

		// The corresponding name of database table
		private string _tableName;

		// The name of a large image of the class
		private string _largeImage = null;

		// The name of a median image of the class
		private string _medianImage = null;

		// The name of a small image of the class
		private string _smallImage = null;

		// the long description of the class
		private string _text = null;

        // is browsable
        private bool _isBrowsable = true;

        // is junction class for many-to-many relationship
        private bool _isJunction = false;

        // show the classes related to the instance on web user interface 
        private bool _showRelatedClasses = true;

        // a collection of custom pages showing on the web client
        private SchemaModelElementCollection _customPages;

        // code run at time of new instance initialization
        private string _initializationCode;

        // code run before an update query is generated
        private string _beforeUpdateCode;

        // code run before an insert query is generated
        private string _beforeInsertCode;

        // code run at time of the Callback function is called by user interface
        private string _callbackFunctionCode;

        // xquery condition to match instances in the class, used when batch loading
        private string _matchCondition;

        // Specify the customized web page for displaying class data and instance data
        private string _classPageUrl = null;
        private string _instancePageUrl = null;

        // Specifiy the data view to be used in a form as a nested table
        private string _dataViewName = null;

		/// <summary>
		/// Initializing a ClassElement object
		/// </summary>
		/// <param name="name">the name of the class</param>
		internal ClassElement(string name) : base(name)
		{
			InitBlock();
		}

		/// <summary>
		/// Initializing a ClassElement object
		/// </summary>
		/// <param name="xmlSchemaElement">
		/// the xml schema element that represents the class
		/// </param>
		internal ClassElement(XmlSchemaAnnotated xmlSchemaElement) : base(xmlSchemaElement)
		{
			InitBlock();
		}
	
		/// <summary>
		/// Gets all direct subclasses of the class.
		/// </summary>
		/// <value>A SchemaModelElementCollection contains ClassElement objects for direct subclasses.</value>
		[BrowsableAttribute(false)]
		public SchemaModelElementCollection Subclasses
		{
			get
			{
				return _subclasses;
			}
		}

        /// <summary>
        /// Gets or sets the information indicating whether the class need to be altered during schema update
        /// </summary>
        /// <value>True need alteration, false, otherwise.</value>
        /// <remarks>it's value is Runm time use only </remarks>
        [BrowsableAttribute(false)]
        public bool NeedToAlter
        {
            get
            {
                return _needToAlter;
            }
            set
            {
                _needToAlter = value;
            }
        }

		/// <summary>
		/// Gets the parent class of the class.
		/// </summary>
		/// <value> The parent class</value>
		/// <remarks>
		/// The parent class can not be changed.
		/// It is provided as a parameter to the constructor.
		/// </remarks>
		[BrowsableAttribute(false)]		
		public ClassElement ParentClass
		{
			get
			{
				return _parentClass;
			}
			set
			{
				_parentClass = value;
			}
		}

		/// <summary>
		/// Gets the information that indicates whether the class is leaf class
		/// </summary>
		/// <value>
		/// true if class is leaf, otherwise false. The default value is true.
		/// </value>
		[BrowsableAttribute(false)]		
		public bool IsLeaf
		{
			get
			{
				return (_subclasses.Count == 0? true : false);
			}
		}

		/// <summary>
		/// Gets the information that indicates whether the class is root class
		/// </summary>
		/// <value>
		/// true if class is root, otherwise false. The default value is true.
		/// </value>
		[BrowsableAttribute(false)]		
		public bool IsRoot
		{
			get
			{
				return (_parentClass == null? true : false);
			}
		}

		/// <summary>
		/// Gets root class of this class
		/// </summary>
		/// 
		/// <value> The root class of the class. If the class is a root class,
		/// it return itself</value>
		[BrowsableAttribute(false)]		
		public ClassElement RootClass
		{
			get
			{
				if (IsRoot)
				{
					return this;
				}
				else
				{
					return ParentClass.RootClass;
				}
			}
		}

		/// <summary>
		/// Gets simple attributes of the class.
		/// </summary>
		/// <value>A SchemaModelElementCollection contains SimpleAttributeElement objects. </value>
		[BrowsableAttribute(false)]		
		public SchemaModelElementCollection SimpleAttributes
		{
			get
			{	
				return _simpleAttributes;
			}
		}

		/// <summary>
		/// Gets relationship attributes of the class.
		/// </summary>
		/// <value>A SchemaModelElementCollection contains RelationshipAttributeElement objects.</value>
		[BrowsableAttribute(false)]		
		public SchemaModelElementCollection RelationshipAttributes
		{
			get
			{	
				return _relationshipAttributes;
			}
		}

		/// <summary>
		/// Gets array attributes of the class.
		/// </summary>
		/// <value>A SchemaModelElementCollection contains ArrayAttributeElement objects. </value>
		[BrowsableAttribute(false)]		
		public SchemaModelElementCollection ArrayAttributes
		{
			get
			{	
				return _arrayAttributes;
			}
		}

        /// <summary>
        /// Gets virtual attributes of the class.
        /// </summary>
        /// <value>A SchemaModelElementCollection contains VirtualAttributeElement objects. </value>
        [BrowsableAttribute(false)]
        public SchemaModelElementCollection VirtualAttributes
        {
            get
            {
                return _virtualAttributes;
            }
        }

        /// <summary>
        /// Gets image attributes of the class.
        /// </summary>
        /// <value>A SchemaModelElementCollection contains ImageAttributeElement objects. </value>
        [BrowsableAttribute(false)]
        public SchemaModelElementCollection ImageAttributes
        {
            get
            {
                return _imageAttributes;
            }
        }

        /// <summary>
        /// Gets or sets category of the attribute.
        /// </summary>
        /// <value>
        /// A string of category name.
        /// </value>
        [
            CategoryAttribute("Appearance"),
            DescriptionAttribute("Specify a category of the class. A category is used to group classes sharing same characteristics together."),
            DefaultValueAttribute(null),
            EditorAttribute("Newtera.Studio.ClassCategoryPropertyEditor, Studio", typeof(UITypeEditor))
        ]
        public string Category
        {
            get
            {
                return _category;
            }
            set
            {
                string oldCategory = _category;
                _category = value;

                // Raise the event for Category change
                if (oldCategory != value && CategoryChanged != null)
                {
                    CategoryChanged(this, new ValueChangedEventArgs("Category", value));
                }
            }
        }

        /// <summary>
        /// Gets or sets the information indicating whether the class is browsable.
        /// </summary>
        /// <value>true if it is browsable, false otherwise, default is true</value>
        /// <remarks>If browsable is false, the subclasses of the class are not browsable either.</remarks>
        [
            CategoryAttribute("Appearance"),
            DescriptionAttribute("Is the class browsable?"),
            DefaultValueAttribute(true)
        ]
        public bool IsBrowsable
        {
            get
            {
                return _isBrowsable;
            }
            set
            {
                _isBrowsable = value;
            }
        }

		/// <summary>
		/// Gets or sets the primary keys of the class.
		/// </summary>
		/// <value>A SchemaModelElementCollection conatins SimpleAttributeElement objects for primary keys</value>
		[
			CategoryAttribute("System"),
			DescriptionAttribute("A collection of primary keys of the class"),
			EditorAttribute("Newtera.Studio.PrimaryKeysPropertyEditor, Studio", typeof(UITypeEditor))
		]
		public SchemaModelElementCollection PrimaryKeys
		{
			get
			{	
				return _primaryKeys;
			}
			set
			{
				// clear the old primary keys
				foreach (SimpleAttributeElement pk in _primaryKeys)
				{
					pk.IsPrimaryKey = false;
				}

				foreach (SimpleAttributeElement pk in value)
				{
					pk.IsPrimaryKey = true;
				}

				_primaryKeys = value;
			}
		}

        /// <summary>
        /// Gets or sets the unique constarint keys of the class.
        /// </summary>
        /// <value>A SchemaModelElementCollection conatins SimpleAttributeElement objects for unique constraint keys</value>
        [
            CategoryAttribute("System"),
            DescriptionAttribute("A collection of attributes that define an unique constraint of the class."),
            EditorAttribute("Newtera.Studio.UniqueKeysPropertyEditor, Studio", typeof(UITypeEditor))
        ]
        public SchemaModelElementCollection UniqueKeys
        {
            get
            {
                return _uniqueKeys;
            }
            set
            {
                _uniqueKeys = value;
            }
        }

        /// <summary>
        /// Gets or sets name of the attribute used for default sorting.
        /// </summary>
        /// <value>A name of an SimpleAttributeElement</value>
        [
            CategoryAttribute("Sort"),
            DescriptionAttribute("The name of an attribute used for default sorting."),
            TypeConverterAttribute("Newtera.Studio.SortAttributeNameConverter, Studio"),
            EditorAttribute("Newtera.Studio.SortAttributePropertyEditor, Studio", typeof(UITypeEditor)),
            DefaultValueAttribute("None")
        ]
        public string SortAttribute
        {
            get
            {
                return _sortAttributeName;
            }
            set
            {
                _sortAttributeName = value;
            }
        }

        /// <summary>
        /// Gets or sets direction of default sorting.
        /// </summary>
        /// <value>One of enum SortDirection values.</value>
        [
            CategoryAttribute("Sort"),
            DescriptionAttribute("The direction of the default sorting."),
            DefaultValueAttribute(SortDirection.Ascending)
        ]
        public SortDirection SortDirection
        {
            get
            {
                return _sortDirection;
            }
            set
            {
                _sortDirection = value;
            }
        }

        /// <summary>
        /// Gets or sets a name of the large class image.
        /// </summary>
        /// <value>An image name</value>
        ///[
        ///    CategoryAttribute("Appearance"),
		///	DescriptionAttribute("The name of a large class image that can be displayed on UI."),
	    ///EditorAttribute("Newtera.Studio.ImageNamePropertyEditor, Studio", typeof(UITypeEditor))		
		///]
        [BrowsableAttribute(false)]
        public string LargeImage
		{
			get
			{
				return _largeImage;
			}
			set
			{
				_largeImage = value;
			}
		}

        /// <summary>
        /// Gets or sets a name of the median class image.
        /// </summary>
        /// <value>An image name</value>
        [BrowsableAttribute(false)]
        public string MedianImage
		{
			get
			{
				return _medianImage;
			}
			set
			{
				_medianImage = value;
			}
		}

        /// <summary>
        /// Gets or sets a name of the small class image.
        /// </summary>
        /// <value>An image name</value>
        [BrowsableAttribute(false)]
        public string SmallImage
		{
			get
			{
				return _smallImage;
			}
			set
			{
				_smallImage = value;
			}
		}

        /// <summary>
        /// Gets or sets a detailed text of the class.
        /// </summary>
        /// <value>A detailed text</value>
        [BrowsableAttribute(false)]
        public string DetailedText
		{
			get
			{
				return _text;
			}
			set
			{
				_text = value;
			}
		}

        /// <summary>
        /// Gets or sets an url of a customized web page for displaying class data.
        /// </summary>
        /// <value>
        /// a string representing a base url
        /// </value>
        /// <remarks>This value is used by web clients</remarks>
        [BrowsableAttribute(false)]
        public string ClassPageUrl
        {
            get
            {
                return _classPageUrl;
            }
            set
            {
                _classPageUrl = value;
            }
        }

        /// <summary>
        /// Gets or sets an url of a customized web page for displaying instance data.
        /// </summary>
        /// <value>
        /// a string representing a base url
        /// </value>
        /// <remarks>This value is used by web clients</remarks>
        [BrowsableAttribute(false)]
        public string InstancePageUrl
        {
            get
            {
                return _instancePageUrl;
            }
            set
            {
                _instancePageUrl = value;
            }
        }

        /// <summary>
        /// Gets or sets information indicating whether to show the classes related to the instance
        /// on web user interface 
        /// </summary>
        /// <remarks>This value is used by web clients</remarks>
        [BrowsableAttribute(false)]
        public bool ShowRelatedClasses
        {
            get
            {
                return _showRelatedClasses;
            }
            set
            {
                _showRelatedClasses = value;
            }
        }

        /// <summary>
        /// Gets or sets the data view name which is used as a nested table in a form
        /// </summary>
        /// <value>The data view name, can be null.</value>		
        [
            CategoryAttribute("Appearance"),
            DescriptionAttribute("The data view which is used to display a nested table in a form"),
            DefaultValueAttribute(null),
            TypeConverterAttribute("Newtera.Studio.DataViewNameConverter, Studio"),
            EditorAttribute("Newtera.Studio.DataViewNamePropertyEditor, Studio", typeof(UITypeEditor)),
        ]
        public string NestedDataViewName
        {
            get
            {
                return _dataViewName;
            }
            set
            {
                _dataViewName = value;
            }
        }

		/// <summary>
		/// Gets or sets the table name.
		/// </summary>
		/// <value>DB Table Name</value>
        [
            CategoryAttribute("System"),
            DescriptionAttribute("The corresponding table name"),
            ReadOnlyAttribute(true),
        ]
		public string TableName
		{
			get
			{
				return _tableName;
			}
			set
			{
				_tableName = value;
			}
		}

        /// <summary>
        /// Gets the information indicating whether the class is full-text search enabled.
        /// A class is full-text search enabled is that the class has a local or inherited
        /// simple attribute which is used for full-text search purpose.
        /// </summary>
        /// <value>true if the class is full-text search-enabled, false otherise</value>
        [BrowsableAttribute(false)]
        public bool IsFullTextSearchEnabled
        {
            get
            {
                bool status = false;

                ClassElement current = this;
                while (current != null)
                {
                    foreach (SimpleAttributeElement attribute in current.SimpleAttributes)
                    {
                        if (attribute.IsFullTextSearchable)
                        {
                            status = true;
                            break;
                        }
                    }

                    current = current.ParentClass;
                }

                return status;
            }
        }

        /// <summary>
        /// Gets or sets the information indicating whether the class is used as a junction for an
        /// many-to-many relationship.
        /// </summary>
        /// <value>true if it is junction, false otherwise, default is false</value>
        /// <remarks>If class is a junction class, it cannot have a sunclass.</remarks>
        [
            CategoryAttribute("Junction"),
            DescriptionAttribute("Is is a junction class?"),
            DefaultValueAttribute(false)
        ]
        public bool IsJunction
        {
            get
            {
                return _isJunction;
            }
            set
            {
                _isJunction = value;
            }
        }

        /// <summary>
        /// Gets or sets the custom pages.
        /// </summary>
        /// <value>
        /// An collection of CustomPage objects
        /// </value>
        ///[
        ///    CategoryAttribute("Appearance"),
        ///    DescriptionAttribute("A collection of custom pages to display as related info on web UI"),
        ///    EditorAttribute("Newtera.Studio.CustomPageCollectionEditor, Studio", typeof(UITypeEditor)),
        ///]
        [BrowsableAttribute(false)]
        public SchemaModelElementCollection CustomPages
        {
            get
            {
                return _customPages;
            }
            set
            {
                _customPages = value;

                FireValueChangedEvent(value); // fire a Value changed event
            }
        }

        /// <summary>
        /// Gets or sets the code for instance initialization
        /// </summary>
        [
            CategoryAttribute("Method"),
            DescriptionAttribute("The initialization code"),
            EditorAttribute("Newtera.Studio.CodeEditor, Studio", typeof(UITypeEditor)),
        ]
        public string InitializationCode
        {
            get
            {
                return _initializationCode;
            }
            set
            {
                _initializationCode = value;
            }
        }

        /// <summary>
        /// Gets or sets the code executed before updating an instance
        /// </summary>
        [
            CategoryAttribute("Method"),
            DescriptionAttribute("The before update code"),
            EditorAttribute("Newtera.Studio.CodeEditor, Studio", typeof(UITypeEditor)),
        ]
        public string BeforeUpdateCode
        {
            get
            {
                return _beforeUpdateCode;
            }
            set
            {
                _beforeUpdateCode = value;
            }
        }

        /// <summary>
        /// Gets or sets the code executed before inserting an instance
        /// </summary>
        [
            CategoryAttribute("Method"),
            DescriptionAttribute("The before insert code"),
            EditorAttribute("Newtera.Studio.CodeEditor, Studio", typeof(UITypeEditor)),
        ]
        public string BeforeInsertCode
        {
            get
            {
                return _beforeInsertCode;
            }
            set
            {
                _beforeInsertCode = value;
            }
        }

        /// <summary>
        /// Gets or sets the code for callback function
        /// </summary>
        [
            CategoryAttribute("Method"),
            DescriptionAttribute("The callback function code"),
            EditorAttribute("Newtera.Studio.CodeEditor, Studio", typeof(UITypeEditor)),
        ]
        public string CallbackFunctionCode
        {
            get
            {
                return _callbackFunctionCode;
            }
            set
            {
                _callbackFunctionCode = value;
            }
        }

        /// <summary>
        /// Gets or sets the xquery condition to match the instances during batch loading
        /// </summary>
        [
            CategoryAttribute("Match"),
            DescriptionAttribute("The instance macthing condition"),
            EditorAttribute("Newtera.WindowsControl.MultipleLineTextEditor, Newtera.WindowsControl", typeof(UITypeEditor))
        ]
        public string MatchCondition
        {
            get
            {
                return _matchCondition;
            }
            set
            {
                _matchCondition = value;
            }
        }

        /// <summary>
        /// If the class element is used as a junction class for a many-to-many relationship,
        /// there will be a pair of many-to-one relationship attributes exist in this class.
        /// This method is used to find another relationship attributes in the pair, given one
        /// of the relationship.
        /// </summary>
        /// <param name="relationshipAttribute">The relationship attribute that connects to the junction class</param>
        /// <returns>The another relationship attribute.</returns>
        /// <exception cref="Exception">Thrown if unable to find the paired relationship attribute.</exception>
        public RelationshipAttributeElement FindPairedRelationshipAttribute(RelationshipAttributeElement relationshipAttribute)
        {
            if (!this.IsJunction)
            {
                // this method is valid if the class is a junction class
                throw new Exception("Invalid method call on a class " + this.Name + " that is not a junction class.");
            }

            RelationshipAttributeElement otherRelationshipAttribute = null;
            foreach (RelationshipAttributeElement ra in this.RelationshipAttributes)
            {
                if (ra.Type == RelationshipType.ManyToOne &&
                    ra.LinkedClass.Name != relationshipAttribute.OwnerClass.Name)
                {
                    otherRelationshipAttribute = ra;
                    break;
                }

                /*
                if ((ra.BackwardRelationship.Name != relationshipAttribute.Name ||
                    ra.OwnerClass.Name != relationshipAttribute.OwnerClass.Name) &&
                    ra.Type == RelationshipType.ManyToOne)
                {
                    otherRelationshipAttribute = ra;
                    break;
                }
                 */
            }

            if (otherRelationshipAttribute == null)
            {
                throw new Exception("Unable to find a paired relationship that connects to a class on a many-side in junction class " + this.Name);
            }

            return otherRelationshipAttribute;
        }
		
		/// <summary>
		/// Accept a visitor of ISchemaModelElementVisitor type to visit itself and
		/// have the subclasses and attributes to accept the visitor next.
		/// </summary>
		/// <param name="visitor">The visitor</param>
		public override void Accept(ISchemaModelElementVisitor visitor)
		{
			visitor.VisitClassElement(this);

			foreach (SchemaModelElement element in _simpleAttributes)
			{
				element.Accept(visitor);
			}

			foreach (SchemaModelElement element in _relationshipAttributes)
			{
				element.Accept(visitor);
			}

			foreach (SchemaModelElement element in _arrayAttributes)
			{
				element.Accept(visitor);
			}

            foreach (SchemaModelElement element in _virtualAttributes)
            {
                element.Accept(visitor);
            }

            foreach (SchemaModelElement element in _imageAttributes)
            {
                element.Accept(visitor);
            }

			foreach (SchemaModelElement element in _subclasses)
			{
				element.Accept(visitor);
			}
		}

		/// <summary>
		/// Add a class as a subclass.
		/// </summary>
		/// <param name="subclass">The subclass object</param>
		/// <returns> The ClassElement object created for the subclass.</returns>
		public ClassElement AddSubclass(ClassElement subclass)
		{	
			subclass.ParentClass = this;

			// add to the subclasses list
			_subclasses.Add(subclass);
			
			return subclass;
		}
	
		/// <summary>
		/// Remove a subclass.
		/// </summary>
		/// <param name="subclass">the ClassElement object to be removed</param>
		public void RemoveSubclass(ClassElement subclass)
		{
			if (_subclasses.Contains(subclass))
			{
				// remove the subclasses of the subclass
				subclass.RemoveSubclasses();

				SchemaModel.RemoveFromClassTable(subclass);			
				_subclasses.Remove(subclass);
			}
		}
		
		/// <summary>
		/// Adds a simple attribute to the class.
		/// </summary>
		/// <param name="attribute">The SimpleAttributeElement object to be added
		/// </param>
		/// <returns> Added SimpleAttributeElement object.
		/// </returns>
		public SimpleAttributeElement AddSimpleAttribute(SimpleAttributeElement attribute)
		{
			// Make sure an attribute of the same name does not exist
			foreach (SimpleAttributeElement att in _simpleAttributes) 
			{
				if (att.Name == attribute.Name)
				{
					throw new DuplicateAttributeNameException("Attrinute with name" + attribute.Name + " already exists.");
				}
			}

			// Set owner class.
			attribute.OwnerClass = this;

			attribute.SchemaModel = SchemaModel;
			
			// Add it to simple attributes list.
			_simpleAttributes.Add(attribute);
			
			return attribute;
		}

		/// <summary>
		/// Adds a primary key to the primary key collection
		/// </summary>
		/// <param name="attribute">The SimpleAttrinuteElement instance as a primary key
		/// </param>
		public void AddPrimaryKey(SimpleAttributeElement attribute)
		{
			if (!_primaryKeys.Contains(attribute))
			{
				_primaryKeys.Add(attribute);
				attribute.IsPrimaryKey = true;
			}
		}

		/// <summary>
		/// Get the SimpleAttributeElement by name
		/// </summary>
		/// <param name="name">the name of attribute</param>
		/// <returns>The SimpleAttributeElement object</returns>
		/// <remarks>The attribute name is case-insensitive</remarks>
		public SimpleAttributeElement FindSimpleAttribute(string name)
		{
			SimpleAttributeElement found = null;

			foreach (SimpleAttributeElement att in _simpleAttributes) 
			{
				if (att.Name.ToUpper() == name.ToUpper())
				{
					found = att;
					break;
				}
			}
			
			return found;
		}
	
		/// <summary>
		/// Get the SimpleAttributeElement by name. This method will search
		/// up the hieararchy if fails to find it at local class element
		/// </summary>
		/// <param name="name">the name of attribute</param>
		/// <returns>The SimpleAttributeElement object</returns>
		/// <remarks>The attribute name is case-insensitive</remarks> 
		public SimpleAttributeElement FindInheritedSimpleAttribute(string name)
		{
			SimpleAttributeElement found = null;

			ClassElement classElement = this;

			while (classElement != null)
			{
				found = classElement.FindSimpleAttribute(name);
				if (found == null)
				{
					classElement = classElement.ParentClass;
				}
				else
				{
					break;
				}
			}
			
			return found;
		}

		/// <summary>
		/// Remove a simple attribute from the class.
		/// </summary>
		/// <param name="name">the name of attribute to be removed
		/// </param>
		public SimpleAttributeElement RemoveSimpleAttribute(string name)
		{
			SimpleAttributeElement attribute = null;

			foreach (SimpleAttributeElement att in _simpleAttributes) 
			{
				if (att.Name == name)
				{
					attribute = att;
					break;
				}
			}
			
			if (attribute != null) 
			{
				RemoveSimpleAttribute(attribute);
			}

			return attribute;
		}

		/// <summary>
		/// Remove a simple attribute from the class.
		/// </summary>
		/// <param name="attribute">the SimpleAttributeElement object to be removed
		/// </param>
		public SimpleAttributeElement RemoveSimpleAttribute(SimpleAttributeElement attribute)
		{
			if (_simpleAttributes.Contains(attribute))
			{
				// Remove it from simple attributes list.
				_simpleAttributes.Remove(attribute);

                // remove it from primary key collection
				if (attribute.IsPrimaryKey)
				{
					this.RemovePrimaryKey(attribute);
				}

                // remove it from unique key collection
                if (_uniqueKeys.Contains(attribute))
                {
                    _uniqueKeys.Remove(attribute);
                }
			}
			
			return attribute;			
		}

		/// <summary>
		/// Removes a primary key from the primary key collection
		/// </summary>
		/// <param name="attribute">The SimpleAttrinuteElement instance to be removed from the collection.
		/// </param>
		public void RemovePrimaryKey(SimpleAttributeElement attribute)
		{
			if (_primaryKeys.Contains(attribute))
			{
				_primaryKeys.Remove(attribute);
				attribute.IsPrimaryKey = false;
			}
		}
		
		/// <summary>
		/// Add a relationship attribute to the class.
		/// </summary>
		/// <param name="attribute">The RelationshipAttributeElement object to be added
		/// </param>
		/// <returns> Added RelationshipAttributeElement object.
		/// </returns>
		public RelationshipAttributeElement AddRelationshipAttribute(RelationshipAttributeElement attribute)
		{
			// Make sure an attribute of the same name does not exist
			foreach (RelationshipAttributeElement att in _relationshipAttributes) 
			{
				if (att.Name == attribute.Name)
				{
					throw new DuplicateAttributeNameException("Attrinute with name" + attribute.Name + " already exists.");
				}
			}

			// Set owner class.
			attribute.OwnerClass = this;

			attribute.SchemaModel = this.SchemaModel;
			
			// Add it to relationship attributes list.
			_relationshipAttributes.Add(attribute);
			
			return attribute;
		}

		/// <summary>
		/// Get the RelationshipAttributeElement by name
		/// </summary>
		/// <param name="name">the name of attribute</param>
		/// <returns>The RelationshipAttributeElement object</returns>
		/// <remarks>The attribute name is case-insensitive</remarks> 
		public RelationshipAttributeElement FindRelationshipAttribute(string name)
		{
			RelationshipAttributeElement found = null;

			foreach (RelationshipAttributeElement att in _relationshipAttributes) 
			{
				if (att.Name.ToUpper() == name.ToUpper())
				{
					found = att;
					break;
				}
			}
			
			return found;
		}

		/// <summary>
		/// Get the RelationshipAttributeElement by name. This method will search
		/// up the hieararchy if fails to find it at local class element
		/// </summary>
		/// <param name="name">the name of attribute</param>
		/// <returns>The RelationshipAttributeElement object</returns>
		/// <remarks>The attribute name is case-insensitive</remarks> 
		public RelationshipAttributeElement FindInheritedRelationshipAttribute(string name)
		{
			RelationshipAttributeElement found = null;

			ClassElement classElement = this;

			while (classElement != null)
			{
				found = classElement.FindRelationshipAttribute(name);
				if (found == null)
				{
					classElement = classElement.ParentClass;
				}
				else
				{
					break;
				}
			}
			
			return found;
		}

		/// <summary>
		/// Remove a relationship attribute from the class.
		/// </summary>
		/// <param name="name">the name of attribute to be removed
		/// </param>
		public RelationshipAttributeElement RemoveRelationshipAttribute(string name)
		{
			RelationshipAttributeElement attribute = null;

			foreach (RelationshipAttributeElement att in _relationshipAttributes) 
			{
				if (att.Name == name)
				{
					attribute = att;
					break;
				}
			}
			
			if (attribute != null) 
			{
				RemoveRelationshipAttribute(attribute);
			}

			return attribute;
		}

		/// <summary>
		/// Remove a relationship attribute from the class.
		/// </summary>
		/// <param name="attribute">the RelationshipAttributeElement object to be removed
		/// </param>
		public RelationshipAttributeElement RemoveRelationshipAttribute(RelationshipAttributeElement attribute)
		{
			if (_relationshipAttributes.Contains(attribute))
			{
				// Remove it from relationship attributes list.
				_relationshipAttributes.Remove(attribute);
			}

            // remove it from unique key collection
            if (_uniqueKeys.Contains(attribute))
            {
                _uniqueKeys.Remove(attribute);
            }
			
			return attribute;			
		}

		/// <summary>
		/// Adds an array attribute to the class.
		/// </summary>
		/// <param name="attribute">The ArrayAttributeElement object to be added
		/// </param>
		/// <returns> Added ArrayAttributeElement object.
		/// </returns>
		public ArrayAttributeElement AddArrayAttribute(ArrayAttributeElement attribute)
		{
			// Make sure an attribute of the same name does not exist
			foreach (ArrayAttributeElement att in _arrayAttributes) 
			{
				if (att.Name == attribute.Name)
				{
					throw new DuplicateAttributeNameException("Attrinute with name" + attribute.Name + " already exists.");
				}
			}

			// Set owner class.
			attribute.OwnerClass = this;

			attribute.SchemaModel = SchemaModel;
			
			// Add it to array attributes list.
			_arrayAttributes.Add(attribute);
			
			return attribute;
		}

        /// <summary>
        /// Adds a virtual attribute to the class.
        /// </summary>
        /// <param name="attribute">The VirtualAttributeElement object to be added
        /// </param>
        /// <returns> Added VirtualAttributeElement object.
        /// </returns>
        public VirtualAttributeElement AddVirtualAttribute(VirtualAttributeElement attribute)
        {
            // Make sure an attribute of the same name does not exist
            foreach (VirtualAttributeElement att in _virtualAttributes)
            {
                if (att.Name == attribute.Name)
                {
                    throw new DuplicateAttributeNameException("Attrinute with name" + attribute.Name + " already exists.");
                }
            }

            // Set owner class.
            attribute.OwnerClass = this;

            attribute.SchemaModel = SchemaModel;

            // Add it to virtual attributes list.
            _virtualAttributes.Add(attribute);

            return attribute;
        }

        /// <summary>
        /// Adds an image attribute to the class.
        /// </summary>
        /// <param name="attribute">The ImageAttributeElement object to be added
        /// </param>
        /// <returns> Added ImageAttributeElement object.
        /// </returns>
        public ImageAttributeElement AddImageAttribute(ImageAttributeElement attribute)
        {
            // Make sure an attribute of the same name does not exist
            foreach (ImageAttributeElement att in _imageAttributes)
            {
                if (att.Name == attribute.Name)
                {
                    throw new DuplicateAttributeNameException("Attrinute with name" + attribute.Name + " already exists.");
                }
            }

            // Set owner class.
            attribute.OwnerClass = this;

            attribute.SchemaModel = SchemaModel;

            // Add it to image attributes list.
            _imageAttributes.Add(attribute);

            return attribute;
        }

		/// <summary>
		/// Get the ArrayAttributeElement by name
		/// </summary>
		/// <param name="name">the name of attribute</param>
		/// <returns>The ArrayAttributeElement object</returns>
		/// <remarks>The attribute name is case-insensitive</remarks>
		public ArrayAttributeElement FindArrayAttribute(string name)
		{
			ArrayAttributeElement found = null;

			foreach (ArrayAttributeElement att in _arrayAttributes) 
			{
				if (att.Name.ToUpper() == name.ToUpper())
				{
					found = att;
					break;
				}
			}
			
			return found;
		}

        /// <summary>
        /// Get the VirtualAttributeElement by name
        /// </summary>
        /// <param name="name">the name of attribute</param>
        /// <returns>The VirtualAttributeElement object</returns>
        /// <remarks>The attribute name is case-insensitive</remarks>
        public VirtualAttributeElement FindVirtualAttribute(string name)
        {
            VirtualAttributeElement found = null;

            foreach (VirtualAttributeElement att in _virtualAttributes)
            {
                if (att.Name.ToUpper() == name.ToUpper())
                {
                    found = att;
                    break;
                }
            }

            return found;
        }

        /// <summary>
        /// Get the ImageAttributeElement by name
        /// </summary>
        /// <param name="name">the name of attribute</param>
        /// <returns>The ImageAttributeElement object</returns>
        /// <remarks>The attribute name is case-insensitive</remarks>
        public ImageAttributeElement FindImageAttribute(string name)
        {
            ImageAttributeElement found = null;

            foreach (ImageAttributeElement att in _imageAttributes)
            {
                if (att.Name.ToUpper() == name.ToUpper())
                {
                    found = att;
                    break;
                }
            }

            return found;
        }
	
		/// <summary>
		/// Get the ArrayAttributeElement by name. This method will search
		/// up the hieararchy if fails to find it at local class element
		/// </summary>
		/// <param name="name">the name of attribute</param>
		/// <returns>The ArrayAttributeElement object</returns>
		/// <remarks>The attribute name is case-insensitive</remarks> 
		public ArrayAttributeElement FindInheritedArrayAttribute(string name)
		{
			ArrayAttributeElement found = null;

			ClassElement classElement = this;

			while (classElement != null)
			{
				found = classElement.FindArrayAttribute(name);
				if (found == null)
				{
					classElement = classElement.ParentClass;
				}
				else
				{
					break;
				}
			}
			
			return found;
		}

        /// <summary>
        /// Get the VirtualAttributeElement by name. This method will search
        /// up the hieararchy if fails to find it at local class element
        /// </summary>
        /// <param name="name">the name of attribute</param>
        /// <returns>The VirtualAttributeElement object</returns>
        /// <remarks>The attribute name is case-insensitive</remarks> 
        public VirtualAttributeElement FindInheritedVirtualAttribute(string name)
        {
            VirtualAttributeElement found = null;

            ClassElement classElement = this;

            while (classElement != null)
            {
                found = classElement.FindVirtualAttribute(name);
                if (found == null)
                {
                    classElement = classElement.ParentClass;
                }
                else
                {
                    break;
                }
            }

            return found;
        }

        /// <summary>
        /// Get the ImageAttributeElement by name. This method will search
        /// up the hieararchy if fails to find it at local class element
        /// </summary>
        /// <param name="name">the name of attribute</param>
        /// <returns>The ImageAttributeElement object</returns>
        /// <remarks>The attribute name is case-insensitive</remarks> 
        public ImageAttributeElement FindInheritedImageAttribute(string name)
        {
            ImageAttributeElement found = null;

            ClassElement classElement = this;

            while (classElement != null)
            {
                found = classElement.FindImageAttribute(name);
                if (found == null)
                {
                    classElement = classElement.ParentClass;
                }
                else
                {
                    break;
                }
            }

            return found;
        }

		/// <summary>
		/// Remove an array attribute from the class.
		/// </summary>
		/// <param name="name">the name of attribute to be removed
		/// </param>
		public ArrayAttributeElement RemoveArrayAttribute(string name)
		{
			ArrayAttributeElement attribute = null;

			foreach (ArrayAttributeElement att in _arrayAttributes) 
			{
				if (att.Name == name)
				{
					attribute = att;
					break;
				}
			}
			
			if (attribute != null) 
			{
				RemoveArrayAttribute(attribute);
			}

			return attribute;
		}

        /// <summary>
        /// Remove a virtual attribute from the class.
        /// </summary>
        /// <param name="name">the name of attribute to be removed
        /// </param>
        public VirtualAttributeElement RemoveVirtualAttribute(string name)
        {
            VirtualAttributeElement attribute = null;

            foreach (VirtualAttributeElement att in _virtualAttributes)
            {
                if (att.Name == name)
                {
                    attribute = att;
                    break;
                }
            }

            if (attribute != null)
            {
                RemoveVirtualAttribute(attribute);
            }

            return attribute;
        }

        /// <summary>
        /// Remove an image attribute from the class.
        /// </summary>
        /// <param name="name">the name of attribute to be removed
        /// </param>
        public ImageAttributeElement RemoveImageAttribute(string name)
        {
            ImageAttributeElement attribute = null;

            foreach (ImageAttributeElement att in _imageAttributes)
            {
                if (att.Name == name)
                {
                    attribute = att;
                    break;
                }
            }

            if (attribute != null)
            {
                RemoveImageAttribute(attribute);
            }

            return attribute;
        }

		/// <summary>
		/// Remove an array attribute from the class.
		/// </summary>
		/// <param name="attribute">the ArrayAttributeElement object to be removed
		/// </param>
		public ArrayAttributeElement RemoveArrayAttribute(ArrayAttributeElement attribute)
		{
			if (_arrayAttributes.Contains(attribute))
			{
				// Remove it from array attributes list.
				_arrayAttributes.Remove(attribute);
			}
			
			return attribute;			
		}

        /// <summary>
        /// Remove a virtual attribute from the class.
        /// </summary>
        /// <param name="attribute">the VirtualAttributeElement object to be removed
        /// </param>
        public VirtualAttributeElement RemoveVirtualAttribute(VirtualAttributeElement attribute)
        {
            if (_virtualAttributes.Contains(attribute))
            {
                // Remove it from virtual attributes list.
                _virtualAttributes.Remove(attribute);
            }

            return attribute;
        }

        /// <summary>
        /// Remove an image attribute from the class.
        /// </summary>
        /// <param name="attribute">the ImageAttributeElement object to be removed
        /// </param>
        public ImageAttributeElement RemoveImageAttribute(ImageAttributeElement attribute)
        {
            if (_imageAttributes.Contains(attribute))
            {
                // Remove it from image attributes list.
                _imageAttributes.Remove(attribute);
            }

            return attribute;
        }

		/// <summary>
		/// Find the attribute, including simple and relationship, from
		/// this class, its parent, and its subclasses.
		/// </summary>
		/// <param name="name">The name of attribute</param>
		/// <param name="direction">The search direction</param>
		/// <returns></returns>
		public AttributeElementBase FindAttribute(string name, SearchDirection direction)
		{
			AttributeElementBase found = null;

			if (_simpleAttributes != null)
			{
				found = FindSimpleAttribute(name);
			}

			if (found == null && _relationshipAttributes != null)
			{
				found = FindRelationshipAttribute(name);
			}

			if (found == null && _arrayAttributes != null)
			{
				found = FindArrayAttribute(name);
			}

            if (found == null && _virtualAttributes != null)
            {
                found = FindVirtualAttribute(name);
            }

            if (found == null && _imageAttributes != null)
            {
                found = FindImageAttribute(name);
            }

			if (direction == SearchDirection.TwoWay || direction == SearchDirection.Upward)
			{
				// search its parent class if it exists
				if (found == null && ParentClass != null)
				{
					found = ParentClass.FindAttribute(name, SearchDirection.Upward);
				}
			}

			if (direction == SearchDirection.TwoWay || direction == SearchDirection.Downward)
			{
				// search its subclasses if they exists
				if (found == null && _subclasses != null)
				{
					foreach (ClassElement subclass in _subclasses)
					{
						found = subclass.FindAttribute(name, SearchDirection.Downward);
						if (found != null)
						{
							break;
						}
					}
				}
			}

			return found;
		}

		/// <summary>
		/// Remove the subclasses of the class from the schema
		/// </summary>
		public void RemoveSubclasses()
		{
			// Since RemoveSunclass call will delete an item at real time,
			// we have to clone the subclass collection
			SchemaModelElementCollection subclasses = new SchemaModelElementCollection();
			foreach (ClassElement subclass in _subclasses)
			{
				subclasses.Add(subclass);
			}

			// now we are ready to remove the subclasses
			foreach (ClassElement subclass in subclasses)
			{
				RemoveSubclass(subclass);
			}
		}

		/// <summary>
		/// Get all leaf classes of the class. If the class itself is a leaf class,
		/// return it self as the single element in the collection
		/// </summary>
		/// <returns>A SchemaModelElementCollection of leaf class</returns>
		public SchemaModelElementCollection GetLeafClasses()
		{
			SchemaModelElementCollection leafClasses = new SchemaModelElementCollection();

			AddLeafClasses(this, leafClasses);

			return leafClasses;
		}

        /// <summary>
        /// Gets the parent class of the given name
        /// </summary>
        /// <returns>A ClassElement object if found, null if not found</returns>
        public ClassElement FindParentClass(string className)
        {
            ClassElement parentClass = this.ParentClass;

            while (parentClass != null)
            {
                if (parentClass.Name == className)
                {
                    break;
                }
                else
                {
                    parentClass = parentClass.ParentClass;
                }
            }

            return parentClass;
        }

		#region IXaclObject Members

		/// <summary>
		/// Return a xpath representation of the SchemaModelElement
		/// </summary>
		/// <returns>a xapth representation</returns>
		public override string ToXPath()
		{
			if (_xpath == null)
			{
				_xpath = this.Parent.ToXPath() + "/" + this.Name;
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
				if (this.ParentClass != null)
				{
					// return the immediate inherited class
					return this.ParentClass;
				}
				else
				{
					// return the schema model
					return this.SchemaModel;
				}
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
            children.Add(this.SimpleAttributes);
            children.Add(this.ArrayAttributes);
            children.Add(this.RelationshipAttributes);
            children.Add(this.ImageAttributes);
            children.Add(this.VirtualAttributes);
            children.Add(this.Subclasses);
            return children.GetEnumerator();
		}

		#endregion

		/// <summary>
		/// Return the name of the ClassElement
		/// </summary>
		protected override string ElementName
		{
			get
			{
				return ((XmlSchemaComplexType) XmlSchemaElement).Name;
			}
		}
		
		/// <summary>
		/// Create xml schema element as an internal representation
		/// of Schema Model element.
		/// </summary>
		/// <returns> Return an XmlSchemaAnnotated object</returns>
		protected override XmlSchemaAnnotated CreateXmlSchemaElement(string name)
		{
			XmlSchemaComplexType complexType = new XmlSchemaComplexType();
			complexType.Name = name;
			if (IsRoot) 
			{
				complexType.Particle = new XmlSchemaSequence();

				// Add obj_id attribute to the complexType for root class
				AddObjIdAttribute(complexType);
			} 
			else 
			{
				// Create a complex type with extension
				XmlSchemaComplexContent complexContent = new XmlSchemaComplexContent();
				complexType.ContentModel = complexContent;
        
				// <xs:extension base="name">
				XmlSchemaComplexContentExtension extensionAddress = new XmlSchemaComplexContentExtension();
				complexContent.Content = extensionAddress;
				extensionAddress.BaseTypeName = new XmlQualifiedName(this.ParentClass.Name);
        
				extensionAddress.Particle = new XmlSchemaSequence();				
			}

			return complexType;
		}

		/// <summary>
		/// Create the member objects from a XML Schema Model
		/// </summary>
		internal override void Unmarshal()
		{
			Reset();

			// first give the base a chance to do its own marshalling
			base.Unmarshal();

            // Set _category member
            _category = this.GetNewteraAttributeValue(NewteraNameSpace.CATEGORY);

			// Set TableName member
			_tableName = GetNewteraAttributeValue(NewteraNameSpace.TABLE_NAME);

            // Sort attribute member
            _sortAttributeName = GetNewteraAttributeValue(NewteraNameSpace.SORT_ATTRIBUTE);
            if (string.IsNullOrEmpty(_sortAttributeName))
            {
                _sortAttributeName = NewteraNameSpace.NONE;
            }

            // Sort direction member
            string str = GetNewteraAttributeValue(NewteraNameSpace.SORT_DIRECTION);
            if (!string.IsNullOrEmpty(str))
            {
                _sortDirection = (SortDirection) Enum.Parse(typeof(SortDirection), str);
            }
            else
            {
                _sortDirection = SortDirection.Ascending;
            }

			// Set _largeImage member
			_largeImage = this.GetNewteraAttributeValue(NewteraNameSpace.LARGE_IMAGE);

			// Set _medianImage member
			_medianImage = this.GetNewteraAttributeValue(NewteraNameSpace.MEDIAN_IMAGE);

			// Set _smallImage member
			_smallImage = this.GetNewteraAttributeValue(NewteraNameSpace.SMALL_IMAGE);

			// Set _text member
			_text = this.GetNewteraAttributeValue(NewteraNameSpace.TEXT);

            // Set _isBrowsable member
            if (this.GetNewteraAttributeValue(NewteraNameSpace.BROWSABLE) != null)
            {
                _isBrowsable = false;
            }
            else
            {
                _isBrowsable = true; // default
            }

            // Set _isJunction member
            if (this.GetNewteraAttributeValue(NewteraNameSpace.JUNCTION) != null)
            {
                _isJunction = true;
            }
            else
            {
                _isJunction = false; // default
            }

            // Set _classPageUrl member
            _classPageUrl = this.GetNewteraAttributeValue(NewteraNameSpace.CLASS_PAGE_URL);

            // Set _instancePageUrl member
            _instancePageUrl = this.GetNewteraAttributeValue(NewteraNameSpace.INSTANCE_PAGE_URL);

            // Set _showRelatedClasses member
            if (this.GetNewteraAttributeValue(NewteraNameSpace.SHOW_RELATED_CLASSES) != null)
            {
                _showRelatedClasses = false;
            }
            else
            {
                _showRelatedClasses = true; // default
            }

            // Set InitializationCode member
            this._initializationCode = this.GetNewteraAttributeValue(NewteraNameSpace.INITIALIZATION);

            // Set BeforeUpdateCode member
            this._beforeUpdateCode = this.GetNewteraAttributeValue(NewteraNameSpace.BEFORE_UPDATE);

            // Set BeforeInsertCode member
            this._beforeInsertCode = this.GetNewteraAttributeValue(NewteraNameSpace.BEFORE_INSERT);

            // Set CallbackFunctionCode member
            this._callbackFunctionCode = this.GetNewteraAttributeValue(NewteraNameSpace.CALLBACK_FUNCTION);

            // Set MatchCondition member
            this._matchCondition = this.GetNewteraAttributeValue(NewteraNameSpace.MATCH_CONDITION);

            // Set _dataViewName member
            _dataViewName = this.GetNewteraAttributeValue(NewteraNameSpace.DATA_VIEW);

			XmlSchemaComplexType complexType = (XmlSchemaComplexType) XmlSchemaElement;

			SetParentChildren(complexType);

			SetSimpleAttributsAndPrimaryKeys(complexType);

			SetRelationshipAttributes(complexType);

			SetArrayAttributes(complexType);

            SetVirtualAttributes(complexType);

            SetImageAttributes(complexType);

            SetCustomPages(complexType);
		}

		/// <summary>
		/// Write objects to XML Schema Model
		/// </summary>
		/// <remarks>
		/// <!--The example show xml schema for a root class "Product" and a subclass
		/// "Furniture"
		/// 
		///	<xsd:complexType name="Product" psd:displayName="Product" psd:order="0" psd:id="1453">
		///	<xsd:sequence>
		///		<xsd:element name="Name" type="xsd:string" minOccurs="1" maxOccurs="1" psd:displayName="Name" psd:order="0" psd:key="true" psd:id="8459" psd:fullText="true" psd:datastore="column" />
		///		<xsd:element name="Supplier" type="xsd:string" minOccurs="1" maxOccurs="1" psd:displayName="Supplier" psd:order="1" psd:id="8460" psd:key="true" />
		///		<xsd:element name="Price" type="xsd:float" minOccurs="0" nillable="true" maxOccurs="1" psd:displayName="¼Û¸ñ" psd:order="2" psd:id="8461" />
		///		<xsd:element name="Description" minOccurs="0" nillable="true" maxOccurs="1" psd:displayName="Description" psd:order="3" psd:multiLine="true" psd:fullText="true" psd:id="8462" psd:datastore="column">
		///			<xsd:simpleType>
		///				<xsd:restriction base="xsd:string">
		///					<xsd:maxLength value="1000" />
		///				 </xsd:restriction>
		///			</xsd:simpleType>
		///		</xsd:element>
		///	</xsd:sequence>
		///	<xsd:attribute name="lineitems" psd:refType="oneToMany" type="xsd:IDREFS" psd:refClass="LineItem" psd:displayName="lineitems" psd:order="4" psd:refAttr="product" psd:joinManager="true" psd:ownership="looseReferenced" minOccurs="0" maxOccurs="1" psd:id="8463" />
		///	<xsd:attribute name="obj_id" type="xsd:ID" />
		///	</xsd:complexType>
		///	
		///	<xsd:complexType name="Furniture" psd:displayName="Furniture" psd:id="1454">
		///	<xsd:complexContent>
		///		<xsd:extension base="Product">
		///		<xsd:sequence>
		///			<xsd:element name="Weight" type="xsd:float" minOccurs="0" nillable="true" maxOccurs="1" psd:displayName="Weight" psd:order="0" psd:id="8464" />
		///			<xsd:element name="Material" type="xsd:string" minOccurs="0" nillable="true" maxOccurs="1" psd:displayName="Material" psd:order="1" psd:id="8465" />
		///		</xsd:sequence>
		///		</xsd:extension>
		///	</xsd:complexContent>
		///	</xsd:complexType>
		/// -->
		/// </remarks>
		internal override void Marshal()
		{
			// Add a XmlSchemaElement to the schema body to represent instances of the class
			SchemaModel.SchemaBody.AddClassElement(this);

			XmlSchemaComplexType complexType = (XmlSchemaComplexType) XmlSchemaElement;

			WriteSimpleAttributesAndPrimaryKeys(complexType);

			WriteRelationshipAttributes(complexType);

			WriteArrayAttributes(complexType);

            WriteVirtualAttributes(complexType);

            WriteImageAttributes(complexType);

            WriteCustomPages(complexType);

            // Write _category member
            if (!string.IsNullOrEmpty(_category))
            {
                SetNewteraAttributeValue(NewteraNameSpace.CATEGORY, _category);
            }

			// write TableName member to xml schema
			SetNewteraAttributeValue(NewteraNameSpace.TABLE_NAME, _tableName);

            // write _sortAttributeName member
            if (!string.IsNullOrEmpty(_sortAttributeName) && _sortAttributeName != NewteraNameSpace.NONE)
            {
                SetNewteraAttributeValue(NewteraNameSpace.SORT_ATTRIBUTE, _sortAttributeName);
            }

            // write sort direction enum value, Acending is default value
            if (_sortDirection != SortDirection.Ascending)
            {
                SetNewteraAttributeValue(NewteraNameSpace.SORT_DIRECTION, Enum.GetName(typeof(SortDirection), _sortDirection));
            }

			// Write _largeImage member
			if (_largeImage != null && _largeImage.Length > 0)
			{
				SetNewteraAttributeValue(NewteraNameSpace.LARGE_IMAGE, _largeImage);	
			}

			// Write _medianImage member
			if (_medianImage != null && _medianImage.Length > 0)
			{
				SetNewteraAttributeValue(NewteraNameSpace.MEDIAN_IMAGE, _medianImage);	
			}

			// Write _smallImage member
			if (_smallImage != null && _smallImage.Length > 0)
			{
				SetNewteraAttributeValue(NewteraNameSpace.SMALL_IMAGE, _smallImage);	
			}

			// Write _text member
			if (_text != null && _text.Length > 0)
			{
				SetNewteraAttributeValue(NewteraNameSpace.TEXT, _text);	
			}

            // write _isBrowsable member
            if (!_isBrowsable)
            {
                SetNewteraAttributeValue(NewteraNameSpace.BROWSABLE, "false");	
            }

            // write _isJunction member
            if (_isJunction)
            {
                SetNewteraAttributeValue(NewteraNameSpace.JUNCTION, "true");
            }

            // Write _classPageUrl member
            if (!string.IsNullOrEmpty(_classPageUrl))
            {
                SetNewteraAttributeValue(NewteraNameSpace.CLASS_PAGE_URL, _classPageUrl);
            }

            // Write _instancePageUrl member
            if (!string.IsNullOrEmpty(_instancePageUrl))
            {
                SetNewteraAttributeValue(NewteraNameSpace.INSTANCE_PAGE_URL, _instancePageUrl);
            }

            // write _showRelatedClasses member
            if (!_showRelatedClasses)
            {
                SetNewteraAttributeValue(NewteraNameSpace.SHOW_RELATED_CLASSES, "false");
            }

            // Write _initializationCode member
            if (_initializationCode != null && _initializationCode.Length > 0)
            {
                SetNewteraAttributeValue(NewteraNameSpace.INITIALIZATION, SecurityElement.Escape(_initializationCode));
            }

            // Write _beforeUpdateCode member
            if (!string.IsNullOrEmpty(_beforeUpdateCode))
            {
                SetNewteraAttributeValue(NewteraNameSpace.BEFORE_UPDATE, SecurityElement.Escape(_beforeUpdateCode));
            }

            // Write _beforeInsertCode member
            if (!string.IsNullOrEmpty(_beforeInsertCode))
            {
                SetNewteraAttributeValue(NewteraNameSpace.BEFORE_INSERT, SecurityElement.Escape(_beforeInsertCode));
            }

            // Write _callBackFunction member
            if (!string.IsNullOrEmpty(this._callbackFunctionCode))
            {
                SetNewteraAttributeValue(NewteraNameSpace.CALLBACK_FUNCTION, SecurityElement.Escape(_callbackFunctionCode));
            }

            // Write _matchCondition member
            if (!string.IsNullOrEmpty(_matchCondition))
            {
                SetNewteraAttributeValue(NewteraNameSpace.MATCH_CONDITION, SecurityElement.Escape(_matchCondition));
            }

            // Write _dataViewName member
            if (!string.IsNullOrEmpty(_dataViewName))
            {
                SetNewteraAttributeValue(NewteraNameSpace.DATA_VIEW, SecurityElement.Escape(_dataViewName));
            }

			// Note: subclasses have been marshalled in SchemaModel's marshal method

			base.Marshal();
		}

		/// <summary>
		/// Instantiate the members
		/// </summary>
		private void InitBlock() 
		{
            _needToAlter = false;

			_subclasses = new SchemaModelElementCollection();
            if (GlobalSettings.Instance.IsWindowClient)
            {
                _subclasses.ValueChanged += new EventHandler(ValueChangedHandler);
            }
			_simpleAttributes = new SchemaModelElementCollection();
            if (GlobalSettings.Instance.IsWindowClient)
            {
                _simpleAttributes.ValueChanged += new EventHandler(ValueChangedHandler);
            }
			_relationshipAttributes = new SchemaModelElementCollection();
            if (GlobalSettings.Instance.IsWindowClient)
            {
                _relationshipAttributes.ValueChanged += new EventHandler(ValueChangedHandler);
            }
			_arrayAttributes = new SchemaModelElementCollection();
            if (GlobalSettings.Instance.IsWindowClient)
            {
                _arrayAttributes.ValueChanged += new EventHandler(ValueChangedHandler);
            }
            _virtualAttributes = new SchemaModelElementCollection();
            if (GlobalSettings.Instance.IsWindowClient)
            {
                _virtualAttributes.ValueChanged += new EventHandler(ValueChangedHandler);
            }
            _imageAttributes = new SchemaModelElementCollection();
            if (GlobalSettings.Instance.IsWindowClient)
            {
                _imageAttributes.ValueChanged += new EventHandler(ValueChangedHandler);
            }
			_primaryKeys = new SchemaModelElementCollection();
            _uniqueKeys = new SchemaModelElementCollection();
            _sortAttributeName = NewteraNameSpace.NONE;
            _sortDirection = SortDirection.Ascending;
            _customPages = new SchemaModelElementCollection();
		}

		/// <summary>
		/// Reset the class element
		/// </summary>
		private void Reset()
		{
			_parentClass = null;
			_subclasses.Clear();
			_simpleAttributes.Clear();
			_relationshipAttributes.Clear();
			_arrayAttributes.Clear();
            _virtualAttributes.Clear();
            _imageAttributes.Clear();
			_primaryKeys.Clear();
            _uniqueKeys.Clear();
		}

		/// <summary>
		/// Set the parent and children relationship.
		/// </summary>
		/// <param name="complexType">The xml schema complexType</param>
		private void SetParentChildren(XmlSchemaComplexType complexType)
		{
			// set the parent class
			XmlSchemaComplexContent complexContent = (XmlSchemaComplexContent) complexType.ContentModel;
			if (complexContent != null)
			{
				// it is not a root
				XmlSchemaComplexContentExtension extension = (XmlSchemaComplexContentExtension) complexContent.Content;
				string parentName = extension.BaseTypeName.Name;
				ClassElement parentClass = SchemaModel.FindClass(parentName);
				ParentClass = parentClass;
				parentClass.AddSubclass(this);
			}		
		}

		/// <summary>
		/// Set the simple attribute and primary key list.
		/// </summary>
		/// <param name="complexType">The xml schema complexType</param>
		private void SetSimpleAttributsAndPrimaryKeys(XmlSchemaComplexType complexType)
		{
			XmlSchemaObjectCollection xmlSchemaElements;

			if (IsRoot) 
			{
				XmlSchemaSequence sequence = (XmlSchemaSequence) ((XmlSchemaComplexType) XmlSchemaElement).Particle;
				xmlSchemaElements = sequence.Items;
			} 
			else 
			{
				// subclass
				XmlSchemaComplexContent complexContent = (XmlSchemaComplexContent) ((XmlSchemaComplexType) XmlSchemaElement).ContentModel;
				XmlSchemaComplexContentExtension extension = (XmlSchemaComplexContentExtension) complexContent.Content;
				XmlSchemaSequence sequence = (XmlSchemaSequence) extension.Particle;
				xmlSchemaElements = sequence.Items;
			}

			foreach (XmlSchemaElement xmlSchemaElement in xmlSchemaElements)
			{
				// if the element has an KEY_REF attribute with true value
				// it is an element for a relationship
				string status = SchemaModelElement.GetNewteraAttributeValue(xmlSchemaElement, NewteraNameSpace.KEY_REF);
				bool isRelationship = (status != null && status == "true" ? true : false);
				status = SchemaModelElement.GetNewteraAttributeValue(xmlSchemaElement, NewteraNameSpace.DIMENSION);
				bool isArrayAttribute = (status != null && status.Length > 0 ? true : false);
                status = SchemaModelElement.GetNewteraAttributeValue(xmlSchemaElement, NewteraNameSpace.CLASS_TYPE);
                bool isVirtualAttribute = (status != null ? true : false);
                status = SchemaModelElement.GetNewteraAttributeValue(xmlSchemaElement, NewteraNameSpace.HEIGHT);
                bool isImageAttribute = (status != null ? true : false);
                status = SchemaModelElement.GetNewteraAttributeValue(xmlSchemaElement, NewteraNameSpace.URL);
                bool isCustomPage = (status != null ? true : false);

                if (!isRelationship && !isArrayAttribute && !isVirtualAttribute && !isImageAttribute && !isCustomPage)
				{
					SimpleAttributeElement attribute = new SimpleAttributeElement(xmlSchemaElement);
					attribute.OwnerClass = this;
					attribute.SchemaModel = SchemaModel;
					attribute.Unmarshal();
					_simpleAttributes.Add(attribute);
					if (attribute.IsPrimaryKey)
					{
						_primaryKeys.Add(attribute);
					}
				}
			}

            SetUniqueKeys();
		}

		/// <summary>
		/// Set the relationship attribute list.
		/// </summary>
		/// <param name="complexType">The xml schema complexType</param>
		private void SetRelationshipAttributes(XmlSchemaComplexType complexType)
		{
			XmlSchemaObjectCollection xmlSchemaAttributes;

			// modify the xml schema
			if (IsRoot) 
			{
				xmlSchemaAttributes = complexType.Attributes;
			} 
			else 
			{
				// subclass
				XmlSchemaComplexContent complexContent = (XmlSchemaComplexContent) ((XmlSchemaComplexType) XmlSchemaElement).ContentModel;
				XmlSchemaComplexContentExtension extension = (XmlSchemaComplexContentExtension) complexContent.Content;
				xmlSchemaAttributes = extension.Attributes;
			}

			foreach (XmlSchemaAttribute xmlSchemaAttribute in xmlSchemaAttributes)
			{
				if (xmlSchemaAttribute.Name != NewteraNameSpace.OBJ_ID)
				{
					RelationshipAttributeElement attribute = new RelationshipAttributeElement(xmlSchemaAttribute);
					attribute.OwnerClass = this;
					attribute.SchemaModel = SchemaModel;
					attribute.Unmarshal();
					_relationshipAttributes.Add(attribute);
				}
			}
		}

		/// <summary>
		/// Set the array attribute list.
		/// </summary>
		/// <param name="complexType">The xml schema complexType</param>
		private void SetArrayAttributes(XmlSchemaComplexType complexType)
		{
			XmlSchemaObjectCollection xmlSchemaElements;

			if (IsRoot) 
			{
				XmlSchemaSequence sequence = (XmlSchemaSequence) ((XmlSchemaComplexType) XmlSchemaElement).Particle;
				xmlSchemaElements = sequence.Items;
			} 
			else 
			{
				// subclass
				XmlSchemaComplexContent complexContent = (XmlSchemaComplexContent) ((XmlSchemaComplexType) XmlSchemaElement).ContentModel;
				XmlSchemaComplexContentExtension extension = (XmlSchemaComplexContentExtension) complexContent.Content;
				XmlSchemaSequence sequence = (XmlSchemaSequence) extension.Particle;
				xmlSchemaElements = sequence.Items;
			}

			foreach (XmlSchemaElement xmlSchemaElement in xmlSchemaElements)
			{
				// if the element has an DIMENSION attribute, then
				// it is an element for an array attribute
				string val = SchemaModelElement.GetNewteraAttributeValue(xmlSchemaElement, NewteraNameSpace.DIMENSION);
				bool isArray = (val != null && val.Length > 0 ? true : false);

				if (isArray)
				{
					ArrayAttributeElement attribute = new ArrayAttributeElement(xmlSchemaElement);
					attribute.OwnerClass = this;
					attribute.SchemaModel = SchemaModel;
					attribute.Unmarshal();
					_arrayAttributes.Add(attribute);
				}
			}
		}

        /// <summary>
        /// Set the virtual attribute list.
        /// </summary>
        /// <param name="complexType">The xml schema complexType</param>
        private void SetVirtualAttributes(XmlSchemaComplexType complexType)
        {
            XmlSchemaObjectCollection xmlSchemaElements;

            if (IsRoot)
            {
                XmlSchemaSequence sequence = (XmlSchemaSequence)((XmlSchemaComplexType)XmlSchemaElement).Particle;
                xmlSchemaElements = sequence.Items;
            }
            else
            {
                // subclass
                XmlSchemaComplexContent complexContent = (XmlSchemaComplexContent)((XmlSchemaComplexType)XmlSchemaElement).ContentModel;
                XmlSchemaComplexContentExtension extension = (XmlSchemaComplexContentExtension)complexContent.Content;
                XmlSchemaSequence sequence = (XmlSchemaSequence)extension.Particle;
                xmlSchemaElements = sequence.Items;
            }

            foreach (XmlSchemaElement xmlSchemaElement in xmlSchemaElements)
            {
                // if the element has a CLASS_TYPE attribute, then
                // it is an element for a virtual attribute
                string val = SchemaModelElement.GetNewteraAttributeValue(xmlSchemaElement, NewteraNameSpace.CLASS_TYPE);
                bool isVirtual = (val != null ? true : false);

                if (isVirtual)
                {
                    VirtualAttributeElement attribute = new VirtualAttributeElement(xmlSchemaElement);
                    attribute.OwnerClass = this;
                    attribute.SchemaModel = SchemaModel;
                    attribute.Unmarshal();
                    _virtualAttributes.Add(attribute);
                }
            }
        }

        /// <summary>
        /// Set the image attribute list.
        /// </summary>
        /// <param name="complexType">The xml schema complexType</param>
        private void SetImageAttributes(XmlSchemaComplexType complexType)
        {
            XmlSchemaObjectCollection xmlSchemaElements;

            if (IsRoot)
            {
                XmlSchemaSequence sequence = (XmlSchemaSequence)((XmlSchemaComplexType)XmlSchemaElement).Particle;
                xmlSchemaElements = sequence.Items;
            }
            else
            {
                // subclass
                XmlSchemaComplexContent complexContent = (XmlSchemaComplexContent)((XmlSchemaComplexType)XmlSchemaElement).ContentModel;
                XmlSchemaComplexContentExtension extension = (XmlSchemaComplexContentExtension)complexContent.Content;
                XmlSchemaSequence sequence = (XmlSchemaSequence)extension.Particle;
                xmlSchemaElements = sequence.Items;
            }

            foreach (XmlSchemaElement xmlSchemaElement in xmlSchemaElements)
            {
                // if the element has an Height attribute, then
                // it is an element for an image attribute
                string val = SchemaModelElement.GetNewteraAttributeValue(xmlSchemaElement, NewteraNameSpace.HEIGHT);
                bool isImage = (val != null && val.Length > 0 ? true : false);

                if (isImage)
                {
                    ImageAttributeElement attribute = new ImageAttributeElement(xmlSchemaElement);
                    attribute.OwnerClass = this;
                    attribute.SchemaModel = SchemaModel;
                    attribute.Unmarshal();
                    _imageAttributes.Add(attribute);
                }
            }
        }

        /// <summary>
        /// Set the custom page list.
        /// </summary>
        /// <param name="complexType">The xml schema complexType</param>
        private void SetCustomPages(XmlSchemaComplexType complexType)
        {
            XmlSchemaObjectCollection xmlSchemaElements;

            if (IsRoot)
            {
                XmlSchemaSequence sequence = (XmlSchemaSequence)((XmlSchemaComplexType)XmlSchemaElement).Particle;
                xmlSchemaElements = sequence.Items;
            }
            else
            {
                // subclass
                XmlSchemaComplexContent complexContent = (XmlSchemaComplexContent)((XmlSchemaComplexType)XmlSchemaElement).ContentModel;
                XmlSchemaComplexContentExtension extension = (XmlSchemaComplexContentExtension)complexContent.Content;
                XmlSchemaSequence sequence = (XmlSchemaSequence)extension.Particle;
                xmlSchemaElements = sequence.Items;
            }

            foreach (XmlSchemaElement xmlSchemaElement in xmlSchemaElements)
            {
                // if the element has an URL attribute, then
                // it is an element for a custom page
                string val = SchemaModelElement.GetNewteraAttributeValue(xmlSchemaElement, NewteraNameSpace.URL);
                bool isCustomPage = (val != null && val.Length > 0 ? true : false);

                if (isCustomPage)
                {
                    CustomPageElement customPage = new CustomPageElement(xmlSchemaElement);
                    customPage.SchemaModel = SchemaModel;
                    customPage.MasetrClassName = this.Name;
                    customPage.Unmarshal();
                    _customPages.Add(customPage);
                }
            }
        }

        /// <summary>
        /// Find and set a collection of simple attributes that define an unique constraint for the class
        /// </summary>
        /// <returns>A SchemaModelElementCollection object</returns>
        private void SetUniqueKeys()
        {
            XmlSchemaUnique uniqueElement = SchemaModel.SchemaBody.GetUniqueConstraint(this);
            if (uniqueElement != null)
            {
                AttributeElementBase uk;
                ClassElement currentClass;
                foreach (XmlSchemaXPath field in uniqueElement.Fields)
                {
                    uk = null;

                    // find the simple attribute from the local and parent classes that matches the name
                    currentClass = this;
                    while (currentClass != null)
                    {
                        foreach (SimpleAttributeElement attr in currentClass.SimpleAttributes)
                        {
                            if (attr.Name == field.XPath)
                            {
                                uk = attr;
                                break;
                            }
                        }

                        if (uk != null)
                        {
                            break;
                        }

                        foreach (RelationshipAttributeElement attr in currentClass.RelationshipAttributes)
                        {
                            if (attr.Name == field.XPath)
                            {
                                uk = attr;
                                break;
                            }
                        }

                        if (uk != null)
                        {
                            break;
                        }
                        else
                        {
                            // not found in the current class, search the parent class
                            currentClass = currentClass.ParentClass;
                        }
                    }

                    if (uk != null)
                    {
                        _uniqueKeys.Add(uk);
                    }
                }
            }
        }

		/// <summary>
		/// Write the simple attributes, and constraints to the xml schema
		/// </summary>
		/// <param name="complexType">The xml schema complexType</param>
		private void WriteSimpleAttributesAndPrimaryKeys(XmlSchemaComplexType complexType)
		{
			XmlSchemaSequence sequence;

			if (IsRoot) 
			{
				sequence = (XmlSchemaSequence) complexType.Particle;
			} 
			else 
			{
				// subclass
				XmlSchemaComplexContent complexContent = (XmlSchemaComplexContent) complexType.ContentModel;
				XmlSchemaComplexContentExtension extension = (XmlSchemaComplexContentExtension) complexContent.Content;
				sequence = (XmlSchemaSequence) extension.Particle;
			}

			foreach (SimpleAttributeElement attribute in _simpleAttributes)
			{
				// make sure it returns a new xml schema element
				attribute.Clear();
				sequence.Items.Add(attribute.XmlSchemaElement);
				attribute.Marshal();
			}

			// write primary keys to the schema body as key element
			if (PrimaryKeys.Count > 0)
			{
				SchemaModel.SchemaBody.AddPrimaryKeys(this, PrimaryKeys);
			}

            // write unique keys to the schema body as key element
            if (UniqueKeys.Count > 0)
            {
                SchemaModel.SchemaBody.AddUniqueConstraint(this, UniqueKeys);
            }
		}

		/// <summary>
		/// Write the relationship attributes to the xml schema
		/// </summary>
		/// <param name="complexType">The xml schema complexType</param>
		private void WriteRelationshipAttributes(XmlSchemaComplexType complexType)
		{
			XmlSchemaObjectCollection attributes;

			// modify the xml schema
			if (IsRoot) 
			{
				attributes = complexType.Attributes;
			} 
			else 
			{
				// subclass
				XmlSchemaComplexContent complexContent = (XmlSchemaComplexContent) ((XmlSchemaComplexType) XmlSchemaElement).ContentModel;
				XmlSchemaComplexContentExtension extension = (XmlSchemaComplexContentExtension) complexContent.Content;
				attributes = extension.Attributes;
			}

			foreach (RelationshipAttributeElement attribute in _relationshipAttributes)
			{
				// make sure it returns a new xml schema element
				attribute.Clear();
				attributes.Add(attribute.XmlSchemaElement);
				attribute.Marshal();
			}
		}

		/// <summary>
		/// Write the array attributes to the xml schema
		/// </summary>
		/// <param name="complexType">The xml schema complexType</param>
		private void WriteArrayAttributes(XmlSchemaComplexType complexType)
		{
			XmlSchemaSequence sequence;

			if (IsRoot) 
			{
				sequence = (XmlSchemaSequence) complexType.Particle;
			} 
			else 
			{
				// subclass
				XmlSchemaComplexContent complexContent = (XmlSchemaComplexContent) complexType.ContentModel;
				XmlSchemaComplexContentExtension extension = (XmlSchemaComplexContentExtension) complexContent.Content;
				sequence = (XmlSchemaSequence) extension.Particle;
			}

			foreach (ArrayAttributeElement attribute in _arrayAttributes)
			{
				// make sure it returns a new xml schema element
				attribute.Clear();
				sequence.Items.Add(attribute.XmlSchemaElement);
				attribute.Marshal();
			}
		}

        /// <summary>
        /// Write the virtual attributes to the xml schema
        /// </summary>
        /// <param name="complexType">The xml schema complexType</param>
        private void WriteVirtualAttributes(XmlSchemaComplexType complexType)
        {
            XmlSchemaSequence sequence;

            if (IsRoot)
            {
                sequence = (XmlSchemaSequence)complexType.Particle;
            }
            else
            {
                // subclass
                XmlSchemaComplexContent complexContent = (XmlSchemaComplexContent)complexType.ContentModel;
                XmlSchemaComplexContentExtension extension = (XmlSchemaComplexContentExtension)complexContent.Content;
                sequence = (XmlSchemaSequence)extension.Particle;
            }

            foreach (VirtualAttributeElement attribute in _virtualAttributes)
            {
                // make sure it returns a new xml schema element
                attribute.Clear();
                sequence.Items.Add(attribute.XmlSchemaElement);
                attribute.Marshal();
            }
        }

        /// <summary>
        /// Write the image attributes to the xml schema
        /// </summary>
        /// <param name="complexType">The xml schema complexType</param>
        private void WriteImageAttributes(XmlSchemaComplexType complexType)
        {
            XmlSchemaSequence sequence;

            if (IsRoot)
            {
                sequence = (XmlSchemaSequence)complexType.Particle;
            }
            else
            {
                // subclass
                XmlSchemaComplexContent complexContent = (XmlSchemaComplexContent)complexType.ContentModel;
                XmlSchemaComplexContentExtension extension = (XmlSchemaComplexContentExtension)complexContent.Content;
                sequence = (XmlSchemaSequence)extension.Particle;
            }

            foreach (ImageAttributeElement attribute in _imageAttributes)
            {
                // make sure it returns a new xml schema element
                attribute.Clear();
                sequence.Items.Add(attribute.XmlSchemaElement);
                attribute.Marshal();
            }
        }

        /// <summary>
        /// Write the custom pages to the xml schema
        /// </summary>
        /// <param name="complexType">The xml schema complexType</param>
        private void WriteCustomPages(XmlSchemaComplexType complexType)
        {
            XmlSchemaSequence sequence;

            if (IsRoot)
            {
                sequence = (XmlSchemaSequence)complexType.Particle;
            }
            else
            {
                // subclass
                XmlSchemaComplexContent complexContent = (XmlSchemaComplexContent)complexType.ContentModel;
                XmlSchemaComplexContentExtension extension = (XmlSchemaComplexContentExtension)complexContent.Content;
                sequence = (XmlSchemaSequence)extension.Particle;
            }

            foreach (CustomPageElement customPage in _customPages)
            {
                // make sure it returns a new xml schema element
                customPage.Clear();
                sequence.Items.Add(customPage.XmlSchemaElement);
                customPage.Marshal();
            }
        }
		
		/// <summary>
		/// Add the built-in attribute "obj_id" to the complexType element.
		/// </summary>
		/// <param name="complexType">xml schema element</param>
		/// <remarks>
		/// <!--<xsd:attribute name="obj_id" type="xsd:ID"/></xsd:attribute>-->
		/// </remarks>
		private void AddObjIdAttribute(XmlSchemaComplexType complexType)
		{
			XmlSchemaAttribute objIdAttribute = new XmlSchemaAttribute();
			objIdAttribute.Name = NewteraNameSpace.OBJ_ID;
			objIdAttribute.SchemaTypeName = new XmlQualifiedName("ID", "http://www.w3.org/2003/XMLSchema"); 
			
			complexType.Attributes.Add(objIdAttribute);
		}

		/// <summary>
		/// Add leaf classes to the collection recursively
		/// </summary>
		/// <param name="parent">The parent class</param>
		/// <param name="leafClasses">The leaf class collection object.</param>
		private void AddLeafClasses(ClassElement parent, SchemaModelElementCollection leafClasses)
		{
			if (parent.IsLeaf)
			{
				leafClasses.Add(parent);
			}
			else
			{
				foreach (ClassElement child in parent.Subclasses)
				{
					AddLeafClasses(child, leafClasses);
				}
			}
		}
	}

	/// <summary>
	/// Specifies the options used by FindAttribute method
	/// </summary>
	public enum SearchDirection
	{
		/// <summary>
		/// TwoWay
		/// </summary>
		TwoWay,
		/// <summary>
		/// Upward
		/// </summary>
		Upward,
		/// <summary>
		/// Downward
		/// </summary>
		Downward
	}

    /// <summary>
    /// Specifies the sorting direction
    /// </summary>
    public enum SortDirection
    {
        /// <summary>
        /// Ascending
        /// </summary>
        Ascending,
        /// <summary>
        /// Descending
        /// </summary>
        Descending
    }
}