using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using UE4Launcher.Debugging;
using UE4Launcher.Root;
using Process = System.Diagnostics.Process;


namespace UE4Launcher.Launcher
{
    internal class LaunchProfileViewModel : NotificationObject, ITrayContextMenuItem
    {
        private static readonly ImageSource SharedIcon =
            BitmapFrame.Create(new Uri("pack://application:,,,/Resources/Images/ic_play_circle_outline_black_18dp.png"));

        public LaunchProfile Profile { get; }
        public bool DeveloperMode { get; }
        public bool EditMode { get; }

        private bool _isModified;
        public bool IsModified
        {
            get { return _isModified; }
            set
            {
                _isModified = value;
                this.RaisePropertyChanged(nameof(this.IsModified));
            }
        }

        public string ProfileName
        {
            get { return this.Profile.ProfileName; }
            set
            {
                this.Profile.ProfileName = value;
                this.RaiseProfilePropertyChanged(nameof(this.ProfileName));
            }
        }

        public string ProfileDescription
        {
            get { return this.Profile.ProfileDescription; }
            set
            {
                this.Profile.ProfileDescription = value;
                this.RaiseProfilePropertyChanged(nameof(this.ProfileDescription));
            }
        }

        public ProfileStorage ProfileStorage
        {
            get { return this.Profile.ProfileStorage; }
            set
            {
                this.Profile.ProfileStorage = value;
                this.RaiseProfilePropertyChanged(nameof(this.ProfileStorage));
            }
        }

        public bool IsLaunchingAsServer => this.LaunchMode == LaunchMode.Server;

        public bool IsListenServerOrClient
            =>
            this.Profile.OpenMode == OpenMode.Connect || (this.LaunchMode == LaunchMode.Server && this.IsListenServer);

        public bool IsListenServer
        {
            get { return this.Profile.GetHasArgument(Arguments.ListenServer); }
            set
            {
                this.Profile.SetEnableArgument(Arguments.ListenServer, value);
                this.RaiseProfilePropertyChanged(nameof(this.IsListenServer));
                this.RaisePropertyChanged(nameof(this.IsListenServerOrClient));
            }
        }

        public bool ShowProjectSelector => this.SelectedExecutableFile.ExecutableType == ExecutableType.Editor;

        public bool UseLocalMap
        {
            get { return this.Profile.OpenMode == OpenMode.LocalMap; }
            set
            {
                this.Profile.OpenMode = value ? OpenMode.LocalMap : OpenMode.Connect;
                this.RaiseProfilePropertyChanged(nameof(this.UseLocalMap));
                this.RaiseProfilePropertyChanged(nameof(this.ConnectToServer));
                this.RaisePropertyChanged(nameof(this.IsListenServerOrClient));
                this.OpenModeChanged?.Invoke(this, EventArgs.Empty);
            }
        }

        private bool _specifyMapName;

        public bool SpecifyMapName
        {
            get { return _specifyMapName; }
            set
            {
                _specifyMapName = value;
                this.UpdateProfileMap();
                this.RaiseProfilePropertyChanged(nameof(this.SpecifyMapName));
            }
        }

        public bool SpecifyLaunchMode
        {
            get { return this.Profile.GetHasArgument(Arguments.LaunchMode); }
            set
            {
                if (this.Profile.SetEnableArgument(Arguments.LaunchMode, value, this.LaunchMode))
                {
                    this.RaiseProfilePropertyChanged(nameof(this.SpecifyLaunchMode));
                    this.RaisePropertyChanged(nameof(this.IsListenServerOrClient));
                    this.RaisePropertyChanged(nameof(this.IsLaunchingAsServer));
                }
            }
        }


        private LaunchMode _launchMode;
        public LaunchMode LaunchMode
        {
            get { return this.GetParameterEnumValue(Arguments.LaunchMode, ref _launchMode); }
            set
            {
                _launchMode = value;
                this.Profile.SetArgumentParameter(Arguments.LaunchMode, value);
                this.RaiseProfilePropertyChanged(nameof(this.LaunchMode));
                this.RaisePropertyChanged(nameof(this.IsListenServerOrClient));
                this.RaisePropertyChanged(nameof(this.IsLaunchingAsServer));
            }
        }

