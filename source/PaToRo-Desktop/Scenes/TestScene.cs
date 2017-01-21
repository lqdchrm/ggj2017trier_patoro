using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
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
    public class TestScene : StarfieldScene
    {
        private DebugOverlay dbgOverlay;
        internal readonly List<TheNewWaveRider> Riders;

        // sound
        private Synth Synth;


        public Level Level;

        private Texture2D part;
        private SoundEffect hitSnd;

        internal static float BaseScale = 1f;

        // generators
        private Generator generator;

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
            Vector2 PlayerPointStringPos = new Vector2(0, 20);
            float LineOffset = 20.0f;
            spriteBatch.Begin();
            for (int i = 0; i < Riders.Count; i++)
            {
                var rider = Riders[i];
                spriteBatch.DrawString(
                    game.Fonts.Get("debug"),
                    $"Player {rider.PlayerNum + 1}: {(int)rider.Points} Points {(rider.Active ? "" : "DEAD")}",
                    PlayerPointStringPos, rider.BaseColor);
                PlayerPointStringPos.Y += LineOffset;
            }
            spriteBatch.End();
        }

        internal override void InternalLoadContent()
        {
            base.InternalLoadContent();

            // Sound
            Synth = new Synth();
            Synth.LoadContent(game.Content);

            hitSnd = game.Content.Load<SoundEffect>("Sounds/fx/hit");



            // Gens
            generator = new SpreadGenerator(new SineStackedGenerator(game));

            Level = new Level(game, 128, TimeSpan.FromSeconds(120), 50, 1000);
            Level.LoadContent(game.Content);
            Level.Generator = generator; // paddle;

            part = game.Content.Load<Texture2D>("Images/particle");
            dbgOverlay = new DebugOverlay(game);

            Children.Add(Level);
            Children.Add(dbgOverlay);
        }

        internal override void Update(GameTime gameTime)
        {
            CheckForNewPlayers();

            var t = (float)gameTime.TotalGameTime.TotalSeconds;

            if (BaseScale > 1)
            {
                BaseScale -= t * 0.05f;
                BaseScale = MathHelper.Clamp(BaseScale, 1, 3);
            }

            // change background color
            //BgColor = new Color(
            //    BaseFuncs.MapTo(0.0f, 0.2f, BaseFuncs.Sin(0.2f * t)),           // red
            //    BaseFuncs.MapTo(0.0f, 0.2f, BaseFuncs.Sin(0.1f * -t + 0.8f)),   // green
            //    BaseFuncs.MapTo(0.0f, 0.2f, BaseFuncs.Sin(0.4f * t + 1.7f)),    // blue
            //    1.0f);

            // manipulate screen matrix
            //screenMatrix = Matrix.CreateTranslation(-game.Screen.Width * 0.5f, -game.Screen.Height * 0.5f, 0)
            //    * Matrix.CreateScale(BaseFuncs.MapTo(1f, 1.15f, BaseFuncs.Saw(t*0.2f)))
            //    * Matrix.CreateRotationZ(0.3f * MathHelper.PiOver4 * BaseFuncs.Sin(0.05f * t))
            //    * Matrix.CreateTranslation(game.Screen.Width * 0.5f, game.Screen.Height * 0.5f, 0);

            // Sound
            Synth.Update(gameTime);

            base.Update(gameTime);

            CheckPlayerCollisions(gameTime);
        }

        private void CheckPlayerCollisions(GameTime gameTime)
        {
            var Actives = Riders.Where(r => r.Active).ToArray();

            var Pairs = new List<Tuple<TheNewWaveRider, TheNewWaveRider>>();

            for (var i = 0; i < Actives.Length; ++i)
            {
                var first = Actives[i];
                for (var j = i + 1; j < Actives.Length; ++j)
                {
                    var second = Actives[j];

                    if (first.Phy.CollidesWith(second.Phy))
                    {
                        Pairs.Add(Tuple.Create(first, second));
                        hitSnd.Play();
                        game.Inputs.Player(first.PlayerNum)?.Rumble(0, 1.0f, 200);
                        game.Inputs.Player(second.PlayerNum)?.Rumble(0, 1.0f, 200);

                    }
                }
            }

            foreach (var pair in Pairs)
            {
                var tmp = pair.Item1.Phy.Spd;
                pair.Item1.Phy.Spd = pair.Item2.Phy.Spd;
                pair.Item2.Phy.Spd = tmp;

                tmp = pair.Item1.Phy.Accel;
                pair.Item1.Phy.Accel = pair.Item2.Phy.Accel;
                pair.Item2.Phy.Accel = tmp;

                var diff = pair.Item2.Phy.Pos - pair.Item1.Phy.Pos;
                var contactPoint = pair.Item1.Phy.Pos + diff * (pair.Item1.Radius / (pair.Item1.Radius + pair.Item2.Radius));

                if (diff.Length() == 0)
                    diff = Vector2.UnitX;

                var diffN = Vector2.Normalize(diff);

                pair.Item2.Phy.Pos = contactPoint + diffN * (pair.Item2.Radius + 1);
                pair.Item1.Phy.Pos = contactPoint - diffN * (pair.Item1.Radius + 1);
            }
        }

        public void Reset()
        {
            Level.Restart();
            foreach (TheNewWaveRider Rider in Riders)
            {
                Rider.Reset();
            }
        }

        private void CheckForNewPlayers()
        {
            while (Riders.Count < game.Inputs.NumPlayers)
            {
                var newRider = new TheNewWaveRider(game, Riders.Count, 32f);
                newRider.LoadContent(game.Content);
                newRider.Level = Level;
                Children.Add(newRider);

                AccelController controller = new AccelController(game, Riders.Count, newRider);
                controller.LoadContent(game.Content);
                Children.Add(controller);

                Riders.Add(newRider);
                newRider.Spawn();
            }
        }

        internal override int HandleInput(GameTime gameTime)
        {
            var numPlayers = base.HandleInput(gameTime);

            if (XnaInput.Keyboard.GetState().IsKeyDown(XnaInput.Keys.D5))
                Synth.Play("ggg_1", true);

            if (XnaInput.Keyboard.GetState().IsKeyDown(XnaInput.Keys.D6))
                Synth.Play("ggg_2", true);

            if (XnaInput.Keyboard.GetState().IsKeyDown(XnaInput.Keys.D7))
                Synth.Play("ggg_3", true);

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
