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
using System.Collections;
using System.Drawing;
using System.Windows.Forms;
using DevExpress.Utils.Drawing;
namespace DevExpress.XtraPivotGrid.ViewInfo {
	public interface IViewInfoControl {
		Rectangle ClientRectangle { get; }
		void Invalidate(Rectangle bounds);
		Control ControlOwner {get; }
		void InvalidateScrollBars();
		void UpdateScrollBars();
		bool IsDesignMode { get; }
	}
	public class ViewInfoPaintArgs {
		Control control;
		Graphics graphics;
		Rectangle clipRectangle;
		GraphicsCache graphicsCache;
		public ViewInfoPaintArgs(Control control, PaintEventArgs e) {
			this.control = control;
			this.graphics = e.Graphics;
			this.clipRectangle = e.ClipRectangle;
			this.graphicsCache = new GraphicsCache(e.Graphics);
		}
		protected Control Control { get { return control; } }
		public Rectangle ClientRectangle { get { return Control != null ? Control.ClientRectangle : Rectangle.Empty; } }
		public Graphics Graphics { get { return graphics; } }
		public Rectangle ClipRectangle { get { return clipRectangle; } }
		public GraphicsCache GraphicsCache { get { return graphicsCache; } }
	}
	public class BaseViewInfo : IDisposable {
		bool isReady;
		BaseViewInfoCollection children;
		BaseViewInfo parent;
		bool destroyChildrenOnClear;
		Rectangle paintBounds;
		Size boundsOffset;
		BaseViewInfo activeViewInfo;
		BaseViewInfo hotTrackViewInfo;
		public BaseViewInfo()
			: this(true) {
		}
		public BaseViewInfo(bool destroyChildrenOnClear) {
			this.destroyChildrenOnClear = destroyChildrenOnClear;
			this.boundsOffset = Size.Empty;
			this.isReady = false;
			this.parent = null;
			this.activeViewInfo = null;
			this.hotTrackViewInfo = null;
		}
		public Rectangle Bounds;
		public Size BoundsOffset {
			get { return boundsOffset; }
			set {
				if(Parent != null && Parent != Root) throw new Exception("BoundsOffset can be set at the first level only");
				boundsOffset = value;
			}
		}
		public virtual Rectangle ControlBounds {
			get {
				return CorrectControlBoundsBasedOnOffsets(OriginalControlBounds);
			}
		}
		Rectangle OriginalControlBounds {
			get {
				Rectangle controlBounds = Bounds;
				controlBounds.Offset(BoundsOffset.Width, BoundsOffset.Height);
				if(parent == null)
					return controlBounds;
				Rectangle parentControlBounds = parent.OriginalControlBounds;
				controlBounds.X += parentControlBounds.Left;
				controlBounds.Y += parentControlBounds.Top;
				return controlBounds;
			}
		}
		Rectangle CorrectControlBoundsBasedOnOffsets(Rectangle rect) {
			BaseViewInfo viewInfo = this;
			while(viewInfo != null) {
				if(viewInfo != this || !viewInfo.BoundsOffset.IsEmpty) {
					Rectangle controlBounds = viewInfo == this ? viewInfo.Bounds : viewInfo.ControlBounds;	
					if(controlBounds.X > rect.X) {
						rect.Width -= controlBounds.X - rect.X;
						rect.X = controlBounds.X;
					}
					if(controlBounds.Y > rect.Y) {
						rect.Height -= controlBounds.Y - rect.Y;
						rect.Y = controlBounds.Y;
					}
				}
				viewInfo = viewInfo.Parent;
			}
			return rect;
		}
		public Rectangle PaintBounds {
			get {
				if(paintBounds.IsEmpty) {
					paintBounds = CalculatePaintBounds();
				}
				return paintBounds;
			}
		}
		protected virtual Rectangle CalculatePaintBounds() {
			Rectangle result = ControlBounds;
			if((result.Width <= 0) || (result.Height <= 0))
				result = Rectangle.Empty;
			return result;
		}
		public void EnsureIsCalculated() {
			if(!IsReady) 
				Calculate();
		}
		protected void Calculate() {
			OnBeforeCalculating();
			if(this.parent != null)
				this.parent.OnBeforeChildCalculating(this);
			CalculateChildren();
			InternalCalculate();
			OnAfterCalculated();
			if(this.parent != null)
				this.parent.OnAfterChildCalculated(this);
			this.isReady = true;
		}
		public void Clear() {
			if(this.children != null) {
				if(DestroyChildrenOnClear)
					ClearChildren();
				else this.children.ClearViewInfo();
			}
			this.isReady = false;
			this.activeViewInfo = null;
			this.hotTrackViewInfo = null;
			Bounds = Rectangle.Empty;
			InternalClear();
		}
		public bool IsReady { get { return isReady; } }
		public virtual void KeyDown(KeyEventArgs e) { }
		public virtual void MouseMove(MouseEventArgs e) {
			if(ActiveViewInfo != null)
				ActiveViewInfo.MouseMoveCore(e);
			else {
				BaseViewInfo viewInfo = GetViewInfoAtPoint(e.X, e.Y);
				HotTrackViewInfo = viewInfo;
				if(viewInfo != null) {
					viewInfo.MouseMoveCore(e);
				}
			}
		}
		public virtual void MouseDown(MouseEventArgs e) {
			BaseViewInfo viewInfo = GetViewInfoAtPoint(e.X, e.Y);
			if(viewInfo != null) {
				this.activeViewInfo = viewInfo.MouseDownCore(e);
				if(this.activeViewInfo != null)
					this.activeViewInfo.Invalidate();
			}
		}
		public virtual void MouseUp(MouseEventArgs e) {
			if(ActiveViewInfo != null)
				ActiveViewInfo.MouseUpCore(e);
			this.activeViewInfo = null;
		}
		public virtual void DoubleClick() {
			if(ActiveViewInfo != null)
				ActiveViewInfo.DoubleClick();
		}
		public void MouseEnter() {
			MouseEnterCore();
		}
		public void MouseLeave() {
			HotTrackViewInfo = null;
			MouseLeaveCore();
		}
		public BaseViewInfo GetViewInfoAtPoint(int x, int y) {
			return GetViewInfoAtPoint(new Point(x, y), true);
		}
		public BaseViewInfo GetViewInfoAtPoint(int x, int y, bool recursive) {
			return GetViewInfoAtPoint(new Point(x, y), recursive);
		}
		public BaseViewInfo GetViewInfoAtPoint(Point pt) {
			return GetViewInfoAtPoint(pt, true);
		}
		public BaseViewInfo GetViewInfoAtPoint(Point pt, bool recursive) {
			if(CheckControlBounds && !ControlBounds.Contains(pt)) return null;
			int startIndex = GetStartLocationAtPoint(pt);
			return GetViewInfoAtPoint(pt, recursive, startIndex);
		}
		protected BaseViewInfo GetViewInfoAtPoint(Point pt, bool recursive, int startIndex) {
			for(int i = ChildCount - 1; i >= startIndex; i--) {
				BaseViewInfo viewInfo = this[i].OriginalControlBounds.Contains(pt) ? this[i] : null;
				if(viewInfo != null) {
					if(recursive)
						viewInfo = this[i].GetViewInfoAtPoint(pt);
					return viewInfo;
				}
			}
			return recursive ? this : null;
		}
		protected virtual int GetStartLocationAtPoint(Point pt) {
			return 0;
		}
		protected virtual bool CheckControlBounds { get { return true; } }
		public void Paint(Control control, PaintEventArgs e) {
			Paint(new ViewInfoPaintArgs(control, e));
		}
		public void Paint(ViewInfoPaintArgs e) {
			if(!IsReady) EnsureIsCalculated();
			this.paintBounds = Rectangle.Empty;
			if(CheckControlBounds) {
				if(!e.ClipRectangle.IsEmpty && (!PaintBounds.IntersectsWith(e.ClipRectangle))) return;
				if(e.ClientRectangle != Rectangle.Empty && !PaintBounds.IntersectsWith(e.ClientRectangle)) return;
			}
			InternalPaint(e);
			PaintChildren(e);
			AfterPaint(e);
		}
		public void Invalidate() {
			Invalidate(ControlBounds);
		}
		protected virtual void Invalidate(Rectangle bounds) { }
		public void AddChild(BaseViewInfo viewInfo) {
			if(children == null)
				children = new BaseViewInfoCollection();
			viewInfo.parent = this;
			children.Add(viewInfo);
		}
		public int ChildIndex { get { return parent != null ? Parent.ChildIndexOf(this) : -1; } }
		public int ChildIndexOf(BaseViewInfo viewInfo) { return children != null ? children.IndexOf(viewInfo) : -1; }
		public int ChildCount { get { return children != null ? children.Count : 0; } }
		public BaseViewInfo GetChild(int index) { return children != null ? children[index] : null; }
		public BaseViewInfo this[int index] { get { return GetChild(index); } }
		public bool HasChildren { get { return ChildCount > 0; } }
		public BaseViewInfo FirstChild { get { return HasChildren ? this[0] : null; } }
		public BaseViewInfo LastChild { get { return HasChildren ? this[ChildCount - 1] : null; } }
		public BaseViewInfo Parent { get { return parent; } }
		public BaseViewInfo Root { get { return Parent == null ? this : Parent.Root; } }
		public bool IsActive { get { return Root != null && Root.ActiveViewInfo == this; } }
		public BaseViewInfo ActiveViewInfo { get { return activeViewInfo; } }
		public bool IsHotTrack { get { return Root != null && Root.HotTrackViewInfo == this; } }
		public BaseViewInfo HotTrackViewInfo {
			get { return hotTrackViewInfo; }
			set {
				if(HotTrackViewInfo == value) return;
				if(HotTrackViewInfo != null)
					HotTrackViewInfo.MouseLeaveCore();
				this.hotTrackViewInfo = value;
				if(HotTrackViewInfo != null)
					HotTrackViewInfo.MouseEnterCore();
			}
		}
		protected bool DestroyChildrenOnClear { get { return destroyChildrenOnClear; } set { destroyChildrenOnClear = value; } }
		protected virtual void MouseMoveCore(MouseEventArgs e) {
		}
		protected virtual BaseViewInfo MouseDownCore(MouseEventArgs e) {
			return null;
		}
		protected virtual void MouseUpCore(MouseEventArgs e) {
		}
		protected virtual void MouseEnterCore() { }
		protected virtual void MouseLeaveCore() { }
		protected virtual void InternalCalculate() { }
		protected virtual void InternalClear() { }
		protected virtual void InternalPaint(ViewInfoPaintArgs e) { }
		protected virtual void AfterPaint(ViewInfoPaintArgs e) { }
		protected virtual void OnAfterCalculated() { }
		protected virtual void OnBeforeCalculating() { }
		protected virtual void OnAfterChildrenCalculated() { }
		protected virtual void OnBeforeChildrenCalculating() { }
		protected virtual void OnAfterChildCalculated(BaseViewInfo viewInfo) { }
		protected virtual void OnBeforeChildCalculating(BaseViewInfo viewInfo) { }
		protected void ClearChildren() { children.Clear(); }
		protected void RemoveChildren(int index) { children.RemoveAt(index); }
		void CalculateChildren() {
			if(this.children == null) return;
			this.children.SetBounds(Bounds);
			OnBeforeChildrenCalculating();
			this.children.CalculateViewInfo();
			OnAfterChildrenCalculated();
		}		
		protected virtual void PaintChildren(ViewInfoPaintArgs e) {
			if(this.children != null)
				this.children.Paint(e);
		}
		#region IDisposable Members
		public virtual void Dispose() {
		}
		#endregion
	}
	public enum PivotGridViewInfoState { Normal, FieldResizing };
	public class BaseViewInfoCollection : CollectionBase {
		public BaseViewInfoCollection() { }
		public BaseViewInfo this[int index] { get { return List[index] as BaseViewInfo; } }
		public int Add(BaseViewInfo viewInfo) { return List.Add(viewInfo); }
		public int IndexOf(BaseViewInfo viewInfo) { return List.IndexOf(viewInfo); }
		public void CalculateViewInfo() {
			for(int i = 0; i < Count; i++)
				this[i].EnsureIsCalculated();
		}
		public void ClearViewInfo() {
			for(int i = 0; i < Count; i++)
				this[i].Clear();
		}
		public void Paint(ViewInfoPaintArgs e) {
			for(int i = 0; i < Count; i++)
				this[i].Paint(e);
		}
		public void SetBounds(Rectangle Bounds) {
			for(int i = 0; i < Count; i++)
				if(this[i].Bounds.IsEmpty) {
					this[i].Bounds.Width = Bounds.Width;
					this[i].Bounds.Height = Bounds.Height;
				}
		}
	}
	internal class RectangleHelper {
		public static Rectangle Shrink(Rectangle rect, int dLeft, int dTop, int dRight, int dBottom) {
			Rectangle result = rect;
			result.X += dLeft;
			result.Width -= dLeft + dRight;
			result.Y += dTop;
			result.Height -= dTop + dBottom;
			return result;
		}
	}
}
