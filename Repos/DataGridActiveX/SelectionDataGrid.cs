using System;
using System.Drawing;
using System.Collections;
using System.ComponentModel;
using System.Windows.Forms;
using System.Data;

namespace Newtera.DataGridActiveX
{
	/// <summary>
	/// Summary description for SelectionDataGrid.
	/// </summary>
	public class SelectionDataGrid : DataGrid
	{
		//fired when a selection is about to change
		public event DataGridSelectionChangingEventHandler SelectionChanging;

        public event DataGridCellClickEventHandler CellClick;

		//(top,left,bottom,right) of selection
		private GridRange _selectedRange;
 
		//used in drawing
		private Rectangle _selectionRectangle;
		private Rectangle _clipRectangle;

		//used to record row-col of click
		private int _mouseDownRow;
		private int _mouseDownCol;
		private int _mouseUpRow;
		private int _mouseUpCol;

		// sort info
		private int _sortColumnIndex;
		private bool _isAscending;

        // hit column
        private int _hitColumn;

		//used in autoscroll
		private bool lastMoveHorz;
		
		public SelectionDataGrid()
		{
			_selectedRange = new GridRange();
			_clipRectangle = Rectangle.Empty;

			//used to redraw selection during a scroll
			this.Scroll += new EventHandler(HandleScroll);

			//used to get a clipping rectange for the initial display
			this.Paint += new PaintEventHandler(FirstPaint);
			
			//set up double buffering to minimize flashing during draws
			this.SetStyle(ControlStyles.AllPaintingInWmPaint, true);
			this.SetStyle(ControlStyles.UserPaint, true);
			this.SetStyle(ControlStyles.DoubleBuffer, true);

			// sort
			_sortColumnIndex = -1; // no column is sorted by default
			_isAscending = true; // default sorting direction is ascending
		}

		#region Paint & Scroll Event Handlers

		public void ScrollToRow(int rowIndex)
		{
			// Expose the protected GridVScrolled method allowing you
			// to programmatically scroll the grid to a particulat row
			if (DataSource != null)
			{
				GridVScrolled(this, new ScrollEventArgs(ScrollEventType.LargeIncrement, rowIndex));
			}
		}

		private void FirstPaint(object sender, PaintEventArgs e)
		{
			//used to mark the size of original clientsize
			this.Paint -= new PaintEventHandler(FirstPaint);
			if(!this.DesignMode && this.DataSource != null)
			{
				_clipRectangle = this.GetCellBounds(0,0);
				_clipRectangle.Height = this.Height - _clipRectangle.Y - SystemInformation.HorizontalScrollBarHeight;
				_clipRectangle.Width = this.Width - _clipRectangle.X - SystemInformation.VerticalScrollBarWidth;
			}
		}

		//helper method that gets the left column from knowing the initial cliprectangle
        public int LeftColumn()
		{
			DataGrid.HitTestInfo hti = this.HitTest(new Point(_clipRectangle.X, _clipRectangle.Y));
			return hti.Column;
		}
		private void HandleScroll(object sender, EventArgs e)
		{
			DrawRange(true);			
		}


		#endregion
		
		#region Sort Property

		public int SortColumnIndex
		{
			get
			{
				return this._sortColumnIndex;
			}
			set
			{
				this._sortColumnIndex = value;
			}
		}

		public bool IsAscending
		{
			get
			{
				return this._isAscending;
			}	
		}

        public int HitColumnIndex
        {
            get
            {
                return _hitColumn;
            }
        }
		
		#endregion

		#region Selected Range property

		public GridRange SelectedRange
		{
			get
            {
                return _selectedRange;
            }
			set
		    { 
				if(SelectionChanging != null)
				{
					DataGridSelectionChangingEventArgs e = new DataGridSelectionChangingEventArgs(_selectedRange,
						value);
					SelectionChanging(this, e);
					if(e.Canceled)
						return;
				}
				
				_selectedRange = value;
			}
		}
		#endregion


		#region mouse events that handle making selections

