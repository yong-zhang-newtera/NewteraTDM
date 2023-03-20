/*
* @(#)IInstanceWrapper.cs
*
* Copyright (c) 2007 Newtera, Inc. All rights reserved.
*
*/
namespace Newtera.Common.Wrapper
{
    using System;
    using System.ComponentModel;
    using System.Reflection;

    using Newtera.Common.MetaData.DataView;

    /// <summary> 
    /// The interface for accessing and updating Newtera Data Instances.
    /// </summary>
    /// <remarks>
    /// <p>
    /// Typically not directly used by application code but rather implicitly
    /// via an <see cref="Newtera.Common.Wrapper.IObjectFactory"/>.
    /// </p>
    /// <p>
    /// Implementing classes have the ability to get and set property values
    /// (individually or in bulk), get property descriptors and query the
    /// readability and writability of properties.
    /// </p>
    /// <p>
    /// This interface supports <b>nested properties</b> enabling the setting
    /// of properties on subproperties to an unlimited depth.
    /// </p>
    /// <p>
    /// If a property update causes an exception, a
    /// <see cref="Newtera.Common.Wrapper.PropertyAccessException"/> will be thrown. Bulk
    /// updates continue after exceptions are encountered, throwing an exception
    /// wrapping <b>all</b> exceptions encountered during the update.
    /// </p>
    /// </remarks>
    public interface IInstanceWrapper
    {
        /// <summary>
        /// The instance wrapped by the wrapper (cannot be <see lang="null"/>).
        /// </summary>
        /// <returns>The instance wrapped by this wrapper.</returns>
        InstanceView WrappedInstance { get; }

        /// <summary>
        /// Return the collection of property descriptors.
        /// </summary>
        /// <returns>A collection of InstanceAttributePropertyDescriptor </returns>
        PropertyDescriptorCollection PropertyDescriptors { get; }

        /// <summary>
        /// Get system generated obj id of the instance
        /// </summary>
        string ObjId { get;}

        /// <summary>
        /// Get the name of the bottom class that owns the instance
        /// </summary>
        string OwnerClassName { get;}

        /// <summary>
        /// Get the id of the database schema that owns the instance
        /// </summary>
        string SchemaId { get;}

        /// The name of the property to get the value of.
        /// </param>
        /// <returns>The boolean value of the property.</returns>
        bool GetBoolean(string theProperty);

        /// <summary>Get date and time value of the specified property
        /// <param name="theProperty">
        /// The name of the property to get the value of.
        /// </param>
        /// <returns>The date and time value of the property.</returns>
        DateTime GetDateTime(string theProperty);

        /// <summary>Get decimal value of the specified property
        /// <param name="theProperty">
        /// The name of the property to get the value of.
        /// </param>
        /// <returns>The decimal value of the property.</returns>
        decimal GetDecimal(string theProperty);

        /// <summary>Get double value of the specified property
        /// <param name="theProperty">
        /// The name of the property to get the value of.
        /// </param>
        /// <returns>The double value of the property.</returns>
        double GetDouble(string theProperty);

        /// <summary>Get float value of the specified property
        /// <param name="theProperty">
        /// The name of the property to get the value of.
        /// </param>
        /// <returns>The float value of the property.</returns>
        float GetFloat(string theProperty);

        /// <summary>Gets the 16-bit signed integer value of the specified property
        /// <param name="theProperty">
        /// The name of the property to get the value of.
        /// </param>
        /// <returns>The the 16-bit signed integer value of the property.</returns>
        short GetInt16(string theProperty);

        /// <summary>Gets the 32-bit signed integer value of the specified property
        /// <param name="theProperty">
        /// The name of the property to get the value of.
        /// </param>
        /// <returns>The 32-bit signed integer value of the property.</returns>
        int GetInt32(string theProperty);

        /// <summary>Gets the 64-bit signed integer value of the specified property
        /// <param name="theProperty">
        /// The name of the property to get the value of.
        /// </param>
        /// <returns>The 64-bit signed integer value of the property.</returns>
        long GetInt64(string theProperty);

        /// <summary>Gets the string value of the specified property
        /// <param name="theProperty">
        /// The name of the property to get the value of.
        /// </param>
        /// <returns>The string value of the property.</returns>
        string GetString(string theProperty);

    	/// <summary>Get the object value of a property.</summary>
    	/// <param name="theProperty">
    	/// The name of the property to get the value of. May be nested.
    	/// </param>
    	/// <returns>The object value of the property.</returns>
    	object GetValue(string theProperty);

        /// <summary>Gets the information indicating whether the value of the specified property is null.</summary>
        /// <param name="theProperty">
        /// The name of the property to get the value of. May be nested.
        /// </param>
        /// <returns>true if it is null, false otherwise.</returns>
        bool IsNull(string theProperty);

        /// <summary>
        /// Gets the information indicating whether the value of the specified property is changed.</summary>
        /// <param name="theProperty">
        /// The name of the property to check
        /// </param>
        /// <returns>true if it is changed, false otherwise.</returns>
        bool IsValueChanged(string theProperty);

        /// <summary>Gets the information indicating whether the value of the specified property contains a value.</summary>
        /// <param name="theProperty">The name of the property to get the value of. May be nested.</param>
        /// <param name="propertyValue">The specified value</param>
        /// <returns>true if it contains the value, false otherwise.</returns>
        /// <remarks>This method is valid for the property whose value is a collection, such as array or multiple choice enum</remarks>
        bool Contains(string theProperty, object propertyValue);

    	/// <summary>
    	/// Get the <see cref="System.Reflection.PropertyInfo"/> for a particular
    	/// property.
    	/// </summary>
    	/// <param name="theProperty">
    	/// The property to be retrieved.
    	/// </param>
    	/// <returns>
        /// The <see cref="Newtera.Common.MetaData.DataView.InstanceAttributePropertyDescriptor"/> for the particular
    	/// property.
    	/// </returns>
		InstanceAttributePropertyDescriptor GetPropertyDescriptor(string theProperty);

    	/// <summary>
    	/// Set a property value.
    	/// </summary>
    	/// <param name="theProperty">
    	/// The name of the property to set value of.
    	/// </param>
    	/// <param name="propertyValue">The new property value.</param>
    	void SetValue(string theProperty, object propertyValue);

        /// <summary>
        /// Save the instance to the database
        /// </summary>
        void Save();

        /// <summary>
        /// Copy the instance values from a source instance
        /// </summary>
        void Copy(IInstanceWrapper srcInstance);
    }
}
