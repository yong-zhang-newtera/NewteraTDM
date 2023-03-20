/*
* @(#)EntityVisitor.cs
*
* Copyright (c) 2003 Newtera, Inc. All rights reserved.
*
*/
namespace Newtera.Server.Engine.Sqlbuilder
{
	using System;
	using Newtera.Server.Engine.Sqlbuilder.Sql;

	/// <summary>
	/// EntityVisitor class declares an interface for each concrete visitor class that
	/// implements a special algorithm to process the structur of objects of Entity type.
	/// </summary>
	/// <version>  	1.0.1 08 Jul 2003 </version>
	/// <author> Yong Zhang </author> 
	public interface EntityVisitor
	{
		/// <summary>
		/// Visit a ClassEntity object.
		/// </summary>
		/// <param name="entity">the class entity to be visited.</param>
		/// <returns> true to continue traverse, false to stop</returns>
		bool VisitClass(ClassEntity entity);

		/// <summary>
		/// Visit an AttributeEntity object.
		/// </summary>
		/// <param name="entity">the attribute entity to be visited.</param>
		/// <returns> true to continue traverse</returns>
		bool VisitAttribute(AttributeEntity entity);

		/// <summary>
		/// Visit a RelationshipEntity object.
		/// </summary>
		/// <param name="entity">the relationship entity to be visited.</param>
		/// <returns> true to continue traverse</returns>
		bool VisitRelationship(RelationshipEntity entity);

		/// <summary>
		/// Visit an AggregateFuncEntity object representing an function such as count,
		/// avg, min, max, sum.
		/// </summary>
		/// <param name="entity">the function entity to be visited.</param>
		/// <returns> true to continue traverse</returns>
		bool VisitFunction(AggregateFuncEntity entity);

		/// <summary>
		/// Visit a SchemaEntity object representing a query schema.
		/// </summary>
		/// <param name="entity">the schema entity to be visited.</param>
		/// <returns> true to continue traverse</returns>
		bool VisitSchema(SchemaEntity entity);

		/// <summary>
		/// Visit a ScoreEntity object representing a score for a full-text search.
		/// </summary>
		/// <param name="entity">the function entity to be visited.</param>
		/// <returns> true to continue traverse</returns>
		bool VisitScore(ScoreEntity entity);
	}
}