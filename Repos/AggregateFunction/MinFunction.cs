/*
* @(#)MinFunction.cs
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
	/// Implementation of an function that calculate the max value of a column in a data grid
	/// </summary>
	/// <version> 1.0.0 12 Dec 2010</version>
    public class MinFunction : AggregateFunctionBase
	{
        private string _minValue;

        /// <summary>
        /// To initialize the function
        /// </summary>
        public override void BeginCalculate()
        {
            _minValue = null;
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

            if (_minValue == null)
            {
                _minValue = fieldValue;
            }
            else if (CompareValues(_minValue, fieldValue) > 0)
            {
                _minValue = fieldValue;
            }
        }

        /// <summary>
        /// Get finalized result of the function
        /// </summary>
        /// <returns>The calculate result</returns>
        public override string EndCalculate()
        {
            return _minValue;
        }
	}
}