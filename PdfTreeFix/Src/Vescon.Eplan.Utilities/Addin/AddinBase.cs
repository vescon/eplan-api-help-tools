using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Threading;
using Eplan.EplApi.ApplicationFramework;
using Eplan.EplApi.Base;
using Eplan.EplApi.DataModel;
using Eplan.EplApi.Gui;
using Vescon.Common.Extensions;
using Vescon.Eplan.Utilities.Action;
using EventHandler = Eplan.EplApi.ApplicationFramework.EventHandler;

namespace Vescon.Eplan.Utilities.Addin
{
    public abstract class AddinBase
    {
        private const string AddinVersionSettingBase = "USER.VesconAddinsVersions.";

        private string _addinVersionSettingPath;
        private string _eplanAddonInstallationPathSetting;
        private string _eplanAddonVersionSetting;

        private EventHandler _onAppStartEvent = new EventHandler();
        private EventHandler _onProjectOpenEvent = new EventHandler();

        private bool _repeatRegistration;

        protected AddinBase()
        {
            SetSettingsPaths();
            GetCurrentVersionAndInstallationPath();
        }

        protected string AddinPath { get; private set; }
        protected string AddinVersion { get; private set; }

        public bool OnExit()
        {
            return true;
        }

        public bool OnInit()
        {
            _onAppStartEvent.SetEvent("Eplan.EplApi.OnMainStart");
            _onAppStartEvent.EplanEvent += OnEplanStart;

            _onProjectOpenEvent.SetEvent("Eplan.EplApi.OnPostOpenProject");
            _onProjectOpenEvent.EplanEvent += OnProjectOpen;

            return true;
        }

        public bool OnInitGui()
        {
            GetCurrentVersionAndInstallationPath();

            SetCultureInfo();

            CreateMenu();

            CreateToolbar(ToToolBarButtons(GetActions(), GetRegistrationDir()));

            OnInitialized();
            return true;
        }

        public bool OnRegister(ref bool loadOnStart)
        {
            RegistrationInfo();
            
            SetAddinVersionInSettings();

            OnRegistered();

            return true;
        }

        public bool OnUnregister()
        {
#if VERSION_23
            _repeatRegistration = false;
#else
            _repeatRegistration = RegistrationNeeded();
#endif
            if (!_repeatRegistration)
            {
                ResetEventHandlers();
                
                var eplanSettings = new Settings();
                if (eplanSettings.ExistSetting(_addinVersionSettingPath))
                {
                    eplanSettings.DeleteSetting(_addinVersionSettingPath);
                }
            }
            
            OnUnRegistered();

            return true;
        }

        protected string GetRegistrationDir()
        {
            return Path.GetDirectoryName(AddinPath);
        }

        protected abstract string GetTitle();
        protected abstract IEnumerable<ActionBase> GetActions();

        protected abstract string GetAppModifier();

        protected virtual string GetMainMenuCaption()
        {
            return GetTitle();
        }

        protected virtual string GetToolBarName()
        {
            return GetTitle();
        }

        protected virtual void OnProjectOpen(Project project)
        {
        }

        protected virtual void OnEplanStart()
        {
        }

        protected virtual void OnInitialized()
        {
        }

        protected virtual void OnRegistered()
        {
        }

        protected virtual void OnUnRegistered()
        {
        }

        protected abstract IEnumerable<string> GetSupportedResourceLanguages();

        private void SetSettingsPaths()
        {
            var appModifier = GetAppModifier();

            _addinVersionSettingPath = AddinVersionSettingBase + appModifier;
            _eplanAddonInstallationPathSetting = $"STATION.AF.ApiModules.{appModifier}";
            _eplanAddonVersionSetting = $"STATION.SYSTEM.{appModifier}.Version";
        }

        private void OnProjectOpen(IEventParameter param)
        {
            var stringParam = new EventParameterString(param);

            var project = new ProjectManager { LockProjectByDefault = false }
                .OpenProjects
                .FirstOrDefault(x => x.ProjectLinkFilePath == stringParam.String);

            if (project == null)
                return;

            OnProjectOpen(project);
        }

        private void OnEplanStart(IEventParameter param)
        {
#if VERSION_23
            _repeatRegistration = RegistrationNeeded();
#endif
            if (_repeatRegistration)
            {
                ReRegister();
            }

            SetToolBarToolTips(ToToolBarButtons(GetActions(), GetRegistrationDir()));

            OnEplanStart();
        }

        private void GetCurrentVersionAndInstallationPath()
        {
            var eplanSettings = new Settings();
            AddinVersion = eplanSettings.GetStringSetting(_eplanAddonVersionSetting, 0);
            AddinPath = eplanSettings.GetStringSetting(_eplanAddonInstallationPathSetting, 0);
        }

        private bool RegistrationNeeded()
        {
            var eplanSettings = new Settings();
            
            if (!eplanSettings.ExistSetting(_addinVersionSettingPath))
                return true;

            return eplanSettings.GetStringSetting(_addinVersionSettingPath, 0) != AddinVersion;
        }

