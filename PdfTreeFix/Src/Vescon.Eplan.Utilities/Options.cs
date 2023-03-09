using CommandLine;
using CommandLine.Text;

namespace Vescon.Eplan.Utilities
{
    internal class Options : CommandLineOptionsBase
    {
        [Option(ProcessOptions.ParamAction, ProcessOptions.ParamActionLong, HelpText = "Action name")]
        public string Action { get; set; }

        [Option(ProcessOptions.ParamProjectPath, ProcessOptions.ParamProjectPathLong, HelpText = "Project path")]
        public string ProjectPath { get; set; }

        [Option(ProcessOptions.ParamTemplatePath, ProcessOptions.ParamTemplatePathLong, HelpText = "Template path")]
        public string TemplatePath { get; set; }

        [Option(ProcessOptions.ParamOutputPath, ProcessOptions.ParamOutputPathLong, HelpText = "Output path")]
        public string OutputPath { get; set; }

        [Option(ProcessOptions.ParamArchiveFile, ProcessOptions.ParamArchiveFileLong, HelpText = "Packed project file")]
        public string ArchiveFile { get; set; }

        [Option(ProcessOptions.ParamOutputName, ProcessOptions.ParamOutputNameLong, HelpText = "Output name")]
        public string OutputName { get; set; }

        public static Options ParseArguments(string[] args)
        {
            var options = new Options();
            
            if (!CommandLineParser.Default.ParseArguments(args, options))
                return null;

            options.TemplatePath = Trim(options.TemplatePath);
            options.OutputPath = Trim(options.OutputPath);
            options.OutputName = Trim(options.OutputName);

            return options;
        }

        [HelpOption]
        public string GetUsage()
        {
            return HelpText.AutoBuild(this, current => HelpText.DefaultParsingErrorsHandler(this, current));
        }

        private static string Trim(string text)
        {
            return !string.IsNullOrWhiteSpace(text) 
                ? text.Trim() 
                : null;
        }
    }
}
