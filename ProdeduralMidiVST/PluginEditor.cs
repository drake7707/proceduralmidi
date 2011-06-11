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

        public System.Drawing.Rectangle Bounds
        {
            get { return _uiWrapper.Bounds; }
        }

        public void Close()
        {
            lock (guiLock)
            {
                _uiWrapper.Close();
                _uiWrapper.SafeInstance.Dispose();
                IsOpen = false;
            }
        }

        public void KeyDown(byte ascii, VstVirtualKey virtualKey, VstModifierKeys modifers)
        {
            // no-op
        }

        public void KeyUp(byte ascii, VstVirtualKey virtualKey, VstModifierKeys modifers)
        {
            // no-op
        }

        public VstKnobMode KnobMode { get; set; }


        private object guiLock = new object();

        public bool IsOpen { get; set; }

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

        public void ProcessIdle()
        {
            // update GUI

            lock (guiLock)
            {
                if (IsOpen)
                {
                    try
                    {
                        Action a = () => _uiWrapper.SafeInstance.UpdateFromBoardSettings(_plugin.BoardSettings);
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
