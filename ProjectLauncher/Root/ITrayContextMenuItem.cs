using System.Windows.Input;
using System.Windows.Media;

namespace UE4Launcher.Root
{
    interface ITrayContextMenuItem
    {
        string Name { get; }
        string Description { get; }
        ImageSource Icon { get; }
        bool IsEnabled { get; }
        ICommand Command { get; }
    }
}
