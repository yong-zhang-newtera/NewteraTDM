/*
* @(#)FindEntityVisitor.cs
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
	using Newtera.Common.MetaData;
	using Newtera.Common.MetaData.Schema;

	/// <summary>
	/// A FindEntityVisitor an entity in an entity tree based on the given XPath.
	/// </summary>
	/// <version>  	1.0.1 31 Jul 2003</version>
	/// <author>  Yong Zhang </author>
	public class FindEntityVisitor : PathIteratorEntityVisitor, EntityVisitor
	{
		// private instance members
		private bool _expectingClass = false;
		
		/// <summary>
		/// Initiating an instance of FindEntityVisitor class.
		/// </summary>
		/// <param name="metaData">the meta model object</param>
		public FindEntityVisitor(MetaDataModel metaData) : base(metaData)
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
								throw new SQLBuilderException("Unknown step " + stepName);
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
						throw new SQLBuilderException(stepName + " is an unknow element attribute name");
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
						throw new SQLBuilderException(stepName + " is an unknown function name");
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
		/// Visits a RelationshipEntity object.
		/// </summary>
		/// <param name="entity">the relationship entity to be visited.</param>
		public bool VisitRelationship(RelationshipEntity entity)
		{	
			string stepName = _currentStep.Name;
			
			switch (_currentStep.AxisCode)
			{
				case Axis.DEREFERENCE: 
					ClassEntity referencedClass = entity.LinkedClass;
					
					if (!referencedClass.HasInheritedAttributes() && !referencedClass.HasInheritedRelationships())
					{
						
						// The reference class is empty
                        throw new SQLBuilderException(referencedClass.Name + " is a class without any properties");
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
		/// Visits a DBEntity object representing an function such as count, avg, min, max, sum.
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
						throw new SQLBuilderException(stepName + " is an unknown attribute");
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
						throw new SQLBuilderException(stepName + " is an unknown element attribute");
					}
					
					break;
				
				default: 
					throw new SQLBuilderException("Unexpected axis " + _currentStep.AxisCode + " found in " + PathEnumerator.ToString());
				
			}
			
			return false;
		}
		
		/// <summary>
		/// Visits a SchemaEntity object representing a query schema.
		/// </summary>
		/// <param name="entity">the schema entity to be visited.</param>
		public virtual bool VisitSchema(SchemaEntity entity)
		{
			string stepName = _currentStep.Name;
			
			switch (_currentStep.AxisCode)
			{
				
				case Axis.DESCENDANT:
				case Axis.CHILD:
					DBEntityCollection roots = entity.RootEntities;
					bool found = false;
					
					for (int i = 0; i < roots.Count; i++)
					{
						_currentEntity = (DBEntity) roots[i];
						if (_currentEntity is ClassEntity)
						{
							if (IsValidClassListName(stepName, _currentEntity.Name))
							{
								// it is a class list name, we expect a class name in next step
								_expectingClass = true;
								found = true;
								break;
							}
							else if (_currentEntity.Name == stepName)
							{
								found = true;
								break;
							}
						}
						else if (_currentEntity is AggregateFuncEntity)
						{
							if (((AggregateFuncEntity) _currentEntity).RootEntity.Name == stepName)
							{
								_currentEntity = ((AggregateFuncEntity) _currentEntity).RootEntity;
								found = true;
								break;
							}
						}
					}
					
					if (!found)
					{
						throw new SQLBuilderException("Failed to find an entity for the step " + stepName);
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