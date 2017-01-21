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
        private float accumulator = 0;          // tracks time till new spawn
        private float xPos;

        public Generator Generator { get; set; }
        public float TimeStep { get; set; }    // time interval to spawn next value

        public float BlockWidth { get { return game.Screen.Width / NumValues; } }
        public float SpdInPixelPerSecond { get { return MathHelper.Lerp(SpdInPixelPerSecondStart, SpdInPixelPerSecondEnd, (float)(localTime / Duration.TotalSeconds)); } }

        public TimeSpan Duration { get; private set; }
        public TimeSpan Elapsed { get { return TimeSpan.FromSeconds(Duration.TotalSeconds - localTime); } }

        public float SpdInPixelPerSecondStart { get; private set; }
        public float SpdInPixelPerSecondEnd { get; private set; }

        public float BlocksPerSecond {  get { return SpdInPixelPerSecond / BlockWidth; } }

        private float localTime;

        public float getUpperAt(float xPos)
        {
            xPos += xOffset;
            var testPos = (xPos / game.Screen.Width) * NumValues;
            var index = ((int)Math.Round(testPos) + start) % (NumValues + 1);
            while(index < 0)
            {
                index += NumValues + 1;
            }
            var result = upper[index];

            // result += BaseFuncs.MapTo(-50, 0, BaseFuncs.Saw(-xPos / 40.0f + localTime * 10) * 2 *BaseFuncs.Sin(xPos / 40.0f + localTime * 5));

            return result;
        }

        public float getLowerAt(float xPos)
        {
            xPos += xOffset;
            var testPos = (xPos / game.Screen.Width) * NumValues;
            var index = ((int)Math.Round(testPos) + start) % (NumValues + 1);
            while (index < 0)
            {
                index += NumValues + 1;
            }
            var result = lower[index];

            // result -= BaseFuncs.MapTo(-50, 0, BaseFuncs.Saw(-xPos / 40.0f + localTime * 10) * 2 * BaseFuncs.Sin(xPos / 40.0f + localTime * 5));

            return result;
        }

        public Level(BaseGame game, int num, TimeSpan duration, float spdInPixelPerSecondStart = 100.0f, float spdInPixelPerSecondEnd = 2000.0f)
        {
            this.game = game;

            upper = new float[num + 1];
            lower = new float[num + 1];
            this.Duration = duration;
            this.SpdInPixelPerSecondStart = spdInPixelPerSecondStart;
            this.SpdInPixelPerSecondEnd = spdInPixelPerSecondEnd;

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
            var t = localTime = (float)gameTime.TotalGameTime.TotalSeconds;

            if (Microsoft.Xna.Framework.Input.Keyboard.GetState().IsKeyDown(Microsoft.Xna.Framework.Input.Keys.D9))
                stopTime = !stopTime;

            if (!stopTime)
            {
                var dx = SpdInPixelPerSecond / 1000.0f * (float)gameTime.ElapsedGameTime.TotalMilliseconds;
                accumulator += dx;
                xPos += dx;
                var numBlocksToSpawn = accumulator / BlockWidth;

                while (numBlocksToSpawn > 1)
                {
                    if (Generator != null)
                    {
                        var dt = numBlocksToSpawn * BlockWidth;
                        Push(Generator.GetUpper((xPos - dt)/ 200), Generator.GetLower((xPos - dt) / 200));
                    }
                    --numBlocksToSpawn;
                    accumulator -= BlockWidth;
                }

                xOffset = (BlockWidth + accumulator) % BlockWidth;
            }
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
                BaseFuncs.MapTo(0.7f, 1.0f, BaseFuncs.Sin(-t)),      // red
                BaseFuncs.MapTo(0.7f, 1.0f, BaseFuncs.Sin(0.8f * t + 0.5f)),    // green
                BaseFuncs.MapTo(0.7f, 1.0f, BaseFuncs.Sin(0.2f * t + 1.7f)),    // blue
                1.0f);


            for (int x = 0; x <= NumValues; ++x)
            {
                // render dots
                var bufferIndex = (x + start) % (NumValues + 1);
                var px = x * (game.Screen.Width / NumValues) - xOffset;
                var pu = getUpperAt(px); // upper[bufferIndex]; // + (float)(Math.Sin(t * 4.2f + x * 0.5f) * 20);
                var pl = getLowerAt(px); // lower[bufferIndex];

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
            pos.X = (game.Scenes.Current as TestScene).Rider.Phy.Pos.X;
            pos.Y = getUpperAt(pos.X);
            var scl = new Vector2(2.5f, 2.5f);
            spriteBatch.Draw(part, pos, null, null, origin, 0, scl, Color.Green);
            pos.Y = getLowerAt(pos.X);
            spriteBatch.Draw(part, pos, null, null, origin, 0, scl, Color.Red);


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
