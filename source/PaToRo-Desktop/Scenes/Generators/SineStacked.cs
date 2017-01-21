using PaToRo_Desktop.Engine;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PaToRo_Desktop.Scenes.Generators
{
    public class SineStackedGenerator : Generator
    {
        private readonly BaseGame game;

        public SineStackedGenerator(BaseGame game)
        {
            this.game = game;
        }

        public float CalcMid(float t)
        {
            //float Tmp1 = (float)(Math.Sin(t*3) * 50);
            //float Tmp2 = (float)(Math.Sin(t*6) * 20);
            float Tmp1 = (float)(Math.Sin(t*3f));
            float Tmp2 = (float)(Math.Sin(t*0.7f + Math.PI));
            float Tmp3 = ((float)(Math.Sin(t*0.4f)) + 1) / 2.0f;
            return game.Screen.Height * 0.5f - (Tmp1 + Tmp2) * Tmp3 * 100;
        }

        public float GetUpper(float t)
        {
            return CalcMid(t) - (float)Math.Sin(t)*100 - 200;
        }
        public float GetLower(float t)
        {
            return CalcMid(t) + (float)Math.Sin(t)*100 + 200;
        }
    }
}
