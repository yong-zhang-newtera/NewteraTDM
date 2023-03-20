/*
* @(#)ViewClass.cs
*
* Copyright (c) 2006 Newtera, Inc. All rights reserved.
*
*/
namespace Newtera.DataGridActiveX.DataGridView
{
	using System;
	using System.Xml;

	/// <summary>
	/// A ViewClass is a lite representation of a class of the class view.
	/// </summary>
	/// 
	/// <version>1.0.1 28 May 2006</version>
	public class ViewClass : DataGridViewElementBase
	{
		private string _alias;
		private string _className;
		private bool _isReferenced;

		/// <summary>
		/// Initiating an instance of ViewClass class
		/// </summary>
		/// <param name="name">Name of the element</param>
		/// <param name="className">The actual name of the class</param>
		public ViewClass(string name, string className) : base(name)
		{
			_alias = null;
			_className = className;
			_isReferenced = false; // run-time use
		}

		/// <summary>
		/// Initiating an instance of ViewClass class
		/// </summary>
		/// <param name="xmlElement">The xml element conatins data of the instance</param>
		internal ViewClass(XmlElement xmlElement) : base()
		{
			Unmarshal(xmlElement);
			_isReferenced = false; // run-time use
		}
		
		/// <summary>
		/// Gets the type of element
		/// </summary>
		/// <value>One of ViewElementType values</value>
		public override ViewElementType ElementType {
			get
			{
				return ViewElementType.Class;
			}
		}

		/// <summary>
		/// Gets the alias of the class. Tha alias is used in an XQuery
		/// </summary>
		public string Alias
		{
			get
			{
				if (_alias == null)
				{
					// using the lower case of class name as alias
					_alias = Name.ToLower();
				}

				return _alias;
			}
		}

		/// <summary>
		/// Gets or sets name of the class.
		/// </summary>
		/// <value>Name of the class</value>
		/// <remarks>The class name can be different from the element name
		/// where the element name is unique, while the class name is not among
		/// the base class and referenced classes.</remarks>
		public string ClassName
		{
			get
			{
				return _className;
			}
			set
			{
				_className = value;
			}
		}

		/// <summary>
		/// Gets or sets the information indicating whether the referenced class is
		/// referenced in the search or result parts of a query
		/// </summary>
		/// <value>true if it is referenced, false otherwise. default is false.</value>
		public bool IsReferenced
		{
			get
			{
				return _isReferenced;
			}
			set
			{
				_isReferenced = value;
			}
		}

		/// <summary>
		/// Accept a visitor of IDataGridViewElementVisitor type to traverse its
		/// elements.
		/// </summary>
		/// <param name="visitor">A visitor</param>
		public override void Accept(IDataGridViewElementVisitor visitor)
		{
			visitor.VisitClassElement(this);
		}
		
		/// <summary>
		/// sets the element members from a XML element.
		/// </summary>
		/// <param name="parent">An xml element</param>
		public override void Unmarshal(XmlElement parent)
		{
			base.Unmarshal(parent);

			// set _className member
			_className = parent.GetAttribute("ClassName");
		}

		/// <summary>
		/// Write values of members to an xml element
		/// </summary>
		/// <param name="parent">An xml element for the element</param>
		public override void Marshal(XmlElement parent)
		{
			base.Marshal(parent);

			// write the _className member
			parent.SetAttribute("ClassName", _className);
		}

		/// <summary>
		/// Text representation of the element
		/// </summary>
		/// <returns>A String</returns>
		public override string ToString()
		{
			return Caption;
		}
	}
}