        public event EventHandler OpenModeChanged;

        public bool ConnectToServer
        {
            get { return this.Profile.OpenMode == OpenMode.Connect; }
            set
            {
                this.Profile.OpenMode = value ? OpenMode.Connect : OpenMode.LocalMap;
                this.RaiseProfilePropertyChanged(nameof(this.UseLocalMap));
                this.RaiseProfilePropertyChanged(nameof(this.ConnectToServer));
                this.RaisePropertyChanged(nameof(this.IsListenServerOrClient));
                this.OpenModeChanged?.Invoke(this, EventArgs.Empty);
            }
        }


        public string IPSection0
        {
            get { return this.GetIPSection(0); }
            set
            {
                this.SetIPSection(0, value);
                this.RaiseProfilePropertyChanged(nameof(this.IPSection0));
            }
        }

        public string IPSection1
        {
            get { return this.GetIPSection(1); }
            set
            {
                this.SetIPSection(1, value);
                this.RaiseProfilePropertyChanged(nameof(this.IPSection1));
            }
        }

        public string IPSection2
        {
            get { return this.GetIPSection(2); }
            set
            {
                this.SetIPSection(2, value);
                this.RaiseProfilePropertyChanged(nameof(this.IPSection2));
            }
        }

        public string IPSection3
        {
            get { return this.GetIPSection(3); }
            set
            {
                this.SetIPSection(3, value);
                this.RaiseProfilePropertyChanged(nameof(this.IPSection3));
            }
        }

        public event EventHandler UrlChanged;

        private void SetIPSection(int index, string value)
        {
            var sections = this.Profile.Url?.Split('.').ToList() ?? new List<string>();
            for (var i = sections.Count; i <= index; ++i)
            {
                sections.Add(string.Empty);
            }
            sections[index] = value;
            this.Profile.Url = string.Join(".", sections);
            this.UrlChanged?.Invoke(this, EventArgs.Empty);
        }

        private string GetIPSection(int i)
        {
            var sections = (this.Profile.Url ?? "127.0.0.1").Split('.');
            if (sections.Length <= i)
                return string.Empty;

            return sections[i];
        }

        public bool SpecifyPort
        {
            get { return this.Profile.GetHasArgument(Arguments.Port); }
            set
            {
                if (this.Profile.SetEnableArgument(Arguments.Port, value, this.Port))
                    this.RaiseProfilePropertyChanged(nameof(this.SpecifyPort));
            }
        }

        private int _port = 7777;
        public int Port
        {
            get
            {
                return this.GetParameterValue(Arguments.Port, ref _port);
            }
            set
            {
                _port = value;
                this.Profile.SetArgumentParameter(Arguments.Port, value);
                this.RaiseProfilePropertyChanged(nameof(this.Port));
            }
        }

        public bool? AsSpectator
        {
            get { return this.Profile.GetHasArgument(Arguments.SpectatorMode); }
            set
            {
                if (this.Profile.SetEnableArgument(Arguments.SpectatorMode, value))
                    this.RaiseProfilePropertyChanged(nameof(this.AsSpectator));
            }
        }

        public bool? LanGame
        {
            get { return this.Profile.GetHasArgument(Arguments.IsLanMatch); }
            set
            {
                if (this.Profile.SetEnableArgument(Arguments.IsLanMatch, value))
                    this.RaiseProfilePropertyChanged(nameof(this.LanGame));
            }
        }

        public bool DoubleNetUpdate
        {
            get { return this.Profile.GetHasArgument(Arguments.DoubleNetUpdate); }
            set
            {
                if (this.Profile.SetEnableArgument(Arguments.DoubleNetUpdate, value))
                    this.RaiseProfilePropertyChanged(nameof(this.DoubleNetUpdate));
            }
        }

        public bool SpecifyNickname
        {
            get { return this.Profile.GetHasArgument(Arguments.PlayerName); }
            set
            {
                if (this.Profile.SetEnableArgument(Arguments.PlayerName, value, this.Nickname))
                    this.RaiseProfilePropertyChanged(nameof(this.SpecifyNickname));
            }
        }

        private string _nickname = "player";

