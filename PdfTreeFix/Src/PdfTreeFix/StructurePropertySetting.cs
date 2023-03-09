using System.Collections.Generic;
using Vescon.Eplan.Utilities;

namespace PdfTreeFix
{
    public class StructurePropertySetting
    {
        public StructurePropertyId PropertyId { get; set; }

        public bool Enabled { get; set; }

        public List<SubSetting> SubSettings { get; } = new List<SubSetting>();

        public bool DescriptionIncluded { get; set; }

        public bool OnlyTopNode { get; set; } // i.e. do not generate description at sub-structures

        public class SubSetting
        {
            public StructurePropertyId PropertyId { get; set; }

            public bool Enabled { get; set; }

            public int Limit { get; set; }
        }
    }
}
