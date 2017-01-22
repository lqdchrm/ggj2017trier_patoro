using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Xna.Framework.Graphics;
using PaToRo_Desktop.Engine.Components;

namespace PaToRo_Desktop.Engine.Entities
{
    public class ParticleSystem : Entity
    {
        private readonly BaseGame game;
        private readonly ParticleEmitter[] emitters;
        private readonly float[] age;
        private readonly float[] maxAge;
        private readonly Action<ParticleEmitter>[] actions;

        private int active;
        private int maxEmitters;

        public ParticleSystem(BaseGame game, int maxEmitters)
        {
            this.game = game;
            this.maxEmitters = maxEmitters;
            this.emitters = new ParticleEmitter[maxEmitters];
            this.age = new float[maxEmitters];
            this.maxAge = new float[maxEmitters];
            this.actions = new Action<ParticleEmitter>[maxEmitters];

            for (int i=0;i< this.emitters.Length; ++i)
            {
                this.emitters[i] = new ParticleEmitter(game, "Images/particle", 1000, 100);
            }
        }

        public void LoadContent(ContentManager content)
        {
            foreach(var e in emitters)
            {
                e.LoadContent(content);
            }
        }

        internal override void Update(GameTime gameTime)
        {
            var delta = (float)gameTime.ElapsedGameTime.TotalSeconds;
            for(int i=0; i<active; ++i)
            {
                actions[i]?.Invoke(emitters[i]);
                emitters[i].Update(gameTime);
                age[i] += delta;
            }

            for (int i = 0; i < active; ++i)
            {
                if (age[i] > maxAge[i])
                {
                    emitters[i].Emitting = false;
                    actions[i] = actions[active - 1];
                    emitters[i] = emitters[active - 1];
                    age[i] = age[active-1];
                    maxAge[i] = age[active - 1];
                    --active;
                }
            }
        }

        internal override void Draw(SpriteBatch spriteBatch, GameTime gameTime)
        {
            for (int i = 0; i < active; ++i)
            {
                emitters[i].Draw(spriteBatch, gameTime);
            }
        }

        public void Explode(Vector2 pos, float time, Color color, Action<ParticleEmitter> onUpdate)
        {
            if (active < maxEmitters)
            {
                actions[active] = onUpdate;
                emitters[active].Pos = Vector2.Zero;
                emitters[active].Spd = Vector2.Zero;
                emitters[active].SpawnPos = pos;
                emitters[active].SpawnColor = color;
                emitters[active].SpawnRate = 0;
                emitters[active].maxAgeInSecs = time;
                emitters[active].Emitting = true;
                age[active] = 0;
                maxAge[active] = time;

                for (int i = 0; i < 70; ++i)
                    emitters[active].Spawn();

            //    Console.WriteLine(string.Format("Emitter {0} fired", active));

                ++active;
            }
        }
    }
}
