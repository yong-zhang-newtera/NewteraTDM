/*
* @(#)ProjectModelElementCollection.cs
*
* Copyright (c) 2005 Newtera, Inc. All rights reserved.
*
*/
namespace Newtera.ParserGen.ProjectModel
{
	using System;
	using System.Text;
	using System.Xml;
	using System.Collections;
	using System.ComponentModel;

	/// <summary>
	/// An object collection class to handle IProjectModelElement when collections are
	/// returned from method calls.
	/// </summary>
	/// <version> 1.0.1 11 Nov 2005 </version>
	/// <author> Yong Zhang</author>
	public class ProjectModelElementCollection : CollectionBase, IProjectModelElement
	{
		private bool _isChanged; // run-time use only

		private IProjectModelElement _parentElement;

		/// <summary>
		/// Value changed event habdler
		/// </summary>
		public event EventHandler ValueChanged;

		/// <summary>
		///  Initializes a new instance of the ProjectModelElementCollection class.
		/// </summary>
		public ProjectModelElementCollection() : base()
		{
		}

		/// <summary>
		/// Initiating an instance of ProjectModelElementCollection class
		/// </summary>
		/// <param name="xmlElement">The xml element conatins data of the instance</param>
		internal ProjectModelElementCollection(XmlElement xmlElement) : base()
		{
			this.Unmarshal(xmlElement);
		}

		/// <summary>
		/// Implemention of Indexer member using attribute index
		/// </summary>
		public IProjectModelElement this[int index]  
		{
			get  
			{
				return( (IProjectModelElement) List[index] );
			}
			set  
			{
				// Raise the event for value change
				if (ValueChanged != null)
				{
					ValueChanged(this, new ValueChangedEventArgs("ProjectModelElementCollection", value));
				}

				value.ValueChanged += new EventHandler(ValueChangedHandler);

				List[index] = value;
			}
		}

		/// <summary>
		/// Implemention of Indexer member using attribute name
		/// </summary>
		public IProjectModelElement this[string name]  
		{
			get  
			{
				IProjectModelElement found = null;

				foreach (IProjectModelElement element in List)
				{
					if (element.Name == name)
					{
						found = element;
						break;
					}
				}

				return found;
			}
			set  
			{
			}
		}

		/// <summary>
		/// Adds an IProjectModelElement to the ProjectModelElementCollection.
		/// </summary>
		/// <param name="value">the object to be added</param>
		/// <returns>The position into which the new element was inserted</returns>
		public int Add(IProjectModelElement value )  
		{
			// Raise the event for value change
			if (ValueChanged != null)
			{
				ValueChanged(this, new ValueChangedEventArgs("ProjectModelElementCollection", value));
			}

			value.ValueChanged += new EventHandler(ValueChangedHandler);

			value.ParentElement = this.ParentElement;

			return (List.Add( value ));
		}

		/// <summary>
		/// determines the index of a specific item in the collection
		/// </summary>
		/// <param name="value">The Object to locate in the collection</param>
		/// <returns>The index of value if found in the list; otherwise, -1</returns>
		public int IndexOf(IProjectModelElement value )  
		{
			return (List.IndexOf( value ));
		}

		/// <summary>
		/// inserts an item to the collection at the specified position
		/// </summary>
		/// <param name="index">The zero-based index at which value should be inserted</param>
		/// <param name="value">The Object to insert into collection</param>
		public void Insert(int index, IProjectModelElement value)  
		{
			// Raise the event for value change
			if (ValueChanged != null)
			{
				ValueChanged(this, new ValueChangedEventArgs("ProjectModelElementCollection", value));
			}

			value.ValueChanged += new EventHandler(ValueChangedHandler);

			List.Insert(index, value );
		}

		/// <summary>
		/// removes the first occurrence of a specific object from the collection
		/// </summary>
		/// <param name="value">The Object to remove from the collection.</param>
		public void Remove(IProjectModelElement value )  
		{
			// Raise the event for value change
			if (ValueChanged != null)
			{
				ValueChanged(this, new ValueChangedEventArgs("ProjectModelElementCollection", value));
			}

			List.Remove(value);
		}

		/// <summary>
		/// determines whether the collection contains a specific value
		/// </summary>
		/// <param name="value">The Object to locate in the collection.</param>
		/// <returns>true if the Object is found in the List; otherwise, false.</returns>
		public bool Contains(IProjectModelElement value )  
		{
			// If value is not of type IProjectModelElement, this will return false.
			return (List.Contains( value ));
		}

		/// <summary>
		/// Performs additional custom processes before inserting a new element into the Collection
		/// </summary>
		/// <param name="index">The zero-based index at which to insert value.</param>
		/// <param name="value">The new value of the element at index.</param>
		protected override void OnInsert(int index, Object value)  
		{
			if (!(value is IProjectModelElement))
			{
				throw new ArgumentException( "value must be of type IProjectModelElement.", "value" );
			}
		}

