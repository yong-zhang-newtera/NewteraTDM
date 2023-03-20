/*
* @(#)MetaDataValidateHelper.cs
*
* Copyright (c) 2007 Newtera, Inc. All rights reserved.
*
*/
namespace Newtera.Common.MetaData
{
	using System;
	using System.Resources;

    using Newtera.Common.MetaData;
    using Newtera.Common.MetaData.Schema;
    using Newtera.Common.MetaData.DataView;
    using Newtera.Common.MetaData.DataView.Taxonomy;

	/// <summary>
	/// Traverse a schema model and validate each element to check if it confirms
	/// to some schema model integrity rules.
	/// </summary>
	/// <version> 1.0.0 14 June 2007 </version>
	public class MetaDataValidateHelper
	{
		private ResourceManager _resources;

        // Static factory object, all invokers will use this factory object.
		private static MetaDataValidateHelper theHelper;
		
		/// <summary>
		/// Private constructor.
		/// </summary>
        private MetaDataValidateHelper()
		{
		}

        /// <summary>
        /// Gets the MetaDataValidateHelper instance.
        /// </summary>
        /// <returns> The MetaDataValidateHelper instance.</returns>
        static public MetaDataValidateHelper Instance
        {
            get
            {
                return theHelper;
            }
        }

		/// <summary>
		/// Gets the source string
		/// </summary>
		/// <param name="element">The schema model element</param>
		/// <returns>A source string</returns>
		public string GetSource(IMetaDataElement element)
		{
            if (_resources == null)
            {
                _resources = new ResourceManager(this.GetType());
            }

			string source = "";

			if (element is ClassElement)
			{
				source = _resources.GetString("Class.Text") + element.Caption;
			}
			else if (element is SimpleAttributeElement)
			{
				source = _resources.GetString("SimpleAttribute.Text") + ((SimpleAttributeElement) element).OwnerClass.Caption + "->" + element.Caption;
			}
			else if (element is RelationshipAttributeElement)
			{
				source = _resources.GetString("RelationshipAttribute.Text") + ((RelationshipAttributeElement) element).OwnerClass.Caption + "->" + element.Caption;
			}
			else if (element is ArrayAttributeElement)
			{
				source = _resources.GetString("ArrayAttribute.Text") + ((ArrayAttributeElement) element).OwnerClass.Caption + "->" + element.Caption;
			}
            else if (element is VirtualAttributeElement)
            {
                source = _resources.GetString("VirtualAttribute.Text") + ((VirtualAttributeElement)element).OwnerClass.Caption + "->" + element.Caption;
            }
			else if (element is EnumElement)
			{
				source = _resources.GetString("EnumConstraint.Text") + element.Caption;
			}
			else if (element is RangeElement)
			{
				source = _resources.GetString("RangeConstraint.Text") + element.Caption;
			}
			else if (element is PatternElement)
			{
				source = _resources.GetString("PatternConstraint.Text") + element.Caption;
			}
			else if (element is ListElement)
			{
				source = _resources.GetString("ListConstraint.Text") + element.Caption;
			}
            else if (element is TaxonomyModel)
            {
                source = _resources.GetString("TaxonomyModel.Text") + element.Caption;
            }
            else if (element is TaxonNode)
            {
                source = _resources.GetString("TaxonNode.Text") + element.Caption;
            }
            else if (element is DataViewModel)
            {
                source = _resources.GetString("DataViewModel.Text") + element.Caption;
            }

			return source;
		}

        static MetaDataValidateHelper()
		{
			// Initializing the helper.
			{
                theHelper = new MetaDataValidateHelper();
			}
		}
	}
}