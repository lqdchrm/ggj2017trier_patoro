using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using PaToRo_Desktop.Engine;
using PaToRo_Desktop.Engine.Input;
using PaToRo_Desktop.Scenes.Funcs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PaToRo_Desktop.Scenes.Backgrounds
{
    public class Starfield : Entity
    {
        private readonly BaseGame game;

        private readonly Vector2[] pos;
        private readonly Vector2[] spd;
        private readonly Vector2[] scale;

        private readonly int numStars;
        private readonly int numLayers;

        private Texture2D tex;
        private Vector2 origin;

        public float Speed { get; set; }

        public Starfield(BaseGame game, int numStars, int numLayers)
        {
            this.game = game;
            this.numStars = numStars;
            this.numLayers = numLayers;

            this.pos = new Vector2[numStars];
            this.spd = new Vector2[numStars];
            this.scale = new Vector2[numLayers];

            for (int i = 0; i < numStars; ++i)
            {
                this.pos[i].X = RandomFuncs.FromRange(0, game.Screen.Width);
                this.pos[i].Y = RandomFuncs.FromRange(0, game.Screen.Height);

                var layer = i % numLayers;
                this.spd[i].X = -RandomFuncs.FromRange(0, (float)Math.Pow(2, 1 + layer));
            }

            for (int i = 0; i < numLayers; ++i)
            {
                var s = (float)Math.Pow(i + 1, 1.5) * 0.01f;
                this.scale[i] = new Vector2(s, s);
            }
        }

        public void LoadContent(ContentManager content)
        {
            tex = content.Load<Texture2D>("Images/particle");
            origin = new Vector2(tex.Width * 0.5f, tex.Height * 0.5f);
        }

        internal override void Update(GameTime gameTime)
        {
            var delta = gameTime.ElapsedGameTime.TotalSeconds;

            float sx = 0;
            float sy = 0;


            sx = -Speed;

            sy = 0;

            for (int i = 0; i < numStars; ++i)
            {
                var factor = (i % numLayers) / (float)numLayers;
                pos[i].X += (float)((spd[i].X + sx) * delta * factor);
                pos[i].Y += (float)((spd[i].Y + sy) * delta * factor);

                if (pos[i].X < -20)
                    pos[i].X = game.Screen.Width + 20;

                if (pos[i].Y < -20)
                    pos[i].Y = game.Screen.Height + 20;

                if (pos[i].Y > game.Screen.Height + 20)
                    pos[i].Y = -20;
            }
        }

        internal override void Draw(SpriteBatch spriteBatch, GameTime gameTime)
        {
            for (int i = 0; i < numStars; ++i)
            {
                var layer = i % numLayers;
                var scl = scale[layer];
                spriteBatch.Draw(tex, pos[i], null, null, origin, 0, scl);
            }
        }
    }
}
