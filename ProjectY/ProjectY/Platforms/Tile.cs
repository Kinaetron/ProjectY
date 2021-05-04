using Microsoft.Xna.Framework;

using PolyOne.Collision;

namespace ProjectY.Platforms
{
    public abstract class Tile : Platform
    {
        public Tile(Vector2 position, int width, int height)
            : base(position)
        {
            this.Tag((int)GameTags.Tile);
            this.Collider = new Hitbox((float)width, (float)height, 0.0f, 0.0f);
        }

        public Tile()
            : base(Vector2.Zero)
        {
            this.Tag((int)GameTags.Tile);
        }
    }
}
