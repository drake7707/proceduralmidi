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
                BoardSettings = GetBoardFromUrl(txtUrl.Text);

                DialogResult = System.Windows.Forms.DialogResult.OK;
                this.Close();
            }
            catch (Exception)
            {
                MessageBox.Show("Unable to parse otomata url");
            }
        }

        private BoardSettings GetBoardFromUrl(string url)
        {
            string rowAndState = "qwertyuiopasdfghjklzxcvbnm0123456789";

            Uri uri = new Uri(url);

            string querystring = uri.Query.Replace("?q=", "");

            BoardSettings settings = new BoardSettings()
            {
                Instrument = 0,
                Sample = "hang.wav",
                UseSamples = true,
                NoteDuration = 3000,
                Notes = "D3, A3, A#3, C4, D4, E4, F4, A5, C5",
                Speed = 250
            };

            Cell[,] cells = new Cell[9, 9];

            for (int j = 0; j < 9; j++)
            {
                for (int i = 0; i < 9; i++)
                    cells[i, j] = new Cell(CellStateEnum.Dead);
            }

            for (int i = 0; i < querystring.Length; i+=2)
            {
                string cell = querystring.Substring(i, 2);

                int col = int.Parse(cell[0].ToString());
                int row = rowAndState.IndexOf(cell[1].ToString()) / 4;
                int state = rowAndState.IndexOf(cell[1].ToString()) % 4;

                if (cells[col, row] != null && cells[col, row].State != CellStateEnum.Dead)
                {
                    if (cells[col, row].State == CellStateEnum.Merged)
                        cells[col, row].MergedStates.Add((CellStateEnum)state);
                    else
                    {
                        CellStateEnum oldState = cells[col, row].State;
                        cells[col, row].State = CellStateEnum.Merged;
                        cells[col, row].MergedStates = new List<CellStateEnum>();
                        cells[col, row].MergedStates.Add(oldState);
                        cells[col, row].MergedStates.Add((CellStateEnum)state);
                    }
                }
                else
                {
                    cells[col, row] = new Cell((CellStateEnum)state);
                }
            }

            settings.Board = new OtomataBoard(9, 9, cells);

            return settings;
        }


    }
}
