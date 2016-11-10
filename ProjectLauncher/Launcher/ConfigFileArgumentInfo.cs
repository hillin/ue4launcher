using System.IO;
using System.Windows;

namespace ProjectLauncher.Launcher
{
    class ConfigFileArgumentInfo : ArgumentInfo
    {
        public ConfigFileArgumentInfo(string name, string command, string description,
                                      ArgumentType argumentType = ArgumentType.Switch, bool hasParameter = false,
                                      object defaultParameter = null)
            : base(name, command, description, argumentType, hasParameter, defaultParameter, true)
        {
        }

        protected override object GetParameter(Argument argument, LaunchProfile launchProfile)
        {
            var parameter = (string)argument.Parameter;

            if (Path.IsPathRooted(parameter))
                return this.HandleQuoteParamter(parameter);

            return this.HandleQuoteParamter(
                Path.GetFullPath(Path.Combine(((App)Application.Current).RootPath,
                                              launchProfile.ProjectName,
                                              "Config",
                                              parameter)));
        }
    }
}