        public string Nickname
        {
            get
            {
                return this.GetParameterReference(Arguments.PlayerName, ref _nickname);
            }
            set
            {
                _nickname = value;
                this.Profile.SetArgumentParameter(Arguments.PlayerName, _nickname);
                this.RaiseProfilePropertyChanged(nameof(this.Nickname));
            }
        }

        public bool VrMode
        {
            get { return this.Profile.GetHasArgument(Arguments.VrMode); }
            set
            {
                if (this.Profile.SetEnableArgument(Arguments.VrMode, value)) this.RaiseProfilePropertyChanged(nameof(this.VrMode));
            }
        }

        public bool SpecifyWindowState
        {
            get { return this.Profile.GetHasArgument(Arguments.WindowState); }
            set
            {
                if (this.Profile.SetEnableArgument(Arguments.WindowState, value, this.WindowState))
                    this.RaiseProfilePropertyChanged(nameof(this.SpecifyWindowState));
            }
        }

        private WindowState _windowState = WindowState.FullScreen;

        public WindowState WindowState
        {
            get
            {
                return this.GetParameterEnumValue(Arguments.WindowState, ref _windowState);
            }
            set
            {
                _windowState = value;
                this.Profile.SetArgumentParameter(Arguments.WindowState, _windowState);
                this.RaiseProfilePropertyChanged(nameof(this.WindowState));
            }
        }

        public bool SpecifyResolution
        {
            get { return this.Profile.GetHasArgument(Arguments.ResolutionX); }
            set
            {
                if (this.Profile.SetEnableArgument(Arguments.ResolutionX, value, this.ResolutionX)
                    && this.Profile.SetEnableArgument(Arguments.ResolutionY, value, this.ResolutionY))
                    this.RaiseProfilePropertyChanged(nameof(this.SpecifyResolution));

            }
        }

        private int _resolutionX = 1920;

        public int ResolutionX
        {
            get
            {
                return this.GetParameterValue(Arguments.ResolutionX, ref _resolutionX);
            }
            set
            {
                _resolutionX = value;
                this.Profile.SetArgumentParameter(Arguments.ResolutionX, _resolutionX);
                this.RaiseProfilePropertyChanged(nameof(this.ResolutionX));
            }
        }

        private int _resolutionY = 1080;

        public int ResolutionY
        {
            get
            {
                return this.GetParameterValue(Arguments.ResolutionY, ref _resolutionY);
            }
            set
            {
                _resolutionY = value;
                this.Profile.SetArgumentParameter(Arguments.ResolutionY, _resolutionY);
                this.RaiseProfilePropertyChanged(nameof(this.ResolutionY));
            }
        }

        public bool SpecifyWindowPosition
        {
            get { return this.Profile.GetHasArgument(Arguments.WindowPositionX); }
            set
            {
                if (this.Profile.SetEnableArgument(Arguments.WindowPositionX, value, this.WindowPositionX)
                    && this.Profile.SetEnableArgument(Arguments.WindowPositionY, value, this.WindowPositionY))
                    this.RaiseProfilePropertyChanged(nameof(this.SpecifyWindowPosition));
            }
        }

        private int _windowPositionX;

        public int WindowPositionX
        {
            get
            {
                return this.GetParameterValue(Arguments.WindowPositionX, ref _windowPositionX);
            }
            set
            {
                _windowPositionX = value;
                this.Profile.SetArgumentParameter(Arguments.WindowPositionX, _windowPositionX);
                this.RaiseProfilePropertyChanged(nameof(this.WindowPositionX));
            }
        }

        private int _windowPositionY;

        public int WindowPositionY
        {
            get
            {
                return this.GetParameterValue(Arguments.WindowPositionY, ref _windowPositionY);
            }
            set
            {
                _windowPositionY = value;
                this.Profile.SetArgumentParameter(Arguments.WindowPositionY, _windowPositionY);
                this.RaiseProfilePropertyChanged(nameof(this.WindowPositionY));
            }
        }

        public bool? UseVSync
        {
            get
            {
                int? useVSync;
                if (!this.Profile.GetArgumentParameter(Arguments.VSync, out useVSync))
                    return null;

                switch (useVSync)
                {
                    case null:
                        return null;
                    default:
                        return useVSync == 1;
                }
            }
            set
            {
                if (this.Profile.SetEnableArgument(Arguments.VSync, value))
                    this.RaiseProfilePropertyChanged(nameof(this.UseVSync));
            }
        }

