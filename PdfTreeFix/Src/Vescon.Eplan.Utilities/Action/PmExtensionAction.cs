using System.Collections.Generic;
using Eplan.EplApi.ApplicationFramework;
using Vescon.Common.Extensions;

namespace Vescon.Eplan.Utilities.Action
{
    class PmExtensionAction : ActionBase, IEplAction
    {
        private static readonly Dictionary<string, List<IPmExtensionTab>> RegisteredTabs = new Dictionary<string, List<IPmExtensionTab>>();
 
        public static void Register(IEnumerable<string> itemTypes, IPmExtensionTab tab)
        {
            itemTypes.ForEach(x =>
            {
                if (!RegisteredTabs.ContainsKey(x))
                    RegisteredTabs[x] = new List<IPmExtensionTab>();

                RegisteredTabs[x].RemoveAll(y => y.GetType() == tab.GetType());

                RegisteredTabs[x].Add(tab);
            });
        }

        public override bool Execute(ActionCallingContext ctx)
        {
/*
#if DEBUG
            System.Diagnostics.Debug.WriteLine("--------------------------");
            foreach (var param in ctx.GetParameters())
            {
                var val = string.Empty;
                ctx.GetParameter(param, ref val);
                System.Diagnostics.Debug.WriteLine("{0} = {1}", param, val);
            }
#endif
*/
            return Execute(() =>
            {
                var itemType = ctx.GetItemType();
                var key = ctx.GetKey();
                var partNumber = ctx.GetPartNumber();
                var partVariant = ctx.GetPartVariant();

                switch (ctx.GetAction())
                {
                    case PmExtensionHelper.SelectItem:
                        if (RegisteredTabs.ContainsKey(itemType))
                            RegisteredTabs[itemType].ForEach(
                                x => x.ItemSelected(itemType, key, partNumber, partVariant));
                        break;

                    case PmExtensionHelper.SaveItem:
                        if (RegisteredTabs.ContainsKey(itemType))
                            RegisteredTabs[itemType].ForEach(
                                x => x.ItemSaved(itemType, key, partNumber, partVariant));
                        break;
                }

                return true;
            });
        }
    }
}
