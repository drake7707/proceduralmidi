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

        /// <summary>
        /// Returns the channel count as reported by the host
        /// </summary>
        public int ChannelCount
        {
            get { return _plugin.ChannelCount; }
        }

        /// <summary>
        /// Last tick timestamp
        /// </summary>
        private DateTime lastTick;


        /// <summary>
        /// A list of all notes that are currently pressed
        /// </summary>
        private List<Note> notesDown = new List<Note>();

        /// <summary>
        /// Removes notes that are expired and generates new notes.
        /// </summary>
        /// <param name="events"></param>
        public void Process(VstEventCollection events)
        {
            // remove notes that are expired
            RemoveNotesThatAreExpired();

            // check if time since last tick is larger than the speed of the board
            if ((DateTime.Now - lastTick).Milliseconds > _plugin.BoardSettings.Speed)
            {
                // advance the board to its next state
                _plugin.BoardSettings.Board.NextState();

                // add notes to play based on the active cells
                AddNotesToPlay(_plugin.BoardSettings);

                // let the editor know to update the board
                var _uiEditor = _plugin.GetInstance<PluginEditor>();
                _uiEditor.UpdateBoard();

                lastTick = DateTime.Now;
            }
        }

        #endregion

        /// <summary>
        /// Midi note down event
        /// </summary>
        private const byte MIDI_NOTE_DOWN = 0x90;

        /// <summary>
        /// Midi note up event
        /// </summary>
        private const byte MIDI_NOTE_UP = 0x80;

        /// <summary>
        /// Plays a note of the corresponding active cell
        /// </summary>
        /// <param name="boardSettings"></param>
        /// <param name="cellIdx"></param>
        public void PlayNote(BoardSettings boardSettings, int cellIdx)
        {
            // create a note
            Note n = NoteController.CreateNote(cellIdx, boardSettings.NoteDuration, boardSettings.Volume, boardSettings.NotesPerCell);

            // build midi data
            byte[] midiData = new byte[4];
            midiData[0] = MIDI_NOTE_DOWN;
            midiData[1] = (byte)n.MidiIndex;
            midiData[2] = boardSettings.Volume;
            midiData[3] = 0;

            // create event of midi data & add it to the events list that will be processed in the audio processor
            VstMidiEvent me = new VstMidiEvent(0, boardSettings.NoteDuration, n.MidiIndex, midiData, 0, boardSettings.Volume);
            Events.Add(me);

            // add the note to the downed notes list
            notesDown.Add(n);
        }

        /// <summary>
        /// Stop playing the note (note up)
        /// </summary>
        /// <param name="midiIndex"></param>
        /// <param name="volume"></param>
        private void StopPlayingNote(byte midiIndex, byte volume)
        {
            // build midi data
            byte[] midiData = new byte[4];
            midiData[0] = MIDI_NOTE_UP;
            midiData[1] = midiIndex;
            midiData[2] = volume;
            midiData[3] = 0;

            // add to events
            VstMidiEvent me = new VstMidiEvent(0, 0, midiIndex, midiData, 0, (byte)volume);
            Events.Add(me);
        }

        /// <summary>
        /// Check all notes that are down and check if their duration has been exceeded
        /// </summary>
        private void RemoveNotesThatAreExpired()
        {
            foreach (var note in notesDown.ToList())
            {
                if ((DateTime.Now - note.TimeDown).TotalMilliseconds > note.DurationMS)
                {
                    // note duration exceeded, up the note
                    StopPlayingNote(note.MidiIndex, note.Volume);

                    // and remove it from the list to check
                    notesDown.Remove(note);
                }
            }
        }

        /// <summary>
        /// Add all notes to play based on the current active cells of the board
        /// </summary>
        /// <param name="boardSettings"></param>
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
                            PlayNote(boardSettings, col);
                        }
                        else if (activeCells[col, row] == AbstractBoard.ActiveState.RowActivated)
                        {
                            // play corresponding note for the row
                            PlayNote(boardSettings, row);
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
