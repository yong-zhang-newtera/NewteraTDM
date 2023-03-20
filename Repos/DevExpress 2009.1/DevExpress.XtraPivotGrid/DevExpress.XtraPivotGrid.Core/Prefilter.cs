#region Copyright (c) 2000-2009 Developer Express Inc.
/*
{*******************************************************************}
{                                                                   }
{       Developer Express .NET Component Library                    }
{                                        }
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
using System.Data;
using DevExpress.Data.Filtering;
using DevExpress.Utils.Serializing;
using DevExpress.Utils.Design;
namespace DevExpress.XtraPivotGrid {
	public interface IPrefilterOwnerBase {
		void CriteriaChanged();
	}
	public class PrefilterBase : IDisposable {
		CriteriaOperator criteria;
		bool enabled;
		[
		Description(""),
		DXDisplayName(typeof(DevExpress.XtraPivotGrid.ResFinder), DXDisplayNameAttribute.DefaultResourceFile, "DevExpress.XtraPivotGrid.PrefilterBase.Enabled"),
		XtraSerializableProperty(),
		DesignerSerializationVisibility(DesignerSerializationVisibility.Visible), DefaultValue(true),
		TypeConverter(typeof(BooleanTypeConverter)),
		NotifyParentProperty(true)
		]
		public bool Enabled {
			get { return enabled; }
			set {
				if(enabled == value) return;
				enabled = value;
				RaiseCriteriaChanged();
			}
		}
		[
		Description(""), XtraSerializableProperty(),
		DXDisplayName(typeof(DevExpress.XtraPivotGrid.ResFinder), DXDisplayNameAttribute.DefaultResourceFile, "DevExpress.XtraPivotGrid.PrefilterBase.Criteria"),
		DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden), DefaultValue(null),
		NotifyParentProperty(true)
		]
		public CriteriaOperator Criteria {
			get { return criteria; }
			set {
				if(object.Equals(criteria, value)) return;
				criteria = value;
				RaiseCriteriaChanged();
			}
		}
		[
		Description(""),
		Browsable(false), EditorBrowsable(EditorBrowsableState.Never),
		DefaultValue(""),
		DXDisplayName(typeof(DevExpress.XtraPivotGrid.ResFinder), DXDisplayNameAttribute.DefaultResourceFile, "DevExpress.XtraPivotGrid.PrefilterBase.CriteriaString"),
		NotifyParentProperty(true)
		]
		public string CriteriaString {
			get { return Convert.ToString(Criteria); }
			set { Criteria = CriteriaOperator.Parse(value); }
		}
		readonly IPrefilterOwnerBase owner;
		protected IPrefilterOwnerBase Owner { get { return owner; } }
		protected void RaiseCriteriaChanged() { 
			Owner.CriteriaChanged();
			if(CriteriaChanged != null) CriteriaChanged(this, new EventArgs());
		}
		[Browsable(false), EditorBrowsable(EditorBrowsableState.Never), Obsolete("Please use the PrefilterCriteriaChanged instead of this event")]
		public event EventHandler CriteriaChanged;
		[
		Browsable(false),
		Description("")
		]
		public bool IsEmpty {
			get {
				return ReferenceEquals(Criteria, null);
			}
		}
		public PrefilterBase(IPrefilterOwnerBase owner) {
			this.owner = owner;
			this.enabled = true;
		}
		public virtual void Dispose() {			
		}		
	}
	public class PrefilterPatcher : DevExpress.Data.Filtering.Helpers.EvaluatorCriteriaValidator {
		PivotGridFieldCollectionBase fields;
		public PrefilterPatcher(PivotGridFieldCollectionBase fields) : base(null) {
			this.fields = fields;
		}
		protected PivotGridFieldCollectionBase Fields { get { return fields; } }
		public override object Visit(OperandProperty theOperand) {
			PivotGridFieldBase field = Fields.GetFieldByName(theOperand.PropertyName);
			if(field != null)
				theOperand.PropertyName = field.DataControllerColumnName;
			return null;
		}		
		public CriteriaOperator Patch(CriteriaOperator criteria) {
			if(ReferenceEquals(criteria, null))
				return null;
			CriteriaOperator clone = (CriteriaOperator)((ICloneable)criteria).Clone();
			Validate(clone);
			return clone;
		}
	}
}
