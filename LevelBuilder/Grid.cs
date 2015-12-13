using System;
using System.Drawing;
using System.Windows.Forms;


namespace GridCtrl
{

	/// <summary>
	/// Summary description for Grid.
	/// </summary>
	public class Grid : System.Windows.Forms.UserControl 
	{
		// Delegates
		public  delegate void CellSelectDelegate(Cell cell);
		public  delegate void BeginEditDelegate(Cell cell);
		public  delegate void EndEditDelegate(Cell cell);

		// Events
		public event CellSelectDelegate OnCellSelect;

		public enum SelectionModeType
		{
			NoSelect,
			CellSelect,
			RowSelect,
			ColumnSelect
		}

		protected enum HitCode
		{
			hitNone,
			hitColumnEdge,
			hitRowEdge,
			hitColumn,
			hitRow,
			hitCell
		}

		public enum ImageStyleType
		{
			Center,
			Tile,
			Stretch,
		}

		/// <summary>
		/// Style of grid lines
		/// </summary>
		public enum GridLineStyle
		{
			None,
			Vertical,
			Horizontal,
			Both,
		}

		/// <summary>
		/// Required designer variable.
		/// </summary>
		///
		private System.ComponentModel.Container components = null;

		#region ****** Grid Specific Variables ******
		public		GridObjectCollection	rowList = new GridObjectCollection();
		public		GridObjectCollection	colList	= new GridObjectCollection();
		protected	Color				clrBk = SystemColors.ControlLight;
		protected	uint				defaultRowHeight = 26;
		protected	uint				defaultColWidth	 = 36;
		protected	CellSizer			sizerCell = new CellSizer();
		protected	SelectionModeType	selectMode = SelectionModeType.CellSelect;
		protected	Image				image;
		protected	ImageStyleType		imageStyle = ImageStyleType.Stretch;
		protected	bool				lockUpdates;
		protected	GridLineStyle		gridLines = GridLineStyle.Both;
		protected	int					Opacity = 255; // Default
		protected	HScrollBarEx hScrollBar			= new HScrollBarEx();
		protected	VScrollBarEx vScrollBar			= new VScrollBarEx();
		protected	Color				gridLineColor	= SystemColors.ControlLight;
		protected	Cell				selectedCell = null;
		protected	GridObject			selectedObject = null;
		private		int					HScrollMax = 0;
		private		int					VScrollMax = 0;
		private		const				int Tolerence = 2;
		private		CellEditor			textBox		= null;				
		private		bool				isEditMode = true;
		private		ToolTip				toolTip = new ToolTip();
		private		bool				toolTips = true;
		private		Cell				tipCell = null;


		#endregion

		////////////////////////////////////////////////////////////////////////////////////////////////
		////////////////////////////////////////////////////////////////////////////////////////////////
		// Number of rows and columns
		////////////////////////////////////////////////////////////////////////////////////////////////	
		////////////////////////////////////////////////////////////////////////////////////////////////

		/// <summary>
		/// Returns the number of rows (including fixed rows)
		/// </summary>
		/// <returns></returns>
		public int GetRowCount()
		{
			return rowList.Count;
		}
			
		/// <summary>
		/// Returns the number of columns (including fixed columns)
		/// </summary>
		/// <returns></returns>
		public int GetColumnCount() 
		{
			return colList.Count;
		}
		
		/// <summary>
		/// Returns the number of fixed rows
		/// </summary>
		/// <returns></returns>
		public int GetFixedRowCount()
		{
			return GetRowHeaderCount();
		}

		/// <summary>
		/// Returns the number of fixed columns
		/// </summary>
		/// <returns></returns>
		public int GetFixedColumnCount()
		{
			return GetColumnHeaderCount();
		}


		/// <summary>
		/// Sets the number of rows (including fixed rows
		/// </summary>
		/// <param name="rows"></param>
		public void SetRowCount(int rows) 
		{
			if (rows < 0)
				return;

			int rowcnt = rowList.Count;

			if (rows == rowList.Count)
				return;

			LockUpdates = true;
			if (rows > rowcnt)
			{
				for (int i = 0; i < rows - rowcnt; i++ )
					AddRow();
			}
			else
			{
				for (int i = 0; i < rowcnt - rows; i++ )
					DeleteRow(rowcnt-1);
			}

			LockUpdates = false;
		}

		/// <summary>
		/// Sets the number of columns (including fixed columns)
		/// </summary>
		/// <param name="cols"></param>
		public void SetColumnCount(int cols) 
		{
			if (cols < 0)
				return;

			int colcnt = colList.Count;

			if (cols == colList.Count)
				return;

			LockUpdates = true;
			if (cols > colcnt)
			{
				for (int i = 0; i < cols - colcnt; i++ )
					AddColumn();
			}
			else
			{
				for (int i = 0; i < colcnt - cols; i++ )
					DeleteColumn(colcnt-1);
			}

			LockUpdates = false;
		}

		/// <summary>
		/// Sets the number of fixed columns
		/// </summary>
		/// <param name="cols"></param>
		public void SetFixedColumnCount(int cols) 
		{
			if (cols < 0)
				return;

			int colcnt = colList.Count;

			LockUpdates = true;
			for (int i = 0; i < Math.Max(colcnt,GetFixedColumnCount()); i++ )
			{
				if (i >= cols)
					GetColumn(i).Header = false;
				else
					GetColumn(i).Header = true;
			}
			LockUpdates = false;
		}

		/// <summary>
		/// Sets the number of rows
		/// </summary>
		/// <param name="rows"></param>
		public void SetFixedRowCount(int rows) 
		{
			if (rows < 0)
				return;

			int rowcnt = rowList.Count;

			LockUpdates = true;
			for (int i = 0; i < Math.Max(rowcnt,GetFixedRowCount()); i++ )
			{
				if (i >= rows)
					GetRow(i).Header = false;
				else
					GetRow(i).Header = true;
			}
			LockUpdates = false;
		}
	


		////////////////////////////////////////////////////////////////////////////////////////////////
		////////////////////////////////////////////////////////////////////////////////////////////////
		// Sizing and position functions
		////////////////////////////////////////////////////////////////////////////////////////////////
		////////////////////////////////////////////////////////////////////////////////////////////////

