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
        /// The sample manager that will be used to play the note, if UseSamples = true
        /// </summary>
        private SampleManager sampleManager;

        /// <summary>
        /// The midi manager that will be used to play the note, if UseSamples = false
        /// </summary>
        private MidiManager midiManager;

        /// <summary>
        /// Creates a new Note controller with midi playback support (but without sample playback support)
        /// </summary>
        public NoteController()
        {
            OutputSound = true;

            midiManager = new MidiManager();

            tmrDoNotesUp = new System.Windows.Forms.Timer();
            tmrDoNotesUp.Interval = 25;
            tmrDoNotesUp.Tick += new EventHandler(tmr_Tick);
            tmrDoNotesUp.Start();

            NotesPerCell = new string[] { "D3", "A3", "A#3", "C4", "D4", "E4", "F4", "A5", "C5" };
        }

        /// <summary>
        /// Creates a new Note controller with midi and sample playback support
        /// </summary>
        public NoteController(System.Windows.Forms.Control owner, bool enableSamples)
            : this()
        {
            if (enableSamples)
                sampleManager = new SampleManager(owner);
        }

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
        private static List<string> notesByMidiIndex = new List<string>
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
        /// Returns the MIDI frequency of the given midi index
        /// </summary>
        /// <param name="midiIdx"></param>
        /// <returns></returns>
        private static int GetFrequencyByMidiIndex(int midiIdx)
        {
            if (midiIdx == -1)
                return -1;
            int a = 440; // a is 440 hz...
            return (int)((a / 32f) * (Math.Pow(2, ((midiIdx + 12 - 9) / 12f))));
        }

        /// <summary>
        /// A list of which notes to play for each cell index
        /// </summary>
        public string[] NotesPerCell { get; set; }

        /// <summary>
        /// A recorder to record the notes to. If one is present (!= null) all played notes will be saved to the recorder.
        /// </summary>
        public Recorder Recorder { get; set; }


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

                    if (OutputSound)
                    {
                        // note duration exceeded, up the note
                        midiManager.NoteUp(note.MidiIndex, MIDI_CHANNEL, 127);
                    }
                    // and remove it from the list to check
                    notesDown.Remove(note);
                }
            }
        }

        /// <summary>
        /// Creates a note
        /// </summary>
        /// <param name="cellIdx"></param>
        /// <param name="durationMS"></param>
        /// <param name="volume"></param>
        /// <returns></returns>
        public Note CreateNote(int cellIdx, int durationMS, byte volume)
        {
            byte midiIndex = GetNoteIndexFromCellIndex(cellIdx);
            Note n = new Note(DateTime.Now, midiIndex, durationMS, volume);
            return n;
        }

   
        /// <summary>
        /// Creates a new note
        /// </summary>
        /// <param name="cellIdx"></param>
        /// <param name="durationMS"></param>
        /// <param name="volume"></param>
        /// <param name="notesPerCell"></param>
        /// <returns></returns>
        public static Note CreateNote(int cellIdx, int durationMS, byte volume, string[] notesPerCell)
        {
            byte midiIndex = GetNoteIndexFromCellIndex(cellIdx, notesPerCell);
            Note n = new Note(DateTime.Now, midiIndex, durationMS, volume);
            return n;
        }

        /// <summary>
        /// Play the note for a specified cell index, with specified duration and volume
        /// </summary>
        /// <param name="n"></param>
        public void PlayNote(Note n)
        {
            if (OutputSound)
            {
                if (UseSamples && sampleManager != null)
                    sampleManager.PlaySample(GetFrequencyByMidiIndex(n.MidiIndex), n.Volume, n.DurationMS);
                else
                    midiManager.NoteDown(n.MidiIndex, MIDI_CHANNEL, n.Volume);

            }
            // add note to down list
            notesDown.Add(n);

            if (Recorder != null)
                Recorder.Notes.Add(n);
        }

        /// <summary>
        /// Returns the midi index of the note for a specified cell index
        /// </summary>
        /// <param name="cellIdx"></param>
        /// <returns></returns>
        private byte GetNoteIndexFromCellIndex(int cellIdx)
        {
            // determine note of cell index, wrap the notes per cell if there are not enough notes specified
            string note = NotesPerCell[cellIdx % NotesPerCell.Length].Replace("BB", "A#");
            return (byte)(notesByMidiIndex.IndexOf(note));
        }

        /// <summary>
        /// Returns the midi index of the note for a specified cell index
        /// </summary>
        /// <param name="cellIdx"></param>
        /// <returns></returns>
        public static byte GetNoteIndexFromCellIndex(int cellIdx, string[] notesPerCell)
        {
            // determine note of cell index, wrap the notes per cell if there are not enough notes specified
            string note = notesPerCell[cellIdx % notesPerCell.Length].Replace("BB", "A#");
            return (byte)(notesByMidiIndex.IndexOf(note));
        }

        /// <summary>
        /// Returns the midi index from the note
        /// </summary>
        /// <param name="note"></param>
        /// <returns></returns>
        public static int GetMidiIndexFromNote(string note)
        {
            return notesByMidiIndex.IndexOf(note);
        }

        /// <summary>
        /// Returns the note representation of the given midi index
        /// </summary>
        /// <param name="midiIndex"></param>
        /// <returns></returns>
        public static string GetNoteFromMidiIndex(int midiIndex)
        {
            if (midiIndex >= 0 && midiIndex < notesByMidiIndex.Count)
                return notesByMidiIndex[midiIndex];
            else
                return "";
        }

        /// <summary>
        /// Determines if the midi notes are sent to the midi controller
        /// </summary>
        public bool OutputSound { get; set; }

        /// <summary>
        /// Use the sample manager to play the note, if false, use the Midi Controller
        /// </summary>
        public bool UseSamples { get; set; }

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

        /// <summary>
        /// Changes the sample
        /// </summary>
        /// <param name="sampleName"></param>
        internal void ChangeSample(string sampleName)
        {
            if (sampleManager != null)
                sampleManager.ChangeSample(sampleName);
        }

        /// <summary>
        /// Changes the midi device to output midi events to
        /// </summary>
        /// <param name="midiDeviceIndex"></param>
        internal void ChangeMidiDevice(int midiDeviceIndex)
        {
            midiManager.ChangeMidiDevice(midiDeviceIndex);
        }

        /// <summary>
        /// Changes the midi instrument
        /// </summary>
        /// <param name="instrumentIndex"></param>
        internal void ChangeMidiInstrument(int instrumentIndex)
        {
            midiManager.ChangeInstrument(instrumentIndex, MIDI_CHANNEL);
        }
    }
}
