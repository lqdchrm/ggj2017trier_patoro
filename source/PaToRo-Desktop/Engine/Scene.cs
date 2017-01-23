﻿using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PaToRo_Desktop.Engine
{
    public class Scene : Group
    {
        protected readonly BaseGame game;

        protected bool initialized;
        private bool loaded;

        protected Color BgColor { get; set; }
        protected SpriteBatch spriteBatch;
        protected Matrix? screenMatrix;

        public Scene(BaseGame game)
        {
            this.game = game;
        }

        internal virtual void Initialize()
        {
            BgColor = Color.Black;
            initialized = true;
        }

        internal void LoadContent()
        {
            if (!loaded)
                InternalLoadContent();
            loaded = true;
        }

        internal virtual void InternalLoadContent()
        {
            spriteBatch = new SpriteBatch(game.GraphicsDevice);
        }


        internal virtual void UnloadContent()
        {

        }

        internal virtual int HandleInput(GameTime gameTime)
        {
            return game.HandleInput(gameTime);
        }

        internal override void Update(GameTime gameTime)
        {
            HandleInput(gameTime);

            base.Update(gameTime);
        }

        protected override void DrawInternal(SpriteBatch batch, GameTime gameTime)
        {
            base.DrawInternal(batch, gameTime);
        }

        internal virtual void Draw(GameTime gameTime)
        {
            game.GraphicsDevice.Clear(BgColor);

            spriteBatch.Begin(SpriteSortMode.Immediate, BlendState.Additive, null, null, null, null, screenMatrix);
            base.Draw(spriteBatch, gameTime);
            spriteBatch.End();
        }
    }
}
