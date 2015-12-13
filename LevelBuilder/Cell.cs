using System;
using System.Drawing;
using System.Collections;
using System.Windows.Forms;

namespace GridCtrl
{
	/// <summary>
	/// Summary description for Cell.
	/// </summary>
	public class Cell
	{
		public enum HorizontalAlignmentType
		{
			Left,
			Center,
			Right
		}

		public enum VerticalAlignmentType
		{
			Top,
			Center,
			Bottom
		}

		public enum ImageAlignmentType
		{
			TopLeft,
			TopCenter,
			TopRight,
			MiddleLeft,
			MiddleCenter,
			MiddleRight,
			BottomLeft,
			BottomCenter,
			BottomRight,
		}
		
		public		Column						col			= null; // Attached Column
		public		Row							row			= null; // Attached Row
		private		Color						clrBack		= SystemColors.Window;
		private		object						rObject		= null;
		private		object						cellValue	= "";	// Default to string
		private		int							textHeight	= 16; // Auto
		private		HorizontalAlignmentType		horzAlign	= HorizontalAlignmentType.Left;
		private		VerticalAlignmentType		vertAlign	= VerticalAlignmentType.Center;
		private		Color						textColor	= SystemColors.WindowText;
		private		Image						image		= null;
		private		ImageAlignmentType			imageAlignment  = ImageAlignmentType.MiddleLeft;
		private		string						fontName	= Control.DefaultFont.FontFamily.GetName(0);
		private		FontStyle					fontStyle	= FontStyle.Regular;
		private		bool						bSelected	= false;
		private		bool						canEdit		= true;
		private		string						tipText		= "";
		

		public Cell()
		{

		}


		public string TipText
		{
			get
			{
				return tipText;
			}

			set 
			{
				tipText = value;
			}
		}

		public int TextHeight
		{
			set 
			{
				TextHeight = value;
			}

			get
			{
				return textHeight;
			}
		}

		public bool Selected
		{
			get
			{
				return bSelected;
			}

			set
			{
				bSelected = value;
				Invalidate(true);
			}
		}


		public void Attach(Row r, Column c)
		{
			row = r;
			col = c;
		}

		public bool CanEdit
		{
			get
			{
				return canEdit;
			}

			set 
			{
				canEdit = value;
			}
		}

		public Color TextColor
		{
			get
			{
				return textColor;
			}

			set
			{
				textColor = value;
				Invalidate(false);
				
			}
		}

		public FontStyle FontStyle
		{
			get
			{
				return fontStyle;
			}

			set
			{
				fontStyle = value;
				Invalidate(false);
			}
		}

		public Image Image
		{
			get
			{
				return image;
			}

			set
			{
				image = value;
				Invalidate(false);
			}

		}
		
		public string FontName
		{
			get
			{
				return fontName;
			}

			set
			{
				fontName = value;
				Invalidate(false);
			}

		}


	
		public ImageAlignmentType ImageAlignment
		{
			get
			{
				return imageAlignment;
			}

			set
			{
				imageAlignment = value;

			}
		}


			public HorizontalAlignmentType HorizontalAlignment
		{
			get
			{
				return horzAlign;
			}

			set
			{
				horzAlign = value;
				Invalidate(false);
				
			}
		}

		public VerticalAlignmentType VerticalAlignment
		{
			get
			{
				return vertAlign;
			}

			set
			{
				vertAlign = value;
				Invalidate(false);
				
			}
		}

		public Color BackColor
		{
			get
			{
				return clrBack;
			}

			set
			{
				clrBack = value;
				Invalidate(false);
			}
		}

		public object Value
		{
			get 
			{
				return cellValue;
			}

			set
			{
				cellValue = value;
				Invalidate(false);
			}
		}

		public object Tag
		{
			get
			{
				return rObject;
			}

			set 
			{
				rObject = value;
				Invalidate(false);
			}
		}

		public Cell(Row row, Column col)
		{
			this.col = col;
			this.row = row;

			//
			// TODO: Add constructor logic here
			//
		}

