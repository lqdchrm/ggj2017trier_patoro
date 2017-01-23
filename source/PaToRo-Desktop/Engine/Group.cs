using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace PaToRo_Desktop.Engine
{
    public class Group : Entity
    {
        public readonly List<Entity> Children;

        public Group()
        {
            Children = new List<Entity>();
        }

        protected override void DrawInternal(SpriteBatch spriteBatch, GameTime gameTime)
        {
            foreach (var c in Children)
                c.Draw(spriteBatch, gameTime);
        }

        internal override void Update(GameTime gameTime)
        {
            foreach (var c in Children)
                c.Update(gameTime);
        }
    }
}
