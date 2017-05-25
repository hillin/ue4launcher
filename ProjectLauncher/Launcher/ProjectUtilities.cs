using System.IO;
using System.Linq;

namespace UE4Launcher.Launcher
{
	internal static class ProjectUtilities
    {

        public static bool IsValidRootPath(string location)
        {
            try
            {
                return Directory.GetDirectories(location).Any(ProjectUtilities.IsValidProjectPath);
            }
            catch
            {
                return false;
            }
        }

        public static bool IsValidProjectPath(string folder)
        {
            try
            {
                return Directory.GetFiles(folder, "*.uproject", SearchOption.TopDirectoryOnly).Any();
            }
            catch
            {
                return false;
            }
        }
    }
}
