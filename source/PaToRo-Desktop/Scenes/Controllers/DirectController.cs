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
using Microsoft.Xna.Framework.Content;

namespace PaToRo_Desktop.Scenes.Controllers
{
    public class DirectController : Entity
    {
        private readonly BaseGame game;
        private int playerIdx;

        private Texture2D tex;
        private Vector2 origin;
        private Vector2 scale;
        private Color color;

        public IHasPhysics Entity { get; set; }

        public DirectController(BaseGame game, int playerIdx, IHasPhysics entity)
        {
            this.game = game;
            this.playerIdx = playerIdx;
            Entity = entity;
        }

        public void LoadContent(ContentManager content)
        {
            tex = content.Load<Texture2D>("Images/particle");
            origin = new Vector2(tex.Width * 0.5f, tex.Height * 0.5f);
            scale = new Vector2(0.7f, 0.7f);
            color = new Color(0.2f, 0.2f, 1.0f, 1.0f);
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

        protected override void DrawInternal(SpriteBatch spriteBatch, GameTime gameTime)
        {
            var pos = new Vector2(Entity.Phy.Pos.X, Entity.Phy.Pos.Y);
            for (int i=1; i<10; ++i)
            {
                spriteBatch.Draw(tex, pos, null, null, origin, 0, scale, color);
                pos.X += Entity.Phy.Spd.X * i * 0.01f;
                pos.Y += Entity.Phy.Spd.Y * i * 0.01f;
            }
        }
    }
}
