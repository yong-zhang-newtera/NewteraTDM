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
using System.Drawing;
using System.ComponentModel;
using System.Windows.Forms;
using DevExpress.Data;
using DevExpress.Data.PivotGrid;
using DevExpress.XtraEditors;
using DevExpress.XtraEditors.Controls;
using DevExpress.XtraEditors.Popup;
using DevExpress.XtraPivotGrid;
using DevExpress.XtraPivotGrid.ViewInfo;
using DevExpress.LookAndFeel;
using DevExpress.Utils;
using DevExpress.Utils.Drawing;
using DevExpress.XtraEditors.Drawing;
using DevExpress.XtraEditors.NavigatorButtons;
using DevExpress.XtraPivotGrid.Data;
using DevExpress.XtraPivotGrid.Localization;
using DevExpress.XtraEditors.Repository;
using DevExpress.Utils.Controls;
namespace DevExpress.XtraPivotGrid.FilterDropDown {
	public interface IPivotGridDropDownFilterEditOwner {
		void CloseFilter();
	}
	public class PivotGridDropDownFilterEdit {
		PivotGridViewInfoData data;
		PivotGridField field;
		Rectangle bounds;
		FilterPopupContainerEdit containerEdit;
		IPivotGridDropDownFilterEditOwner owner;
		public PivotGridDropDownFilterEdit(IPivotGridDropDownFilterEditOwner owner, PivotGridViewInfoData data, PivotGridField field, Rectangle bounds) {
			this.field = field;
			this.bounds = bounds;
			this.data = data;
			this.owner = owner;
		}
		PivotGridViewInfoData Data { get { return data; } }
		Control ParentControl { get { return Data != null  && (Data as IViewInfoControl) != null ? (Data as IViewInfoControl).ControlOwner : null; } }
		PivotGridField Field { get { return field; } }
		Rectangle Bounds { get { return bounds; } }
		IPivotGridDropDownFilterEditOwner Owner { get { return owner; } }
		public void Show() {
			if(Data == null || ParentControl == null || Field == null) return;
			SetupContainerEdit();
			ContainerEdit.ShowPopup();
		}
		protected FilterPopupContainerEdit ContainerEdit { get { return containerEdit; } }
		void SetupContainerEdit() {
			PivotGridFilterItems filterItems = new PivotGridFilterItems(Data, Field);
			filterItems.CreateItems();
			this.containerEdit = new PivotFilterPopupContainerEdit(filterItems);
			ContainerEdit.Text = string.Empty;
			ContainerEdit.Properties.AutoHeight = false;
			ContainerEdit.Properties.LookAndFeel.ParentLookAndFeel = Data.ActiveLookAndFeel;
			ContainerEdit.Properties.Appearance.BackColor = Color.Transparent;
			ContainerEdit.Properties.BorderStyle = DevExpress.XtraEditors.Controls.BorderStyles.NoBorder;
			ContainerEdit.Properties.Buttons.Clear();		
			ContainerEdit.CloseUp += new DevExpress.XtraEditors.Controls.CloseUpEventHandler(OnCloseUp);
			ContainerEdit.Closed += new ClosedEventHandler(OnClosed);
			ContainerEdit.Bounds = Bounds;
			if(!Field.DropDownFilterListSize.IsEmpty)
				ContainerEdit.Properties.PopupFormSize = Field.DropDownFilterListSize;
			ContainerEdit.Parent = ParentControl;
		}
		void OnClosed(object sender, ClosedEventArgs e) {
			if(owner != null)
				owner.CloseFilter();
			ContainerEdit.Dispose();
			this.containerEdit = null;
		}
		void OnCloseUp(object sender, DevExpress.XtraEditors.Controls.CloseUpEventArgs e) {
			Field.DropDownFilterListSize = ContainerEdit.PopupFormSize;			
		}	
	}
	public class PivotFilterPopupContainerEdit : FilterPopupContainerEdit {
		public PivotFilterPopupContainerEdit(IFilterItems filterItems) : base(filterItems) { }
		protected override PopupBaseForm CreatePopupForm() {
			return new PivotFilterPopupContainerForm(this, FilterItems);
		}
	}
	public class PivotFilterPopupContainerForm : FilterPopupContainerForm {
		public PivotFilterPopupContainerForm(BlobBaseEdit ownerEdit, IFilterItems filterItems) : base(ownerEdit, filterItems) { }
		protected override string GetShowAllItemCaption() {
			return PivotGridLocalizer.GetString(PivotGridStringId.FilterShowAll);
		}
	}
}