		/// <summary>
		/// Get Row Height
		/// </summary>
		/// <param name="row"></param>
		/// <returns></returns>
		public int GetRowHeight(int row)
		{
			if (row < 0 || row > rowList.Count-1)
				throw new GridException("Row out of range");


			return rowList[row].Size;
		}
	
		/// <summary>
		/// Set the row height
		/// </summary>
		/// <param name="row"></param>
		/// <param name="height"></param>
		public void SetRowHeight(int row, uint height)
		{
			if (row < 0 || row > rowList.Count-1)
				throw new GridException("Row out of range");

			rowList[row].Size = (int) height;
		}

		/// <summary>
		/// Get Column Width
		/// </summary>
		/// <param name="col"></param>
		/// <returns></returns>
		public int GetColumnWidth(int col)
		{
			if (col < 0 || col > colList.Count-1)
				throw new GridException("Column out of range");

			return colList[col].Size;
		}

		/// <summary>
		/// Set Column Width
		/// </summary>
		/// <param name="col"></param>
		/// <returns></returns>
		public void SetColumnWidth(int col, int width)
		{
			if (col < 0 || col > colList.Count-1)
				throw new GridException("Column out of range");

			colList[col].Size = width;
		}	

		////////////////////////////////////////////////////////////////////////////////////////////////
		////////////////////////////////////////////////////////////////////////////////////////////////
		// General appearance and features
		////////////////////////////////////////////////////////////////////////////////////////////////
		////////////////////////////////////////////////////////////////////////////////////////////////
		
		/// <summary>
		/// Enables/disables tooltips
		/// </summary>
		public bool ToolTips
		{
			get
			{
				return toolTips;
			}
			set
			{
				toolTips = value;
				toolTip.Active = false;
			}
		}
		/// <summary>
		/// Gets/Sets whether or not grid can edit
		/// </summary>
		public bool EditMode
		{
			set
			{
				CancelEditing();
				isEditMode = value;
			}

			get 
			{
				return isEditMode;
			}
		}

		/// <summary>
		/// Sets/Gets grid line style
		/// </summary>
		public GridLineStyle GridLine
		{
			get
			{
				return gridLines;
			}

			set
			{
				gridLines = value;
				Invalidate();
			}			
		}


		/// <summary>
		/// Sets/Gets image style
		/// </summary>
		public ImageStyleType ImageStyle
		{
			get
			{
				return imageStyle;
			}

			set
			{
				imageStyle = value;
				Invalidate();
			}
		}

		/// <summary>
		/// Sets or Get the grid background image
		/// </summary>
		public Image Image
		{
			get
			{
				return this.image;
			}

			set
			{
				this.image = value;
				Invalidate();
			}
		}

		/// <summary>
		/// Sets/Gets the selection mode of the grid
		/// </summary>
		public SelectionModeType SelectionMode
		{
			get
			{
				return selectMode;
			}

			set
			{
				// Clear Previous selections
				if (selectMode == SelectionModeType.CellSelect)
					SelectCell(null);
				else
					SelectObject(null);

				selectMode = value;
			
				Invalidate();
			}
		}

		////////////////////////////////////////////////////////////////////////////////////////////////
		////////////////////////////////////////////////////////////////////////////////////////////////
		// Colors/Colours
		////////////////////////////////////////////////////////////////////////////////////////////////
		////////////////////////////////////////////////////////////////////////////////////////////////
		public Color GridLineColor
		{
			set
			{
				gridLineColor = value;
				Invalidate();
			}

			get
			{
				return gridLineColor;
			}
		}


		/// <summary>
		/// Returns a row collection
		/// </summary>
		/// <returns></returns>
		public GridObjectCollection GetRowList() 
		{	
			return rowList;
		}

		/// <summary>
		/// Returns a column collection
		/// </summary>
		/// <returns></returns>
		public GridObjectCollection GetColList() 
		{	
			return colList;
		}


		public bool LockUpdates
		{
			set
			{
				lockUpdates = value;
			}

			get
			{
				return lockUpdates;
			}
		}


		public int GetGridOpacity()
		{
			return Opacity;
		}

		////////////////////////////////////////////////////////////////////////////////////////////////
		////////////////////////////////////////////////////////////////////////////////////////////////
		// Operations
		////////////////////////////////////////////////////////////////////////////////////////////////
		////////////////////////////////////////////////////////////////////////////////////////////////



		public Grid()
		{
			// This call is required by the Windows.Forms Form Designer.
			InitializeComponent();

			vScrollBar.ValueChanged  += new EventHandler(vScrollBar_ValueChanged);
			hScrollBar.ValueChanged  += new EventHandler(hScrollBar_ValueChanged);

			this.SetStyle(ControlStyles.DoubleBuffer, true);
			this.SetStyle(ControlStyles.UserPaint, true);
			this.SetStyle(ControlStyles.AllPaintingInWmPaint, true);

			this.MouseWheel += new MouseEventHandler(Grid_MouseWheel);
			this.DoubleClick +=new EventHandler(Grid_DoubleClick);

			this.BackColor = SystemColors.Window;

			hScrollBar.Visible = false;
			vScrollBar.Visible = false;

			toolTip.AutoPopDelay = 5000;
			toolTip.InitialDelay = 1000;
			toolTip.ReshowDelay = 1000;


			this.Controls.Add(hScrollBar);
			this.Controls.Add(vScrollBar);
			this.Focus();		
		}


		/// <summary>
		/// Clean up any resources being used.
		/// </summary>
		protected override void Dispose( bool disposing )
		{
			if( disposing )
			{
				if( components != null )
					components.Dispose();
			}
			base.Dispose( disposing );
		}

