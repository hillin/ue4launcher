﻿using System.IO;
using System.Linq;
using UE4Launcher.Launcher;

namespace UE4Launcher
{
	internal class ProjectInfo
    {
        public static readonly ProjectInfo[] Projects;

        static ProjectInfo()
        {
            Projects =
               Directory.GetDirectories(App.CurrentRootPath)
                        .Where(ProjectUtilities.IsValidProjectPath)
                        .Select(folder => new ProjectInfo(folder))
                        .ToArray();
        }

        public string Path { get; }
        public string Name { get; }

        public ProjectInfo(string path)
        {
            this.Path = path;
            this.Name = System.IO.Path.GetFileName(path);
        }

    }
}
