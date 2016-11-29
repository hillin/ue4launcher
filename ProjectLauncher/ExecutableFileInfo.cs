using System.Collections.Generic;
using System.IO;
using System.Linq;
using UE4Launcher.Launcher;
using IOPath = System.IO.Path;

namespace UE4Launcher
{
    class ExecutableFileInfo
    {
        public static readonly ExecutableFileInfo[] ExecutableFiles;

        static ExecutableFileInfo()
        {

            var executables = new List<ExecutableFileInfo>();
            var rootPath = App.CurrentRootPath;
            var binariesFolder = IOPath.Combine(rootPath, "Engine", "Binaries");

            executables.AddRange(
                Directory.GetFiles(binariesFolder, "UE4*.exe", SearchOption.AllDirectories)
                         .Select(
                             file =>
                                 new ExecutableFileInfo(file, file.Substring(rootPath.Length + 1),
                                                        file.Substring(binariesFolder.Length + 1),
                                                        ExecutableType.Editor)));

            foreach (var project in ProjectInfo.Projects)
            {
                var file = IOPath.GetFullPath(IOPath.Combine(project.Path, "..", project.Name + ".exe"));
                if (File.Exists(file))
                    executables.Add(new ExecutableFileInfo(file, file.Substring(rootPath.Length + 1),
                                                           IOPath.GetFileName(file), ExecutableType.Build));
            }

            ExecutableFiles = executables.ToArray();
        }

        public string Path { get; }
        public string ProjectRelativePath { get; }
        public string DisplayName { get; }
        public string ProcessName { get; }
        public ExecutableType ExecutableType { get; }

        public ExecutableFileInfo(string path, string projectRelativePath, string displayName, ExecutableType executableType)
        {
            this.Path = path;
            this.ProcessName = IOPath.GetFileNameWithoutExtension(path);
            this.ProjectRelativePath = projectRelativePath;
            this.ExecutableType = executableType;
            this.DisplayName = displayName;
        }

    }
}
