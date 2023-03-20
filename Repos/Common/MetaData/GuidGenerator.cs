/*
* @(#)GuidGenerator.cs
*
* Copyright (c) 2013 Newtera, Inc. All rights reserved.
*
*/
namespace Newtera.Common.MetaData
{
	using System;
    using System.Threading;
	using System.Collections.Specialized;

    using Newtera.Common.MetaData.Schema;
    using Newtera.Common.Wrapper;

	/// <summary>
	/// Represents a generator that generates a GUID value for an auto-generated value atttribute
	/// </summary>
	/// <version> 1.0.0 01 Jun 2013 </version>
    public class GuidGenerator : IAttributeValueGenerator
	{
        public string GetValue(string id, IInstanceWrapper instance, MetaDataModel metaData)
        {
            return Guid.NewGuid().ToString("N");
        }
	}
}