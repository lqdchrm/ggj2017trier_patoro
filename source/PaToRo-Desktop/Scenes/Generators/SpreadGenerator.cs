using Microsoft.Xna.Framework;
using PaToRo_Desktop.Engine;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PaToRo_Desktop.Scenes.Generators
{
    public class SpreadGenerator : Generator
    {
        private readonly Generator baseGenerator;

        private float spread = 1000;
        private float spreadLength = 1000;

        public SpreadGenerator(Generator baseGenerator)
        {
            this.baseGenerator = baseGenerator;
        }


        public float GetUpper(float t)
        {
            return baseGenerator.GetUpper(t) - MathHelper.Lerp(0, spread, MathHelper.Clamp(t, 0, spreadLength));
        }
        public float GetLower(float t)
        {
            return baseGenerator.GetLower(t) + MathHelper.Lerp(0, spread, MathHelper.Clamp(t, 0, spreadLength));

        }
    }
}
