/*
* @(#)DataViewValidateVisitor.cs
*
* Copyright (c) 2003 Newtera, Inc. All rights reserved.
*
*/
namespace Newtera.Common.MetaData.DataView.Validate
{
	using System;
	using System.Resources;

	using Newtera.Common.MetaData.Schema;
	using Newtera.Common.MetaData.DataView;
	using Newtera.Common.MetaData.DataView.Taxonomy;

	/// <summary>
	/// Represents a visitor that validate a data view to see if has any inconsistencies
	/// between the data view and schema model.
	/// </summary>
	/// <version> 1.0.0 26 Nov 2003 </version>
	/// <author> Yong Zhang</author>
	public class DataViewValidateVisitor : IDataViewElementVisitor
	{
		private DataViewValidateResult _result;
		private ResourceManager _resources;
        private DataViewModel _dataView;

		/// <summary>
		/// Initiate an instance of DataViewValidateVisitor class
		/// </summary>
		public DataViewValidateVisitor()
		{
			_result = new DataViewValidateResult();
			_resources = new ResourceManager(this.GetType());
            _dataView = null;
		}

		/// <summary>
		/// Gets the validate result
		/// </summary>
		/// <value>A DataViewValidateResult</value>
		public DataViewValidateResult ValidateResult
		{
			get
			{
				return _result;
			}
		}

		/// <summary>
		/// Viste a data view element.
		/// </summary>
		/// <param name="element">A DataViewModel instance</param>
		/// <returns>true to contibute visiting nested elements, false to stop</returns>		
		public bool VisitDataView(DataViewModel element)
		{
            _dataView = element;

			if (element != null && element.ResultAttributes.Count == 0)
			{
                string msg = string.Format(_resources.GetString("NoResultAttributes"), (_dataView != null ? _dataView.Caption : ""));
				DataValidateResultEntry entry = new DataValidateResultEntry(msg, GetSource(element), element);
				_result.AddError(entry);
			}

			return true;
		}

		/// <summary>
		/// Viste a data class element.
		/// </summary>
		/// <param name="element">A DataClass instance</param>
		/// <returns>true to contibute visiting nested elements, false to stop</returns>		
		public bool VisitDataClass(DataClass element)
		{
			ClassElement classElement = element.GetSchemaModelElement() as ClassElement;
            DataValidateResultEntry entry;
            string msg;

			if (classElement == null)
			{
                msg = string.Format(_resources.GetString("UnknownClass"), "");
				entry = new DataValidateResultEntry(msg, GetSource(element), element);
				_result.AddError(entry);
			}
            else if (element.Type == DataClassType.ReferencedClass && _dataView != null)
            {
                DataClass referringClass = _dataView.FindClass(element.ReferringClassAlias);
                if (referringClass == null)
                {
                    msg = string.Format(_resources.GetString("UnknownParentClass"), (_dataView != null ? _dataView.Caption : ""));

                    entry = new DataValidateResultEntry(msg, GetSource(element), element);
                    _result.AddError(entry);
                }
                else
                {
                    ClassElement referringClassElement = referringClass.GetSchemaModelElement() as ClassElement;

                    // validate the relatioship
                    RelationshipAttributeElement relationship = referringClassElement.FindInheritedRelationshipAttribute(element.ReferringRelationshipName);
                    if (relationship == null)
                    {
                        msg = string.Format(_resources.GetString("UnknownRelationshipAttribute"), (_dataView != null ? _dataView.Caption : ""));

                        entry = new DataValidateResultEntry(msg, GetSource(referringClass), referringClass);
                        _result.AddError(entry);
                    }
                }
            }

			return true;
		}

		/// <summary>
		/// Viste a filter element.
		/// </summary>
		/// <param name="element">A Filter instance</param>
		/// <returns>true to contibute visiting nested elements, false to stop</returns>		
		public bool VisitFilter(Filter element)
		{
			return true;
		}

		/// <summary>
		/// Viste a simple attribute element.
		/// </summary>
		/// <param name="element">A DataSimpleAttribute instance</param>
		/// <returns>true to contibute visiting nested elements, false to stop</returns>
		public bool VisitSimpleAttribute(DataSimpleAttribute element)
		{
			SimpleAttributeElement schemaModelElement = (SimpleAttributeElement) element.GetSchemaModelElement();
			
			if (schemaModelElement == null)
			{
                string msg = string.Format(_resources.GetString("UnknownSimpleAttribute"), (_dataView != null ? _dataView.Caption : ""));

				DataValidateResultEntry entry = new DataValidateResultEntry(msg, GetSource(element), element);
				_result.AddError(entry);
			}

			return true;
		}

