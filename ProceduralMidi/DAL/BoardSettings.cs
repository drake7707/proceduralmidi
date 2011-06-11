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
        public string Notes { get; set; }
        public int NoteDuration { get; set; }
        public int Speed { get; set; }
        public int Instrument { get; set; }

        public bool UseSamples { get; set; }
        public string Sample { get; set; }


        public static BoardSettings GetDefaultBoard()
        {
            return new BoardSettings()
            {
                Board = new ProceduralMidi.OtomataBoard(9, 9),
                Instrument = 0,
                NoteDuration = 250,
                Notes = "D3, A3, A#3, C4, D4, E4, F4, A5, C5",
                Sample = "",
                Speed = 250,
                UseSamples = false
            };
        }

    }
}
