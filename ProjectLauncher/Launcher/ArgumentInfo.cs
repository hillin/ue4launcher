using System;
using System.Text;

namespace ProjectLauncher.Launcher
{
    public class ArgumentInfo
    {
        public string Name { get; }
        protected string Command { get; }
        public string Description { get; protected set; }
        public ArgumentType ArgumentType { get; protected set; }
        public bool HasParameter { get; protected set; }
        public object DefaultParameter { get; protected set; }
        public bool QuoteParameter { get; }
        public OpenMode? RestrictedToOpenMode { get; protected set; }

        public ArgumentInfo(string name, string command, string description,
                            ArgumentType argumentType = ArgumentType.Switch, bool hasParameter = false,
                            object defaultParameter = null, bool quoteParameter = false,
                            OpenMode? restrictedToOpenMode = null)
            : this(name)
        {
            this.Command = command;
            this.Description = description;
            this.ArgumentType = argumentType;
            this.HasParameter = hasParameter;
            this.DefaultParameter = defaultParameter;
            this.QuoteParameter = quoteParameter;
            this.RestrictedToOpenMode = restrictedToOpenMode;
        }

        protected ArgumentInfo(string name)
        {
            this.Name = name;
        }

        protected virtual string GetCommand(Argument argument, LaunchProfile launchProfile)
        {
            return this.Command;
        }

        public virtual void ToCommandLine(Argument argument, StringBuilder builder, LaunchProfile launchProfile)
        {
            if (this.RestrictedToOpenMode != null && this.RestrictedToOpenMode.Value != launchProfile.OpenMode)
                return;

            this.WritePrefix(builder);

            builder.Append(this.GetCommand(argument, launchProfile));
            if (this.HasParameter)
                builder.Append('=').Append(this.GetParameter(argument, launchProfile));
        }

        protected virtual object GetParameter(Argument argument, LaunchProfile launchProfile)
        {
            return this.HandleQuoteParamter(argument.Parameter);
        }

        protected object HandleQuoteParamter(object parameter)
        {
            return this.QuoteParameter ? $"\"{parameter}\"" : parameter;
        }

        protected void WritePrefix(StringBuilder builder)
        {
            switch (this.ArgumentType)
            {
                case ArgumentType.Option:
                    builder.Append('?');
                    break;
                case ArgumentType.Switch:
                    builder.Append(" -");
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }
        }
    }
}
