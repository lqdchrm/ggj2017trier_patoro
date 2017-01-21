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

        public IHasPhysics Entity { get; set; }


        public DirectController(BaseGame game, int playerIdx, IHasPhysics entity)
        {
            this.game = game;
            this.playerIdx = playerIdx;
            Entity = entity;
        }


        internal override void Update(GameTime gameTime)
        {
            if (Entity != null && Entity.Phy != null && game.Inputs.NumPlayers > playerIdx)
            {
                var cntrl = game.Inputs.Player(playerIdx);
                Entity.Phy.Spd.X = cntrl.Value(Sliders.LeftStickX) * 800;
                Entity.Phy.Spd.Y = cntrl.Value(Sliders.LeftStickY) * 800;
            }
        }

        internal override void Draw(SpriteBatch spriteBatch, GameTime gameTime)
        {
        }
    }
}
