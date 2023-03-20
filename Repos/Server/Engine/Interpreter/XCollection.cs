/*
* @(#)XCollection.cs
*
* Copyright (c) 2003 Newtera, Inc. All rights reserved.
*
*/
namespace Newtera.Server.Engine.Interpreter
{
	using System;
	using System.Text;
	using System.Xml;
	using Newtera.Server.Engine.Vdom;

	/// <summary>
	/// Represents a collection of Value objects
	/// </summary>
	/// <version> 1.0.0 14 Aug 2003 </version>
	/// <author> Yong Zhang</author>
	public class XCollection : Value, ISelectable
	{
		private ValueCollection _content;

		/// <summary>
		/// Initiate an instance of XCollection object.
		/// </summary>
		/// <param name="content">A collection of Value objects</param>
		public XCollection(ValueCollection content) : base(new CollectionType())
		{
			_content = content;
		}

		/// <summary>
		/// Get value content in its native type
		/// </summary>
		public ValueCollection Content
		{
			get
			{
				return _content;
			}
		}

		/// <summary>
		/// Get the value as an boolean value.
		/// </summary>
		/// <returns>The converted boolean</returns>
		/// <exception cref="FormatException">Thrown if the convertion failed</exception>
		public override bool ToBoolean()
		{
			if (_content.Count == 1)
			{
				return _content[0].ToBoolean();
			}
			else
			{
				throw new InvalidOperationException("Unable to convert a collection to boolean value");
			}
		}

		/// <summary>
		/// Get the value as a ValueCollection.
		/// </summary>
		/// <returns>The converted collection</returns>
		/// <exception cref="FormatException">Thrown if the convertion failed</exception>
		public override ValueCollection ToCollection()
		{
            // get a flattened value collection
            ValueCollection values = new ValueCollection();
            foreach (Value val in _content)
            {
                if (val is XCollection)
                {
                    ExpanedXCollection((XCollection)val, values);
                }
                else
                {
                    values.Add(val);
                }
            }
            return values;
		}

		/// <summary>
		/// Get the value as a decimal value.
		/// </summary>
		/// <returns>The converted decimal</returns>
		/// <exception cref="FormatException">Thrown if the convertion failed</exception>
		public override decimal ToDecimal()
		{
			if (_content.Count == 1)
			{
				return _content[0].ToDecimal();
			}
			else
			{
				throw new InvalidOperationException("Unable to convert a collection to decimal value");
			}
		}

		/// <summary>
		/// Get the value as a double value.
		/// </summary>
		/// <returns>The converted double</returns>
		/// <exception cref="FormatException">Thrown if the convertion failed</exception>
		public override double ToDouble()
		{
			if (_content.Count == 1)
			{
				return _content[0].ToDouble();
			}
			else
			{
				throw new InvalidOperationException("Unable to convert a collection to double value");
			}
		}

		/// <summary>
		/// Get the value as a float value.
		/// </summary>
		/// <returns>The converted boolean</returns>
		/// <exception cref="FormatException">Thrown if the convertion failed</exception>
		public override float ToFloat()
		{
			if (_content.Count == 1)
			{
				return _content[0].ToFloat();
			}
			else
			{
				throw new InvalidOperationException("Unable to convert a collection to Float value");
			}
		}

		/// <summary>
		/// Get the value as an integer.
		/// </summary>
		/// <returns>The converted integer</returns>
		/// <exception cref="FormatException">Thrown if the convertion failed</exception>
		public override int ToInt()
		{
			if (_content.Count == 1)
			{
				return _content[0].ToInt();
			}
			else
			{
				throw new InvalidOperationException("Unable to convert a collection to integer value");
			}
		}

		/// <summary>
		/// Get the value as an XmlNode.
		/// </summary>
		/// <returns>The converted XmlNode</returns>
		/// <exception cref="FormatException">Thrown if the convertion failed</exception>
		public override XmlNode ToNode()
		{
			if (_content.Count == 0)
			{
				return null;
			}
			else if (_content.Count == 1)
			{
				return _content[0].ToNode();
			}
			else
			{
				throw new InvalidOperationException("Unable to convert a collection to XmlNode value");
			}
		}

		/// <summary>
		/// Get the value as a string
		/// </summary>
		/// <returns>The value in string form</returns>
		public override string ToString()
		{
			if (_content.Count == 1)
			{
				return _content[0].ToString();
			}
            else if (_content.Count == 0)
            {
                return ""; // return an empty string for an empty collection
            }
            else
            {
                StringBuilder builder = new StringBuilder();
                for (int i = 0; i < _content.Count; i++)
                {
                    if (i > 0)
                    {
                        builder.Append(";");
                    }

                    builder.Append(_content[i].ToString());
                }
                throw new InvalidOperationException("Unable to convert a collection to string value :" + builder.ToString());
            }
		}

		/// <summary>
		/// Print the information about the value for debug purpose.
		/// </summary>
		public override void Print()
		{
			System.Console.WriteLine("String Value : " + ToString());
		}

		#region ISelectable Members

		/// <summary>
		/// Select nodes using a path.
		/// </summary>
		/// <param name="enumerator">The path enumerator</param>
		/// <returns>The XCollection containing selected nodes</returns>
		public XCollection SelectNodes(PathEnumerator enumerator)
		{
			// Reset the enumerator to move the cursor to the beginning
			enumerator.Reset();

			PathNavigator navigator = new PathNavigator();

			// navigate through the path step by step to get the final list of nodes
			return new XCollection(navigator.Navigate(_content, enumerator));
		}

		#endregion

        private void ExpanedXCollection(XCollection collection, ValueCollection values)
        {
            ValueCollection childValues = collection.ToCollection();
            foreach (Value val in childValues)
            {
                if (val is XCollection)
                {
                    ExpanedXCollection((XCollection)val, values);
                }
                else
                {
                    values.Add(val);
                }
            }
        }
	}
}