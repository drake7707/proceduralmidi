using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Drawing;
using System.Drawing.Drawing2D;

namespace ProceduralMidi
{
    /// <summary>
    /// Provides extension methods to allow Graphics objects draw cell states
    /// </summary>
    public static class CellDrawing
    {

        /// <summary>
        /// The characters to draw for the different cell states
        /// </summary>
        private static string[] cellStateText = { "▲", "►", "▼", "◄", "", "", "█", "♫", "∩", "U" };

        private static Bitmap bmpWall = global::ProceduralMidi.Properties.Resources.wall;
        private static Bitmap bmpSoundWall = global::ProceduralMidi.Properties.Resources.volume;
        private static Bitmap bmpRotateClockwise = global::ProceduralMidi.Properties.Resources.clockwise;
        private static Bitmap bmpRotateCounterClockwise = global::ProceduralMidi.Properties.Resources.counterclockwise;

        private static Bitmap[] cellStateBmp = { null, null, null, null, null, null, bmpWall, bmpSoundWall, bmpRotateClockwise, bmpRotateCounterClockwise };

        // create required brushes, etc
        private static SolidBrush singleStateBrush = new SolidBrush(Color.DarkGray);
        private static SolidBrush mergedStateBrush = new SolidBrush(Color.White);
        private static SolidBrush txtbrush = new SolidBrush(Color.White);
        private static Font f = new Font("Times New Roman", 10f, FontStyle.Bold);

        /// <summary>
        /// Draws a cell using the given graphics
        /// </summary>
        /// <param name="g"></param>
        /// <param name="cellState"></param>
        /// <param name="cellBounds"></param>
        public static void DrawCellFancy(this Graphics g, Cell cell, RectangleF cellBounds)
        {
            if (cell.State != CellStateEnum.Dead)
            {
                // to draw the background of the cell use mergedbrush if cell state is merged, otherwise singlestatebrush
                Brush b = (cell.State == CellStateEnum.Merged ? mergedStateBrush : singleStateBrush);
                g.FillRoundRect(b, cellBounds, 3);

                // get character for cellstate

                Bitmap bmp = cellStateBmp[(int)cell.State];
                if (bmp == null)
                {
                    string str = cellStateText[(int)cell.State];
                    SizeF sizeStr = g.MeasureString(str, f);
                    // draw character in center of cell
                    g.DrawString(str, f, txtbrush, new PointF(cellBounds.Left + cellBounds.Width / 2 - sizeStr.Width / 2,
                                                                 cellBounds.Top + cellBounds.Height / 2 - sizeStr.Height / 2));

                }
                else
                    g.DrawImage(bmp, (int)(cellBounds.Left + cellBounds.Width / 2 - bmp.Width / 2),
                                             (int)(cellBounds.Top + cellBounds.Height / 2 - bmp.Height / 2),
                                             bmp.Width,
                                             bmp.Height);
            }
        }

        /// <summary>
        /// Draws a cell using the given graphics
        /// </summary>
        /// <param name="g"></param>
        /// <param name="cellState"></param>
        /// <param name="cellBounds"></param>
        public static void DrawCellFast(this Graphics g, Cell cell, RectangleF cellBounds)
        {
            if (cell.State != CellStateEnum.Dead)
            {
                // to draw the background of the cell use mergedbrush if cell state is merged, otherwise singlestatebrush
                Brush b = (cell.State == CellStateEnum.Merged ? mergedStateBrush : singleStateBrush);
                g.FillRectangle(b, cellBounds);

                // get character for cellstate
                string str = cellStateText[(int)cell.State];
                SizeF sizeStr = g.MeasureString(str, f);
                // draw character in center of cell
                g.DrawString(str, f, txtbrush, new PointF(cellBounds.Left + cellBounds.Width / 2 - sizeStr.Width / 2,
                                                             cellBounds.Top + cellBounds.Height / 2 - sizeStr.Height / 2));


            }
        }

        /// <summary>
        /// Draw a rounded rectangle
        /// </summary>
        /// <param name="g"></param>
        /// <param name="p"></param>
        /// <param name="r"></param>
        /// <param name="radius"></param>
        public static void DrawRoundRect(this Graphics g, Pen p, RectangleF r, float radius)
        {
            using (GraphicsPath gp = new GraphicsPath())
            {
                gp.AddLine(r.X + radius, r.Y, r.X + r.Width - (radius * 2), r.Y); // Line
                gp.AddArc(r.X + r.Width - (radius * 2), r.Y, radius * 2, radius * 2, 270, 90); // Corner
                gp.AddLine(r.X + r.Width, r.Y + radius, r.X + r.Width, r.Y + r.Height - (radius * 2)); // Line
                gp.AddArc(r.X + r.Width - (radius * 2), r.Y + r.Height - (radius * 2), radius * 2, radius * 2, 0, 90); // Corner
                gp.AddLine(r.X + r.Width - (radius * 2), r.Y + r.Height, r.X + radius, r.Y + r.Height); // Line
                gp.AddArc(r.X, r.Y + r.Height - (radius * 2), radius * 2, radius * 2, 90, 90); // Corner
                gp.AddLine(r.X, r.Y + r.Height - (radius * 2), r.X, r.Y + radius); // Line
                gp.AddArc(r.X, r.Y, radius * 2, radius * 2, 180, 90); // Corner
                gp.CloseFigure();

                g.DrawPath(p, gp);
            }
        }

        /// <summary>
        /// Draws a filled round rectangle
        /// </summary>
        /// <param name="g"></param>
        /// <param name="b"></param>
        /// <param name="r"></param>
        /// <param name="radius"></param>
        public static void FillRoundRect(this Graphics g, Brush b, RectangleF r, float radius)
        {
            using (GraphicsPath gp = new GraphicsPath())
            {
                gp.AddLine(r.X + radius, r.Y, r.X + r.Width - (radius * 2), r.Y); // Line
                gp.AddArc(r.X + r.Width - (radius * 2), r.Y, radius * 2, radius * 2, 270, 90); // Corner
                gp.AddLine(r.X + r.Width, r.Y + radius, r.X + r.Width, r.Y + r.Height - (radius * 2)); // Line
                gp.AddArc(r.X + r.Width - (radius * 2), r.Y + r.Height - (radius * 2), radius * 2, radius * 2, 0, 90); // Corner
                gp.AddLine(r.X + r.Width - (radius * 2), r.Y + r.Height, r.X + radius, r.Y + r.Height); // Line
                gp.AddArc(r.X, r.Y + r.Height - (radius * 2), radius * 2, radius * 2, 90, 90); // Corner
                gp.AddLine(r.X, r.Y + r.Height - (radius * 2), r.X, r.Y + radius); // Line
                gp.AddArc(r.X, r.Y, radius * 2, radius * 2, 180, 90); // Corner
                gp.CloseFigure();

                g.FillPath(b, gp);
            }
        }
    }
}
