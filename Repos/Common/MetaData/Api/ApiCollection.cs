/*
* @(#)ApiCollection.cs
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
	/// Represents a collection of Api instances.
	/// </summary>
	/// <version>1.0.0 16 Sep 2013</version>
	public class ApiCollection : ApiNodeCollection
	{
		/// <summary>
		/// Initiating an instance of ApiCollection class
		/// </summary>
		public ApiCollection() : base()
		{
		}
		
		/// <summary>
		/// Initiating an instance of ApiCollection class
		/// </summary>
		/// <param name="xmlElement">The xml element conatins data of the instance</param>
		internal ApiCollection(XmlElement xmlElement) : base(xmlElement)
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
				return ApiNodeType.ApiCollection;
			}
		}

        /// <summary>
        /// Accept a visitor of IApiNodeVisitor type to traverse its
        /// elements.
        /// </summary>
        /// <param name="visitor">A visitor</param>
        public override void Accept(IApiNodeVisitor visitor)
        {
            if (visitor.VisitApiCollection(this))
            {
                foreach (Api element in List)
                {
                    element.Accept(visitor);
                }
            }
        }
	}
}