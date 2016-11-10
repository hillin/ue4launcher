using System;

namespace ProjectLauncher.Launcher
{
    [Serializable]
    class ProfileItem<T>
    {
        public T DefaultValue { get; }

        public bool IsEnabled { get; set; }

        public T Value { get; set; }

        public ProfileItem(T defaultValue = default(T))
        {
            this.DefaultValue = defaultValue;
        }

    }
}
