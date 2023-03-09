using Eplan.EplApi.Base;

namespace PlaceholderConfigLoader.EplAddin
{
    static class PmExtensionHelper
    {
        public const string OpenDatabase = "OpenDatabase";
        public const string CloseDatabase = "CloseDatabase";
        public const string GetRootLevel = "GetRootLevel";
        public const string GetNextLevel = "GetNextLevel";
        public const string PreShowTab = "PreShowTab";
        public const string SelectItem = "SelectItem";
        public const string SaveItem = "SaveItem";
        public const string DeleteItem = "DeleteItem";
        public const string NewItem = "NewItem";
        public const string ExportCustomItem = "ExportCustomItem";
        public const string ImportCustomItem = "ImportCustomItem";
        public const string ExportEplanItem = "ExportEplanItem";
        public const string ImportEplanItem = "ImportEplanItem";
        public const string CopyItem = "CopyItem";
        public const string AddPartToProject = "AddPartToProject";
        public const string AddPartToDatabase = "AddPartToDatabase";

        public static string GetKey(this IContext ctx)
        {
            return Get(ctx, "key");
        }

        public static string GetItemType(this IContext ctx)
        {
            return Get(ctx, "itemtype");
        }

        public static string GetPartNumber(this IContext ctx)
        {
            return Get(ctx, "partnr");
        }

        public static string GetPartVariant(this IContext ctx)
        {
            return Get(ctx, "variant");
        }

        public static string GetKey(this IContext ctx, int index)
        {
            return Get(ctx, string.Format("key.{0}", index));
        }

        public static string GetType(this IContext ctx, int index)
        {
            return Get(ctx, string.Format("itemtype.{0}", index));
        }

        public static string GetPartNumber(this IContext ctx, int index)
        {
            return Get(ctx, string.Format("partnr.{0}", index));
        }

        public static string GetPartVariant(this IContext ctx, int index)
        {
            return Get(ctx, string.Format("variant.{0}", index));
        }

        public static string GetAction(this IContext ctx)
        {
            return Get(ctx, "action");
        }

        public static string GetItemCount(this IContext ctx)
        {
            return Get(ctx, "itemcount");
        }

        public static string GetDatabase(this IContext ctx)
        {
            return Get(ctx, "database");
        }

        public static string GetSourceKey(this IContext ctx)
        {
            return Get(ctx, "sourcekey");
        }

        public static string GetReadOnly(this IContext ctx)
        {
            return Get(ctx, "readonly");
        }

        public static string GetValue(this IContext ctx)
        {
            return Get(ctx, "value");
        }

        private static string Get(IContext ctx, string param)
        {
            var s = string.Empty;
            ctx.GetParameter(param, ref s);
            return s;
        }
    }
}
