using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PaToRo_Desktop.Engine
{
    public class Scene : Group
    {
        protected readonly BaseGame game;

        protected Color BgColor { get; set; }
        protected SpriteBatch spriteBatch;

        public Scene(BaseGame game)
        {
            this.game = game;
        }

        internal virtual void Initialize()
        {
            BgColor = Color.Black;
        }

        internal virtual void LoadContent()
        {
            spriteBatch = new SpriteBatch(game.GraphicsDevice);
        }

        internal virtual void UnloadContent()
        {

        }

        internal virtual int HandleInput(GameTime gameTime)
        {
            return game.HandleInput(gameTime);
        }

        internal override void Update(GameTime gameTime)
        {
            HandleInput(gameTime);
            base.Update(gameTime);
        }

        internal override void Draw(SpriteBatch batch, GameTime gameTime)
        {
            throw new NotSupportedException();
        }

        internal virtual void Draw(GameTime gameTime)
        {
            game.GraphicsDevice.Clear(BgColor);

            spriteBatch.Begin();
            base.Draw(spriteBatch, gameTime);
            spriteBatch.End();
        }
    }
}
