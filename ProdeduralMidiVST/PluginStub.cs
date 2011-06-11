using Jacobi.Vst.Framework.Plugin;
using System;

namespace ProdeduralMidiVST
{
  

    /// <summary>
    /// The public Plugin Command Stub implementation derived from the framework provided <see cref="StdPluginCommandStub"/>.
    /// </summary>
    public sealed class PluginCommandStub : StdPluginDeprecatedCommandStub, Jacobi.Vst.Core.Plugin.IVstPluginCommandStub
    {
        /// <summary>
        /// Called by the framework to create the plugin root class.
        /// </summary>
        /// <returns>Never returns null.</returns>
        protected override Jacobi.Vst.Framework.IVstPlugin CreatePluginInstance()
        {
            AppDomain.CurrentDomain.AssemblyResolve += new ResolveEventHandler(CurrentDomain_AssemblyResolve);

            return new Plugin();
        }

        System.Reflection.Assembly CurrentDomain_AssemblyResolve(object sender, ResolveEventArgs args)
        {
            string curPath = System.IO.Path.GetDirectoryName(System.Reflection.Assembly.GetExecutingAssembly().Location);
            if (args.Name.Contains("ProceduralMidi"))
                return System.Reflection.Assembly.LoadFile(System.IO.Path.Combine(curPath, "ProceduralMidi.exe"));

            return null;
        }
    }
}
