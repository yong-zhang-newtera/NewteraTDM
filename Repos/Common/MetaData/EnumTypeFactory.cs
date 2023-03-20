/*
* @(#)EnumTypeFactory.cs
*
* Copyright (c) 2003 Newtera, Inc. All rights reserved.
*
*/
namespace Newtera.Common.MetaData
{
	using System;
	using System.Resources;
	using System.Collections;
	using System.Collections.Specialized;
	using System.Threading;
	using System.Reflection;
	using System.Reflection.Emit;

	using Newtera.Common.Core;
	using Newtera.Common.MetaData.Schema;

	/// <summary>
	/// A singleton class that creates an Enum Type of a property descriptor based
	/// on a SchemaModelElement
	/// </summary>
	/// <version>  	1.0.0 14 Nov 2003 </version>
	/// <author> Yong Zhang </author>
	public class EnumTypeFactory
	{
		private ModuleBuilder _moduleBuilder;
		private Type _booleanEnumType;
		private Hashtable _enumTypes;
        private Hashtable _enumValueCollectionTable;

		/// <summary>
		/// Singleton's private instance.
		/// </summary>
		private static EnumTypeFactory theFactory;
		
		static EnumTypeFactory()
		{
			theFactory = new EnumTypeFactory();
		}

		/// <summary>
		/// The private constructor.
		/// </summary>
		private EnumTypeFactory()
		{
			_moduleBuilder = null;
			_booleanEnumType = null;
			_enumTypes = new Hashtable();
            _enumValueCollectionTable = new Hashtable();
		}

		/// <summary>
		/// Gets the EnumTypeFactory instance.
		/// </summary>
		/// <returns> The EnumTypeFactory instance.</returns>
		static public EnumTypeFactory Instance
		{
			get
			{
				return theFactory;
			}
		}
		
		/// <summary>
		/// Creates an Enum Type of a property descriptor based
		/// on a SchemaModelElement
		/// </summary>
		/// <param name="schemaModelElement">The schema model element</param>
		/// <returns>A Type represent an enum class</returns>
		public Type Create(AttributeElementBase schemaModelElement)
		{
			// make sure it doesn't create a duplicated enum type when accessing
			// by multiple threads
			lock(this)
			{
				Type type = null;

				if (schemaModelElement.DataType == DataType.Boolean)
				{
					if (_booleanEnumType == null)
					{
						_booleanEnumType = CreateBooleanEnumType();
					}

					type = _booleanEnumType;
				}
				else if (schemaModelElement is SimpleAttributeElement &&
					((SimpleAttributeElement) schemaModelElement).Constraint != null &&
					 ((SimpleAttributeElement) schemaModelElement).Constraint is IEnumConstraint)
				{
					SimpleAttributeElement simpleAttribute = (SimpleAttributeElement) schemaModelElement;
					// create an unique enum type name among all schemas
                    string typeName = simpleAttribute.CreateEnumTypeName();

					// we need a dynamic enum type for the attribute, first to see
					// if it has already been created
					type = (Type) _enumTypes[typeName];

                    if (type == null)
					{
                        // first time reference
						type = CreateAttributeEnumType(typeName, simpleAttribute.Constraint, simpleAttribute);
                        
                        _enumTypes[typeName] = type; // avoid to create it second time
					}
                    else if (IsChanged(type, simpleAttribute.Constraint))
                    {
                        // the constraint definition has been changed, re-create all enum types
                        Reset();

                        type = CreateAttributeEnumType(typeName, simpleAttribute.Constraint, simpleAttribute);

                        _enumTypes[typeName] = type; // avoid to create it second time
                    }
				}
                else if (schemaModelElement is VirtualAttributeElement &&
                    ((VirtualAttributeElement)schemaModelElement).Constraint != null &&
                     ((VirtualAttributeElement)schemaModelElement).Constraint is IEnumConstraint)
                {
                    VirtualAttributeElement virtualAttribute = (VirtualAttributeElement)schemaModelElement;
                    // create an unique enum type name among all schemas
                    string typeName = "Newtera.Common.Types." + virtualAttribute.SchemaModel.SchemaInfo.Name +
                        virtualAttribute.SchemaModel.SchemaInfo.Version + virtualAttribute.Constraint.EnumTypeName;

                    // we need a dynamic enum type for the attribute, first to see
                    // if it has already been created
                    type = (Type)_enumTypes[typeName];

                    if (type == null)
                    {
                        type = CreateAttributeEnumType(typeName, virtualAttribute.Constraint, virtualAttribute);

                        _enumTypes[typeName] = type; // avoid to create it second time
                    }
                    else if (IsChanged(type, virtualAttribute.Constraint))
                    {
                        // the constraint definition has been changed, re-create all enum types
                        Reset();

                        type = CreateAttributeEnumType(typeName, virtualAttribute.Constraint, virtualAttribute);

                        _enumTypes[typeName] = type; // avoid to create it second time
                    }
                }
				
				return type;
			}
		}

