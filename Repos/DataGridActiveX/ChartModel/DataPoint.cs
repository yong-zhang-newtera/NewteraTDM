/*
* @(#)DataPoint.cs
*
* Copyright (c) 2006 Newtera, Inc. All rights reserved.
*
*/
namespace Newtera.DataGridActiveX.ChartModel
{
	using System;
	using System.IO;
	using System.Xml;
    using System.Text;

	/// <summary>
	/// The class represents definition of a data point in chart.
	/// </summary>
	/// <version>1.0.0 24 Apr 2006</version>
	public class DataPoint : ChartNodeBase
	{
		private string _x;
		private string _y;
		private string _z;
		
		/// <summary>
		/// Initiate an instance of DataPoint class.
		/// </summary>
		public DataPoint() : base()
		{
			_x = null;
			_y = null;
			_z = null;
		}

		/// <summary>
		/// Initiating an instance of DataPoint class
		/// </summary>
		/// <param name="xmlElement">The xml element conatins data of the instance</param>
		internal DataPoint(XmlElement xmlElement) : base()
		{
			Unmarshal(xmlElement);
		}

		/// <summary>
		/// Gets or sets X data
		/// </summary>
		public string X
		{
			get
			{
				if (_x != null && _x.Length > 0)
				{
					return _x;
				}
				else
				{
					return null;
				}
			}
			set
			{
				_x = value;
				FireValueChangedEvent(value);
			}
		}

		/// <summary>
		/// Gets or sets Y data
		/// </summary>
		public string Y
		{
			get
			{
				if (_y != null && _y.Length > 0)
				{
					return _y;
				}
				else
				{
					return null;
				}			
			}
			set
			{
				_y = value;
				FireValueChangedEvent(value);
			}
		}

		/// <summary>
		/// Gets or sets Z data
		/// </summary>
		public string Z
		{
			get
			{
				if (_z != null && _z.Length > 0)
				{
					return _z;
				}
				else
				{
					return null;
				}			
			}
			set
			{
				_z = value;
				FireValueChangedEvent(value);
			}
		}

		/// <summary>
		/// Get x value of float type
		/// </summary>
		public float FloatX
		{
			get
			{
				float val = 0;
				if (_x != null)
				{
					try
					{
						val = float.Parse(_x);
					}
					catch (Exception)
					{
                        // x value is not a number, it could be a time in string,
                        // try to geneate a number based on the string
                        val = ConvertToSingle(_x);
					}
				}

				return val;
			}
		}

		/// <summary>
		/// Get y value of float type
		/// </summary>
		public float FloatY
		{
			get
			{
				float val = 0;
				if (_y != null)
				{
					try
					{
						val = float.Parse(_y);
					}
					catch (Exception)
					{
                        // y value is not a number, it could be a time in string,
                        // try to geneate a number based on the string
                        val = ConvertToSingle(_y);
					}
				}

				return val;
			}
		}

		/// <summary>
		/// Get z value of float type
		/// </summary>
		public float FloatZ
		{
			get
			{
				float val = 0;
				if (_z != null)
				{
					try
					{
						val = float.Parse(_z);
					}
					catch (Exception)
					{
                        // z value is not a number, it could be a time in string,
                        // try to geneate a number based on the string
                        val = ConvertToSingle(_z);
					}
				}

				return val;
			}
		}

        /// <summary>
        /// Get x value of double type
        /// </summary>
        public double DoubleX
        {
            get
            {
                double val = 0;
                if (_x != null)
                {
                    try
                    {
                        val = double.Parse(_x);
                    }
                    catch (Exception)
                    {
                        // x value is not a number, it could be a time in string,
                        // try to geneate a number based on the string
                        val = ConvertToDouble(_x);
                    }
                }

                return val;
            }
        }

        /// <summary>
        /// Get y value of double type
        /// </summary>
        public double DoubleY
        {
            get
            {
                double val = 0;
                if (_y != null)
                {
                    try
                    {
                        val = double.Parse(_y);
                    }
                    catch (Exception)
                    {
                        // y value is not a number, it could be a time in string,
                        // try to geneate a number based on the string
                        val = ConvertToDouble(_y);
                    }
                }

                return val;
            }
        }

        /// <summary>
        /// Get z value of double type
        /// </summary>
        public double DoubleZ
        {
            get
            {
                double val = 0;
                if (_z != null)
                {
                    try
                    {
                        val = double.Parse(_z);
                    }
                    catch (Exception)
                    {
                        // z value is not a number, it could be a time in string,
                        // try to geneate a number based on the string
                        val = ConvertToDouble(_z);
                    }
                }

                return val;
            }
        }

		/// <summary>
		/// Gets the type of node
		/// </summary>
		/// <value>One of NodeType values</value>
		public override NodeType NodeType
		{
			get
			{
				return NodeType.DataPoint;
			}
		}

		/// <summary>
		/// create an DataPoint from a xml document.
		/// </summary>
		/// <param name="parent">An xml element</param>
		public override void Unmarshal(XmlElement parent)
		{
			base.Unmarshal(parent);

			X = parent.GetAttribute("x");
			Y = parent.GetAttribute("y");
			Z = parent.GetAttribute("z");
		}

		/// <summary>
		/// write DataPoint to xml document
		/// </summary>
		/// <param name="parent">An xml element for the element</param>
		public override void Marshal(XmlElement parent)
		{
			base.Marshal(parent);

			if (X != null)
			{
				parent.SetAttribute("x", _x);
			}

			if (Y != null)
			{
				parent.SetAttribute("y", _y);
			}

			if (Z != null)
			{
				parent.SetAttribute("z", _z);
			}
		}

        /// <summary>
        /// Convert a string into a single
        /// </summary>
        /// <param name="val">The string</param>
        /// <returns>The converted float number</returns>
        private float ConvertToSingle(string val)
        {
            StringBuilder builder = new StringBuilder();
            int num;

            if (!string.IsNullOrEmpty(val))
            {
                for (int i = 0; i < val.Length; i++)
                {
                    try
                    {
                        num = Int32.Parse(val[i].ToString());
                        // append number together
                        builder.Append(num);
                    }
                    catch (Exception)
                    {
                        // not a number, ignore it
                    }
                }
            }

            float retNum = 0;
            try
            {
                retNum = Convert.ToSingle(builder.ToString());
            }
            catch (Exception)
            {
            }

            return retNum;
        }

        /// <summary>
        /// Convert a string into a double
        /// </summary>
        /// <param name="val">The string</param>
        /// <returns>The converted double number</returns>
        private double ConvertToDouble(string val)
        {
            StringBuilder builder = new StringBuilder();
            int num;

            if (!string.IsNullOrEmpty(val))
            {
                for (int i = 0; i < val.Length; i++)
                {
                    try
                    {
                        num = Int32.Parse(val[i].ToString());
                        // append number together
                        builder.Append(num);
                    }
                    catch (Exception)
                    {
                        // not a number, ignore it
                    }
                }
            }

            double retNum = 0;
            try
            {
                retNum = Convert.ToDouble(builder.ToString());
            }
            catch (Exception)
            {
            }

            return retNum;
        }
	}
}