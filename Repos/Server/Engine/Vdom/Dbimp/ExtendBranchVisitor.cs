/*
* @(#)ExtendBranchVisitor.cs
*
* Copyright (c) 2003 Newtera, Inc. All rights reserved.
*
*/
namespace Newtera.Server.Engine.Vdom.Dbimp
{
	using System;
	using System.Collections;
	using System.Xml;
	using System.Data;

	using Newtera.Server.Engine.Interpreter;
	using Newtera.Server.Engine.Sqlbuilder;
	using Newtera.Server.Engine.Sqlbuilder.Sql;
	using Newtera.Common.MetaData.XaclModel.Processor;
	using Newtera.Common.MetaData;
	using Newtera.Common.MetaData.Schema;
	using Newtera.Common.MetaData.XaclModel;
	using Newtera.Server.DB;

	/// <summary>
	/// A ExtendBranchVisitor an entity in an entity tree based on the given XPath.
	/// </summary>
	/// <version>  	1.0.1 31 Jul 2003 </version>
	/// <author>  Yong Zhang  </author>
	public class ExtendBranchVisitor : PathIteratorEntityVisitor, EntityVisitor
	{
		private static string VARIABLE_NAME = "$this/"; // The variable name at beginning of a xpath	
		
		// private instance members
		private IDataProvider _dataProvider;
		private Interpreter _interpreter;
		
