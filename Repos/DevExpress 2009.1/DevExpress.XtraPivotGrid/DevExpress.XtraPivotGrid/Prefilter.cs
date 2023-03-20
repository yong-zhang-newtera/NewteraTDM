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
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using DevExpress.XtraEditors;
using DevExpress.XtraPivotGrid.Data;
using DevExpress.Data.Filtering;
using DevExpress.LookAndFeel;
using DevExpress.XtraEditors.Filtering;
using DevExpress.Utils.Drawing;
using DevExpress.XtraPivotGrid.ViewInfo;
using DevExpress.XtraEditors.Controls;
using DevExpress.XtraPivotGrid.Localization;
using System.Drawing.Design;
using DevExpress.Utils.Serializing;
using System.ComponentModel.Design;
namespace DevExpress.XtraPivotGrid {
	internal partial class PrefilterForm : XtraForm {
		public static readonly Point DefaultLocation = new Point(-10000, -10000);
		readonly IPrefilterFormOwner formOwner;
		protected IPrefilterFormOwner FormOwner { get { return formOwner; } }
		protected Control ControlOwner { get { return FormOwner.ControlOwner; } }
		protected CriteriaOperator Criteria {
			get { return filterControl.FilterCriteria; }
		}
		protected IFilteredComponent SourceControl {
			get { return filterControl.SourceControl as IFilteredComponent; }
			set { filterControl.SourceControl = value; }
		}
		public PrefilterForm(IPrefilterFormOwner owner) {
			if(owner == null) throw new Exception("PrefilterFormOwner cannot be null.");
			this.formOwner = owner;
			InitializeComponent();
			PivotGridControl pivot;
			if(owner != null && (pivot = owner.ControlOwner as PivotGridControl) != null)
				this.filterControl.MenuManager = pivot.MenuManager;
			Text = PivotGridLocalizer.GetString(PivotGridStringId.PrefilterFormCaption);
			Visible = false;
			RegisterAsOwnedForm(this.ControlOwner);
			btnOK.Text = Localizer.Active.GetLocalizedString(StringId.OK);
			btnCancel.Text = Localizer.Active.GetLocalizedString(StringId.Cancel);
			btnApply.Text = Localizer.Active.GetLocalizedString(StringId.Apply);
		}
		public DialogResult ShowPrefilterForm(Point location, bool showApplyButton) {
			filterControl.SourceControl = FormOwner.FilteredComponent;
			filterControl.FilterCriteria = FormOwner.FilteredComponent.RowCriteria;
			SetLocation(location, this.ControlOwner);
			ShowApplyButton(showApplyButton);
			CheckApplyButtonEnabled();
			return ShowDialog();
		}
		void ShowApplyButton(bool showApplyButton) {
			btnApply.Visible = showApplyButton;
			btnCancel.Left = showApplyButton ? btnApply.Left - btnCancel.Width - 6 : btnApply.Left;
			btnOK.Left = btnCancel.Left - btnOK.Width - 6;
		}
		void SetLocation(Point location, Control controlOwner) {
			if(!location.IsEmpty) {
				if(location == DefaultLocation) {
					if(controlOwner != null && controlOwner.FindForm() != null) {
						Form parentForm = controlOwner.FindForm();
						location = parentForm.PointToScreen(new Point(parentForm.ClientRectangle.Left, parentForm.ClientRectangle.Top));
						location.Offset((parentForm.ClientSize.Width - this.Width) / 2, (parentForm.ClientSize.Height - this.Height) / 2);
					} else
						location = new Point((Screen.PrimaryScreen.Bounds.Width - this.Width) / 2,
							(Screen.PrimaryScreen.Bounds.Height - this.Height) / 2);
				}
				Location = location;
			}
		}
		void RegisterAsOwnedForm(Control controlOwner) {
			if(controlOwner != null && controlOwner.FindForm() != null) {
				if(controlOwner.FindForm().MdiParent != null)
					controlOwner.FindForm().MdiParent.AddOwnedForm(this);
				else
					controlOwner.FindForm().AddOwnedForm(this);
			}
		}
		void ApplyCriteria() {
			SourceControl.RowCriteria = Criteria;
			CheckApplyButtonEnabled();
		}
		void CheckApplyButtonEnabled() {
			btnApply.Enabled = !object.Equals(Criteria, SourceControl.RowCriteria);
		}
		void btnApply_Click(object sender, EventArgs e) {
			ApplyCriteria();
		}
		void btnOK_Click(object sender, EventArgs e) {
			ApplyCriteria();
			DialogResult = DialogResult.OK;
			Close();
		}
		void btnCancel_Click(object sender, EventArgs e) {
			DialogResult = DialogResult.Cancel;
			Close();			
		}
		void filterControl_FilterChanged(object sender, FilterChangedEventArgs e) {
			CheckApplyButtonEnabled();
		}
		void PrefilterForm_KeyDown(object sender, KeyEventArgs e) {
			switch(e.KeyCode) {
				case Keys.Escape:
					Close();
					break;
			}
		}
	}
	public interface IPrefilterFormOwner {
		Control ControlOwner { get; }
		IFilteredComponent FilteredComponent { get; }
	}
	public interface IPrefilterOwner : IPrefilterOwnerBase {
		Control ControlOwner { get; }
		UserLookAndFeel ActiveLookAndFeel { get; }
		IFilteredComponent FilteredComponent { get; }
	}
	[TypeConverter(typeof(ExpandableObjectConverter))]
	public class Prefilter : PrefilterBase, IPrefilterFormOwner {
		PrefilterForm fPrefilterForm;
		Rectangle fPrefilterFormBounds;
		protected internal new IPrefilterOwner Owner { get { return (IPrefilterOwner)base.Owner; } }
		public Prefilter(IPrefilterOwner owner)
			: base(owner) {
		}
		public override void Dispose() {
			fPrefilterForm.Dispose();
		}
		[Description("Gets or sets the Prefilter's expression."), DefaultValue(null),
		Editor(typeof(CriteriaEditor), typeof(UITypeEditor)), NotifyParentProperty(true), XtraSerializableProperty(),
		DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden),
		DXDisplayName(typeof(DevExpress.XtraPivotGrid.ResFinder), DXDisplayNameAttribute.DefaultResourceFile, "DevExpress.XtraPivotGrid.Prefilter.Criteria"),
		]
		public new CriteriaOperator Criteria {
			get { return base.Criteria; }
			set { base.Criteria = value; }
		}
		[Description("")]
		public void ChangePrefilterVisible() {
			if(IsPrefilterFormShowing) DestroyPrefilterForm();
			else ShowPrefilterForm(true);
		}
		[Description("Gets a value indicating whether the Prefilter editor is currently displayed."), 
		Browsable(false)]
		public bool IsPrefilterFormShowing { get { return fPrefilterForm != null; } }
		internal DialogResult ShowPrefilterForm(bool showApplyButton) {
			fPrefilterForm = new PrefilterForm(this);
			fPrefilterForm.LookAndFeel.ParentLookAndFeel = Owner.ActiveLookAndFeel;
			if(!fPrefilterFormBounds.IsEmpty) fPrefilterForm.Bounds = fPrefilterFormBounds;
			DialogResult result = fPrefilterForm.ShowPrefilterForm(PrefilterForm.DefaultLocation, showApplyButton);
			DestroyPrefilterForm();
			return result;
		}
		void DestroyPrefilterForm() {
			if(IsPrefilterFormShowing) {
				fPrefilterFormBounds = fPrefilterForm.Bounds;
				fPrefilterForm.Dispose();
				fPrefilterForm = null;
			}
		}
		#region IPrefilterFormOwner Members
		Control IPrefilterFormOwner.ControlOwner {
			get { return Owner.ControlOwner; }
		}
		IFilteredComponent IPrefilterFormOwner.FilteredComponent {
			get { return Owner.FilteredComponent; }
		}
		#endregion
		public override string ToString() {
			return object.ReferenceEquals(Criteria, null) ? string.Empty : Criteria.ToString();
		}
		[EditorBrowsable(EditorBrowsableState.Never)]
		public void Reset() {
			DestroyPrefilterForm();
			Enabled = true;
			Criteria = null;			
		}
		[EditorBrowsable(EditorBrowsableState.Never)]
		public bool ShouldSerialize() {
			return Enabled != true || !object.ReferenceEquals(Criteria, null);
		}
	}
	public class CriteriaEditor : UITypeEditor {
		public override UITypeEditorEditStyle GetEditStyle(ITypeDescriptorContext context) {
			return UITypeEditorEditStyle.Modal;
		}
		public override object EditValue(ITypeDescriptorContext context, IServiceProvider provider, object value) {
			if(context.Instance is Prefilter) {
				Prefilter prefilter = (Prefilter)context.Instance;
				if(prefilter.ShowPrefilterForm(false) == DialogResult.OK) {
					PropertyDescriptor criteriaDescriptor = TypeDescriptor.GetProperties(prefilter)["Criteria"];
					IComponentChangeService changeService = (IComponentChangeService)provider.GetService(typeof(IComponentChangeService));
					changeService.OnComponentChanging(prefilter, criteriaDescriptor);
					changeService.OnComponentChanged(prefilter, criteriaDescriptor, value, prefilter.Criteria);
					return prefilter.Criteria;
				} else
					return value;
			}
			return base.EditValue(context, provider, value);
		}
	}
}
