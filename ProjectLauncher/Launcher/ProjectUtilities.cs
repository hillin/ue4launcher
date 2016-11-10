using System.IO;
using System.Linq;

namespace ProjectLauncher.Launcher
{
    static class ProjectUtilities
    {

        public static bool IsValidRootPath(string location)
        {
            return Directory.GetDirectories(location).Any(ProjectUtilities.IsValidProjectPath);
        }

        public static bool IsValidProjectPath(string folder)
        {
            return Directory.GetFiles(folder, "*.uproject", SearchOption.TopDirectoryOnly).Any();
        }
    }
}
