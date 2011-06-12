using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using ProceduralMidi.DAL;

namespace ProceduralMidi
{
    /// <summary>
    /// Exports the board to a midi for a number of states
    /// </summary>
    public partial class FrmGenerateMidi : Form
    {
        /// <summary>
        /// The initial board settings
        /// </summary>
        private BoardSettings boardSettings;

        /// <summary>
        /// Create a new form with the initial board settings
        /// </summary>
        /// <param name="settings"></param>
        public FrmGenerateMidi(BoardSettings settings)
        {
            this.boardSettings = settings;
            InitializeComponent();

            rdbTimeElapsed.Checked = true;
        }

        /// <summary>
        /// Update time elapsed
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void nudNumberOfSteps_ValueChanged(object sender, EventArgs e)
        {
            nudTimeElapsed.Value = (nudNumberOfSteps.Value * boardSettings.Speed) / 1000;
        }

        /// <summary>
        /// Update nr of steps
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void nudTimeElapsed_ValueChanged(object sender, EventArgs e)
        {
            nudNumberOfSteps.Value = (nudTimeElapsed.Value * 1000) / boardSettings.Speed;
        }

        /// <summary>
        /// Update the enabled property of corresponding numeric box 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void rdbNumberOfSteps_CheckedChanged(object sender, EventArgs e)
        {
            UpdateEnabled();
        }
        /// <summary>
        /// Update the enabled property of corresponding numeric box 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void rdbTimeElapsed_CheckedChanged(object sender, EventArgs e)
        {
            UpdateEnabled();
        }
        /// <summary>
        /// Update the enabled property of corresponding numeric box 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void rdbNumberOfNotes_CheckedChanged(object sender, EventArgs e)
        {
            UpdateEnabled();
        }

        /// <summary>
        /// Update the enabled property of corresponding numeric box 
        /// </summary>
        private void UpdateEnabled()
        {
            if (rdbNumberOfNotes.Checked)
            {
                nudNumberOfNotes.Enabled = true;
                nudNumberOfSteps.Enabled = false;
                nudTimeElapsed.Enabled = false;
            }
            else if (rdbNumberOfSteps.Checked)
            {
                nudNumberOfNotes.Enabled = false;
                nudNumberOfSteps.Enabled = true;
                nudTimeElapsed.Enabled = false;
            }
            else if (rdbTimeElapsed.Checked)
            {
                nudNumberOfNotes.Enabled = false;
                nudNumberOfSteps.Enabled = false;
                nudTimeElapsed.Enabled = true;
            }
        }

        /// <summary>
        /// Determines if generating has to be cancelled
        /// </summary>
        private bool cancelGenerating;

        /// <summary>
        /// Either generate or stop generating
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btnGenerate_Click(object sender, EventArgs e)
        {
            if (btnGenerate.Tag == null || (bool)btnGenerate.Tag == false)
            {
                btnGenerate.Text = "Stop";
                btnGenerate.Tag = true;

                Generate();

                btnGenerate.Text = "Generate";
                btnGenerate.Tag = false;
            }
            else
            {
                btnGenerate.Text = "Generate";
                btnGenerate.Tag = false;
                cancelGenerating = true;
            }
        }

