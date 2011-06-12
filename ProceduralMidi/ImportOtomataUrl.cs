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
    /// Form where the user can put an otomata url
    /// </summary>
    public partial class ImportOtomataUrl : Form
    {
        public ImportOtomataUrl()
        {
            InitializeComponent();

            try
            {
                string text = Clipboard.GetText();
                if (text.Contains("http://www.earslap.com/projectslab/otomata?q="))
                    txtUrl.Text = text;
            }
            catch (Exception)
            {
            }
        }


        public BoardSettings BoardSettings { get; private set; }

        private void btnOK_Click(object sender, EventArgs e)
        {
            try
            {
                BoardSettings = BoardMapper.GetBoardFromOtomataUrl(txtUrl.Text);

                DialogResult = System.Windows.Forms.DialogResult.OK;
                this.Close();
            }
            catch (Exception)
            {
                MessageBox.Show("Unable to parse otomata url");
            }
        }

       


    }
}
