using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using Eplan.EplApi.DataModel;
using Vescon.Common.Extensions;
using Vescon.Eplan.Utilities.Misc;

namespace Vescon.Eplan.Utilities
{
    public class ProjectStructureHelper
    {
        private readonly string _pddHierarchyPages;

        private readonly string[] _structureDelimiters;

        public ProjectStructureHelper(Project project)
        {
            Project = project;

            _pddHierarchyPages = GetStringProperty(EplanPropertyId.ProjectPddHierarchyPages);

            var identifyingStructurePages = GetStringProperty(EplanPropertyId.ProjectIdentifyingStructurePages);
            var completeStructurePages = GetStringProperty(EplanPropertyId.ProjectCompleteStructurePages);

            var descriptiveStructsVisiblePages = GetBoolProperty(EplanPropertyId.ProjectDescriptiveStructsVisiblePages);

            PagesNavigatorStructure = descriptiveStructsVisiblePages
                ? GetPagesStructureProps(completeStructurePages)
                : GetPagesStructureProps(identifyingStructurePages);

            _structureDelimiters = GetStringsArray(GetStringProperty((EplanPropertyId)10018));
        }

        public Project Project { get; }

        public StructurePropertyId[] PagesNavigatorStructure { get; }

        public string GetStructurePropertyName(StructurePropertyId id)
        {
            switch (id)
            {
                case StructurePropertyId.HigherLevelFunction:
                    return $"Higher-level function ({GetStructurePropertyPrefix(id)})";

                case StructurePropertyId.Location:
                    return $"Mounting location ({GetStructurePropertyPrefix(id)})";

                case StructurePropertyId.FunctionalAssignment:
                    return $"Functional assignment ({GetStructurePropertyPrefix(id)})";

                case StructurePropertyId.InstallationSite:
                    return $"Installation site ({GetStructurePropertyPrefix(id)})";

                case StructurePropertyId.DocumentType:
                    return $"Document type ({GetStructurePropertyPrefix(id)})";

                case StructurePropertyId.UserDefined:
                    return $"User defined ({GetStructurePropertyPrefix(id)})";

                case StructurePropertyId.HlfNumber:
                    return $"Higher-level function number ({GetStructurePropertyPrefix(id)})";

                default:
                    throw new ArgumentOutOfRangeException(nameof(id), id, null);
            }    
        }

        public string GetStructurePropertyPrefix(StructurePropertyId id)
        {
            switch (id)
            {
                case StructurePropertyId.FunctionalAssignment:
                    return _structureDelimiters[0];

                case StructurePropertyId.HigherLevelFunction:
                    return _structureDelimiters[2];

                case StructurePropertyId.InstallationSite:
                    return _structureDelimiters[4];

                case StructurePropertyId.Location:
                    return _structureDelimiters[6];

                case StructurePropertyId.HlfNumber:
                    return _structureDelimiters[8];

                case StructurePropertyId.DocumentType:
                    return _structureDelimiters[10];

                case StructurePropertyId.UserDefined:
                    return _structureDelimiters[12];

                default:
                    throw new ArgumentOutOfRangeException(nameof(id), id, null);
            }    
        }

        public string GetStructurePropertySubPartSeparator(StructurePropertyId id)
        {
            switch (id)
            {
                case StructurePropertyId.FunctionalAssignment:
                    return _structureDelimiters[1];

                case StructurePropertyId.HigherLevelFunction:
                    return _structureDelimiters[3];

                case StructurePropertyId.InstallationSite:
                    return _structureDelimiters[5];

                case StructurePropertyId.Location:
                    return _structureDelimiters[7];

                case StructurePropertyId.HlfNumber:
                    return _structureDelimiters[9];

                case StructurePropertyId.DocumentType:
                    return _structureDelimiters[11];

                case StructurePropertyId.UserDefined:
                    return _structureDelimiters[13];

                default:
                    throw new ArgumentOutOfRangeException(nameof(id), id, null);
            }    
        }

        private StructurePropertyId[] GetPagesStructureProps(string format)
        {
            var formatIds = GetIntsFromAngleBrackets(format);

            int[] pagePddIds;
            var aa = GetIntsArrays(_pddHierarchyPages);
            if (aa.Length == 0)
            {
                Debug.WriteLine("Project doesn't contain page structure in prop 10092.");
                pagePddIds = formatIds;
            }
            else
            {
                pagePddIds = aa
                    .Where(x => x[2] == 1)
                    .Select(x => x[0])
                    .ToArray();
            }

            var props = pagePddIds
                .SelectMany(x =>
                {
                    if (formatIds.Contains(x))
                        switch (x)
                        {
                            case 10001:
                                return StructurePropertyId.FunctionalAssignment.AsArray();
                            case 10002:
                                return StructurePropertyId.HigherLevelFunction.AsArray();
                            case 10003:
                                return StructurePropertyId.InstallationSite.AsArray();
                            case 10004:
                                return StructurePropertyId.Location.AsArray();
                            case 10005:
                                return StructurePropertyId.HlfNumber.AsArray();
                            case 10006:
                                return StructurePropertyId.DocumentType.AsArray();
                            case 10007:
                                return StructurePropertyId.UserDefined.AsArray();
                        }

                    return new StructurePropertyId[0];
                })
                .ToArray();

            if (props.IsEmpty())
                return new[]
                {
                    StructurePropertyId.FunctionalAssignment,
                    StructurePropertyId.HigherLevelFunction,
                    StructurePropertyId.InstallationSite,
                    StructurePropertyId.Location,
                    StructurePropertyId.HlfNumber,
                    StructurePropertyId.DocumentType,
                    StructurePropertyId.UserDefined
                };

            return props;
        }

        private bool GetBoolProperty(EplanPropertyId propertyId)
        {
            var v = GetStringProperty(propertyId).Trim().ToLowerInvariant();
            return v == "true" 
                || v == "1" 
                || v == "y" 
                || v == "yes";
        }

        private string GetStringProperty(EplanPropertyId propertyId)
        {
            return Project.Properties[(int)propertyId].ToStringSafe();
        }

        private int[] GetIntsFromAngleBrackets(string s)
        {
            if (s.IsEmpty())
                return new int[0];

            var values = s
                .Replace('>', '|')
                .Replace('<', '|')
                .Split("|".ToCharArray(), StringSplitOptions.RemoveEmptyEntries);

            return values
                .Select(x =>
                {
                    int n;
                    if (int.TryParse(x, out n))
                        return (int?)n;
                    return null;
                })
                .Where(x => x != null)
                .Select(x => x.Value)
                .ToArray();
        }

        private int[][] GetIntsArrays(string s)
        {
            if (s.IsEmpty())
                return new int[0][];

            var a = s.Split('\t');
            int step;
            if (a.Length == 0
                || !int.TryParse(a[0], out step))
                return new int[0][];

            var arrays = new List<int[]>();

            for (var i = 1; i < a.Length; i += step)
            {
                var aa = new int[step];
                for (var ii = 0; ii < step; ++ii)
                {
                    if (int.TryParse(a[i + ii], out aa[ii]))
                        continue;
                    aa = null;
                    break;
                }

                if (aa != null)
                    arrays.Add(aa);
            }

            return arrays.ToArray();
        }

        private string[] GetStringsArray(string s)
        {
            if (s.IsEmpty())
                return new string[0];

            var a = s.Split('\t');
            int count;
            if (a.Length == 0
                || !int.TryParse(a[0], out count))
                return new string[0];

            var aa = new string[count];

            for (var i = 1; i < a.Length && i <= count; ++i)
                aa[i - 1] = a[i];

            return aa;
        }
    }
}