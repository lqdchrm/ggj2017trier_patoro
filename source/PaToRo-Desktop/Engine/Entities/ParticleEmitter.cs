using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Content;
using PaToRo_Desktop.Scenes.Funcs;

namespace PaToRo_Desktop.Engine.Entities
{

    public class ParticleEmitter : Entity
    {
        private readonly BaseGame game;

        private int maxParticles;
        private int active;

        private float accumulator;

        private Vector2[] pos;
        private Vector2[] spd;
        private Vector2[] scl;
        private Vector2[] maxScl;
        private Color[] col;
        private Color[] maxCol;

        private float[] age;

        private string asset;
        private Texture2D tex;
        private Vector2 origin;

        public float maxAgeInSecs;

        public Vector2 Pos;
        public Vector2 Spd;
        public float Dmp;

        public float SpawnRate;
        public Vector2 SpawnPos { get; set; }
        public Color SpawnColor { get; set; }

        private bool _emitting;
        public bool Emitting { get { return _emitting; } set { _emitting = value; if (!value) active = 0; } }

        public ParticleEmitter(BaseGame game, string asset, int maxParticles, float spawnRateInPartPerSec)
        {
            this.game = game;
            this.asset = asset;
            this.maxParticles = maxParticles;
            this.SpawnRate = spawnRateInPartPerSec;
            this.pos = new Vector2[maxParticles];
            this.spd = new Vector2[maxParticles];

            this.scl = new Vector2[maxParticles];
            this.maxScl = new Vector2[maxParticles];

            this.col = new Color[maxParticles];
            this.maxCol = new Color[maxParticles];

            this.age = new float[maxParticles];

            Dmp = 0.92f;
        }

        public void LoadContent(ContentManager content)
        {
            tex = content.Load<Texture2D>(asset);
            origin = new Vector2(tex.Width * 0.5f, tex.Height * 0.5f);
            SpawnPos = new Vector2(game.Screen.Width * 0.5f, game.Screen.Height * 0.5f);
            maxAgeInSecs = 1;
        }

        public void Spawn()
        {
            if (Emitting && active < maxParticles)
            {
                pos[active] = SpawnPos;
                spd[active] = new Vector2(RandomFuncs.FromRange(-400, 400), RandomFuncs.FromRange(-400, 400));
                scl[active] = Vector2.One * 0.8f;
                maxScl[active] = Vector2.One * 0.1f;
                col[active] = SpawnColor;
                maxCol[active] = col[active];
                maxCol[active].A = 0;
                age[active] = 0;
                active++;
            }
        }

        protected override void DrawInternal(SpriteBatch spriteBatch, GameTime gameTime)
        {
            for (int i = 0; i < active; ++i)
            {
                var _scl = Vector2.Lerp(scl[i], maxScl[i], age[i] / maxAgeInSecs);
                var _col = Color.Lerp(col[i], maxCol[i], age[i] / maxAgeInSecs);
                spriteBatch.Draw(tex, Pos + pos[i], null, null, origin, 0, _scl, _col);
            }
        }

        internal override void Update(GameTime gameTime)
        {
            var delta = (float)gameTime.ElapsedGameTime.TotalSeconds;

            Pos += Spd * delta;

            accumulator += delta;
            var numParticlesToSpawn = accumulator * SpawnRate;

            while (numParticlesToSpawn > 1)
            {
                Spawn();
                accumulator -= 1 / SpawnRate;
                --numParticlesToSpawn;
            }

            // Update
            for (int i = 0; i < active; ++i)
            {
                pos[i] += spd[i] * delta * Dmp;
                age[i] += delta;
            }

            // kill old particles by copying in from end
            for (int i = 0; i < active; ++i)
            {
                if (age[i] > maxAgeInSecs)
                {
                    pos[i] = pos[active - 1];
                    spd[i] = spd[active - 1];
                    scl[i] = scl[active - 1];
                    maxScl[i] = maxScl[active - 1];
                    col[i] = col[active - 1];
                    maxCol[i] = maxCol[active - 1];
                    age[i] = age[active - 1];
                    --i;
                    --active;
                }
            }
        }
    }
}
