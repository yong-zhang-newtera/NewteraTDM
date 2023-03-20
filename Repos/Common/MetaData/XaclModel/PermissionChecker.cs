/*
* @(#)PermissionChecker.cs
*
* Copyright (c) 2003 Newtera, Inc. All rights reserved.
*
*/
namespace Newtera.Common.MetaData.XaclModel
{
	using System;
	using System.Collections;
	using System.Xml;

	using Newtera.Common.MetaData.Schema;
	using Newtera.Common.MetaData.XaclModel.Processor;

	/// <summary>
	/// The class mainly provides some utility methods for handling permission related stuff
	/// </summary>
	/// <version>  	1.0.0 11 Jul 2003</version>
	/// <author>  		Yong Zhang</author>
	public sealed class PermissionChecker
	{	
		private XaclProcessor _processor;

		/// <summary>
		/// Singleton's private instance.
		/// </summary>
		private static PermissionChecker theChecker;
		
		static PermissionChecker()
		{
			theChecker = new PermissionChecker();
		}

		/// <summary>
		/// The private constructor.
		/// </summary>
		private PermissionChecker()
		{
			_processor = new XaclProcessor();
		}

		/// <summary>
		/// Gets the PermissionChecker instance.
		/// </summary>
		/// <returns> The PermissionChecker instance.</returns>
		static public PermissionChecker Instance
		{
			get
			{
				return theChecker;
			}
		}
		
		/// <summary>
		/// Gets or sets the runner object that is responsible for running the 
		/// rules' conditions expressed in xquery
		/// </summary>
		/// <value>An IConditionRunner</value>
		public IConditionRunner ConditionRunner
		{
			get
			{
				return _processor.ConditionRunner;
			}
			set
			{
				_processor.ConditionRunner = value;
			}
		}

		/// <summary>
		/// Get information indicating whether there is a permission to
		/// take an action to an IXaclObject
		/// </summary>
		/// <param name="policy">the xacl policy model object</param>
		/// <param name="element">An IXaclObject to be evaluated</param>
		/// <param name="action">an action of request, there are 4 kind
		/// of them: read, write, create, delete, all of them defined in class
		/// XaclActionType.
		/// </param>
		/// <returns>true if access to the IXaclObject is allowed, otherwise false.</returns>
		public bool HasPermission(XaclPolicy policy, IXaclObject element, XaclActionType action)
		{
			return HasPermission(policy, element, action, null);
		}
		
		/// <summary>
		/// Get information indicating whether there is a permission to
		/// take an action to an XaclObject given an instance as context
		/// </summary>
		/// <param name="policy">the xacl policy model object</param>
		/// <param name="element">An IXaclObject to be accessed
		/// </param>
		/// <param name="action">an action of request, there are 4 kind
		/// of them: read, write, create, delete, all of them defined in class
		/// XaclActionType.
		/// </param>
		/// <param name="currentInstance">the current instance being evaluated.</param>
		/// <returns>true if access to the meta model element is allowed, otherwise false.</returns>
		public bool HasPermission(XaclPolicy policy, IXaclObject element, XaclActionType action, XmlElement currentInstance)
		{
            if (element != null)
            {
                bool hasPermission = HasPermissionInternal(policy, element, action, currentInstance);
                if (!hasPermission &&
                    policy.Setting.ConflictResolutionType == XaclConflictResolutionType.Gtp)
                {
                    // for Grant-take-preference setting,
                    // if user has permission to any of children, then he/she has permission to the parent
                    IEnumerator children = element.GetChildren();
                    while (children.MoveNext())
                    {
                        IXaclObject child = children.Current as IXaclObject;

                        if (child != null &&
                            HasPermission(policy, child, action, currentInstance))
                        {
                            hasPermission = true;
                            break;
                        }
                    }

                    return hasPermission;
                }

                return hasPermission;
            }
            else
            {
                return false;
            }
		}

