/*
* @(#)ForEachActivity.cs
*
* Copyright (c) 2007 Newtera, Inc. All rights reserved.
*
*/

using System;
using System.Collections;
using System.ComponentModel;
using System.ComponentModel.Design;
using System.Drawing.Design;
using System.Drawing;
using System.Workflow.ComponentModel;
using System.Workflow.ComponentModel.Design;
using System.Workflow.ComponentModel.Compiler;

using Newtera.Common.Core;
using Newtera.Common.MetaData;
using Newtera.WorkflowServices;
using Newtera.WFModel;

namespace Newtera.Activities
{
    #region ExecutionType

    public enum ExecutionType
    {
        /// <summary>
        /// Executes activities in sequential order. Each activity is executed in turn, after the previous activity has finished running. 
        /// </summary>
        Sequence,
        /// <summary>
        /// Executes activities in parallel
        /// </summary>
        Parallel
    }
    #endregion

    [ToolboxBitmapAttribute(typeof(ForEachActivity), "Resources.ForEach.png")]
    [ActivityValidator(typeof(ForEachValidator))]
    [DesignerAttribute(typeof(ForEachDesigner), typeof(IDesigner))]
    public sealed partial class ForEachActivity : CompositeActivity, ICustomTypeDescriptor
    {
        // Define dependency property objects for all properties and events of this activity.
        public static readonly DependencyProperty ExecutionTypeProperty = DependencyProperty.Register("ExecutionType", typeof(ExecutionType), typeof(ForEachActivity));
        public static readonly DependencyProperty ItemsProperty = DependencyProperty.Register("Items", typeof(IEnumerable), typeof(ForEachActivity));
        public static readonly DependencyProperty ItemsBindingProperty = DependencyProperty.Register("ItemsBinding", typeof(ParameterBindingInfo), typeof(ForEachActivity));
        private static readonly DependencyProperty EnumeratorProperty = DependencyProperty.Register("Enumerator", typeof(IEnumerator), typeof(ForEachActivity));
        public static readonly DependencyProperty DataItemProperty = DependencyProperty.RegisterAttached("DataItem", typeof(object), typeof(ForEachActivity));

        // Define private constants for the Validation Errors 
        private const int UnknownAssignment = 1;

        [NonSerialized]
        private PropertyDescriptorCollection _globalizedProperties = null;

        public ForEachActivity()
        {
        }

        #region Public Properties

        [
            CategoryAttribute("Activity"),
            DefaultValueAttribute(ExecutionType.Sequence),
            DescriptionAttribute("Gets or sets the ExecutionType for the ForEachActivity."),
        ]
        public ExecutionType ExecutionType
        {
            get
            {
                return (ExecutionType) base.GetValue(ExecutionTypeProperty);
            }
            set
            {
                base.SetValue(ExecutionTypeProperty, value);
            }
        }

        [DescriptionAttribute("The binding info to Items property of the ForEachActivity.")]
        [CategoryAttribute("Activity")]
        [EditorAttribute("WorkflowStudio.ItemsBindingEditor, WorkflowStudio", typeof(UITypeEditor))]
        [TypeConverterAttribute("WorkflowStudio.ItemsBindingConverter, WorkflowStudio")]
        public ParameterBindingInfo ItemsBinding
        {
            get
            {
                return base.GetValue(ItemsBindingProperty) as ParameterBindingInfo;
            }
            set
            {
                base.SetValue(ItemsBindingProperty, value);
            }
        }

        [Browsable(false)]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public System.Collections.IEnumerable Items
        {
            get
            {
                return base.GetValue(ItemsProperty) as IEnumerable;
            }
            set
            {
                base.SetValue(ItemsProperty, value);
            }
        }

        // This is a read-only property that returns the data item of current iteration saved in
        // the dynamic child.
        [Browsable(false)]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public object CurrentItem
        {
            get
            {
                object dataItem = "服务器";

                try
                {
                    if (!this.DesignMode)
                    {
                        if (this.DynamicActivity != null)
                        {
                            dataItem = this.DynamicActivity.GetValue(ForEachActivity.DataItemProperty);
                        }
                    }
                }
                catch (Exception)
                {
                }
                finally
                {
                    if (dataItem == null)
                    {
                        dataItem = "服务器";
                    }
                }

                return dataItem;
            }
        }

