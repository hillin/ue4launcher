# ue4launcher
**ue4launcher** is a project launcher utility for Unreal Engine 4 projects. It serves to help launching editor, game or server instances for complex UE4 projects, with many useful command-line arguments exposed in a profile-enforced GUI. The launcher also features a process manager (but only for UE4 executables), which allows you to quickly attaching a debugger, or kill a process.

# Features
- Launching UE4 projects in editor, game or server mode
- Many useful command-line arguments exposed, including logging, windowed/fullscreen options, VR options and ini overridding
- Per-project based profile, with both public and personal storage (i.e. you can share profiles with your team in public storage, and keep some local profiles in personal storage)
- Launch UE4 projects with debugger (Visual Studio) attached
- Ping display for client profiles
- Process manager for attaching debuggers or killing UE4 processes
- Both editor and cooked version of your UE4 project are supported
- User-friendly GUI driven by WPF + a bit of MVVM

# Build and Run
You will need Visual Studio 2015 and .NET Framework 4.6.1 to build this project.
Once you have this project built, run *ProjectLauncher.exe* with *-dev -edit* args (read the section below to learn more).

## Third-Party Libraries
Extended WPF Toolkit is employed in this project. It will be restored through nuget automatically.

# Developer Mode and Edit Mode
**ue4launcher** has 3 modes:
- **Public Mode**: ue4launcher is used as a public launcher, with most of its features hidden. This could be used as a user launcher of your cooked project. To run ue4launcher in public mode, simply run *ProjectLauncher.exe* without any arguments.
- **Developer Mode**: run *ProjectLauncher.exe* with *-dev* arg. In developer mode, all the features are available, except for you can't save your modifications to profiles. This could be the daily mode for project developers.
- **Developer Edit Mode**: Profiles are allowed to be modified in this mode.
