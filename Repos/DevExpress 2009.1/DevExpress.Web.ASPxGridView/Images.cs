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
using System.ComponentModel;
using System.Collections.Generic;
using System.Drawing;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using DevExpress.Web.ASPxClasses;
using DevExpress.Web.ASPxClasses.Internal;
using DevExpress.Web.ASPxGridView;
using DevExpress.Web.ASPxEditors;
using DevExpress.Web.ASPxGridView.Localization;
namespace DevExpress.Web.ASPxGridView {
	public class GridViewImages : ImagesBase {
		protected internal const string ElementName_ArrowDragDownImage = "IADD";
		protected internal const string ElementName_ArrowDragUpImage = "IADU";
		protected internal const string ElementName_DragHideColumnImage = "IDHF";
		protected internal const string LoadingPanelOnStatusBarName = "gvLoadingOnStatusBar",
			CollapsedButtonName = "gvCollapsedButton",
			ExpandedButtonName = "gvExpandedButton",
			DetailCollapsedButtonName = "gvDetailCollapsedButton",
			DetailExpandedButtonName = "gvDetailExpandedButton",
			HeaderFilterName = "gvHeaderFilter",
			HeaderFilterActiveName = "gvHeaderFilterActive",
			HeaderSortDownName = "gvHeaderSortDown",
			HeaderSortUpName = "gvHeaderSortUp",
			DragAndDropArrowDownName = "gvDragAndDropArrowDown",
			DragAndDropArrowUpName = "gvDragAndDropArrowUp",
			DragAndDropHideColumnName = "gvDragAndDropHideColumn",
			ParentGroupRowsName = "gvParentGroupRows",
			FilterRowButtonName = "gvFilterRowButton";
		ImageProperties customizationWindowClose;
		ImageProperties popupEditFormWindowClose;
		ImageProperties filterBuilderClose;
		public GridViewImages(ISkinOwner owner) : base(owner) {
			this.customizationWindowClose = new ImageProperties(owner);
			this.popupEditFormWindowClose = new ImageProperties(owner);
			this.filterBuilderClose = new ImageProperties(owner);
		}
		protected override void PopulateImageInfoList(List<ImageInfo> list) {
			base.PopulateImageInfoList(list);
			list.Add(new ImageInfo(CollapsedButtonName, ImageFlags.IsPng | ImageFlags.HasDesignModeImage, 9, 10, delegate() { return ASPxGridViewLocalizer.GetString(ASPxGridViewStringId.Alt_Expand); }));
			list.Add(new ImageInfo(ExpandedButtonName, ImageFlags.IsPng | ImageFlags.HasDesignModeImage, 9, 10, delegate() { return ASPxGridViewLocalizer.GetString(ASPxGridViewStringId.Alt_Collapse); }));
			list.Add(new ImageInfo(DetailCollapsedButtonName, ImageFlags.IsPng | ImageFlags.HasDesignModeImage, 9, 10, delegate() { return ASPxGridViewLocalizer.GetString(ASPxGridViewStringId.Alt_Expand); }));
			list.Add(new ImageInfo(DetailExpandedButtonName, ImageFlags.IsPng | ImageFlags.HasDesignModeImage, 9, 10, delegate() { return ASPxGridViewLocalizer.GetString(ASPxGridViewStringId.Alt_Collapse); }));
			list.Add(new ImageInfo(FilterRowButtonName, ImageFlags.IsPng, 13, 13, delegate() { return ASPxGridViewLocalizer.GetString(ASPxGridViewStringId.Alt_FilterRowButton); }));
			list.Add(new ImageInfo(HeaderFilterName, delegate() { return ASPxGridViewLocalizer.GetString(ASPxGridViewStringId.Alt_HeaderFilterButton); }));
			list.Add(new ImageInfo(HeaderFilterActiveName, delegate() { return ASPxGridViewLocalizer.GetString(ASPxGridViewStringId.Alt_HeaderFilterButtonActive); }));
			list.Add(new ImageInfo(HeaderSortDownName, delegate() { return ASPxGridViewLocalizer.GetString(ASPxGridViewStringId.Alt_SortedDescending); }));
			list.Add(new ImageInfo(HeaderSortUpName, delegate() { return ASPxGridViewLocalizer.GetString(ASPxGridViewStringId.Alt_SortedAscending); }));
			list.Add(new ImageInfo(DragAndDropArrowDownName, "|"));
			list.Add(new ImageInfo(DragAndDropArrowUpName, "|"));
			list.Add(new ImageInfo(DragAndDropHideColumnName, delegate() { return ASPxGridViewLocalizer.GetString(ASPxGridViewStringId.Alt_DragAndDropHideColumnIcon); }));
			list.Add(new ImageInfo(ParentGroupRowsName, "..."));
			list.Add(new ImageInfo(LoadingPanelOnStatusBarName));
		}
		public override string ToString() { return string.Empty; }
		protected override Type GetResourceControlType() {
			return typeof(ASPxGridView);
		}
		protected override string GetResourceImagePath() {
			return ASPxGridView.WebResourceImagePath;
		}
		[Description("Gets the settings of an image displayed within a Loading Panel when it is displayed at the ASPxGridView's Status Bar."),
		Category("Images"), PersistenceMode(PersistenceMode.InnerProperty), AutoFormatEnable,
		DesignerSerializationVisibility(DesignerSerializationVisibility.Content), NotifyParentProperty(true)]
		public ImageProperties LoadingPanelOnStatusBar { get { return GetImage(LoadingPanelOnStatusBarName); } }
		[Description("Gets the settings of an image displayed within expand buttons of collapsed group rows."),
		Category("Images"), PersistenceMode(PersistenceMode.InnerProperty), AutoFormatEnable,
		DesignerSerializationVisibility(DesignerSerializationVisibility.Content), NotifyParentProperty(true)]
		public ImageProperties CollapsedButton { get { return GetImage(CollapsedButtonName); } }
		[Description("Gets the settings of an image displayed within expand buttons of group rows."),
		Category("Images"), PersistenceMode(PersistenceMode.InnerProperty), AutoFormatEnable,
		DesignerSerializationVisibility(DesignerSerializationVisibility.Content), NotifyParentProperty(true)]
		public ImageProperties ExpandedButton { get { return GetImage(ExpandedButtonName); } }
		[Description("Gets the settings of an image displayed within expand buttons of collapsed data rows (master-detail mode)."),
		Category("Images"), PersistenceMode(PersistenceMode.InnerProperty), AutoFormatEnable,
		DesignerSerializationVisibility(DesignerSerializationVisibility.Content), NotifyParentProperty(true)]
		public ImageProperties DetailCollapsedButton { get { return GetImage(DetailCollapsedButtonName); } }
		[Description("Gets the settings of an image displayed within expand buttons of data rows (master-detail mode)."),
		Category("Images"), PersistenceMode(PersistenceMode.InnerProperty), AutoFormatEnable,
		DesignerSerializationVisibility(DesignerSerializationVisibility.Content), NotifyParentProperty(true)]
		public ImageProperties DetailExpandedButton { get { return GetImage(DetailExpandedButtonName); } }
		[Description("Gets the settings of an image displayed within filter buttons"),
		Category("Images"), PersistenceMode(PersistenceMode.InnerProperty), AutoFormatEnable,
		DesignerSerializationVisibility(DesignerSerializationVisibility.Content), NotifyParentProperty(true)]
		public ImageProperties HeaderFilter { get { return GetImage(HeaderFilterName); } }
		[Description("Gets the settings of an image displayed within filter buttons displayed within columns that are involved in filtering."),
		Category("Images"), PersistenceMode(PersistenceMode.InnerProperty), AutoFormatEnable,
		DesignerSerializationVisibility(DesignerSerializationVisibility.Content), NotifyParentProperty(true)]
		public ImageProperties HeaderActiveFilter { get { return GetImage(HeaderFilterActiveName); } }
		[Description("Gets the settings of an image displayed within a column's header when the column is sorted in descending order."),
		Category("Images"), PersistenceMode(PersistenceMode.InnerProperty), AutoFormatEnable,
		DesignerSerializationVisibility(DesignerSerializationVisibility.Content), NotifyParentProperty(true)]
		public ImageProperties HeaderSortDown { get { return GetImage(HeaderSortDownName); } }
		[Description("Gets the settings of an image displayed within a column's header when the column is sorted in ascending order."),
		Category("Images"), PersistenceMode(PersistenceMode.InnerProperty), AutoFormatEnable,
		DesignerSerializationVisibility(DesignerSerializationVisibility.Content), NotifyParentProperty(true)]
		public ImageProperties HeaderSortUp { get { return GetImage(HeaderSortUpName); } }
		[Description("Gets the settings of an image displayed at the column header's bottom when it is dragged by an end-user."),
		Category("Images"), PersistenceMode(PersistenceMode.InnerProperty), AutoFormatEnable,
		DesignerSerializationVisibility(DesignerSerializationVisibility.Content), NotifyParentProperty(true)]
		public ImageProperties DragAndDropArrowDown { get { return GetImage(DragAndDropArrowDownName); } }
		[Description("Gets the settings of an image displayed at the column header's top when it is dragged by an end-user."),
		Category("Images"), PersistenceMode(PersistenceMode.InnerProperty), AutoFormatEnable,
		DesignerSerializationVisibility(DesignerSerializationVisibility.Content), NotifyParentProperty(true)]
		public ImageProperties DragAndDropArrowUp { get { return GetImage(DragAndDropArrowUpName); } }
		[Description("Gets the settings of an image displayed below the column header's  when it is dragged to the Customization Window by an end-user."),
		Category("Images"), PersistenceMode(PersistenceMode.InnerProperty), AutoFormatEnable,
		DesignerSerializationVisibility(DesignerSerializationVisibility.Content), NotifyParentProperty(true)]
		public ImageProperties DragAndDropColumnHide { get { return GetImage(DragAndDropHideColumnName); } }
		[Description("Gets the settings of an image used to indicate to which group the data rows belongs."),
		Category("Images"), PersistenceMode(PersistenceMode.InnerProperty), AutoFormatEnable,
		DesignerSerializationVisibility(DesignerSerializationVisibility.Content), NotifyParentProperty(true)]
		public ImageProperties ParentGroupRows { get { return GetImage(ParentGroupRowsName); } }
		[Description("Gets the settings of an image displayed within a filter button."),
		Category("Images"), PersistenceMode(PersistenceMode.InnerProperty), AutoFormatEnable,
		DesignerSerializationVisibility(DesignerSerializationVisibility.Content), NotifyParentProperty(true)]
		public ImageProperties FilterRowButton { get { return GetImage(FilterRowButtonName); } }
		[Description("Gets the settings of an image displayed within the Customization Window's close button."),
		Category("Images"), PersistenceMode(PersistenceMode.InnerProperty), AutoFormatEnable,
		DesignerSerializationVisibility(DesignerSerializationVisibility.Content), NotifyParentProperty(true)]
		public ImageProperties CustomizationWindowClose { get { return customizationWindowClose; } }
		[Description("Gets the settings of an image displayed within the Popup Edit Form's close button."),
		Category("Images"), PersistenceMode(PersistenceMode.InnerProperty), AutoFormatEnable,
		DesignerSerializationVisibility(DesignerSerializationVisibility.Content), NotifyParentProperty(true)]
		public ImageProperties PopupEditFormWindowClose { get { return popupEditFormWindowClose; } }
		[Description("Gets the settings of an image displayed within the size grip."),
		Category("Images"), PersistenceMode(PersistenceMode.InnerProperty), AutoFormatEnable,
		DesignerSerializationVisibility(DesignerSerializationVisibility.Content), NotifyParentProperty(true)]
		public ImageProperties WindowResizer { get { return WindowResizerInternal; } }
		[Description("Gets the settings of an image displayed within the Filter Control's Close button."),
		Category("Images"), PersistenceMode(PersistenceMode.InnerProperty), AutoFormatEnable,
		DesignerSerializationVisibility(DesignerSerializationVisibility.Content), NotifyParentProperty(true)]
		public ImageProperties FilterBuilderClose { get { return filterBuilderClose; } }
	}
	public class GridViewEditorImages : EditorImages {
		public GridViewEditorImages(ISkinOwner skinOwner) : base(skinOwner) { }
	}
}
