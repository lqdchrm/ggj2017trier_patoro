using Microsoft.Xna.Framework;
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
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using XnaInput = Microsoft.Xna.Framework.Input;

namespace PaToRo_Desktop.Scenes
{
    enum state
    {
        Lobby, Prepare, Game, Score
    }

    public class TestScene : StarfieldScene
    {
        class PlayerAndPoints
        {
            public int Player;
            public float Points;
            public Color color;
        }

        // score state
        private Color[] Colors = { new Color(191, 91, 91), new Color(198, 185, 85), new Color(134, 180, 96), new Color(60, 141, 136), new Color(89, 87, 88) };
        private static float TextAmplitude = 50.0f;
        private static float TextWavelength = 150.0f;
        private static float TextSpeed = 500.0f;
        private List<PlayerAndPoints> FinalPoints = new List<PlayerAndPoints>();

        private state State = state.Lobby;
        private static float StartZoneSize = 200.0f;

        private float GameOverPositionX;
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
        private bool startSpreading = false;
        private TimeSpan gameEndedAt;

        public TestScene(BaseGame game) : base(game)
        {
            Riders = new List<TheNewWaveRider>();
        }

        internal override void Initialize()
        {
            GameOverPositionX = game.Screen.Width;
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
            else if (State == state.Prepare)
            {
                colorOffset += 0.05f;
                if (colorOffset > 1.0f) colorOffset = 0;
                float dotOffset = 25.0f;
                Vector2 DotPosition = new Vector2(game.Screen.Width - StartZoneSize, 0);
                DotPosition.X = (PrepareTimer / DefaultPrepareTimerInSeconds) * game.Screen.Width - StartZoneSize;
                while (DotPosition.Y < game.Screen.Height)
                {
                    spriteBatch.Draw(part, DotPosition, Color.White);
                    DotPosition.Y += dotOffset;
                }
                Vector2 ArrowPos = new Vector2(DotPosition.X + 50.0f, game.Screen.Height / 2.0f - 128);
                spriteBatch.Draw(arrow, ArrowPos, new Color(0.8f * colorOffset, 0.8f * colorOffset, 0.8f * colorOffset, 0.5f * colorOffset));
                ArrowPos.X += 10.0f;
                spriteBatch.Draw(arrow, ArrowPos, new Color(0.8f * (1.0f - colorOffset), 0.8f * (1.0f - colorOffset), 0.8f * (1.0f - colorOffset), 0.5f * (1.0f - colorOffset)));

            }
            else if (State == state.Score)
            {
                int Counter = 0;
                float OffsetX = 30.0f;
                float OffsetY = 100.0f;
                Vector2 TextPosition = new Vector2(GameOverPositionX, OffsetY);
                foreach (char c in "Game Over !")
                {
                    TextPosition.X += OffsetX;
                    TextPosition.Y = (float)Math.Sin(TextPosition.X * 1 / TextWavelength) * TextAmplitude + OffsetY;
                    spriteBatch.DrawString(game.Fonts.Get("PressStart2P"), $"{c}", TextPosition, Colors[Counter % Colors.Length]);
                    Counter++;
                }


                var drawColor = Colors[0];
                drawColor = Color.Lerp(Color.Transparent, drawColor, MathHelper.Clamp((float)(gameTime.TotalGameTime - gameEndedAt).TotalSeconds / 2, 0, 1));

                Vector2 ScorePosition = new Vector2(game.Screen.Width / 2.0f - 200f, game.Screen.Height / 2.0f);
                spriteBatch.DrawString(game.Fonts.Get("PressStart2P"), "Score", ScorePosition, drawColor);
                ScorePosition.Y += 60;
                foreach (PlayerAndPoints pap in FinalPoints.OrderBy(x => x.Points))
                {

                    drawColor = pap.color;
                    drawColor = Color.Lerp(Color.Transparent, drawColor, MathHelper.Clamp((float)(gameTime.TotalGameTime - gameEndedAt).TotalSeconds / 2, 0, 1));


                    spriteBatch.DrawString(game.Fonts.Get("PressStart2P"), $"Player {pap.Player}: {pap.Points:0}", ScorePosition, drawColor);
                    ScorePosition.Y += 30;
                }
            }
            else
            {
                Vector2 PlayerPointStringPos = new Vector2(8f, 10f);
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

            Level = new Level(game, 128, TimeSpan.FromSeconds(90), 500, 1000);
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

        internal void endgame(GameTime gameTime)
        {
            FinalPoints.Clear();
            foreach (TheNewWaveRider Rider in Riders)
            {
                PlayerAndPoints score = new PlayerAndPoints();
                score.Player = Rider.PlayerNum + 1;
                score.Points = Rider.Points;
                score.color = Rider.BaseColor;
                FinalPoints.Add(score);
            }
            Reset();
            gameEndedAt = gameTime.TotalGameTime;
            State = state.Score;
        }

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
                    State = state.Prepare;
                }
            }
            else if (State == state.Prepare)
            {
                PrepareTimer -= (float)gameTime.ElapsedGameTime.TotalSeconds;
                starfield.Speed = 1000.0f;
                foreach (TheNewWaveRider Rider in Riders)
                {
                    Rider.Phy.Spd.X -= 10.0f;
                }
                if (PrepareTimer <= 0)
                {
                    starfield.Speed = 100.0f;
                    Level.Restart();
                    Level.isActive = true;
                    State = state.Game;
                }
            }
            else if (State == state.Score)
            {
                if (game.Inputs[0].IsDown(Engine.Input.Buttons.Start) || XnaInput.Keyboard.GetState().IsKeyDown(XnaInput.Keys.Enter))
                {
                    Reset();
                    State = state.Lobby;
                    return;
                }
                if (GameOverPositionX > -500.0f)
                {
                    GameOverPositionX -= TextSpeed * (float)gameTime.ElapsedGameTime.TotalSeconds;
                }
                else
                {
                    GameOverPositionX = game.Screen.Width;
                }
            }
            else
            {
                if (Level.Elapsed.TotalSeconds < 2.5)
                {
                    if (!startSpreading)
                    {
                        startSpreading = true;
                        generator.NewSpread(Level.CurrentLevelPosition + 1, Level.CurrentLevelPosition + 6, 500);
                    }
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
            startSpreading = false;
            generator.Reset();
            generator.NewSpread(0, 8, 0);
            PrepareTimer = DefaultPrepareTimerInSeconds;
            Level.isActive = false;
            Level.Restart();
            State = state.Lobby;
            starfield.Speed = 100.0f;
            foreach (TheNewWaveRider Rider in Riders)
            {
                Rider.Reset(false);
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
                //dbgOverlay.Text = string.Format("Player {0}, please press a button", numPlayers);
                game.Inputs.AssignToPlayer(numPlayers);
            }

            return numPlayers;
        }
    }
}
