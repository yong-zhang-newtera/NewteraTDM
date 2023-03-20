/*
* @(#)DataGridControlImageColumn.cs
*
* Copyright (c) 2003 Newtera, Inc. All rights reserved.
*
*/
namespace Newtera.DataGridActiveX
{
	using System;
	using System.Windows.Forms;
	using System.Drawing;
	
	/// <summary>
	/// A DataGrid column that display an image.
	/// </summary>
	/// <version>  1.0.1 10 Jan 2004</version>
	/// <author> Yong Zhang</author>
	public class DataGridControlImageColumn : DataGridColumnStyle 
	{
		private Image _image;
        private IDataGridControlImageGetter _imageGetter;

		/// <summary>
		/// Instantiate an instance of DataGridControlImageColumn class
		/// </summary>
		public DataGridControlImageColumn() : base() 
		{
			_image = null;
            _imageGetter = null;
		}

		/// <summary>
		/// Gets or sets the image for displaying in the column.
		/// </summary>
		public Image ColumnImage
		{
			get
			{
				return _image;
			}
			set
			{
				_image = value;
			}
		}

        /// <summary>
        /// Gets or sets an image getter.
        /// </summary>
        public IDataGridControlImageGetter ImageGetter
        {
            get
            {
                return _imageGetter;
            }
            set
            {
                _imageGetter = value;
            }
        }

		/// <summary>
		/// When overridden in a derived class, initiates a request
		/// to interrupt an edit procedure.
		/// </summary>
		/// <param name="rowNum">The row number upon which an operation is being interrupted</param>
		protected override void Abort(int rowNum)
		{
			// do nothing, since the image column intends to be read-only
		}

		/// <summary>
		/// When overridden in a derived class, initiates a request to complete
		/// an editing procedure.
		/// </summary>
		/// <param name="dataSource">The CurrencyManager for the DataGridColumnStyle. </param>
		/// <param name="rowNum">The number of the row being edited.</param>
		/// <returns>true if the editing procedure committed successfully; otherwise, false.</returns>
		protected override bool Commit(CurrencyManager dataSource, int rowNum) 
		{
			// do nothing, since the image column intends to be read-only
			return true;
		}

		/// <summary>
		/// Prepares the cell for editing a value.
		/// </summary>
		/// <param name="source">The CurrencyManager for the DataGridColumnStyle.</param>
		/// <param name="rowNum">The row number in this column which is being edited.</param>
		/// <param name="bounds">The Rectangle in which the control is to be sited.</param>
		/// <param name="readOnly">A value indicating whether the column is a read-only. true if the value is read-only; otherwise, false.</param>
		/// <param name="instantText">The text to display in the control.</param>
		/// <param name="cellIsVisible">A value indicating whether the cell is visible. true if the cell is visible; otherwise, false.</param>
		protected override void Edit(CurrencyManager source, int rowNum,
			Rectangle bounds, bool readOnly, string instantText, bool cellIsVisible) 
		{
			// do nothing, since the image column intends to be read-only
		}

		/// <summary>
		/// When overridden in a derived class, gets the width and height of the
		/// specified value. The width and height are used when the user navigates
		/// to DataGridTableStyle using the DataGridColumnStyle.
		/// </summary>
		/// <param name="g">A Graphics object.</param>
		/// <param name="value">An object value for which you want to know the screen height and width.</param>
		/// <returns>A Size that contains the dimensions of the cell.</returns>
		protected override Size GetPreferredSize(Graphics g, object value) 
		{
			if (_image != null)
			{
				return _image.Size;
			}
			else
			{
				return new Size(16, 16);
			}
		}

		/// <summary>
		/// When overridden in a derived class, gets the minimum height of a row.
		/// </summary>
		/// <returns>The minimum height of a row.</returns>
		protected override int GetMinimumHeight() 
		{
			if (_image != null)
			{
				return _image.Height;
			}
			else
			{
				return 16;
			}
		}