		/// <summary>
		/// Viste an array attribute element.
		/// </summary>
		/// <param name="element">A DataArrayAttribute instance</param>
		/// <returns>true to contibute visiting nested elements, false to stop</returns>
		public bool VisitArrayAttribute(DataArrayAttribute element)
		{
			ArrayAttributeElement schemaModelElement = (ArrayAttributeElement) element.GetSchemaModelElement();
			
			if (schemaModelElement == null)
			{
                string msg = string.Format(_resources.GetString("UnknownArrayAttribute"), (_dataView != null ? _dataView.Caption : ""));
				DataValidateResultEntry entry = new DataValidateResultEntry(msg, GetSource(element), element);
				_result.AddError(entry);
			}

			return true;
		}

        /// <summary>
        /// Viste a virtual attribute element.
        /// </summary>
        /// <param name="element">A DataVirtualAttribute instance</param>
        /// <returns>true to contibute visiting nested elements, false to stop</returns>		
        public bool VisitVirtualAttribute(DataVirtualAttribute element)
        {
            VirtualAttributeElement schemaModelElement = (VirtualAttributeElement)element.GetSchemaModelElement();

            if (schemaModelElement == null)
            {
                string msg = string.Format(_resources.GetString("UnknownVirtualAttribute"), (_dataView != null ? _dataView.Caption : ""));

                DataValidateResultEntry entry = new DataValidateResultEntry(msg, GetSource(element), element);
                _result.AddError(entry);
            }

            return true;
        }

        /// <summary>
        /// Viste an image attribute element.
        /// </summary>
        /// <param name="element">A DataImageAttribute instance</param>
        /// <returns>true to contibute visiting nested elements, false to stop</returns>
        public bool VisitImageAttribute(DataImageAttribute element)
        {
            ImageAttributeElement schemaModelElement = (ImageAttributeElement)element.GetSchemaModelElement();

            if (schemaModelElement == null)
            {
                string msg = string.Format(_resources.GetString("UnknownImageAttribute"), (_dataView != null ? _dataView.Caption : ""));
                DataValidateResultEntry entry = new DataValidateResultEntry(msg, GetSource(element), element);
                _result.AddError(entry);
            }

            return true;
        }

		/// <summary>
		/// Viste a relationship attribute element.
		/// </summary>
		/// <param name="element">A DataRelationshipAttribute instance</param>
		/// <returns>true to contibute visiting nested elements, false to stop</returns>
		public bool VisitRelationshipAttribute(DataRelationshipAttribute element)
		{
			DataValidateResultEntry entry = null;
			RelationshipAttributeElement schemaModelElement = (RelationshipAttributeElement) element.GetSchemaModelElement();

			if (schemaModelElement == null)
			{
                string msg = string.Format(_resources.GetString("UnknownRelationshipAttribute"), (_dataView != null ? _dataView.Caption : ""));
                entry = new DataValidateResultEntry(msg, GetSource(element), element);
				_result.AddError(entry);
			}

			return false;
		}

		/// <summary>
		/// Viste a result attribute Collection.
		/// </summary>
		/// <param name="element">A ResultAttributeCollection instance</param>
		/// <returns>true to contibute visiting nested elements, false to stop</returns>
		public bool VisitResultAttributes(ResultAttributeCollection element)
		{
			return true;
		}

		/// <summary>
		/// Viste a referenced class Collection.
		/// </summary>
		/// <param name="element">A ReferencedClassCollection instance</param>
		/// <returns>true to contibute visiting nested elements, false to stop</returns>
		public bool VisitReferencedClasses(ReferencedClassCollection element)
		{
			return true;
		}

		/// <summary>
		/// Viste a binary expression.
		/// </summary>
		/// <param name="element">A BinaryExpr instance</param>
		/// <returns>true to contibute visiting nested elements, false to stop</returns>
		public bool VisitBinaryExpr(BinaryExpr element)
		{
			return true;
		}

		/// <summary>
		/// Viste a left parenthesis.
		/// </summary>
		/// <param name="element">A ParenthesizedExpr instance</param>
		/// <returns>true to contibute visiting nested elements, false to stop</returns>
		public bool VisitLeftParenthesis(ParenthesizedExpr element)
		{
			return true;
		}

		/// <summary>
		/// Start visite a DataRelationshipAttribute.
		/// </summary>
		/// <param name="element">A ParenthesizedExpr instance</param>
		/// <returns>true to contibute visiting nested elements, false to stop</returns>
		public bool VisitRelationshipBegin(DataRelationshipAttribute element)
		{
			return true;
		}

		/// <summary>
		/// End visite a DataRelationshipAttribute.
		/// </summary>
		/// <param name="element">A ParenthesizedExpr instance</param>
		/// <returns>true to contibute visiting nested elements, false to stop</returns>
		public bool VisitRelationshipEnd(DataRelationshipAttribute element)
		{
			return true;
		}

