using Eplan.EplApi.ApplicationFramework;
using Eplan.EplApi.Base;
using Eplan.EplApi.DataModel;
using Vescon.Eplan.Utilities;

namespace PdfTreeFix.Actions
{
    class MoveDescriptions : BaseMoveDescriptions, IEplAction
    {
        public override string Description => "Copies nodes' descriptions to the 'Description 2' property.";
        public override string MenuCaption => "Copy nodes' descriptions to the 'Description 2' property.";

        public override string IconPath => @"toolbar\Icon2.jpg";
        
        protected override string GetConfirmationMessage(ProjectStructureHelper structureHelper)
        {
            return "Descriptions of the following structure nodes will be copied:"
               + Addin.GetExistingNodesMessage(structureHelper, false);            
        }

        protected override void MoveDescription(Location l)
        {
            var mls = !l.Properties.LOCATION_DESCRIPTION.IsEmpty
                ? l.Properties.LOCATION_DESCRIPTION.ToMultiLangString()
                : new MultiLangString();

            l.Properties.LOCATION_DESCRIPTION_2 = mls;
        }
    }
}