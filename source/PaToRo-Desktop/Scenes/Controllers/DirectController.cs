using Microsoft.Xna.Framework;
using PaToRo_Desktop.Engine;
using PaToRo_Desktop.Engine.Components;
using PaToRo_Desktop.Engine.Input;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Xna.Framework.Graphics;

namespace PaToRo_Desktop.Scenes.Controllers
{
    public class DirectController : Entity
    {
        private readonly BaseGame game;
        private int playerIdx;

        public Physics Physics { get; set; }


        public DirectController(BaseGame game, int playerIdx, Physics physics)
        {
            this.game = game;
            this.playerIdx = playerIdx;
            Physics = physics;
        }


        internal override void Update(GameTime gameTime)
        {
            if (Physics != null && game.Inputs.NumPlayers > playerIdx)
            {
                var cntrl = game.Inputs.Player(playerIdx);
                Physics.Spd.X = cntrl.Value(Sliders.LeftStickX) * 800;
                Physics.Spd.Y = cntrl.Value(Sliders.LeftStickY) * 800;
            }
        }

        internal override void Draw(SpriteBatch spriteBatch, GameTime gameTime)
        {
        }
    }
}
