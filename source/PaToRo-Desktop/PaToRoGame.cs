using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using PaToRo_Desktop.Engine;
using PaToRo_Desktop.Scenes;

namespace PaToRo_Desktop
{
    class PaToRoGame : BaseGame
    {

        public PaToRoGame() : base()
        {
        }

        protected override void Initialize()
        {
            base.Initialize();
            Scenes.Add("test", new TestScene(this));
            Scenes.Show("test");
        }
    }
}
