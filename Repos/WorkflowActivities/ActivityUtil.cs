using System;
using System.Data;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using System.Workflow.ComponentModel;
using System.Reflection;
using System.Threading;

using Newtera.Common.Core;
using Newtera.WFModel;
using Newtera.Common.MetaData.DataView;
using Newtera.Common.MetaData.Principal;

namespace Newtera.Activities
{
    /// <summary>
    /// A utility class used by the activities
    /// </summary>
    public class ActivityUtil
    {
        public static INewteraWorkflow GetRootActivity(Activity activity)
        {
            INewteraWorkflow rootActivity = null;
            while (activity != null)
            {
                if (activity is INewteraWorkflow)
                {
                    rootActivity = (INewteraWorkflow)activity;
                    break;
                }

                activity = activity.Parent;
            }

            if (rootActivity == null)
            {
                throw new InvalidOperationException("Unable to find root activity of INewteraWorkflow");
            }

            return rootActivity;
        }

        /// <summary>
        /// Get the current user
        /// </summary>
        /// <returns>The display text of the current user</returns>
        public static string GetCurrentUser()
        {
            string userName = "Unknown";
            CustomPrincipal principal = null;

            principal = (CustomPrincipal)Thread.CurrentPrincipal;
            if (principal != null)
            {
                userName = principal.DisplayText;
            }

            return userName;
        }

        /// <summary>
        /// Gets the information indicating whether an attribute specified as binding source still exist
        /// </summary>
        /// <param name="attributeName"></param>
        /// <param name="activity"></param>
        /// <returns></returns>
        public static bool IsAttributeExist(string attributeName, Activity activity)
        {
            bool exist = true;

            INewteraWorkflow rootActivity = ActivityUtil.GetRootActivity(activity);
            if (!ActivityValidatingServiceProvider.Instance.ValidateService.IsValidAttributeName(rootActivity.SchemaId, rootActivity.ClassName, attributeName))
            {
                exist = false;
            }

            return exist;
        }

        /// <summary>
        /// gets the information indicating whether a parameter specified as binding source still exist
        /// </summary>
        /// <param name="parameterName"></param>
        /// <param name="activity"></param>
        /// <returns></returns>
        public static bool IsParameterExist(string parameterName, Activity activity)
        {
            bool exist = false;

            INewteraWorkflow rootActivity = ActivityUtil.GetRootActivity(activity);
            foreach (InputParameter parameter in rootActivity.InputParameters)
            {
                if (parameter.Name == parameterName)
                {
                    exist = true;
                    break;
                }
            }

            return exist;
        }

        /// <summary>
        /// gets the information indicating whether an activity property specified as binding source still exist
        /// </summary>
        /// <param name="path"></param>
        /// <param name="activity"></param>
        /// <returns></returns>
        public static bool IsActivityPropertyExist(string path, Activity activity)
        {
            bool exist = false;

            string activityName = ParameterBindingInfo.GetActivityName(path);
            string propertyName = ParameterBindingInfo.GetPropertyName(path);
            if (activityName != null && propertyName != null)
            {
                Activity rootActivity = (Activity)ActivityUtil.GetRootActivity(activity);
                Activity found = rootActivity.GetActivityByName(activityName);
                if (found != null)
                {
                    PropertyInfo[] propertyInfos = found.GetType().GetProperties();
                    foreach (PropertyInfo propertyInfo in propertyInfos)
                    {
                        if (propertyInfo.Name == propertyName)
                        {
                            exist = true;
                            break;
                        }
                    }
                }
            }

            return exist;
        }

        /// <summary>
        /// Gets the obj id of the first data instance from the dataset of specified data class
        /// </summary>
        /// <param name="ds">The data set</param>
        /// <param name="dataClassName">The specified data class</param>
        /// <returns>The obj id of the first data instance in the data set, null if the data set contains no instances</returns>
        public static string GetObjId(DataSet ds, string dataClassName)
        {
            string objId = null;

            if (!DataSetHelper.IsEmptyDataSet(ds, dataClassName))
            {
                // get the obj_id of the first data instance
                objId = DataSetHelper.GetCellValue(ds, dataClassName,
                    NewteraNameSpace.OBJ_ID, 0);
            }

            return objId;
        }

        /// <summary>
        /// Gets a value based on the specs in a parameter binding info
        /// </summary>
        /// <param name="bindingInfo">The parameter binding info</param>
        /// <param name="rootActivity">The Root Activity</param>
        /// <param name="invokingActivity">The activity that invokes the method</param>
        /// <returns>The parameter value</returns>
        public static object GetParameterValue(ParameterBindingInfo bindingInfo, INewteraWorkflow rootActivity, Activity invokingActivity)
        {
            object val = null;

