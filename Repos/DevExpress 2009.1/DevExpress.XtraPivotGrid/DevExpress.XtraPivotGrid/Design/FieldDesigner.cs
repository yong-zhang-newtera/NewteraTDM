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
using System.Collections;
using System.ComponentModel;
using System.Drawing;
using System.Windows.Forms;
using DevExpress.XtraPivotGrid;
using DevExpress.XtraPivotGrid.Design;
using DevExpress.Utils.Frames;
using DevExpress.XtraEditors.Frames;
using DevExpress.Data.Helpers;
namespace DevExpress.XtraPivotGrid.Design.Frames {
	[ToolboxItem(false)]
	public class FieldDesigner : DevExpress.XtraEditors.Frames.ColumnDesignerBase	{
		PropertyDescriptor columnDescriptor = null;
		protected override PropertyDescriptor ColumnsDescriptor {
			get {
				if(columnDescriptor == null) {
					columnDescriptor = TypeDescriptor.GetProperties(ColumnsOwner)["Fields"];
				}
				return columnDescriptor;
			}
		}
		public PivotGridControl PivotGrid { get { return EditingObject as PivotGridControl; } }
		protected override Component ColumnsOwner { get {return PivotGrid; } }
		protected override CollectionBase Columns { get { return PivotGrid != null ? PivotGrid.Fields : null; } }
		protected override ColumnObject CreateColumnObject(object column) { return new PivotGridFieldObject(column); }
		protected override void RetrieveColumnsCore() {
			if(PivotGrid != null) {
				PivotGrid.RetrieveFields();
			}
		}
		protected override object CreateNewColumn(string fieldName, int index) {
			if(PivotGrid == null) return null;
			PivotGridField field = PivotGrid.Fields.Add(fieldName, PivotArea.ColumnArea);
			field.Index = index;
			return field;
		}
		protected override void OnColumnRemoved (object column, int index ) {
			PivotGrid.Fields.RemoveAt(index);
		}
		protected override string[] GetDataFieldList(){
			return PivotGrid.GetFieldList();
		}
		protected override bool CanRetrieveFields { get { return true; } }
		protected override string GroupControlColumnsText { get { return "PivotGrid Fields"; } }
		protected override string DescriptionText { 
			get { return "You can add and delete PivotGrid fields and modify their settings." + (VisibleList ? " Select and drag field to the PivotGrid fields panel to create PivotGrid field." : ""); }
		}
		protected override void OnPropertyValueChanged (System.Windows.Forms.PropertyValueChangedEventArgs e) {
			base.OnPropertyValueChanged(e);
			if(e.ChangedItem != null && e.ChangedItem.PropertyDescriptor.Name == "FieldName") {
				Invalidate(true);
			}
		}
	}
	public class PivotGridFieldObject : ColumnObject {
		public PivotGridFieldObject(object field) : base(field) {}
		public override string FieldName { get { return Field.FieldName; } }
		public override string Caption { get { return Field.ToString(); } }
		public override int AbsoluteIndex { get { return Field.Index; } set { Field.Index = value;  } }
		protected PivotGridField Field { get { return Column as PivotGridField; } }
	}
}
