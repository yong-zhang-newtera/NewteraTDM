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
using DevExpress.Web.ASPxClasses;
using DevExpress.Web.ASPxClasses.Internal;
using DevExpress.Web.ASPxPivotGrid.Data;
using System.Collections.Generic;
using System.Web.UI;
using System.ComponentModel;
using DevExpress.XtraPivotGrid;
using DevExpress.XtraPivotGrid.Data;
using DevExpress.Utils.Serializing.Helpers;
using DevExpress.Utils.Serializing;
using DevExpress.XtraPivotGrid.Localization;
namespace DevExpress.Web.ASPxPivotGrid {
	public class PivotGridImages : ImagesBase {
		internal const string ElementName_ArrowDragDownImage = "IADD";
		internal const string ElementName_ArrowDragUpImage = "IADU";
		internal const string ElementName_DragHideFieldImage = "IDHF";
		internal const string ElementName_GroupSeparatorImage = "IGS";
		internal const string FieldValueCollapsedName = "pgCollapsedButton";
		internal const string FieldValueExpandedName = "pgExpandedButton";
		internal const string HeaderSortDownName = "pgSortDownButton";
		internal const string HeaderSortUpName = "pgSortUpButton";
		internal const string FilterWindowSizeGripName = "pgFilterResizer";
		internal const string HeaderFilterName = "pgFilterButton";
		internal const string HeaderActiveFilterName = "pgFilterButtonActive";
		internal const string CustomizationFieldsCloseName = "pgCustomizationFormCloseButton";
		internal const string CustomizationFieldsBackgroundName = "pgCustomizationFormBackground";
		internal const string DragArrowDownName = "pgDragArrowDown";
		internal const string DragArrowUpName = "pgDragArrowUp";
		internal const string DragHideFieldName = "pgDragHideField";
		internal const string DataHeadersPopupName = "pgDataHeaders";
		internal const string GroupSeparatorName = "pgGroupSeparator";
		internal const string SortByColumnName = "pgSortByColumn";
		internal const string PrefilterButtonName = "pgPrefilterButton";
		public PivotGridImages(ASPxPivotGrid owner)
			: base(owner) {
		}
		ASPxPivotGrid PivotGrid { get { return (ASPxPivotGrid)Owner; } }
		protected override void PopulateImageInfoList(List<ImageInfo> list) {
			list.Add(new ImageInfo(FieldValueCollapsedName, delegate() { return PivotGridLocalizer.GetString(PivotGridStringId.Alt_Expand); }, typeof(PivotGridImageProperties)));
			list.Add(new ImageInfo(FieldValueExpandedName, delegate() { return PivotGridLocalizer.GetString(PivotGridStringId.Alt_Collapse); }, typeof(PivotGridImageProperties)));
			list.Add(new ImageInfo(HeaderSortDownName, delegate() { return PivotGridLocalizer.GetString(PivotGridStringId.Alt_SortedDescending); }, typeof(PivotGridImageProperties)));
			list.Add(new ImageInfo(HeaderSortUpName, delegate() { return PivotGridLocalizer.GetString(PivotGridStringId.Alt_SortedAscending); }, typeof(PivotGridImageProperties)));
			list.Add(new ImageInfo(FilterWindowSizeGripName, delegate() { return PivotGridLocalizer.GetString(PivotGridStringId.Alt_FilterWindowSizeGrip); }, typeof(PivotGridImageProperties)));
			list.Add(new ImageInfo(HeaderFilterName, delegate() { return PivotGridLocalizer.GetString(PivotGridStringId.Alt_FilterButton); }, typeof(PivotGridImageProperties)));
			list.Add(new ImageInfo(HeaderActiveFilterName, delegate() { return PivotGridLocalizer.GetString(PivotGridStringId.Alt_FilterButtonActive); }, typeof(PivotGridImageProperties)));
			list.Add(new ImageInfo(CustomizationFieldsCloseName, typeof(PivotGridImageProperties)));
			list.Add(new ImageInfo(CustomizationFieldsBackgroundName, typeof(PivotGridImageProperties)));
			list.Add(new ImageInfo(DragArrowDownName, "|", typeof(PivotGridImageProperties)));
			list.Add(new ImageInfo(DragArrowUpName, "|", typeof(PivotGridImageProperties)));
			list.Add(new ImageInfo(DragHideFieldName, delegate() { return PivotGridLocalizer.GetString(PivotGridStringId.Alt_DragHideField); }, typeof(PivotGridImageProperties)));
			list.Add(new ImageInfo(LoadingPanelImageName, typeof(PivotGridImageProperties)));
			list.Add(new ImageInfo(DataHeadersPopupName, typeof(PivotGridImageProperties)));
			list.Add(new ImageInfo(GroupSeparatorName, "-", typeof(PivotGridImageProperties)));
			list.Add(new ImageInfo(SortByColumnName, "*", typeof(PivotGridImageProperties)));
			list.Add(new ImageInfo(PrefilterButtonName, ImageFlags.IsPng, 13, 13, 
				delegate() { return PivotGridLocalizer.GetString(PivotGridStringId.Alt_FilterButton); }, 
				typeof(PivotGridImageProperties)));
		}
		protected override Type GetResourceControlType() {
			return typeof(ASPxPivotGrid);
		}
		protected override string GetResourceImagePath() {
			return PivotGridWebData.PivotGridImagesResourcePath;
		}
		protected new PivotGridImageProperties GetImage(string imageName) {
			return (PivotGridImageProperties)base.GetImage(imageName);
		}
		[Category("Images"), PersistenceMode(PersistenceMode.InnerProperty),
		DesignerSerializationVisibility(DesignerSerializationVisibility.Content), NotifyParentProperty(true)]
		[XtraSerializableProperty(XtraSerializationVisibility.Content), AutoFormatEnable()]
		public PivotGridImageProperties FieldValueCollapsed { get { return GetImage(FieldValueCollapsedName); } }
		[Category("Images"), PersistenceMode(PersistenceMode.InnerProperty),
		DesignerSerializationVisibility(DesignerSerializationVisibility.Content), NotifyParentProperty(true)]
		[XtraSerializableProperty(XtraSerializationVisibility.Content), AutoFormatEnable()]
		public PivotGridImageProperties FieldValueExpanded { get { return GetImage(FieldValueExpandedName); } }
		[Category("Images"), PersistenceMode(PersistenceMode.InnerProperty),
		DesignerSerializationVisibility(DesignerSerializationVisibility.Content), NotifyParentProperty(true)]
		[XtraSerializableProperty(XtraSerializationVisibility.Content), AutoFormatEnable()]
		public PivotGridImageProperties HeaderFilter { get { return GetImage(HeaderFilterName); } }
		[Category("Images"), PersistenceMode(PersistenceMode.InnerProperty),
		DesignerSerializationVisibility(DesignerSerializationVisibility.Content), NotifyParentProperty(true)]
		[XtraSerializableProperty(XtraSerializationVisibility.Content), AutoFormatEnable()]
		public PivotGridImageProperties HeaderActiveFilter { get { return GetImage(HeaderActiveFilterName); } }
		[Category("Images"), PersistenceMode(PersistenceMode.InnerProperty),
		DesignerSerializationVisibility(DesignerSerializationVisibility.Content), NotifyParentProperty(true)]
		[XtraSerializableProperty(XtraSerializationVisibility.Content), AutoFormatEnable()]
		public PivotGridImageProperties HeaderSortDown { get { return GetImage(HeaderSortDownName); } }
		[Category("Images"), PersistenceMode(PersistenceMode.InnerProperty),
		DesignerSerializationVisibility(DesignerSerializationVisibility.Content), NotifyParentProperty(true)]
		[XtraSerializableProperty(XtraSerializationVisibility.Content), AutoFormatEnable()]
		public PivotGridImageProperties HeaderSortUp { get { return GetImage(HeaderSortUpName); } }
		[Category("Images"), PersistenceMode(PersistenceMode.InnerProperty),
		DesignerSerializationVisibility(DesignerSerializationVisibility.Content), NotifyParentProperty(true)]
		[XtraSerializableProperty(XtraSerializationVisibility.Content), AutoFormatEnable()]
		public PivotGridImageProperties FilterWindowSizeGrip { get { return GetImage(FilterWindowSizeGripName); } }
		[Category("Images"), PersistenceMode(PersistenceMode.InnerProperty),
		DesignerSerializationVisibility(DesignerSerializationVisibility.Content), NotifyParentProperty(true)]
		[XtraSerializableProperty(XtraSerializationVisibility.Content), AutoFormatEnable()]
		public PivotGridImageProperties CustomizationFieldsClose { get { return GetImage(CustomizationFieldsCloseName); } }
		[Category("Images"), PersistenceMode(PersistenceMode.InnerProperty),
		DesignerSerializationVisibility(DesignerSerializationVisibility.Content), NotifyParentProperty(true)]
		[XtraSerializableProperty(XtraSerializationVisibility.Content), AutoFormatEnable()]
		public PivotGridImageProperties CustomizationFieldsBackground { get { return GetImage(CustomizationFieldsBackgroundName); } }
		[Category("Images"), PersistenceMode(PersistenceMode.InnerProperty),
		DesignerSerializationVisibility(DesignerSerializationVisibility.Content), NotifyParentProperty(true)]
		[XtraSerializableProperty(XtraSerializationVisibility.Content), AutoFormatEnable()]
		public PivotGridImageProperties DragArrowDown { get { return GetImage(DragArrowDownName); } }
		[Category("Images"), PersistenceMode(PersistenceMode.InnerProperty),
		DesignerSerializationVisibility(DesignerSerializationVisibility.Content), NotifyParentProperty(true)]
		[XtraSerializableProperty(XtraSerializationVisibility.Content), AutoFormatEnable()]
		public PivotGridImageProperties DragArrowUp { get { return GetImage(DragArrowUpName); } }
		[Category("Images"), PersistenceMode(PersistenceMode.InnerProperty),
		DesignerSerializationVisibility(DesignerSerializationVisibility.Content), NotifyParentProperty(true)]
		[XtraSerializableProperty(XtraSerializationVisibility.Content), AutoFormatEnable()]
		public PivotGridImageProperties DragHideField { get { return GetImage(DragHideFieldName); } }
		[Category("Images"), PersistenceMode(PersistenceMode.InnerProperty),
		DesignerSerializationVisibility(DesignerSerializationVisibility.Content), NotifyParentProperty(true)]
		[XtraSerializableProperty(XtraSerializationVisibility.Content)]
		[Browsable(false), EditorBrowsable(EditorBrowsableState.Never), AutoFormatEnable()]
		public new PivotGridImageProperties LoadingPanel { get { return GetImage(LoadingPanelImageName); } }
		[Category("Images"), PersistenceMode(PersistenceMode.InnerProperty),
		DesignerSerializationVisibility(DesignerSerializationVisibility.Content), NotifyParentProperty(true)]
		[XtraSerializableProperty(XtraSerializationVisibility.Content), AutoFormatEnable()]
		public PivotGridImageProperties DataHeadersPopup { get { return GetImage(DataHeadersPopupName); } }
		[Category("Images"), PersistenceMode(PersistenceMode.InnerProperty),
		DesignerSerializationVisibility(DesignerSerializationVisibility.Content), NotifyParentProperty(true)]
		[XtraSerializableProperty(XtraSerializationVisibility.Content), AutoFormatEnable()]
		public PivotGridImageProperties GroupSeparator { get { return GetImage(GroupSeparatorName); } }
		[Category("Images"), PersistenceMode(PersistenceMode.InnerProperty),
		DesignerSerializationVisibility(DesignerSerializationVisibility.Content), NotifyParentProperty(true)]
		[XtraSerializableProperty(XtraSerializationVisibility.Content), AutoFormatEnable()]
		public PivotGridImageProperties SortByColumn { get { return GetImage(SortByColumnName); } }
		[Category("Images"), PersistenceMode(PersistenceMode.InnerProperty),
		DesignerSerializationVisibility(DesignerSerializationVisibility.Content), NotifyParentProperty(true)]
		[XtraSerializableProperty(XtraSerializationVisibility.Content), AutoFormatEnable()]
		public PivotGridImageProperties PrefilterButton { get { return GetImage(PrefilterButtonName); } }
	}
	public class PivotGridImageProperties : ImageProperties, IXtraSerializable2 {
		public PivotGridImageProperties() : base() { }
		public PivotGridImageProperties(IPropertiesOwner owner) : base(owner) { }
		public PivotGridImageProperties(string url) : base(url) { }
		#region IXtraSerializable2 Members
		void IXtraSerializable2.Deserialize(System.Collections.IList list) {
			StateManagerSerializeHelper.DeserializeObject(this, (IXtraPropertyCollection)list);
		}
		XtraPropertyInfo[] IXtraSerializable2.Serialize() {
			ReadOnlyViewState.SetDirty(true);
			return StateManagerSerializeHelper.SerializeObject(this);
		}
		#endregion
	}
	public class PivotKPIImages : ImagesBase {
		ASPxPivotGrid pivot;
		public PivotKPIImages(ASPxPivotGrid owner)
			: base(null) {
			this.pivot = owner;
		}
		protected ASPxPivotGrid PivotGrid { get { return pivot; } }
		protected override void PopulateImageInfoList(List<ImageInfo> list) {
			foreach(PivotKPIGraphic graphic in Enum.GetValues(typeof(PivotKPIGraphic))) {
				if(graphic == PivotKPIGraphic.None || graphic == PivotKPIGraphic.ServerDefined) continue;
				string name = graphic.ToString();
				list.Add(new ImageInfo(name + ".-1", ImageFlags.IsPng));
				list.Add(new ImageInfo(name + ".0", ImageFlags.IsPng));
				list.Add(new ImageInfo(name + ".1", ImageFlags.IsPng));
			}
		}		
		protected override Type GetResourceControlType() {
			return typeof(PivotGridData);
		}
		protected override string GetResourceImagePath() {
			return PivotGridData.PivotGridImagesResourcePath;
		}
		public ImageProperties GetImage(PivotKPIGraphic graphic, PivotKPIType type, int state) {
			if(state != 0 && state != -1 && state != 1) throw new ArgumentException("state");
			if(graphic == PivotKPIGraphic.None || graphic == PivotKPIGraphic.ServerDefined) throw new ArgumentException("graphic");
			ImageProperties res = GetImageProperties(PivotGrid.Page, graphic.ToString() + "." + state.ToString());
			res.AlternateText = PivotGrid.Data.GetKPITooltipText(type, state);
			return res;
		}				
	}
}