            switch (bindingInfo.SourceType)
            {
                case SourceType.Activity:

                    val = GetActivityPropertyValue(bindingInfo.Path, rootActivity, invokingActivity);
                    break;

                case SourceType.DataInstance:

                    val = GetDataInstanceValue(bindingInfo.Path, rootActivity);

                    break;

                case SourceType.Parameter:

                    val = GetParameterValue(bindingInfo.Path, rootActivity);

                    break;
            }

            return val;
        }

        /// <summary>
        /// convert and return the DataType of a class attribute descriptor
        /// </summary>
        /// <param name="attribute"></param>
        /// <returns>System.Type</returns>
        public static Type GetClassAttributeType(InstanceAttributePropertyDescriptor property)
        {
            Type type = typeof(string);

            if (property != null)
            {
                type = property.PropertyType;
            }

            return type;
        }

        /// <summary>
        /// Get the DataType of a parameter
        /// </summary>
        /// <param name="attribute"></param>
        /// <returns>System.Type</returns>
        public static Type GetParameterType(InputParameter parameter)
        {
            Type type = typeof(string);

            if (parameter != null)
            {
                switch (parameter.DataType)
                {
                    case ParameterDataType.Boolean:
                        type = typeof(bool);
                        break;
                    case ParameterDataType.Date:
                        type = typeof(DateTime);
                        break;
                    case ParameterDataType.DateTime:
                        type = typeof(DateTime);
                        break;
                    case ParameterDataType.Decimal:
                        type = typeof(decimal);
                        break;
                    case ParameterDataType.Double:
                        type = typeof(double);
                        break;
                    case ParameterDataType.Float:
                        type = typeof(float);
                        break;
                    case ParameterDataType.Integer:
                        type = typeof(int);
                        break;
                    case ParameterDataType.BigInteger:
                        type = typeof(Int64);
                        break;
                    case ParameterDataType.String:
                        type = typeof(string);
                        break;
                    case ParameterDataType.Array:
                        type = typeof(Array);
                        break;
                }
            }

            return type;
        }

        /// <summary>
        /// Convert and return the Type of an activity property
        /// </summary>
        /// <param name="propertyInfo"></param>
        /// <returns>System.Type</returns>
        public static Type GetActivityPropertyType(PropertyInfo propertyInfo)
        {
            Type dataType = typeof(string);

            if (propertyInfo != null)
            {
                dataType = propertyInfo.PropertyType;
            }

            return dataType;
        }

        /// <summary>
        /// Gets information indicate whether a ParameterDataType type representing a target value can be converted form a Type representing a source value
        /// </summary>
        /// <param name="sourceType"></param>
        /// <param name="targetType"></param>
        /// <returns>true if it can be converted, false otherwise.</returns>
        public static bool CanConvertFrom(Type sourceType, ParameterDataType targetType)
        {
            bool status = false;

            switch (targetType)
            {
                case ParameterDataType.Boolean:
                    if (sourceType == typeof(bool))
                    {
                        status = true;
                    }
                    break;

                case ParameterDataType.BigInteger:
                    if (sourceType == typeof(Int64))
                    {
                        status = true;
                    }
                    break;
                case ParameterDataType.Date:
                case ParameterDataType.DateTime:
                    if (sourceType == typeof(DateTime))
                    {
                        status = true;
                    }
                    break;
                case ParameterDataType.Decimal:
                    if (sourceType == typeof(decimal))
                    {
                        status = true;
                    }
                    break;
                case ParameterDataType.Double:
                    if (sourceType == typeof(double))
                    {
                        status = true;
                    }
                    break;
                case ParameterDataType.Float:
                    if (sourceType == typeof(float))
                    {
                        status = true;
                    }
                    break;
                case ParameterDataType.Integer:
                    if (sourceType == typeof(Int32))
                    {
                        status = true;
                    }
                    break;
                case ParameterDataType.Array:
                    if (sourceType.IsArray || sourceType.IsEnum || sourceType == typeof(DataTable))
                    {
                        // The property type of an attribute with multiple choice is enum
                        status = true;
                    }
                    break;
                case ParameterDataType.String:
                    status = true; // any type can convert to string
                    break;
            }

            return status;
        }

