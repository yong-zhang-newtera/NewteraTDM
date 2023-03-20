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
using System.Web.UI.WebControls;
using DevExpress.Web.ASPxClasses.Internal;
using DevExpress.Web.ASPxClasses;
using DevExpress.XtraPivotGrid;
using System.Collections.Generic;
using DevExpress.Web.ASPxPivotGrid.Data;
using System.Text;
using DevExpress.XtraPivotGrid.Data;
using System.Web.UI;
namespace DevExpress.Web.ASPxPivotGrid {
	public class ASPxPivotGridRenderHelper {
		ASPxPivotGrid pivotGrid;
		readonly Dictionary<string, object[]> fieldValueContextMenuParams;
		readonly Dictionary<string, object[]> headerContextMenuParams;
		public ASPxPivotGridRenderHelper(ASPxPivotGrid pivotGrid) {
			this.pivotGrid = pivotGrid;
			this.fieldValueContextMenuParams = new Dictionary<string, object[]>();
			this.headerContextMenuParams = new Dictionary<string, object[]>();
		}
		protected ASPxPivotGrid PivotGrid {
			get { return pivotGrid; }
		}
		protected PivotGridFieldCollection Fields {
			get { return PivotGrid.Fields; }
		}
		protected PivotGridWebData Data {
			get { return PivotGrid.Data; }
		}
		protected string ClientID {
			get { return PivotGrid.ClientID; }
		}
		protected Page Page {
			get { return PivotGrid.Page; }
		}
		protected PivotGridWebGroupCollection Groups {
			get { return PivotGrid.Groups; }
		}
		ImageProperties RenderImage(string imageName) {
			return PivotGrid.Images.GetImageProperties(PivotGrid.Page, imageName, true);
		}
		public ImageProperties GetHeaderFilterImage() {
			return RenderImage(PivotGridImages.HeaderFilterName);
		}
		public ImageProperties GetHeaderActiveFilterImage() {
			return RenderImage(PivotGridImages.HeaderActiveFilterName);
		}
		public ImageProperties GetHeaderSortImage(PivotSortOrder sortOrder) {
			return sortOrder == PivotSortOrder.Ascending ?
				RenderImage(PivotGridImages.HeaderSortUpName) :
				RenderImage(PivotGridImages.HeaderSortDownName);
		}
		public ImageProperties GetFieldValueCollapsedImage(bool isCollapsed) {
			return GetExpandedCollapsedImage(!isCollapsed);
		}
		public ImageProperties GetGroupSeparatorImage() {
			return RenderImage(PivotGridImages.GroupSeparatorName);
		}
		public ImageProperties GetGroupButtonImage(bool isExpanded) {
			return GetExpandedCollapsedImage(isExpanded);
		}
		public ImageProperties GetExpandedCollapsedImage(bool isExpanded) {
			return isExpanded ?
				RenderImage(PivotGridImages.FieldValueExpandedName) :
				RenderImage(PivotGridImages.FieldValueCollapsedName);
		}
		public ImageProperties GetFilterWindowSizeGripImage() {
			return RenderImage(PivotGridImages.FilterWindowSizeGripName);
		}
		public ImageProperties GetCustomizationFieldsCloseImage() {
			return RenderImage(PivotGridImages.CustomizationFieldsCloseName);
		}
		public ImageProperties GetCustomizationFieldsBackgroundImage() {
			return RenderImage(PivotGridImages.CustomizationFieldsBackgroundName);
		}
		public ImageProperties GetDragArrowUpImage() {
			return RenderImage(PivotGridImages.DragArrowUpName);
		}
		public ImageProperties GetDragArrowDownImage() {
			return RenderImage(PivotGridImages.DragArrowDownName);
		}
		public ImageProperties GetDragHideFieldImage() {
			return RenderImage(PivotGridImages.DragHideFieldName);
		}
		public ImageProperties GetLoadingPanelImage() {
			return RenderImage(PivotGridImages.LoadingPanelImageName);
		}
		public ImageProperties GetDataHeadersImage() {
			return RenderImage(PivotGridImages.DataHeadersPopupName);
		}
		public ImageProperties GetSortByColumnImage() {
			return RenderImage(PivotGridImages.SortByColumnName);
		}
		public ImageProperties GetPrefilterImage() {
			return RenderImage(PivotGridImages.PrefilterButtonName);
		}
		public string GetAllowedAreaIdsScript() {
			Dictionary<string, List<string>> allowed = GetAllowedAreaIds();
			if(allowed.Count == 0)
				return string.Empty;
			StringBuilder stb = new StringBuilder();
			stb.Append("pivotGrid_AllowedAreaIds[\"").Append(ClientID).AppendLine("\"] = {");
			foreach(string key in allowed.Keys) {
				List<string> ids = allowed[key];
				stb.Append("\"").Append(ClientID).Append("_").Append(key).Append("\" : new Array(");
				for(int i = 0; i < ids.Count; i++) {
					string value = ids[i];
					stb.Append("\"").Append(ClientID).Append("_").Append(value).Append("\", ");
				}
				stb.Length -= 2;
				stb.AppendLine("),");
			}
			stb.Length -= 3;
			stb.Append("};");
			return stb.ToString();
		}
		protected internal Dictionary<string, List<string>> GetAllowedAreaIds() {
			Dictionary<string, List<string>> dictionary = new Dictionary<string, List<string>>();
			for(int i = 0; i < Fields.Count; i++) {
				PivotGridField field = Fields[i];
				if(field.AllowedAreas == PivotGridAllowedAreas.All || field.InnerGroupIndex > 0)
					continue;
				dictionary.Add(Data.GetID(field), GetAllowedAreaIds(field));
			}
			return dictionary;
		}
		protected internal List<string> GetAllowedAreaIds(PivotGridField field) {
			List<string> ids = new List<string>();
			foreach(PivotArea area in Enum.GetValues(typeof(PivotArea))) {
				if(!field.IsAreaAllowed(area))
					continue;
				PivotGridField[] fields = Data.GetFieldsByArea(area);
				int beforeCount = ids.Count;
				AddAreaIds(ids, fields);
				if(ids.Count == beforeCount)
					ids.Add(Data.GetAreaID(area));
			}
			return ids;
		}
		void AddAreaIds(List<string> ids, PivotGridField[] fields) {
			for(int i = 0; i < fields.Length; i++) {
				PivotGridField field = fields[i];
				if(field.InnerGroupIndex > 0)
					continue;
				ids.Add(Data.GetID(field));
			}
		}
		public string GetAfterCallBackInitializeScript() {
			return "pivotGrid_AfterCallBackInitialize(\"" + ClientID + "\");";
		}
		public void AddFieldValueContextMenu(string id, PivotFieldValueItem item, List<PivotGridFieldPair> sortedFields) {
			object[] contextMenuParams = new object[] { 
					Data.GetCollapsedFieldValueChangeState(item), 
					item.IsCollapsed,
					item.Field != null ? ((PivotGridField)item.Field).ClientID : string.Empty,
					item.VisibleIndex,
					item.IsLastFieldLevel,
					item.Area.ToString(),
					GetSortedFieldsString(sortedFields),
					item.IsDataLocatedInThisArea && item.DataField != null ? item.DataField.Index : -1
				};
			if(!fieldValueContextMenuParams.ContainsKey(id))
				fieldValueContextMenuParams.Add(id, contextMenuParams);
		}		
		public void AddHeaderContextMenu(string id, PivotGridField field) {
			if(!headerContextMenuParams.ContainsKey(id)) {
				string hideFieldArgs = field != null ? PivotGridWebData.HideField +
					PivotGridWebData.ArgumentsSeparator + field.Index.ToString() : string.Empty,
					fieldID = field != null ? field.ClientID : "";
				headerContextMenuParams.Add(id, new string[] { hideFieldArgs, fieldID });
			}
		}
		public void ResetMenus() {
			fieldValueContextMenuParams.Clear();
			headerContextMenuParams.Clear();
		}
		public string GetContextMenuScript() {
			StringBuilder stb = new StringBuilder();
			GetContextMenuScriptCore(stb, "pivotGrid_FieldValueCMParams", fieldValueContextMenuParams);
			GetContextMenuScriptCore(stb, "pivotGrid_HeaderCMParams", headerContextMenuParams);
			return stb.ToString();
		}
		protected void GetContextMenuScriptCore(StringBuilder stb, string varName, Dictionary<string, object[]> menuParams) {
			stb.Append(varName + "[\"").Append(ClientID).Append("\"] = new Array(\r\n");
			bool first = true;
			foreach(string elemID in menuParams.Keys) {
				if(!first)
					stb.Append(",\r\n");
				stb.Append("new Array(\"").Append(elemID).Append("\", ");
				object[] pars = menuParams[elemID];
				for(int i = 0; i < pars.Length; i++) {
					AddParamStr(stb, pars[i]);
					if(i != pars.Length - 1)
						stb.Append(", ");
				}
				stb.Append(")");
				first = false;
			}
			stb.Append(");\r\n");
		}
		protected void AddParamStr(StringBuilder stb, object param) {
			if(param == null)
				throw new ArgumentNullException("param");
			if(param is string) {
				stb.Append("\"").Append(param.ToString()).Append("\"");
				return;
			}
			if(param is int) {
				stb.Append((int)param);
				return;
			}
			if(param is bool) {
				stb.Append((bool)param ? "true" : "false");
				return;
			}
			if(param is string[]) {
				AddStringArrayParam(stb, (string[])param);
				return;
			}
			throw new ArgumentException("param");
		}
		protected void AddStringArrayParam(StringBuilder stb, object[] p) {
			stb.Append("[");
			for(int i = 0; i < p.Length; i++) {
				AddParamStr(stb, p[i]);
				if(i != p.Length - 1)
					stb.Append(", ");
			}
			stb.Append("]");
		}
		protected string[] GetSortedFieldsString(List<PivotGridFieldPair> sortedFields) {
			if(sortedFields == null || sortedFields.Count == 0) return new string[0];
			string[] res = new string[sortedFields.Count];
			for(int i = 0; i < sortedFields.Count; i++) {
				PivotGridFieldPair pair = sortedFields[i];
				res[i] = pair.Field.Index.ToString() + "_" + pair.DataField.Index.ToString();
			}
			return res;
		}
		public string GetHoverScript() {
			StringBuilder stb = new StringBuilder();
			StateScriptRenderHelper helper = new StateScriptRenderHelper(Page, ClientID);
			for(int i = 0; i < Fields.Count; i++) {
				PivotGridField field = Fields[i];
				helper.AddStyle(Data.GetHeaderHoverStyle(field), Data.GetHeaderID(field), PivotGridWebData.FieldHeaderIdPostfixes);
			}
			helper.GetCreateHoverScript(stb);
			return stb.ToString();
		}
		public string GetGroupsScript() {
			StringBuilder stb = new StringBuilder();
			stb.Append("pivotGrid_Groups[\"").Append(ClientID).AppendLine("\"] = new Array(");
			for(int i = 0; i < Groups.Count; i++) {
				PivotGridWebGroup group = Groups[i];
				stb.Append("\tnew ASPxClientPivotGridGroup(\"").Append(ClientID).Append("\", ");
				GetGroupFields(stb, group);
				stb.Append(")");
				if(i != Groups.Count - 1)
					stb.AppendLine(", ");
				else
					stb.AppendLine();
			}
			stb.Append(");");
			return stb.ToString();
		}
		protected void GetGroupFields(StringBuilder stb, PivotGridWebGroup group) {
			stb.Append("new Array(");
			List<PivotGridFieldBase> fields = group.GetVisibleFields();
			for(int i = 0; i < fields.Count; i++) {
				PivotGridField field = (PivotGridField)fields[i];
				stb.Append("\"").Append(Data.GetHeaderID(field)).Append("\"");
				if(i != fields.Count - 1)
					stb.Append(", ");
			}
			stb.Append(")");
		}
	}
}
