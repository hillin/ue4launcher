using System.Collections.Generic;
using LaunchModeEnum = UE4Launcher.Launcher.LaunchMode;
using WindowStateEnum = UE4Launcher.Launcher.WindowState;

namespace UE4Launcher.Launcher
{
    static class Arguments
    {
        private static readonly Dictionary<string, ArgumentInfo> _arguments
            = new Dictionary<string, ArgumentInfo>();

        public static ArgumentInfo GetArgument(string name)
        {
            ArgumentInfo argumentInfo;
            return _arguments.TryGetValue(name.ToLowerInvariant(), out argumentInfo) ? argumentInfo : null;
        }

        static Arguments()
        {
            foreach (var field in typeof(Arguments).GetFields())
            {
                if (typeof(ArgumentInfo).IsAssignableFrom(field.FieldType))
                {
                    var value = (ArgumentInfo)field.GetValue(null);
                    _arguments.Add(value.Name.ToLowerInvariant(), value);
                }
            }
        }

        public static readonly ArgumentInfo LaunchMode =
            new MappingArgumentInfo<LaunchModeEnum>("LaunchMode", new Dictionary<LaunchMode, string>
            {
                {LaunchModeEnum.Game, "game"},
                {LaunchModeEnum.Server, "server"},
            }, "Set run mode", restrictedToOpenMode: OpenMode.LocalMap);

        public static readonly ArgumentInfo PlayerName = new ArgumentInfo("PlayerName", "name", "Player name to use",
                                                                          ArgumentType.Option, true, quoteParameter: true);

        public static readonly ArgumentInfo ListenServer = new ArgumentInfo("ListenServer", "listen",
                                                                            "Specify server as a listen server",
                                                                            ArgumentType.Option);

        public static readonly ArgumentInfo IsLanMatch = new ArgumentInfo("IsLanMatch", "bIsLanMatch",
                                                                          "Set whether multiplayer game is on the local network",
                                                                          ArgumentType.Option, true, 0);

        public static readonly ArgumentInfo SpectatorMode = new ArgumentInfo("SpectatorMode", "spectatoronly",
                                                                             "Start the game in spectator mode",
                                                                             ArgumentType.Option, true, 1);

        public static readonly ArgumentInfo Port = new ArgumentInfo("Port", "PORT",
                                                                     "Tells the engine to use a specific port number",
                                                                     ArgumentType.Switch, true, 7777);

        public static readonly ArgumentInfo DoubleNetUpdate = new ArgumentInfo("DoubleNetUpdate", "LANPLAY",
                                                                              "Tells the engine to not cap client bandwidth when connecting to servers. Causes double the amount of server updates and can saturate client's bandwidth");

        public static readonly ArgumentInfo VrMode = new ArgumentInfo("VrMode", "VR", "Force VR mode");

        public static readonly ArgumentInfo WindowState =
            new MappingArgumentInfo<WindowState>("WindowState", new Dictionary<WindowStateEnum, string>
            {
                {WindowStateEnum.FullScreen, "FULLSCREEN"},
                {WindowStateEnum.Windowed, "WINDOWED"},
            }, "Set game to run in fullscreen or windowed mode");

        public static readonly ArgumentInfo ResolutionX = new ArgumentInfo("ResolutionX", "ResX",
                                                                           "Set horizontal resolution for game window",
                                                                           ArgumentType.Switch, true, 1920);

        public static readonly ArgumentInfo ResolutionY = new ArgumentInfo("ResolutionY", "ResY",
                                                                           "Set vertical resolution for game window",
                                                                           ArgumentType.Switch, true, 1080);

        public static readonly ArgumentInfo WindowPositionX = new ArgumentInfo("WindowPositionX", "WinX",
                                                                               "Set the horizontal position of the game window on the screen",
                                                                               ArgumentType.Switch, true, 0);

        public static readonly ArgumentInfo WindowPositionY = new ArgumentInfo("WindowPositionY", "WinY",
                                                                               "Set the vertical position of the game window on the screen",
                                                                               ArgumentType.Switch, true, 0);

        public static readonly ArgumentInfo VSync =
            new MappingArgumentInfo<int>("VSync", new Dictionary<int, string>
            {
                {1, "VSYNC"},
                {0, "NOVSYNC"},
            }, "Prevents tearing of the image but costs fps and causes input latency");

        public static readonly ArgumentInfo ShowLogConsole = new ArgumentInfo("ShowLogConsole", "log",
                                                                              "opens a seperate window to display the contents of the log in real time");

        public static readonly ArgumentInfo ConsolePositionX = new ArgumentInfo("ConsolePositionX", "ConsoleX",
                                                                       "Set the horizontal position of the console output window",
                                                                       ArgumentType.Switch, true, 0);

        public static readonly ArgumentInfo ConsolePositionY = new ArgumentInfo("ConsolePositionY", "ConsoleY",
                                                                               "Set the vertical position of the console output window",
                                                                               ArgumentType.Switch, true, 0);

        public static readonly ArgumentInfo LogFilename = new ArgumentInfo("LogFilename", "log",
                                                                           "tells the engine to use the log filename of the string",
                                                                           ArgumentType.Switch, true, quoteParameter: true);


        public static readonly ArgumentInfo NoWriteLogFile = new ArgumentInfo("NoWriteLogFile", "NOWRITE", " Disable output to log");

        public static readonly ArgumentInfo LogTime =
            new MappingArgumentInfo<int>("LogTime", new Dictionary<int, string>
            {
                {1, "LOGTIMES"},
                {0, "NOLOGTIMES"},
            }, "Print time with log output");

        public static readonly ArgumentInfo Verbose = new ArgumentInfo("Verbose", "VERBOSE", "Set script compiler to use verbose output");

        public static readonly ArgumentInfo SilentMode = new ArgumentInfo("SilentMode", "SILENT", "Disable output and feedback");

        public static readonly ArgumentInfo OverrideDefaultEngine = new ConfigFileArgumentInfo("OverrideDefaultEngine", "DEFENGINEINI",
                                                                                     "Override the default engine file",
                                                                                     ArgumentType.Switch, true,
                                                                                     "defaultengine.ini");

        public static readonly ArgumentInfo OverrideDefaultGame = new ConfigFileArgumentInfo("OverrideDefaultGame", "DEFGAMEINI",
                                                                                     "Override the default game file",
                                                                                     ArgumentType.Switch, true,
                                                                                     "defaultgame.ini");

        public static readonly ArgumentInfo OverrideDefaultEditor = new ConfigFileArgumentInfo("OverrideDefaultEditor", "DEFEDITORINI",
                                                                                     "Override the default editor file",
                                                                                     ArgumentType.Switch, true,
                                                                                     "defaulteditor.ini");

        public static readonly ArgumentInfo OverrideDefaultInput = new ConfigFileArgumentInfo("OverrideDefaultInput", "DEFINPUTINI",
                                                                                     "Override the default input file",
                                                                                     ArgumentType.Switch, true,
                                                                                     "defaultinput.ini");

        public static readonly ArgumentInfo NoSound = new ArgumentInfo("NoSound", "NOSOUND", "Disable any sound output from the engine");
        public static readonly ArgumentInfo NoSplash = new ArgumentInfo("NoSplash", "NOSPLASH", "Disable use of splash image when loading game");

        public static readonly ArgumentInfo Culture = new ArgumentInfo("Culture", "CULTURE",
                                                                       "Override culture",
                                                                       ArgumentType.Switch, true,
                                                                       "en", true);
    }
}
