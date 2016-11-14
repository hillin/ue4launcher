using System;
using System.Text;
using System.Xml.Serialization;

namespace UE4Launcher.Launcher
{
    [Serializable]
    public class Argument : NotificationObject
    {
        [XmlAttribute]
        public string ArgumentName { get; set; }

        private ArgumentInfo _cachedArgumentInfo;
        public ArgumentInfo ArgumentInfo => _cachedArgumentInfo ?? (_cachedArgumentInfo = Arguments.GetArgument(this.ArgumentName));

        private object _parameter;
        public object Parameter
        {
            get { return _parameter; }
            set
            {
                this.RaisePropertyChanged(nameof(this.Parameter));
                _parameter = value;
            }
        }

        public Argument(string argumentName, object parameter = null)
        {
            this.ArgumentName = argumentName;
            _parameter = parameter;
        }

        public Argument(ArgumentInfo info, object parameter = null)
        {
            this.ArgumentName = info.Name;
            _cachedArgumentInfo = info;
            _parameter = parameter;
        }

        public Argument()
        {
            // for serializer
        }


        public void ToCommandLine(StringBuilder builder, LaunchProfile launchProfile)
        {
            this.ArgumentInfo.ToCommandLine(this, builder, launchProfile);
        }

        public Argument Clone()
        {
            return (Argument)this.MemberwiseClone();
        }
    }
}
