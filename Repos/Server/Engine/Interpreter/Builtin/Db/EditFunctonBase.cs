/*
* @(#)EditFunctionBase.cs
*
* Copyright (c) 2003 Newtera, Inc. All rights reserved.
*
*/
namespace Newtera.Server.Engine.Interpreter.Builtin.Db
{
	using System;
	using System.Threading;
	using System.Xml;
	using System.Collections;
	using System.Collections.Specialized;
	using Newtera.Common.Core;
	using Newtera.Server.Engine.Interpreter;
	using Newtera.Server.Engine.Vdom;
	using Newtera.Server.Engine.Validator;
	using Newtera.Server.Engine.Sqlbuilder.Sql;
	using Newtera.Common.MetaData;
	using Newtera.Common.MetaData.Schema;
	using Newtera.Common.MetaData.Principal;
	
	/// 
	/// <summary>
	/// This is base class for some DB virtual document edit-related builtin function
	/// implementation classes.
	/// </summary>
	/// <version>  1.0 24 Aug 2003</version>
	/// <author>  Yong Zhang </author>
	public abstract class EditFunctionBase : FunctionImpBase
	{
		protected VDocument _vdoc = null;
		
		protected ValueCollection _instances = null;
		
		/// <summary>
		/// The default constructor.
		/// </summary>
		public EditFunctionBase() : base()
		{
		}

		/// <summary>
		/// Validate the invoking arguments of a function
		/// </summary>
		/// <param name="arguments">The arguments</param>
		/// <exception cref="InterpreterException">Thrown if the number of arguments mismatched or invalid arguments.</exception>
		public override void CheckArgs(ExprCollection arguments)
		{
			if (arguments.Count != 2)
			{
				throw new InterpreterException("Expecting two arguments, but got " + arguments.Count);
			}	
		}
		
		/// <summary>
		/// Evaluate the arguments of the function
		/// </summary>
		protected void EvalArgs()
		{
			// The first arg must be VDocument.
			Value result = ((IExpr) _arguments[0]).Eval();
			if (!(result is XNode) || !(result.ToNode() is VDocument))
			{
				throw new InterpreterException("Wrong type of the first argument, expected VDocument");
			}
			else
			{
				_vdoc = (VDocument) result.ToNode();
			}
			
			// Check the second arguments
			result = ((IExpr) _arguments[1]).Eval();
			if (result.DataType.IsCollection)
			{
				// Get the instancs' list.
				_instances = result.ToCollection();
			}
			else if (result.DataType.IsXmlNode && result.ToNode() is XmlElement)
			{
				_instances = new ValueCollection();
				_instances.Add(result);
			}
			else
			{
				throw new InterpreterException("Wrong type of second argument, expected a path");
			}	
		}

		/// <summary>
		/// Add a collection of given data instances to the specified VDocument object.
		/// </summary>
		/// <returns>
		/// A string consisting of obj_id(s) created for the inserted instances.
		/// </returns>
		protected string Add()
		{
			string objIds = "";
			Interpreter interpreter = new Interpreter();
			
			if (_instances.Count == 0)
			{
				throw new InterpreterException("No instances to be inserted.");
			}
			
			/*
			* To support auto-increment, adds auto-increment element with a
			* bogus value.
			* Modified by Yong Zhang, 2003-06-18
			*/
			AddIncrementElement();
			
			// validate the instances
			Validate(_instances);
			
			objIds = _vdoc.InsertNodes(_instances, interpreter);
			
			return objIds;
		}
		
		/// <summary>
		/// To support auto-increment, adds auto-increment element with a bogus value.
		/// </summary>
		protected void AddIncrementElement()
		{
			SchemaModel schemaModel = _vdoc.MetaData.SchemaModel;
			
			foreach (XNode instance in _instances)
			{
				XmlElement element = (XmlElement) instance.ToNode();
				string className = element.GetAttribute(XMLSchemaInstanceNameSpace.TYPE, XMLSchemaInstanceNameSpace.URI);
				if (className == null)
				{
					className = element.Name; // instance name can be a class name
				}
				ClassElement classElem = schemaModel.FindClass(className);
				
				if (classElem == null)
					return ;
				
				// go through all attributes including those inherited ones
				ClassElement current = classElem;
				while (current != null)
				{
					SchemaModelElementCollection attrs = current.SimpleAttributes;				
					foreach (SimpleAttributeElement attr in attrs)
					{				
						if (attr.IsAutoIncrement && (attr.IsPrimaryKey || attr.IsRequired))
						{
							// find the corresponding child element in the instance
							XmlElement child = element[attr.Name];
							if (child == null)
							{
								child = _vdoc.CreateElement(attr.Name);
								child.InnerText = "0"; // bogus value
								// add the child element to the instance
								// TODO, insert the child at the right position
								element.AppendChild(child);
							}
						}
					}

					current = current.ParentClass;
				}
			}
		}
		
