using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.Drawing;

namespace ProceduralMidi
{
    /// <summary>
    /// Displays the different cell states in a list, where the user can choose from
    /// </summary>
    class Palette : ListBox
    {
        public Palette()
        {

            DrawMode = System.Windows.Forms.DrawMode.OwnerDrawFixed;
            ItemHeight = 34;

            SetStyle(ControlStyles.OptimizedDoubleBuffer, true);
            SetStyle(ControlStyles.Selectable, true);
        }

        /// <summary>
        /// The available cell states to show in the list
        /// </summary>
        private CellStateEnum[] states;
        /// <summary>
        /// The available cell states to show in the list
        /// </summary>
        public CellStateEnum[] States
        {
            get { return states; }
            set
            {
                states = value;
                this.Items.Clear();
                if (states != null)
                    this.Items.AddRange(states.Cast<object>().ToArray());
                this.Invalidate();
            }
        }

        /// <summary>
        /// Overrides the default list item  drawing with
        /// the visual representation of a cell state
        /// </summary>
        /// <param name="e"></param>
        protected override void OnDrawItem(DrawItemEventArgs e)
        {
            if (e.Index < 0 || e.Index >= Items.Count)
                return;

            // the item is selected, draw selection color as background
            if ((e.State & DrawItemState.Selected) == DrawItemState.Selected)
            {
                using (SolidBrush br = new SolidBrush(Color.LightSkyBlue))
                    e.Graphics.FillRectangle(br, e.Bounds);
            }
            else
            {
                // not selected, use default listbox backcolor
                using (SolidBrush br = new SolidBrush(BackColor))
                    e.Graphics.FillRectangle(br, e.Bounds);
            }
            // calculate center of listbox
            RectangleF bounds = new RectangleF(e.Bounds.Left + e.Bounds.Width / 2 - ItemHeight / 2 + 1, e.Bounds.Top + 1, ItemHeight - 2, ItemHeight - 2);

            e.Graphics.SmoothingMode = System.Drawing.Drawing2D.SmoothingMode.AntiAlias;
            // draw cell state
            e.Graphics.DrawCellFancy(new Cell((CellStateEnum)Items[e.Index]), bounds);

            base.OnDrawItem(e);
        }

        /// <summary>
        /// The selected cell state from the list
        /// </summary>
        public CellStateEnum SelectedState
        {
            get
            {
                if (SelectedIndex >= 0 && SelectedIndex < States.Length)
                    return States[SelectedIndex];
                else
                    return CellStateEnum.Dead;
            }
        }

    }
}
