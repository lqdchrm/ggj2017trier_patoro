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
        public Generator BaseGenerator { get; set; }

        private float from;
        private float to;
        private float targetSpread;
        private float lastSpread;
        private float startSpread;

        public float CurrentSpread { get; private set; }

        public SpreadGenerator(Generator baseGenerator, float startSpread)
        {
            this.BaseGenerator = baseGenerator;
            this.targetSpread = startSpread;
            this.lastSpread = startSpread;
            this.startSpread = startSpread;
        }


        public void NewSpread(float from, float to, float targetSpread)
        {
            this.from = from;
            this.to = to;
            this.lastSpread = this.targetSpread;
            this.targetSpread = targetSpread;
        }



        public float GetUpper(float t)
        {
            float spread = GetSpread(t);

            return BaseGenerator.GetUpper(t) - spread;
        }


        public float GetLower(float t)
        {
            float spread = GetSpread(t);
            return BaseGenerator.GetLower(t) + spread;

        }


        private float GetSpread(float t)
        {
            var currentPosition = Funcs.BaseFuncs.MapTo(0, 1, MathHelper.Clamp(t, from, to), from, to);
            var spread = MathHelper.Lerp(lastSpread, targetSpread, currentPosition);
            CurrentSpread = spread;
            return spread;
        }

        internal void Reset()
        {
            targetSpread = startSpread;
            lastSpread = startSpread;
        }
    }
}
