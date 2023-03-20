/*
* @(#)Range.cs
*
* Copyright (c) 2003 Newtera, Inc. All rights reserved.
*
*/
namespace Newtera.Server.Engine.Sqlbuilder.Sql
{
	using System;
	
	/// <summary>
	/// A Range object represents the a range of result rows to be returned for a
	/// query.
	/// </summary>
	/// <version>  	1.0.1 04 Jul 2003 </version>
	/// <author>Yong Zhang </author>
	public class Range
	{
		public const int DEFAULT_PAGE_SIZE = 100;
		public const int MAX_PAGE_SIZE = 1000;
		
		// private instance members
		private int _from;
		private int _to;
		
		/// <summary>
		/// Initiating a Range object
		/// </summary>
		public Range()
		{
			_from = 1;
			_to = Range.DEFAULT_PAGE_SIZE;
		}
		
		/// <summary>
		/// Gets from vaue of the range
		/// </summary>
		/// <value> from value in integer</value>
		public int From
		{
			get
			{
				return _from;
			}
			set
			{
				_from = value;
			}
		}

		/// <summary>
		/// Get to vaue of the range.
		/// </summary>
		/// <value> to value in integer.</value>
		public int To
		{
			get
			{
				return _to;
			}
			set
			{
				_to = value;
			}
		}
	}
}