using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Xml.Serialization;

namespace UE4Launcher.Launcher
{
    [Serializable]
    public class LaunchProfile
    {
        [XmlAttribute]
        public string ProfileName { get; set; } = "New Profile";
        [XmlAttribute]
        public string ProfileDescription { get; set; } = "<No description>";

        public ProfileStorage ProfileStorage { get; set; } = ProfileStorage.Public;
        public string ExecutableFile { get; set; } = "Engine\\Binaries\\Win64\\UE4Editor.exe";
        public bool IsBuild { get; set; }
        public OpenMode OpenMode { get; set; }
        public string ProjectName { get; set; }
        public string Map { get; set; }
        public string Url { get; set; } = "127.0.0.1";

        public List<Argument> Arguments { get; set; }

        public string AdditionalParameters { get; set; }
        public string AdditionalOptions { get; set; }

        public LaunchProfile()
        {
            this.Arguments = new List<Argument>();
        }


        public string GetExecutableFile()
        {
            return Path.Combine(App.CurrentRootPath, this.ExecutableFile);
        }


        public string GetCommandLineArguments()
        {
            var builder = new StringBuilder();

            switch (this.OpenMode)
            {
                case OpenMode.LocalMap:
                    if (!this.IsBuild && !string.IsNullOrEmpty(this.ProjectName))
                        builder.Append(' ').Append(this.ProjectName);

                    if (!string.IsNullOrEmpty(this.Map))
                        builder.Append(' ').Append(this.Map);

                    break;
                case OpenMode.Connect:
                    if (!this.IsBuild && !string.IsNullOrEmpty(this.ProjectName))
                        builder.Append(' ').Append(this.ProjectName);

                    if (!string.IsNullOrEmpty(this.Url))
                        builder.Append(' ').Append(this.Url);

                    break;
            }

            foreach (var argument in this.Arguments
                                         .Where(a => a.ArgumentInfo.ArgumentType == ArgumentType.Option))
            {
                argument.ToCommandLine(builder, this);
            }

            if (!string.IsNullOrWhiteSpace(this.AdditionalOptions))
                builder.Append(this.AdditionalOptions);

            if (this.OpenMode == OpenMode.Connect)
            {
                builder.Append(" -game");
            }

            foreach (var argument in this.Arguments
                                         .Where(a => a.ArgumentInfo.ArgumentType == ArgumentType.Switch))
            {
                argument.ToCommandLine(builder, this);
            }

            if (!string.IsNullOrWhiteSpace(this.AdditionalParameters))
                builder.Append(' ').Append(this.AdditionalParameters);

            return builder.ToString();
        }

        public bool GetHasArgument(ArgumentInfo info)
        {
            return this.Arguments.Any(a => a.ArgumentInfo == info);
        }

        public bool GetArgumentParameter<T>(ArgumentInfo info, out T value)
        {
            var argument = this.Arguments.FirstOrDefault(a => a.ArgumentInfo == info);
            if (argument != null)
            {
                value = (T)argument.Parameter;
                return true;
            }

            value = default(T);
            return false;
        }

        public bool GetArgumentEnumParameter<T>(ArgumentInfo info, out T? value)
            where T : struct
        {
            var argument = this.Arguments.FirstOrDefault(a => a.ArgumentInfo == info);
            if (argument != null)
            {
                if (argument.Parameter is T)
                {
                    value = (T?)argument.Parameter;
                    return true;
                }

                value = (T?)Enum.ToObject(typeof(T), argument.Parameter);
                return true;
            }

            value = null;
            return false;
        }


        public object GetArgumentParameter(ArgumentInfo info)
        {
            return this.Arguments.FirstOrDefault(a => a.ArgumentInfo == info)?.Parameter;
        }

        public bool SetArgumentParameter(ArgumentInfo info, object value)
        {
            var argument = this.Arguments.FirstOrDefault(a => a.ArgumentInfo == info);
            if (argument != null)
            {
                argument.Parameter = value;
                return true;
            }

            return false;
        }

        /// <returns>true if enable state changed</returns>
        public bool SetEnableArgument(ArgumentInfo info, bool enabled, object parameter = null)
        {
            if (enabled)
            {
                var argument = this.Arguments.FirstOrDefault(a => a.ArgumentInfo == info);
                if (argument != null)
                {
                    if (argument.Parameter.Equals(parameter))
                        return false;
                    argument.Parameter = parameter;
                    return true;
                }

                argument = new Argument(info, parameter);
                this.Arguments.Add(argument);
                return true;

            }

            return this.Arguments.RemoveAll(a => a.ArgumentInfo == info) > 0;
        }

        public bool SetEnableArgument(ArgumentInfo info, bool? enabled)
        {
            return this.SetEnableArgument(info, enabled != null, enabled == true ? 1 : 0);
        }

        public LaunchProfile Clone()
        {
            var clone = (LaunchProfile)this.MemberwiseClone();
            clone.Arguments = this.Arguments.Select(a => a.Clone()).ToList();
            return clone;
        }
    }
}