		/// <summary>
		/// When overridden in a derived class, gets the height used for
		/// automatically resizing columns.
		/// </summary>
		/// <param name="g">A Graphics object.</param>
		/// <param name="value">A object value for which you want to know the screen height and width.</param>
		/// <returns>The height used for auto resizing a cell.</returns>
		protected override int GetPreferredHeight(Graphics g, object value) 
		{
			if (_image != null)
			{
				return _image.Height + 4;
			}
			else
			{
				return 20;
			}
		}

		/// <summary>
		/// When overridden in a derived class, paints the column in a System.Windows.Forms.DataGrid control.
		/// </summary>
		/// <param name="g">The Graphics object to draw to.</param>
		/// <param name="bounds">The bounding Rectangle to paint into.</param>
		/// <param name="source">The CurrencyManager of the System.Windows.Forms.DataGrid control the column belongs to.</param>
		/// <param name="rowNum">The number of the row in the underlying data table being referred to.</param>
		protected override void Paint(Graphics g, Rectangle bounds, 
			CurrencyManager source, int rowNum) 
		{
			Paint(g, bounds, source, rowNum, false);
		}

		/// <summary>
		/// When overridden in a derived class, paints the column in a System.Windows.Forms.DataGrid control.
		/// </summary>
		/// <param name="g">The Graphics object to draw to.</param>
		/// <param name="bounds">The bounding Rectangle to paint into.</param>
		/// <param name="source">The CurrencyManager of the System.Windows.Forms.DataGrid control the column belongs to.</param>
		/// <param name="rowNum">The number of the row in the underlying data table being referred to.</param>
		/// <param name="alignToRight">A value indicating whether to align the content to the right. true if the content is aligned to the right, otherwise, false.</param>
		protected override void Paint(Graphics g, Rectangle bounds,
			CurrencyManager source, int rowNum, bool alignToRight) 
		{
			Paint(
				g,bounds, 
				source, 
				rowNum, 
				Brushes.Red, 
				Brushes.Blue, 
				alignToRight);
		}

		/// <summary>
		/// When overridden in a derived class, paints the column in a System.Windows.Forms.DataGrid control.
		/// </summary>
		/// <param name="g">The Graphics object to draw to.</param>
		/// <param name="bounds">The bounding Rectangle to paint into.</param>
		/// <param name="source">The CurrencyManager of the System.Windows.Forms.DataGrid control the column belongs to.</param>
		/// <param name="rowNum">The number of the row in the underlying data table being referred to.</param>
		/// <param name="backBrush">A Brush used to paint the background color. </param>
		/// <param name="foreBrush">A Color used to paint the foreground color.</param>
		/// <param name="alignToRight">A value indicating whether to align the content to the right. true if the content is aligned to the right, otherwise, false.</param>
		protected override void Paint(
			Graphics g, 
			Rectangle bounds,
			CurrencyManager source, 
			int rowNum,
			Brush backBrush, 
			Brush foreBrush,
			bool alignToRight) 
		{
			Rectangle rect = bounds;
			g.FillRectangle(backBrush, rect);
			object val = GetColumnValueAtRow(source, rowNum);
            Image image = null;
            if (_imageGetter != null)
            {
                image = _imageGetter.GetImage(val);
            }

			if (image != null)
			{
                // center align the image 
                Rectangle imageRect;
                if (bounds.Size.Width > image.Width)
                {
                    int x = bounds.X + (bounds.Size.Width - image.Width) / 2;
                    int y = bounds.Y;
                    Point location = new Point(x, y);
                    imageRect = new Rectangle(location, image.Size);
                }
                else
                {
                    imageRect = new Rectangle(bounds.Location, image.Size);
                }

                // draw the image in preferred size
                g.DrawImage(image, imageRect);
			}
		}

		/// <summary>
		/// Sets the System.Windows.Forms.DataGrid for the column.
		/// </summary>
		/// <param name="value">A System.Windows.Forms.DataGrid.</param>
		protected override void SetDataGridInColumn(DataGrid value) 
		{
			base.SetDataGridInColumn(value);
		}
	}
}