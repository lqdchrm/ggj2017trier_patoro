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
        private readonly Dictionary<Font, SpriteFont> fonts;

        public Fonts(BaseGame game)
        {
            this.game = game;
            this.fonts = new Dictionary<Font, SpriteFont>();
        }

        public void LoadContent()
        {
            foreach (Font font in Enum.GetValues(typeof(Font)))
            {
                var spriteFont = game.Content.Load<SpriteFont>($"Fonts/{font.ToString()}");
                fonts.Add(font, spriteFont);
            }
        }


        public SpriteFont Get(Font name)
        {
            return fonts[name];
        }

        public void Clear()
        {
            fonts.Clear();
        }
    }

    public enum Font
    {
        DebugFont,
        PressStart2P20,
        PressStart2P18,
        PressStart2P16,
        PressStart2P12,
        PressStart2P10,
        PressStart2P09,
        PressStart2P07,
    }
}
