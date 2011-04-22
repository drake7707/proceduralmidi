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
        public Note(DateTime timeDown, short midiIdx, int durationMs)
        {
            this.TimeDown = timeDown;
            this.MidiIndex = midiIdx;
            this.DurationMS = durationMs;
        }

        /// <summary>
        /// The time when the note was pressed
        /// </summary>
        public DateTime TimeDown { get; set; }

        /// <summary>
        /// The midi index of the note
        /// </summary>
        public short MidiIndex { get; set; }

        /// <summary>
        /// The duration the note has to be pressed
        /// </summary>
        public int DurationMS { get; set; }
    }
}
