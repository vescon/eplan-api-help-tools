using System.Collections.Generic;
using System.Linq;
using Eplan.EplApi.ApplicationFramework;
using PdfTreeFix.Actions.Data;
using Vescon.Common.Extensions;
using Vescon.Eplan.Utilities;

namespace PdfTreeFix.Actions
{
    class UpdateDescriptions : BaseUpdateDescriptions, IEplAction
    {
        public override string Description => "Modifies nodes' descriptions in the selected project.";
        public override string MenuCaption => "Update nodes' descriptions.";

        public override string IconPath => @"toolbar\Icon1.jpg";

        protected override string GetMenuCaption()
        {
            return MenuCaption;
        }

        protected override string GetInfo(IEnumerable<PageInfo> pages, StructurePropertyId propertyId, int limit)
        {
            var prefix = StructureHelper.GetStructurePropertyPrefix(propertyId);

            var values = pages
                .Select(x => x.NameParts[propertyId].ToString())
                .Distinct()
                .Where(x => x.IsNotEmpty())
                .OrderBy(x => x)
                .Select(x => prefix + x)
                .ToList();

            if (values.IsEmpty())
                return string.Empty;

            if (values.Count == 1)
                return values[0];

            return limit == 0 || limit >= values.Count
                ? values.Concatenate("/")
                : values
                    .Take(limit)
                    .Concat($"...({values.Count})".AsEnumerable())
                    .Concatenate("/");
        }

        protected override string GetInfoCompact(IEnumerable<PageInfo> pages, StructurePropertyId propertyId, int limit)
        {
            var values = pages
                .Select(x => x.NameParts[propertyId].ToString())
                .Distinct()
                .Where(x => x.IsNotEmpty())
                .OrderBy(x => x)
                .ToList();

            if (values.IsEmpty())
                return string.Empty;

            var prefix = StructureHelper.GetStructurePropertyPrefix(propertyId);

            if (values.Count == 1)
                return prefix + values[0];

            var valuesConcatenated = limit == 0 || limit >= values.Count
                ? values.Concatenate(", ")
                : values
                    .Take(limit)
                    .Concat($"...({values.Count})".AsEnumerable())
                    .Concatenate("/");

            return $"{prefix}({valuesConcatenated})";
        }
    }
}