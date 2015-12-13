using System;
using System.Drawing;
using System.IO;
using System.Windows.Forms;

namespace LevelBuilder
{
    public partial class Form1 : Form
    {
        GridCtrl.Grid grid;
        public Form1()
        {
            InitializeComponent();
            grid = new GridCtrl.Grid();
            grid.OnCellSelect += OnCellSelect;
            grid.Parent = gridpanel;
            grid.Anchor = AnchorStyles.Bottom | AnchorStyles.Left | AnchorStyles.Right;
        }

        public Slot[,] slots;

        private void button1_Click(object sender, EventArgs e)
        {
            grid.colList.Clear();
            grid.rowList.Clear();
            int rows = (int)rowsInput.Value;
            int cols = (int)columnsInput.Value;
            slots = new Slot[rows,cols];
            
            for(int i = 0; i < cols; ++i)
            {
                grid.AddColumn();
            }
            for(int i = 0; i < rows-1; ++i)
            {
                grid.AddRow();
            }
            for(int i=0;i< rows; ++i)
            {
                for(int j=0;j< cols; ++j)
                {
                    GridCtrl.Cell cell = grid.GetCell(i, j);
                    cell.Value = String.Format("{0},{1}", i, j);
                    cell.HorizontalAlignment = GridCtrl.Cell.HorizontalAlignmentType.Center;
                    slots[i, j] = Slot.MakeSlot("0,0,0,0,0,0");
                    CellSlotPopulate(slots[i, j], cell);
                }
            }
        }

        private void CellSlotPopulate(Slot slot, GridCtrl.Cell cell)
        {
            if (slot.Winning)
            {
                cell.BackColor = Color.White;
            } else
            {
                cell.BackColor = Color.Red;
            }
            string tip = "Occupied: ";
            if (slot.Occupied)
            {
                tip += "Yes";
            }
            else
            {
                tip += "No";
            }
            tip += "\n";
            if (slot.NorthWall)
            {
                tip += "North, ";
            }
            if (slot.EastWall)
            {
                tip += "East, ";
            }
            if (slot.SouthWall)
            {
                tip += "South, ";
            }
            if (slot.WestWall)
            {
                tip += "West, ";
            }
            cell.TipText = tip;
            cell.Selected = false;
        }

        private void OnCellSelect(GridCtrl.Cell cell)
        {
            Dialog d = new Dialog();
            string[] s = (cell.Value as string).Split(',');
            int row = int.Parse(s[0]);
            int col = int.Parse(s[1]);
            if (slots[row, col] != null)
            {
                Console.WriteLine("slots[row, col] not null");
                d.propertiesCheckBox.SetItemChecked(0, slots[row, col].NorthWall);
                d.propertiesCheckBox.SetItemChecked(1, slots[row, col].EastWall);
                d.propertiesCheckBox.SetItemChecked(2, slots[row, col].SouthWall);
                d.propertiesCheckBox.SetItemChecked(3, slots[row, col].WestWall);
                d.propertiesCheckBox.SetItemChecked(4, slots[row, col].Occupied);
                d.propertiesCheckBox.SetItemChecked(5, slots[row, col].Winning);
            } else
            {
                Console.WriteLine("slot null {0},{1}", row, col);
            }

            d.ShowDialog(this);
            CheckedListBox.CheckedIndexCollection c = d.propertiesCheckBox.CheckedIndices;
            slots[row, col] = new Slot(c.Contains(0),
                c.Contains(1),
                c.Contains(2),
                c.Contains(3),
                c.Contains(4),
                c.Contains(5)
            );
            CellSlotPopulate(slots[row, col], cell);
        }
        