		#region Component Designer generated code
		/// <summary>
		/// Required method for Designer support - do not modify 
		/// the contents of this method with the code editor.
		/// </summary>
		private void InitializeComponent()
		{
			// 
			// Grid
			// 
			this.AllowDrop = true;
			this.Name = "Grid";
			this.Size = new System.Drawing.Size(288, 200);
			this.Resize += new System.EventHandler(this.Grid_Resize);
			this.MouseUp += new System.Windows.Forms.MouseEventHandler(this.Grid_MouseUp);
			this.MouseMove += new System.Windows.Forms.MouseEventHandler(this.Grid_MouseMove);
			this.MouseDown += new System.Windows.Forms.MouseEventHandler(this.Grid_MouseDown);
			this.KeyDown += new KeyEventHandler(Grid_KeyDown);

		}
		#endregion

		protected override void OnPaint(PaintEventArgs pe)
		{
			if (!lockUpdates)
			{
				int rowOffset = 0;
				Rectangle clipRect = ClientRectangle;
				
				if (vScrollBar.Visible)
				{
					rowOffset = vScrollBar.Value;
					clipRect.Width -= vScrollBar.Width;

				}

				int colOffset = 0;
				if (hScrollBar.Visible)
				{
					colOffset = hScrollBar.Value;
					clipRect.Height -= hScrollBar.Height;
				}

				pe.Graphics.SetClip(clipRect);
				
				Draw(colOffset,rowOffset,pe.Graphics);
			}

			// Calling the base class OnPaint
			base.OnPaint(pe);
		}

		private void Grid_Resize(object sender, System.EventArgs e)
		{
			AdjustScrollbars();
		}


		public void SetCellType(int row, int col, Cell cell)
		{
			if (row < 0 || row > rowList.Count-1)
				throw new GridException("Row out of range");

			if (col < 0 || col > colList.Count-1)
				throw new GridException("Column out of range");

			Cell replaceCell = GetCell(row,col);

			// Remove old cell
			Row rowReplace  = replaceCell.row;
			rowReplace.ReplaceCell(replaceCell, cell);
		}

		public void AdjustScrollbars()
		{
			// No decide how many visible rows/columns are 
			// offscreen so we can calculate max values for scroll bars
			Size sizeDisplay = GetDisplaySize();
			Size sizeVirt = GetVirtualSize();
			bool bHVisible = false;
			bool bVVisible = false;

			if (sizeVirt.Width > ClientRectangle.Width)
			{
				hScrollBar.Top = ClientRectangle.Height - hScrollBar.Height;
				hScrollBar.Width = ClientRectangle.Width - 1;				
				sizeDisplay.Height -= hScrollBar.Height;
				HScrollMax = sizeVirt.Width;
				hScrollBar.Visible = true;
				bHVisible = true;
			}
			else
				hScrollBar.Visible = false;

			if (sizeVirt.Height > sizeDisplay.Height)
			{
				vScrollBar.Left = ClientRectangle.Width - vScrollBar.Width;
				vScrollBar.Height = ClientRectangle.Height - 1;
				sizeDisplay.Width -= vScrollBar.Width;
				VScrollMax = sizeVirt.Height;
				vScrollBar.Visible = true;
				bVVisible = true;
			}
			else
				vScrollBar.Visible = false;

			hScrollBar.Minimum = 0;
			hScrollBar.Maximum = HScrollMax;

			vScrollBar.Minimum = 0;
			vScrollBar.Maximum = VScrollMax;
			
			hScrollBar.SmallChange = hScrollBar.LargeChange = HScrollMax > 0 ? sizeDisplay.Width  : 0;
			vScrollBar.SmallChange = vScrollBar.LargeChange = VScrollMax > 0 ? sizeDisplay.Height : 0;

			if (bHVisible && bVVisible)
			{
				hScrollBar.Width = ClientRectangle.Width - vScrollBar.Width;
				vScrollBar.Height = ClientRectangle.Height - hScrollBar.Height;
			}
		}

		#region *** Grid Functions ***

		/// <summary>
		/// Get the size of the grid
		/// </summary>
		/// <returns></returns>
		virtual public Size GridSize()
		{
			Size sz = new Size();
			foreach (Row r in rowList)
				if (r.Visible)
					sz.Height += r.Size;

			foreach (Column c in colList)
				if (c.Visible)
					sz.Width += c.Size;

			return sz;
		}


		/// <summary>
		/// Adds a row using the default value
		/// </summary>
		/// <returns></returns>
		virtual public Row AddRow()
		{
			Row row = new Row(this, (int) defaultRowHeight);
			rowList.Add(row);
			AdjustScrollbars();
			Invalidate();
			return row;
		}

		/// <summary>
		/// Adds a row using the a predefined height
		/// </summary>
		/// <param name="height"></param>
		/// <returns></returns>
		virtual public Row AddRow(uint height)
		{
			Row row = new Row(this, (int) height);
			rowList.Add(row);
			AdjustScrollbars();
			Invalidate();
			return row;
		}

		/// <summary>
		/// Adds a column using a default width
		/// </summary>
		/// <returns></returns>
		virtual public Column AddColumn()
		{
			if (rowList.Count == 0)
				AddRow();
			
			Column col = new Column(this, defaultColWidth);
			colList.Add(col);

			// Iterator now rows adding new cell
			foreach (Row r in rowList)
				r.AddCell(col);

			AdjustScrollbars();
			Invalidate();
			
			return col;
		}

		/// <summary>
		/// Adds a column using a width
		/// </summary>
		/// <param name="width"></param>
		/// <returns></returns>
		virtual public Column AddColumn(int width)
		{
			if (rowList.Count == 0)
				AddRow();
			
			Column col = new Column(this, (uint) width);
			colList.Add(col);

			// Iterator now rows adding new cell
			foreach (Row r in rowList)
				r.AddCell(col);

			AdjustScrollbars();
			Invalidate();
		
			return col;
		}

		/// <summary>
		/// Gets a row object from a row number
		/// </summary>
		/// <param name="r"></param>
		/// <returns></returns>
		virtual public GridObject GetRow(int r)
		{
			if (r > rowList.Count-1)
				throw new GridException("Row out of range");				

			return rowList[r];
		}

		/// <summary>
		/// Gets a column from a predefined column
		/// </summary>
		/// <param name="c"></param>
		/// <returns></returns>
		virtual public GridObject GetColumn(int c)
		{
			if (c > colList.Count-1)
				throw new GridException("Column out of range");				

			return colList[c];
		}

