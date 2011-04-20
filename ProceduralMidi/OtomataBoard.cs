using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ProceduralMidi
{
    [Serializable]
    public class OtomataBoard : AbstractBoard
    {
        public OtomataBoard(int rows, int cols)
            : base(rows, cols)
        { }

        public OtomataBoard(int rows, int cols, Cell[,] cells)
            : base(rows, cols, cells)
        { }

        /// <summary>
        /// Moves the board to a next state:
        ///  - If cell is not at boundary it will advance in the direction of its cell state
        ///  - Otherwise it will bounce off and gets the opposite direction as cell state
        ///  - Cells can merge, the merged cell state becomes merges and contains all the new states of the old cells
        /// </summary>
        public override void NextState()
        {
            Cell[,] newCellsState = GetCleanState(Rows, Cols);

            for (int row = 0; row < Rows; row++)
            {
                for (int col = 0; col < Cols; col++)
                {
                    Cell currentCell = Cells[col, row];

                    // move cell 1 to left
                    if (currentCell.State == CellStateEnum.Left)
                    {
                        if (col - 1 >= 0)
                            SetState(ref newCellsState[col - 1, row], CellStateEnum.Left);
                        else // out bounds, let it bounce to the other side
                            SetState(ref newCellsState[col + 1, row], CellStateEnum.Right);
                    }
                    // move cell 1 to right
                    else if (currentCell.State == CellStateEnum.Right)
                    {
                        if (col + 1 < Cols)
                            SetState(ref newCellsState[col + 1, row], CellStateEnum.Right);
                        else // out bounds, let it bounce to the other side
                            SetState(ref newCellsState[col - 1, row], CellStateEnum.Left);
                    }
                    // move cell 1 to top
                    else if (currentCell.State == CellStateEnum.Up)
                    {
                        if (row - 1 >= 0)
                            SetState(ref newCellsState[col, row - 1], CellStateEnum.Up);
                        else // out bounds, let it bounce to the other side
                            SetState(ref newCellsState[col, row + 1], CellStateEnum.Down);
                    }
                    // move cell 1 to bottom
                    else if (currentCell.State == CellStateEnum.Down)
                    {
                        if (row + 1 < Rows)
                            SetState(ref newCellsState[col, row + 1], CellStateEnum.Down);
                        else // out bounds, let it bounce to the other side
                            SetState(ref newCellsState[col, row - 1], CellStateEnum.Up);
                    }
                    else if (currentCell.State == CellStateEnum.Merged)
                    {
                        foreach (var mergedState in currentCell.MergedStates)
                        {
                            if (mergedState == CellStateEnum.Left)
                            {
                                if (col - 1 >= 0)
                                    SetState(ref newCellsState[col - 1, row], CellStateEnum.Left);
                                else // out bounds, let it bounce to the other side
                                    SetState(ref newCellsState[col + 1, row], CellStateEnum.Right);
                            }
                            else if (mergedState == CellStateEnum.Right)
                            {
                                if (col + 1 < Cols)
                                    SetState(ref newCellsState[col + 1, row], CellStateEnum.Right);
                                else // out bounds, let it bounce to the other side
                                    SetState(ref newCellsState[col - 1, row], CellStateEnum.Left);
                            }
                            else if (mergedState == CellStateEnum.Up)
                            {
                                if (row - 1 >= 0)
                                    SetState(ref newCellsState[col, row - 1], CellStateEnum.Up);
                                else // out bounds, let it bounce to the other side
                                    SetState(ref newCellsState[col, row + 1], CellStateEnum.Down);
                            }
                            else if (mergedState == CellStateEnum.Down)
                            {
                                if (row + 1 < Rows)
                                    SetState(ref newCellsState[col, row + 1], CellStateEnum.Down);
                                else // out bounds, let it bounce to the other side
                                    SetState(ref newCellsState[col, row - 1], CellStateEnum.Up);
                            }
                        }
                    }
                }
            }

            // rotate all merged cells, merged states contain the new states of the to-spawn cells
            for (int row = 0; row < Rows; row++)
            {
                for (int col = 0; col < Cols; col++)
                {
                    for (int rotateCount = 0; rotateCount < newCellsState[col, row].MergedStates.Count - 1; rotateCount++)
                        newCellsState[col, row].MergedStates = newCellsState[col, row].MergedStates.ToList().Select(s => RotateClockWise(s)).ToList();
                }
            }

            Cells = newCellsState;
        }

        #region  OLD rule code
        // making it unnecessary complicated by iterating on the new cells and looking back on the old
        // rather than the other way around

        //public override void NextState()
        //{
        //    Cell[,] newCellsState = GetCleanState(Rows, Cols);

        //    for (int row = 0; row < Rows; row++)
        //    {
        //        for (int col = 0; col < Cols; col++)
        //        {
        //            // check cell left of current if state is right
        //            if (col - 1 >= 0 && Cells[col - 1, row].State == CellStateEnum.Right)
        //                SetState(ref newCellsState[col, row], CellStateEnum.Right);

        //            // check cell left of current if state is merged and contains up (clockwise turn = right)
        //            if (col - 1 >= 0 && CellIsMergedAndContainsState(Cells[col - 1, row], CellStateEnum.Up))
        //                SetState(ref newCellsState[col, row], CellStateEnum.Right);

        //            // check cell right of current if state is left
        //            if (col + 1 < Cols && Cells[col + 1, row].State == CellStateEnum.Left)
        //                SetState(ref newCellsState[col, row], CellStateEnum.Left);

        //            // check cell right of current if state is merged and contains down (clockwise turn = left)
        //            if (col + 1 < Cols && CellIsMergedAndContainsState(Cells[col + 1, row], CellStateEnum.Down))
        //                SetState(ref newCellsState[col, row], CellStateEnum.Left);

        //            // check if old cell was near left boundary and state was left (bouncy bouncy)
        //            if (col == 0 && Cells[col, row].State == CellStateEnum.Left)
        //                SetState(ref newCellsState[col + 1, row], CellStateEnum.Right);

        //            // check if old cell was near right boundary and state was right (bouncy bouncy)
        //            if (col == Cols - 1 && Cells[col, row].State == CellStateEnum.Right)
        //                SetState(ref newCellsState[col - 1, row], CellStateEnum.Left);




        //            // check cell up of current if state is down
        //            if (row - 1 >= 0 && Cells[col, row - 1].State == CellStateEnum.Down)
        //                SetState(ref newCellsState[col, row], CellStateEnum.Down);

        //            // check cell up of current if state is merged and contains right (clockwise turn = down)
        //            if (row - 1 >= 0 && CellIsMergedAndContainsState(Cells[col, row - 1], CellStateEnum.Right))
        //                SetState(ref newCellsState[col, row], CellStateEnum.Down);

        //            // check cell down of current if state is up
        //            if (row + 1 < Rows && Cells[col, row + 1].State == CellStateEnum.Up)
        //                SetState(ref newCellsState[col, row], CellStateEnum.Up);

        //            // check cell down of current if state is merged and contains left (clockwise turn = up)
        //            if (row + 1 < Rows && CellIsMergedAndContainsState(Cells[col, row + 1], CellStateEnum.Left))
        //                SetState(ref newCellsState[col, row], CellStateEnum.Up);

        //            // check if old cell was near top boundary and state was up (bouncy bouncy)
        //            if (row == 0 && Cells[col, row].State == CellStateEnum.Up)
        //                SetState(ref newCellsState[col, row + 1], CellStateEnum.Down);

        //            // check if old cell was near bottom boundary and state was down (bouncy bouncy)
        //            if (row == Rows - 1 && Cells[col, row].State == CellStateEnum.Down)
        //                SetState(ref newCellsState[col, row - 1], CellStateEnum.Up);
        //        }
        //    }
        //    Cells = newCellsState;
        //}
        #endregion


        /// <summary>
        /// Determine if the cell is merged and its merged states contain the specified state
        /// </summary>
        /// <param name="c"></param>
        /// <param name="state"></param>
        /// <returns></returns>
        private bool CellIsMergedAndContainsState(Cell c, CellStateEnum state)
        {
            return (c.State == CellStateEnum.Merged && c.MergedStates.Contains(state));
        }

        /// <summary>
        /// Sets the state of the given cell. If it already has a state that is not dead,
        /// it will become merged and all the states that are assigned are added in the MergedStates list
        /// </summary>
        /// <param name="c"></param>
        /// <param name="state"></param>
        private void SetState(ref Cell c, CellStateEnum state)
        {
            if (c.State != CellStateEnum.Dead)
            {
                // already have state
                c.MergedStates.Add(state);
                c.State = CellStateEnum.Merged;
            }
            else
            {
                c.State = state;
                c.MergedStates = new List<CellStateEnum>() { state };
            }
        }

        /// <summary>
        /// Rotate cell state clockwise
        /// </summary>
        /// <param name="s"></param>
        /// <returns></returns>
        private CellStateEnum RotateClockWise(CellStateEnum s)
        {
            if (s == CellStateEnum.Up)
                return CellStateEnum.Right;
            else if (s == CellStateEnum.Right)
                return CellStateEnum.Down;
            else if (s == CellStateEnum.Down)
                return CellStateEnum.Left;
            else if (s == CellStateEnum.Left)
                return CellStateEnum.Up;
            else
                return s;
        }


        /// <summary>
        /// Determine the cell active in the current state
        /// 
        /// In this implementation: check on the borders if there are any cells that have
        /// a state that matches the border (e.g left border, left state)
        /// </summary>
        public override bool[,] ActiveCells
        {
            get
            {
                bool[,] activeCells = new bool[Cols, Rows];

                for (int row = 0; row < Rows; row++)
                {
                    // check the left border if any cell states are left or merged cells that came from left but are rotated
                    if (Cells[0, row].State == CellStateEnum.Left || CellIsMergedAndContainsState(Cells[0, row], RotateClockWise(CellStateEnum.Left)))
                        activeCells[0, row] = true;

                    // check the right border if any cell states are right or merged cells that came from right but are rotated
                    if (Cells[Cols - 1, row].State == CellStateEnum.Right || CellIsMergedAndContainsState(Cells[Cols - 1, row], RotateClockWise(CellStateEnum.Right)))
                        activeCells[Cols - 1, row] = true;
                }

                for (int col = 0; col < Cols; col++)
                {
                    // check the top border if any cell states are up or merged cells that came from up but are rotated
                    if (Cells[col, 0].State == CellStateEnum.Up || CellIsMergedAndContainsState(Cells[col, 0], RotateClockWise(CellStateEnum.Up)))
                        activeCells[col, 0] = true;

                    // check the bottom border if any cell states are down or merged cells that came from down but are rotated
                    if (Cells[col, Rows - 1].State == CellStateEnum.Down || CellIsMergedAndContainsState(Cells[col, Rows - 1], RotateClockWise(CellStateEnum.Down)))
                        activeCells[col, Rows - 1] = true;
                }

                return activeCells;
            }
        }
    }
}
