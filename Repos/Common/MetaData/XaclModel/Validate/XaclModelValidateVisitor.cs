/*
* @(#)XaclModelValidateVisitor.cs
*
* Copyright (c) 2007 Newtera, Inc. All rights reserved.
*
*/
namespace Newtera.Common.MetaData.XaclModel.Validate
{
	using System;
	using System.Resources;

    using Newtera.Common.MetaData;
    using Newtera.Common.MetaData.Principal;
    using Newtera.Common.MetaData.Schema;
    using Newtera.Common.MetaData.Schema.Validate;
	using Newtera.Common.MetaData.XaclModel;

	/// <summary>
	/// Traverse a xacl model and validate each rule
	/// </summary>
	/// <version> 1.0.0 16 Apr 2007 </version>
    public class XaclModelValidateVisitor : IXaclNodeVisitor
	{
        private ValidateResult _result;
		private ResourceManager _resources;
        private MetaDataModel _metaData;
        private IUserManager _userManager;
        private string[] _users;
        private string[] _roles;
        private IMetaDataElement _metaDataElement;

		/// <summary>
		/// Instantiate an instance of XaclModelValidateVisitor class
		/// </summary>
        public XaclModelValidateVisitor(MetaDataModel metaData, IUserManager userManager, ValidateResult result)
		{
            _result = result;
			_resources = new ResourceManager(this.GetType());
            _metaData = metaData;
            _userManager = userManager;
            _users = null;
            _roles = null;
            _metaDataElement = null;
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
        /// Viste a policy element.
        /// </summary>
        /// <param name="element">A XaclPolicy instance</param>
        /// <returns>true to contibute visiting nested elements, false to stop</returns>
        public bool VisitXaclPolicy(XaclPolicy element)
        {
            return true;
        }

        /// <summary>
        /// Viste a XaclDefCollection element.
        /// </summary>
        /// <param name="element">A XaclDefCollection instance</param>
        /// <returns>true to contibute visiting nested elements, false to stop</returns>
        public bool VisitXaclDefCollection(XaclDefCollection element)
        {
            return true;
        }

        /// <summary>
        /// Viste a XaclDef element.
        /// </summary>
        /// <param name="element">A XaclDef instance</param>
        /// <returns>true to contibute visiting nested elements, false to stop</returns>
        public bool VisitXaclDef(XaclDef element)
        {
            string xpath = element.Object.Href; // keep track the object href of the rule

            // find the meta data element of the xpath
            _metaDataElement = _metaData.FindMetaModelElementByXPath(xpath);
            if (_metaDataElement == null)
            {
                // the schema model element to which the xacl definition is attached has been deleted
                // mark the xacl definition object as obsolete so that it will be removed later
                element.IsObsolete = true;

                return false;
            }
            else
            {
                return true;
            }
        }

        /// <summary>
        /// Viste a XaclObject element.
        /// </summary>
        /// <param name="element">A XaclObject instance</param>
        /// <returns>true to contibute visiting nested elements, false to stop</returns>
        public bool VisitXaclObject(XaclObject element)
        {
            return true;
        }

        /// <summary>
        /// Viste a XaclRuleCollection element.
        /// </summary>
        /// <param name="element">A XaclRuleCollection instance</param>
        /// <returns>true to contibute visiting nested elements, false to stop</returns>
        public bool VisitXaclRuleCollection(XaclRuleCollection element)
        {
            return true;
        }

        /// <summary>
        /// Viste a XaclRule element.
        /// </summary>
        /// <param name="element">A XaclRule instance</param>
        /// <returns>true to contibute visiting nested elements, false to stop</returns>		
        public bool VisitXaclRule(XaclRule element)
        {
            return true;
        }

        /// <summary>
        /// Viste XaclSubject element.
        /// </summary>
        /// <param name="element">A XaclSubject instance</param>
        /// <returns>true to contibute visiting nested elements, false to stop</returns>		
        public bool VisitXaclSubject(XaclSubject element)
        {
            ValidateResultEntry entry;
            string msg;

            if (_metaDataElement != null)
            {
                if (element.Uid != null && !IsUserExist(element.Uid))
                {
                    msg = String.Format(_resources.GetString("Xacl.UnknownUser"), element.Uid);
                    entry = new ValidateResultEntry(msg, GetSource(_metaDataElement), EntryType.Error, _metaDataElement);
                    _result.AddError(entry);
                }

                if (element.Roles != null)
                {
                    foreach (string role in element.Roles)
                    {
                        if (role != XaclSubject.EveryOne && !IsRoleExist(role))
                        {
                            msg = String.Format(_resources.GetString("Xacl.UnknownRole"), role);
                            entry = new ValidateResultEntry(msg, GetSource(_metaDataElement), EntryType.Error, _metaDataElement);
                            _result.AddError(entry);
                        }
                    }
                }
            }

            return true;
        }

        /// <summary>
        /// Viste XaclCondition element.
        /// </summary>
        /// <param name="element">A XaclCondition instance</param>
        /// <returns>true to contibute visiting nested elements, false to stop</returns>		
        public bool VisitXaclCondition(XaclCondition element)
        {
            return true;
        }

        /// <summary>
        /// Viste a XaclActionCollection element.
        /// </summary>
        /// <param name="element">A XaclActionCollection instance</param>
        /// <returns>true to contibute visiting nested elements, false to stop</returns>		
        public bool VisitXaclActionCollection(XaclActionCollection element)
        {
            return true;
        }

        /// <summary>
        /// Viste a XaclAction element.
        /// </summary>
        /// <param name="element">A XaclAction instance</param>
        /// <returns>true to contibute visiting nested elements, false to stop</returns>		
        public bool VisitXaclAction(XaclAction element)
        {
            return true;
        }

        /// <summary>
        /// Gets the information if the given user exists or not
        /// </summary>
        /// <param name="user"></param>
        /// <returns></returns>
        private bool IsUserExist(string user)
        {
            bool status = false;

            if (_users == null)
            {
                _users = _userManager.GetAllUsers();
            }

            if (user != XaclSubject.AnonymousUser)
            {
                if (_users != null)
                {
                    foreach (string usr in _users)
                    {
                        if (user == usr)
                        {
                            status = true;
                            break;
                        }
                    }
                }
            }
            else
            {
                status = true;
            }

            return status;
        }

        /// <summary>
        /// Gets the information if the given role exists or not
        /// </summary>
        /// <param name="role"></param>
        /// <returns></returns>
        private bool IsRoleExist(string role)
        {
            bool status = false;

            if (_roles == null)
            {
                _roles = _userManager.GetAllRoles();
            }

            if (role != XaclSubject.EveryOne)
            {
                if (_roles != null)
                {
                    foreach (string rl in _roles)
                    {
                        if (role == rl)
                        {
                            status = true;
                            break;
                        }
                    }
                }
            }
            else
            {
                status = true;
            }

            return status;
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