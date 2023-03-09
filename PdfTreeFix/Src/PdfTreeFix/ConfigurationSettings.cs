using System.Collections.Generic;

namespace PdfTreeFix
{
    public class ConfigurationSettings
    {
        public bool GenerateCompactDescriptions { get; set; }

        public List<StructurePropertySetting> NodeSettings { get; set; } = new List<StructurePropertySetting>();
    }
}