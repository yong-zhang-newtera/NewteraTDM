/*
* @(#)FindExtendingEntityVisitor.cs
*
* Copyright (c) 2003 Newtera, Inc. All rights reserved.
*
*/
namespace Newtera.Server.Engine.Vdom.Dbimp
{
	using System;
	using Newtera.Server.Engine.Interpreter;
	using Newtera.Server.Engine.Sqlbuilder;
	using Newtera.Server.Engine.Sqlbuilder.Sql;
	using Newtera.Common.MetaData.Schema;
	using Newtera.Common.MetaData;

	/// <summary>
	/// A FindExtendingEntityVisitor an entity from where to extend rest of branch
	/// </summary>
	/// <version>  	1.0.0 31 Jul 2003 </version>
	/// <author>  Yong Zhang </author>
	public class FindExtendingEntityVisitor : PathIteratorEntityVisitor, EntityVisitor
	{
		// private instance members
		private bool _expectingClass = false;
		
		/// <summary>
		/// Initiating an instance of FindExtendingEntityVisitor class.
		/// </summary>
		/// <param name="metaData">the meta model object.</param>
		public FindExtendingEntityVisitor(MetaDataModel metaData) : base(metaData)
		{
		}
		
		/// <summary>
		/// Visits a ClassEntity object.
		/// </summary>
		/// <param name="entity">the class entity object to be visited.</param>
		public bool VisitClass(ClassEntity entity)
		{
			string stepName = _currentStep.Name;
			
			switch (_currentStep.AxisCode)
			{
				
				case Axis.CHILD: 
					if (_expectingClass)
					{
						/*
						* The step must represents a class
						*/
						if (entity.Name == stepName)
						{
							_expectingClass = false;
							_currentEntity = entity;
						}
						else
						{
							throw new SQLBuilderException(stepName + " is an invalid class name");
						}
					}
					else
					{
						/*
						* The step must represent a AttributeEntity or a function entity
						* Therefore, try to find it from the list of AttributeEntity or
						* function objects of the current class entity.
						*/
						AttributeEntity foundAttribute = entity.GetInheritedAttribute(stepName);
						
						if (foundAttribute != null)
						{
							foundAttribute.IsReferenced = true;
							_currentEntity = foundAttribute;
						}
						else
						{
							// try to see if the step represents a function entity
							DBEntity function = entity.GetFunction(stepName);
							if (function != null)
							{
								_currentEntity = function;
							}
							else
							{
								_stopIteration = true; // Extending point found
							}
						}
					}
					
					break;
				
				case Axis.DESCENDANT: 
					if (IsValidClassListName(stepName, entity.Name))
					{
						// it is a class list name, we expect a class name in next step
						_expectingClass = true;
					}
					else if (entity.Name != stepName)
					{
						throw new SQLBuilderException(stepName + " is an invalid class name");
					}
					
					break;
				
				case Axis.ATTRIBUTE: 
					RelationshipEntity foundRelationship = entity.GetInheritedRelationship(stepName);
					
					if (foundRelationship != null)
					{
						_currentEntity = foundRelationship;
					}
					else if (entity.ObjIdEntity.Name == stepName)
					{
						// It is a obj_id attribute
						_currentEntity = entity.ObjIdEntity;
					}
					else if (entity.ClsIdEntity.Name == stepName)
					{
						// It is a class type attribute
						_currentEntity = entity.ClsIdEntity;
					}
					else if (entity.AttachmentEntity.Name == stepName)
					{
						// It is an attachment type attribute
						_currentEntity = entity.AttachmentEntity;
					}
					else if (entity.PermissionEntity.Name == stepName)
					{
						// It is a permission attribute
						_currentEntity = entity.PermissionEntity;
					}
                    else if (entity.ReadOnlyEntity.Name == stepName)
                    {
                        // It is a read-only attribute
                        _currentEntity = entity.ReadOnlyEntity;
                    }
					else
					{
						_stopIteration = true; // Extending point found
					}
					
					break;
				
				case Axis.FUNCTION: 
					DBEntity functionEntity = entity.GetFunction(stepName);
					
					if (functionEntity != null)
					{
						_currentEntity = functionEntity;
					}
					else
					{
						_stopIteration = true; // Extending point found
					}
					
					break;
				
				default: 
					throw new SQLBuilderException("Unexpected axis " + _currentStep.AxisCode + " found for step " + stepName);
				
			}
			
			return false;
		}
		
		/// <summary>
		/// Visits an AttributeEntity object and add TABLE_ALIAS property to the object.
		/// </summary>
		/// <param name="entity">the attribute entity to be visited.</param>
		public bool VisitAttribute(AttributeEntity entity)
		{
			return false;
		}
		
