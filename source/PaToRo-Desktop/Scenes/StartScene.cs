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
        private float startPressed;

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

            var baseColor = Color.Red;
            if (this.startGame)
            {
                var progress = MathHelper.Clamp(((float)gameTime.TotalGameTime.TotalSeconds - (float)startPressed), 0f, 1f);
                baseColor = Color.Lerp(baseColor, Color.Transparent, progress);
            }

            spriteBatch.DrawString(font, title, position, baseColor);

        }

        internal override void Update(GameTime gameTime)
        {
            base.Update(gameTime);

            if (game.Inputs.Any(x => x.AnyButtonDown))
            {
                startGame = true;
                this.startPressed = (float)gameTime.TotalGameTime.TotalSeconds;
            }

            if (this.startGame)
            {
                var progress = MathHelper.Clamp(((float)gameTime.TotalGameTime.TotalSeconds - (float)startPressed), 0f, 1f);
                if (progress == 1)
                {
                    game.Scenes.Show("level");
                }
            }
        }

    }
}
