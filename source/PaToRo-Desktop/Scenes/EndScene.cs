using Microsoft.Xna.Framework;
using PaToRo_Desktop.Engine;
using System;

namespace PaToRo_Desktop.Scenes
{
    internal class EndScene : Scene
    {
        private TestScene testScene;
        private Color[] Colors = { new Color(191, 91, 91), new Color(198, 185, 85), new Color(134, 180, 96), new Color(60, 141, 136), new Color(89, 87, 88) };
        private float GameOverPositionX;
        private static float TextAmplitude = 50.0f;
        private static float TextWavelength = 150.0f;
        private static float TextSpeed = 500.0f;

        public EndScene(BaseGame game, TestScene testScene) : base(game)
        {
            this.testScene = testScene;
            GameOverPositionX = game.Screen.Width;
        }

        internal override void Draw(GameTime gameTime)
        {
            base.Draw(gameTime);
            spriteBatch.Begin();
            int Counter = 0;
            float OffsetX = 30.0f;
            float OffsetY = 100.0f;
            Vector2 TextPosition = new Vector2(GameOverPositionX, OffsetY);
            foreach (char c in "Game Over !")
            {
                TextPosition.X += OffsetX;
                TextPosition.Y = (float)Math.Sin(TextPosition.X * 1/TextWavelength) * TextAmplitude + OffsetY;
                spriteBatch.DrawString(game.Fonts.Get("PressStart2P"), $"{c}", TextPosition, Colors[Counter%Colors.Length]);
                Counter++;
            }
            Vector2 ScorePosition = new Vector2(game.Screen.Width / 2.0f, game.Screen.Height / 2.0f);
            Counter = 0;
            foreach(TheNewWaveRider Rider in testScene.Riders)
            {
                spriteBatch.DrawString(game.Fonts.Get("PressStart2P"), $"Score Player {Rider.PlayerNum}: {Rider.Points}", ScorePosition, Colors[Counter%Colors.Length]);
                ScorePosition.Y += 30;
                Counter++;
            }
            spriteBatch.End();
        }

        internal override void Update(GameTime gameTime)
        {
            base.Update(gameTime);
            if (game.Inputs[0].IsDown(Engine.Input.Buttons.Start))
            {
                testScene.Level.Restart();
                game.Scenes.Show("level");
                return;
            } 
            if (GameOverPositionX > - 500.0f) {
                GameOverPositionX -= TextSpeed * (float)gameTime.ElapsedGameTime.TotalSeconds;
            } else {
                GameOverPositionX = game.Screen.Width;
            }
        }

        internal override int HandleInput(GameTime gameTime)
        {
            return base.HandleInput(gameTime);
        }
    }
}