#region Copyright (c) 2000-2009 Developer Express Inc.
/*
{*******************************************************************}
{                                                                   }
{       Developer Express .NET Component Library                    }
{       XtraPivotGrid                                 }
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
using System.Data;
using System.Collections;
using System.Drawing;
using System.ComponentModel;
using System.Windows.Forms;
using DevExpress.Data;
using DevExpress.Utils.Design;
using DevExpress.Utils;
using DevExpress.Utils.Serializing;
using DevExpress.XtraPivotGrid;
using DevExpress.XtraPivotGrid.Data;
using DevExpress.XtraEditors;
using DevExpress.XtraGrid;
namespace DevExpress.XtraPivotGrid {
	public enum PivotGridCellType { Cell, Total, GrandTotal, CustomTotal };
	[ListBindable(false)]
	public class PivotGridFormatConditionCollection : FormatConditionCollectionBase {
		PivotGridData data;
		public PivotGridFormatConditionCollection() {
		}
		[Description("Provides indexed access to individual items within the collection.")]
		public new PivotGridStyleFormatCondition this[int index] { get { return base[index] as PivotGridStyleFormatCondition; } }
		[Description("Provides indexed access to individual items within the collection.")]
		public new PivotGridStyleFormatCondition this[object tag] { get { return base[tag] as PivotGridStyleFormatCondition; } }
		protected internal PivotGridData Data { get { return data; } set { data = value; } }
		[Description("Indicates whether the pivot grid that owns the current collection is currently being initialized.")]
		public override bool IsLoading { get { return Data == null || Data.IsLoading; } }
		public override int CompareValues(object val1, object val2) {
			return Data.CompareValues(val1, val2);
		}
		public PivotGridStyleFormatCondition GetStyleFormatByValue(PivotGridFieldBase field, object value, PivotGridCellType cellType) {
			int cnt = Count;
			if(cnt == 0) return null;
			for(int n = 0; n < cnt; n++) {
				PivotGridStyleFormatCondition cond = this[n];
				if(cond.CanApplyToCell(cellType) && cond.CheckValue(field, value)) 
					return cond;
			}
			return null;
		}
		protected override object CreateItem() { return new PivotGridStyleFormatCondition(); }
		public void Add(PivotGridStyleFormatCondition condition) {
			base.Add(condition);
		}
		public void AddRange(PivotGridStyleFormatCondition[] conditions) {
			base.AddRange(conditions);
		}
	}
	[TypeConverter(typeof(DevExpress.Utils.Design.UniversalTypeConverterEx))]
	public class PivotGridStyleFormatCondition : StyleFormatConditionBase, IPivotGridViewInfoDataOwner {
		bool applytoCell = true, applytoTotalCell = true, applytoGrandTotalCell = true, applytoCustomTotalCell = true;
		public PivotGridStyleFormatCondition() : base() {}
		[Description("Gets the PivotGrid control which owns the current style format condition."), Browsable(false)]
		public PivotGridControl PivotGrid {
			get {
				PivotGridFormatConditionCollection collection = Collection as PivotGridFormatConditionCollection;
				if(collection != null && collection.Data != null)
					return ((PivotGridViewInfoData)collection.Data).PivotGrid;
				return null;
			}
		}
		protected PivotGridData Data {
			get {
				PivotGridFormatConditionCollection collection = Collection as PivotGridFormatConditionCollection;
				return collection != null ? collection.Data : null;
			}
		}
		PivotGridViewInfoData IPivotGridViewInfoDataOwner.DataViewInfo {
			get { return this.Data as PivotGridViewInfoData; }
		}
		[
		Description("Gets or sets the field which identifies the columns whose values are used in conditional formatting."),
		DXDisplayName(typeof(DevExpress.XtraPivotGrid.ResFinder), DXDisplayNameAttribute.DefaultResourceFile, "DevExpress.XtraPivotGrid.PivotGridStyleFormatCondition.Field"),
		DefaultValue(null), DesignerSerializationVisibility(DesignerSerializationVisibility.Visible),
		TypeConverter("DevExpress.XtraPivotGrid.TypeConverters.FieldReferenceConverter, " + AssemblyInfo.SRAssemblyPivotGrid)
		]
		public PivotGridFieldBase Field {
			get { return (PivotGridFieldBase)Column; }
			set { Column = value; }
		}
		[Browsable(false), EditorBrowsable(EditorBrowsableState.Never), XtraSerializableProperty()]
		public string FieldName {
			get { 
				if(Field != null) 
					return Field.Name;
				return string.Empty;
			}
			set {
				if(value != null && value != string.Empty && Data != null) {
					Field = (PivotGridFieldBase)Data.Fields.GetFieldByName(value);
				}
			}
		}
		[
		Description("Gets or sets whether the appearance settings can be applied to data cells."),
		DXDisplayName(typeof(DevExpress.XtraPivotGrid.ResFinder), DXDisplayNameAttribute.DefaultResourceFile, "DevExpress.XtraPivotGrid.PivotGridStyleFormatCondition.ApplyToCell"),
		TypeConverter(typeof(BooleanTypeConverter)),
		XtraSerializableProperty(), DefaultValue(true)
		]
		public bool ApplyToCell {
			get { return applytoCell; }
			set {
				if(ApplyToCell == value) return;
				applytoCell = value;
				ItemChanged();
			}
		}
		[
		Description("Gets or sets whether the appearance settings are applied to automatic total cells."),
		DXDisplayName(typeof(DevExpress.XtraPivotGrid.ResFinder), DXDisplayNameAttribute.DefaultResourceFile, "DevExpress.XtraPivotGrid.PivotGridStyleFormatCondition.ApplyToTotalCell"),
		TypeConverter(typeof(BooleanTypeConverter)),
		XtraSerializableProperty(), DefaultValue(true)
		]
		public bool ApplyToTotalCell {
			get { return applytoTotalCell; }
			set {
				if(ApplyToTotalCell == value) return;
				applytoTotalCell = value;
				ItemChanged();
			}
		}
		[
		Description("Gets or sets whether the appearance settings are applied to grand total cells."),
		DXDisplayName(typeof(DevExpress.XtraPivotGrid.ResFinder), DXDisplayNameAttribute.DefaultResourceFile, "DevExpress.XtraPivotGrid.PivotGridStyleFormatCondition.ApplyToGrandTotalCell"),
		TypeConverter(typeof(BooleanTypeConverter)),
		XtraSerializableProperty(), DefaultValue(true)
		]
		public bool ApplyToGrandTotalCell {
			get { return applytoGrandTotalCell; }
			set {
				if(ApplyToGrandTotalCell == value) return;
				applytoGrandTotalCell = value;
				ItemChanged();
			}
		}
		[
		Description("Gets or sets whether the appearance settings are applied to custom total cells."),
		DXDisplayName(typeof(DevExpress.XtraPivotGrid.ResFinder), DXDisplayNameAttribute.DefaultResourceFile, "DevExpress.XtraPivotGrid.PivotGridStyleFormatCondition.ApplyToCustomTotalCell"),
		TypeConverter(typeof(BooleanTypeConverter)),
		XtraSerializableProperty(), DefaultValue(true)
		]
		public bool ApplyToCustomTotalCell {
			get { return applytoCustomTotalCell; }
			set {
				if(ApplyToCustomTotalCell == value) return;
				applytoCustomTotalCell = value;
				ItemChanged();
			}
		}
		public bool CanApplyToCell(PivotGridCellType cellType) {
			if(cellType == PivotGridCellType.Cell)
				return ApplyToCell;
			if(cellType == PivotGridCellType.Total)
				return ApplyToTotalCell;
			if(cellType == PivotGridCellType.GrandTotal)
				return ApplyToGrandTotalCell;
			if(cellType == PivotGridCellType.CustomTotal)
				return ApplyToCustomTotalCell;
			return true;
		}
	}
}
