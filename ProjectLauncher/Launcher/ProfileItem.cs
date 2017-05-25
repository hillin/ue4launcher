using System;

namespace UE4Launcher.Launcher
{
    [Serializable]
    internal class ProfileItem<T>
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
