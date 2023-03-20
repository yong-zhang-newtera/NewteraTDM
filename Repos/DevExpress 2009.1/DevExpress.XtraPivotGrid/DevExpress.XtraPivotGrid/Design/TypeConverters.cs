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
using System.Data;
using System.ComponentModel.Design;
using System.Collections;
using System.Globalization;
using System.Reflection;
using System.ComponentModel.Design.Serialization;
using System.ComponentModel;
using System.Windows.Forms.Design;
using System.Windows.Forms;
using DevExpress.XtraPivotGrid;
using DevExpress.Data;
using DevExpress.Data.Helpers;
using DevExpress.Utils;
using DevExpress.Utils.Design;
using DevExpress.XtraPivotGrid.Data;
using DevExpress.Data.Browsing.Design;
namespace DevExpress.XtraPivotGrid.TypeConverters {
	public class PivotColumnNameEditor : ColumnNameEditor {
		protected override object GetDataSource(ITypeDescriptorContext context) {
			PivotGridViewInfoData data = GetViewInfoData(context);
			if(data == null) 
				return null;
			if(!String.IsNullOrEmpty(data.OLAPConnectionString))
				return data.GetDesignOLAPDataSourceObject();
			IDataContainerBase control = data.Control as IDataContainerBase;
			if(control == null)
				return null;
			return MasterDetailHelper.GetDataSource(new BindingContext(), control.DataSource, control.DataMember);
		}
		protected PivotGridViewInfoData GetViewInfoData(ITypeDescriptorContext context) {
			object instanceValue = DXObjectWrapper.GetInstance(context);
			if(instanceValue is IPivotGridViewInfoDataOwner)
				return ((IPivotGridViewInfoDataOwner)instanceValue).DataViewInfo;
			if(instanceValue is PivotGridFieldSortBySummaryInfo) {
				IPivotGridViewInfoDataOwner owner = ((PivotGridFieldSortBySummaryInfo)instanceValue).Owner as IPivotGridViewInfoDataOwner;
				if(owner != null) return owner.DataViewInfo;
			}
			return null;
		}
	}
	public class FieldReferenceConverter : System.ComponentModel.TypeConverter {
		protected virtual string None { get { return "(none)"; } }
		protected virtual string GetFieldName(PivotGridField field) {
			IComponent comp = field as IComponent;
			if(comp.Site != null)
				return comp.Site.Name;
			return "Field" + field.Index.ToString();
		}
		protected virtual PivotGridViewInfoData GetViewInfoData(ITypeDescriptorContext context) {
			object instanceValue = DXObjectWrapper.GetInstance(context);
			if(instanceValue is IPivotGridViewInfoDataOwner)
				return ((IPivotGridViewInfoDataOwner)instanceValue).DataViewInfo;
			return null;
		}
		public override object ConvertTo(ITypeDescriptorContext context, CultureInfo culture, object value, Type destinationType) {
			if(destinationType.Equals(typeof(string))) {
				if(value == null) return None;
				if(value is PivotGridField) {
					return GetFieldName(value as PivotGridField);
				}
			}
			return base.ConvertTo(context, culture, value, destinationType);
		}
		public override bool CanConvertFrom(ITypeDescriptorContext context, Type type) {
			if(GetViewInfoData(context) != null) {
				if(type != null && type.Equals(typeof(string))) {
					return true;
				}
			}
			return base.CanConvertFrom(context, type);
		}
		public override object ConvertFrom(ITypeDescriptorContext context, CultureInfo culture, object value) {
			if(value == null) return null;
			if(value is string) {
				string source = value.ToString();
				if(source == None) return null;
				PivotGridViewInfoData data = GetViewInfoData(context);
				if(data != null) {
					foreach(PivotGridField field in data.Fields) {
						if(source == GetFieldName(field)) return field;
					}
				}
			}
			return base.ConvertFrom(context, culture, value);
		}
		public override StandardValuesCollection GetStandardValues(ITypeDescriptorContext context) {
			if(context == null || context.Instance == null) return null;
			ArrayList array = new ArrayList();
			array.Add(null);
			PivotGridViewInfoData data = GetViewInfoData(context);
			if(data != null) {
				foreach(PivotGridField field in data.Fields) {
					array.Add(field);
				}
			}
			return new StandardValuesCollection(array);
		}
		public override bool GetStandardValuesSupported(ITypeDescriptorContext context) {
			return true;
		}
	}
	public class FieldEditConverter : ComponentConverter {
		public FieldEditConverter(Type t) : base(t) { }
		public override bool GetStandardValuesSupported(ITypeDescriptorContext context) {
			return false;
		}
	}
}
