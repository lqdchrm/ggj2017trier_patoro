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
        private Generator BaseScreen;

        private float from;
        private float to;
        private float targetSpread;
        private float lastSpread;

        public float CurrentSpread { get; private set; }

        public void NewSpread(float from, float to, float targetSpread)
        {
            this.from = from;
            this.to = to;
            this.lastSpread = targetSpread;
            this.targetSpread = targetSpread;


        }


        public SpreadGenerator(Generator baseGenerator, float startSpread)
        {
            this.BaseScreen = baseGenerator;
            this.targetSpread = startSpread;
            this.lastSpread = startSpread;
        }


        public float GetUpper(float t)
        {
            float spread = GetSpread(t);

            return BaseScreen.GetUpper(t) - spread;
        }


        public float GetLower(float t)
        {
            float spread = GetSpread(t);
            return BaseScreen.GetLower(t) + spread;

        }


        private float GetSpread(float t)
        {
            var currentPosition = MathHelper.Clamp(t, from, to);
            var spread = MathHelper.Lerp(lastSpread, targetSpread, currentPosition);
            CurrentSpread = spread;
            return spread;
        }

    }
}