        //save fiel
        private void button2_Click(object sender, EventArgs e)
        {
            byte rows = (byte)rowsInput.Value;
            byte cols = (byte)columnsInput.Value;
            SaveFileDialog fd = new SaveFileDialog();
            DialogResult result = fd.ShowDialog(this);
            if (result != DialogResult.OK && result != DialogResult.Yes)
            {
                return;
            }
            using (StreamWriter bw = new StreamWriter(fd.OpenFile()))
            {
                bw.WriteLine(rows);
                bw.WriteLine(cols);
                for(int i=0;i< rows; ++i)
                {
                    for(int j=0;j< cols; ++j)
                    {
                        if (slots[i, j] == null)
                        {
                            MessageBox.Show(String.Format("Something fucky happened @{0},{1}", i, j));
                        }
                        string slot = slots[i, j].WriteSlot();
                        Console.WriteLine("Writing Slot {0},{1} = {2}", i, j, slot);
                        bw.WriteLine(slot);
                    }
                }
            }
            using (StreamWriter sw = new StreamWriter(new FileStream(fd.FileName + "sql", FileMode.OpenOrCreate)))
            {
                sw.Write("INSERT INTO levels (level,leveldata) VALUES (#,'");

                sw.WriteLine(rows);
                sw.WriteLine(cols);
                for (int i = 0; i < rows; ++i)
                {
                    for (int j = 0; j < cols; ++j)
                    {
                        if (slots[i, j] == null)
                        {
                            MessageBox.Show(String.Format("Something fucky happened @{0},{1}", i, j));
                        }
                        string slot = slots[i, j].WriteSlot();
                        sw.WriteLine(slot);
                    }
                }
                sw.WriteLine("');");
            }
        }

        //open file
        private void button3_Click(object sender, EventArgs e)
        {
            //open existing form
            OpenFileDialog fd = new OpenFileDialog();
            DialogResult result = fd.ShowDialog(this);
            if (result != DialogResult.OK && result != DialogResult.Yes)
            {
                return;
            }
            int rows, cols;
            //bool binary = false;
            //using (BinaryReader asdf = new BinaryReader(fd.OpenFile()))
            //{
            //    if (asdf.PeekChar() < '0')
            //    {
            //        //binary
            //        binary = true;
            //    }
            //}
            //if (binary)
            //{
            //using (BinaryReader br = new BinaryReader(fd.OpenFile()))
            //{
            //    rows = br.ReadByte();
            //    cols = br.ReadByte();
            //    rowsInput.Value = rows;
            //    columnsInput.Value = cols;
            //    slots = new Slot[rows, cols];
            //    grid.colList.Clear();
            //    grid.rowList.Clear();
            //    for (int i = 0; i < cols; ++i)
            //    {
            //        grid.AddColumn();
            //    }
            //    for (int i = 0; i < rows - 1; ++i)
            //    {
            //        grid.AddRow();
            //    }
            //    for (int i = 0; i < rows; ++i)
            //    {
            //        for (int j = 0; j < cols; ++j)
            //        {
            //            slots[i, j] = Slot.MakeSlot(br.ReadByte());
            //            GridCtrl.Cell cell = grid.GetCell(i, j);
            //            cell.Value = String.Format("{0},{1}", i, j);
            //            CellSlotPopulate(slots[i, j], cell);
            //        }
            //    }
            //}
            //}
            //else
            //{ 
            using (StreamReader sr = new StreamReader(fd.OpenFile()))
            {
                rows = int.Parse(sr.ReadLine());
                cols = int.Parse(sr.ReadLine());
                rowsInput.Value = rows;
                columnsInput.Value = cols;
                slots = new Slot[rows, cols];
                grid.colList.Clear();
                grid.rowList.Clear();
                for (int i = 0; i < cols; ++i)
                {
                    grid.AddColumn();
                }
                for (int i = 0; i < rows - 1; ++i)
                {
                    grid.AddRow();
                }
                for (int i = 0; i < rows; ++i)
                {
                    for (int j = 0; j < cols; ++j)
                    {
                        slots[i, j] = Slot.MakeSlot(sr.ReadLine());
                        GridCtrl.Cell cell = grid.GetCell(i, j);
                        cell.Value = String.Format("{0},{1}", i, j);
                        CellSlotPopulate(slots[i, j], cell);
                    }
                }
            }
            //}
        }
    }
}
