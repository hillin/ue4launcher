using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Input;
using System.Windows.Media;

namespace UE4Launcher
{
    static class Utilities
    {
        public static void NavigateFile(string file)
        {
            if (Directory.Exists(file))
                Process.Start(file);
            else if (File.Exists(file))
                Process.Start("explorer.exe", $"/select, \"{file}\"");
            else
            {
                var directory = Directory.GetParent(file);
                if (directory != null)
                    Process.Start(directory.FullName);
            }
        }


        public static bool IsCtrlDown => Keyboard.IsKeyDown(Key.LeftCtrl) || Keyboard.IsKeyDown(Key.RightCtrl);
    }
}
