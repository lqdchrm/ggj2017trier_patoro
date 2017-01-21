using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using PaToRo_Desktop.Engine;
using PaToRo_Desktop.Engine.Input;
using PaToRo_Desktop.Scenes.Backgrounds;
using PaToRo_Desktop.Scenes.Controllers;
using PaToRo_Desktop.Scenes.Funcs;
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
        internal TheNewWaveRider Rider;
        private DirectController control;
        private Starfield starfield;

        private Level level;

        public float Phase;
        public float LevelSpeedX;
        public float LevelSpeedY;
        public float SpeedX;
        public float SpeedY;
        public float Direction;
        private Texture2D part;
        private PlayerGenerator paddle;
        private SineStackedGenerator sineGen;

        public TestScene(BaseGame game) : base(game)
        {
        }

        internal override void Initialize()
        {
            if (!initialized)
            {
                base.Initialize();
                BgColor = Color.Black;
                Direction = 1.0f;
            }

            if (Rider != null)
            {
                Rider.Radius = 32.0f;
                Rider.Phy.Pos.X = game.Screen.Width * 0.1f;
                Rider.Phy.Pos.Y = game.Screen.Height * 0.5f;
            }
        }

        internal override void LoadContent()
        {
            if (!loaded)
            {
                base.LoadContent();

                starfield = new Starfield(game, 700, 8);
                starfield.LoadContent(game.Content);

                // Gens
                paddle = new PlayerGenerator(game);
                sineGen = new SineStackedGenerator(game);

                level = new Level(game, 128, 500);
                level.LoadContent(game.Content);
                level.Generator = sineGen; // paddle;

                Rider = new TheNewWaveRider(game, 32f);
                Rider.LoadContent(game.Content);
                Rider.Level = level;
                Rider.Phy.Pos.X = game.Screen.Width * 0.1f;
                Rider.Phy.Pos.Y = game.Screen.Height * 0.5f;

                control = new DirectController(game, 0, Rider);
                control.LoadContent(game.Content);

                LevelSpeedX = 100.0f;
                LevelSpeedY = 150.0f;
                part = game.Content.Load<Texture2D>("Images/particle");

                dbgOverlay = new DebugOverlay(game);

                Children.Add(starfield);
                Children.Add(paddle);
                Children.Add(level);
                Children.Add(control);
                Children.Add(Rider);
                Children.Add(dbgOverlay);
            }
        }

        internal override void Update(GameTime gameTime)
        {
            var t = (float)gameTime.TotalGameTime.TotalSeconds;

            // change background color
            BgColor = new Color(
                BaseFuncs.MapTo(0.0f, 0.2f, BaseFuncs.Sin(0.2f * t)),           // red
                BaseFuncs.MapTo(0.0f, 0.2f, BaseFuncs.Sin(0.1f * -t + 0.8f)),   // green
                BaseFuncs.MapTo(0.0f, 0.2f, BaseFuncs.Sin(0.4f * t + 1.7f)),    // blue
                1.0f);

            // manipulate screen matrix
            //screenMatrix = Matrix.CreateTranslation(-game.Screen.Width * 0.5f, -game.Screen.Height * 0.5f, 0)
            //    * Matrix.CreateScale(BaseFuncs.MapTo(1f, 1.15f, BaseFuncs.Saw(t*0.2f)))
            //    * Matrix.CreateRotationZ(0.3f * MathHelper.PiOver4 * BaseFuncs.Sin(0.05f * t))
            //    * Matrix.CreateTranslation(game.Screen.Width * 0.5f, game.Screen.Height * 0.5f, 0);

            base.Update(gameTime);
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

            return numPlayers;
        }
    }
}
