﻿using PaToRo_Desktop.Engine;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PaToRo_Desktop.Scenes
{
    public class SineGenerator : Generator
    {
        private readonly BaseGame game;

        public SineGenerator(BaseGame game)
        {
            this.game = game;
        }

        public float CalcMid(float t)
        {
            return (game.Screen.Height * 0.5f) -(float)(Math.Sin(t*2) * 50);
        }

        public float GetUpper(float t)
        {
            return CalcMid(t) - 50;
        }
        public float GetLower(float t)
        {
            return CalcMid(t) + 50;
        }
    }
}
