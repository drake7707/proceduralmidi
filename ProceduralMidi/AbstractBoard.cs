using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ProceduralMidi
{

    /// <summary>
    /// Describes a board with cells
    /// </summary>
    public abstract class AbstractBoard
    {
        /// <summary>
        ///  The cells of the board
        /// </summary>
        public Cell[,] Cells { get; protected set; }

        /// <summary>
        /// The number of rows of the board
        /// </summary>
        public int Rows { get; protected set; }

        /// <summary>
        /// The number of columns of the board
        /// </summary>
        public int Cols { get; protected set; }

        /// <summary>
        /// Creates an empty board with specified dimension (all cells are dead)
        /// </summary>
        /// <param name="rows"></param>
        /// <param name="cols"></param>
        public AbstractBoard(int rows, int cols)
            : this(rows, cols, GetCleanState(rows, cols))
        { }

        /// <summary>
        /// Creates a board with specified dimensions and cell states
        /// </summary>
        /// <param name="rows"></param>
        /// <param name="cols"></param>
        /// <param name="cells"></param>
        public AbstractBoard(int rows, int cols, Cell[,] cells)
        {
            this.Rows = rows;
            this.Cols = cols;
            this.Cells = cells;
        }

        /// <summary>
        /// Returns a clean cell state (all cells are dead)
        /// </summary>
        /// <param name="rows"></param>
        /// <param name="cols"></param>
        /// <returns></returns>
        protected static Cell[,] GetCleanState(int rows, int cols)
        {
            Cell[,] cells = new Cell[cols, rows];
            for (int row = 0; row < rows; row++)
                for (int col = 0; col < cols; col++)
                    cells[col, row] = new Cell(CellStateEnum.Dead);

            return cells;
        }
        /// <summary>
        /// Clears the board (all cells die)
        /// </summary>
        public void Clear()
        {
            Cells = GetCleanState(Rows, Cols);
        }

        /// <summary>
        /// Change the board size to a new dimension
        /// </summary>
        /// <param name="rows"></param>
        /// <param name="cols"></param>
        public void ChangeSize(int rows, int cols)
        {
            // get new sized cell states
            Cell[,] newboard = GetCleanState(rows, cols);

            // determine upper bounds to iterate
            int minRows = Math.Min(this.Rows, rows);
            int minCols = Math.Min(this.Cols, cols);

            // copy cell states
            for (int row = 0; row < minRows; row++)
            {
                for (int col = 0; col < minCols; col++)
                    newboard[col, row] = Cells[col, row];
            }

            this.Rows = rows;
            this.Cols = cols;
            this.Cells = newboard;
        }

        /// <summary>
        /// Moves the board to the next state
        /// </summary>
        public abstract void NextState();

        /// <summary>
        /// Returns all the active cells of the current state
        /// </summary>
        public abstract ActiveState[,] ActiveCells { get; }

        /// <summary>
        /// Returns all possible states that the user can choose from the palette
        /// </summary>
        public abstract CellStateEnum[] PossibleStatesForPalette { get; }

        /// <summary>
        /// Describes the active state of a cell
        /// </summary>
        public enum ActiveState
        {
            NotActive,
            RowActivated,
            ColumnActivated,
            BothActivated
        }
    }
}
