using System;
using System.Collections.Generic;
using System.Linq;
using Eplan.EplApi.ApplicationFramework;
using Eplan.EplApi.DataModel;

using PdfTreeFix.Actions;
using Vescon.Common.Extensions;
using Vescon.Eplan.Utilities;
using Vescon.Eplan.Utilities.Action;
using Vescon.Eplan.Utilities.Addin;
using Vescon.Eplan.Utilities.Misc;

namespace PdfTreeFix
{
    // ReSharper disable once ClassNeverInstantiated.Global
    public class Addin : AddinBase, IEplAddIn
    {
        private const string MainCaption = "Vescon Tools 1.10";

        public static string Title => MainCaption + " - PdfTreeFix";

        private const string ConfigSettingId = "{29B4A979-18AA-4109-A35E-A2E458A88AB1}";

        protected override string GetTitle()
        {
            return Title;
        }

        protected override IEnumerable<ActionBase> GetActions()
        {
            return new ActionBase[]
            {
                new Configure(), 
                new MoveDescriptions(), 
                new UpdateDescriptions(),
                new RestoreDescriptions()
            };
        }

        protected override string GetAppModifier()
        {
            return "PdfTreeFix";
        }

        protected override IEnumerable<string> GetSupportedResourceLanguages()
        {
            return new[] { "en" };
        }

        protected override void OnInitialized()
        {
        }

        public static string GetEnabledNodesWarningMessage(
            IEnumerable<StructurePropertySetting> configSettings, 
            ProjectStructureHelper structureHelper)
        {
            var msg = Environment.NewLine
                + structureHelper.PagesNavigatorStructure
                    .Where(x => configSettings.SingleOrDefault(y => y.PropertyId == x && y.Enabled) != null)
                    .Select(x => $"\t- {structureHelper.GetStructurePropertyName(x)}")
                    .Concatenate(Environment.NewLine);

            msg += Environment.NewLine
                   + Environment.NewLine
                   + "WARNING: Existing node's descriptions will be overwritten."+ Environment.NewLine
                   + "Do you want to continue ?";

            return msg;
        }

        public static string GetExistingNodesMessage(
            ProjectStructureHelper structureHelper,
            bool withWarning)
        {
            var msg = Environment.NewLine
                + structureHelper.PagesNavigatorStructure
                    .Select(x => $"\t- {structureHelper.GetStructurePropertyName(x)}")
                    .Concatenate(Environment.NewLine);

            if (withWarning)
            {
                msg += Environment.NewLine
                       + Environment.NewLine
                       + "WARNING: Existing node's descriptions will be overwritten."+ Environment.NewLine
                       + "Do you want to continue ?";
            }
            else
            {
                msg += Environment.NewLine
                       + Environment.NewLine
                       + "Do you want to continue ?";
            }

            return msg;
        }

        public static ConfigurationSettings ReadConfigSettings(Project project, Action<string> errorAction)
        {
            var value = new ConfigurationSettings();
            
            var projectSettings = new ProjectSettings(project);
            if (projectSettings.Exists(ConfigSettingId))
                try
                {
                    value = projectSettings.GetString(ConfigSettingId)
                        .FromJson<ConfigurationSettings>();
                }
                // ReSharper disable once EmptyGeneralCatchClause
                catch {}

            if (errorAction == null)
                return value;

            if (value.NodeSettings.IsEmpty() 
                || value.NodeSettings.All(x => !x.Enabled))
            {
                errorAction("Structure properties are not configured.");
                return null;
            }

            return value;
        }

        public static void StoreConfigSettings(Project project, ConfigurationSettings settings)
        {
            var projectSettings = new ProjectSettings(project);
            projectSettings.Add(ConfigSettingId, settings.ToJson());
        }
    }
}