using System;
using System.Drawing;
using System.Windows.Forms;

namespace GridCtrl
{
	/// <summary>
	/// 
	/// </summary>
	public class CellEditor : System.Windows.Forms.TextBox
	{
		private	Grid	 gridCtrl		= null;
		private	Cell	 selectedCell	= null;

		public CellEditor(Grid gridCtrl, Cell selectedCell, Rectangle rect)
		{
			this.gridCtrl = gridCtrl;
			this.selectedCell = selectedCell;
			this.Font = new Font(selectedCell.FontName,selectedCell.TextHeight,selectedCell.FontStyle, GraphicsUnit.Pixel); 
			this.AutoSize = false;
			this.BorderStyle = BorderStyle.None;
			this.Bounds = rect;
			this.Text = selectedCell.Value.ToString();
			this.HideSelection = true;
			this.KeyDown +=new KeyEventHandler(CellEditor_KeyDown);			
		}

		private void CellEditor_KeyDown(object sender, KeyEventArgs e)
		{
			if (e.KeyCode == Keys.Return || e.KeyCode == Keys.Escape)
			{
				EndEditing();
			}
		}

		private void EndEditing()
		{
			gridCtrl.Controls.Remove(this);
			selectedCell.Value = this.Text;
		}
	}
}
