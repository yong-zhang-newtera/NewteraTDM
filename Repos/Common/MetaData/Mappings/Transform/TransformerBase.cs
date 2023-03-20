/*
* @(#)TransformerBase.cs
*
* Copyright (c) 2003-2004 Newtera, Inc. All rights reserved.
*
*/
namespace Newtera.Common.MetaData.Mappings.Transform
{
	using System;
	using System.Xml;
	using System.Collections.Specialized;
    using System.Data;

	using Newtera.Common.MetaData.Mappings;

	/// <summary> 
	/// The base class for all transformers
	/// </summary>
	/// <version> 1.0.0 22 Nov 2004</version>
	/// <author>Yong Zhang</author>
	public abstract class TransformerBase : ITransformer
	{
        private DataTable _sourceDataTable;

		/// <summary>
		/// Initiate an instance of TransformerBase class
		/// </summary>
		public TransformerBase()
		{
            _sourceDataTable = null;
		}

        /// <summary>
        /// Gets or sets the DataTable object that represents the source data to be transformed
        /// </summary>
        public DataTable SourceDataTable
        {
            get
            {
                return _sourceDataTable;
            }
            set
            {
                _sourceDataTable = value;
            }
        }

        /// <summary>
        /// Gets a name representing the data source, usually a file name
        /// </summary>
        public string SourceName
        {
            get
            {
                if (_sourceDataTable != null)
                {
                    return _sourceDataTable.DataSet.DataSetName;
                }
                else
                {
                    return "";
                }
            }
        }

        /// <summary>
        /// An overloaded method for one-to-one transformation
        /// </summary>
        /// <param name="srcValue">A source value</param>
        /// <returns>A transformed destination value</returns>
        public virtual string OneToOneTransform(string srcValue)
		{
			return null;
		}

		/// <summary>
		/// An overloaded method for many-to-one transformation
		/// </summary>
		/// <param name="srcValues">A collection of name/value pairs representing source values</param>
		/// <returns>A transformed destination value</returns>
		public virtual string ManyToOneTransform(NameValueCollection srcValues)
		{
			return null;
		}

		/// <summary>
		/// An overloaded method for one-to-many transformation
		/// </summary>
		/// <param name="srcValue">A source value</param>
		/// <param name="dstValues">A collection of destination name/value pairs to store transformed values</param>
		public virtual void OneToManyTransform(string srcValue, NameValueCollection dstValues)
		{
		}

		/// <summary>
		/// An overloaded method for many-to-many transformation
		/// </summary>
		/// <param name="srcValues">A collection of source values</param>
		/// <param name="dstValues">A collection of destination name/value pairs to store transformed values</param>
		public virtual void ManyToManyTransform(NameValueCollection srcValues,
			NameValueCollection dstValues)
		{
		}

        /// <summary>
        /// An overloaded method for selecting a row in a datatable
        /// </summary>
        /// <param name="dt">The datatable</param>
        /// <param name="rowIndex">The current row index</param>
        /// <returns>true if the row is selected, false otherwise.</returns>
        public virtual bool IsRowSelected(System.Data.DataTable dt,
            Int32 rowIndex)
        {
            return true;
        }

        /// <summary>
        /// An overloaded method for identifying a row in a datatable
        /// </summary>
        /// <param name="srcDataTable">The source data table</param>
        /// <param name="srcRowIndex">The current row index in the source data table</param>
        /// <param name="dstDataTable">The destination data tavle</param>
        /// <returns>A zero or positive int indicating an existing row in the destination data table identified, -1 indicating no row has been identified</returns>
        public virtual int IdentifyRow(System.Data.DataTable srcDataTable,
            Int32 srcRowIndex, System.Data.DataTable dstDataTable)
        {
            return -1;
        }

		#region ITransformer interface implementation

		/// <summary>
		/// Gets the type of transformer.
		/// </summary>
		/// <value>One of the NodeType enum values</value>
		public abstract NodeType TransformType { get;}

		/// <summary>
		/// Transform a collection of source values to a collection of destination
		/// values.
		/// </summary>
		/// <param name="srcValues">A collection of name/value pairs representing source values.</param>
		/// <param name="dstValues">A collection of name/value pairs that contains transformed destination values.</param>
		public void Transform(NameValueCollection srcValues, NameValueCollection dstValues)
		{
			string dstValue;

			if (srcValues != null && dstValues != null)
			{
				switch (TransformType)
				{
					case NodeType.AttributeMapping:
						if (srcValues.Count == 1 && dstValues.Count == 1)
						{
							dstValue = OneToOneTransform(srcValues[0]);
							dstValues[dstValues.GetKey(0)] = dstValue;
						}

						break;

					case NodeType.ManyToOneMapping:

						if (dstValues.Count == 1)
						{
							dstValue = ManyToOneTransform(srcValues);
							dstValues[dstValues.GetKey(0)] = dstValue;
						}

						break;

					case NodeType.OneToManyMapping:
						if (srcValues.Count == 1 && dstValues.Count > 0)
						{
							OneToManyTransform(srcValues[0], dstValues);
						}

						break;

					case NodeType.ManyToManyMapping:

						if (srcValues.Count > 0 && dstValues.Count > 0)
						{
							ManyToManyTransform(srcValues, dstValues);
						}

						break;
				}
			}
		}

		#endregion

	}
}