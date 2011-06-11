using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Jacobi.Vst.Core;
using Jacobi.Vst.Framework;
using ProceduralMidi;
using ProceduralMidi.DAL;
using System.Diagnostics;

namespace ProdeduralMidiVST
{
    /// <summary>
    /// Implements the incoming Midi event handling for the plugin.
    /// </summary>
    class MidiProcessor : IVstMidiProcessor
    {
        private Plugin _plugin;

        /// <summary>
        /// Constructs a new instance.
        /// </summary>
        /// <param name="plugin">Must not be null.</param>
        public MidiProcessor(Plugin plugin)
        {
            _plugin = plugin;
            Events = new VstEventCollection();
        }

        /// <summary>
        /// Gets the midi events that should be processed in the current cycle.
        /// </summary>
        public VstEventCollection Events { get; private set; }

        #region IVstMidiProcessor Members

        public int ChannelCount
        {
            get { return _plugin.ChannelCount; }
        }


        private DateTime lastTick;


        /// <summary>
        /// A list of all notes that are currently pressed
        /// </summary>
        private List<Note> notesDown = new List<Note>();

        public void Process(VstEventCollection events)
        {
            RemoveNotesThatAreExpired();

            if ((DateTime.Now - lastTick).Milliseconds > _plugin.BoardSettings.Speed)
            {
                _plugin.BoardSettings.Board.NextState();
                AddNotesToPlay(_plugin.BoardSettings);

                var _uiEditor = _plugin.GetInstance<PluginEditor>();
                _uiEditor.UpdateBoard();

                lastTick = DateTime.Now;
            }

            //foreach (VstEvent evnt in events)
            //{
            //    if (evnt.EventType != VstEventTypes.MidiEvent) continue;

            //    VstMidiEvent midiEvent = (VstMidiEvent)evnt;

            //    if (((midiEvent.Data[0] & 0xF0) == 0x80 || (midiEvent.Data[0] & 0xF0) == 0x90))
            //    {
            //        // add original event
            //        Events.Add(evnt);
            //    }
            //}
        }

        #endregion










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


        private short GetNoteIndexFromCellIndex(int cellIdx)
        {
            string[] notesPerCell = _plugin.BoardSettings.Notes.Split(',');

            // determine note of cell index, wrap the notes per cell if there are not enough notes specified
            string note = notesPerCell[cellIdx % notesPerCell.Length].Replace("BB", "A#");
            note = note.Trim();
            return (short)(notesByMidiIndex.IndexOf(note));
        }

        public void PlayNote(BoardSettings boardSettings, int cellIdx, short volume)
        {
            short midiIndex = GetNoteIndexFromCellIndex(cellIdx);

            byte[] midiData = new byte[4];
            midiData[0] = 0x90;
            midiData[1] = (byte)midiIndex;
            midiData[2] = 100;
            midiData[3] = 0;

            VstMidiEvent me = new VstMidiEvent(0, boardSettings.NoteDuration, 4, midiData, 0, (byte)volume);
            Events.Add(me);

            Note n = new Note(DateTime.Now, midiIndex, boardSettings.NoteDuration);
            notesDown.Add(n);
        }

        private void StopPlayingNote(short midiIndex, short volume)
        {
            byte[] midiData = new byte[4];
            midiData[0] = 0x80;
            midiData[1] = (byte)midiIndex;
            midiData[2] = 100;
            midiData[3] = 0;

            VstMidiEvent me = new VstMidiEvent(0, 0, 4, midiData, 0, (byte)volume);
            Events.Add(me);
        }

        private void RemoveNotesThatAreExpired()
        {
            foreach (var note in notesDown.ToList())
            {
                if ((DateTime.Now - note.TimeDown).TotalMilliseconds > note.DurationMS)
                {
                    // note duration exceeded, up the note
                    StopPlayingNote(note.MidiIndex, 127);

                    // and remove it from the list to check
                    notesDown.Remove(note);
                }
            }
        }


        private void AddNotesToPlay(BoardSettings boardSettings)
        {
            try
            {
                OtomataBoard.ActiveState[,] activeCells = boardSettings.Board.ActiveCells;

                for (int row = 0; row < boardSettings.Board.Rows; row++)
                {
                    for (int col = 0; col < boardSettings.Board.Cols; col++)
                    {
                        if (activeCells[col, row] == AbstractBoard.ActiveState.ColumnActivated)
                        {
                            // play corresponding note for the col
                            PlayNote(boardSettings, col, 127);
                        }
                        else if (activeCells[col, row] == AbstractBoard.ActiveState.RowActivated)
                        {
                            // play corresponding note for the row
                            PlayNote(boardSettings, row, 127);
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Debug.WriteLine("Error: " + ex.GetType().FullName + " - " + ex.Message);
            }
        }
    }

}
