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
using PlaceholderConfigLoader.Extensions;
using EventHandler = Eplan.EplApi.ApplicationFramework.EventHandler;

namespace PlaceholderConfigLoader.EplAddin
{
    public abstract class AddinBase<TAddin>
    {
        private const int SystemMessagesMenuItemId = 35032;
        private readonly string _addinName;
        private readonly string _addinPath;
        private readonly string _addinVersion;
        private readonly string _addinVersionSettingPath;

        private EventHandler _onAppStartEvent = new EventHandler();
        private EventHandler _onProjectOpenEvent = new EventHandler();

        protected AddinBase()
        {
            _addinPath = GetType().Assembly.Location;
            AddinDir = Path.GetDirectoryName(_addinPath);
            _addinName = Path.GetFileNameWithoutExtension(_addinPath);
            _addinVersion = GetType().Assembly.GetName().Version.ToString();
            _addinVersionSettingPath = "USER.VesconAddinsVersions." + _addinName.Replace('.', '_');
        }

        public static string Name => typeof(TAddin).FullName.Replace('.', '_');

        public bool OnEplanStartEventOccurred { get; private set; }

        protected string AddinDir { get; private set; }

        protected bool IsSingleAction { get; set; }

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
            SetCultureInfo();
            CreateMenu();
            
            OnInitialized();
            return true;
        }

        public bool OnRegister(ref bool loadOnStart)
        {
            SetCultureInfo();
            CreateToolbar(GetToolBarButtons());
            
            RegistrationInfo();
            
            SetAddinVersionInSettings();

            OnRegistered();
            return true;
        }

        public bool OnUnregister()
        {
            var dummy = false;
            var t = new Toolbar();
            var toolBarName = GetToolBarName();
            if (toolBarName != null
                && t.ExistsToolbar(toolBarName, ref dummy))
                t.RemoveCustomToolbar(toolBarName);

            var eplanSettings = new Settings();
            if (eplanSettings.ExistSetting(_addinVersionSettingPath))
                eplanSettings.DeleteSetting(_addinVersionSettingPath);

            ResetEventHandlers();

            OnUnRegistered();

            return true;
        }

        protected virtual List<ToolBarButtonInfo> GetToolBarButtons()
        {
            return null;
        }

        protected virtual MenuItem GetMenuItem()
        {
            throw new NotImplementedException();
        }

        protected virtual List<MenuItem> GetMenuItems(out string mainMenuCaption)
        {
            throw new NotImplementedException();
        }

        protected virtual string GetToolBarName()
        {
            return null;
        }

        protected abstract string GetTitle();

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

        private void OnProjectOpen(IEventParameter param)
        {
            var stringParam = new EventParameterString(param);

            var project = new ProjectManager { LockProjectByDefault = false }
                .OpenProjects
                .FirstOrDefault(x => x.ProjectLinkFilePath == stringParam.String);

            if (project != null)
            {
                OnProjectOpen(project);
            }
        }

        private void OnEplanStart(IEventParameter param)
        {
            if (RegistrationNeeded())
                ReRegister();

            SetToolBarToolTips(GetToolBarButtons());

            OnEplanStart();

            OnEplanStartEventOccurred = true;
        }

        private bool RegistrationNeeded()
        {
            var eplanSettings = new Settings();
            if (eplanSettings.ExistSetting(_addinVersionSettingPath)
                && eplanSettings.GetStringSetting(_addinVersionSettingPath, 0) == _addinVersion)
                return false;
            
            return true;
        }

        private void SetAddinVersionInSettings()
        {
#pragma warning disable 618
            new Settings().AddStringSetting(
                _addinVersionSettingPath,
                new string[0],
                new string[0],
                string.Empty,
                new[] { _addinVersion },
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
            if (IsSingleAction)
            {
                var menuItem = GetMenuItem();

                var m = new Menu();
                m.AddMenuItem(
                    menuItem.MenuCaption,
                    menuItem.ActionName,
                    menuItem.ActionDescription,
                    SystemMessagesMenuItemId,
                    1,
                    true,
                    false);

                return;
            }

            string mainMenuCaption;
            var items = GetMenuItems(out mainMenuCaption);
            if (items != null
                && mainMenuCaption.IsNotEmpty()
                && items.Any())
            {
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
                            SystemMessagesMenuItemId,
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
                cli.Execute("EplApiModuleAction /unregisterInternal:\"" + _addinName + "\""); // 'Internal' doesn't call OnUnregister method
                ResetEventHandlers();
            }
            catch (Exception ex)
            {
                new BaseException(
                    $"{GetTitle()} un-registration failed.",
                    MessageLevel.Error,
                    ex as BaseException)
                    .FixMessage();
            }

            try
            {
                cli.Execute("EplApiModuleAction /register:\"" + _addinPath);
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
                new BaseException($"Version: {_addinVersion}.", MessageLevel.Message))
                .FixMessage();
        }

        private void SetCultureInfo()
        {
            var eplanLanguage = new Settings()
                .GetStringSetting("USER.SYSTEM.GUI.LANGUAGE", 0)
                .ToLowerInvariant();

            var isGerman = eplanLanguage
                .StartsWith("de");

            var info = new CultureInfo(isGerman ? "de-DE" : "en-US");
            Thread.CurrentThread.CurrentUICulture = info;
            Thread.CurrentThread.CurrentCulture = info;
        }

        protected struct ToolBarButtonInfo
        {
            public ToolBarButtonInfo(string actionPath, string iconPath, string toolTip)
                : this()
            {
                ActionName = actionPath;
                IconPath = iconPath;
                TooTip = toolTip;
            }

            public string ActionName { get; }
            public string IconPath { get; }
            public string TooTip { get; }
        }

        protected struct MenuItem
        {
            public MenuItem(string menuCaption, string actionName, string actionDescription)
                : this()
            {
                MenuCaption = menuCaption;
                ActionName = actionName;
                ActionDescription = actionDescription;
            }

            public string MenuCaption { get; }
            public string ActionName { get; }
            public string ActionDescription { get; }
            public bool SeparatorBefore { get; set; }
        }
    }
}
