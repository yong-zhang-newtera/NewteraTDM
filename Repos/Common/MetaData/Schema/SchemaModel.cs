/*
* @(#)SchemaModel.cs
*
* Copyright (c) 2003 Newtera, Inc. All rights reserved.
*
*/
namespace Newtera.Common.MetaData.Schema
{
	using System;
	using System.IO;
	using System.Text;
	using System.Collections;
	using System.Xml;
	using System.Xml.Schema;
	using System.Resources;

	using Newtera.Common.Core;
	using Newtera.Common.MetaData;
	using Newtera.Common.MetaData.Schema.Validate;
	using Newtera.Common.MetaData.XaclModel;

	/// <summary>
	/// A SchemaModel represents a particular Object-relational schema that simplies
	/// the modelling of business problems in certain areas.
	/// 
	/// You can create a SchemaModel in two ways. One is to create it from scratch using
	/// the SchemaModel API. The SchemaModel provides a set of classes and methods to
	/// allow creation, deletion, and modification of a schema model that consists of some
	/// logical entities, such as Class, SimpleAttribute, RelationshipAttribute
	/// Constraints. Then the SchemaModel can be saved as XML Schema. Alternatively,
	/// a SchemaModel can be generated from a saved XML Schema, be modified, and be
	/// saved as XML Schema again.
	/// </summary>
	/// 
	/// <version>1.0.1 25 May 2003</version>
	/// <author>Yong Zhang</author>
	/// 
	/// <remarks> Microsoft's SOM (Schema Object Model) as internal representation of 
	/// XML schema is used for internal structuring.
	/// </remarks>
	public class SchemaModel : IXaclObject
	{	
		// indicate whether the schema model is valid
		private bool _isValid = true;

		// indicate whether the schema model has been altered
		private bool _isAltered = false;

		// The schemaInfo.
		private SchemaInfoElement _schemaInfo = null;
		
		// Root classes
		private SchemaModelElementCollection _rootClasses = null;
		
		// Enumerations in the schema
		private SchemaModelElementCollection _enums = null;

		// Ranges in the schema
		private SchemaModelElementCollection _ranges = null;

		// A list of patterns in the schema
		private SchemaModelElementCollection _patterns = null;

		// A list of list constraints in the schema
		private SchemaModelElementCollection _lists = null;
		
		// All classes hashTable, using the className as key
		private Hashtable _allClassesByName = null;

		// All classes hashtable, using the class id as key
		private Hashtable _allClassesById = null;

		// All constraint hashtable, using constraint name as key
		private Hashtable _allConstraintsByName = null;

		private SchemaModelBody _schemaBody = null;

		private MetaDataModel _metaData = null;

		private string _xpath = null; // run-time data

		private ResourceManager _resources;

		/// <summary>
		/// Value changed handler
		/// </summary>
		public event EventHandler ValueChanged;

        /// <summary>
        /// Class Category changed handler
        /// </summary>
        public event EventHandler CategoryChanged;
		
		/// <summary>
		/// Initiating a SchemaModel object
		/// </summary>
		public SchemaModel()
        {
            _rootClasses = new SchemaModelElementCollection();
            if (GlobalSettings.Instance.IsWindowClient)
            {
                _rootClasses.ValueChanged += new EventHandler(ValueChangedHandler);
            }
            _enums = new SchemaModelElementCollection();
            if (GlobalSettings.Instance.IsWindowClient)
            {
                _enums.ValueChanged += new EventHandler(ValueChangedHandler);
            }
            _ranges = new SchemaModelElementCollection();
            if (GlobalSettings.Instance.IsWindowClient)
            {
                _ranges.ValueChanged += new EventHandler(ValueChangedHandler);
            }
            _patterns = new SchemaModelElementCollection();
            if (GlobalSettings.Instance.IsWindowClient)
            {
                _patterns.ValueChanged += new EventHandler(ValueChangedHandler);
            }
            _lists = new SchemaModelElementCollection();
            {
                _lists.ValueChanged += new EventHandler(ValueChangedHandler);
            }
            _allClassesByName = new Hashtable();
            _allClassesById = new Hashtable();
            _allConstraintsByName = new Hashtable();
            _resources = new ResourceManager(this.GetType());

            if (_schemaInfo == null)
            {
                _schemaInfo = new SchemaInfoElement("New Schema");
                _schemaInfo.Version = "1.0";
            }
        }

		/// <summary>
		/// Initiating a SchemaModel object
		/// </summary>
		public SchemaModel(SchemaInfo schemaInfo) : this()
		{
			_schemaInfo = new SchemaInfoElement(schemaInfo.Name);
			_schemaInfo.Version = schemaInfo.Version;
			_resources = new ResourceManager(this.GetType());
		}

