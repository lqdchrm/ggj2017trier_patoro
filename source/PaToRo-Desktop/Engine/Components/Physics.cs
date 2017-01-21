using Microsoft.Xna.Framework;
using PaToRo_Desktop.Scenes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PaToRo_Desktop.Engine.Components
{
    public class Physics
    {
        public Vector2 Pos;
        public float Rot;

        public Vector2 Spd;
        public float RotSpd;

        public Vector2 Accel;
        public float Dmp = 1.0f;

        public Circle HitBox;

        private BaseGame game;

        public Physics(float? radius, BaseGame game)
        {
            Pos = new Vector2();
            Rot = 0;

            Spd = new Vector2();
            RotSpd = 0;

            HitBox = radius.HasValue ? new Circle(Vector2.Zero, radius.Value) : null;

            this.game = game;
        }

        public void Update(GameTime gameTime)
        {
            // timestep
            var delta = gameTime.ElapsedGameTime.Milliseconds / 1000.0f;

            // update movement
            Spd.X += Accel.X * delta;
            Spd.Y += Accel.Y * delta;
            Pos.X = Pos.X + Spd.X * delta;
            Pos.Y = Pos.Y + Spd.Y * delta;
            Rot = Rot + RotSpd * delta;

            Spd.X *= Dmp;
            Spd.Y *= Dmp;


            Accel = Vector2.Zero;

            // update hitbox
            if (HitBox != null)
            {
                HitBox.Center.X = Pos.X;
                HitBox.Center.Y = Pos.Y;
            }

            if (game != null)
                foreach (var physics in game.Scenes.Current.Children.OfType<IHasPhysics>())
                {
                    if (ReferenceEquals(physics.Phy, this) || (physics as TheNewWaveRider) == null || !(physics as TheNewWaveRider).Active)
                        continue;

                    if (CollidesWith(physics.Phy))
                    {
                        var accelerationVector = Pos - physics.Phy.Pos;
                        accelerationVector.Normalize();
                        this.Accel += accelerationVector * 5000;
                    }

                }

        }

        public bool CollidesWith(Physics other)
        {
            if (HitBox == null || other.HitBox == null)
                return false;

            return HitBox.Intersects(other.HitBox);
        }

    }
}
