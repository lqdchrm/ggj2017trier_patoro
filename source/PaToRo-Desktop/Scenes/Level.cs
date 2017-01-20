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
        public int NumValues {  get { return upper.Length; } }

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

            for (int i = 0; i < NumValues; ++i)
            {
                pos.X = i * (game.Screen.Width / NumValues);
                pos.Y = upper[i];
                spriteBatch.Draw(part, pos, Color.White);
                pos.Y = lower[i];
                spriteBatch.Draw(part, pos, Color.White);
            }
        }
    }
}
