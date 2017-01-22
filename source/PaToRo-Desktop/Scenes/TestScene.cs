﻿using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Graphics;
using PaToRo_Desktop.Engine;
using PaToRo_Desktop.Engine.Entities;
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
    enum state
    {
        Lobby, Prepare, Game
    }

    public class TestScene : StarfieldScene
    {


        private state State = state.Lobby;
        private static float StartZoneSize = 200.0f;

        // sound
        private Synth Synth;

        // entities
        public Level Level;
        private DebugOverlay dbgOverlay;
        internal readonly List<TheNewWaveRider> Riders;
        public ParticleSystem particles;

        // assets
        private Texture2D part;
        private SoundEffect hitSnd;
        private Texture2D arrow;
        private float colorOffset;
        private float PrepareTimer;
        private static float DefaultPrepareTimerInSeconds = 3.0f;
        private SpreadGenerator generator;

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
            spriteBatch.Begin();
            if (State == state.Lobby)
            {
                colorOffset += 0.05f;
                if (colorOffset > 1.0f) colorOffset = 0;
                float dotOffset = 25.0f;
                Vector2 DotPosition = new Vector2(game.Screen.Width - StartZoneSize, 0);
                while (DotPosition.Y < game.Screen.Height)
                {
                    spriteBatch.Draw(part, DotPosition, Color.White);
                    DotPosition.Y += dotOffset;
                }
                Vector2 ArrowPos = new Vector2(game.Screen.Width - StartZoneSize / 2.0f - 50.0f, game.Screen.Height / 2.0f - 128);
                spriteBatch.Draw(arrow, ArrowPos, new Color(0.8f * colorOffset, 0.8f * colorOffset, 0.8f * colorOffset, 0.5f * colorOffset));
                ArrowPos.X += 10.0f;
                spriteBatch.Draw(arrow, ArrowPos, new Color(0.8f * (1.0f - colorOffset), 0.8f * (1.0f - colorOffset), 0.8f * (1.0f - colorOffset), 0.5f * (1.0f - colorOffset)));
            }
            else
            {
                Vector2 PlayerPointStringPos = new Vector2(0, 20);
                float LineOffset = 20.0f;
                for (int i = 0; i < Riders.Count; i++)
                {
                    var rider = Riders[i];
                    spriteBatch.DrawString(
                        game.Fonts.Get("debug"),
                        $"Player {rider.PlayerNum + 1}: {(int)rider.Points} Points {(rider.Active ? "" : "DEAD")}",
                        PlayerPointStringPos, rider.BaseColor);
                    PlayerPointStringPos.Y += LineOffset;
                }
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
            //Generator generator = new UpDownGenerator(game);
            //Generator generator = new SpikeGenerator(game);
            generator = new SpreadGenerator(new SineStackedGenerator(game), 500);
            generator.NewSpread(0, 8, 0);

            Level = new Level(game, 128, TimeSpan.FromSeconds(30), 500, 1000);
            Level.LoadContent(game.Content);
            Level.Generator = generator; // paddle;

            arrow = game.Content.Load<Texture2D>("Images/Arrow");
            particles = new ParticleSystem(game, 10);
            particles.LoadContent(game.Content);

            part = game.Content.Load<Texture2D>("Images/particle");
            dbgOverlay = new DebugOverlay(game);

            Children.Add(Level);
            Children.Add(dbgOverlay);

            Reset();
            Children.Add(particles);

            PrepareTimer = DefaultPrepareTimerInSeconds;
        }
        private bool test;
        internal override void Update(GameTime gameTime)
        {
            base.Update(gameTime);
            if (State == state.Lobby)
            {
                CheckForNewPlayers();
                bool start = Riders.Count > 0 ? true : false;
                foreach (TheNewWaveRider Rider in Riders)
                {
                    start = start && (Rider.Phy.Pos.X > game.Screen.Width - StartZoneSize);
                }
                if (start)
                {
                    foreach (TheNewWaveRider Rider in Riders)
                    {
                        Rider.Points = 0;
                    }
                    Level.Restart();
                    Level.isActive = true;
                    State = state.Prepare;
                }
            }
            else if (State == state.Prepare)
            {
                Level.Restart();
                State = state.Game;
                //PrepareTimer -= (float)gameTime.ElapsedGameTime.TotalSeconds;
                //foreach (TheNewWaveRider Rider in Riders)
                //{
                //    Rider.Phy.Pos.X = game.Screen.Width * 0.1f + (PrepareTimer / DefaultPrepareTimerInSeconds) * (game.Screen.Width - StartZoneSize - game.Screen.Width * 0.1f);
                //    Rider.Phy.Spd.Y = 0.0f;
                //}
                //if (PrepareTimer <= 0)
                //{
                //    State = state.Game;
                //}
            }
            else
            {
                if (Level.CurrentLevelPosition > 20 && !test)
                {
                    test = true;
                    generator.NewSpread(Level.CurrentLevelPosition + 2f, Level.CurrentLevelPosition + 7, 500);

                }
            }
            Synth.Update(gameTime);
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
            PrepareTimer = DefaultPrepareTimerInSeconds;
            Level.isActive = false;
            Level.Restart();
            State = state.Lobby;
            starfield.Speed = 100.0f;
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