		/// <summary>
		/// Viste a right parenthesis.
		/// </summary>
		/// <param name="element">A ParenthesizedExpr instance</param>
		/// <returns>true to contibute visiting nested elements, false to stop</returns>
		public bool VisitRightParenthesis(ParenthesizedExpr element)
		{
			return true;
		}

		/// <summary>
		/// Viste a search parameter.
		/// </summary>
		/// <param name="element">A Parameter instance</param>
		/// <returns>true to contibute visiting nested elements, false to stop</returns>
		public bool VisitParameter(Parameter element)
		{
			SimpleAttributeElement schemaModelElement = (SimpleAttributeElement) element.GetSchemaModelElement();
			
			if (schemaModelElement != null && element.ParameterValue != null &&
				element.ParameterValue.Length > 0 &&
				schemaModelElement.DataType != element.DataType)
			{
				// if the attribute is multiple choice, the type of parameter is set to
				// string which is different from the attribute type
				if (!(schemaModelElement.IsMultipleChoice && element.DataType == DataType.String))
				{
                    string msg = string.Format(_resources.GetString("MismatchedDataType"), (_dataView != null ? _dataView.Caption : ""));
                    DataValidateResultEntry entry = new DataValidateResultEntry(msg, GetSource(_dataView), element);
					_result.AddError(entry);
				}
			}

			return true;
		}

		/// <summary>
		/// Visite a collection of parameters.
		/// </summary>
		/// <param name="element">A ParameterCollection instance</param>
		/// <returns>true to contibute visiting nested elements, false to stop</returns>		
		public bool VisitParametersBegin(ParameterCollection element)
		{
			return true;
		}

		/// <summary>
		/// End visite a collection of parameters.
		/// </summary>
		/// <param name="element">A ParameterCollection instance</param>
		/// <returns>true to contibute visiting nested elements, false to stop</returns>		
		public bool VisitParametersEnd(ParameterCollection element)
		{
			return true;
		}

		/// <summary>
		/// Visit a TaxonomyModel.
		/// </summary>
		/// <param name="element">A TaxonomyModel instance</param>
		/// <returns>true to contibute visiting nested elements, false to stop</returns>		
		public bool VisitTaxonomyModel(TaxonomyModel element)
		{
            _dataView = element.GetDataView(null);
            string msg;

			// is the class name still valid
			if (element.ClassName != null &&
				element.ClassName.Length > 0 &&
				element.MetaDataModel.SchemaModel.FindClass(element.ClassName) == null)
			{
                string className = "";
                if (!string.IsNullOrEmpty(element.ClassName))
                {
                    className = element.ClassName;
                }
                msg = string.Format(_resources.GetString("UnknownClass"), className);

				DataValidateResultEntry entry = new DataValidateResultEntry(msg, GetSource(element), element);
				_result.AddError(entry);

				return false;
			}

			// is the data view valid
			string dataViewName = element.DataViewName;
			if (dataViewName != null && dataViewName.Length > 0 &&
				element.MetaDataModel.DataViews[element.DataViewName] == null)
			{
                msg = string.Format(_resources.GetString("InvalidDataViewName"), element.Caption);
				DataValidateResultEntry entry = new DataValidateResultEntry(msg, GetSource(element), element);
				_result.AddError(entry);

				return false;
			}

			return true;
		}

		/// <summary>
		/// Visit a TaxonNode.
		/// </summary>
		/// <param name="element">A TaxonNode instance</param>
		/// <returns>true to contibute visiting nested elements, false to stop</returns>		
		public bool VisitTaxonNode(TaxonNode element)
		{
			_dataView = element.GetDataView(null);
			DataValidateResultEntry entry;
            string msg;

			// is class name a valid class
			string className = element.ClassName;
			if (className != null && className.Length > 0)
			{
				ClassElement classElement = element.MetaDataModel.SchemaModel.FindClass(className);

                if (classElement == null)
                {
                    msg = string.Format(_resources.GetString("UnknownClass"), className);
                    entry = new DataValidateResultEntry(msg, GetSource(element), element);
                    _result.AddError(entry);

                    return false;
                }
                else if (IsInheritedClassDefined(element))
                {
                    // The node has been locally associated with a class, therefore, it cannot
                    // inherite a dataview from the parent node as well
                    entry = new DataValidateResultEntry(_resources.GetString("InvalidInheritedDataView"), GetSource(element), element);
                    _result.AddError(entry);
                }
			}
			else
			{
                if (!string.IsNullOrEmpty(element.DataViewName))
                {
                    entry = new DataValidateResultEntry(_resources.GetString("BadDataViewName"), GetSource(element), element);
                    _result.AddError(entry);
                }

				// if it is a leaf node, make sure that it has a class name or dataview defined
				// or inherited
				if (element.ChildrenNodes.Count == 0)
				{
					if (!IsClassDefined(element) && !IsDataViewDefined(element))
					{
						entry = new DataValidateResultEntry(_resources.GetString("InvalidLeafNode"), GetSource(element), element);
						_result.AddError(entry);
					}
				}
			}

			return true;
		}

