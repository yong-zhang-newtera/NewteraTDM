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
using System.ComponentModel;
using System.ComponentModel.Design;
using System.Text;
using System.Web.UI;
using System.Web.UI.Design;
using DevExpress.Utils.About;
using DevExpress.Web.ASPxClasses.Internal;
using DevExpress.Web.ASPxPivotGrid.Data;
using System.Collections;
using System.Collections.Generic;
using DevExpress.Web.ASPxClasses.Design;
using DevExpress.Web.ASPxPivotGrid;
using DevExpress.Web.ASPxPivotGrid.HtmlControls;
using System.Web.UI.Design.WebControls;
namespace DevExpress.Web.ASPxPivotGrid.Design {
	public class PivotGridControlDesigner : ASPxDataWebControlDesigner, IDataSourceViewSchemaAccessor {
		public override string CssTemplatePath {
			get { return PivotGridWebData.PivotGridCssResourceName; }
		}
		protected ASPxPivotGrid Grid {
			get { return Component as ASPxPivotGrid; }
		}
		public override void Initialize(IComponent component) {
			base.Initialize(component);
			SetViewFlags(ViewFlags.TemplateEditing, true);
		}
		protected override string GetDesignTimeHtmlInternal() {
			Grid.Data.DataSourceViewSchemaAccessor = this;
			TypeDescriptor.Refresh(Component);
			return base.GetDesignTimeHtmlInternal();
		}
		protected override int SampleRowCount {
			get {
				return 10;
			}
		}
		protected int SelectedFieldIndex {
			get { return Grid.Data.SelectedFieldIndex; }
			set { Grid.Data.SelectedFieldIndex = value; }
		}
		static string[] controlTemplateNames = new string[] { "HeaderTemplate", "EmptyAreaTemplate", "FieldValueTemplate", "CellTemplate" };
		static string fieldTemplateName = "Fields[{0}].{1}";
		static string[] fieldTemplateNames = new string[] { "HeaderTemplate", "ValueTemplate" };
		public override TemplateGroupCollection TemplateGroups {
			get {
				TemplateGroupCollection templateGroups = base.TemplateGroups;
				for(int i = 0; i < Grid.Fields.Count; i++) {
					PivotGridField field = Grid.Fields[i];
					TemplateGroup templateGroup = new TemplateGroup(string.Format(fieldTemplateName, i, field.ToString()));
					for(int j = 0; j < fieldTemplateNames.Length; j++) {
						TemplateDefinition templateDefinition = new TemplateDefinition(this, fieldTemplateNames[j], field, fieldTemplateNames[j], this.GetTemplateStyle());
						templateDefinition.SupportsDataBinding = true;
						templateGroup.AddTemplateDefinition(templateDefinition);
					}
					templateGroups.Add(templateGroup);
				}
				for(int i = 0; i < controlTemplateNames.Length; i++) {
					TemplateGroup templateGroup = new TemplateGroup(controlTemplateNames[i]);
					TemplateDefinition templateDefinition = new TemplateDefinition(this, controlTemplateNames[i], base.Component, controlTemplateNames[i], this.GetTemplateStyle());
					templateDefinition.SupportsDataBinding = true;
					templateGroup.AddTemplateDefinition(templateDefinition);
					templateGroups.Add(templateGroup);
				}
				return templateGroups;
			}
		}
		System.Web.UI.WebControls.Style GetTemplateStyle() {
			return Grid.ControlStyle;
		}
		public string[] GetFieldList() {
			return Grid.Data.GetFieldList();
		}
		protected override ASPxWebControlDesignerActionList CreateCommonActionList() {
			return new PivotGridDesignerActionList(this);
		}
		protected override DataWebControlActionListBase CreateDataActionList() {
			return new PivotGridDataWebControlActionList(this);
		}
		public override bool HasHttpHandler() {
			return true;
		}
		public void EditFields() {
			ShowDialog(CreateEditorForm("Fields", Grid.Fields));
		}
		public void EditGroups() {
			ShowDialog(CreateEditorForm("Groups", Grid.Groups));
		}
		public override void ShowAbout() {
			PivotGridAboutDialogHelper.ShowAbout(Component.Site);
		}
		object IDataSourceViewSchemaAccessor.DataSourceViewSchema {
			get {
				if(DesignerView == null) return null;
				if(DesignerView.Schema == null && DataSourceDesigner != null) {
					DataSourceDesigner.RefreshSchema(true);
				}
				return DesignerView.Schema;
			}
			set { ;}
		}
		protected override void PreFilterProperties(IDictionary properties) {
			base.PreFilterProperties(properties);
			PerformPrefilterProperty(properties, "DataSourceID", typeof(PivotGridDataSourceIDConverter));
			foreach(object propName in new ArrayList(properties.Keys)) {
				PropertyDescriptor prop = (PropertyDescriptor)properties[propName];
				if(prop.Attributes[typeof(ObsoleteAttribute)] as ObsoleteAttribute != null) {
					properties[propName] = TypeDescriptor.CreateProperty(typeof(ASPxPivotGrid), prop, new DesignerSerializationVisibilityAttribute(DesignerSerializationVisibility.Hidden));
				}
			}
		}
		public override void EnsureControlReferences() {
			base.EnsureControlReferences();
			EnsureReferences("DevExpress.Web" + AssemblyInfo.VSuffix);
			EnsureReferences("DevExpress.XtraPivotGrid" + AssemblyInfo.VSuffix + ".Core");
			EnsureReferences("DevExpress.Web.ASPxPivotGrid" + AssemblyInfo.VSuffix);
			EnsureReferences("DevExpress.Web.ASPxEditors" + AssemblyInfo.VSuffix);
		}
	}
	public class PivotGridDataSourceIDConverter : DataSourceIDConverter {
		ASPxPivotGrid pivotGrid;
		public override TypeConverter.StandardValuesCollection GetStandardValues(ITypeDescriptorContext context) {
			this.pivotGrid = GetPivotGrid(context);
			StandardValuesCollection res = base.GetStandardValues(context);
			this.pivotGrid = null;
			return res;
		}
		protected ASPxPivotGrid GetPivotGrid(ITypeDescriptorContext context) {
			ASPxPivotGrid res = context.Instance as ASPxPivotGrid;
			if(res != null) 
				return res;
			PivotGridDataWebControlActionList list = context.Instance as PivotGridDataWebControlActionList;
			if(list != null)
				return list.Component as ASPxPivotGrid;
			return null;
		}
		protected override bool IsValidDataSource(IComponent component) {
			return component != pivotGrid && base.IsValidDataSource(component);
		}
	}
	public class PivotGridDesignerActionList : ASPxWebControlDesignerActionList {
		PivotGridControlDesigner designer;
		public PivotGridDesignerActionList(PivotGridControlDesigner designer)
			: base(designer) {
			this.designer = designer;
		}
		protected new PivotGridControlDesigner Designer {
			get { return designer; }
		}
		public override DesignerActionItemCollection GetSortedActionItems() {
			DesignerActionItemCollection collection = base.GetSortedActionItems();
			collection.Insert(0, new DesignerActionMethodItem(this, "EditGroups",
				  "Groups", StringResources.ActionList_MiscCategory, string.Empty, true));
			collection.Insert(0, new DesignerActionMethodItem(this, "EditFields",
				  "Fields", StringResources.ActionList_MiscCategory, string.Empty, true));
			return collection;
		}
		protected void EditFields() {
			Designer.EditFields();
		}
		protected void EditGroups() {
			Designer.EditGroups();
		}
	}
	public class PivotGridDataWebControlActionList : DataWebControlActionList {
		PivotGridControlDesigner designer;
		protected new PivotGridControlDesigner Designer { get { return designer; } }
		[Editor(typeof(DevExpress.Web.ASPxPivotGrid.Design.OLAPConnectionEditor), typeof(System.Drawing.Design.UITypeEditor))]
		public string OLAPConnectionString {
			get { return ((ASPxPivotGrid)Designer.Component).OLAPConnectionString; }
			set {
				ControlDesigner.InvokeTransactedChange(Designer.Component, new TransactedChangeCallback(SetOLAPConnectionStringCallback),
					value, "");
			}
		}
		[TypeConverter(typeof(PivotGridDataSourceIDConverter))]
		public override string DataSourceID {
			get { return base.DataSourceID; }
			set { base.DataSourceID = value; }
		}
		public PivotGridDataWebControlActionList(PivotGridControlDesigner designer) 
			: base(designer, designer.DataSourceDesigner) {
			this.designer = designer;
		}
		public override DesignerActionItemCollection GetSortedActionItems() {
			DesignerActionItemCollection result = base.GetSortedActionItems();
			result.Add(DesignUtils.CreateDesignerPropertyItem(Designer, "OLAPConnectionString", Designer.Component, StringResources.DataControl_ConfigureOLAPDataVerb));
			return result;
		}
		private bool SetOLAPConnectionStringCallback(object context) {
			((ASPxPivotGrid)Designer.Component).OLAPConnectionString = (string)context;
			return true;
		}
	}
}