        #endregion

        #region Private Properties

        // This is a private property whose value can be serialized at runtime.  
        // The object that implements the IEnumerator interface must be Serializable 
        // and be able to maintaine its state between serialization/deserialization.
        [Browsable(false)]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        private System.Collections.IEnumerator Enumerator
        {
            get
            {
                return (IEnumerator)base.GetValue(EnumeratorProperty);
            }
            set
            {
                base.SetValue(EnumeratorProperty, value);
            }
        }

        // This is a read-only property that returns the current running child activity at runtime.
        // For each iteration, a new child activity instance is created. Instances created during
        // ealier iterations will have their Status remain as Closed.
        [Browsable(false)]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public System.Workflow.ComponentModel.Activity DynamicActivity
        {
            get
            {
                if (this.DesignMode)
                    return null;

                if (this.EnabledActivities.Count > 0)
                {
                    Activity[] dynamicChildren = this.GetDynamicActivities(this.EnabledActivities[0]);
                    if (dynamicChildren.Length != 0)
                        return dynamicChildren[0];
                }
                return null;
            }
        }

        #endregion

        #region Activity Overrides

        /// <summary>
        ///	This is the main execution logic for the ForEachActivity Activity.  The activity and its
        /// child is executed once for each item in the collection.
        /// </summary>
        /// <param name="context"></param>
        /// <returns></returns>
        protected override ActivityExecutionStatus Execute(ActivityExecutionContext context)
        {
            try
            {
                ActivityExecutionStatus executionStatus;

                if (context == null)
                    throw new ArgumentNullException("activity execution context is null.");

                if (this.Items == null &&
                    this.ItemsBinding != null)
                {
                    // initialize the Items property
                    INewteraWorkflow rootActivity = ActivityUtil.GetRootActivity(this);
                    object val = ActivityUtil.GetParameterValue(this.ItemsBinding, rootActivity, null);
 
                    if (val != null)
                    {
                        if (val is IEnumerable)
                        {
                            this.Items = (IEnumerable)val;
                        }
                        else
                        {
                            // make a dummy collection
                            object[] vals = new object[1];
                            vals[0] = val;
                            this.Items = (IEnumerable)vals;
                        }
                    }
                    else
                    {
                        throw new Exception("Unable to find a value for the Items property of the activity " + this.Name);
                    }
                }

                if (this.Items != null)
                {
                    this.Enumerator = this.Items.GetEnumerator();

                    if (this.ExecutionType == ExecutionType.Parallel)
                    {
                        // execute the child activity in parallel,
                        // For each iteration, a new child activity instance is created.
                        // ForEach activity isn't closed untill all child activities are closed
                        while (true)
                        {
                            if (!ExecuteNext(context))
                            {
                                break;
                            }
                        }

                        // since workflow runtime uses a single thread to execute child activities,
                        // the child activities won't start execution until this method returns, so it's
                        // safe to return Executing status
                        executionStatus = ActivityExecutionStatus.Executing;
                    }
                    else
                    {
                        // The default behavious is sequential execution, which executes the child activity once
                        // for each item contained in the Items collection.
                        // If the return value is false, we're at the end of the collection, activity is closed,
                        // otherwise, activity is executing.
                        if (ExecuteNext(context))
                        {
                            executionStatus = ActivityExecutionStatus.Executing;
                        }
                        else
                        {
                            executionStatus = ActivityExecutionStatus.Closed;
                        }
                    }
                }
                else
                {
                    throw new Exception("The Items property of ForEachActivity " + this.Name + " has not been initialized.");
                }

                return executionStatus;
            }
            catch (Exception ex)
            {
                ErrorLog.Instance.WriteLine(ex.Message + "\n" + ex.StackTrace);
                throw ex;
            }
        }