        public bool ShowLogConsole
        {
            get { return this.Profile.GetHasArgument(Arguments.ShowLogConsole); }
            set
            {
                if (this.Profile.SetEnableArgument(Arguments.ShowLogConsole, value))
                    this.RaiseProfilePropertyChanged(nameof(this.ShowLogConsole));
            }
        }

        public bool SpecifyConsolePosition
        {
            get { return this.Profile.GetHasArgument(Arguments.ConsolePositionX); }
            set
            {
                if (this.Profile.SetEnableArgument(Arguments.ConsolePositionX, value, this.ConsolePositionX)
                    && this.Profile.SetEnableArgument(Arguments.ConsolePositionY, value, this.ConsolePositionY))
                    this.RaiseProfilePropertyChanged(nameof(this.SpecifyConsolePosition));
            }
        }

        private int _consolePositionX;

        public int ConsolePositionX
        {
            get
            {
                return this.GetParameterValue(Arguments.ConsolePositionX, ref _consolePositionX);
            }
            set
            {
                _consolePositionX = value;
                this.Profile.SetArgumentParameter(Arguments.ConsolePositionX, _consolePositionX);
                this.RaiseProfilePropertyChanged(nameof(this.ConsolePositionX));
            }
        }

        private int _consolePositionY;

        public int ConsolePositionY
        {
            get
            {
                return this.GetParameterValue(Arguments.ConsolePositionY, ref _consolePositionY);
            }
            set
            {
                _consolePositionY = value;
                this.Profile.SetArgumentParameter(Arguments.ConsolePositionY, _consolePositionY);
                this.RaiseProfilePropertyChanged(nameof(this.ConsolePositionY));
            }
        }

        public bool WriteToLogFile
        {
            get { return !this.Profile.GetHasArgument(Arguments.NoWriteLogFile); }
            set
            {
                if (this.Profile.SetEnableArgument(Arguments.NoWriteLogFile, !value))
                    this.RaiseProfilePropertyChanged(nameof(this.WriteToLogFile));
            }
        }

        public bool SpecifyLogFilename
        {
            get { return this.Profile.GetHasArgument(Arguments.LogFilename); }
            set
            {
                if (this.Profile.SetEnableArgument(Arguments.LogFilename, value, this.LogFilename))
                    this.RaiseProfilePropertyChanged(nameof(this.SpecifyLogFilename));
            }
        }

        private string _logFilename = "mylog.log";

        public string LogFilename
        {
            get
            {
                return this.GetParameterReference(Arguments.LogFilename, ref _logFilename);
            }
            set
            {
                _logFilename = value;
                this.Profile.SetArgumentParameter(Arguments.LogFilename, _logFilename);
                this.RaiseProfilePropertyChanged(nameof(this.LogFilename));
            }
        }

        public bool VerboseLog
        {
            get { return this.Profile.GetHasArgument(Arguments.Verbose); }
            set
            {
                if (this.Profile.SetEnableArgument(Arguments.Verbose, value))
                    this.RaiseProfilePropertyChanged(nameof(this.VerboseLog));
            }
        }

        public bool? LogTime
        {
            get
            {
                int? logTime;
                if (!this.Profile.GetArgumentParameter(Arguments.LogTime, out logTime))
                    return null;

                switch (logTime)
                {
                    case null:
                        return null;
                    default:
                        return logTime == 1;
                }

            }
            set
            {
                if (this.Profile.SetEnableArgument(Arguments.LogTime, value))
                    this.RaiseProfilePropertyChanged(nameof(this.LogTime));
            }
        }

        public bool SilentMode
        {
            get { return this.Profile.GetHasArgument(Arguments.SilentMode); }
            set
            {
                if (this.Profile.SetEnableArgument(Arguments.SilentMode, value))
                    this.RaiseProfilePropertyChanged(nameof(this.SilentMode));
            }
        }

        public bool OverrideDefaultEngine
        {
            get { return this.Profile.GetHasArgument(Arguments.OverrideDefaultEngine); }
            set
            {
                if (this.Profile.SetEnableArgument(Arguments.OverrideDefaultEngine, value, this.DefaultEngineFilename))
                    this.RaiseProfilePropertyChanged(nameof(this.OverrideDefaultEngine));
            }
        }

