/*
* @(#)Selector.cs
*
* Copyright (c) 2003-2005 Newtera, Inc. All rights reserved.
*
*/
namespace Newtera.Common.MetaData.Mappings
{
	using System;
	using System.Xml;
	using System.Data;
	using System.Collections;
	using System.Text;
	using System.CodeDom.Compiler;
	using System.Reflection;
	using System.ComponentModel;
	using System.Drawing.Design;

	using Newtera.Common.MetaData.XaclModel;
	using Newtera.Common.MetaData.DataView;
	using Newtera.Common.MetaData.Mappings.Generator;

	/// <summary>
	/// The class represents selector that selects values from multiple sources
	/// instances and copy them to a single destination instance. it is used for
	/// creating a new instance of a class from existing instances of different
	/// classes. A Selector contains one or more ClassMapping objects.
	/// </summary>
	/// <version>  1.0.0 09 Jan 2005</version>
	/// <author> Yong Zhang </author>
	public class Selector : MappingNodeBase, IXaclObject
	{
		private string _dstClassName;
		private ClassMappingCollection _classMappings;
		private string _xpath = null; // run-time use only
		private SelectorManager _manager = null; // run-time use only

		/// <summary>
		/// Initiate an instance of a Selector class.
		/// </summary>
		/// <param name="name">The unique name of selector.</param>
		public Selector(string name) : base()
		{
			this.Name = name;

			_dstClassName = null;
			_classMappings = new ClassMappingCollection();
            if (GlobalSettings.Instance.IsWindowClient)
            {
                _classMappings.ValueChanged += new EventHandler(this.ValueChangedHandler);
            }
		}
		
		/// <summary>
		/// Initiating an instance of Selector class
		/// </summary>
		/// <param name="xmlElement">The xml element conatins data of the instance</param>
		internal Selector(XmlElement xmlElement) : base()
		{
			Unmarshal(xmlElement);
		}

		/// <summary>
		/// Gets or sets the name of destination class of this selector
		/// </summary>
		/// <value>The string for destination class name.</value>
		[BrowsableAttribute(false)]
		public string DestinationClassName
		{
			get
			{
				return _dstClassName;
			}
			set
			{
				_dstClassName = value;
			}
		}

		/// <summary>
		/// Gets the ClassMapping instances contained in a Selector
		/// </summary>
		/// <value> A collection of ClassMapping instances.</value>
		[BrowsableAttribute(false)]
		public ClassMappingCollection ClassMappings
		{
			get
			{
				return _classMappings;
			}
		}

		/// <summary>
		/// Gets the type of node
		/// </summary>
		/// <value>One of NodeType values</value>
		[BrowsableAttribute(false)]
		public override NodeType NodeType 
		{
			get
			{
				return NodeType.Selector;
			}
		}

		/// <summary>
		/// Gets or sets the SelectorManager that manages this Selector.
		/// </summary>
		/// <value>A SelectorManager instance.</value>
		[BrowsableAttribute(false)]		
		internal SelectorManager SelectorManager
		{
			get
			{
				return _manager;
			}
			set
			{
				_manager = value;
			}
		}

		/// <summary>
		/// Clear the selector
		/// </summary>
		public void Clear()
		{
			_dstClassName = null;
			_classMappings.Clear();
		}

		/// <summary>
		/// Select data from source DataSet and copy it to an instance in a destination
		/// DataSet according to the selection definitions.
		/// </summary>
		/// <param name="sourceDataSet">The DataSet representing the source data</param>
		/// <returns>The DataSet representing for destination data.</returns>
		public DataSet Select(DataSet sourceDataSet)
		{
			DataSet dstDataSet = new DataSet();
			
			// gets the default data view for the destination class in each of
			// the class mappings
			DataViewModel dstDataView = _manager.MetaData.GetDetailedDataView(DestinationClassName);

			// perform the selection at class basis
			foreach (ClassMapping classMapping in _classMappings)
			{
				classMapping.SourceDataView = _manager.MetaData.GetDetailedDataView(classMapping.SourceClassName);
				classMapping.DestinationDataView = dstDataView;
				classMapping.Select(sourceDataSet, dstDataSet, null);
			}

			return dstDataSet;
		}