		/// <summary>
		/// Delete a collection of given data instances from the specified VDocument object.
		/// </summary>
		/// <returns> a string consisting of obj_ids of deleted instances</returns>
		protected virtual string Delete()
		{
			string ret;
			Interpreter interpreter = new Interpreter();
			
			ret = _vdoc.DeleteNodes(_instances, interpreter);
			
			return ret;
		}
		
		/// <summary>
		/// Update a set of given XML instances to the specified VDocument object.
		/// </summary>
		/// <returns> a string consisting of obj_ids of updated instances.</returns>
		/// <exception cref="">throws InterpreterException If update instance failure!</exception>
		protected virtual string Update()
		{
			string ret;
			ValueCollection originalInstances = new ValueCollection();
			Interpreter interpreter = new Interpreter();
            if (_vdoc.Interpreter.HasGlobalTransaction)
            {
                //ErrorLog.Instance.WriteLine("Upadate called with global transaction");
                interpreter.IDbConnection = _vdoc.Interpreter.IDbConnection;
                interpreter.IDbTransaction = _vdoc.Interpreter.IDbTransaction;
            }
			
			/*
			* get the original instances to make to merge with the updating instances.
			* At the same time, keep the documents that contains original instances
			* for permission checking purpose.
			*/
			CustomPrincipal principal = (CustomPrincipal) Thread.CurrentPrincipal;

			// tell the SearchExecutor to keep a copy of VDocument at user info
			principal.NeedCurrentDocumentStatus = true;
			foreach (XNode updateInstance in _instances)
			{
				XmlElement originalInstance = GetOriginalInstance((XmlElement) updateInstance.ToNode(), interpreter);
				originalInstances.Add(new XNode(originalInstance));
			}
			principal.NeedCurrentDocumentStatus = false;
			principal.CurrentDocument = null;
			
			/*
			* The update instances may contain only changed values.
			* However, the validator requires all the required values to be present in
			* the instances. Therefore, we have to merge original instances and updated
			* instances, and then give them over to the validator.
			*/
			ValueCollection mergedInstances = GetMergedInstances(_instances, originalInstances);
			
			Validate(mergedInstances, false); // Validate the full instances		

            ret = _vdoc.UpdateNodes(_instances, interpreter);

			return ret;
		}
		
		/// <summary>
		/// Valicate a set of XML instances, which will do the DB validation.
		/// </summary>
		/// <param name="instances">The List of given XML instances.</param>
		/// <exception cref="ValidateException">If any instance is not valid.</exception>
		protected virtual void Validate(ValueCollection instances)
		{
			Validate(instances, true);
		}
		
		/// <summary>
		/// Validate a set of XML instances, which are stored in the given list.
		/// </summary>
		/// <param name="instances">The collection of given XML instances.</param>
		/// <param name="isDBEnabled">Indicate whether do the DB validation.</param>
		/// <exception cref="ValidateException">ValidateException If any instance is not valid.</exception>
		protected virtual void Validate(ValueCollection instances, bool isDBEnabled)
		{
			SchemaModel schemaModel = _vdoc.MetaData.SchemaModel;
			SchemaValidator validator = new SchemaValidator(schemaModel, isDBEnabled);
			
			foreach (XNode instance in instances)
			{
				// Replace the empty sub-element with xsi:nil= true syntax
				FixEmptyElement((XmlElement) instance.ToNode());
				
				// Validate this instance.
				VDocument instanceDoc = GetWholeDocument((XmlElement) instance.ToNode(), schemaModel);
					
				//System.Console.WriteLine("XML to be validated is: ");
				
				validator.Validate(instanceDoc);
			}
		}
		
		/// <summary>
		/// Creates the whole XML instance document from the given xml element.
		/// </summary>
		/// <param name="inElement">The given xml Element
		/// </param>
		/// <returns> The whole XML document </returns>
		protected VDocument GetWholeDocument(XmlElement inElement, SchemaModel schemaModel)
		{
			string rootName = schemaModel.SchemaInfo.Name;
			VDocument instanceDoc = DocumentFactory.Instance.Create();
	
			// Construct the root element of the whole XML instance document
			XmlElement root = instanceDoc.CreateElement(rootName);
			//TODO root.NamespaceURI = NewteraNameSpace.URI;
			//root.addNamespaceDeclaration(XMLSchemaInstanceNameSpace.NS);TODO
			
			instanceDoc.AppendChild(root);

			// Construct the XXXList element where XXX is name of a root class.
			XmlElement element = instanceDoc.CreateElement(inElement.Name + NewteraNameSpace.LIST_SUFFIX);
			root.AppendChild(element);

			// Add the given element to the XXXList element
			XmlElement ele = (XmlElement) instanceDoc.ImportNode(inElement, true);
			//SetURI(ele, NewteraNameSpace.URI);
			element.AppendChild(ele);
			
			return instanceDoc;
		}
		
