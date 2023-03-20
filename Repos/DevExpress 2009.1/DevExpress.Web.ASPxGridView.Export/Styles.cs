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
using System.ComponentModel;
using System.Drawing;
using DevExpress.Web.ASPxClasses;
using DevExpress.Web.ASPxGridView;
using System.Web.UI;
using DevExpress.Web.ASPxClasses.Internal;
using DevExpress.XtraPrinting;
using System.Web.UI.WebControls;
namespace DevExpress.Web.ASPxGridView.Export {
	public class GridViewExportAppearance : AppearanceStyle {
		[Browsable(false), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden),
		EditorBrowsable(EditorBrowsableState.Never)]
		public override AppearanceSelectedStyle HoverStyle { get { return base.HoverStyle; } }
		[Browsable(false), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden),
		EditorBrowsable(EditorBrowsableState.Never)]
		public override BackgroundImage BackgroundImage { get { return base.BackgroundImage; } }
		[Browsable(false), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden),
		EditorBrowsable(EditorBrowsableState.Never)]
		public override BorderWrapper Border { get { return base.Border; } }
		[Browsable(false), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden),
		EditorBrowsable(EditorBrowsableState.Never)]
		public override Border BorderBottom { get { return base.BorderBottom; } }
		[Browsable(false), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden),
		EditorBrowsable(EditorBrowsableState.Never)]
		public override Border BorderTop { get { return base.BorderTop; } }
		[Browsable(false), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden),
		EditorBrowsable(EditorBrowsableState.Never)]
		public override Border BorderLeft { get { return base.BorderLeft; } }
		[Browsable(false), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden),
		EditorBrowsable(EditorBrowsableState.Never)]
		public override Border BorderRight { get { return base.BorderRight; } }
		[Browsable(false), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden),
		EditorBrowsable(EditorBrowsableState.Never)]
		public override string Cursor { get { return base.Cursor; } set { base.Cursor = value; } }
		[Browsable(false), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden),
		EditorBrowsable(EditorBrowsableState.Never)]
		public new string CssClass { get { return base.CssClass; } set { } }
		[Browsable(false), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden),
		EditorBrowsable(EditorBrowsableState.Never)]
		public override Unit ImageSpacing { get { return base.ImageSpacing; } set { } }
		[Browsable(false), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden),
		EditorBrowsable(EditorBrowsableState.Never)]
		public override Unit Spacing { get { return base.Spacing; } set { } }
		[Description("This property is not in effect."),
		NotifyParentProperty(true), DefaultValue(1)]
		public new int BorderWidth {
			get { return ViewStateUtils.GetIntProperty(ViewState, "BorderWidth", 1); }
			set {
				if(value < 0) value = 0;
				ViewStateUtils.SetIntProperty(ViewState, "BorderWidth", 1, value);
			}
		}
		[Description("Gets or sets the border color."),
		NotifyParentProperty(true)]
		public new Color BorderColor {
			get { return Border.BorderColor; }
			set {
				Border.BorderColor = value;
			}
		}
		[Description("Gets or sets a value that specifies which border sides are to be drawn."),
		NotifyParentProperty(true), DefaultValue(BorderSide.All)]
		public BorderSide BorderSides {
			get { return (BorderSide)ViewStateUtils.GetIntProperty(ViewState, "BorderSides", (int)BorderSide.All); }
			set {
				ViewStateUtils.SetIntProperty(ViewState, "BorderSides", (int)BorderSide.All, (int)value);
			}
		}
		public override void CopyFrom(System.Web.UI.WebControls.Style style) {
			base.CopyFrom(style);
			if(style is GridViewExportAppearance) {
				GridViewExportAppearance exportStyle = style as GridViewExportAppearance;
				BorderWidth = exportStyle.BorderWidth;
				BorderSides = exportStyle.BorderSides;
				if(!exportStyle.BorderColor.Equals(Color.Empty)) {
					BorderColor = exportStyle.BorderColor;
				}
			}
		}
		public override void MergeWith(System.Web.UI.WebControls.Style style) {
			base.MergeWith(style);
			if(style is GridViewExportAppearance) {
				GridViewExportAppearance exportStyle = style as GridViewExportAppearance;
				BorderWidth = exportStyle.BorderWidth;
				BorderSides = exportStyle.BorderSides;
				if(BorderColor.Equals(Color.Empty)) {
					BorderColor = exportStyle.BorderColor;
				}
			}
		}
	}
	public class GridViewExportOptionalAppearance : GridViewExportAppearance {
		[NotifyParentProperty(true), DefaultValue(DefaultBoolean.Default)]
		public DefaultBoolean Enabled {
			get { return (DefaultBoolean)ViewStateUtils.GetEnumProperty(ViewState, "Enabled", DefaultBoolean.Default); }
			set { ViewStateUtils.SetEnumProperty(ViewState, "Enabled", DefaultBoolean.Default, value); }
		}
		public override void CopyFrom(Style style) {
			base.CopyFrom(style);
			GridViewExportOptionalAppearance exportStyle = style as GridViewExportOptionalAppearance;
			if(exportStyle == null) return;
			Enabled = exportStyle.Enabled;
		}
		public override void MergeWith(Style style) {
			base.MergeWith(style);
			GridViewExportOptionalAppearance exportStyle = style as GridViewExportOptionalAppearance;
			if(exportStyle == null) return;
			if(Enabled == DefaultBoolean.Default)
				Enabled = exportStyle.Enabled;
		}
	}
	public class GridViewExportStyles : PropertiesBase {
		GridViewExportAppearance header, cell, footer, groupFooter, groupRow, preview, _default, title, hyperLink;
		GridViewExportOptionalAppearance altCell;
		public GridViewExportStyles(IPropertiesOwner owner) : base(owner) {
			this._default = new GridViewExportAppearance();
			this.header = new GridViewExportAppearance();
			this.cell = new GridViewExportAppearance();
			this.footer = new GridViewExportAppearance();
			this.groupFooter = new GridViewExportAppearance();
			this.groupRow = new GridViewExportAppearance();
			this.preview = new GridViewExportAppearance();
			this.title = new GridViewExportAppearance();
			this.hyperLink = new GridViewExportAppearance();
			this.altCell = new GridViewExportOptionalAppearance();
		}
		[Description("Gets the default appearance settings."),
		PersistenceMode(PersistenceMode.InnerProperty), NotifyParentProperty(true)]
		public GridViewExportAppearance Default { get { return _default; } }
		[Description("Gets the appearance settings used to paint column headers."),
		PersistenceMode(PersistenceMode.InnerProperty), NotifyParentProperty(true)]
		public GridViewExportAppearance Header { get { return header; } }
		[Description("Gets the appearance settings used to paint data cells when the grid is exported."),
		PersistenceMode(PersistenceMode.InnerProperty), NotifyParentProperty(true)]
		public GridViewExportAppearance Cell { get { return cell; } }
		[Description("Gets the appearance settings used to paint the Footer."),
		PersistenceMode(PersistenceMode.InnerProperty), NotifyParentProperty(true)]
		public GridViewExportAppearance Footer { get { return footer; } }
		[Description("Gets the appearance settings used to paint group footers."),
		PersistenceMode(PersistenceMode.InnerProperty), NotifyParentProperty(true)]
		public GridViewExportAppearance GroupFooter { get { return groupFooter; } }
		[Description("Gets the appearance settings used to paint group rows."),
		PersistenceMode(PersistenceMode.InnerProperty), NotifyParentProperty(true)]
		public GridViewExportAppearance GroupRow { get { return groupRow; } }
		[Description("Gets the appearance settings used to paint preview rows."),
		PersistenceMode(PersistenceMode.InnerProperty), NotifyParentProperty(true)]
		public GridViewExportAppearance Preview { get { return preview; } }
		[Description("Gets the appearance settings used to paint the grid's Title Panel."),
		PersistenceMode(PersistenceMode.InnerProperty), NotifyParentProperty(true)]
		public GridViewExportAppearance Title { get { return title; } }
		[Description("Gets the appearance settings used to paint hyper links."),
		PersistenceMode(PersistenceMode.InnerProperty), NotifyParentProperty(true)]
		public GridViewExportAppearance HyperLink { get { return hyperLink; } }
		[Description("Gets the style settings used to paint Alternating Data Row."),
		PersistenceMode(PersistenceMode.InnerProperty), NotifyParentProperty(true)]
		public GridViewExportOptionalAppearance AlternatingRowCell { get { return altCell; } }
		[Obsolete]
		public void CopyFrom(GridViewExportStyles styles) {
			Default.CopyFrom(styles.Default);
			Cell.CopyFrom(styles.Cell);
			Header.CopyFrom(styles.Header);
			GroupRow.CopyFrom(styles.GroupRow);
			Preview.CopyFrom(styles.Preview);
			Footer.CopyFrom(styles.Footer);
			GroupFooter.CopyFrom(styles.GroupFooter);
			Title.CopyFrom(styles.Title);
			HyperLink.CopyFrom(styles.HyperLink);
			AlternatingRowCell.CopyFrom(styles.AlternatingRowCell);
		}
		[Obsolete]
		public void MergeWithDefault() {
			Cell.MergeWith(Default);
			Header.MergeWith(Default);
			GroupRow.MergeWith(Default);
			Preview.MergeWith(Default);
			Footer.MergeWith(Default);
			GroupFooter.MergeWith(Default);
			Title.MergeWith(Default);
			HyperLink.MergeWith(Default);
			AlternatingRowCell.MergeWith(Default);
		}
	}
}
