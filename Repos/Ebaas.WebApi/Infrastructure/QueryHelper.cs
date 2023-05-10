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
using Newtera.Common.MetaData.DataView;
using Newtera.Common.MetaData.Api;
using Newtera.Data;
using Newtera.WebForm;
using Newtera.Server.Engine.Workflow;
using Newtera.Common.MetaData.XaclModel;

namespace Ebaas.WebApi.Infrastructure
{
    /// <summary>
    /// Query helper for DataController api
    /// </summary>
    public class QueryHelper
    {
        private const string CONNECTION_STRING = @"SCHEMA_NAME={schemaName};SCHEMA_VERSION=1.0";
        private string _leftOperandPropertyName = null;

        public void SetFilters(DataViewModel dataView, string filters)
        {
            SetFilters(dataView, filters, false);
        }

        /// <summary>
        /// Set the filter to the dataview
        /// </summary>
        /// <param name="dataView"></param>
        /// <param name="filters"></param>
        /// <param name="overrideFilter">true to oveeride the existing filter in the dataview, false otherwise.</param>
        public void SetFilters(DataViewModel dataView, string filters, bool overrideFilter)
        {

            var filterObject = JsonConvert.DeserializeObject(filters);

            JArray filterTree = JArray.FromObject(filterObject);

            // filter example [["Category","contains","bug"],"and",[["Status","=","Closed"],"or",["Status","=","Fixed"]]]
            // ["type", "in", "('L1', 'L2', 'L3')"]
            // [["Category", "=", "Bug"]]
            // convert to a tree
            CriteriaOperator searchCriteriaTree = BuildSearchCriteriaTree(filterTree);

            IDataViewElement filterExpression = ConvertOperator(searchCriteriaTree, dataView);

            if (filterExpression != null)
            {
                /*
                FlattenSearchFiltersVisitor visitor = new FlattenSearchFiltersVisitor();

                filterExpression.Accept(visitor);

                DataViewElementCollection  flattenExprs = visitor.FlattenedSearchFilters;

                string filterString = "";
                foreach (IDataViewElement element in flattenExprs)
                {
                    filterString += element.ToString();
                }
                */

                if (filterExpression is ParenthesizedExpr)
                {
                    // remove the parenthesis for the root expression
                    filterExpression = ((ParenthesizedExpr)filterExpression).Expression;
                }

                if (overrideFilter)
                    dataView.ClearSearchExpression();
                dataView.AddSearchExpr(filterExpression, ElementType.And);
            }
        }

        // Add the filter to the dataveiw which may have an existing filter expression
        public void AddFilters(DataViewModel dataView, string filters)
        {
            var filterObject = JsonConvert.DeserializeObject(filters);

            JArray filterTree = JArray.FromObject(filterObject);

            // filters example [["Category","contains","bug"],"and",[["Status","=","Closed"],"or",["Status","=","Fixed"]]]
            // convert to a tree
            CriteriaOperator searchCriteriaTree = BuildSearchCriteriaTree(filterTree);

            IDataViewElement filterExpression = ConvertOperator(searchCriteriaTree, dataView);

            if (filterExpression != null)
            {
                dataView.AddSearchExpr(filterExpression, ElementType.And);
            }
        }

