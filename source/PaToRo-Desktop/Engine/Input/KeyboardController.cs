using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using XnaInput = Microsoft.Xna.Framework.Input;

namespace PaToRo_Desktop.Engine.Input
{
    public class KeyboardController : InputProvider
    {
        private int index;
        private XnaInput.KeyboardState st;


        public KeyboardController(int index)
        {
            this.index = index;
        }

        public override void Update(GameTime gameTime)
        {
            st = XnaInput.Keyboard.GetState();
        }

        public override bool Get(Buttons btn)
        {
            switch (btn)
            {
                case Buttons.A: return st.IsKeyDown(XnaInput.Keys.A);
                case Buttons.B: return st.IsKeyDown(XnaInput.Keys.S);
                case Buttons.X: return st.IsKeyDown(XnaInput.Keys.Q);
                case Buttons.Y: return st.IsKeyDown(XnaInput.Keys.W);

                case Buttons.R: return st.IsKeyDown(XnaInput.Keys.D1);
                case Buttons.L: return st.IsKeyDown(XnaInput.Keys.D2);
                case Buttons.LeftStick: return false;
                case Buttons.RightStick: return false;

                case Buttons.DPad_Left: return st.IsKeyDown(XnaInput.Keys.Left);
                case Buttons.DPad_Right: return st.IsKeyDown(XnaInput.Keys.Right);
                case Buttons.DPad_Up: return st.IsKeyDown(XnaInput.Keys.Up);
                case Buttons.DPad_Down: return st.IsKeyDown(XnaInput.Keys.Down);

                case Buttons.Select: return st.IsKeyDown(XnaInput.Keys.LeftControl) || st.IsKeyDown(XnaInput.Keys.RightControl);
                case Buttons.Start: return st.IsKeyDown(XnaInput.Keys.Space);

                case Buttons.ToggleFullscreen: return st.IsKeyDown(XnaInput.Keys.F1);
            }
            return false;
        }

        public float XAxis()
        {
            var left = st.IsKeyDown(XnaInput.Keys.Left) ? -1 : 0;
            var right = st.IsKeyDown(XnaInput.Keys.Right) ? 1 : 0;
            return left + right;
        }

        public float YAxis()
        {
            var up = st.IsKeyDown(XnaInput.Keys.Up) ? -1 : 0;
            var down = st.IsKeyDown(XnaInput.Keys.Down) ? 1 : 0;
            return up + down;
        }

        public override float Get(Sliders sldr)
        {
            switch (sldr)
            {
                case Sliders.LeftStickX: return XAxis();
                case Sliders.LeftStickY: return YAxis();
                //case Sliders.RightStickX: return st.ThumbSticks.Right.X;
                //case Sliders.RightStickY: return -st.ThumbSticks.Right.Y;
                case Sliders.LeftTrigger: return st.IsKeyDown(XnaInput.Keys.LeftShift) ? 1.0f : 0.0f;
                case Sliders.RightTrigger: return st.IsKeyDown(XnaInput.Keys.RightShift) ? 1.0f : 0.0f;
            }
            return 0.0f;
        }

        private bool triggered = false;
        private bool vibrating = false;
        private double cooldown = 0;
        private SharpDX.XInput.Vibration vibration;

        public override void Rumble(float low, float high, int ms)
        {
            if (!vibrating)
            {
                triggered = true;
                cooldown = ms;
                vibration.LeftMotorSpeed = (ushort)(ushort.MaxValue * low);
                vibration.RightMotorSpeed = (ushort)(ushort.MaxValue * high);
            }
        }
    }
}
