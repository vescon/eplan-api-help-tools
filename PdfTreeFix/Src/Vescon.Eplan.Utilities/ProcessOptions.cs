using System;
using System.Configuration;
using System.IO;

namespace Vescon.Eplan.Utilities
{
    public static class ProcessOptions
    {
        public const string ActionCreateProject = "CreateProject";
        public const string ActionUnpackProject = "UnpackProject";
        public const string ActionSortLocations = "SortLocations";
        public const string ActionSynchronizeArticles = "SynchronizeArticles";
        public const string ActionGenerateReports = "GenerateReports";
        public const string ActionPackProject = "PackProject";
        
        public const string ParamProjectPath = "p";
        public const string ParamProjectPathLong = "project";
        public const string ParamTemplatePath = "t";
        public const string ParamTemplatePathLong = "template";
        public const string ParamOutputPath = "o";
        public const string ParamOutputPathLong = "out";
        public const string ParamAction = "a";
        public const string ParamActionLong = "action";
        public const string ParamArchiveFile = "z";
        public const string ParamArchiveFileLong = "zw1";
        public const string ParamOutputName = "n";
        public const string ParamOutputNameLong = "name";

        public const string JobGuid = "g";
        public const string JobGuidLong = "jobGuid";
        public const string DataFileGuid = "md";

        // for internal use
        public const string StartDebugger = "dbg"; // for internal use
        public const string PropertyDescriptionsLanguage = "pdLang"; // for internal use
        public const string AllPropertyDescriptions = "pdAll"; // for internal use
        public const string ChildProcessIndex = "cpI"; // for internal use
        public const string ImportPages = "pi"; // for internal use
        public const string ProjectGuid = "piP"; // for internal use
        
        public static string Assembly => new Uri(typeof(ProcessOptions).Assembly.EscapedCodeBase).LocalPath;

        public static string GetExePathFromAppConfig(string key, string eplan32Bit)
        {
            var path = ConfigurationManager.AppSettings[key];

            path = path.Replace(".exe", eplan32Bit == "1" ? ".x86.exe" : ".x64.exe");

            var absolutePath = GetAbsoluteFileName(GetApplicationDirectory(), path);

            if (!File.Exists(path))
                throw new FileNotFoundException($"File does not exist: '{path}'. Check Jobs Runner's app.config file.", path);

            return absolutePath;
        }

        private static string GetAbsoluteFileName(string absoluteBaseDirectory, string filePath)
        {
            return
                Path.IsPathRooted(filePath)
                    ? filePath
                    : Path.GetFullPath(Path.Combine(absoluteBaseDirectory, filePath));
        }

        private static string GetApplicationDirectory()
        {
            return Path.GetDirectoryName(System.Reflection.Assembly.GetEntryAssembly().Location);
        }
    }
}
