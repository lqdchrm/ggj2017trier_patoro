using Microsoft.Xna.Framework;
using PaToRo_Desktop.Engine;
using PaToRo_Desktop.Engine.Input;
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
        private Level level;
        public float Phase;
        public float LevelSpeedX;
        public float LevelSpeedY;
        public float SpeedX;
        public float SpeedY;
        public float Direction;
        public float TimePerPhase;

        public TestScene(BaseGame game) : base(game)
        {
        }

        internal override void Initialize()
        {
            base.Initialize();
            BgColor = Color.Blue;
            TimePerPhase = 5000.0f; // 5 seconds
            Direction = 1.0f;
        }

        internal override void LoadContent()
        {
            base.LoadContent();

            ball = new TheWaveRider();
            ball.LoadContent(game.Content);
            ball.Phy.Pos.X += game.Screen.Width * 0.1f;
            ball.Phy.Pos.Y += game.Screen.Height * 0.5f;
            LevelSpeedX = 100.0f;
            LevelSpeedY = 150.0f;
            Children.Add(ball);

            level = new Level(game, 64);
            level.LoadContent(game.Content);
            level.Generator = new SineGenerator(game);
            Children.Add(level);

            dbgOverlay = new DebugOverlay(game);
            Children.Add(dbgOverlay);
        }

        internal override void Update(GameTime gameTime)
        {
            base.Update(gameTime);
            TimePerPhase -= gameTime.ElapsedGameTime.Milliseconds;
            ball.Phy.Pos.Y += 1000.0f * (1.0f + SpeedX)/2.0f * gameTime.ElapsedGameTime.Milliseconds/1000.0f * Direction;
            ball.Phy.Pos.Y += LevelSpeedY * gameTime.ElapsedGameTime.Milliseconds/1000.0f * Direction;
            float Tmp = (SpeedY / 6.0f) + 0.1f;
            if (ball.Phy.Pos.Y < game.Screen.Height * Tmp)
            {
                Direction *= -1.0f;
                ball.Phy.Pos.Y = game.Screen.Height * Tmp;
            }
            if (ball.Phy.Pos.Y > game.Screen.Height * (1.0f - Tmp))
            {
                Direction *= -1.0f;
                ball.Phy.Pos.Y = game.Screen.Height * (1.0f - Tmp);

            }
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

            // Move TheWaveRider
            if (numPlayers > 0)
            {
                SpeedY = game.Inputs.Player(0).Value(Sliders.LeftStickY);
                SpeedX = game.Inputs.Player(0).Value(Sliders.RightStickX);
            }

            return numPlayers;
        }
    }
}
