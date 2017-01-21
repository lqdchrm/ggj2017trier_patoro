using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using XnaInput = Microsoft.Xna.Framework.Input;

namespace PaToRo_Desktop.Engine.Input
{
    public class XBoxController : InputProvider
    {
        private int index;
        private XnaInput.GamePadState st;

        private bool vibrationFaild;

        /* Hack for vibration, f**ng MonoGame doesn't support it */
        private readonly SharpDX.XInput.Controller Controller;

        public XBoxController(int index)
        {
            this.index = index;
            this.Controller = new SharpDX.XInput.Controller((SharpDX.XInput.UserIndex)index);
        }

        public override void Update(GameTime gameTime)
        {
            st = XnaInput.GamePad.GetState(index);
            if (cooldown > 0)
            {
                if (triggered)
                {
                    try
                    {
                        if (!vibrationFaild)
                            Controller.SetVibration(vibration);

                    }
                    catch (Exception)
                    {
                        vibrationFaild = true;
                    }
                    triggered = false;
                    vibrating = true;
                }
                cooldown -= gameTime.ElapsedGameTime.TotalMilliseconds;
                if (cooldown <= 0)
                {
                    vibration.LeftMotorSpeed = 0;
                    vibration.RightMotorSpeed = 0;
                    try
                    {
                        if (!vibrationFaild)
                            Controller.SetVibration(vibration);
                        vibrating = false;
                    }
                    catch (Exception)
                    {
                        vibrationFaild = true;
                    }
                }
            }
        }

        public override bool Get(Buttons btn)
        {
            switch (btn)
            {
                case Buttons.A: return st.Buttons.A == XnaInput.ButtonState.Pressed;
                case Buttons.B: return st.Buttons.B == XnaInput.ButtonState.Pressed;
                case Buttons.X: return st.Buttons.X == XnaInput.ButtonState.Pressed;
                case Buttons.Y: return st.Buttons.Y == XnaInput.ButtonState.Pressed;

                case Buttons.R: return st.Buttons.RightShoulder == XnaInput.ButtonState.Pressed;
                case Buttons.L: return st.Buttons.LeftShoulder == XnaInput.ButtonState.Pressed;
                case Buttons.LeftStick: return st.Buttons.LeftStick == XnaInput.ButtonState.Pressed;
                case Buttons.RightStick: return st.Buttons.RightStick == XnaInput.ButtonState.Pressed;

                case Buttons.DPad_Left: return st.DPad.Left == XnaInput.ButtonState.Pressed;
                case Buttons.DPad_Right: return st.DPad.Right == XnaInput.ButtonState.Pressed;
                case Buttons.DPad_Up: return st.DPad.Up == XnaInput.ButtonState.Pressed;
                case Buttons.DPad_Down: return st.DPad.Down == XnaInput.ButtonState.Pressed;

                case Buttons.Select: return st.Buttons.Back == XnaInput.ButtonState.Pressed;
                case Buttons.Start: return st.Buttons.Start == XnaInput.ButtonState.Pressed;

                case Buttons.ToggleFullscreen: return st.Buttons.Back == XnaInput.ButtonState.Pressed;
            }
            return false;
        }

        public override float Get(Sliders sldr)
        {
            switch (sldr)
            {
                case Sliders.LeftStickX: return st.ThumbSticks.Left.X;
                case Sliders.LeftStickY: return -st.ThumbSticks.Left.Y;
                case Sliders.RightStickX: return st.ThumbSticks.Right.X;
                case Sliders.RightStickY: return -st.ThumbSticks.Right.Y;
                case Sliders.LeftTrigger: return st.Triggers.Left;
                case Sliders.RightTrigger: return st.Triggers.Right;
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
