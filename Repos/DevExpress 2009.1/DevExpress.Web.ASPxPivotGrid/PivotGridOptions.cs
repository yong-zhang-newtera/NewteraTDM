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
using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Text;
using System.ComponentModel;
using System.ComponentModel.Design;
using DevExpress.Data;
using DevExpress.Data.Helpers;
using DevExpress.Data.PivotGrid;
using DevExpress.XtraPivotGrid.Data;
using DevExpress.XtraPivotGrid;
using DevExpress.WebUtils;
using DevExpress.Utils.Serializing;
using DevExpress.Web.ASPxPager;
using DevExpress.Utils.Serializing.Helpers;
using System.Web.UI;
using System.Web.UI.WebControls;
using DevExpress.Web.ASPxClasses;
using DevExpress.Web.ASPxClasses.Internal;
namespace DevExpress.Web.ASPxPivotGrid {
	public enum PivotDataHeadersDisplayMode { Default, Popup };
	public class PivotGridWebOptionsView : PivotGridOptionsViewBase {
		bool showContextMenus, showContextMenusForAllFieldValues;
		PivotDataHeadersDisplayMode dataHeadersDisplayMode;
		int dataHeadersPopupMinCount;
		public PivotGridWebOptionsView(EventHandler optionsChanged, IViewBagOwner viewBagOwner, string objectPath)
			: base(optionsChanged, viewBagOwner, objectPath) {
			this.showContextMenus = true;
			this.dataHeadersDisplayMode = PivotDataHeadersDisplayMode.Default;
			this.dataHeadersPopupMinCount = 3;
		}
		[Description(""),
		DefaultValue(PivotDataHeadersDisplayMode.Default), XtraSerializableProperty(), NotifyParentProperty(true)]
		public PivotDataHeadersDisplayMode DataHeadersDisplayMode {
			get { return dataHeadersDisplayMode; }
			set {
				if(value == dataHeadersDisplayMode) return;
				dataHeadersDisplayMode = value;
				OnOptionsChanged();
			}
		}
		[Description(""),
		DefaultValue(3), XtraSerializableProperty(), NotifyParentProperty(true)]
		public int DataHeadersPopupMinCount {
			get { return dataHeadersPopupMinCount; }
			set {
				if(value == dataHeadersPopupMinCount) return;
				if(value < 0) value = 0;
				dataHeadersPopupMinCount = value;
				OnOptionsChanged();
			}
		}
		[Browsable(false), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden), EditorBrowsable(EditorBrowsableState.Never)]
		public new bool ShowHorzLines {
			get { return base.ShowHorzLines; }
			set { base.ShowHorzLines = value; }
		}
		[Browsable(false), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden), EditorBrowsable(EditorBrowsableState.Never)]
		public new bool ShowVertLines {
			get { return base.ShowVertLines; }
			set { base.ShowVertLines = value; }
		}
		[Description("")]
		[NotifyParentProperty(true), DefaultValue(true), XtraSerializableProperty]
		public bool ShowContextMenus {
			get { return showContextMenus; }
			set { showContextMenus = value; }
		}
		[Description("")]
		[NotifyParentProperty(true), DefaultValue(false), XtraSerializableProperty, Browsable(false)]
		[Obsolete("This property is obselete and will be removed in the next release")]
		public bool ShowContextMenusForAllFieldValues {
			get { return showContextMenusForAllFieldValues; }
			set { showContextMenusForAllFieldValues = value; }
		}
	}
	public class PivotGridWebOptionsCustomization : PivotGridOptionsCustomization {
		int customizationWindowWidth, customizationWindowHeight;
		public PivotGridWebOptionsCustomization(EventHandler optionsChanged, IViewBagOwner viewBagOwner, string objectPath)
			: base(optionsChanged, viewBagOwner, objectPath) {
			customizationWindowWidth = 150;
			customizationWindowHeight = 170;
		}
		[Browsable(false), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden), EditorBrowsable(EditorBrowsableState.Never)]
		public new AllowHideFieldsType AllowHideFields {
			get { return base.AllowHideFields; }
			set { base.AllowHideFields = value; }
		}
		[Description("")]
		[NotifyParentProperty(true), DefaultValue(150), XtraSerializableProperty]
		public int CustomizationWindowWidth {
			get { return customizationWindowWidth; }
			set { customizationWindowWidth = value; }
		}
		[Description("")]
		[NotifyParentProperty(true), DefaultValue(170), XtraSerializableProperty]
		public int CustomizationWindowHeight {
			get { return customizationWindowHeight; }
			set { customizationWindowHeight = value; }
		}
	}
	public class PivotGridWebOptionsDataField : PivotGridOptionsDataField {
		public PivotGridWebOptionsDataField(PivotGridData data, IViewBagOwner viewBagOwner, string objectPath)
			: base(data, viewBagOwner, objectPath) {
		}
		[Browsable(false), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden), EditorBrowsable(EditorBrowsableState.Never)]
		public new int RowHeaderWidth {
			get { return base.RowHeaderWidth; }
			set { base.RowHeaderWidth = value; }
		}
	}
	public class PivotGridWebOptionsLoadingPanel : PivotGridOptionsBase {
		readonly ASPxPivotGrid owner;
		protected ASPxPivotGrid Owner { get { return owner; } }
		public PivotGridWebOptionsLoadingPanel(ASPxPivotGrid owner)
			: base(null, null, string.Empty) {
			this.owner = owner;
		}
		[Category("LoadingPanel"), NotifyParentProperty(true), PersistenceMode(PersistenceMode.InnerProperty),
		DesignerSerializationVisibility(DesignerSerializationVisibility.Content), AutoFormatEnable()]
		public ImageProperties Image { get { return Owner.LoadingPanelImage; } }
		[Category("LoadingPanel"), NotifyParentProperty(true), PersistenceMode(PersistenceMode.InnerProperty),
		DesignerSerializationVisibility(DesignerSerializationVisibility.Content)]
		[XtraSerializableProperty(XtraSerializationVisibility.Visible), AutoFormatEnable()]
		public ImagePosition ImagePosition { get { return Owner.LoadingPanelImagePosition; } set { Owner.LoadingPanelImagePosition = value; } }
		[Category("LoadingPanel"), NotifyParentProperty(true), PersistenceMode(PersistenceMode.InnerProperty),
	   DesignerSerializationVisibility(DesignerSerializationVisibility.Content), AutoFormatEnable()]
		public LoadingPanelStyle Style { get { return Owner.LoadingPanelStyle; } }
		[Category("LoadingPanel"), NotifyParentProperty(true), PersistenceMode(PersistenceMode.Attribute),
		DesignerSerializationVisibility(DesignerSerializationVisibility.Visible)]
		[XtraSerializableProperty(XtraSerializationVisibility.Visible)]
		[DefaultValue(StringResources.LoadingPanelText), Localizable(true), AutoFormatEnable()]
		public string Text { get { return Owner.LoadingPanelText; } set { Owner.LoadingPanelText = value; } }
	}
	public enum PagerAlign { Left, Center, Right, Justify };	
	public class PivotGridWebOptionsPager : PagerSettingsEx {
		public enum OptionsPagerChangedReason { Unknown, PagerAlign, RowsPerPage, PageIndex };
		public class OptionsPagerChangedEventArgs : EventArgs {
			OptionsPagerChangedReason reason;
			public OptionsPagerChangedReason Reason { get { return reason; } }
			public OptionsPagerChangedEventArgs(OptionsPagerChangedReason reason) {
				this.reason = reason;
			}
		}
		public delegate void OptionsPagerChangedEventHandler(PivotGridWebOptionsPager sender, OptionsPagerChangedEventArgs e);
		OptionsPagerChangedEventHandler optionsChanged;
		protected OptionsPagerChangedEventHandler OptionsChanged { get { return optionsChanged; } }
		public PivotGridWebOptionsPager(OptionsPagerChangedEventHandler optionsChanged, IViewBagOwner viewBagOwner, string objectPath)
			: base() {
			this.optionsChanged = optionsChanged;
		}
		protected void Changed(OptionsPagerChangedReason reason) {
			if(OptionsChanged != null) OptionsChanged(this, new OptionsPagerChangedEventArgs(reason));
		}
		protected override void Changed() {
			if(OptionsChanged != null) OptionsChanged(this, new OptionsPagerChangedEventArgs(OptionsPagerChangedReason.Unknown));
		}
		[Description("Gets or sets the pager's horizontal alignment within the pivot grid."),
		DefaultValue(PagerAlign.Left), XtraSerializableProperty, NotifyParentProperty(true),
		AutoFormatEnable()]
		public PagerAlign PagerAlign {
			get { return (PagerAlign)GetEnumProperty("PagerAlign", PagerAlign.Left); }
			set {
				if(PagerAlign != value) {
					SetEnumProperty("PagerAlign", PagerAlign.Left, value);
					Changed(OptionsPagerChangedReason.PagerAlign);
				}
			}
		}
		[Description("Gets or sets the maximum number of rows displayed within a page."),
		DefaultValue(10), XtraSerializableProperty, NotifyParentProperty(true), AutoFormatEnable()]
		public int RowsPerPage {
			get { return GetIntProperty("RowsPerPage", 10); }
			set {
				if(RowsPerPage != value && value >= 0) {
					SetIntProperty("RowsPerPage", 10, value);
					Changed(OptionsPagerChangedReason.RowsPerPage);
				}
			}
		}
		[Description("Gets or sets the index of the page currently being selected."),
		DefaultValue(0), XtraSerializableProperty, NotifyParentProperty(true), AutoFormatDisable()]
		public int PageIndex {
			get { return GetIntProperty("PageIndex", 0); }
			set {
				if(PageIndex != value && value >= -1) {
					SetIntProperty("PageIndex", 0, value);
					Changed(OptionsPagerChangedReason.PageIndex);
				}
			}
		}
		[Browsable(false)]
		public bool HasPager { get { return Visible && RowsPerPage > 0; } }
	}
	public class PivotGridWebFieldOptions : PivotGridFieldOptions {
		public PivotGridWebFieldOptions(EventHandler optionsChanged, IViewBagOwner viewBagOwner, string objectPath)
			: base(optionsChanged, viewBagOwner, objectPath) { }
		[Browsable(false), EditorBrowsable(EditorBrowsableState.Never)]
		public new bool AllowRunTimeSummaryChange { get { return false; } }
		[Browsable(false), EditorBrowsable(EditorBrowsableState.Never)]
		public new bool ShowSummaryTypeName { get { return false; } }
	}
	public class PivotGridWebOptionsChartDataSource : PivotGridOptionsChartDataSourceBase {
		[Description(""),
		PersistenceMode(PersistenceMode.Attribute), NotifyParentProperty(true), XtraSerializableProperty(),
		DesignerSerializationVisibility(DesignerSerializationVisibility.Visible)]
		public new bool ChartDataVertical { get { return base.ChartDataVertical; } set { base.ChartDataVertical = value; } }
		[Description(""),
		PersistenceMode(PersistenceMode.Attribute), NotifyParentProperty(true), XtraSerializableProperty(),
		DesignerSerializationVisibility(DesignerSerializationVisibility.Visible)]
		public new bool ShowColumnCustomTotals { get { return base.ShowColumnCustomTotals; } set { base.ShowColumnCustomTotals = value; } }
		[Description(""),
		PersistenceMode(PersistenceMode.Attribute), NotifyParentProperty(true), XtraSerializableProperty(),
		DesignerSerializationVisibility(DesignerSerializationVisibility.Visible)]
		public new bool ShowColumnGrandTotals { get { return base.ShowColumnGrandTotals; } set { base.ShowColumnGrandTotals = value; } }
		[Description(""),
		PersistenceMode(PersistenceMode.Attribute), NotifyParentProperty(true), XtraSerializableProperty(),
		DesignerSerializationVisibility(DesignerSerializationVisibility.Visible)]
		public new bool ShowColumnTotals { get { return base.ShowColumnTotals; } set { base.ShowColumnTotals = value; } }
		[Description(""),
		PersistenceMode(PersistenceMode.Attribute), NotifyParentProperty(true), XtraSerializableProperty(),
		DesignerSerializationVisibility(DesignerSerializationVisibility.Visible)]
		public new bool ShowRowCustomTotals { get { return base.ShowRowCustomTotals; } set { base.ShowRowCustomTotals = value; } }
		[Description(""),
		PersistenceMode(PersistenceMode.Attribute), NotifyParentProperty(true), XtraSerializableProperty(),
		DesignerSerializationVisibility(DesignerSerializationVisibility.Visible)]
		public new bool ShowRowGrandTotals { get { return base.ShowRowGrandTotals; } set { base.ShowRowGrandTotals = value; } }
		[Description(""),
		PersistenceMode(PersistenceMode.Attribute), NotifyParentProperty(true), XtraSerializableProperty(),
		DesignerSerializationVisibility(DesignerSerializationVisibility.Visible)]
		public new bool ShowRowTotals { get { return base.ShowRowTotals; } set { base.ShowRowTotals = value; } }
	}
}
