/*
* @(#)MergeMetaDataVisitor.cs
*
* Copyright (c) 2013 Newtera, Inc. All rights reserved.
*
*/
namespace Newtera.Server.DB.MetaData
{
	using System;
    using System.Collections;

	using Newtera.Common.MetaData;
	using Newtera.Common.MetaData.Schema;
	using Newtera.Server.DB;

	/// <summary>
	/// Traverse the new meta data and find modified elements, such as
	/// ClassElement, SimpleAttributeElement, RelationshipAttributeElement, and
	/// EnumElement etc, and merge the changes to the old meta data.
	/// </summary>
	/// <version> 1.0.0 16 Jan 2013 </version>
	public class MergeMetaDataVisitor : ISchemaModelElementVisitor
	{
		private MetaDataModel _newMetaDataModel;
		private MetaDataModel _oldMetaDataModel;
        private SchemaModelElementCollection _usedConstraints;
		
        /// <summary>
        /// Instantiate an instance of MergeMetaDataVisitor class
        /// </summary>
        /// <param name="newMetaDataModel">The new meta data model</param>
        /// <param name="oldMetaDataModel">The old meta data model</param>
        public MergeMetaDataVisitor(MetaDataModel newMetaDataModel,
            MetaDataModel oldMetaDataModel)
        {
            _newMetaDataModel = newMetaDataModel;
            _oldMetaDataModel = oldMetaDataModel;
            _usedConstraints = new SchemaModelElementCollection();
        }

		/// <summary>
		/// Viste a class element.
		/// </summary>
		/// <param name="element">A ClassElement instance</param>
		public void VisitClassElement(ClassElement element)
		{
            if (element.NeedToAlter)
            {
                ClassElement parentClassInOldMetaData;

                if (_oldMetaDataModel.SchemaModel.FindClass(element.Name) == null)
                {
                    // it is a newly created class in the new meta data model,
                    // add it to the old meta data model
                    if (element.ParentClass != null)
                    {
                        // find the parent class in the old meta data model, add the class as a child class
                        parentClassInOldMetaData = _oldMetaDataModel.SchemaModel.FindClass(element.ParentClass.Name);
                        if (parentClassInOldMetaData.IsLeaf)
                        {
                            throw new Exception("Unable to add an class to a bottom class in the existing schema.");
                        }
                        else
                        {
                            parentClassInOldMetaData.AddSubclass(element); // add as a subclass
                        }
                    }
                    else
                    {
                        // add as a root class
                        _oldMetaDataModel.SchemaModel.AddRootClass(element);
                    }
                }
            }
		}

		/// <summary>
		/// Viste a simple attribute element.
		/// </summary>
		/// <param name="element">A SimpleAttributeElement instance</param>
		public void VisitSimpleAttributeElement(SimpleAttributeElement element)
		{
            if (element.OwnerClass.NeedToAlter)
            {
                ClassElement oldClassElement = null;
                SimpleAttributeElement oldAttribute = null;

                if ((oldClassElement = _oldMetaDataModel.SchemaModel.FindClass(element.OwnerClass.Name)) == null)
                {
                    throw new Exception("Unable to find the class " + element.OwnerClass.Name + " in the existing schema that owns attribute " + element.Name);
                }

                if ((oldAttribute = oldClassElement.FindSimpleAttribute(element.Name)) == null)
                {
                    oldClassElement.AddSimpleAttribute(element); // add at the end
                }
                else if (!oldClassElement.NeedToAlter)
                {
                    // the old class is an existing class, replace the old attribute with the new one
                    oldClassElement.SimpleAttributes.Remove(oldAttribute);
                    oldClassElement.SimpleAttributes.Add(element); // element's display order determine its display order
                }

                if (element.Constraint != null)
                {
                    _usedConstraints.Add(element.Constraint);
                }
            }
		}

		/// <summary>
		/// Viste a relationship attribute element.
		/// </summary>
		/// <param name="element">A RelationshipAttributeElement instance</param>
		public void VisitRelationshipAttributeElement(RelationshipAttributeElement element)
		{
            if (element.OwnerClass.NeedToAlter)
            {
                ClassElement oldClassElement = null;
                RelationshipAttributeElement oldAttribute = null;

                if ((oldClassElement = _oldMetaDataModel.SchemaModel.FindClass(element.OwnerClass.Name)) == null)
                {
                    throw new Exception("Unable to find the class " + element.OwnerClass.Name + " in the existing schema that owns relationship " + element.Name);
                }

                if ((oldAttribute = oldClassElement.FindRelationshipAttribute(element.Name)) == null)
                {
                    oldClassElement.AddRelationshipAttribute(element); // add at the end
                }
                else if (!oldClassElement.NeedToAlter)
                {
                    // the old class is an existing class, replace the old relationship with the new one
                    oldClassElement.RelationshipAttributes.Remove(oldAttribute);
                    oldClassElement.RelationshipAttributes.Add(element);
                }
            }
		}

