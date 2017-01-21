using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using PaToRo_Desktop.Engine;
using PaToRo_Desktop.Engine.Input;
using PaToRo_Desktop.Scenes.Controllers;
using PaToRo_Desktop.Scenes.Generators;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PaToRo_Desktop.Scenes
{
    public class TestScene : Scene
    {
        private DebugOverlay dbgOverlay;
        private TheWaveRider ball;
        private DirectController control;
        private Level level;
        public float Phase;
        public float LevelSpeedX;
        public float LevelSpeedY;
        public float SpeedX;
        public float SpeedY;
        public float Direction;
        private Texture2D part;

        public TestScene(BaseGame game) : base(game)
        {
        }

        internal override void Initialize()
        {
            base.Initialize();
            BgColor = Color.Black;
            Direction = 1.0f;
        }

        internal override void LoadContent()
        {
            base.LoadContent();

            level = new Level(game, 128, 15);
            level.LoadContent(game.Content);
            level.Generator = new SineStackedGenerator(game);

            ball = new TheWaveRider();
            ball.LoadContent(game.Content);
            ball.Phy.Pos.X += game.Screen.Width * 0.1f;
            ball.Phy.Pos.Y += game.Screen.Height * 0.5f;

            control = new DirectController(game, 0, ball.Phy);

            LevelSpeedX = 100.0f;
            LevelSpeedY = 150.0f;
            part = game.Content.Load<Texture2D>("Images/particle");

            dbgOverlay = new DebugOverlay(game);

            Children.Add(level);
            Children.Add(control);
            Children.Add(ball);
            Children.Add(dbgOverlay);
        }

        //internal override void Draw(GameTime gameTime)
        //{
        //    base.Draw(gameTime);
        //    int MaxPoints = 100;
        //    float PosX = ball.Phy.Pos.X;
        //    float PosY = ball.Phy.Pos.Y;
        //    float InternalDirection = Direction;
        //    spriteBatch.Begin();
        //    for (int i = 0; i <= MaxPoints; ++i)
        //    {
        //        PosX += 15.0f; // level speed
        //        PosY += 1000.0f * (1.0f + SpeedX) / 2.0f * gameTime.ElapsedGameTime.Milliseconds / 1000.0f * InternalDirection;
        //        PosY += LevelSpeedY * gameTime.ElapsedGameTime.Milliseconds / 1000.0f * InternalDirection;
        //        float Tmp = (SpeedY / 6.0f) + 0.1f;
        //        if (PosY < game.Screen.Height * Tmp)
        //        {
        //            InternalDirection *= -1.0f;
        //            PosY = game.Screen.Height * Tmp;
        //        }
        //        if (PosY > game.Screen.Height * (1.0f - Tmp))
        //        {
        //            InternalDirection *= -1.0f;
        //            PosY = game.Screen.Height * (1.0f - Tmp);

        //        }
        //        Vector2 Pos = new Vector2(PosX, PosY);
        //        spriteBatch.Draw(part, Pos, Color.White);
        //    }
        //    spriteBatch.End();
        //}

        //internal override void Update(GameTime gameTime)
        //{
        //    base.Update(gameTime);
        //    ball.Phy.Pos.Y += 1000.0f * (1.0f + SpeedX) / 2.0f * gameTime.ElapsedGameTime.Milliseconds / 1000.0f * Direction;
        //    ball.Phy.Pos.Y += LevelSpeedY * gameTime.ElapsedGameTime.Milliseconds / 1000.0f * Direction;
        //    float Tmp = (SpeedY / 6.0f) + 0.1f;
        //    if (ball.Phy.Pos.Y < game.Screen.Height * Tmp)
        //    {
        //        Direction *= -1.0f;
        //        ball.Phy.Pos.Y = game.Screen.Height * Tmp;
        //    }
        //    if (ball.Phy.Pos.Y > game.Screen.Height * (1.0f - Tmp))
        //    {
        //        Direction *= -1.0f;
        //        ball.Phy.Pos.Y = game.Screen.Height * (1.0f - Tmp);

        //    }
        //}

        internal override int HandleInput(GameTime gameTime)
        {
            var numPlayers = base.HandleInput(gameTime);

            // register Players
            if (numPlayers < 2)
            {
                dbgOverlay.Text = string.Format("Player {0}, please press a button", numPlayers);
                game.Inputs.AssignToPlayer(numPlayers);

            }

            //// Move TheWaveRider
            //if (numPlayers > 0)
            //{
            //    SpeedY = game.Inputs.Player(0).Value(Sliders.LeftStickY);
            //    SpeedX = game.Inputs.Player(0).Value(Sliders.RightStickX);
            //}

            return numPlayers;
        }
    }
}
