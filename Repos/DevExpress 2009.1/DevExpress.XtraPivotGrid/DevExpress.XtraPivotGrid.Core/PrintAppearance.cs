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
using System.ComponentModel;
using DevExpress.Utils.Serializing;
using System.Drawing;
using System.Drawing.Drawing2D;
using DevExpress.Utils.Controls;
using DevExpress.Utils;
using System.Reflection;
using DevExpress.Utils.Serializing.Helpers;
using System.IO;
using System.IO.Compression;
using System.Text;
namespace DevExpress.XtraPivotGrid.Printing {
	public enum HorzAlignment {
		Default = 0,
		Near = 1,
		Center = 2,
		Far = 3,
	}
	[TypeConverter(typeof(ExpandableObjectConverter))]
	public class WebPrintAppearanceObject {
		public override string ToString() { return string.Empty; }
		#region Properties
		Color backColor;
		[Description("")]
		[NotifyParentProperty(true), XtraSerializableProperty]
		public Color BackColor { get { return backColor; } set { backColor = value; } }
		void ResetBackColor() { BackColor = Color.Empty; }
		bool ShouldSerializeBackColor() { return BackColor != Color.Empty; }
		Color backColor2;
		[Description("")]
		[NotifyParentProperty(true), XtraSerializableProperty]
		public Color BackColor2 { get { return backColor2; } set { backColor2 = value; } }
		void ResetBackColor2() { BackColor2 = Color.Empty; }
		bool ShouldSerializeBackColor2() { return BackColor2 != Color.Empty; }
		Color borderColor;
		[Description("")]
		[NotifyParentProperty(true), XtraSerializableProperty]
		public Color BorderColor { get { return borderColor; } set { borderColor = value; } }
		void ResetBorderColor() { BorderColor = Color.Empty; }
		bool ShouldSerializeBorderColor() { return BorderColor != Color.Empty; }
		static Font defaultFont = null;
		[Description(""), NotifyParentProperty(true)]
		public static Font DefaultFont {
			get {
				if(defaultFont == null) defaultFont = CreateDefaultFont();
				return defaultFont;
			}
			set {
				if(value == null) value = CreateDefaultFont();
				defaultFont = value;
			}
		}
		static Font CreateDefaultFont() {
			return new Font(new FontFamily("Tahoma"), SystemFonts.DefaultFont.Size);
		}
		Font font = null;
		[Description("")]
		[NotifyParentProperty(true), XtraSerializableProperty]
		[RefreshProperties(RefreshProperties.All)]
		public Font Font {
			get { return font; }
			set {
				if(value == null) value = DefaultFont;
				if(Font == value) return;
				font = value;
			}
		}
		void ResetFont() { Font = null; }
		bool ShouldSerializeFont() { return Font != null && !Font.Equals(DefaultFont); }
		Color foreColor;
		[Description("")]
		[NotifyParentProperty(true), XtraSerializableProperty]
		public Color ForeColor { get { return foreColor; } set { foreColor = value; } }
		void ResetForeColor() { ForeColor = Color.Empty; }
		bool ShouldSerializeForeColor() { return ForeColor != Color.Empty; }
		LinearGradientMode gradientMode = LinearGradientMode.Horizontal;
		[Description("")]
		[NotifyParentProperty(true), XtraSerializableProperty]
		[DefaultValue(LinearGradientMode.Horizontal)]
		public LinearGradientMode GradientMode { get { return gradientMode; } set { gradientMode = value; } }
		Image image = null;
		[Description("")]
		[NotifyParentProperty(true), DefaultValue(null)]
		[XtraSerializableProperty]
		public Image Image { get { return image; } set { image = value; } }
		#endregion
		#region Serialization
		public bool ShouldSerialize() {
			return ShouldSerializeBackColor() || ShouldSerializeBackColor2() || ShouldSerializeBorderColor() ||
				ShouldSerializeFont() || ShouldSerializeForeColor() || GradientMode != LinearGradientMode.Horizontal ||
				Image != null;
		}
		public void Reset() {
			BackColor = Color.Empty;
			BackColor2 = Color.Empty;
			BorderColor = Color.Empty;
			Font = null;
			ForeColor = Color.Empty;
			GradientMode = LinearGradientMode.Horizontal;
			Image = null;
		}
		#endregion
	}
	[TypeConverter(typeof(ExpandableObjectConverter))]
	public class WebPrintAppearance {
		WebPrintAppearanceObject Get(ref WebPrintAppearanceObject obj) {
			if(obj == null) obj = new WebPrintAppearanceObject();
			return obj;
		}
		void Set(ref WebPrintAppearanceObject cell, WebPrintAppearanceObject value) {
			if(value == null) cell = new WebPrintAppearanceObject();
			else cell = value;
		}
		public override string ToString() { return string.Empty; }
		WebPrintAppearanceObject cell;
		bool ShouldSerializeCell() { return Cell.ShouldSerialize(); }
		void ResetCell() { Cell.Reset(); }
		[Description(""),
		NotifyParentProperty(true), DesignerSerializationVisibility(DesignerSerializationVisibility.Content)]
		[XtraSerializableProperty(XtraSerializationVisibility.Content), AutoFormatEnable()]
		public WebPrintAppearanceObject Cell { get { return Get(ref cell); } set { Set(ref cell, value); } }		
		WebPrintAppearanceObject fieldHeader;
		bool ShouldSerializeFieldHeader() { return FieldHeader.ShouldSerialize(); }
		void ResetFieldHeader() { FieldHeader.Reset(); }
		[Description(""),
		NotifyParentProperty(true), DesignerSerializationVisibility(DesignerSerializationVisibility.Content)]
		[XtraSerializableProperty(XtraSerializationVisibility.Content), AutoFormatEnable()]
		public WebPrintAppearanceObject FieldHeader { get { return Get(ref fieldHeader); } set { Set(ref fieldHeader, value); } }
		WebPrintAppearanceObject totalCell;
		bool ShouldSerializeTotalCell() { return TotalCell.ShouldSerialize(); }
		void ResetTotalCell() { TotalCell.Reset(); }
		[Description(""),
		NotifyParentProperty(true), DesignerSerializationVisibility(DesignerSerializationVisibility.Content)]
		[XtraSerializableProperty(XtraSerializationVisibility.Content), AutoFormatEnable()]
		public WebPrintAppearanceObject TotalCell { get { return Get(ref totalCell); } set { Set(ref totalCell, value); } }
		WebPrintAppearanceObject grandTotalCell;
		bool ShouldSerializeGrandTotalCell() { return GrandTotalCell.ShouldSerialize(); }
		void ResetGrandTotalCell() { GrandTotalCell.Reset(); }
		[Description(""),
		NotifyParentProperty(true), DesignerSerializationVisibility(DesignerSerializationVisibility.Content)]
		[XtraSerializableProperty(XtraSerializationVisibility.Content), AutoFormatEnable()]
		public WebPrintAppearanceObject GrandTotalCell { get { return Get(ref grandTotalCell); } set { Set(ref grandTotalCell, value); } }
		WebPrintAppearanceObject customTotalCell;
		bool ShouldSerializeCustomTotalCell() { return CustomTotalCell.ShouldSerialize(); }
		void ResetCustomTotalCell() { CustomTotalCell.Reset(); }
		[Description(""),
		NotifyParentProperty(true), DesignerSerializationVisibility(DesignerSerializationVisibility.Content)]
		[XtraSerializableProperty(XtraSerializationVisibility.Content), AutoFormatEnable()]
		public WebPrintAppearanceObject CustomTotalCell { get { return Get(ref customTotalCell); } set { Set(ref customTotalCell, value); } }
		WebPrintAppearanceObject fieldValue;
		bool ShouldSerializeFieldValue() { return FieldValue.ShouldSerialize(); }
		void ResetFieldValue() { FieldValue.Reset(); }
		[Description(""),
		NotifyParentProperty(true), DesignerSerializationVisibility(DesignerSerializationVisibility.Content)]
		[XtraSerializableProperty(XtraSerializationVisibility.Content), AutoFormatEnable()]
		public WebPrintAppearanceObject FieldValue { get { return Get(ref fieldValue); } set { Set(ref fieldValue, value); } }
		WebPrintAppearanceObject fieldValueTotal;
		bool ShouldSerializeFieldValueTotal() { return FieldValueTotal.ShouldSerialize(); }
		void ResetFieldValueTotal() { FieldValueTotal.Reset(); }
		[Description(""),
		NotifyParentProperty(true), DesignerSerializationVisibility(DesignerSerializationVisibility.Content)]
		[XtraSerializableProperty(XtraSerializationVisibility.Content), AutoFormatEnable()]
		public WebPrintAppearanceObject FieldValueTotal { get { return Get(ref fieldValueTotal); } set { Set(ref fieldValueTotal, value); } }
		WebPrintAppearanceObject fieldValueGrandTotal;
		bool ShouldSerializeFieldValueGrandTotal() { return FieldValueGrandTotal.ShouldSerialize(); }
		void ResetFieldValueGrandTotal() { FieldValueGrandTotal.Reset(); }
		[Description(""),
		NotifyParentProperty(true), DesignerSerializationVisibility(DesignerSerializationVisibility.Content)]
		[XtraSerializableProperty(XtraSerializationVisibility.Content), AutoFormatEnable()]
		public WebPrintAppearanceObject FieldValueGrandTotal { get { return Get(ref fieldValueGrandTotal); } set { Set(ref fieldValueGrandTotal, value); } }
		WebPrintAppearanceObject lines;
		bool ShouldSerializeLines() { return Lines.ShouldSerialize(); }
		void ResetLines() { Lines.Reset(); }
		[Description(""),
		NotifyParentProperty(true), DesignerSerializationVisibility(DesignerSerializationVisibility.Content)]
		[XtraSerializableProperty(XtraSerializationVisibility.Content), AutoFormatEnable()]
		public WebPrintAppearanceObject Lines { get { return Get(ref lines); } set { Set(ref lines, value); } }
		WebPrintAppearanceObject filterSeparator;
		bool ShouldSerializeFilterSeparator() { return FilterSeparator.ShouldSerialize(); }
		void ResetFilterSeparator() { FilterSeparator.Reset(); }
		[Description(""),
		NotifyParentProperty(true), DesignerSerializationVisibility(DesignerSerializationVisibility.Content)]
		[XtraSerializableProperty(XtraSerializationVisibility.Content), AutoFormatEnable()]
		public WebPrintAppearanceObject FilterSeparator { get { return Get(ref filterSeparator); } set { Set(ref filterSeparator, value); } }
		WebPrintAppearanceObject headerGroupLine;
		bool ShouldSerializeHeaderGroupLine() { return HeaderGroupLine.ShouldSerialize(); }
		void ResetHeaderGroupLine() { HeaderGroupLine.Reset(); }
		[Description(""),
		NotifyParentProperty(true), DesignerSerializationVisibility(DesignerSerializationVisibility.Content)]
		[XtraSerializableProperty(XtraSerializationVisibility.Content), AutoFormatEnable()]
		public WebPrintAppearanceObject HeaderGroupLine { get { return Get(ref headerGroupLine); } set { Set(ref headerGroupLine, value); } }
		public void Reset() {
			MethodInfo[] methods = this.GetType().GetMethods(BindingFlags.NonPublic | BindingFlags.Instance);
			for(int i = 0; i < methods.Length; i++) {
				if(!methods[i].Name.StartsWith("Reset") || methods[i].Name == "Reset") continue;
				methods[i].Invoke(this, null);
			}
		}
		public bool ShouldSerialize() {
			MethodInfo[] methods = this.GetType().GetMethods(BindingFlags.NonPublic | BindingFlags.Instance);
			for(int i = 0; i < methods.Length; i++) {
				if(!methods[i].Name.StartsWith("ShouldSerialize") || methods[i].Name == "ShouldSerialize") continue;
				if((bool)methods[i].Invoke(this, null)) return true;
			}
			return false;
		}
	}
}
namespace DevExpress.XtraPivotGrid.Web {
	public class Base64XtraSerializer : BinaryXtraSerializer {
		bool resetProperties;
		public string Serialize(IXtraPropertyCollection properties) {
			using(MemoryStream stream = new MemoryStream()) {
				using(DeflateStream compressor = new DeflateStream(stream, CompressionMode.Compress, true)) {
					using(BufferedStream buffered = new BufferedStream(compressor)) {
						Serialize(buffered, properties, "ASPxPivotGrid");
					}
				}
				return Convert.ToBase64String(stream.ToArray());
			}
		}
		public void Deserialize(object obj, string base64String) {
			Deserialize(obj, base64String, true);
		}
		public void Deserialize(object obj, string base64String, bool resetProperties) {
			this.resetProperties = resetProperties;
			using(MemoryStream compressed = new MemoryStream(Convert.FromBase64String(base64String))) {
				using(DeflateStream decompressor = new DeflateStream(compressed, CompressionMode.Decompress)) {
					using(MemoryStream stream = new MemoryStream()) {
						byte[] buffer = new byte[0x1000];
						while(true) {
							int bytesRead = decompressor.Read(buffer, 0, buffer.Length);
							if(bytesRead == 0) break;
							stream.Write(buffer, 0, bytesRead);
						}
						stream.Position = 0;
						DeserializeObject(obj, stream, "ASPxPivotGrid");
					}
				}
			}
		}
		protected override void DeserializeObject(object obj, IXtraPropertyCollection store, OptionsLayoutBase options) {
			if(options == null)
				options = OptionsLayoutBase.FullLayout;
			if(store == null)
				return;
			XtraPropertyCollection coll = new XtraPropertyCollection();
			coll.AddRange(store);
			DeserializeHelper helper = new DeserializeHelper(obj, this.resetProperties);
			helper.DeserializeObject(obj, coll, options);
		}
	}
}
