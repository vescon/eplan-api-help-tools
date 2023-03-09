using System.Linq;
using Eplan.EplApi.DataModel;
using Eplan.EplApi.DataModel.EObjects;
using Eplan.EplApi.DataModel.Graphics;
using Eplan.EplApi.HEServices;
using PlaceholderConfigLoader.Extensions;

namespace PlaceholderConfigLoader
{
    internal static class EplanHelper
    {
        public static string GetCurrentProjectName(string def = null)
        {
            var prj = GetCurrentProject();
            return prj == null 
                ? def ?? string.Empty
                : prj.Properties.PROJ_NAME.ToStringSafe();
        }

        public static Project GetCurrentProject()
        {
            return new SelectionSet { LockProjectByDefault = false, LockSelectionByDefault = false }
                .GetCurrentProject(false);
        }

        public static Function GetSelectedMainFunction()
        {
            var selectedFunctions = new SelectionSet { LockProjectByDefault = false, LockSelectionByDefault = false }
                .Selection
                .OfType<Function>();

            return selectedFunctions.SingleOrDefault(x => x.IsMainFunction || (x is Terminal && ((Terminal)x).IsMainTerminal));
        }

        public static PlaceHolder GetSelectedPlaceholder()
        {
            var selectedPlaceholders = new SelectionSet { LockProjectByDefault = false, LockSelectionByDefault = false }
                .Selection
                .OfType<PlaceHolder>()
                .ToList();

            return selectedPlaceholders.Count == 1 
                ? selectedPlaceholders[0] 
                : null;
        }
    }
}
