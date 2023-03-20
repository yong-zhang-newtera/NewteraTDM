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
using System.Collections;
using System.ComponentModel;
using System.ComponentModel.Design;
using System.Data;
using System.Data.OleDb;
using System.Runtime.Serialization;
using System.IO;
using System.Globalization;
using System.Collections.Generic;
using DevExpress.Data;
using DevExpress.Data.Helpers;
using DevExpress.Data.PivotGrid;
using DevExpress.XtraPivotGrid.Data;
using DevExpress.Utils.Controls;
using DevExpress.Utils.Design;
using DevExpress.Utils;
using DevExpress.Utils.Serializing;
using DevExpress.XtraPivotGrid.Localization;
using DevExpress.Data.IO;
using DevExpress.WebUtils;
using System.Text;
using System.Diagnostics;
using Microsoft.Win32;
using System.Drawing;
using System.IO.Compression;
using System.Windows.Forms;
using System.Windows.Forms.Design;
using System.Reflection;
namespace DevExpress.XtraPivotGrid.Data {
	class OLAPSchemaGuid {
		public static readonly Guid Catalogs = new Guid("C8B52211-5CF3-11CE-ADE5-00AA0044773D");
		public static readonly Guid Cubes = new Guid("C8B522D8-5CF3-11CE-ADE5-00AA0044773D");
		public static readonly Guid Dimensions = new Guid("C8B522D9-5CF3-11CE-ADE5-00AA0044773D");
		public static readonly Guid Hierarchies = new Guid("C8B522DA-5CF3-11CE-ADE5-00AA0044773D");
		public static readonly Guid Measures = new Guid("C8B522DC-5CF3-11CE-ADE5-00AA0044773D");
		public static readonly Guid KPIs = new Guid("2AE44109-ED3D-4842-B16F-B694D1CB0E3F");
		public static readonly Guid Members = new Guid("C8B522DE-5CF3-11CE-ADE5-00AA0044773D");
		public static readonly Guid Levels = new Guid("C8B522DB-5CF3-11CE-ADE5-00AA0044773D");
		public static readonly Guid Sets = new Guid("A07CCD0B-8148-11D0-87BB-00C04FC33942");
		public static readonly Guid Properties = new Guid("C8B522DD-5CF3-11CE-ADE5-00AA0044773D");
	}
	class OleDbTypeConverter {
		public static Type Convert(OleDbType type) {
			switch(type) {
				case OleDbType.BigInt: return typeof(Int64);
				case OleDbType.Binary: return typeof(Byte);
				case OleDbType.Boolean: return typeof(Boolean);
				case OleDbType.BSTR: return typeof(String);
				case OleDbType.Char: return typeof(String);
				case OleDbType.Currency: return typeof(Decimal);
				case OleDbType.Date: return typeof(DateTime);
				case OleDbType.DBDate: return typeof(DateTime);
				case OleDbType.DBTime: return typeof(TimeSpan);
				case OleDbType.DBTimeStamp: return typeof(DateTime);
				case OleDbType.Decimal: return typeof(Decimal);
				case OleDbType.Double: return typeof(Double);
				case OleDbType.Empty: return null;
				case OleDbType.Error: return typeof(Exception);
				case OleDbType.Filetime: return typeof(DateTime);
				case OleDbType.Guid: return typeof(Guid);
				case OleDbType.IDispatch: return typeof(Object);
				case OleDbType.Integer: return typeof(Int32);
				case OleDbType.IUnknown: return typeof(Object);
				case OleDbType.LongVarBinary: return typeof(Byte);
				case OleDbType.LongVarChar: return typeof(String);
				case OleDbType.LongVarWChar: return typeof(String);
				case OleDbType.Numeric: return typeof(Decimal);
				case OleDbType.PropVariant: return typeof(Object);
				case OleDbType.Single: return typeof(Single);
				case OleDbType.SmallInt: return typeof(Int16);
				case OleDbType.TinyInt: return typeof(SByte);
				case OleDbType.UnsignedBigInt: return typeof(UInt64);
				case OleDbType.UnsignedInt: return typeof(UInt32);
				case OleDbType.UnsignedSmallInt: return typeof(UInt16);
				case OleDbType.UnsignedTinyInt: return typeof(Byte);
				case OleDbType.VarBinary: return typeof(Byte);
				case OleDbType.VarChar: return typeof(String);
				case OleDbType.Variant: return typeof(Object);
				case OleDbType.VarNumeric: return typeof(Decimal);
				case OleDbType.VarWChar: return typeof(String);
				case OleDbType.WChar: return typeof(String);
			}
			return null;
		}
	}
	class OLAPTuple {
		readonly OLAPMember[] members;
		string flattenedString;
		OLAPGroupInfo baseGroup;
		public int MemberCount { get { return members.Length; } }
		public OLAPMember this[int index] { get { return members[index]; } }
		public OLAPMember Last {
			get {
				if(MemberCount == 0) return null;
				return this[MemberCount - 1];
			}
		}
		public OLAPGroupInfo BaseGroup { get { return baseGroup; } set { baseGroup = value; } }
		public OLAPTuple(params OLAPMember[] members) {
			this.members = new OLAPMember[members == null ? 0 : members.Length];
			if(members != null) members.CopyTo(this.members, 0);
		}
		public OLAPTuple(OLAPGroupInfo baseGroup, params OLAPMember[] members)
			: this(members) {
			this.baseGroup = baseGroup;
		}
		public OLAPTuple(OLAPGroupInfo group, OLAPAreaFieldValues fieldValues) {
			OLAPGroupInfo[] hierarchy = fieldValues.GetHierarchy(group);
			List<OLAPMember> members = new List<OLAPMember>();
			for(int i = 0; i < hierarchy.Length; i++) {
				Debug.Assert(!hierarchy[i].Member.IsTotal && !hierarchy[i].Member.IsOthers);
				if(i == hierarchy.Length - 1 || hierarchy[i + 1].Column.ParentColumn != hierarchy[i].Column)
					members.Add(hierarchy[i].Member);
			}
			this.members = members.ToArray();
			this.baseGroup = group;
		}
		public override bool Equals(object obj) {
			OLAPTuple tuple = obj as OLAPTuple;
			if(tuple == null || tuple.MemberCount != MemberCount) return false;
			for(int i = 0; i < MemberCount; i++)
				if(tuple[i] != this[i]) return false;
			return true;
		}
		public override string ToString() {
			StringBuilder result = new StringBuilder();
			result.Append("{ ");
			for(int i = 0; i < MemberCount; i++) {
				result.Append(this[i]);
				if(i != MemberCount - 1) result.Append(", ");
			}
			result.Append(" }");
			return result.ToString();
		}
		public string FlattenedString {
			get {
				if(string.IsNullOrEmpty(flattenedString)) {
					StringBuilder builder = new StringBuilder();
					for(int i = 0; i < MemberCount; i++) {
						if(i != MemberCount - 1 && this[i].Column.IsParent(this[i + 1].Column)) continue;
						builder.Append(this[i].UniqueName);
						if(i != MemberCount - 1) builder.Append(".");
					}
					flattenedString = builder.ToString();
				}
				return flattenedString;
			}
		}
		public override int GetHashCode() {
			return Last.UniqueName.GetHashCode();
		}
	}
	class QueryTempMember {
		bool isMember;
		string name, mdx;
		public bool IsMember { get { return isMember; } }
		public string Name { get { return name; } }
		public string MDX { get { return mdx; } protected set { mdx = value; } }
		public QueryTempMember(bool isMember, string name, string mdx) {
			this.isMember = isMember;
			this.name = name;
			this.mdx = mdx;
		}
		public override string ToString() {
			return Name;
		}
	}
	abstract class SortByTempMember : QueryTempMember {
		static string GetName(OLAPCubeColumn column, bool measure) {
			return (measure ? OLAPDataSourceQueryBase.MeasuresString : column.Hierarchy.UniqueName) + 
				".[XtraPivotGrid " + column.GetHashCode().ToString() + " Sort]"; 
		}
		OLAPCubeColumn column;
		bool isMeasure;
		public SortByTempMember(OLAPCubeColumn column, bool isMeasure)
			: base(true, GetName(column, isMeasure), "") {
			this.column = column;
			this.isMeasure = isMeasure;
			MDX = GetSortBy(column);
		}
		public OLAPCubeColumn Column { get { return column; } }
		public bool IsMeasure { get { return isMeasure; } }
		protected abstract string GetSortBy(OLAPCubeColumn column);		
	}
	class SortByTempMember2005 : SortByTempMember {
		public SortByTempMember2005(OLAPCubeColumn column, bool isMeasure)
			: base(column, isMeasure) {
		}
		protected override string GetSortBy(OLAPCubeColumn column) {
			if(column.SortBySummary == null) {
				StringBuilder result = new StringBuilder();
				result.Append(column.Hierarchy).Append(".currentmember.");
				switch(column.SortMode) {
					case PivotSortMode.Key:
						result.Append("member_key");
						break;
					case PivotSortMode.DisplayText:
						result.Append("member_caption");
						break;
					case PivotSortMode.Custom:
					case PivotSortMode.Default:
					case PivotSortMode.Value:
						result.Append("member_value");
						break;
				}
				return result.ToString();
			} else
				return column.GetSortBySummaryMDX();
		}
	}
	class SortByTempMember2000 : SortByTempMember {
		public SortByTempMember2000(OLAPCubeColumn column, bool isMeasure)
			: base(column, isMeasure) {
		}		
		protected override string GetSortBy(OLAPCubeColumn column) {
			if(column.SortBySummary == null) {
				StringBuilder result = new StringBuilder();
				result.Append(column.Hierarchy).Append(".currentmember.properties(\"");
				switch(column.SortMode) {
					case PivotSortMode.Key:
						result.Append("key");
						break;
					case PivotSortMode.DisplayText:
					case PivotSortMode.Custom:
					case PivotSortMode.Default:
					case PivotSortMode.Value:
						result.Append("caption");
						break;
				}
				return result.Append("\")").ToString();
			} else
				return column.GetSortBySummaryMDX();
		}
	}
	class QueryBuilder {
		Dictionary<string, QueryTempMember> withMembers;
		List<string> whereMembers;
		StringBuilder onColumns, onRows;
		string cubeName;
		QueryBuilder subSelect;
		bool isSubSelect;
		Dictionary<OLAPCubeColumn, SortByTempMember> sortByMembers;
		public QueryBuilder(string cubeName)
			: this(cubeName, false) {
		}
		public QueryBuilder(string cubeName, bool isSubSelect) {
			this.withMembers = new Dictionary<string, QueryTempMember>();
			this.onColumns = new StringBuilder();
			this.onRows = new StringBuilder();
			this.whereMembers = new List<string>();
			this.cubeName = cubeName;
			this.subSelect = null;
			this.isSubSelect = isSubSelect;
		}
		protected Dictionary<string, QueryTempMember> WithMembers { get { return withMembers; } }
		public Dictionary<OLAPCubeColumn, SortByTempMember> SortByMembers {
			get {
				if(sortByMembers == null)
					sortByMembers = new Dictionary<OLAPCubeColumn, SortByTempMember>();
				return sortByMembers;
			}
		}
		public StringBuilder OnColumns { get { return onColumns; } }
		public StringBuilder OnRows { get { return onRows; } }
		public string CubeName { get { return cubeName; } set { cubeName = value; } }
		public List<string> WhereMembers { get { return whereMembers; } }
		public QueryBuilder SubSelect { get { return subSelect; } set { subSelect = value; } }
		public bool IsSubSelect { get { return isSubSelect; } }
		protected bool NonEmptyBehaviour { get { return !IsSubSelect; } }
		protected string NonEmptyString { get { return NonEmptyBehaviour ? "non empty " : ""; } }
		public void AddWithMember(QueryTempMember member) {
			if(WithMembers.ContainsKey(member.Name)) return;
			WithMembers.Add(member.Name, member);
			SortByTempMember sortBy = member as SortByTempMember;
			if(sortBy != null)
				SortByMembers.Add(sortBy.Column, sortBy);
		}
		public override string ToString() {
			StringBuilder result = new StringBuilder();
			if(WithMembers.Count > 0) {
				result.AppendLine("with");
				foreach(QueryTempMember member in WithMembers.Values)
					WriteTempMember(result, member);
			}
			result.AppendLine("select").Append(NonEmptyString)
				.AppendLine(OnColumns.ToString()).Append("on columns");
			if(OnRows.Length > 0) {
				result.AppendLine(",").Append(NonEmptyString)
					.AppendLine(OnRows.ToString()).Append("on rows");
			}
			result.AppendLine().Append("from ");
			if(SubSelect == null)
				result.Append("[").Append(CubeName).AppendLine("]");
			else
				result.AppendLine("(").Append(SubSelect.ToString()).Append(")");
			if(WhereMembers.Count > 0) {
				result.AppendLine("where").Append("( ");
				foreach(string member in WhereMembers) {
					result.Append(member).Append(", ");
				}
				result.Length -= 2;
				result.Append(" )");
			}
			return result.ToString();
		}
		protected void WriteTempMember(StringBuilder result, QueryTempMember member) {
			if(member.IsMember)
				result.Append("member ");
			else
				result.Append("set ");
			result.Append(member.Name).Append(" as '").Append(EscapeApostroph(member.MDX)).AppendLine("' ");
		}
		string EscapeApostroph(string str) {
			StringBuilder escapeString = new StringBuilder(str);
			for(int i = 0; i < escapeString.Length; i++)
				if(escapeString[i] == '\'') {
					i++;
					escapeString.Insert(i, '\'');
				}
			return escapeString.ToString();
		}
		public void CreateSubSelect() {
			if(SubSelect == null)
				SubSelect = new QueryBuilder(CubeName, true);
		}
		public QueryBuilder GetInnerSubSelect() {
			QueryBuilder res = this;
			while(res.SubSelect != null)
				res = res.SubSelect;
			return res;
		}
	}
	abstract class OLAPDataSourceQueryBase {
		protected delegate void WriteContent();
		public const string MeasuresString = "[Measures]";
		public const string TempMeasureName = "[Measures].[XtraPivotGrid Temp Measure]";
		public const string TempMeasureNameFormat = "[Measures].[XtraPivotGrid Temp Measure {0}]";
		public const string FilterMemberString = " XtraPivotGrid Filter]";
		public const string OthersMemberString = " XtraPivotGrid Others]";
		public const string EmptyMeasureString = "[Measures].[XtraPivotGrid Empty]";
		public const string UniqueNameMeasureString = "[Measures].[XtraPivotGrid Member Unique Name]";
		public const string MemberUniqueName = "[MEMBER_UNIQUE_NAME]";
		public static bool IsOthersMember(string uniqueName) { return uniqueName.EndsWith(OthersMemberString); }	
		public static string GetTotalMember(OLAPCubeColumn column) { return column.TotalMember.UniqueName; }
		public static string GetVisualTotalsName(OLAPCubeColumn column) { return "[" + column.Name + " Visual Totals]"; }
		public static string GetFilterMember(string hierarchy, string name) { return hierarchy + ".[" + name + FilterMemberString; }
		public static string GetFilterMember(OLAPCubeColumn column) { return GetFilterMember(column.Hierarchy.UniqueName, column.Name); }		
		protected abstract bool AllowInternalNonEmpty { get; }
		public virtual string GetDrillDownQueryString(string cubeName, List<string> filteredValues, 
													List<string> returnColumns, int maxRowCount) {
			if(string.IsNullOrEmpty(cubeName)) return null;
			StringBuilder result = new StringBuilder("drillthrough ");
			if(maxRowCount > 0)
				result.Append("maxrows " + maxRowCount.ToString() + " ");
			result.Append("select from [").Append(cubeName).Append("] ");
			result.Append("where ( ");
			foreach(string member in filteredValues)
				result.Append(member).Append(", ");
			result.Length -= 2;
			result.Append(" )");
			return result.ToString();
		}
		public string GetQueryString(string cubeName, List<OLAPCubeColumn> columns, List<OLAPCubeColumn> rows,
			List<OLAPTuple> columnTuples, List<OLAPTuple> rowTuples, List<OLAPCubeColumn> measures, List<OLAPCubeColumn> columnRowFilters,
			List<OLAPCubeColumn> whereFilters, bool columnExpand, bool rowExpand) {
			if(measures == null || measures.Count == 0 || string.IsNullOrEmpty(cubeName)) return null;
			QueryBuilder result = new QueryBuilder(cubeName);
			if(!columnExpand) {
				for(int i = 0; i < columns.Count; i++) {
					if(i > 0 && columns[i - 1].IsParent(columns[i].ParentColumn) || columns[i].HasTotalMember) 
						continue;
					result.AddWithMember(CreateTotal(result, columns[i]));
				}
			}
			if(!rowExpand) {
				for(int i = 0; i < rows.Count; i++) {
					if(i > 0 && rows[i - 1].IsParent(rows[i].ParentColumn) || rows[i].HasTotalMember) 
						continue;
					result.AddWithMember(CreateTotal(result, rows[i]));
				}
			}
			CreateColumnRowFilters(result, columnRowFilters);
			CreateWhereFilters(result, whereFilters);
			result.OnColumns.Append("{ ");
			if(columns.Count > 0) {
				result.OnColumns.Append(GetEverything(result, columns, columnTuples, columnExpand));
				result.OnColumns.Append("* ");
			}
			WriteMeasures(result.OnColumns, measures);
			result.OnColumns.Append("}");
			if(rows.Count > 0) {
				result.OnRows.Append(GetEverything(result, rows, rowTuples, rowExpand));
				result.OnRows.Append("dimension properties member_unique_name");
			}
			result.CubeName = cubeName;
			return result.ToString();
		}
		internal string GetEverything(QueryBuilder queryBuilder, List<OLAPCubeColumn> columns, List<OLAPTuple> tuples, bool isExpand) {
			if(isExpand) {
				Debug.Assert(columns.Count == 1, "Invalid columns count");
				return GetExpand(queryBuilder, columns[0], tuples);
			} else {
				if(tuples.Count > 0)
					return GetAllTuples(tuples);
				else {
					Debug.Assert(columns.Count == 1, "Invalid columns count");
					return GetColumnMembers(queryBuilder, columns[0]);
				}
			}
		}
		private string GetColumnMembers(QueryBuilder queryBuilder, OLAPCubeColumn column) {
			StringBuilder result = new StringBuilder();
			WriteAllMembersWithSorting(queryBuilder, result, column, true);
			return result.ToString();
		}
		private string GetAllTuples(List<OLAPTuple> tuples) {
			CheckTuples(tuples);
			StringBuilder result = new StringBuilder();
			result.Append("{ ");
			for(int i = 0; i < tuples.Count; i++) {
				WriteTuple(result, tuples[i]);
				if(i != tuples.Count - 1)
					result.Append(", ");
			}
			result.Append(" } ");
			return result.ToString();
		}
		private string GetExpand(QueryBuilder queryBuilder, OLAPCubeColumn column, List<OLAPTuple> tuples) {
			if(tuples[0].Last.IsTotal || tuples[0].Last.IsOthers) throw new Exception("Cannot expand total or others member");
			StringBuilder result = new StringBuilder();
			result.Append("{ ");
			if(tuples[0].Last.Column.IsParent(column)) {
				foreach(OLAPTuple tuple in tuples)
					result.Append(GetHierarchyExpand(queryBuilder, column, tuple)).Append(", ");
			} else {
				foreach(OLAPTuple tuple in tuples)
					result.Append(GetCrossJoinExpand(queryBuilder, column, tuple)).Append(", ");
			}
			result.Length -= 2;
			result.Append(" } ");
			return result.ToString();
		}
		private string GetHierarchyExpand(QueryBuilder queryBuilder, OLAPCubeColumn column, OLAPTuple tuple) {
			StringBuilder result = new StringBuilder();
			result.Append("{ ");
			WriteTuple(result, tuple);
			result.Append(", ");
			if(tuple.MemberCount > 1) {
				result.Append("{");
				WriteTuple(result, tuple, false);
				result.Append("} * ");
			}
			if(column.TopValueCount == 0)
				WriteSortedMembers(queryBuilder, result, column, delegate() { WriteDescendantsCore(result, column, tuple); });
			else
				WriteTopMembers(result, column, delegate() { WriteDescendantsCore(result, column, tuple); });
			result.Append(" } ");
			return result.ToString();
		}
		private void WriteDescendantsCore(StringBuilder result, OLAPCubeColumn column, OLAPTuple tuple) {
			if(!column.Filtered)
				result.Append("descendants(").Append(tuple.Last.UniqueName).Append(", ").Append(column.UniqueName).Append(")");
			else {
				List<string> filteredValues = column.FilteredValues;
				if(filteredValues.Count > 0) {
					result.Append("intersect({");
					WriteMembers(result, filteredValues, false);
					result.Append("}, descendants(").Append(tuple.Last.UniqueName).Append(", ").Append(column.UniqueName).Append("))");
				} else result.Append("{}");
			}
		}
		private string GetCrossJoinExpand(QueryBuilder queryBuilder, OLAPCubeColumn column, OLAPTuple tuple) {
			StringBuilder result = new StringBuilder();
			if(column.TopValueCount == 0) {
				bool sortByMeasure = tuple.Last.Column.Hierarchy.Dimension != column.Hierarchy.Dimension; 
				WriteSortedMembers(queryBuilder, result, column, delegate() { 
					WriteCrossJoinExpandCore(queryBuilder, result, column, tuple);
				}, CreateSortByTempMember(column, sortByMeasure));
			} else {			   
				WriteTopMembers(result, column, delegate() { 
					WriteCrossJoinExpandCore(queryBuilder, result, column, tuple); 
				});
			}
			return result.ToString();
		}
		void WriteCrossJoinExpandCore(QueryBuilder queryBuilder, StringBuilder result, OLAPCubeColumn column,
					OLAPTuple tuple) {
			bool allowNonEmpty = AllowInternalNonEmpty && column.HasCalculatedMembers == UndefinedBoolean.No;
			if(allowNonEmpty)
				result.Append("nonempty(");
			result.Append("crossjoin({");
			WriteTuple(result, tuple);
			result.Append("}, ");
			WriteAllMembers(queryBuilder, result, column, false);
			if(allowNonEmpty)
				result.Append(")) ");
			else
				result.Append(") ");
		}
		protected void WriteTuple(StringBuilder result, OLAPTuple tuple) {
			WriteTuple(result, tuple, true);
		}
		protected void WriteTuple(StringBuilder result, OLAPTuple tuple, bool includeLastMember) {
			result.Append("( ");
			int count = includeLastMember ? tuple.MemberCount : tuple.MemberCount - 1;
			for(int j = 0; j < count; j++) {
				if(j != count - 1 && tuple[j].Column.IsParent(tuple[j + 1].Column)) 
					continue;
				result.Append(tuple[j].UniqueName);
				if(j != count - 1)
					result.Append(", ");
			}
			result.Append(" )");
		}
		void CheckTuples(List<OLAPTuple> tuples) {
			if(tuples.Count == 0) throw new Exception("Invalid tuple count (0)");
			int memberCount = tuples[0].MemberCount;
			for(int i = 0; i < tuples.Count; i++)
				if(tuples[i].MemberCount != memberCount) throw new Exception("Tuples have different count of members");
		}
		protected abstract QueryTempMember CreateTotal(QueryBuilder queryBuilder, OLAPCubeColumn column);
		protected abstract void CreateColumnRowFilters(QueryBuilder result, List<OLAPCubeColumn> filters);		
		protected QueryTempMember CreateFilter(List<string> members, int setId) {
			StringBuilder mdx = new StringBuilder();
			mdx.Append("visualtotals(hierarchize(");
			WriteMembers(mdx, members, true);
			mdx.Append("))");
			return new QueryTempMember(false, "XtraPivotGridVTSet" + setId.ToString(), mdx.ToString()); 
		}
		protected void CreateWhereFilters(QueryBuilder result, List<OLAPCubeColumn> filters) {
			for(int i = filters.Count - 1; i >= 0; i--) {
				if(i < filters.Count - 1 && filters[i] == filters[i + 1].ParentColumn) continue;
				QueryTempMember tempMember = CreateFilterAggregate(filters[i]);
				result.AddWithMember(tempMember);
				result.WhereMembers.Add(tempMember.Name);
			}
		}
		protected QueryTempMember CreateFilterAggregate(OLAPCubeColumn column) {
			StringBuilder mdx = new StringBuilder();
			if(column.FilteredValues.Count > 1) {
				mdx.Append("aggregate(");
				WriteMembers(mdx, column.FilteredValues, true);
				mdx.Append(")");
			} else
				WriteMembers(mdx, column.FilteredValues, false);
			return new QueryTempMember(true, GetFilterMember(column), mdx.ToString());
		}
		protected void WriteMeasures(StringBuilder result, IEnumerable<OLAPCubeColumn> measures) {
			result.Append("{ ");
			foreach(OLAPCubeColumn measure in measures)
				result.Append(measure.UniqueName).Append(", ");
			result.Length -= 2;
			result.Append(" } ");
		}
		public abstract string GetMembersQueryString(string cubeName, string levelUniqueName, string hierarchy, string[] members);
		public abstract string GetNullValuesQueryString(string cubeName, string levelUniqueName, string hierarchy);
		public string GetCalculatedMembersQueryString(string cubeName, List<OLAPCubeColumn> columns) {
			QueryBuilder result = new QueryBuilder(cubeName);
			result.OnColumns.Append("{");
			for(int i = 0; i < columns.Count; i++) {
				QueryTempMember tempMeasure = new QueryTempMember(true, 
					string.Format(TempMeasureNameFormat, i),
					string.Format("count(addcalculatedmembers({{ {0}.members }})) - count({{ {0}.members }})", columns[i].UniqueName));
				result.AddWithMember(tempMeasure);
				result.OnColumns.Append(tempMeasure.Name);
				if(i != columns.Count - 1)
					result.OnColumns.Append(", ");
			}
			result.OnColumns.Append("}");
			return result.ToString();
		}
		protected virtual string GetUniqueNameScript(OLAPCubeColumn column) {
			return column.Hierarchy.UniqueName + ".currentmember.properties(\"unique_name\")";
		}
		public string GetSortQueryString(string cubeName, string[] members, OLAPCubeColumn column) {
			QueryBuilder result = new QueryBuilder(cubeName);
			result.AddWithMember(new QueryTempMember(true, UniqueNameMeasureString, GetUniqueNameScript(column)));
			result.OnColumns.Append("{").Append(UniqueNameMeasureString).Append("}");
			WriteSortedMembers(result, result.OnRows, column, delegate() {
				if(members.Length == 0)
					WriteAllMembers(result, result.OnRows, column, true);
				else
					WriteMembers(result.OnRows, members, true);
			});
			result.OnRows.Append(" dimension properties member_unique_name");
			return result.ToString();
		}
		protected void WriteTopMembers(StringBuilder result, OLAPCubeColumn column, WriteContent writeContent) {
			bool topMembers = column.SortBySummary != null ^ column.SortOrder == PivotSortOrder.Ascending;
			switch(column.TopValueType) {
				case PivotTopValueType.Absolute:
					result.Append(topMembers ? "topcount(nonempty({" : "bottomcount(nonempty({");
					break;
				case PivotTopValueType.Percent:
					result.Append(topMembers ? "toppercent(nonempty({" : "bottompercent(nonempty({");
					break;
				default:
					throw new InvalidOperationException();
			}
			writeContent();
			result.Append("}), ").Append(column.TopValueCount);
			if(column.SortBySummary != null)
				result.Append(", ").Append(column.GetSortBySummaryMDX());
			result.Append(")");
		}
		protected void WriteSortedMembers(QueryBuilder queryBuilder, StringBuilder result, OLAPCubeColumn column, 
			WriteContent writeContent) {
			WriteSortedMembers(queryBuilder, result, column, writeContent, CreateSortByTempMember(column, true));
		}
		protected void WriteSortedMembers(QueryBuilder queryBuilder, StringBuilder result, OLAPCubeColumn column, 
			WriteContent writeContent, SortByTempMember sortBy) {
			if(column.SortMode != PivotSortMode.None) {				
				queryBuilder.AddWithMember(sortBy);
				result.Append("order({");
				writeContent();
				result.Append("}, ").Append(sortBy.Name).Append(", ").Append(GetSortOrder(column)).Append(")");
			} else {
				result.Append("{");
				writeContent();
				result.Append("}");
			}
		}
		protected abstract SortByTempMember CreateSortByTempMember(OLAPCubeColumn column, bool isMeasure);
		protected string GetSortOrder(OLAPCubeColumn column) {
			return column.SortOrder == PivotSortOrder.Ascending ? "asc" : "desc";
		}
		protected void WriteMembers(StringBuilder result, IEnumerable<string> members, bool includeBrackets) {
			if(includeBrackets) result.Append("{ ");
			int oldLength = result.Length;
			foreach(string member in members)
				result.Append(member).Append(", ");
			if(result.Length - oldLength > 4) {
				result.Length -= 2;
				result.Append(" ");
			}
			if(includeBrackets) result.Append("} ");
		}
		protected void WriteAllMembers(QueryBuilder queryBuilder, StringBuilder result, OLAPCubeColumn column) {
			WriteAllMembers(queryBuilder, result, column, false);
		}
		protected void WriteAllMembers(QueryBuilder queryBuilder, StringBuilder result, OLAPCubeColumn column, bool includeTotal) {
			result.Append("{ ");
			if(includeTotal)
				result.Append(GetTotalMember(column)).Append(", ");
			WriteAllMembersCore(queryBuilder, result, column, includeTotal);			
			result.Append("} ");
		}
		protected void WriteAllMembersCore(QueryBuilder queryBuilder, StringBuilder result, OLAPCubeColumn column, bool excludeTotal) {
			if(column.Filtered) {
				WriteMembers(result, column.FilteredValues, false);
			} else {
				if(column.HasCalculatedMembers == UndefinedBoolean.Yes)
					result.Append("addcalculatedmembers({");
				result.Append(column.UniqueName).Append(".members");
				if(column.HasCalculatedMembers == UndefinedBoolean.Yes) {
					result.Append("})");
					SortByTempMember sortBy;
					if(queryBuilder.SortByMembers.TryGetValue(column, out sortBy) && !sortBy.IsMeasure) {
						result.Append(" - {").Append(sortBy.Name).Append("}");
					}
				}
				if(excludeTotal)
					result.Append(" - {").Append(GetTotalMember(column)).Append("}");
			}
		}
		protected void WriteAllMembersWithSorting(QueryBuilder queryBuilder, StringBuilder result, OLAPCubeColumn column, bool includeTotal) {
			result.Append("{ ");
			if(includeTotal)
				result.Append(GetTotalMember(column)).Append(", ");
			if(column.TopValueCount == 0)
				WriteSortedMembers(queryBuilder, result, column, delegate() { 
					WriteAllMembersCore(queryBuilder, result, column, includeTotal); 
				});
			else
				WriteTopMembers(result, column, delegate() { 
					WriteAllMembersCore(queryBuilder, result, column, includeTotal); 
				});
			result.Append("} ");
		}
		protected static void WriteDescendants(StringBuilder result, string parent) {
			result.Append("descendants(").Append(parent).Append(", 1)");
		}
		public string GetIntersectFilterValuesQuery(string cubeName, OLAPCubeColumn childColumn, OLAPCubeColumn parentColumn) {
			if(!parentColumn.IsParent(childColumn) && !childColumn.IsParent(parentColumn)) 
				throw new Exception("This method can be called for the hierarchies only");
			bool isParent = parentColumn.IsParent(childColumn);
			QueryBuilder result = new QueryBuilder(cubeName);
			QueryTempMember emptyMeasure = new QueryTempMember(true, EmptyMeasureString, "0");
			result.AddWithMember(emptyMeasure);
			result.OnColumns.Append("{").Append(emptyMeasure.Name).Append("}");
			result.OnRows.Append("distinct({");
			for(int i = 0; i < childColumn.FilteredValues.Count; i++) {
				result.OnRows.Append(isParent ? "ancestor(" : "descendants("); 
				result.OnRows.Append(childColumn.FilteredValues[i])
					.Append(", ").Append(parentColumn).Append(")");
				if(i != childColumn.FilteredValues.Count - 1)
					result.OnRows.Append(", ");
			}
			result.OnRows.Append("}) dimension properties member_unique_name");
			CreateIntersectFilterValuesFilter(result, parentColumn);
			return result.ToString();
		}
		protected virtual void CreateIntersectFilterValuesFilter(QueryBuilder result, OLAPCubeColumn parentColumn) {
			if(!parentColumn.Filtered) return;
			List<OLAPCubeColumn> filters = new List<OLAPCubeColumn>(1);
			filters.Add(parentColumn);
			CreateColumnRowFilters(result, filters);   
		}
		public string GetKPIValueQuery(string kpiName, string cubeName) {
			return string.Format("select {{ KPIValue(\"{0}\"), KPIGoal(\"{0}\"), KPIStatus(\"{0}\"), KPITrend(\"{0}\"), KPIWeight(\"{0}\") }} " +
				"on columns from [{1}]", kpiName, cubeName);
		}
	}
	class OLAPDataSourceQuery2005 : OLAPDataSourceQueryBase {
		protected override bool AllowInternalNonEmpty { get { return true; } }
		protected override SortByTempMember CreateSortByTempMember(OLAPCubeColumn column, bool isMeasure) {
			return new SortByTempMember2005(column, isMeasure);
		}
		public override string GetDrillDownQueryString(string cubeName, List<string> filteredValues, 
									List<string> returnColumns, int maxRowCount) {
			string res = base.GetDrillDownQueryString(cubeName, filteredValues, returnColumns, maxRowCount);
			if(returnColumns != null && returnColumns.Count > 0) {
				StringBuilder result = new StringBuilder(res);
				result.AppendLine().Append("return ");
				for(int i = 0; i < returnColumns.Count; i++) {
					result.Append(returnColumns[i]);
					if(i != returnColumns.Count - 1)
						result.Append(", ");
				}
				return result.ToString();
			}
			return res;
		}
		public override string GetMembersQueryString(string cubeName, string levelUniqueName, string hierarchy, string[] members) {
			if(string.IsNullOrEmpty(cubeName)) return null;
			StringBuilder result = new StringBuilder();
			result.Append("with member ").Append(TempMeasureName).Append(" as ").Append(hierarchy)
				.Append(".currentmember.Properties(\"MEMBER_VALUE\", TYPED) select { ")
				.Append(TempMeasureName).Append(" } on columns, { ");
			if(members == null || members.Length == 0) {
				result.Append(levelUniqueName).Append(".members");
			} else {
				for(int i = 0; i < members.Length; i++) {
					result.Append(members[i]);
					if(i != members.Length - 1)
						result.Append(", ");
				}
			}
			result.Append(" } dimension properties member_unique_name on rows ")
				.Append("from [").Append(cubeName).Append("]");
			return result.ToString();
		}
		public override string GetNullValuesQueryString(string cubeName, string levelUniqueName, string hierarchy) {
			if(string.IsNullOrEmpty(cubeName)) return null;
			StringBuilder result = new StringBuilder();
			result.Append("select filter({ ").Append(levelUniqueName).Append(".members }, ").Append(hierarchy)
				.Append(".currentmember.member_value = null) on columns from [").Append(cubeName).Append("]");
			return result.ToString();
		}
		protected override QueryTempMember CreateTotal(QueryBuilder queryBuilder, OLAPCubeColumn column) {
			StringBuilder mdx = new StringBuilder();
			if(column.AllMember != null) {
				mdx.Append(column.AllMember.UniqueName);
			} else {
				mdx.Append("aggregate(");
				WriteAllMembers(queryBuilder, mdx, column, true);
				mdx.Append(")");
			}
			return new QueryTempMember(true, GetTotalMember(column), mdx.ToString());
		}
		protected override void CreateColumnRowFilters(QueryBuilder result, List<OLAPCubeColumn> filters) {
			for(int i = 0; i < filters.Count; i++) {
				result.CreateSubSelect();
				WriteMembers(result.SubSelect.OnColumns, filters[i].FilteredValues, true);
				result = result.SubSelect;
			}
		}
	}
	class OLAPDataSourceQuery2000 : OLAPDataSourceQueryBase {
		protected override bool AllowInternalNonEmpty { get { return false; } }
		public override string GetMembersQueryString(string cubeName, string levelUniqueName, string hierarchy, string[] members) {
			if(string.IsNullOrEmpty(cubeName)) return null;
			StringBuilder result = new StringBuilder();
			result.Append("with member ").Append(TempMeasureName).Append(" as '").Append(hierarchy)
				.Append(".currentmember.Properties(\"CAPTION\")' select { ")
				.Append(TempMeasureName).Append(" } on columns, { ");
			if(members == null || members.Length == 0) {
				result.Append(levelUniqueName).Append(".members");
			} else {
				for(int i = 0; i < members.Length; i++) {
					result.Append(members[i]);
					if(i != members.Length - 1)
						result.Append(", ");
				}
			}
			result.Append(" } dimension properties member_unique_name on rows ")
				.Append("from [").Append(cubeName).Append("]");
			return result.ToString();
		}
		public override string GetNullValuesQueryString(string cubeName, string levelUniqueName, string hierarchy) {
			if(string.IsNullOrEmpty(cubeName)) return null;
			StringBuilder result = new StringBuilder();
			result.Append("select filter({ ").Append(levelUniqueName).Append(".members }, ").Append(hierarchy)
				.Append(".currentmember.properties(\"CAPTION\") = \"\") on columns from [").Append(cubeName).Append("]");
			return result.ToString();
		}
		protected override SortByTempMember CreateSortByTempMember(OLAPCubeColumn column, bool isMeasure) {
			return new SortByTempMember2000(column, isMeasure);
		}		
		protected override QueryTempMember CreateTotal(QueryBuilder queryBuilder, OLAPCubeColumn column) {
			StringBuilder mdx = new StringBuilder();
			if(column.AllMember != null) {
				mdx.Append(column.AllMember.UniqueName);
			} else {
				mdx.Append("aggregate(");
				WriteAllMembers(queryBuilder, mdx, column, true);
				mdx.Append(" - {").Append(GetTotalMember(column)).Append("})");
			}
			return new QueryTempMember(true, GetTotalMember(column), mdx.ToString());
		}
		protected override void CreateColumnRowFilters(QueryBuilder result, List<OLAPCubeColumn> filters) {
			CreateVisualTotals(result, filters);
		}
		protected void CreateVisualTotals(QueryBuilder result, List<OLAPCubeColumn> filters) {
			List<string> members = new List<string>();
			for(int i = 0, id = 0; i < filters.Count; i++) {
				if(filters[i].VisualTotalsIncludeAllMember && filters[i].AllMember != null)
					members.Add(filters[i].AllMember.UniqueName);
				members.AddRange(filters[i].FilteredValues);
				if(i == filters.Count - 1 || filters[i + 1].ParentColumn != filters[i]) {
					result.AddWithMember(CreateFilter(members, id++));
					members.Clear();
				}
			}
		}
	}
	public class OLAPConnectionStringBuilder {
		public static int Unassigned = int.MinValue;
		string cubeName;
		int queryTimeout = DefaultQueryTimeout;
		public static int DefaultQueryTimeout = 30;
		public static int DefaultConnectionTimeout = 60;
		public static string[] PropertiesOrder = new string[] { "Provider", "ServerName", "CatalogName", "CubeName", "QueryTimeout", "LocaleIdentifier", "ConnectionTimeout", "UserId", "Password" };
		public static int DefaultLCID { get { return CultureInfo.CurrentCulture.LCID; } }
		[Basic, TypeConverter(typeof(ProviderTypeConverter))]
		public string Provider { get { return GetParameter("Provider"); } set { SetParameter("Provider", value); } }
		[Basic, TypeConverter(typeof(ServerTypeConverter)), DisplayName("Server Name")]
		public string ServerName { get { return GetParameter("Data Source"); } set { SetParameter("Data Source", value); } }
		[Basic, TypeConverter(typeof(CatalogTypeConverter)), DisplayName("Catalog Name")]
		public string CatalogName { get { return GetParameter("Initial Catalog"); } set { SetParameter("Initial Catalog", value); } }
		[TypeConverter(typeof(LocaleTypeConverter)), DisplayName("Language")]
		public int LocaleIdentifier { get { return (int)GetParameter("Locale Identifier", typeof(int), DefaultLCID); } set { SetParameter("Locale Identifier", value.ToString(), DefaultLCID.ToString()); } }
		[DisplayName("Connection Timeout (seconds)")]
		public int ConnectionTimeout { get { return (int)GetParameter("Connect Timeout", typeof(int), DefaultConnectionTimeout); } set { SetParameter("Connect Timeout", value.ToString(), DefaultConnectionTimeout.ToString()); } }
		public string UserId { get { return GetParameter("User ID"); } set { SetParameter("User ID", value); } }
		[PasswordPropertyText(true)]
		public string Password { get { return GetParameter("Password"); } set { SetParameter("Password", value); } }
		[Basic, TypeConverter(typeof(CubeTypeConverter)), DisplayName("Cube Name")]
		public string CubeName { get { return cubeName; } set { cubeName = value; } }
		[DisplayName("Query Timeout (seconds)")]
		public int QueryTimeout { get { return queryTimeout; } set { queryTimeout = value; } }
		readonly Dictionary<string, string> parameters;
		protected Dictionary<string, string> Parameters { get { return parameters; } }
		[Browsable(false)]
		public string FullConnectionString { get { return GetConnectionString(false).ToString(); } set { ParseConnectionString(value, true); } }
		[Browsable(false)]
		public string ConnectionString { get { return GetConnectionString(true).ToString(); } set { ParseConnectionString(value, false); } }
		public OLAPConnectionStringBuilder() {
			parameters = new Dictionary<string, string>();
		}
		public OLAPConnectionStringBuilder(string fullConnectionString)
			: this() {
			FullConnectionString = fullConnectionString;
		}
		protected string GetParameter(string key) {
			if(Parameters.ContainsKey(key)) return (string)Parameters[key];
			else return string.Empty;
		}
		protected object GetParameter(string key, Type type, int defaultValue) {
			string parameter = GetParameter(key);
			if(type == typeof(int)){
				int value = 0;
				if(int.TryParse(parameter, out value))
					return value;
				return defaultValue;
			}
			return parameter;
		}
		protected void SetParameter(string key, string value) {
			if(Parameters.ContainsKey(key)) Parameters[key] = value;
			else Parameters.Add(key, value);
		}
		protected void SetParameter(string key, string value, string defaultValue) {
			string recordingValue;
			if(value == defaultValue)
				recordingValue = "";
			else
				recordingValue = value;
			SetParameter(key, recordingValue);
		}
		void ParseConnectionString(string value, bool parseMyParameters) {
			if(parseMyParameters)
				ResetMyParameters();
			Parameters.Clear();
			if(String.IsNullOrEmpty(value)) return;
			string[] values = value.Split(';');
			foreach(string val in values)
				ParseValue(val, parseMyParameters);
		}
		void ResetMyParameters() {
			CubeName = string.Empty;
			QueryTimeout = DefaultQueryTimeout;
		}
		void ParseValue(string value, bool parseMyParameters) {
			if(string.IsNullOrEmpty(value)) return;
			value = value.Replace(";", "");
			string[] parts = value.Split('=');
			if(parts.Length != 2 || string.IsNullOrEmpty(parts[0])) return;
			parts[0] = parts[0].Trim();
			if(parts[0] != "Password")
				parts[1] = parts[1].Trim();
			if(parts[0] == "Cube Name") {
				if(parseMyParameters)
					CubeName = parts[1];
				return;
			}
			if(parts[0] == "Query Timeout") {
				if(parseMyParameters) {
					int result = 0;
					if(int.TryParse(parts[1], out result))
						QueryTimeout = Math.Abs(result);
					else
						QueryTimeout = DefaultQueryTimeout;
				}
				return;
			}
			SetParameter(parts[0], parts[1]);
		}
		StringBuilder GetConnectionString(bool ignoreMyParameters) {
			if(String.IsNullOrEmpty(ServerName) && String.IsNullOrEmpty(CatalogName)) return new StringBuilder();
			StringBuilder result = new StringBuilder();
			if(!Parameters.ContainsKey("Provider"))
				result.Append("Provider=msolap;");
			foreach(string key in Parameters.Keys)
				AppendParameter(result, key);
			if(!ignoreMyParameters) {
				AppendParameter(result, "Cube Name", CubeName);
				if(QueryTimeout != DefaultQueryTimeout)
					AppendParameter(result, "Query Timeout", QueryTimeout.ToString());
			}
			return result;
		}
		void AppendParameter(StringBuilder result, string key) {
			string value = Parameters[key];
			if(key == "" || value == "")
				return;
			AppendParameter(result, key, value);
		}
		void AppendParameter(StringBuilder result, string key, object value) {
			result.Append(key).Append("=").Append(value.ToString()).Append(";");
		}
	}
	class BasicAttribute : Attribute {
		public override object TypeId { get { return "Basic"; } }
	}
	abstract class ConnectionTypeConverter : OLAPTypeConverterBase {
		List<string> fNames;
		OLAPMetaGetter metaGetter = new OLAPMetaGetter();
		protected OLAPMetaGetter MetaGetter { get { return metaGetter; } }
		public override StandardValuesCollection GetStandardValues(ITypeDescriptorContext context) {
			return new StandardValuesCollection(GetNames((OLAPConnectionStringBuilder)context.Instance));
		}
		protected void Connect(string providerName, string serverName, string catalogName) {
			if(!string.IsNullOrEmpty(serverName) && !string.IsNullOrEmpty(providerName)) {
				MetaGetter.ConnectionString = GetBaseConnectionString(providerName, serverName);
				if(!MetaGetter.Connected) {
					MessageBox.Show("Couldn't connect to the server.", "Error");
					return;
				}
				if(!String.IsNullOrEmpty(serverName)) {
					metaGetter.ConnectionString += ";Initial Catalog=" + catalogName;
				}
				if(!metaGetter.Connected) {
					MessageBox.Show("Couldn't connect to the \"" + catalogName + "\" database.", "Error");
					return;
				}
			}
		}
		string GetBaseConnectionString(string providerName, string serverName) {
			return "Provider=" + providerName + ";Data Source=" + serverName;
		}
		protected List<string> GetNames(OLAPConnectionStringBuilder options) {
			if(!SkipRefresh(options)) {
				fNames = GetNamesCore(options);
			}
			return fNames;
		}
		protected abstract bool SkipRefresh(OLAPConnectionStringBuilder options);
		protected abstract List<string> GetNamesCore(OLAPConnectionStringBuilder options);
	}
	class OLAPTypeConverterBase : TypeConverter {
		public override bool CanConvertFrom(ITypeDescriptorContext context, Type sourceType) {
			if(sourceType == typeof(string)) return true;
			return base.CanConvertFrom(context, sourceType);
		}
		public override bool CanConvertTo(ITypeDescriptorContext context, Type destinationType) {
			if(destinationType == typeof(string)) return true;
			return base.CanConvertTo(context, destinationType);
		}
		public override object ConvertFrom(ITypeDescriptorContext context, System.Globalization.CultureInfo culture, object value) {
			if(value is string) return (string)value;
			return base.ConvertFrom(context, culture, value);
		}
		public override object ConvertTo(ITypeDescriptorContext context, System.Globalization.CultureInfo culture, object value, Type destinationType) {
			if(destinationType == typeof(string)) return Convert.ToString(value);
			return base.ConvertTo(context, culture, value, destinationType);
		}
		public override bool GetStandardValuesExclusive(ITypeDescriptorContext context) {
			return false;
		}
		public override bool GetStandardValuesSupported(ITypeDescriptorContext context) {
			return true;
		}
	}
	class ProviderTypeConverter : OLAPTypeConverterBase {
		List<string> providers;
		public ProviderTypeConverter() {
			providers = OLAPMetaGetter.GetProviders();
			if(providers.Count == 0) {
				MessageBox.Show("In order to use the PivotGrid OLAP functionality, you should have a MS OLAP OleDb provider installed on your system.\nYou can download it here: " +
					"http://www.microsoft.com/downloads/details.aspx?FamilyID=50b97994-8453-4998-8226-fa42ec403d17#ASOLEDB");
			}
		}
		public override StandardValuesCollection GetStandardValues(ITypeDescriptorContext context) {
			return new StandardValuesCollection(providers);
		}
	}
	class ServerTypeConverter : OLAPTypeConverterBase {
		public override bool GetStandardValuesSupported(ITypeDescriptorContext context) {
			return false;
		}
	}
	class CatalogTypeConverter : ConnectionTypeConverter {
		string oldProvider, oldServerName;
		protected override bool SkipRefresh(OLAPConnectionStringBuilder options) {
			bool skip = oldProvider == options.Provider && oldServerName == options.ServerName;
			if(!skip) {
				oldProvider = options.Provider;
				oldServerName = options.ServerName;
			}
			return skip;
		}
		protected override List<string> GetNamesCore(OLAPConnectionStringBuilder options) {
			Connect(options.Provider, options.ServerName, string.Empty);
			return MetaGetter.GetCatalogs();
		}
	}
	class CubeTypeConverter : ConnectionTypeConverter {
		string oldProvider, oldServerName, oldCatalogName;
		protected override bool SkipRefresh(OLAPConnectionStringBuilder options) {
			bool skip = oldProvider == options.Provider && oldServerName == options.ServerName && oldCatalogName == options.CatalogName;
			if(!skip) {
				oldProvider = options.Provider;
				oldServerName = options.ServerName;
				oldCatalogName = options.CatalogName;
			}
			return skip;
		}
		protected override List<string> GetNamesCore(OLAPConnectionStringBuilder options) {
			Connect(options.Provider, options.ServerName, options.CatalogName);
			return MetaGetter.GetCubes(options.CatalogName);
		}
	}
	class LocaleTypeConverter : TypeConverter {
		List<CultureInfo> fCultures;
		List<string> fEnglishNames;
		public LocaleTypeConverter() {
			fCultures = new List<CultureInfo>(CultureInfo.GetCultures(CultureTypes.InstalledWin32Cultures));
			fEnglishNames = new List<string>(fCultures.Count);
			foreach(CultureInfo info in fCultures)
				fEnglishNames.Add(info.EnglishName);
			fEnglishNames.Sort();
		}
		public override bool CanConvertFrom(ITypeDescriptorContext context, Type sourceType) {
			if(sourceType == typeof(string)) return true;
			return base.CanConvertFrom(context, sourceType);
		}
		public override bool CanConvertTo(ITypeDescriptorContext context, Type destinationType) {
			if(destinationType == typeof(string)) return true;
			return base.CanConvertTo(context, destinationType);
		}
		public override object ConvertFrom(ITypeDescriptorContext context, System.Globalization.CultureInfo culture, object value) {
			if(value is string) return GetLCID((string)value);
			return base.ConvertFrom(context, culture, value);
		}
		public override object ConvertTo(ITypeDescriptorContext context, System.Globalization.CultureInfo culture, object value, Type destinationType) {
			if(value is string) return (string)value;
			if(destinationType == typeof(string)) return GetName((int)value);
			return base.ConvertTo(context, culture, value, destinationType);
		}
		public override bool GetStandardValuesExclusive(ITypeDescriptorContext context) {
			return true;
		}
		public override bool GetStandardValuesSupported(ITypeDescriptorContext context) {
			return true;
		}
		public override StandardValuesCollection GetStandardValues(ITypeDescriptorContext context) {
			return new StandardValuesCollection(fEnglishNames);
		}
		string GetName(int LCID) {
			CultureInfo info = FindByLCID(LCID);
			if(info == null)
				return FindByLCID(OLAPConnectionStringBuilder.DefaultLCID).EnglishName;
			return info.EnglishName;
		}
		CultureInfo FindByName(string englishName) {
			foreach(CultureInfo info in fCultures) {
				if(info.EnglishName == englishName)
					return info;
			}
			return null;
		}
		CultureInfo FindByLCID(int LCID) {
			foreach(CultureInfo info in fCultures) {
				if(info.LCID == LCID)
					return info;
			}
			return null;
		}
		int GetLCID(string englishName) {
			return FindByName(englishName).LCID;
		}
		class CultureComparer : IComparer<CultureInfo> {
			int IComparer<CultureInfo>.Compare(CultureInfo x, CultureInfo y) {
				return Comparer.Default.Compare(x.EnglishName, y.EnglishName);
			}
		}
	}
	public class OLAPPropertyGrid : PropertyGrid {
		ToolStripButton[] viewTabButtonsCache;
		PropertyTab[] viewTabsCache;
		public void AddTabs() {
			PropertyTabs.AddTabType(typeof(BasicTab));
			PropertyTabs.AddTabType(typeof(AdvancedTab));
		}
		ToolStripButton[] ViewTabButtons {
			get {
				if(viewTabButtonsCache == null) {
					FieldInfo fInfo = typeof(PropertyGrid).GetField("viewTabButtons", BindingFlags.Instance | BindingFlags.NonPublic);
					viewTabButtonsCache = (ToolStripButton[])fInfo.GetValue(this);
				}
				return viewTabButtonsCache;
			}
		}
		PropertyTab[] ViewTabs {
			get {
				if(viewTabsCache == null) {
					FieldInfo fInfo = typeof(PropertyGrid).GetField("viewTabs", BindingFlags.Instance | BindingFlags.NonPublic);
					viewTabsCache = (PropertyTab[])fInfo.GetValue(this);
				}
				return viewTabsCache;
			}
		}
		public new Type SelectedTab {
			get {
				if(base.SelectedTab == null) return null;
				return base.SelectedTab.GetType();
			}
			set {
				int tabIndex = FindTabIndex(value);
				if(tabIndex < 0) return;
				MethodInfo mInfo = typeof(PropertyGrid).GetMethod("SelectViewTabButtonDefault", BindingFlags.Instance | BindingFlags.NonPublic);
				mInfo.Invoke(this, new object[] { ViewTabButtons[tabIndex] });
				Refresh();
			}
		}
		protected override PropertyTab CreatePropertyTab(Type tabType) {
			viewTabButtonsCache = null;
			viewTabsCache = null;
			return (PropertyTab)Activator.CreateInstance(tabType);
		}
		int FindTabIndex(Type type) {
			for(int i = 0; i < ViewTabs.Length; i++) {
				if(ViewTabs[i].GetType() == type)
					return i;
			}
			return -1;
		}
	}
	public class BasicTab : PropertyTab {
		Bitmap bitmap;
		public override PropertyDescriptorCollection GetProperties(object component, Attribute[] attributes) {
			PropertyDescriptorCollection properties = TypeDescriptor.GetProperties(component.GetType(), new Attribute[] { new BasicAttribute() });
			return properties.Sort(OLAPConnectionStringBuilder.PropertiesOrder);
		}
		public override string TabName {
			get { return "Basic"; }
		}
		public override Bitmap Bitmap {
			get {
				if(bitmap == null)
					bitmap = new Bitmap(16, 16);
				return bitmap;
			}
		}
	}
	public class AdvancedTab : PropertyTab {
		Bitmap bitmap;
		public override PropertyDescriptorCollection GetProperties(object component, Attribute[] attributes) {
			PropertyDescriptorCollection properties = TypeDescriptor.GetProperties(component, attributes);
			return properties.Sort(OLAPConnectionStringBuilder.PropertiesOrder);
		}
		public override string TabName {
			get { return "Advanced"; }
		}
		public override Bitmap Bitmap {
			get {
				if(bitmap == null)
					bitmap = new Bitmap(16, 16);
				return bitmap;
			}
		}
	}
	enum UndefinedBoolean { Undefined, Yes, No };
	class OLAPHierarchies : Dictionary<string, OLAPHierarchy> {
		public void Add(OLAPHierarchy hierarchy) {
			if(ContainsKey(hierarchy.UniqueName)) return;
			Add(hierarchy.UniqueName, hierarchy);
		}
		public void SaveToStream(BinaryWriter writer) {
			writer.Write(Count);
			foreach(OLAPHierarchy hierarchy in Values)
				hierarchy.SaveToStream(writer);
		}
		public void RestoreFromStream(BinaryReader reader) {
			int hierarchiesCount = reader.ReadInt32();
			for(int i = 0; i < hierarchiesCount; i++) {
				OLAPHierarchy hierarchy = new OLAPHierarchy();
				hierarchy.RestoreFromStream(reader);
				Add(hierarchy);
			}
		}
	}
	class OLAPHierarchy {
		string uniqueName, name, caption, dimension;
		public string UniqueName { get { return uniqueName; } }
		public string Name { get { return name; } }
		public string Caption { get { return caption; } }
		public string Dimension { 
			get {
				if(string.IsNullOrEmpty(dimension)) {
					string[] parts = uniqueName.Split('.');
					dimension = parts[0];
				}
				return dimension; 
			} 
		}
		public OLAPHierarchy() { }
		public OLAPHierarchy(string dataPrefix, DataRow row) {
			this.uniqueName = (string)row[dataPrefix + "_UNIQUE_NAME"];
			this.name = (row[dataPrefix + "_NAME"] is DBNull) ? UniqueName : (string)row[dataPrefix + "_NAME"];
			this.caption = (row[dataPrefix + "_CAPTION"] is DBNull) ? Name : (string)row[dataPrefix + "_CAPTION"];
		}
		public OLAPHierarchy(string uniqueName, string name) : this(uniqueName, name, string.Empty) { }
		public OLAPHierarchy(string uniqueName, string name, string caption) {
			this.uniqueName = uniqueName;
			this.name = name;
			this.caption = caption != null ? caption : string.Empty;
		}
		public virtual void SaveToStream(BinaryWriter writer) {
			writer.Write(uniqueName);
			writer.Write(name);
			writer.Write(caption);
		}
		public virtual void RestoreFromStream(BinaryReader reader) {
			uniqueName = reader.ReadString();
			name = reader.ReadString();
			caption = reader.ReadString();
		}
		public override string ToString() {
			return UniqueName;
		}
	}
	class OLAPKPIColumn : OLAPCubeColumn {
		static Dictionary<string, PivotKPIGraphic> ServerGraphicMap;
		static OLAPKPIColumn() {
			ServerGraphicMap = new Dictionary<string, PivotKPIGraphic>();
			ServerGraphicMap.Add("Shapes", PivotKPIGraphic.Shapes);
			ServerGraphicMap.Add("Traffic Light", PivotKPIGraphic.TrafficLights);
			ServerGraphicMap.Add("Traffic Light - Single", PivotKPIGraphic.TrafficLights);
			ServerGraphicMap.Add("Traffic Light - Multiple", PivotKPIGraphic.TrafficLights);
			ServerGraphicMap.Add("Road Signs", PivotKPIGraphic.RoadSigns);
			ServerGraphicMap.Add("Gauge - Ascending", PivotKPIGraphic.Gauge);
			ServerGraphicMap.Add("Gauge - Descending", PivotKPIGraphic.ReversedGauge);
			ServerGraphicMap.Add("Thermometer", PivotKPIGraphic.Thermometer);
			ServerGraphicMap.Add("Cylinder", PivotKPIGraphic.Cylinder);
			ServerGraphicMap.Add("Smiley Face", PivotKPIGraphic.Faces);
			ServerGraphicMap.Add("Variance Arrow", PivotKPIGraphic.VarianceArrow);	
			ServerGraphicMap.Add("Standard Arrow", PivotKPIGraphic.StandardArrow);
			ServerGraphicMap.Add("Status Arrow - Ascending", PivotKPIGraphic.StatusArrow);
			ServerGraphicMap.Add("Status Arrow - Descending", PivotKPIGraphic.ReversedStatusArrow);
		}
		PivotKPIGraphic graphic;
		PivotKPIType type;
		public PivotKPIGraphic Graphic { get { return graphic; } }
		public PivotKPIType Type { get { return type; } }
		public OLAPKPIColumn() { }
		public OLAPKPIColumn(string uniqueName, string name, string caption, OLAPHierarchy hierarchy, string graphic, PivotKPIType type) 
			: base(uniqueName, name, caption, hierarchy, 0, OleDbType.Variant, null, null) {
			this.graphic = ConvertGraphic(graphic);
			this.type = type;
		}
		protected PivotKPIGraphic ConvertGraphic(string graphic) {
			if(!string.IsNullOrEmpty(graphic) && ServerGraphicMap.ContainsKey(graphic)) return ServerGraphicMap[graphic];
			return PivotKPIGraphic.None;
		}
		public override void SaveToStream(BinaryWriter writer) {
			base.SaveToStream(writer);
			writer.Write((int)Graphic);
			writer.Write((int)Type);
		}
		public override void RestoreFromStream(BinaryReader reader, OLAPHierarchies hierarchies, OLAPCubeColumns columns) {
			base.RestoreFromStream(reader, hierarchies, columns);
			this.graphic = (PivotKPIGraphic)reader.ReadInt32();
			this.type = (PivotKPIType)reader.ReadInt32();
		}
		public override byte TypeCode { get { return OLAPKPIColumnTypeCode; } }
	}
	class OLAPCubeColumn : OLAPHierarchy {
		public const int OLAPCubeColumnTypeCode = 0, OLAPKPIColumnTypeCode = 1;
		public const string TotalMemberString = " XtraPivotGrid Total]";
		#region Fields
		OLAPHierarchy hierarchy;
		int level;
		Type dataType;
		OLAPCubeColumn parentColumn;
		Dictionary<string, OLAPMember> members;
		NullableHashtable values;
		OLAPMember totalMember;
		OLAPMember allMember;
		UndefinedBoolean hasNullValues;
		UndefinedBoolean hasCalculatedMembers;
		PivotSortOrder sortOrder;
		OLAPCubeColumn sortBySummary;
		int topValueCount;
		bool topValueShowOthers;
		PivotTopValueType topValueType;
		List<string> filteredValues;
		bool filtered;
		bool allMembersLoaded;
		PivotSortMode sortMode;
		bool visualTotalsIncludeAllMember;
		PivotSummaryType summaryType;
		OleDbType baseDataType;
		List<OLAPMember> sortBySummaryMembers;
		string drilldownColumn;
		#endregion
		#region Properties
		public OLAPHierarchy Hierarchy { get { return hierarchy; } }
		public int Level { get { return level; } }
		public Type DataType { get { return dataType; } }
		public OleDbType BaseDataType { get { return baseDataType; } }
		public OLAPCubeColumn ParentColumn { get { return parentColumn; } }
		public bool IsMeasure { get { return Hierarchy.UniqueName == "[Measures]"; } }
		public UndefinedBoolean HasNullValues { get { return hasNullValues; } set { hasNullValues = value; } }
		public UndefinedBoolean HasCalculatedMembers { get { return hasCalculatedMembers; } set { hasCalculatedMembers = value; } }
		public PivotSummaryType SummaryType { get { return summaryType; } set { summaryType = value; } }
		public PivotSortOrder SortOrder { get { return sortOrder; } set { sortOrder = value; } }
		public OLAPCubeColumn SortBySummary {
			get { return sortBySummary; }
			set {
				if(value != null && !value.IsMeasure) return;
				sortBySummary = value;
			}
		}
		public List<OLAPMember> SortBySummaryMembers {
			get { return sortBySummaryMembers; }
		}
		public PivotSortMode SortMode { get { return sortMode; } set { sortMode = value; } }
		public int TopValueCount {
			get { return topValueCount; }
			set {
				if(TopValueType == PivotTopValueType.Absolute && value >= 0) topValueCount = value;
				if(TopValueType == PivotTopValueType.Percent && value >= 0 && value <= 100) topValueCount = value;
			}
		}
		public bool TopValueShowOthers { get { return topValueShowOthers; } set { topValueShowOthers = value; } }
		public PivotTopValueType TopValueType {
			get { return topValueType; }
			set {
				if(value != topValueType) {
					topValueType = value;
					if(value == PivotTopValueType.Percent && TopValueCount > 100)
						topValueCount = 100;
				}
			}
		}
		public bool Filtered { get { return filtered; } set { filtered = value; } }
		public List<string> FilteredValues { get { return filteredValues; } }
		public bool VisualTotalsIncludeAllMember { get { return visualTotalsIncludeAllMember; } set { visualTotalsIncludeAllMember = value; } }
		public List<string> SortedFilterValues {
			get {
				switch(SortOrder) {
					case PivotSortOrder.Ascending:
						FilteredValues.Sort();
						break;
					case PivotSortOrder.Descending:
						FilteredValues.Sort(new ReverseComparer<string>());
						break;
				}
				return FilteredValues;
			}
		}
		public int MembersCount { get { return members.Count; } }
		public OLAPMember this[string uniqueName] {
			get {
				if(!string.IsNullOrEmpty(uniqueName) && members.ContainsKey(uniqueName))
					return members[uniqueName];
				return null;
			}
		}		
		public bool AllMembersLoaded { get { return allMembersLoaded; } set { allMembersLoaded = value; } }
		public OLAPMember TotalMember { get { return HasTotalMember ? AllMember : totalMember; } }
		public bool HasTotalMember { get { return AllMember != null; } }
		public OLAPMember AllMember { get { return allMember; } }
		public string AllMemberUniqueName {
			get { return AllMember != null ? AllMember.UniqueName : string.Empty; }
			set {
				RemoveMember(allMember);
				RemoveMember(totalMember);
				if(string.IsNullOrEmpty(value)) {
					allMember = null;
					totalMember = new OLAPMember(this, GetTotalMemberUniqueName(), null);
					AddMember(totalMember);
				} else {
					allMember = new OLAPMember(this, value, OLAPMember.AllValue);
					totalMember = null;
					AddMember(allMember);
				}
			}
		}
		public string DrillDownColumn { 
			get {
				if(!string.IsNullOrEmpty(drilldownColumn))
					return drilldownColumn;
				string columnName = IsMeasure ? UniqueName : Hierarchy.UniqueName;
				return columnName.Insert(1, "$");
			} 
		}
		#endregion
		public OLAPCubeColumn() { }
		public OLAPCubeColumn(string dataPrefix, DataRow row, OLAPHierarchy hierarchy, int level, OleDbType dataType, OLAPCubeColumn parentColumn, string allMemberUniqueName, string drilldownColumn) : base(dataPrefix, row) {
			Initialize(hierarchy, level, dataType, parentColumn, allMemberUniqueName, drilldownColumn);
		}
		public OLAPCubeColumn(string name, string uniqueName, OLAPHierarchy hierarchy, int level, OleDbType dataType)
			: this(uniqueName, name, null, hierarchy, level, dataType, null, string.Empty) { }
		public OLAPCubeColumn(string name, string uniqueName, OLAPHierarchy hierarchy, int level, OleDbType dataType, OLAPCubeColumn parentColumn)
			: this(uniqueName, name, null, hierarchy, level, dataType, parentColumn, null) { }
		public OLAPCubeColumn(string uniqueName, string name, string caption, OLAPHierarchy hierarchy, int level,
			OleDbType dataType, OLAPCubeColumn parentColumn, string allMemberUniqueName)
			: base(uniqueName, name, caption) {
			Initialize(hierarchy, level, dataType, parentColumn, allMemberUniqueName, null);
		}
		void Initialize(OLAPHierarchy hierarchy, int level, OleDbType dataType, 
					OLAPCubeColumn parentColumn, string allMemberUniqueName,
					string drilldownColumn) {
			this.members = new Dictionary<string, OLAPMember>();
			this.values = new NullableHashtable();
			this.hierarchy = hierarchy;
			this.level = level;
			this.baseDataType = dataType;
			this.dataType = OleDbTypeConverter.Convert(dataType);
			this.parentColumn = parentColumn;
			this.drilldownColumn = drilldownColumn;
			this.sortOrder = PivotSortOrder.Ascending;
			this.sortBySummary = null;
			this.sortBySummaryMembers = new List<OLAPMember>();
			this.topValueCount = 0;
			this.topValueShowOthers = false;
			this.filteredValues = new List<string>();
			this.allMembersLoaded = false;
			AllMemberUniqueName = allMemberUniqueName;
		}
		protected string GetTotalMemberUniqueName() {
			return Hierarchy + ".[" + Name + TotalMemberString;
		}
		public static OLAPCubeColumn CreateFromTypeCode(byte typeCode) {
			switch(typeCode) {
				case OLAPCubeColumnTypeCode: return new OLAPCubeColumn();
				case OLAPKPIColumnTypeCode: return new OLAPKPIColumn();
			}
			throw new ArgumentException("Unknown typeCode");
		}
		public List<string> GetMembersNames() {
			List<string> result = new List<string>(members.Count);
			foreach(OLAPMember member in members.Values) {
				if(member.IsTotal) continue;
				result.Add(member.UniqueName);
			}
			return result;
		}
		public OLAPMember[] GetMembers() {
			OLAPMember[] result = new OLAPMember[members.Count];
			members.Values.CopyTo(result, 0);
			return result;
		}
		public void ClearMembers() {
			members.Clear();
			AllMemberUniqueName = null;
			allMembersLoaded = false;
		}
		public List<string> GetUniqueNamesByValue(object value) {
			if(values.ContainsKey(value))
				return (List<string>)values[value];
			return new List<string>();
		}
		public static string RemoveLastElement(string name) {
			int dotPos = name.LastIndexOf('.');
			return dotPos >= 0 ? name.Substring(0, dotPos) : string.Empty;
		}
		public bool IsTotalMember(string memberUniqueName) {
			return memberUniqueName == TotalMember.UniqueName;
		}
		public void AddMember(OLAPMember member) {
			members[member.UniqueName] = member;
			SetValue(member.Value, member.UniqueName);
		}
		public void RemoveMember(OLAPMember member) {
			if(member == null || !members.ContainsKey(member.UniqueName)) return;
			members.Remove(member.UniqueName);
			UnsetValue(member.Value, member.UniqueName);
		}
		void SetValue(object value, string memberUniqueName) {
			if(IsTotalMember(memberUniqueName)) return;
			if(values.ContainsKey(value)) {
				List<string> list = (List<string>)values[value];
				if(!list.Contains(memberUniqueName))
					list.Add(memberUniqueName);
			} else
				values.Add(value, new List<string>(new string[] { memberUniqueName }));
		}
		void UnsetValue(object value, string memberUniqueName) {
			if(IsTotalMember(memberUniqueName)) return;
			if(values.ContainsKey(value)) {
				List<string> list = (List<string>)values[value];
				int index = list.IndexOf(memberUniqueName);
				if(index >= 0)
					list.RemoveAt(index);
			}
		}
		public bool IsParent(OLAPCubeColumn column) {
			if(column == null) return false;
			while(column.ParentColumn != null) {
				if(column.ParentColumn == this) return true;
				column = column.ParentColumn;
			}
			return false;
		}
		public bool IsChildOrParent(OLAPCubeColumn column) {
			return IsParent(column) || column.IsParent(this);
		}
		public virtual byte TypeCode { get { return OLAPCubeColumnTypeCode; } }
		public override void SaveToStream(BinaryWriter writer) {
			base.SaveToStream(writer);
			writer.Write(Hierarchy.UniqueName);
			writer.Write(Level);
			writer.Write((int)BaseDataType);
			writer.Write(ParentColumn != null ? ParentColumn.UniqueName : string.Empty);
			writer.Write(AllMemberUniqueName != null ? AllMemberUniqueName : string.Empty);
			writer.Write(drilldownColumn != null ? drilldownColumn : string.Empty);
		}
		public override void RestoreFromStream(BinaryReader reader) {
			throw new Exception("Please call the overloaded method instead.");
		}
		public virtual void RestoreFromStream(BinaryReader reader, OLAPHierarchies hierarchies, OLAPCubeColumns columns) {
			base.RestoreFromStream(reader);
			string hierarchyUniqueName = reader.ReadString();
			OLAPHierarchy hierarchy = hierarchies[hierarchyUniqueName];
			int level = reader.ReadInt32(),
				baseDataType = reader.ReadInt32();
			string parentUniqueName = reader.ReadString();
			OLAPCubeColumn parent = string.IsNullOrEmpty(parentUniqueName) ? null : columns[parentUniqueName];
			string allMemberUniqueName = reader.ReadString();
			string drilldownColumn = reader.ReadString();
			Initialize(hierarchy, level, (OleDbType)baseDataType, parent, allMemberUniqueName, drilldownColumn);
		}
		public override string ToString() {
			return UniqueName;
		}
		public void LoadFilteredValues(bool filtered, object[] includedValues) {
			this.filtered = filtered;
			FilteredValues.Clear();
			foreach(object value in includedValues) {
				List<string> uniqueNames = GetUniqueNamesByValue(value);
				FilteredValues.AddRange(uniqueNames);
			}
		}
		public void IntersectFilteredValues(Dictionary<string, object> includedMembers) {
			if(!Filtered) {
				FilteredValues.Clear();
				FilteredValues.AddRange(includedMembers.Keys);
				Filtered = true;
			} else {
				for(int i = FilteredValues.Count - 1; i >= 0; i--) {
					if(!includedMembers.ContainsKey(FilteredValues[i]))
						FilteredValues.RemoveAt(i);
				}
			}
		}
		public void Assign(PivotGridFieldBase field) {
			SortOrder = field.SortOrder;
			SortMode = field.SortMode;
			TopValueCount = field.TopValueCount;
			TopValueShowOthers = field.TopValueShowOthers;
			TopValueType = field.TopValueType;
			SummaryType = field.SummaryType;
		}
		public string GetSortBySummaryMDX() {
			if(SortBySummaryMembers.Count == 0)
				return SortBySummary.UniqueName;
			else {
				StringBuilder res = new StringBuilder("(");
				int membersCount = SortBySummaryMembers.Count;
				for(int i = 0; i < membersCount; i++) {
					res.Append(SortBySummaryMembers[i].UniqueName).Append(", ");
				}
				res.Append(SortBySummary.UniqueName).Append(")");
				return res.ToString();
			}
		}
	}
	class OLAPCubeColumns : Dictionary<string, OLAPCubeColumn> {
		readonly List<OLAPCubeColumn> columnsList;
		readonly Dictionary<string, object> drillDownColumns;
		public OLAPCubeColumns()
			: base() {
			columnsList = new List<OLAPCubeColumn>();
			drillDownColumns = new Dictionary<string, object>();
		}
		public new OLAPCubeColumn this[string key] {
			get {
				if(string.IsNullOrEmpty(key)) return null;
				OLAPCubeColumn result;
				if(!TryGetValue(key, out result)) return null;
				return result;
			}
			set {
				base[key] = value;
			}
		}
		public object GetByDrillDownColumn(string drilldownColumn) {
			object res;
			return !drillDownColumns.TryGetValue(drilldownColumn, out res) ? null : res;
		}
		public OLAPCubeColumn this[int index] {
			get { return columnsList[index]; }
		}
		public string GetHierarchy(string levelUniqueName) {
			if(!ContainsKey(levelUniqueName)) throw new Exception("Column \"" + levelUniqueName + "\" doesn't exists.");
			return this[levelUniqueName].Hierarchy.UniqueName;
		}
		public void Add(OLAPCubeColumn column) {
			base.Add(column.UniqueName, column);
			columnsList.Add(column);
			object duplicateColumn;
			if(!drillDownColumns.TryGetValue(column.DrillDownColumn, out duplicateColumn))
				drillDownColumns.Add(column.DrillDownColumn, column);
			else {
				List<OLAPCubeColumn> list = duplicateColumn as List<OLAPCubeColumn>;
				if(list == null) {
					list = new List<OLAPCubeColumn>();
					list.Add(duplicateColumn as OLAPCubeColumn);
					drillDownColumns[column.DrillDownColumn] = list;
				}
				list.Add(column);
			}
		}
		public new void Clear() {
			base.Clear();
			columnsList.Clear();
		}
	}
	class OLAPMember : IOLAPMember {
		readonly object value;
		readonly string uniqueName;
		readonly OLAPCubeColumn column;
		public object Value { get { return value; } }
		public string UniqueName { get { return uniqueName; } }
		public OLAPCubeColumn Column { get { return column; } }
		public bool IsTotal { get { return Column.IsTotalMember(UniqueName); } }
		public bool IsOthers { get { return OLAPDataSourceQueryBase.IsOthersMember(UniqueName); } }
		public OLAPMember(OLAPCubeColumn column, string uniqueName, object value) {
			this.column = column;
			this.value = value;
			this.uniqueName = uniqueName;
		}
		public override string ToString() {
			return UniqueName;
		}
		static object allValue;
		public static object AllValue {
			get {
				if(allValue == null)
					allValue = new object();
				return allValue;
			}
		}
	}
	class OLAPGroupInfo {
		readonly int level;
		OLAPMember member;
		public int Level { get { return level; } }
		public OLAPMember Member { get { return member; } set { member = value; } }
		public virtual bool IsTotal { get { return member.IsTotal; } }
		public OLAPCubeColumn Column { get { return member != null ? member.Column : null; } }
		static OLAPGrandTotalGroupInfo grandTotalGroup = new OLAPGrandTotalGroupInfo();
		public static OLAPGroupInfo GrandTotalGroup { get { return grandTotalGroup; } }
		public OLAPGroupInfo(int level, OLAPMember member) {
			if(member == null) throw new ArgumentException("member can not be null");
			this.level = level;
			this.member = member;
		}
		public override string ToString() {
			return IsTotal ? "Total" : Member.ToString();
		}
	}
	class OLAPGrandTotalGroupInfo : OLAPGroupInfo {
		public override bool IsTotal { get { return true; } }
		public OLAPGrandTotalGroupInfo()
			: base(-1, new OLAPMember(null, OLAPCubeColumn.TotalMemberString, null)) {
		}
	}
	class OLAPSummary {
		readonly Dictionary<OLAPCubeColumn, object> summary;
		public OLAPSummary() {
			summary = new Dictionary<OLAPCubeColumn, object>();
		}
		public object this[OLAPCubeColumn column] {
			get {
				if(summary.ContainsKey(column)) return summary[column];
				return null;
			}
			set {
				if(!column.IsMeasure) throw new Exception("Summary can contain only measures.");
				if(summary.ContainsKey(column)) summary[column] = value;
				else summary.Add(column, value);
			}
		}
	}
	class DictionaryContainer<TKey, TValue> where TValue : new() {
		Dictionary<TKey, TValue> dictionary;
		public DictionaryContainer() {
			dictionary = new Dictionary<TKey, TValue>();
		}
		public Dictionary<TKey, TValue>.KeyCollection Keys { get { return dictionary.Keys; } }
		public Dictionary<TKey, TValue>.ValueCollection Values { get { return dictionary.Values; } }
		public TValue this[TKey key] {
			get {
				if(dictionary.ContainsKey(key))
					return dictionary[key];
				TValue result = new TValue();
				dictionary.Add(key, result);
				return result;
			}
			set { dictionary[key] = value; }
		}
		public void Clear() {
			dictionary.Clear();
		}
	}
	class OLAPGroupInfoColumn : DictionaryContainer<OLAPGroupInfo, OLAPSummary> {
		public OLAPGroupInfoColumn() {
		}
	}
	class OLAPCellTable : DictionaryContainer<OLAPGroupInfo, OLAPGroupInfoColumn> {
		public OLAPCellTable() {
		}
		public void ClearRows() {
			foreach(OLAPGroupInfoColumn column in Values)
				column.Clear();
		}
	}
	class OLAPAreaFieldValues {
		readonly List<OLAPGroupInfo> groups;
		public OLAPAreaFieldValues() {
			this.groups = new List<OLAPGroupInfo>();
		}
		public int Count { get { return Math.Max(0, groups.Count - 1); } }
		public OLAPGroupInfo this[int index] { 
			get { return index >= -1 && index + 1 < groups.Count ? groups[index + 1] : OLAPGroupInfo.GrandTotalGroup; } 
		}
		public OLAPGroupInfo this[int index, int level] {
			get {
				OLAPGroupInfo group = this[index];
				while(level < group.Level)
					group = this[--index];
				return group.Level == level ? group : null;
			}
		}
		public void Add(OLAPGroupInfo group) { groups.Add(group); }
		public void Add(IEnumerable<OLAPGroupInfo> groups) { this.groups.AddRange(groups); }
		public void Add(OLAPGroupInfo group, IEnumerable<OLAPGroupInfo> childs) {
			int index = groups.IndexOf(group);
			if(index < 0) throw new Exception("Field values doesn't contain the group.");
			List<OLAPGroupInfo> realChilds = new List<OLAPGroupInfo>();
			foreach(OLAPGroupInfo child in childs)
				if(child.Level > group.Level)
					realChilds.Add(child);
			groups.InsertRange(index + 1, realChilds);
		}
		public void Clear() { groups.Clear(); }
		public void RemoveChilds(int index) {
			if(index < 0 || index >= Count) throw new Exception("Field values doesn't contain the group.");
			if(index == Count - 1) return;
			int nextIndex = GetNextOrPrevIndex(index, true);
			if(nextIndex < 0) nextIndex = Count;
			RemoveRange(index + 1, nextIndex);
		}
		public bool Contains(OLAPGroupInfo group) {
			return groups.IndexOf(group) > -1;
		}
		public OLAPGroupInfo[] GetHierarchy(OLAPGroupInfo group) {
			int index = groups.IndexOf(group);
			if(index == -1) throw new Exception("Field values doesn't contain the group.");
			return GetHierarchy(index - 1);
		}
		public OLAPGroupInfo[] GetHierarchy(int index) {
			if(index < -1 || index >= Count) throw new Exception("Invalid group index.");
			OLAPGroupInfo group = this[index];
			OLAPGroupInfo[] result = new OLAPGroupInfo[group.Level + 1];
			for(int i = index + 1, level = group.Level; i >= 1 && level >= 0; i--)
				if(groups[i].Level == level) {
					result[level] = groups[i];
					level--;
				}
			return result;
		}
		public OLAPMember[] GetHierarchyMembers(int index) {
			OLAPGroupInfo[] hierarchy = GetHierarchy(index);
			OLAPMember[] members = new OLAPMember[hierarchy.Length];
			for(int i = 0; i < hierarchy.Length; i++)
				members[i] = hierarchy[i].Member;
			return members;
		}
		public int[] GetAllIndexes(int level) {
			if(level < 0) return new int[] { -1 };
			List<int> result = new List<int>();
			for(int i = 0; i < Count; i++)
				if(this[i].Level == level) result.Add(i);
			return result.ToArray();
		}
		public int[] GetAllIndexes(int level, object value) {
			if(level < 0) return new int[] { -1 };
			List<int> result = new List<int>();
			for(int i = 0; i < Count; i++)
				if(this[i].Level == level && object.Equals(this[i].Member.Value, value)) result.Add(i);
			return result.ToArray();
		}
		public OLAPGroupInfo GetNextGroup(int index) {
			return index < Count - 1 ? this[index + 1] : null;
		}
		public string GetTupleString(List<OLAPCubeColumn> columns, OLAPGroupInfo group) {
			if(group == OLAPGroupInfo.GrandTotalGroup)
				return columns[0].TotalMember.UniqueName;
			OLAPGroupInfo[] hierarchy = GetHierarchy(group);
			return GetTupleStringCore(columns, group, hierarchy, false);
		}
		public string GetTupleString(List<OLAPCubeColumn> columns, int publicIndex) {
			return GetTupleString(columns, publicIndex, false);
		}
		public string GetTupleString(List<OLAPCubeColumn> columns, int publicIndex, bool mdxFormat) {
			if(publicIndex == -1)
				return columns.Count > 0 ? columns[0].TotalMember.UniqueName : string.Empty;
			OLAPGroupInfo[] hierarchy = GetHierarchy(publicIndex);
			return GetTupleStringCore(columns, this[publicIndex], hierarchy, mdxFormat);
		}
		static string GetTupleStringCore(List<OLAPCubeColumn> columns, OLAPGroupInfo group, OLAPGroupInfo[] hierarchy, bool mdxFormat) {
			StringBuilder tuple = new StringBuilder();
			if(mdxFormat) tuple.Append("{ ");
			for(int i = 0; i < hierarchy.Length; i++) {
				if(hierarchy[i].Level != columns.Count - 1 && hierarchy[i] != group &&
					columns[hierarchy[i].Level + 1].ParentColumn != null &&
					columns[hierarchy[i].Level] != null &&
					columns[hierarchy[i].Level].IsParent(columns[hierarchy[i].Level + 1])) continue;
				tuple.Append(hierarchy[i].Member.UniqueName);
				if(mdxFormat) tuple.Append(" } * { ");
				else tuple.Append(".");
			}
			if(mdxFormat) {
				tuple.Length -= (" } * { ").Length;
				tuple.Append(" } ");
			} else tuple.Length--;
			return tuple.ToString();
		}
		public int GetIndex(object[] values) {
			if(values != null && values.Length > 0) {
				int level = 0;
				for(int i = 0; i < Count; i++)
					if(this[i].Level == level && object.Equals(this[i].Member.Value, values[level])) {
						level++;
						if(level == values.Length) return i;
					}
			}
			return -1;
		}
		public object[] GetValues(int index) {
			if(index < 0 || index >= Count) return null;
			object[] result = new object[this[index].Level + 1];
			for(int i = index, level = this[index].Level; i >= 0 && level >= 0; i--)
				if(this[i].Level == level) {
					result[level] = this[i].Member.Value;
					level--;
				}
			return result;
		}
		public object[] GetValues() {
			object[] result = new object[Count];
			for(int i = 0; i < Count; i++) {
				result[i] = this[i].Member.Value;
			}
			return result;
		}
		public int GetNextOrPrevIndex(int index, bool isNext) {
			int level = this[index].Level;
			if(isNext) {
				for(int i = index + 1; i < Count; i++)
					if(this[i].Level <= level) return i;
			} else {
				for(int i = index - 1; i >= 0; i--)
					if(this[i].Level <= level) return i;
			}
			return -1;
		}
		public bool Equals(List<OLAPGroupInfo> groups) {
			if(groups.Count != this.groups.Count) return false;
			for(int i = 0; i < groups.Count; i++)
				if(groups[i] != this.groups[i]) return false;
			return true;
		}
		public OLAPGroupInfo[] ToArray() {
			return ToArray(true);
		}
		public OLAPGroupInfo[] ToArray(bool includeGrandTotal) {
			if(includeGrandTotal) {
				if(groups.Count <= 1) return new OLAPGroupInfo[0];
				OLAPGroupInfo[] result = new OLAPGroupInfo[groups.Count - 1];
				groups.CopyTo(1, result, 0, groups.Count - 1);
				return result;
			} else
				return groups.ToArray();
		}
		public string[] GetUniqueMembers(OLAPCubeColumn column) {
			Dictionary<string, object> uniqueMembers = new Dictionary<string, object>();
			for(int i = 0; i < Count; i++) {
				if(this[i].Member.Column == column && !uniqueMembers.ContainsKey(this[i].Member.UniqueName))
					uniqueMembers.Add(this[i].Member.UniqueName, null);
			}
			string[] members = new string[uniqueMembers.Count];
			int j = 0;
			foreach(string member in uniqueMembers.Keys)
				members[j++] = member;
			return members;
		}
		public void Sort(int level, string[] sortedMembers) {
			if(level == 0) SortArea(0, Count, level, sortedMembers);
			else {
				for(int i = 0; i < Count; i++) {
					if(this[i].Level == level - 1) {
						int endIndex = GetNextOrPrevIndex(i, true);
						if(endIndex == -1) endIndex = Count;
						SortArea(i + 1, endIndex, level, sortedMembers);
						i = endIndex - 1;
					}
				}
			}
		}
		void SortArea(int startIndex, int endIndex, int level, string[] sortedMembers) {
			Dictionary<string, List<OLAPGroupInfo>> childs = new Dictionary<string, List<OLAPGroupInfo>>();
			for(int i = startIndex; i < endIndex; i++) {
				if(this[i].Level == level) {
					int nextIndex = GetNextOrPrevIndex(i, true);
					if(nextIndex < 0) nextIndex = endIndex;
					childs.Add(this[i].Member.UniqueName, GetRange(i, nextIndex));
				}
			}
			int currentIndex = startIndex;
			for(int i = 0; i < sortedMembers.Length; i++) {
				if(!childs.ContainsKey(sortedMembers[i])) continue;
				List<OLAPGroupInfo> groupChilds = childs[sortedMembers[i]];
				CopyFrom(groupChilds, currentIndex);
				currentIndex += groupChilds.Count;
			}
		}
		void RemoveRange(int startIndex, int endIndex) {
			groups.RemoveRange(startIndex + 1, endIndex - startIndex);
		}
		List<OLAPGroupInfo> GetRange(int startIndex, int endIndex) {
			return groups.GetRange(startIndex + 1, endIndex - startIndex);
		}
		void CopyFrom(List<OLAPGroupInfo> source, int destIndex) {
			int endIndex = Math.Min(groups.Count, destIndex + 1 + source.Count);
			for(int i = destIndex + 1; i < endIndex; i++)
				groups[i] = source[i - destIndex - 1];
		}
		public int MaxLevel { get { return GetMaxLevel(groups); } }
		public static int GetMaxLevel(IEnumerable<OLAPGroupInfo> groups) {
			int maxLevel = -1;
			foreach(OLAPGroupInfo group in groups)
				if(group.Level > maxLevel) maxLevel = group.Level;
			return maxLevel;
		}
	}
	class OLAPCellSet {
		List<string> columns;
		List<object[]> rows;
		Dictionary<int, OLAPGroupInfo> columnGroups, rowGroups;
		Dictionary<int, string> measures;
		List<OLAPTuple> columnTuples, rowTuples;
		int rowAreaColumnsCount;
		public List<OLAPTuple> ColumnTuples { get { return columnTuples; } }
		public List<OLAPTuple> RowTuples { get { return rowTuples; } }
		public OLAPCellSet(IDataReader reader) {
			columns = new List<string>();
			rows = new List<object[]>();
			columnGroups = new Dictionary<int, OLAPGroupInfo>();
			rowGroups = new Dictionary<int, OLAPGroupInfo>();
			measures = new Dictionary<int, string>();
			columnTuples = new List<OLAPTuple>();
			rowTuples = new List<OLAPTuple>();
			rowAreaColumnsCount = -1;
			Read(reader);
		}
		void Read(IDataReader reader) {
			columns.Clear();
			for(int i = 0; i < reader.FieldCount; i++)
				columns.Add(reader.GetName(i));
			rows.Clear();
			while(reader.Read()) {
				object[] row = new object[reader.FieldCount];
				reader.GetValues(row);
				for(int i = 0; i < row.Length; i++)
					if(row[i] is DBNull) row[i] = null;
				rows.Add(row);
			}
		}
		public object[] this[int rowIndex] { get { return rows[rowIndex]; } }
		public int ColumnCount { get { return columns.Count; } }
		public int RowCount { get { return rows.Count; } }
		public int RowAreaColumnsCount {
			get {
				if(rowAreaColumnsCount == -1) {
					rowAreaColumnsCount = columns.FindIndex(delegate(string target) { return !target.EndsWith(OLAPDataSourceQueryBase.MemberUniqueName); });
					Debug.Assert(rowAreaColumnsCount >= 0);
				}
				return rowAreaColumnsCount;
			}
		}
		public string GetColumn(int index) { return columns[index]; }
		public int GetOrdinal(string column) { return columns.IndexOf(column); }
		public int GetOrdinal(OLAPCubeColumn column) { return GetOrdinal(column.UniqueName + "." + OLAPDataSourceQueryBase.MemberUniqueName); }
		public bool ContainsColumn(string column) { return columns.Contains(column); }
		public int FindColumn(int startIndex, params string[] nameParts) {
			for(int i = startIndex; i < columns.Count; i++)
				if(IsColumnFits(columns[i], nameParts)) return i;
			return -1;
		}
		public int FindColumn(int startIndex, OLAPTuple tuple) {
			for(int i = startIndex; i < columns.Count; i++)
				if(columns[i].StartsWith(tuple.FlattenedString)) return i;
			return -1;
		}		
		public int FindRow(int startIndex, OLAPTuple tuple) {
			Dictionary<int, string> needle = new Dictionary<int, string>();
			Dictionary<int, bool> totals = new Dictionary<int, bool>();
			for(int i = 0; i < tuple.MemberCount; i++) {
				int columnIndex = GetOrdinal(tuple[i].Column.UniqueName + "." + OLAPDataSourceQueryBase.MemberUniqueName);
				if(columnIndex < 0) return -1;
				needle.Add(columnIndex, tuple[i].UniqueName);
				totals.Add(columnIndex, tuple[i].IsTotal);
			}
			return FindRowCore(startIndex, needle, totals);
		}
		protected int FindRowCore(int startIndex, Dictionary<int, string> needles, Dictionary<int, bool> totals) {
			for(int i = startIndex; i < rows.Count; i++) {
				if(IsRowFits(rows[i], needles, totals)) 
					return i;
			}
			return -1;
		}
		protected bool IsColumnFits(string column, string[] nameParts) {
			foreach(string namePart in nameParts)
				if(!column.Contains(namePart)) return false;
			return true;
		}
		protected bool IsRowFits(object[] row, Dictionary<int, string> needles, Dictionary<int, bool> totals) {
			foreach(KeyValuePair<int, string> needle in needles) {
				if(needle.Key >= row.Length || (!totals[needle.Key] && row[needle.Key] as string != needle.Value) ||
					(totals[needle.Key] && row[needle.Key] != null && row[needle.Key] as string != needle.Value)) return false;
			}
			return true;
		}
		public OLAPGroupInfo GetColumnGroup(int columnIndex) {
			if(columnGroups.ContainsKey(columnIndex))
				return columnGroups[columnIndex];
			return null;
		}
		public void SetColumnGroup(int columnIndex, OLAPGroupInfo group) {
			Debug.Assert(group != null);
			columnGroups.Add(columnIndex, group);
		}
		public bool IsColumnGroupsEmpty { get { return columnGroups.Count == 0; } }
		public OLAPGroupInfo GetRowGroup(int rowIndex) {
			if(rowGroups.ContainsKey(rowIndex))
				return rowGroups[rowIndex];
			return null;
		}
		public void SetRowGroup(int rowIndex, OLAPGroupInfo group) {
			Debug.Assert(group != null);
			rowGroups.Add(rowIndex, group);
		}
		public bool IsRowGroupsEmpty { get { return rowGroups.Count == 0; } }
		public string GetMeasure(int columnIndex) {
			if(measures.ContainsKey(columnIndex))
				return measures[columnIndex];
			return null;
		}
		public void SetMeasure(int columnIndex, string measure) {
			measures.Add(columnIndex, measure);
		}
		public bool IsMeasuresEmpty { get { return measures.Count == 0; } }
	}
	abstract class AbstractActionProvider {
		public delegate void QueryMembersDelegate(OLAPCubeColumn column, string[] uniqueNames);
		public abstract List<OLAPCubeColumn> GetColumns(List<OLAPCubeColumn> area, OLAPAreaFieldValues fieldValues, OLAPGroupInfo[] groups);
		public abstract List<OLAPTuple> GetTuples(List<OLAPCubeColumn> area, OLAPAreaFieldValues fieldValues, OLAPGroupInfo[] groups);
		public virtual void ParseColumns(OLAPCellSet queryResult, List<OLAPCubeColumn> area, List<OLAPCubeColumn> dataArea,
			OLAPAreaFieldValues fieldValues, List<OLAPTuple> tuples, QueryMembersDelegate queryMembers) {
			for(int i = 0; i < queryResult.ColumnCount; i++) {
				string tuple = queryResult.GetColumn(i);
				if(tuple.Contains("[MEMBER_UNIQUE_NAME]")) continue;
				int measureIndex = tuple.IndexOf("[Measures].");
				queryResult.SetMeasure(i, tuple.Substring(measureIndex));
			}
		}
		public abstract void ParseRows(OLAPCellSet queryResult, List<OLAPCubeColumn> area, OLAPAreaFieldValues fieldValues, List<OLAPTuple> tuples,
			QueryMembersDelegate queryMembers);
		protected List<string> GetUniqueMembers(List<string> members) {
			List<string> result = new List<string>();
			for(int i = 0; i < members.Count; i++) {
				if(i > 0 && members[i] == members[i - 1]) continue;
				result.Add(members[i]);
			}
			return result;
		}
	}
	class ExpandActionProvider : AbstractActionProvider {
		public override List<OLAPCubeColumn> GetColumns(List<OLAPCubeColumn> area, OLAPAreaFieldValues fieldValues, OLAPGroupInfo[] groups) {
			List<OLAPCubeColumn> result = new List<OLAPCubeColumn>();
			Debug.Assert(groups[0].Level < area.Count - 1, "Cannot expand last level");
			result.Add(area[groups[0].Level + 1]);
			return result;
		}
		public override List<OLAPTuple> GetTuples(List<OLAPCubeColumn> area, OLAPAreaFieldValues fieldValues, OLAPGroupInfo[] groups) {
			List<OLAPTuple> result = new List<OLAPTuple>();
			foreach(OLAPGroupInfo group in groups) {
				Debug.Assert(!group.IsTotal);
				result.Add(new OLAPTuple(group, fieldValues));
			}
			return result;
		}
		OLAPTuple GetCurrentTuple(List<OLAPTuple> tuples, string tupleString, bool isParent) {
			foreach(OLAPTuple tuple in tuples) {
				if((isParent && tupleString.Contains(tuple.FlattenedString + ".[Measures].")) ||
					(!isParent && tupleString.Contains(tuple.FlattenedString))) return tuple;
			}
			return null;
		}
		public override void ParseColumns(OLAPCellSet queryResult, List<OLAPCubeColumn> area, List<OLAPCubeColumn> dataArea, OLAPAreaFieldValues fieldValues,
				List<OLAPTuple> tuples, QueryMembersDelegate queryMembers) {
			base.ParseColumns(queryResult, area, dataArea, fieldValues, tuples, queryMembers);
			bool isParent = tuples[0].Last.Column.IsParent(area[0]);
			Dictionary<OLAPTuple, int> tuplesStartIndexes = new Dictionary<OLAPTuple, int>();
			Dictionary<OLAPTuple, List<string>> tuplesMembers = new Dictionary<OLAPTuple, List<string>>();
			foreach(OLAPTuple tuple in tuples) {
				tuplesStartIndexes.Add(tuple, -1);
				tuplesMembers.Add(tuple, new List<string>());
			}
			OLAPTuple currentTuple = null, lastTuple = null;
			for(int i = queryResult.RowAreaColumnsCount; i < queryResult.ColumnCount; i++) {
				string tuple = queryResult.GetColumn(i);
				OLAPTuple columnTuple = GetCurrentTuple(tuples, tuple, isParent);
				if(columnTuple != null) {
					if(currentTuple != columnTuple)
						tuplesStartIndexes[columnTuple] = i;
					currentTuple = columnTuple;
					if(isParent) {
						lastTuple = columnTuple;
						continue;
					}
				} else {
					if(lastTuple != null && isParent)
						tuplesStartIndexes[lastTuple] = i;
				}
				lastTuple = columnTuple;
				if(currentTuple == null) continue;
				int measureIndex = tuple.IndexOf("[Measures]"),
					hierarchyIndex = tuple.IndexOf(area[0].Hierarchy.UniqueName);
				Debug.Assert(measureIndex >= 0);
				Debug.Assert(hierarchyIndex >= 0);
				string member = tuple.Substring(hierarchyIndex, measureIndex - hierarchyIndex - 1);
				tuplesMembers[currentTuple].Add(member);
			}
			List<string> allMembers = new List<string>();
			foreach(List<string> members in tuplesMembers.Values)
				allMembers.AddRange(members);
			if(allMembers.Count > 0) queryMembers(area[0], GetUniqueMembers(allMembers).ToArray());
			foreach(OLAPTuple tuple in tuples) {
				List<string> membersUniqueName = tuplesMembers[tuple];
				if(membersUniqueName.Count == 0) continue;
				int startIndex = tuplesStartIndexes[tuple];
				CreateChildGroups(area[0], fieldValues, tuple, startIndex, membersUniqueName, queryResult.SetColumnGroup);
			}
		}
		public override void ParseRows(OLAPCellSet queryResult, List<OLAPCubeColumn> area, OLAPAreaFieldValues fieldValues, List<OLAPTuple> tuples,
			QueryMembersDelegate queryMembers) {
			int columnOrdinal = queryResult.GetOrdinal(area[0]);
			if(columnOrdinal < 0) return;
			bool isParent = tuples[0].Last.Column.IsParent(area[0]);
			Dictionary<OLAPTuple, List<string>> tuplesMembers = new Dictionary<OLAPTuple, List<string>>();
			Dictionary<int, OLAPTuple> tuplesStartIndexes = new Dictionary<int, OLAPTuple>();
			Dictionary<OLAPTuple, int> tuplesStartIndexes2 = new Dictionary<OLAPTuple, int>();
			int currentIndex = 0;
			foreach(OLAPTuple tuple in tuples) {
				int index = queryResult.FindRow(currentIndex, tuple);
				if(index < 0) continue;
				if(isParent) index++;
				tuplesStartIndexes.Add(index, tuple);
				tuplesStartIndexes2.Add(tuple, index);
				currentIndex = index;
				tuplesMembers.Add(tuple, new List<string>());
			}
			if(tuplesStartIndexes.Count == 0) return;
			OLAPTuple currentTuple = null;
			for(int i = 0; i < queryResult.RowCount; i++) {
				if(tuplesStartIndexes.ContainsKey(i))
					currentTuple = tuplesStartIndexes[i];
				if(currentTuple == null) continue;
				string member = (string)queryResult[i][columnOrdinal];
				if(string.IsNullOrEmpty(member)) continue;
				tuplesMembers[currentTuple].Add(member);
			}
			List<string> allMembers = new List<string>();
			foreach(List<string> members in tuplesMembers.Values)
				allMembers.AddRange(members);
			if(allMembers.Count > 0) queryMembers(area[0], GetUniqueMembers(allMembers).ToArray());
			foreach(OLAPTuple tuple in tuples) {
				List<string> membersUniqueName = tuplesMembers[tuple];
				if(membersUniqueName.Count == 0) continue;
				int startIndex = tuplesStartIndexes2[tuple];
				CreateChildGroups(area[0], fieldValues, tuple, startIndex, membersUniqueName, queryResult.SetRowGroup);
			}
		}
		protected delegate void SetGroupIndex(int index, OLAPGroupInfo group);
		protected void CreateChildGroups(OLAPCubeColumn column, OLAPAreaFieldValues fieldValues, OLAPTuple tuple, int startIndex,
				List<string> membersUniqueName, SetGroupIndex setGroupIndex) {
			List<OLAPGroupInfo> childs = new List<OLAPGroupInfo>();
			OLAPGroupInfo lastGroupInfo = null;
			for(int i = 0; i < membersUniqueName.Count; i++) {
				if(i > 0 && membersUniqueName[i] == membersUniqueName[i - 1]) {
					setGroupIndex(i + startIndex, lastGroupInfo);
					continue;
				}
				OLAPGroupInfo groupInfo = new OLAPGroupInfo(tuple.BaseGroup.Level + 1, column[membersUniqueName[i]]);
				childs.Add(groupInfo);
				setGroupIndex(i + startIndex, groupInfo);
				lastGroupInfo = groupInfo;
			}
			fieldValues.Add(tuple.BaseGroup, childs);
		}
	}
	class AllGroupsActionProvider : AbstractActionProvider {
		public override List<OLAPCubeColumn> GetColumns(List<OLAPCubeColumn> area, OLAPAreaFieldValues fieldValues, OLAPGroupInfo[] groups) {
			List<OLAPCubeColumn> result = new List<OLAPCubeColumn>();
			if(area.Count == 0) return result;
			int maxLevel = OLAPAreaFieldValues.GetMaxLevel(groups);
			for(int i = 0; i <= maxLevel; i++)
				result.Add(area[i]);
			return result;
		}
		public override List<OLAPTuple> GetTuples(List<OLAPCubeColumn> area, OLAPAreaFieldValues fieldValues, OLAPGroupInfo[] groups) {
			int maxLevel = fieldValues.MaxLevel;
			Debug.Assert(area.Count > maxLevel);
			if(maxLevel == -1) return new List<OLAPTuple>();
			List<OLAPTuple> result = new List<OLAPTuple>();
			List<OLAPMember> lastMembers = new List<OLAPMember>(maxLevel + 1);
			List<int> levelMap = new List<int>(maxLevel + 1);
			int tupleIndex = -1;
			for(int i = 0; i < maxLevel + 1; i++) {
				lastMembers.Add(null);
				if(i == 0 || area[i].ParentColumn != area[i - 1]) tupleIndex++;
				levelMap.Add(tupleIndex);
			}
			OLAPMember[] members = new OLAPMember[tupleIndex + 1];
			result.Add(CreateGrandTotal(area, levelMap, maxLevel, tupleIndex + 1));
			for(int i = 0; i < fieldValues.Count; i++) {
				if(lastMembers[fieldValues[i].Level] != fieldValues[i].Member) {
					lastMembers[fieldValues[i].Level] = fieldValues[i].Member;
					for(int j = fieldValues[i].Level + 1; j < lastMembers.Count; j++)
						lastMembers[j] = area[j].TotalMember;
				}
				CopyLastMembers(lastMembers, levelMap, members);
				result.Add(new OLAPTuple(fieldValues[i], members));
			}
			return result;
		}
		static OLAPTuple CreateGrandTotal(List<OLAPCubeColumn> area, List<int> levelMap, int maxLevel, int tupleLevelCount) {
			OLAPMember[] members = new OLAPMember[tupleLevelCount];
			for(int i = maxLevel; i >= 0; i--)
				members[levelMap[i]] = area[i].TotalMember;
			return new OLAPTuple(OLAPGroupInfo.GrandTotalGroup, members);
		}
		static void ResetMembers(OLAPMember[] members) {
			for(int j = 0; j < members.Length; j++)
				members[j] = null;
		}
		static void CopyLastMembers(List<OLAPMember> lastMembers, List<int> levelMap, OLAPMember[] members) {
			ResetMembers(members);
			for(int j = 0; j < lastMembers.Count; j++) {
				if(members[levelMap[j]] == null || !lastMembers[j].IsTotal)
					members[levelMap[j]] = lastMembers[j];
			}
		}
		public override void ParseColumns(OLAPCellSet queryResult, List<OLAPCubeColumn> area, List<OLAPCubeColumn> dataArea, OLAPAreaFieldValues fieldValues,
			List<OLAPTuple> tuples, QueryMembersDelegate queryMembers) {
			base.ParseColumns(queryResult, area, dataArea, fieldValues, tuples, queryMembers);
			int startIndex = queryResult.RowAreaColumnsCount;
			for(int i = 0; i < tuples.Count; i++) {
				int tupleIndex = queryResult.FindColumn(startIndex, tuples[i]);
				if(tupleIndex < 0) continue;
				string match = tuples[i].FlattenedString + ".[Measures]";
				while(tupleIndex < queryResult.ColumnCount && queryResult.GetColumn(tupleIndex).StartsWith(match)) {
					queryResult.SetColumnGroup(tupleIndex, tuples[i].BaseGroup);
					tupleIndex++;
				}
				startIndex = tupleIndex;
			}
		}
		public override void ParseRows(OLAPCellSet queryResult, List<OLAPCubeColumn> area, OLAPAreaFieldValues fieldValues, List<OLAPTuple> tuples,
			QueryMembersDelegate queryMembers) {
			int startIndex = 0;
			for(int i = 0; i < tuples.Count; i++) {
				int tupleIndex = queryResult.FindRow(startIndex, tuples[i]);
				if(tupleIndex < 0) continue;
				queryResult.SetRowGroup(tupleIndex, tuples[i].BaseGroup);
				startIndex = tupleIndex;
			}
		}
	}
	class FirstLevelActionProvider : AbstractActionProvider {
		public override List<OLAPCubeColumn> GetColumns(List<OLAPCubeColumn> area, OLAPAreaFieldValues fieldValues, OLAPGroupInfo[] groups) {
			List<OLAPCubeColumn> result = new List<OLAPCubeColumn>();
			if(area.Count > 0) result.Add(area[0]);
			return result;
		}
		public override List<OLAPTuple> GetTuples(List<OLAPCubeColumn> area, OLAPAreaFieldValues fieldValues, OLAPGroupInfo[] groups) {
			return new List<OLAPTuple>();
		}
		public override void ParseColumns(OLAPCellSet queryResult, List<OLAPCubeColumn> area, List<OLAPCubeColumn> dataArea, OLAPAreaFieldValues fieldValues,
			List<OLAPTuple> tuples, QueryMembersDelegate queryMembers) {
			base.ParseColumns(queryResult, area, dataArea, fieldValues, tuples, queryMembers);
			if(area.Count == 0) {
				for(int i = queryResult.RowAreaColumnsCount; i < queryResult.ColumnCount; i++) {
					Debug.Assert(queryResult.GetColumn(i).StartsWith("[Measures]"));
					queryResult.SetColumnGroup(i, OLAPGroupInfo.GrandTotalGroup);
				}
				return;
			}
			Debug.Assert(area.Count == 1);
			List<string> membersUniqueName = new List<string>();
			for(int i = queryResult.RowAreaColumnsCount; i < queryResult.ColumnCount; i++) {
				string tuple = queryResult.GetColumn(i);
				int measureIndex = tuple.IndexOf("[Measures].");
				Debug.Assert(measureIndex > 0);
				string member = tuple.Substring(0, measureIndex - 1);
				membersUniqueName.Add(member);
			}
			queryMembers(area[0], GetUniqueMembers(membersUniqueName).ToArray());
			fieldValues.Clear();
			OLAPGroupInfo lastGroupInfo = null;
			for(int i = 0; i < membersUniqueName.Count; i++) {
				if(i > 0 && membersUniqueName[i] == membersUniqueName[i - 1]) {
					queryResult.SetColumnGroup(i + queryResult.RowAreaColumnsCount, lastGroupInfo);
					continue;
				}
				OLAPMember groupMember = area[0][membersUniqueName[i]];
				if(groupMember == null) continue;
				OLAPGroupInfo groupInfo = groupMember.IsTotal ? OLAPGroupInfo.GrandTotalGroup : new OLAPGroupInfo(0, groupMember);
				fieldValues.Add(groupInfo);
				queryResult.SetColumnGroup(i + queryResult.RowAreaColumnsCount, groupInfo);
				lastGroupInfo = groupInfo;
			}
		}
		public override void ParseRows(OLAPCellSet queryResult, List<OLAPCubeColumn> area, OLAPAreaFieldValues fieldValues, List<OLAPTuple> tuples,
			QueryMembersDelegate queryMembers) {
			if(area.Count == 0) {
				queryResult.SetRowGroup(0, OLAPGroupInfo.GrandTotalGroup);
				return;
			}
			Debug.Assert(area.Count == 1);
			int ordinal = queryResult.GetOrdinal(area[0]);
			List<string> membersUniqueName = new List<string>();
			for(int i = 0; i < queryResult.RowCount; i++) {
				string member = (string)queryResult[i][ordinal];
				if(string.IsNullOrEmpty(member)) member = OLAPDataSourceQueryBase.GetTotalMember(area[0]);
				membersUniqueName.Add(member);
			}
			queryMembers(area[0], membersUniqueName.ToArray());
			fieldValues.Clear();
			for(int i = 0; i < membersUniqueName.Count; i++) {
				OLAPMember groupMember = area[0][membersUniqueName[i]];
				if(groupMember == null) continue;
				OLAPGroupInfo groupInfo = groupMember.IsTotal ? OLAPGroupInfo.GrandTotalGroup : new OLAPGroupInfo(0, groupMember);
				fieldValues.Add(groupInfo);
				queryResult.SetRowGroup(i, groupInfo);
			}
		}
	}
	interface IOLAPDataSourceQueryExecutorOwner {
		List<OLAPCubeColumn> ColumnArea { get; }
		List<OLAPCubeColumn> RowArea { get; }
		List<OLAPCubeColumn> DataArea { get; }
		List<OLAPCubeColumn> FilterArea { get; }
		bool IsDesignMode { get; }
		OLAPCellTable Cells { get; }
		OLAPAreaFieldValues ColumnValues { get; }
		OLAPAreaFieldValues RowValues { get; }
		OleDbConnection OLAPConnection { get; }
		int QueryTimeout { get; }
		string CubeName { get; }
		OLAPCubeColumns CubeColumns { get; }
		PivotOLAPKPIMeasures GetKPIMeasures(string name);
	}
	abstract class OLAPDataSourceQueryExecutorBase {
		IOLAPDataSourceQueryExecutorOwner owner;
		OLAPDataSourceQueryBase query;
		protected IOLAPDataSourceQueryExecutorOwner Owner { get { return owner; } }
		public OLAPDataSourceQueryBase QueryBuilder { get { return query; } }
		protected List<OLAPCubeColumn> ColumnArea { get { return Owner.ColumnArea; } }
		protected List<OLAPCubeColumn> RowArea { get { return Owner.RowArea; } }
		protected List<OLAPCubeColumn> DataArea { get { return Owner.DataArea; } }
		protected List<OLAPCubeColumn> FilterArea { get { return Owner.FilterArea; } }
		protected bool IsDesignMode { get { return Owner.IsDesignMode; } }
		protected OLAPCellTable Cells { get { return Owner.Cells; } }
		protected OLAPAreaFieldValues ColumnValues { get { return Owner.ColumnValues; } }
		protected OLAPAreaFieldValues RowValues { get { return Owner.RowValues; } }
		protected bool Connected { get { return OLAPConnection != null; } }
		protected OleDbConnection OLAPConnection { get { return Owner.OLAPConnection; } }
		protected int QueryTimeout { get { return Owner.QueryTimeout; } }
		protected string CubeName { get { return Owner.CubeName; } }
		protected OLAPCubeColumns CubeColumns { get { return Owner.CubeColumns; } }
		public OLAPDataSourceQueryExecutorBase(IOLAPDataSourceQueryExecutorOwner owner) {
			if(owner == null) throw new Exception("OLAPDataSourceQueryExecutorBase can't be instantiated with a null owner.");
			this.owner = owner;
			this.query = CreateQuery();
		}
		protected abstract OLAPDataSourceQueryBase CreateQuery();
		public OLAPPivotDrillDownDataSource QueryDrillDown(OLAPMember[] columnMembers, OLAPMember[] rowMembers,
							OLAPCubeColumn measure, int maxRowCount, List<string> customColumns) {
			if(columnMembers == null || rowMembers == null) throw new ArgumentException("null argument");
			List<string> filters = new List<string>();
			filters.Add(measure.UniqueName);
			CreateFilters(filters, columnMembers);
			CreateFilters(filters, rowMembers);
			foreach(OLAPCubeColumn column in GetFilters(FilterArea)) {
				if(column.FilteredValues.Count > 1)
					throw new Exception(PivotGridLocalizer.GetString(PivotGridStringId.OLAPDrillDownFilterException));
				filters.AddRange(column.FilteredValues);
			}
			List<string> drilldownColumns;
			if(customColumns != null && customColumns.Count > 0)
				drilldownColumns = customColumns;
			else {
				drilldownColumns = new List<string>(ColumnArea.Count + RowArea.Count + 1);
				CreateDrillDownColumns(drilldownColumns, ColumnArea, false);
				CreateDrillDownColumns(drilldownColumns, RowArea, false);
				CreateDrillDownColumns(drilldownColumns, FilterArea, true);
				CreateDrillDownColumn(drilldownColumns, measure);
			}
			string mdxQuery = QueryBuilder.GetDrillDownQueryString(CubeName, filters, drilldownColumns, maxRowCount);
			using(OleDbCommand command = new OleDbCommand(mdxQuery, OLAPConnection)) {
				command.CommandTimeout = QueryTimeout;
				try {
					using(OleDbDataReader reader = ExecuteReader(command)) {
						List<OLAPCubeColumn[]> returnColumns = GetReturnColumns(reader);
						return new OLAPPivotDrillDownDataSource(reader, new DrillDownFilter(returnColumns).GetFilter());
					}
				} catch { return null; }
			}
		}
		List<OLAPCubeColumn[]> GetReturnColumns(OleDbDataReader reader) {
			List<OLAPCubeColumn[]> res = new List<OLAPCubeColumn[]>();
			for(int i = 0; i < reader.FieldCount; i++) {
				object columns = CubeColumns.GetByDrillDownColumn(reader.GetName(i));
				if(columns == null) {
					res.Add(null);	
				} else {
					OLAPCubeColumn column = columns as OLAPCubeColumn;
					if(column != null) {
						res.Add(column.Filtered ? new OLAPCubeColumn[] { column } : null);
					} else {
						List<OLAPCubeColumn> list = (List<OLAPCubeColumn>)columns;
						for(int j = list.Count - 1; j >= 0; j--) {
							if(!list[j].Filtered)
								list.RemoveAt(j);
						}
						if(list.Count > 0)
							res.Add(list.ToArray());
						else
							res.Add(null);
					}					
				}
			}
			return res;
		}
		void CreateDrillDownColumns(List<string> drilldownColumns, List<OLAPCubeColumn> columns, bool filteredOnly) {
			for(int i = 0; i < columns.Count; i++) {
				if(filteredOnly && !columns[i].Filtered) continue;
				CreateDrillDownColumn(drilldownColumns, columns[i]);
			}
		}
		void CreateDrillDownColumn(List<string> drilldownColumns, OLAPCubeColumn column) {
			if(!column.IsMeasure)
				drilldownColumns.Add("MemberValue(" + column.DrillDownColumn + ")");
			else
				drilldownColumns.Add(column.DrillDownColumn);
		}
		void CreateFilters(List<string> filters, OLAPMember[] members) {
			OLAPMember lastMember = null;
			foreach(OLAPMember member in members) {
				if(lastMember == null || !lastMember.Column.IsParent(member.Column))
					filters.Add(member.UniqueName);
				else
					filters[filters.Count - 1] = member.UniqueName;
				lastMember = member;
			}
		}
		public bool QueryData(OLAPGroupInfo[] columns, OLAPGroupInfo[] rows, bool columnExpand, bool rowExpand) {
			Debug.Assert((columns != null && rows != null) || (columns == null && rows == null));
			if(DataArea.Count == 0) return false;
			AbstractActionProvider columnProvider = GetActionProvider(columns, columnExpand),
				rowProvider = GetActionProvider(rows, rowExpand);
			List<OLAPCubeColumn> columnArea = columnProvider.GetColumns(ColumnArea, ColumnValues, columns),
				rowArea = rowProvider.GetColumns(RowArea, RowValues, rows);
			List<OLAPTuple> columnTuples = columnProvider.GetTuples(ColumnArea, ColumnValues, columns),
				rowTuples = rowProvider.GetTuples(RowArea, RowValues, rows);
			string mdxQuery = QueryBuilder.GetQueryString(CubeName, columnArea, rowArea, columnTuples, rowTuples, DataArea,
				GetColumnRowFilters(), GetFilters(FilterArea), columnExpand, rowExpand);
			using(OleDbCommand command = new OleDbCommand(mdxQuery, OLAPConnection)) {
				command.CommandTimeout = QueryTimeout;
				try {
					using(OleDbDataReader reader = ExecuteReader(command)) {
						if(!reader.HasRows || reader.FieldCount == 0)
							return false;
						OLAPCellSet queryResult = new OLAPCellSet(reader);
						columnProvider.ParseColumns(queryResult, columnArea, DataArea, ColumnValues, columnTuples, QueryMembers);
						rowProvider.ParseRows(queryResult, rowArea, RowValues, rowTuples, QueryMembers);
						if(queryResult.IsColumnGroupsEmpty || queryResult.IsRowGroupsEmpty || queryResult.IsMeasuresEmpty)
							return false;
						else
							ReadData(queryResult);
					}
				} catch { return false; }
			}
			return true;
		}
		OleDbDataReader ExecuteReader(OleDbCommand command) {
			try {
				return command.ExecuteReader();
			} catch(OleDbException exception) {
				if(IsTimeoutException(exception))
					RaiseOLAPQueryTimeoutEvent(exception);
				throw exception;
			}
		}
		bool IsTimeoutException(OleDbException exception) {
			return exception.ErrorCode == -2147217900 ||
				(exception.ErrorCode == -2147467259 && exception.Errors.Count > 0 &&
							exception.Errors[0].NativeError == -1056178127);
		}
		void RaiseOLAPQueryTimeoutEvent(OleDbException exception) {
			(((PivotGridOLAPDataSource)Owner).Data).OLAPQueryTimeout();
		}
		List<OLAPCubeColumn> GetColumnRowFilters() {
			List<OLAPCubeColumn> filterArea = new List<OLAPCubeColumn>();
			filterArea.AddRange(GetFilters(ColumnArea));
			filterArea.AddRange(GetFilters(RowArea));
			return filterArea;
		}
		List<OLAPCubeColumn> GetFilters(List<OLAPCubeColumn> area) {
			List<OLAPCubeColumn> result = new List<OLAPCubeColumn>();
			for(int i = 0; i < area.Count; i++)
				if(area[i].Filtered) {
					result.Add(area[i]);
					area[i].VisualTotalsIncludeAllMember = i == 0 || area[i].ParentColumn != area[i - 1];
				}
			return result;
		}
		void ReadData(OLAPCellSet queryResult) {
			for(int i = 0; i < queryResult.ColumnCount; i++)
				for(int j = 0; j < queryResult.RowCount; j++) {
					OLAPGroupInfo columnGroup = queryResult.GetColumnGroup(i),
						rowGroup = queryResult.GetRowGroup(j);
					if(columnGroup == null || rowGroup == null) continue;
					string measureUniqueName = queryResult.GetMeasure(i);
					OLAPCubeColumn measure = CubeColumns[measureUniqueName];
					if(measure != null) Cells[columnGroup][rowGroup][measure] = queryResult[j][i];
				}
		}
		AbstractActionProvider GetActionProvider(OLAPGroupInfo[] groups, bool isExpand) {
			if(isExpand)
				return new ExpandActionProvider();
			else {
				if(groups.Length == 0)
					return new FirstLevelActionProvider();
				else
					return new AllGroupsActionProvider();
			}
		}
		public void QueryMembers(OLAPCubeColumn column, string[] uniqueNames) {
			if(!Connected || IsDesignMode) return;
			string[] newMembers = uniqueNames != null && uniqueNames.Length > 0 ? Array.FindAll<string>(uniqueNames,
					delegate(string target) {
						return column[target] == null && !column.IsTotalMember(target);
					}) : null;
			if(uniqueNames != null && newMembers.Length == 0) return;
			string mdxQuery = QueryBuilder.GetMembersQueryString(CubeName, column.UniqueName, column.Hierarchy.UniqueName, newMembers);
			using(OleDbCommand command = new OleDbCommand(mdxQuery, OLAPConnection)) {
				command.CommandTimeout = QueryTimeout;
				using(OleDbDataReader reader = ExecuteReader(command)) {
					ReadColumnMembersCore(column, reader);
				}
			}
			if(uniqueNames == null || uniqueNames.Length == 0) column.AllMembersLoaded = true;
		}
		protected void ReadColumnMembersCore(OLAPCubeColumn column, OleDbDataReader reader) {
			int memberNameIndex = -1,
				memberValueIndex = -1;
			try {
				memberNameIndex = reader.GetOrdinal(column.UniqueName + "." + OLAPDataSourceQueryBase.MemberUniqueName);
				memberValueIndex = reader.GetOrdinal(OLAPDataSourceQueryBase.TempMeasureName);
			} catch(IndexOutOfRangeException) { }
			if(memberNameIndex < 0 || memberValueIndex < 0) return;
			while(reader.Read()) {
				string uniqueName = (string)reader.GetValue(memberNameIndex);
				object value = reader.GetValue(memberValueIndex);
				if(value is DBNull)
					value = null;
				column.AddMember(new OLAPMember(column, uniqueName, value));
			}
		}
		public bool QueryNullValues(OLAPCubeColumn column) {
			if(!Connected || IsDesignMode) return false;
			string mdxQuery = QueryBuilder.GetNullValuesQueryString(CubeName, column.UniqueName, column.Hierarchy.UniqueName);
			using(OleDbCommand command = new OleDbCommand(mdxQuery, OLAPConnection)) {
				command.CommandTimeout = QueryTimeout;
				try {
					using(OleDbDataReader reader = ExecuteReader(command)) {
						return reader.FieldCount > 0;
					}
				} catch { return false; }
			}
		}
		public bool[] QueryCalculatedMembers(List<OLAPCubeColumn> columns) {
			if(!Connected || IsDesignMode) return null;
			string mdxQuery = QueryBuilder.GetCalculatedMembersQueryString(CubeName, columns);
			using(OleDbCommand command = new OleDbCommand(mdxQuery, OLAPConnection)) {
				command.CommandTimeout = QueryTimeout;
				bool[] res = new bool[columns.Count];
				try {
					using(OleDbDataReader reader = ExecuteReader(command)) {						
						if(!reader.Read())
							return null;
						for(int i = 0; i < res.Length; i++) {
							res[i] = (int)reader.GetValue(i) > 0;
						}
						return res;
					}
				} catch { return null; }
			}
		}
		public void QueryLevelReverse(bool isColumn, int level) {
			if(!Connected || IsDesignMode || GetArea(isColumn).Count <= level) return;
			string[] members = GetFieldValues(isColumn).GetUniqueMembers(GetArea(isColumn)[level]);
			if(members.Length == 0) return;
			Array.Reverse(members);
			GetFieldValues(isColumn).Sort(level, members);
		}
		public string[] QuerySortMembers(OLAPCubeColumn column, string[] members) {
			if(!Connected || IsDesignMode) return new string[0];
			if(members.Length == 0) return new string[0];
			string mdxQuery = QueryBuilder.GetSortQueryString(CubeName, members, column);
			using(OleDbCommand command = new OleDbCommand(mdxQuery, OLAPConnection)) {
				command.CommandTimeout = QueryTimeout;
				using(OleDbDataReader reader = ExecuteReader(command)) {
					int columnOrdinal = reader.GetOrdinal(OLAPDataSourceQueryBase.UniqueNameMeasureString);
					List<string> sortedMembers = new List<string>(members.Length);
					while(reader.Read())
						sortedMembers.Add((string)reader.GetValue(columnOrdinal));
					if(sortedMembers.Count != members.Length) throw new Exception("OLAP server returned less or more items than have been sent.");
					return sortedMembers.ToArray();
				}
			}
		}
		public void QueryTopValuesSortLevel(bool isColumn, int level) {
			QueryData(isColumn ? new OLAPGroupInfo[0] : ColumnValues.ToArray(), isColumn ? RowValues.ToArray() : new OLAPGroupInfo[0], false, false);
		}
		public string[] QueryFilterIntersect(OLAPCubeColumn childColumn, OLAPCubeColumn parentColumn) {
			if(!Connected || IsDesignMode) return new string[0];
			string mdxQuery = QueryBuilder.GetIntersectFilterValuesQuery(CubeName, childColumn, parentColumn);
			using(OleDbCommand command = new OleDbCommand(mdxQuery, OLAPConnection)) {
				try {
					using(OleDbDataReader reader = ExecuteReader(command)) {
						int columnOrdinal = reader.GetOrdinal(parentColumn.UniqueName + ".[MEMBER_UNIQUE_NAME]");
						List<string> result = new List<string>();
						while(reader.Read()) {
							string member = (string)reader.GetValue(columnOrdinal);
							if(!result.Contains(member))
								result.Add(member);
						}
						return result.ToArray();
					}
				} catch { return new string[0]; }
			}
		}
		public PivotOLAPKPIValue QueryKPIValue(string kpiName) {
			if(!Connected || IsDesignMode) return null;
			PivotOLAPKPIMeasures measures = Owner.GetKPIMeasures(kpiName);
			if(measures == null) return null;
			string mdxQuery = QueryBuilder.GetKPIValueQuery(kpiName, CubeName);
			using(OleDbCommand command = new OleDbCommand(mdxQuery, OLAPConnection)) {
				try {
					using(OleDbDataReader reader = ExecuteReader(command)) {
						reader.Read();						
						object value = null, goal = null;
						int status = 0, trend = 0;
						double weight = double.NaN;
						for(int i = 0; i < reader.FieldCount; i++) {
							if(reader.GetName(i) == measures.ValueMeasure)
								value = reader.GetValue(i);
							if(reader.GetName(i) == measures.GoalMeasure)
								goal = reader.GetValue(i);
							if(reader.GetName(i) == measures.StatusMeasure)
								status = Convert.ToInt32(reader.GetValue(i));
							if(reader.GetName(i) == measures.TrendMeasure)
								trend = Convert.ToInt32(reader.GetValue(i));
							if(reader.GetName(i) == measures.WeightMeasure)
								weight = Convert.ToDouble(reader.GetValue(i));
						}
						return new PivotOLAPKPIValue(value, goal, status, trend, weight);
					}
				} catch { return null; }
			}
		}
		protected List<OLAPCubeColumn> GetArea(PivotArea area) {
			switch(area) {
				case PivotArea.ColumnArea:
					return ColumnArea;
				case PivotArea.DataArea:
					return DataArea;
				case PivotArea.FilterArea:
					return FilterArea;
				case PivotArea.RowArea:
					return RowArea;
				default:
					return null;
			}
		}
		protected List<OLAPCubeColumn> GetArea(bool isColumn) { return isColumn ? ColumnArea : RowArea; }
		protected OLAPAreaFieldValues GetFieldValues(bool isColumn) { return isColumn ? ColumnValues : RowValues; }
		class DrillDownFilter {
			readonly List<OLAPCubeColumn[]> returnColumns;
			readonly Dictionary<OLAPCubeColumn, Dictionary<object, object>> filterValuesCache;
			public DrillDownFilter(List<OLAPCubeColumn[]> returnColumns) {
				this.returnColumns = returnColumns;
				this.filterValuesCache = new Dictionary<OLAPCubeColumn, Dictionary<object, object>>();
			}
			public bool RequiresFilter {
				get {
					for(int i = 0; i < returnColumns.Count; i++) {
						if(returnColumns[i] != null)
							return true;
					}
					return false;
				}
			}
			public OLAPPivotDrillDownDataSource.IsRowFitDelegate GetFilter() {
				if(!RequiresFilter)
					return null;
				else
					return IsRowFit;
			}			
			Dictionary<object, object> CreateFilterCache(OLAPCubeColumn column) {
				Dictionary<object, object> res = new Dictionary<object, object>();
				for(int i = 0; i < column.FilteredValues.Count; i++) {
					OLAPMember member = column[column.FilteredValues[i]];
					if(member != null)
						res.Add(member.Value, null);
				}
				return res;
			}
			bool IsRowFit(object[] row) {
				if(row.Length != returnColumns.Count)
					throw new ArgumentException("Invalid row");
				for(int i = 0; i < row.Length; i++) {
					if(!IsValueFit(i, row[i]))
						return false;
				}
				return true;
			}
			bool IsValueFit(int columnIndex, object value) {
				OLAPCubeColumn[] columns = returnColumns[columnIndex];
				if(columns == null) return true;
				for(int i = 0; i < columns.Length; i++) {
					if(!IsValueFitCore(columns[i], value))
						return false;
				}
				return true;
			}
			bool IsValueFitCore(OLAPCubeColumn column, object value) {
				Dictionary<object, object> cache;
				if(!filterValuesCache.TryGetValue(column, out cache)) {
					cache = CreateFilterCache(column);
					filterValuesCache.Add(column, cache);
				}
				return cache.ContainsKey(value);
			}
		}
	}
	class OLAPDataSourceQueryExecutor2005 : OLAPDataSourceQueryExecutorBase {
		public OLAPDataSourceQueryExecutor2005(IOLAPDataSourceQueryExecutorOwner owner) : base(owner) { }
		protected override OLAPDataSourceQueryBase CreateQuery() { return new OLAPDataSourceQuery2005(); }
	}
	class OLAPDataSourceQueryExecutor2000 : OLAPDataSourceQueryExecutorBase {
		public OLAPDataSourceQueryExecutor2000(IOLAPDataSourceQueryExecutorOwner owner) : base(owner) { }
		protected override OLAPDataSourceQueryBase CreateQuery() { return new OLAPDataSourceQuery2000(); }
	}
	internal enum OLAPLevelType {
		Regular = 0x0, All = 0x1, Calculated = 0x2, Time = 0x4, Reserved1 = 0x8, TimeYears = 0x14, TimeHalfYear = 0x24, TimeQuarters = 0x44,
		TimeMonths = 0x84, TimeWeeks = 0x104, TimeDays = 0x204, TimeHours = 0x304, TimeMinutes = 0x404, TimeSeconds = 0x804, TimeUndefined = 0x1004,
		GeoContinent = 0x2001, GeoRegion = 0x2002, GeoCountry = 0x2003, GeoStateOrProvince = 0x2004, GeoCounty = 0x2005, GeoCity = 0x2006,
		GeoPostalCode = 0x2007, GeoPoint = 0x2008, OrgUnit = 0x1011, BomResource = 0x1012, Quantitative = 0x1013, Account = 0x1014, Customer = 0x1021,
		CustomerGroup = 0x1022, CustomerHousehold = 0x1023, Product = 0x1031, ProductGroup = 0x1032, Scenario = 0x1015, Utility = 0x1016,
		Person = 0x1041, Company = 0x1042, CurrencySource = 0x1051, CurrencyDestination = 0x1052, Channel = 0x1061, Representative = 0x1062,
		Promotion = 0x1071
	};
	internal enum OLAPMemberType {
		Unknown = 0, Regular = 1, All = 2, Measure = 3, Formula = 4
	}
	public class OLAPConnectionException : Exception {
		public const string DefaultMessage = "Couldn't connect to the Analysis Services.";
		public OLAPConnectionException() : base() { }
		public OLAPConnectionException(string message) : base(message) { }
		public OLAPConnectionException(string message, Exception innerException) : base(message, innerException) { }
	}
	public class PivotGridOLAPDataSource : IPivotGridDataSource, IOLAPDataSourceQueryExecutorOwner {
		readonly PivotGridData data;
		readonly OLAPConnectionStringBuilder connectionStringBuilder;
		OleDbConnection olapConnection;
		Nullable<bool> isAS2000;
		internal readonly OLAPCubeColumns cubeColumns;
		readonly OLAPHierarchies hierarchies;
		readonly List<OLAPCubeColumn> columnArea, rowArea, dataArea, filterArea;
		readonly OLAPCellTable cells;
		readonly OLAPAreaFieldValues columnValues, rowValues;
		readonly List<PivotOLAPKPIMeasures> kpis;
		OLAPDataSourceQueryExecutorBase queryExecutor;
		ValueComparer valueComparer;
		OLAPConnectionStringBuilder ConnectionStringBuilder { get { return connectionStringBuilder; } }
		public string CubeName {
			get { return ConnectionStringBuilder.CubeName; }
			set {
				if(CubeName != value) {
					ConnectionStringBuilder.CubeName = value;
					Connect(false);
				}
			}
		}
		string CatalogName { get { return ConnectionStringBuilder.CatalogName; } }
		string ConnectionString {
			get { return ConnectionStringBuilder.ConnectionString; }
			set {
				if(ConnectionString != value) {
					ConnectionStringBuilder.ConnectionString = value;
					ConnectionStringChanged();
				}
			}
		}
		public string FullConnectionString {
			get { return ConnectionStringBuilder.FullConnectionString; }
			set {
				if(FullConnectionString != value) {
					ConnectionStringBuilder.FullConnectionString = value;
					ConnectionStringChanged();
				}
			}
		}
		internal protected PivotGridData Data { get { return data; } }
		internal List<OLAPCubeColumn> ColumnArea { get { return columnArea; } }
		internal List<OLAPCubeColumn> RowArea { get { return rowArea; } }
		internal List<OLAPCubeColumn> DataArea { get { return dataArea; } }
		internal List<OLAPCubeColumn> FilterArea { get { return filterArea; } }
		internal bool IsDesignMode { get { return Data != null ? Data.IsDesignMode : false; } }
		internal OLAPCellTable Cells { get { return cells; } }
		internal OLAPAreaFieldValues ColumnValues { get { return columnValues; } }
		internal OLAPAreaFieldValues RowValues { get { return rowValues; } }
		internal List<PivotOLAPKPIMeasures> KPIs { get { return kpis; } }
		bool Connected { get { return olapConnection != null; } }
		ValueComparer ValueComparer {
			get {
				if(valueComparer == null) valueComparer = new ValueComparer();
				return valueComparer;
			}
		}
		#region IOLAPDataSourceQueryExecutorOwner Members
		List<OLAPCubeColumn> IOLAPDataSourceQueryExecutorOwner.ColumnArea { get { return ColumnArea; } }
		List<OLAPCubeColumn> IOLAPDataSourceQueryExecutorOwner.RowArea { get { return RowArea; } }
		List<OLAPCubeColumn> IOLAPDataSourceQueryExecutorOwner.DataArea { get { return DataArea; } }
		List<OLAPCubeColumn> IOLAPDataSourceQueryExecutorOwner.FilterArea { get { return FilterArea; } }
		bool IOLAPDataSourceQueryExecutorOwner.IsDesignMode { get { return IsDesignMode; } }
		OLAPCellTable IOLAPDataSourceQueryExecutorOwner.Cells { get { return Cells; } }
		OLAPAreaFieldValues IOLAPDataSourceQueryExecutorOwner.ColumnValues { get { return ColumnValues; } }
		OLAPAreaFieldValues IOLAPDataSourceQueryExecutorOwner.RowValues { get { return RowValues; } }
		OleDbConnection IOLAPDataSourceQueryExecutorOwner.OLAPConnection { get { return olapConnection; } }
		int IOLAPDataSourceQueryExecutorOwner.QueryTimeout { get { return ConnectionStringBuilder.QueryTimeout; } }
		string IOLAPDataSourceQueryExecutorOwner.CubeName { get { return CubeName; } }
		OLAPCubeColumns IOLAPDataSourceQueryExecutorOwner.CubeColumns { get { return cubeColumns; } }
		#endregion
		OLAPDataSourceQueryExecutorBase QueryExecutor { get { return queryExecutor; } }
		public PivotGridOLAPDataSource(PivotGridData pivotGridData) {
			this.data = pivotGridData;
			this.connectionStringBuilder = new OLAPConnectionStringBuilder();
			hierarchies = new OLAPHierarchies();
			cubeColumns = new OLAPCubeColumns();
			columnArea = new List<OLAPCubeColumn>();
			rowArea = new List<OLAPCubeColumn>();
			dataArea = new List<OLAPCubeColumn>();
			filterArea = new List<OLAPCubeColumn>();
			cells = new OLAPCellTable();
			columnValues = new OLAPAreaFieldValues();
			rowValues = new OLAPAreaFieldValues();
			kpis = new List<PivotOLAPKPIMeasures>();
		}
		void ConnectionStringChanged() {
			ClearState();
			Connect(false);
		}
		void ClearState() {
			cubeColumns.Clear();
			KPIs.Clear();
			ClearFields();
			ClearGroupsAndCells();
			Data.LayoutChanged();
		}
		internal void ClearFields() {
			ColumnArea.Clear();
			RowArea.Clear();
			DataArea.Clear();
			FilterArea.Clear();
			ColumnValues.Clear();
			RowValues.Clear();
		}
		internal void ClearGroupsAndCells() {
			Cells.Clear();
			ColumnValues.Clear();
			RowValues.Clear();
		}
		void Connect(bool forced) {
			if(!IsDesignMode || forced) ConnectCore();
		}
		protected string ServerVersion { get { return olapConnection.ServerVersion; } }
		protected bool IsAS2000 { 
			get {
				if(!isAS2000.HasValue)
					isAS2000 = ServerVersion.StartsWith("8.");
				return isAS2000.Value; 
			} 
		}
		protected virtual void ConnectCore() {
			if(Connected) Disconnect();
			if(String.IsNullOrEmpty(ConnectionString)) return;
			try {
				olapConnection = new OleDbConnection(ConnectionString);
				olapConnection.Open();
				queryExecutor = CreateQueryExecutor(olapConnection.ServerVersion);
			} catch(Exception exception) {
				Disconnect();
				throw new OLAPConnectionException(OLAPConnectionException.DefaultMessage, exception);
			}
		}
		OLAPDataSourceQueryExecutorBase CreateQueryExecutor(string serverVersion) {
			if(IsAS2000) return new OLAPDataSourceQueryExecutor2000(this);
			return new OLAPDataSourceQueryExecutor2005(this);
		}
		internal virtual void Disconnect() {
			if(!Connected)
				return;
			queryExecutor = null;
			olapConnection.Dispose();
			olapConnection = null;
			isAS2000 = null;
		}
		#region schema
		DataTable GetSchema(Guid guid, object[] restrictions) {
			if(!Connected) return null;
			return olapConnection.GetOleDbSchemaTable(guid, restrictions);
		}
		DataTable GetDimensions() {
			return GetSchema(OLAPSchemaGuid.Dimensions, new object[] { CatalogName, null, CubeName });
		}
		DataTable GetHierarchies() {
			return GetSchema(OLAPSchemaGuid.Hierarchies, new object[] { CatalogName, null, CubeName });
		}
		DataTable GetMeasures() {
			return GetSchema(OLAPSchemaGuid.Measures, new object[] { CatalogName, null, CubeName });
		}
		DataTable GetMeasure(string uniqueName) {
			return GetSchema(OLAPSchemaGuid.Measures, new object[] { CatalogName, null, CubeName, null, uniqueName });
		}
		DataTable GetKPIs() {
			if(IsAS2000) return null;	
			return GetSchema(OLAPSchemaGuid.KPIs, new object[] { CatalogName, null, CubeName });
		}
		DataTable GetLevels(string hierarchyUniqueName) {
			return GetSchema(OLAPSchemaGuid.Levels, new object[] { CatalogName, null, CubeName, null, hierarchyUniqueName });
		}
		DataTable GetMembers(string hierarchyUniqueName, string levelUniqueName, OLAPMemberType memberType) {
			return GetSchema(OLAPSchemaGuid.Members, new object[] { CatalogName, null, CubeName, null, hierarchyUniqueName, levelUniqueName, null,
				null, null, null, (int)memberType });
		}
		string GetAllMember(string hierarchyUniqueName) {
			DataTable membersTable = GetMembers(hierarchyUniqueName, null, OLAPMemberType.All);
			if(membersTable == null || membersTable.Rows.Count == 0) return null;
			return (string)membersTable.Rows[0]["MEMBER_UNIQUE_NAME"];
		}
		DataTable GetProperties(string levelUniqueName) {
			return GetSchema(OLAPSchemaGuid.Properties, new object[] { CatalogName, null, CubeName, null, null, levelUniqueName });
		}
		public bool PopulateColumns() {
			bool wasConnected = Connected;
			if(!Connected) Connect(true);
			if(!Connected) return false;
			cubeColumns.Clear();
			hierarchies.Clear();
			DataTable hierarchyTable = GetHierarchies(),
				measureTable = GetMeasures(),
				dimensionTable = GetDimensions(),
				kpisTable = GetKPIs();
			if(hierarchyTable != null) {
				for(int i = 0; i < hierarchyTable.Rows.Count; i++) {
					DataRow row = hierarchyTable.Rows[i];
					if((string)row["HIERARCHY_UNIQUE_NAME"] == "[Measures]") continue;
					OLAPHierarchy hierarchy = new OLAPHierarchy("HIERARCHY", row);
					hierarchies.Add(hierarchy);
					PopulateHierarchyLevels(hierarchy);
				}
			}
			OLAPHierarchy measures = new OLAPHierarchy("[Measures]", "Measures", PivotGridLocalizer.GetString(PivotGridStringId.OLAPMeasuresCaption));
			hierarchies.Add(measures);
			if(measureTable != null) {
				for(int i = 0; i < measureTable.Rows.Count; i++) {
					DataRow row = measureTable.Rows[i];
					int dataType = GetMeasureDataType(row);
					string drilldownColumn = GetMeasureDrillDownColumn(row);
					OLAPCubeColumn column = new OLAPCubeColumn("MEASURE", row, measures, 0,
						(OleDbType)dataType, null, null, drilldownColumn);
					cubeColumns.Add(column);
				}
			}
			if(kpisTable != null) {
				foreach(DataRow row in kpisTable.Rows) {
					string name = (string)row["KPI_NAME"],
						caption = (string)row["KPI_CAPTION"];
					PopulateKPIMeasure((string)row["KPI_VALUE"], name, caption + " Value", measures, null, PivotKPIType.Value);
					PopulateKPIMeasure((string)row["KPI_GOAL"], name, caption + " Goal", measures, null, PivotKPIType.Goal);
					PopulateKPIMeasure((string)row["KPI_STATUS"], name, caption + " Status", measures, (string)row["KPI_STATUS_GRAPHIC"], PivotKPIType.Status);
					PopulateKPIMeasure((string)row["KPI_TREND"], name, caption + " Trend", measures, (string)row["KPI_TREND_GRAPHIC"], PivotKPIType.Trend);
					PopulateKPIMeasure((string)row["KPI_WEIGHT"], name, caption + " Weight", measures, null, PivotKPIType.Weight);
					KPIs.Add(new PivotOLAPKPIMeasures(name, (string)row["KPI_VALUE"], (string)row["KPI_GOAL"], (string)row["KPI_STATUS"],
						(string)row["KPI_TREND"], (string)row["KPI_WEIGHT"]));
				}
			}
			if(dimensionTable != null) {
				for(int i = 0; i < dimensionTable.Rows.Count; i++)
					hierarchies.Add(new OLAPHierarchy("DIMENSION", dimensionTable.Rows[i]));
			}
			if(!wasConnected) Disconnect();
			return true;
		}
		int GetMeasureDataType(DataRow row) {
			return (row["DATA_TYPE"].GetType() == typeof(System.Int32)) ? (int)row["DATA_TYPE"] : int.Parse((string)row["DATA_TYPE"]);
		}
		string GetMeasureDrillDownColumn(DataRow row) {
			if(IsAS2000) return null;	 
			string measureGroup = row["MEASUREGROUP_NAME"].ToString(),
				columnName = row["MEASURE_NAME_SQL_COLUMN_NAME"].ToString();
			if(string.IsNullOrEmpty(measureGroup) || string.IsNullOrEmpty(columnName))
				return null;
			return String.Format("[{0}].[{1}]", measureGroup, columnName);
		}
		void PopulateKPIMeasure(string uniqueName, string name, string caption, OLAPHierarchy measures, string graphic, PivotKPIType type) {
			if(cubeColumns[uniqueName] != null || string.IsNullOrEmpty(uniqueName)) return;
			OLAPKPIColumn column = new OLAPKPIColumn(uniqueName, name, caption, measures, graphic, type);
			cubeColumns.Add(column);
		}
		OLAPHierarchy hierachyValue = new OLAPHierarchy();
		OLAPCubeColumn columnValue = new OLAPCubeColumn();
		internal string GetHierarchyCaption(string hierarchyName) {
			if(!string.IsNullOrEmpty(hierarchyName)) {
				if(cubeColumns.TryGetValue(hierarchyName, out columnValue))
					return columnValue.Caption;
				if(hierarchies.TryGetValue(hierarchyName, out hierachyValue))
					return hierachyValue.Caption;
			}
			return null;
		}
		bool FieldExists(PivotGridFieldBase field) {
			return cubeColumns.ContainsKey(field.FieldName);
		}
		void PopulateHierarchyLevels(OLAPHierarchy hierarchy) {
			DataTable levels = GetLevels(hierarchy.UniqueName);
			OLAPCubeColumn parent = null;
			for(int i = 0; i < levels.Rows.Count; i++) {
				DataRow row = levels.Rows[i];
				if((int)row["LEVEL_TYPE"] == (int)OLAPLevelType.All) continue;
				parent = new OLAPCubeColumn("LEVEL", row, hierarchy, i, GetLevelDataType(row),
					parent, null, GetLevelDrillDownColumn(row));
				cubeColumns.Add(parent);
			}
		}
		OleDbType GetLevelDataType(DataRow levelRow) {
			string levelUniqueName = (string)levelRow["LEVEL_UNIQUE_NAME"];
			DataTable props = GetProperties(levelUniqueName);
			foreach(DataRow row in props.Rows) {
				if((string)row["PROPERTY_NAME"] != "MEMBER_VALUE") continue;
				return (OleDbType)row["DATA_TYPE"];
			}
			return OleDbType.WChar;
		}
		string GetLevelDrillDownColumn(DataRow row) {
			if(IsAS2000) return null;
			string attribute = (string)row["LEVEL_ATTRIBUTE_HIERARCHY_NAME"],
				dimension = ((string)row["DIMENSION_UNIQUE_NAME"]).Insert(1, "$");
			return string.Format("{0}.[{1}]", dimension, attribute);
		}
		List<OLAPCubeColumn> GetArea(PivotArea area) {
			switch(area) {
				case PivotArea.ColumnArea:
					return ColumnArea;
				case PivotArea.DataArea:
					return DataArea;
				case PivotArea.FilterArea:
					return FilterArea;
				case PivotArea.RowArea:
					return RowArea;
				default:
					return null;
			}
		}
		internal List<OLAPCubeColumn> GetArea(bool isColumn) {
			return isColumn ? ColumnArea : RowArea;
		}
		internal OLAPAreaFieldValues GetFieldValues(bool isColumn) {
			return isColumn ? ColumnValues : RowValues;
		}
		#endregion
		internal void SpreadFields(PivotGridFieldReadOnlyCollection sortedFields) {
			ClearFields();
			List<OLAPCubeColumn> columnRowFields = new List<OLAPCubeColumn>();
			for(int i = 0; i < sortedFields.Count; i++) {
				PivotGridFieldBase field = sortedFields[i];
				if(field.UnboundType != UnboundColumnType.Bound ||
					!FieldExists(field) || (!field.Visible && field.Area != PivotArea.FilterArea)) continue;
				OLAPCubeColumn column = cubeColumns[field.FieldName];				
				GetArea(field.Area).Add(column);
				if(field.IsColumnOrRow)
					columnRowFields.Add(column);
			}
			EnsureHasCalculatedMembers(columnRowFields);
		}
		void SetColumnsSortAndTop(PivotGridFieldReadOnlyCollection sortedFields) {
			for(int i = 0; i < sortedFields.Count; i++) {
				PivotGridFieldBase field = sortedFields[i];
				OLAPCubeColumn column = cubeColumns[field.FieldName];
				if(column == null) continue;
				SetColumnSortAndTop(field, column);
			}
			PrepareHierarchiesFilteredValues(FilterArea, true);
		}
		void PrepareHierarchiesFilteredValues(List<OLAPCubeColumn> area, bool skipUnfilteredValues) {
			Dictionary<string, object>[] areaIntersectedMembers = new Dictionary<string, object>[area.Count];
			for(int i = area.Count - 1; i >= 0; i--) {
				if(!area[i].Filtered) 
					continue;
				for(int j = area.Count - 1; j >= 0; j--) {
					if(!area[j].IsChildOrParent(area[i]) || (skipUnfilteredValues && !area[j].Filtered))
						continue;
					string[] intersectedMembers = QueryFilterIntersect(area[i], area[j]);
					if(areaIntersectedMembers[j] == null) {
						areaIntersectedMembers[j] = new Dictionary<string, object>();
						foreach(string member in intersectedMembers)
							areaIntersectedMembers[j].Add(member, null);
					} else {
						Dictionary<string, object> newIntersection = new Dictionary<string, object>();
						foreach(string member in intersectedMembers) {
							if(areaIntersectedMembers[j].ContainsKey(member))
								newIntersection.Add(member, null);
						}
						areaIntersectedMembers[j] = newIntersection;
					}
				}
			}
			ApplyIntersectAreaMembers(area, areaIntersectedMembers);
		}
		void ApplyIntersectAreaMembers(List<OLAPCubeColumn> area, Dictionary<string, object>[] intersectedMembersCache) {
			for(int i = 0; i < area.Count; i++) {
				if(intersectedMembersCache[i] == null) continue;
				area[i].IntersectFilteredValues(intersectedMembersCache[i]);
			}
		}
		string[] QueryFilterIntersect(OLAPCubeColumn childColumn, OLAPCubeColumn parentColumn) {
			if(QueryExecutor == null) return new string[0];
			return QueryExecutor.QueryFilterIntersect(childColumn, parentColumn);
		}
		internal virtual bool QueryData(OLAPGroupInfo[] columns, OLAPGroupInfo[] rows, bool columnExpand, bool rowExpand) {
			if(QueryExecutor == null) return false;
			if(!QueryExecutor.QueryData(columns, rows, columnExpand, rowExpand)) return false;
			Data.LayoutChanged();
			return true;
		}
		internal virtual void QueryMembers(OLAPCubeColumn column, string[] uniqueNames) {
			if(QueryExecutor == null) return;
			QueryExecutor.QueryMembers(column, uniqueNames);
		}
		bool QueryNullValues(OLAPCubeColumn column) {
			if(QueryExecutor == null) return false;
			return QueryExecutor.QueryNullValues(column);
		}
		bool[] QueryCalculatedMembers(List<OLAPCubeColumn> columns) {
			if(QueryExecutor == null) return null;
			return QueryExecutor.QueryCalculatedMembers(columns);
		}
		void QuerySortLevel(bool isColumn, int level) {
			if(QueryExecutor == null) return;
			QueryExecutor.QueryLevelReverse(isColumn, level);
		}
		void QueryTopValuesSortLevel(bool isColumn, int level) {
			if(QueryExecutor == null) return;
			List<List<object[]>> columnState = SaveCollapsedStateCore(true),
				rowState = SaveCollapsedStateCore(false);
			QueryExecutor.QueryTopValuesSortLevel(isColumn, level);
			LoadCollapsedStateCore(true, columnState);
			LoadCollapsedStateCore(false, rowState);
		}
		OLAPPivotDrillDownDataSource QueryDrilldown(int columnIndex, int rowIndex, int dataIndex, 
				int maxRowCount, List<string> customColumns) {
			if(QueryExecutor == null) return null;
			OLAPMember[] columnMembers = columnIndex >= 0 ? ColumnValues.GetHierarchyMembers(columnIndex) : new OLAPMember[0],
				rowMembers = rowIndex >= 0 ? RowValues.GetHierarchyMembers(rowIndex) : new OLAPMember[0];
			return QueryExecutor.QueryDrillDown(columnMembers, rowMembers, DataArea[dataIndex], 
							maxRowCount, customColumns);
		}
		PivotOLAPKPIValue QueryKPIValue(string kpiName) {
			if(QueryExecutor == null) return null;
			return QueryExecutor.QueryKPIValue(kpiName);
		}
		static bool IsMeasure(PivotGridFieldBase field) {
			return IsMeasure(field.FieldName);
		}
		static bool IsMeasure(string fieldName) {
			return fieldName.StartsWith("[Measures]");
		}
		bool IsObjectCollapsedCore(bool isColumn, int visibleIndex) {
			OLAPAreaFieldValues values = GetFieldValues(isColumn);
			return values[visibleIndex].Level < GetArea(isColumn).Count - 1 &&
						 (visibleIndex == values.Count - 1 || values[visibleIndex + 1].Level <= values[visibleIndex].Level);
		}
		void CollapseCore(bool isColumn, int visibleIndex) {
			GetFieldValues(isColumn).RemoveChilds(visibleIndex);
		}
		void ChangeFieldExpandedCore(bool expanded, bool isColumn, OLAPAreaFieldValues fieldValues, int[] indexes) {
			int[] filteredIndexes = Array.FindAll<int>(indexes, delegate(int target) {
				return expanded ? IsObjectCollapsedCore(isColumn, target) : !IsObjectCollapsedCore(isColumn, target);
			});
			if(filteredIndexes.Length == 0) return;
			if(expanded) {
				const int maxGroupCount = 100;
				OLAPGroupInfo[] groups = new OLAPGroupInfo[filteredIndexes.Length];
				for(int i = 0; i < filteredIndexes.Length; i++)
					groups[i] = fieldValues[filteredIndexes[i]];
				for(int i = 0; i < groups.Length; i += maxGroupCount) {
					int groupCount = Math.Min(maxGroupCount, groups.Length - i);
					OLAPGroupInfo[] groupsPart = new OLAPGroupInfo[groupCount];
					for(int j = 0; j < groupsPart.Length; j++)
						groupsPart[j] = groups[i + j];
					ExpandCore(isColumn, groupsPart);
				}
			} else {
				for(int i = filteredIndexes.Length - 1; i >= 0; i--)
					CollapseCore(isColumn, filteredIndexes[i]);
			}
		}
		bool ExpandCore(bool isColumn, int visibleIndex) {
			return ExpandCore(isColumn, GetFieldValues(isColumn)[visibleIndex]);
		}
		bool ExpandCore(bool isColumn, OLAPGroupInfo group) {
			OLAPGroupInfo[] columns = isColumn ? new OLAPGroupInfo[] { group } : ColumnValues.ToArray(),
							rows = isColumn ? RowValues.ToArray() : new OLAPGroupInfo[] { group };
			return QueryData(columns, rows, isColumn, !isColumn);
		}
		bool ExpandCore(bool isColumn, OLAPGroupInfo[] groups) {
			OLAPGroupInfo[] columns = isColumn ? groups : ColumnValues.ToArray(),
							rows = isColumn ? RowValues.ToArray() : groups;
			return QueryData(columns, rows, isColumn, !isColumn);
		}
		void DoRefreshCore(PivotGridFieldReadOnlyCollection sortedFields) {
			if(cubeColumns.Count == 0 && !IsDesignMode) PopulateColumns();
			Cells.Clear();
			SpreadFields(sortedFields);
			SetColumnsSortAndTop(sortedFields);
			QueryData(new OLAPGroupInfo[0], new OLAPGroupInfo[0], false, false);
			Data.LayoutChanged();
		}
		void SetColumnSortAndTop(PivotGridFieldBase field, OLAPCubeColumn column) {			
			column.Assign(field);
			if(column.AllMember == null)
				column.AllMemberUniqueName = GetAllMember(column.Hierarchy.UniqueName);
			SetColumnSortBySummary(field, column);
			SetColumnFilters(field, column);
		}		
		internal void SetColumnSortBySummary(PivotGridFieldBase field, OLAPCubeColumn column) {
			if(field.SortBySummaryInfo.Field != null && IsMeasure(field.SortBySummaryInfo.Field) && cubeColumns.ContainsKey(field.SortBySummaryInfo.Field.FieldName)) {
				column.SortBySummary = cubeColumns[field.SortBySummaryInfo.Field.FieldName];
			} else {
				if(!string.IsNullOrEmpty(field.SortBySummaryInfo.FieldName) &&
					cubeColumns.ContainsKey(field.SortBySummaryInfo.FieldName) && IsMeasure(field.SortBySummaryInfo.FieldName)) {
					column.SortBySummary = cubeColumns[field.SortBySummaryInfo.FieldName];
				} else
					column.SortBySummary = null;
			}
			SetColumnSortBySummaryConditions(field, column);
		}
		void SetColumnSortBySummaryConditions(PivotGridFieldBase field, OLAPCubeColumn column) {
			column.SortBySummaryMembers.Clear();
			if(field.SortBySummaryInfo.Conditions.Count > 0 && column.SortBySummary != null) {
				List<OLAPCubeColumn> conditionArea = field.IsColumn ? RowArea : ColumnArea;
				OLAPMember[] conditionMembers = new OLAPMember[conditionArea.Count];
				for(int i = 0; i < field.SortBySummaryInfo.Conditions.Count; i++) {
					PivotGridFieldSortCondition condition = field.SortBySummaryInfo.Conditions[i];
					if(condition.Field == null)
						continue;
					OLAPCubeColumn conditionColumn = cubeColumns[condition.Field.FieldName];
					if(!condition.Field.IsColumnOrRow || condition.Field.Area == field.Area || conditionColumn == null)
						continue;
					EnsureColumnMembersLoaded(conditionColumn);
					OLAPMember conditionMember = conditionColumn[condition.OLAPUniqueMemberName];
					int conditionMemberIndex = conditionArea.IndexOf(conditionColumn);
					if(conditionColumn != null && !conditionColumn.IsMeasure && conditionMember != null && conditionMemberIndex >= 0) {
						conditionMembers[conditionMemberIndex] = conditionMember;
					}
				}
				for(int i = 0; i < conditionMembers.Length; i++) {
					if(conditionMembers[i] != null)
						column.SortBySummaryMembers.Add(conditionMembers[i]);
				}
			}
		}
		void SetColumnFilters(PivotGridFieldBase field, OLAPCubeColumn column) {
			if(!field.FilterValues.IsEmpty) {
				if(IsMeasure(field))
					throw new NotSupportedException("Measure filtering is not supported");
				EnsureColumnMembersLoaded(column);
				column.LoadFilteredValues(!field.FilterValues.IsEmpty, field.FilterValues.ValuesIncluded);
			} else {
				column.Filtered = false;
				column.FilteredValues.Clear();
			}
		}
		void EnsureColumnMembersLoaded(OLAPCubeColumn column) {			
			if(!column.AllMembersLoaded) QueryMembers(column, null);
		}
		void SaveCollapsedStateToStreamCore(Stream stream) {
			WriteCollapsedStateToStream(stream, SaveCollapsedStateCore(true));
			WriteCollapsedStateToStream(stream, SaveCollapsedStateCore(false));
		}
		List<List<object[]>> SaveCollapsedStateCore(bool isColumn) {
			int levelCount = GetArea(isColumn).Count;
			OLAPAreaFieldValues fieldValues = GetFieldValues(isColumn);
			List<List<object[]>> result = new List<List<object[]>>(levelCount);
			for(int i = 0; i < levelCount - 1; i++) {
				int[] levelIndexes = Array.FindAll<int>(fieldValues.GetAllIndexes(i), delegate(int target) { return !IsObjectCollapsedCore(isColumn, target); });
				List<object[]> levelState = new List<object[]>(levelIndexes.Length);
				foreach(int index in levelIndexes)
					levelState.Add(fieldValues.GetValues(index));
				result.Add(levelState);
			}
			return result;
		}
		void WriteCollapsedStateToStream(Stream stream, List<List<object[]>> state) {
			TypedBinaryWriter writer = new TypedBinaryWriter(stream);
			writer.Write(state.Count);
			for(int i = 0; i < state.Count; i++) {
				writer.Write(state[i].Count);
				foreach(object[] values in state[i]) {
					foreach(object value in values)
						writer.WriteTypedObject(value);
				}
			}
		}
		void LoadCollapsedStateFromStreamCore(Stream stream) {
			LoadCollapsedStateCore(true, ReadCollapsedStateFromStream(stream));
			LoadCollapsedStateCore(false, ReadCollapsedStateFromStream(stream));
		}
		void LoadCollapsedStateCore(bool isColumn, List<List<object[]>> state) {
			((IPivotGridDataSource)this).ChangeExpandedAll(isColumn, false);
			OLAPAreaFieldValues fieldValues = GetFieldValues(isColumn);
			int levelCount = Math.Min(GetArea(isColumn).Count, state.Count);
			for(int i = 0; i < levelCount; i++) {
				List<int> indexes = new List<int>();
				for(int j = 0; j < state[i].Count; j++) {
					int index = fieldValues.GetIndex(state[i][j]);
					if(index >= 0) indexes.Add(index);
				}
				ChangeFieldExpandedCore(true, isColumn, fieldValues, indexes.ToArray());
			}
		}
		List<List<object[]>> ReadCollapsedStateFromStream(Stream stream) {
			TypedBinaryReader reader = new TypedBinaryReader(stream);
			int levelCount = reader.ReadInt32();
			List<List<object[]>> result = new List<List<object[]>>(levelCount);
			for(int i = 0; i < levelCount; i++) {
				int groupCount = reader.ReadInt32();
				result.Add(new List<object[]>(groupCount));
				for(int j = 0; j < groupCount; j++) {
					object[] values = new object[i + 1];
					for(int k = 0; k < values.Length; k++)
						values[k] = reader.ReadTypedObject();
					result[i].Add(values);
				}
			}
			return result;
		}
		const uint GrandTotalGroupIndex = uint.MaxValue;
		public string SaveStateToString() {
			using(MemoryStream stream = new MemoryStream()) {
				using(TypedBinaryWriter writer = new TypedBinaryWriter(stream)) {
							Dictionary<OLAPMember, object> uniqueMembers = new Dictionary<OLAPMember, object>(ColumnValues.Count + RowValues.Count);
							for(int i = 0; i < ColumnValues.Count; i++)
								if(!ColumnValues[i].IsTotal && !uniqueMembers.ContainsKey(ColumnValues[i].Member))
									uniqueMembers.Add(ColumnValues[i].Member, null);
							for(int i = 0; i < RowValues.Count; i++)
								if(!RowValues[i].IsTotal && !uniqueMembers.ContainsKey(RowValues[i].Member))
									uniqueMembers.Add(RowValues[i].Member, null);
							Dictionary<OLAPCubeColumn, List<OLAPMember>> members = new Dictionary<OLAPCubeColumn, List<OLAPMember>>();
							foreach(OLAPMember member in uniqueMembers.Keys) {
								if(!members.ContainsKey(member.Column))
									members.Add(member.Column, new List<OLAPMember>());
								members[member.Column].Add(member);
							}
							hierarchies.SaveToStream(writer);
							writer.Write(cubeColumns.Count);
							for(int i = 0; i < cubeColumns.Count; i++) {
								writer.Write(cubeColumns[i].TypeCode);
								cubeColumns[i].SaveToStream(writer);
								if(members.ContainsKey(cubeColumns[i])) {
									List<OLAPMember> columnMembers = members[cubeColumns[i]];
									writer.Write(columnMembers.Count);
									foreach(OLAPMember member in columnMembers) {
										writer.Write(member.UniqueName);
										writer.WriteObject(member.Value);
									}
								} else
									writer.Write((int)0);
							}
				}
				return Convert.ToBase64String(stream.GetBuffer());
			}
		}
		public void RestoreStateFromString(string stateString) {
			Debug.Assert(cubeColumns.Count == 0); 
			using(MemoryStream stream = new MemoryStream(Convert.FromBase64String(stateString))) {
				using(TypedBinaryReader reader = new TypedBinaryReader(stream)) {
						hierarchies.RestoreFromStream(reader);
						int columnsCount = reader.ReadInt32();
						for(int i = 0; i < columnsCount; i++) {
							byte typeCode = reader.ReadByte();
							OLAPCubeColumn column = OLAPCubeColumn.CreateFromTypeCode(typeCode);
							column.RestoreFromStream(reader, hierarchies, cubeColumns);							
							cubeColumns.Add(column);
							int membersCount = reader.ReadInt32();
							for(int j = 0; j < membersCount; j++) {
								string memberUniqueName = reader.ReadString();
								object value = reader.ReadObject(column.DataType);
								OLAPMember member = new OLAPMember(column, memberUniqueName, value);
								column.AddMember(member);
							}
						}
				}
			}
		}
		#region IDisposable implementation
		void IDisposable.Dispose() {
			Disconnect();
		}
		#endregion
		#region IPivotGridDataSource Members
		event EventHandler IPivotGridDataSource.ListSourceChanged { add { ; } remove { ; } }
		IList IPivotGridDataSource.ListSource { get { return null; } set { ; } }
		bool IPivotGridDataSource.CaseSensitive { get { return true; } set { ; } }
		bool IPivotGridDataSource.SupportsUnboundColumns { get { return true; } }
		void IPivotGridDataSource.RetrieveFields() {
			if(cubeColumns.Count == 0) PopulateColumns();
			foreach(KeyValuePair<string, OLAPCubeColumn> column in cubeColumns) {
				PivotGridFieldBase field = Data.Fields.Add(column.Key, column.Value.IsMeasure ? PivotArea.DataArea : PivotArea.FilterArea);
			}
		}
		void IPivotGridDataSource.ReloadData() {
			QueryData(new OLAPGroupInfo[0], new OLAPGroupInfo[0], false, false);
		}
		void IPivotGridDataSource.DoRefresh(PivotGridFieldReadOnlyCollection sortedFields) {
			List<List<object[]>> columnState = SaveCollapsedStateCore(true),
				rowState = SaveCollapsedStateCore(false);
			DoRefreshCore(sortedFields);
			LoadCollapsedStateCore(true, columnState);
			LoadCollapsedStateCore(false, rowState);
		}
		void IPivotGridDataSource.BindColumns(PivotGridFieldReadOnlyCollection sortedFields) {
		}
		Type IPivotGridDataSource.GetFieldType(PivotGridFieldBase field, bool raw) {
			if(!cubeColumns.ContainsKey(field.FieldName)) return typeof(object);
			return cubeColumns[field.FieldName].DataType;
		}
		int IPivotGridDataSource.CompareValues(object val1, object val2) {
			return Comparer.DefaultInvariant.Compare(val1, val2);
		}
		bool IPivotGridDataSource.ChangeFieldSortOrder(PivotGridFieldBase field) {
			if(!cubeColumns.ContainsKey(field.FieldName)) return false;
			SetColumnSortAndTop(field, cubeColumns[field.FieldName]);
			if(field.TopValueCount == 0)
				QuerySortLevel(field.IsColumn, field.AreaIndex);
			else
				QueryTopValuesSortLevel(field.IsColumn, field.AreaIndex);
			return true;
		}
		object IPivotGridDataSource.GetListSourceRowValue(int listSourceRow, string fieldName) {
			throw new Exception("The operation is not supported.");	
		}
		PivotDrillDownDataSource IPivotGridDataSource.GetDrillDownDataSource(int columnIndex, int rowIndex, int dataIndex, int maxRowCount) {
			return GetDrillDownDataSource(columnIndex, rowIndex, dataIndex, maxRowCount, null);
		}
		PivotDrillDownDataSource IPivotGridDataSource.GetDrillDownDataSource(GroupRowInfo groupRow, VisibleListSourceRowCollection visibleListSourceRows) {
			throw new Exception("The operation is not supported."); 
		}
		object IPivotGridDataSource.GetCellValue(int columnIndex, int rowIndex, int dataIndex, PivotSummaryType summaryType) {
			if(columnIndex < -1 || columnIndex >= ColumnValues.Count ||
				rowIndex < -1 || rowIndex >= RowValues.Count ||
				dataIndex < 0 || dataIndex >= DataArea.Count) return null;
			if(summaryType == PivotSummaryType.Custom) throw new Exception("The PivotGrid doesn't support custom summaries in OLAP mode.");
			if(DataArea[dataIndex].SummaryType != summaryType) {
				List<int> columnCells = GetUnderlyingCells(ColumnValues, columnIndex),
					rowCells = GetUnderlyingCells(RowValues, rowIndex);
				PivotSummaryValue summaryValue = new PivotSummaryValue(ValueComparer);
				for(int i = 0; i < columnCells.Count; i++)
					for(int j = 0; j < rowCells.Count; j++) {
						object value = Cells[ColumnValues[columnCells[i]]][RowValues[rowCells[j]]][DataArea[dataIndex]];
						if(value != null) summaryValue.AddValue(value, Convert.ToDecimal(value));
					}
				return summaryValue.GetValue(summaryType);
			}
			return Cells[ColumnValues[columnIndex]][RowValues[rowIndex]][DataArea[dataIndex]];
		}
		List<int> GetUnderlyingCells(OLAPAreaFieldValues fieldValues, int index) {
			List<int> result = new List<int>();
			if(index < fieldValues.Count - 1 && fieldValues[index + 1].Level > fieldValues[index].Level) {
				for(int i = index + 1; i < fieldValues.Count && fieldValues[i].Level > fieldValues[index].Level; i++)
					if(fieldValues[i].Level == fieldValues[index].Level + 1) result.Add(i);
			} else result.Add(index);
			return result;
		}
		PivotSummaryValue IPivotGridDataSource.GetCellSummaryValue(int columnIndex, int rowIndex, int dataIndex) {
			return null;
		}
		int IPivotGridDataSource.GetVisibleIndexByValues(bool isColumn, object[] values) {
			return GetFieldValues(isColumn).GetIndex(values);
		}
		int IPivotGridDataSource.GetNextOrPrevVisibleIndex(bool isColumn, int visibleIndex, bool isNext) {
			return GetFieldValues(isColumn).GetNextOrPrevIndex(visibleIndex, isNext);
		}
		bool IPivotGridDataSource.IsObjectCollapsed(bool isColumn, int visibleIndex) {
			return IsObjectCollapsedCore(isColumn, visibleIndex);
		}
		bool IPivotGridDataSource.IsObjectCollapsed(bool isColumn, object[] values) {
			int visibleIndex = GetFieldValues(isColumn).GetIndex(values);
			if(visibleIndex < 0) return false;
			return IsObjectCollapsedCore(isColumn, visibleIndex);
		}
		bool IPivotGridDataSource.ChangeExpanded(bool isColumn, int visibleIndex, bool expanded) {
			if(visibleIndex < 0 || visibleIndex >= GetFieldValues(isColumn).Count) return false;
			bool isCollapsed = IsObjectCollapsedCore(isColumn, visibleIndex);
			if(expanded && isCollapsed) {
				return ExpandCore(isColumn, visibleIndex);
			}
			if(!expanded && !isCollapsed) {
				CollapseCore(isColumn, visibleIndex);
				Data.LayoutChanged();
			}
			return true;
		}
		void IPivotGridDataSource.ChangeExpandedAll(bool isColumn, bool expanded) {
			List<OLAPCubeColumn> area = GetArea(isColumn);
			OLAPAreaFieldValues fieldValues = GetFieldValues(isColumn);
			if(expanded) {
				for(int i = 0; i < area.Count - 1; i++)
					ChangeFieldExpandedCore(expanded, isColumn, fieldValues, fieldValues.GetAllIndexes(i));
			} else {
				ChangeFieldExpandedCore(expanded, isColumn, fieldValues, fieldValues.GetAllIndexes(0));
			}
		}
		void IPivotGridDataSource.ChangeFieldExpanded(PivotGridFieldBase field, bool expanded) {
			bool isColumn = field.Area == PivotArea.ColumnArea;
			OLAPAreaFieldValues fieldValues = GetFieldValues(isColumn);
			int[] indexes = fieldValues.GetAllIndexes(field.AreaIndex);
			ChangeFieldExpandedCore(expanded, isColumn, fieldValues, indexes);
		}
		void IPivotGridDataSource.ChangeFieldExpanded(PivotGridFieldBase field, bool expanded, object value) {
			bool isColumn = field.Area == PivotArea.ColumnArea;
			OLAPAreaFieldValues fieldValues = GetFieldValues(isColumn);
			int[] indexes = fieldValues.GetAllIndexes(field.AreaIndex, value);
			ChangeFieldExpandedCore(expanded, isColumn, fieldValues, indexes);
		}
		object IPivotGridDataSource.GetFieldValue(bool isColumn, int visibleIndex, int areaIndex) {
			OLAPAreaFieldValues values = GetFieldValues(isColumn);
			if(visibleIndex < -1 || visibleIndex >= values.Count) return null;
			OLAPGroupInfo group = values[visibleIndex, areaIndex];
			return group == null ? null : group.Member.Value;
		}
		object[] IPivotGridDataSource.GetUniqueFieldValues(PivotGridFieldBase field) {
			if(cubeColumns.ContainsKey(field.FieldName) && !IsMeasure(field)) {
				OLAPCubeColumn column = cubeColumns[field.FieldName];
				if(!column.AllMembersLoaded)
					QueryMembers(column, null);
				List<string> members = column.GetMembersNames();
				NullableHashtable uniqueValues = new NullableHashtable(members.Count);
				for(int i = 0; i < members.Count; i++) {
					OLAPMember member = column[members[i]];
					if(uniqueValues.ContainsKey(member.Value)) 
						continue;
					uniqueValues.Add(member.Value, null);
				}
				object[] result = new object[uniqueValues.Count];
				uniqueValues.CopyKeysTo(result, 0);
				Array.Sort(result);
				return result;
			}
			return new object[0];
		}
		bool IPivotGridDataSource.HasNullValues(PivotGridFieldBase field) {
			if(!cubeColumns.ContainsKey(field.FieldName) || IsMeasure(field)) return false;
			OLAPCubeColumn column = cubeColumns[field.FieldName];
			return HasNullValues(column);
		}
		bool IPivotGridDataSource.GetIsOthersFieldValue(bool isColumn, int visibleIndex, int levelIndex) {
			return false;
		}
		int IPivotGridDataSource.GetCellCount(bool isColumn) {
			return GetFieldValues(isColumn).Count;
		}
		int IPivotGridDataSource.GetObjectLevel(bool isColumn, int visibleIndex) {
			OLAPAreaFieldValues values = GetFieldValues(isColumn);
			if(visibleIndex < 0 || visibleIndex >= values.Count) return -1;
			return values[visibleIndex].Level;
		}
		void IPivotGridDataSource.SaveCollapsedStateToStream(Stream stream) {
			SaveCollapsedStateToStreamCore(stream);
		}
		void IPivotGridDataSource.WebSaveCollapsedStateToStream(Stream stream) {
			SaveCollapsedStateToStreamCore(stream);
		}
		void IPivotGridDataSource.SaveDataToStream(Stream stream, bool compressed) {
			throw new Exception("The operation is not supported.");
		}
		void IPivotGridDataSource.LoadCollapsedStateFromStream(Stream stream) {
			LoadCollapsedStateFromStreamCore(stream);
		}
		void IPivotGridDataSource.WebLoadCollapsedStateFromStream(Stream stream) {
			LoadCollapsedStateFromStreamCore(stream);
		}
		bool IPivotGridDataSource.IsAreaAllowed(PivotGridFieldBase field, PivotArea area) {
			if(IsMeasure(field)) return area == PivotArea.DataArea;
			else return area != PivotArea.DataArea;
		}
		string[] IPivotGridDataSource.GetFieldList() {
			if(cubeColumns.Count == 0) PopulateColumns();
			List<string> result = new List<string>(cubeColumns.Count);
			foreach(KeyValuePair<string, OLAPCubeColumn> column in cubeColumns)
				result.Add(column.Key);
			return result.ToArray();
		}
		string IPivotGridDataSource.GetFieldCaption(string fieldName) {
			if(cubeColumns.Count == 0) PopulateColumns();
			if(!cubeColumns.ContainsKey(fieldName)) return null;
			return !string.IsNullOrEmpty(cubeColumns[fieldName].Caption) ? cubeColumns[fieldName].Caption : cubeColumns[fieldName].Name;
		}
		int IPivotGridDataSource.GetFieldHierarchyLevel(string fieldName) {
			if(cubeColumns.Count == 0) PopulateColumns();
			if(!cubeColumns.ContainsKey(fieldName)) return 0;
			return cubeColumns[fieldName].Level;
		}
		PivotKPIType IPivotGridDataSource.GetKPIType(PivotGridFieldBase field) {
			if(cubeColumns.Count == 0) PopulateColumns();
			OLAPKPIColumn column = cubeColumns[field.FieldName] as OLAPKPIColumn;
			if(column != null) return column.Type;
			return PivotKPIType.None; 
		}
		PivotKPIGraphic IPivotGridDataSource.GetKPIGraphic(PivotGridFieldBase field) {
			if(cubeColumns.Count == 0) PopulateColumns();
			OLAPKPIColumn column = cubeColumns[field.FieldName] as OLAPKPIColumn;
			if(column != null) return column.Graphic;
			return PivotKPIGraphic.None; 
		}
		#endregion
		public List<string> GetKPIList() {
			List<string> res = new List<string>();
			foreach(PivotOLAPKPIMeasures kpi in KPIs) {
				res.Add(kpi.KPIName);
			}
			return res;
		}
		public PivotOLAPKPIMeasures GetKPIMeasures(string kpiName) {
			foreach(PivotOLAPKPIMeasures kpi in KPIs) {
				if(kpi.KPIName == kpiName) return kpi;
			}
			return null;
		}
		public PivotOLAPKPIValue GetKPIValue(string kpiName) {
			return QueryKPIValue(kpiName);
		}
		public PivotKPIGraphic GetKPIServerDefinedGraphic(string kpiName, PivotKPIType kpiType) {
			if(kpiType != PivotKPIType.Status && kpiType != PivotKPIType.Trend) return PivotKPIGraphic.None;
			PivotOLAPKPIMeasures measures = GetKPIMeasures(kpiName);
			if(measures == null) return PivotKPIGraphic.None;
			return ((OLAPKPIColumn)cubeColumns[kpiType == PivotKPIType.Trend ? measures.TrendMeasure : measures.StatusMeasure]).Graphic;
		}
		public IOLAPMember[] GetUniqueMembers(string fieldName) {
			OLAPCubeColumn column = cubeColumns[fieldName];
			if(column == null || column.IsMeasure) return null;
			EnsureColumnMembersLoaded(column);
			return column.GetMembers();
		}
		public string GetDrillDownColumnName(string fieldName) {
			OLAPCubeColumn column = cubeColumns[fieldName];
			return column == null ? null : column.DrillDownColumn;
		}
		public PivotDrillDownDataSource GetDrillDownDataSource(int columnIndex, int rowIndex, int dataIndex, int maxRowCount, 
				List<string> customColumns) {
			if(columnIndex < -1 || columnIndex >= ColumnValues.Count ||
				rowIndex < -1 || rowIndex >= RowValues.Count ||
				dataIndex < 0 || dataIndex >= DataArea.Count) return null;
			return QueryDrilldown(columnIndex, rowIndex, dataIndex, maxRowCount, customColumns);
		}
		internal bool HasNullValues(OLAPCubeColumn column) {
			if(column.HasNullValues == UndefinedBoolean.Undefined)
				column.HasNullValues = QueryNullValues(column) ? UndefinedBoolean.Yes : UndefinedBoolean.No;
			return column.HasNullValues == UndefinedBoolean.Yes;
		}
		internal void EnsureHasCalculatedMembers(List<OLAPCubeColumn> columns) {
			if(columns == null || columns.Count == 0) return;
			List<OLAPCubeColumn> cols = null;
			for(int i = 0; i < columns.Count; i++) {
				if(columns[i].HasCalculatedMembers == UndefinedBoolean.Undefined) {
					if(cols == null)
						cols = new List<OLAPCubeColumn>();
					cols.Add(columns[i]);
				}
			}
			if(cols == null) return;
			bool[] res = QueryCalculatedMembers(cols);
			if(res == null) return;
			for(int i = 0; i < cols.Count; i++)
				cols[i].HasCalculatedMembers = res[i] ? UndefinedBoolean.Yes : UndefinedBoolean.No;
		}
	}
	public class OLAPMetaGetter : IDisposable {
		OleDbConnection olapConnection;
		protected OleDbConnection OlapConnection { get { return olapConnection; } }
		string connectionString;
		public string ConnectionString {
			get { return connectionString; }
			set {
				if(connectionString != value || !Connected) {
					connectionString = value;
					Connect();
				}
			}
		}
		public bool Connected { 
			get { return olapConnection != null; }
			set {
				if(Connected == value) return;
				if(value) Connect();
				else Disconnect();
			}
		}
		DataTable GetSchema(Guid guid, object[] restrictions) {
			if(!Connected) return null;
			return OlapConnection.GetOleDbSchemaTable(guid, restrictions);
		}
		public List<string> GetCatalogs() {
			if(!Connected) return null;
			DataTable catalogsTable = GetSchema(OLAPSchemaGuid.Catalogs, null);
			if(catalogsTable == null) return null;
			List<string> catalogs = new List<string>(catalogsTable.Rows.Count);
			for(int i = 0; i < catalogsTable.Rows.Count; i++)
				catalogs.Add((string)catalogsTable.Rows[i]["CATALOG_NAME"]);
			return catalogs;
		}
		public List<string> GetCubes(string catalogName) {
			if(!Connected || string.IsNullOrEmpty(catalogName)) return null;
			OlapConnection.ChangeDatabase(catalogName);
			DataTable cubesTable = GetSchema(OLAPSchemaGuid.Cubes, new object[] { catalogName });
			if(cubesTable == null) return null;
			List<string> cubes = new List<string>(cubesTable.Rows.Count);
			for(int i = 0; i < cubesTable.Rows.Count; i++)
				cubes.Add((string)cubesTable.Rows[i]["CUBE_NAME"]);
			return cubes;
		}
		public static List<string> GetProviders() {
			List<string> result = new List<string>();
			OleDbDataReader enumerator = OleDbEnumerator.GetRootEnumerator();
			while(enumerator.Read()) {
				if(((string)enumerator.GetValue(0)).ToLowerInvariant().StartsWith("msolap")) {
					RegistryKey progId = Registry.ClassesRoot.OpenSubKey("CLSID\\" + enumerator.GetValue(5).ToString() + "\\ProgID", false);
					if(progId != null) {
						string provider = progId.GetValue("") as string;
						if(!string.IsNullOrEmpty(provider) && !result.Contains(provider))
							result.Add(provider);
					}
				}
			}
			if(result.Count > 0 && !result.Contains("MSOLAP"))
				result.Insert(0, "MSOLAP");
			return result;
		}
		public static bool IsProviderAvailable { get { return GetProviders().Count > 0; } }
		string connectionException;
		public string ConnectionException { get { return connectionException; } }
		void Connect() {
			if(Connected) Disconnect();
			if(ConnectionString == null) return;
			olapConnection = new OleDbConnection(ConnectionString);
			try {
				olapConnection.Open();
				connectionException = "";
			} catch(Exception ex) {
				connectionException = ex.Message;
				Disconnect();
			}
		}
		void Disconnect() {
			if(!Connected) return;
			olapConnection.Dispose();
			olapConnection = null;
		}
		#region IDisposable Members
		public void Dispose() {
			Disconnect();
		}
		#endregion
	}
}
