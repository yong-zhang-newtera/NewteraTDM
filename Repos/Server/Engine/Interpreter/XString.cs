/*
* @(#)XString.cs
*
* Copyright (c) 2003 Newtera, Inc. All rights reserved.
*
*/
namespace Newtera.Server.Engine.Interpreter
{
	using System;
	using System.Xml;

	/// <summary>
	/// Represents a string value
	/// </summary>
	/// <version> 1.0.0 14 Aug 2003 </version>
	/// <author> Yong Zhang</author>
	public class XString : Value
	{
		private string _content;

		/// <summary>
		/// Initiate an instance of XString object.
		/// </summary>
		/// <param name="content">The content of XString object</param>
		public XString(string content) : base(new StringType())
		{
			_content = content;
		}

		/// <summary>
		/// Get value content in its native type
		/// </summary>
		public string Content
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
			return System.Convert.ToInt32(_content);
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
            element.InnerText = _content;
            return element;
		}

		/// <summary>
		/// Get the value as a string
		/// </summary>
		/// <returns>The value in string form</returns>
		public override string ToString()
		{
			return _content;
		}

		/// <summary>
		/// Print the information about the value for debug purpose.
		/// </summary>
		public override void Print()
		{
			System.Console.WriteLine("String Value : " + ToString());
		}
	}
}