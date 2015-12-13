using System;
using System.Collections;

namespace GridCtrl
{
	/// <summary>
	/// Summary description for Column.
	/// </summary>
	public class Column : GridObject
	{


		public override ObjectType GetObjectType()
		{
			return GridObject.ObjectType.Column;
		}

		// the destructor
		~Column()
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
					// dispose managed resources
				}
				disposed=true;
			}
			else
			{
				// Resources already disposed
			}
		}


		public object Value(Row row)
		{
			return grid.GetRowList().IndexOf(row);
		}


		public Column(Grid grid, uint Width)
		{
			this.grid = grid;
			this.Size = (int) Width;
		}
	}


}