		/// <summary>
		/// Draws the grid
		/// </summary>
		/// <param name="xOffset"></param>
		/// <param name="yOffset"></param>
		/// <param name="g"></param>
		private void Draw(int xOffset, int yOffset, Graphics g)
		{
			// Give the canvas a change to paint
			DrawBackground(g);

			int y = 0;
			int yCnt=0;
			foreach (Row r in rowList)
			{
				if (r.Visible)
				{
					if (!r.Header && yCnt < yOffset)
					{
						yCnt+=r.Size;
						continue;
					}

					if (y >= ClientRectangle.Y && y <= ClientRectangle.Bottom)
						r.Draw(xOffset, y, g);

					y += r.Size;
				}
			}			
		}


		/// <summary>
		/// Override to draw grid background
		/// </summary>
		/// <param name="g"></param>
		virtual public void DrawBackground(Graphics g)
		{
			if (image != null)
				DrawImage(g);
			else
				g.FillRectangle(new SolidBrush(this.BackColor), 0,0, ClientRectangle.Width, ClientRectangle.Height);
		}

		private void DrawImage(Graphics g)
		{
			switch (imageStyle)
			{
				case ImageStyleType.Center:
					break;
				case ImageStyleType.Stretch:
					g.DrawImage(image,ClientRectangle, new Rectangle(0,0,image.Width, image.Height), GraphicsUnit.Pixel);
					break;
				case ImageStyleType.Tile:
				{
					for (int x=0;x < ClientRectangle.Width; x += image.Width+1)
					{
						for (int y=0;y < ClientRectangle.Height; y += image.Height+1)
							g.DrawImage(image,x,y, new Rectangle(0,0,image.Width, image.Height), GraphicsUnit.Pixel);
					}
					break;
				}
			}
		}

		/// <summary>
		/// Gets a cell at a given row and column
		/// </summary>
		/// <param name="r"></param>
		/// <param name="c"></param>
		/// <returns></returns>
		public Cell GetCell(int r, int c)
		{
			Cell cell = null;
			try
			{
				Row row =  (Row) GetRow(r);
				cell =  row.GetCell(c);

			}
			catch (GridException ge)
			{
				throw new GridException("GetCell out of range" + ge.Message);
			}

			return cell;
		}

		public Cell GetCell(int r, int c, bool bCreate)
		{
			Cell cell = null;
			try
			{
				Row row =  (Row)GetRow(r);
				cell =  row.GetCell(c);
			}
			catch (GridException)
			{
				if (bCreate)
				{
					// For virtual grid ops
				}
				else // Return a copy
				{
					cell = CreateCell((Row) rowList[r], (Column) colList[c]);

				}
			}

			return cell;
		}

		public void DeleteRow(int row)
		{
			if (row < 0 || row > rowList.Count-1)
				throw new GridException("Row out of range");				

			rowList.RemoveAt(row);
			Invalidate();
		}

		public void DeleteColumn(int col)
		{
			if (col < 0 || col > colList.Count-1)
				throw new GridException("Column out of range");				

			colList.RemoveAt(col);

			foreach (Row row in rowList)
				row.cellList.RemoveAt(col);

			Invalidate();

		}

		private void Grid_MouseMove(object sender, System.Windows.Forms.MouseEventArgs e)
		{
		
			if (sizerCell.IsDragging)
			{
				Graphics g = Graphics.FromHwnd(this.Handle);
				sizerCell.Drag(g, new Point(e.X, e.Y));
			}
			else
			{
				Point pt = new Point(e.X, e.Y);
				GridObject gridObject;
				HandleHitTest(ref pt, out gridObject);
			}					

			Cell cell = GetCellFromPoint(new Point(e.X, e.Y));
			if (ToolTips)
			{
				if (cell != null)
				{
					if (tipCell != cell)
					{
						tipCell = cell;
						toolTip.SetToolTip(this, tipCell.TipText);			
					}
				}
				else
					toolTip.SetToolTip(this, "");			
			}
		}

		/// <summary>
		/// Internal stuff for handling hit testing
		/// </summary>
		/// <param name="pt"></param>
		/// <param name="selectObject"></param>
		/// <returns></returns>
		protected HitCode HandleHitTest(ref Point pt, out GridObject selectObject)
		{
			// Check list
			// a.	Row/Col Resize Cursor
			// b.	Fire OnCellHitTest
			selectObject = null;

			this.Cursor = Cursors.Arrow;
			int xOffset = 0;
			int yOffset = 0;
			int nSkipCnt;

			nSkipCnt = 0;
			int nRowPos = this.GetFirstVisibleRowPosition();

			for (int r=0;r < rowList.Count;r++)
			{
				Cell cell = GetCell(r,0);

				if (!cell.row.Visible)
					continue;

				if (!cell.row.Header && nSkipCnt < nRowPos)
				{
					nSkipCnt+=cell.row.Size;
					continue;
				}

				yOffset += cell.row.Size;

				Rectangle rcCell = new Rectangle(0,yOffset, (int) cell.col.Size, (int) cell.row.Size);
							
				
				if (pt.X >= rcCell.X && pt.X <= rcCell.Right)
				{
					if (pt.Y >= rcCell.Y-Tolerence && pt.Y <= rcCell.Y+Tolerence)
					{
						this.Cursor = Cursors.SizeNS;							
						pt.Y = yOffset - cell.row.Size;
						selectObject = cell.row;

						if (!selectObject.CanResize)
						{
							this.Cursor = Cursors.Arrow;
							return HitCode.hitNone;
						}

						return HitCode.hitRowEdge;
					}
				}
			}

			yOffset = 0;
			nSkipCnt = 0;

			int nColPos = this.GetFirstVisibleColumnPosition();

			for (int c=0;c < colList.Count;c++)
			{
				Cell cell = GetCell(0,c);
				if (!cell.col.Visible)
					continue;

				if (!cell.col.Header && nSkipCnt < nColPos)
				{
					nSkipCnt+=cell.col.Size;
					continue;
				}


				xOffset += cell.col.Size;
				Rectangle rcCell = new Rectangle(xOffset,0, (int) cell.col.Size, (int) cell.row.Size);					
				
				if (pt.Y >= rcCell.Y && pt.Y <= rcCell.Bottom)
				{
					if (pt.X >= rcCell.X-Tolerence && pt.X <= rcCell.X+Tolerence)
					{
						this.Cursor = Cursors.SizeWE;
						pt.X =  xOffset - cell.col.Size;
						selectObject = cell.col;

						if (!selectObject.CanResize)
						{
							this.Cursor = Cursors.Arrow;
							return HitCode.hitNone;
						}

						return HitCode.hitColumnEdge;
					}
				}
			}

			return HitCode.hitNone;
		}