		/// <summary>
		/// Viste an array attribute element.
		/// </summary>
		/// <param name="element">A ArrayAttributeElement instance</param>
		public void VisitArrayAttributeElement(ArrayAttributeElement element)
		{
            if (element.OwnerClass.NeedToAlter)
            {
                ClassElement oldClassElement = null;
                ArrayAttributeElement oldAttribute = null;

                if ((oldClassElement = _oldMetaDataModel.SchemaModel.FindClass(element.OwnerClass.Name)) == null)
                {
                    throw new Exception("Unable to find the class " + element.OwnerClass.Name + " in the existing schema that owns attribute " + element.Name);
                }

                if ((oldAttribute = oldClassElement.FindArrayAttribute(element.Name)) == null)
                {
                    oldClassElement.AddArrayAttribute(element); // add at the end
                }
                else if (!oldClassElement.NeedToAlter)
                {
                    // the old class is an existing class, replace the old attribute with the new one
                    oldClassElement.ArrayAttributes.Remove(oldAttribute);
                    oldClassElement.ArrayAttributes.Add(element);
                }
            }
		}

        /// <summary>
        /// Viste a virtual attribute element.
        /// </summary>
        /// <param name="element">A VirtualAttributeElement instance</param>
        public void VisitVirtualAttributeElement(VirtualAttributeElement element)
        {
            if (element.OwnerClass.NeedToAlter)
            {
                ClassElement oldClassElement = null;
                VirtualAttributeElement oldAttribute = null;

                if ((oldClassElement = _oldMetaDataModel.SchemaModel.FindClass(element.OwnerClass.Name)) == null)
                {
                    throw new Exception("Unable to find the class " + element.OwnerClass.Name + " in the existing schema that owns attribute " + element.Name);
                }

                if ((oldAttribute = oldClassElement.FindVirtualAttribute(element.Name)) == null)
                {
                    oldClassElement.AddVirtualAttribute(element);
                }
                else if (!oldClassElement.NeedToAlter)
                {
                    // the old class is an existing class, replace the old attribute with the new one
                    oldClassElement.VirtualAttributes.Remove(oldAttribute);
                    oldClassElement.VirtualAttributes.Add(element); // element's display order determine its display order
                }

                if (element.Constraint != null)
                {
                    _usedConstraints.Add(element.Constraint);
                }
            }
        }

        /// <summary>
        /// Viste an image attribute element.
        /// </summary>
        /// <param name="element">A ImageAttributeElement instance</param>
        public void VisitImageAttributeElement(ImageAttributeElement element)
        {
            if (element.OwnerClass.NeedToAlter)
            {
                ClassElement oldClassElement = null;
                ImageAttributeElement oldAttribute = null;

                if ((oldClassElement = _oldMetaDataModel.SchemaModel.FindClass(element.OwnerClass.Name)) == null)
                {
                    throw new Exception("Unable to find the class " + element.OwnerClass.Name + " in the existing schema that owns attribute " + element.Name);
                }

                if ((oldAttribute = oldClassElement.FindImageAttribute(element.Name)) == null)
                {
                    oldClassElement.AddImageAttribute(element);
                }
                else if (!oldClassElement.NeedToAlter)
                {
                    // the old class is an existing class, replace the old attribute with the new one
                    oldClassElement.ImageAttributes.Remove(oldAttribute);
                    oldClassElement.ImageAttributes.Add(element); // element's display order determine its display order
                }
            }
        }

        /// <summary>
        /// Viste an custom page element.
        /// </summary>
        /// <param name="element">A CustomPageElement instance</param>
        public void VisitCustomPageElement(CustomPageElement element)
        {
        }

		/// <summary>
		/// Viste a schema info element.
		/// </summary>
		/// <param name="element">A SchemaInfoElement instance</param>
		public void VisitSchemaInfoElement(SchemaInfoElement element)
		{
		}

