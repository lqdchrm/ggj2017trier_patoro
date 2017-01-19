﻿using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using PaToRo_Desktop.Engine;
using PaToRo_Desktop.Engine.Input;
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

            for (int i=0; i<4; ++i)
                Inputs[i].Provider = new XBoxController(i);

            Scenes.Add("test", new TestScene(this));
            Scenes.Show("test");
        }
    }
}
