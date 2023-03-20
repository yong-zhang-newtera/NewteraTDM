/*
* @(#)DummyInstance.cs
*
* Copyright (c) 2007 Newtera, Inc. All rights reserved.
*
*/

namespace Newtera.Common.Wrapper
{
    using System;
    using System.Collections;
    using System.ComponentModel;
    using System.Text;

    using Newtera.Common.MetaData.DataView;

    /// <summary>
    /// A dummy implementation of IIstanceWrapper. It is set to a workflow instance initially
    /// so that we can provide a better error message for debugging.
    /// </summary>
    public class DummyInstance : IInstanceWrapper
    {
        /// <summary> 
        /// Creates a new instance of the <see cref="Newtera.Common.Wrapper.DummyInstance"/> class.
        /// </summary>
        /// <param name="instance">
        /// The instance wrapped by this <see cref="Newtera.Common.Wrapper.DummyInstance"/>.
        /// </param>
        public DummyInstance()
        {
        }

        /// <summary>
        /// The object wrapped by this <see cref="Newtera.Common.Wrapper.DummyInstance"/>.
        /// </summary>
        public InstanceView WrappedInstance
        {
            get 
            {
                throw new InvalidOperationException("The workflow has not had a binding data instance yet. Make sure to bind a data instance to the workflow before referencing to it.");
            }
        }

        /// <summary>
        /// Return the collection of property descriptors.
        /// </summary>
        /// <returns>A collection of InstanceAttributePropertyDescriptor </returns>
        public PropertyDescriptorCollection PropertyDescriptors
        {
            get
            {
                throw new InvalidOperationException("The workflow has not had a binding data instance yet. Make sure to bind a data instance to the workflow before referencing to it.");
            }
        }

        /// <summary>
        /// Get system generated obj id of the instance
        /// </summary>
        public string ObjId
        {
            get
            {
                return null;
            }
        }

        /// <summary>
        /// Get the name of the bottom class that owns the instance
        /// </summary>
        public string OwnerClassName
        {
            get
            {
                return null;
            }
        }

        /// <summary>
        /// Get the id of the database schema that owns the instance
        /// </summary>
        public string SchemaId
        {
            get
            {
                return null;
            }
        }

        /// <summary>Get boolean value of the specified property
        /// <param name="theProperty">
        /// The name of the property to get the value of.
        /// </param>
        /// <returns>The boolean value of the property.</returns>
        public bool GetBoolean(string theProperty)
        {
            throw new InvalidOperationException("The workflow has not had a binding data instance yet. Make sure to bind a data instance to the workflow before referencing to it.");
        }

        /// <summary>Get date and time value of the specified property
        /// <param name="theProperty">
        /// The name of the property to get the value of.
        /// </param>
        /// <returns>The date and time value of the property.</returns>
        public DateTime GetDateTime(string theProperty)
        {
            throw new InvalidOperationException("The workflow has not had a binding data instance yet. Make sure to bind a data instance to the workflow before referencing to it.");
        }

        /// <summary>Get decimal value of the specified property
        /// <param name="theProperty">
        /// The name of the property to get the value of.
        /// </param>
        /// <returns>The decimal value of the property.</returns>
        public decimal GetDecimal(string theProperty)
        {
            throw new InvalidOperationException("The workflow has not had a binding data instance yet. Make sure to bind a data instance to the workflow before referencing to it.");
        }

        /// <summary>Get double value of the specified property
        /// <param name="theProperty">
        /// The name of the property to get the value of.
        /// </param>
        /// <returns>The double value of the property.</returns>
        public double GetDouble(string theProperty)
        {
            throw new InvalidOperationException("The workflow has not had a binding data instance yet. Make sure to bind a data instance to the workflow before referencing to it.");
        }

        /// <summary>Get float value of the specified property
        /// <param name="theProperty">
        /// The name of the property to get the value of.
        /// </param>
        /// <returns>The float value of the property.</returns>
        public float GetFloat(string theProperty)
        {
            throw new InvalidOperationException("The workflow has not had a binding data instance yet. Make sure to bind a data instance to the workflow before referencing to it.");
        }

