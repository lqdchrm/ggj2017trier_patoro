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
using Microsoft.Xna.Framework.Audio;

namespace PaToRo_Desktop.Scenes
{
    public class TheNewWaveRider : Entity, IHasPhysics
    {
        private readonly BaseGame game;

        private Texture2D halo;
        private Vector2 haloOrigin;

        private Texture2D part;
        private Vector2 partOrigin;

        private SoundEffect hitSnd;
        private SoundEffect damageSnd;

        private Color color;
        public Color BaseColor { get; private set; }

        public bool Active;

        public Physics Phy { get; private set; }

        private float initialRadius;
        public float Radius
        {
            get { return this.Phy.HitBox.Radius; }
            set { this.Phy.HitBox.Radius = value; }
        }
        public float Colliding;
        public static float POINTS_PER_FRAME = 0.2f;
        public float Points;

        public Level Level { get; set; }

        public int PlayerNum { get; private set; }
        public float RespawnTimerInSec { get; private set; }

        public static Color[] colors = new Color[] {
            new Color(0xCC, 0x00, 0x00), new Color(0x99, 0xFF, 0x00), new Color(0xFF, 0xCC, 0x00), new Color(0x33, 0x33, 0xFF)
        };

        public TheNewWaveRider(BaseGame game, int playerNum, float radius)
        {
            this.game = game;
            this.PlayerNum = playerNum;
            this.BaseColor = colors[playerNum % colors.Length];
            Phy = new Physics(radius, game);
            initialRadius = radius;
        }

        internal void LoadContent(ContentManager content)
        {
            halo = content.Load<Texture2D>("Images/halo");
            haloOrigin = new Vector2(halo.Width * 0.5f, halo.Height * 0.5f);

            part = content.Load<Texture2D>("Images/particle");
            partOrigin = new Vector2(part.Width * 0.5f, part.Height * 0.5f);

            hitSnd = content.Load<SoundEffect>("Sounds/fx/hit2");
            damageSnd = content.Load<SoundEffect>("Sounds/fx/damage");
        }

        public void Spawn(bool restePosition = true)
        {
            if (!Active)
            {
                Active = true;
                if (restePosition)
                {
                    Phy.Pos.X = game.Screen.Width * 0.1f;
                    Phy.Pos.Y = game.Screen.Height * 0.5f;
                    Phy.Spd = Vector2.Zero;
                    Phy.Accel = Vector2.Zero;
                }
                Radius = initialRadius;
            }
        }

        public void Reset(bool resetPosition = true)
        {
            Points = 0.0f;
            Active = false;
            Spawn(resetPosition);
        }

        public void Die()
        {
            if (Active)
            {
                Active = false;
                RespawnTimerInSec = 3;
            }
        }

        internal override void Draw(SpriteBatch spriteBatch, GameTime gameTime)
        {
            if (Active)
            {
                var t = (float)gameTime.TotalGameTime.TotalSeconds;
                var delta = (float)gameTime.ElapsedGameTime.TotalSeconds;

                // Colliding Viz
                if (Colliding <= 0)
                {
                    color = new Color(BaseColor.R, BaseColor.G, BaseColor.B, BaseColor.A);
                    //new Color(
                    //    BaseFuncs.MapTo(0.5f, 1.0f, BaseFuncs.Sin(t)),      // red
                    //    BaseFuncs.MapTo(0.5f, 1.0f, BaseFuncs.Sin(t + 1)),    // green
                    //    BaseFuncs.MapTo(0.5f, 1.0f, BaseFuncs.Sin(t + 2)),    // blue
                    //    1.0f);
                    Points += POINTS_PER_FRAME;
                }
                else
                {
                    Colliding -= delta;
                }

                // Outer Halo
                var scl = 2 * Radius / halo.Width;

                var dot = Vector2.Dot(Vector2.Normalize(Phy.Spd), Vector2.Normalize(Phy.Accel));
                var squish = BaseFuncs.MapTo(1, 1.3f, Phy.Spd.Length(), 0, 2000);
                if (float.IsNaN(dot))
                    dot = -0.5f;

                squish = (float)Math.Pow(squish, 2 * dot);

                var scale = new Vector2(scl * squish, scl / squish);
                spriteBatch.Draw(halo, Phy.Pos, null, null, haloOrigin, Phy.Rot, scale, color);


                // Dot
                spriteBatch.Draw(part, Phy.Pos, null, null, partOrigin, Phy.Rot, null, color);

                // Halos
                float factor = BaseFuncs.MapTo(0.5f, 1.0f, BaseFuncs.SawUp(t));
                scale.X = scl * factor * squish;
                scale.Y = scl * factor / squish;
                color.A = (byte)(255 * factor);
                spriteBatch.Draw(halo, Phy.Pos, null, null, haloOrigin, Phy.Rot, scale, color);

                // Inner Halos
                factor = BaseFuncs.ToZeroOne(BaseFuncs.SawUp(2 + t * 2.6f));   // -> 0..1
                scale.X = scl * factor * squish;
                scale.Y = scl * factor / squish;
                color.A = (byte)(255 * factor);
                spriteBatch.Draw(halo, Phy.Pos, null, null, haloOrigin, Phy.Rot, scale, color);

                factor = BaseFuncs.ToZeroOne(BaseFuncs.SawUp(0.5f + t * 1.4f));   // -> 0..1
                scale.X = scl * factor * squish;
                scale.Y = scl * factor / squish;
                color.A = (byte)(255 * factor);
                spriteBatch.Draw(halo, Phy.Pos, null, null, haloOrigin, Phy.Rot, scale, color);

                // Border
                var pos = new Vector2();
                pos.X = Phy.Pos.X;
                pos.Y = (game.Scenes.Current as TestScene).Level.getUpperAt(pos.X);
                var _scl = new Vector2(2.5f, 2.5f);
                spriteBatch.Draw(part, pos, null, null, partOrigin, 0, _scl, BaseColor);
                pos.Y = (game.Scenes.Current as TestScene).Level.getLowerAt(pos.X);
                spriteBatch.Draw(part, pos, null, null, partOrigin, 0, _scl, BaseColor);
            }
        }

