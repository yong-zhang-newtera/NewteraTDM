/*
* @(#)CurveFit.cs
*
* Copyright (c) 2003-2004 Newtera, Inc. All rights reserved.
*
*/
namespace Newtera.Util
{
	using System;
	using System.Xml;
	using System.IO;
	using System.Data;
	using System.Text;
	using System.ComponentModel;
	using System.Collections;

	using Newtera.Common.MetaData.DataView;

	/// <summary> 
	/// The class implements the CurveFit algorithm (曲线拟合)
	/// </summary>
	/// <version> 1.0.0 09 Apr 2005</version>
	public class CurveFit
	{
		private const int MAX_EXPONENT = 25;

		private int _exponent;
		private double[][] _coefficients;
		private DataTable _arrayDataTable;
		private int _xAxisIndex;

		/// <summary>
		/// Initiate an instance of CurveFit class
		/// </summary>
		public CurveFit()
		{
			_exponent = 0;
			_coefficients = null;
			_arrayDataTable = null;
			_xAxisIndex = 0;
		}

		/// <summary>
		/// Gets or sets the exponent for the polynomial
		/// </summary>
		/// <value>The max exponent of the polynomial, the default is 0, and maximum is 25 </value>
		public int Exponent
		{
			get
			{
				return _exponent;
			}
			set
			{
				if (value < 0)
				{
					_exponent = 0;
				}
				else if (value > MAX_EXPONENT)
				{
					_exponent = MAX_EXPONENT;
				}
				else
				{
					_exponent = value;
				}
			}
		}

		/// <summary>
		/// Gets or sets the array data table that contains experiment data for curves
		/// </summary>
		public DataTable ArrayDataTable
		{
			get
			{
				return _arrayDataTable;
			}
			set
			{
				_arrayDataTable = value;
			}
		}

		/// <summary>
		/// Gets or sets the index of x axis in the array data table
		/// </summary>
		public int XAxisIndex
		{
			get
			{
				return _xAxisIndex;
			}
			set
			{
				if (value < 0)
				{
					_xAxisIndex = 0;
				}
				else
				{
					_xAxisIndex = value;
				}
			}
		}

		/// <summary>
		/// Perform the algorithm and insert points into the data array
		/// </summary>
		/// <param name="beforeData">The original array data</param>
		/// <param name="xAxisIndex">X Axis Index in the given array data.</param>
		/// <param name="points">The points to be inserted in the array data.</param>
		/// <returns>The array data calculated using curve fit polynomials.</returns>
		public DataTable Compute(DataTable beforeData, int xAxisIndex, double[] points)
		{
			DataTable afterData = beforeData.Copy();

			if (_coefficients == null)
			{
				CalculateCoefficients();
			}

			// 添加插值点
			if (points != null)
			{
				int rowIndex;
				for (int i = 0; i < points.Length; i++)
				{
					rowIndex = FindInsertRowIndex(afterData, xAxisIndex, points[i]);

					if (rowIndex > 0)
					{
						// instantiate a new row with calculated values
						DataRow newRow = afterData.NewRow();

						newRow[xAxisIndex] = points[i];

						// insert the new row
						afterData.Rows.InsertAt(newRow, rowIndex);
					}
				}
			}

			if (_coefficients != null)
			{
				for (int i = 0; i < afterData.Columns.Count; i++)
				{
					if (i != this._xAxisIndex)
					{
						for (int j = 0; j < afterData.Rows.Count; j++)
						{
							// calculate y value using curve fit polynomial
							double x = 0.0;
							try
							{
								x = double.Parse(afterData.Rows[j][xAxisIndex].ToString());
							}
							catch (Exception)
							{
								x = 0.0;
							}

							afterData.Rows[j][i] = this.CalculateValue(x, i);
						}
					}
				}
			}

			return afterData;
		}

		/// <summary>
		/// Find the insert position where to add a new row in the datatable
		/// </summary>
		/// <param name="dataTable">The data table</param>
		/// <param name="xAxisIndex">The x axis index</param>
		/// <param name="point">The point to be inserted</param>
		/// <returns>The row index where the new row to be inserted</returns>
		/// <remarks>Find the row index that satisfy xi-1 < point < xi, assuming that
		/// x0 < x1 < .... < xn</remarks>
		private int FindInsertRowIndex(DataTable dataTable, int xAxisIndex, double point)
		{
			int rowIndex = -1;
	
			for (int i = 0; i < dataTable.Rows.Count; i++)
			{
				try
				{
					double x = double.Parse(dataTable.Rows[i][xAxisIndex].ToString());
					if (x >= point)
					{
						// found
						rowIndex = i;
						break;
					}
				}
				catch(Exception)
				{
				}
			}

			return rowIndex;
		}

		/// <summary>
		/// Get the coefficient of an exponent in a curve fit polynomial for an y axis.
		/// </summary>
		/// <param name="exponent">The x value</param>
		/// <param name="yAxisIndex">Indicate which y axis the polynomial is for. </param>
		/// <returns>The coefficient of the exponent.</returns>
		public double GetCoefficient(int exponent, int yAxisIndex)
		{
			double cofficient = 0;
			
			if (_coefficients == null)
			{
				CalculateCoefficients();
			}

			if (_coefficients != null)
			{
				if (exponent >= 0 && exponent < _coefficients[0].Length)
				{
					cofficient = _coefficients[yAxisIndex][exponent];
				}
			}

			return cofficient;
		}

