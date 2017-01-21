using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using PaToRo_Desktop.Engine;
using PaToRo_Desktop.Engine.Input;
using PaToRo_Desktop.Engine.Sound;
using PaToRo_Desktop.Scenes.Backgrounds;
using PaToRo_Desktop.Scenes.Controllers;
using PaToRo_Desktop.Scenes.Funcs;
using PaToRo_Desktop.Scenes.Generators;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using XnaInput = Microsoft.Xna.Framework.Input;

namespace PaToRo_Desktop.Scenes
{
    public class TestScene : Scene
    {
        private DebugOverlay dbgOverlay;
        internal readonly List<TheNewWaveRider> Riders;

        // sound
        private Synth Synth;

        private Starfield starfield;

        public Level Level;

        private Texture2D part;

        // generators
        private Generator sineGen;

        public TestScene(BaseGame game) : base(game)
        {
            Riders = new List<TheNewWaveRider>();
        }

        internal override void Initialize()
        {
            if (!initialized)
            {
                base.Initialize();
                BgColor = Color.Black;
            }
        }

        internal override void Draw(GameTime gameTime)
        {
            base.Draw(gameTime);
            Vector2 PlayerPointStringPos = new Vector2();
            float LineOffset = 20.0f;
            spriteBatch.Begin();
            for (int i = 0; i < Riders.Count; i++)
            {
                var rider = Riders[i];
                spriteBatch.DrawString(
                    game.Fonts.Get("debug"),
                    $"Player {rider.PlayerNum}: {(int)rider.Points} Points",
                    PlayerPointStringPos, rider.BaseColor);
                PlayerPointStringPos.Y += LineOffset;
            }
            spriteBatch.End();
        }

        internal override void LoadContent()
        {
            if (!loaded)
            {
                base.LoadContent();

                // Sound
                Synth = new Synth();
                Synth.LoadContent(game.Content);

                // Background
                starfield = new Starfield(game, 700, 8);
                starfield.LoadContent(game.Content);

                // Gens
                sineGen = new SpreadGenerator( new SineStackedGenerator(game));

                Level = new Level(game, 128, TimeSpan.FromMinutes(2), 100, 2000);
                Level.LoadContent(game.Content);
                Level.Generator = sineGen; // paddle;

                part = game.Content.Load<Texture2D>("Images/particle");
                dbgOverlay = new DebugOverlay(game);

                Children.Add(starfield);
                Children.Add(Level);
                Children.Add(dbgOverlay);
            }
        }

        internal override void Update(GameTime gameTime)
        {
            while (Riders.Count < game.Inputs.NumPlayers)
            {
                var newRider = new TheNewWaveRider(game, Riders.Count, 32f);
                newRider.LoadContent(game.Content);
                newRider.Level = Level;
                newRider.Phy.Pos.X = game.Screen.Width * 0.1f;
                newRider.Phy.Pos.Y = game.Screen.Height * 0.5f;
                Children.Add(newRider);

                AccelController controller = new AccelController(game, Riders.Count, newRider);
                controller.LoadContent(game.Content);
                Children.Add(controller);

                Riders.Add(newRider);
            }
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

            // Sound
            Synth.Update(gameTime);

            base.Update(gameTime);
        }

        internal override int HandleInput(GameTime gameTime)
        {
            var numPlayers = base.HandleInput(gameTime);

            if (XnaInput.Keyboard.GetState().IsKeyDown(XnaInput.Keys.D5))
                Synth.Play("ggg_1");

            if (XnaInput.Keyboard.GetState().IsKeyDown(XnaInput.Keys.D6))
                Synth.Play("ggg_2");

            if (XnaInput.Keyboard.GetState().IsKeyDown(XnaInput.Keys.D7))
                Synth.Play("ggg_3");

            // register Players
            if (numPlayers < 6)
            {
                dbgOverlay.Text = string.Format("Player {0}, please press a button", numPlayers);
                game.Inputs.AssignToPlayer(numPlayers);

            }

            return numPlayers;
        }
    }
}
