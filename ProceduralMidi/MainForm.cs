using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.Runtime.InteropServices;
using ProceduralMidi.DAL;

namespace ProceduralMidi
{
    public partial class MainForm : Form
    {
        private OtomataBoard board;

        private NoteController noteController = new NoteController();


        public MainForm()
        {
            InitializeComponent();

            // create a new board with the default columns/rows
            board = new OtomataBoard(Cols, Rows);
            cellPalette.States = board.PossibleStatesForPalette;
            if (cellPalette.Items.Count > 0)
                cellPalette.SelectedIndex = 0;

            // fill dropdowns
            FillMidiDevices();
            FillInstruments();

            if (ddlMidiDevices.Items.Count > 0)
                ddlMidiDevices.SelectedIndex = 0;

            if (ddlInstruments.Items.Count > 0)
                ddlInstruments.SelectedIndex = 0;
        }

        /// <summary>
        /// Fill dropdown with all possible instruments
        /// </summary>
        private void FillInstruments()
        {
            ddlInstruments.Items.Clear();
            ddlInstruments.Items.AddRange(Midi.Instruments.ToArray());
        }

        /// <summary>
        /// Fill dropdown with all midi devices
        /// </summary>
        private void FillMidiDevices()
        {
            ddlMidiDevices.Items.Clear();
            ddlMidiDevices.Items.AddRange(Midi.MidiDevices.ToArray());
        }

        /// <summary>
        /// Change instrument if another instrument is selected
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void ddlInstrument_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (ddlInstruments.SelectedIndex >= 0)
                Midi.ChangeInstrument(ddlInstruments.SelectedIndex, NoteController.MIDI_CHANNEL);
        }

        /// <summary>
        /// Change midi device to current selected
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void ddlMidiDevices_SelectedIndexChanged(object sender, EventArgs e)
        {
            Midi.ChangeMidiDevice(ddlMidiDevices.SelectedIndex);
        }

        /// <summary>
        /// Enable the timer for the iterations if run is checked
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void chkRun_CheckedChanged(object sender, EventArgs e)
        {
            tmrIterate.Enabled = chkRun.Checked;
        }




        /// <summary>
        /// Draws the board
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void picBoard_Paint(object sender, PaintEventArgs e)
        {
            Graphics g = e.Graphics;

            Rectangle bounds = new Rectangle(0, 0, picBoard.Width - 1, picBoard.Height - 1);

            // determine cell size
            SizeF cellSize = new SizeF(((float)bounds.Width / (float)Cols), ((float)bounds.Height / (float)Rows));

            // draw grid
            using (Pen p = new Pen(Color.Gray))
            {
                for (int i = 0; i <= Cols; i++)
                    g.DrawLine(p, new PointF(i * cellSize.Width, 0), new PointF(i * cellSize.Width, bounds.Bottom));

                for (int i = 0; i <= Rows; i++)
                    g.DrawLine(p, new PointF(0, i * cellSize.Height), new PointF(bounds.Right, i * cellSize.Height));
            }

            int rows = Rows;
            int cols = Cols;

            g.TextRenderingHint = System.Drawing.Text.TextRenderingHint.SingleBitPerPixel;

            // for each row on the board
            for (int row = 0; row < rows; row++)
            {
                // for each col on the board
                for (int col = 0; col < cols; col++)
                {
                    // if the cell isn't dead, draw it 
                    RectangleF cellBounds = new RectangleF(col * cellSize.Width, row * cellSize.Height, cellSize.Width, cellSize.Height);
                    g.DrawCell(board.Cells[col, row], cellBounds);
                }
            }

            // draw the highlights if there are any alive
            foreach (Highlight hl in highlights)
            {
                RectangleF r;
                if (hl.IsRow)
                    r = new RectangleF(0, hl.Index * cellSize.Height, cellSize.Width * Cols, cellSize.Height);
                else
                    r = new RectangleF(hl.Index * cellSize.Width, 0, cellSize.Width, cellSize.Height * Rows);

                // draw rectangle for entire column or row
                using (SolidBrush br = new SolidBrush(Color.FromArgb((int)(hl.Alpha * 255), Color.White)))
                    g.FillRectangle(br, r);
            }



        }

