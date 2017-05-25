# ue4launcher
**ue4launcher** is a project launcher utility for Unreal Engine 4 projects. It serves to help launching editor, game or server instances for complex UE4 projects, with many useful command-line arguments exposed in a profile-enforced GUI. The launcher also features a process manager (but only for UE4 executables), which allows you to quickly attaching a debugger, or kill a process; and a file search engine which provides search results in project folder quickly.

![ue4launcher Screenshot](/images/screenshot/ue4launcher.png?raw=true)

# Features
- Launching UE4 projects in editor, game or server mode
- Many useful command-line arguments exposed, including logging, windowed/fullscreen options, VR options and ini overridding
- Per-project based profile, with both public and personal storage (i.e. you can share profiles with your team in public storage, and keep some local profiles in personal storage)
- Launch UE4 projects with debugger (Visual Studio) attached
- Ping display for client profiles
- Process manager for attaching debuggers or killing UE4 processes
- Fast file and folder search in project folder, search result can be bookmarked for quick access
- Tray icon with quick access to launch profiles, processes and bookmarked places
- Automatically start with Windows (optional)
- Both editor and cooked version of your UE4 project are supported
- User-friendly GUI driven by WPF + a pinch of MVVM

# Build and Run
You will need Visual Studio 2017 and .NET Framework 4.0 to build this project.

Once you have this project built, place *launcher.exe* at any place and depth under your UE4 project folder and run it with *-edit* args (read the section below to learn more).

## Third-Party Libraries
- **Extended WPF Toolkit** is employed in this project. It will be restored through nuget automatically.
- **Cortura.Fody** is used to pack all the assemblies together, to make this utility a single-file program.

# Developer Mode and Edit Mode
**ue4launcher** has 2 modes:
- **Standard Mode**: run *ProjectLauncher.exe* directly. In standard mode, all the features are available, except for you can't save your modifications to profiles. This could be the daily mode for project developers.
- **Edit Mode**: run *ProjectLauncher.exe* with *-edit* arg. Profiles are allowed to be modified in this mode.
