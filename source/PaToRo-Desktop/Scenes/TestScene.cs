﻿using Microsoft.Xna.Framework;
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

        // controls
        private DirectController directControl;
        private AccelController accelControl;

        private Starfield starfield;

        public Level Level;

        private Texture2D part;

        // generators
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
            }

            if (Rider != null)
            {
                Rider.Radius = 32.0f;
                Rider.Phy.Pos.X = game.Screen.Width * 0.1f;
                Rider.Phy.Pos.Y = game.Screen.Height * 0.5f;
            }
        }

        internal override void Draw(GameTime gameTime)
        {
            base.Draw(gameTime);
            Vector2 PlayerPointStringPos = new Vector2();
            float LineOffset = 20.0f;
            spriteBatch.Begin();
            for (int i = 0; i < game.Inputs.NumPlayers; i++)
            {
                spriteBatch.DrawString(game.Fonts.Get("debug"), $"Player 1: {(int)Rider.Points} Points", PlayerPointStringPos, Color.White);
                PlayerPointStringPos.Y += LineOffset;
            }
            spriteBatch.End();
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

                Level = new Level(game, 128, TimeSpan.FromMinutes(2), 100, 2000);
                Level.LoadContent(game.Content);
                Level.Generator = sineGen; // paddle;

                Rider = new TheNewWaveRider(game, 32f);
                Rider.LoadContent(game.Content);
                Rider.Level = Level;
                Rider.Phy.Pos.X = game.Screen.Width * 0.1f;
                Rider.Phy.Pos.Y = game.Screen.Height * 0.5f;

                var Rider2 = new TheNewWaveRider(game, 32f);
                Rider2.LoadContent(game.Content);
                Rider2.Level = Level;
                Rider2.Phy.Pos.X = game.Screen.Width * 0.3f;
                Rider2.Phy.Pos.Y = game.Screen.Height * 0.5f;

                // controllers
                directControl = new DirectController(game, 0, Rider);
                directControl.LoadContent(game.Content);

                accelControl = new AccelController(game, 0, Rider);
                accelControl.LoadContent(game.Content);

                part = game.Content.Load<Texture2D>("Images/particle");

                dbgOverlay = new DebugOverlay(game);

                Children.Add(starfield);
                Children.Add(paddle);
                Children.Add(Level);

                // Children.Add(directControl);
                Children.Add(accelControl);

                Children.Add(Rider);
                Children.Add(Rider2);
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
