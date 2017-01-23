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
    class DebugOverlay : Group
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
            if (Text != null)
                spriteBatch.DrawString(game.Fonts.Get(Font.PressStart2P20), Text, Vector2.Zero, Color.Blue);

            var elapsed = (game.Scenes.Current as TestScene).Level.Elapsed;
            var shownTime = elapsed.ToString("mm") + ":" + elapsed.ToString("ss");

            spriteBatch.DrawString(game.Fonts.Get(Font.PressStart2P20), shownTime, new Vector2(game.Screen.Width * 0.5f - 50f, 10f), Color.Yellow);
        }
    }
}
