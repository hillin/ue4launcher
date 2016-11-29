using System;
using System.Xml.Serialization;

namespace UE4Launcher.Places
{
    [Serializable]
    public class Location
    {
        [XmlAttribute]
        public string DisplayName { get; set; }
        [XmlAttribute]
        public string RelativePath { get; set; }
    }
}