		protected override void OnMouseDown(System.Windows.Forms.MouseEventArgs e)
		{
			base.OnMouseDown(e);

			//remember the initial click	
			if(e.Button == MouseButtons.Left)
			{
				DataGrid.HitTestInfo hti = this.HitTest(new Point(e.X, e.Y));
				_mouseDownRow = hti.Row;
				_mouseDownCol = hti.Column;
				_mouseUpRow = _mouseDownRow;
				_mouseUpCol = _mouseDownCol;

				//clear any existing selection
				SelectedRange.Clear();

				DrawRange(false);
			}
		}

		protected override void OnMouseMove(System.Windows.Forms.MouseEventArgs e)
		{
			base.OnMouseMove(e);

			if(e.Button == MouseButtons.Left )
			{
				DataGrid.HitTestInfo hti = this.HitTest(new Point(e.X, e.Y));

				switch(hti.Type)
				{
					//move to a visible cell with no scrolling
					case HitTestType.Cell:
						if(hti.Column != this._mouseUpCol || hti.Row != this._mouseUpRow)
						{
							lastMoveHorz = this._mouseUpCol != hti.Column;

							DrawRange(false);
							this._mouseUpCol = hti.Column;
							this._mouseUpRow = hti.Row;
							SelectedRange = new GridRange(this._mouseUpRow, this._mouseUpCol, this._mouseDownRow, this._mouseDownCol);
							DrawRange(true);
						}
						break;

						#region handle autoscrolling....
					case HitTestType.ColumnHeader:
						if(this._mouseDownRow == -1)
							break;

						if(this.VertScrollBar.Value > 0)
						{
							this.VertScrollBar.Value--;
							this.CurrentRowIndex = this.VertScrollBar.Value;
							
							this._mouseUpCol = hti.Column;
							this._mouseUpRow--;
							SelectedRange = new GridRange(this._mouseUpRow, this._mouseUpCol, this._mouseDownRow, this._mouseDownCol);
							DrawRange(true);
							//this.Invalidate();
						}
						break;
					case HitTestType.RowHeader:
					{
						if(this._mouseDownCol == -1)
							break;

						int rightCol = this.CheckBound(this._mouseUpCol);
						Rectangle r = this.GetCellBounds(0, rightCol);
						if(this.HorizScrollBar.Value >= r.Width)
						{
							this.HorizScrollBar.Value -= r.Width;
							this._mouseUpCol--;
							this.CurrentCell = new DataGridCell(this.CurrentRowIndex,this._mouseUpCol) ;
							
							this._mouseUpRow = hti.Row;
							SelectedRange = new GridRange(this._mouseUpRow, this._mouseUpCol, this._mouseDownRow, this._mouseDownCol);
							DrawRange(true);
							//this.Invalidate();
						}
					}
					break;
					case HitTestType.None:
						//Console.WriteLine("HitTestType.None");
						if (this.DataSource != null)
						{
							if(lastMoveHorz)
							{
								Console.WriteLine("lastMoveHorz");
								if(this._mouseUpCol < this.BindingContext[this.DataSource, this.DataMember].GetItemProperties().Count)
								{
									int rightCol = this.CheckBound(this._mouseUpCol);
									Rectangle r = this.GetCellBounds(0, rightCol);
									if(this.HorizScrollBar.Value + r.Width <= this.HorizScrollBar.Maximum)
									{
										this.HorizScrollBar.Value += r.Width;
										if(this._mouseUpCol < this.BindingContext[this.DataSource, this.DataMember].GetItemProperties().Count - 1)
											this._mouseUpCol++;
										this.CurrentCell = new DataGridCell(hti.Row, this._mouseUpCol) ;
									
										if(hti.Row > -1)
											this._mouseUpRow = hti.Row;
										SelectedRange = new GridRange(this._mouseUpRow, this._mouseUpCol, this._mouseDownRow, this._mouseDownCol);
										DrawRange(true);
									
										//this.Invalidate();
									}
								}
							}
							else
							{
								if(this._mouseUpRow < this.BindingContext[this.DataSource, this.DataMember].Count)
								{
									//this.HorizScrollBar.Value--;
									if(this._mouseUpRow < this.BindingContext[this.DataSource, this.DataMember].Count - 1)
										this._mouseUpRow++;
									if(hti.Column > 0)
										this._mouseUpCol = hti.Column;
									this.CurrentCell = new DataGridCell(this._mouseUpRow, this._mouseUpCol) ;
															
									SelectedRange = new GridRange(this._mouseUpRow, this._mouseUpCol, this._mouseDownRow, this._mouseDownCol);
									DrawRange(true);
									//this.Invalidate();
								}
							}
						}
						break;
						#endregion
				}
			}
		}

