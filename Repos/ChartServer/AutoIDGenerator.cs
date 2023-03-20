/*
* @(#)AutoIDGenerator.cs
*
* Copyright (c) 2007 Newtera, Inc. All rights reserved.
*
*/
namespace Newtera.ChartServer
{
	using System;
	using System.Xml;

	using Newtera.Common.MetaData;
    using Newtera.Common.MetaData.Schema;
    using Newtera.Common.Wrapper;

	/// <summary> 
	/// The class that generats an unique project id.
	/// </summary>
    public class AutoIDGenerator : IAttributeValueGenerator
    {
        /// <summary>
        /// Initiate an instance of ProjectIDGenerator class
        /// </summary>
        public AutoIDGenerator()
        {
        }

        #region IAttributeValueGenerator interface implementation

        /// <summary>
        /// Generate an unique project id
        /// </summary>
        /// <param name="id">An unique id provided by the system that may be used as part of the generated value.</param>
        /// <param name="instance">The data instance to be inserted</param>
        /// <param name="metaData">The meta-data of the database</param>
        /// <returns>The generated unique project id</returns>
        public string GetValue(string id, IInstanceWrapper instance, MetaDataModel metaData)
        {
            string value = instance.GetString("ÏîÄ¿Ãû³Æ") + "_" + id;

            return value;
        }

        #endregion
    }
}