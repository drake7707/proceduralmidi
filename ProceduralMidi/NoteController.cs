using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ProceduralMidi
{
    /// <summary>
    /// Translates the activated cell indices to notes, and send midi messages
    /// for those notes
    /// </summary>
    public class NoteController : IDisposable
    {
        /// <summary>
        /// The MIDI channel to output on
        /// </summary>
        public const int MIDI_CHANNEL = 15;

        /// <summary>
        /// The timer that stops the notes from being in the down state
        /// </summary>
        private System.Windows.Forms.Timer tmrDoNotesUp;

        /// <summary>
        /// A list of all notes that are currently pressed
        /// </summary>
        private List<Note> notesDown = new List<Note>();

        /// <summary>
        /// All possible notes by MIDI index
        /// </summary>
        private List<string> notesByMidiIndex = new List<string>
        { 
            "A-1", "A#-1", "B-1", "C-1", "C#-1", "D-1", "D#-1", "E-1", "F-1", "F#-1", "G-1", "G#-1",
            "A0", "A#0", "B0", "C0", "C#0", "D0", "D#0", "E0", "F0", "F#0", "G0", "G#0",
            "A1", "A#1", "B1", "C1", "C#1", "D1", "D#1", "E1", "F1", "F#1", "G1", "G#1",
            "A2", "A#2", "B2", "C2", "C#2", "D2", "D#2", "E2", "F2", "F#2", "G2", "G#2",
            "A3", "A#3", "B3", "C3", "C#3", "D3", "D#3", "E3", "F3", "F#3", "G3", "G#3",
            "A4", "A#4", "B4", "C4", "C#4", "D4", "D#4", "E4", "F4", "F#4", "G4", "G#4",
            "A5", "A#5", "B5", "C5", "C#5", "D5", "D#5", "E5", "F5", "F#5", "G5", "G#5",
            "A6", "A#6", "B6", "C6", "C#6", "D6", "D#6", "E6", "F6", "F#6", "G6", "G#6",
            "A7", "A#7", "B7", "C7", "C#7", "D7", "D#7", "E7", "F7", "F#7", "G7", "G#7",
            "A8", "A#8", "B8", "C8", "C#8", "D8", "D#8", "E8", "F8", "F#8", "G8", "G#8",
            "A9", "A#9", "B9", "C9", "C#9", "D9", "D#9", "E9", "F9", "F#9", "G9", "G#9",

        };

        /// <summary>
        /// A list of which notes to play for each cell index
        /// </summary>
        public string[] NotesPerCell { get; set; }

        /// <summary>
        /// Creates a new Note controller
        /// </summary>
        public NoteController()
        {
            tmrDoNotesUp = new System.Windows.Forms.Timer();
            tmrDoNotesUp.Interval = 25;
            tmrDoNotesUp.Tick += new EventHandler(tmr_Tick);
            tmrDoNotesUp.Start();

            NotesPerCell = new string[] { "D3", "A3", "A#3", "C4", "D4", "E4", "F4", "A5", "C5" };
        }

        /// <summary>
        /// Checks all currently pressed notes and if their duration is exceeded
        /// unpress the note
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        void tmr_Tick(object sender, EventArgs e)
        {
            foreach (var note in notesDown.ToList())
            {
                if ((DateTime.Now - note.TimeDown).TotalMilliseconds > note.DurationMS)
                {
                    // note duration exceeded, up the note
                    Midi.NoteUp(note.MidiIndex, MIDI_CHANNEL, 127);
                    // and remove it from the list to check
                    notesDown.Remove(note);
                }
            }
        }

        /// <summary>
        /// Play the note for a specified cell index, with specified duration and volume
        /// </summary>
        /// <param name="cellIdx"></param>
        /// <param name="durationMS"></param>
        /// <param name="volume"></param>
        public void PlayNote(int cellIdx, int durationMS, short volume)
        {
            short midiIndex =GetNoteIndexFromCellIndex(cellIdx);
            Midi.NoteDown(midiIndex, MIDI_CHANNEL, volume);
            notesDown.Add(new Note(DateTime.Now, midiIndex, durationMS));
        }

        /// <summary>
        /// Returns the midi index of the note for a specified cell index
        /// </summary>
        /// <param name="cellIdx"></param>
        /// <returns></returns>
        private short GetNoteIndexFromCellIndex(int cellIdx)
        {
            // determine note of cell index, wrap the notes per cell if there are not enough notes specified
            string note = NotesPerCell[cellIdx % NotesPerCell.Length].Replace("BB", "A#");
            return (short)(notesByMidiIndex.IndexOf(note));
        }

        /// <summary>
        /// Describes a note that is played
        /// </summary>
        private class Note
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

        /// <summary>
        /// Clean up the running timers
        /// </summary>
        public void Dispose()
        {
            if (tmrDoNotesUp != null)
            {
                tmrDoNotesUp.Stop();
                tmrDoNotesUp.Dispose();
            }
        }
    }
}
