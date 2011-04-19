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

        /// <summary>
        /// The characters to draw for the different cell states
        /// </summary>
        private string[] cellStateText = { "▲", "►", "▼", "◄" };

        public MainForm()
        {
            InitializeComponent();

            // create a new board with the default columns/rows
            board = new OtomataBoard(Cols, Rows);

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

            // create required brushes, etc
            SolidBrush singleStateBrush = new SolidBrush(Color.DarkGray);
            SolidBrush mergedStateBrush = new SolidBrush(Color.White);
            SolidBrush txtbrush = new SolidBrush(Color.White);
            Font f = new Font("Times New Roman", 10f, FontStyle.Regular);

            // for each row on the board
            for (int row = 0; row < Rows; row++)
            {
                // for each col on the board
                for (int col = 0; col < Cols; col++)
                {
                    var cellState = board.Cells[col, row].State;
                    if (cellState != CellStateEnum.Dead)
                    {
                        // if the cell isn't dead, draw it 

                        // to draw the background of the cell use mergedbrush if cell state is merged, otherwise singlestatebrush
                        Brush b = (cellState == CellStateEnum.Merged ? mergedStateBrush : singleStateBrush);
                        g.FillRectangle(b, new RectangleF(col * cellSize.Width, row * cellSize.Height, cellSize.Width, cellSize.Height));

                        // draw the arrows of a cell if it is any of these cell states
                        if (cellState == CellStateEnum.Up || cellState == CellStateEnum.Down ||
                           cellState == CellStateEnum.Left || cellState == CellStateEnum.Right)
                        {
                            // get character for cellstate
                            string str = cellStateText[(int)cellState];
                            SizeF sizeStr = g.MeasureString(str, f);
                            // draw character in center of cell
                            g.DrawString(str, f, txtbrush, new PointF(col * cellSize.Width + cellSize.Width / 2 - sizeStr.Width / 2,
                                                                         row * cellSize.Height + cellSize.Height / 2 - sizeStr.Height / 2));
                        }
                    }
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

            // clean up
            singleStateBrush.Dispose();
            mergedStateBrush.Dispose();
            txtbrush.Dispose();
            f.Dispose();
        }




        /// <summary>
        /// Define cell state of certain cell or show debug info of selected cell
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void picBoard_MouseDown(object sender, MouseEventArgs e)
        {
            Rectangle bounds = new Rectangle(0, 0, picBoard.Width - 1, picBoard.Height - 1);

            // determine cell size
            SizeF cellSize = new SizeF(((float)bounds.Width / (float)Cols), ((float)bounds.Height / (float)Rows));

            // determine current cell index
            int col = (int)(e.X / cellSize.Width);
            int row = (int)(e.Y / cellSize.Height);

            // left click
            if (e.Button == System.Windows.Forms.MouseButtons.Left)
            {
                // switch to next state
                board.Cells[col, row].State = (CellStateEnum)((int)(board.Cells[col, row].State + 1) % ((int)CellStateEnum.Dead + 1));

                // but skip merged state
                if (board.Cells[col, row].State == CellStateEnum.Merged)
                    board.Cells[col, row].State = (CellStateEnum)((int)(board.Cells[col, row].State + 1) % ((int)CellStateEnum.Dead + 1));

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
                Alpha = 1;
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
            bool[,] activeCells = board.ActiveCells;

            for (int row = 0; row < board.Rows; row++)
            {
                for (int col = 0; col < board.Cols; col++)
                {
                    if (activeCells[col, row])
                    {
                        if (col == 0 || col == board.Cols - 1)
                        {
                            // add highlight for the row
                            highlights.Add(new Highlight(true, row));
                            // play corresponding note for the row
                            noteController.PlayNote(row, NoteDuration, Volume);
                        }
                        else if (row == 0 || row == board.Rows - 1)
                        {
                            // add highlight for the col
                            highlights.Add(new Highlight(false, col));
                            // play corresponding note for the col
                            noteController.PlayNote(col, NoteDuration, Volume);
                        }
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
            board.ChangeSize(Rows, Cols);
            picBoard.Invalidate();
        }

        /// <summary>
        /// Update board size if nr of columns is changed
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void nudColumns_ValueChanged(object sender, EventArgs e)
        {
            board.ChangeSize(Rows, Cols);
            picBoard.Invalidate();
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
        }


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
    }
}