		/// <summary>
		/// Calculate Y value for a X value using the Curve Fit polynomial.
		/// </summary>
		/// <param name="x">The x value</param>
		/// <param name="yAxisIndex">Indicate which y axis to calculate the Y value. </param>
		/// <returns>The calculated Y value.</returns>
		private double CalculateValue(double x, int yAxisIndex)
		{
			double val = 0;
			
			if (_coefficients == null)
			{
				CalculateCoefficients();
			}

			if (_coefficients != null)
			{
				// calculate y value using curve fit polynomial in the form of
				// y(x) = C0 * x ** 0 + C1 * x ** 1 + C2 * x ** 2 + ... + Cn * x ** n
				// where n is the power degree
				for (int i = 0; i <= this._exponent; i ++)
				{
					val += _coefficients[yAxisIndex][i] * System.Math.Pow(x, i);
				}
			}

			return val;
		}

		/// <summary>
		/// Calculate the coefficients of polynomials for all Y axis in the DataTable
		/// </summary>
		private void CalculateCoefficients()
		{
			InitializeCoefficients();
			double[] xs = new double[_arrayDataTable.Rows.Count];
			double[] ys = new double[_arrayDataTable.Rows.Count];

			if (_coefficients != null)
			{
				// build x array
				for (int i = 0; i < _arrayDataTable.Rows.Count; i++)
				{
					double x = 0.0;
					try
					{
						x = double.Parse(_arrayDataTable.Rows[i][XAxisIndex].ToString());
					}
					catch (Exception)
					{
						x = 0.0;
					}

					xs[i] = x;
				}

				// 调用曲线回归函数来计算各个多项式的常数
				for (int i = 0; i < _coefficients.Length; i++)
				{
					// calculate coefficients for each Y axis in data table
					if (i != this.XAxisIndex)
					{
						// build y array
						for (int j = 0; j < _arrayDataTable.Rows.Count; j++)
						{
							double y = 0.0;
							try
							{
								y = double.Parse(_arrayDataTable.Rows[j][i].ToString());
							}
							catch (Exception)
							{
								y = 0.0;
							}

							ys[j] = y;
						}

						CalculateCurveParameter(xs, ys, Exponent,  _arrayDataTable.Rows.Count,
							_coefficients[i]);
					}
				}
			}
		}

		/// <summary>
		/// Initialize an 2-D array to store coefficients
		/// </summary>
		private void InitializeCoefficients()
		{
			if (_arrayDataTable != null)
			{
				int size = _exponent + 1;
				// create the two-dimenssional array to store the calculated cofficients
				_coefficients = new double[_arrayDataTable.Columns.Count][];
				for (int i = 0; i < _arrayDataTable.Columns.Count; i++)
				{
					_coefficients[i] = new double[size];
				}
			}
		}


		/// <summary>
		/// 最小二乘法曲线拟合.
		/// </summary>
		/// <param name="X">X轴值</param>
		/// <param name="Y">Y轴值</param>
		/// <param name="M">结果变量组数</param>
		/// <param name="N">采样数目</param>
		/// <param name="A">结果参数</param>
		private void CalculateCurveParameter(double[] X, double[] Y, int M, int N, double[] A)
		{
			int i, j, k;
			double Z,D1,D2,C,P,G,Q;
			double[] B,T,S;

			B = new double[N];
			T = new double[N];
			S = new double[N];
			
			if (M>N) 
			{
				M=N;
			}

			for (i=0; i<M; i++)
			{
				A[i] = 0;
			}

			Z = 0;
			B[0] = 1;
			D1 = N;
			P = 0;
			C = 0;
			Q = 0;

			for (i=0; i<N; i++)
			{
				P += X[i] - Z;
				C += Y[i];
			}

			C = C/D1;
			P = P/D1;
			A[0] = C * B[0];

			if ( M>1 )
			{
				T[1] = 1;
				T[0] = -P;
				D2 = 0;
				C = 0;
				G = 0;
				for(i=0; i<N; i++)
				{
					Q = X[i] - Z-P;
					D2 = D2 + Q*Q;
					C = Y[i]*Q + C;
					G = (X[i]-Z) * Q * Q + G;
				}

				C = C/D2;
				P = G/D2;
				Q = D2/D1;
				D1 = D2;
				A[1] = C * T[1];
				A[0] = C * T[0] + A[0];
			}

			for(j=2; j<M; j++)
			{
				S[j] = T[j-1];
				S[j-1] = -P * T[j-1] + T[j-2];
				if (j>=3)
				{
					for(k=j-2; k>=1; k--)
					{
						S[k] = -P * T[k] + T[k-1] - Q * B[k];
					}
				}

				S[0] = -P * T[0] - Q * B[0];
				D2=0;
				C=0;
				G=0;
				for (i=0; i<N; i++)
				{
					Q=S[j];
					for(k=j-1; k>=0; k--)
					{
						Q = Q * (X[i] - Z) + S[k];
					}

					D2 = D2 + Q*Q;
					C = Y[i] * Q + C;
					G=(X[i] - Z) * Q * Q + G;
				}

				C = C/D2;
				P = G/D2;
				Q = D2/D1;
				D1 = D2;
				A[j] = C * S[j];
				T[j] = S[j];
				for (k=j-1; k>=0; k--)
				{
					A[k] = C * S[k] + A[k];
					B[k]=T[k];
					T[k]=S[k];
				}
			}
		}
	}
}