        private string _defaultEngineFilename;

        public string DefaultEngineFilename
        {
            get
            {
                return this.GetParameterReference(Arguments.OverrideDefaultEngine, ref _defaultEngineFilename);
            }
            set
            {
                if (_defaultEngineFilename == value) return;
                _defaultEngineFilename = value;
                this.Profile.SetArgumentParameter(Arguments.OverrideDefaultEngine, _defaultEngineFilename);
                this.RaiseProfilePropertyChanged(nameof(this.DefaultEngineFilename));
            }
        }


        public bool OverrideDefaultGame
        {
            get { return this.Profile.GetHasArgument(Arguments.OverrideDefaultGame); }
            set
            {
                if (this.Profile.SetEnableArgument(Arguments.OverrideDefaultGame, value, this.DefaultGameFilename))
                    this.RaiseProfilePropertyChanged(nameof(this.OverrideDefaultGame));
            }
        }

        private string _defaultGameFilename;

        public string DefaultGameFilename
        {
            get
            {
                return this.GetParameterReference(Arguments.OverrideDefaultGame, ref _defaultGameFilename);
            }
            set
            {
                if (_defaultGameFilename == value) return;
                _defaultGameFilename = value;
                this.Profile.SetArgumentParameter(Arguments.OverrideDefaultGame, _defaultGameFilename);
                this.RaiseProfilePropertyChanged(nameof(this.DefaultGameFilename));
            }
        }


        public bool OverrideDefaultEditor
        {
            get { return this.Profile.GetHasArgument(Arguments.OverrideDefaultEditor); }
            set
            {
                if (this.Profile.SetEnableArgument(Arguments.OverrideDefaultEditor, value, this.DefaultEditorFilename))
                    this.RaiseProfilePropertyChanged(nameof(this.OverrideDefaultEditor));
            }
        }

        private string _defaultEditorFilename;

        public string DefaultEditorFilename
        {
            get
            {
                return this.GetParameterReference(Arguments.OverrideDefaultEditor, ref _defaultEditorFilename);
            }
            set
            {
                if (_defaultEditorFilename == value) return;
                _defaultEditorFilename = value;
                this.Profile.SetArgumentParameter(Arguments.OverrideDefaultEditor, _defaultEditorFilename);
                this.RaiseProfilePropertyChanged(nameof(this.DefaultEditorFilename));
            }
        }


        public bool OverrideDefaultInput
        {
            get { return this.Profile.GetHasArgument(Arguments.OverrideDefaultInput); }
            set
            {
                if (this.Profile.SetEnableArgument(Arguments.OverrideDefaultInput, value, this.DefaultInputFilename))
                    this.RaiseProfilePropertyChanged(nameof(this.OverrideDefaultInput));
            }
        }

        private string _defaultInputFilename;

        public string DefaultInputFilename
        {
            get
            {
                return this.GetParameterReference(Arguments.OverrideDefaultInput, ref _defaultInputFilename);
            }
            set
            {
                if (_defaultInputFilename == value) return;
                _defaultInputFilename = value;
                this.Profile.SetArgumentParameter(Arguments.OverrideDefaultInput, _defaultInputFilename);
                this.RaiseProfilePropertyChanged(nameof(this.DefaultInputFilename));
            }
        }


        public bool SpecifyCulture
        {
            get { return this.Profile.GetHasArgument(Arguments.Culture); }
            set
            {
                if (this.Profile.SetEnableArgument(Arguments.Culture, value, this.Culture.Name))
                {
                    this.RaiseProfilePropertyChanged(nameof(this.SpecifyCulture));
                }
            }
        }

        private CultureViewModel _culture;
        public CultureViewModel Culture
        {
            get { return _culture; }
            set
            {
                _culture = value;
                this.Profile.SetArgumentParameter(Arguments.Culture, _culture?.Name ?? Arguments.Culture.DefaultParameter);
                this.RaiseProfilePropertyChanged(nameof(this.Culture));
            }
        }

        private CultureViewModel[] _availableCultures;