		/// <summary>
		///  Set the specified element's URL to the given URL.
		/// </summary>
		/// <param name="element">The specified element
		/// </param>
		/// <param name="na">The given URI.
		/// 
		/// </param>
		private void SetURI(XmlElement element, string uri)
		{
			// TODO element.NamespaceURI = uri;
			
			if (element.HasChildNodes)
			{
				foreach (XmlElement subElem in element.ChildNodes)
				{
					SetURI(subElem, uri);
				}
			}
		}
		
		/// <summary>
		/// Get the merged instances by combining the original instances with the updated
		/// values.
		/// </summary>
		/// <param name="updateInstances">the instances containing updated values
		/// </param>
		/// <param name="originalInstances">the instances containing original values
		/// </param>
		/// <returns> the merged instances </returns>
		private ValueCollection GetMergedInstances(ValueCollection updateInstances, ValueCollection originalInstances)
		{
			
			ValueCollection fullInstances = new ValueCollection();
			XmlElement updateChild;
			ClassElement classElem;

			for (int i = 0; i < updateInstances.Count; i++)
			{
				// Get each update instance.
				XmlElement updateInstance = (XmlElement) updateInstances[i].ToNode();
				XmlElement originalInstance = (XmlElement) originalInstances[i].ToNode();

				// Create the merged instance stub using name of root class
				// to meet the requirement of validator
				classElem = _vdoc.MetaData.SchemaModel.FindClass(originalInstance.Name);				
				XmlElement mergedInstance = _vdoc.CreateElement(classElem.RootClass.Name);
				
				/*
				* Merge the update instance with the original one to form a full instance
				* which will pass to the validator.
				*
				* TODO, currently relationship elements in update instance are not merged
				*       due to the fact that the original instance does not have element child
				*       representing a relationship (relationships are treated as
				*       attributes with obj_id as values). Therefore, it is difficult to
				*       place elements representing releationships at right positions in the
				*       merged instance. In the future, when an original instance returned
				*       contains relationship elements, this problem can be easily solved.
				*/
				foreach (XmlElement orignalChild in originalInstance.ChildNodes)
				{
					updateChild = (XmlElement) updateInstance[orignalChild.Name];
					XmlElement child;

					// merging
					if (updateChild != null)
					{
						if (updateChild.InnerText != orignalChild.InnerText)
						{
							if (updateChild.OwnerDocument != mergedInstance.OwnerDocument)
							{
								child = (XmlElement) mergedInstance.OwnerDocument.ImportNode(updateChild, true);
							}
							else
							{
								child = updateChild;
							}
						}
						else
						{
							if (orignalChild.OwnerDocument != mergedInstance.OwnerDocument)
							{
								child = (XmlElement) mergedInstance.OwnerDocument.ImportNode(orignalChild, true);
							}
							else
							{
								child = orignalChild;
							}

							// No change, remove element from the update instance
							//updateInstance.RemoveChild(updateChild); // comment out to allow update when value is not changed in order to force firing events (changed  on 2016-09-20)
						}

						mergedInstance.AppendChild(child);
					}
					else
					{
						if (orignalChild.OwnerDocument != mergedInstance.OwnerDocument)
						{
							child = (XmlElement) mergedInstance.OwnerDocument.ImportNode(orignalChild, true);
						}
						else
						{
							child = orignalChild;
						}

						mergedInstance.AppendChild(child);
					}
				}
				
				/*
				* Copy only relationship attributes from the original instance
				* to the merged instance
				*/
				XmlAttribute att;
				foreach (XmlAttribute orignalAttribute in originalInstance.Attributes)
				{
					XmlAttribute updateAttribute = updateInstance.GetAttributeNode(orignalAttribute.Name);
					
					if (updateAttribute != null)
					{
						if (orignalAttribute.LocalName != SQLElement.ATTACHMENT_COUNT &&
							orignalAttribute.LocalName != SQLElement.OBJ_ID &&
							orignalAttribute.LocalName != SQLElement.CLS_ID &&
							updateAttribute.Value == orignalAttribute.Value)
						{
							/*
							* If the attribute values are same, remove the attribute from updateInstance
							*/
							updateInstance.RemoveAttribute(updateAttribute.Name);
						}
						
						att = (XmlAttribute) _vdoc.ImportNode(updateAttribute, true);
						mergedInstance.SetAttributeNode(att);
					}
					else
					{
						/*
						* TODO: this check should be removed once the retrieved instance contains
						*       the elements for relationships.
						*
						* If the update instance contains an empty element with the same name
						* as the attribute. For example, the attribute name is customer, and the
						* update instance contains an element look like: <customer xsi:nil="true" />
						* then this update is intended to set customer attribute to null, therefore,
						* do not add the attribute to the merged instance.
						*
						*/
						updateChild = (XmlElement) updateInstance[orignalAttribute.Name];
						if (updateChild == null ||
							updateChild.GetAttribute(XMLSchemaInstanceNameSpace.NIL, XMLSchemaInstanceNameSpace.URI) == "" ||
							updateChild.GetAttribute(XMLSchemaInstanceNameSpace.NIL, XMLSchemaInstanceNameSpace.URI) != "true")
						{
							att = (XmlAttribute) _vdoc.ImportNode(orignalAttribute, true);
							mergedInstance.SetAttributeNode(att);
						}
					}
				}
				
				fullInstances.Add(new XNode(mergedInstance));
			}
			
			return fullInstances;
		}
		
