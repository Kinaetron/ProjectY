using Microsoft.Xna.Framework;

using PolyOne.Scenes;
using PolyOne.Engine;
using PolyOne.Utility;
using PolyOne.LevelProcessor;

using ProjectY.Platforms;

namespace ProjectY
{
    public class Level : Scene
    {
        private Tiles tiles;
        private Player player;

        public LevelTiler LevelTiler { get; private set; } = new LevelTiler();
        private LevelData levelData = new LevelData();

        public override void LoadContent()
        {
            base.LoadContent();

            LevelTiler.LoadContent(Engine.Instance.Content.Load<LevelData>("TestLevel"));
            tiles = new Tiles(LevelTiler.TileConverison(LevelTiler.CollisionLayer, 2));
            this.Add(tiles);

            player = new Player(LevelTiler.PlayerPosition[0]);
            this.Add(player);
            player.Added(this);


            foreach (Entity entity in LevelTiler.Entites)
            {
                if (entity.Type == "Enemy")
                {
                    Enemy enemy = new Enemy(entity.Position);
                    this.Add(enemy);
                    enemy.Added(this);
                }
            }
        }

        public override void UnloadContent()
        {
            base.UnloadContent();
        }

        public override void Update()
        {
            base.Update();
        }

        public override void Draw()
        {
            Engine.Begin(Resolution.GetScaleMatrix);
            LevelTiler.DrawBackground();
            base.Draw();
            Engine.End();
        }
    }
}
