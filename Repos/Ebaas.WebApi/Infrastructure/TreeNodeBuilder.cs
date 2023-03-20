/*
* @(#) TreeNodeBuilder.cs
*
* Copyright (c) 2016 Newtera, Inc. All rights reserved.
*
*/

namespace Ebaas.WebApi.Infrastructure
{
	using System;
	using System.Xml;
	using System.IO;
	using System.Text;
    using System.Web;
    using Newtonsoft.Json.Linq;

    using Newtera.Common.Core;
    using Newtera.Common.MetaData;
	using Newtera.Common.MetaData.Schema;
    using Newtera.Common.MetaData.DataView;
	using Newtera.Common.MetaData.XaclModel;
	using Newtera.Common.MetaData.DataView.Taxonomy;

	/// <summary> 
	/// A utility class that builds tree nodes in xml format for TreeView control
	/// from IE Web Control libary
	/// </summary>
	public class TreeNodeBuilder
	{
		internal static int _nodeId;

		/// <summary>
		/// This class provides utilities through static methods. You dont need
		/// to create an instance of this class.
		/// </summary>
		private TreeNodeBuilder()
		{
		}

        /// <summary>
        /// Get a JSON tree representing tree structure of class hierarchy
        /// for a given MetaDataModel
        /// </summary>
        /// <param name="metaData">The MetaDataModel that provides class hierarchy info</param>
        /// <returns>A JSON array. each node in the jarray is a tree root</returns>
        public static JArray GetClassTree(MetaDataModel metaData)
        {
            return GetClassTree(metaData, null);
        }

        /// <summary>
        /// Get a JSON tree representing tree structure of class hierarchy
        /// for a given MetaDataModel and, optionally, a given root class.
        /// </summary>
        /// <param name="metaData">The MetaDataModel that provides class hierarchy info</param>
        /// <param name="rootClassName">The name of a root class of the generated tree. If null, the generated tree will includes all classes in the MetaDataModel.</param>
        /// <returns>A JSON array, each node in the jarray is a tree root</returns>
        public static JArray GetClassTree(MetaDataModel metaData, string rootClassName)
		{
            JArray treeRoots = new JArray();

            TreeNodeBuilder._nodeId = 0;

            if (metaData != null)
            {
                string rootName = metaData.SchemaInfo.Name;
                string rootTitle = metaData.SchemaInfo.Name;

                SchemaModelElementCollection childClasses;

                // find the first level of child classes
                if (rootClassName == null)
                {
                    childClasses = metaData.SchemaModel.RootClasses;
                }
                else
                {
                    ClassElement rootClass = metaData.SchemaModel.FindClass(rootClassName);
                    rootName = rootClass.Name;
                    rootTitle = rootClass.Caption;

                    if (rootClass != null)
                    {
                        childClasses = rootClass.Subclasses;
                    }
                    else
                    {
                        // root class name is incorrect, get all root classes from schema
                        childClasses = metaData.SchemaModel.RootClasses;
                    }
                }

                JObject treeRoot = new JObject();

                treeRoot.Add("name", rootName);
                treeRoot.Add("title", rootTitle);
                treeRoot.Add("collapsed", false);
                treeRoot.Add("children", new JArray());

                treeRoots.Add(treeRoot);

                AppendClassChildNodes(metaData, treeRoot, childClasses);
            }

			return treeRoots;
		}

        /// <summary>
        /// Gets a JSON tree representing a given taxonomy of a schema
        /// </summary>
        /// <param name="metaData">The meta data model</param>
        /// <param name="taxonomy">A Taxonomy model</param>
        /// <returns>A Tree of json object with a tree root</returns>
        public static JObject GetTaxonomyTree(MetaDataModel metaData, TaxonomyModel taxonomy)
        {
            var treeRoot = new JObject();

            if (taxonomy != null)
            {
                treeRoot.Add("name", taxonomy.Name);
                treeRoot.Add("title", taxonomy.Caption);
                treeRoot.Add("class", taxonomy.ClassName);
                treeRoot.Add("children", new JArray());

                AddTaxonomyTreeChildren(metaData, treeRoot, taxonomy.ChildrenNodes);
            }

            return treeRoot;
        }

        /// <summary>
        /// Get a JSON tree representing relationship tree starting from a base class.
        /// </summary>
        /// <param name="metaData">The MetaDataModel that provides class hierarchy info</param>
        /// <param name="baseClassName">The name of a base class as root of the tree.</param>
        /// <param name="depth">The depth of relationships that tree builder travels</param>
        /// <returns>A JObject tree with a single root</returns>
        public static JObject GetRelationshipTree(MetaDataModel metaData, string baseClassName, int depth)
        {
            JObject treeRoot = new JObject();

            TreeNodeBuilder._nodeId = 0;

            int currentDepth = 0;

            DataViewModel dataView = metaData.GetDefaultDataView(baseClassName);
            if (dataView != null)
            {
                treeRoot.Add("name", baseClassName);
                treeRoot.Add("title", dataView.BaseClass.Caption);
                treeRoot.Add("collapsed", true);
                treeRoot.Add("children", new JArray());

                AddRelatedClassNodes(treeRoot, dataView, null, metaData, depth, currentDepth);
            }
            else
            {
                throw new Exception("Unable to find a dataview in the meta data for class " + baseClassName);
            }

            return treeRoot;
        }

