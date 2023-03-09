public class RegisterAddin
{
    private const string AddinDll32 = "Vescon.PlaceholderConfigLoader.EplAddin.1.dll";
    private const string AddinType32 = "Vescon.PlaceholderConfigLoader.EplAddin.1";
    private const string AddinDll64 = "Vescon.PlaceholderConfigLoader.EplAddin.1.x64.dll";
    private const string AddinType64 = "Vescon.PlaceholderConfigLoader.EplAddin.1.x64";

    [Start]
    public void RegisterScript()
    {
        string addinDll;
        string addinType;
        if (typeof(CommandLineInterpreter).Assembly.GetName().ProcessorArchitecture == System.Reflection.ProcessorArchitecture.X86)
        {
            addinDll = AddinDll32;
            addinType = AddinType32;
        }
        else
        {
            addinDll = AddinDll64;
            addinType = AddinType64;
        }

        var cli = new CommandLineInterpreter();
        try
        {
            cli.Execute("EplApiModuleAction /unregisterInternal:\"" + addinType + "\"");
        }
        catch
        {
        }

        try
        {
            var addinPath = System.IO.Path.Combine(GetAddinBinFolder(), addinDll);
            System.IO.File.ReadAllBytes(addinPath);

            cli.Execute("EplApiModuleAction /register:\"" + addinPath);
        }
        catch (System.Exception ex)
        {
            MessageBox.Show(ex.ToString(), string.Empty, MessageBoxButtons.OK, MessageBoxIcon.Stop);
        }
    }

    private string GetAddinBinFolder()
    {
        string result = new Settings().GetExpandedStringSetting("USER.FileSelection.1.PermamentSelection.FolderName", 0);
        return result;
    }
}