		/// <summary>
		/// Get the original instance given obj_id and leaf class name. 
		/// </summary>
		/// <param name="updateInstance">the XmlElement containing the obj_id and leaf class name
		/// </param>
		/// <param name="interpreter">the interpreter to run a query for getting an instance
		/// </param>
		/// <returns> the original instance
		/// </returns>
		private XmlElement GetOriginalInstance(XmlElement updateInstance, Interpreter interpreter)
		{
			XmlElement originalInstance;
			XmlAttribute typeAttr = updateInstance.GetAttributeNode(SQLElement.CLS_ID, XMLSchemaInstanceNameSpace.URI);
			XmlAttribute objIdAttr = updateInstance.GetAttributeNode(SQLElement.OBJ_ID);
			
			string query = BuildOrignalInstanceQuery(updateInstance, true);
			
			// run the query to get the instance
			interpreter.Reset();
			XmlDocument doc = interpreter.Query(query);
			XmlNodeList children = doc.DocumentElement.ChildNodes;
			if (children.Count == 1)
			{
				originalInstance = (XmlElement) children[0];
				
				// HACK, remove the attachment attribute because the validator dose not like it
				originalInstance.RemoveAttribute(SQLElement.ATTACHMENT_COUNT);
			}
			else
			{
				throw new InterpreterException("Either you don't have permission to access the instance with obj_id of " + objIdAttr.Value + " in class " + typeAttr.Value + " or it does not exist.");
			}
			
			return originalInstance;
		}
		
		/// <summary>
		/// Build an xquery to retrieve the original instance given obj_id and leaf class name.
		/// </summary>
		/// <param name="updateInstance">the XmlElement containing the obj_id and leaf class name
		/// </param>
		/// <param name="hasObjID">whether the instance contains obj_id value.
		/// </param>
		/// <returns> the xquery</returns>
		private string BuildOrignalInstanceQuery(XmlElement updateInstance, bool hasObjID)
		{
			XmlAttribute typeAttr, objIdAttr;
			System.Text.StringBuilder query = new System.Text.StringBuilder();
			
			// Constructing a XQuery for retrieving the instance
			query.Append("document(\"").Append(_vdoc.URL).Append("\")//");
			typeAttr = updateInstance.GetAttributeNode(SQLElement.CLS_ID, XMLSchemaInstanceNameSpace.URI);
			
			if (typeAttr != null)
			{
				// Constructing part of XPath, e.g. CustomerList/Customer
				query.Append(typeAttr.Value).Append(SQLElement.ELEMENT_CLASS_NAME_SUFFIX).Append("/");
				query.Append(typeAttr.Value);
			}
			else
			{
				throw new InterpreterException("Missing xsi:type attribute");
			}
			
			query.Append("[@obj_id=\"");
			if (hasObjID)
			{
				objIdAttr = updateInstance.GetAttributeNode(SQLElement.OBJ_ID);
				if (objIdAttr != null)
				{
					query.Append(objIdAttr.Value);
				}
				else
				{
					throw new InterpreterException("Missing obj_id attribute");
				}
			}
			else
			{
				query.Append("??");
			}

			query.Append("\"]");
			
			return query.ToString();
		}
		
		/// <summary>
		/// Replace the empty elements in an instance with the xsi:nil=true syntax.
		/// For example, an instance has an element <Name></Name>, replace it with
		/// <Name xsi:nil=true>
		/// </summary>
		/// <param name="instance">the instance whose elements to be fixed
		/// 
		/// </param>
		private void FixEmptyElement(XmlElement instance)
		{
			foreach (XmlElement element in instance.ChildNodes)
			{
				if (element.InnerText == "" && element.GetAttributeNode(XMLSchemaInstanceNameSpace.NIL, XMLSchemaInstanceNameSpace.URI) == null)
				{
					element.SetAttribute(XMLSchemaInstanceNameSpace.NIL, XMLSchemaInstanceNameSpace.URI, "true");
				}
			}
		}
	}
}