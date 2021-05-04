using System;

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

using PolyOne;
using PolyOne.Collision;
using PolyOne.Engine;
using PolyOne.Scenes;

namespace ProjectY
{
    public class Enemy : Entity
    {
        private Texture2D texture;

        private Vector2 velocity;
        private Vector2 remainder;

        private const float gravity = 0.31f;
        private const float fallspeed = 6.0f;

        private int lifePoints = 6;

        public Enemy(Vector2 position)
            :base(position)
        {
            Tag((int)GameTags.Enenmy);
            Collider = new Hitbox((float)32.0f, (float)32.0f, 0.0f, 0.0f);
            Visible = true;

            texture = Engine.Instance.Content.Load<Texture2D>("Enemy");
        }
 
        public override void Added(Scene Scene)
        {
            base.Added(Scene);
        }

        public override void Update()
        {
            base.Update();

            if (this.CollideFirst((int)GameTags.Bullet, Position) != null) {
                lifePoints--;
            }

            if(lifePoints <= 0) {
                RemoveSelf();
            }

            velocity.Y += gravity;
            velocity.Y = MathHelper.Clamp(velocity.Y, -fallspeed, fallspeed);
            MovementVerical(velocity.Y);
        }

        private void MovementVerical(float amount)
        {
            remainder.Y += amount;
            int move = (int)Math.Round((double)remainder.Y);

            if (move < 0)
            {
                remainder.Y -= move;
                while (move != 0)
                {
                    Vector2 newPosition = Position + new Vector2(0, -1.0f);

                    if (CollideFirst((int)GameTags.Tile, newPosition) != null)
                    {
                        velocity.Y = 0;
                        remainder.Y = 0;
                        break;
                    }

                    Position.Y += -1.0f;
                    move -= -1;
                }
            }
            else if (move > 0)
            {
                remainder.Y -= move;
                while (move != 0)
                {
                    Vector2 newPosition = Position + new Vector2(0, 1.0f);

                    if (CollideFirst((int)GameTags.Tile, newPosition) != null)
                    {
                        remainder.Y = 0;
                        break;
                    }

                    Position.Y += 1.0f;
                    move -= 1;
                }
            }
        }

        public override void Draw()
        {
            base.Draw();
            Engine.SpriteBatch.Draw(texture, Position, Color.White);
        }
    }
}
