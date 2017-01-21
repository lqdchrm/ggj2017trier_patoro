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
    public class AccelController : Entity
    {
        private readonly BaseGame game;
        private int playerIdx;

        private Texture2D tex;
        private Vector2 origin;
        private Vector2 scale;
        private Color color;

        public TheNewWaveRider Rider { get; set; }

        public AccelController(BaseGame game, int playerIdx, TheNewWaveRider rider)
        {
            this.game = game;
            this.playerIdx = playerIdx;
            this.Rider = rider;
            Rider.Phy.Dmp = 0.95f;
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
            if (Rider != null && Rider.Phy != null && game.Inputs.NumPlayers > playerIdx)
            {
                var cntrl = game.Inputs.Player(playerIdx);
                Rider.Phy.Accel.X += cntrl.Value(Sliders.LeftStickX) * 2000;
                Rider.Phy.Accel.Y += cntrl.Value(Sliders.LeftStickY) * 2000;

                //if (!Rider.Active && game.Inputs.Player(playerIdx).AnyButtonDown)
                //{
                //    Rider.Spawn();
                //}
            }
        }

        internal override void Draw(SpriteBatch spriteBatch, GameTime gameTime)
        {
            if (Rider.Active)
            {
                var pos = new Vector2(Rider.Phy.Pos.X, Rider.Phy.Pos.Y);
                for (int i = 1; i < 10; ++i)
                {
                    spriteBatch.Draw(tex, pos, null, null, origin, 0, scale, Rider.BaseColor);
                    pos.X += Rider.Phy.Spd.X * i * 0.01f;
                    pos.Y += Rider.Phy.Spd.Y * i * 0.01f;
                }
            }
        }
    }
}