        /// <summary>
        ///	This override function cancels the execution of the child activity if cancel is called
        /// on the ForEachActivity activity itself.
        /// </summary>
        /// <param name="context"></param>
        /// <returns></returns>
        protected override ActivityExecutionStatus Cancel(ActivityExecutionContext context)
        {
            try
            {
                if (context == null)
                    throw new ArgumentNullException("activity execution context is null.");

                // If there is no child activity, the ForEachActivity activity is closed.
                if (this.EnabledActivities.Count == 0)
                    return ActivityExecutionStatus.Closed;

                Activity childActivity = this.EnabledActivities[0];
                ActivityExecutionContext childContext =
                    context.ExecutionContextManager.GetExecutionContext(childActivity);

                if (childContext != null)
                {
                    Activity[] dynamicChildren = this.GetDynamicActivities(childActivity);
                    foreach (Activity dynamicChildActivity in dynamicChildren)
                    {
                        if (dynamicChildActivity.ExecutionStatus == ActivityExecutionStatus.Executing)
                        {
                            // Cancel the executing child activity.
                            childContext.CancelActivity(dynamicChildActivity);
                        }
                    }

                    return ActivityExecutionStatus.Canceling;
                }
                return ActivityExecutionStatus.Closed;
            }
            catch (Exception ex)
            {
                ErrorLog.Instance.WriteLine(ex.Message + "\n" + ex.StackTrace);
                throw ex;
            }
        }


        protected override void OnClosed(IServiceProvider prov)
        {
            // We're done, clean up the private instance properties.
            // We do this to minimize runtime serialization overhead.
            base.RemoveProperty(EnumeratorProperty);
            base.OnClosed(prov);
        }

        #endregion

        #region Private Functions

        // This function executes the ForEachActivity activity.  It advances the current index of the collection.
        // If the end of the collection is reached, return false.  Otherwise, it executes any child activity
        // and return true.
        private bool ExecuteNext(ActivityExecutionContext context)
        {
            // First, move to the next position.
            if (!this.Enumerator.MoveNext())
                return false;

            // Execute the child activity.
            if (this.EnabledActivities.Count > 0)
            {
                // Add the child activity to the execution context and setup the event handler to 
                // listen to the child Close event.
                // A new instance of the child activity is created for each iteration.
                ActivityExecutionContext innerContext = 
                    context.ExecutionContextManager.CreateExecutionContext(this.EnabledActivities[0]);
                innerContext.Activity.Closed += this.OnChildClose;

                // Keep the current data item in the dynamic child activity
                innerContext.Activity.SetValue(ForEachActivity.DataItemProperty, this.Enumerator.Current);

                // Execute the child activity again.  
                innerContext.ExecuteActivity(innerContext.Activity);

                if (TraceLog.Instance.Enabled)
                {
                    string[] messages = {"ForEachActivity execute a child activity with iteration value of : " +  this.Enumerator.Current,
                            "Activity Name: " + this.Name,
                            "Workflow Instance Id: " + this.WorkflowInstanceId.ToString()};
                    TraceLog.Instance.WriteLines(messages);
                }
            }
            else
            {
                // an empty foreach loop. 
                // If the ForEachActivity activity is still executing, then execute the next one.
                if (this.ExecutionStatus == ActivityExecutionStatus.Executing)
                {
                    if (!ExecuteNext(context))
                        context.CloseActivity();
                }
            }
            return true;
        }


        // When a child activity is closed, it's removed from the execution context,
        private void OnChildClose(Object sender, ActivityExecutionStatusChangedEventArgs e)
        {
            if (e == null)
                throw new ArgumentNullException("OnChildClose parameter 'e' is null.");
            if (sender == null)
                throw new ArgumentNullException("OnChildClose parameter 'sender' is null.");

            ActivityExecutionContext context = sender as ActivityExecutionContext;

            if (context == null)
                throw new ArgumentException("OnChildClose parameter 'sender' is not ActivityExecutionContext.");

            ForEachActivity foreachActivity = context.Activity as ForEachActivity;

            if (foreachActivity == null)
                throw new ArgumentException("OnChildClose parameter 'sender' does not contain a 'ForEachActivity' activity.");

            // Remove the event handler first from the dynamic child activity.
            e.Activity.Closed -= this.OnChildClose;

            // Then remove the child activity from the execution context.
            context.ExecutionContextManager.CompleteExecutionContext(context.ExecutionContextManager.GetExecutionContext(e.Activity));

            if (this.ExecutionStatus == ActivityExecutionStatus.Canceling)
            {
                context.CloseActivity();
            }
            else if (this.ExecutionStatus == ActivityExecutionStatus.Executing)
            {
                if (this.ExecutionType == ExecutionType.Parallel)
                {
                    if (IsAllChildActivitiesClosed())
                    {
                        context.CloseActivity();
                    }
                }
                else
                {
                    // default
                    // Move on to the next iteration.
                    if (!ExecuteNext(context))
                    {
                        context.CloseActivity();
                    }
                }
            }
        }

