/*
* @(#)AliasVisitor.cs
*
* Copyright (c) 2003 Newtera, Inc. All rights reserved.
*
*/
namespace Newtera.Server.Engine.Sqlbuilder
{
	using System;
	using System.Collections;
	using Newtera.Server.Engine.Sqlbuilder.Sql;

	/// <summary>
	/// A AliasVisitor object gives an alias to each table
	/// </summary>
	/// <version>  	1.0.1 08 Jul 2003</version>
	/// <author> Yong Zhang </author>
	public class AliasVisitor : EntityVisitor
	{
		// private instance members
		private Hashtable _usedAliases;
		private int _aliasNum;
		
		/// <summary>
		/// Initiating an AliasVistor object
		/// </summary>
		public AliasVisitor()
		{
			_usedAliases = new Hashtable();
			_aliasNum = 0;
		}
		
		/// <summary>
		/// Visit a ClassEntity object and add TABLE_ALIAS property to the object.
		/// </summary>
		/// <param name="entity">the class entity object to be visited</param>
		public bool VisitClass(ClassEntity entity)
		{
			string alias;
						
			// Create an alias for the class that has attributes or relationships
			if (!entity.HasInheritedAttributes() && !entity.HasInheritedRelationships())
			{
				return true;
			}
			
			// create an alias for this class
			while (true)
			{
				alias = "T" + _aliasNum; // Alias starts with letter T and follows by number
				_aliasNum++;
				if (_usedAliases.ContainsKey(alias))
				{
					// already used
					_aliasNum++;
				}
				else
				{
					_usedAliases[alias] = alias;
					break;
				}
			}
			
			entity.Alias = alias; // The ClassEntity object keeps the alias
			
			return true;
		}
		
		/// <summary>
		/// Visit an AttributeEntity object and add TABLE_ALIAS property to the object.
		/// </summary>
		/// <param name="entity">the attribute entity to be visited.</param>
		public bool VisitAttribute(AttributeEntity entity)
		{
			return true;
		}
		
		/// <summary>
		/// Visit a RelationshipEntity object.
		/// </summary>
		/// <param name="entity">the relationship entity to be visited</param>
		public bool VisitRelationship(RelationshipEntity entity)
		{
			return true;
		}
		
		/// <summary>
		/// Visit a DBEntity object representing an function
		/// such as count, avg, min, max, sum.
		/// </summary>
		/// <param name="entity">the function entity to be visited.</param>
		public bool VisitFunction(AggregateFuncEntity entity)
		{
			string alias;
			
			// Create an alias for the function that has a parent
			if (entity.OwnerClass == null)
			{
				return true;
			}
			
			// create an alias for this class
			while (true)
			{
				alias = "T" + _aliasNum; // Alias starts with letter T and follows by number
				_aliasNum++;
				if (_usedAliases.ContainsKey(alias))
				{
					// already used
					_aliasNum++;
				}
				else
				{
					_usedAliases[alias] = alias;
					break;
				}
			}
			
			entity.Alias = alias; // The ClassEntity object keeps the alias
			
			return true;
		}
		
		/// <summary>
		/// Visit a SchemaEntity object representing a query schema.
		/// </summary>
		/// <param name="entity">the schema entity to be visited.</param>
		public bool VisitSchema(SchemaEntity entity)
		{
			return true;
		}
		
		/// <summary>
		/// Visit a ScoreEntity object.
		/// </summary>
		/// <param name="entity">the score entity to be visited </param>
		public bool VisitScore(ScoreEntity entity)
		{
			return true;
		}
	}
}