		/// <summary>
		/// Gets the information indicating whether the schema model is a valid one.
		/// </summary>
		/// <value>True if it is valid, false otherwise. Default is true</value>
		/// <remarks>
		/// When a schema model is valid, it will be able to generate corresponding
		/// database schema without compile-time errors. It may still have run-time
		/// errors due to database problems.
		/// </remarks>
		public bool IsValid
		{
			get
			{
				return _isValid;
			}
		}

		/// <summary>
		/// Gets the information indicating whether the schema model has been altered
		/// since last setting of value.
		/// </summary>
		/// <value>True if it is altered, false otherwise. Default is false</value>
		/// <remarks>
		/// It falg allows UI tool to save the schema when the tool exits without
		/// saving the schema.
		/// </remarks>
		public bool IsAltered
		{
			get
			{
				return _isAltered;
			}
			set
			{
				_isAltered = value;
				_isValid = false; // When altered, assuming it become invalid
			}
		}

		/// <summary>
		/// Gets or sets the meta data that owns the Schema
		/// </summary>
		/// <returns> A MetaDataModel object</returns>
		public MetaDataModel MetaData
		{
			get
			{
				return _metaData;
			}
			set
			{
				_metaData = value;
			}
		}

		/// <summary>
		/// Gets or sets information about the Schema
		/// </summary>
		/// <returns> A SchemaInfoElement object</returns>
		/// <remarks>
		/// The schema information is stored as attributes of the root XmlSchemaElement
		/// object.
		/// </remarks>
		public SchemaInfoElement SchemaInfo
		{
			get
			{
				return _schemaInfo;
			}
			set
			{
				_schemaInfo = value;
			}
		}

		/// <summary>
		/// Gets the root classes of the schema.
		/// </summary>
		/// <value>
		/// A SchemaModelElementCollection contains ClassElement objects for root classes.
		/// </value>
		public SchemaModelElementCollection RootClasses
		{
			get
			{
				return _rootClasses;
			}
		}

		/// <summary>
		/// Gets enumeration constraints of the schema
		/// </summary>
		/// <value>A SchemaModelElementCollection contains EnumElement objects</value>
		public SchemaModelElementCollection EnumConstraints
		{
			get
			{
				return _enums;
			}
		}

		/// <summary>
		/// Gets range constraints of the schema
		/// </summary>
		/// <value>A SchemaModelElementCollection contains RangeElement objects</value>
		public SchemaModelElementCollection RangeConstraints
		{
			get
			{
				return _ranges;
			}
		}

		/// <summary>
		/// Gets pattern constraints of the schema
		/// </summary>
		/// <value>A SchemaModelElementCollection contains PatternElement objects</value>
		public SchemaModelElementCollection PatternConstraints
		{
			get
			{
				return _patterns;
			}
		}

		/// <summary>
		/// Gets list constraints of the schema
		/// </summary>
		/// <value>A SchemaModelElementCollection contains ListElement objects</value>
		public SchemaModelElementCollection ListConstraints
		{
			get
			{
				return _lists;
			}
		}

		/// <summary>
		/// Gets all constraints in the schema in a sorted order
		/// </summary>
		public IList AllConstraints
		{
			get
			{
				MetaDataElementSortedList childrenSortedList = new MetaDataElementSortedList();
				SchemaModelElementCollection constraints = this.EnumConstraints;
				foreach (EnumElement enumElement in constraints)
				{
					childrenSortedList.Add(enumElement.Position, enumElement);
				}

				constraints = this.RangeConstraints;
				foreach (RangeElement rangeElement in constraints)
				{
					childrenSortedList.Add(rangeElement.Position, rangeElement);
				}

				constraints = this.PatternConstraints;
				foreach (PatternElement patternElement in constraints)
				{
					childrenSortedList.Add(patternElement.Position, patternElement);
				}

				constraints = this.ListConstraints;
				foreach (ListElement listElement in constraints)
				{
					childrenSortedList.Add(listElement.Position, listElement);
				}

				return childrenSortedList.Values;
			}
		}
		
		/// <summary>
		/// Accept a visitor of ISchemaModelElementVisitor type to traverse its
		/// elements.
		/// </summary>
		/// <param name="visitor"></param>
		public void Accept(ISchemaModelElementVisitor visitor)
		{
			_schemaInfo.Accept(visitor);

			foreach (SchemaModelElement element in _rootClasses)
			{
				element.Accept(visitor);
			}

			foreach (SchemaModelElement element in _enums)
			{
				element.Accept(visitor);
			}
		
			foreach (SchemaModelElement element in _ranges)
			{
				element.Accept(visitor);
			}

			foreach (SchemaModelElement element in _patterns)
			{
				element.Accept(visitor);
			}

			foreach (SchemaModelElement element in _lists)
			{
				element.Accept(visitor);
			}
		}

