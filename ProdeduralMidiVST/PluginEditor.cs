using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Jacobi.Vst.Core;
using Jacobi.Vst.Framework;
using Jacobi.Vst.Framework.Common;
using ProceduralMidi;

namespace ProdeduralMidiVST
{
    /// <summary>
    /// Implements the custom UI editor for the plugin.
    /// </summary>
    class PluginEditor : IVstPluginEditor
    {
        private Plugin _plugin;
        private WinFormsControlWrapper<BoardEditor> _uiWrapper = new WinFormsControlWrapper<BoardEditor>();

        /// <summary>
        /// Constructs a new instance.
        /// </summary>
        /// <param name="plugin">Must not be null.</param>
        public PluginEditor(Plugin plugin)
        {
            _plugin = plugin;
        }

        #region IVstPluginEditor Members

        /// <summary>
        /// Returns the editors boundaries
        /// </summary>
        public System.Drawing.Rectangle Bounds
        {
            get { return _uiWrapper.Bounds; }
        }

        /// <summary>
        /// Closes the editor
        /// </summary>
        public void Close()
        {
            lock (guiLock) // only 1 thread can access the GUI at the same to prevent deadlocks
            {
                _uiWrapper.Close();
                _uiWrapper.SafeInstance.Dispose();
                IsOpen = false;
            }
        }

        /// <summary>
        /// Keydown is sent to the editor, ignore
        /// </summary>
        /// <param name="ascii"></param>
        /// <param name="virtualKey"></param>
        /// <param name="modifers"></param>
        public void KeyDown(byte ascii, VstVirtualKey virtualKey, VstModifierKeys modifers)
        {
            // no-op
        }

        /// <summary>
        /// Keyup is sent to the editor, ignore
        /// </summary>
        /// <param name="ascii"></param>
        /// <param name="virtualKey"></param>
        /// <param name="modifers"></param>
        public void KeyUp(byte ascii, VstVirtualKey virtualKey, VstModifierKeys modifers)
        {
            // no-op
        }

        /// <summary>
        /// Knobmode, not used but implementation was required
        /// </summary>
        public VstKnobMode KnobMode { get; set; }

        /// <summary>
        /// GUI lock,  only 1 thread can access the GUI at the same to prevent deadlocks
        /// </summary>
        private object guiLock = new object();

        /// <summary>
        /// Is the editor open
        /// </summary>
        public bool IsOpen { get; set; }


        /// <summary>
        /// Open an instance of the board editor
        /// </summary>
        /// <param name="hWnd"></param>
        public void Open(IntPtr hWnd)
        {
            lock (guiLock)
            {
                _uiWrapper = new WinFormsControlWrapper<BoardEditor>();
                // databind to board settings

                try
                {
                    Action a = () =>
                        {
                            _uiWrapper.SafeInstance.IsForPlugin = true;
                            _uiWrapper.SafeInstance.BoardSettings = _plugin.BoardSettings;
                        };
                    if (_uiWrapper.SafeInstance.InvokeRequired)
                    {
                        a();
                    }
                    else
                        _uiWrapper.SafeInstance.BeginInvoke(a);

                }
                catch (Exception)
                {
                    // could give exceptions when handle is not created
                }

                _uiWrapper.Open(hWnd);

                IsOpen = true;
            }
        }

        /// <summary>
        /// Idling, update the GUI
        /// </summary>
        public void ProcessIdle()
        {
            // update GUI

            lock (guiLock)
            {
                if (IsOpen)
                {
                    try
                    {
                        Action a = () => _uiWrapper.SafeInstance.UpdateGUIFromBoardSettings(_plugin.BoardSettings);
                        if (_uiWrapper.SafeInstance.InvokeRequired)
                            _uiWrapper.SafeInstance.BeginInvoke(a);
                        else
                            a();

                    }
                    catch (Exception)
                    {
                        // could give exceptions when handle is not created
                    }
                }
            }

        }

        #endregion

        /// <summary>
        /// Updates the GUI for the current board state
        /// </summary>
        internal void UpdateBoard()
        {
            lock (guiLock)
            {
                if (IsOpen)
                {
                    _uiWrapper.SafeInstance.AddHighlightsForActiveCells();
                    _uiWrapper.SafeInstance.UpdateBoardGUI();
                }
            }
        }
    }
}