		/// <summary>
		/// Performs additional custom processes when removing an element from the Collection.
		/// </summary>
		/// <param name="index">The zero-based index at which to remove value.</param>
		/// <param name="value">The new value of the element at index.</param>
		protected override void OnRemove( int index, Object value)  
		{
			if (!(value is IProjectModelElement))
			{
				throw new ArgumentException( "value must be of type IProjectModelElement.", "value" );
			}
		}

		/// <summary>
		/// Performs additional custom processes before setting a value in the Collection.
		/// </summary>
		/// <param name="index">The zero-based index at which oldValue can be found.</param>
		/// <param name="oldValue">The value to replace with newValue.</param>
		/// <param name="newValue">The new value of the element at index.</param>
		protected override void OnSet(int index, Object oldValue, Object newValue)  
		{
			if (!(newValue is IProjectModelElement))
			{
				throw new ArgumentException( "newValue must be of type IProjectModelElement.", "newValue" );
			}
		}

		/// <summary>
		/// Performs additional custom processes when validating a value.
		/// </summary>
		/// <param name="value">The object to validate.</param>
		protected override void OnValidate(Object value)  
		{
			if (!(value is IProjectModelElement))
			{
				throw new ArgumentException( "value must be of type IProjectModelElement." );
			}
		}

		/// <summary>
		/// Handler for Value Changed event fired by elements of a collection
		/// </summary>
		/// <param name="sender">The element that fires the event</param>
		/// <param name="e">The event arguments</param>
		protected virtual void ValueChangedHandler(object sender, EventArgs e)
		{
			this.IsValueChanged = true;
		}

		#region IProjectModelElement members

		/// <summary>
		/// Gets or sets the information indicating whether the value of element
		/// is changed or not
		/// </summary>
		/// <value>true if it is changed, false otherwise.</value>
		/// <remarks> Run-time use only, no need to write to data view xml</remarks>
		public bool IsValueChanged
		{
			get
			{
				return _isChanged;
			}
			set
			{
				_isChanged = value;

				// propogate the change up
				if (this.ParentElement != null)
				{
					this.ParentElement.IsValueChanged = value;
				}
			}
		}

		/// <summary>
		/// Gets the type of element
		/// </summary>
		/// <value>One of ElementType values</value>
		public virtual ElementType ElementType	{
			get
			{
				return ElementType.Collection;
			}
		}

		/// <summary>
		/// Gets the name of element
		/// </summary>
		/// <value>The collection name, default is null</value>
		public string Name
		{
			get
			{
				return null;
			}
		}

		/// <summary>
		/// Gets or sets the caption of element
		/// </summary>
		/// <value>The element caption.</value>
		public string Caption
		{
			get
			{
				return null;
			}
			set
			{
			}
		}

		/// <summary>
		/// Gets or sets position of this element among its sibling.
		/// </summary>
		/// <value>A zero-based integer representing the position.</value>
		public int Position
		{
			get
			{
				return 0;
			}
			set
			{
			}
		}
		
		/// <summary>
		/// Gets or sets the description of element
		/// </summary>
		/// <value>The element description.</value>
		public string Description
		{
			get
			{
				return null;
			}
			set
			{
			}
		}

		/// <summary>
		/// Gets or sets the parent element of the element
		/// </summary>
		[BrowsableAttribute(false)]	
		public IProjectModelElement ParentElement
		{
			get
			{
				return _parentElement;
			}
			set
			{
				_parentElement = value;
				for (int i = 0; i < this.Count; i++)
				{
					this[i].ParentElement = value;
				}
			}
		}

		/// <summary>
		/// Reset the value change status in the project model
		/// </summary>
		public virtual void Reset()
		{
		}

		/// <summary>
		/// Accept a visitor of IProjectModelElementVisitor type to traverse its
		/// elements.
		/// </summary>
		/// <param name="visitor">A visitor</param>
		public virtual void Accept(IProjectModelElementVisitor visitor)
		{
			foreach (IProjectModelElement element in this.List)
			{
				element.Accept(visitor);
			}
		}

		/// <summary>
		/// sets the element members from a XML element.
		/// </summary>
		/// <param name="parent">An xml element</param>
		public virtual void Unmarshal(XmlElement parent)
		{
			foreach (XmlElement xmlElement in parent.ChildNodes)
			{
				IProjectModelElement element = ElementFactory.Instance.Create(xmlElement);

				this.Add(element);
				element.ParentElement = this.ParentElement;
			}
		}

		/// <summary>
		/// Write values of members to an xml element
		/// </summary>
		/// <param name="parent">An xml element for the element</param>
		public virtual void Marshal(XmlElement parent)
		{
			XmlElement child;

			foreach (IProjectModelElement element in List)
			{
				child = parent.OwnerDocument.CreateElement(ElementFactory.ConvertTypeToString(element.ElementType));
				element.Marshal(child);
				parent.AppendChild(child);
			}
		}

		#endregion
	}
}