		/// <summary>
		/// Read an XML Schema into the model from a file.
		/// </summary>
		/// 
		/// <param name="fileName">the name of the XML Schema file</param>
		/// 
		/// <exception cref="SchemaModelException"> SchemaModelException is thrown when it fails to
		/// read the XML Schema file
		/// </exception>
		public void Read(string fileName)
		{
			try
			{
				//Open the stream and read XSD from it.
				using (FileStream fs = File.OpenRead(fileName)) 
				{
					Read(fs);					
				}
			}
			catch (Exception ex)
			{
				throw new ReadSchemaException(_resources.GetString("SchemaModel.FailedToReadFile") + fileName, ex);
			}
		}
		
		/// <summary>
		/// Read an XML Schema into the model from a Stream.
		/// </summary>
		/// 
		/// <param name="stream">
		/// The stream object from which to read a XML schema
		/// </param>
		/// <exception cref="SchemaModelException"> SchemaModelException is thrown when it fails to read
		/// the XML Schema file
		/// </exception>
		public void Read(Stream stream)
		{
			if (stream != null)
			{
				try
				{
					XmlSchema xmlSchema = XmlSchema.Read(stream, null);
				
					// Initializing the objects from the xml schema
					Unmarshal(xmlSchema);
				}
				catch (Exception e)
				{
					throw new ReadSchemaException(_resources.GetString("SchemaModel.FailedToReadModel"), e);
				}
			}
		}

		/// <summary>
		/// Read an XML Schema into the model from a TextReader.
		/// </summary>
		/// 
		/// <param name="reader">
		/// The text reader object from which to read a XML schema
		/// </param>
		/// <exception cref="SchemaModelException"> SchemaModelException is thrown when it fails to read
		/// the XML Schema file
		/// </exception>
		public void Read(TextReader reader)
		{
			if (reader != null)
			{
				try
				{
					XmlSchema xmlSchema = XmlSchema.Read(reader, null);
				
					// Initializing the objects from the xml schema
					Unmarshal(xmlSchema);
				}
				catch (Exception e)
				{
					throw new ReadSchemaException(_resources.GetString("SchemaModel.FailedToReadModel"), e);
				}
			}
		}

		/// <summary>
		/// Write the schema as a XML Schema to a file.
		/// </summary>
		/// <param name="fileName">The output file name.</param>
		public void Write(string fileName)
		{
			try
			{
				//Open the stream and read XSD from it.
				using (FileStream fs = File.Open(fileName, FileMode.Create)) 
				{
					Write(fs);
					fs.Flush();
				}
			}
			catch (System.IO.IOException ex)
			{
				throw new ReadSchemaException(_resources.GetString("SchemaModel.FailedToWriteFile") + fileName, ex);
			}
		}
		
		/// <summary>
		/// Write the schema as a XML Schema to a Stream.
		/// </summary>
		/// <param name="stream">the stream object to which to write a XML schema
		/// </param>
		/// 
		/// <exception cref="SchemaModelException"> SchemaModelException is thrown when it fails to write
		/// the XML Schema file
		/// </exception>
		public void Write(Stream stream)
		{
			try
			{
				XmlSchema xmlSchema = GetXmlSchema();

				xmlSchema.Write(stream);
			}
			catch (System.IO.IOException ex)
			{
				throw new WriteSchemaException(_resources.GetString("SchemaModel.FailedToWriteModel"), ex);
			}
		}

		/// <summary>
		/// Write the schema as a XML Schema to a TextWriter.
		/// </summary>
		/// <param name="writer">the TextWriter instance to which to write a XML schema
		/// </param>
		/// <exception cref="SchemaModelException"> SchemaModelException is thrown when it fails to write
		/// the XML Schema file
		/// </exception>
		public void Write(TextWriter writer)
		{
			try
			{
				XmlSchema xmlSchema = GetXmlSchema();

				xmlSchema.Write(writer);
			}
			catch (System.IO.IOException ex)
			{
				throw new WriteSchemaException(_resources.GetString("SchemaModel.FailedToWriteModel"), ex);
			}
		}

		/// <summary>
		/// Validate the schema model to see if it confirm to schema model integrity
		/// rules.
		/// </summary>
		/// <returns>The result in ValidateResult object</returns>
		public ValidateResult Validate()
		{
			ValidateResult result = new ValidateResult(); // an empty result

			if (!_isValid)
			{
				SchemaModelValidateVisitor visitor = new SchemaModelValidateVisitor(this);

				Accept(visitor); // start validating

				result = visitor.ValidateResult;

				if (!result.HasError && !result.HasDoubt)
				{
					_isValid = true; // set the valid flag to true
				}
			}

			return result;
		}

		/// <summary>
		/// Find a class by name.</summary>
		/// <param name="name">The class name</param>
		/// <returns> Return ClassElement object. If the class of given name don't exist
		/// return null</returns>
		/// <remarks>the class name is case-insensitive.</remarks>
		public ClassElement FindClass(string name)
		{
            if (name != null)
            {
                return (ClassElement)_allClassesByName[name.ToUpper()];
            }
            else
            {
                return null;
            }
		}