		/// <summary>
		/// Get the flags indicating whether an user has permission to perform certain
		/// action(s) to an object
		/// </summary>
		/// <param name="policy">the xacl policy model object</param>
		/// <param name="element">An XaclObject which the actions will be performed to.
		/// </param>
		/// <param name="actions">actions requested</param>
		/// <param name="currentInstance">the current instance as a context for condition evaluation.</param>
		/// <returns>A combined flags of XaclPermissionFlag values.</returns>
		public XaclPermissionFlag GetPermissionFlags(XaclPolicy policy, IXaclObject element, XaclActionType actions, XmlElement currentInstance)
		{
			XaclPermissionFlag flags = 0;
		
			//TODO, get flags for all action types in one pass
			if ((actions & XaclActionType.Read) != 0)
			{			
				// send the request to the xacl processor and get a conclusion
                Conclusion conclusion = EvaluateInternal(policy, element, XaclActionType.Read, currentInstance);
				if (conclusion.Permission != XaclPermissionType.Deny)
				{
					flags = (flags | XaclPermissionFlag.GrantRead);
				}
			}

			if ((actions & XaclActionType.Write) != 0)
			{			
				// send the request to the xacl processor and get a conclusion
                Conclusion conclusion = EvaluateInternal(policy, element, XaclActionType.Write, currentInstance);

				if (conclusion.Permission != XaclPermissionType.Deny)
				{
					flags = (flags | XaclPermissionFlag.GrantWrite);
				}
			}

			if ((actions & XaclActionType.Create) != 0)
			{			
				// send the request to the xacl processor and get a conclusion
                Conclusion conclusion = EvaluateInternal(policy, element, XaclActionType.Create, currentInstance);

				if (conclusion.Permission != XaclPermissionType.Deny)
				{
					flags = (flags | XaclPermissionFlag.GrantCreate);
				}
			}

			if ((actions & XaclActionType.Delete) != 0)
			{			
				// send the request to the xacl processor and get a conclusion
                Conclusion conclusion = EvaluateInternal(policy, element, XaclActionType.Delete, currentInstance);

				if (conclusion.Permission != XaclPermissionType.Deny)
				{
					flags = (flags | XaclPermissionFlag.GrantDelete);
				}
			}

			if ((actions & XaclActionType.Upload) != 0)
			{			
				// send the request to the xacl processor and get a conclusion
                Conclusion conclusion = EvaluateInternal(policy, element, XaclActionType.Upload, currentInstance);

				if (conclusion.Permission != XaclPermissionType.Deny)
				{
					flags = (flags | XaclPermissionFlag.GrantUpload);
				}
			}

			if ((actions & XaclActionType.Download) != 0)
			{			
				// send the request to the xacl processor and get a conclusion
                Conclusion conclusion = EvaluateInternal(policy, element, XaclActionType.Download, currentInstance);

				if (conclusion.Permission != XaclPermissionType.Deny)
				{
					flags = (flags | XaclPermissionFlag.GrantDownload);
				}
			}
			
			return flags;
		}
		
		/// <summary>
		/// Gets a Conclusion object as the result of evaluating a access
		/// request against the policy.
		/// </summary>
		/// <param name="policy">the xacl policy model object
		/// </param>
		/// <param name="element">An IXaclObject to be accessed</param>
		/// <param name="action">an action of request, there are 4 kind
		/// of them: read, write, create, delete, all of them defined in class
		/// XaclActionType.
		/// </param>
		/// <returns> 
		/// An Conclusion object.
		/// </returns>
		public Conclusion GetConclusion(XaclPolicy policy, IXaclObject element, XaclActionType action)
		{
            Conclusion conclusion = GetConclusionInternal(policy, element, action);
            if (conclusion.Permission == XaclPermissionType.Deny &&
                policy.Setting.ConflictResolutionType == XaclConflictResolutionType.Gtp)
            {
                // for Grant-take-preference setting,
                // if user has permission to any of children, then he/she has permission to the parent
                IEnumerator children = element.GetChildren();
                while (children.MoveNext())
                {
                    IXaclObject child = children.Current as IXaclObject;

                    if (child != null)
                    {
                        conclusion = GetConclusion(policy, child, action);
                        if (conclusion.Permission != XaclPermissionType.Deny)
                        {
                            break;
                        }
                    }
                }
            }

            return conclusion;
		}

