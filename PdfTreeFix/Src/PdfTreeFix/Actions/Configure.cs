using Eplan.EplApi.ApplicationFramework;
using Eplan.EplApi.DataModel;
using Vescon.Eplan.Utilities;
using Vescon.Eplan.Utilities.Action;
using Vescon.Eplan.Utilities.Misc;

namespace PdfTreeFix.Actions
{
    class Configure : ActionBase, IEplAction
    {
        private const string WindowSettingId = "{084870F5-8F14-44FC-89D7-A736AD63921A}";

        public override string Description => "Shows configuration dialog.";
        public override string MenuCaption => "Show configuration dialog.";
        
        public override string IconPath => @"toolbar\Icon3.jpg";

        public override bool Execute(ActionCallingContext ctx)
        {
            return Execute(() =>
            {
                Project project;
                if (!GetCurrentProject(out project))
                    return true;

                var configSettings = Addin.ReadConfigSettings(project, null);

                var windowSettings = ReadWindowSettings(project);

                var projectStructureHelper = new ProjectStructureHelper(project);

                var dlg = new ConfigureView(
                    projectStructureHelper.PagesNavigatorStructure,
                    configSettings,
                    windowSettings,
                    projectStructureHelper);

                if (dlg.ShowDialog() == true)
                {
                    Addin.StoreConfigSettings(project, dlg.GetConfigSettings());
                }

                StoreWindowSettings(project, dlg.GetWindowSettings());

                return true;
            });
        }

        protected override string Title => Addin.Title;

        private void StoreWindowSettings(Project project, ConfigureView.WindowSettings settings)
        {
            var settingsJson = settings.ToJson();

            new ProjectSettings(project)
                .Add(WindowSettingId, settingsJson);
        }

        private ConfigureView.WindowSettings ReadWindowSettings(Project project)
        {
            var projectSettings = new ProjectSettings(project);
            if (projectSettings.Exists(WindowSettingId))
                try
                {
                    var windowSettings = projectSettings.GetString(WindowSettingId)
                        .FromJson<ConfigureView.WindowSettings>();

                    if (windowSettings != null)
                        return windowSettings;
                }
                // ReSharper disable once EmptyGeneralCatchClause
                catch {}

            return new ConfigureView.WindowSettings();
        }
    }
}