using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Content;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PaToRo_Desktop.Engine.Sound
{
    public class Synth
    {
        private readonly Dictionary<string, Dictionary<string, SoundEffectInstance>> tracks;

        private readonly string[] songs = new string[] { "ggg_1", "ggg_2", "ggg_3" };
        private readonly string[] voices = new string[] { "Arps", "Buzz", "Drums", "Vibrant", "Bells", "Strings" };

        private string playing;
        private float elapsedMillis;

        private float loops;
        private float bar;

        public Synth()
        {
            tracks = new Dictionary<string, Dictionary<string, SoundEffectInstance>>();
            foreach (var song in songs)
            {
                tracks.Add(song, new Dictionary<string, SoundEffectInstance>());
            }
        }

        public void LoadContent(ContentManager content)
        {
            foreach (var song in songs)
            {
                foreach (var voice in voices)
                {
                    var sfx = content.Load<SoundEffect>(string.Format("Sounds/{0}/ggg_{1}", song, voice));
                    var sfxi = sfx.CreateInstance();
                    tracks[song].Add(voice, sfxi);
                }
            }
        }

        public void Update(GameTime gameTime)
        {
            elapsedMillis += (float)gameTime.ElapsedGameTime.TotalMilliseconds;

            bar = elapsedMillis * 16 / 7232f;

            if (elapsedMillis >= 7232f)
            {
                bar = 0;
                Play(playing);
            }

        }

        public void Play(string song, bool reset = false)
        {
            elapsedMillis = 0;
            if (!string.IsNullOrEmpty(playing) && tracks.ContainsKey(playing))
            {
                if (playing != song || reset)
                {
                    loops = -1;
                    bar = 0;
                }

                var oldSong = tracks[playing];
                foreach (var track in oldSong.Values)
                {
                    try
                    {
                        track.Stop();
                    }
                    catch (Exception)
                    {

                    }
                    bar = 0;
                }
            }

            if (!string.IsNullOrEmpty(song) && tracks.ContainsKey(song))
            {
                loops += 1;
                //Console.WriteLine($"{loops}, {bar}");
                var s = tracks[song];
                foreach (var kvp in s)
                {
                    var track = kvp.Value;
                    track.IsLooped = false;
                    switch (kvp.Key)
                    {
                        case "Arps": track.Volume = 1; break;
                        case "Buzz": track.Volume = loops > 1 ? 1 : 0; break;
                        case "Drums": track.Volume = loops > 3 ? 1 : 0; break;
                        case "Vibrant": track.Volume = loops > 7 ? 1 : 0; break;
                        case "Bells": track.Volume = loops > 5 ? 1 : 0; break;
                        case "Strings": track.Volume = loops > 9 ? 1 : 0; break;
                        default: track.Volume = 0.0f; break;
                    }
                    try
                    {
                        track.Play();

                    }
                    catch (Exception)
                    {

                    }
                }
            }
            playing = song;
        }

        public void SetVolumeForTrack(string name)
        {

        }
    }
}