		/// <summary>
		/// Add a ClassMapping instance to the Selector's class mapping collection
		/// if the mapping does not exist, otherwise, modify the existing ClassMapping instance
		/// A ClassMapping instance is identified by the source name.
		/// </summary>
		/// <param name="sourceName">The source name</param>
		/// <param name="destinationName">The destination name</param>
		/// <returns>The ClassMapping instance added or located.</returns>
		public ClassMapping AddClassMapping(string sourceName, string destinationName)
		{
			ClassMapping theClassMapping = null;

			// first to find out if a ClassMapping instance has been created for the source
			foreach (ClassMapping clsMapping in this.ClassMappings)
			{
				if (clsMapping.SourceClassName == sourceName)
				{
					theClassMapping = clsMapping;
					break;
				}
			}

			if (theClassMapping != null)
			{
				if (theClassMapping.DestinationClassName != destinationName)
				{
					// destination has been changed, clear the attribute mapping
					theClassMapping.DestinationClassName = destinationName;
					theClassMapping.Clear();
				}
			}
			else
			{
				theClassMapping = new ClassMapping(sourceName, destinationName);
				this.ClassMappings.Add(theClassMapping);
			}

			return theClassMapping;
		}

		/// <summary>
		/// Gets the first ClassMapping instance whose source class name equals
		/// to the given one.
		/// </summary>
		/// <param name="srcName">The source class name</param>
		/// <returns>The matched ClassMapping instance or null if not matched.</returns>
		public ClassMapping FindClassMappingBySrcName(string srcName)
		{
			ClassMapping found = null;

			foreach (ClassMapping classMapping in _classMappings)
			{
				if (classMapping.SourceClassName == srcName)
				{
					found = classMapping;
					break;
				}
			}

			return found;
		}

		/// <summary>
		/// create Ruleset instance from a xml document.
		/// </summary>
		/// <param name="parent">An xml element</param>
		public override void Unmarshal(XmlElement parent)
		{
			base.Unmarshal(parent);

			// set value of the _dstClassName member
			string text;
			text = parent.GetAttribute("Destination");
			if (text != null && text.Length >0)
			{
				_dstClassName = text;
			}

			// then a collection of  class mapping instances
			_classMappings = (ClassMappingCollection) MappingNodeFactory.Instance.Create((XmlElement) parent.ChildNodes[0]);
            if (GlobalSettings.Instance.IsWindowClient)
            {
                _classMappings.ValueChanged += new EventHandler(this.ValueChangedHandler);
            }
		}

		/// <summary>
		/// write Selector instance to xml document
		/// </summary>
		/// <param name="parent">An xml element for the element</param>
		public override void Marshal(XmlElement parent)
		{
			base.Marshal(parent);

			// write the _dstClassName member
			if (_dstClassName != null && _dstClassName.Length > 0)
			{
				parent.SetAttribute("Destination", _dstClassName);
			}

			// write the class mapping instances
			XmlElement child = parent.OwnerDocument.CreateElement(MappingNodeFactory.ConvertTypeToString(_classMappings.NodeType));
			_classMappings.Marshal(child);
			parent.AppendChild(child);
		}

		#region IXaclObject Members

		/// <summary>
		/// Return a xpath representation of the Selector node
		/// </summary>
		/// <returns>a xapth representation</returns>
		public string ToXPath()
		{
			if (_xpath == null)
			{
				_xpath = this.Parent.ToXPath() + "/" + this.Name;
			}

			return _xpath;
		}

		/// <summary>
		/// Return a  parent of the Taxonomy node
		/// </summary>
		/// <returns>The parent of the Taxonomy node</returns>
		[BrowsableAttribute(false)]		
		public IXaclObject Parent
		{
			get
			{
				return _manager;
			}
		}

		/// <summary>
		/// Return a  of children of the Taxonomy node
		/// </summary>
		/// <returns>The collection of IXaclObject nodes</returns>
		public IEnumerator GetChildren()
		{
            // return an empty enumerator
            ArrayList children = new ArrayList();
            return children.GetEnumerator();
		}

		#endregion
	}
}