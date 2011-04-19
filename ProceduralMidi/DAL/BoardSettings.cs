using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ProceduralMidi.DAL
{
    /// <summary>
    /// Describes a state of the board and the settings applied
    /// </summary>
    [Serializable()]
    public class BoardSettings
    {
        public OtomataBoard Board { get; set; }
        public string  Notes { get; set; }
        public int NoteDuration { get; set; }
        public int Speed { get; set; }
        public int Instrument { get; set; }
    }
}
