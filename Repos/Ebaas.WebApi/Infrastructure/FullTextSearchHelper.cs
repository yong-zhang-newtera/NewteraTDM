using System.Collections.Specialized;
using Newtonsoft.Json.Linq;
using System.Collections;
using System.Collections.Generic;

using Newtera.Common.MetaData;
using Newtera.Common.MetaData.Schema;
using Newtera.Data;
using Newtera.Common.MetaData.XaclModel;
using Newtera.Server.Engine.Cache;

namespace Ebaas.WebApi.Infrastructure
{
    /// <summary>
    /// Query helper for FullTextSearch api
    /// </summary>
    public class FullTextSearchHelper
    {
        private const string CONNECTION_STRING = @"SCHEMA_NAME={schemaName};SCHEMA_VERSION=1.0";

        public static StringCollection GetInstanceIdsFromQueryResult(IReadOnlyCollection<JObject> results)
        {
            StringCollection instanceIds = new StringCollection();

            foreach (JObject result in results)
            {
                if (result.ContainsKey("obj_id"))
                {
                    instanceIds.Add(result.GetValue("obj_id")?.ToString());
                }
            }
            return instanceIds;
        }

        public static SchemaModelElementCollection GetAccessibleClasses(string schemaName)
        {
            QueryHelper queryHelper = new QueryHelper();
            SchemaModelElementCollection classElements = new SchemaModelElementCollection();

            using (CMConnection con = new CMConnection(queryHelper.GetConnectionString(CONNECTION_STRING, schemaName)))
            {
                con.Open();

                MetaDataModel metaData = con.MetaDataModel;
                // get accessible classes for the user from sitemap
                Hashtable accessibleClasses = GetUserAccessibleClasses(schemaName);
                foreach (ClassElement root in metaData.SchemaModel.RootClasses)
                {
                    if (PermissionChecker.Instance.HasPermission(metaData.XaclPolicy, root, XaclActionType.Read))
                    {
                        // add full-text search enabled bottom classes
                        AddFullTextSearchEnabledBottomClasses(metaData, classElements, root, accessibleClasses);
                    }
                }
            }

            return classElements;
        }

        private static Hashtable GetUserAccessibleClasses(string schemaName)
        {
            Hashtable tbl = null;

            XaclPolicy policy = SiteMapManager.Instance.Policy;
            Newtera.Common.MetaData.SiteMap.SiteMap siteMap = SiteMapManager.Instance.SiteMap;
            if (siteMap != null)
            {
                GetAccessibleClassesVisitor visitor = new GetAccessibleClassesVisitor(policy, schemaName);

                siteMap.Accept(visitor);

                tbl = visitor.AccessibleClasses;
            }

            return tbl;
        }

        private static void AddFullTextSearchEnabledBottomClasses(MetaDataModel metaData, SchemaModelElementCollection classElements,
            ClassElement classElement, Hashtable accessibleClasses)
        {
            // get the bottom classes and check each of them if it has an attribute with "IsFullTextSearchAttribute" enabled
            SchemaModelElementCollection bottomClasses;

            if (classElement.IsLeaf)
            {
                bottomClasses = new SchemaModelElementCollection();
                bottomClasses.Add(classElement);
            }
            else
            {
                // get the bottom classes and check each of them if it has an attribute with "IsFullTextSearchAttribute" enabled
                bottomClasses = metaData.GetBottomClasses(classElement.Name);
            }

            bool hasFullTextSearchableAttribute;
            foreach (ClassElement bottomClass in bottomClasses)
            {
                // check if the bottom class is accessible to the user
                if (accessibleClasses[bottomClass.Name] != null)
                {
                    hasFullTextSearchableAttribute = false;
                    ClassElement currentClass = bottomClass;
                    while (currentClass != null)
                    {
                        foreach (SimpleAttributeElement simpleAttribute in currentClass.SimpleAttributes)
                        {
                            if (simpleAttribute.IsFullTextSearchAttribute)
                            {
                                hasFullTextSearchableAttribute = true;
                                break;
                            }
                        }

                        if (hasFullTextSearchableAttribute)
                        {
                            break;
                        }

                        currentClass = currentClass.ParentClass;
                    }

                    if (hasFullTextSearchableAttribute)
                    {
                        classElements.Add(bottomClass);
                    }
                }
            }
        }
    }
}