        /// <summary>
        /// Define cell state of certain cell or show debug info of selected cell
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void picBoard_MouseMove(object sender, MouseEventArgs e)
        {
            Rectangle bounds = new Rectangle(0, 0, picBoard.Width - 1, picBoard.Height - 1);

            // determine cell size
            SizeF cellSize = new SizeF(((float)bounds.Width / (float)Cols), ((float)bounds.Height / (float)Rows));

            // determine current cell index
            int col = (int)(e.X / cellSize.Width);
            int row = (int)(e.Y / cellSize.Height);

            // don't go out of bounds
            if (col < 0 || col >= Cols || row < 0 || row >= Rows)
                return;

            // left click
            if (e.Button == System.Windows.Forms.MouseButtons.Left)
            {
                board.Cells[col, row].State = cellPalette.SelectedState;

                // refresh board
                picBoard.Invalidate();
            }
            // right click
            else if (e.Button == System.Windows.Forms.MouseButtons.Right)
            {
                // show debug info
                lblDebug.Text = "Pos: " + col + "," + row + Environment.NewLine +
                                "State: " + board.Cells[col, row].State + Environment.NewLine +
                                "Merged states: " + string.Join(",", board.Cells[col, row].MergedStates.Select(s => s.ToString()).ToArray());
            }
        }

        private void picBoard_MouseDown(object sender, MouseEventArgs e)
        {
            picBoard_MouseMove(sender, e);
        }



        /// <summary>
        /// A list of highlights that are alive
        /// </summary>
        private List<Highlight> highlights = new List<Highlight>();

        /// <summary>
        /// Describes a highlight that is drawn on the board and is fading away
        /// </summary>
        private class Highlight
        {
            public Highlight(bool isRow, int index)
            {
                IsRow = isRow;
                Index = index;
                Alpha = 0.5f;
            }

            /// <summary>
            /// Is the highlight for a row
            /// </summary>
            public bool IsRow { get; set; }

            /// <summary>
            /// The row or column index to draw the highlight on
            /// </summary>
            public int Index { get; set; }

            /// <summary>
            /// The transparency of the highlight, once it reaches 0 it's dead and removed from the board
            /// </summary>
            public float Alpha { get; set; }
        }


        /// <summary>
        /// Move to next state
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btnNext_Click(object sender, EventArgs e)
        {
            MoveToNextState();
        }

        /// <summary>
        /// Move to next state
        /// </summary>
        private void MoveToNextState()
        {
            // advance the board to the next state
            board.NextState();

            // check if any acive cells have to be played
            PlayNotesForActiveCells();

            // refresh the board
            picBoard.Invalidate();
        }

        /// <summary>
        /// Play the notes for all active cells
        /// </summary>
        private void PlayNotesForActiveCells()
        {
            OtomataBoard.ActiveState[,] activeCells = board.ActiveCells;

            for (int row = 0; row < board.Rows; row++)
            {
                for (int col = 0; col < board.Cols; col++)
                {
                    if (activeCells[col, row] == AbstractBoard.ActiveState.ColumnActivated)
                    {
                        // add highlight for the col
                        highlights.Add(new Highlight(false, col));
                        // play corresponding note for the col
                        noteController.PlayNote(col, NoteDuration, Volume);                      
                    }
                    else if (activeCells[col, row] == AbstractBoard.ActiveState.RowActivated)
                    {
                        // add highlight for the row
                        highlights.Add(new Highlight(true, row));
                        // play corresponding note for the row
                        noteController.PlayNote(row, NoteDuration, Volume);
                    }
                }
            }
        }

        /// <summary>
        /// Timer tick advances the board to the next state
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void tmrIterate_Tick(object sender, EventArgs e)
        {
            MoveToNextState();
        }

        /// <summary>
        /// Remove all the cells from the board
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btnClean_Click(object sender, EventArgs e)
        {
            board.Clear();
            picBoard.Invalidate();
        }

