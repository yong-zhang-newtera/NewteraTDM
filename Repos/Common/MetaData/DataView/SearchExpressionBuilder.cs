/*
* @(#)SearchExpressionBuilder.cs
*
* Copyright (c) 2007 Newtera, Inc. All rights reserved.
*
*/
namespace Newtera.Common.MetaData.DataView
{
	using System;
	using System.Text;
	using System.Collections;
	using System.Xml;

    using Newtera.Common.Core;
    using Newtera.Common.MetaData.Schema;
	using Newtera.Common.MetaData.DataView;

	/// <summary>
	/// An utility class that builds various search expressions
	/// </summary>
	/// <version>1.0.1 04 Dec 2007</version>
	public class SearchExpressionBuilder
	{
		/// <summary>
		/// Initiating an instance of SearchExpressionBuilder class
		/// </summary>
		public SearchExpressionBuilder()
		{
		}

		/// <summary>
		/// Builds an search expression that uses a relationship bewteen two classes
		/// </summary>
        /// <returns>A Search expression</returns>
		public IDataViewElement BuildSearchExprForRelationship(DataViewModel referencedClassDataView,
            DataClass referencedClass, string objId)
		{
            IDataViewElement expr = null;

            if (referencedClass.ReferringRelationship.IsForeignKeyRequired)
            {
                DataSimpleAttribute left = new DataSimpleAttribute(NewteraNameSpace.OBJ_ID_ATTRIBUTE, referencedClassDataView.BaseClass.Alias);
                Parameter right = new Parameter(NewteraNameSpace.OBJ_ID_ATTRIBUTE, referencedClassDataView.BaseClass.Alias, DataType.String);

                right.ParameterValue = objId;
                expr = new RelationalExpr(ElementType.Equals, left, right);
            }
            else
            {
                // find the class from referenced classes that owns the referring relationship,
                // all referenced classes are stored in the data view model
                DataClass referringClass = null;
                foreach (DataClass refClass in referencedClassDataView.ReferencedClasses)
                {
                    if (referencedClass.ReferringRelationshipName == refClass.ReferringRelationship.BackwardRelationship.Name &&
                        referencedClass.ReferringRelationship.OwnerClass.Name == refClass.ReferringRelationship.BackwardRelationship.OwnerClass.Name)
                    {
                        // found the referring class
                        referringClass = refClass;
                        break;
                    }
                }

                if (referringClass != null)
                {
                    // it is a one-to-many relationship
                    DataSimpleAttribute left = new DataSimpleAttribute(NewteraNameSpace.OBJ_ID_ATTRIBUTE, referringClass.Alias);
                    Parameter right = new Parameter(NewteraNameSpace.OBJ_ID_ATTRIBUTE, referringClass.Alias, DataType.String);
                    right.ParameterValue = objId;
                    expr = new RelationalExpr(ElementType.Equals, left, right);
                }
                else
                {
                    throw new Exception("unable to find referring class for class " + referencedClass.ClassName + " with referring relationship name " + referencedClass.ReferringRelationshipName + " and owner class name " + referencedClass.ReferringRelationship.OwnerClass.Name);
                }
            }

            return expr;
		}
	}
}