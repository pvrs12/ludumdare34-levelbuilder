using System;
using System.Drawing;
using System.Collections;

namespace GridCtrl
{
	/// <summary>
	/// Row provides implementation for a grid row
	/// </summary>
	/// <remarks>
	/// <para>
	/// You should inherit from Row if you want to change the appearance or function of
	/// a grid row
	/// </para>
	/// </remarks>
	public class Row : GridObject
	{
		public		CellCollection		cellList				= new CellCollection();

		public override ObjectType GetObjectType()
		{
			return GridObject.ObjectType.Row;
		}

		// the destructor
		~Row()
		{		
			// call Dispose with false.  Since we're in the
			// destructor call, the managed resources will be
			// disposed of anyways.
			Dispose(false);
		}

		protected override void Dispose(bool disposeManagedResources)
		{
			// process only if mananged and unmanaged resources have
			// not been disposed of.
			if (!this.disposed)
			{
				if (disposeManagedResources)
				{
					foreach (Cell c in cellList)
					{
						c.Image = null;
					}
				}
				disposed=true;
			}
			else
			{
				// Resources already disposed
			}
		}

		/// <summary>
		/// Gets a gell at a given column index
		/// </summary>
		/// <param name="c"></param>
		/// <returns></returns>
		public Cell GetCell(int c)
		{
			if (c > cellList.Count-1)
				throw new GridException("Cell out of range");

			return cellList[c];
		}

		/// <summary>
		/// Draws a row of cells
		/// </summary>
		/// <param name="xOffset"></param>
		/// <param name="y"></param>
		/// <param name="g"></param>
		public virtual void Draw(int xOffset, int y, Graphics g)
		{
			int x = 0;
			int xCnt=0;
			foreach (Cell c in cellList)
			{
				if (c.col.Visible)
				{
					if (!c.col.Header && xCnt < xOffset)
					{
						xCnt += c.col.Size;
						continue;
					}

					c.Draw(x,y,g);
					x += c.col.Size;
				}
			}
		}

		/// <summary>
		/// Constructs a row with a given height
		/// </summary>
		/// <param name="grid"></param>
		/// <param name="defaultHeight"></param>
		public Row(Grid grid, int defaultHeight)
		{
			
			this.grid = grid;
			Size = defaultHeight;

			foreach (Column c in grid.colList)
				AddCell(c);
		}
	
		/// <summary>
		/// Adds a cell to a given column
		/// </summary>
		/// <param name="c"></param>
		/// <returns></returns>
		public Cell AddCell(Column c)
		{
			Cell cell = grid.CreateCell(this, c);
			cellList.Add(cell);
			return cell;
		}

		/// <summary>
		/// Gets the value of a row (header) from a given column 
		/// </summary>
		/// <param name="col"></param>
		/// <returns></returns>
		public virtual object Value(Column col)
		{
			return grid.GetColList().IndexOf(col).ToString();
		}

		public void ReplaceCell(Cell replaceCell, Cell cell)
		{
			int index = cellList.IndexOf(replaceCell);

			cellList.RemoveAt(index);
			cell.Attach(replaceCell.row, replaceCell.col);
			cellList.Insert(index, cell);
		}
	}	
}
