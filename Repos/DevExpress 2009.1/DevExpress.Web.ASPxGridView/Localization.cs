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

using DevExpress.Utils.Localization;
using DevExpress.Utils.Localization.Internal;
using DevExpress.Web.ASPxClasses.Internal;
using DevExpress.Web.ASPxClasses.Localization;
namespace DevExpress.Web.ASPxGridView.Localization {
	public enum ASPxGridViewStringId {
		GroupPanel, EmptyDataRow, EmptyHeaders,
		ConfirmDelete, GroupContinuedOnNextPage,
		CustomizationWindowCaption, PopupEditFormCaption,
		HeaderFilterShowAllItem,
		CommandEdit, CommandNew, CommandDelete, CommandSelect, CommandCancel,
		CommandUpdate, CommandClearFilter,
		AutoFilterBeginsWith, AutoFilterContains, AutoFilterEndsWith, AutoFilterEquals,
		AutoFilterGreater, AutoFilterGreaterOrEqual, AutoFilterLess, AutoFilterLessOrEqual,
		AutoFilterNotEqual,
		Alt_HeaderFilterButton, Alt_HeaderFilterButtonActive,
		Alt_SortedAscending, Alt_SortedDescending,
		Alt_DragAndDropHideColumnIcon,
		Alt_Expand, Alt_Collapse, Alt_FilterRowButton,
		Checked, Unchecked,
		Outlook_Older, Outlook_LastMonth, Outlook_EarlierThisMonth, 
		Outlook_ThreeWeeksAgo, Outlook_TwoWeeksAgo, Outlook_LastWeek,
		Outlook_Yesterday, Outlook_Today, Outlook_Tomorrow,
		Outlook_NextWeek, Outlook_TwoWeeksAway, Outlook_ThreeWeeksAway,
		Outlook_LaterThisMonth, Outlook_NextMonth, Outlook_BeyondNextMonth
	}
	public class ASPxGridViewResLocalizer : ASPxResLocalizerBase<ASPxGridViewStringId> {
		public ASPxGridViewResLocalizer()
			: base(new ASPxGridViewLocalizer()) {
		}
		protected override string AssemblyName {
			get { return AssemblyInfo.SRAssemblyASPxGridView; }
		}
	}
	public class ASPxGridViewLocalizer : XtraLocalizer<ASPxGridViewStringId> {
		static ASPxGridViewLocalizer() {			
			ASPxActiveLocalizerProvider<ASPxGridViewStringId> provider = new ASPxActiveLocalizerProvider<ASPxGridViewStringId>(CreateResLocalizerInstance);
			SetActiveLocalizerProvider(provider);
		}
		static XtraLocalizer<ASPxGridViewStringId> CreateResLocalizerInstance() {
			return new ASPxGridViewResLocalizer();
		}
		public static string GetString(ASPxGridViewStringId id) {
			return Active.GetLocalizedString(id);
		}
		public override XtraLocalizer<ASPxGridViewStringId> CreateResXLocalizer() {
			return CreateResLocalizerInstance();
		}
		protected override void PopulateStringTable() {
			AddString(ASPxGridViewStringId.GroupPanel, StringResources.GridView_GroupPanel);
			AddString(ASPxGridViewStringId.EmptyDataRow, StringResources.GridView_EmptyDataRow);
			AddString(ASPxGridViewStringId.HeaderFilterShowAllItem, StringResources.GridView_HeaderFilterShowAllItem);
			AddString(ASPxGridViewStringId.EmptyHeaders, StringResources.GridView_EmptyHeaders);
			AddString(ASPxGridViewStringId.ConfirmDelete, StringResources.DataEditing_ConfirmDelete);
			AddString(ASPxGridViewStringId.CustomizationWindowCaption, StringResources.CustomizationWindowCaption);
			AddString(ASPxGridViewStringId.PopupEditFormCaption, StringResources.GridView_PopupEditFormCaption);
			AddString(ASPxGridViewStringId.GroupContinuedOnNextPage, StringResources.GridView_GroupContinuedOnNextPage);
			AddString(ASPxGridViewStringId.CommandEdit, StringResources.DataEditing_CommandEdit);
			AddString(ASPxGridViewStringId.CommandNew, StringResources.DataEditing_CommandNew);
			AddString(ASPxGridViewStringId.CommandDelete, StringResources.DataEditing_CommandDelete);
			AddString(ASPxGridViewStringId.CommandSelect, StringResources.DataEditing_CommandSelect);
			AddString(ASPxGridViewStringId.CommandCancel, StringResources.DataEditing_CommandCancel);
			AddString(ASPxGridViewStringId.CommandUpdate, StringResources.DataEditing_CommandUpdate);
			AddString(ASPxGridViewStringId.CommandClearFilter, StringResources.GridView_CommandClearFilter);
			AddString(ASPxGridViewStringId.AutoFilterBeginsWith, StringResources.FilterMenu_BeginsWith);
			AddString(ASPxGridViewStringId.AutoFilterContains, StringResources.FilterMenu_Contains);
			AddString(ASPxGridViewStringId.AutoFilterEndsWith, StringResources.FilterMenu_EndsWith);
			AddString(ASPxGridViewStringId.AutoFilterEquals, StringResources.FilterMenu_Equals);
			AddString(ASPxGridViewStringId.AutoFilterGreater, StringResources.FilterMenu_Greater);
			AddString(ASPxGridViewStringId.AutoFilterGreaterOrEqual, StringResources.FilterMenu_GreaterOrEqual);
			AddString(ASPxGridViewStringId.AutoFilterLess, StringResources.FilterMenu_Less);
			AddString(ASPxGridViewStringId.AutoFilterLessOrEqual, StringResources.FilterMenu_LessOrEqual);
			AddString(ASPxGridViewStringId.AutoFilterNotEqual, StringResources.FilterMenu_NotEqual);
			AddString(ASPxGridViewStringId.Alt_HeaderFilterButton, StringResources.Alt_FilterButton);
			AddString(ASPxGridViewStringId.Alt_HeaderFilterButtonActive, StringResources.Alt_FilterButtonActive);
			AddString(ASPxGridViewStringId.Alt_SortedAscending, StringResources.Alt_SortedAscending);
			AddString(ASPxGridViewStringId.Alt_SortedDescending, StringResources.Alt_SortedDescending);
			AddString(ASPxGridViewStringId.Alt_DragAndDropHideColumnIcon, StringResources.Alt_DragAndDropHideColumnIcon);
			AddString(ASPxGridViewStringId.Alt_Expand, StringResources.Alt_ExpandButton);
			AddString(ASPxGridViewStringId.Alt_Collapse, StringResources.Alt_CollapseButton);
			AddString(ASPxGridViewStringId.Alt_FilterRowButton, StringResources.Alt_FilterRowButton);
			AddString(ASPxGridViewStringId.Checked, "Checked");
			AddString(ASPxGridViewStringId.Unchecked, "Unchecked");
			AddString(ASPxGridViewStringId.Outlook_Older, "Older");
			AddString(ASPxGridViewStringId.Outlook_LastMonth, "Last Month");
			AddString(ASPxGridViewStringId.Outlook_EarlierThisMonth, "Earlier this Month");
			AddString(ASPxGridViewStringId.Outlook_ThreeWeeksAgo, "Three Weeks Ago");
			AddString(ASPxGridViewStringId.Outlook_TwoWeeksAgo, "Two Weeks Ago");
			AddString(ASPxGridViewStringId.Outlook_LastWeek, "Last Week");
			AddString(ASPxGridViewStringId.Outlook_Yesterday, "Yesterday");
			AddString(ASPxGridViewStringId.Outlook_Today, "Today");
			AddString(ASPxGridViewStringId.Outlook_Tomorrow, "Tomorrow");
			AddString(ASPxGridViewStringId.Outlook_NextWeek, "NextWeek");
			AddString(ASPxGridViewStringId.Outlook_TwoWeeksAway, "Two Weeks Away");
			AddString(ASPxGridViewStringId.Outlook_ThreeWeeksAway, "Three Weeks Away");
			AddString(ASPxGridViewStringId.Outlook_LaterThisMonth, "Later this Month");
			AddString(ASPxGridViewStringId.Outlook_NextMonth, "Next Month");
			AddString(ASPxGridViewStringId.Outlook_BeyondNextMonth, "Beyond Next Month");			
		}
	}
}
