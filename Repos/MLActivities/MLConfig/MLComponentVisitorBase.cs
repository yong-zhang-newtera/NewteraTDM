/*
* @(#)ISchemaModelElementVisitor.cs
*
* Copyright (c) 2017 Newtera, Inc. All rights reserved.
*
*/
namespace Newtera.MLActivities.MLConfig
{
	using System;
    using System.Runtime.Serialization.Formatters.Binary;
    using System.IO;

    /// <summary>
    /// Base class for all IMLComponentVisitor implementations which provide common utility
    /// </summary>
    public class MLComponnetVisitorBase
	{
        public MLComponnetVisitorBase()
        {

        }

        public static T DeepClone<T>(T obj)
        {
            using (var ms = new MemoryStream())
            {
                var formatter = new BinaryFormatter();
                formatter.Serialize(ms, obj);
                ms.Position = 0;

                return (T)formatter.Deserialize(ms);
            }
        }
    }
}