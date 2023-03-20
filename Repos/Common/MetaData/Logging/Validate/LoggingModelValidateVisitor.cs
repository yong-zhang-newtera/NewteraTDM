/*
* @(#)LoggingModelValidateVisitor.cs
*
* Copyright (c) 2009 Newtera, Inc. All rights reserved.
*
*/
namespace Newtera.Common.MetaData.Logging.Validate
{
	using System;
	using System.Resources;

    using Newtera.Common.MetaData;
    using Newtera.Common.MetaData.Principal;
    using Newtera.Common.MetaData.Schema;
    using Newtera.Common.MetaData.Schema.Validate;
	using Newtera.Common.MetaData.Logging;

	/// <summary>
	/// Traverse a logging model and validate each rule
	/// </summary>
	/// <version> 1.0.0 04 Jan 2009 </version>
    public class LoggingModelValidateVisitor : ILoggingNodeVisitor
	{
        private ValidateResult _result;
		private ResourceManager _resources;
        private MetaDataModel _metaData;
        private IUserManager _userManager;
        private string[] _users;
        private string[] _roles;
        private IMetaDataElement _metaDataElement;

		/// <summary>
		/// Instantiate an instance of LoggingModelValidateVisitor class
		/// </summary>
        public LoggingModelValidateVisitor(MetaDataModel metaData, IUserManager userManager, ValidateResult result)
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
        /// <param name="element">A LoggingPolicy instance</param>
        /// <returns>true to contibute visiting nested elements, false to stop</returns>
        public bool VisitLoggingPolicy(LoggingPolicy element)
        {
            return true;
        }

        /// <summary>
        /// Viste a LoggingDefCollection element.
        /// </summary>
        /// <param name="element">A LoggingDefCollection instance</param>
        /// <returns>true to contibute visiting nested elements, false to stop</returns>
        public bool VisitLoggingDefCollection(LoggingDefCollection element)
        {
            return true;
        }

        /// <summary>
        /// Viste a LoggingDef element.
        /// </summary>
        /// <param name="element">A LoggingDef instance</param>
        /// <returns>true to contibute visiting nested elements, false to stop</returns>
        public bool VisitLoggingDef(LoggingDef element)
        {
            string xpath = element.Object.Href; // keep track the object href of the rule

            // find the meta data element of the xpath
            _metaDataElement = _metaData.FindMetaModelElementByXPath(xpath);
            if (_metaDataElement == null)
            {
                // the schema model element to which the logging definition is attached has been deleted
                // mark the logging definition object as obsolete so that it will be removed later
                element.IsObsolete = true;

                return false;
            }
            else
            {
                return true;
            }
        }

        /// <summary>
        /// Viste a LoggingObject element.
        /// </summary>
        /// <param name="element">A LoggingObject instance</param>
        /// <returns>true to contibute visiting nested elements, false to stop</returns>
        public bool VisitLoggingObject(LoggingObject element)
        {
            return true;
        }

        /// <summary>
        /// Viste a LoggingRuleCollection element.
        /// </summary>
        /// <param name="element">A LoggingRuleCollection instance</param>
        /// <returns>true to contibute visiting nested elements, false to stop</returns>
        public bool VisitLoggingRuleCollection(LoggingRuleCollection element)
        {
            return true;
        }

        /// <summary>
        /// Viste a LoggingRule element.
        /// </summary>
        /// <param name="element">A LoggingRule instance</param>
        /// <returns>true to contibute visiting nested elements, false to stop</returns>		
        public bool VisitLoggingRule(LoggingRule element)
        {
            return true;
        }

        /// <summary>
        /// Viste LoggingSubject element.
        /// </summary>
        /// <param name="element">A LoggingSubject instance</param>
        /// <returns>true to contibute visiting nested elements, false to stop</returns>		
        public bool VisitLoggingSubject(LoggingSubject element)
        {
            ValidateResultEntry entry;
            string msg;

            if (_metaDataElement != null)
            {
                if (element.Uid != null && !IsUserExist(element.Uid))
                {
                    msg = String.Format(_resources.GetString("Logging.UnknownUser"), element.Uid);
                    entry = new ValidateResultEntry(msg, GetSource(_metaDataElement), EntryType.Error, _metaDataElement);
                    _result.AddError(entry);
                }

                if (element.Roles != null)
                {
                    foreach (string role in element.Roles)
                    {
                        if (!IsRoleExist(role))
                        {
                            msg = String.Format(_resources.GetString("Logging.UnknownRole"), role);
                            entry = new ValidateResultEntry(msg, GetSource(_metaDataElement), EntryType.Error, _metaDataElement);
                            _result.AddError(entry);
                        }
                    }
                }
            }

            return true;
        }

        /// <summary>
        /// Viste a LoggingActionCollection element.
        /// </summary>
        /// <param name="element">A LoggingActionCollection instance</param>
        /// <returns>true to contibute visiting nested elements, false to stop</returns>		
        public bool VisitLoggingActionCollection(LoggingActionCollection element)
        {
            return true;
        }

        /// <summary>
        /// Viste a LoggingAction element.
        /// </summary>
        /// <param name="element">A LoggingAction instance</param>
        /// <returns>true to contibute visiting nested elements, false to stop</returns>		
        public bool VisitLoggingAction(LoggingAction element)
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

            if (user != LoggingSubject.AnonymousUser)
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

            if (role != LoggingSubject.EveryOne)
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