using System.Linq;

using Eplan.EplApi.ApplicationFramework;
using Eplan.EplApi.DataModel;

namespace Vescon.EplAddin.Qdt.Actions
{
    public class CorrectCustomPropArrangementsWithStandard : CorrectCustomPropArrangementsBase, IEplAction
    {
        public override string MenuCaption => "Correct property arrangements (standard schemas included)";
        public override string Description => "Corrects property arrangements after property arrangements import. (Standard arrangements included)";

        protected override bool TryCorrectSymbol(SymbolReference symbolRef)
        {
            var userDefinedSchemaObj = symbolRef.PropertyPlacementsSchemas.Selected;
            var userDefinedSchema = new PropertyPlacementsSchema(symbolRef);

            var allSchemas = symbolRef.PropertyPlacementsSchemas.All
                .Where(x => x.ID != 128)
                .ToList();

            foreach (var schema in allSchemas)
            {
                symbolRef.PropertyPlacementsSchemas.Selected = schema;
                if (new PropertyPlacementsSchema(symbolRef).Equals(userDefinedSchema))
                    return true;
            }

            symbolRef.PropertyPlacementsSchemas.Selected = userDefinedSchemaObj;
            return false;
        }
    }
}