		/// <summary>
		/// Gets an enumerator for all classes
		/// </summary>
		/// <returns>the enumerator</returns>
		public IEnumerator GetClassEnumerator() 
		{
			return _allClassesByName.Values.GetEnumerator();
		}

		/// <summary>
		/// Find a class by id.</summary>
		/// <param name="classId">The class id</param>
		/// <returns> Return ClassElement object. If the class of given id don't exist
		/// return null</returns>
		public ClassElement FindClassById(string classId)
		{
			if (classId != null)
			{
				return (ClassElement) _allClassesById[classId];
			}
			else
			{
				return null;
			}
		}

        /// <summary>
        /// Find a SchemaModelElement by XPath.</summary>
        /// <param name="xpath">The xpath</param>
        /// <returns> Return SchemaModelElement object. If the SchemaModelElement of given xpath don't exist
        /// return null</returns>
        public SchemaModelElement FindSchemaModelElementByXPath(string xpath)
        {
            FindSchemaModelElementByXPathVisitor visitor = new FindSchemaModelElementByXPathVisitor(xpath);

            Accept(visitor); // start finding

            return visitor.SchemaModelElement;
        }

		/// <summary>
		/// Add a class to a hashtable with the class name as key
		/// </summary>
		/// <param name="classElement">The class to be added</param>
		/// <remarks>the class name is case-insensitive.</remarks>
		internal void AddToClassTable(ClassElement classElement)
		{
			_allClassesByName[classElement.Name.ToUpper()] = classElement;

            classElement.CategoryChanged += new EventHandler(this.CategoryChangedHandler);
		}

		/// <summary>
		/// Remove a class from a hashtable with the class name as key
		/// </summary>
		/// <param name="classElement">The class to be removed</param>
		/// <remarks>the class name is case-insensitive.</remarks>
		internal void RemoveFromClassTable(ClassElement classElement)
		{
			_allClassesByName.Remove(classElement.Name.ToUpper());

            classElement.CategoryChanged -= new EventHandler(this.CategoryChangedHandler);
		}

		/// <summary>
		/// Add a class to a hashtable with the class id as key
		/// </summary>
		/// <param name="classElement">The class element to be added</param>
		internal void AddToClassIdTable(ClassElement classElement)
		{
			if (classElement.ID != null)
			{
				_allClassesById[classElement.ID] = classElement;
			}
		}

		/// <summary>
		/// Remove a class from a hashtable with the class id as key
		/// </summary>
		/// <param name="classId">The id of class to be removed</param>
		internal void RemoveFromClassIdTable(string classId)
		{
			if (classId != null)
			{
				_allClassesById.Remove(classId);
			}
		}

		/// <summary>
		/// Find a constraint by name.</summary>
		/// <param name="name">The constraint name</param>
		/// 
		/// <returns> Return ConstraintElementBase object. If the constraint of given name don't exist
		/// return null</returns>
		public ConstraintElementBase FindConstraint(string name)
		{
			return (ConstraintElementBase) _allConstraintsByName[name];
		}

		/// <summary>
		/// Add a constraint to a hashtable with the constraint name as key
		/// </summary>
		/// <param name="constraintElement">The constraint to be added</param>
		internal void AddToConstraintTable(ConstraintElementBase constraintElement)
		{
			_allConstraintsByName[constraintElement.Name] = constraintElement;
		}

		/// <summary>
		/// Remove a constraint from a hashtable with the constraint name as key
		/// </summary>
		/// <param name="constraintElement">The constraint to be removed</param>
		internal void RemoveFromConstraintTable(ConstraintElementBase constraintElement)
		{
			_allConstraintsByName.Remove(constraintElement.Name);
		}

		/// <summary>
		/// Create a ClassElement
		/// </summary>
		/// <param name="name">The name of ClassElement to be created
		/// </param>
		/// <returns> The created ClassElement object</returns>
		/// <exception cref="DuplicateClassNameException">The exception is thrown if a class of the same
		/// name has already existed</exception>
		public ClassElement CreateClass(string name)
		{
			// check if a class of the same name exists
			if (FindClass(name) != null) 
			{
				throw new DuplicateClassNameException(_resources.GetString("SchemaModel.DuplicatClassName") + name);
			}
			
			ClassElement classElement = new ClassElement(name);
			classElement.SchemaModel = this;
			
			AddToClassTable(classElement);
			
			return classElement;
		}
		
		/// <summary> Add a root class to the schema model
		/// </summary>
		/// <param name="classElement">The ClassElement to be added
		/// </param>
		/// <returns> The ClassElement object</returns>
		/// <exception cref="DuplicateClassNameException">The exception is thrown if a class of the same
		/// name has already existed</exception>
		public ClassElement AddRootClass(ClassElement classElement)
		{	
			_rootClasses.Add(classElement);
			
			return classElement;
		}
		
