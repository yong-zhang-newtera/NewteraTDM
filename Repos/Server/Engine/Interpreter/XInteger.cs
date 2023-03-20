/*
* @(#)XInteger.cs
*
* Copyright (c) 2003 Newtera, Inc. All rights reserved.
*
*/
namespace Newtera.Server.Engine.Interpreter
{
	using System;
	using System.Xml;

	/// <summary>
	/// Represents an integer value
	/// </summary>
	/// <version> 1.0.0 14 Aug 2003 </version>
	/// <author> Yong Zhang</author>
	public class XInteger : Value
	{
		private int _content;

		/// <summary>
		/// Initiate an instance of XInteger object.
		/// </summary>
		/// <param name="content">The content of XInteger object</param>
		public XInteger(int content) : base(new IntegerType())
		{
			_content = content;
		}

		/// <summary>
		/// Initiate an instance of XInteger object.
		/// </summary>
		/// <param name="content">The content of XInteger object</param>
		public XInteger(string content) : base(new IntegerType())
		{
			_content = System.Convert.ToInt32(content);
		}

		/// <summary>
		/// Get value content in its native type
		/// </summary>
		public int Content
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
			return System.Convert.ToBoolean(_content);
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
			return System.Convert.ToDecimal(_content);
		}

		/// <summary>
		/// Get the value as a double value.
		/// </summary>
		/// <returns>The converted double</returns>
		/// <exception cref="FormatException">Thrown if the convertion failed</exception>
		public override double ToDouble()
		{
			return System.Convert.ToDouble(_content);
		}

		/// <summary>
		/// Get the value as a float value.
		/// </summary>
		/// <returns>The converted boolean</returns>
		/// <exception cref="FormatException">Thrown if the convertion failed</exception>
		public override float ToFloat()
		{
			return System.Convert.ToSingle(_content);
		}

		/// <summary>
		/// Get the value as an integer.
		/// </summary>
		/// <returns>The converted integer</returns>
		/// <exception cref="FormatException">Thrown if the convertion failed</exception>
		public override int ToInt()
		{
			return _content;
		}

		/// <summary>
		/// Get the value as an XmlNode.
		/// </summary>
		/// <returns>The converted XmlNode</returns>
		/// <exception cref="FormatException">Thrown if the convertion failed</exception>
		public override XmlNode ToNode()
		{
            XmlDocument doc = new XmlDocument();
            XmlElement element = doc.CreateElement("XmlNode");
            element.InnerText = _content.ToString();
            return element;
        }

		/// <summary>
		/// Get the value as a string
		/// </summary>
		/// <returns>The value in string form</returns>
		public override string ToString()
		{
			return System.Convert.ToString(_content);
		}

		/// <summary>
		/// Print the information about the value for debug purpose.
		/// </summary>
		public override void Print()
		{
			System.Console.WriteLine("Integer Value : " + ToString());
		}
	}
}