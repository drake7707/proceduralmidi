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
    public partial class BoardEditor : UserControl
    {
        ///// <summary>
        ///// The current board that will be drawn & manipulated
        ///// </summary>
        //private OtomataBoard board;
        ///// <summary>
        ///// The current board that will be drawn & manipulated
        ///// </summary>
        //public OtomataBoard Board
        //{
        //    get { return board; }
        //    set
        //    {
        //        if (board != value && board != null)
        //        {
        //            board = value;
        //            picBoard.Invalidate();
        //        }
        //    }
        //}

        public BoardSettings BoardSettings { get; set; }


        /// <summary>
        /// The note controller to play & record notes
        /// </summary>
        private NoteController noteController;

        /// <summary>
        /// Creates a new Otomata board form
        /// </summary>
        public BoardEditor()
        {
            InitializeComponent();

            // try creating the notecontroller with sample support
            try
            {
                noteController = new NoteController(this, true);
            }
            catch (Exception ex)
            {
                // it failed (directx not installed, ..), disable samples
                noteController = new NoteController(this, false);
                rdbSample.Enabled = false;
                ddlSamples.Enabled = false;
            }



            //// default values
            //sldVolume.Value = 64;
            //sldSpeed.Value = 250;
            //sldNoteDuration.Value = 500;
            //sldSpeed_ValueChanged(sldSpeed, EventArgs.Empty);
            //sldNoteDuration_ValueChanged(sldNoteDuration, EventArgs.Empty);


            //// create a new board with the default columns/rows
            //board = new OtomataBoard(Cols, Rows);


            // fill dropdowns
            FillMidiDevices();
            FillInstruments();
            FillSamples();

            // create states
            CreateStates();

            //// set default dropdown selection
            if (ddlMidiDevices.Items.Count > 0)
                ddlMidiDevices.SelectedIndex = 0;

            //if (ddlInstruments.Items.Count > 0)
            //    ddlInstruments.SelectedIndex = 0;

            //if (ddlSamples.Items.Count > 0)
            //    ddlSamples.SelectedIndex = 0;

            UpdateFromBoardSettings(BoardSettings.GetDefaultBoard());

            cellPalette.States = BoardSettings.Board.PossibleStatesForPalette;
            if (cellPalette.Items.Count > 0)
                cellPalette.SelectedIndex = 0;

        }

        /// <summary>
        /// Create all the state menu items
        /// </summary>
        private void CreateStates()
        {
            for (int i = 0; i < 10; i++)
            {
                var mnuItm = new ToolStripMenuItem("Empty state", null, (sender, e) =>
                    {
                        LoadState(mnuStates.DropDownItems.IndexOf((ToolStripItem)sender));
                    });
                mnuItm.ToolTipText = "Save state with Ctrl+<nr>, load state with Shift+<nr>";
                mnuItm.ShortcutKeyDisplayString = "Ctrl/Shift+" + i.ToString();
                mnuStates.DropDownItems.Add(mnuItm);
            }
        }

        /// <summary>
        /// Save the state of the board in the given slot
        /// </summary>
        /// <param name="i"></param>
        private void SaveState(int i)
        {
            mnuStates.DropDownItems[i].Text = "State " + DateTime.Now.ToString("HH:mm:ss");
            mnuStates.DropDownItems[i].Tag = BoardMapper.GetStringFromBoardSettings(BoardSettings);
        }

        /// <summary>
        /// Load the state of the board from the given slot, nothing will be loaded if the slot is empty
        /// </summary>
        /// <param name="i"></param>
        private void LoadState(int i)
        {
            var boardsettingsString = (string)mnuStates.DropDownItems[i].Tag;
            if (boardsettingsString != null)
            {
                BoardSettings boardsettings;
                if (BoardMapper.TryGetBoardSettingsFromString(boardsettingsString, out boardsettings))
                    UpdateFromBoardSettings(boardsettings);
            }
        }

        /// <summary>
        /// Fill dropdown with all possible instruments
        /// </summary>
        private void FillInstruments()
        {
            ddlInstruments.Items.Clear();
            ddlInstruments.Items.AddRange(MidiManager.Instruments.ToArray());
        }

        /// <summary>
        /// Fill dropdown with all midi devices
        /// </summary>
        private void FillMidiDevices()
        {
            ddlMidiDevices.Items.Clear();
            ddlMidiDevices.Items.AddRange(MidiManager.MidiDevices.ToArray());
        }

        /// <summary>
        /// Fill the samples dropdown
        /// </summary>
        private void FillSamples()
        {
            try
            {
                ddlSamples.Items.Clear();
                ddlSamples.Items.AddRange(SampleManager.Samples.ToArray());

            }
            catch (Exception ex)
            {
                MessageBox.Show("Could not load the available samples. Is the samples subdirectory available? Error: " + ex.Message);
            }
        }

        /// <summary>
        /// Change instrument if another instrument is selected
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void ddlInstrument_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (ddlInstruments.SelectedIndex >= 0)
            {
                MidiManager.ChangeInstrument(ddlInstruments.SelectedIndex, NoteController.MIDI_CHANNEL);
                BoardSettings.Instrument = ddlInstruments.SelectedIndex;
            }
        }

        /// <summary>
        /// Change midi device to current selected
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void ddlMidiDevices_SelectedIndexChanged(object sender, EventArgs e)
        {
            MidiManager.ChangeMidiDevice(ddlMidiDevices.SelectedIndex);
        }


        /// <summary>
        /// Draws the board
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void picBoard_Paint(object sender, PaintEventArgs e)
        {

            BoardSettings settings = this.BoardSettings;

            Graphics g = e.Graphics;

            Rectangle bounds = new Rectangle(0, 0, picBoard.Width - 1, picBoard.Height - 1);

            int rows = settings.Board.Rows;
            int cols = settings.Board.Cols;


            // determine cell size
            SizeF cellSize = new SizeF(((float)bounds.Width / (float)cols), ((float)bounds.Height / (float)rows));

            // draw grid
            if (mnuShowGrid.Checked)
            {
                using (Pen p = new Pen(Color.Gray))
                {
                    for (int i = 0; i <= settings.Board.Cols; i++)
                        g.DrawLine(p, new PointF(i * cellSize.Width, 0), new PointF(i * cellSize.Width, bounds.Bottom));

                    for (int i = 0; i <= settings.Board.Rows; i++)
                        g.DrawLine(p, new PointF(0, i * cellSize.Height), new PointF(bounds.Right, i * cellSize.Height));
                }
            }

            bool drawFancy = mnuFancy.Checked;
            if (drawFancy)
            {
                g.SmoothingMode = System.Drawing.Drawing2D.SmoothingMode.AntiAlias;
                g.TextRenderingHint = System.Drawing.Text.TextRenderingHint.AntiAliasGridFit;
            }
            else
            {
                g.SmoothingMode = System.Drawing.Drawing2D.SmoothingMode.HighSpeed;
                g.TextRenderingHint = System.Drawing.Text.TextRenderingHint.SingleBitPerPixel;
            }

            // for each row on the board
            for (int row = 0; row < rows; row++)
            {
                // for each col on the board
                for (int col = 0; col < cols; col++)
                {
                    // if the cell isn't dead, draw it 
                    RectangleF cellBounds = new RectangleF(col * cellSize.Width + 2, row * cellSize.Height + 2, cellSize.Width - 4, cellSize.Height - 4);
                    if (drawFancy)
                        g.DrawCellFancy(settings.Board.Cells[col, row], cellBounds);
                    else
                        g.DrawCellFast(settings.Board.Cells[col, row], cellBounds);
                }
            }

            // draw the highlights if there are any alive, // tolist because vst plugin accesses it from different thread
            foreach (Highlight hl in highlights.ToList())
            {
                RectangleF r;
                if (hl.IsRow)
                    r = new RectangleF(0, hl.Index * cellSize.Height, cellSize.Width * cols, cellSize.Height);
                else
                    r = new RectangleF(hl.Index * cellSize.Width, 0, cellSize.Width, cellSize.Height * rows);

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
            int rows = BoardSettings.Board.Rows;
            int cols = BoardSettings.Board.Cols;

            Rectangle bounds = new Rectangle(0, 0, picBoard.Width - 1, picBoard.Height - 1);

            // determine cell size
            SizeF cellSize = new SizeF(((float)bounds.Width / (float)cols), ((float)bounds.Height / (float)rows));

            // determine current cell index
            int col = (int)(e.X / cellSize.Width);
            int row = (int)(e.Y / cellSize.Height);

            // don't go out of bounds
            if (col < 0 || col >= cols || row < 0 || row >= rows)
                return;

            // left click
            if (e.Button == System.Windows.Forms.MouseButtons.Left)
            {
                BoardSettings.Board.Cells[col, row].State = cellPalette.SelectedState;

                // refresh board
                UpdateBoardGUI();
            }
            // right click
            else if (e.Button == System.Windows.Forms.MouseButtons.Right)
            {
                // show debug info
                lblDebug.Text = "Pos: " + col + "," + row + Environment.NewLine +
                                "State: " + BoardSettings.Board.Cells[col, row].State + Environment.NewLine +
                                "Merged states: " + string.Join(",", BoardSettings.Board.Cells[col, row].MergedStates.Select(s => s.ToString()).ToArray());
            }
        }

        /// <summary>
        /// Define cell state of certain cell or show debug info of selected cell
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
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
                Alpha = 0.7f;
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
        private void MoveToNextState()
        {
            // advance the board to the next state
            BoardSettings.Board.NextState();

            // check if any acive cells have to be played
            PlayNotesForActiveCells();

            // refresh the board
            UpdateBoardGUI();
        }

        /// <summary>
        /// Play the notes for all active cells
        /// </summary>
        private void PlayNotesForActiveCells()
        {
            try
            {
                OtomataBoard.ActiveState[,] activeCells = BoardSettings.Board.ActiveCells;

                for (int row = 0; row < BoardSettings.Board.Rows; row++)
                {
                    for (int col = 0; col < BoardSettings.Board.Cols; col++)
                    {
                        if (activeCells[col, row] == AbstractBoard.ActiveState.ColumnActivated)
                        {
                            // add highlight for the col
                            highlights.Add(new Highlight(false, col));
                            // play corresponding note for the col
                            noteController.PlayNote(col, BoardSettings.NoteDuration, Volume);
                        }
                        else if (activeCells[col, row] == AbstractBoard.ActiveState.RowActivated)
                        {
                            // add highlight for the row
                            highlights.Add(new Highlight(true, row));
                            // play corresponding note for the row
                            noteController.PlayNote(row, BoardSettings.NoteDuration, Volume);
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                btnRun.Checked = false;
                btnRun_CheckedChanged(btnRun, EventArgs.Empty);

                MessageBox.Show("Unable to play sound for current state, error: " + ex.GetType().FullName + " - " + ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        /// <summary>
        /// Adds highlights all active cells
        /// </summary>
        public void AddHighlightsForActiveCells()
        {
            OtomataBoard.ActiveState[,] activeCells = BoardSettings.Board.ActiveCells;

            for (int row = 0; row < BoardSettings.Board.Rows; row++)
            {
                for (int col = 0; col < BoardSettings.Board.Cols; col++)
                {
                    if (activeCells[col, row] == AbstractBoard.ActiveState.ColumnActivated)
                    {
                        // add highlight for the col
                        highlights.Add(new Highlight(false, col));
                    }
                    else if (activeCells[col, row] == AbstractBoard.ActiveState.RowActivated)
                    {
                        // add highlight for the row
                        highlights.Add(new Highlight(true, row));
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

            if (btnRecord.Checked)
                lblStatus.Text = "Recording.. (" + noteController.Recorder.Notes.Count + " notes in " + new DateTime((DateTime.Now - noteController.Recorder.Start).Ticks).ToString("HH:mm:ss") + ")";
        }

        /// <summary>
        /// Remove all the cells from the board
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btnClean_Click(object sender, EventArgs e)
        {
            BoardSettings.Board.Clear();
            UpdateBoardGUI();
        }

        /// <summary>
        /// Draw the highlights on the board if there are any active
        /// and decrease the alpha values so that they fade out
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void tmrDrawHighlights_Tick(object sender, EventArgs e)
        {
            bool drawFancy = mnuFancy.Checked;

            // if there are any highlights queued, draw them
            if (highlights.Count > 0)
            {
                foreach (var hl in highlights.ToList())
                {
                    if (drawFancy)
                        hl.Alpha -= 0.1f;
                    else
                        hl.Alpha -= 0.5f;

                    if (hl.Alpha <= 0) // remove from  list if fadeout is complete
                        highlights.Remove(hl);

                }

                UpdateBoardGUI();
            }
        }

        /// <summary>
        /// Changes the speed & updates the label
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void sldSpeed_ValueChanged(object sender, EventArgs e)
        {
            tmrIterate.Interval = sldSpeed.Value;
            lblSpeed.Text = "Speed (in bpm, current=" + ((int)(60000f / (float)sldSpeed.Value)) + ")";
            BoardSettings.Speed = sldSpeed.Value;
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
                BoardSettings.Board.ChangeSize((int)nudRows.Value, (int)nudColumns.Value);
                UpdateBoardGUI();
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
                BoardSettings.Board.ChangeSize((int)nudRows.Value, (int)nudColumns.Value);
                UpdateBoardGUI();
            }
        }

        ///// <summary>
        ///// The current nr of rows used
        ///// </summary>
        //public int Rows { get { return (int)nudRows.Value; } }

        ///// <summary>
        ///// The current nr of columns used
        ///// </summary>
        //public int Cols { get { return (int)nudColumns.Value; } }

        ///// <summary>
        ///// The current note duration used
        ///// </summary>
        //public int NoteDuration { get { return sldNoteDuration.Value; } }

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
            {
                noteController.NotesPerCell = txtNotes.Text.ToUpper().Split(',').Select(n => n.Trim()).ToArray();
                BoardSettings.Notes = txtNotes.Text;
            }
        }

        /// <summary>
        /// Update label to display current noteduration
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void sldNoteDuration_ValueChanged(object sender, EventArgs e)
        {
            lblNoteDuration.Text = "Note duration (in ms, current=" + sldNoteDuration.Value + ")";
            BoardSettings.NoteDuration = sldNoteDuration.Value;
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
                UpdateFromBoardSettings(boardSettings);
                lastPathOpened = path;
            }
            ignoreNudRowColValueChange = false;
        }

        /// <summary>
        /// Set controls to the values specified in the board settings
        /// </summary>
        /// <param name="boardSettings"></param>
        public void UpdateFromBoardSettings(BoardSettings boardSettings)
        {
            if (BoardSettings == null)
                BoardSettings = boardSettings;
            else
            {
                // don't make a new instance of boarsettings, otherwise the databinding with the plugin will break;
                BoardSettings.Board = boardSettings.Board;
                BoardSettings.Instrument = boardSettings.Instrument;
                BoardSettings.NoteDuration = boardSettings.NoteDuration;
                BoardSettings.Notes = boardSettings.Notes;
                BoardSettings.Sample = boardSettings.Sample;
                BoardSettings.Speed = boardSettings.Speed;
                BoardSettings.UseSamples = boardSettings.UseSamples;
            }

            sldNoteDuration.Value = boardSettings.NoteDuration;
            sldSpeed.Value = boardSettings.Speed;
            txtNotes.Text = boardSettings.Notes;
            ddlInstruments.SelectedIndex = boardSettings.Instrument;
            nudRows.Value = boardSettings.Board.Rows;
            nudColumns.Value = boardSettings.Board.Cols;
            rdbSample.Checked = boardSettings.UseSamples;
            rdbMidi.Checked = !boardSettings.UseSamples;
            ddlSamples.SelectedIndex = ddlSamples.FindString(boardSettings.Sample);

            UpdateBoardGUI();
        }

        /// <summary>
        /// Ignore the row/col value change events
        /// </summary>
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
                        BoardMapper.Save(sfd.FileName, BoardSettings);
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Could not save file, error: " + ex.GetType().FullName + " - " + ex.Message, "Could not save file");
            }
        }

        ///// <summary>
        ///// Returns the current board and its settings
        ///// </summary>
        ///// <returns></returns>
        //private BoardSettings GetCurrentBoardSettings()
        //{

        //    return new BoardSettings()
        //    {
        //        Board = board,
        //        Instrument = ddlInstruments.SelectedIndex,
        //        NoteDuration = NoteDuration,
        //        Notes = txtNotes.Text,
        //        Speed = sldSpeed.Value,
        //        UseSamples = rdbSample.Checked,
        //        Sample = (ddlSamples.SelectedIndex >= 0 ? ddlSamples.Items[ddlSamples.SelectedIndex].ToString() : "")
        //    };
        //}

        //public void UpdateToBoardSettings(BoardSettings settings)
        //{
        //    settings.Board = board;
        //    settings.Instrument = ddlInstruments.SelectedIndex;
        //    settings.NoteDuration = NoteDuration;
        //    settings.Notes = txtNotes.Text;
        //    settings.Speed = sldSpeed.Value;
        //    settings.UseSamples = rdbSample.Checked;
        //    settings.Sample = (ddlSamples.SelectedIndex >= 0 ? ddlSamples.Items[ddlSamples.SelectedIndex].ToString() : "");
        //}


        /// <summary>
        /// Closes the app
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void mnuExit_Click(object sender, EventArgs e)
        {
            EventHandler ev = Close;
            if (ev != null)
                ev(this, EventArgs.Empty);
        }

        public event EventHandler Close;

        /// <summary>
        /// Shorcut keys for run and next step
        /// </summary>
        /// <param name="msg"></param>
        /// <param name="keyData"></param>
        /// <returns></returns>
        protected override bool ProcessCmdKey(ref Message msg, Keys keyData)
        {
            if (keyData == Keys.F5)
            {
                btnRun.Checked = !btnRun.Checked;
            }
            else if (keyData == Keys.F6)
                btnStep_Click(btnStep, EventArgs.Empty);
            else if (keyData == Keys.F7)
                btnRecord.Checked = !btnRecord.Checked;
            else
            {
                if (!txtNotes.Focused) // shift+nr will give problems on azerty keyboards (like mine)
                {
                    for (int i = 0; i < 9; i++)
                    {
                        if (keyData == (Keys.Control | (Keys)(Keys.D0 + i)) ||
                           keyData == (Keys.Control | (Keys)(Keys.NumPad0 + i)))
                            SaveState(i);
                        else if (keyData == (Keys.Shift | (Keys)(Keys.D0 + i)) ||
                           keyData == (Keys.Shift | (Keys)(Keys.NumPad0 + i)))
                            LoadState(i);
                    }
                }
            }

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

        /// <summary>
        /// Randomizes the board
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btnRandomize_Click(object sender, EventArgs e)
        {
            RandomizeBoard(BoardSettings.Board);
            UpdateBoardGUI();
        }



        private void RandomizeBoard(OtomataBoard board)
        {
            Random rnd = new Random();

            List<CellStateEnum> possibleStates = board.PossibleStatesForPalette.Where(s => s != CellStateEnum.Dead).ToList();

            for (int row = 0; row < board.Rows; row++)
            {
                for (int col = 0; col < board.Cols; col++)
                {
                    if (rnd.NextDouble() >= 0.8f) // only 20% of cells are not dead
                        board.Cells[col, row] = new Cell(possibleStates[rnd.Next(possibleStates.Count)]);
                    else
                        board.Cells[col, row] = new Cell(CellStateEnum.Dead);
                }
            }
        }

        /// <summary>
        /// Toggles the graphic setting
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void mnuFancy_Click(object sender, EventArgs e)
        {
            // toggle fancy setting
            mnuFancy.Checked = !mnuFancy.Checked;
            UpdateBoardGUI();
        }

        /// <summary>
        /// Toggles the grid
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void mnuShowGrid_Click(object sender, EventArgs e)
        {
            mnuShowGrid.Checked = !mnuShowGrid.Checked;
            UpdateBoardGUI();
        }

        /// <summary>
        /// Enable the timer for the iterations if run is checked
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btnRun_CheckedChanged(object sender, EventArgs e)
        {
            tmrIterate.Enabled = btnRun.Checked;
        }

        /// <summary>
        /// Add recorder to note controller and save to midi when recording is stopped
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btnRecord_CheckedChanged(object sender, EventArgs e)
        {
            if (btnRecord.Checked)
            {
                noteController.Recorder = new Recorder();
                noteController.Recorder.Start = DateTime.Now;
                tmrIterate.Enabled = true;
            }
            else
            {
                tmrIterate.Enabled = false;
                lblStatus.Text = "Recording stopped";
                try
                {
                    using (SaveFileDialog sfd = new SaveFileDialog())
                    {
                        sfd.Filter = "*.mid|*.mid";
                        if (sfd.ShowDialog(this) == System.Windows.Forms.DialogResult.OK)
                        {
                            MidiWriter.Write(sfd.FileName, ddlInstruments.SelectedIndex, noteController.Recorder.Notes);
                            lblStatus.Text = "Midi saved to " + sfd.FileName;
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

        /// <summary>
        /// Move to next state
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btnStep_Click(object sender, EventArgs e)
        {
            MoveToNextState();
        }

        /// <summary>
        /// Open a saved state
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btnOpen_Click(object sender, EventArgs e)
        {
            mnuOpen_Click(mnuOpen, EventArgs.Empty);
        }

        /// <summary>
        /// Saves the current state & settings to a text file
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btnSave_Click(object sender, EventArgs e)
        {
            mnuSave_Click(mnuSave, EventArgs.Empty);
        }

        /// <summary>
        /// Repaint board when size changes
        /// </summary>
        /// <param name="e"></param>
        protected override void OnSizeChanged(EventArgs e)
        {
            UpdateBoardGUI();
            base.OnSizeChanged(e);
        }

        /// <summary>
        /// Show about box
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void mnuAbout_Click(object sender, EventArgs e)
        {
            using (AboutBox box = new AboutBox())
                box.ShowDialog(this);
        }

        /// <summary>
        /// Use the sample manager to play notes
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void rdbSample_CheckedChanged(object sender, EventArgs e)
        {
            noteController.UseSamples = rdbSample.Checked;
            BoardSettings.UseSamples = rdbSample.Checked;
        }

        /// <summary>
        /// Use the midi controller to play notes
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void rdbMidi_CheckedChanged(object sender, EventArgs e)
        {
            noteController.UseSamples = rdbSample.Checked;
            BoardSettings.UseSamples = rdbSample.Checked;
        }

        /// <summary>
        /// Change the sample on the sample manager
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void ddlSamples_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (ddlSamples.SelectedIndex >= 0 && noteController.SampleManager != null)
            {
                noteController.SampleManager.ChangeSample(ddlSamples.Items[ddlSamples.SelectedIndex].ToString());
                BoardSettings.Sample = ddlSamples.Items[ddlSamples.SelectedIndex].ToString();
            }
        }

        /// <summary>
        /// Reloads all the samples from the sample directory
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void mnuReloadSamples_Click(object sender, EventArgs e)
        {
            string currentSample;
            if (ddlSamples.SelectedIndex >= 0)
                currentSample = ddlSamples.Items[ddlSamples.SelectedIndex].ToString();
            else
                currentSample = "";

            FillSamples();

            if (!string.IsNullOrEmpty(currentSample))
                ddlSamples.SelectedIndex = ddlSamples.FindString(currentSample);
        }

        /// <summary>
        /// Imports an otomata url to the the board
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void mnuImportAutomataUrl_Click(object sender, EventArgs e)
        {
            using (ImportOtomataUrl dlg = new ImportOtomataUrl())
            {
                if (dlg.ShowDialog(this) == System.Windows.Forms.DialogResult.OK)
                {
                    ignoreNudRowColValueChange = true;
                    UpdateFromBoardSettings(dlg.BoardSettings);
                    lastPathOpened = "";
                    ignoreNudRowColValueChange = false;
                }
            }
        }

        /// <summary>
        /// Exports the current state of the board to an otomata url
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void mnuExportOtomataUrl_Click(object sender, EventArgs e)
        {
            string rowAndState = "qwertyuiopasdfghjklzxcvbnm0123456789";

            StringBuilder str = new StringBuilder();
            str.Append("http://earslap.com/projectslab/otomata?q=");

            var board = BoardSettings.Board;

            for (int row = 0; row < Math.Min(9, board.Rows); row++)
            {
                for (int col = 0; col < Math.Min(9, board.Cols); col++)
                {
                    if (board.Cells[col, row].State == CellStateEnum.Up ||
                       board.Cells[col, row].State == CellStateEnum.Right ||
                       board.Cells[col, row].State == CellStateEnum.Down ||
                       board.Cells[col, row].State == CellStateEnum.Left)
                    {
                        string colStr = col.ToString();
                        string rowStr = rowAndState[(row * 4 + (int)board.Cells[col, row].State)].ToString();
                        str.Append(colStr + rowStr);
                    }
                    else if (board.Cells[col, row].State == CellStateEnum.Merged)
                    {
                        string colStr = col.ToString();
                        foreach (CellStateEnum state in board.Cells[col, row].MergedStates)
                        {
                            string rowStr = rowAndState[(row * 4 + (int)state)].ToString();
                            str.Append(colStr + rowStr);
                        }
                    }
                }
            }

            try
            {
                Clipboard.Clear();
                Clipboard.SetText(str.ToString());

                MessageBox.Show("The url is copied onto the clipboard");
            }
            catch (Exception ex)
            {
                MessageBox.Show("Could not copy the url to the clipboard, error: " + ex.Message);
            }
        }

        public void UpdateBoardGUI()
        {
            try
            {
                if (picBoard.InvokeRequired)
                    this.BeginInvoke(new Action(() => picBoard.Invalidate()));
                else
                    picBoard.Invalidate();
            }
            catch (Exception)
            {
            }
        }

        private bool isForPlugin;

        public bool IsForPlugin
        {
            get { return isForPlugin; }
            set
            {
                isForPlugin = value;


                btnRun.Enabled = !isForPlugin;
                btnRecord.Enabled = !isForPlugin;
                rdbMidi.Enabled = !isForPlugin;
                rdbSample.Enabled = !isForPlugin;
                ddlMidiDevices.Enabled = !isForPlugin;
                ddlInstruments.Enabled = !isForPlugin;
                ddlSamples.Enabled = !isForPlugin;
                sldVolume.Enabled = !isForPlugin;
                mnuReloadSamples.Enabled = !isForPlugin;
                btnStep.Enabled = !isForPlugin;
            }
        }

        /* A test that didn't work so well
        private void btnImportMidi_Click(object sender, EventArgs e)
        {
            // test
            List<IEnumerable<string>> notesByBeat = new List<IEnumerable<string>>();
            notesByBeat.Add(new string[] { });
            notesByBeat.Add(new string[] { });
            notesByBeat.Add(new string[] { });
            notesByBeat.Add(new string[] { "C3" });
            notesByBeat.Add(new string[] { "C4" });
            notesByBeat.Add(new string[] { "A3" });
            notesByBeat.Add(new string[] { });


            long minDiff = long.MaxValue;
            OtomataBoard bestMatchingBoard = null;

            foreach (var notes in GetPermutationsOf(new HashSet<string>(notesByBeat.SelectMany(n => n)).ToList()))
            {
                string[] notesPerCell = notes.ToArray();

                for (int rowSize = notes.Count; rowSize < 100; rowSize += notes.Count)
                {
                    //for (int colSize = notes.Count; colSize < 100; colSize += notes.Count)
                    //{

                    OtomataBoard board = new OtomataBoard(rowSize, rowSize);

                    for (int tries = 0; tries < 100000; tries++)
                    {
                        RandomizeBoard(board);

                        var notesOfBoard = GetNotesOfBoard(notesByBeat.Count, notesPerCell, board);

                        long diff = DifferenceInNotesByBeat(notesByBeat, notesOfBoard);

                        this.board = board;
                        this.txtNotes.Text = string.Join(",", notes.ToArray());
                        ignoreNudRowColValueChange = true;
                        nudColumns.Value = rowSize;
                        nudRows.Value = rowSize;
                        ignoreNudRowColValueChange = false;
                        minDiff = diff;
                        picBoard.Invalidate();
                        Application.DoEvents();

                        if (diff == 0)
                        {
                            // found exact matching board!
                            bestMatchingBoard = board;
                            this.board = board;
                            this.txtNotes.Text = string.Join(",", notes.ToArray());
                            ignoreNudRowColValueChange = true;
                            nudColumns.Value = rowSize;
                            nudRows.Value = rowSize;
                            ignoreNudRowColValueChange = false;
                            minDiff = diff;
                            picBoard.Invalidate();
                            Application.DoEvents();
                            return;
                        }
                        else
                        {
                            if (minDiff > diff)
                            {
                                bestMatchingBoard = board;
                                this.board = board;
                                this.txtNotes.Text = string.Join(",", notes.ToArray());
                                ignoreNudRowColValueChange = true;
                                nudColumns.Value = rowSize;
                                nudRows.Value = rowSize;
                                ignoreNudRowColValueChange = false;
                                minDiff = diff;
                                picBoard.Invalidate();
                                Application.DoEvents();
                            }
                        }
                        //}
                    }
                }
            }

            //BoardSettings settings = new BoardSettings()
            //{
            //    Instrument = 0,
            //    NoteDuration = 500,
            //    Notes = note
            //};

        }

        private long DifferenceInNotesByBeat(List<IEnumerable<string>> notesByBeat1, List<IEnumerable<string>> notesByBeat2)
        {
            long diff = 0;
            for (int i = 0; i < Math.Min(notesByBeat1.Count, notesByBeat2.Count); i++)
            {
                diff += DifferenceInNotes(notesByBeat1[i], notesByBeat2[2]);
            }
            return diff;
        }

        private long DifferenceInNotes(IEnumerable<string> n1, IEnumerable<string> n2)
        {
            List<string> notes1;
            List<string> notes2;
            if (n1.Count() < n2.Count())
            {
                notes1 = n1.ToList();
                notes2 = n2.ToList();
            }
            else
            {
                notes1 = n2.ToList();
                notes2 = n1.ToList();
            }

            long diff = 0;
            if (notes1.Count < notes2.Count)
            {
                int i = 0;
                while (i < notes1.Count)
                {
                    diff += Math.Abs((NoteController.GetMidiIndexFromNote(notes1[i]) - NoteController.GetMidiIndexFromNote(notes2[i])));
                    i++;
                }
                diff += (notes2.Count - i) * 1000;
            }

            return diff;
        }

        private List<IEnumerable<string>> GetNotesOfBoard(int iterations, string[] notePerCell, AbstractBoard board)
        {
            List<IEnumerable<string>> notesByBeat = new List<IEnumerable<string>>();

            OtomataBoard.ActiveState[,] activeCells = board.ActiveCells;

            for (int i = 0; i < iterations; i++)
            {
                List<string> notesOfIteration = new List<string>();

                for (int row = 0; row < board.Rows; row++)
                {
                    for (int col = 0; col < board.Cols; col++)
                    {
                        if (activeCells[col, row] == AbstractBoard.ActiveState.ColumnActivated)
                        {
                            string note = notePerCell[col % notePerCell.Length].Replace("BB", "A#");
                            notesOfIteration.Add(note);
                        }
                        else if (activeCells[col, row] == AbstractBoard.ActiveState.RowActivated)
                        {
                            string note = notePerCell[row % notePerCell.Length].Replace("BB", "A#");
                            notesOfIteration.Add(note);
                        }
                    }
                }
                notesByBeat.Add(notesOfIteration);
            }

            return notesByBeat;
        }

        private IEnumerable<List<string>> GetPermutationsOf(List<string> notes)
        {
            if (notes.Count() == 1)
            {
                List<List<string>> lst = new List<List<string>>();
                lst.Add(notes);
                return lst;
            }
            else
            {
                List<List<string>> lst = new List<List<string>>();
                foreach (string note in notes)
                {
                    IEnumerable<List<string>> subperms = GetPermutationsOf(notes.Except(new string[] { note }).ToList());
                    foreach (var sub in subperms)
                    {
                        sub.Insert(0, note);
                        lst.Add(sub);
                    }
                }
                return lst;
            }
        }
        */
    }
}