		/// <summary> Remove a root class.
		/// </summary>
		/// <param name="classElement">the class to be removed
		/// </param>
		/// <returns>The ClassElement object removed</returns>
		public ClassElement RemoveRootClass(ClassElement classElement)
		{				
			if (_rootClasses.Contains(classElement))
			{	
				// Remove its subclasses first
				classElement.RemoveSubclasses();

				// Remove from _rootClasses list
				_rootClasses.Remove(classElement);

				// Remove from _allClassesByName list
				RemoveFromClassTable(classElement);
			}
		
			return classElement;
		}
		
		/// <summary> Add an Enumeration Constraint.</summary>
		/// <param name="enumElement">The supplied EnumElement.
		/// </param>
		/// <returns>The EnumElement object added
		/// </returns>
		public EnumElement AddEnumConstraint(EnumElement enumElement)
		{
			enumElement.SchemaModel = this;
			
			// add it to the enum constraint list
			_enums.Add(enumElement);

            if (GlobalSettings.Instance.IsWindowClient)
            {
                enumElement.ValueChanged += new EventHandler(ValueChangedHandler);
            }
			
			AddToConstraintTable(enumElement);

			return enumElement;
		}
		
		/// <summary>
		/// Remove an enumeration constraint from the model.
		/// </summary>
		/// <param name="name">The name of enumeration constraint to be removed
		/// </param>
		/// <returns>The removed EnumElement object</returns>
		/// <exception cref="ConstraintUsedException">An exception is thrown if the
		/// constraint is being referenced</exception>
		public EnumElement RemoveEnumConstraint(string name)
		{
			EnumElement theEnum = FindConstraint(name) as EnumElement;
			
			if (theEnum != null) 
			{
				RemoveEnumConstraint(theEnum);
			}

			return theEnum;
		}

		/// <summary>
		/// Remove an enumeration constraint from the model.
		/// </summary>
		/// <param name="enumElement">The enumElement to be removed
		/// </param>
		/// <returns>The removed EnumElement object</returns>
		/// <exception cref="ConstraintUsedException">An exception is thrown if the
		/// constraint is being referenced</exception> 
		internal EnumElement RemoveEnumConstraint(EnumElement enumElement)
		{
            ClassElement refClassElement;
            AttributeElementBase refAttribute;
            if (IsConstraintReferenced(enumElement, out refClassElement, out refAttribute))
            {
                string msg = string.Format(_resources.GetString("SchemaModel.ConstraintUsed"), refClassElement.Caption, refAttribute.Caption);
                throw new ConstraintUsedException(msg);
            }
			
			// remove the enum from enum constraints list
			_enums.Remove(enumElement);

			this.RemoveFromConstraintTable(enumElement);

			return enumElement;
		}
		
		/// <summary> Add a Range Constraint.</summary>
		/// <param name="rangeElement">The supplied RangeElement.
		/// </param>
		/// <returns>The RangeElement object added
		/// </returns>
		public RangeElement AddRangeConstraint(RangeElement rangeElement)
		{
			rangeElement.SchemaModel = this;
			
			// add it to the range constraint list
			_ranges.Add(rangeElement);

            if (GlobalSettings.Instance.IsWindowClient)
            {
                rangeElement.ValueChanged += new EventHandler(ValueChangedHandler);
            }

			AddToConstraintTable(rangeElement);

			return rangeElement;
		}
		
		/// <summary>
		/// Remove a range constraint from the schema model.
		/// </summary>
		/// <param name="name">The name of range constraint to be removed
		/// </param>
		/// <returns>The removed RangeElement object</returns>
		public RangeElement RemoveRangeConstraint(string name)
		{
			RangeElement theRange = FindConstraint(name) as RangeElement;
			
			if (theRange != null) 
			{
				RemoveRangeConstraint(theRange);
			}

			return theRange;
		}

		/// <summary>
		/// Remove a range constraint from the schema model.
		/// </summary>
		/// <param name="rangeElement">The rangeElement to be removed
		/// </param>
		/// <returns>The removed RangeElement object</returns>
		internal RangeElement RemoveRangeConstraint(RangeElement rangeElement)
		{
            ClassElement refClass;
            AttributeElementBase refAttribute;
            if (IsConstraintReferenced(rangeElement, out refClass, out refAttribute))
            {
                string msg = string.Format(_resources.GetString("SchemaModel.ConstraintUsed"), refClass.Caption, refAttribute.Caption);
                throw new ConstraintUsedException(msg);
            }
			
			// remove the range from enum constraints list
			_ranges.Remove(rangeElement);

			RemoveFromConstraintTable(rangeElement);

			return rangeElement;
		}
		
		/// <summary> Add a pattern Constraint.</summary>
		/// <param name="patternElement">The supplied PatternElement.
		/// </param>
		/// <returns>The PatternElement object added
		/// </returns>
		public PatternElement AddPatternConstraint(PatternElement patternElement)
		{
			patternElement.SchemaModel = this;
			
			// add it to the range constraint list
			_patterns.Add(patternElement);

            if (GlobalSettings.Instance.IsWindowClient)
            {
                patternElement.ValueChanged += new EventHandler(ValueChangedHandler);
            }

			AddToConstraintTable(patternElement);

			return patternElement;
		}
		
