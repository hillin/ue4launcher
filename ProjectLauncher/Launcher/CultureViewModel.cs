using System.Windows;

namespace UE4Launcher.Launcher
{
	internal class CultureViewModel
    {
        public string Name { get; }
        public string DisplayName { get; }
        public bool IsDefined { get; }
        public FontWeight FontWeight => this.IsDefined ? FontWeights.Bold : FontWeights.Normal;

        public CultureViewModel(string name, string displayName, bool isDefined)
        {
            this.Name = name;
            this.DisplayName = displayName;
            this.IsDefined = isDefined;
        }
    }
}
