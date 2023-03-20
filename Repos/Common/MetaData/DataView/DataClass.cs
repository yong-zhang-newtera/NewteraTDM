/*
* @(#)DataClass.cs
*
* Copyright (c) 2003 Newtera, Inc. All rights reserved.
*
*/
namespace Newtera.Common.MetaData.DataView
{
	using System;
	using System.Xml;
    using System.Text;

	using Newtera.Common.MetaData.Schema;

	/// <summary>
	/// A DataClass instance represents a class in a data view. It can represent either
	/// a base class or a referenced class.
	/// </summary>
	/// 
	/// <version>1.0.1 28 Oct 2003</version>
	/// <author>Yong Zhang</author>
	public class DataClass : DataViewElementBase
	{
        private const char PRIMARY_KEY_SEPARATOR = '&';

		private string _alias;
		private DataClassType _type;
		private string _className;
		private string _referringClassAlias;
		private string _referringRelationshipName;
		private bool _isReferenced;
        private ReferencedClassCollection _referencedClasses = null; // run-time use
        private RelationshipAttributeElement _referringRelatioshipElement = null; // run-time
        private bool _isLeafClass;

		/// <summary>
		/// Initiating an instance of DataClass class
		/// </summary>
		/// <param name="name">Name of the element</param>
		/// <param name="className">The actual name of the class</param>
		/// <param name="type">One of the DataClassType values</param>
		public DataClass(string name, string className, DataClassType type) : base(name)
		{
			_type = type;
			_alias = null;
			_className = className;
			_referringClassAlias = null;
			_referringRelationshipName = null;
			_isReferenced = false; // run-time use
            _isLeafClass = true;
		}

		/// <summary>
		/// Initiating an instance of DataClass class
		/// </summary>
		/// <param name="xmlElement">The xml element conatins data of the instance</param>
		internal DataClass(XmlElement xmlElement) : base()
		{
			Unmarshal(xmlElement);
			_isReferenced = false; // run-time use
            _isLeafClass = true;
		}
		
		/// <summary>
		/// Gets the type of element
		/// </summary>
		/// <value>One of ElementType values</value>
		public override ElementType ElementType {
			get
			{
				return ElementType.Class;
			}
		}

		/// <summary>
		/// Gets the type of data class
		/// </summary>
		/// <value>One of DataClassType values</value>
		public DataClassType Type
		{
			get
			{
				return _type;
			}
		}

		/// <summary>
		/// Gets the alias of the class. Tha alias is used in an XQuery
		/// </summary>
		public string Alias
		{
			get
			{
				if (_alias == null)
				{
					// using the lower case of class name as alias
					_alias = Name.ToLower();
				}

				return _alias;
			}
		}

		/// <summary>
		/// Gets or sets name of the class.
		/// </summary>
		/// <value>Name of the class</value>
		/// <remarks>The class name can be different from the element name
		/// where the element name is unique, while the class name is among
		/// the base class and referenced classes.</remarks>
		public string ClassName
		{
			get
			{
				return _className;
			}
			set
			{
				_className = value;
			}
		}

		/// <summary>
		/// Gets or sets alias of the referring class to this class.
		/// </summary>
		/// <value>Alias of the referring class, null if this is a base class.</value>
		public string ReferringClassAlias
		{
			get
			{
				return _referringClassAlias;
			}
			set
			{
				_referringClassAlias = value;
			}
		}

		/// <summary>
		/// Gets or sets name of the relationship attribute that establish the relation
		/// between the referring class and this class. The relationship
		/// attribute is on the referring class side.
		/// </summary>
		/// <value>Name of the relationship attribute, null if this is a base class.</value>
		public string ReferringRelationshipName
		{
			get
			{
				return _referringRelationshipName;
			}
			set
			{
				_referringRelationshipName = value;
			}
		}

        /// <summary>
        /// Gets or sets the relationship attribute that establish the relation
        /// between the referring class and this class. The relationship
        /// attribute is on the referring class side.
        /// </summary>
        public RelationshipAttributeElement ReferringRelationship
        {
            get
            {
                if (_referringRelatioshipElement == null && _referringRelationshipName != null)
                {
                    throw new Exception("ReferringRelationship has not been set during the run-time");
                }

                return _referringRelatioshipElement;
            }
            set
            {
                _referringRelatioshipElement = value;
            }
        }

		/// <summary>
		/// Gets or sets the information indicating whether the referenced class is
		/// referenced in the search or result parts of a query
		/// </summary>
		/// <value>true if it is referenced, false otherwise. default is false.</value>
		public bool IsReferenced
		{
			get
			{
				return _isReferenced;
			}
			set
			{
				_isReferenced = value;
			}
		}

        /// <summary>
        /// Gets or sets the information indicating whether the class represented by the data class is
        /// a leaf class
        /// </summary>
        /// <value>true if it is leaf class, false otherwise.</value>
        public bool IsLeafClass
        {
            get
            {
                return _isLeafClass;
            }
            set
            {
                _isLeafClass = value;
            }
        }