        /// <summary>
        /// Draw the highlights on the board if there are any active
        /// and decrease the alpha values so that they fade out
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void tmrDrawHighlights_Tick(object sender, EventArgs e)
        {
            // if there are any highlights queued, draw them
            if (highlights.Count > 0)
            {
                foreach (var hl in highlights.ToList())
                {
                    hl.Alpha -= 0.1f;
                    if (hl.Alpha <= 0) // remove from  list if fadeout is complete
                        highlights.Remove(hl);
                }

                picBoard.Invalidate();
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void sldSpeed_ValueChanged(object sender, EventArgs e)
        {
            tmrIterate.Interval = sldSpeed.Value;
            lblSpeed.Text = "Speed (in ms, current=" + sldSpeed.Value + ")";
        }

        /// <summary>
        /// Update board size if nr of rows is changed
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void nudRows_ValueChanged(object sender, EventArgs e)
        {
            if (!ignoreNudRowColValueChange)
            {
                board.ChangeSize(Rows, Cols);
                picBoard.Invalidate();
            }
        }

        /// <summary>
        /// Update board size if nr of columns is changed
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void nudColumns_ValueChanged(object sender, EventArgs e)
        {
            if (!ignoreNudRowColValueChange)
            {
                board.ChangeSize(Rows, Cols);
                picBoard.Invalidate();
            }
        }

        /// <summary>
        /// The current nr of rows used
        /// </summary>
        public int Rows { get { return (int)nudRows.Value; } }

        /// <summary>
        /// The current nr of columns used
        /// </summary>
        public int Cols { get { return (int)nudColumns.Value; } }

        /// <summary>
        /// The current note duration used
        /// </summary>
        public int NoteDuration { get { return sldNoteDuration.Value; } }

        /// <summary>
        /// The volume used
        /// </summary>
        public short Volume { get { return (short)sldVolume.Value; } }

        /// <summary>
        /// Update the notes for each corresponding cell
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void txtNotes_TextChanged(object sender, EventArgs e)
        {
            if (!string.IsNullOrEmpty(txtNotes.Text))
                noteController.NotesPerCell = txtNotes.Text.ToUpper().Split(',').Select(n => n.Trim()).ToArray();
        }

        /// <summary>
        /// Update label to display current noteduration
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void sldNoteDuration_ValueChanged(object sender, EventArgs e)
        {
            lblNoteDuration.Text = "Note duration (in ms, current=" + sldNoteDuration.Value + ")";
        }


        /// <summary>
        /// The last opened file
        /// </summary>
        private string lastPathOpened;

        /// <summary>
        /// Open a board & settings from a file
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void mnuOpen_Click(object sender, EventArgs e)
        {
            try
            {
                using (OpenFileDialog ofd = new OpenFileDialog())
                {
                    ofd.Filter = "*.txt|*.txt";

                    if (ofd.ShowDialog(this) == System.Windows.Forms.DialogResult.OK)
                        LoadBoard(ofd.FileName);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Could not open file, error: " + ex.GetType().FullName + " - " + ex.Message, "Could not open file");
            }
        }


        /// <summary>
        /// Load the board from a file
        /// </summary>
        /// <param name="path"></param>
        private void LoadBoard(string path)
        {
            ignoreNudRowColValueChange = true;
            BoardSettings boardSettings;
            if (BoardMapper.TryLoad(path, out boardSettings))
            {
                board = boardSettings.Board;
                sldNoteDuration.Value = boardSettings.NoteDuration;
                sldSpeed.Value = boardSettings.Speed;
                txtNotes.Text = boardSettings.Notes;
                ddlInstruments.SelectedIndex = boardSettings.Instrument;
                nudRows.Value = boardSettings.Board.Rows;
                nudColumns.Value = boardSettings.Board.Cols;

                picBoard.Invalidate();

                lastPathOpened = path;
            }
            ignoreNudRowColValueChange = false;
        }

        private bool ignoreNudRowColValueChange;


        /// <summary>
        /// Reloads the last loaded file, if nothing was loaded, the open dialog will be displayed
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void mnuReload_Click(object sender, EventArgs e)
        {
            try
            {
                if (!string.IsNullOrEmpty(lastPathOpened))
                    LoadBoard(lastPathOpened);
                else
                    mnuOpen_Click(sender, e);
            }
            catch (Exception ex)
            {
                MessageBox.Show("Could not open file, error: " + ex.GetType().FullName + " - " + ex.Message, "Could not open file");
            }
        }

        /// <summary>
        /// Saves the current board & settings to a text file
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void mnuSave_Click(object sender, EventArgs e)
        {
            try
            {
                using (SaveFileDialog sfd = new SaveFileDialog())
                {
                    sfd.Filter = "*.txt|*.txt";
                    sfd.OverwritePrompt = true;
                    if (sfd.ShowDialog(this) == System.Windows.Forms.DialogResult.OK)
                    {
                        BoardMapper.Save(sfd.FileName, new BoardSettings()
                        {
                            Board = board,
                            Instrument = ddlInstruments.SelectedIndex,
                            NoteDuration = NoteDuration,
                            Notes = txtNotes.Text,
                            Speed = sldSpeed.Value
                        });
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Could not save file, error: " + ex.GetType().FullName + " - " + ex.Message, "Could not save file");
            }
        }


        /// <summary>
        /// Closes the app
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void mnuExit_Click(object sender, EventArgs e)
        {
            this.Close();
        }


        /// <summary>
        /// Shorcut keys for run and next step
        /// </summary>
        /// <param name="msg"></param>
        /// <param name="keyData"></param>
        /// <returns></returns>
        protected override bool ProcessCmdKey(ref Message msg, Keys keyData)
        {
            if (keyData == Keys.F5)
                chkRun.Checked = !chkRun.Checked;
            else if (keyData == Keys.F6)
                btnNext_Click(btnNext, EventArgs.Empty);

            return base.ProcessCmdKey(ref msg, keyData);
        }


        /// <summary>
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            if (disposing && noteController != null)
                noteController.Dispose();

            base.Dispose(disposing);
        }

        private void chkRec_CheckedChanged(object sender, EventArgs e)
        {
            if (chkRec.Checked)
            {
                noteController.Recorder = new Recorder();
                tmrIterate.Enabled = true;
            }
            else
            {
                tmrIterate.Enabled = false;

                try
                {
                    using (SaveFileDialog sfd = new SaveFileDialog())
                    {
                        sfd.Filter = "*.mid|*.mid";
                        if (sfd.ShowDialog(this) == System.Windows.Forms.DialogResult.OK)
                        {
                            MidiWriter.Write(sfd.FileName, ddlInstruments.SelectedIndex, noteController.Recorder.Notes);
                        }
                    }
                }
                catch (Exception ex)
                {
                    MessageBox.Show("Could not save file, error: " + ex.GetType().FullName + " - " + ex.Message, "Could not save file");
                }

                noteController.Recorder = null;
            }
        }

        protected override void OnMouseWheel(MouseEventArgs e)
        {
            if (cellPalette.Items.Count > 0 && picBoard.Focused || cellPalette.Focused)
            {
                if (e.Delta > 0)
                    cellPalette.SelectedIndex = (cellPalette.SelectedIndex + 1) % cellPalette.Items.Count;
                else if (e.Delta < 0)
                    cellPalette.SelectedIndex = ((cellPalette.SelectedIndex - 1) < 0 ? cellPalette.Items.Count - 1 : cellPalette.SelectedIndex - 1);
            }
            base.OnMouseWheel(e);
        }

        private void btnRandomize_Click(object sender, EventArgs e)
        {
            Random rnd = new Random();

            List<CellStateEnum> possibleStates = board.PossibleStatesForPalette.Where(s => s != CellStateEnum.Dead).ToList();

            for (int row = 0; row < board.Rows; row++)
            {
                for (int col = 0; col < board.Cols; col++)
                {
                    if (rnd.NextDouble() > 0.8f)
                    {
                        board.Cells[col, row] = new Cell(possibleStates[rnd.Next(possibleStates.Count)]);
                    }
                    else
                        board.Cells[col, row] = new Cell(CellStateEnum.Dead);
                }
            }
            picBoard.Invalidate();
        }
    }
}
