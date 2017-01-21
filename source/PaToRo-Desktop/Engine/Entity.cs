using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using PaToRo_Desktop.Engine.Components;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PaToRo_Desktop.Engine
{
    public abstract class Entity
    {
        internal abstract void Update(GameTime gameTime);

        internal abstract void Draw(SpriteBatch spriteBatch, GameTime gameTime);
    }
}
