
namespace LevelBuilder
{
    public class Slot
    {

        public bool NorthWall { get; set; }
        public bool EastWall { get; set; }
        public bool SouthWall { get; set; }
        public bool WestWall { get; set; }
        public bool Occupied { get; set; }
        public bool Winning { get; set; }

        public Slot(bool north,bool east, bool south, bool west, bool occ, bool win)
        {
            NorthWall = north;
            EastWall = east;
            SouthWall = south;
            WestWall = west;
            Occupied = occ;
            Winning = win;
        }

        public static Slot MakeSlot(string slot_data)
        {
            string[] s = slot_data.Split(',');
            bool northwall = s[0] == "1";
            bool eastwall = s[1] == "1";
            bool southwall = s[2] == "1";
            bool westwall = s[3] == "1";
            bool occupied = s[4] == "1";
            bool winning = s[5] == "1";
            Slot slot = new Slot(northwall, eastwall, southwall, westwall, occupied, winning);
            return slot;
        }

        public static Slot MakeSlot(int slot_data)
        {
            bool winning = (slot_data & 1) == 1;
            slot_data >>= 1;
            bool occupied = (slot_data & 1) == 1;
            slot_data >>= 1;
            bool westwall = (slot_data & 1) == 1;
            slot_data >>= 1;
            bool southwall = (slot_data & 1) == 1;
            slot_data >>= 1;
            bool eastwall = (slot_data & 1) == 1;
            slot_data >>= 1;
            bool northwall = (slot_data & 1) == 1;

            Slot slot = new Slot(northwall, eastwall, southwall, westwall, occupied, winning);
            return slot;
        }

        public string WriteSlot()
        {
            string slot = "";
            slot += (NorthWall ? "1" : "0") + ",";
            slot += (EastWall ? "1" : "0") + ",";
            slot += (SouthWall ? "1" : "0") + ",";
            slot += (WestWall ? "1" : "0") + ",";
            slot += (Occupied ? "1" : "0") + ",";
            slot += (Winning ? "1" : "0");
            return slot;
        }

        //public int WriteSlot()
        //{
        //    int slot = 0;
        //    slot += NorthWall ? 1 : 0;
        //    slot <<= 1;
        //    slot += EastWall ? 1 : 0;
        //    slot <<= 1;
        //    slot += SouthWall ? 1 : 0;
        //    slot <<= 1;
        //    slot += WestWall ? 1 : 0;
        //    slot <<= 1;
        //    slot += Occupied ? 1 : 0;
        //    slot <<= 1;
        //    slot += Winning ? 1 : 0;
        //    return slot;
        //}
    }
}
