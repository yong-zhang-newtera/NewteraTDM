/*
* @(#)RelationshipResultElement.cs
*
* Copyright (c) 2003 Newtera, Inc. All rights reserved.
*
*/
namespace Newtera.Common.MetaData.DataView.QueryBuilder
{
	using System;
	using System.Text;
	using System.Collections;

	using Newtera.Common.Core;
    using Newtera.Common.MetaData.DataView;

	/// <summary>
	/// Represents a root xml element in return clause of a XQuery.
	/// </summary>
	/// <version>  	1.0.0 04 Nov 2003</version>
	/// <author>  Yong Zhang </author>
	internal class RelationshipResultElement : QueryElementBase
	{
		private bool _isForeignKeyRequired;
		private DataRelationshipAttribute _relationshipAttribute;
		private bool _includePrimaryKeys;
        private string _baseClassName;

		/// <summary>
		/// Initiating an instance of RelationshipResultElement class
		/// </summary>
		/// <param name="relationshipAttribute">The base class of a data view</param>
		/// <param name="includePrimaryKeys">Value to indicate whether to include
		/// primary key values as part of search result for relationship attributes</param> 
		public RelationshipResultElement(DataRelationshipAttribute relationshipAttribute, bool includePrimaryKeys,
            string baseClassName) : base()
		{
			_isForeignKeyRequired = false;
			_relationshipAttribute = relationshipAttribute;
			_includePrimaryKeys = includePrimaryKeys;
            _baseClassName = baseClassName;
		}

		/// <summary>
		/// Gets or sets the information indicating if a foreign key column is
		/// created for the relationship. This is used to determine the xquery syntax
		/// generated for the relationship attribute. The value needs to be set by
		/// the result visitor.
		/// </summary>
		/// <value>True if a foreign key is required, false otherwise</value>
		public bool IsForeignKeyRequired
		{
			get
			{
				return _isForeignKeyRequired;
			}
			set
			{
				_isForeignKeyRequired = value;
			}
		}

		/// <summary>
		/// Gets the XQuery representation of the element.
		/// </summary>
		/// <returns>A XQuery segmentation</returns>
		public override string ToXQuery()
		{
			StringBuilder query = new StringBuilder();

			if (_relationshipAttribute.HasFunction)
			{
				// treated as a single result field
				query.Append("<").Append(_relationshipAttribute.Caption).Append(">");

				query.Append("{").Append(((IQueryElement) _relationshipAttribute).ToXQuery()).Append("}");

				query.Append("</").Append(_relationshipAttribute.Caption).Append(">");
			}
			else if (_includePrimaryKeys)
			{
				AddPrimaryKeyElements(query); // slower query
			}
			else
			{
				AddObjIdElement(query); // faster query
			}

			query.Append("\n");

			return query.ToString();
		}

		/// <summary>
		/// Add objId element as child element of a relationship element to avoid
		/// joining with linked classes
		/// </summary>
		/// <param name="query">query</param>
		private void AddObjIdElement(StringBuilder query)
		{
			// make the relationship as a new element
            query.Append("<").Append(DataRelationshipAttribute.GetRelationshipDataTableName(_baseClassName, _relationshipAttribute.Name)).Append(">");

			query.Append("<").Append(NewteraNameSpace.OBJ_ID).Append(">{text($").Append(_relationshipAttribute.OwnerClassAlias);

			if (_isForeignKeyRequired)
			{
				query.Append("/@").Append(_relationshipAttribute.Name); // foreign key's value as obj_id
			}
			else
			{
				query.Append("/").Append(NewteraNameSpace.OBJ_ID_ATTRIBUTE); // this instance's obj_id
			}

			query.Append(")}</").Append(NewteraNameSpace.OBJ_ID).Append(">");

            query.Append("</").Append(DataRelationshipAttribute.GetRelationshipDataTableName(_baseClassName, _relationshipAttribute.Name)).Append(">");
		}

		/// <summary>
		/// Add primary element(s) as child element of a relationship element. This is
		/// required when getting an instance for editing purpose.
		/// </summary>
		/// <param name="query">query</param>
		private void AddPrimaryKeyElements(StringBuilder query)
		{
			// only apply to the relationships that have foreign key(s)
			if (_isForeignKeyRequired)
			{
				// use the name of base class and relationship as name of a element
				query.Append("<").Append(DataRelationshipAttribute.GetRelationshipDataTableName(_baseClassName, _relationshipAttribute.Name)).Append(">");

				query.Append("<").Append(NewteraNameSpace.OBJ_ID).Append(">{text($").Append(_relationshipAttribute.OwnerClassAlias);

				if (_isForeignKeyRequired)
				{
					query.Append("/@").Append(_relationshipAttribute.Name); // foreign key's value as obj_id
				}
				else
				{
					query.Append("/").Append(NewteraNameSpace.OBJ_ID_ATTRIBUTE); // this instance's obj_id
				}

				query.Append(")}</").Append(NewteraNameSpace.OBJ_ID).Append(">");

				DataViewElementCollection primaryKeys = _relationshipAttribute.PrimaryKeys;
				if (primaryKeys != null)
				{
					foreach (DataSimpleAttribute pk in primaryKeys)
					{
						query.Append("{$").Append(pk.OwnerClassAlias).Append("/").Append(pk.Name).Append("}");
					}
				}

                query.Append("</").Append(DataRelationshipAttribute.GetRelationshipDataTableName(_baseClassName, _relationshipAttribute.Name)).Append(">");
			}
		}
	}
}