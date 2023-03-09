using System.Collections.Generic;
using System.Linq;
using Eplan.EplApi.DataModel;
using Vescon.Eplan.Utilities;

namespace PdfTreeFix.Actions.Data
{
    public class PageInfo
    {
        public PageInfo(
            Page page, 
            ProjectStructureHelper structureHelper)
        {
            Page = page;

            NameParts = structureHelper.PagesNavigatorStructure.ToDictionary(
                x => x,
                x => page.Properties[(int)x].ToString());
        }

        public Page Page { get; }

        public Dictionary<StructurePropertyId, string> NameParts { get;  }
    }
}