        public CriteriaOperator BuildSearchCriteriaTree(JArray array)
        {
            CriteriaOperator criteria = null;

            if (array[0].Type == JTokenType.String)
            {
                string name = array[0].ToString();
                string op = array[1].ToString();
                string val = array[2].ToString();

                BinaryOperatorType opType = BinaryOperatorType.Equal;

                switch (op)
                {
                    case "=":
                        opType = BinaryOperatorType.Equal;
                        break;

                    case "<>":
                        opType = BinaryOperatorType.NotEqual;
                        break;

                    case "contains":
                        opType = BinaryOperatorType.Like;
                        break;

                    case ">":
                        opType = BinaryOperatorType.Greater;
                        break;

                    case ">=":
                        opType = BinaryOperatorType.GreaterOrEqual;
                        break;

                    case "<":
                        opType = BinaryOperatorType.Less;
                        break;

                    case "<=":
                        opType = BinaryOperatorType.LessOrEqual;
                        break;

                    case "in":
                        opType = BinaryOperatorType.In;
                        break;

                }

                BinaryOperator binaryOperator = new BinaryOperator(opType);
                binaryOperator.LeftOperand = new OperandProperty(name);
                binaryOperator.RightOperand = new OperandValue(val);

                criteria = binaryOperator;
            }
            else
            {
                GroupOperator groupOperator = null;
                CriteriaOperator lastOperator = null;
                for (int i = 0; i < array.Count; i++)
                {
                    if (array[i].ToString().Equals("and") ||
                        array[i].ToString().Equals("or"))
                    {
                        GroupOperatorType opType = GroupOperatorType.And;
                        switch (array[i].ToString())
                        {
                            case "and":
                                opType = GroupOperatorType.And;
                                break;

                            case "or":
                                opType = GroupOperatorType.Or;
                                break;
                        }

                        if (groupOperator == null)
                        {
                            groupOperator = new GroupOperator(opType);
                            if (lastOperator != null)
                            {
                                // a binary operator must have been created before logical operator
                                groupOperator.Operands.Add(lastOperator);
                                lastOperator = null;
                            }
                        }
                        else if (groupOperator.OperatorType != opType)
                        {
                            // logical operator is changed, start a new GroupOperator
                            groupOperator = new GroupOperator(opType);
                            if (lastOperator != null)
                            {
                                // a binary operator must have been created before logical operator
                                groupOperator.Operands.Add(lastOperator);
                                lastOperator = null;
                            }
                        }
                    }
                    else
                    {
                        lastOperator = BuildSearchCriteriaTree((JArray)array[i]);
                        if (groupOperator != null)
                        {
                            groupOperator.Operands.Add(lastOperator);
                            lastOperator = null;
                        }
                        else
                        {
                            groupOperator = new GroupOperator(GroupOperatorType.Or);
                            if (lastOperator != null)
                            {
                                groupOperator.Operands.Add(lastOperator);
                            }
                        }
                    }
                }

                criteria = groupOperator;
            }

            return criteria;
        }

