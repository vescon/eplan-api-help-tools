using System.Collections.Generic;
using System.Linq;
using Eplan.EplApi.DataModel;
using Vescon.Eplan.Utilities;
using Vescon.Eplan.Utilities.Misc;

namespace PdfTreeFix.Actions.Data
{
    public class ProjectInfo
    {
        public ProjectInfo(Project project, ProjectStructureHelper structureHelper)
        {
            Project = project;

            Pages = project.Pages
                .Select(x => new PageInfo(x, structureHelper))
                .ToList();

            Locations = project.GetLocationObjects()
                .Select(x => new LocationInfo(x, structureHelper.GetStructurePropertySubPartSeparator(x.HierarchyId.GetPropertyId())))
                .ToList();
        }

        public Project Project { get; }
        public List<PageInfo> Pages { get; }
        public List<LocationInfo> Locations { get; }

        public Dictionary<StructurePropertyId, List<string>> DistinctNodeNames { get; } = new Dictionary<StructurePropertyId, List<string>>();
    }
}