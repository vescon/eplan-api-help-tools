using Eplan.EplApi.ApplicationFramework;
using Eplan.EplApi.Base;
using Eplan.EplApi.DataModel;
using Vescon.Eplan.Utilities;

namespace PdfTreeFix.Actions
{
    class RestoreDescriptions : BaseMoveDescriptions, IEplAction
    {
        public override string Description => "Restores nodes' descriptions from the 'Description 2' property.";
        public override string MenuCaption => "Restore nodes' descriptions from the 'Description 2' property.";

        public override string IconPath => @"toolbar\Icon4.jpg";
        
        protected override string GetConfirmationMessage(ProjectStructureHelper structureHelper)
        {
            return "Descriptions of the following structure nodes will be restored:"
                + Addin.GetExistingNodesMessage(structureHelper, true);            
        }

        protected override void MoveDescription(Location l)
        {
            var mls = !l.Properties.LOCATION_DESCRIPTION_2.IsEmpty
              ? l.Properties.LOCATION_DESCRIPTION_2.ToMultiLangString()
              : new MultiLangString();

            l.Properties.LOCATION_DESCRIPTION = mls;
        }
    }
}