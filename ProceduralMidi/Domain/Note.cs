using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ProceduralMidi
{
    /// <summary>
    /// Describes a note that is played
    /// </summary>
    public class Note
    {
        internal Note(DateTime timeDown, byte midiIdx, int durationMs, byte volume)
        {
            this.TimeDown = timeDown;
            this.MidiIndex = midiIdx;
            this.DurationMS = durationMs;
            this.Volume = volume;
        }

        /// <summary>
        /// The time when the note was pressed
        /// </summary>
        public DateTime TimeDown { get; set; }

        /// <summary>
        /// The midi index of the note
        /// </summary>
        public byte MidiIndex { get; set; }

        /// <summary>
        /// The duration the note has to be pressed
        /// </summary>
        public int DurationMS { get; set; }

        /// <summary>
        /// The volume at which to play the note at 
        /// </summary>
        public byte Volume { get; set; }
    }
}
