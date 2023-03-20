/*
* @(#)ApiModelValidateVisitor.cs
*
* Copyright (c) 2015 Newtera, Inc. All rights reserved.
*
*/
namespace Newtera.Common.MetaData.Api
{
	using System;
	using System.Resources;

    using Newtera.Common.MetaData;
    using Newtera.Common.MetaData.Principal;
    using Newtera.Common.MetaData.Schema;
    using Newtera.Common.MetaData.Schema.Validate;
	using Newtera.Common.MetaData.Api;
    using Newtera.Common.MetaData.Events;

	/// <summary>
	/// Traverse a api model and validate each api
	/// </summary>
	/// <version> 1.0.0 22 Sep 2013 </version>
    public class ApiModelValidateVisitor : IApiNodeVisitor
	{
        private ValidateResult _result;
		private ResourceManager _resources;
        private MetaDataModel _metaData;
        private IUserManager _userManager;
   
		/// <summary>
		/// Instantiate an instance of ApiModelValidateVisitor class
		/// </summary>
        public ApiModelValidateVisitor(MetaDataModel metaData, IUserManager userManager, ValidateResult result)
		{
            _result = result;
			_resources = new ResourceManager(this.GetType());
            _metaData = metaData;
            _userManager = userManager;
		}

		/// <summary>
		/// Gets the validate result.
		/// </summary>
		/// <value>The validate result in ValidateResult object</value>
		public ValidateResult ValidateResult
		{
			get
			{
				return _result;
			}
		}

        /// <summary>
        /// Viste a ApiManager element.
        /// </summary>
        /// <param name="element">A ApiManager instance</param>
        /// <returns>true to contibute visiting nested elements, false to stop</returns>
        public bool VisitApiManager(ApiManager element)
        {
            return true;
        }

        /// <summary>
        /// Viste an ApiGroupCollection element.
        /// </summary>
        /// <param name="element">A ApiGroupCollection instance</param>
        /// <returns>true to contibute visiting nested elements, false to stop</returns>
        public bool VisitApiGroupCollection(ApiGroupCollection element)
        {
            return true;
        }

        /// <summary>
        /// Viste an ApiGroup element.
        /// </summary>
        /// <param name="element">A ApiGroup instance</param>
        /// <returns>true to contibute visiting nested elements, false to stop</returns>
        public bool VisitApiGroup(ApiGroup element)
        {
            return true;
        }

        /// <summary>
        /// Viste an ApiCollection element.
        /// </summary>
        /// <param name="element">A ApiCollection instance</param>
        /// <returns>true to contibute visiting nested elements, false to stop</returns>
        public bool VisitApiCollection(ApiCollection element)
        {
            return true;
        }

        /// <summary>
        /// Viste a Api element.
        /// </summary>
        /// <param name="element">A Api instance</param>
        /// <returns>true to contibute visiting nested elements, false to stop</returns>		
        public bool VisitApi(Api element)
        {
            ValidateResultEntry entry;

            ClassElement classElement = _metaData.SchemaModel.FindClass(element.ClassName);

            if (!string.IsNullOrEmpty(element.ExternalHanlder))
            {
                // make sure the external handler exists or defined correctly
                entry = new ValidateResultEntry(element.Name + ":" + _resources.GetString("Api.InvalidExternalHandler"), MetaDataValidateHelper.Instance.GetSource(classElement), EntryType.Error, classElement, element);
                _result.AddDoubt(entry);
            }

            return true;
        }

        /// <summary>
        /// Gets the source string
        /// </summary>
        /// <param name="element">The schema model element</param>
        /// <returns>A source string</returns>
        private string GetSource(IMetaDataElement element)
        {
            string source = "";

            // get source msg 
            source = MetaDataValidateHelper.Instance.GetSource(element);

            return source;
        }
	}
}