		/// <summary>
		/// Visit a RelationshipEntity object.
		/// </summary>
		/// <param name="entity">the relationship entity to be visited.</param>
		public virtual bool VisitRelationship(RelationshipEntity entity)
		{	
			string stepName = _currentStep.Name;
			
			switch (_currentStep.AxisCode)
			{
				
				case Axis.DEREFERENCE: 
					ClassEntity referencedClass = entity.LinkedClass;
					
					if (!referencedClass.HasInheritedAttributes() && !referencedClass.HasInheritedRelationships())
					{
						_stopIteration = true; // Extending point found
					}
					else if (referencedClass.Name == stepName)
					{
						_currentEntity = referencedClass;
					}
					else
					{
						throw new SQLBuilderException(stepName + " is an invalid class name");
					}
					
					break;
				
				default: 
					throw new SQLBuilderException("Unexpected axis " + _currentStep.AxisCode + " found for step " + stepName);
			}
			
			return false;
		}
		
		/// <summary>
		/// Visit a DBEntity object representing an function such as count, avg, min, max, sum.
		/// </summary>
		/// <param name="entity">the function entity to be visited.</param>
		public bool VisitFunction(AggregateFuncEntity entity)
		{
			string stepName = _currentStep.Name;
			DBEntity foundEntity;
			
			switch (_currentStep.AxisCode)
			{
				case Axis.CHILD: 
					
					foundEntity = entity.RootEntity;
					
					if (foundEntity != null && foundEntity is AttributeEntity && foundEntity.Name == stepName)
					{
						_currentEntity = foundEntity;
					}
					else
					{
						_stopIteration = true; // Extending point found
					}
					
					break;
				
				case Axis.DESCENDANT: 
					
					if (entity.RootEntity.Name == stepName)
					{
						_currentEntity = entity.RootEntity;
					}
					else
					{
						throw new SQLBuilderException("Unknown function name " + stepName);
					}
					
					break;
				
				case Axis.ATTRIBUTE: 
					
					foundEntity = entity.RootEntity;
					
					if (foundEntity != null && foundEntity is RelationshipEntity && foundEntity.Name == stepName)
					{
						_currentEntity = foundEntity;
					}
					else
					{
						_stopIteration = true; // Extending point found
					}
					
					break;
				
				default: 
					throw new SQLBuilderException("Unexpected axis " + _currentStep.AxisCode + " found for step " + stepName);
			}
			
			return false;
		}
		
		/// <summary>
		/// Visits a SchemaEntity object representing a query schema.
		/// </summary>
		/// <param name="entity">the schema entity to be visited.</param>
		public bool VisitSchema(SchemaEntity entity)
		{
			string stepName = _currentStep.Name;
			
			switch (_currentStep.AxisCode)
			{
				
				case Axis.DESCENDANT: 
				case Axis.CHILD:
					DBEntity currentEntity = null;
					DBEntityCollection roots = entity.RootEntities;
					bool found = false;
					
					for (int i = 0; i < roots.Count; i++)
					{
						currentEntity = roots[i];
						if (currentEntity is ClassEntity)
						{
							if (IsValidClassListName(stepName, currentEntity.Name))
							{
								// it is a class list name, we expect a class name in next step
								_expectingClass = true;
								found = true;
								break;
							}
							else if (currentEntity.Name == stepName)
							{
								found = true;
								break;
							}
						}
						else if (currentEntity is AggregateFuncEntity)
						{
							if (((AggregateFuncEntity) currentEntity).RootEntity.Name == stepName)
							{
								currentEntity = ((AggregateFuncEntity) currentEntity).RootEntity;
								found = true;
								break;
							}
						}
					}
					
					if (!found)
					{
						_currentEntity = entity; // SchemaEntity is extending entity
						_stopIteration = true; // Extending point found
					}
					else
					{
						_currentEntity = currentEntity; // found entity is current entity
					}
					
					break;
				
				case Axis.FUNCTION: 
					if (entity.RootEntities.Count == 1)
					{
						_currentEntity = (DBEntity) entity.RootEntities[0];
						
						if (_currentEntity is AggregateFuncEntity)
						{
							if (_currentEntity.Name != stepName)
							{
								throw new SQLBuilderException("Failed to match " + stepName + " to a function");
							}
						}
						else
						{
							throw new SQLBuilderException("Unknown function " + stepName + " is found");
						}
					}
					else
					{
						throw new SQLBuilderException("Misused function " + stepName + " is encountered");
					}
					
					break;
				
				default: 
					throw new SQLBuilderException("Unexpected axis " + _currentStep.AxisCode + " found for step " + stepName);
			}
			
			return false;
		}
		
		/// <summary>
		/// Visits a ScoreEntity object.
		/// </summary>
		/// <param name="entity">the score entity to be visited.</param>
		public bool VisitScore(ScoreEntity entity)
		{
			return false;
		}
	}
}