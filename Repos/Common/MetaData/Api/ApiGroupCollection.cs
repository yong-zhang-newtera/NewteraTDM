/*
* @(#)ApiGroupCollection.cs
*
* Copyright (c) 2015 Newtera, Inc. All rights reserved.
*
*/
namespace Newtera.Common.MetaData.Api
{
	using System;
	using System.Text;
	using System.Xml;

	/// <summary>
	/// Represents a collection of ApiGroup instances.
	/// </summary>
	/// <version>1.0.0 16 Oct 2015</version>
	public class ApiGroupCollection : ApiNodeCollection
	{
		/// <summary>
		/// Initiating an instance of ApiGroupCollection class
		/// </summary>
		public ApiGroupCollection() : base()
		{
		}
		
		/// <summary>
		/// Initiating an instance of ApiGroupCollection class
		/// </summary>
		/// <param name="xmlElement">The xml element conatins data of the instance</param>
		internal ApiGroupCollection(XmlElement xmlElement) : base(xmlElement)
		{
		}

		/// <summary>
		/// Gets the type of node
		/// </summary>
        /// <value>One of ApiNodeType values</value>
		public override ApiNodeType NodeType
		{
			get
			{
                return ApiNodeType.ApiGroupCollection;
			}
		}

        /// <summary>
        /// Accept a visitor of IApiNodeVisitor type to traverse its
        /// elements.
        /// </summary>
        /// <param name="visitor">A visitor</param>
        public override void Accept(IApiNodeVisitor visitor)
        {
            if (visitor.VisitApiGroupCollection(this))
            {
                foreach (ApiGroup element in List)
                {
                    element.Accept(visitor);
                }
            }
        }
	}
}