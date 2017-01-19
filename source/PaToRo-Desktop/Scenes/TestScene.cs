using Microsoft.Xna.Framework;
using PaToRo_Desktop.Engine;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PaToRo_Desktop.Scenes
{
    public class TestScene : Scene
    {
        public TestScene(BaseGame game) : base(game)
        {
        }

        internal override void Initialize()
        {
            base.Initialize();
            BgColor = Color.Green;
        }

        internal override void LoadContent()
        {
            base.LoadContent();

            var ball = new Ball();
            ball.LoadContent(game.Content);
            Children.Add(ball);

            var dbgOverlay = new DebugOverlay(game);
            Children.Add(dbgOverlay);
        }
    }
}