        /// <summary>
        /// Ask user where to save the midi & generate midi
        /// </summary>
        private void Generate()
        {
            try
            {
                using (SaveFileDialog sfd = new SaveFileDialog())
                {
                    sfd.Filter = "*.mid|*.mid";
                    if (sfd.ShowDialog(this) == System.Windows.Forms.DialogResult.OK)
                    {
                        cancelGenerating = false;
                        List<Note> notes = GenerateNotes();

                        if (notes != null && notes.Count > 0)
                        {
                            MidiWriter.Write(sfd.FileName, boardSettings.Instrument, notes);
                            lblStatus.Text = "Midi saved to " + sfd.FileName;
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Could not save file, error: " + ex.GetType().FullName + " - " + ex.Message, "Could not save file");
            }
        }

        /// <summary>
        /// Generate notes from the board
        /// </summary>
        /// <returns></returns>
        private List<Note> GenerateNotes()
        {
            pbProgress.Visible = true;
            lblStatus.Text = "Generating...";

            // determine ending value and condition
            int nrOfStepsToProcess = (int)nudNumberOfSteps.Value;
            int nrOfNotesToPlay = (int)nudNumberOfNotes.Value;
            int nrOfMsToPass = (int)nudTimeElapsed.Value * 1000;

            ExportConditionEnum condition = ExportConditionEnum.TimeLimit;
            if (rdbNumberOfNotes.Checked)
                condition = ExportConditionEnum.NoteLimit;
            else if (rdbNumberOfSteps.Checked)
                condition = ExportConditionEnum.StepLimit;
            else if (rdbTimeElapsed.Checked)
                condition = ExportConditionEnum.TimeLimit;

            // keep track of steps since last note to prevent infinite loop
            int stepsSinceLastNote = 0;

            int curStep = 0;
            int curNoteCount = 0;
            int curMsPassed = 0;

            DateTime startTime = DateTime.Now;

            // take a deep copy of the board settings (don't want to alter the current board)
            BoardSettings boardsettings;
            if (!BoardMapper.TryGetBoardSettingsFromString(BoardMapper.GetStringFromBoardSettings(boardSettings), out boardsettings))
            {
                MessageBox.Show("Could not create a copy of the current board", "Error", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return null;
            }

            List<Note> notes = new List<Note>();
            double progress;

            // continue advancing to next state until either cancelled or condition is met
            while (!cancelGenerating && !ExportConditionMet(condition, curStep, curNoteCount, curMsPassed, nrOfStepsToProcess, nrOfNotesToPlay, nrOfMsToPass, out progress))
            {
                // advance board
                boardsettings.Board.NextState();

                // get the active notes of the board
                var activeNotes = GetNotesForActiveCells(boardsettings);
                // and update the correct time played
                foreach (var n in activeNotes)
                    n.TimeDown = startTime.AddMilliseconds(curMsPassed);

                // and add them to the notes list
                notes.AddRange(activeNotes);

                curStep++;
                curNoteCount += activeNotes.Count;
                curMsPassed += boardsettings.Speed;

                // check steps since last note
                if (activeNotes.Count == 0)
                    stepsSinceLastNote++;
                else
                    stepsSinceLastNote = 0;

                // prevent infinite loop when no notes are played anymore, ask user to continue
                if (stepsSinceLastNote > 10000)
                {
                    DialogResult result = MessageBox.Show("There have been no notes for 10000 steps now, do you want to continue generating?", "No notes for a long time", MessageBoxButtons.YesNo, MessageBoxIcon.Question);
                    if (result == System.Windows.Forms.DialogResult.No)
                        break;
                    else
                        stepsSinceLastNote = 0; // reset steps since last note
                }

                // update gui each 100 steps
                if (curStep % 100 == 0)
                {
                    pbProgress.Value = (int)(progress * 100);
                    lblStatus.Text = "Generating... | Current iteration " + curStep;
                    Application.DoEvents();
                }
            }

            pbProgress.Visible = false;

            if (notes.Count <= 0)
                lblStatus.Text = "No midi generated, there weren't any notes to record";
            else
                lblStatus.Text = "Midi generated";

            return notes;
        }

        /// <summary>
        /// The possible ending conditions
        /// </summary>
        private enum ExportConditionEnum
        {
            StepLimit,
            NoteLimit,
            TimeLimit
        }

        /// <summary>
        /// Determines if the condition to stop advancing the board state is met
        /// </summary>
        /// <param name="condition"></param>
        /// <param name="curStep"></param>
        /// <param name="curNoteCount"></param>
        /// <param name="curMsPassed"></param>
        /// <param name="nrOfStepsToProcess"></param>
        /// <param name="nrOfNotesToPlay"></param>
        /// <param name="nrOfMsToPass"></param>
        /// <param name="progress"></param>
        /// <returns></returns>
        private bool ExportConditionMet(ExportConditionEnum condition, int curStep, int curNoteCount, int curMsPassed, int nrOfStepsToProcess, int nrOfNotesToPlay, int nrOfMsToPass, out double progress)
        {
            if (condition == ExportConditionEnum.StepLimit)
            {
                progress = curStep / (double)nrOfStepsToProcess;
                return curStep >= nrOfStepsToProcess;
            }
            else if (condition == ExportConditionEnum.NoteLimit)
            {
                progress = curNoteCount / (double)nrOfNotesToPlay;
                return curNoteCount >= nrOfNotesToPlay;
            }
            else if (condition == ExportConditionEnum.TimeLimit)
            {
                progress = curMsPassed / (double)nrOfMsToPass;
                return curMsPassed >= nrOfMsToPass;
            }
            else
            {
                progress = 0;
                return true;
            }
        }

        /// <summary>
        /// Returns a list of notes based on the active cells of the given board
        /// </summary>
        /// <param name="boardSettings"></param>
        /// <returns></returns>
        public static List<Note> GetNotesForActiveCells(BoardSettings boardSettings)
        {
            List<Note> notes = new List<Note>();

            OtomataBoard.ActiveState[,] activeCells = boardSettings.Board.ActiveCells;

            for (int row = 0; row < boardSettings.Board.Rows; row++)
            {
                for (int col = 0; col < boardSettings.Board.Cols; col++)
                {
                    if (activeCells[col, row] == AbstractBoard.ActiveState.ColumnActivated)
                    {
                        // play corresponding note for the col
                        Note n = NoteController.CreateNote(col, boardSettings.NoteDuration, boardSettings.Volume, boardSettings.NotesPerCell);
                        notes.Add(n);
                    }
                    else if (activeCells[col, row] == AbstractBoard.ActiveState.RowActivated)
                    {
                        // play corresponding note for the row
                        Note n = NoteController.CreateNote(row, boardSettings.NoteDuration, boardSettings.Volume, boardSettings.NotesPerCell);
                        notes.Add(n);
                    }
                }
            }
            return notes;
        }
    }
}