        public IDataViewElement ConvertOperator(CriteriaOperator criteriaOperator, DataViewModel dataView)
        {
            IDataViewElement rootExpression = null;
            IDataViewElement childExpression = null;
            IDataViewElement leftOperand = null;
            IDataViewElement rightOperand = null;
            ElementType expressionOp = ElementType.Unknown;

            if (criteriaOperator is GroupOperator)
            {
                ParenthesizedExpr groupExpression = new ParenthesizedExpr(); // Newtera's group operator
                GroupOperator groupOperator = (GroupOperator)criteriaOperator;

                expressionOp = ConvertGroupOperatorType(groupOperator.OperatorType);
                foreach (CriteriaOperator operand in groupOperator.Operands)
                {
                    childExpression = ConvertOperator(operand, dataView);
                    groupExpression.AddSearchExpr(childExpression, expressionOp);
                }

                rootExpression = groupExpression;
            }
            else if (criteriaOperator is BinaryOperator)
            {
                BinaryOperator binaryOperator = (BinaryOperator)criteriaOperator;

                expressionOp = ConvertBinaryOperatorType(binaryOperator.OperatorType);

                leftOperand = ConvertOperator(binaryOperator.LeftOperand, dataView);
                rightOperand = ConvertOperator(binaryOperator.RightOperand, dataView);
                rootExpression = new RelationalExpr(expressionOp, leftOperand, rightOperand);
            }
            else if (criteriaOperator is OperandProperty)
            {
                OperandProperty operandProperty = (OperandProperty)criteriaOperator;
                DataSimpleAttribute simpleAttribute = dataView.ResultAttributes[operandProperty.PropertyName] as DataSimpleAttribute;
                if (simpleAttribute == null)
                {
                    // the attribute is not part of result list, find it from the search expression
                    DataViewElementCollection searchExprs = dataView.FlattenedSearchFilters;

                    foreach (IDataViewElement searchExpr in searchExprs)
                    {
                        if (searchExpr is DataSimpleAttribute &&
                            searchExpr.Name == operandProperty.PropertyName)
                        {
                            simpleAttribute = (DataSimpleAttribute)searchExpr;
                        }
                    }
                }

                if (simpleAttribute != null)
                {
                    rootExpression = new DataSimpleAttribute(simpleAttribute.Name, simpleAttribute.OwnerClassAlias);

                    // remember the property name for converting the right operand
                    _leftOperandPropertyName = simpleAttribute.Name;
                }
                else if (operandProperty.PropertyName == "@obj_id")
                {
                    _leftOperandPropertyName = operandProperty.PropertyName;
                }
                else
                {
                    // try relationship attribute
                    DataRelationshipAttribute relationshipAttribute = dataView.ResultAttributes[operandProperty.PropertyName] as DataRelationshipAttribute;
                    if (relationshipAttribute == null)
                    {
                        // the attribute is not part of result list, find it from the search expression
                        DataViewElementCollection searchExprs = dataView.FlattenedSearchFilters;

                        foreach (IDataViewElement searchExpr in searchExprs)
                        {
                            if (searchExpr is DataRelationshipAttribute &&
                                searchExpr.Name == operandProperty.PropertyName)
                            {
                                relationshipAttribute = (DataRelationshipAttribute)searchExpr;
                            }
                        }
                    }

                    if (relationshipAttribute != null)
                    {
                        rootExpression = new DataRelationshipAttribute(relationshipAttribute.Name, relationshipAttribute.OwnerClassAlias, relationshipAttribute.LinkedClassName);

                        // remember the property name for converting the right operand
                        _leftOperandPropertyName = relationshipAttribute.Name;
                    }
                }
            }
            else if (criteriaOperator is OperandValue)
            {
                Newtera.Common.MetaData.DataView.Parameter parameter = null;

                OperandValue operandValue = (OperandValue)criteriaOperator;
                DataSimpleAttribute simpleAttribute = dataView.ResultAttributes[_leftOperandPropertyName] as DataSimpleAttribute;
                if (simpleAttribute == null)
                {
                    // the attribute is not part of result list, find it from the search expression
                    DataViewElementCollection searchExprs = dataView.FlattenedSearchFilters;

                    foreach (IDataViewElement searchExpr in searchExprs)
                    {
                        if (searchExpr is DataSimpleAttribute &&
                            searchExpr.Name == _leftOperandPropertyName)
                        {
                            simpleAttribute = (DataSimpleAttribute)searchExpr;
                        }
                    }
                }

                if (simpleAttribute != null)
                {
                    SimpleAttributeElement simpleAttributeElement = simpleAttribute.GetSchemaModelElement() as SimpleAttributeElement;

                    if (simpleAttributeElement.IsMultipleChoice)
                    {
                        // Use String type for the attribute with multiple choices
                        parameter = new Newtera.Common.MetaData.DataView.Parameter(simpleAttributeElement.Name, dataView.BaseClass.Alias, DataType.String);
                    }
                    else
                    {
                        parameter = new Newtera.Common.MetaData.DataView.Parameter(simpleAttributeElement.Name, dataView.BaseClass.Alias, simpleAttributeElement.DataType);
                    }

                    if (operandValue.Value != null)
                    {
                        // remove the '%' at the begining and end of the value if exist, since SQL Builder will do the job for LIKE operator
                        string searchValue = operandValue.Value.ToString();
                        if (searchValue.StartsWith("%"))
                        {
                            searchValue = searchValue.Substring(1);

                            if (searchValue.EndsWith("%"))
                            {
                                searchValue = searchValue.Substring(0, searchValue.Length - 1);
                            }
                        }

                        parameter.ParameterValue = searchValue;
                    }
                }
                else if (_leftOperandPropertyName == "@obj_id")
                {
                    parameter = new Newtera.Common.MetaData.DataView.Parameter(_leftOperandPropertyName, dataView.BaseClass.Alias, DataType.String);
                    parameter.ParameterValue = operandValue.Value;
                }
                else
                {
                    // unable to find a simple attribute for the given name, try relatiosnhip attribute
                    DataRelationshipAttribute relationshipAttribute = dataView.ResultAttributes[_leftOperandPropertyName] as DataRelationshipAttribute;
                    if (relationshipAttribute == null)
                    {
                        // the attribute is not part of result list, find it from the search expression
                        DataViewElementCollection searchExprs = dataView.FlattenedSearchFilters;

                        foreach (IDataViewElement searchExpr in searchExprs)
                        {
                            if (searchExpr is DataRelationshipAttribute &&
                                searchExpr.Name == _leftOperandPropertyName)
                            {
                                relationshipAttribute = (DataRelationshipAttribute)searchExpr;
                            }
                        }
                    }

                    if (relationshipAttribute != null)
                    {
                        parameter = new Newtera.Common.MetaData.DataView.Parameter(relationshipAttribute.Name, dataView.BaseClass.Alias, DataType.String);

                        if (operandValue.Value != null)
                        {
                            // remove the '%' at the begining and end of the value if exist, since SQL Builder will do the job for LIKE operator
                            string searchValue = operandValue.Value.ToString();
                            if (searchValue.StartsWith("%"))
                            {
                                searchValue = searchValue.Substring(1);

                                if (searchValue.EndsWith("%"))
                                {
                                    searchValue = searchValue.Substring(0, searchValue.Length - 1);
                                }
                            }

                            parameter.ParameterValue = searchValue;
                        }
                    }
                }

                rootExpression = parameter;
            }
            else
            {
                //throw new Exception("Unsupported operator");
            }

            return rootExpression;
        }

