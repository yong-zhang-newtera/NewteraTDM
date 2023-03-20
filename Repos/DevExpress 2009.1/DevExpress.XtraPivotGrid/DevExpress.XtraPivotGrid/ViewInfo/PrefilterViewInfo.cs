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
using DevExpress.Utils;
using System.Drawing;
using DevExpress.Utils.Drawing;
using DevExpress.XtraEditors.Drawing;
using DevExpress.XtraEditors.Controls;
using DevExpress.XtraPivotGrid.Data;
using System.Windows.Forms;
using DevExpress.XtraPivotGrid.Localization;
using DevExpress.Data.Filtering;
using DevExpress.Data.Filtering.Helpers;
using System.Collections.Generic;
using DevExpress.XtraEditors.Filtering;
namespace DevExpress.XtraPivotGrid.ViewInfo {
	public class PivotPrefilterPanelViewInfo : PivotPrefilterPanelViewInfoBase {
		StyleObjectInfoArgs activeButton;
		protected StyleObjectInfoArgs ActiveButton { get { return activeButton; } set { activeButton = value; } }
		public PivotPrefilterPanelViewInfo(PivotGridViewInfoBase viewInfo)
			: base(viewInfo) {
		}
		protected bool PtInButton(StyleObjectInfoArgs button, int x, int y) {
			Rectangle bounds = button is PrefilterCheckBox ? ((PrefilterCheckBox)button).GlyphRect : button.Bounds;
			return bounds.Contains(x, y);
		}
		protected override void MouseMoveCore(MouseEventArgs e) {
			for(int i = 0; i < Buttons.Length; i++)
				ButtonMouseMove(Buttons[i], e);
			base.MouseMoveCore(e);
		}
		private void ButtonMouseMove(StyleObjectInfoArgs button, MouseEventArgs e) {
			if(PtInButton(button, e.X, e.Y)) {
				if(button.State == ObjectState.Normal)
					button.State = ObjectState.Hot;
			} else {
				if(button.State != ObjectState.Normal)
					button.State = ObjectState.Normal;
			}
		}
		protected override BaseViewInfo MouseDownCore(System.Windows.Forms.MouseEventArgs e) {
			for(int i = 0; i < Buttons.Length; i++)
				if(PtInButton(Buttons[i], e.X, e.Y)) {
					activeButton = Buttons[i];
					Buttons[i].State = ObjectState.Pressed;
				}
			return this;
		}
		protected override void MouseUpCore(System.Windows.Forms.MouseEventArgs e) {
			if(ActiveButton != null && PtInButton(ActiveButton, e.X, e.Y)) {
				ActiveButton.State = ObjectState.Hot;
				ActiveButton = null;
			}
			base.MouseUpCore(e);
		}
		protected override void MouseLeaveCore() {
			for(int i = 0; i < Buttons.Length; i++)
				Buttons[i].State = ObjectState.Normal;
			base.MouseLeaveCore();
		}
	}
	public class PivotPrefilterPanelViewInfoBase : PivotViewInfo {
		const int Indent = 3;
		readonly EditorButtonPainter buttonPainter;
		readonly CheckObjectPainter checkBoxPainter;
		readonly StyleObjectInfoArgs[] buttons;
		readonly FooterPanelInfoArgs backgroundArgs;
		protected Prefilter Prefilter { get { return Data.Prefilter; } }
		protected AppearanceObject PaintAppearance { get { return Data.PaintAppearance.PrefilterPanel; } }
		protected EditorButtonPainter ButtonPainter { get { return buttonPainter; } }
		protected CheckObjectPainter CheckBoxPainter { get { return checkBoxPainter; } }
		protected FooterPanelPainter PanelPainter { get { return Data.ActiveLookAndFeel.Painter.FooterPanel; } }
		protected StyleObjectInfoArgs[] Buttons { get { return buttons; } }
		protected PrefilterButton CloseButtonArgs { get { return (PrefilterButton)buttons[0]; } }
		protected PrefilterButton EditButtonArgs { get { return (PrefilterButton)buttons[1]; } }
		protected PrefilterCheckBox EnabledCheckBox { get { return (PrefilterCheckBox)buttons[2]; } }
		protected int ButtonHeight { get { return Bounds.Height - 2 * Indent; } }
		protected FooterPanelInfoArgs BackgroundArgs { get { return backgroundArgs; } }		
		public PivotPrefilterPanelViewInfoBase(PivotGridViewInfoBase viewInfo)
			: base(viewInfo) {
			this.buttons = new StyleObjectInfoArgs[3];
			this.buttons[0] = new PrefilterButton(ButtonPredefines.Close, this);			
			this.buttons[1] = new PrefilterButton(ButtonPredefines.Glyph, this);
			this.buttons[2] = new PrefilterCheckBox(this);
			this.backgroundArgs = new FooterPanelInfoArgs(null, 1, 1);
			this.buttonPainter = EditorButtonHelper.GetPainter(BorderStyles.Default, Data.ActiveLookAndFeel);
			this.checkBoxPainter = CheckPainterHelper.GetPainter(Data.ActiveLookAndFeel);
			CloseButtonArgs.Click += CloseButtonClick;
			EditButtonArgs.Click += EditButtonClick;
			EnabledCheckBox.Changed += EnabledCheckBoxChanged;
			EnabledCheckBox.SetCheckState(Prefilter.Enabled);
		}
		protected override Rectangle CalculatePaintBounds() {
			PivotGridViewInfoBase rootViewInfo = Root as PivotGridViewInfoBase;
			if(rootViewInfo != null)
				return CalculateBounds(rootViewInfo.ScrollableRectangle);
			else
				return base.CalculatePaintBounds();
		}
		public override Rectangle ControlBounds { get { return Bounds; }}
		public virtual Rectangle CalculateBounds(Rectangle scrollableRectangle) {
			Rectangle result = new Rectangle();
			result.X = scrollableRectangle.Left;
			result.Y = scrollableRectangle.Bottom;
			result.Width = scrollableRectangle.Width;
			result.Height = PivotPrefilterPanelViewInfoBase.CalcHeight(Data);
			return result;
		}
		protected override void InternalClear() {
			CloseButtonArgs.Click -= CloseButtonClick;
			EditButtonArgs.Click -= EditButtonClick;
			EnabledCheckBox.Changed -= EnabledCheckBoxChanged;
			base.InternalClear();
		}		
		protected override void InternalPaint(ViewInfoPaintArgs e) {
			DrawBackground(e);
			DrawCloseButton(e);
			DrawEditButton(e);
			DrawEnabledCheckBox(e);
		}	
		void DrawBackground(ViewInfoPaintArgs e) {
			BackgroundArgs.Bounds = PaintBounds;
			BackgroundArgs.Cache = e.GraphicsCache;
			BackgroundArgs.CellHeight = PaintBounds.Height;
			PanelPainter.DrawObject(BackgroundArgs);
		}
		void DrawCloseButton(ViewInfoPaintArgs e) {
			CloseButtonArgs.Cache = e.GraphicsCache;
			CloseButtonArgs.Bounds = GetCloseButtonBounds(e);
			ObjectPainter.DrawObject(e.GraphicsCache, ButtonPainter, CloseButtonArgs);
		}		
		void DrawEnabledCheckBox(ViewInfoPaintArgs e) {
			EnabledCheckBox.Appearance.TextOptions.Trimming = Trimming.EllipsisCharacter;
			EnabledCheckBox.Cache = e.GraphicsCache;
			EnabledCheckBox.Caption = !ReferenceEquals(Prefilter.Criteria, null) ? GetCriteriaCaption(Prefilter.Criteria) : "None";
			EnabledCheckBox.GlyphRect = GetCheckBoxBounds(e);
			EnabledCheckBox.CaptionRect = GetTextBounds(e);
			ObjectPainter.DrawObject(e.GraphicsCache, CheckBoxPainter, EnabledCheckBox);
		}
		string GetCriteriaCaption(CriteriaOperator criteria) {
			return PivotCriteriaProcessor.ToString(criteria, Data);
		}
		void DrawEditButton(ViewInfoPaintArgs e) {
			EditButtonArgs.Cache = e.GraphicsCache;
			EditButtonArgs.Button.Caption = PivotGridLocalizer.GetString(PivotGridStringId.EditPrefilter);
			Size minSize = CalcMinEditButtonSize(Data, e.Graphics);
			EditButtonArgs.Bounds = new Rectangle(PaintBounds.Right - Indent - minSize.Width, PaintBounds.Top + (PaintBounds.Height - minSize.Height) / 2, minSize.Width, minSize.Height);
			ObjectPainter.DrawObject(e.GraphicsCache, ButtonPainter, EditButtonArgs);
		}				
		protected Rectangle GetCloseButtonBounds(ViewInfoPaintArgs e) {
			return new Rectangle(PaintBounds.Left + Indent, PaintBounds.Top + (PaintBounds.Height - ButtonHeight) / 2, ButtonHeight, ButtonHeight);
		}
		protected Rectangle GetCheckBoxBounds(ViewInfoPaintArgs e) {
			int size = ObjectPainter.CalcObjectMinBounds(e.Graphics, CheckBoxPainter, EnabledCheckBox).Height;
			return new Rectangle(2 * Indent + ButtonHeight, PaintBounds.Top + (PaintBounds.Height - size) / 2, size, size);
		}
		protected Rectangle GetTextBounds(ViewInfoPaintArgs e) {
			Rectangle bounds = PaintBounds;
			bounds.X += GetCheckBoxBounds(e).Right;
			bounds.Width = EditButtonArgs.Bounds.Left - bounds.X;
			return bounds;
		}
		public static int CalcHeight(PivotGridViewInfoData data) {
			FooterCellInfoArgs cellInfo = new FooterCellInfoArgs();
			cellInfo.SetAppearance(data.PaintAppearance.PrefilterPanel);
			cellInfo.DisplayText = "Qq";
			cellInfo.Bounds = new Rectangle(0, 0, int.MaxValue, int.MaxValue);
			Graphics graphics = GraphicsInfo.Default.AddGraphics(null);
			GraphicsCache graphicsCache = new GraphicsCache(graphics);
			int height = 0;
			try {
				cellInfo.Cache = graphicsCache;
				height = Math.Max(height, data.ActiveLookAndFeel.Painter.FooterCell.CalcObjectMinBounds(cellInfo).Height);
				height = Math.Max(height, CalcMinCloseButtonHeight(data, graphics));
				height = Math.Max(height, CalcMinEditButtonSize(data, graphics).Height);
			} finally {
				graphicsCache.Dispose();
				GraphicsInfo.Default.ReleaseGraphics();
			}
			return height + 2 * Indent;
		}
		static int CalcMinCloseButtonHeight(PivotGridViewInfoData data, Graphics graphics) {
			EditorButtonObjectInfoArgs closeButtonArgs = new EditorButtonObjectInfoArgs(new EditorButton(ButtonPredefines.Close), null);
			closeButtonArgs.Graphics = graphics;
			EditorButtonPainter painter = EditorButtonHelper.GetPainter(BorderStyles.Default, data.ActiveLookAndFeel);
			return painter.CalcKindMinSize(closeButtonArgs).Height;
		}
		static Size CalcMinEditButtonSize(PivotGridViewInfoData data, Graphics graphics) {
			EditorButtonObjectInfoArgs editButtonArgs = new EditorButtonObjectInfoArgs(new EditorButton(ButtonPredefines.Glyph), null);
			editButtonArgs.Graphics = graphics;
			editButtonArgs.Button.Caption = PivotGridLocalizer.GetString(PivotGridStringId.EditPrefilter);
			EditorButtonPainter painter = EditorButtonHelper.GetPainter(BorderStyles.Default, data.ActiveLookAndFeel);
			return painter.CalcObjectMinBounds(editButtonArgs).Size;
		}
		protected override void Invalidate(Rectangle bounds) {
			Data.Invalidate(bounds);
		}
		protected void CloseButtonClick(object sender, EventArgs args) {
			Prefilter.Criteria = null;
		}
		protected void EditButtonClick(object sender, EventArgs args) {
			Data.ChangePrefilterVisible();
		}
		protected void EnabledCheckBoxChanged(object sender, EventArgs args) {
			Prefilter.Enabled = EnabledCheckBox.CheckState == CheckState.Checked;
		}
	}
	class PivotCriteriaProcessor : LocalaizableCriteriaToStringProcessorCore {
		public readonly Localizer Localizer;
		static PivotCriteriaProcessor instance = null;
		protected static PivotCriteriaProcessor Instance {
			get {
				if(instance == null || instance.Localizer != Localizer.Active)
					instance = new PivotCriteriaProcessor();
				return instance;
			}
		}
		PivotGridField field;
		PivotGridData data;
		protected PivotGridField Field { get { return field; } set { field = value; } }
		protected PivotGridData Data { get { return data; } set { data = value; } }
		public PivotCriteriaProcessor()
			: base(new LocalaizableCriteriaToStringProcessor.LocalaizableCriteriaToStringProcessorLocalizerWrapper(DevExpress.XtraEditors.Controls.Localizer.Active)) {
			this.Localizer = ((LocalaizableCriteriaToStringProcessor.LocalaizableCriteriaToStringProcessorLocalizerWrapper)OpNamesSource).Localizer;
		}
		public override object Visit(BinaryOperator operand) {
			PivotGridField oldField = Field;
			Field = GetField(operand.LeftOperand as OperandProperty);
			object result = base.Visit(operand);
			Field = oldField;
			return result;
		}
		public override object Visit(BetweenOperator operand) {
			PivotGridField oldField = Field;
			Field = GetField(operand.TestExpression as OperandProperty);
			object result = base.Visit(operand);
			Field = oldField;
			return result;
		}
		public override object Visit(InOperator operand) {
			PivotGridField oldField = Field;
			Field = GetField(operand.LeftOperand as OperandProperty);
			object result = base.Visit(operand);
			Field = oldField;
			return result;
		}
		public override object Visit(OperandValue operand) {
			if(Field != null) {
				string displayText = Field.GetDisplayText(operand.Value);
				if(Field.GroupInterval == PivotGroupInterval.Numeric)
					displayText = "\"" + displayText + "\"";
				return new CriteriaToStringVisitResult(displayText);
			}
			return base.Visit(operand);
		}
		public override object Visit(OperandProperty operand) {
			PivotGridField field = GetField(operand);
			if(field != null)
				return new CriteriaToStringVisitResult("[" + field.ToString() + "]");
			return base.Visit(operand);
		}
		protected CriteriaToStringVisitResult Process(CriteriaOperator operand, PivotGridData data) {
			this.field = null;
			this.data = data;
			return base.Process(operand);
		}
		protected PivotGridField GetField(OperandProperty fieldProperty) {
			if(ReferenceEquals(fieldProperty, null)) return null;
			PivotGridFieldBase field = Data.Fields.GetFieldByName(fieldProperty.PropertyName);
			if(field == null)
				field = Data.GetFieldByFieldNameOrUnboundFieldName(fieldProperty.PropertyName);
			return (PivotGridField)field;
		}
		public static string ToString(CriteriaOperator operand, PivotGridData data) {
			if(ReferenceEquals(operand, null))
				return string.Empty;
			return Instance.Process(operand, data).Result;
		}
	}
	public class PrefilterButton : EditorButtonObjectInfoArgs {
		readonly PivotPrefilterPanelViewInfoBase owner;
		protected PivotPrefilterPanelViewInfoBase Owner { get { return owner; } }
		public event EventHandler Click;
		public PrefilterButton(ButtonPredefines kind, PivotPrefilterPanelViewInfoBase owner)
			: base(new EditorButton(kind), null) {
			this.owner = owner;
		}
		public override ObjectState State {
			get {
				return base.State;
			}
			set {
				if(base.State == value) return;
				base.State = value;
				Owner.Invalidate();
				if(value == ObjectState.Pressed) DoClick();
			}
		}
		void DoClick() {
			if(Click != null) Click(this, new EventArgs());
		}
	}
	public class PrefilterCheckBox : CheckObjectInfoArgs {
		readonly PivotPrefilterPanelViewInfoBase owner;
		protected PivotPrefilterPanelViewInfoBase Owner { get { return owner; } }
		public event EventHandler Changed;
		public PrefilterCheckBox(PivotPrefilterPanelViewInfoBase owner)
			: base(null) {
			this.owner = owner;
		}
		public override CheckState CheckState {
			get { return base.CheckState; }
			set {
				if(base.CheckState == value) return;
				base.CheckState = value;
				Owner.Invalidate();
				DoChanged();
			}
		}
		public void SetCheckState(bool check) {
			base.CheckState = check ? CheckState.Checked : CheckState.Unchecked;
		}
		public override ObjectState State {
			get { return base.State; }
			set {
				if(base.State == value) return;
				base.State = value;
				Owner.Invalidate();
				if(value == ObjectState.Pressed) 
					CheckState = CheckState == CheckState.Checked ? CheckState.Unchecked : CheckState.Checked;
			}
		}
		void DoChanged() {
			if(Changed != null) Changed(this, new EventArgs());
		}
	}
}
