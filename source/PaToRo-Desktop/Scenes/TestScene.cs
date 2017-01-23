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
    enum State
    {
        Lobby,
        Prepare,
        Game,
        //Score
    }

    public class TestScene : StarfieldScene
    {
        class PlayerAndPoints
        {
            public int Player;
            public float Points;
            public Color color;
        }

        class StateManager
        {

            /// <summary>
            /// The current State of the Game
            /// </summary>
            public State CurrentState { get; private set; }

            /// <summary>
            /// The last State of the Game.
            /// </summary>
            public State LastState { get; private set; }

            /// <summary>
            /// The time when the state Changed. used for transisionts.
            /// </summary>
            public TimeSpan StateChangedTime { get; private set; }


            public void ChangeState(State newState, GameTime gameTime)
            {
                if (CurrentState != newState)
                {
                    LastState = CurrentState;
                    CurrentState = newState;
                    StateChangedTime = gameTime.TotalGameTime;

                }
            }

            public readonly TimeSpan stateTransitionTime = TimeSpan.FromSeconds(1.5);

            public StateManager(State lobby)
            {
                this.CurrentState = lobby;
                this.LastState = lobby;
                StateChangedTime = TimeSpan.Zero;
            }

            public bool IsStateOrTransitionOfState(State stateToCheck, GameTime gameTime)
            {
                return CurrentState == stateToCheck || LastState == stateToCheck && gameTime.TotalGameTime - StateChangedTime < stateTransitionTime;
            }

        }

        // score state
        private Color[] Colors = { new Color(191, 91, 91), new Color(198, 185, 85), new Color(134, 180, 96), new Color(60, 141, 136), new Color(89, 87, 88) };
        private static float TextAmplitude = 50.0f;
        private static float TextWavelength = 150.0f;
        private static float TextSpeed = 500.0f;
        private List<PlayerAndPoints> FinalPoints = new List<PlayerAndPoints>();

        private StateManager State { get; } = new StateManager(Scenes.State.Lobby);


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

        // hints
        private readonly string[] hints = new string[] {
            "Press A to Dash",
            "Press B to Break (Keyboard s)",
            "As long as you do not hit the waves\nyou gain Points",
            "If You hit the waves to often you die",
            "If your dead you will be Respawn",
            "But you will not get any points\nuntill then",
            "Smash in your oponents\nso they hit the wave",
            "There is no reson to die"
        };
        private Button button1;
        private Button button2;
        private Button button3;
        private UpDownGenerator upDownGenerator;
        private SpikeGenerator spikeGenerator;
        private SineStackedGenerator sineGenerator;

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
            if (State.CurrentState == Scenes.State.Lobby) // Because we have a Transition state between Loby and Game we do not add this in the if block below this.
            {
                colorOffset += 0.05f;
                if (colorOffset > 1.0f)
                    colorOffset = 0;
                float dotOffset = 25.0f;
                Vector2 DotPosition = new Vector2(game.Screen.Width - StartZoneSize, 0);
                while (DotPosition.Y < game.Screen.Height)
                {
                    spriteBatch.Draw(part, DotPosition, BlendInOrOut(Scenes.State.Lobby, gameTime, Color.White));
                    DotPosition.Y += dotOffset;
                }
                Vector2 ArrowPos = new Vector2(game.Screen.Width - StartZoneSize / 2.0f - 50.0f, game.Screen.Height / 2.0f - 128);
                spriteBatch.Draw(arrow, ArrowPos, BlendInOrOut(Scenes.State.Lobby, gameTime, new Color(0.8f * colorOffset, 0.8f * colorOffset, 0.8f * colorOffset, 0.5f * colorOffset)));
                ArrowPos.X += 10.0f;
                spriteBatch.Draw(arrow, ArrowPos, BlendInOrOut(Scenes.State.Lobby, gameTime, new Color(0.8f * (1.0f - colorOffset), 0.8f * (1.0f - colorOffset), 0.8f * (1.0f - colorOffset), 0.5f * (1.0f - colorOffset))));
            }

            if (State.IsStateOrTransitionOfState(Scenes.State.Lobby, gameTime))
            {

                // Hints


                var font = this.game.Fonts.Get(Font.PressStart2P16);
                const int timeToShowText = 4;
                var hintIndex = ((int)gameTime.TotalGameTime.TotalSeconds / timeToShowText) % hints.Length;
                var hint = hints[hintIndex];
                var m = font.MeasureString(hint);
                var position = (new Vector2(game.Screen.Width, game.Screen.Height / 2f) - m) / 2f;

                spriteBatch.DrawString(font, hint, position, BlendInOrOut(Scenes.State.Lobby, gameTime, Color.Red));




                if (FinalPoints.Any())
                {
                    int Counter = 0;
                    float OffsetX = 30.0f;
                    float OffsetY = 100.0f;
                    Vector2 TextPosition = new Vector2(GameOverPositionX, OffsetY);
                    foreach (char c in "Game Over !")
                    {
                        TextPosition.X += OffsetX;
                        TextPosition.Y = (float)Math.Sin(TextPosition.X * 1 / TextWavelength) * TextAmplitude + OffsetY;
                        var color = Colors[Counter % Colors.Length];

                        color = BlendInOrOut(Scenes.State.Lobby, gameTime, color);


                        spriteBatch.DrawString(game.Fonts.Get(Font.PressStart2P20), $"{c}", TextPosition, color);
                        Counter++;
                    }


                    var drawColor = Colors[0];
                    if (State.CurrentState == Scenes.State.Lobby)
                        drawColor = BlendIn(gameTime, drawColor);
                    else
                        drawColor = BlendOut(gameTime, drawColor);

                    Vector2 ScorePosition = new Vector2(game.Screen.Width / 2.0f - 200f, game.Screen.Height / 2.0f);
                    spriteBatch.DrawString(game.Fonts.Get(Font.PressStart2P20), "Score", ScorePosition, drawColor);
                    ScorePosition.Y += 60;
                    foreach (PlayerAndPoints pap in FinalPoints.OrderByDescending(x => x.Points))
                    {

                        drawColor = pap.color;
                        drawColor = BlendInOrOut(Scenes.State.Lobby, gameTime, drawColor);

                        spriteBatch.DrawString(game.Fonts.Get(Font.PressStart2P20), $"Player {pap.Player}: {pap.Points:0}", ScorePosition, drawColor);
                        ScorePosition.Y += 30;
                    }


                    drawColor = Colors[0];
                    drawColor = BlendInOrOut(Scenes.State.Lobby, gameTime, drawColor);


                    var subTitle = "Press Start To Join";

                    var subFont = this.game.Fonts.Get(Font.PressStart2P12);
                    var subM = subFont.MeasureString(subTitle);
                    var subPosition = new Vector2(game.Screen.Width / 2f - subM.X / 2f, ScorePosition.Y);


                    if (Math.Floor(gameTime.TotalGameTime.TotalSeconds) % 2 == 0 && this.game.Inputs.NumPlayers < 6)
                        spriteBatch.DrawString(subFont, subTitle, subPosition, drawColor);

                }
                else // Draw Start Text
                {
                    font = this.game.Fonts.Get(Font.PressStart2P20);
                    var title = "Wave Tracer";
                    m = font.MeasureString(title);
                    position = (new Vector2(game.Screen.Width, game.Screen.Height) - m) / 2f;

                    var baseColor = Color.Red;
                    if (State.CurrentState != Scenes.State.Lobby)
                        baseColor = BlendOut(gameTime, baseColor);

                    spriteBatch.DrawString(font, title, position, baseColor);

                    var subTitle = "Press Start To Join";

                    var subFont = this.game.Fonts.Get(Font.PressStart2P12);
                    var subM = subFont.MeasureString(subTitle);
                    var subPosition = (new Vector2(game.Screen.Width, game.Screen.Height + 2 * m.Y) - subM) / 2f;


                    if (Math.Floor(gameTime.TotalGameTime.TotalSeconds) % 2 == 0 && this.game.Inputs.NumPlayers < 6)
                        spriteBatch.DrawString(subFont, subTitle, subPosition, baseColor);


                }

            }

            if (State.CurrentState == Scenes.State.Prepare)
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

            if (State.CurrentState == Scenes.State.Game)
            {
                Vector2 PlayerPointStringPos = new Vector2(8f, 10f);
                float LineOffset = 20.0f;
                for (int i = 0; i < Riders.Count; i++)
                {
                    var rider = Riders[i];
                    spriteBatch.DrawString(
                        game.Fonts.Get(Font.PressStart2P07),
                        $"Player {rider.PlayerNum + 1}: {(int)rider.Points} Points {(rider.Active ? "" : "DEAD")}",
                        PlayerPointStringPos, rider.BaseColor);
                    PlayerPointStringPos.Y += LineOffset;
                }
            }
            spriteBatch.End();
        }

        private Color BlendInOrOut(Scenes.State state, GameTime gameTime, Color drawColor)
        {
            if (State.CurrentState == state)
                drawColor = BlendIn(gameTime, drawColor);
            else
                drawColor = BlendOut(gameTime, drawColor);
            return drawColor;
        }

        private Color BlendOut(GameTime gameTime, Color baseColor)
        {
            var progress = MathHelper.Clamp(((float)((gameTime.TotalGameTime - State.StateChangedTime).TotalSeconds / State.stateTransitionTime.TotalSeconds)), 0f, 1f);
            baseColor = Color.Lerp(baseColor, Color.Transparent, progress);
            return baseColor;
        }

        private Color BlendIn(GameTime gameTime, Color baseColor)
        {
            var progress = MathHelper.Clamp(((float)((gameTime.TotalGameTime - State.StateChangedTime).TotalSeconds / State.stateTransitionTime.TotalSeconds)), 0f, 1f);
            baseColor = Color.Lerp(Color.Transparent, baseColor, progress);
            return baseColor;
        }


        internal override void InternalLoadContent()
        {
            base.InternalLoadContent();

            // Sound
            Synth = new Synth();
            Synth.LoadContent(game.Content);
            hitSnd = game.Content.Load<SoundEffect>("Sounds/fx/hit");

            // Buttons
            button1 = new Button("L1", new Vector2(50, 200), game);
            button2 = new Button("L2", new Vector2(50, 400), game);
            button3 = new Button("L3", new Vector2(50, 600), game);
            var levelSelection = new SwitchButtons(button1, button2, button3);

            Children.Add(levelSelection);

            // Gens
            upDownGenerator = new UpDownGenerator(game);
            spikeGenerator = new SpikeGenerator(game);
            sineGenerator = new SineStackedGenerator(game);
            generator = new SpreadGenerator(sineGenerator, 500);
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

            Reset(new GameTime());
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
            Reset(gameTime);
            generator.BaseGenerator = null; // we must only reset the basegenerator after the game, not every time reset is called.
            // and we must reset the buttons.
            button1.IsSelected = false;
            button2.IsSelected = false;
            button3.IsSelected = false;

            State.ChangeState(Scenes.State.Lobby, gameTime);
        }

        internal override void Update(GameTime gameTime)
        {
            base.Update(gameTime);
            if (State.CurrentState == Scenes.State.Lobby)
            {
                button1.IsVisible = true;
                button2.IsVisible = true;
                button3.IsVisible = true;

                if (button1.IsSelected)
                {
                    generator.BaseGenerator = sineGenerator;
                }
                else if (button2.IsSelected)
                {
                    generator.BaseGenerator = spikeGenerator;
                }
                else if (button3.IsSelected)
                {
                    generator.BaseGenerator = upDownGenerator;
                }
                else
                {
                    generator.BaseGenerator = null;
                }


                CheckForNewPlayers();
                bool start = Riders.Count > 0 ? true : false;
                foreach (TheNewWaveRider Rider in Riders)
                {
                    start = start && (Rider.Phy.Pos.X > game.Screen.Width - StartZoneSize);
                }
                if (start && generator.BaseGenerator != null)
                {
                    foreach (TheNewWaveRider Rider in Riders)
                    {
                        Rider.Points = 0;
                    }
                    State.ChangeState(Scenes.State.Prepare, gameTime);
                }


            }
            else
            {
                button1.IsVisible = false;
                button2.IsVisible = false;
                button3.IsVisible = false;
            }


            if (State.IsStateOrTransitionOfState(Scenes.State.Lobby, gameTime))
            {
                if (FinalPoints.Any())
                {
                    if (GameOverPositionX > -500.0f)
                    {
                        GameOverPositionX -= TextSpeed * (float)gameTime.ElapsedGameTime.TotalSeconds;
                    }
                    else
                    {
                        GameOverPositionX = game.Screen.Width;
                    }
                }
            }

            if (State.CurrentState == Scenes.State.Prepare)
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
                    State.ChangeState(Scenes.State.Game, gameTime);
                }
            }
            //else if (State == state.Score)
            //{
            //    if (game.Inputs[0].IsDown(Engine.Input.Buttons.Start) || XnaInput.Keyboard.GetState().IsKeyDown(XnaInput.Keys.Enter))
            //    {
            //        Reset();
            //        State = state.Lobby;
            //        return;
            //    }
            //    if (GameOverPositionX > -500.0f)
            //    {
            //        GameOverPositionX -= TextSpeed * (float)gameTime.ElapsedGameTime.TotalSeconds;
            //    }
            //    else
            //    {
            //        GameOverPositionX = game.Screen.Width;
            //    }
            //}
            if (State.CurrentState == Scenes.State.Game)
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

        public void Reset(GameTime gameTime)
        {
            startSpreading = false;
            var random = new Random();
            var randValue = random.NextDouble();

            if (randValue < 0.3)
                Synth.Play("ggg_1", true);
            else if (randValue < 0.6)
                Synth.Play("ggg_2", true);
            else
                Synth.Play("ggg_3", true);

            generator.Reset();
            generator.NewSpread(0, 8, 0);
            PrepareTimer = DefaultPrepareTimerInSeconds;
            Level.isActive = false;
            Level.Restart();
            State.ChangeState(Scenes.State.Lobby, gameTime);
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
