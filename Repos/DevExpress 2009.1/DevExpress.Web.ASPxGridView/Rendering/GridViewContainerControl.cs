#region Copyright (c) 2000-2009 Developer Express Inc.
/*
{*******************************************************************}
{                                                                   }
{       Developer Express .NET Component Library                    }
{       ASPxGridView                                 }
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
using DevExpress.Web.ASPxGridView;
using DevExpress.Web.ASPxClasses;
using DevExpress.Web.ASPxClasses.Internal;
using DevExpress.Web.Data;
using DevExpress.Web.ASPxEditors;
using DevExpress.Web.ASPxEditors.FilterControl;
namespace DevExpress.Web.ASPxGridView.Rendering {
	public class GridViewContainerControl : ASPxInternalWebControl {
		ASPxGridView grid;
		GridViewHtmlCustomizationWindow customizationWindow;
		GridViewHtmlEditFormPopup popupEditForm;
		WebFilterControlPopup popupFilterControlForm;
		LoadingPanelControl loadingPanel;
		WebControl loadingDiv;
		HiddenField callbackState;
		public GridViewContainerControl(ASPxGridView grid) {
			this.grid = grid;
		}
		protected ASPxGridView Grid { get { return grid; } }
		protected WebDataProxy DataProxy { get { return Grid.DataProxy; } }
		protected ASPxGridViewScripts Scripts { get { return RenderHelper.Scripts; } }
		protected ASPxGridViewRenderHelper RenderHelper { get { return Grid.RenderHelper; } }
		protected ASPxGridViewSettings Settings { get { return Grid.Settings; } }
		protected ASPxGridViewCustomizationWindowSettings SettingsCustomizationWindow { get { return Grid.SettingsCustomizationWindow; } }
		protected ASPxGridViewPagerSettings SettingsPager { get { return Grid.SettingsPager; } }
		protected ASPxGridViewLoadingPanelSettings SettingsLoadingPanel { get { return Grid.SettingsLoadingPanel; } }
		protected GridViewImages Images { get { return Grid.Images; } }
		protected GridViewStyles Styles { get { return Grid.Styles; } }
		protected GridViewHtmlCustomizationWindow CustomizationWindow { get { return customizationWindow; } }
		protected GridViewHtmlEditFormPopup PopupEditForm { get { return popupEditForm; } }
		protected WebFilterControlPopup PopupFilterControlForm { get { return popupFilterControlForm; } }
		protected LoadingPanelControl LoadingPanel { get { return loadingPanel; } }
		protected WebControl LoadingDiv { get { return loadingDiv; } }
		protected HiddenField CallbackState { get { return callbackState; } }
		protected override void CreateControlHierarchy() {
			if(Grid.IsErrorOnCallbackCore) return;
			Grid.OnBeforeCreateControlHierarchy();
			RenderHelper.ClearControlHierarchy();
			if(RequireRenderTopPagerControl) {
				Controls.Add(new GridViewHtmlTopPagerPanel(RenderHelper));
			}
			if(Grid.Settings.ShowTitlePanel) {
				Controls.Add(new GridViewHtmlTitle(Grid));
			}
			if(Grid.Settings.ShowGroupPanel) {
				Controls.Add(new GridViewGroupPanel(RenderHelper, Grid.SortedColumns));
			}
			Controls.Add(CreateMainTable());
			CreateFixedColumnsScrollBar();
			Controls.Add(CreateHiddenImage(GridViewImages.ElementName_ArrowDragDownImage, GridViewImages.DragAndDropArrowDownName));
			Controls.Add(CreateHiddenImage(GridViewImages.ElementName_ArrowDragUpImage, GridViewImages.DragAndDropArrowUpName));
			Controls.Add(CreateHiddenImage(GridViewImages.ElementName_DragHideColumnImage, GridViewImages.DragAndDropHideColumnName));
			if(CanRenderPopupControls) {
				CreateCustomizationWindow();
				CreatePopupEditForm();
				CreatePopupFilterControlForm();
				if(DataProxy.HasParentRows) {
					Controls.Add(new GridViewHtmlParentRowsWindow(RenderHelper, DataProxy));
				}
			}
			if(RequireRenderBottomPagerControl) {
				Controls.Add(new GridViewHtmlBottomPagerPanel(RenderHelper));
			}
			if(RequireRenderFilterBar) {
				Controls.Add(new WebFilterControlPopupRow(Grid));
			}
			if(RequireRenderStatusBar) {
				Controls.Add(new GridViewHtmlStatusBar(Grid));
			}
			CreateLoadingPanel();
			CreateInputControlsAndStyleTable();
			if(RenderHelper.RequireRenderFilterRowMenu)
				Controls.Add(new GridViewFilterRowMenu(Grid));
		}
		protected override void PrepareControlHierarchy() {
			if(CallbackState != null) {
				Grid.OnAfterCreateControlHierarchy();
				CallbackState.Value = Grid.GetCallbackStateString();
			}
			if(LoadingPanel != null) {
				RenderHelper.PrepareLoadingPanel(LoadingPanel);
			}
			if(LoadingDiv != null) {
				RenderHelper.PrepareLoadingDiv(LoadingDiv);
			}
		}
		protected virtual WebControl CreateMainTable() {
			if(RenderHelper.ShowVerticalScrolling) return new GridViewHtmlScrollableControl(RenderHelper);
			WebControl table = new GridViewHtmlTable(RenderHelper);
			table.ID = RenderHelper.MainTableID;
			if (!RenderHelper.ShowHorizontalScrolling) return table;
			return new GridViewHtmlHorizontalScrollableControl(RenderHelper, table);
		}
		protected virtual void CreateFixedColumnsScrollBar() {
			if (!RenderHelper.HasFixedColumns) return;
			Controls.Add(new GridViewHtmlFixedColumnsScrollableControl(RenderHelper));
		}
		protected virtual bool CanRenderPopupControls { get { return !DesignMode && Page != null; } }
		protected virtual bool RequireRenderTopPagerControl { get { return RenderHelper.RequireRenderTopPagerControl; } }
		protected virtual bool RequireRenderBottomPagerControl { get { return RenderHelper.RequireRenderBottomPagerControl; } }
		public bool RequireRenderStatusBar {
			get {
				if(Settings.ShowStatusBar == GridViewStatusBarMode.Auto)
					return SettingsLoadingPanel.Mode == GridViewLoadingPanelMode.ShowOnStatusBar;
				return Settings.ShowStatusBar == GridViewStatusBarMode.Visible;
			}
		}
		public bool RequireRenderFilterBar {
			get {
				if(Settings.ShowFilterBar == GridViewStatusBarMode.Auto)
					return !string.IsNullOrEmpty(Grid.FilterExpression);
				return Settings.ShowFilterBar == GridViewStatusBarMode.Visible;
			}
		}
		protected bool HasLoadingPanel() {
			return SettingsLoadingPanel.Mode != GridViewLoadingPanelMode.Disabled;
		}
		protected virtual void CreateLoadingPanel() {
			if(!HasLoadingPanel() || Page == null || Grid.DesignMode) return;
			this.loadingPanel = new LoadingPanelControl();
			this.loadingPanel.EnableViewState = false;
			Controls.Add(LoadingPanel);
			LoadingPanel.ID = Grid.GetLoadingPanelIDInternal();
			LoadingPanel.Image = SettingsLoadingPanel.Mode == GridViewLoadingPanelMode.ShowAsPopup ?
				Grid.Images.GetImageProperties(Page, GridViewImages.LoadingPanelImageName) :
				Grid.Images.GetImageProperties(Page, GridViewImages.LoadingPanelOnStatusBarName);
			LoadingPanel.Settings = SettingsLoadingPanel;
			this.loadingDiv = RenderUtils.CreateWebControl(HtmlTextWriterTag.Div);
			LoadingDiv.ID = Grid.GetLoadingDivIDInternal(); 
			Controls.Add(LoadingDiv);
		}
		protected virtual void CreateInputControlsAndStyleTable() {
			if (Page == null || Grid.DesignMode) return;
			Controls.Add(RenderHelper.GenerateSelectInput());
			if (Grid.SettingsBehavior.AllowFocusedRow) {
				Controls.Add(RenderHelper.GenerateFocusedRowInput());
			}
			Controls.Add(new GridViewHtmlStyleTable(Grid));
			CreateCallbackViewState();
			if (RenderHelper.AllowColumnResizing) {
				Controls.Add(RenderHelper.GenereateColumnResizingInput());
			}
		}
		protected virtual void CreateCustomizationWindow() {
			if(RenderHelper.RequireRenderCustomizationWindow) {
				this.customizationWindow = new GridViewHtmlCustomizationWindow(Grid);
				Controls.Add(CustomizationWindow);
				CustomizationWindow.EnableViewState = false;
				CustomizationWindow.PopupHorizontalAlign = SettingsCustomizationWindow.PopupHorizontalAlign;
				CustomizationWindow.PopupHorizontalOffset = SettingsCustomizationWindow.PopupHorizontalOffset;
				CustomizationWindow.PopupVerticalAlign = SettingsCustomizationWindow.PopupVerticalAlign;
				CustomizationWindow.PopupVerticalOffset = SettingsCustomizationWindow.PopupVerticalOffset;
				if(!string.IsNullOrEmpty(Grid.ClientSideEvents.CustomizationWindowCloseUp)) {
					CustomizationWindow.CloseUp = Scripts.GetCustomizationWindowCloseUpHandler();
				}
			}
		}
		protected virtual void CreatePopupEditForm() {
			if(!Grid.SettingsEditing.IsPopupEditForm || !DataProxy.IsEditing || Grid.IsFilterControlVisible) return;
			this.popupEditForm = new GridViewHtmlEditFormPopup(Grid, DataProxy.EditingRowVisibleIndex);
			Controls.Add(PopupEditForm);
			PopupEditForm.EnableViewState = false;
			PopupEditForm.CloseUp = string.Format("function (s, e) {{ {0} }}", Scripts.GetCancelEditFunction(DataProxy.EditingRowVisibleIndex));
		}
		protected virtual void CreatePopupFilterControlForm() {
			if(!Grid.IsFilterControlVisible) return;
			this.popupFilterControlForm = new WebFilterControlPopup(Grid);
			Controls.Add(PopupFilterControlForm);
			PopupFilterControlForm.EnableViewState = false;
		}
		WebControl CreateHiddenImage(string id, string imageName) {
			Image image = RenderUtils.CreateImage();
			image.ID = id;
			image.Style.Add(HtmlTextWriterStyle.Position, "absolute");
			image.Style.Add(HtmlTextWriterStyle.Visibility, "hidden");
			RenderHelper.AssignImageToControl(imageName, image);
			return image;
		}
		protected virtual void CreateCallbackViewState() {
			this.callbackState = RenderUtils.CreateHiddenField(ASPxGridViewRenderHelper.CallbackHiddenFieldName);
			Controls.Add(CallbackState);
		}
		internal void ForceUpdate() {
			ResetControlHierarchy();
			CreateControlHierarchy();
		}
	}
}
