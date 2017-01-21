using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PaToRo_Desktop.Scenes.Funcs
{
    public static class RandomFuncs
    {
        private static Random rng = new Random();

        public static float FromRange(float min, float max)
        {
            return (float)(min + rng.NextDouble() * (max - min));
        }
    }
}