        private static void AddRelatedClassNodes(JObject currentNode, DataViewModel currentDataView, DataViewModel parentDataView, MetaDataModel metaData, int depth, int currentDepth)
        {
            JObject childNode;

            foreach (DataClass relatedClass in currentDataView.BaseClass.RelatedClasses)
            {
                if (parentDataView != null &&
                    parentDataView.BaseClass.Name == relatedClass.Name)
                {
                    continue; // prevent circular loop
                }

                childNode = new JObject();
                childNode.Add("name", relatedClass.ClassName);
                childNode.Add("title", relatedClass.Caption);
                childNode.Add("collapsed", true);
                childNode.Add("children", new JArray());

                if (relatedClass.IsLeafClass)
                {
                    childNode.Add("leaf", true);
                }
                else
                {
                    childNode.Add("leaf", false);
                }

                ((JArray)currentNode["children"]).Add(childNode);

                if ((currentDepth + 1) < depth)
                {
                    DataViewModel childDataView = metaData.GetDefaultDataView(relatedClass.Name);
                    AddRelatedClassNodes(childNode, childDataView, currentDataView, metaData, depth, currentDepth + 1);
                }
            }
        }

        private static void AddTaxonomyTreeChildren(MetaDataModel metaData, JObject parentNode, TaxonNodeCollection children)
        {
            JObject childNode;

            foreach (TaxonNode node in children)
            {
                // make sure that only show the node that current principal
                // has permission to see, and the class is browsable
                if (PermissionChecker.Instance.HasPermission(metaData.XaclPolicy, node, XaclActionType.Read))
                {
                    childNode = new JObject();
                    childNode.Add("name", node.Name);
                    childNode.Add("title", node.Caption);
                    childNode.Add("icon", node.SmallImage);
                    childNode.Add("class", node.ClassName);
                    childNode.Add("children", new JArray());

                    ((JArray)parentNode["children"]).Add(childNode);

                    // call AddTreeChildren recursively
                    if (node.ChildrenNodes != null &&
                        node.ChildrenNodes.Count > 0)
                    {
                        AddTaxonomyTreeChildren(metaData, childNode, node.ChildrenNodes);
                    }
                }
            }
        }

		/// <summary>
		/// Append the child classes as child tree node to the parent
		/// node.
		/// </summary>
		/// <param name="metaData">The MetaDataModel</param>
		/// <param name="parentNode">The parent json node</param>
		/// <param name="childClasses">The collection of child classes</param>
		private static void AppendClassChildNodes(MetaDataModel metaData, JObject parentNode,
			SchemaModelElementCollection childClasses)
		{
			if (childClasses == null)
			{
				return;
			}

            JObject childNode;
			foreach (ClassElement childClass in childClasses)
			{
				// make sure that only show the classes that current principal
				// has permission to see, and the class is browsable
				if (PermissionChecker.Instance.HasPermission(metaData.XaclPolicy, childClass, XaclActionType.Read) &&
                    childClass.IsBrowsable)
				{
                    // if the class belongs to a category, add the class tree node under
                    // a node represents the category
                    if (!string.IsNullOrEmpty(childClass.Category))
                    {
                        // Get or create a category node, and add the class node under the category node
                        JObject categoryNode = GetCategoryNode(parentNode, childClass);
                        childNode = CreateClassTreeNode(categoryNode, childClass);

                        ((JArray)categoryNode["children"]).Add(childNode);
                    }
                    else
                    {
                        // class with no category
                        childNode = CreateClassTreeNode(parentNode, childClass);
                        ((JArray)parentNode["children"]).Add(childNode);
                    }

					// append the child classes reursively
					AppendClassChildNodes(metaData, childNode, childClass.Subclasses);
				}
			}
		}

        /// <summary>
        /// Create a tree node representing a class
        /// </summary>
        /// <param name="parentNode">The parent tree node</param>
        /// <param name="childClass">The meta data class element</param>
        /// <returns>A JObject as tree node.</returns>
        private static JObject CreateClassTreeNode(JObject parentNode, ClassElement childClass)
        {
            JObject childNode = new JObject();

            childNode.Add("id", "node" + TreeNodeBuilder._nodeId++);
            childNode.Add("name", childClass.Name);
            childNode.Add("title", childClass.Caption);
            childNode.Add("collapsed", true);
            childNode.Add("children", new JArray());

            return childNode;
        }

        /// <summary>
        /// Get a category node under the parent node, if the category node deosn't exist,
        /// create a category node, add it to the parent node, then return it.
        /// </summary>
        /// <param name="parentNode">The parent node</param>
        /// <param name="classElement">The class element</param>
        /// <returns>A JObject tree node</returns>
        private static JObject GetCategoryNode(JObject parentNode, ClassElement classElement)
        {
            JObject categoryNode = null;
 
            JArray childNodes = parentNode["children"].Value<JArray>();
            foreach (JObject childNode in childNodes)
            {
                string type = null;
                if (childNode["type"] != null)
                {
                    type = childNode["type"].Value<string>();
                }
                if (!string.IsNullOrEmpty(type) &&
                    type == "Folder" &&
                    childNode["title"].Value<string>() == classElement.Category)
                {
                    categoryNode = childNode;
                    break;
                }
            }

            if (categoryNode == null)
            {
                categoryNode = CreateCategoryTreeNode(classElement.Category);
                ((JArray)parentNode["children"]).Add(categoryNode);
            }

            return categoryNode;
        }

        /// <summary>
        /// Create a tree node representing a category
        /// </summary>
        /// <param name="categoryName">The name of category</param>
        /// <returns>A XmlElement as tree node.</returns>
        private static JObject CreateCategoryTreeNode(string categoryName)
        {
            JObject childNode = new JObject();

            childNode.Add("id", "node" + TreeNodeBuilder._nodeId++);
            childNode.Add("name", categoryName);
            childNode.Add("type", "Folder");
            childNode.Add("title", categoryName);
            childNode.Add("collapsed", true);
            childNode.Add("children", new JArray());

            return childNode;
        }
	}
}
