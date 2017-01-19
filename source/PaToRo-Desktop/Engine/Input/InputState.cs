using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PaToRo_Desktop.Engine.Input
{
    public class InputState
    {
        private readonly bool[] buttons;
        private readonly float[] sliders;

        private readonly bool[] oldButtons;
        private readonly float[] oldSliders;

        public InputProvider Provider { get; set; }

        public int PlayerNo { get; set; }

        public bool AnyButtonDown {  get { return buttons.Any((b) => b); } }

        public InputState(int playerNo = -1)
        {
            this.PlayerNo = -1;
            buttons = new bool[Enum.GetNames(typeof(Buttons)).Length];
            oldButtons = new bool[buttons.Length];

            sliders = new float[Enum.GetNames(typeof(Sliders)).Length];
            oldSliders = new float[sliders.Length];
        }

        public void Update(GameTime gameTime)
        {
            for (int i = 0; i < buttons.Length; ++i)
                oldButtons[i] = buttons[i];

            for (int i = 0; i < sliders.Length; ++i)
                oldSliders[i] = sliders[i];

            if (Provider != null)
            {
                Provider.Update(gameTime);

                foreach (Buttons btn in Enum.GetValues(typeof(Buttons)))
                    buttons[(int)btn] = Provider.Get(btn);

                foreach (Sliders sldr in Enum.GetValues(typeof(Sliders)))
                    sliders[(int)sldr] = Provider.Get(sldr);
            }
            else
            {
                for (int i = 0; i < buttons.Length; ++i)
                    buttons[i] = false;

                for (int i = 0; i < sliders.Length; ++i)
                    sliders[i] = 0.0f;
            }
        }

        public bool IsDown(Buttons btn) { return buttons[(int)btn]; }

        public bool WasPressed(Buttons btn) { return !oldButtons[(int)btn] && buttons[(int)btn]; }

        public bool WasReleased(Buttons btn) { return oldButtons[(int)btn] && !buttons[(int)btn]; }

        public float Value(Sliders sldr) { return sliders[(int)sldr]; }

        public float Delta(Sliders sldr) { return sliders[(int)sldr] - oldSliders[(int)sldr]; }

        public void Rumble(float low, float high, int ms)
        {
            if (Provider != null)
                Provider.Rumble(low, high, ms);
        }
    }
}
