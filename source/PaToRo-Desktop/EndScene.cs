using Microsoft.Xna.Framework;
using PaToRo_Desktop.Engine;

namespace PaToRo_Desktop
{
    internal class EndScene : Scene
    {
        private bool LastButtonPress;

        public EndScene(BaseGame game) : base(game)
        {
            LastButtonPress = false;
        }

        internal override void Draw(GameTime gameTime)
        {
            base.Draw(gameTime);
            spriteBatch.Begin();
            spriteBatch.DrawString(game.Fonts.Get("debug"), "You Loose", Vector2.Zero, Color.Blue);
            spriteBatch.End();
        }

        internal override int HandleInput(GameTime gameTime)
        {
            var numPlayers = base.HandleInput(gameTime);
            bool ButtonPress = game.Inputs.Player(0).AnyButtonDown;
            if (!ButtonPress && LastButtonPress)
            {
                game.Scenes.Show("level");
            }
            LastButtonPress = ButtonPress;
            return numPlayers;
        }
    }
}