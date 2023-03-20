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
using System.Collections;
using System.Collections.Generic;
using System.Text;
using System.Globalization;
using System.ComponentModel;
using System.Drawing.Design;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Web.UI.Design;
using DevExpress.Data;
using DevExpress.Web.ASPxClasses;
using DevExpress.Web.Data;
using DevExpress.Web.ASPxEditors;
using DevExpress.Web.ASPxEditors.Design;
namespace DevExpress.Web.ASPxGridView {
	[ControlBuilder(typeof(ControlBuilder))]
	public abstract class GridViewEditDataColumn : GridViewDataColumn {
		public static GridViewEditDataColumn CreateColumn(Type dataType) {
			if(dataType == null) return new GridViewDataTextColumn();
			Type underlying = Nullable.GetUnderlyingType(dataType);	
			if(underlying != null) dataType = underlying;
			if(dataType.Equals(typeof(DateTime))) return new GridViewDataDateColumn();
			if(dataType.Equals(typeof(bool))) return new GridViewDataCheckColumn();
			return new GridViewDataTextColumn();
		}
		[DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden),
		Browsable(false), Localizable(false), PersistenceMode(PersistenceMode.Attribute)]
		public override EditPropertiesBase PropertiesEdit {
			get {
				if(base.PropertiesEdit == null) base.PropertiesEdit = CreateEditProperties();
				return base.PropertiesEdit; 
			}
			set { }
		}
		[Description("This member supports the .NET Framework infrastructure and cannot be used directly from your code."), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		public override string PropertiesEditType {
			get { return string.Empty; }
			set { }
		}
		protected abstract EditPropertiesBase CreateEditProperties();
		protected override void AssignEditor(GridViewDataColumn source) {
			PropertiesEdit.Assign(source.PropertiesEdit);
		}
		protected override void OnPropertiesEditTypeChanged() { }
	}
	public class GridViewDataTextColumn : GridViewEditDataColumn {
		protected override EditPropertiesBase CreateEditProperties() { return new TextBoxProperties(); }
		[Description("Gets the column editor's settings."),
		Category("Behavior"), PersistenceMode(PersistenceMode.InnerProperty),
		DesignerSerializationVisibility(DesignerSerializationVisibility.Content), NotifyParentProperty(true)]
		public TextBoxProperties PropertiesTextEdit { get { return (TextBoxProperties)PropertiesEdit; } }
	}
	public class GridViewDataButtonEditColumn : GridViewEditDataColumn {
		protected override EditPropertiesBase CreateEditProperties() { return new ButtonEditProperties(); }
		[Description("Gets the column editor's settings."),
		Category("Behavior"), PersistenceMode(PersistenceMode.InnerProperty),
		DesignerSerializationVisibility(DesignerSerializationVisibility.Content), NotifyParentProperty(true)]
		public ButtonEditProperties PropertiesButtonEdit { get { return (ButtonEditProperties)PropertiesEdit; } }
	}
	public class GridViewDataMemoColumn : GridViewEditDataColumn {
		protected override EditPropertiesBase CreateEditProperties() { return new MemoProperties(); }
		[Description("Gets the column editor's settings."),
		Category("Behavior"), PersistenceMode(PersistenceMode.InnerProperty),
		DesignerSerializationVisibility(DesignerSerializationVisibility.Content), NotifyParentProperty(true)]
		public MemoProperties PropertiesMemoEdit { get { return (MemoProperties)PropertiesEdit; } }
	}
	public class GridViewDataHyperLinkColumn : GridViewEditDataColumn {
		protected override EditPropertiesBase CreateEditProperties() { return new HyperLinkProperties(); }
		[Description("Gets the column editor's settings."),
		Category("Behavior"), PersistenceMode(PersistenceMode.InnerProperty),
		DesignerSerializationVisibility(DesignerSerializationVisibility.Content), NotifyParentProperty(true)]
		public HyperLinkProperties PropertiesHyperLinkEdit { get { return (HyperLinkProperties)PropertiesEdit; } }
	}
	public class GridViewDataCheckColumn : GridViewEditDataColumn {
		protected override EditPropertiesBase CreateEditProperties() { return new CheckBoxProperties(); }
		[Description("Gets the column editor's settings."),
		Category("Behavior"), PersistenceMode(PersistenceMode.InnerProperty),
		DesignerSerializationVisibility(DesignerSerializationVisibility.Content), NotifyParentProperty(true)]
		public CheckBoxProperties PropertiesCheckEdit { get { return (CheckBoxProperties)PropertiesEdit; } }
	}
	public class GridViewDataDateColumn : GridViewEditDataColumn {
		protected override EditPropertiesBase CreateEditProperties() { return new DateEditProperties(); }
		[Description("Gets the column editor's settings."),
		Category("Behavior"), PersistenceMode(PersistenceMode.InnerProperty),
		DesignerSerializationVisibility(DesignerSerializationVisibility.Content), NotifyParentProperty(true)]
		public DateEditProperties PropertiesDateEdit { get { return (DateEditProperties)PropertiesEdit; } }
	}
	public class GridViewDataSpinEditColumn : GridViewEditDataColumn {
		protected override EditPropertiesBase CreateEditProperties() { return new SpinEditProperties(); }
		[Description("Gets the column editor's settings."),
		Category("Behavior"), PersistenceMode(PersistenceMode.InnerProperty),
		DesignerSerializationVisibility(DesignerSerializationVisibility.Content),
		NotifyParentProperty(true)]
		public SpinEditProperties PropertiesSpinEdit { get { return (SpinEditProperties)PropertiesEdit; } }
	}
	public class GridViewDataComboBoxColumn : GridViewEditDataColumn {
		protected override EditPropertiesBase CreateEditProperties() { return new ComboBoxProperties(); }
		[Description("Gets the column editor's settings."), 
		Category("Behavior"), PersistenceMode(PersistenceMode.InnerProperty),
		DesignerSerializationVisibility(DesignerSerializationVisibility.Content), NotifyParentProperty(true)]
		public ComboBoxProperties PropertiesComboBox { get { return (ComboBoxProperties)PropertiesEdit; } }
	}
	public class GridViewDataImageColumn : GridViewEditDataColumn {
		protected override EditPropertiesBase CreateEditProperties() { return new ImageEditProperties(); }
		[Description(""), 
		Category("Behavior"), PersistenceMode(PersistenceMode.InnerProperty),
		DesignerSerializationVisibility(DesignerSerializationVisibility.Content), NotifyParentProperty(true)]
		public ImageEditProperties PropertiesImage { get { return (ImageEditProperties)PropertiesEdit; } }
		protected internal override bool GetAllowSort() { return false; }
		protected internal override bool GetAllowGroup() { return false; }
		protected internal override bool GetAllowAutoFilter() { return false;  }
	}
	public class GridViewDataBinaryImageColumn : GridViewEditDataColumn {
		protected override EditPropertiesBase CreateEditProperties() { return new BinaryImageEditProperties(); }
		[Description("Gets the column editor's settings."),
		Category("Behavior"), PersistenceMode(PersistenceMode.InnerProperty),
		DesignerSerializationVisibility(DesignerSerializationVisibility.Content), NotifyParentProperty(true)]
		public BinaryImageEditProperties PropertiesBinaryImage { get { return (BinaryImageEditProperties)PropertiesEdit; } }
		protected internal override bool GetAllowSort() { return false; }
		protected internal override bool GetAllowGroup() { return false; }
		protected internal override bool GetAllowAutoFilter() { return false; }
	}
	public class GridViewDataProgressBarColumn : GridViewEditDataColumn {
		protected override EditPropertiesBase CreateEditProperties() { return new ProgressBarProperties(); }
		[Description("Gets the column editor's settings."),
		Category("Behavior"), PersistenceMode(PersistenceMode.InnerProperty),
		DesignerSerializationVisibility(DesignerSerializationVisibility.Content), NotifyParentProperty(true)]
		public ProgressBarProperties PropertiesProgressBar { get { return (ProgressBarProperties)PropertiesEdit; } }
	}
}
