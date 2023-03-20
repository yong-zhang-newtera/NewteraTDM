using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Linq;
using System.Xml;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System.Web;
using System.Data;
using System.Collections;
using System.Text;
using System.IO;
using System.Runtime.Remoting;
using System.Text.RegularExpressions;

using HtmlAgilityPack;

using Ebaas.WebApi.Models;
using Newtera.Common.Core;
using Newtera.Common.MetaData;
using Newtera.Common.MetaData.Schema;
using Newtera.Data;
using Newtera.Common.MetaData.XaclModel;
using Newtera.Server.Engine.Cache;
using Newtera.ElasticSearchIndexer;

namespace Ebaas.WebApi.Infrastructure
{
    /// <summary>
    /// Query helper for FullTextSearch api
    /// </summary>
    public class FullTextSearchHelper
    {
        private const string CONNECTION_STRING = @"SCHEMA_NAME={schemaName};SCHEMA_VERSION=1.0";

        public static JObject BuildQueryBody(string searchText, int startRow, int pageSize)
        {
            JObject queryBody;

            if (startRow >= 0)
            {
                queryBody = JObject.Parse(@"{
                   'from' :" + startRow + @", 'size' :" + pageSize + @",
                   'query': {
                        'match': {
                            'catch_all': {
                                'query': '" + searchText + @"',
                                'operator': 'and'
                              }
                          }
                     }
                }");
            }
            else
            {
                queryBody = JObject.Parse(@"{
                   'query': {
                        'match': {
                            'catch_all': {
                                'query': '" + searchText + @"',
                                'operator': 'and'
                              }
                          }
                     }
                }");
            }

            return queryBody;
        }

        public static JObject BuildGetSuggestionsBody(string prefix, int size)
        {
            JObject queryBody;

            queryBody = JObject.Parse(@"{
                'size': 0,
                'suggest': {
                    'my-suggest': {
                        'prefix': '" + prefix + @"',
                        'completion': {
                            'field': 'suggest',
                            'size' : " + size + @",
                            'skip_duplicates': true,
                            'fuzzy': {
                                'fuzziness': 'AUTO'
                            }
                        }
                    }
                 }
            }");

            return queryBody;
        }

        public static StringCollection GetInstanceIdsFromSearchEngine(string schemaName, string className, string searchText,
            int startRow, int pageSize)
        {
            StringCollection instanceIds = new StringCollection();

            JObject queryBody = FullTextSearchHelper.BuildQueryBody(searchText, startRow, pageSize);

            JObject result = ElasticSearchWrapper.GetSearchResult(schemaName, className, queryBody);
            if (result["hits"]["hits"] != null)
            {
                foreach (JObject hit in result["hits"]["hits"])
                {
                    string instanceId = hit["_id"].ToObject<string>();
                    instanceIds.Add(instanceId);
                }
            }
            return instanceIds;
        }

        public static StringCollection GetCompletionSuggestionsSearchEngine(string schemaName, string className, JObject queryBody)
        {
            StringCollection suggestions = new StringCollection();

            JObject result = ElasticSearchWrapper.GetSuggestions(schemaName, className, queryBody);

            //var jsonString = JsonConvert.SerializeObject(result, Newtonsoft.Json.Formatting.Indented);
            //ErrorLog.Instance.WriteLine("Class = " + className + " with result:" + jsonString);

            if (result["suggest"] != null)
            {
                if (result["suggest"]["my-suggest"] != null)
                {
                    foreach (JObject mySuggest in result["suggest"]["my-suggest"])
                    {
                        if (mySuggest["options"] != null)
                        {
                            foreach (JObject suggest in mySuggest["options"])
                            {
                                string suggestText = suggest["text"].ToObject<string>();
                                suggestions.Add(suggestText);
                            }
                        }
                    }
                }
            }
            return suggestions;
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
            // get the bottom classes and check each of them if it has an attribute with "IsGoodForFullTextSearch" enabled
            SchemaModelElementCollection bottomClasses;

            if (classElement.IsLeaf)
            {
                bottomClasses = new SchemaModelElementCollection();
                bottomClasses.Add(classElement);
            }
            else
            {
                // get the bottom classes and check each of them if it has an attribute with "IsGoodForFullTextSearch" enabled
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
                            if (simpleAttribute.IsGoodForFullTextSearch)
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