		/// <summary>
		/// Remove a pattern constraint from the schema model.
		/// </summary>
		/// <param name="name">The name of pattern constraint to be removed
		/// </param>
		/// <returns>The removed PatternElement object</returns>
		public PatternElement RemovePatternConstraint(string name)
		{
			PatternElement thePattern = FindConstraint(name) as PatternElement;
			
			if (thePattern != null) 
			{
				RemovePatternConstraint(thePattern);
			}

			return thePattern;
		}

		/// <summary>
		/// Remove a pattern constraint from the schema model.
		/// </summary>
		/// <param name="patternElement">The patternElement to be removed
		/// </param>
		/// <returns>The removed PatternElement object</returns>
		internal PatternElement RemovePatternConstraint(PatternElement patternElement)
		{
            ClassElement refClass;
            AttributeElementBase refAttribute;
            if (IsConstraintReferenced(patternElement, out refClass, out refAttribute))
            {
                string msg = string.Format(_resources.GetString("SchemaModel.ConstraintUsed"), refClass.Caption, refAttribute.Caption);
                throw new ConstraintUsedException(msg);
            }
			
			// remove the pattern from pattern constraints list
			_patterns.Remove(patternElement);

			RemoveFromConstraintTable(patternElement);

			return patternElement;
		}

		/// <summary> Add a List Constraint.</summary>
		/// <param name="listElement">The supplied ListElement.
		/// </param>
		/// <returns>The ListElement object added
		/// </returns>
		public ListElement AddListConstraint(ListElement listElement)
		{
			listElement.SchemaModel = this;
			
			// add it to the list constraint list
			_lists.Add(listElement);

            if (GlobalSettings.Instance.IsWindowClient)
            {
                listElement.ValueChanged += new EventHandler(ValueChangedHandler);
            }

			AddToConstraintTable(listElement);

			return listElement;
		}
		
		/// <summary>
		/// Remove an list constraint from the model.
		/// </summary>
		/// <param name="name">The name of list constraint to be removed
		/// </param>
		/// <returns>The removed ListElement object</returns>
		/// <exception cref="ConstraintUsedException">An exception is thrown if the
		/// constraint is being referenced</exception>
		public ListElement RemoveListConstraint(string name)
		{
			ListElement theList = FindConstraint(name) as ListElement;
			
			if (theList != null) 
			{
				RemoveListConstraint(theList);
			}

			return theList;
		}

		/// <summary>
		/// Gets the information whether the constraint is referred by any attributes
		/// </summary>
		/// <param name="constraint">The constraint</param>
        /// <param name="refClassElement">The referencing class element</param>
        /// <param name="refAttributeElement">The the referencing attribute element</param>
		/// <returns> true if it is used by at least one attribute.</returns>
        public bool IsConstraintReferenced(SchemaModelElement constraint, out ClassElement refClassElement, out AttributeElementBase refAttributeElement)
		{
			bool result = false;
            refClassElement = null;
            refAttributeElement = null;

			IDictionaryEnumerator classEnumerator = _allClassesByName.GetEnumerator();
			while ( classEnumerator.MoveNext() )
			{
				ClassElement classElement = (ClassElement) classEnumerator.Value;
                refClassElement = classElement;

				foreach (SimpleAttributeElement attribute in classElement.SimpleAttributes)
				{
					if (attribute.Constraint == constraint)
					{
                        refAttributeElement = attribute;
						result = true;
						break;
					}
				}

				if (result)
				{
					break;
				}

                foreach (VirtualAttributeElement attribute in classElement.VirtualAttributes)
                {
                    if (attribute.Constraint == constraint)
                    {
                        refAttributeElement = attribute;
                        result = true;
                        break;
                    }
                }

                if (result)
                {
                    break;
                }
			}
			
			return result;
		}

        /// <summary>
        /// Get information indicating whether a data view is referenced by any of
        /// the class in the schema model
        /// </summary>
        /// <param name="dataViewName">The data view name</param>
        /// <param name="classCaption">The caption of the referencing class first found</param>
        /// <returns>true if the data view is referenced, false otherwise.</returns>
        public bool IsDataViewReferenced(string dataViewName, out string classCaption)
        {
            bool status = false;
            classCaption = null;

            IDictionaryEnumerator classEnumerator = _allClassesByName.GetEnumerator();
            while (classEnumerator.MoveNext())
            {
                ClassElement classElement = (ClassElement)classEnumerator.Value;

                if (!string.IsNullOrEmpty(classElement.NestedDataViewName) &&
                    classElement.NestedDataViewName == dataViewName)
                {
                    status = true;
                    classCaption = classElement.Caption;
                }

                if (status)
                {
                    break;
                }
            }

            return status;
        }

