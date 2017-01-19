using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PaToRo_Desktop.Engine.Components
{
    public class Circle
    {
        public Vector2 Center;
        public float Radius;

        public Circle(Vector2 center, float radius)
        {
            Center.X = center.X;
            Center.Y = center.Y;
            Radius = radius;
        }

        public bool Intersects(Circle other)
        {
            var dx = other.Center.X - Center.X;
            var dy = other.Center.Y - Center.Y;
            var dist = Math.Sqrt(dx * dx + dy * dy);
            return dist < (Radius + other.Radius);
        }
    }
}
