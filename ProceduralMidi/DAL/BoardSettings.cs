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
        /// <summary>
        /// The board where the settings are for
        /// </summary>
        public AbstractBoard Board { get; set; }

        /// <summary>
        /// The different notes for each cell column/row
        /// </summary>
        private string notes;
        /// <summary>
        /// The different notes for each cell column/row
        /// </summary>
        public string Notes
        {
            get { return notes; }
            set
            {
                notes = value;
                if (notes != null)
                    NotesPerCell = notes.ToUpper().Split(',').Select(n => n.Trim()).ToArray();
            }
        }

        /// <summary>
        /// The different notes for each cell column/row, split in array and cleaned
        /// </summary>
        public string[] NotesPerCell { get; set; }

        /// <summary>
        /// The note duration for each note played
        /// </summary>
        public int NoteDuration { get; set; }

        /// <summary>
        /// The speed (in ms) to advance to the next state
        /// </summary>
        public int Speed { get; set; }

        /// <summary>
        /// The midi instrument used to play notes
        /// </summary>
        public int Instrument { get; set; }

        /// <summary>
        /// Output to samples or midi
        /// </summary>
        public bool UseSamples { get; set; }

        /// <summary>
        /// The sample used to play notes
        /// </summary>
        public string Sample { get; set; }

        /// <summary>
        /// The volume to play the note at
        /// </summary>
        public byte Volume { get; set; }

        /// <summary>
        /// Returns the default board
        /// </summary>
        /// <returns></returns>
        public static BoardSettings GetDefaultBoard()
        {
            return new BoardSettings()
            {
                Board = new ProceduralMidi.OtomataBoard(9, 9),
                Instrument = 0,
                NoteDuration = 500,
                Notes = "D3, A3, A#3, C4, D4, E4, F4, A5, C5",
                Sample = "",
                Speed = 250,
                UseSamples = false,
                Volume = 64
            };
        }

    }
}
