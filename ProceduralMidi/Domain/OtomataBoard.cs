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
                        CheckLeft(newCellsState, currentCell.State, row, col);
                    // move cell 1 to right
                    else if (currentCell.State == CellStateEnum.Right)
                        CheckRight(newCellsState, currentCell.State, row, col);
                    // move cell 1 to top
                    else if (currentCell.State == CellStateEnum.Up)
                        CheckUp(newCellsState, currentCell.State, row, col);
                    // move cell 1 to bottom
                    else if (currentCell.State == CellStateEnum.Down)
                        CheckDown(newCellsState, currentCell.State, row, col);

                    // walls stay walls, rotation blocks stay the same too (immutable)
                    else if (currentCell.State == CellStateEnum.Wall || currentCell.State == CellStateEnum.SoundWall ||
                             currentCell.State == CellStateEnum.RotateClockWise || currentCell.State == CellStateEnum.RotateCounterClockWise)
                        newCellsState[col, row].State = currentCell.State;

                    else if (currentCell.State == CellStateEnum.Merged)
                    {
                        foreach (var mergedState in currentCell.MergedStates)
                        {
                            if (mergedState == CellStateEnum.Left)
                                CheckLeft(newCellsState, mergedState, row, col);
                            else if (mergedState == CellStateEnum.Right)
                                CheckRight(newCellsState, mergedState, row, col);
                            else if (mergedState == CellStateEnum.Up)
                                CheckUp(newCellsState, mergedState, row, col);
                            else if (mergedState == CellStateEnum.Down)
                                CheckDown(newCellsState, mergedState, row, col);
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

        /// <summary>
        /// Checks the position down of the given position to set the new cell state
        /// </summary>
        /// <param name="newCellsState"></param>
        /// <param name="row"></param>
        /// <param name="col"></param>
        private void CheckDown(Cell[,] newCellsState, CellStateEnum currentCellState, int row, int col)
        {
            if (row + 1 >= Rows || IsWall(col, row + 1))
            { // reached edge or wall, bounce

                // if there is room to bounce back
                if (row - 1 >= 0 && !IsBlocked(col, row - 1))
                    SetState(newCellsState[col, row - 1], CellStateEnum.Up);
                else
                    SetState(newCellsState[col, row], CellStateEnum.Up);
            }
            else
            {
                if (Cells[col, row + 1].State == CellStateEnum.RotateClockWise)
                {
                    if (col - 1 >= 0 && !IsBlocked(col - 1, row))
                        SetState(newCellsState[col - 1, row], RotateClockWise(currentCellState));
                    else
                        SetState(newCellsState[col, row], RotateClockWise(currentCellState));
                }
                else if (Cells[col, row + 1].State == CellStateEnum.RotateCounterClockWise)
                {
                    if (col + 1 < Cols && !IsBlocked(col + 1, row))
                        SetState(newCellsState[col + 1, row], RotateCounterClockWise(currentCellState));
                    else
                        SetState(newCellsState[col, row], RotateCounterClockWise(currentCellState));
                }
                else
                {
                    // free to go to the up side
                    SetState(newCellsState[col, row + 1], CellStateEnum.Down);
                }
            }


            //if (row + 1 < Rows && Cells[col, row + 1].State != CellStateEnum.Wall && Cells[col, row + 1].State != CellStateEnum.SoundWall)
            //    SetState(newCellsState[col, row + 1], CellStateEnum.Down);
            //else // out bounds, let it bounce to the other side
            //{
            //    // if there is room to bounce back
            //    if (row - 1 >= 0 && Cells[col, row - 1].State != CellStateEnum.Wall && Cells[col, row - 1].State != CellStateEnum.SoundWall)
            //        SetState(newCellsState[col, row - 1], CellStateEnum.Up);
            //    else // otherwise stay put and change cell
            //        SetState(newCellsState[col, row], CellStateEnum.Up);
            //}
        }

        /// <summary>
        /// Checks the position up of the given position to set the new cell state
        /// </summary>
        /// <param name="newCellsState"></param>
        /// <param name="row"></param>
        /// <param name="col"></param>
        private void CheckUp(Cell[,] newCellsState, CellStateEnum currentCellState, int row, int col)
        {
            if (row - 1 < 0 || IsWall(col, row - 1))
            { // reached edge or wall, bounce

                // if there is room to bounce back
                if (row + 1 < Rows && !IsBlocked(col, row + 1))
                    SetState(newCellsState[col, row + 1], CellStateEnum.Down);
                else
                    SetState(newCellsState[col, row], CellStateEnum.Down);
            }
            else
            {
                if (Cells[col, row - 1].State == CellStateEnum.RotateClockWise)
                {
                    if (col + 1 < Cols && !IsBlocked(col + 1, row))
                        SetState(newCellsState[col + 1, row], RotateClockWise(currentCellState));
                    else
                        SetState(newCellsState[col, row], RotateClockWise(currentCellState));
                }
                else if (Cells[col, row - 1].State == CellStateEnum.RotateCounterClockWise)
                {
                    if (col - 1 >= 0 && !IsBlocked(col - 1, row))
                        SetState(newCellsState[col - 1, row], RotateCounterClockWise(currentCellState));
                    else
                        SetState(newCellsState[col, row], RotateCounterClockWise(currentCellState));
                }
                else
                {
                    // free to go to the up side
                    SetState(newCellsState[col, row - 1], CellStateEnum.Up);
                }
            }

            //if (row - 1 >= 0 && Cells[col, row - 1].State != CellStateEnum.Wall && Cells[col, row - 1].State != CellStateEnum.SoundWall)
            //    SetState(newCellsState[col, row - 1], CellStateEnum.Up);
            //else // out bounds, let it bounce to the other side
            //{
            //    // if there is room to bounce back
            //    if (row + 1 < Rows && Cells[col, row + 1].State != CellStateEnum.Wall && Cells[col, row + 1].State != CellStateEnum.SoundWall)
            //        SetState(newCellsState[col, row + 1], CellStateEnum.Down);
            //    else
            //        SetState(newCellsState[col, row], CellStateEnum.Down);
            //}
        }

        /// <summary>
        /// Checks the position right of the given position to set the new cell state
        /// </summary>
        /// <param name="newCellsState"></param>
        /// <param name="row"></param>
        /// <param name="col"></param>
        private void CheckRight(Cell[,] newCellsState, CellStateEnum currentCellState, int row, int col)
        {
            if (col + 1 >= Cols || IsWall(col + 1, row))
            { // reached edge or wall, bounce

                // if there is room to bounce back
                if (col - 1 >= 0 && !IsBlocked(col - 1, row))
                    SetState(newCellsState[col - 1, row], CellStateEnum.Left);
                else
                    SetState(newCellsState[col, row], CellStateEnum.Left);
            }
            else
            {
                if (Cells[col + 1, row].State == CellStateEnum.RotateClockWise)
                {
                    if (row + 1 < Rows && !IsBlocked(col, row + 1))
                        SetState(newCellsState[col, row + 1], RotateClockWise(currentCellState));
                    else
                        SetState(newCellsState[col, row], RotateClockWise(currentCellState));
                }
                else if (Cells[col + 1, row].State == CellStateEnum.RotateCounterClockWise)
                {
                    if (row - 1 >= 0 && !IsBlocked(col, row - 1))
                        SetState(newCellsState[col, row - 1], RotateCounterClockWise(currentCellState));
                    else
                        SetState(newCellsState[col, row], RotateCounterClockWise(currentCellState));
                }
                else
                {
                    // free to go to the right side
                    SetState(newCellsState[col + 1, row], CellStateEnum.Right);
                }
            }

            //if (col + 1 < Cols && Cells[col + 1, row].State != CellStateEnum.Wall && Cells[col + 1, row].State != CellStateEnum.SoundWall)
            //    SetState(newCellsState[col + 1, row], CellStateEnum.Right);
            //else // out bounds, let it bounce to the other side
            //{
            //    // if there is room to bounce back
            //    if (col - 1 >= 0 && Cells[col - 1, row].State != CellStateEnum.Wall && Cells[col - 1, row].State != CellStateEnum.SoundWall)
            //        SetState(newCellsState[col - 1, row], CellStateEnum.Left);
            //    else
            //        SetState(newCellsState[col, row], CellStateEnum.Left);
            //}
        }

        /// <summary>
        /// Checks the position left of the given position to set the new cell state
        /// </summary>
        /// <param name="newCellsState"></param>
        /// <param name="row"></param>
        /// <param name="col"></param>
        private void CheckLeft(Cell[,] newCellsState, CellStateEnum currentCellState, int row, int col)
        {
            if (col - 1 < 0 || IsWall(col - 1, row))
            { // reached edge or wall, bounce

                // if there is room to bounce back
                if (col + 1 < Cols && !IsBlocked(col + 1, row))
                    SetState(newCellsState[col + 1, row], CellStateEnum.Right);
                else
                    SetState(newCellsState[col, row], CellStateEnum.Right);
            }
            else
            {
                if (Cells[col - 1, row].State == CellStateEnum.RotateClockWise)
                {
                    if (row - 1 >= 0 && !IsBlocked(col, row - 1))
                        SetState(newCellsState[col, row - 1], RotateClockWise(currentCellState));
                    else
                        SetState(newCellsState[col, row], RotateClockWise(currentCellState));
                }
                else if (Cells[col - 1, row].State == CellStateEnum.RotateCounterClockWise)
                {
                    if (row + 1 < Rows && !IsBlocked(col, row + 1))
                        SetState(newCellsState[col, row + 1], RotateCounterClockWise(currentCellState));
                    else
                        SetState(newCellsState[col, row], RotateCounterClockWise(currentCellState));
                }
                else
                {
                    // free to go to the left side
                    SetState(newCellsState[col - 1, row], CellStateEnum.Left);
                }
            }


            //if (col - 1 >= 0 && 
            //    Cells[col - 1, row].State != CellStateEnum.Wall && Cells[col - 1, row].State != CellStateEnum.SoundWall && 
            //    Cells[col - 1, row].State != CellStateEnum.RotateClockWise && Cells[col - 1, row].State != CellStateEnum.RotateCounterClockWise)
            //    // free to go to the left side
            //    SetState(newCellsState[col - 1, row], CellStateEnum.Left);
            //else
            //{
            //    // let it bounce to the other side

            //    // if there is room to bounce back
            //    if (col + 1 < Cols && Cells[col + 1, row].State != CellStateEnum.Wall && Cells[col + 1, row].State != CellStateEnum.SoundWall)
            //        SetState(newCellsState[col + 1, row], CellStateEnum.Right);
            //    else
            //        SetState(newCellsState[col, row], CellStateEnum.Right);
            //}
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
        /// Is the cell blocked on the given position (can't go to given position)
        /// </summary>
        /// <param name="col"></param>
        /// <param name="row"></param>
        /// <returns></returns>
        private bool IsBlocked(int col, int row)
        {
            return Cells[col, row].State == CellStateEnum.Wall || Cells[col, row].State == CellStateEnum.SoundWall ||
                   Cells[col, row].State == CellStateEnum.RotateClockWise || Cells[col, row].State == CellStateEnum.RotateCounterClockWise;
        }

        /// <summary>
        /// Is the cell at given position a wall
        /// </summary>
        /// <param name="col"></param>
        /// <param name="row"></param>
        /// <returns></returns>
        private bool IsWall(int col, int row)
        {
            return Cells[col, row].State == CellStateEnum.Wall || Cells[col, row].State == CellStateEnum.SoundWall;
        }


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
        private void SetState(Cell c, CellStateEnum state)
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

            return s;
        }

        /// <summary>
        /// Rotate cell state counter clockwise
        /// </summary>
        /// <param name="s"></param>
        /// <returns></returns>
        private CellStateEnum RotateCounterClockWise(CellStateEnum s)
        {
            if (s == CellStateEnum.Up)
                return CellStateEnum.Left;
            else if (s == CellStateEnum.Left)
                return CellStateEnum.Down;
            else if (s == CellStateEnum.Down)
                return CellStateEnum.Right;
            else if (s == CellStateEnum.Right)
                return CellStateEnum.Up;
            else
                return s;
        }

        /// <summary>
        /// Determine the cell active in the current state
        /// 
        /// In this implementation: check on the borders if there are any cells that have
        /// a state that matches the border (e.g left border, left state), or if a cell will bounce
        /// against a sound wall
        /// </summary>
        public override ActiveState[,] ActiveCells
        {
            get
            {
                ActiveState[,] activeCells = new ActiveState[Cols, Rows];

                for (int row = 0; row < Rows; row++)
                {
                    // check the left border if any cell states are left or merged cells that came from left but are rotated
                    if (Cells[0, row].State == CellStateEnum.Left || CellIsMergedAndContainsState(Cells[0, row], RotateClockWise(CellStateEnum.Left)))
                        activeCells[0, row] = ActiveState.RowActivated;

                    // check the right border if any cell states are right or merged cells that came from right but are rotated
                    if (Cells[Cols - 1, row].State == CellStateEnum.Right || CellIsMergedAndContainsState(Cells[Cols - 1, row], RotateClockWise(CellStateEnum.Right)))
                        activeCells[Cols - 1, row] = ActiveState.RowActivated;
                }

                for (int col = 0; col < Cols; col++)
                {
                    // check the top border if any cell states are up or merged cells that came from up but are rotated
                    if (Cells[col, 0].State == CellStateEnum.Up || CellIsMergedAndContainsState(Cells[col, 0], RotateClockWise(CellStateEnum.Up)))
                        activeCells[col, 0] = ActiveState.ColumnActivated;

                    // check the bottom border if any cell states are down or merged cells that came from down but are rotated
                    if (Cells[col, Rows - 1].State == CellStateEnum.Down || CellIsMergedAndContainsState(Cells[col, Rows - 1], RotateClockWise(CellStateEnum.Down)))
                        activeCells[col, Rows - 1] = ActiveState.ColumnActivated;
                }

                for (int row = 0; row < Rows; row++)
                {
                    for (int col = 0; col < Cols; col++)
                    {
                        // check left
                        if (Cells[col, row].State == CellStateEnum.Left || CellIsMergedAndContainsState(Cells[col, row], RotateClockWise(CellStateEnum.Left)))
                        {
                            if (col - 1 < 0 || Cells[col - 1, row].State == CellStateEnum.SoundWall)
                                activeCells[col, row] = ActiveState.RowActivated;
                        }

                        // check right
                        if (Cells[col, row].State == CellStateEnum.Right || CellIsMergedAndContainsState(Cells[col, row], RotateClockWise(CellStateEnum.Right)))
                        {
                            if (col + 1 >= Cols || Cells[col + 1, row].State == CellStateEnum.SoundWall)
                                activeCells[col, row] = ActiveState.RowActivated;
                        }

                        // check up
                        if (Cells[col, row].State == CellStateEnum.Up || CellIsMergedAndContainsState(Cells[col, row], RotateClockWise(CellStateEnum.Up)))
                        {
                            if (row - 1 < 0 || Cells[col, row - 1].State == CellStateEnum.SoundWall)
                                activeCells[col, row] = ActiveState.ColumnActivated;
                        }

                        // check down
                        if (Cells[col, row].State == CellStateEnum.Down || CellIsMergedAndContainsState(Cells[col, row], RotateClockWise(CellStateEnum.Down)))
                        {
                            if (row + 1 >= Rows || Cells[col, row + 1].State == CellStateEnum.SoundWall)
                                activeCells[col, row] = ActiveState.ColumnActivated;
                        }
                    }
                }

                return activeCells;
            }
        }

        /// <summary>
        /// Otomata boards can only draw dead, the 4 directions and walls
        /// </summary>
        public override CellStateEnum[] PossibleStatesForPalette
        {
            get
            {
                return new CellStateEnum[] { 
                    CellStateEnum.Dead, 
                    CellStateEnum.Up, 
                    CellStateEnum.Right, 
                    CellStateEnum.Down, 
                    CellStateEnum.Left, 
                    CellStateEnum.Wall,
                    CellStateEnum.SoundWall,
                    CellStateEnum.RotateClockWise ,
                    CellStateEnum.RotateCounterClockWise
                };
            }
        }
    }
}