        public  ElementType ConvertGroupOperatorType(GroupOperatorType operatorType)
        {
            ElementType opType = ElementType.Unknown;
            switch (operatorType)
            {
                case GroupOperatorType.And:
                    opType = ElementType.And;
                    break;

                case GroupOperatorType.Or:
                    opType = ElementType.Or;
                    break;
                default:
                    throw new Exception("Unsupported group type");
            }

            return opType;
        }

        public  ElementType ConvertBinaryOperatorType(BinaryOperatorType operatorType)
        {
            ElementType opType = ElementType.Unknown;
            switch (operatorType)
            {
                case BinaryOperatorType.Equal:
                    opType = ElementType.Equals;
                    break;

                case BinaryOperatorType.NotEqual:
                    opType = ElementType.NotEquals;
                    break;

                case BinaryOperatorType.Greater:
                    opType = ElementType.GreaterThan;
                    break;

                case BinaryOperatorType.GreaterOrEqual:
                    opType = ElementType.GreaterThanEquals;
                    break;

                case BinaryOperatorType.Less:
                    opType = ElementType.LessThan;
                    break;

                case BinaryOperatorType.LessOrEqual:
                    opType = ElementType.LessThanEquals;
                    break;

                case BinaryOperatorType.Like:
                    opType = ElementType.Like;
                    break;

                case BinaryOperatorType.In:
                    opType = ElementType.In;
                    break;

                default:
                    throw new Exception("Unsupported operator type");

                    //throw new Exception(Enum.GetName(typeof(BinaryOperatorType), operatorType) + " operator is currently not supported.");
            }

            return opType;
        }

        /// <summary>
        /// Get the search text for Keywords
        /// </summary>
        /// <param name="filters"></param>
        /// <returns>The search text or null</returns>
        public string GetSearchText(string filters)
        {
            string searchText = null;
            var filterObject = JsonConvert.DeserializeObject(filters);

            JArray array = JArray.FromObject(filterObject);

            if (array[0].Type == JTokenType.String)
            {
                string name = array[0].ToString();
                string op = array[1].ToString();
                searchText = array[2].ToString();
            }

            return searchText;
        }

        public  void SetSort(DataViewModel dataView, string sortField, bool sortReverse)
        {
            // clear the existing sort expression
            dataView.ClearSortBy();

            // add as a sort attribute
            SortAttribute sortAttribute;
            DataSimpleAttribute simpleAttribute = dataView.ResultAttributes[sortField] as DataSimpleAttribute;

            if (simpleAttribute != null)
            {
                sortAttribute = new SortAttribute(sortField, simpleAttribute.OwnerClassAlias);
            }
            else
            {
                sortAttribute = new SortAttribute(sortField, dataView.BaseClass.Alias);
            }
   
            if (sortReverse)
            {
                sortAttribute.SortDirection = Newtera.Common.MetaData.Schema.SortDirection.Descending;
            }
            else
            {
                sortAttribute.SortDirection = Newtera.Common.MetaData.Schema.SortDirection.Ascending;
            }

            dataView.SortBy.SortAttributes.Add(sortAttribute);
        }

