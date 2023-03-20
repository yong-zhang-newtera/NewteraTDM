/*
* @(#)RelationshipEntity.cs
*
* Copyright (c) 2003 Newtera, Inc. All rights reserved.
*
*/
namespace Newtera.Server.Engine.Sqlbuilder
{
	using System;
	using System.Xml;
	using System.Collections;

	using Newtera.Common.MetaData.Schema;
	using Newtera.Server.Engine.Sqlbuilder.Sql;
	using Newtera.Common.Core;
    using Newtera.Common.MetaData.XaclModel;

	/// <summary>
	/// A RelationshipEntity object represents a relationship between two classes in an
	/// object-relational data model. A relationship stores information about how a
	/// relationship is mapped to a relational database foreign key(s).
	/// </summary>
	/// <version>  	1.0.1 08 Jul 2003</version>
	/// <author>  Yong Zhang </author>
	public class RelationshipEntity : DBEntity
	{
		/* private instance variables */
		private ClassEntity _referencedClass; // The referenced class    
		private RelationshipAttributeElement _relationshipElement; // Its counter-part in the meta model
		private RelationshipEntity _referencedRelationship; // The opposite relationship entity
        private ClassEntity _baseClass; // the base class

		/// <summary>
		/// Initiating an instance of RelationshipEntity class.
		/// </summary>
		/// <param name="element">the relationship element in meta model.</param>
		public RelationshipEntity(RelationshipAttributeElement element) : base()
		{
			_relationshipElement = element;
			_referencedRelationship = null;
			// create the referenced class with its empty inherited classes
			_referencedClass = new ClassEntity(_relationshipElement.LinkedClass);
			_referencedClass.CreateEmptyAncestorClasses();
            _baseClass = null;
		}

		/// <summary>
		/// Gets Schema Model element for this RelationshipEntity object. metaData element
		/// provides meta data information about this relationship.
		/// </summary>
		/// <returns> a RelationshipAttributeElement object.</returns>
		public RelationshipAttributeElement SchemaModelElement
		{
			get
			{
				return _relationshipElement;
			}
		}

		/// <summary>
		/// Gets the relationship attribute name.
		/// </summary>
		/// <value> the relationship attribute name.</value>
		public override string Name
		{
			get
			{
				return _relationshipElement.Name;
			}
		}

		/// <summary>
		/// Gets the data type of relationship.
		/// </summary>
		/// <value> one of DataType enum.</value>
		/// <remarks>Relationships are connected through object ids. Therefore, the value of a
		/// relationship attribute is an object id. The id type is Integer.
		/// </remarks>
		public override DataType Type
		{
			get
			{
				return SQLElement.OBJ_ID_TYPE;
			}
		}

		/// <summary>
		/// Gets the DB foreign key name for the relationship attribute. The foreign key name is
		/// provided by meta model element.
		/// </summary>
		/// <value> the foreign key name for the relationship attribute.</value>
		public override string ColumnName
		{
			get
			{
				return _relationshipElement.ColumnName;
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
		/// Gets  or sets the referenced class of this relationship.
		/// </summary>
		/// <value> The referenced class. </value>
		public ClassEntity LinkedClass
		{
			get
			{
				return _referencedClass;
			}
            set
            {
                _referencedClass = value;
            }
		}

		/// <summary>
		/// Gets the corresponding relationship from the referenced class.
		/// </summary>
		/// <value> The relationship on the side of referenced class.</value>
		public RelationshipEntity ReferencedRelationship
		{
			get
			{
				if (_referencedRelationship == null)
				{
					//referencedRelationship = new RelationshipEntity(_relationshipElement.BackwardRelationship);
                    string relationshipName = _relationshipElement.BackwardRelationship.Name;

                    _referencedRelationship = _referencedClass.GetInheritedRelationship(relationshipName);

                    if (_referencedRelationship == null)
                    {
                        throw new Exception("Unable to find an inherited relationship with name " + relationshipName + " in the class " + _referencedClass.Name);
                    }
				}
				
				return _referencedRelationship;
			}
		}

		/// <summary>
		/// Gets the information indicating whether the the relationship is a required attribute.
		/// </summary>
		/// <true> true if it is required, false otherwise.</value>
		public bool IsRequired
		{
			get
			{
				return _relationshipElement.IsRequired;
			}
		}

		/// <summary>
		/// Get the direction of relationship. The direction is called forward if the owner class
		/// has a foreign key pointing to the referenced class, and backward, otherwise.
		/// 
		/// The direction of a relationship is determined based on following criteria:
		/// 
		/// 1. If the relationship type is many-to-one, the direction is forward.
		/// 2. If the relationship type is one-to-one and owner class is the join manager,
		/// the direction is forward.
		/// 3. Other than above, the direction is considered backward.
		/// </summary>
		/// <value> one of RelationshipDirection enum values.</value>
		public RelationshipDirection Direction
		{
			get
			{
				RelationshipDirection direction = RelationshipDirection.Backward;
				
				switch (_relationshipElement.Type)
				{
					case RelationshipType.ManyToOne:
						direction = RelationshipDirection.Forward;
						break;
					case RelationshipType.OneToOne:
						if (!_relationshipElement.IsJoinManager)
						{
							// TODO, the assumption is that a foreign key for one-to-one
							// relationship is created at the non-joinmanager side. If this
							// assumption does not hold true, it may break.
							direction = RelationshipDirection.Forward;
						}
						break;
					default:
						break;
				}
				
				return direction;
			}
		}
				
		/// <summary>
		/// Accept a EntityVistor to visit itself and referenced class.
		/// </summary>
		/// <param name="visitor">the visiting visitor.</param>
		public override void Accept(EntityVisitor visitor)
		{
			// visit itself
			if (visitor.VisitRelationship(this))
			{
				//_referencedClass.Accept(visitor); // Move this action to ClassEntity's accept method to delay visit related classes
			}
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
                _xpath = this.Parent.ToXPath() + "/@" + this.Name + NewteraNameSpace.ATTRIBUTE_SUFFIX;
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
                // the base class is the leaf class of the inheritance tree
                return BaseClassEntity;
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