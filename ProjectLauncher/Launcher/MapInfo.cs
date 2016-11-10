namespace ProjectLauncher.Launcher
{
    class MapInfo
    {
        public string Path { get; }
        public string Name { get; }

        public MapInfo(string path, string name)
        {
            this.Path = path;
            this.Name = name;
        }
    }
}