		protected override void OnMouseUp(System.Windows.Forms.MouseEventArgs e)
		{
			DataGrid.HitTestInfo hti = this.HitTest(new Point(e.X, e.Y));

			if (hti.Type == HitTestType.ColumnHeader)
			{
				if (this._sortColumnIndex == hti.Column)
				{
					// hit the same column, change the sort direction
					this._isAscending = !this._isAscending;
				}
				else
				{
					// hit a different column
					this._sortColumnIndex = hti.Column;
					this._isAscending = true;
				}
			}

            _hitColumn = hti.Column; // hit column

			//if normal click clear things & click
			if(this._mouseUpCol == this._mouseDownCol 
				&& this._mouseUpRow == this._mouseDownRow)
			{
				DrawRange(false);
				_selectionRectangle = Rectangle.Empty;

                // fire a cell click event
                if (CellClick != null && hti.Type == HitTestType.Cell)
                {
                    DataGridCellClickEventArgs cellClickEvent = new DataGridCellClickEventArgs(this._mouseUpRow, this._mouseUpCol);

                    CellClick(this, cellClickEvent);
                }

				base.OnMouseDown(e);
				base.OnMouseUp(e);
			}
		}

		#endregion

		#region Drawing Code

		private int CheckBound(int bound)
		{
			int val = bound;

			if (val < 0)
			{
				val = 0;
			}
			else if (this.TableStyles.Count > 0)
			{
				int columns = this.TableStyles[0].GridColumnStyles.Count;
				if (val >= columns)
				{
					val = columns - 1;
				}
			}

			return val;
		}

		private void DrawRange(bool showRange)
		{
			//if removing selection just redraw
			if(!showRange)
			{
				_selectionRectangle = Rectangle.Empty;
				this.Invalidate();
				
				return;
			}

			// check the bounds
			if( SelectedRange.Left < 0 ||
				SelectedRange.Bottom < 0 ||
				SelectedRange.Right < 0 ||
				SelectedRange.Top < 0)
				return;

			if(SelectedRange.Right < LeftColumn())
			{
				_selectionRectangle = Rectangle.Empty;
				return;
			}

			SelectedRange.Right = CheckBound(SelectedRange.Right);
			SelectedRange.Left = CheckBound(SelectedRange.Left);

			Rectangle rect = this.GetCellBounds(SelectedRange.Top, SelectedRange.Left);
			Rectangle rect1 = this.GetCellBounds(SelectedRange.Bottom, SelectedRange.Right);
			int x, y, h, w;
			if(rect.Left < rect1.Left)
			{
				x = rect.Left;
				w = rect1.Left - rect.Left + rect1.Width;
			}
			else
			{
				x = rect1.Left;
				w = rect.Left - rect1.Left + rect.Width;
			}
			if(rect.Top < rect1.Top)
			{
				y = rect.Top;
				h = rect1.Top - rect.Top + rect1.Height;
			}
			else
			{
				y = rect1.Top;
				h = rect.Top - rect1.Top + rect.Height;
			}
			_selectionRectangle = new Rectangle(x, y, w, h);
			_selectionRectangle.Intersect(_clipRectangle);
			//Console.WriteLine(rect.ToString() + rect1.ToString() + _selectionRectangle.ToString());
			this.Invalidate(_selectionRectangle, true);

			//this little piece of code hides the current cell's TextBox
			foreach(Control c in this.Controls)
			{
				TextBox t = c as TextBox;
				if(t != null)
				{
					t.Hide();
				}
			}
		}

