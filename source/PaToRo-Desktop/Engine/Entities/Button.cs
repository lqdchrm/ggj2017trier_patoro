using MonoGame.Extended.Shapes;
using PaToRo_Desktop.Engine.Components;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using PaToRo_Desktop.Scenes;

namespace PaToRo_Desktop.Engine.Entities
{
    public class Button : Entity, IHasPhysics
    {
        private readonly Physics phy;
        private readonly string innerText;
        private readonly BaseGame game;
        public bool IsPresd { get; private set; }

        public bool IsSelected { get; set; }

        public Physics Phy => phy;

        public Button(string innerText, Vector2 position, BaseGame game)
        {
            this.innerText = innerText;
            this.game = game;
            phy = new Components.Physics(20f, game);
            phy.Pos = position;
        }

        internal override void Update(GameTime gameTime)
        {
            phy.Update(gameTime);
            IsPresd = (game.Scenes.Current as TestScene)?.Riders.Any(r => r.Phy.CollidesWith(phy)) ?? false;
        }

        protected override void DrawInternal(SpriteBatch spriteBatch, GameTime gameTime)
        {
            var circle = this.phy.HitBox;
            spriteBatch.DrawCircle(circle, 360, IsPresd ? Color.Red : Color.Green, 2);
            circle.Radius += 10;
            spriteBatch.DrawCircle(circle, 360, IsPresd ? Color.Red : Color.Green, 8);

            if (IsSelected)
            {
                circle.Radius += 10;
                spriteBatch.DrawCircle(circle, 360, IsPresd ? Color.Red : Color.Green, 4);
            }

            var font = game.Fonts.Get(Font.PressStart2P12);
            var textSize = font.MeasureString(innerText);

            spriteBatch.DrawString(font, innerText, circle.Center - textSize / 2f, Color.White);

        }
    }
}
