/*
* @(#)ConstraintUsageEnum.cs
*
* Copyright (c) 2003-2014 Newtera, Inc. All rights reserved.
*
*/
namespace Newtera.Common.MetaData.Schema
{
	/// <summary>
	/// Describes the options for constarint usage
	/// </summary>
    public enum ConstraintUsage
	{
		/// <summary>
		/// Restriction,using to restrcit attribute values
		/// </summary>
		Restriction,
		/// <summary>
		/// Suggestion, using to suggest attribute values
		/// </summary>
		Suggestion
	}
}