		/// <summary>
		/// Remove an list constraint from the model.
		/// </summary>
		/// <param name="listElement">The listElement to be removed
		/// </param>
		/// <returns>The removed ListElement object</returns>
		/// <exception cref="ConstraintUsedException">An exception is thrown if the
		/// constraint is being referenced</exception> 
		internal ListElement RemoveListConstraint(ListElement listElement)
		{
            ClassElement refClass;
            AttributeElementBase refAttribute;
            if (IsConstraintReferenced(listElement, out refClass, out refAttribute)) 
			{
                string msg = string.Format(_resources.GetString("SchemaModel.ConstraintUsed"), refClass.Caption, refAttribute.Caption);
				throw new ConstraintUsedException(msg);
			}
			
			// remove the enum from list constraints list
			_lists.Remove(listElement);

			this.RemoveFromConstraintTable(listElement);

			return listElement;
		}

		/// <summary>
		/// Gets the Schema Model Body
		/// </summary>
		/// <value>The SchemaModelBody object</value>
		internal SchemaModelBody SchemaBody
		{
			get
			{
				if (_schemaBody == null)
				{
					_schemaBody = new SchemaModelBody(SchemaInfo.XmlSchemaElement);
				}
				return _schemaBody;
			}
		}

		/// <summary>
		/// Create a constraint element.
		/// </summary>
		/// <param name="name">The name of constraint to be created</param>
		/// <param name="type">The one of ConstraintType values</param>
		/// <returns> The created constraint object</returns>
		/// <exception cref="DuplicateConstraintNameException">The exception is thrown if a constraint of the same
		/// name has already existed</exception>
		internal ConstraintElementBase CreateConstraint(string name, ConstraintType type)
		{
			// check if a constraint of the same name exists
			if (FindConstraint(name) != null) 
			{
				throw new DuplicateConstraintNameException(_resources.GetString("SchemaModel.DuplicateConstraintName" + name));
			}
			
			ConstraintElementBase constraint = null;
			switch (type)
			{
				case ConstraintType.Enumeration:
					constraint = new EnumElement(name);
					break;
				case ConstraintType.Pattern:
					constraint = new PatternElement(name);
					break;
				case ConstraintType.Range:
					constraint = new RangeElement(name);
					break;
				case ConstraintType.List:
					constraint = new ListElement(name);
					break;
			}
			
			AddToConstraintTable(constraint);
			
			return constraint;
		}
		
		/// <summary>
		/// Gets the xml schema that represents the schema model
		/// </summary>
		/// <returns>A XmlSchema instance</returns>
		private XmlSchema GetXmlSchema()
		{
			// Marshal the objects to xml schema
			XmlSchema xmlSchema = new XmlSchema();

			// Add Newtera Namespace
			xmlSchema.TargetNamespace = "http://www.newtera.com";
			xmlSchema.Namespaces.Add("psd", "http://www.newtera.com");

			Marshal(xmlSchema);

			return xmlSchema;
		}
		
		/// <summary>
		/// Create the member objects from a XML Schema Model
		/// </summary>
		/// <param name="xmlSchema">The xml schema model</param>
		private void Unmarshal(XmlSchema xmlSchema)
		{
			// reset the schema model
			Reset();

			ArrayList allClasses = new ArrayList();
			// creating the internal data objects from xml schema
			foreach (XmlSchemaAnnotated xmlSchemaObject in xmlSchema.Items)
			{
				// ComplexType element represents a ClassElement
				if (xmlSchemaObject is XmlSchemaComplexType)
				{
					ClassElement classElement = new ClassElement(xmlSchemaObject);
					classElement.SchemaModel = this;
					// Delay calling Unmarshal method to the class later
					allClasses.Add(classElement);
					AddToClassTable(classElement);
				} 
				else if (xmlSchemaObject is XmlSchemaSimpleType) 
				{
					// SimpleType element represents a constraint
					if (ListElement.isList(xmlSchemaObject))
					{
						ListElement constraint = new ListElement(xmlSchemaObject);
						constraint.SchemaModel = this;						
						constraint.Unmarshal();
						_lists.Add(constraint);
						AddToConstraintTable(constraint);
					}
					else if (EnumElement.isEnum(xmlSchemaObject))
					{
						EnumElement constraint = new EnumElement(xmlSchemaObject);
						constraint.SchemaModel = this;
						constraint.Unmarshal();
						_enums.Add(constraint);
						AddToConstraintTable(constraint);
					} 
					else if (RangeElement.isRange(xmlSchemaObject))
					{
						RangeElement constraint = new RangeElement(xmlSchemaObject);
						constraint.SchemaModel = this;
						constraint.Unmarshal();
						_ranges.Add(constraint);
						AddToConstraintTable(constraint);
					}
					else if (PatternElement.isPattern(xmlSchemaObject))
					{
						PatternElement constraint = new PatternElement(xmlSchemaObject);
						constraint.SchemaModel = this;						
						constraint.Unmarshal();
						_patterns.Add(constraint);
						AddToConstraintTable(constraint);
					}
				}
				else if (xmlSchemaObject is XmlSchemaElement)
				{
					// The Element at schema level contains schema information
					_schemaInfo = new SchemaInfoElement(xmlSchemaObject);
					_schemaInfo.Unmarshal();
				}
			}

			// calling Unmarshal to each class in the schema model so that the parent-child
			// relationship can be setup correctly while classes are unmarshalled
			foreach (ClassElement classElement in allClasses)
			{
				classElement.Unmarshal();
			}

			// now parent-children relationships are setup correctly,
			// add root classes to the list.
			// since the class id is available in class elements,
			// hash class elements by their ids
			foreach (ClassElement classElement in allClasses)
			{
				if (classElement.IsRoot)
				{
					_rootClasses.Add(classElement);
				}

				AddToClassIdTable(classElement);
			}
		}

