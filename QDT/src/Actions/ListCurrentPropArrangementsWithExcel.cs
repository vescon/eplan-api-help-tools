using Eplan.EplApi.ApplicationFramework;

namespace Vescon.EplAddin.Qdt.Actions
{
    public class ListCurrentPropArrangementsWithExcel : ActionBase, IEplAction
    {
        public override string MenuCaption => "List current property arrangements (and open in Excel)";
        public override string Description => "Lists current property arrangements of project's symbol references to a file and opens the file in Excel.";

        public override bool Execute(ActionCallingContext ctx)
        {
            return Execute(() =>
            {
                var output = new ListCurrentPropArrangements().PrepareOutput();

                if (output != null)
                    SaveOutput(output, true);

                return true;
            });
        }
    }
}