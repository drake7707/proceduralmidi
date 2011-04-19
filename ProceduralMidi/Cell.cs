using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ProceduralMidi
{
    /// <summary>
    /// Describes a cell on the board
    /// </summary>
    [Serializable]
    public struct Cell
    {
        public Cell(CellStateEnum state)
        {
            State = state;
            MergedStates = new HashSet<CellStateEnum>();
        }

        /// <summary>
        /// The current cell state
        /// </summary>
        public CellStateEnum State;

        /// <summary>
        /// The merged states of the cell
        /// </summary>
        public HashSet<CellStateEnum> MergedStates;
    }

    public enum CellStateEnum
    {
        Up,
        Right,
        Down,
        Left,
        Merged,
        Dead
    }
}
