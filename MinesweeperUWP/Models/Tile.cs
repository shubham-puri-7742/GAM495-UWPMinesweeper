using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MinesweeperUWP.Models
{
    internal class Tile
    {
        // True if the tile is a mine
        public bool IsMine { get; set; }
        // True if clicked
        public bool IsClicked { get; set; }
        // True if flagged
        public bool IsFlagged { get; set; }
        // True if ambiguous (none of the above)
        public bool IsAmbiguous { get; set; }

        // Row of the tile
        public int Row { get; set; }
        // Column of the tile
        public int Col { get; set; }

        // A list of 8 tiles around the centre tile
        public List<Tile> Neighbours { get; set; }
        // Check for mines in the surrounding 8 tiles
        public int MinesAround => Neighbours.Count(x => x.IsMine);

        // Could add IsMineFlagged

        // ctor
        public Tile(int row, int col)
        {
            // Create a list of neighbours and set row and col values
            Neighbours = new List<Tile>();
            Row = row;
            Col = col;
        }

    }
}
