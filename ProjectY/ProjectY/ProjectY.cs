using Microsoft.Xna.Framework;

using PolyOne.Engine;
using PolyOne.Utility;

using ProjectY.Screens;

namespace ProjectY
{
    public class ProjectY : Engine
    {
        public ProjectY()
            :base(640, 360, "ProjectY", 2.0f, false)
        {
        }

        static readonly string[] preloadAssets =
        {
            "MenuAssets/gradient",
        };

        protected override void Initialize()
        {

            base.Initialize();

            TileInformation.TileDiemensions(16, 16);

            screenManager.AddScreen(new BackgroundScreen(), null);
            screenManager.AddScreen(new MainMenuScreen(), null);
        }

        protected override void LoadContent()
        {
            base.LoadContent();

            foreach (string asset in preloadAssets)
            {
                Engine.Instance.Content.Load<object>(asset);
            }
        }

        protected override void Update(GameTime gameTime)
        {
            base.Update(gameTime);
        }

        protected override void Draw(GameTime gameTime)
        {
            base.Draw(gameTime);
        }
    }

    static class Program
    {
        static void Main(string[] args)
        {
            using (ProjectY game = new ProjectY())
            {
                game.Run();
            }
        }
    }
}
