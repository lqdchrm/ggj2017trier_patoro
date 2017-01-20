using MonoGame.Extended.Shapes;
using PaToRo_Desktop.Engine;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Content;

namespace PaToRo_Desktop.Scenes
{
    public class Level : Entity
    {
        private readonly BaseGame game;
        public int NumValues { get { return upper.Length; } }

        private Texture2D part;

        public float[] upper;
        public float[] lower;

        public Level(BaseGame game, int num)
        {
            this.game = game;

            upper = new float[num];
            lower = new float[num];

            FillStatic();
        }

        private void FillStatic()
        {
            for (int i = 0; i < upper.Length; ++i)
            {
                upper[i] = (float)(game.Screen.Height * 0.5f) - 50;
                lower[i] = (float)(game.Screen.Height * 0.5f) + 50;
            }
        }

        internal void LoadContent(ContentManager content)
        {
            part = content.Load<Texture2D>("Images/particle");
        }

        internal override void Update(GameTime gameTime)
        {

        }

        internal override void Draw(SpriteBatch spriteBatch, GameTime gameTime)
        {
            Vector2 pos = Vector2.Zero;

            var distanceBetweenPoints = (float)game.Screen.Width / NumValues;
            var distanceFromSceenBounds = distanceBetweenPoints / 2f;

            for (int i = 0; i < NumValues; ++i)
            {


                spriteBatch.FillRectangle(new RectangleF(i * distanceBetweenPoints - distanceFromSceenBounds, 0, distanceBetweenPoints, upper[i]), Color.Red);

                spriteBatch.DrawRectangle(new RectangleF(i * distanceBetweenPoints - distanceFromSceenBounds, 0, distanceBetweenPoints,  upper[i]), Color.DarkRed);



                spriteBatch.FillRectangle(new RectangleF(i * distanceBetweenPoints - distanceFromSceenBounds, lower[i], distanceBetweenPoints, game.Screen.Height - lower[i]), Color.Green);

                spriteBatch.DrawRectangle(new RectangleF(i * distanceBetweenPoints - distanceFromSceenBounds, lower[i], distanceBetweenPoints, game.Screen.Height - lower[i]), Color.DarkGreen);
            }

#if DEBUG
            for (int x = 1; x < NumValues; x++)
            {
                spriteBatch.DrawLine(
                    new Vector2(x * distanceBetweenPoints + distanceFromSceenBounds, upper[x]),
                    new Vector2((x - 1) * distanceBetweenPoints + distanceFromSceenBounds, upper[x - 1]),
                    Color.Green);

                spriteBatch.DrawLine(
                    new Vector2(x * distanceBetweenPoints + distanceFromSceenBounds, lower[x]),
                    new Vector2((x - 1) * distanceBetweenPoints + distanceFromSceenBounds, lower[x - 1]),
                    Color.Red);

            }

#endif
        }
    }
}