        /// <summary>
        /// Gets the cached enum value collection for a schema model attribute
        /// </summary>
        /// <param name="schemaModelElement">The schema model element</param>
        /// <param name="constraint">The list constraint</param>
        /// <returns>A EnumValueCollection object</returns>
        public EnumValueCollection GetEnumValues(SchemaModelElement schemaModelElement, ListElement constraint)
        {
            // synchronize the access
            lock (this)
            {
                string typeName = GetEnumTypeName(schemaModelElement);
                EnumValueCollection enumValues;

                if (typeName != null)
                {
                    enumValues = (EnumValueCollection)_enumValueCollectionTable[typeName];

                    if (enumValues == null)
                    {
                        // ListEnum requires the constrainted attribute as a context
                        // for getting values
                        enumValues = constraint.GetAllEnumValues(schemaModelElement);
                        _enumValueCollectionTable[typeName] = enumValues;
                    }

                    return enumValues;
                }
                else
                {
                    return null;
                }
            }
        }

        /// <summary>
        /// Clear the cached enum types created so that it will be recreated
        /// next time.
        /// </summary>
        public void ClearEnumTypes()
        {
            // make sure to prevent accessing while clearing the cache
            lock (this)
            {
                Reset();
            }
        }

        /// <summary>
        /// Remove the all enum types that have been created
        /// </summary>
        private void Reset()
        {
            _moduleBuilder = null;
            _booleanEnumType = null;
            _enumTypes.Clear();
            _enumValueCollectionTable.Clear();
        }

        /// <summary>
        /// Gets the information indicating whether values defined in the constaint has been
        /// changed, comparing to the values in the enum type object
        /// </summary>
        /// <param name="type">The enum type</param>
        /// <param name="constraint">The constraint</param>
        /// <returns>true if it has been changed, false otherwise.</returns>
        private bool IsChanged(Type type, ConstraintElementBase constraint)
        {
            bool status = false;

            EnumValueCollection values = null;
            string[] enumValues = Enum.GetNames(type);
            
            if (constraint is EnumElement)
            {
                values = ((EnumElement)constraint).Values;
            }
            else if (constraint is ListElement)
            {
                // Getting values for a ListElement may be very expensive, we have to clear
                // enum types for ListElements at some specific events, for example, when user
                // information is changed
                return false;
            }
           
            if (values == null)
            {
                throw new InvalidOperationException("Unexpected constraint type encountered.");
            }

            // the first value from enum type is Unknow which is added while creating the enum type
            // so ignore it.
            if (values.Count != (enumValues.Length - 1))
            {
                status = true;
            }
            else
            {
                int index = 1;
                foreach (EnumValue enumVal in values)
                {
                    if (enumVal.DisplayText != enumValues[index])
                    {
                        status = true;
                        break;
                    }
                    index++;
                }
            }

            return status;
        }

        // create an unique enum type name among all schemas
        private string GetEnumTypeName(SchemaModelElement schemaModelElement)
        {
            string typeName = null;
            if (schemaModelElement is SimpleAttributeElement &&
                ((SimpleAttributeElement)schemaModelElement).Constraint != null &&
                ((SimpleAttributeElement)schemaModelElement).Constraint is IEnumConstraint)
            {
                SimpleAttributeElement simpleAttribute = (SimpleAttributeElement)schemaModelElement;

                typeName = "Newtera.Common.Types." + simpleAttribute.SchemaModel.SchemaInfo.Name +
                        simpleAttribute.SchemaModel.SchemaInfo.Version + simpleAttribute.Constraint.EnumTypeName;
            }
            else if (schemaModelElement is VirtualAttributeElement &&
                    ((VirtualAttributeElement)schemaModelElement).Constraint != null &&
                    ((VirtualAttributeElement)schemaModelElement).Constraint is IEnumConstraint)
            {
                VirtualAttributeElement virtualAttribute = (VirtualAttributeElement)schemaModelElement;

                typeName = "Newtera.Common.Types." + virtualAttribute.SchemaModel.SchemaInfo.Name +
                    virtualAttribute.SchemaModel.SchemaInfo.Version + virtualAttribute.Constraint.EnumTypeName;
            }

            return typeName;
        }