		/// <summary>
		/// Gets a combined condition which combines conditions in rules defined
		/// an IXaclObject. If there are multiple conditions existed, they are
		/// combined using logical and operators.
		/// </summary>
		/// <param name="policy">the xacl policy</param>
		/// <param name="element">An IXaclObject to be accessed</param>
		/// <returns> 
		/// A combined condition string.
		/// </returns>
		public string GetCondition(XaclPolicy policy, IXaclObject element)
		{
			string condition = null;
			XaclObject obj = new XaclObject(element.ToXPath());

			XaclRuleCollection rules = policy.GetLocalRules(obj);
			if (rules != null)
			{
				foreach (XaclRule rule in rules)
				{
					if (rule.Condition.Condition != null &&
						rule.Condition.Condition.Length > 0)
					{
						if (condition == null)
						{
							condition = rule.Condition.Condition;
						}
						else
						{
							condition += " and " + rule.Condition.Condition;
						}
					}
				}
			}

			return condition;
		}

        /// <summary>
        /// Get information indicating whether there is a permission to
        /// take an action to an XaclObject given an instance as context
        /// </summary>
        /// <param name="policy">the xacl policy model object</param>
        /// <param name="element">An IXaclObject to be accessed
        /// </param>
        /// <param name="action">an action of request, there are 4 kind
        /// of them: read, write, create, delete, all of them defined in class
        /// XaclActionType.
        /// </param>
        /// <param name="currentInstance">the current instance being evaluated.</param>
        /// <returns>true if access to the meta model element is allowed, otherwise false.</returns>
        private bool HasPermissionInternal(XaclPolicy policy, IXaclObject element, XaclActionType action, XmlElement currentInstance)
        {
            bool hasPermission = true;

            // build a access request object
            AccessRequestBuilder builder = AccessRequestBuilder.Instance;
            AccessRequest request = builder.BuildAccessRequest(element, action);

            // send the request to the xacl processor and get a conclusion
            Conclusion conclusion = _processor.Evaluate(request, policy, currentInstance);
            if (conclusion.Permission == XaclPermissionType.Deny)
            {
                hasPermission = false;
            }

            return hasPermission;
        }

        /// <summary>
        /// Gets a Conclusion object as the result of evaluating a access
        /// request against the policy.
        /// </summary>
        /// <param name="policy">the xacl policy model object
        /// </param>
        /// <param name="element">An IXaclObject to be accessed</param>
        /// <param name="action">an action of request, there are 4 kind
        /// of them: read, write, create, delete, all of them defined in class
        /// XaclActionType.
        /// </param>
        /// <returns> 
        /// An Conclusion object.
        /// </returns>
        private Conclusion GetConclusionInternal(XaclPolicy policy, IXaclObject element, XaclActionType action)
        {
            // build a access request object
            AccessRequestBuilder builder = AccessRequestBuilder.Instance;
            AccessRequest request = builder.BuildAccessRequest(element, action);

            // return the conclusion
            return _processor.Evaluate(request, policy);
        }

        // evaluate a request
        private Conclusion EvaluateInternal(XaclPolicy policy, IXaclObject element, XaclActionType action, XmlElement currentInstance)
        {
            // build a access request object
            AccessRequestBuilder builder = AccessRequestBuilder.Instance;
            AccessRequest request = builder.BuildAccessRequest(element, action);

            // return the conclusion
            Conclusion conclusion = _processor.Evaluate(request, policy, currentInstance);

            return conclusion;
        }
	}
}