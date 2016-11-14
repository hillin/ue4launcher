using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Runtime.InteropServices;
using System.Windows;

namespace UE4Launcher.Debugging
{
    static class DebuggerLauncher
    {
        private static readonly Dictionary<VisualStudioVersions, string> DteProgIds
            = new Dictionary<VisualStudioVersions, string>
            {
                {VisualStudioVersions.VS2015, "VisualStudio.DTE.14.0"},
                {VisualStudioVersions.VS2013, "VisualStudio.DTE.12.0"},
                {VisualStudioVersions.VS2012, "VisualStudio.DTE.11.0"},
                {VisualStudioVersions.VS2010, "VisualStudio.DTE.10.0"},
            };

        public static bool GetDebuggerAvailability(VisualStudioVersions vsVersion)
        {
            if (vsVersion == VisualStudioVersions.Automatic)
                return DteProgIds.Keys.Any(DebuggerLauncher.GetDebuggerAvailability);

            try
            {
                dynamic dte = Marshal.GetActiveObject(DteProgIds[vsVersion]);
                return dte != null;
            }
            catch
            {
                return false;
            }
        }

        public static bool Attach(Process process, VisualStudioVersions vsVersion = VisualStudioVersions.Automatic)
        {
            MessageFilter.Register();

            try
            {
                if (vsVersion == VisualStudioVersions.Automatic)
                {
                    var attachSucceed = false;
                    foreach (var progId in DteProgIds.Values)
                    {
                        try
                        {
                            DebuggerLauncher.TryAttachDebugger(process, progId);
                            attachSucceed = true;
                            break;
                        }
                        catch (Exception)
                        {
                            // ignored
                        }
                    }

                    if (attachSucceed)
                        return true;

                    MessageBox.Show(
                        "Unable to launch and attach to debugger, make sure you have Visual Studio (2010/2012/2013/2015) installed correctly.",
                        "Launch and Debug", MessageBoxButton.OK, MessageBoxImage.Exclamation);

                    return false;
                }
                else
                {
                    try
                    {
                        DebuggerLauncher.TryAttachDebugger(process, DteProgIds[vsVersion]);
                        return true;
                    }
                    catch (Exception exception)
                    {
                        MessageBox.Show(
                            $"Unable to launch and attach to debugger, make sure you have Visual Studio of selected version installed correctly.\n\nError: {exception.Message}",
                            "Launch and Debug", MessageBoxButton.OK, MessageBoxImage.Exclamation);
                        return false;
                    }
                }
            }
            finally
            {
                MessageFilter.Revoke();
            }
        }

        private static void TryAttachDebugger(Process process, string progId)
        {
            dynamic dte = Marshal.GetActiveObject(progId);
            if (dte == null)
                throw new Exception($"'{progId}' not found");

            var processes = ((IEnumerable)dte.Debugger.LocalProcesses).OfType<dynamic>();
            var dteProcess = processes.SingleOrDefault(x => x.ProcessID == process.Id);
            if (dteProcess == null)
                throw new Exception("Process not found");

            dteProcess.Attach();
        }
    }
}
