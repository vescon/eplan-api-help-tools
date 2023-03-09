using Eplan.EplApi.ApplicationFramework;
using Eplan.EplApi.Gui;
using Vescon.EplAddin.Qdt.Actions;
using Temporary = Vescon.EplAddin.Qdt.Actions.Temporary;

namespace Vescon.EplAddin.Qdt
{
    public class Addin : IEplAddIn
    {
        internal const string AddinName = "Quick'n'Dirty Tools";
        internal static string Title = AddinName;

        public static string Name => typeof(Addin).FullName?.Replace('.', '_');

        public bool OnExit()
        {
            return true;
        }

        public bool OnInit()
        {
            return true;
        }

        public bool OnInitGui()
        {
            var version = GetType().Assembly.GetName().Version;

            Title = AddinName + $", v{version.Major}.{version.Minor}";

            var items = new[]
            {
                new Item(new ListCurrentPropArrangements()),
                new Item(new ListCurrentPropArrangementsWithExcel()),
                
                new Item(new ListPlacementsOnSpecificLayersWholeProject(), true),
                
                new Item(new CorrectCustomPropArrangementsOnlyCustom(), true),
                new Item(new CorrectCustomPropArrangementsWithStandard()),

                new Item(new ListAllPropArrangementsWithExcel(), true),
                new Item(new RemoveAllCustomPropArrangements()),

                new Item(new SetProperty10069(), true),
                new Item(new SetProperty11076()),

                new Item(new RemoveLabellingSettingsSchemas(), true),
                
                new Item(new GenerateMacros(), true),

                new Item(new RemoveUnusedPlaceholdersVariables(), true),
                
                new Item(new PdfExportFromMultipleProject(), true),
/*
                new Item(new Temporary.ShowProjectStructureLocations(), true),
                new Item(new Temporary.ShowPlacementsLocations(), true),
                new Item(new Temporary.ShowProperty10019()),
                new Item(new Temporary.SetProperty11023()),
*/
            };

            var mainMenuCaption = $"QDT v{version.Major}.{version.Minor}";

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
                        int.MaxValue,
                        x.SeparatorBefore,
                        false);

                    if (x.SeparatorBefore)
                        ++index;
                }
            }

            return true;
        }

        public bool OnRegister(ref bool loadOnStart)
        {
            return true;
        }

        public bool OnUnregister()
        {
            return true;
        }

        public class Item
        {
            public Item(
                IEplAction action, 
                bool separatorBefore = false)
            {
                if (action is ActionBase a)
                {
                    MenuCaption = a.MenuCaption;
                    ActionName = a.Name;
                    ActionDescription = a.Description;
                }
                
                SeparatorBefore = separatorBefore;
            }

            public string MenuCaption { get; }
            public string ActionName { get; }
            public string ActionDescription { get; }
            
            public bool SeparatorBefore { get; }
        }
    }
}
