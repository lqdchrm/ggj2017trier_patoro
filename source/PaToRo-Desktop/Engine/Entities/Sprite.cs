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
    public class Sprite : Entity
    {
        private Physics phy;
        private Image gfx;

        public Sprite(string assetPath)
        {
            gfx = new Image(assetPath);
        }

        public void LoadContent(ContentManager content)
        {
            gfx.LoadContent(content);
            phy = new Physics(gfx.origin.X);
        }

        internal override void Draw(SpriteBatch spriteBatch, GameTime gameTime)
        {
            gfx.Draw(spriteBatch, phy.Pos, phy.Rot);
        }

        internal override void Update(GameTime gameTime)
        {
            phy.Update(gameTime);
        }
    }
}
