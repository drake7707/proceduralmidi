using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Jacobi.Vst.Core;
using Jacobi.Vst.Framework;
using Jacobi.Vst.Framework.Common;
using Jacobi.Vst.Framework.Plugin;
using ProceduralMidi.DAL;

namespace ProdeduralMidiVST
{
    /// <summary>
    /// VST Plugin manager that manages the midi/audio processor and editor
    /// </summary>
    public class Plugin : VstPluginWithInterfaceManagerBase, IVstPluginMidiSource
    {

        /// <summary>
        /// Constructs a new instance.
        /// </summary>
        public Plugin()
            : base("Procedural Midi VST", new VstProductInfo("Procedural Midi", "http://proceduralmidi.codeplex.com", 1000),
                VstPluginCategory.Generator, VstPluginCapabilities.NoSoundInStop | VstPluginCapabilities.ReceiveTimeInfo, 0, 0x77073233)
        {
            BoardSettings = BoardSettings.GetDefaultBoard();
        }


        /// <summary>
        /// Creates a default instance and reuses that for all threads.
        /// </summary>
        /// <param name="instance">A reference to the default instance or null.</param>
        /// <returns>Returns the default instance.</returns>
        protected override IVstPluginAudioProcessor CreateAudioProcessor(IVstPluginAudioProcessor instance)
        {
            if (instance == null) return new AudioProcessor(this);

            return instance;
        }

        /// <summary>
        /// Creates a default instance and reuses that for all threads.
        /// </summary>
        /// <param name="instance">A reference to the default instance or null.</param>
        /// <returns>Returns the default instance.</returns>
        protected override IVstPluginEditor CreateEditor(IVstPluginEditor instance)
        {
            if (instance == null) return new PluginEditor(this);

            return instance;
        }

        /// <summary>
        /// Creates a default instance and reuses that for all threads.
        /// </summary>
        /// <param name="instance">A reference to the default instance or null.</param>
        /// <returns>Returns the default instance.</returns>
        protected override IVstMidiProcessor CreateMidiProcessor(IVstMidiProcessor instance)
        {
            if (instance == null) return new MidiProcessor(this);

            return instance;
        }

        /// <summary>
        ///// Creates a default instance and reuses that for all threads.
        ///// </summary>
        ///// <param name="instance">A reference to the default instance or null.</param>
        ///// <returns>Returns the default instance.</returns>
        //protected override IVstPluginPersistence CreatePersistence(IVstPluginPersistence instance)
        //{
        //    if (instance == null) return new PluginPersistence(this);

        //    return instance;
        //}

        /// <summary>
        /// Always returns <b>this</b>.
        /// </summary>
        /// <param name="instance">A reference to the default instance or null.</param>
        /// <returns>Returns the default instance <b>this</b>.</returns>
        protected override IVstPluginMidiSource CreateMidiSource(IVstPluginMidiSource instance)
        {
            return this;
        }

        #region IVstPluginMidiSource Members

        /// <summary>
        /// Returns the channel count as reported by the host
        /// </summary>
        public int ChannelCount
        {
            get
            {
                IVstMidiProcessor midiProcessor = null;

                if (Host != null)
                    midiProcessor = Host.GetInstance<IVstMidiProcessor>();

                if (midiProcessor != null)
                    return midiProcessor.ChannelCount;

                return 0;
            }
        }

        /// <summary>
        /// The current state of the board and its settings
        /// </summary>
        public BoardSettings BoardSettings { get; set; }

        #endregion
    }
}
