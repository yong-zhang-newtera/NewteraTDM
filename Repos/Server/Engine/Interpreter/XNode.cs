/*
* @(#)XNode.cs
*
* Copyright (c) 2003 Newtera, Inc. All rights reserved.
*
*/
namespace Newtera.Server.Engine.Interpreter
{
	using System;
	using System.Xml;
	using Newtera.Server.Engine.Vdom;

	/// <summary>
	/// Represents a XmlNode value
	/// </summary>
	/// <version> 1.0.0 14 Aug 2003 </version>
	/// <author> Yong Zhang</author>
	public class XNode : Value, ISelectable
	{
		private XmlNode _content;

		/// <summary>
		/// Initiate an instance of XNode object.
		/// </summary>
		/// <param name="content">An XNode object</param>
		public XNode(XmlNode content) : base(new NodeType())
		{
			_content = content;
		}

		/// <summary>
		/// Get value content in its native type
		/// </summary>
		public XmlNode Content
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
            string val = ToString();

            if (!string.IsNullOrEmpty(val))
            {
                return System.Convert.ToBoolean(val);
            }
            else
            {
                return false; // default
            }
		}

		/// <summary>
		/// Get the value as a ValueCollection.
		/// </summary>
		/// <returns>The converted collection</returns>
		/// <exception cref="FormatException">Thrown if the convertion failed</exception>
		public override ValueCollection ToCollection()
		{
			ValueCollection values = new ValueCollection();
			values.Add(this);

			return values;
		}

		/// <summary>
		/// Get the value as a decimal value.
		/// </summary>
		/// <returns>The converted decimal</returns>
		/// <exception cref="FormatException">Thrown if the convertion failed</exception>
		public override decimal ToDecimal()
		{
            string val = ToString();

            if (!string.IsNullOrEmpty(val))
            {
                return System.Convert.ToDecimal(val);
            }
            else
            {
                return decimal.Zero; // default
            }
		}

		/// <summary>
		/// Get the value as a double value.
		/// </summary>
		/// <returns>The converted double</returns>
		/// <exception cref="FormatException">Thrown if the convertion failed</exception>
		public override double ToDouble()
		{
            string val = ToString();

            if (!string.IsNullOrEmpty(val))
            {
                return System.Convert.ToDouble(val);
            }
            else
            {
                return 0; // default
            }
		}

		/// <summary>
		/// Get the value as a float value.
		/// </summary>
		/// <returns>The converted boolean</returns>
		/// <exception cref="FormatException">Thrown if the convertion failed</exception>
		public override float ToFloat()
		{
            string val = ToString();

            if (!string.IsNullOrEmpty(val))
            {
                return System.Convert.ToSingle(val);
            }
            else
            {
                return 0; // default
            }
		}

		/// <summary>
		/// Get the value as an integer.
		/// </summary>
		/// <returns>The converted integer</returns>
		/// <exception cref="FormatException">Thrown if the convertion failed</exception>
		public override int ToInt()
		{
            string val = ToString();

            if (!string.IsNullOrEmpty(val))
            {
                return System.Convert.ToInt32(val);
            }
            else
            {
                return 0; // default
            }
		}

		/// <summary>
		/// Get the value as an XmlNode.
		/// </summary>
		/// <returns>The converted XmlNode</returns>
		/// <exception cref="FormatException">Thrown if the convertion failed</exception>
		public override XmlNode ToNode()
		{
			return _content;
		}

		/// <summary>
		/// Get the value as a string
		/// </summary>
		/// <returns>The value in string form</returns>
		public override string ToString()
		{
			string val;

			switch (_content.NodeType)
			{
				case XmlNodeType.Attribute:
					val = ((XmlAttribute) _content).Value;
					break;
				case XmlNodeType.Element:
					val = ((XmlElement) _content).InnerText;
					break;
				default:
					throw new InvalidOperationException("Unable to convert a XmlNode to a string for node type " + _content.NodeType);
			}

			return val;
		}

		/// <summary>
		/// Print the information about the value for debug purpose.
		/// </summary>
		public override void Print()
		{
			System.Console.WriteLine("XmlNode Value : " + ToString());
		}

		#region ISelectable Members

		/// <summary>
		/// Select nodes using a path.
		/// </summary>
		/// <param name="enumerator">The path enumerator</param>
		/// <returns>The XCollection containing selected nodes</returns>
		public XCollection SelectNodes(PathEnumerator enumerator)
		{
			ValueCollection nodes = ((VDocument) _content.OwnerDocument).SelectNodes(_content, enumerator);
			return new XCollection(nodes);
		}

		#endregion
	}
}