        public CultureViewModel[] AvailableCultures
        {
            get { return _availableCultures; }
            set
            {
                _availableCultures = value;
                this.RaiseProfilePropertyChanged(nameof(this.AvailableCultures));
            }
        }

        public bool NoSound
        {
            get { return this.Profile.GetHasArgument(Arguments.NoSound); }
            set
            {
                if (this.Profile.SetEnableArgument(Arguments.NoSound, value)) this.RaiseProfilePropertyChanged(nameof(this.NoSound));
            }
        }

        public bool NoSplash
        {
            get { return this.Profile.GetHasArgument(Arguments.NoSplash); }
            set
            {
                if (this.Profile.SetEnableArgument(Arguments.NoSplash, value))
                    this.RaiseProfilePropertyChanged(nameof(this.NoSplash));
            }
        }

        public string AdditionalParams
        {
            get { return this.Profile.AdditionalParameters; }
            set
            {
                this.Profile.AdditionalParameters = value;
                this.RaiseProfilePropertyChanged(nameof(this.AdditionalParams));
            }
        }

        public string AdditionalOptions
        {
            get { return this.Profile.AdditionalOptions; }
            set
            {
                this.Profile.AdditionalOptions = value;
                this.RaiseProfilePropertyChanged(nameof(this.AdditionalOptions));
            }
        }

        

        private ExecutableFileInfo[] _executableFiles;
        public ExecutableFileInfo[] ExecutableFiles
        {
            get { return _executableFiles; }
            set
            {
                _executableFiles = value;
                this.RaiseProfilePropertyChanged(nameof(this.ExecutableFiles));
            }
        }

        public event EventHandler SelectedExecutableFileChanged;

        private ExecutableFileInfo _selectedExecutableFile;
        public ExecutableFileInfo SelectedExecutableFile
        {
            get { return _selectedExecutableFile; }
            set
            {
                _selectedExecutableFile = value;
                this.Profile.ExecutableFile = value.ProjectRelativePath;
                this.Profile.IsBuild = value.ExecutableType == ExecutableType.Build;
                this.RaiseProfilePropertyChanged(nameof(this.SelectedExecutableFile));
                this.RaisePropertyChanged(nameof(this.ShowProjectSelector));
                this.SelectedExecutableFileChanged?.Invoke(this, EventArgs.Empty);
            }
        }



        private ProjectInfo[] _projects;

        public ProjectInfo[] Projects
        {
            get { return _projects; }
            set
            {
                _projects = value;
                this.RaiseProfilePropertyChanged(nameof(this.Projects));
            }
        }

        private ProjectInfo _selectedProject;
        public ProjectInfo SelectedProject
        {
            get { return _selectedProject; }
            set
            {
                _selectedProject = value;
                this.Profile.ProjectName = _selectedProject.Name;
                this.RaiseProfilePropertyChanged(nameof(this.SelectedProject));
                this.OnSelectedProjectChanged();
            }
        }


        private MapInfo[] _maps;

        public MapInfo[] Maps
        {
            get { return _maps; }
            set
            {
                if (_maps == value) return;
                _maps = value;
                this.RaiseProfilePropertyChanged(nameof(this.Maps));
            }
        }

        private MapInfo _selectedMap;
        public MapInfo SelectedMap
        {
            get { return _selectedMap; }
            set
            {
                _selectedMap = value;
                this.UpdateProfileMap();
                this.RaiseProfilePropertyChanged(nameof(this.SelectedMap));
            }
        }



        private string[] _iniFiles;

        public string[] IniFiles
        {
            get { return _iniFiles; }
            set
            {
                if (_iniFiles == value) return;
                _iniFiles = value;
                this.RaiseProfilePropertyChanged(nameof(this.IniFiles));
            }
        }


        public string ConsoleCommandLine => $"{this.Profile.GetExecutableFile()}{this.Profile.GetCommandLineArguments()}";



        string ITrayContextMenuItem.Name => this.ProfileName;

        ImageSource ITrayContextMenuItem.Icon => SharedIcon;

        bool ITrayContextMenuItem.IsEnabled => true;


        private readonly ICommand _trayContextMenuCommand;

        ICommand ITrayContextMenuItem.Command => _trayContextMenuCommand;

