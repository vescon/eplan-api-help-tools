using System;
using System.Collections.Specialized;
using System.IO;
using System.Linq;
using System.Threading;
using Eplan.EplApi.DataModel;
using Eplan.EplApi.HEServices;
using Vescon.Common.Extensions;

namespace Vescon.Eplan.Utilities
{
    class Program
    {
        private const int AttemptsCount = 20;
        private const int PauseTime = 5;
        private const int OtherInstanceExitCode = -1;
        
        private const string ElkFileExtension = ".elk";

        [STAThread]
        static int Main(string[] args)
        {
            for (var i = 0; i < AttemptsCount; i++)
            {
                var result = IgnoreSystemWide
                    .IfSingleInstance(typeof(Program).FullName, OtherInstanceExitCode, () => Start(args));
                
                if (result != OtherInstanceExitCode)
                    return result;
                
                Thread.Sleep(TimeSpan.FromSeconds(PauseTime));
            }

            return OtherInstanceExitCode;
        }

        private static int Start(string[] args)
        {
            try
            {
                using (new EplanAssemblyResolver())
                {
                    var options = Options.ParseArguments(args);
                    switch (options.Action)
                    {
                        case ProcessOptions.ActionCreateProject:
                            CreateProject(options.TemplatePath, options.OutputPath);
                            break;

                        case ProcessOptions.ActionUnpackProject:
                            UnpackProject(options.ArchiveFile, options.OutputPath, options.OutputName);
                            break;

                        case ProcessOptions.ActionSortLocations:
                            SortLocations(options.ProjectPath);
                            break;

                        case ProcessOptions.ActionSynchronizeArticles:
                            SynchronizeArticles(options.ProjectPath);
                            break;

                        case ProcessOptions.ActionGenerateReports:
                            GenerateReports(options.ProjectPath);
                            break;

                        case ProcessOptions.ActionPackProject:
                            PackProject(options.ProjectPath, options.OutputPath, options.OutputName);
                            break;

                        default:
                            throw new ArgumentException("Not supported action.");
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.ToDetailString());
                return 1;
            }

            return 0;
        }

        private static void PackProject(string projectPath, string outputPath, string outputName)
        {
            projectPath = Path.GetFullPath(projectPath);
            outputPath = Path.GetFullPath(outputPath);

            EplanApplicationStarter.Start();

            // export the project to a zw1 file
            new Backup().Project(
                projectPath,
                "Created with SO3.",
                outputPath,
                outputName,
                Backup.Type.MakeBackup,
                Backup.Medium.Disk,
                0,
                Backup.Amount.All,
                true, // clean-up
                true, // include 'DOC' folder
                true, // include 'Images' folder
                true); // include external documents
        }

        private static void GenerateReports(string projectPath)
        {
            projectPath = Path.GetFullPath(projectPath);

            EplanApplicationStarter.Start();

            using (new LockingStep())
            using (var project = new ProjectManager().OpenProject(projectPath, ProjectManager.OpenMode.Exclusive))
            {
                var reports = new Reports();
                reports.GenerateProject(project);

                project.Close();
            }
        }

        private static void SynchronizeArticles(string projectPath)
        {
            projectPath = Path.GetFullPath(projectPath);

            EplanApplicationStarter.Start();

            using (new LockingStep())
            using (var project = new ProjectManager().OpenProject(projectPath, ProjectManager.OpenMode.Exclusive))
            {
                var synchronize = new Synchronize();
                synchronize.PartsFromSystemToProject(project);

                project.Close();
            }
        }

        private static void SortLocations(string projectPath)
        {
            projectPath = Path.GetFullPath(projectPath);

            EplanApplicationStarter.Start();

            using (new LockingStep())
            using (var project = new ProjectManager().OpenProject(projectPath, ProjectManager.OpenMode.Exclusive))
            {
                var locations = project
                    .GetLocationObjects()
                    .GroupBy(x => x.HierarchyId);

                locations.ForEach(x =>
                {
                    var names = x.Select(y => y.Name).ToList();
                    names.Sort();

                    project.SetSortedLocations(x.Key, names.ToArray());
                });

                project.Close();
            }
        }

        private static void CreateProject(string templatePath, string outputPath)
        {
            templatePath = Path.GetFullPath(templatePath);
            outputPath = Path.GetFullPath(outputPath);

            EplanApplicationStarter.Start();

            using (new LockingStep())
            using (var project = new ProjectManager().CreateProject(outputPath, templatePath))
            {
                if (project != null)
                    project.Close();
            }
        }

        private static void UnpackProject(string packedProjectPath, string outputDirectory, string projectName)
        {
            packedProjectPath = Path.GetFullPath(packedProjectPath);
            outputDirectory = Path.GetFullPath(outputDirectory ?? Path.GetDirectoryName(packedProjectPath) ?? @".");
            projectName = projectName ?? Path.GetFileNameWithoutExtension(packedProjectPath);

            EplanApplicationStarter.Start();

            var restore = new Restore();
            restore.Project(
                new StringCollection { packedProjectPath },
                outputDirectory,
                projectName,
                false,
                true,
                1);

            using (new LockingStep())
            using (var project = new ProjectManager().OpenProject(Path.Combine(outputDirectory, projectName + ElkFileExtension)))
            {
                if (project != null)
                    project.Close();
            }
        }
    }
}