		public virtual void Draw(int x, int y, Graphics g)
		{
			Rectangle r = new Rectangle(x,y, GetSize().Width, GetSize().Height);


			Color fontColor = textColor;

			object o = cellValue;

			if (row.Header || col.Header)
			{
				if (row.Selected || col.Selected)
					ControlPaint.DrawButton(g,r, ButtonState.Pushed );
				else
					ControlPaint.DrawButton(g,r, ButtonState.Normal );
				
				if (row.Header && !col.Header)
					o = row.Value(col);
				if (col.Header && !row.Header)
					o = col.Value(row);
			}
			else
			{
				bool bSel = bSelected;
				if (!bSel)
				{
					bSel = ((row.Selected && row.grid.SelectionMode == Grid.SelectionModeType.RowSelect) | 
						(col.Selected && col.grid.SelectionMode == Grid.SelectionModeType.ColumnSelect));
				}
					


				Color clr = bSel ? Color.FromArgb(255, SystemColors.Highlight ) : Color.FromArgb(row.grid.GetGridOpacity(), clrBack);
				if (bSel)
					fontColor = SystemColors.HighlightText;



				g.FillRectangle(new SolidBrush(clr), r);

				if (row.grid.GridLine == Grid.GridLineStyle.Both)
					g.DrawRectangle(new Pen(row.grid.GridLineColor,1), r);

				if (row.grid.GridLine == Grid.GridLineStyle.Horizontal)
				{
					g.DrawLine(new Pen(row.grid.GridLineColor,1), r.X, r.Y, r.Right, r.Y);
					g.DrawLine(new Pen(row.grid.GridLineColor,1), r.X, r.Bottom, r.Right, r.Bottom);
				}

				if (row.grid.GridLine == Grid.GridLineStyle.Vertical)
				{
					g.DrawLine(new Pen(row.grid.GridLineColor,1), r.X, r.Y, r.X, r.Bottom);
					g.DrawLine(new Pen(row.grid.GridLineColor,1), r.Right, r.Y, r.Right, r.Bottom);
				}


			}

			int fontHeight = textHeight;
			if (fontHeight == -1)
				fontHeight = r.Height - 2;

			StringFormat sf = new StringFormat();

			switch (horzAlign)
			{
				case HorizontalAlignmentType.Center:
					sf.Alignment = StringAlignment.Center;
					break;
				case HorizontalAlignmentType.Left:
					sf.Alignment = StringAlignment.Near;
					break;
				case HorizontalAlignmentType.Right:
					sf.Alignment = StringAlignment.Far;
					break;
				default:
					sf.Alignment = StringAlignment.Near;
					break;
			}

			switch (vertAlign)
			{
				case VerticalAlignmentType.Center:
					sf.LineAlignment = StringAlignment.Center;
					break;
				case VerticalAlignmentType.Top:
					sf.LineAlignment = StringAlignment.Near;
					break;
				case VerticalAlignmentType.Bottom:
					sf.LineAlignment = StringAlignment.Far;
					break;
				default:
					sf.LineAlignment = StringAlignment.Near;
					break;

			}

			sf.FormatFlags = StringFormatFlags.NoWrap;
			sf.Trimming = StringTrimming.None;

			Rectangle rcImage = r;

			// 
			if (image != null)
			{
				int height = image.Size.Height;
				if (height > r.Height)
					height = r.Height;

				int width = image.Size.Width;
				if (width > r.Width)
					width = r.Width;

				switch (imageAlignment)
				{
					case ImageAlignmentType.TopLeft:
						r.Y += height;
						break;
					case ImageAlignmentType.TopCenter:
						r.Y += height;
						break;
					case ImageAlignmentType.TopRight:
						r.Y += height;
						break;

					case ImageAlignmentType.MiddleLeft:
						r.X += width;
						g.DrawImage(image,rcImage.X,rcImage.Y,width, height);
						break;
					case ImageAlignmentType.MiddleCenter:
						break;
					case ImageAlignmentType.MiddleRight:
						break;

					case ImageAlignmentType.BottomLeft:
						r.Height -= height;
						break;
					case ImageAlignmentType.BottomCenter:
						r.Height -= height;
						break;
					case ImageAlignmentType.BottomRight:
						r.Height -= height;
						break;
				}
			}

			Font drawFont = new Font(fontName, fontHeight, fontStyle, GraphicsUnit.Pixel);
			SolidBrush drawBrush = new SolidBrush(fontColor);
			g.DrawString(o.ToString(), drawFont, drawBrush , r, sf);
		}

		protected Size GetSize()
		{
			return new Size((int) col.Size, (int) row.Size);
		}

		public void BeginEdit()
		{

		}


		protected void Invalidate(bool forceUpdate)
		{
			// Invalidate Cell Rect
			if (row != null)
				row.grid.InvalidateCell(this, forceUpdate);
		}
	}

	public class CellCollection : CollectionBase  
	{

		public Cell this[ int index ]  
		{
			get  
			{
				return( (Cell) List[index] );
			}
			set  
			{
				List[index] = value;
			}
		}

		public int Add( Cell value )  
		{
			return( List.Add( value ) );
		}

		public int IndexOf( Cell value )  
		{
			return( List.IndexOf( value ) );
		}

		public void Insert( int index, Cell value )  
		{
			List.Insert( index, value );
		}

		public void Remove( Cell value )  
		{
			List.Remove( value );
		}

		public bool Contains( Cell value )  
		{
			// If value is not of type Cell, this will return false.
			return( List.Contains( value ) );
		}

		protected override void OnInsert( int index, Object value )  
		{
			// Insert additional code to be run only when inserting values.
		}

		protected override void OnRemove( int index, Object value )  
		{
			// Insert additional code to be run only when removing values.
		}

		protected override void OnSet( int index, Object oldValue, Object newValue )  
		{
			// Insert additional code to be run only when setting values.
		}

		protected override void OnValidate( Object value )  
		{
			if ( value.GetType() != Type.GetType("GridCtrl.Cell") )
				throw new ArgumentException( "value must be of type Cell.", "value" );
		}
	}
}
