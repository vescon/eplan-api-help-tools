using System.Collections.Generic;

namespace PlaceholderConfigLoader.Data
{
    public class PlaceholderVariablesSet
    {
        public bool Enabled { get; set; }

        public string Name { get; set; }

        public Dictionary<string, object> Variables { get; set; }
    }
}