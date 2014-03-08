namespace ProceduralMidi
{
    partial class FrmGenerateMidi
    {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

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
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(FrmGenerateMidi));
            this.btnGenerate = new System.Windows.Forms.Button();
            this.tableLayoutPanel1 = new System.Windows.Forms.TableLayoutPanel();
            this.rdbNumberOfSteps = new System.Windows.Forms.RadioButton();
            this.rdbNumberOfNotes = new System.Windows.Forms.RadioButton();
            this.rdbTimeElapsed = new System.Windows.Forms.RadioButton();
            this.nudNumberOfSteps = new System.Windows.Forms.NumericUpDown();
            this.nudNumberOfNotes = new System.Windows.Forms.NumericUpDown();
            this.nudTimeElapsed = new System.Windows.Forms.NumericUpDown();
            this.label1 = new System.Windows.Forms.Label();
            this.statusStrip1 = new System.Windows.Forms.StatusStrip();
            this.pbProgress = new System.Windows.Forms.ToolStripProgressBar();
            this.lblStatus = new System.Windows.Forms.ToolStripStatusLabel();
            this.tableLayoutPanel1.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.nudNumberOfSteps)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.nudNumberOfNotes)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.nudTimeElapsed)).BeginInit();
            this.statusStrip1.SuspendLayout();
            this.SuspendLayout();
            // 
            // btnGenerate
            // 
            this.btnGenerate.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.btnGenerate.Location = new System.Drawing.Point(360, 172);
            this.btnGenerate.Name = "btnGenerate";
            this.btnGenerate.Size = new System.Drawing.Size(75, 23);
            this.btnGenerate.TabIndex = 0;
            this.btnGenerate.Text = "Generate";
            this.btnGenerate.UseVisualStyleBackColor = true;
            this.btnGenerate.Click += new System.EventHandler(this.btnGenerate_Click);
            // 
            // tableLayoutPanel1
            // 
            this.tableLayoutPanel1.ColumnCount = 3;
            this.tableLayoutPanel1.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Absolute, 20F));
            this.tableLayoutPanel1.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Absolute, 179F));
            this.tableLayoutPanel1.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this.tableLayoutPanel1.Controls.Add(this.nudTimeElapsed, 2, 3);
            this.tableLayoutPanel1.Controls.Add(this.btnGenerate, 2, 5);
            this.tableLayoutPanel1.Controls.Add(this.nudNumberOfNotes, 2, 2);
            this.tableLayoutPanel1.Controls.Add(this.rdbNumberOfSteps, 1, 1);
            this.tableLayoutPanel1.Controls.Add(this.rdbNumberOfNotes, 1, 2);
            this.tableLayoutPanel1.Controls.Add(this.rdbTimeElapsed, 1, 3);
            this.tableLayoutPanel1.Controls.Add(this.nudNumberOfSteps, 2, 1);
            this.tableLayoutPanel1.Controls.Add(this.label1, 0, 0);
            this.tableLayoutPanel1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.tableLayoutPanel1.Location = new System.Drawing.Point(0, 0);
            this.tableLayoutPanel1.Name = "tableLayoutPanel1";
            this.tableLayoutPanel1.Padding = new System.Windows.Forms.Padding(3);
            this.tableLayoutPanel1.RowCount = 7;
            this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 29F));
            this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 29F));
            this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 29F));
            this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 29F));
            this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 45F));
            this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 34F));
            this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 82F));
            this.tableLayoutPanel1.Size = new System.Drawing.Size(441, 222);
            this.tableLayoutPanel1.TabIndex = 1;
            // 
            // rdbNumberOfSteps
            // 
            this.rdbNumberOfSteps.AutoSize = true;
            this.rdbNumberOfSteps.Dock = System.Windows.Forms.DockStyle.Fill;
            this.rdbNumberOfSteps.Location = new System.Drawing.Point(26, 35);
            this.rdbNumberOfSteps.Name = "rdbNumberOfSteps";
            this.rdbNumberOfSteps.Size = new System.Drawing.Size(173, 23);
            this.rdbNumberOfSteps.TabIndex = 0;
            this.rdbNumberOfSteps.TabStop = true;
            this.rdbNumberOfSteps.Text = "Number of steps";
            this.rdbNumberOfSteps.UseVisualStyleBackColor = true;
            this.rdbNumberOfSteps.CheckedChanged += new System.EventHandler(this.rdbNumberOfSteps_CheckedChanged);
            // 
            // rdbNumberOfNotes
            // 
            this.rdbNumberOfNotes.AutoSize = true;
            this.rdbNumberOfNotes.Dock = System.Windows.Forms.DockStyle.Fill;
            this.rdbNumberOfNotes.Location = new System.Drawing.Point(26, 64);
            this.rdbNumberOfNotes.Name = "rdbNumberOfNotes";
            this.rdbNumberOfNotes.Size = new System.Drawing.Size(173, 23);
            this.rdbNumberOfNotes.TabIndex = 1;
            this.rdbNumberOfNotes.TabStop = true;
            this.rdbNumberOfNotes.Text = "Number of notes";
            this.rdbNumberOfNotes.UseVisualStyleBackColor = true;
            this.rdbNumberOfNotes.CheckedChanged += new System.EventHandler(this.rdbNumberOfNotes_CheckedChanged);
            // 
            // rdbTimeElapsed
            // 
            this.rdbTimeElapsed.AutoSize = true;
            this.rdbTimeElapsed.Dock = System.Windows.Forms.DockStyle.Fill;
            this.rdbTimeElapsed.Location = new System.Drawing.Point(26, 93);
            this.rdbTimeElapsed.Name = "rdbTimeElapsed";
            this.rdbTimeElapsed.Size = new System.Drawing.Size(173, 23);
            this.rdbTimeElapsed.TabIndex = 2;
            this.rdbTimeElapsed.TabStop = true;
            this.rdbTimeElapsed.Text = "Time (in seconds) elapsed";
            this.rdbTimeElapsed.UseVisualStyleBackColor = true;
            this.rdbTimeElapsed.CheckedChanged += new System.EventHandler(this.rdbTimeElapsed_CheckedChanged);
            // 
            // nudNumberOfSteps
            // 
            this.nudNumberOfSteps.Anchor = System.Windows.Forms.AnchorStyles.Left;
            this.nudNumberOfSteps.Location = new System.Drawing.Point(205, 36);
            this.nudNumberOfSteps.Maximum = new decimal(new int[] {
            1000000,
            0,
            0,
            0});
            this.nudNumberOfSteps.Name = "nudNumberOfSteps";
            this.nudNumberOfSteps.Size = new System.Drawing.Size(230, 20);
            this.nudNumberOfSteps.TabIndex = 3;
            this.nudNumberOfSteps.Value = new decimal(new int[] {
            1000,
            0,
            0,
            0});
            this.nudNumberOfSteps.ValueChanged += new System.EventHandler(this.nudNumberOfSteps_ValueChanged);
            // 
            // nudNumberOfNotes
            // 
            this.nudNumberOfNotes.Anchor = System.Windows.Forms.AnchorStyles.Left;
            this.nudNumberOfNotes.Location = new System.Drawing.Point(205, 65);
            this.nudNumberOfNotes.Maximum = new decimal(new int[] {
            1000000,
            0,
            0,
            0});
            this.nudNumberOfNotes.Name = "nudNumberOfNotes";
            this.nudNumberOfNotes.Size = new System.Drawing.Size(230, 20);
            this.nudNumberOfNotes.TabIndex = 4;
            this.nudNumberOfNotes.Value = new decimal(new int[] {
            1000,
            0,
            0,
            0});
            // 
            // nudTimeElapsed
            // 
            this.nudTimeElapsed.Anchor = System.Windows.Forms.AnchorStyles.Left;
            this.nudTimeElapsed.Location = new System.Drawing.Point(205, 94);
            this.nudTimeElapsed.Maximum = new decimal(new int[] {
            10000000,
            0,
            0,
            0});
            this.nudTimeElapsed.Name = "nudTimeElapsed";
            this.nudTimeElapsed.Size = new System.Drawing.Size(230, 20);
            this.nudTimeElapsed.TabIndex = 5;
            this.nudTimeElapsed.Value = new decimal(new int[] {
            300,
            0,
            0,
            0});
            this.nudTimeElapsed.ValueChanged += new System.EventHandler(this.nudTimeElapsed_ValueChanged);
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.tableLayoutPanel1.SetColumnSpan(this.label1, 2);
            this.label1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.label1.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label1.Location = new System.Drawing.Point(6, 3);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(193, 29);
            this.label1.TabIndex = 6;
            this.label1.Text = "Choose output criteria";
            this.label1.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // statusStrip1
            // 
            this.statusStrip1.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.pbProgress,
            this.lblStatus});
            this.statusStrip1.Location = new System.Drawing.Point(0, 200);
            this.statusStrip1.Name = "statusStrip1";
            this.statusStrip1.Size = new System.Drawing.Size(441, 22);
            this.statusStrip1.SizingGrip = false;
            this.statusStrip1.TabIndex = 7;
            this.statusStrip1.Text = "statusStrip1";
            // 
            // pbProgress
            // 
            this.pbProgress.Name = "pbProgress";
            this.pbProgress.Size = new System.Drawing.Size(100, 16);
            this.pbProgress.Visible = false;
            // 
            // lblStatus
            // 
            this.lblStatus.Name = "lblStatus";
            this.lblStatus.Size = new System.Drawing.Size(0, 17);
            // 
            // FrmGenerateMidi
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(441, 222);
            this.Controls.Add(this.statusStrip1);
            this.Controls.Add(this.tableLayoutPanel1);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedSingle;
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.Name = "FrmGenerateMidi";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
            this.Text = "Generate Midi";
            this.tableLayoutPanel1.ResumeLayout(false);
            this.tableLayoutPanel1.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.nudNumberOfSteps)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.nudNumberOfNotes)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.nudTimeElapsed)).EndInit();
            this.statusStrip1.ResumeLayout(false);
            this.statusStrip1.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Button btnGenerate;
        private System.Windows.Forms.TableLayoutPanel tableLayoutPanel1;
        private System.Windows.Forms.NumericUpDown nudTimeElapsed;
        private System.Windows.Forms.NumericUpDown nudNumberOfNotes;
        private System.Windows.Forms.RadioButton rdbNumberOfSteps;
        private System.Windows.Forms.RadioButton rdbNumberOfNotes;
        private System.Windows.Forms.RadioButton rdbTimeElapsed;
        private System.Windows.Forms.NumericUpDown nudNumberOfSteps;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.StatusStrip statusStrip1;
        private System.Windows.Forms.ToolStripProgressBar pbProgress;
        private System.Windows.Forms.ToolStripStatusLabel lblStatus;
    }
}