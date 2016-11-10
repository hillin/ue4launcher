using System.Collections.Generic;

namespace ProjectLauncher.Launcher
{
    class MappingArgumentInfo<T> : ArgumentInfo
    {
        private readonly Dictionary<T, string> _commandMap;
        
        public MappingArgumentInfo(string name, Dictionary<T, string> commandMap, string description,
                                   ArgumentType argumentType = ArgumentType.Switch,
                                   bool hasParameter = false, object defaultParameter = null, OpenMode? restrictedToOpenMode = null)
            : base(name)
        {
            _commandMap = commandMap;
            this.Description = description;
            this.ArgumentType = argumentType;
            this.HasParameter = hasParameter;
            this.DefaultParameter = defaultParameter;
            this.RestrictedToOpenMode = restrictedToOpenMode;
        }
        
        protected override string GetCommand(Argument argument, LaunchProfile launchProfile)
        {
            return _commandMap[(T)argument.Parameter];
        }
    }
}
