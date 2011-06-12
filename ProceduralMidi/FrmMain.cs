using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace ProceduralMidi
{
    /// <summary>
    /// Default winform host for the board editor
    /// </summary>
    public partial class FrmMain : Form
    {
        /// <summary>
        /// The board editor to dock
        /// </summary>
        private BoardEditor editor;

        /// <summary>
        /// Create a new host for the board editor
        /// </summary>
        public FrmMain()
        {
            InitializeComponent();

            editor = new BoardEditor();
            editor.Dock = DockStyle.Fill;
            editor.Close += new EventHandler(editor_Close);

            this.Controls.Add(editor);
        }

        /// <summary>
        /// Editor wants to close, close host
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        void editor_Close(object sender, EventArgs e)
        {
            this.Close();
        }
    }
}
