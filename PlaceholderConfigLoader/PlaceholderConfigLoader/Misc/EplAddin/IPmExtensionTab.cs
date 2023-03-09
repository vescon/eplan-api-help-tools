namespace PlaceholderConfigLoader.EplAddin
{
    public interface IPmExtensionTab
    {
        void ItemSelected(string itemType, string key, string partNumber, string partVariant);

        void ItemSaved(string itemType, string key, string partNumber, string partVariant);
    }
}