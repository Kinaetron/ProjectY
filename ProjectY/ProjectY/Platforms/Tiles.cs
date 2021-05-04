using PolyOne.Collision;
using PolyOne.Utility;

namespace ProjectY.Platforms
{
    public class Tiles : Tile
    {
        public Grid Grid { get; private set; }

        public Tiles(bool[,] data)
        {
            this.Active = false;
            this.Collider = (this.Grid = new Grid(TileInformation.TileWidth, TileInformation.TileHeight, data));
        }
    }
}