		/// <summary>
		/// Constructor for an ExtendBranchVisitor object.
		/// </summary>
		/// <param name="metaData">the meta model object.</param>
		public ExtendBranchVisitor(MetaDataModel metaData, Interpreter interpreter, IDataProvider dataProvider) : base(metaData)
		{
			_dataProvider = dataProvider;
			_interpreter = interpreter;
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
					AttributeEntity createdAttribute = entity.CreateInheritedAttribute(stepName);
					
					if (createdAttribute != null)
					{
						_currentEntity = createdAttribute;
					}
					else
					{
						throw new SQLBuilderException("Property " + stepName + " doesn't exist in class " + entity.Name);
					}
					
					break;
				
				case Axis.ATTRIBUTE: 
					RelationshipEntity createdRelationship = entity.CreateInheritedRelationship(stepName);
					
					if (createdRelationship != null)
					{
						_currentEntity = createdRelationship;
					}
					else
					{
						throw new SQLBuilderException("Attribute " + stepName + " doesn't exist in class " + entity.Name);
					}
					
					break;
				
				case Axis.FUNCTION: 
					DBEntity funcEntity;
					
					/*
					* Step with function axis is to represent an aggregate function.
					* Create an AggregateFuncEntity for it. The function step can be the first
					* step of a path
					*/
					funcEntity = new AggregateFuncEntity(stepName, _dataProvider);
					// link the function entity to the class entity
					entity.AddFunction(funcEntity);
					
					_currentEntity = funcEntity;
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
			// check the read permission to the relationship entity
			if (!PermissionChecker.Instance.HasPermission(_metaData.XaclPolicy, entity, XaclActionType.Read))
			{
				throw new PermissionViolationException("Do not have read permission to relationship " + entity.Name);
			}
			
			string stepName = _currentStep.Name;
			
			switch (_currentStep.AxisCode)
			{
				
				case Axis.DEREFERENCE: 
					ClassEntity referencedClass = entity.LinkedClass;

                    if (referencedClass.Name != stepName && IsParentOf(referencedClass.Name, stepName))
                    {
                        ClassElement childClassElement = _metaData.SchemaModel.FindClass(stepName);
                        referencedClass = new ClassEntity(childClassElement);
                        entity.LinkedClass = referencedClass;
                    }
					
					if (referencedClass.Name == stepName)
					{
						// check the read permission to the referenced class entity				
						Conclusion conclusion = PermissionChecker.Instance.GetConclusion(_metaData.XaclPolicy, referencedClass.SchemaElement, XaclActionType.Read);
						
						if (conclusion.Permission == XaclPermissionType.ConditionalDeny ||
							conclusion.Permission == XaclPermissionType.ConditionalGrant)
						{
							/*
							* add the conditions as part of xquery to filter out unwanted instances.
							*/
                            Hashtable addedConditions = new Hashtable();
                            // Note: The master class is left outer joined with the referenced classes,
                            // therefore, adding conditions defined for the referenced classes causes
                            // resulting query failing to return any results in the master class.
                            //AddCondition(_currentStep, conclusion, addedConditions);
						}
						else if (conclusion.Permission == XaclPermissionType.Deny)
						{
							throw new PermissionViolationException("Do not have read permission to class " + referencedClass.Name);
						}
						
						/*
						* Create attributes of local and inherited for the class
						*
						* TODO: create only those attributes that are actually used
						* (referenced) in a query. This strategy will help to generate
						* optimized SQL without getting unused data from database.
						*/
						referencedClass.MakeFullBlown();
						
						_currentEntity = referencedClass;
					}
					else
					{
						throw new SQLBuilderException("Class " + stepName + " is not a referenced class");
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
			ClassEntity ownerClass;

			switch (_currentStep.AxisCode)
			{
				
				case Axis.CHILD: 
					if (IsValidClassName(stepName))
					{
						ClassEntity clsEntity = CreateClassEntity(_currentStep);
						
						entity.RootEntity = clsEntity;
						
						_currentEntity = clsEntity;
					}
					else
					{
						/*
						* Then try to see if the step represents an attribute.
						*/
						ownerClass = entity.OwnerClass;
						AttributeEntity createdAttribute = ownerClass.CreateInheritedAttribute(stepName);
						
						if (createdAttribute != null)
						{
							entity.RootEntity = createdAttribute;
							_currentEntity = createdAttribute;
						}
						else
						{
							//UPGRADE_TODO: The equivalent in .NET for method 'java.Object.toString' may return a different value. 'ms-help://MS.VSCC.2003/commoner/redir/redirect.htm?keyword="jlca1043"'
							throw new SQLBuilderException("Invalid step " + stepName + " found in " + this.PathEnumerator.ToString());
						}
					}
					
					break;
				
				case Axis.DESCENDANT: 
					if (!IsValidClassListName(stepName))
					{
						ClassEntity clsEntity = CreateClassEntity(_currentStep);
						entity.RootEntity = clsEntity;
						
						_currentEntity = clsEntity;
					}
					
					break;
				
				case Axis.ATTRIBUTE: 
					ownerClass = entity.OwnerClass;
					RelationshipEntity createdRelationship = ownerClass.CreateInheritedRelationship(stepName, false);
					
					if (createdRelationship != null)
					{
						entity.RootEntity = createdRelationship;
						_currentEntity = createdRelationship;
					}
					else
					{
						throw new SQLBuilderException("Attribute " + stepName + " doesn't exist in class " + ownerClass.Name);
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
				
				case Axis.CHILD: 
					if (IsValidClassName(stepName))
					{
						ClassEntity clsEntity = CreateClassEntity(_currentStep);
						
						entity.AddRootEntity(clsEntity);
						
						_currentEntity = clsEntity;
					}
					else if (IsValidClassListName(stepName))
					{
						_currentEntity = entity;
					}
					else
					{
						//UPGRADE_TODO: The equivalent in .NET for method 'java.Object.toString' may return a different value. 'ms-help://MS.VSCC.2003/commoner/redir/redirect.htm?keyword="jlca1043"'
						throw new SQLBuilderException("Invalid step " + stepName + " found in " + PathEnumerator.ToString());
					}
					
					break;
				
				case Axis.DESCENDANT: 
					if (!IsValidClassListName(stepName))
					{
						ClassEntity clsEntity = CreateClassEntity(_currentStep);
						entity.AddRootEntity(clsEntity);
						
						_currentEntity = clsEntity;
					}
					else
					{
						_currentEntity = entity;
					}
					
					break;
				
				case Axis.FUNCTION: 
					DBEntity funcEntity;
					
					/*
					* Step with function axis is to represent an aggregate function.
					* Create an AggregateFuncEntity for it. The function step can be the first
					* step of a path
					*/
					funcEntity = new AggregateFuncEntity(stepName, _dataProvider);
					entity.AddRootEntity(funcEntity);
					
					_currentEntity = funcEntity;
					
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
		
		/// <summary>
		/// Creates a ClassEntity object without attributes and relationships.
		/// </summary>
		/// <param name="pathStep">a step of path. </param>
		/// <returns> the ClassEntity object</returns>
		private ClassEntity CreateClassEntity(Step pathStep)
		{
			
			ClassEntity classEntity;
			ClassElement foundElement = (ClassElement) _metaData.SchemaModel.FindClass(pathStep.Name);
            Hashtable addedConditions = new Hashtable();

			if (foundElement != null)
			{
				/*
				* create a ClassEntity with empty inherited classes
				*/
				classEntity = new ClassEntity(foundElement);
				
				// check the read permission to the referenced class entity				
				Conclusion conclusion = PermissionChecker.Instance.GetConclusion(_metaData.XaclPolicy, classEntity.SchemaElement, XaclActionType.Read);
				
				if (conclusion.Permission == XaclPermissionType.ConditionalDeny ||
					conclusion.Permission == XaclPermissionType.ConditionalGrant)
				{
					/*
					* add the conditions as part of xquery to filter out unwanted instances.
					*/
                    AddCondition(pathStep, conclusion, addedConditions);
				}
				else if (conclusion.Permission == XaclPermissionType.Deny)
				{
					throw new PermissionViolationException("Do not have read permission to class " + classEntity.Name);
				}
				
				/*
				* Create attributes of local and inherited for the class
				*
				* TODO: create only those attributes that are actually used
				* (referenced) in a query. This strategy will help to generate
				* optimized SQL without getting unused data from database.
				*/
				classEntity.MakeFullBlown();
				
				/*
				* get the conditions defined in rules for the attributes and relationships in the
				* class and insert it into the path as an AlwaysTrue condition so that it has no effect
				* on selecting on instances but will cause the SQL to include the data to be used in
				* running the conditions by Xacl authorization later.
				*/
				string condition = GetAttributeConditions(classEntity, addedConditions);
				if (condition != null)
				{
                    // some condition may not have $this, do not add it as part of sql condition
                    if (ContainVariable(condition))
                    {
                        pathStep.InsertQualifier(RemoveVariable(condition), ConditionInsertOption.AlwaysTrue);
                    }
				}

                /*
                 * Set the condition that may be set by application as an extra condition to the query generated by the meta-data.
                 */
                if (!string.IsNullOrEmpty(_interpreter.ExtraCondition))
                {
                    pathStep.InsertQualifier(RemoveVariable(_interpreter.ExtraCondition), ConditionInsertOption.Including);
                }
			}
			else
			{
				throw new SQLBuilderException("Unknown Element Name " + pathStep.Name);
			}
			
			return classEntity;
		}
		
		/// <summary>
		/// If there are xacl conditions defined for any attributes of the class, the conditions
		/// are not inserted as qualifiers to the xquery, as class conditions do. Rather, we only
		/// need to get the xpaths representing the left-side of conditions, and call prepareNodes
		/// method on those xpaths.
		/// </summary>
		/// <param name="classEntity">the class entity in which the attributes to be processed.</param>
		private string GetAttributeConditions(ClassEntity classEntity, Hashtable addedConditions)
		{
			string combinedCondition = null;
			string condition;
			DBEntityCollection attributes = classEntity.InheritedAttributes;
			
			if (attributes != null)
			{
				foreach (AttributeEntity attribute in attributes)
				{
					/*
					* Get the condition in the matched xacl rules defined for the attribute.
					*/
					condition = PermissionChecker.Instance.GetCondition(_metaData.XaclPolicy, attribute.SchemaModelElement);
					
					if (condition != null && addedConditions[condition] == null)
					{
                        addedConditions[condition] = "1"; // remember it so it doesn't get added twice
						if (combinedCondition == null)
						{
							combinedCondition = condition;
						}
						else
						{
							combinedCondition += " and " + condition;
						}
					}
				}
			}
			
			attributes = classEntity.InheritedRelationships;
			if (attributes != null)
			{
				foreach (RelationshipEntity relationship in attributes)
				{					
					/*
					* Get the condition in the matched xacl rules defined for the attribute.
					*/
					condition = PermissionChecker.Instance.GetCondition(_metaData.XaclPolicy, relationship.SchemaModelElement);

                    if (condition != null && addedConditions[condition] == null)
					{
                        addedConditions[condition] = "1"; // remember it so it doesn't get added twice

						if (combinedCondition == null)
						{
							combinedCondition = condition;
						}
						else
						{
							combinedCondition += " AND " + condition;
						}
					}
				}
			}
			
			return combinedCondition;
		}
		
		/// <summary>
		/// Add the condition as additional qualifiers to the current step so that it gets
		/// added to the generated SQL as additional filters to the unwanted instances.
		/// </summary>
		/// <param name="step">the current step to which to add qualifiers.</param>
		/// <param name="conclusion">from which to get conditions.</param>
        /// <param name="addedConditions">The table that saves the added conditions.</param>
		/// <exception cref=""> is thrown when there is error in parsing the condition</exception>
		private void AddCondition(Step step, Conclusion conclusion, Hashtable addedConditions)
		{
			string condition;

			XaclPermissionType permission = conclusion.Permission;

			if (permission == XaclPermissionType.ConditionalDeny)
			{
				/*
				* Add deny conditions as excluding qualifiers, and also add grant conditions
                 * as AlwaysTrue condition in order to build a sql that get the info (including
                 * ones from related classes) for evaluating access control rules for editing, 
                 * creation, and deletion etc.
				*/
				foreach (Decision decision in conclusion.DecisionList)
				{
					if (decision.Permission == XaclPermissionType.Deny &&
						decision.Rule.HasCondition)
					{
						condition = decision.Rule.Condition.Condition;
                        // some condition may not have $this, do not add it as part of sql condition
                        if (ContainVariable(condition))
                        {
                            addedConditions[condition] = "1"; // remember it
                            step.InsertQualifier(RemoveVariable(condition), ConditionInsertOption.Excluding);
                        }
					}
                    else if (decision.Permission == XaclPermissionType.Grant &&
                        decision.Rule.HasCondition)
                    {
                        condition = decision.Rule.Condition.Condition;
                        // some condition may not have $this, do not add it as part of sql condition
                        // also make sure the same condition has not been added
                        if (ContainVariable(condition) && addedConditions[condition] == null)
                        {
                            addedConditions[condition] = "1";
                            step.InsertQualifier(RemoveVariable(condition), ConditionInsertOption.AlwaysTrue);
                        }
                    }
				}
			}
			else if (permission == XaclPermissionType.ConditionalGrant)
			{
				foreach (Decision decision in conclusion.DecisionList)
				{
					if (decision.Permission == XaclPermissionType.Grant &&
						decision.Rule.HasCondition)
					{
						condition = decision.Rule.Condition.Condition;
                        // some condition may not have $this, do not add it as part of sql condition
                        if (ContainVariable(condition))
                        {
                            addedConditions[condition] = "1"; // remember it
                            step.InsertQualifier(RemoveVariable(condition), ConditionInsertOption.Including);
                        }
					}
                    else if (decision.Permission == XaclPermissionType.Deny &&
                        decision.Rule.HasCondition)
                    {
                        condition = decision.Rule.Condition.Condition;
                        // some condition may not have $this, do not add it as part of sql condition
                        // also make sure the same condition has not been added
                        if (ContainVariable(condition) && addedConditions[condition] == null)
                        {
                            addedConditions[condition] = "1";
                            step.InsertQualifier(RemoveVariable(condition), ConditionInsertOption.AlwaysTrue);
                        }
                    }
				}
			}
		}

        /// <summary>
        /// Gets the information indicating whether a condition contains a variable $this
        /// </summary>
        /// <param name="condition">The condition expression</param>
        /// <returns>true if it contains, false otherwise</returns>
        private bool ContainVariable(string condition)
        {
            bool status = false;

            if (!string.IsNullOrEmpty(condition))
            {
                int pos = condition.IndexOf(VARIABLE_NAME);
                if (pos >= 0)
                {
                    status = true;
                }
            }

            return status;
        }
		
		/// <summary>
		/// Remove the variable ($this) from xpath strings appears in the condition.
		/// </summary>
		/// <param name="condition">the condition string. </param>
		/// <returns> the modified condition.</returns>
		private string RemoveVariable(string condition)
		{
			System.Text.StringBuilder buf = new System.Text.StringBuilder();
			int start = 0;
			int end = condition.IndexOf(VARIABLE_NAME, 0);
			while (end >= 0)
			{
				buf.Append(condition.Substring(start, (end) - (start)));
				start = end + VARIABLE_NAME.Length;
				end = condition.IndexOf(VARIABLE_NAME, start);
			}
			
			buf.Append(condition.Substring(start));
			
			return buf.ToString();
		}
	}
}