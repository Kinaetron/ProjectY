using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

using PolyOne;
using PolyOne.Collision;
using PolyOne.Engine;
using PolyOne.Scenes;

namespace ProjectY
{
    public class Bullet : Entity
    {
        private Texture2D texture;
        private const float speed = 6.75f;

        private Direction direction = Direction.None;

        public Bullet(Vector2 position, Direction direction)
           : base(position)
        {
            texture = Engine.Instance.Content.Load<Texture2D>("Bullet");

            this.Tag((int)GameTags.Bullet);
            this.Collider = new Hitbox(10, 10);

            this.Visible = true;

            this.direction = direction;
        }

        public override void Added(Scene scene) {
            base.Added(scene);
        }

        public override void Update()
        {
            if (direction == Direction.Right) {
                Position.X += speed;
            }

            if (direction == Direction.Left) {
                Position.X -= speed;
            }

            if (this.CollideFirst((int)GameTags.Tile, Position) != null) {
                RemoveSelf();
            }

            if (this.CollideFirst((int)GameTags.Enenmy, Position) != null) {
                //RemoveSelf();
            }

            base.Update();
        }

        public override void Draw()
        {
            Engine.SpriteBatch.Draw(texture, Position, Color.White);
            base.Draw();
        }
    }
}
