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
using PaToRo_Desktop.Scenes.Generators;
using PaToRo_Desktop.Scenes.Funcs;

namespace PaToRo_Desktop.Scenes
{
    public class Level : Entity
    {
        private readonly BaseGame game;
        private Texture2D part;

        private int NumValues { get { return upper.Length - 1; } }

        private float[] upper;
        private float[] lower;
        private int start = 0;
        private float xOffset;
        private float cooldown = 0;          // tracks time till new spawn

        public Generator Generator { get; set; }
        public float TimeStep { get; set; }    // time interval to spawn next value

        public float BlockWidth { get { return game.Screen.Width / NumValues; } }
        public float SpdInPixelPerSecond {  get { return BlockWidth * 1000.0f / TimeStep; } }

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

        private bool stopTime;

        internal override void Update(GameTime gameTime)
        {
            if (Microsoft.Xna.Framework.Input.Keyboard.GetState().IsKeyDown(Microsoft.Xna.Framework.Input.Keys.D9))
                stopTime = !stopTime;

            if (!stopTime)
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

            float alpha = (TimeStep - cooldown) / TimeStep;
            xOffset = BlockWidth * alpha;
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
            Vector2 origin = new Vector2(part.Width * 0.5f, part.Height * 0.5f);
            var t = (float)gameTime.TotalGameTime.TotalSeconds;

            var color = new Color(
                BaseFuncs.MapTo(0.5f, 1.0f, BaseFuncs.Sin(t)),      // red
                BaseFuncs.MapTo(0.5f, 1.0f, BaseFuncs.Sin(t + 0.5f)),    // green
                BaseFuncs.MapTo(0.5f, 1.0f, BaseFuncs.Sin(t + 1.7f)),    // blue
                1.0f);


            for (int x = 0; x <= NumValues; ++x)
            {
                // render dots
                var bufferIndex = (x + start) % (NumValues + 1);
                var px = x * (game.Screen.Width / NumValues) - xOffset;
                var pu = upper[bufferIndex]; // + (float)(Math.Sin(t * 4.2f + x * 0.5f) * 20);
                var pl = lower[bufferIndex];

                pos.X = px;
                pos.Y = pu;
                spriteBatch.Draw(part, pos, null, null, origin, 0, null, color);

                pos.Y = pl;
                spriteBatch.Draw(part, pos, null, null, origin, 0, null, color);

                // render rects
                //spriteBatch.FillRectangle(new RectangleF(px - (0.5f * BlockWidth), 0, BlockWidth, pu), Color.Red);
                //spriteBatch.DrawRectangle(new RectangleF(px - (0.5f * BlockWidth), 0, BlockWidth, pu), Color.DarkRed);

                //spriteBatch.FillRectangle(new RectangleF(px - (0.5f * BlockWidth), pl, BlockWidth, game.Screen.Height - pl), Color.Green);
                //spriteBatch.DrawRectangle(new RectangleF(px - (0.5f * BlockWidth), pl, BlockWidth, game.Screen.Height - pl), Color.DarkGreen);
            }

            // render check points
            pos.X = 50;
            pos.Y = getUpperAt(pos.X);
            spriteBatch.Draw(part, pos, null, null, origin, 0, null, Color.Green);
            pos.Y = getLowerAt(pos.X);
            spriteBatch.Draw(part, pos, null, null, origin, 0, null, Color.Red);


#if DEBUG
            //for (int x = 1; x <= NumValues; x++)
            //{
            //    var bufferIndex = (x + start) % (NumValues + 1);
            //    var previousBufferindex = bufferIndex - 1;
            //    if (previousBufferindex == -1)
            //        previousBufferindex = NumValues;
            //    spriteBatch.DrawLine(
            //        new Vector2(x * BlockWidth - xOffset, upper[bufferIndex]),
            //        new Vector2((x - 1) * BlockWidth - xOffset, upper[previousBufferindex]),
            //        Color.Green);


            //    spriteBatch.DrawLine(
            //        new Vector2(x * BlockWidth - xOffset, lower[bufferIndex]),
            //        new Vector2((x - 1) * BlockWidth - xOffset, lower[previousBufferindex]),
            //        Color.Red);

            //}

#endif
        }
    }
}
