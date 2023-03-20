#region Copyright (c) 2000-2009 Developer Express Inc.
/*
{*******************************************************************}
{                                                                   }
{       Developer Express .NET Component Library                    }
{       ASPxPivotGrid                                 }
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
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Text;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using DevExpress.Web.ASPxPivotGrid.Data;
using DevExpress.XtraPivotGrid;
using DevExpress.XtraPivotGrid.Data;
using DevExpress.Web.ASPxClasses.Internal;
using DevExpress.Web.ASPxClasses;
using DevExpress.Web.ASPxPopupControl;
using System.Collections;
namespace DevExpress.Web.ASPxPivotGrid.HtmlControls {
	[ToolboxItem(false)]
	public class PivotGridHtmlCustomizationFields : DevExpress.Web.ASPxPopupControl.ASPxPopupControl {
		const string ContentDivID = "ContentDiv";
		ASPxPivotGrid pivotGrid;
		Table maintable;
		WebControl contentDiv;
		public PivotGridHtmlCustomizationFields(ASPxPivotGrid pivotGrid) {
			this.pivotGrid = pivotGrid;
			Left = 100;
			Top = 100;
			ParentSkinOwner = PivotGrid;
			ID = PivotGridWebData.ElementName_CustomizationFieldsId;
			HeaderText = DevExpress.XtraPivotGrid.Localization.PivotGridLocalizer.GetString(DevExpress.XtraPivotGrid.Localization.PivotGridStringId.CustomizationFormCaption);
			AllowResize = true;
			AllowDragging = true;
			CloseAction = CloseAction.CloseButton;
			ClientSideEvents.PopUp = "function(s, e) { DoCustomizationFieldsVisibleChanged(s); }";
			ClientSideEvents.CloseUp = "function(s, e) { DoCustomizationFieldsVisibleChanged(s); }";
			ClientSideEvents.Shown = "function(s, e) { DoUpdateContentSize(s); }";
			ClientSideEvents.BeforeResizing = "function(s, e) { DoResetContentSize(s); }";
			ClientSideEvents.AfterResizing = "function(s, e) { DoUpdateContentSize(s); }";
		}
		public void Refresh() {
			ResetControlHierarchy();
		}
		protected ASPxPivotGrid PivotGrid { get { return pivotGrid; } }
		protected PivotGridWebData Data { get { return PivotGrid.Data; } }
		protected PivotGridFieldCollection Fields { get { return Data.Fields; } }
		protected Table MainTable { get { return maintable; } }
		protected WebControl ContentDiv { get { return contentDiv; } }
		protected int WindowWidth { get { return Data.OptionsCustomization.CustomizationWindowWidth; } }
		protected int WindowHeight { get { return Data.OptionsCustomization.CustomizationWindowHeight; } }
		protected override Paddings GetContentPaddings(PopupWindow window) {
			return new Paddings();
		}
		protected Paddings GetHeaderPaddings() {
			Paddings paddings = new Paddings();
			paddings.CopyFrom(Data.Styles.GetCustomizationFieldsPaddings(GetContentStyle(DefaultWindow).Font));
			paddings.CopyFrom(GetContentStyle(DefaultWindow).Paddings);
			return paddings;
		}
		protected override void ClearControlFields() {
			base.ClearControlFields();
			this.maintable = null;
			this.contentDiv = null;
		}
		protected override void CreateControlHierarchy() {
			this.contentDiv = RenderUtils.CreateWebControl(HtmlTextWriterTag.Div);
			this.maintable = RenderUtils.CreateTable();
			ContentDiv.ID = ContentDivID;
			CreateHeaders();
			ContentDiv.Controls.Add(MainTable);
			Controls.Clear();
			Controls.Add(ContentDiv);
			base.CreateControlHierarchy();
		}
		protected override void PrepareControlHierarchy() {
			base.PrepareControlHierarchy();
			Height = WindowHeight;
			Width = WindowWidth;
			if(ContentDiv != null) {
				if(IsFirefox) {
					ContentDiv.Width = Unit.Pixel(0);
					ContentDiv.Height = Unit.Pixel(0);
				} else {
					ContentDiv.Width = Unit.Percentage(100);
					ContentDiv.Height = Unit.Percentage(100);
				}
				if(RenderUtils.IsOverflowStyleSepareted)
					ContentDiv.Style.Add(HtmlTextWriterStyle.OverflowX, "hidden");
				ContentDiv.Style.Add(RenderUtils.IsOverflowStyleSepareted ? HtmlTextWriterStyle.OverflowY : HtmlTextWriterStyle.Overflow,
					DesignMode ? "hidden" : "auto");
			}
			if(MainTable != null) {
				MainTable.Width = Unit.Percentage(100);
				MainTable.GridLines = GridLines.None;
				MainTable.Style.Add(HtmlTextWriterStyle.BorderCollapse, "collapse");
				MainTable.CellPadding = 0; 
				MainTable.CellSpacing = 0;
				Paddings headerPaddings = GetHeaderPaddings();
				foreach(TableRow row in MainTable.Rows) {
					foreach(TableCell cell in row.Cells) {
						headerPaddings.AssignToControl(cell);
					}
				}
			}
			Styles.Header.Paddings.Assign(Data.Styles.GetCustomizationFieldsCaptionPaddings(Styles.Header.Font));
			Styles.Content.Paddings.Assign(Data.Styles.GetCustomizationFieldsPaddings(Styles.Content.Font));
			CloseButtonImage.CopyFrom(PivotGrid.RenderHelper.GetCustomizationFieldsCloseImage());
			ControlStyle.CopyFrom(PivotGrid.Styles.CustomizationFieldsStyle);
			CloseButtonStyle.CopyFrom(PivotGrid.Styles.CustomizationFieldsCloseButtonStyle);
			HeaderStyle.CopyFrom(PivotGrid.Styles.GetCustomizationFieldsHeaderStyle());
			ContentStyle.CopyFrom(PivotGrid.Styles.GetCustomizationFieldsContentStyle());
		}
		void CreateHeaders() {
			List<PivotGridField> fields = new List<PivotGridField>();
			for(int i = 0; i < Fields.Count; i++) {
				if(!Fields[i].CanShowInCustomizationForm) continue;
				fields.Add(Fields[i]);
			}
			fields.Sort(new StringComparer());
			foreach(PivotGridField field in fields)
				CreateHeader(field);
			TableRow row = RenderUtils.CreateTableRow();
			MainTable.Rows.Add(row);
			row.Cells.Add(RenderUtils.CreateTableCell());
		}
		void CreateHeader(PivotGridField field) {
			WebControl header = HeaderHelper.CreateHeader(field, Data, true);
			if(header == null)
				return;
			TableRow row = RenderUtils.CreateTableRow();
			MainTable.Rows.Add(row);
			TableCell cell = RenderUtils.CreateTableCell();
			cell.Controls.Add(header);
			row.Cells.Add(cell);
		}
		class StringComparer : IComparer<PivotGridField> {
			int IComparer<PivotGridField>.Compare(PivotGridField x, PivotGridField y) {
				return Comparer.Default.Compare(x.ToString(), y.ToString());
			}
		}
	}
}
