using System;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
using System.Windows;
using System.Windows.Input;
using System.Windows.Interop;
using System.Windows.Media;
using System.Windows.Media.Imaging;

namespace UE4Launcher
{
	internal static class Helpers
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
		public static bool IsAltDown => Keyboard.IsKeyDown(Key.LeftAlt) || Keyboard.IsKeyDown(Key.RightAlt);


		public static ImageSource GetFileSystemIcon(string path, bool bSmall)
		{
			var info = new Interop.SHFILEINFO(true);
			var cbFileInfo = Marshal.SizeOf(info);

			var flags = Interop.SHGFI.Icon
						| Interop.SHGFI.UseFileAttributes
						| (bSmall ? Interop.SHGFI.SmallIcon : Interop.SHGFI.LargeIcon);

			var attribute = Directory.Exists(path)
				? Interop.FileAttributes.Directory
				: Interop.FileAttributes.File;
			try
			{
				Interop.SHGetFileInfo(path, attribute, out info, (uint)cbFileInfo, flags);


				var icon = Imaging.CreateBitmapSourceFromHIcon(info.hIcon,
															   Int32Rect.Empty,
															   BitmapSizeOptions.FromEmptyOptions());
				Interop.DestroyIcon(info.hIcon);
				return icon;
			}
			catch
			{
				return null;
			}
		}

		public static void BringProcessToFront(Process process)
		{
			var handle = process.MainWindowHandle;
			if (Interop.IsIconic(handle))
				Interop.ShowWindow(handle, Interop.SW_RESTORE);

			Interop.SetForegroundWindow(handle);
		}

		public static string WhereSearch(string filename)
		{
			var paths = new[] { Environment.CurrentDirectory }
				.Concat((Environment.GetEnvironmentVariable("PATH") ?? string.Empty).Split(';'));
			var extensions = new[] { String.Empty }
				.Concat((Environment.GetEnvironmentVariable("PATHEXT") ?? string.Empty).Split(';')
					.Where(e => e.StartsWith(".")));
			var combinations = paths.SelectMany(x => extensions,
				(path, extension) => Path.Combine(path, filename + extension));
			return combinations.FirstOrDefault(File.Exists);
		}
	}
}