		/// <summary>
		/// Create a module builder to build EnumBuilder
		/// </summary>
		/// <returns>A module builder</returns>
		private ModuleBuilder CreateModuleBuilder()
		{
			AppDomain appDomain = Thread.GetDomain();

			// Create a name for the assembly.
			AssemblyName assemblyName = new AssemblyName();
			assemblyName.Name = "NewteraDynamicTypesAssembly";

			// Create the dynamic assembly.
			AssemblyBuilder assemblyBuilder = appDomain.DefineDynamicAssembly(assemblyName, 
				AssemblyBuilderAccess.RunAndSave);

			// Create a dynamic module.
			return assemblyBuilder.DefineDynamicModule("NewteraDynamicTypesModule", 
														"NewteraDynamicTypesModule.mod");
		}

		/// <summary>
		/// Create an Enum Type for boolean property in which there are three values:
		/// None, True, False. Depending on the user's culture, the values will be
		/// localized.
		/// </summary>
		/// <returns>A dynamic Boolean Enum type</returns>
		private Type CreateBooleanEnumType()
		{
            if (_moduleBuilder == null)
            {
                _moduleBuilder = CreateModuleBuilder();
            }

			EnumBuilder enumBuilder = _moduleBuilder.DefineEnum("Newtera.Common.Types.BooleanEnum", 
                                 TypeAttributes.Public, typeof(Int32));
			LocaleInfo localeInfo = LocaleInfo.Instance;
			FieldBuilder fieldBuilder1 = enumBuilder.DefineLiteral(localeInfo.None, 0);
			FieldBuilder fieldBuilder2 = enumBuilder.DefineLiteral(localeInfo.True, 1);
			FieldBuilder fieldBuilder3 = enumBuilder.DefineLiteral(localeInfo.False, 2);

			return enumBuilder.CreateType();
		}

		/// <summary>
		/// Create an Enum Type for a simple attribute that has an enum constraint
		/// </summary>
		/// <param name="typeName">An unique type name among schemas</param>
		/// <param name="schemaModelElement">The attribute element</param>
		/// <returns>A Dynamic Enum Type</returns>
		private Type CreateAttributeEnumType(string typeName, ConstraintElementBase constraint, SchemaModelElement schemaModelElement)
		{
            EnumBuilder enumBuilder = null;
            try
            {
                if (_moduleBuilder == null)
                {
                    _moduleBuilder = CreateModuleBuilder();
                }

                enumBuilder = _moduleBuilder.DefineEnum(typeName, TypeAttributes.Public, typeof(Int32));

                EnumValueCollection values;
                if (constraint is EnumElement)
                {
                    values = ((EnumElement)constraint).Values;
                }
                else if (constraint is ListElement)
                {
                    values = ((ListElement)constraint).GetEnumValues(schemaModelElement);
                }
                else
                {
                    values = new EnumValueCollection();
                }

                // add none value first
                FieldBuilder fieldBuilder = enumBuilder.DefineLiteral(LocaleInfo.Instance.None, 0);

                int enumValue = 1;
                string displayText;
                foreach (EnumValue enumVal in values)
                {
                    displayText = enumVal.DisplayText;
                    if (string.IsNullOrEmpty(displayText))
                    {
                        displayText = enumValue.ToString();
                    }
                    fieldBuilder = enumBuilder.DefineLiteral(displayText, enumValue++);
                }

                return enumBuilder.CreateType();
            }
            catch (Exception ex)
            {
                ErrorLog.Instance.WriteLine(ex.Message + "\n" + ex.StackTrace);
                if (enumBuilder != null)
                {
                    return enumBuilder.CreateType();
                }
                else
                {
                    throw ex;
                }
            }
		}
	}
}