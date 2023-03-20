/*
* @(#)AutoClassifyDef.cs
*
* Copyright (c) 2008 Newtera, Inc. All rights reserved.
*
*/
namespace Newtera.Common.MetaData.DataView.Taxonomy
{
	using System;
	using System.Xml;
	using System.Collections;
	using System.ComponentModel;
	using System.Drawing.Design;

	using Newtera.Common.Core;
	using Newtera.Common.MetaData.Schema;
	using Newtera.Common.MetaData.DataView;
	using Newtera.Common.MetaData.XaclModel;

	/// <summary> 
	/// The class contains the definition of an auto-generated hierarchy
	/// </summary>
	/// <version> 1.0.0 13 June 2008</version>
	/// <remarks>
	/// The DesignStudio can generate a hierarchy of nodes using the definition contained
    /// in this object and the specified data retrieved from the database
	/// </remarks>
	public class AutoClassifyDef : DataViewElementBase
	{
		private string _rootNodeCaption; // runtime
		private AutoClassifyLevelCollection _levelDefs;

		/// <summary>
		/// Initiate an instance of AutoClassifyDef class
		/// </summary>
		public AutoClassifyDef() : base("AutoDef")
		{
            _rootNodeCaption = null;
			_levelDefs = new AutoClassifyLevelCollection();
            if (GlobalSettings.Instance.IsWindowClient)
            {
                _levelDefs.ValueChanged += new EventHandler(ValueChangedHandler);
            }
		}

		/// <summary>
		/// Initiating an instance of AutoClassifyDef class
		/// </summary>
		/// <param name="xmlElement">The xml element conatins data of the instance</param>
		internal AutoClassifyDef(XmlElement xmlElement) : base()
		{
			Unmarshal(xmlElement);

            if (GlobalSettings.Instance.IsWindowClient)
            {
                // register the value change handlers
                _levelDefs.ValueChanged += new EventHandler(ValueChangedHandler);
            }
		}

        /// <summary>
        /// Gets or sets the caption of hierarchy root node.
        /// </summary>
        public string RootNodeCaption
        {
            get
            {
                return _rootNodeCaption;
            }
            set
            {
                _rootNodeCaption = value;
            }
        }

        /// <summary>
        /// Gets the information indicating whether it contains the hierarchy generating definition
        /// </summary>
        public bool HasDefinition
        {
            get
            {
                if (_levelDefs.Count > 0)
                {
                    return true;
                }
                else
                {
                    return false;
                }
            }
        }

        /// <summary>
        /// Gets a collection of AutoClassifyLevel defintion objects
        /// </summary>
        /// <remarks>
        /// the first item in the collection represents the definition for the first level,
        /// the second represents the sencond level, and so forth.
        /// </remarks>
        public AutoClassifyLevelCollection AutoClassifyLevels
        {
            get
            {
                return _levelDefs;
            }
        }

        /// <summary>
        /// Gets or sets the DataViewModel that owns this element
        /// </summary>
        /// <value>DataViewModel object</value>
        [BrowsableAttribute(false)]
        public override DataViewModel DataView
        {
            get
            {
                return base.DataView;
            }
            set
            {
                base.DataView = value;

                foreach (AutoClassifyLevel levelDef in _levelDefs)
                {
                    levelDef.DataView = value;
                }
            }
        }

		/// <summary>
		/// Gets the type of element
		/// </summary>
		/// <value>One of ElementType values</value>
		[BrowsableAttribute(false)]		
		public override ElementType ElementType 
		{
			get
			{
				return ElementType.AutoClassifyDef;
			}
		}

        /// <summary>
        ///  clear the definition
        /// </summary>
        public void Clear()
        {
            _levelDefs.Clear();
        }

        /// <summary>
        /// Clone the object
        /// </summary>
        /// <returns></returns>
        public AutoClassifyDef Clone()
        {
            // use Marshal and Unmarshal to clone a DataViewModel
            XmlDocument doc = new XmlDocument();
            XmlElement root = doc.CreateElement("AutoClassifyDef");
            doc.AppendChild(root);
            XmlElement child = doc.CreateElement(ElementFactory.ConvertTypeToString(this.ElementType));
            this.Marshal(child);
            root.AppendChild(child);

            // create a new AutoClassifyDef and unmarshal from the xml element as source
            AutoClassifyDef newDef = new AutoClassifyDef(child);

            return newDef;
        }

        /// <summary>
        /// Add a level to the classifying defintion
        /// </summary>
        public void AddLevel(AutoClassifyLevel levelDef)
        {
            levelDef.DataView = this.DataView;
            _levelDefs.Add(levelDef);
        }

        /// <summary>
        /// Remove the last level definition
        /// </summary>
        public void RemoveLastLevel()
        {
            // remove the last level
            int lastLevelIndex = _levelDefs.Count - 1;
            _levelDefs.RemoveAt(lastLevelIndex);
        }

        /// <summary>
        /// Gets the AutoClassifyLevel object at the given index
        /// </summary>
        /// <param name="index">The index</param>
        /// <returns>The the indicated item</returns>
        public AutoClassifyLevel GetClassifyLevelAt(int index)
        {
            return (AutoClassifyLevel)_levelDefs[index];
        }

        /// <summary>
        /// Get information indicating whether an attribute is referenced by any of levels
        /// in the auto-classifying definition.
        /// </summary>
        /// <param name="ownerNode">The node that owns the definition</param>
        /// <param name="className">The owner class name</param>
        /// <param name="attributeName">The attribute name</param>
        /// <returns>true if the attribute is referenced any of levels
        /// in the auto-classifying definition, false otherwise.</returns>
        public bool IsAttributeReferenced(ITaxonomy ownerNode, string className, string attributeName)
        {
            bool status = false;

            foreach (AutoClassifyLevel autoLevel in _levelDefs)
            {
                if (autoLevel.IsAttributeReferenced(ownerNode, className, attributeName))
                {
                    status = true;
                    break;
                }
            }

            return status;
        }

		/// <summary>
		/// Accept a visitor of IDataViewElementVisitor type to traverse its
		/// elements.
		/// </summary>
		/// <param name="visitor">A visitor</param>
		public override void Accept(IDataViewElementVisitor visitor)
		{
			if (visitor.VisitClassifyDef(this))
			{
				foreach (AutoClassifyDef level in _levelDefs)
				{
					level.Accept(visitor);
				}
			}
		}

		/// <summary>
		/// sets the element members from a XML element.
		/// </summary>
		/// <param name="parent">An xml element</param>
		public override void Unmarshal(XmlElement parent)
		{
			base.Unmarshal(parent);

            // restore the _levelDefs
            _levelDefs = (AutoClassifyLevelCollection)ElementFactory.Instance.Create((XmlElement)parent.ChildNodes[0]);
		}

		/// <summary>
		/// Write values of members to an xml element
		/// </summary>
		/// <param name="parent">An xml element for the element</param>
		public override void Marshal(XmlElement parent)
		{
			base.Marshal(parent);

			// write the _levelDefs
            XmlElement child = parent.OwnerDocument.CreateElement(ElementFactory.ConvertTypeToString(_levelDefs.ElementType));
			_levelDefs.Marshal(child);
			parent.AppendChild(child);
		}
	}
}