		/// <summary>
		/// Visit a SortBy.
		/// </summary>
		/// <param name="element">A SortBy instance</param>
		/// <returns>true to contibute visiting nested elements, false to stop</returns>		
		public bool VisitSortBy(SortBy element)
		{
			return false;
		}

        /// <summary>
        /// Visit a SortAttribute.
        /// </summary>
        /// <param name="element">A SortAttribute instance</param>
        /// <returns>true to contibute visiting nested elements, false to stop</returns>		
        public bool VisitSortAttribute(SortAttribute element)
        {
            return false;
        }

        /// <summary>
        /// Visit a function element.
        /// </summary>
        /// <param name="element">A function instance</param>
        /// <returns>true to contibute visiting nested elements, false to stop</returns>		
        public bool VisitFunction(IFunctionElement element)
        {
            return false;
        }

        /// <summary>
        /// Visit a AutoClassifyDef element.
        /// </summary>
        /// <param name="element">A AutoClassifyDef instance</param>
        /// <returns>true to contibute visiting nested elements, false to stop</returns>		
        public bool VisitClassifyDef(AutoClassifyDef element)
        {
            return false;
        }

        /// <summary>
        /// Visit a AutoClassifyLevel element.
        /// </summary>
        /// <param name="element">A AutoClassifyLevel instance</param>
        /// <returns>true to contibute visiting nested elements, false to stop</returns>		
        public bool VisitClassifyLevel(AutoClassifyLevel element)
        {
            return false;
        }

		/// <summary>
		/// Gets the source string
		/// </summary>
		/// <param name="element">The data view element</param>
		/// <returns>A source string</returns>
		private string GetSource(IDataViewElement element)
		{
			string source = "";

			switch (element.ElementType)
			{
				case ElementType.View:
					source = ((DataViewModel) element).Caption;
					break;

				case ElementType.Class:
					source = ((DataClass) element).ClassName;
					break;
				case ElementType.SimpleAttribute:
					source = element.Name;
					break;
				case ElementType.ArrayAttribute:
					source = element.Name;
					break;
                case ElementType.VirtualAttribute:
                    source = element.Name;
                    break;
                case ElementType.ImageAttribute:
                    source = element.Name;
                    break;
				case ElementType.RelationshipAttribute:
					source = element.Name;
					break;
				case ElementType.Taxonomy:
					source = element.Caption;
					break;
				case ElementType.TaxonNode:
					source = element.Caption;
					break;
			}

			return source;
		}

		/// <summary>
		/// Gets the information indicating whether the taxon node has an associated
		/// class.
		/// </summary>
		/// <param name="element">The given taxon node</param>
		/// <returns>true if it has an associated class, false otherwise.</returns>
		private bool IsClassDefined(TaxonNode element)
		{
			bool status = false;

			ITaxonomy currentNode = element;

			while (currentNode != null)
			{
				if (currentNode.ClassName != null && currentNode.ClassName.Length > 0)
				{
					status = true;
					break;
				}

				currentNode = currentNode.ParentNode;
			}

			return status;
		}

        /// <summary>
        /// Gets the information indicating whether any parent node of the given taxon node has an associated
        /// class.
        /// </summary>
        /// <param name="element">The given taxon node</param>
        /// <returns>true if it has an inherited class, false otherwise.</returns>
        private bool IsInheritedClassDefined(TaxonNode element)
        {
            bool status = false;

            ITaxonomy currentNode = element.ParentNode;

            while (currentNode != null)
            {
                if (currentNode.ClassName != null && currentNode.ClassName.Length > 0)
                {
                    status = true;
                    break;
                }

                currentNode = currentNode.ParentNode;
            }

            return status;
        }

		/// <summary>
		/// Gets the information indicating whether the taxon node has an associated
		/// data view.
		/// </summary>
		/// <param name="element">The given taxon node</param>
		/// <returns>true if it has an associated data view, false otherwise.</returns>
		private bool IsDataViewDefined(TaxonNode element)
		{
			bool status = false;

			ITaxonomy currentNode = element;

			while (currentNode != null)
			{
				if (currentNode.DataViewName != null && currentNode.DataViewName.Length > 0)
				{
					status = true;
					break;
				}

				currentNode = currentNode.ParentNode;
			}

			return status;
		}
	}
}