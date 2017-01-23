using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using PaToRo_Desktop.Engine.Components;
using Microsoft.Xna.Framework.Content;

namespace PaToRo_Desktop.Engine.Entities
{
    public class Sprite : Entity, IHasPhysics
    {
        public Physics Phy { get; private set; }
        public Image Gfx { get; private set; }

        public Sprite(string assetPath)
        {
            Gfx = new Image(assetPath);
        }

        public void LoadContent(ContentManager content)
        {
            Gfx.LoadContent(content);
            Phy = new Physics(Gfx.origin.X, null);
        }

        protected override void DrawInternal(SpriteBatch spriteBatch, GameTime gameTime)
        {
            Gfx.Draw(spriteBatch, Phy.Pos, Phy.Rot);
        }

        internal override void Update(GameTime gameTime)
        {
            Phy.Update(gameTime);
        }
    }
}