        // return true if all child activities' status are closed, false otherwise.
        private bool IsAllChildActivitiesClosed()
        {
            bool status = true;

            if (this.EnabledActivities.Count > 0)
            {
                // get the dynamic child activities that are still executing
                Activity[] dynamicChildren = this.GetDynamicActivities(this.EnabledActivities[0]);
                if (dynamicChildren.Length > 0)
                {
                    // when a child activity is closed , it is removed from the dynamic child activity list
                    status = false;
                }
            }

            return status;
        }

        #endregion

        #region ICustomTypeDescriptor

        /// <summary>
        /// Refer to ICustomTypeDescriptor specification
        /// </summary>
        /// <returns></returns>
        public String GetClassName()
        {
            return TypeDescriptor.GetClassName(this, true);
        }

        /// <summary>
        /// Refer to ICustomTypeDescriptor specification
        /// </summary>
        /// <returns></returns>
        public AttributeCollection GetAttributes()
        {
            return TypeDescriptor.GetAttributes(this, true);
        }

        /// <summary>
        /// Refer to ICustomTypeDescriptor specification
        /// </summary>
        /// <returns></returns>
        public String GetComponentName()
        {
            return TypeDescriptor.GetComponentName(this, true);
        }

        /// <summary>
        /// Refer to ICustomTypeDescriptor specification
        /// </summary>
        /// <returns></returns>
        public TypeConverter GetConverter()
        {
            return TypeDescriptor.GetConverter(this, true);
        }

        /// <summary>
        /// Refer to ICustomTypeDescriptor specification
        /// </summary>
        /// <returns></returns>
        public EventDescriptor GetDefaultEvent()
        {
            return TypeDescriptor.GetDefaultEvent(this, true);
        }

        /// <summary>
        /// Refer to ICustomTypeDescriptor specification
        /// </summary>
        /// <returns></returns>
        public PropertyDescriptor GetDefaultProperty()
        {
            return TypeDescriptor.GetDefaultProperty(this, true);
        }

        /// <summary>
        /// Refer to ICustomTypeDescriptor specification
        /// </summary>
        /// <param name="editorBaseType"></param>
        /// <returns></returns>
        public object GetEditor(Type editorBaseType)
        {
            return TypeDescriptor.GetEditor(this, editorBaseType, true);
        }

        /// <summary>
        /// Refer to ICustomTypeDescriptor specification
        /// </summary>
        /// <param name="attributes"></param>
        /// <returns></returns>
        public EventDescriptorCollection GetEvents(Attribute[] attributes)
        {
            return TypeDescriptor.GetEvents(this, attributes, true);
        }

        /// <summary>
        /// Refer to ICustomTypeDescriptor specification
        /// </summary>
        /// <returns></returns>
        public EventDescriptorCollection GetEvents()
        {
            return TypeDescriptor.GetEvents(this, true);
        }

        /// <summary>
        /// Called to get the properties of a type.
        /// </summary>
        /// <param name="attributes"></param>
        /// <returns></returns>
        public PropertyDescriptorCollection GetProperties(Attribute[] attributes)
        {
            if (_globalizedProperties == null)
            {
                // Get the collection of properties
                PropertyDescriptorCollection baseProps = TypeDescriptor.GetProperties(this, attributes, true);

                _globalizedProperties = new PropertyDescriptorCollection(null);

                // For each property use a property descriptor of our own that is able to be globalized
                foreach (PropertyDescriptor property in baseProps)
                {
                    _globalizedProperties.Add(new GlobalizedPropertyDescriptor(property));
                }
            }

            return _globalizedProperties;
        }