        string ITrayContextMenuItem.Description
            => $"{this.ProfileDescription}\nClick to launch this profile, Ctrl-click to launch with debugger attached";

        public LaunchProfileViewModel(LaunchProfile profile, bool developerMode, bool writeMode)
        {
            this.Profile = profile;
            this.DeveloperMode = developerMode;
            this.EditMode = writeMode;
            this.LoadProjects();
            this.LoadExectuableFiles();
            this.OnSelectedProjectChanged();
            this.IsModified = false;

            _trayContextMenuCommand = new SimpleCommand(this.ExecuteTrayContextMenuCommand);
        }

        [SuppressMessage("ReSharper", "InconsistentNaming")]
        private void LoadAvailableCultures()
        {

            var l10nConfigFilePath = Path.Combine(this.SelectedProject.Path, "Config/Localization/Game.ini");

            var definedCultures = new HashSet<string>();
            var nativeCulture = Arguments.Culture.DefaultParameter as string;

            if (File.Exists(l10nConfigFilePath))
            {
                //kludge culture detection
                var l10nConfig = File.ReadAllText(l10nConfigFilePath);
                var matches = Regex.Matches(l10nConfig, @"^\s*CulturesToGenerate=([\w\-]+)\s*$", RegexOptions.Multiline);
                foreach (var culture in matches.OfType<Match>().Select(m => m.Groups[1].Value))
                    definedCultures.Add(culture.ToLowerInvariant());

                var match = Regex.Match(l10nConfig, @"^\s*NativeCulture=([\w\-]+)\s*$", RegexOptions.Multiline);
                if (match.Success)
                {
                    nativeCulture = match.Groups[1].Value;
                    definedCultures.Add(nativeCulture);
                }
            }

            var allCultures = CultureInfo.GetCultures(CultureTypes.InstalledWin32Cultures);

            this.AvailableCultures =
                allCultures.Union(allCultures.Select(c => c.Parent).Where(p => p != null).Distinct())
                           .Where(c => !object.Equals(c, CultureInfo.InvariantCulture))
                           .Where(c => this.DeveloperMode || definedCultures.Count == 0 || definedCultures.Contains(c.Name.ToLowerInvariant()))
                           .Select(
                               c =>
                                   new CultureViewModel(c.Name, c.NativeName,
                                                   definedCultures.Contains(c.Name.ToLowerInvariant())))
                           .OrderByDescending(c => c.IsDefined).ThenBy(c => c.Name).ToArray();

            var cultureName = nativeCulture;
            string currentCultureName;
            if (this.Profile.GetArgumentParameter(Arguments.Culture, out currentCultureName))
                cultureName = currentCultureName;

            if (!string.IsNullOrEmpty(cultureName))
                this.Culture = this.AvailableCultures.FirstOrDefault(c => c.Name.Equals(cultureName, StringComparison.InvariantCultureIgnoreCase));

            if (this.Culture == null)
                this.Culture = this.AvailableCultures.FirstOrDefault();
        }

        private void ExecuteTrayContextMenuCommand(object obj)
        {
            this.Launch(Utilities.IsCtrlDown ? DebuggerInfo.Automatic : null);
        }

        private void LoadExectuableFiles()
        {
            this.ExecutableFiles = ExecutableFileInfo.ExecutableFiles;

            this.SelectedExecutableFile = this.ExecutableFiles.FirstOrDefault(e => e.ProjectRelativePath.Equals(this.Profile.ExecutableFile))
                                          ?? this.ExecutableFiles.FirstOrDefault();
        }

        private void RaiseProfilePropertyChanged(string propertyName)
        {
            this.RaisePropertyChanged(propertyName);
            this.RaisePropertyChanged(nameof(this.ConsoleCommandLine));

            if (this.DeveloperMode && this.EditMode)
                this.IsModified = true;
        }


        private void LoadProjects()
        {
            this.Projects = ProjectInfo.Projects;
            this.SelectedProject = this.Projects.FirstOrDefault(p => p.Name == this.Profile.ProjectName)
                                   ?? this.Projects.FirstOrDefault();
        }

        private void OnSelectedProjectChanged()
        {
            this.LoadMaps();
            this.LoadIniFileList();
            this.LoadAvailableCultures();
        }

