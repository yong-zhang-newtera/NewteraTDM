/*
* @(#)PrintVisitor.cs
*
* Copyright (c) 2003 Newtera, Inc. All rights reserved.
*
*/
namespace Newtera.Server.Engine.Sqlbuilder
{
	using System;
	using Newtera.Server.Engine.Sqlbuilder.Sql;

	/// <summary>
	/// A PrintVisitor object print out the entity structure information to logger
	/// for debugging purpose.
	/// </summary>
	/// <version>  	1.0.0 11 Jul 2003 </version>
	/// <author>  Yong Zhang  </author>
	class PrintVisitor : EntityVisitor
	{
		/// <summary> Constructor for a PrintVisitor object
		/// </summary>
		public PrintVisitor()
		{
		}
		
		/// <summary>
		/// Visit a ClassEntity object.
		/// </summary>
		/// <param name="attribute">the attribute object to be visited.</param>
		public bool VisitClass(ClassEntity entity)
		{			
			Console.WriteLine(entity.ToString());
			
			return true;
		}
		
		/// <summary>
		/// Visit an AttributeEntity object.
		/// </summary>
		/// <param name="entity">the attribute entity to be visited.</param>
		public bool VisitAttribute(AttributeEntity entity)
		{
			Console.WriteLine(entity.ToString());
			
			return true;
		}
		
		/// <summary>
		/// Visit a RelationshipEntity object.
		/// </summary>
		/// <param name="entity">the relationship entity to be visited.</param>
		public bool VisitRelationship(RelationshipEntity entity)
		{
			Console.WriteLine(entity.ToString());
			
			return true;
		}
		
		/// <summary>
		/// Visit a DBEntity object representing an function such as count, avg, min, max, sum.
		/// </summary>
		/// <param name="entity">the function entity to be visited.</param>
		public bool VisitFunction(AggregateFuncEntity entity)
		{
			Console.WriteLine(entity.ToString());
			
			return true;
		}
		
		/// <summary>
		/// Visit a SchemaEntity object representing a query schema.
		/// </summary>
		/// <param name="entity">the schema entity to be visited.</param>
		public bool VisitSchema(SchemaEntity entity)
		{
			Console.WriteLine(entity.ToString());

			return true;
		}
		
		/// <summary>
		/// Visit a ScoreEntity object.
		/// </summary>
		/// <param name="entity">the score entity to be visited.</param>
		public bool VisitScore(ScoreEntity entity)
		{
			Console.WriteLine(entity.ToString());
			
			return true;
		}
	}
}