		private void Grid_MouseDown(object sender, System.Windows.Forms.MouseEventArgs e)
		{
			// Cancel Editing
			CancelEditing();

			Point pt = PointToClient(Cursor.Position);
			Point ptSave = pt;
			GridObject	gridObj;

			switch (HandleHitTest(ref pt, out gridObj))
			{
				case HitCode.hitColumnEdge:
				{
					sizerCell.BeginDrag(this, gridObj, ptSave , pt, CellSizer.SizeType.HorzSize);
					return;
				}
				case HitCode.hitRowEdge:
				{
					sizerCell.BeginDrag(this, gridObj, ptSave ,pt, CellSizer.SizeType.VertSize);
					return;
				}
			}

			switch (selectMode)
			{
				case SelectionModeType.CellSelect:
				{

					Cell cell = GetCellFromPoint(pt);
					if (cell == null)
						return;
					
					if (cell.row.Header || cell.col.Header)
						return;
					SelectCell(cell);
					break;
				}
				case SelectionModeType.RowSelect:
				{
					Cell cell = GetCellFromPoint(pt);
					if (cell == null)
						return;

					if (cell.col.Header)
						return;

					SelectObject(cell.row);
					break;
				}
				case SelectionModeType.ColumnSelect:
				{
					Cell cell = GetCellFromPoint(pt);
					if (cell == null)
						return;

					if (cell.row.Header)
						return;

					SelectObject(cell.col);
					break;
				}
			}
		}

		public void SelectObject(GridObject obj)
		{		
			if (selectedObject != null)
				selectedObject.Selected = false;

			if (obj != null)
			{	
				obj.Selected = true;
			}
			selectedObject = obj;
		}

		public void SelectCell(Cell cell)
		{
			if (selectedCell != null)
			{
				selectedCell.Selected = false;
				selectedCell.row.Selected = false;
				selectedCell.col.Selected = false;
			}

			if (cell != null)
			{
				if (cell.row.Header || cell.col.Header)
					return;
	
				cell.Selected = true;
                if(OnCellSelect != null)
                {
                    OnCellSelect(cell);
                }
			}
			selectedCell = cell;
			if (selectedCell != null)
			{
				selectedCell.row.Selected = true;
				selectedCell.col.Selected = true;
			}
		}

		public void InvalidateCell(Cell cell, bool forceUpdate)
		{
			Invalidate(GetCellRect(cell));
		}

		public void Invalidate(GridObject gridObj)
		{
			// Need revisit this as we definitely should really opn querying object type
			switch (gridObj.GetObjectType())
			{
				case GridObject.ObjectType.Row:
					Invalidate(GetRowRect(gridObj));
					break;
				
				case GridObject.ObjectType.Column:
					Invalidate(GetColumnRect(gridObj));
					break;				
			}			
		}
	
		public Rectangle GetRowRect(GridObject rowComp)
		{
			int yOffset = 0;

			int nRowPos = this.GetFirstVisibleRowPosition();
			int nColPos = this.GetFirstVisibleColumnPosition();

			// Locate Row / Then Column
			Rectangle rcCell = new Rectangle();
			foreach (Row row in rowList)
			{
				if (!row.Visible)
					continue;

				if (nRowPos>0)
				{
					nRowPos--;
					continue;
				}
				
				if (row == rowComp)
				{
					// Found Correct col
					rcCell.Y = yOffset;
					rcCell.Height = row.Size;

					foreach (Column col in colList)
					{
						if (col.Visible)
						{
							if (nColPos>0)
							{
								nColPos--;
								continue;
							}
							rcCell.Width += col.Size;
						}
					}
				}
				yOffset += row.Size;
			}

			return rcCell; // Didn't find it so show as empty
		}


		public Rectangle GetCellRect(Cell cell)
		{
			int yOffset = 0;

			int nRowPos = this.GetFirstVisibleRowPosition();

			int nRowSkipCnt = 0;

			// Locate Row / Then Column
			foreach (Row row in rowList)
			{
				if (!row.Visible)
					continue;

				if (!row.Header && nRowSkipCnt < nRowPos)
				{
					nRowSkipCnt+=row.Size;
					continue;
				}

				
				if (row == cell.row)
				{
					// Found Correct col
					int nColPos = this.GetFirstVisibleColumnPosition();
					Rectangle rcCell = new Rectangle(0,yOffset, 0, (int) row.Size);

					int nColSkipCnt = 0;

					foreach (Column col in colList)
					{
						if (!col.Visible)
							continue;
						
						if (!col.Header && nColSkipCnt < nColPos)
						{
							nColSkipCnt += col.Size;
							continue;
						}

						rcCell.Width = col.Size;
						if (col == cell.col)
						{
							return rcCell;
						}
						
						rcCell.Offset((int) col.Size,0);
					}
				}
				yOffset += row.Size;
			}

			return new Rectangle(); // Didn't find it so show as empty
		}

		public Rectangle GetColumnRect(GridObject colComp)
		{
			int xOffset = 0;

			int nRowPos = this.GetFirstVisibleRowPosition();
			int nColPos = this.GetFirstVisibleColumnPosition();

			// Locate Row / Then Column
			Rectangle rcCell = new Rectangle();
			foreach (Column col in colList)
			{
				if (!col.Visible)
					continue;

				if (nColPos>0)
				{
					nColPos--;
					continue;
				}
				
				if (col == colComp)
				{
					// Found Correct col
					rcCell.X = xOffset;
					rcCell.Width= col.Size;
					foreach (Row row in rowList)
					{
						if (row.Visible)
						{
							if (nRowPos>0)
							{
								nRowPos--;
								continue;
							}

							rcCell.Height += row.Size;
						}
					}
				}
				xOffset += col.Size;
			}

			return rcCell; // Didn't find it so show as empty
		}

