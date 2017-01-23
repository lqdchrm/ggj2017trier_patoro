using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace PaToRo_Desktop.Engine.Entities
{
    public class SwitchButtons : Entity
    {
        private readonly Button[] buttons;


        public SwitchButtons(params Button[] buttons)
        {
            this.buttons = buttons;
        }

        protected override void DrawInternal(SpriteBatch spriteBatch, GameTime gameTime)
        {
            foreach (var button in buttons)
                button.Draw(spriteBatch, gameTime);
        }

        internal override void Update(GameTime gameTime)
        {
            foreach (var button in buttons)
                button.Update(gameTime);

            var numberOfPressedButtons = buttons.Count(b => b.IsPresd);
            if (numberOfPressedButtons == 1) // only one button can be selected 
            {
                buttons.First(b => b.IsPresd).IsSelected = true;
                foreach (var button in buttons.Where(b => !b.IsPresd))
                    button.IsSelected = false;

            }
            else if (numberOfPressedButtons > 1) // Deselect all buttons
            {
                foreach (var button in buttons)
                    button.IsSelected = false;

            }
        }
    }
}
