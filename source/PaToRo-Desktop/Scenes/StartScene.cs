using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using PaToRo_Desktop.Engine;

namespace PaToRo_Desktop.Scenes
{
    public class StartScene : StarfieldScene
    {
        private SpriteFont font;
        private bool startGame;

        public StartScene(BaseGame game) : base(game)
        {
        }


        internal override void InternalLoadContent()
        {
            base.InternalLoadContent();

            font = game.Fonts.Get("PressStart2P");
        }

        internal override void Draw(SpriteBatch batch, GameTime gameTime)
        {
            base.Draw(batch, gameTime);

            var title = "Wave Tracer";

            var m = font.MeasureString(title);


            var position = (new Vector2(game.Screen.Width, game.Screen.Height) - m) / 2f;
            spriteBatch.DrawString(font, title, position, Color.Red);

        }

        internal override void Update(GameTime gameTime)
        {
            base.Update(gameTime);

            if (game.Inputs.Any(x => x.AnyButtonDown))
                startGame = true;
        }

    }
}
