using System.Windows;

namespace UE4Launcher.Launcher
{
    class MapInfo
    {
        public string Path { get; }
        public string Name { get; }
        public bool IsSublevel { get; set; }

        public FontWeight FontWeight => this.IsSublevel ? FontWeights.Normal : FontWeights.Bold;
        public Thickness Margin => this.IsSublevel ? new Thickness(24, 0, 0, 0) : new Thickness();

        public MapInfo(string path, string name)
        {
            this.Path = path;
            this.Name = name;
        }
    }
}