		/// <summary>
		/// Write objects to XML Schema Model
		/// </summary>
		/// <param name="xmlSchema">The xml schema model</param>
		private void Marshal(XmlSchema xmlSchema)
		{
            // add global schema info as the first thing
            // make sure to get a new xml schema element
            _schemaInfo.Clear();
            _schemaBody = null;
            xmlSchema.Items.Add(_schemaInfo.XmlSchemaElement);
            _schemaInfo.Marshal();

			foreach(EnumElement enumElement in _enums)
			{
				// make sure to get a new xml schema element
				enumElement.Clear();
				xmlSchema.Items.Add(enumElement.XmlSchemaElement);
				enumElement.Marshal();
			}

			foreach(RangeElement rangeElement in _ranges)
			{
				// make sure to get a new xml schema element
				rangeElement.Clear();
				xmlSchema.Items.Add(rangeElement.XmlSchemaElement);
				rangeElement.Marshal();
			}

			foreach(PatternElement patternElement in _patterns)
			{
				// make sure to get a new xml schema element
				patternElement.Clear();
				xmlSchema.Items.Add(patternElement.XmlSchemaElement);
				patternElement.Marshal();
			}

			foreach(ListElement listElement in _lists)
			{
				// make sure to get a new xml schema element
				listElement.Clear();
				xmlSchema.Items.Add(listElement.XmlSchemaElement);
				listElement.Marshal();
			}

			// Write root classes and their subclasses to xml schema
			WriteClasses(xmlSchema, _rootClasses);
		}

		/// <summary>
		/// The reset the model to the begining
		/// </summary>
		private void Reset()
		{
			_rootClasses.Clear();
			_enums.Clear();
			_ranges.Clear();
			_patterns.Clear();
			_lists.Clear();
			_allClassesByName.Clear();
            _schemaBody = null;
		}

		/// <summary>
		/// Write class objects to xml schema
		/// </summary>
		/// <param name="xmlSchema">The xml schema</param>
		/// <param name="classList">The list of class objects</param>
		private void WriteClasses(XmlSchema xmlSchema, SchemaModelElementCollection classList)
		{
			foreach(ClassElement classElement in classList)
			{
				// make sure to get a new xml schema element
				classElement.Clear();
				xmlSchema.Items.Add(classElement.XmlSchemaElement);
				classElement.Marshal();

				if (!classElement.IsLeaf)
				{
					WriteClasses(xmlSchema, classElement.Subclasses);
				}
			}
		}

		/// <summary>
		/// A handler to call when a value of the schema model changed
		/// </summary>
		/// <param name="sender">the IMetaDataElement that cause the event</param>
		/// <param name="e">the arguments</param>
		private void ValueChangedHandler(object sender, EventArgs e)
		{
			IsAltered = true;

            if (ValueChanged != null)
            {
                // Raise the event
                ValueChanged(sender, e);
            }
		}

        /// <summary>
        /// A handler to call when a category changed event is raised from a class element
        /// </summary>
        /// <param name="sender">the class element that raises the event.</param>
        /// <param name="e">the arguments</param>
        private void CategoryChangedHandler(object sender, EventArgs e)
        {
            if (CategoryChanged != null)
            {
                CategoryChanged(sender, e);
            }
        }

		#region IXaclObject

		/// <summary>
		/// Return a xpath for the SchemaModel
		/// </summary>
		/// <returns>a xapth string</returns>
		public string ToXPath()
		{
			if (_xpath == null)
			{
				_xpath = this.Parent.ToXPath() + "/schema";
			}

			return _xpath;
		}

		/// <summary>
		/// Gets parent of IXaclObject
		/// </summary>
		/// <returns>null since the schema model is a root.</returns>
		public IXaclObject Parent
		{
			get
			{
				return _metaData;
			}
		}

		/// <summary>
		/// Gets children of the SchemaModel
		/// </summary>
		/// <returns>The collection of IXaclObject nodes for root classes</returns>
		public IEnumerator GetChildren()
		{
			return this._rootClasses.GetEnumerator();
		}

		#endregion
	}
}