using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PaToRo_Desktop.Engine
{
    public class Scenes
    {
        private readonly BaseGame game;
        private readonly Dictionary<string, Scene> scenes;

        public Scene Current { get; private set; }

        public Scenes(BaseGame game)
        {
            this.game = game;
            this.scenes = new Dictionary<string, Scene>();
            Current = null;
        }

        public void Add(string name, Scene scene)
        {
            if (scene == null)
                throw new ArgumentNullException("scene");

            scenes.Add(name, scene);
        }

        public void Clear()
        {
            foreach (var scene in scenes.Values)
                scene.UnloadContent();
            scenes.Clear();
        }

        public void Show(Scene scene)
        {
            if (scene != Current)
            {
                // Load new
                scene.Initialize();
                scene.LoadContent();

                // Unload old
                if (Current != null)
                    Current.UnloadContent();

                Current = scene;
            }
        }

        public void Show(string name)
        {
            Scene scene;
            if (scenes.TryGetValue(name, out scene))
            {
                Show(scene);
            }
        }
    }
}
