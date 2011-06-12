using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace ProceduralMidi.DAL
{
    /// <summary>
    /// Manages the loading & saving of a board and its settings
    /// </summary>
    public class BoardMapper
    {
        /// <summary>
        /// Saves the board and settings to a text file
        /// </summary>
        /// <param name="path"></param>
        /// <param name="boardsettings"></param>
        public static void Save(string path, BoardSettings boardsettings)
        {
            string str = GetStringFromBoardSettings(boardsettings);
            System.IO.File.WriteAllText(path, str);
        }

        /// <summary>
        /// Returns the string representation of the board settings
        /// </summary>
        /// <param name="boardsettings"></param>
        /// <returns></returns>
        internal static string GetStringFromBoardSettings(BoardSettings boardsettings)
        {
            AbstractBoard  board = boardsettings.Board;

            StringBuilder str = new StringBuilder();
            str.AppendLine("boardType=" + board.GetType().FullName);
            str.AppendLine("rows=" + board.Rows);
            str.AppendLine("cols=" + board.Cols);
            str.AppendLine("noteduration=" + boardsettings.NoteDuration);
            str.AppendLine("instrument=" + boardsettings.Instrument);
            str.AppendLine("speed=" + boardsettings.Speed);
            str.AppendLine("notes=" + boardsettings.Notes);
            str.AppendLine("usesamples=" + (boardsettings.UseSamples ? "1" : "0"));
            str.AppendLine("sample=" + boardsettings.Sample);

            str.AppendLine("[cells]");

            for (int row = 0; row < boardsettings.Board.Rows; row++)
            {
                StringBuilder strRow = new StringBuilder();
                for (int col = 0; col < board.Cols; col++)
                {
                    Cell c = board.Cells[col, row];
                    strRow.Append((int)c.State);
                    if (c.State == CellStateEnum.Merged)
                        strRow.Append("_" + string.Join("_", c.MergedStates.Select(s => ((int)s).ToString()).ToArray()));

                    if (col != board.Cols - 1)
                        strRow.Append(",");
                }

                str.AppendLine(strRow.ToString());
            }
            return str.ToString();
        }

        /// <summary>
        /// Try loading a board and settings from a text file
        /// </summary>
        /// <param name="path"></param>
        /// <param name="boardSettings"></param>
        /// <returns></returns>
        public static bool TryLoad(string path, out BoardSettings boardSettings)
        {
            
            string str = System.IO.File.ReadAllText(path);
            return TryGetBoardSettingsFromString(str, out boardSettings);
        }

        /// <summary>
        /// Try loading the board settings from the string
        /// </summary>
        /// <param name="str"></param>
        /// <param name="boardSettings"></param>
        /// <returns></returns>
        internal static bool TryGetBoardSettingsFromString(string str, out BoardSettings boardSettings)
        {
            boardSettings = null;
            string[] lines = str.Split(Environment.NewLine.ToCharArray()).Where(l => !string.IsNullOrEmpty(l)).ToArray();

            if (lines.Length <= 0)
            {
                MessageBox.Show("File is empty", "Invalid file");
                return false;
            }

            int rows = 0;
            int cols = 0;
            int noteduration = 0;
            string notes = "";
            int instrument = 0;
            int usesamples = 0;
            string sample = "";

            int speed = 0;
            Cell[,] cells = null;

            for (int i = 0; i < lines.Length; i++)
            {
                if (lines[i].Contains('='))
                {
                    string[] parts = lines[i].Split('=');
                    string propName = parts[0];
                    string propValue = parts[1];

                    if (propName.ToLower() == "rows")
                    {
                        if (!int.TryParse(propValue, out rows))
                        {
                            MessageBox.Show("Invalid nr of rows", "Invalid file");
                            return false;
                        }
                    }
                    else if (propName.ToLower() == "cols")
                    {
                        if (!int.TryParse(propValue, out cols))
                        {
                            MessageBox.Show("Invalid nr of cols", "Invalid file");
                            return false;
                        }
                    }
                    else if (propName.ToLower() == "noteduration")
                    {
                        if (!int.TryParse(propValue, out noteduration))
                        {
                            MessageBox.Show("Invalid noteduration", "Invalid file");
                            return false;
                        }
                    }
                    else if (propName.ToLower() == "instrument")
                    {
                        if (!int.TryParse(propValue, out instrument))
                        {
                            MessageBox.Show("Invalid instrument", "Invalid file");
                            return false;
                        }
                    }
                    else if (propName.ToLower() == "usesamples")
                    {
                        if (!int.TryParse(propValue, out usesamples))
                        {
                            MessageBox.Show("Invalid property value for 'Use samples'", "Invalid file");
                            return false;
                        }
                    }
                    else if (propName.ToLower() == "sample")
                    {
                        sample = propValue;
                    }
                    else if (propName.ToLower() == "speed")
                    {
                        if (!int.TryParse(propValue, out speed))
                        {
                            MessageBox.Show("Invalid speed", "Invalid file");
                            return false;
                        }
                    }
                    else if (propName.ToLower() == "notes")
                    {
                        notes = propValue;
                    }
                }
                else if (lines[i].Trim() == "[cells]")
                {

                    cells = new Cell[cols, rows];

                    i++;
                    int curRow = 0;
                    while (curRow < rows)
                    {
                        string cellLine = lines[i];

                        string[] cellPart = cellLine.Split(',');


                        for (int col = 0; col < cellPart.Length; col++)
                        {
                            int cellState;
                            List<int> mergedStates = new List<int>();

                            if (cellPart[col].Contains("_"))
                            {
                                string[] cellSubParts = cellPart[col].Split('_');
                                if (!int.TryParse(cellSubParts[0], out cellState))
                                {
                                    MessageBox.Show("Invalid cell state at (" + col + "," + curRow + ")", "Invalid file");
                                    return false;
                                }
                                for (int mergedIdx = 1; mergedIdx < cellSubParts.Length; mergedIdx++)
                                {
                                    int mergedState;
                                    if (!int.TryParse(cellSubParts[mergedIdx], out mergedState))
                                    {
                                        MessageBox.Show("Invalid merged state of cell at (" + col + "," + curRow + ")", "Invalid file");
                                        return false;
                                    }
                                    mergedStates.Add(mergedState);
                                }
                            }
                            else
                            {
                                if (!int.TryParse(cellPart[col], out cellState))
                                {
                                    MessageBox.Show("Invalid cell state at (" + col + "," + curRow + ")", "Invalid file");
                                    return false;
                                }
                                mergedStates.Add(cellState);
                            }

                            cells[col, curRow] = new Cell((CellStateEnum)cellState);
                            cells[col, curRow].MergedStates = new List<CellStateEnum>(mergedStates.Select(s => (CellStateEnum)s));
                        }

                        curRow++;
                        i++;
                    }
                }
            }

            boardSettings = new BoardSettings()
            {
                Board = new OtomataBoard(rows, cols, cells),
                NoteDuration = noteduration,
                Speed = speed,
                Notes = notes,
                Instrument = instrument,
                UseSamples = (usesamples != 0 ? true : false),
                Sample = sample,
                Volume = 64 // default volume, TODO save in file too ?
            };
            return true;
        }

        /// <summary>
        /// Generates a board from the given otomata url
        /// </summary>
        /// <param name="url"></param>
        /// <returns></returns>
        public static BoardSettings GetBoardFromOtomataUrl(string url)
        {
            // format of querystring is as follows:
            // <col><row&state><col><row&state>
            // with each <col><row&state> for an active cell
            // if cells are merged, then that pair will be in the querystring multiple times
            // to determine the row and state seperately
            // the rowAndState string below has to be split in chunks of 4 characters
            // the chunk index is the row, the character index in that chunk is the state 
            string rowAndState = "qwertyuiopasdfghjklzxcvbnm0123456789";

            Uri uri = new Uri(url);

            string querystring = uri.Query.Replace("?q=", "");

            // create settings that are similar to the otomata settings
            BoardSettings settings = new BoardSettings()
            {
                Instrument = 0,
                Sample = "hang.wav",
                UseSamples = true,
                NoteDuration = 3000,
                Notes = "D3, A3, A#3, C4, D4, E4, F4, A5, C5",
                Speed = 250
            };

            // create cell array
            Cell[,] cells = new Cell[9, 9];
            // fill default cells with all dead
            for (int j = 0; j < 9; j++)
            {
                for (int i = 0; i < 9; i++)
                    cells[i, j] = new Cell(CellStateEnum.Dead);
            }

            // foreach pair
            for (int i = 0; i < querystring.Length; i += 2)
            {
                // <col><row&state>
                string cell = querystring.Substring(i, 2);

                // parse col
                int col = int.Parse(cell[0].ToString());

                // parse row & state
                int row = rowAndState.IndexOf(cell[1].ToString()) / 4;
                int state = rowAndState.IndexOf(cell[1].ToString()) % 4;

                // add the cell to the board, if there is already a cell present, make the cell merged
                // and add it to the merged cells list
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

        /// <summary>
        /// Creates an otomata url based on the board state
        /// </summary>
        /// <param name="settings"></param>
        /// <returns></returns>
        public static string ExportOtomataUrl(BoardSettings settings)
        {
            string rowAndState = "qwertyuiopasdfghjklzxcvbnm0123456789";

            StringBuilder str = new StringBuilder();
            str.Append("http://earslap.com/projectslab/otomata?q=");

            var board = settings.Board;

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
            return str.ToString();
        }
    }
}
