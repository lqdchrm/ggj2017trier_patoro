using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PaToRo_Desktop.Engine
{
    public class Fonts
    {
        private readonly BaseGame game;
        private readonly Dictionary<string, SpriteFont> fonts;

        public Fonts(BaseGame game)
        {
            this.game = game;
            this.fonts = new Dictionary<string, SpriteFont>();
        }

        public void Add(string name, string assetPath)
        {
            var font = game.Content.Load<SpriteFont>(assetPath);
            fonts.Add(name, font);
        }

        public SpriteFont Get(string name)
        {
            return fonts[name];
        }

        public void Clear()
        {
            fonts.Clear();
        }
    }
}
