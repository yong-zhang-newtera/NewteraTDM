/*
* @(#)SubscriberModelValidateVisitor.cs
*
* Copyright (c) 2013 Newtera, Inc. All rights reserved.
*
*/
namespace Newtera.Common.MetaData.Subscribers
{
	using System;
	using System.Resources;

    using Newtera.Common.MetaData;
    using Newtera.Common.MetaData.Principal;
    using Newtera.Common.MetaData.Schema;
    using Newtera.Common.MetaData.Schema.Validate;
	using Newtera.Common.MetaData.Subscribers;
    using Newtera.Common.MetaData.Events;

	/// <summary>
	/// Traverse a subscriber model and validate each subscriber
	/// </summary>
	/// <version> 1.0.0 22 Sep 2013 </version>
    public class SubscriberModelValidateVisitor : ISubscriberNodeVisitor
	{
        private ValidateResult _result;
		private ResourceManager _resources;
        private MetaDataModel _metaData;
        private IUserManager _userManager;
   
		/// <summary>
		/// Instantiate an instance of SubscriberModelValidateVisitor class
		/// </summary>
        public SubscriberModelValidateVisitor(MetaDataModel metaData, IUserManager userManager, ValidateResult result)
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
        /// Viste a SubscriberManager element.
        /// </summary>
        /// <param name="element">A SubscriberManager instance</param>
        /// <returns>true to contibute visiting nested elements, false to stop</returns>
        public bool VisitSubscriberManager(SubscriberManager element)
        {
            return true;
        }

        /// <summary>
        /// Viste an SubscriberGroupCollection element.
        /// </summary>
        /// <param name="element">A SubscriberGroupCollection instance</param>
        /// <returns>true to contibute visiting nested elements, false to stop</returns>
        public bool VisitSubscriberGroupCollection(SubscriberGroupCollection element)
        {
            return true;
        }

        /// <summary>
        /// Viste an SubscriberGroup element.
        /// </summary>
        /// <param name="element">A SubscriberGroup instance</param>
        /// <returns>true to contibute visiting nested elements, false to stop</returns>
        public bool VisitSubscriberGroup(SubscriberGroup element)
        {
            return true;
        }

        /// <summary>
        /// Viste an SubscriberCollection element.
        /// </summary>
        /// <param name="element">A SubscriberCollection instance</param>
        /// <returns>true to contibute visiting nested elements, false to stop</returns>
        public bool VisitSubscriberCollection(SubscriberCollection element)
        {
            return true;
        }

        /// <summary>
        /// Viste a Subscriber element.
        /// </summary>
        /// <param name="element">A Subscriber instance</param>
        /// <returns>true to contibute visiting nested elements, false to stop</returns>		
        public bool VisitSubscriber(Subscriber element)
        {
            ValidateResultEntry entry;

            ClassElement classElement = _metaData.SchemaModel.FindClass(element.ClassName);

            if (string.IsNullOrEmpty(element.EventName))
            {
                entry = new ValidateResultEntry(element.Name + _resources.GetString("Subscriber.EmptyEventName"), MetaDataValidateHelper.Instance.GetSource(classElement), EntryType.Error, classElement,element);
                _result.AddError(entry);
            }
            else
            {
                EventCollection events = _metaData.EventManager.GetClassEvents(classElement);

                bool exist = false;
                foreach (EventDef eventDef in events)
                {
                    if (eventDef.Name == element.EventName)
                    {
                        exist = true;
                        break;
                    }
                }

                if (!exist)
                {
                    //entry = new ValidateResultEntry(element.Name + ":" + element.EventName + _resources.GetString("Subscriber.InvalidEventName") , MetaDataValidateHelper.Instance.GetSource(classElement), EntryType.Error, classElement, element);
                    //_result.AddError(entry);
                }
            }

            if (!string.IsNullOrEmpty(element.ExternalHanlder))
            {
                // make sure the external handler exists or defined correctly
                entry = new ValidateResultEntry(element.Name + ":" + _resources.GetString("Subscriber.InvalidExternalHandler"), MetaDataValidateHelper.Instance.GetSource(classElement), EntryType.Error, classElement, element);
                _result.AddDoubt(entry);
            }

            if (element.SendEmail)
            {
                if (string.IsNullOrEmpty(element.Subject) || string.IsNullOrEmpty(element.Description))
                {
                    entry = new ValidateResultEntry(element.Name + ":" + _resources.GetString("Subscriber.EmptySubjectOrBody"), MetaDataValidateHelper.Instance.GetSource(classElement), EntryType.Error, classElement,element);
                    _result.AddError(entry);
                }
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