/*
* @(#)ParameterCollection.cs
*
* Copyright (c) 2003 Newtera, Inc. All rights reserved.
*
*/
namespace Newtera.Common.MetaData.DataView
{
	using System;
	using System.Text;
	using System.Xml;

    using Newtera.Common.MetaData.Schema;

	/// <summary>
	/// Represents a collection of parameters in "in" or "not in" expressions.
	/// </summary>
	/// <version>1.0.1 29 Nov 2003</version>
	/// <author>Yong Zhang</author>
	public class ParameterCollection : DataViewElementCollection, IQueryElement, IParameter
	{
		/// <summary>
		/// Initiating an instance of ParameterCollection class
		/// </summary>
		public ParameterCollection() : base()
		{
		}
		
		/// <summary>
		/// Initiating an instance of ParameterCollection class
		/// </summary>
		/// <param name="xmlElement">The xml element conatins data of the instance</param>
		internal ParameterCollection(XmlElement xmlElement) : base(xmlElement)
		{
		}

		/// <summary>
		/// Gets the type of element
		/// </summary>
		/// <value>One of ElementType values</value>
		public override ElementType ElementType {
			get
			{
				return ElementType.Parameters;
			}
		}

		/// <summary>
		/// Accept a visitor of IDataViewElementVisitor type to traverse its
		/// elements.
		/// </summary>
		/// <param name="visitor">A visitor</param>
		public override void Accept(IDataViewElementVisitor visitor)
		{
			if (visitor.VisitParametersBegin(this))
			{
				foreach (IDataViewElement element in List)
				{
					element.Accept(visitor);
				}
			}

			visitor.VisitParametersEnd(this);
		}

        #region IParameter members

        /// <summary>
        /// Gets information indicating whether the parameters has value.
        /// </summary>
        /// <returns>true if the parameter has value, false otherwise.</returns>
        public bool HasValue
        {
            get
            {
                bool hasValue = false;
                foreach (IDataViewElement element in List)
                {
                    if (element is IParameter && ((IParameter)element).HasValue)
                    {
                        hasValue = true;
                        break;
                    }
                }

                return hasValue;
            }
        }

        /// <summary>
        /// If the paramter's value is composed of multiple values separated by "&",
        /// this method return the same type of IParameter that contains a single value
        /// whose position is indicated by the index.
        /// </summary>
        /// <param name="index">The value index</param>
        /// <param name="name">Parameter name</param>
        /// <param name="dataType">The data type of parameter</param>
        /// <returns>An IParameter whose value is standalone, null if there isn't a value at the given index.</returns>
        public IParameter GetParameterByIndex(int index, string name, DataType dataType)
        {
            ParameterCollection parameters = new ParameterCollection();
            IDataViewElement parameter;

            foreach (IDataViewElement element in List)
            {
                if (element is IParameter)
                {
                    parameter = (IDataViewElement)((IParameter)element).GetParameterByIndex(index, name, dataType);

                    if (parameter != null)
                    {
                        parameters.Add(parameter);
                    }
                }
            }

            if (parameters.Count > 0)
            {
                return parameters;
            }
            else
            {
                return null;
            }
        }

        #endregion

		#region IQueryElement Members

		/// <summary>
		/// Gets the XQuery representation of the element.
		/// </summary>
		/// <returns>A XQuery segmentation</returns>
		public string ToXQuery()
		{
			StringBuilder builder = new StringBuilder();

			builder.Append("(");

			int index = 0;
			foreach (IQueryElement element in List)
			{
				builder.Append(element.ToXQuery());
				if (index < List.Count - 1)
				{
					builder.Append(", ");
				}

				index++;
			}

			builder.Append(")");

			return builder.ToString();
		}

		#endregion
	}
}