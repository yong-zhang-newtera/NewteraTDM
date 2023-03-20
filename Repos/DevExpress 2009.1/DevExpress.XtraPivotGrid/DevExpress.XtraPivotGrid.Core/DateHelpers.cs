#region Copyright (c) 2000-2009 Developer Express Inc.
/*
{*******************************************************************}
{                                                                   }
{       Developer Express .NET Component Library                    }
{                                        }
{                                                                   }
{       Copyright (c) 2000-2009 Developer Express Inc.              }
{       ALL RIGHTS RESERVED                                         }
{                                                                   }
{   The entire contents of this file is protected by U.S. and       }
{   International Copyright Laws. Unauthorized reproduction,        }
{   reverse-engineering, and distribution of all or any portion of  }
{   the code contained in this file is strictly prohibited and may  }
{   result in severe civil and criminal penalties and will be       }
{   prosecuted to the maximum extent possible under the law.        }
{                                                                   }
{   RESTRICTIONS                                                    }
{                                                                   }
{   THIS SOURCE CODE AND ALL RESULTING INTERMEDIATE FILES           }
{   ARE CONFIDENTIAL AND PROPRIETARY TRADE                          }
{   SECRETS OF DEVELOPER EXPRESS INC. THE REGISTERED DEVELOPER IS   }
{   LICENSED TO DISTRIBUTE THE PRODUCT AND ALL ACCOMPANYING .NET    }
{   CONTROLS AS PART OF AN EXECUTABLE PROGRAM ONLY.                 }
{                                                                   }
{   THE SOURCE CODE CONTAINED WITHIN THIS FILE AND ALL RELATED      }
{   FILES OR ANY PORTION OF ITS CONTENTS SHALL AT NO TIME BE        }
{   COPIED, TRANSFERRED, SOLD, DISTRIBUTED, OR OTHERWISE MADE       }
{   AVAILABLE TO OTHER INDIVIDUALS WITHOUT EXPRESS WRITTEN CONSENT  }
{   AND PERMISSION FROM DEVELOPER EXPRESS INC.                      }
{                                                                   }
{   CONSULT THE END USER LICENSE AGREEMENT FOR INFORMATION ON       }
{   ADDITIONAL RESTRICTIONS.                                        }
{                                                                   }
{*******************************************************************}
*/
#endregion Copyright (c) 2000-2009 Developer Express Inc.

using System;
using System.Globalization;
using System.Collections.Generic;
namespace DevExpress.Utils.DateHelpers {
	public class DateHelper {
		public static int GetWeekOfMonth(DateTime dateTime) {
			DateTime weekDateTime = new DateTime(dateTime.Year, dateTime.Month, 1);
			return (dateTime.Day - 1 + (int)weekDateTime.DayOfWeek) / 7 + 1;
		}
		public static int GetWeekOfYear(DateTime dateTime) {
			CultureInfo cultureInfo = CultureInfo.CurrentCulture;
			return cultureInfo.Calendar.GetWeekOfYear(dateTime, cultureInfo.DateTimeFormat.CalendarWeekRule, cultureInfo.DateTimeFormat.FirstDayOfWeek);
		}
		public static int GetFullYears(DateTime start, DateTime end) {
			int fullYear = end.Year - start.Year;
			if(fullYear > 0) {
				if(end.Month < start.Month) {
					fullYear--;
				} else {
					if(end.Month == start.Month && end.Day < start.Day) {
						fullYear--;
					}
				}
			}
			return fullYear;
		}
		public static int GetFullMonths(DateTime start, DateTime end) {
			int fullMonth = 0;
			if(start.Year == end.Year) {
				fullMonth = end.Month - start.Month;
			} else {
				fullMonth = 12 - start.Month + end.Month;
				if(end.Year > start.Year + 1) {
					fullMonth += 12 * (end.Year - start.Year - 1);
				}
			}
			if(fullMonth > 0) {
				if(end.Day < start.Day) {
					fullMonth--;
				}
			}
			return fullMonth;
		}
		public static int GetFullWeeks(DateTime start, DateTime end) {
			return (int)(GetFullDays(start, end) / 7);
		}
		public static int GetFullDays(DateTime start, DateTime end) {
			return (end - start).Days;
		}
		public static int CompareDayOfWeek(DayOfWeek val1, DayOfWeek val2) {
			return Comparer<int>.Default.Compare(GetAbsDayOfWeek(val1), GetAbsDayOfWeek(val2));
		}
		static int GetAbsDayOfWeek(DayOfWeek val) {
			if(val < CultureInfo.CurrentCulture.DateTimeFormat.FirstDayOfWeek)
				return 7 + (int)val;
			else
				return (int)val;
		}
	}
}
