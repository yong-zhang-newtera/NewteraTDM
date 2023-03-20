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

using System.ComponentModel;
using DevExpress.Web.ASPxClasses;
using DevExpress.Web.ASPxGridView.Localization;
namespace DevExpress.Web.ASPxGridView.Rendering {
	[ToolboxItem(false)]
	public class GridViewFilterRowMenu : DevExpress.Web.ASPxMenu.ASPxPopupMenu {
		ASPxGridView grid;
		public GridViewFilterRowMenu(ASPxGridView grid)
			: base(grid) {
			this.grid = grid;
			CreateItems();
			EnableViewState = false;
			ParentSkinOwner = grid;
			ControlStyle.CopyFrom(Grid.Styles.FilterRowMenu);
			ItemStyle.CopyFrom(Grid.Styles.FilterRowMenuItem);
			PopupVerticalAlign = PopupVerticalAlign .Below;
			EnableClientSideAPIInternal = true;
			ID = ASPxGridViewRenderHelper.DXFilterRowMenu;
			ClientSideEvents.ItemClick = Grid.RenderHelper.Scripts.GetFilterRowMenuItemClick();
		}
		protected ASPxGridView Grid { get { return grid; } }
		void CreateItems() {
			//CreateItem(ASPxGridViewStringId.AutoFilterBeginsWith, AutoFilterCondition.BeginsWith);
			CreateItem(ASPxGridViewStringId.AutoFilterContains, AutoFilterCondition.Contains);
			//CreateItem(ASPxGridViewStringId.AutoFilterEndsWith, AutoFilterCondition.EndsWith);
			CreateItem(ASPxGridViewStringId.AutoFilterEquals, AutoFilterCondition.Equals);
			CreateItem(ASPxGridViewStringId.AutoFilterNotEqual, AutoFilterCondition.NotEqual);
			CreateItem(ASPxGridViewStringId.AutoFilterLess, AutoFilterCondition.Less);
			CreateItem(ASPxGridViewStringId.AutoFilterLessOrEqual, AutoFilterCondition.LessOrEqual);
			CreateItem(ASPxGridViewStringId.AutoFilterGreater, AutoFilterCondition.Greater);
			CreateItem(ASPxGridViewStringId.AutoFilterGreaterOrEqual, AutoFilterCondition.GreaterOrEqual);
		}
		void CreateItem(ASPxGridViewStringId stringId, AutoFilterCondition value) {
			Items.Add(ASPxGridViewLocalizer.GetString(stringId), ((int)value).ToString()).GroupName = "_";
		}
	}
}