        /// <summary>
        /// Our implementation overrides GetProperties() only and creates a
        /// collection of custom property descriptors of type GlobalizedPropertyDescriptor
        /// and returns them to the caller instead of the default ones.
        /// </summary>
        /// <returns>A collection of Property Descriptors.</returns>
        public PropertyDescriptorCollection GetProperties()
        {
            // Only do once
            if (_globalizedProperties == null)
            {
                // Get the collection of properties
                PropertyDescriptorCollection baseProperties = TypeDescriptor.GetProperties(this, true);

                _globalizedProperties = new PropertyDescriptorCollection(null);

                // For each property use a property descriptor of our own that is able to 
                // be globalized
                foreach (PropertyDescriptor property in baseProperties)
                {
                    // create our custom property descriptor and add it to the collection
                    _globalizedProperties.Add(new GlobalizedPropertyDescriptor(property));
                }
            }

            return _globalizedProperties;
        }

        /// <summary>
        /// Refer to ICustomTypeDescriptor specification
        /// </summary>
        /// <param name="pd"></param>
        /// <returns></returns>
        public object GetPropertyOwner(PropertyDescriptor pd)
        {
            return this;
        }

        #endregion

        #region ForEach Validatir
        internal sealed class ForEachValidator : CompositeActivityValidator
        {
            private const int InvalidNumberOfChildren = 100;

            /// <summary>
            ///	Overridden to validate the activity properties and populate the error collection.
            /// Only one child activity is allowed.  If multiple acitivties need to be executed,
            /// place them in a sequence or other appropriate composite activities.
            /// </summary>
            /// <param name="manager"></param>
            /// <param name="obj"></param>
            /// <returns></returns>
            public override ValidationErrorCollection Validate(ValidationManager manager, object obj)
            {
                ValidationErrorCollection validationErrors = new ValidationErrorCollection(base.Validate(manager, obj));

                ForEachActivity foreachActivity = obj as ForEachActivity;
                if (foreachActivity == null)
                {
                    throw new ArgumentException("Validate parameter 'obj' is not a ForEachActivity object.");
                }

                if (foreachActivity.EnabledActivities.Count > 1)
                {
                    validationErrors.Add(new ValidationError("Only one child is allowed in the ForEach activity.", InvalidNumberOfChildren));
                }

                if (foreachActivity.ItemsBinding == null)
                {
                    validationErrors.Add(ValidationError.GetNotSetValidationError(ForEachActivity.ItemsBindingProperty.Name));
                }
                else
                {
                    // make sure that ItemsBinding is still valid
                    if (foreachActivity.ItemsBinding.SourceType == SourceType.Unknown)
                    {
                        validationErrors.Add(new ValidationError("The ItemsBinding has an unknown binding source.", UnknownAssignment));
                    }
                    else
                    {
                        switch (foreachActivity.ItemsBinding.SourceType)
                        {
                            case SourceType.DataInstance:
                                if (!ActivityUtil.IsAttributeExist(foreachActivity.ItemsBinding.Path, foreachActivity))
                                {
                                    validationErrors.Add(new ValidationError("Unable to find the attribute with name " + foreachActivity.ItemsBinding.Path + " in the binding data class, make sure the data class hasn't been changed.", UnknownAssignment));
                                }
                                break;

                            case SourceType.Parameter:
                                if (!ActivityUtil.IsParameterExist(foreachActivity.ItemsBinding.Path, foreachActivity))
                                {
                                    validationErrors.Add(new ValidationError("Unable to find the parameter with name " + foreachActivity.ItemsBinding.Path + " among the input parameters of the workflow.", UnknownAssignment));
                                }
                                break;

                            case SourceType.Activity:
                                if (!ActivityUtil.IsActivityPropertyExist(foreachActivity.ItemsBinding.Path, foreachActivity))
                                {
                                    validationErrors.Add(new ValidationError("Unable to find the " + foreachActivity.ItemsBinding.Path + " as activity/property specification, make sure that the activity name hasn't been changed.", UnknownAssignment));
                                }
                                break;
                        }
                    }
                }

                return validationErrors;
            }
        }
        #endregion
    }
}