        public  string GetConnectionString(string template, string schemaName)
        {
            string connectionString = template.Replace("{schemaName}", schemaName);

            return connectionString;
        }

        /// <summary>
        /// Create an instance of the IApiFunction as specified.
        /// </summary>
        /// <param name="handlerDefinition">The definition of the custom function</param>
        /// <returns>An instance of the IApiFunction, null if failed to create the instance.</returns>
        public  IApiFunction CreateExternalHandler(string handlerDefinition)
        {
            IApiFunction function = null;

            if (!string.IsNullOrEmpty(handlerDefinition))
            {
                int index = handlerDefinition.IndexOf(",");
                string assemblyName = null;
                string className;

                if (index > 0)
                {
                    className = handlerDefinition.Substring(0, index).Trim();
                    assemblyName = handlerDefinition.Substring(index + 1).Trim();
                }
                else
                {
                    className = handlerDefinition.Trim();
                }

                try
                {
                    ObjectHandle obj = Activator.CreateInstance(assemblyName, className);
                    function = (IApiFunction)obj.Unwrap();
                }
                catch
                {
                    function = null;
                }
            }

            return function;
        }

        public  string InsertFilter(string query, string filters)
        {
            StringBuilder builder = new StringBuilder();

            int pos = query.IndexOf("]");

            if (pos > 0)
            {
                // query is in form of the follwoing example
                //for $issues in document("db://IssueTracker.xml") / IssuesList / Issues[1 to 1000]
                // return
                // < Issues {$issues / @obj_id, $issues / @xsi:type, $issues / @attachments, $issues / @permission, $issues / @read_only}>
                // {$issues / Title}
                // {$issues / Description}
                // {$issues / Status}
                // {$issues / Category}
                // {$issues / SubmitDate}
                //{$issues / SubmitBy}
                // </ Issues >
                // the filter is inserted $ position at [1 to 1000 and $]
                builder.Append(query.Substring(0, pos));
                builder.Append(" and ").Append(filters);
                builder.Append(query.Substring(pos));
            }

            return builder.ToString();
        }

        public string GetSchemaId(string connectionStr)
        {
            Hashtable properties = GetProperties(connectionStr);
            Newtera.Common.Core.SchemaInfo schemaInfo = new Newtera.Common.Core.SchemaInfo();
            schemaInfo.Name = (string)properties["SCHEMA_NAME"];
            schemaInfo.Version = (string)properties["SCHEMA_VERSION"];

            return schemaInfo.NameAndVersion;
        }

        /// <summary>
        /// Convert json or xml string data into corresponding object
        /// </summary>
        /// <param name="postData">Data posted</param>
        /// <param name="mineType">Mine Type</param>
        /// <returns>Object</returns>
        public dynamic ConvertToObject(string postData, string mineType)
        {
            if (string.IsNullOrEmpty(mineType) || mineType.StartsWith("application/json"))
            {
                // default mine type is json
                return JsonConvert.DeserializeObject<JObject>(postData);
            }
            else if (mineType.StartsWith("application/xml"))
            {
                XmlDocument xmlDoc = new XmlDocument();
                xmlDoc.LoadXml(postData);

                return xmlDoc;
            }
            else
            {
                throw new Exception("Unsupported mine type " + mineType);
            }
        }

        /// <summary>
        /// Get key/value pairs from the connectionString and save them in a hashtable
        /// </summary>
        /// <param name="connectionString">The connectionString</param>
        /// <returns>The hashtable</returns>
        /// <exception cref="InvalidConnectionStringException">
        /// Thrown if missing some critical key/value pairs in the connection string.
        /// </exception>
        private Hashtable GetProperties(string connectionString)
        {
            Hashtable properties = new Hashtable();

            // Compile regular expression to find "name = value" pairs
            Regex regex = new Regex(@"\w+\s*=\s*[^;]*");

            MatchCollection matches = regex.Matches(connectionString);
            foreach (Match match in matches)
            {
                int pos = match.Value.IndexOf("=");
                string key = match.Value.Substring(0, pos).TrimEnd();
                string val = match.Value.Substring(pos + 1).TrimStart();
                properties[key] = val;
            }

            return properties;
        }
    }
}