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
        private Texture2D part;

        private int NumValues {  get { return upper.Length - 1; } }

        private float[] upper;
        private float[] lower;
        private int start = 0;
        private float xOffset;
        private float cooldown = 0;          // tracks time till new spawn

        public Generator Generator { get; set; }
        public float TimeStep { get; set; }    // time interval to spawn next value

        public float getUpperAt(float xPos)
        {
            xPos -= xOffset;
            var testPos = (xPos / game.Screen.Width) * NumValues;
            var index = ((int)Math.Round(testPos) + start) % (NumValues + 1);
            var result = upper[index];
            return result;
        }

        public float getLowerAt(float xPos)
        {
            xPos -= xOffset;
            var testPos = (xPos / game.Screen.Width) * NumValues;
            var index = ((int)Math.Round(testPos) + start) % (NumValues + 1);
            var result = lower[index];
            return result;
        }

        public Level(BaseGame game, int num, float timeStep = 50.0f)
        {
            this.game = game;

            upper = new float[num + 1];
            lower = new float[num + 1];
            this.TimeStep = timeStep;

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
            cooldown -= (float)gameTime.ElapsedGameTime.TotalMilliseconds;

            if (cooldown <= 0)
            {
                cooldown = TimeStep;

                if (Generator != null)
                {
                    var t = (float)gameTime.TotalGameTime.TotalSeconds;
                    Push(Generator.GetUpper(t), Generator.GetLower(t));
                }
            }

            float stepWidth = (game.Screen.Width / NumValues);
            float alpha = (TimeStep - cooldown) / TimeStep;
            xOffset = stepWidth * alpha;
        }

        private void Push(float _upper, float _lower)
        {
            var len = upper.Length;
            start = (start + 1) % len;
            int insertPos = (start + len - 1) % len;
            upper[insertPos] = _upper;
            lower[insertPos] = _lower;
        }

        internal override void Draw(SpriteBatch spriteBatch, GameTime gameTime)
        {

            Vector2 pos = Vector2.Zero;

            for (int i = 0; i <= NumValues; ++i)
            {
                var _i = (i + start) % (NumValues+1);

                pos.X = i * (game.Screen.Width / NumValues) - xOffset;
                pos.Y = upper[_i];
                spriteBatch.Draw(part, pos, Color.White);
                pos.Y = lower[_i];
                spriteBatch.Draw(part, pos, Color.White);
            }

            pos.X = 50;
            pos.Y = getUpperAt(pos.X);
            spriteBatch.Draw(part, pos, Color.Green);
            pos.Y = getLowerAt(pos.X);
            spriteBatch.Draw(part, pos, Color.Red);

        }
    }
}