		/// <summary>
		/// Viste an enum constraint element.
		/// </summary>
		/// <param name="element">A EnumElement instance</param>
		public void VisitEnumElement(EnumElement element)
		{
            // merge the affected constraints to the old meta data model
            if (_usedConstraints.Contains(element))
            {
                ConstraintElementBase oldConstraint = _oldMetaDataModel.SchemaModel.FindConstraint(element.Name);
                if (oldConstraint == null)
                {
                    _oldMetaDataModel.SchemaModel.AddEnumConstraint(element);
                }
                else if (oldConstraint != null && oldConstraint is EnumElement)
                {
                    EnumElement oldEnumElement = (EnumElement)oldConstraint;
                    // if new enum does not have all the values of the old enum element,
                    // the Compare method will return -1
                    if (EnumElement.Compare(element, oldEnumElement) < 0)
                    {
                        _oldMetaDataModel.SchemaModel.RemoveEnumConstraint(element.Name);
                        _oldMetaDataModel.SchemaModel.AddEnumConstraint(element);
                    }
                }
                else
                {
                    throw new Exception("Unable to merge the enum constraint with name " + element.Name + ". An constraint with the same name already exists which is not an enum constraint.");
                }
            }
		}

		/// <summary>
		/// Viste a range constraint element.
		/// </summary>
		/// <param name="element">A RangeElement instance</param>
		public void VisitRangeElement(RangeElement element)
		{
            // merge the affected constraints to the old meta data model
            if (_usedConstraints.Contains(element))
            {
                ConstraintElementBase oldConstraint = _oldMetaDataModel.SchemaModel.FindConstraint(element.Name);
                if (oldConstraint == null)
                {
                    _oldMetaDataModel.SchemaModel.AddRangeConstraint(element);
                }
                else if (oldConstraint != null && oldConstraint is RangeElement)
                {
                    RangeElement oldRangeElement = (RangeElement)oldConstraint;
                    // if range definitions are different,
                    // the Compare method will return false
                    if (!RangeElement.Compare(element, oldRangeElement))
                    {
                        _oldMetaDataModel.SchemaModel.RemoveRangeConstraint(element.Name);
                        _oldMetaDataModel.SchemaModel.AddRangeConstraint(element);
                    }
                }
                else
                {
                    throw new Exception("Unable to merge the range constraint with name " + element.Name + ". An constraint with the same name already exists which is not a range constraint.");
                }
            }
		}

		/// <summary>
		/// Viste a pattern constraint element.
		/// </summary>
		/// <param name="element">A PatternElement instance</param>
		public void VisitPatternElement(PatternElement element)
		{
            // merge the affected constraints to the old meta data model
            if (_usedConstraints.Contains(element))
            {
                ConstraintElementBase oldConstraint = _oldMetaDataModel.SchemaModel.FindConstraint(element.Name);
                if (oldConstraint == null)
                {
                    _oldMetaDataModel.SchemaModel.AddPatternConstraint(element);
                }
                else if (oldConstraint != null && oldConstraint is PatternElement)
                {
                    PatternElement oldPatternElement = (PatternElement)oldConstraint;
                    // if the pattern definitions are different,
                    // the Compare method will return false
                    if (!PatternElement.Compare(element, oldPatternElement))
                    {
                        _oldMetaDataModel.SchemaModel.RemovePatternConstraint(element.Name);
                        _oldMetaDataModel.SchemaModel.AddPatternConstraint(element);
                    }
                }
                else
                {
                    throw new Exception("Unable to merge the pattern constraint with name " + element.Name + ". An constraint with the same name already exists which is not a pattern constraint.");
                }
            }
		}

		/// <summary>
		/// Viste a list constraint element.
		/// </summary>
		/// <param name="element">A ListElement instance</param>
		public void VisitListElement(ListElement element)
		{
            // merge the affected constraints to the old meta data model
            if (_usedConstraints.Contains(element))
            {
                ConstraintElementBase oldConstraint = _oldMetaDataModel.SchemaModel.FindConstraint(element.Name);
                if (oldConstraint == null)
                {
                    _oldMetaDataModel.SchemaModel.AddListConstraint(element);
                }
                else if (oldConstraint != null && oldConstraint is ListElement)
                {
                    ListElement oldListElement = (ListElement)oldConstraint;
                    // if the pattern definitions are different,
                    // the Compare method will return false
                    if (!ListElement.Compare(element, oldListElement))
                    {
                        _oldMetaDataModel.SchemaModel.RemoveListConstraint(element.Name);
                        _oldMetaDataModel.SchemaModel.AddListConstraint(element);
                    }
                }
                else
                {
                    throw new Exception("Unable to merge the list constraint with name " + element.Name + ". An constraint with the same name already exists which is not a list constraint.");
                }
            }
		}
	}
}