public class RegisterAddon
{
    private const string Caption = "Vescon.PdfTreeFix.EplAddin";

    [Start]
    public void Execute()
    {
        string installFile = null;

        var path = typeof(CommandLineInterpreter).Assembly.Location;
        var version = new System.IO.DirectoryInfo(path).Parent.Parent.Name;
        
        if (version.StartsWith("2.3"))
        {
            installFile = System.IO.Path.Combine(GetAddinBinFolder(), @"2.3\cfg\install.xml");
        }
        else if (version.StartsWith("2.4"))
        {
            installFile = System.IO.Path.Combine(GetAddinBinFolder(), @"2.4\cfg\install.xml");
        }
        else
        {
            installFile = System.IO.Path.Combine(GetAddinBinFolder(), @"2.6\cfg\install.xml");
        }

        var cli = new CommandLineInterpreter();
        
        try
        {
            // check if the addin's dll is possible to read
            System.IO.File.ReadAllBytes(installFile);
            
            cli.Execute("XSettingsRegisterAction /installFile:\"" + installFile + "\"" + " /reinstall:1");
        }
        catch (System.Exception ex)
        {
            MessageBox.Show(ex.ToString(), Caption, MessageBoxButtons.OK, MessageBoxIcon.Stop);
        }
    }

    private string GetAddinBinFolder()
    {
        string result = new Settings().GetExpandedStringSetting("USER.FileSelection.1.PermamentSelection.FolderName", 0);
        return result;
    }
}