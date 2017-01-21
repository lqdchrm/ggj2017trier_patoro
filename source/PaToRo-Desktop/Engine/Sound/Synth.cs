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
        private readonly string[] voices = new string[] { "Arps", "Bells", "Buzz", "Drums", "Hat", "Hihats", "Kick", "Snare", "Strings", "Vibrant" };

        private string playing;
        private float elapsedMillis;

        public Synth()
        {
            tracks = new Dictionary<string, Dictionary<string, SoundEffectInstance>>();
            foreach(var song in songs)
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
                    try
                    {
                        var sfx = content.Load<SoundEffect>(string.Format("Sounds/{0}/ggg_{1}", song, voice));
                        var sfxi = sfx.CreateInstance();
                        tracks[song].Add(voice, sfxi);
                    }
                    catch (Exception)
                    {

                    }
                }
            }
        }

        public void Update(GameTime gameTime)
        {
            elapsedMillis += (float)gameTime.ElapsedGameTime.TotalMilliseconds;
            if (elapsedMillis >= 7232f)
            {
                Play(playing);
            }
            
        }

        public void Play(string song)
        {
            elapsedMillis = 0;
            if (!string.IsNullOrEmpty(playing) && tracks.ContainsKey(playing))
            {
                var oldSong = tracks[playing];
                foreach (var track in oldSong.Values)
                {
                    track.Stop();
                }
            }

            if (!string.IsNullOrEmpty(song) && tracks.ContainsKey(song))
            {
                var s = tracks[song];
                foreach (var track in s.Values)
                {
                    track.IsLooped = false;
                    track.Volume = 0.5f;
                    track.Play();
                }
            }
            playing = song;
        }

        public void SetVolumeForTrack(string name)
        {

        }
    }
}
