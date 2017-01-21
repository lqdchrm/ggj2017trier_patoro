using Microsoft.Xna.Framework;
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

        public Vector2 Accel;
        public Vector2 Spd;
        public float RotSpd;


        public Circle HitBox;

        public Physics(float? radius)
        {
            Pos = new Vector2();
            Rot = 0;

            Spd = new Vector2();
            RotSpd = 0;

            HitBox = radius.HasValue ? new Circle(Vector2.Zero, radius.Value) : null;
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

            // update hitbox
            if (HitBox != null)
            {
                HitBox.Center.X = Pos.X;
                HitBox.Center.Y = Pos.Y;
            }
        }

        public bool CollidesWith(Physics other)
        {
            if (HitBox != null && other.HitBox != null)
                return false;
                
            return HitBox.Intersects(other.HitBox);
        }

    }
}