		private void Grid_MouseUp(object sender, System.Windows.Forms.MouseEventArgs e)
		{
			sizerCell.EndDrag(PointToClient(Cursor.Position));
		}

		private Column GetFirstNonHeaderColumn()
		{		
			foreach (Column c in colList)
			{
				if (c.Visible)
				{
					if (c.Header)
						continue;

					return c;
				}
			}

			return null;				
		}

		private int GetRowHeaderSize()
		{		
			int nSize=0;
			foreach (Row r in rowList)
			{
				if (r.Visible)
				{
					if (r.Header)
					{
						nSize+=r.Size;
						continue;
					}

					break;
				}
			}

			return nSize;
		}

		public int GetRowHeaderCount()
		{		
			int cnt=0;
			foreach (Row r in rowList)
			{
				if (r.Visible)
				{
					if (r.Header)
					{
						cnt++;
						continue;
					}

					break;
				}
			}

			return cnt;
		}

		public int GetColumnHeaderCount()
		{		
			int cnt=0;
			foreach (Column c in colList)
			{
				if (c.Visible)
				{
					if (c.Header)
					{
						cnt++;
						continue;
					}

					break;
				}
			}
			return cnt;
		}




		private int GetColumnHeaderSize()
		{		
			int nSize=0;
			foreach (Column c in colList)
			{
				if (c.Visible)
				{
					if (c.Header)
					{
						nSize+=c.Size;
						continue;
					}

					break;
				}
			}

			return nSize;

		}

		private Row GetFirstNonHeaderRow()
		{		
			foreach (Row r in rowList)
			{
				if (r.Visible)
				{
					if (r.Header)
						continue;

					return r;
				}
			}

			return null;				
		}

		private int GetFirstVisibleColumnPosition()
		{
			int i = 0;
			if (hScrollBar.Visible == false)
				return i;

			return hScrollBar.Value;
		}

		private int GetFirstVisibleRowPosition()
		{
			int i = 0;
			if (vScrollBar.Visible == false)
				return i;

			return vScrollBar.Value;
		}
		
		/// <summary>
		/// Gets the bounding grid rectangle
		/// </summary>
		/// <returns></returns>
		public Size GetBounds()
		{
			Size sz = GridSize();
			if (sz.Width > ClientRectangle.Width)
				sz.Width = ClientRectangle.Width - (vScrollBar.Visible ? vScrollBar.Width : 0);
		
			if (sz.Height > ClientRectangle.Height)
				sz.Height = ClientRectangle.Height- (hScrollBar.Visible ? hScrollBar.Height : 0);

			return sz;
		}

		/// <summary>
		/// Sets or gets the Opacity value
		/// </summary>
		public int GridOpacity
		{
			get 
			{
				double d = (Opacity / 2.55) + 0.5;
				return (int) d;
			}

			set
			{
				Opacity = (int) ((((double) value * 0.01) * 255) + 0.5);
				Invalidate();
			}
		}

		/// <summary>
		/// Gets a cell from a mouse position
		/// </summary>
		/// <param name="pt"></param>
		/// <returns></returns>
		/// 
		public Cell GetCellFromPoint(Point pt)
		{
			int yOffset = 0;

			int nRowPos = this.GetFirstVisibleRowPosition();

			// Locate Row / Then Column
			int nRowSkipCnt=0;
			for (int r=0;r < rowList.Count;r++)
			{
				Row row = (Row) GetRow(r);
				if (!row.Visible)
					continue;

				if (!row.Header && nRowSkipCnt < nRowPos)
				{
					nRowSkipCnt+=row.Size;
					continue;
				}
			
				if (pt.Y >= yOffset && pt.Y <= yOffset + row.Size)
				{

					int nColPos = this.GetFirstVisibleColumnPosition();
					int nColSkipCnt=0;
					// Found Correct Row
					Rectangle rcCell = new Rectangle(0,yOffset, 0, (int)row.Size);
					foreach (Column col in colList)
					{
						if (!col.Visible)
							continue;

						if (!col.Header && nColSkipCnt < nColPos)
						{
							nColSkipCnt += col.Size;
							continue;
						}

						rcCell.Width = col.Size;
						if (pt.X >= rcCell.X && pt.X <= rcCell.Right)
						{
							return GetCell(rowList.IndexOf(row), colList.IndexOf(col), false);
						}
						rcCell.Offset((int) col.Size,0);
						
					}
				}

				yOffset += row.Size;
			}
			return null;
		}

		public Row GetRowFromPoint(Point pt)
		{
			int yOffset = 0;

			int nRowPos = this.GetFirstVisibleRowPosition();

			// Locate Row / Then Column
			foreach (Row row in rowList)
			{
				if (!row.Visible)
					continue;

				if (!row.Header && nRowPos>0)
				{
					nRowPos-=row.Size;
					continue;
				}
				
				if (pt.Y >= yOffset && pt.Y <= yOffset + row.Size)
					return row;

				yOffset += row.Size;
			}

			return null;
		}

		public Column GetColumnFromPoint(Point pt)
		{
			int xOffset = 0;

			int nColPos = this.GetFirstVisibleColumnPosition();

			// Locate Row / Then Column
			foreach (Column col in colList)
			{
				if (!col.Visible)
					continue;

				if (!col.Header && nColPos>0)
				{
					nColPos-=col.Size;
					continue;
				}
				
				if (pt.X >= xOffset && pt.X <= xOffset + col.Size)
					return col;

				xOffset += col.Size;
			}

			return null;
		}

