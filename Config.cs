using System;
using System.IO;
using System.Runtime.InteropServices;
using System.Text;

namespace HG_prog
{
    public static class Config
    {
        private static string _path;

        [DllImport("kernel32.dll", CharSet = CharSet.Unicode)]
        private static extern int GetPrivateProfileString(
            string section, string key, string def, StringBuilder retVal, int size, string filePath);

        [DllImport("kernel32.dll", CharSet = CharSet.Unicode)]
        private static extern long WritePrivateProfileString(
            string section, string key, string val, string filePath);

        public static string FilePath
        {
            get
            {
                if (_path == null)
                {
                    // conf.ini рядом с .exe
                    _path = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "conf.ini");
                }
                return _path;
            }
        }

        public static string ComPort
        {
            get
            {
                var sb = new StringBuilder(32);
                int len = GetPrivateProfileString("main", "ComPort", "COM5", sb, sb.Capacity, FilePath);
                string port = sb.ToString().Trim();
                return string.IsNullOrEmpty(port) ? "COM5" : port;
            }
            set => WritePrivateProfileString("main", "ComPort", value ?? "", FilePath);
        }

        public static void EnsureExists()
        {
            if (!File.Exists(FilePath))
            {
                ComPort = "COM5"; // создаст файл и запишет [main] ComPort=COM5
            }
        }
    }
}