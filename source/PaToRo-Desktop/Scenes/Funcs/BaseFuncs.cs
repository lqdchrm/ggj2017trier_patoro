using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PaToRo_Desktop.Scenes.Funcs
{
    /// <summary>
    /// Base Functions that should loop somehow around 2*PI and provide values between -1..1
    /// </summary>
    public static class BaseFuncs
    {
        public static float Sin(float t)
        {
            return (float)Math.Sin(t);
        }

        public static float Cos(float t)
        {
            return (float)Math.Cos(t);
        }

        /// <summary>
        /// sawtooth from -1 to -1 over 1, i.e /\
        /// </summary>
        /// <param name="t"></param>
        /// <returns></returns>
        public static float Saw(float t)
        {
            float min = -1;
            float max = 1;
            float alpha = (MathHelper.Pi + MathHelper.WrapAngle(t)) / MathHelper.TwoPi;
            if (alpha < 0.5)
                return MathHelper.Lerp(min, max, 2 * alpha);
            else
                return MathHelper.Lerp(min, max, 2 * (1-alpha));
        }

        /// <summary>
        /// sawtooth from -1 to 1 increasing, i.e /|
        /// </summary>
        /// <param name="t"></param>
        /// <returns></returns>
        public static float SawUp(float t)
        {
            float min = -1;
            float max = 1;
            float alpha = (MathHelper.Pi + MathHelper.WrapAngle(t)) / MathHelper.TwoPi;
            float result = MathHelper.Lerp(min, max, alpha);
            return result;
        }

        /// <summary>
        /// sawtooth from 1 to -1 decreasing, i.e |\
        /// </summary>
        /// <param name="t"></param>
        /// <returns></returns>
        public static float SawDown(float t)
        {
            float max = 1;
            float min = -1;
            float alpha = (MathHelper.Pi + MathHelper.WrapAngle(t)) / MathHelper.TwoPi;
            return MathHelper.Lerp(max, min, alpha);
        }

        /// <summary>
        /// Maps a value of -1..1 to 0..1
        /// </summary>
        /// <param name="val"></param>
        /// <returns></returns>
        public static float ToZeroOne(float val)
        {
            return MapTo(0, 1, val);
        }

        /// <summary>
        /// maps a value from a source interval to a target interval
        /// </summary>
        /// <returns></returns>
        public static float MapTo(float targetMin, float targetMax, float val, float srcMin = -1, float srcMax = 1)
        {
            var alpha = (val - srcMin) / (srcMax - srcMin);
            return MathHelper.Lerp(targetMin, targetMax, alpha);
        }
    }
}