		public Size GetDisplaySize()
		{
			Size sz = ClientRectangle.Size;
			
			foreach (Row r in rowList)
			{
				if (r.Header && r.Visible)
					sz.Height -= r.Size;
			}

			foreach (Column c in colList)
			{
				if (c.Visible && c.Header)
					sz.Width -= c.Size;
			}

			return sz;
		}

		public Size GetVirtualSize()
		{
			Size sz = new Size();
			
			foreach (Row r in rowList)
			{
				if (r.Visible && !r.Header)
					sz.Height += r.Size;
			}

			foreach (Column c in colList)
			{
				if (c.Visible && !c.Header)
					sz.Width += c.Size;
			}

			return sz;
		}

		private void vScrollBar_ValueChanged(object sender, EventArgs e)
		{
			CancelEditing();
			Invalidate();
		}

		private void hScrollBar_ValueChanged(object sender, EventArgs e)
		{
			CancelEditing();
			Invalidate();
		}

		public void EnsureVisible(Row r)
		{
			int offsetFwd = 0;
			int viewCnt = 0;
			int objPos = this.GetFirstVisibleRowPosition();
			int topPos = 0;
			bool bTop = false;

			// Locate Row / Then Column
			foreach (Row row in rowList)
			{
				if (!row.Visible)
					continue;

				if (!row.Header)
					offsetFwd += row.Size;

				// This deal with a row
				if (objPos >0)
				{
					objPos -= row.Size;
					if (row == r)
					{
						vScrollBar.Value = offsetFwd - row.Size;						
						return;
					}
					continue;
				}

				if (!bTop)
				{
					topPos = offsetFwd;
					bTop = true;
				}


				// Visible rows
				viewCnt += row.Size;

				if (r == row)
				{
					if (viewCnt <= GetRowHeaderSize())
					{
						vScrollBar.Value -= row.Size;
						return;
					}

					if (viewCnt >= ClientRectangle.Height)
						vScrollBar.Value = topPos + row.Size ;

					return;
				}
			}
		}

		public void EnsureVisible(Column c)
		{
			int offsetFwd = 0;
			int viewCnt = 0;
			int objPos = this.GetFirstVisibleColumnPosition();
			int topPos = 0;
			bool bTop = false;

			// Locate Row / Then Column
			foreach (Column col in colList)
			{
				if (!col.Visible)
					continue;

				if (!col.Header)
					offsetFwd += col.Size;

				// This deal with a row
				if (objPos >0)
				{
					objPos -= col.Size;
					if (col == c)
					{
						hScrollBar.Value = offsetFwd - col.Size;						
						return;
					}
					continue;
				}

				if (!bTop)
				{
					topPos = offsetFwd;
					bTop = true;
				}


				// Visible rows
				viewCnt += col.Size;

				if (c == col)
				{
					if (viewCnt <= GetColumnHeaderSize())
					{
						hScrollBar.Value -= col.Size;
						return;
					}

					if (viewCnt >= ClientRectangle.Width)
						hScrollBar.Value = topPos + col.Size ;

					return;
				}
			}
		}

		public Rectangle GetVisualRect()
		{
			Rectangle r = ClientRectangle;
			if (vScrollBar.Visible == true)
				r.Width -= vScrollBar.Width;

			if (hScrollBar.Visible == true)
				r.Height -= hScrollBar.Height;

			return r;
		}


	
		public void EnsureVisible(Cell cell)
		{
			EnsureVisible(cell.row);
			EnsureVisible(cell.col);
		}

		/// <summary>
		/// Support for mousewheel 
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		private void Grid_MouseWheel(object sender, MouseEventArgs e)
		{
			if (vScrollBar.Visible == false)
				return;

			if (e.Delta<0)			
			{
				if (vScrollBar.Value + GetVisibleRow().Size + GetDisplaySize().Height < vScrollBar.Maximum)
					vScrollBar.Value +=	GetVisibleRow().Size; 
				else
					vScrollBar.Value = vScrollBar.Maximum - (GetDisplaySize().Height - + GetVisibleRow().Size);
			}
			else
			{
				if (e.Delta>0)
				{
					if (vScrollBar.Value - GetVisibleRow().Size > 0)
						vScrollBar.Value -=	GetVisibleRow().Size; 
					else
						vScrollBar.Value = 0;
				}
			}
		}


		private Row GetVisibleRow()
		{
			int y=0;
			Row row = null;
			foreach(Row r in rowList)
			{
				if (r.Visible)
				{
					if (y >= vScrollBar.Value)
						return r;

					y+=r.Size;
				}
			}
			return row;
		}

		/// <summary>
		/// To do implement double click on cell header (row/col autosize)
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		private void Grid_DoubleClick(object sender, EventArgs e)
		{
			Point pt = PointToClient(Cursor.Position);
			GridObject gridObject;
			switch (HandleHitTest(ref pt, out gridObject))
			{
				case HitCode.hitColumnEdge:
				{
					break;
				}
				case HitCode.hitRowEdge:
				{
					break;
				}

				default:
					BeginEditing();
					break;
			}


		}

        public Cell SelectedCell {
            get
            {
                return selectedCell;
            }
        }

		private void SelectionUp()
		{
			// Not Applicable
			if (selectMode == SelectionModeType.ColumnSelect)
				return;

			if (selectMode == SelectionModeType.RowSelect && selectedObject != null)
			{
				if (rowList.IndexOf(selectedObject) <= rowList.IndexOf(GetFirstNonHeaderRow()))
					return;

				selectedObject.Selected = false;
				GridObject gridObj = rowList.GetPrevObject(selectedObject);

				SelectObject(gridObj);
				EnsureVisible((Row) gridObj);
			}

			if (selectMode == SelectionModeType.CellSelect && selectedCell != null)
			{
				if (rowList.IndexOf(selectedCell.row) <= rowList.IndexOf(GetFirstNonHeaderRow()))
					return;

				GridObject gridObj = rowList.GetPrevObject(selectedCell.row);

				Cell cell = GetCell(rowList.IndexOf(gridObj), colList.IndexOf(selectedCell.col));

				SelectCell(cell);
				EnsureVisible(cell);
				
			}
		}

