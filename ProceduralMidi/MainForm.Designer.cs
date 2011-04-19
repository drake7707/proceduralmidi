namespace ProceduralMidi
{
    partial class MainForm
    {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

     
        #region Windows Form Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.components = new System.ComponentModel.Container();
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(MainForm));
            this.tmrIterate = new System.Windows.Forms.Timer(this.components);
            this.chkRun = new System.Windows.Forms.CheckBox();
            this.picBoard = new System.Windows.Forms.PictureBox();
            this.btnNext = new System.Windows.Forms.Button();
            this.btnClean = new System.Windows.Forms.Button();
            this.lblDebug = new System.Windows.Forms.Label();
            this.tmrDrawHighlights = new System.Windows.Forms.Timer(this.components);
            this.groupBox1 = new System.Windows.Forms.GroupBox();
            this.ddlMidiDevices = new System.Windows.Forms.ComboBox();
            this.sldNoteDuration = new System.Windows.Forms.TrackBar();
            this.lblNoteDuration = new System.Windows.Forms.Label();
            this.label4 = new System.Windows.Forms.Label();
            this.label3 = new System.Windows.Forms.Label();
            this.nudColumns = new System.Windows.Forms.NumericUpDown();
            this.nudRows = new System.Windows.Forms.NumericUpDown();
            this.sldSpeed = new System.Windows.Forms.TrackBar();
            this.lblSpeed = new System.Windows.Forms.Label();
            this.label2 = new System.Windows.Forms.Label();
            this.sldVolume = new System.Windows.Forms.TrackBar();
            this.ddlInstruments = new System.Windows.Forms.ComboBox();
            this.label1 = new System.Windows.Forms.Label();
            this.txtNotes = new System.Windows.Forms.TextBox();
            this.label5 = new System.Windows.Forms.Label();
            this.menuStrip1 = new System.Windows.Forms.MenuStrip();
            this.fileToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.mnuOpen = new System.Windows.Forms.ToolStripMenuItem();
            this.mnuReload = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripSeparator = new System.Windows.Forms.ToolStripSeparator();
            this.mnuSave = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripSeparator2 = new System.Windows.Forms.ToolStripSeparator();
            this.mnuExit = new System.Windows.Forms.ToolStripMenuItem();
            ((System.ComponentModel.ISupportInitialize)(this.picBoard)).BeginInit();
            this.groupBox1.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.sldNoteDuration)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.nudColumns)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.nudRows)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.sldSpeed)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.sldVolume)).BeginInit();
            this.menuStrip1.SuspendLayout();
            this.SuspendLayout();
            // 
            // tmrIterate
            // 
            this.tmrIterate.Interval = 250;
            this.tmrIterate.Tick += new System.EventHandler(this.tmrIterate_Tick);
            // 
            // chkRun
            // 
            this.chkRun.Appearance = System.Windows.Forms.Appearance.Button;
            this.chkRun.Location = new System.Drawing.Point(6, 20);
            this.chkRun.Name = "chkRun";
            this.chkRun.Size = new System.Drawing.Size(91, 23);
            this.chkRun.TabIndex = 1;
            this.chkRun.Text = "Run (F5)";
            this.chkRun.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            this.chkRun.UseVisualStyleBackColor = true;
            this.chkRun.CheckedChanged += new System.EventHandler(this.chkRun_CheckedChanged);
            // 
            // picBoard
            // 
            this.picBoard.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom)
                        | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.picBoard.BackColor = System.Drawing.Color.Black;
            this.picBoard.Location = new System.Drawing.Point(2, 27);
            this.picBoard.Name = "picBoard";
            this.picBoard.Size = new System.Drawing.Size(472, 437);
            this.picBoard.TabIndex = 2;
            this.picBoard.TabStop = false;
            this.picBoard.Paint += new System.Windows.Forms.PaintEventHandler(this.picBoard_Paint);
            this.picBoard.MouseDown += new System.Windows.Forms.MouseEventHandler(this.picBoard_MouseDown);
            // 
            // btnNext
            // 
            this.btnNext.Location = new System.Drawing.Point(6, 48);
            this.btnNext.Name = "btnNext";
            this.btnNext.Size = new System.Drawing.Size(91, 23);
            this.btnNext.TabIndex = 3;
            this.btnNext.Text = "Next state (F6)";
            this.btnNext.UseVisualStyleBackColor = true;
            this.btnNext.Click += new System.EventHandler(this.btnNext_Click);
            // 
            // btnClean
            // 
            this.btnClean.Location = new System.Drawing.Point(103, 20);
            this.btnClean.Name = "btnClean";
            this.btnClean.Size = new System.Drawing.Size(71, 23);
            this.btnClean.TabIndex = 4;
            this.btnClean.Text = "Clean";
            this.btnClean.UseVisualStyleBackColor = true;
            this.btnClean.Click += new System.EventHandler(this.btnClean_Click);
            // 
            // lblDebug
            // 
            this.lblDebug.Location = new System.Drawing.Point(9, 367);
            this.lblDebug.Name = "lblDebug";
            this.lblDebug.Size = new System.Drawing.Size(161, 67);
            this.lblDebug.TabIndex = 5;
            // 
            // tmrDrawHighlights
            // 
            this.tmrDrawHighlights.Enabled = true;
            this.tmrDrawHighlights.Interval = 25;
            this.tmrDrawHighlights.Tick += new System.EventHandler(this.tmrDrawHighlights_Tick);
            // 
            // groupBox1
            // 
            this.groupBox1.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.groupBox1.Controls.Add(this.lblDebug);
            this.groupBox1.Controls.Add(this.lblSpeed);
            this.groupBox1.Controls.Add(this.ddlMidiDevices);
            this.groupBox1.Controls.Add(this.sldNoteDuration);
            this.groupBox1.Controls.Add(this.lblNoteDuration);
            this.groupBox1.Controls.Add(this.label4);
            this.groupBox1.Controls.Add(this.label3);
            this.groupBox1.Controls.Add(this.nudColumns);
            this.groupBox1.Controls.Add(this.nudRows);
            this.groupBox1.Controls.Add(this.sldSpeed);
            this.groupBox1.Controls.Add(this.label2);
            this.groupBox1.Controls.Add(this.sldVolume);
            this.groupBox1.Controls.Add(this.ddlInstruments);
            this.groupBox1.Controls.Add(this.label1);
            this.groupBox1.Controls.Add(this.btnNext);
            this.groupBox1.Controls.Add(this.chkRun);
            this.groupBox1.Controls.Add(this.btnClean);
            this.groupBox1.Location = new System.Drawing.Point(480, 27);
            this.groupBox1.Name = "groupBox1";
            this.groupBox1.Size = new System.Drawing.Size(180, 437);
            this.groupBox1.TabIndex = 6;
            this.groupBox1.TabStop = false;
            // 
            // ddlMidiDevices
            // 
            this.ddlMidiDevices.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.ddlMidiDevices.FormattingEnabled = true;
            this.ddlMidiDevices.Location = new System.Drawing.Point(12, 126);
            this.ddlMidiDevices.Name = "ddlMidiDevices";
            this.ddlMidiDevices.Size = new System.Drawing.Size(155, 21);
            this.ddlMidiDevices.TabIndex = 18;
            this.ddlMidiDevices.SelectedIndexChanged += new System.EventHandler(this.ddlMidiDevices_SelectedIndexChanged);
            // 
            // sldNoteDuration
            // 
            this.sldNoteDuration.Location = new System.Drawing.Point(15, 319);
            this.sldNoteDuration.Maximum = 3000;
            this.sldNoteDuration.Minimum = 25;
            this.sldNoteDuration.Name = "sldNoteDuration";
            this.sldNoteDuration.Size = new System.Drawing.Size(155, 45);
            this.sldNoteDuration.TabIndex = 17;
            this.sldNoteDuration.TickFrequency = 100;
            this.sldNoteDuration.Value = 250;
            this.sldNoteDuration.ValueChanged += new System.EventHandler(this.sldNoteDuration_ValueChanged);
            // 
            // lblNoteDuration
            // 
            this.lblNoteDuration.AutoSize = true;
            this.lblNoteDuration.Location = new System.Drawing.Point(9, 303);
            this.lblNoteDuration.Name = "lblNoteDuration";
            this.lblNoteDuration.Size = new System.Drawing.Size(167, 13);
            this.lblNoteDuration.TabIndex = 16;
            this.lblNoteDuration.Text = "Note duration (in ms, current=500)";
            // 
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.Location = new System.Drawing.Point(89, 252);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(47, 13);
            this.label4.TabIndex = 15;
            this.label4.Text = "Columns";
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(12, 252);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(34, 13);
            this.label3.TabIndex = 14;
            this.label3.Text = "Rows";
            // 
            // nudColumns
            // 
            this.nudColumns.Location = new System.Drawing.Point(92, 271);
            this.nudColumns.Minimum = new decimal(new int[] {
            2,
            0,
            0,
            0});
            this.nudColumns.Name = "nudColumns";
            this.nudColumns.Size = new System.Drawing.Size(69, 20);
            this.nudColumns.TabIndex = 13;
            this.nudColumns.Value = new decimal(new int[] {
            9,
            0,
            0,
            0});
            this.nudColumns.ValueChanged += new System.EventHandler(this.nudColumns_ValueChanged);
            // 
            // nudRows
            // 
            this.nudRows.Location = new System.Drawing.Point(12, 271);
            this.nudRows.Minimum = new decimal(new int[] {
            2,
            0,
            0,
            0});
            this.nudRows.Name = "nudRows";
            this.nudRows.Size = new System.Drawing.Size(69, 20);
            this.nudRows.TabIndex = 12;
            this.nudRows.Value = new decimal(new int[] {
            9,
            0,
            0,
            0});
            this.nudRows.ValueChanged += new System.EventHandler(this.nudRows_ValueChanged);
            // 
            // sldSpeed
            // 
            this.sldSpeed.Location = new System.Drawing.Point(12, 220);
            this.sldSpeed.Maximum = 1000;
            this.sldSpeed.Minimum = 25;
            this.sldSpeed.Name = "sldSpeed";
            this.sldSpeed.Size = new System.Drawing.Size(155, 45);
            this.sldSpeed.TabIndex = 11;
            this.sldSpeed.TickFrequency = 100;
            this.sldSpeed.Value = 250;
            this.sldSpeed.ValueChanged += new System.EventHandler(this.sldSpeed_ValueChanged);
            // 
            // lblSpeed
            // 
            this.lblSpeed.AutoSize = true;
            this.lblSpeed.Location = new System.Drawing.Point(9, 201);
            this.lblSpeed.Name = "lblSpeed";
            this.lblSpeed.Size = new System.Drawing.Size(134, 13);
            this.lblSpeed.TabIndex = 10;
            this.lblSpeed.Text = "Speed (in ms, current=250)";
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(9, 150);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(42, 13);
            this.label2.TabIndex = 9;
            this.label2.Text = "Volume";
            // 
            // sldVolume
            // 
            this.sldVolume.Location = new System.Drawing.Point(12, 169);
            this.sldVolume.Maximum = 127;
            this.sldVolume.Name = "sldVolume";
            this.sldVolume.Size = new System.Drawing.Size(155, 45);
            this.sldVolume.TabIndex = 8;
            this.sldVolume.TickFrequency = 12;
            this.sldVolume.Value = 64;
            // 
            // ddlInstruments
            // 
            this.ddlInstruments.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.ddlInstruments.FormattingEnabled = true;
            this.ddlInstruments.Location = new System.Drawing.Point(12, 106);
            this.ddlInstruments.Name = "ddlInstruments";
            this.ddlInstruments.Size = new System.Drawing.Size(155, 21);
            this.ddlInstruments.TabIndex = 7;
            this.ddlInstruments.SelectedIndexChanged += new System.EventHandler(this.ddlInstrument_SelectedIndexChanged);
            // 
            // label1
            // 
            this.label1.Location = new System.Drawing.Point(9, 74);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(158, 42);
            this.label1.TabIndex = 6;
            this.label1.Text = "Click left to change cell state, right to see debug info of cell";
            // 
            // txtNotes
            // 
            this.txtNotes.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.txtNotes.Location = new System.Drawing.Point(2, 483);
            this.txtNotes.Name = "txtNotes";
            this.txtNotes.Size = new System.Drawing.Size(658, 20);
            this.txtNotes.TabIndex = 7;
            this.txtNotes.Text = "D3, A3, A#3, C4, D4, E4, F4, A5, C5";
            this.txtNotes.TextChanged += new System.EventHandler(this.txtNotes_TextChanged);
            // 
            // label5
            // 
            this.label5.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.label5.AutoSize = true;
            this.label5.Location = new System.Drawing.Point(9, 467);
            this.label5.Name = "label5";
            this.label5.Size = new System.Drawing.Size(466, 13);
            this.label5.TabIndex = 15;
            this.label5.Text = "Notes per cell (seperated by \',\' ). If there are not enough notes the specified n" +
                "otes will be wrapped";
            // 
            // menuStrip1
            // 
            this.menuStrip1.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.fileToolStripMenuItem});
            this.menuStrip1.Location = new System.Drawing.Point(0, 0);
            this.menuStrip1.Name = "menuStrip1";
            this.menuStrip1.Size = new System.Drawing.Size(665, 24);
            this.menuStrip1.TabIndex = 16;
            this.menuStrip1.Text = "menuStrip1";
            // 
            // fileToolStripMenuItem
            // 
            this.fileToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.mnuOpen,
            this.mnuReload,
            this.toolStripSeparator,
            this.mnuSave,
            this.toolStripSeparator2,
            this.mnuExit});
            this.fileToolStripMenuItem.Name = "fileToolStripMenuItem";
            this.fileToolStripMenuItem.Size = new System.Drawing.Size(37, 20);
            this.fileToolStripMenuItem.Text = "&File";
            // 
            // mnuOpen
            // 
            this.mnuOpen.Image = ((System.Drawing.Image)(resources.GetObject("mnuOpen.Image")));
            this.mnuOpen.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.mnuOpen.Name = "mnuOpen";
            this.mnuOpen.ShortcutKeys = ((System.Windows.Forms.Keys)((System.Windows.Forms.Keys.Control | System.Windows.Forms.Keys.O)));
            this.mnuOpen.Size = new System.Drawing.Size(146, 22);
            this.mnuOpen.Text = "&Open";
            this.mnuOpen.Click += new System.EventHandler(this.mnuOpen_Click);
            // 
            // mnuReload
            // 
            this.mnuReload.Name = "mnuReload";
            this.mnuReload.ShortcutKeys = System.Windows.Forms.Keys.F2;
            this.mnuReload.Size = new System.Drawing.Size(146, 22);
            this.mnuReload.Text = "Reload";
            this.mnuReload.Click += new System.EventHandler(this.mnuReload_Click);
            // 
            // toolStripSeparator
            // 
            this.toolStripSeparator.Name = "toolStripSeparator";
            this.toolStripSeparator.Size = new System.Drawing.Size(143, 6);
            // 
            // mnuSave
            // 
            this.mnuSave.Image = ((System.Drawing.Image)(resources.GetObject("mnuSave.Image")));
            this.mnuSave.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.mnuSave.Name = "mnuSave";
            this.mnuSave.ShortcutKeys = ((System.Windows.Forms.Keys)((System.Windows.Forms.Keys.Control | System.Windows.Forms.Keys.S)));
            this.mnuSave.Size = new System.Drawing.Size(146, 22);
            this.mnuSave.Text = "&Save";
            this.mnuSave.Click += new System.EventHandler(this.mnuSave_Click);
            // 
            // toolStripSeparator2
            // 
            this.toolStripSeparator2.Name = "toolStripSeparator2";
            this.toolStripSeparator2.Size = new System.Drawing.Size(143, 6);
            // 
            // mnuExit
            // 
            this.mnuExit.Name = "mnuExit";
            this.mnuExit.Size = new System.Drawing.Size(146, 22);
            this.mnuExit.Text = "E&xit";
            this.mnuExit.Click += new System.EventHandler(this.mnuExit_Click);
            // 
            // Form1
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(665, 511);
            this.Controls.Add(this.label5);
            this.Controls.Add(this.txtNotes);
            this.Controls.Add(this.groupBox1);
            this.Controls.Add(this.picBoard);
            this.Controls.Add(this.menuStrip1);
            this.MainMenuStrip = this.menuStrip1;
            this.Name = "Form1";
            this.Text = "Procedural Midi";
            ((System.ComponentModel.ISupportInitialize)(this.picBoard)).EndInit();
            this.groupBox1.ResumeLayout(false);
            this.groupBox1.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.sldNoteDuration)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.nudColumns)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.nudRows)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.sldSpeed)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.sldVolume)).EndInit();
            this.menuStrip1.ResumeLayout(false);
            this.menuStrip1.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Timer tmrIterate;
        private System.Windows.Forms.CheckBox chkRun;
        private System.Windows.Forms.PictureBox picBoard;
        private System.Windows.Forms.Button btnNext;
        private System.Windows.Forms.Button btnClean;
        private System.Windows.Forms.Label lblDebug;
        private System.Windows.Forms.Timer tmrDrawHighlights;
        private System.Windows.Forms.GroupBox groupBox1;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.ComboBox ddlInstruments;
        private System.Windows.Forms.TrackBar sldVolume;
        private System.Windows.Forms.TrackBar sldSpeed;
        private System.Windows.Forms.Label lblSpeed;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.NumericUpDown nudColumns;
        private System.Windows.Forms.NumericUpDown nudRows;
        private System.Windows.Forms.TextBox txtNotes;
        private System.Windows.Forms.Label label5;
        private System.Windows.Forms.Label lblNoteDuration;
        private System.Windows.Forms.TrackBar sldNoteDuration;
        private System.Windows.Forms.MenuStrip menuStrip1;
        private System.Windows.Forms.ToolStripMenuItem fileToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem mnuOpen;
        private System.Windows.Forms.ToolStripSeparator toolStripSeparator;
        private System.Windows.Forms.ToolStripMenuItem mnuSave;
        private System.Windows.Forms.ToolStripSeparator toolStripSeparator2;
        private System.Windows.Forms.ToolStripMenuItem mnuExit;
        private System.Windows.Forms.ToolStripMenuItem mnuReload;
        private System.Windows.Forms.ComboBox ddlMidiDevices;
    }
}

