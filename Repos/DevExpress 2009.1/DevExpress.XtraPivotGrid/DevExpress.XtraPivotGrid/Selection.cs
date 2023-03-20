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
using System.Drawing;
using System.Collections.Generic;
using System.Collections;
using DevExpress.Utils.Controls;
namespace DevExpress.XtraPivotGrid.ViewInfo {
	public interface IMultipleSelection {
		ReadOnlyCells SelectedCells { get; }
		void SetSelection(Point[] points);
	}
	public class ReadOnlyCells : IEnumerable<Point> {
		readonly List<Point> cells;
		public ReadOnlyCells(List<Point> cells) {
			this.cells = cells;
		}
		public ReadOnlyCells(Point theOnlyCell)
			: this(new List<Point>()) {
			cells.Add(theOnlyCell);
		}
		public bool IsEmpty { get { return cells.Count == 0; } }
		public int Count { get { return cells.Count; } }
		public Point this[int index] { get { return cells[index]; } }
		public bool Contains(Point point) { return cells.Contains(point); }
		public Rectangle Rectangle {
			get {
				if(IsEmpty) return Rectangle.Empty;
				int minX = cells[0].X,
					minY = cells[0].Y,
					maxX = cells[0].X,
					maxY = cells[0].Y;
				for(int i = 1; i < cells.Count; i++) {
					if(cells[i].X < minX) minX = cells[i].X;
					if(cells[i].Y < minY) minY = cells[i].Y;
					if(cells[i].X > maxX) maxX = cells[i].X;
					if(cells[i].Y > maxY) maxY = cells[i].Y;
				}
				return new Rectangle(minX, minY, maxX - minX + 1, maxY - minY + 1);
			}
		}
		public void CopyTo(List<Point> cells) {
			cells.AddRange(this.cells);
		}
		public static ReadOnlyCells operator -(ReadOnlyCells c1, ReadOnlyCells c2) {
			List<Point> result = new List<Point>(c1.cells);
			foreach(Point point in c2.cells)
				result.Remove(point);
			return new ReadOnlyCells(result);
		}
		#region IEnumerable<Point> Members
		public IEnumerator<Point> GetEnumerator() {
			return cells.GetEnumerator();
		}
		IEnumerator IEnumerable.GetEnumerator() {
			return cells.GetEnumerator();
		}
		#endregion
		public static ReadOnlyCells Empty = new ReadOnlyCells(new List<Point>());
	}
	public class PivotGridSelection {
		readonly List<Point> fCells;
		readonly ReadOnlyCells readOnlyCells;
		readonly Dictionary<Point, object> cellsHache;
		ReadOnlyCells fLastState = ReadOnlyCells.Empty;
		bool isLastSelectionIsMultiSelect;
		bool isLastSelectionIsShiftDown;
		bool isChanged = false;
		public bool IsEmpty { get { return Cells.IsEmpty; } }
		public ReadOnlyCells Cells { get { return readOnlyCells; } }
		public Rectangle Rectangle {
			get { return Cells.Rectangle; }
			set {
				LoadLastState();
				Clear();
				AddCore(value);
			}
		}
		public PivotGridSelection() {
			fCells = new List<Point>();
			readOnlyCells = new ReadOnlyCells(fCells);
			cellsHache = new Dictionary<Point, object>();
		}
		public void Add(Rectangle selection) {
			LoadLastState();
			AddCore(selection);
		}
		internal bool IsChanged { get { return isChanged; } set { isChanged = value; } }
		void AddCore(Rectangle selection) {
			if(selection.IsEmpty) return;
			bool changed = false;
			List<Point> newPoints = new List<Point>();
			for(int i = selection.Left; i < selection.Right; i++)
				for(int j = selection.Top; j < selection.Bottom; j++) {
					Point newPoint = new Point(i, j);
					if(!cellsHache.ContainsKey(newPoint)) {
						newPoints.Add(newPoint);
						changed = true;
					}
				}
			AddRange(newPoints);
			IsChanged = changed;
		}
		void AddRange(IEnumerable<Point> points) {
			IsChanged = true;
			fCells.AddRange(points);
			foreach(Point p in points)
				cellsHache.Add(p, null);
		}
		public void Subtract(Rectangle selection) {
			LoadLastState();
			SubtractCore(selection);
		}
		void SubtractCore(Rectangle selection) {
			if(IsEmpty) return;
			bool changed = false;
			for(int i = selection.Left; i < selection.Right; i++)
				for(int j = selection.Top; j < selection.Bottom; j++) {
					Point newPoint = new Point(i, j);
					if(cellsHache.ContainsKey(newPoint)) {
						fCells.Remove(newPoint);
						cellsHache.Remove(newPoint);
						changed = true;
					}
				}
			IsChanged = changed;
		}
		public void Clear() {
			if(fCells.Count == 0)
				return;
			IsChanged = true;
			fCells.Clear();
			cellsHache.Clear();
		}
		public void SetSelection(Point[] points) {
			StartSelection(false);
			AddRange(points);
		}
		public void StartSelection(bool multiSelect) {
			StartSelection(multiSelect, multiSelect, false);
		}
		public void StartSelection(bool multiSelect, bool isControlDown, bool isShiftDown) {
			if(isLastSelectionIsMultiSelect = multiSelect && (isControlDown || isShiftDown)) {
				fLastState = Save();
			}
			else {
				fLastState = ReadOnlyCells.Empty;
				Clear();
			}
			isLastSelectionIsShiftDown = isShiftDown;
		}
		public void LoadLastState() {
			Load(fLastState);
			IsChanged = false;
		}
		public ReadOnlyCells LastSelection {
			get {
				return Cells - fLastState;
			}
		}
		public bool IsLastSelectionIsMultiSelect { get { return isLastSelectionIsMultiSelect; } }
		public bool IsLastSelectionIsShiftDown { get { return isLastSelectionIsShiftDown; } }
		ReadOnlyCells Save() {
			return new ReadOnlyCells(new List<Point>(fCells));
		}
		void Load(ReadOnlyCells cells) {
			Clear();
			AddRange(cells);
		}
		public void CorrectSelection(int columnCount, int rowCount, int maxWidth, int maxHeight, Rectangle lastSelection) {
			Rectangle r = new Rectangle(0, 0, columnCount, rowCount);
			r.Intersect(Rectangle);
			r = CorrectSelectionConstraints(lastSelection, r, maxWidth, maxHeight);
			for(int i = 0; i < fCells.Count; i++) {
				if(r.Contains(fCells[i])) continue;
				fCells.Remove(fCells[i]);
				i--;
			}
		}
		Rectangle CorrectSelectionConstraints(Rectangle curSelection, Rectangle newSelecion, int maxWidth, int maxHeight) {
			Rectangle result = newSelecion;
			if(maxWidth >= 0 && newSelecion.Width > maxWidth) {
				if(newSelecion.Left < curSelection.Left)  		
					result.X = newSelecion.Right - maxWidth;
				result.Width = maxWidth;
			}
			if(maxHeight >= 0 && newSelecion.Height > maxHeight) {
				if(newSelecion.Top < curSelection.Top)  		
					result.Y = newSelecion.Bottom - maxHeight;
				result.Height = maxHeight;
			}
			return result;
		}
	}
	public class PivotGridViewScroller : OfficeScroller {
		PivotGridViewInfoBase view;
		public PivotGridViewScroller(PivotGridViewInfoBase view) {
			this.view = view;
		}
		protected PivotGridViewInfoBase View { get { return view; } }
		protected override void OnHScroll(int delta) { View.LeftTopCoord = new Point(View.LeftTopCoord.X + delta, View.LeftTopCoord.Y); }
		protected override void OnVScroll(int delta) { View.LeftTopCoord = new Point(View.LeftTopCoord.X, View.LeftTopCoord.Y + delta); }
		protected override bool AllowVScroll { get { return true; } }
		protected override bool AllowHScroll { get { return true; } }
	}
}