		private void SelectionDown()
		{
			// Not Applicable
			if (selectMode == SelectionModeType.ColumnSelect)
				return;

			if (selectMode == SelectionModeType.RowSelect && selectedObject != null)
			{
				selectedObject.Selected = false;
				GridObject gridObj = rowList.GetNextObject(selectedObject);

				SelectObject(gridObj);
				EnsureVisible((Row) gridObj);

			}

			if (selectMode == SelectionModeType.CellSelect && selectedCell != null)
			{
				GridObject gridObj = rowList.GetNextObject(selectedCell.row);

				Cell cell = GetCell(rowList.IndexOf(gridObj), colList.IndexOf(selectedCell.col));

				SelectCell(cell);
				EnsureVisible(cell);			
			}
		}

		private void SelectionLeft()
		{
			// Not Applicable
			if (selectMode == SelectionModeType.RowSelect)
				return;

			if (selectMode == SelectionModeType.ColumnSelect && selectedObject != null)
			{
				if (colList.IndexOf(selectedObject) <= colList.IndexOf(GetFirstNonHeaderColumn()))
					return;

				selectedObject.Selected = false;
				GridObject gridObj = colList.GetPrevObject(selectedObject);

				SelectObject(gridObj);
				EnsureVisible((Column) gridObj);
			}

			if (selectMode == SelectionModeType.CellSelect && selectedCell != null)
			{

				if (colList.IndexOf(selectedCell.col) <= colList.IndexOf(GetFirstNonHeaderColumn()))
					return;

				GridObject gridObj = colList.GetPrevObject(selectedCell.col);

				Cell cell = GetCell(rowList.IndexOf(selectedCell.row), colList.IndexOf(gridObj));

				SelectCell(cell);
				EnsureVisible(cell);				
			}
		}

		private void SelectionRight()
		{
			if (selectMode == SelectionModeType.RowSelect)
				return;

			if (selectMode == SelectionModeType.ColumnSelect)
			{
				selectedObject.Selected = false;
				GridObject gridObj = colList.GetNextObject(selectedObject);

				SelectObject(gridObj);
				EnsureVisible((Column) gridObj);
			}

			if (selectMode == SelectionModeType.CellSelect && selectedCell != null)
			{

				GridObject gridObj = colList.GetNextObject(selectedCell.col);

				Cell cell = GetCell(rowList.IndexOf(selectedCell.row), colList.IndexOf(gridObj));

				SelectCell(cell);
				EnsureVisible(cell);			
			}
		}

		protected override bool ProcessDialogKey(Keys keyData)
		{
			switch (keyData)
			{
				case Keys.Down:
					SelectionDown();
					break;
				case Keys.Up:
					SelectionUp();
					break;
				case Keys.Left:
					SelectionLeft();
					break;
				case Keys.Right:
					SelectionRight();
					break;
				case Keys.Tab:
					TabForward();
					break;
			}
			return base.ProcessDialogKey (keyData);
		}


		protected void TabForward()
		{
			if (selectMode != SelectionModeType.CellSelect)
				return;

			if (selectedCell == null)
				return;

			if (colList.Count == 0 || rowList.Count == 0)
				return;

			if (selectedCell.col == colList[colList.Count-1])
			{
				GridObject gridObj = rowList.GetNextObject(selectedCell.row);
				if (gridObj != null)
				{
					Cell cell = GetCell(rowList.IndexOf(gridObj), colList.IndexOf(GetFirstNonHeaderColumn()));
					SelectCell(cell);
					EnsureVisible(cell);
				}
			}
			else
			{
				GridObject gridObj = colList.GetNextObject(selectedCell.col);
				if (gridObj != null)
				{
					Cell cell = GetCell(rowList.IndexOf(selectedCell.row), colList.IndexOf(gridObj));
					SelectCell(cell);
					EnsureVisible(cell);
				}
			}
		}

		protected void TabBackward()
		{
			if (selectMode != SelectionModeType.CellSelect)
				return;

			if (selectedCell == null)
				return;

			if (colList.Count == 0 || rowList.Count == 0)
				return;

			if (selectedCell.col == colList[0])
			{
				GridObject gridObj = rowList.GetPrevObject(selectedCell.row);
				if (gridObj != null)
				{
					Cell cell = GetCell(rowList.IndexOf(gridObj), colList.Count-1);
					SelectCell(cell);
					EnsureVisible(cell);
				}
			}
			else
			{
				GridObject gridObj = colList.GetPrevObject(selectedCell.col);
				if (gridObj != null)
				{
					Cell cell = GetCell(rowList.IndexOf(selectedCell.row), colList.IndexOf(gridObj));
					SelectCell(cell);
					EnsureVisible(cell);
				}
			}
		}

		private void Grid_KeyDown(object sender, KeyEventArgs e)
		{
			if (e.KeyData == Keys.F2)
			{
				BeginEditing();
			}
		}


		private void BeginEditing()
		{
			if (!isEditMode)
				return;

			if (selectMode != SelectionModeType.CellSelect)
				return;

			if (selectedCell == null)
				return;

			if (selectedCell.CanEdit == false)
				return;


			// Bring into view
			EnsureVisible(selectedCell.row);
			EnsureVisible(selectedCell.col);


			selectedCell.BeginEdit();	

			Rectangle rect = GetCellRect(selectedCell);

			textBox = new CellEditor(this, selectedCell,rect);
			this.Controls.Add(textBox);
			textBox.Focus();

		}

		private void CancelEditing()
		{
			if (textBox != null)
			{
				textBox.Dispose();
				textBox = null;
			}
		}


		/// <summary>
		/// Override to construct your own cells
		/// </summary>
		/// <param name="r"></param>
		/// <param name="c"></param>
		/// <returns></returns>
		virtual public Cell CreateCell(Row r, Column c)
		{
			// Normal Circumstances create a cell
			return new Cell(r,c);
		}

	}


	///////////////////////////////////////////////////////////////////////////////
	///////////////////////////////////////////////////////////////////////////////

	public class GridException : Exception
	{
		public GridException(string message)
			: base(message)
		{
		}

		public GridException(string message, Exception inner)
			: base(message, inner)
		{
		}
	}

	#endregion
}
