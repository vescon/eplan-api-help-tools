using System.Collections.Generic;
using System.Linq;
using System.Xml;
using System.Xml.Linq;
using Eplan.EplApi.ApplicationFramework;
using Eplan.EplApi.DataModel;

namespace Vescon.EplAddin.Qdt.Actions.Old
{
    public class CorrectCustomPropArrangements_Old : CorrectCustomPropArrangementsBase, IEplAction
    {
        public override string MenuCaption => "Correct property arrangements";
        public override string Description => "Corrects property arrangements after property arrangements import.";

        protected override bool TryCorrectSymbol(SymbolReference symbolRef)
        {
            if (symbolRef.PropertyPlacementsSchemas.All.All(x => !x.IsCustom))
                return false;

            using (var tempFileName = new TemporaryFileName())
            {
                symbolRef.PropertyPlacementsSchemas.Export(tempFileName);

                var customSchemas = ReadSchemas(tempFileName)
                    .Select(x => new PropertyPlacementsSchema(x))
                    .ToList();

                var currentSchema = new PropertyPlacementsSchema(symbolRef);

                var schemaToSet = customSchemas.FirstOrDefault(x => x.EqualsOld(currentSchema));
                if (schemaToSet == null) 
                    return false;
                
                symbolRef.PropertyPlacementsSchemas.Selected = symbolRef.PropertyPlacementsSchemas.All.First(x => x.Name == schemaToSet.Name);
                return true;
            }
        }

        private static IEnumerable<XElement> ReadSchemas(string inputUrl)
        {
            using (var reader = XmlReader.Create(inputUrl))
            {
                reader.MoveToContent();
                while (reader.Read())
                {
                    if (reader.NodeType == XmlNodeType.Element)
                    {
                        if (reader.Name == "O155")
                        {
                            if (XNode.ReadFrom(reader) is XElement el)
                                yield return el;
                        }
                    }
                }
            }
        }
    }
}