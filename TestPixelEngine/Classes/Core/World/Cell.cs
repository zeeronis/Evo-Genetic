using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EvoGenome
{
    public class CellBase: WorldObject
    {
        public Cell[] upCell; //order by Sides
    }

    public class Cell: CellBase
    {
        public Unit unit;
        public float crystalsLevel; 
        public float sunLevel; 
            
        public bool IsEmpty => unit == null;


        public Cell(int x, int y)
        {
            this.x = x;
            this.y = y;
        }
    }

    public enum Sides
    {
        LeftUp,
        Up,
        RightUp,
        Right,
        RightDown,
        Down,
        LeftDown,
        Left,
    }
}
