using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PaToRo_Desktop.Engine
{
    public class BaseGame : Game
    {
        protected readonly GraphicsDeviceManager graphics;
        public readonly Scenes Scenes;
        public readonly Fonts Fonts;

        public BaseGame()
        {
            graphics = new GraphicsDeviceManager(this);
            Scenes = new Scenes(this);
            Fonts = new Fonts(this);

            Content.RootDirectory = "Content";
        }

        protected override void Initialize()
        {
            base.Initialize();
        }

        protected override void LoadContent()
        {
            Fonts.Add("debug", "Fonts/DebugFont");
        }

        protected override void UnloadContent()
        {
            Scenes.Clear();
            Fonts.Clear();
        }

        protected override void Update(GameTime gameTime)
        {
            if (Scenes.Current != null)
                Scenes.Current.Update(gameTime);

            base.Update(gameTime);
        }

        protected override void Draw(GameTime gameTime)
        {
            if (Scenes.Current != null)
                Scenes.Current.Draw(gameTime);

            base.Draw(gameTime);
        }

    }
}
