using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;

// ReSharper disable All

namespace UE4Launcher.Root
{
    public static class MouseHook
    {
        public static event EventHandler MouseAction = delegate { };

        public static void Start()
        {
            _hookID = MouseHook.SetHook(Proc);
        }

        public static void Stop()
        {
            MouseHook.UnhookWindowsHookEx(_hookID);
        }

        private static readonly LowLevelMouseProc Proc = MouseHook.HookCallback;
        private static IntPtr _hookID = IntPtr.Zero;

        private static IntPtr SetHook(LowLevelMouseProc proc)
        {
            // http://stackoverflow.com/questions/11607133/global-mouse-event-handler
            // SetWindowsHookEx() requires a valid module handle but doesn't actually use it when you 
            // set a low-level mouse hook. So any handle will do, you can pass the handle for user32.dll, 
            // always loaded in a .NET application
            return MouseHook.SetWindowsHookEx(WH_MOUSE_LL, proc,
                                              MouseHook.GetModuleHandle("user32"), 0);

        }

        private delegate IntPtr LowLevelMouseProc(int nCode, IntPtr wParam, IntPtr lParam);

        private static IntPtr HookCallback(
            int nCode, IntPtr wParam, IntPtr lParam)
        {
            if (nCode >= 0 && MouseMessages.WM_LBUTTONDOWN == (MouseMessages)wParam)
            {
                MSLLHOOKSTRUCT hookStruct = (MSLLHOOKSTRUCT)Marshal.PtrToStructure(lParam, typeof(MSLLHOOKSTRUCT));
                MouseHook.MouseAction(null, EventArgs.Empty);
            }
            return MouseHook.CallNextHookEx(_hookID, nCode, wParam, lParam);
        }

        private const int WH_MOUSE_LL = 14;

        private enum MouseMessages
        {
            WM_LBUTTONDOWN = 0x0201,
            WM_LBUTTONUP = 0x0202,
            WM_MOUSEMOVE = 0x0200,
            WM_MOUSEWHEEL = 0x020A,
            WM_RBUTTONDOWN = 0x0204,
            WM_RBUTTONUP = 0x0205
        }

        [StructLayout(LayoutKind.Sequential)]
        private struct POINT
        {
            public readonly int x;
            public readonly int y;
        }

        [StructLayout(LayoutKind.Sequential)]
        private struct MSLLHOOKSTRUCT
        {
            public readonly POINT pt;
            public readonly uint mouseData;
            public readonly uint flags;
            public readonly uint time;
            public readonly IntPtr dwExtraInfo;
        }

        [DllImport("user32.dll", CharSet = CharSet.Auto, SetLastError = true)]
        private static extern IntPtr SetWindowsHookEx(int idHook,
                                                      LowLevelMouseProc lpfn, IntPtr hMod, uint dwThreadId);

        [DllImport("user32.dll", CharSet = CharSet.Auto, SetLastError = true)]
        [return: MarshalAs(UnmanagedType.Bool)]
        private static extern bool UnhookWindowsHookEx(IntPtr hhk);

        [DllImport("user32.dll", CharSet = CharSet.Auto, SetLastError = true)]
        private static extern IntPtr CallNextHookEx(IntPtr hhk, int nCode,
                                                    IntPtr wParam, IntPtr lParam);

        [DllImport("kernel32.dll", CharSet = CharSet.Auto, SetLastError = true)]
        private static extern IntPtr GetModuleHandle(string lpModuleName);
    }
}
