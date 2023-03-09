using System;
using Eplan.EplApi.ApplicationFramework;
using Eplan.EplApi.DataModel;
using Vescon.Common.Extensions;
using Vescon.Eplan.Utilities;
using Vescon.Eplan.Utilities.Action;
using Vescon.Eplan.Utilities.Misc;

namespace PdfTreeFix.Actions
{
    abstract class BaseMoveDescriptions : ActionBase
    {
        public override bool Execute(ActionCallingContext ctx)
        {
            return Execute(() =>
            {
                Project project;
                if (!GetCurrentProject(out project))
                    return true;

                var structureHelper = new ProjectStructureHelper(project);

                if (!Confirm(GetConfirmationMessage(structureHelper)))
                    return true;

                using (new UndoManager().CreateStep(Addin.Title + ", " + MenuCaption))
                using (new LockingStep())
                {
                    structureHelper.PagesNavigatorStructure
                        .ForEach(x =>
                        {
                            var locations = project.GetLocationObjects(x.GetLocationType());
                            locations
                                .ForEach(y =>
                                {
                                    y.LockObject();
                                    
                                    MoveDescription(y);
                                });
                        });
                }

                Info("Done." 
                    + Environment.NewLine + "(Please, select the Pages Navigator and press F5 to refresh it.)");

                return true;
            });
        }

        protected override string Title => Addin.Title;

        protected abstract string GetConfirmationMessage(ProjectStructureHelper structureHelper);

        protected abstract void MoveDescription(Location l);
    }
}