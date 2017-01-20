using Microsoft.Xna.Framework;
using PaToRo_Desktop.Engine;
using PaToRo_Desktop.Engine.Input;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PaToRo_Desktop.Scenes
{
    public class LevelScene : Scene
    {
        private DebugOverlay dbgOverlay;
        private Level level;

        public LevelScene(BaseGame game) : base(game)
        {
        }

        internal override void Initialize()
        {
            base.Initialize();
            BgColor = Color.Black;
        }

        internal override void LoadContent()
        {
            base.LoadContent();

            level = new Level(game, 64);
            level.LoadContent(game.Content);
            level.Generator = new SineGenerator(game);
            Children.Add(level);

            dbgOverlay = new DebugOverlay(game);
            Children.Add(dbgOverlay);
        }

        internal override int HandleInput(GameTime gameTime)
        {
            var numPlayers = base.HandleInput(gameTime);

            // register Players
            if (numPlayers < 2)
            {
                dbgOverlay.Text = string.Format("Player {0}, please press a button", numPlayers);
                game.Inputs.AssignToPlayer(numPlayers);

            }

            // Rumble
            if (numPlayers > 1)
            {
                dbgOverlay.Text = "Use Triggers to rumble";


                if (game.Inputs.Player(0).Value(Sliders.LeftTrigger) > 0.2
                 || game.Inputs.Player(0).Value(Sliders.RightTrigger) > 0.2)
                {
                    game.Inputs.Player(1).Rumble(
                        game.Inputs.Player(0).Value(Sliders.LeftTrigger),
                        game.Inputs.Player(0).Value(Sliders.RightTrigger),
                        200
                    );
                }

                if (game.Inputs.Player(1).Value(Sliders.LeftTrigger) > 0.2
                 || game.Inputs.Player(1).Value(Sliders.RightTrigger) > 0.2)
                {
                    game.Inputs.Player(0).Rumble(
                        game.Inputs.Player(1).Value(Sliders.LeftTrigger),
                        game.Inputs.Player(1).Value(Sliders.RightTrigger),
                        200
                    );
                }
            }

            return numPlayers;
        }
    }
}
