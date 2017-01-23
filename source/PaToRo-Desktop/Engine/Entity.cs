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
        public bool IsVisible { get; set; } = true;

        internal abstract void Update(GameTime gameTime);


        public void Draw(SpriteBatch spriteBatch, GameTime gameTime)
        {
            if (IsVisible)
                DrawInternal(spriteBatch, gameTime);
        }
        protected abstract void DrawInternal(SpriteBatch spriteBatch, GameTime gameTime);
    }
}
