/*
* @(#)SumFunction.cs
*
* Copyright (c) 2010 Newtera, Inc. All rights reserved.
*
*/
namespace Newtera.Function
{
	using System;
	using System.Xml;
	using System.Data;
    using System.Text;

	/// <summary>
	/// Implementation of an function that calculate the sum value of a column in a data grid
	/// </summary>
	/// <version> 1.0.0 12 Dec 2010</version>
    public class SumFunction : AggregateFunctionBase
	{
        private string _sumValue;

        /// <summary>
        /// To initialize the function
        /// </summary>
        public override void BeginCalculate()
        {
            _sumValue = null;
        }

        /// <summary>
        /// Calculate method is called for each row in a data grid
        /// </summary>
        /// <param name="fieldName">A field name of the currently processed row</param>
        /// <param name="dataRow">the currently processed data row</param>
        public override void Calculate(string fieldName, DataRow dataRow)
        {
            string fieldValue = null;

            if (!dataRow.IsNull(fieldName))
            {
                fieldValue = dataRow[fieldName].ToString();
            }

            if (_sumValue == null)
            {
                _sumValue = fieldValue;
            }
            else
            {
                _sumValue = AddValues(_sumValue, fieldValue);
            }
        }

        /// <summary>
        /// Get finalized result of the function
        /// </summary>
        /// <returns>The calculate result</returns>
        public override string EndCalculate()
        {
            if (_sumValue != null)
            {
                return _sumValue;
            }
            else
            {
                return "0";
            }
        }
	}
}