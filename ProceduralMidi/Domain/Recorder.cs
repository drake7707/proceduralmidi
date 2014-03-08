using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ProceduralMidi
{
    /// <summary>
    /// Contains recording information for played midi notes
    /// </summary>
    public class Recorder
    {
        public Recorder()
        {
            Notes = new List<Note>();
        }
        /// <summary>
        /// The notes that have been recorded
        /// </summary>
        public List<Note> Notes { get; set; }

        /// <summary>
        /// The start time of the recording
        /// </summary>
        public DateTime Start { get; set; }
    }
}
