/*
* @(#)EventModelValidateVisitor.cs
*
* Copyright (c) 2013 Newtera, Inc. All rights reserved.
*
*/
namespace Newtera.Common.MetaData.Events
{
	using System;
	using System.Resources;

    using Newtera.Common.MetaData;
    using Newtera.Common.MetaData.Principal;
    using Newtera.Common.MetaData.Schema;
    using Newtera.Common.MetaData.Schema.Validate;
	using Newtera.Common.MetaData.Events;

	/// <summary>
	/// Traverse an event model and validate each event.
	/// </summary>
	/// <version> 1.0.0 22 Sep 2013 </version>
    public class EventModelValidateVisitor : IEventNodeVisitor
	{
        private ValidateResult _result;
		private ResourceManager _resources;
        private MetaDataModel _metaData;
        private IUserManager _userManager;
   
		/// <summary>
		/// Instantiate an instance of EventModelValidateVisitor class
		/// </summary>
        public EventModelValidateVisitor(MetaDataModel metaData, IUserManager userManager, ValidateResult result)
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
        /// Viste a EventManager element.
        /// </summary>
        /// <param name="element">A EventManager instance</param>
        /// <returns>true to contibute visiting nested elements, false to stop</returns>
        public bool VisitEventManager(EventManager element)
        {
            return true;
        }

        /// <summary>
        /// Viste an EventGroupCollection element.
        /// </summary>
        /// <param name="element">A EventGroupCollection instance</param>
        /// <returns>true to contibute visiting nested elements, false to stop</returns>
        public bool VisitEventGroupCollection(EventGroupCollection element)
        {
            return true;
        }

        /// <summary>
        /// Viste an EventGroup element.
        /// </summary>
        /// <param name="element">A EventGroup instance</param>
        /// <returns>true to contibute visiting nested elements, false to stop</returns>
        public bool VisitEventGroup(EventGroup element)
        {
            return true;
        }

        /// <summary>
        /// Viste an EventCollection element.
        /// </summary>
        /// <param name="element">A EventCollection instance</param>
        /// <returns>true to contibute visiting nested elements, false to stop</returns>
        public bool VisitEventCollection(EventCollection element)
        {
            return true;
        }

        /// <summary>
        /// Viste a EventDef element.
        /// </summary>
        /// <param name="element">A EventDef instance</param>
        /// <returns>true to contibute visiting nested elements, false to stop</returns>		
        public bool VisitEventDef(EventDef element)
        {
            ValidateResultEntry entry;

            ClassElement classElement = _metaData.SchemaModel.FindClass(element.ClassName);

            if (element.OperationType == OperationType.Timer &&
                string.IsNullOrEmpty(element.TimerCondition))
            {
                entry = new ValidateResultEntry(element.Name + ":" +  _resources.GetString("Event.EmptyCondition"), MetaDataValidateHelper.Instance.GetSource(classElement), EntryType.Error, classElement, element);
                _result.AddError(entry);
            }

            if (element.OperationType == OperationType.Timer &&
                !string.IsNullOrEmpty(element.TimerCondition))
            {
                // make sure the condition defined correctly
                entry = new ValidateResultEntry(element.Name + ":" +  _resources.GetString("Subscriber.InvalidTimerCondition"), MetaDataValidateHelper.Instance.GetSource(classElement), EntryType.Error, classElement, element);
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