using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.DirectX.DirectSound;

namespace ProceduralMidi
{
    public class SampleManager : IDisposable
    {

        /// <summary>
        /// Timer to stop & dispose the played sounds after their duration limit has been exceeded
        /// </summary>
        private System.Windows.Forms.Timer timer;

        /// <summary>
        /// List of all sounds playing
        /// </summary>
        private List<Sound> sounds = new List<Sound>();


        /// <summary>
        /// Create a new sample manager
        /// </summary>
        /// <param name="owner"></param>
        public SampleManager(System.Windows.Forms.Control owner)
        {
            // Create Device
            device = new Device();
            device.SetCooperativeLevel(owner, CooperativeLevel.Priority);

            // create & start timer to stop playing sounds
            timer = new System.Windows.Forms.Timer();
            timer.Interval = 25;
            timer.Tick += new EventHandler(timer_Tick);
            timer.Start();
        }

        /// <summary>
        /// Stop playing sounds
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        void timer_Tick(object sender, EventArgs e)
        {
            foreach (var sound in sounds.ToList())
            {
                // if duration has passed, stop & dispose sound
                if ((DateTime.Now - sound.TimeStartedPlaying).TotalMilliseconds > sound.Duration)
                {
                    sound.Stop();
                    sound.Dispose();
                    sounds.Remove(sound);
                }
            }
        }

        /// <summary>
        /// Returns a list of all available samples in the samples subdirectory
        /// </summary>
        public static IEnumerable<string> Samples
        {
            get
            {
                string samplePath = GetSamplePath();

                // create sample directory if it doesn't exist
                if (!System.IO.Directory.Exists(samplePath))
                    System.IO.Directory.CreateDirectory(samplePath);

                // get files ending on .wav
                return System.IO.Directory.GetFiles(samplePath)
                                          .Where(sample => sample.ToLower().EndsWith(".wav"))
                                          .Select(sample => System.IO.Path.GetFileName(sample));
            }
        }

        /// <summary>
        /// Returns the samples directory path
        /// </summary>
        /// <returns></returns>
        private static string GetSamplePath()
        {
            string samplePath = System.IO.Path.GetDirectoryName(System.Reflection.Assembly.GetExecutingAssembly().Location);
            samplePath = System.IO.Path.Combine(samplePath, "samples");
            return samplePath;
        }


        /// <summary>
        /// The current sample for the sounds
        /// </summary>
        private string currentSample;

        /// <summary>
        /// The device that will play the sounds
        /// </summary>
        private Device device;

        /// <summary>
        /// Change the sample to a different wav with specified filename
        /// </summary>
        /// <param name="sampleName"></param>
        public void ChangeSample(string sampleName)
        {
            string fullSamplePath = System.IO.Path.Combine(GetSamplePath(), sampleName);
            if (System.IO.File.Exists(fullSamplePath))
            {
                currentSample = fullSamplePath;
            }
        }

        /// <summary>
        /// Plays a sample with given frequency, volume and duration
        /// </summary>
        /// <param name="freq">MIDI frequency</param>
        /// <param name="volume">MIDI Volume (0-127)</param>
        /// <param name="duration">Duration of sound in milliseconds</param>
        public void PlaySample(int freq, int volume, int duration)
        {
            // invalid frequency, don't play sound
            if (freq == -1)
                return;

            // Create a new sound, add it to the playing list and play the sound
            Sound s = new Sound(currentSample, device, freq, volume, duration);
            sounds.Add(s);
            s.Play();
        }

        /// <summary>
        /// Disposes the resources
        /// </summary>
        public void Dispose()
        {
            device.Dispose();
            timer.Stop();
            timer.Dispose();
        }

        /// <summary>
        /// A representation of a sound that will be played
        /// </summary>
        private class Sound : IDisposable
        {
            /// <summary>
            /// MIDI frequency of the sound
            /// </summary>
            public int Frequency { get; private set; }

            /// <summary>
            /// MIDI volume of the sound
            /// </summary>
            public int Volume { get; private set; }

            /// <summary>
            /// Duration of the sound in milliseconds
            /// </summary>
            public int Duration { get; private set; }

            /// <summary>
            /// The time when the sound started playing
            /// </summary>
            public DateTime TimeStartedPlaying { get; private set; }

            /// <summary>
            /// The directsound buffer for the sound
            /// </summary>
            private SecondaryBuffer buffer;

            /// <summary>
            /// Creates a new sound
            /// </summary>
            /// <param name="sample"></param>
            /// <param name="device"></param>
            /// <param name="freq"></param>
            /// <param name="volume"></param>
            /// <param name="duration"></param>
            public Sound(string sample, Device device, int freq, int volume, int duration)
            {
                Frequency = freq;
                Volume = volume;
                Duration = duration;

                // calculates the directsound volume from the midi volume
                // volume 0- 127 ==> -3000 - 0
                int dxVolume = (volume == 0 ? -10000 : (int)(((volume / 127f) * 3000) - 3000));

                BufferDescription d = new BufferDescription();
                d.Flags = BufferDescriptionFlags.ControlVolume | BufferDescriptionFlags.ControlFrequency | BufferDescriptionFlags.ControlPan | BufferDescriptionFlags.ControlEffects;
                // play even when the window is inactive
                d.GlobalFocus = true;

                // create a new buffer
                buffer = new SecondaryBuffer(sample, d, device);

                // calculate the sample rate from the MIDI frequency
                int dxFreq = (int)((freq / 440f) * buffer.Format.SamplesPerSecond);

                buffer.Volume = dxVolume;
                buffer.Frequency = dxFreq;
            }

            /// <summary>
            /// Play the sound
            /// </summary>
            public void Play()
            {
                TimeStartedPlaying = DateTime.Now;
                buffer.Play(0, BufferPlayFlags.Default);
            }

            /// <summary>
            /// Stop the sound
            /// </summary>
            public void Stop()
            {
                buffer.Stop();
            }

            /// <summary>
            /// Clean up the resource
            /// </summary>
            public void Dispose()
            {
                buffer.Dispose();
            }
        }
    }
}