        /// <summary>
        /// Gets information indicate whether a ParameterDataType type representing a source value can be converted to a Type object representing target value
        /// </summary>
        /// <param name="sourceType"></param>
        /// <param name="targetType"></param>
        /// <returns>true if it can be converted, false otherwise.</returns>
        public static bool CanConvertTo(ParameterDataType sourceType, Type targetType)
        {
            bool status = false;

            switch (sourceType)
            {
                case ParameterDataType.Boolean:
                    if (targetType == typeof(bool) || targetType == typeof(string))
                    {
                        status = true;
                    }
                    break;

                case ParameterDataType.BigInteger:
                    if (targetType == typeof(Int64) || targetType == typeof(string))
                    {
                        status = true;
                    }
                    break;
                case ParameterDataType.Date:
                case ParameterDataType.DateTime:
                    if (targetType == typeof(DateTime) || targetType == typeof(string))
                    {
                        status = true;
                    }
                    break;
                case ParameterDataType.Decimal:
                    if (targetType == typeof(decimal) || targetType == typeof(string))
                    {
                        status = true;
                    }
                    break;
                case ParameterDataType.Double:
                    if (targetType == typeof(double) || targetType == typeof(string))
                    {
                        status = true;
                    }
                    break;
                case ParameterDataType.Float:
                    if (targetType == typeof(float) || targetType == typeof(string))
                    {
                        status = true;
                    }
                    break;
                case ParameterDataType.Integer:
                    if (targetType == typeof(Int32) || targetType == typeof(string))
                    {
                        status = true;
                    }
                    break;
                case ParameterDataType.Array:
                    if (targetType.IsArray)
                    {
                        status = true;
                    }
                    break;
                case ParameterDataType.String:
                    if (targetType == typeof(string) || targetType.IsEnum)
                    {
                        status = true;
                    }
                    break;
            }

            return status;
        }

        /// <summary>
        /// Gets a value from an activity property
        /// </summary>
        /// <param name="path">The path indicating the activity property</param>
        /// <param name="rootActivity">The root activity of the workflow</param>
        /// <returns>A value</returns>
        private static object GetActivityPropertyValue(string path, INewteraWorkflow rootActivity, Activity invokingActivity)
        {
            object val = null;

            string activityName = ParameterBindingInfo.GetActivityName(path);
            string propertyName = ParameterBindingInfo.GetPropertyName(path);

            Activity activity = ((Activity)rootActivity).GetActivityByName(activityName);
            if (activity != null)
            {
                if (invokingActivity != null &&
                    activity is ForEachActivity &&
                    propertyName == "CurrentItem")
                {
                    // Get the CurrentItem value from the direct child of a ForEachActivity which is
                    // dynamically created by ForEachActivity object and is assigned with
                    // CurrentItem value
                    Activity dynamicChildactivity = invokingActivity;

                    while (dynamicChildactivity != null && !(dynamicChildactivity is ForEachActivity))
                    {
                        try
                        {
                            val = dynamicChildactivity.GetValue(ForEachActivity.DataItemProperty);
                        }
                        catch (Exception)
                        {
                            val = null;
                        }

                        if (val != null)
                        {
                            break;
                        }

                        dynamicChildactivity = dynamicChildactivity.Parent;
                    }
                }
                else
                {
                    PropertyInfo propertyInfo = activity.GetType().GetProperty(propertyName);
                    if (propertyInfo != null)
                    {
                        val = propertyInfo.GetValue(activity, null);
                    }
                }
            }
            else
            {
                throw new Exception("Unable to find an activity with name " + activityName + " for parameter binding.");
            }

            return val;
        }

        /// <summary>
        /// Gets a value from an attribute of the bound data instance indicated by the path
        /// </summary>
        /// <param name="path">The path indicating the attribute of the bound data instance</param>
        /// <param name="rootActivity">The root activity of the workflow</param>
        /// <returns>A value</returns>
        private static object GetDataInstanceValue(string path, INewteraWorkflow rootActivity)
        {
            object val = null;

            if (rootActivity.Instance != null)
            {
                // convert attribute name to attribute display name since
                // rootActivity.Instance.GetValue method takes attribute display name
                // as parameter.
                InstanceAttributePropertyDescriptor pd = (InstanceAttributePropertyDescriptor)rootActivity.Instance.WrappedInstance.GetProperties(null)[path];
                if (pd != null)
                {
                    val = rootActivity.Instance.GetValue(pd.DisplayName);
                }
            }

            return val;
        }

        /// <summary>
        /// Gets a value from an input parameter of the current workflow
        /// </summary>
        /// <param name="path">The path indicating the input parameter</param>
        /// <param name="rootActivity">The root activity of the workflow</param>
        /// <returns>A value</returns>
        private static object GetParameterValue(string path, INewteraWorkflow rootActivity)
        {
            object val = null;

            foreach (InputParameter parameter in rootActivity.InputParameters)
            {
                if (parameter.Name == path)
                {
                    val = parameter.Value;
                }
            }

            return val;
        }
    }
}
