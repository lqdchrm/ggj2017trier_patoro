using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PaToRo_Desktop.Engine.Input
{
    public abstract class InputProvider
    {
        public abstract void Update(GameTime gameTime);

        public abstract bool Get(Buttons btn);

        public abstract float Get(Sliders sldr);

        public abstract void Rumble(float low, float high, int ms);
    }
}