        private void SetAddinVersionInSettings()
        {
#pragma warning disable 618
            new Settings().AddStringSetting(
                _addinVersionSettingPath,
                new string[0],
                new string[0],
                string.Empty,
                new[] { AddinVersion },
                ISettings.CreationFlag.Overwrite);
/*
    The deprecated method must be used to keep compatibility with Eplan P8 2.3.
    If that requirement is not valid anymore, the following should be used:
 
            new Settings().AddStringSetting(
                _addinVersionSettingPath,
                new[] { _addinVersion },
                new string[0],
                ISettings.CreationFlag.Overwrite);
*/
#pragma warning restore 618
        }

        private void CreateMenu()
        {
            var mainMenuCaption = GetMainMenuCaption();
            if (mainMenuCaption.IsEmpty())
                return;

            var items = ToMenuItems(GetActions());
            if (items.IsEmpty())
                return;

            uint id = 0;
            var index = 1;
            var m = new Menu();

            foreach (var x in items)
            {
                if (index == 1)
                {
                    id = m.AddPopupMenuItem(
                        mainMenuCaption,
                        x.MenuCaption,
                        x.ActionName,
                        x.ActionDescription,
                        35032, // 'System messages' menu item id
                        index++,
                        true,
                        false);
                }
                else
                {
                    m.AddMenuItem(
                        x.MenuCaption,
                        x.ActionName,
                        x.ActionDescription,
                        id,
                        index++,
                        x.SeparatorBefore,
                        false);

                    if (x.SeparatorBefore)
                        ++index;
                }
            }
        }

        private void CreateToolbar(List<ToolBarButtonInfo> buttons)
        {
            if (buttons == null
                || !buttons.Any())
                return;

            var toolBarName = GetToolBarName();
            if (toolBarName == null)
                return;

            var t = new Toolbar();

            var countOfButtonsToRemove = 0;
            var dummy = true;
            if (t.ExistsToolbar(toolBarName, ref dummy))
                countOfButtonsToRemove = t.GetCountOfButtons(toolBarName);
            else
                t.CreateCustomToolbar(toolBarName, Toolbar.ToolBarDockPos.eToolbarFloat, 0, 0, false);

            var index = countOfButtonsToRemove;
            buttons.ForEach(x => t.AddButton(toolBarName, index++, x.ActionName, x.IconPath, string.Empty));

            while (countOfButtonsToRemove-- > 0)
                t.RemoveButton(toolBarName, 0);
        }

        private void SetToolBarToolTips(List<ToolBarButtonInfo> buttons)
        {
            if (buttons == null
                || !buttons.Any())
                return;

            var toolBarName = GetToolBarName();
            if (toolBarName == null)
                return;

            var t = new Toolbar();

            var dummy = true;
            if (t.ExistsToolbar(toolBarName, ref dummy))
            {
                var count = Math.Min(buttons.Count, t.GetCountOfButtons(toolBarName));
                for (var i = 0; i < count; ++i)
                    t.SetButtonToolTip(toolBarName, i, buttons[i].TooTip);
            }
        }

        private void ReRegister()
        {
            var cli = new CommandLineInterpreter();

            try
            {
                var installFile = new FileInfo(Path.Combine(GetRegistrationDir(), @"../cfg/install.xml")).FullName;

                cli.Execute("XSettingsUnregisterAction /installFile:\"" + installFile + "\"");
                
                cli.Execute("XSettingsRegisterAction /installFile:\"" + installFile + "\"" + " /reinstall:1");
            }
            catch (Exception ex)
            {
                new BaseException(
                    $"{GetTitle()} registration failed.",
                    MessageLevel.Error,
                    ex as BaseException)
                    .FixMessage();
            }
        }

        private void ResetEventHandlers()
        {
            _onAppStartEvent.Dispose();
            _onAppStartEvent = new EventHandler();

            _onProjectOpenEvent.Dispose();
            _onProjectOpenEvent = new EventHandler();
        }

        private void RegistrationInfo()
        {
            new BaseException(
                $"{GetTitle()} registered successfully.",
                MessageLevel.Message,
                new BaseException($"Version: {AddinVersion}.", MessageLevel.Message))
                .FixMessage();
        }

        private void SetCultureInfo()
        {
            var eplanLanguage = new Settings()
                .GetStringSetting("USER.SYSTEM.GUI.LANGUAGE", 0)
                .ToLowerInvariant();

            var isGerman = eplanLanguage.StartsWith("de");

            var info = new CultureInfo(isGerman ? "de-DE" : "en-US");
            Thread.CurrentThread.CurrentUICulture = info;
            Thread.CurrentThread.CurrentCulture = info;
        }

        private List<MenuItem> ToMenuItems(IEnumerable<ActionBase> actions)
        {
            return actions
                .Select(x => new MenuItem(
                    x.MenuCaption, 
                    x.Name, 
                    x.Description))
                .ToList();
        }

        private List<ToolBarButtonInfo> ToToolBarButtons(IEnumerable<ActionBase> actions, string addinDir)
        {
            return actions
                .Where(x => x.IconPath != null)
                .Select(x => new ToolBarButtonInfo(
                    x.Name, 
                    Path.Combine(addinDir, x.IconPath), 
                    x.Description))
                .ToList();
        }
    }
}
