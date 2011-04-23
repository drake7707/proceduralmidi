using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ProceduralMidi
{
    public class Recorder
    {
        public Recorder()
        {
            Notes = new List<Note>();
        }

        public List<Note> Notes { get; set; }

        public DateTime Start { get; set; }
    }
}
