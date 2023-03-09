public class UnRegisterAddin
{
    private const string AddinType32 = "Vescon.PlaceholderConfigLoader.EplAddin.1";
    private const string AddinType64 = "Vescon.PlaceholderConfigLoader.EplAddin.1.x64";

    [Start]
    public void RegisterScript()
    {
        string addinType;
        if (typeof(CommandLineInterpreter).Assembly.GetName().ProcessorArchitecture == System.Reflection.ProcessorArchitecture.X86)
        {
            addinType = AddinType32;
        }
        else
        {
            addinType = AddinType64;
        }

        var cli = new CommandLineInterpreter();
        try
        {
            cli.Execute("EplApiModuleAction /unregister:\"" + addinType + "\"");
        }
        catch (System.Exception ex)
        {
            MessageBox.Show(ex.ToString(), string.Empty, MessageBoxButtons.OK, MessageBoxIcon.Stop);
        }
    }
}