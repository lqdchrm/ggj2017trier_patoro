using PaToRo_Desktop.Engine;
using PaToRo_Desktop.Scenes.Backgrounds;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PaToRo_Desktop.Scenes
{
    public class StarfieldScene : Scene
    {
        protected static Starfield starfield;


        public StarfieldScene(BaseGame game) : base(game)
        {
        }

        internal override void InternalLoadContent()
        {
            base.InternalLoadContent();
            // Background
            if (starfield == null)
            {
                starfield = new Starfield(game, 700, 8);
                starfield.LoadContent(game.Content);
            }
            Children.Add(starfield);

        }
    }
}