        private void LoadIniFileList()
        {
            if (this.SelectedProject != null)
            {
                var folder = Path.Combine(this.SelectedProject.Path, "Config");
                this.IniFiles =
                    Directory.GetFiles(folder, "*.ini", SearchOption.AllDirectories)
                             .Select(f => f.Substring(folder.Length + 1))
                             .ToArray();
            }
            else
                this.IniFiles = new string[0];

            this.DefaultEditorFilename = this.GetIniFile(Arguments.OverrideDefaultEditor);
            this.DefaultGameFilename = this.GetIniFile(Arguments.OverrideDefaultGame);
            this.DefaultEngineFilename = this.GetIniFile(Arguments.OverrideDefaultEngine);
            this.DefaultInputFilename = this.GetIniFile(Arguments.OverrideDefaultInput);
        }

        private string GetIniFile(ArgumentInfo argument)
        {
            string filename;
            if (this.Profile.GetArgumentParameter(argument, out filename) && !string.IsNullOrEmpty(filename))
                return this.IniFiles.FirstOrDefault(f => f.Equals(filename, StringComparison.InvariantCultureIgnoreCase))
                       ?? this.IniFiles.FirstOrDefault();

            return this.IniFiles.FirstOrDefault();
        }

        private void LoadMaps()
        {
            var maps = new List<MapInfo>();
            if (this.SelectedProject != null)
            {
                var folder = Path.Combine(this.SelectedProject.Path, "Content");
                foreach (var path in Directory.GetFiles(folder, "*.umap", SearchOption.AllDirectories))
                {
                    if (path.StartsWith(Path.Combine(folder, "Maps")))
                        maps.Add(new MapInfo(path, Path.GetFileNameWithoutExtension(path)));
                    else
                    {
                        var mapName = "/Game/"
                                      + path.Substring(folder.Length, path.Length - folder.Length - ".umap".Length)
                                            .Replace('\\', '/');
                        maps.Add(new MapInfo(path, mapName));
                    }
                }
            }

            this.Maps = maps.OrderBy(m => m.Path).ToArray();

            MapInfo rootMap = null;
            foreach (var map in this.Maps)
            {
                if (rootMap == null)
                {
                    map.IsSublevel = false;
                    rootMap = map;
                }
                else
                {
                    if (map.Path.Substring(0, map.Path.Length - ".umap".Length)
                           .StartsWith(rootMap.Path.Substring(0, rootMap.Path.Length - ".umap".Length)))
                    {
                        map.IsSublevel = true;
                    }
                    else
                    {
                        map.IsSublevel = false;
                        rootMap = map;
                    }
                }
            }

            if (this.Profile.Map != null)
            {
                _specifyMapName = true;
                this.SelectedMap = this.Maps.FirstOrDefault(m => m.Name == this.Profile.Map)
                                   ?? this.Maps.FirstOrDefault();
            }
            else
            {
                _specifyMapName = false;
                this.SelectedMap = this.Maps.FirstOrDefault();
            }
        }


        private void UpdateProfileMap()
        {
            this.Profile.Map = this.SpecifyMapName ? this.SelectedMap?.Name : null;
        }


        private T GetParameterValue<T>(ArgumentInfo argument, ref T variable)
            where T : struct
        {
            T? value;
            if (this.Profile.GetArgumentParameter(argument, out value))
                variable = value ?? variable;
            return variable;
        }

        private T GetParameterEnumValue<T>(ArgumentInfo argument, ref T variable)
            where T : struct
        {
            T? value;
            if (this.Profile.GetArgumentEnumParameter(argument, out value))
                variable = value ?? variable;
            return variable;
        }

        private T GetParameterReference<T>(ArgumentInfo argument, ref T variable)
            where T : class
        {
            T value;
            if (this.Profile.GetArgumentParameter(argument, out value))
                variable = value ?? variable;
            return variable;
        }

        public void Launch(DebuggerInfo debugger)
        {
            var process = Process.Start(this.Profile.GetExecutableFile(), this.Profile.GetCommandLineArguments());
            if (debugger != null)
                debugger.AttachProcess(process);
            else
                App.ReportStatus("Profile launched successfully.");

        }

    }
}
