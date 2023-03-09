namespace PlaceholderConfigLoader.Data
{
    public class PlaceholderConfig
    {
        public string Name { get; set; }

        public bool ClearAll { get; set; }

        public PlaceholderVariablesSet[] Sets { get; set; }
    }
}