        internal override void Update(GameTime gameTime)
        {
            var delta = (float)gameTime.ElapsedGameTime.TotalSeconds;

            if (Active)
            {
                Phy.Update(gameTime);

                // Ensure Physics Bounds
                if (Phy.Pos.X < Radius) Phy.Pos.X = Radius;
                if (Phy.Pos.X > game.Screen.Width - Radius) Phy.Pos.X = game.Screen.Width - Radius;
                if (Phy.Pos.Y < Radius) Phy.Pos.Y = Radius;
                if (Phy.Pos.Y > game.Screen.Height - Radius) Phy.Pos.Y = game.Screen.Height - Radius;

                if (Phy.Spd.X > 1200) Phy.Spd.X = 1200;
                if (Phy.Spd.X < -1200) Phy.Spd.X = -1200;
                if (Phy.Spd.Y > 1200) Phy.Spd.Y = 1200;
                if (Phy.Spd.Y < -1200) Phy.Spd.Y = -1200;

                // Check Collision with Level
                if (Level != null && Level.isActive)
                {
                    var upper = Level.getUpperAt(Phy.Pos.X);
                    var lower = Level.getLowerAt(Phy.Pos.X);

                    if (Phy.Pos.Y < upper + Radius)
                    {
                        Phy.Spd.Y = 200f + 0.5f * Math.Abs(Phy.Spd.Y);
                        Phy.Pos.Y = upper + Radius + Phy.Spd.Y * delta;
                        Phy.Accel.Y = 0;
                        Collide(true);
                    }

                    if (Phy.Pos.Y > lower - Radius)
                    {
                        Phy.Spd.Y = -200f + 0.5f * -Math.Abs(Phy.Spd.Y);
                        Phy.Pos.Y = lower - Radius + Phy.Spd.Y * delta;
                        Phy.Accel.Y = 0;
                        Collide(false);
                    }
                }
            }
            else
            {
                RespawnTimerInSec -= delta;
                if (RespawnTimerInSec <= 0)
                {
                    Spawn();
                }
            }
        }

        public void Collide(bool upper)
        {
            hitSnd.Play(0.5f, 0, 0);
            var level = (game.Scenes.Current as TestScene)?.Level;
            if (level == null) return; // sometimes crashes on scene change to end scene
            if (upper)
            {
                level.upperColl.Hit(this);
            }
            else
            {
                level.lowerColl.Hit(this);
            }


            game.Inputs.Player(PlayerNum)?.Rumble(1.0f, 0, 200);
            if (Colliding <= 0)
            {
                var particles = (game.Scenes.Current as TestScene)?.particles;
                particles?.Explode(Phy.Pos + Vector2.UnitY * Radius * (upper ? -1 : 1), 0.5f, BaseColor, (e) =>
                {
                    e.Spd.X = -level.SpdInPixelPerSecond;
                });

                Colliding = 0.5f;

                damageSnd.Play(0.5f, 0, 0);

                if (Radius <= 15.0f)
                {
                    Die();
                }
                else
                {
                    Radius -= 1.2f;
                }
            }
        }
    }
}
