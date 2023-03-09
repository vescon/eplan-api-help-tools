using System.Linq;
using Eplan.EplApi.DataModel;
using Eplan.EplApi.DataModel.EObjects;
using Eplan.EplApi.HEServices;

namespace Vescon.Eplan.Utilities.Misc
{
    public static class EplanHelper
    {
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
    }
}