        /// <summary>
        /// Gets or sets a collection of the DataClass objects that represents the data classes that are
        /// referenced to this data class through the relationship attributes
        /// </summary>
        public ReferencedClassCollection RelatedClasses
        {
            get
            {
                if (_referencedClasses == null)
                {
                    _referencedClasses = new ReferencedClassCollection();
                }

                return _referencedClasses;
            }
            set
            {
                _referencedClasses = value;
            }
        }

        /// <summary>
        /// Get the name representing the primary key of the class
        /// </summary>
        public string PrimaryKeyName
        {
            get
            {
                string primaryKeyName = null;

                ClassElement currentClass = GetSchemaModelElement() as ClassElement;

                while (currentClass != null)
                {
                    SchemaModelElementCollection pks = currentClass.PrimaryKeys;
                    StringBuilder pkNames = new StringBuilder();
                    if (pks != null && pks.Count > 0)
                    {
                        for (int i = 0; i < pks.Count; i++)
                        {
                            if (pkNames.Length == 0)
                            {
                                pkNames.Append(pks[i].Name);
                            }
                            else
                            {
                                pkNames.Append(PRIMARY_KEY_SEPARATOR).Append(pks[i].Name);
                            }
                        }

                        primaryKeyName = pkNames.ToString();
                        break;
                    }

                    currentClass = currentClass.ParentClass;
                }

                return primaryKeyName;

            }
        }

		/// <summary>
		/// Accept a visitor of IDataViewElementVisitor type to traverse its
		/// elements.
		/// </summary>
		/// <param name="visitor">A visitor</param>
		public override void Accept(IDataViewElementVisitor visitor)
		{
			visitor.VisitDataClass(this);
		}
		
		/// <summary>
		/// Gets or sets the schema model element that the data view element associates with.
		/// </summary>
		/// <value>The SchemaModelElement.</value>
		public override SchemaModelElement GetSchemaModelElement()
		{
			if (_schemaModelElement == null && DataView != null)
			{
				DataClass ownerClass = DataView.FindClass(Alias);
                if (ownerClass != null)
                {
                    _schemaModelElement = DataView.SchemaModel.FindClass(ownerClass.ClassName);
                }
			}

			return _schemaModelElement;
		}

		/// <summary>
		/// sets the element members from a XML element.
		/// </summary>
		/// <param name="parent">An xml element</param>
		public override void Unmarshal(XmlElement parent)
		{
			base.Unmarshal(parent);

			// set _className member
			_className = parent.GetAttribute("ClassName");

			// set value of type member
			string typeStr = parent.GetAttribute("Type");

			_type = ConvertStringToDataClassType(typeStr);

			_referringClassAlias = parent.GetAttribute("ParentClass");

			_referringRelationshipName = parent.GetAttribute("ParentRelationship");
		}

		/// <summary>
		/// Write values of members to an xml element
		/// </summary>
		/// <param name="parent">An xml element for the element</param>
		public override void Marshal(XmlElement parent)
		{
			base.Marshal(parent);

			// write the _className member
			parent.SetAttribute("ClassName", _className);

			// write the type member
			string typeStr = ConvertDataClassTypeToString(_type);
			parent.SetAttribute("Type", typeStr);

			if (_referringClassAlias != null && _referringClassAlias.Length > 0)
			{
				parent.SetAttribute("ParentClass", _referringClassAlias);
			}

			if (_referringRelationshipName != null && _referringRelationshipName.Length > 0)
			{
				parent.SetAttribute("ParentRelationship", _referringRelationshipName);
			}
		}

		/// <summary>
		/// Text representation of the element
		/// </summary>
		/// <returns>A String</returns>
		public override string ToString()
		{
			return Caption;
		}

		/// <summary>
		/// Convert a DataClassType value to a string
		/// </summary>
		/// <param name="type">One of DataClassType value</param>
		/// <returns>The corresponding string</returns>
		private string ConvertDataClassTypeToString(DataClassType type)
		{
			string str = "Unknown";

			switch (type)
			{
				case DataClassType.BaseClass:
					str = "Base";
					break;

				case DataClassType.ReferencedClass:
					str = "Referenced";
					break;
			}

			return str;
		}

		/// <summary>
		/// Convert a string to a DataClassType value
		/// </summary>
		/// <param name="str">A string</param>
		/// <returns>One of DataClassType value</returns>
		private DataClassType ConvertStringToDataClassType(string str)
		{
			DataClassType type = DataClassType.Unknown;

			if (str != null && str.Length > 0)
			{
				switch (str)
				{
					case "Base":
						type = DataClassType.BaseClass;
						break;

					case "Referenced":
						type = DataClassType.ReferencedClass;
						break;
				}
			}

			return type;
		}
	}

	/// <summary>
	/// Specify the types of data class.
	/// </summary>
	public enum DataClassType
	{
		/// <summary>
		/// Unknown Data Class Type
		/// </summary>
		Unknown = 0,
		/// <summary>
		/// Base Class Type
		/// </summary>
		BaseClass,
		/// <summary>
		/// Referenced Class Type
		/// </summary>
		ReferencedClass
	}
}