		protected override void OnPaint(System.Windows.Forms.PaintEventArgs pe)
		{
			base.OnPaint(pe);
			if(!this._selectionRectangle.Equals(Rectangle.Empty))
			{
				pe.Graphics.FillRectangle(new SolidBrush(Color.FromArgb(60, 0, 0, 255)), this._selectionRectangle);
			}
		}
	
		protected override void OnLayout(System.Windows.Forms.LayoutEventArgs levent)
		{
			base.OnLayout(levent);

			//reset the clipping rectangle
			if(!_clipRectangle.Equals(Rectangle.Empty))
			{
				_clipRectangle.Height = this.Height - _clipRectangle.Y - SystemInformation.HorizontalScrollBarHeight;
				_clipRectangle.Width = this.Width - _clipRectangle.X - SystemInformation.VerticalScrollBarWidth;
			}
		}
		#endregion

	}

	#region GridRange class implementation
	public class GridRange
	{
		private int _top;
		private int _left;
		private int _bottom;
		private int _right;

        public GridRange()
		{
			_top = -1;
			_left = -1;
			_bottom = -1;
			_right = -1;
		}

		public GridRange(int t, int l, int b, int r)
		{
			_top = Math.Min(t, b);
			_left = Math.Min(l, r);
			_bottom = Math.Max(t,b);
			_right = Math.Max(l, r);
		}

		public int Top
		{
			get{return _top;}
			set{_top = value;}
		}
		public int Bottom
		{
			get{return _bottom;}
			set{_bottom = value;}
		}
		public int Left
		{
			get{return _left;}
			set{_left = value;}
		}
		public int Right
		{
			get{return _right;}
			set{_right = value;}
		}

		public void Clear()
		{
			this.Bottom = -1;
			this.Top = -1;
			this.Left = -1;
			this.Right = -1;
		}

		public override string ToString()
		{
			return "TopLeft:" + this.Top.ToString() + "," + this.Left.ToString() + "   BottomRight:" + this.Bottom.ToString() + "," + this.Right.ToString();
		}
	}


	#endregion

	#region SelectionChangingEvent class implementation
	public delegate void DataGridSelectionChangingEventHandler(object sender, DataGridSelectionChangingEventArgs e);

	public class DataGridSelectionChangingEventArgs : EventArgs
	{
		private GridRange _oldRange;
		private GridRange _newRange;
		private bool _canceled;
		
		public DataGridSelectionChangingEventArgs(GridRange oldValue, GridRange newValue)
		{
			_oldRange = new GridRange(oldValue.Top, oldValue.Left, oldValue.Bottom, oldValue.Right);
			_newRange = new GridRange(newValue.Top, newValue.Left, newValue.Bottom, newValue.Right);
			_canceled = false;
		}

		public bool Canceled
		{
			get{ return _canceled;}
			set {_canceled = value;}
		}

		public GridRange OldRange
		{
			get{ return _oldRange;}
			set
			{
				_oldRange.Bottom = value.Bottom;
				_oldRange.Top = value.Top;
				_oldRange.Left = value.Left;
				_oldRange.Right = value.Right;
			}
		}
		public GridRange NewRange
		{
			get{ return _newRange;}
			set
			{
				_newRange.Bottom = value.Bottom;
				_newRange.Top = value.Top;
				_newRange.Left = value.Left;
				_newRange.Right = value.Right;
			}
		}
	}
    #endregion

    #region CellClickEvent class implementation
    public delegate void DataGridCellClickEventHandler(object sender, DataGridCellClickEventArgs e);

    public class DataGridCellClickEventArgs : EventArgs
    {
        private int _row;
        private int _col;

        public DataGridCellClickEventArgs(int row, int col)
        {
            _row = row;
            _col = col;
        }

        public int Row
        {
            get { return _row; }
        }

        public int Col
        {
            get { return _col; }
        }
    }
    #endregion

}