        /// <summary>Gets the 16-bit signed integer value of the specified property
        /// <param name="theProperty">
        /// The name of the property to get the value of.
        /// </param>
        /// <returns>The the 16-bit signed integer value of the property.</returns>
        public short GetInt16(string theProperty)
        {
            throw new InvalidOperationException("The workflow has not had a binding data instance yet. Make sure to bind a data instance to the workflow before referencing to it.");
        }

        /// <summary>Gets the 32-bit signed integer value of the specified property
        /// <param name="theProperty">
        /// The name of the property to get the value of.
        /// </param>
        /// <returns>The 32-bit signed integer value of the property.</returns>
        public int GetInt32(string theProperty)
        {
            throw new InvalidOperationException("The workflow has not had a binding data instance yet. Make sure to bind a data instance to the workflow before referencing to it.");
        }

        /// <summary>Gets the 64-bit signed integer value of the specified property
        /// <param name="theProperty">
        /// The name of the property to get the value of.
        /// </param>
        /// <returns>The 64-bit signed integer value of the property.</returns>
        public long GetInt64(string theProperty)
        {
            throw new InvalidOperationException("The workflow has not had a binding data instance yet. Make sure to bind a data instance to the workflow before referencing to it.");
        }

        /// <summary>Gets the string value of the specified property
        /// <param name="theProperty">
        /// The name of the property to get the value of.
        /// </param>
        /// <returns>The string value of the property.</returns>
        public string GetString(string theProperty)
        {
            throw new InvalidOperationException("The workflow has not had a binding data instance yet. Make sure to bind a data instance to the workflow before referencing to it.");
        }

        /// <summary>Get the object value of a property.</summary>
        /// <param name="theProperty">
        /// The name of the property to get the value of. May be nested.
        /// </param>
        /// <returns>The object value of the property.</returns>
        public object GetValue(string theProperty)
        {
            throw new InvalidOperationException("The workflow has not had a binding data instance yet. Make sure to bind a data instance to the workflow before referencing to it.");
        }

        /// <summary>Gets the information indicating whether the value of the specified property is null.</summary>
        /// <param name="theProperty">
        /// The name of the property to get the value of. May be nested.
        /// </param>
        /// <returns>true if it is null, false otherwise.</returns>
        public bool IsNull(string theProperty)
        {
            throw new InvalidOperationException("The workflow has not had a binding data instance yet. Make sure to bind a data instance to the workflow before referencing to it.");
        }

        /// <summary>
        /// Gets the information indicating whether the value of the specified property is changed.</summary>
        /// <param name="theProperty">
        /// The name of the property to check
        /// </param>
        /// <returns>true if it is changed, false otherwise.</returns>
        public bool IsValueChanged(string theProperty)
        {
            throw new InvalidOperationException("The workflow has not had a binding data instance yet. Make sure to bind a data instance to the workflow before referencing to it.");
        }

        /// <summary>Gets the information indicating whether the value of the specified property contains a value.</summary>
        /// <param name="theProperty">The name of the property to get the value of. May be nested.</param>
        /// <param name="propertyValue">The specified value</param>
        /// <returns>true if it contains the value, false otherwise.</returns>
        /// <remarks>This method is valid for the property whose value is a collection, such as array or multiple choice enum</remarks>
        public bool Contains(string theProperty, object propertyValue)
        {
            throw new InvalidOperationException("The workflow has not had a binding data instance yet. Make sure to bind a data instance to the workflow before referencing to it.");
        }

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
        public InstanceAttributePropertyDescriptor GetPropertyDescriptor(string theProperty)
        {
            throw new InvalidOperationException("The workflow has not had a binding data instance yet. Make sure to bind a data instance to the workflow before referencing to it.");
        }

        /// <summary>
        /// Set a property value.
        /// </summary>
        /// <param name="theProperty">
        /// The name of the property to set value of.
        /// </param>
        /// <param name="propertyValue">The new property value.</param>
        public void SetValue(string theProperty, object propertyValue)
        {
            throw new InvalidOperationException("The workflow has not had a binding data instance yet. Make sure to bind a data instance to the workflow before referencing to it.");
        }

        /// <summary>
        /// Save the instance to the database
        /// </summary>
        public void Save()
        {
            // do nothing
        }

        /// <summary>
        /// Copy the instance values from a source instance
        /// </summary>
        public void Copy(IInstanceWrapper srcInstance)
        {
        }
    }
}