using MonoGame.Extended.Shapes;
using PaToRo_Desktop.Engine;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace PaToRo_Desktop.Scenes.Generators
{
    public class PlayerGenerator : Entity, Generator
    {
        private readonly BaseGame game;

        const float paddleThikness = 10f;

        private float position;
        private float braodness;

        private float minbraodness;
        private float maxbraodness;
        private float distanceFromEdge;

        private float braodnessSpeed = 10;
        private float baseSpeed = 5;

        public PlayerGenerator(BaseGame game)
        {
            this.game = game;

            minbraodness = 120;
            maxbraodness = 400;
            distanceFromEdge = 10f;


            braodness = minbraodness + (maxbraodness - minbraodness) / 2f;

        }

        public float CalcMid(float t)
        {
            return (game.Screen.Height * 0.5f) - (float)(Math.Sin(t * 3) * 50);
        }

        public float GetUpper(float t)
        {
            return Top;
        }
        public float GetLower(float t)
        {
            return Bottom;
        }

        internal override void Update(GameTime gameTime)
        {
            var inputState = game.Inputs.Player(1);

            float braodnessChange=0;
            float possitionChange=0;


            if (inputState?.Provider is Engine.Input.XBoxController)
            {
                braodnessChange = inputState.Value(Engine.Input.Sliders.RightStickX) * braodnessSpeed;
                possitionChange = inputState.Value(Engine.Input.Sliders.LeftStickY) * baseSpeed * (braodness / 120);
            }
            else if (inputState?.Provider is Engine.Input.KeyboardController)
            {
                braodnessChange += inputState.IsDown(Engine.Input.Buttons.X) ? braodnessSpeed : 0;
                braodnessChange += inputState.IsDown(Engine.Input.Buttons.A) ? -braodnessSpeed : 0;
                possitionChange += inputState.IsDown(Engine.Input.Buttons.DPad_Up) ? baseSpeed * (braodness / 120) : 0;
                possitionChange += inputState.IsDown(Engine.Input.Buttons.DPad_Down) ? -baseSpeed * (braodness / 120) : 0;
            }


            braodness = MathHelper.Clamp(braodness + braodnessChange, minbraodness, maxbraodness);
            var distanceFromCenter = game.Screen.Height / 2f - distanceFromEdge;
            position = MathHelper.Clamp(position + possitionChange, -(distanceFromCenter - braodness / 2f), distanceFromCenter - braodness / 2f);
        }

        internal override void Draw(SpriteBatch spriteBatch, GameTime gameTime)
        {

            var left = Left;
            float top = Top;
            var height = this.braodness;
            var width = paddleThikness;

            spriteBatch.FillRectangle(new RectangleF(left, top, width, height), Color.Red);
        }

        private float Top => position - this.braodness / 2f + game.Screen.Height / 2f;
        private float Bottom => position + this.braodness / 2f + game.Screen.Height / 2f;
        private float Left => game.Screen.Width - paddleThikness;
        private float Rigth => game.Screen.Width;



    }
}
