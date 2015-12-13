using System;
using System.Windows.Forms;
using System.Drawing;
using System.Drawing.Drawing2D;


namespace GridCtrl
{
	/// <summary>
	/// Summary description for CellSizer.
	/// </summary>
	public class CellSizer
	{
		public enum SizeType
		{
			HorzSize,
			VertSize,
		}

		protected class Line
		{
			protected	Point	ptStart;
			protected	Point	ptEnd;

			public Line(Point ptStart, Point ptEnd)
			{
				this.ptStart = ptStart;
				this.ptEnd	 = ptEnd;
			}

			public void Draw(Grid grid)
			{
				
				ControlPaint.DrawReversibleLine(grid.PointToScreen(ptStart), grid.PointToScreen(ptEnd), SystemColors.Window);
			}      
		}

		protected	Grid		grid = null;
		protected	Point		ptStart;
		protected	SizeType	sizeType;
		protected	bool		isDragging = false;
		protected	Line		lineSave = null;
		protected	GridObject	gridObj;


		public CellSizer()
		{
		}

		public void BeginDrag(Grid ctrl,GridObject gridObj, Point pt, Point ptLimit, SizeType type)
		{
			grid			= ctrl;
			isDragging		= true;
			ctrl.Capture	= true;
			ptStart			= ptLimit;
			sizeType		= type;
			this.gridObj	= gridObj;
			
					
			Drag(grid.CreateGraphics(), pt);
		}

		public void Drag(Graphics g, Point pt)
		{

			if (lineSave != null)
			{
				lineSave.Draw(grid);
				lineSave = null;
			}

			if (sizeType == SizeType.VertSize)
			{
				if (IsDragging)
				{
					if (pt.Y < 0)
						pt.Y = 0;

					if (pt.Y > grid.GetVisualRect().Height)
						pt.Y = grid.GetVisualRect().Height;

					if (pt.Y < ptStart.Y)
						pt.Y = ptStart.Y;

					if (pt.X < ptStart.X)
						pt.X = ptStart.X;

					lineSave = new Line(new Point(0,pt.Y), new Point(grid.GetBounds().Width,pt.Y));
					lineSave.Draw(grid);
				}
			}

			if (sizeType == SizeType.HorzSize)
			{
				if (IsDragging)
				{

					if (pt.X < 0)
						pt.X = 0;

					if (pt.X > grid.GetVisualRect().Width)
						pt.X = grid.GetVisualRect().Width;

					if (pt.X < ptStart.X)
						pt.X = ptStart.X;

					lineSave = new Line(new Point(pt.X,0), new Point(pt.X,grid.GetBounds().Height));
					lineSave.Draw(grid);
				}
			}

		}

		public void EndDrag(Point pt)
		{
			if (IsDragging)
			{
				isDragging = false;
				grid.Capture = false;
				Drag(grid.CreateGraphics(), pt);

				if (sizeType == SizeType.VertSize && pt.Y >= ptStart.Y)
					gridObj.Size = (pt.Y - ptStart.Y);
				
				if (sizeType == SizeType.HorzSize && pt.X >= ptStart.X)
					gridObj.Size =  (pt.X - ptStart.X);
			}
			
	}

		public bool IsDragging
		{
			get
			{
				return isDragging;
			}
		}
	}
}
