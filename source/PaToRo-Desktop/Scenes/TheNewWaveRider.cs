using PaToRo_Desktop.Engine;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using PaToRo_Desktop.Engine.Components;
using Microsoft.Xna.Framework.Content;
using PaToRo_Desktop.Scenes.Funcs;

namespace PaToRo_Desktop.Scenes
{
    public class TheNewWaveRider : Entity, IHasPhysics
    {
        private readonly BaseGame game;

        private Texture2D halo;
        private Vector2 haloOrigin;

        private Texture2D part;
        private Vector2 partOrigin;

        public Physics Phy { get; private set; }
        public float Radius;

        public Level Level { get; set; }

        public TheNewWaveRider(BaseGame game, float radius)
        {
            this.game = game;
            Phy = new Physics(null);
            this.Radius = radius;
        }

        internal void LoadContent(ContentManager content)
        {
            halo = content.Load<Texture2D>("Images/halo");
            haloOrigin = new Vector2(halo.Width * 0.5f, halo.Height * 0.5f);

            part = content.Load<Texture2D>("Images/particle");
            partOrigin = new Vector2(part.Width * 0.5f, part.Height * 0.5f);
        }

        internal override void Draw(SpriteBatch spriteBatch, GameTime gameTime)
        {
            var t = (float)gameTime.TotalGameTime.TotalSeconds;

            var color = new Color(
                BaseFuncs.MapTo(0.5f, 1.0f, BaseFuncs.Sin(t)),      // red
                BaseFuncs.MapTo(0.5f, 1.0f, BaseFuncs.Sin(t+1)),    // green
                BaseFuncs.MapTo(0.5f, 1.0f, BaseFuncs.Sin(t+2)),    // blue
                1.0f);

            var scl = 2 * Radius / halo.Width;
            var scale = new Vector2(scl, scl);
            spriteBatch.Draw(halo, Phy.Pos, null, null, haloOrigin, 0, scale, color);
            spriteBatch.Draw(part, Phy.Pos, null, null, partOrigin, 0, null, color);

            float factor = BaseFuncs.ToZeroOne(BaseFuncs.SawUp(t));   // -> 0..1
            scale.X = scale.Y = scl * factor;
            color.A = (byte)(255 * factor);
            spriteBatch.Draw(halo, Phy.Pos, null, null, haloOrigin, 0, scale, color);

            factor = BaseFuncs.ToZeroOne(BaseFuncs.SawUp(2 + t*2.6f));   // -> 0..1
            scale.X = scale.Y = scl * factor;
            color.A = (byte)(255 * factor);
            spriteBatch.Draw(halo, Phy.Pos, null, null, haloOrigin, 0, scale, color);

            factor = BaseFuncs.ToZeroOne(BaseFuncs.SawUp(0.5f + t * 1.4f));   // -> 0..1
            scale.X = scale.Y = scl * factor;
            color.A = (byte)(255 * factor);
            spriteBatch.Draw(halo, Phy.Pos, null, null, haloOrigin, 0, scale, color);
        }

        internal override void Update(GameTime gameTime)
        {
            Phy.Update(gameTime);

            // Check Collision with Level
            if (Level != null)
            {
                var upper = Level.getUpperAt(Phy.Pos.X);
                var lower = Level.getLowerAt(Phy.Pos.X);

                if (Phy.Pos.Y < upper + Radius)
                {
                    Phy.Pos.Y = upper + Radius;
                    game.Inputs.Player(0)?.Rumble(0.5f, 0, 200);
                }

                if (Phy.Pos.Y > lower - Radius)
                {
                    Phy.Pos.Y = lower - Radius;
                    game.Inputs.Player(0)?.Rumble(0, 0.5f, 200);
                }
            }
        }
    }
}
