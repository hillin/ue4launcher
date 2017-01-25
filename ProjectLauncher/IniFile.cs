using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Runtime.InteropServices;
using System.Text;

namespace UE4Launcher
{
    internal class IniFile
    {
        private readonly string _path;

        [DllImport("kernel32", CharSet = CharSet.Unicode)]
        private static extern long WritePrivateProfileString(string section, string key, string value, string filePath);

        [DllImport("kernel32", CharSet = CharSet.Unicode)]
        private static extern int GetPrivateProfileString(string section, string key, string Default, StringBuilder retVal, int size, string filePath);

        public IniFile(string path)
        {
            _path = new FileInfo(path).FullName;
        }

        public string Read(string key, string section)
        {
            var result = new StringBuilder(255);
            IniFile.GetPrivateProfileString(section, key, "", result, 255, _path);
            return result.ToString();
        }

        public void Write(string key, string value, string section)
        {
            IniFile.WritePrivateProfileString(section, key, value, _path);
        }

        public void DeleteKey(string key, string section)
        {
            this.Write(key, null, section);
        }

        public void DeleteSection(string section)
        {
            this.Write(null, null, section);
        }

        public bool KeyExists(string key, string section)
        {
            return this.Read(key, section).Length > 0;
        }
    }
}
