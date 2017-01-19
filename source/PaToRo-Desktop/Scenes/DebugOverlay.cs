using PaToRo_Desktop.Engine;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace PaToRo_Desktop.Scenes
{
    class DebugOverlay: Group
    {
        private readonly BaseGame game;
        public string Text { get; set; }

        public DebugOverlay(BaseGame game)
        {
            this.game = game;
        }

        internal override void Draw(SpriteBatch spriteBatch, GameTime gameTime)
        {
            base.Draw(spriteBatch, gameTime);

            spriteBatch.DrawString(game.Fonts.Get("debug"), Text, Vector2.Zero, Color.Red);
        }
    }
}
