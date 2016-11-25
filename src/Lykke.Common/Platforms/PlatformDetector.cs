using System.Collections.Generic;

namespace Common.Platforms

{

    public enum FormFactor
    {
        Unknown, Desktop, Mobile, Tablet
    }

    public enum PlarformType
    {
        Unknown, Windows,
        Android,
        Apple,
        Linux,
        Symbian,
        BlackBerry,


    }

    public enum ApplicationHost
    {
        Unknown, Native, Opera, Chrome, Ie, Firefox, Safari, UcBrowser, Android, WeChat
    }

    public enum ArchitectureType
    {
        Unknown, X86, X64
    }


    public class DeviceInfo
    {
        public FormFactor FormFactor { get; set; }
        public PlarformType PlarformType { get; set; }
        public ApplicationHost AppHost { get; set; }
        public string OsVersion { get; set; }
        public string AppVersion { get; set; }
        public ArchitectureType Architecture { get; set; }

    }
}
