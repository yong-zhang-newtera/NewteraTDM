using System;
using System.Data;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Web;
using System.Threading;
using System.Text;
using System.Xml;
using System.IO;
using System.Text.RegularExpressions;
using System.Runtime.Remoting;
using Newtonsoft.Json.Linq;

using HtmlAgilityPack;

using Newtera.Common.Core;
using Newtera.Common.MetaData;
using Newtera.Common.MetaData.DataView;
using Newtera.Common.MetaData.Schema;
using Newtera.Common.MetaData.Logging;
using Newtera.Common.MetaData.Principal;
using Newtera.Common.MetaData.XaclModel;
using Newtera.Server.Logging;

namespace Newtera.WebForm
{
    /// <remarks>
    /// RelatedInstanceEditor handles rendering view, generating view model and converting view mode to internal model for
   /// a related instance which can have many -to-one, one-to-many, or one-to-one relationship with the master instance
    /// 
    /// </remarks>
    public class RelatedInstanceEditor : InstanceEditor
    {
        private bool _isForeignKeyRequired = false;
        PropertyDescriptorCollection _properties = null;

        /// <summary>
        /// Gets or sets the information indicating whether the related class requires a foreignKey pointing to the master class
        /// 
        /// </summary>
        /// <value>
        /// True if a foreign key is required, false otherwise.
        /// </value>
        public bool IsForeignKeyRequired
        {
            get
            {
                return _isForeignKeyRequired;
            }
            set
            {
                _isForeignKeyRequired = value;
            }
        }

        /// <summary>
        /// Gets the information indicating whether th editor is for a related class to the master class
        /// </summary>
        public override bool IsRelatedClass
        {
            get
            {
                return true;
            }
        }

        /// <summary>
        /// Gets a root path of the view model for the editor
        /// </summary>
        public override string ViewModelPath
        {
            get
            {
                // View model path for a related class editor
                return base.ViewModelPath + this.InstanceClassName + ".";
            }
        }

        /// <summary>
        /// Gets a root path of accessing an array in the view model
        /// </summary>
        public override string ArrayBasePath
        {
            get
            {
                return base.ArrayBasePath + this.InstanceClassName + ".";
            }
        }

        /// <summary>
        /// Translate an instance of JSON format(ViewModel) to an instance of InstanceView format (Model)
        /// </summary>
        /// <param name="instance">An instance in JSON format</param>
        /// <returns> A collection of converted IInstanceEditor objects</returns>
        public override List<IInstanceEditor> ConvertToModel(JObject instance)
        {
            List<IInstanceEditor> convertedInstances = new List<IInstanceEditor>();

            // if the related instance is a single instance, convert it using the normal way,
            // otherwise, let the customized control to perform the conversion
            if (IsForeignKeyRequired)
            {
                foreach (PropertyPlaceHolder placeHolder in PropertyPlaceHolders)
                {
                    IPropertyControl propertyControl = CreateRelationshipPropertyControl(placeHolder.PropertyName, placeHolder.IsReadOnly);

                    if (propertyControl != null)
                    {
                        // value of the jproperty is an JArray
                        JProperty relatedProperty = instance.Property(placeHolder.PropertyName);

                        if (relatedProperty != null)
                        {
                            convertedInstances = propertyControl.GetInstanceEditors(relatedProperty);
                        }
                    }
                }
            }
            else
            {
                // it is a single related instance, let the parent class convert it 
                base.ConvertToModel(instance);

                convertedInstances.Add(this);
            }

            return convertedInstances;
        }

        /// <summary>
        /// Translate an XmlElement object to an instance of InstanceView format (Model)
        /// </summary>
        /// <param name="element">An XmlElement object</param>
        /// <returns> A collection of converted IInstanceEditor objects</returns>
        public override List<IInstanceEditor> ConvertToModel(XmlElement element)
        {
            List<IInstanceEditor> convertedInstances = new List<IInstanceEditor>();

            // if the related instance is a single instance, convert it using the normal way,
            // otherwise, let the customized control to perform the conversion
            if (IsForeignKeyRequired)
            {
                foreach (PropertyPlaceHolder placeHolder in PropertyPlaceHolders)
                {
                    IPropertyControl propertyControl = CreateRelationshipPropertyControl(placeHolder.PropertyName, placeHolder.IsReadOnly);

                    if (propertyControl != null)
                    {
                        // The child elements are the data instances of the related class
                        convertedInstances = propertyControl.GetInstanceEditors(element);
                    }
                }
            }
            else
            {
                // it is a single related instance, let the parent class convert it 
                base.ConvertToModel(element);

                convertedInstances.Add(this);
            }

            return convertedInstances;
        }

        /// <summary>
        /// Validate the model represented by the InstanceView instance against the schema.
        /// </summary>
        /// <param name="convert">true to convert model value to viewmodel value, otherwise, no conversion performed</param>
        /// <returns>Return if the instance model is valid. It will throw an exception if a validating error occures </returns>
        public override void Validate()
        {

        }

        /// <summary>
        /// ConvertToViewModel converts values from the InstanceView instance (Model) to a JSON instance (ViewModel)
        /// the EditInstance
        /// </summary>
        /// <param name="convert">true to convert model value to viewmodel value, otherwise, no conversion performed</param>
        /// <returns>The JSON instance</returns>
        public override JObject ConvertToViewModel(bool convert)
        {
            // if the related instance is a single instance, convert it using the normal way,
            // otherwise, let the customized control to perform the conversion
            if (IsForeignKeyRequired)
            {
                // it is a many-to-one relationship, let the customized property control to convert instance(s) data to view model
                JObject instance = new JObject();

                foreach (PropertyPlaceHolder placeHolder in PropertyPlaceHolders)
                {
                    IPropertyControl propertyControl = CreateRelationshipPropertyControl(placeHolder.PropertyName, placeHolder.IsReadOnly);

                    if (propertyControl != null)
                    {
                        JToken propertyViewModel = propertyControl.GetPropertyViewModel();
                        if (propertyViewModel != null)
                        {
                            instance.Add(placeHolder.PropertyName, propertyViewModel);
                        }
                    }
                }

                return instance;
            }
            else
            {
                // it is a single related instance, let the parent class convert it 
                return base.ConvertToViewModel(convert);
            }
        }

        /// <summary>
        /// if the related editor has a many-to-one relationship to the master instance, only allow the relationship
        /// property to be created. The relationship property must have the UIControlCreator value defined, using
        /// RelationshipGridViewControl to render, convert between view model and model
        /// </summary>
        /// <param name="propertyName"></param>
        /// <param name="isReadOnly">If it is a readonly property</param>
        /// <returns>An IPropertyControl</returns>
        public IPropertyControl CreateRelationshipPropertyControl(string propertyName, bool isReadOnly)
        {
            if (_properties == null)
            {
                _properties = GetPropertyDescriptorList(this.EditInstance);
            }


            // filter out non-relationship properties
            InstanceAttributePropertyDescriptor pd = _properties[propertyName] as InstanceAttributePropertyDescriptor;

            if (pd.IsRelationship && pd.IsForeignKeyRequired)
            {
                return CreatePropertyControl(propertyName, isReadOnly, null);
            }
            else
            {
                return null;
            }
        }
    }
}

