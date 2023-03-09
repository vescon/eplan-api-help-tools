using System.Collections.Generic;
using System.Linq;
using System.Xml.Linq;

using Eplan.EplApi.DataModel;

namespace Vescon.EplAddin.Qdt
{
    public class PropertyPlacementsSchema
    {
        public PropertyPlacementsSchema(XElement schemaElement)
        {
            var attrs = schemaElement.Attributes();
            Name = attrs.FirstOrDefault(x => x.Name == "A2454")?.Value;

            Elements = schemaElement.Elements()
                .Where(x => x.Name == "S53x5")
                .Select(x => new PropertyPlacement(x))
                .ToList();
        }

        public PropertyPlacementsSchema(SymbolReference symbolRef)
        {
            Elements = symbolRef.PropertyPlacements.Select(x => new PropertyPlacement(x)).ToList();
        }

        public List<PropertyPlacement> Elements { get; set; }
        public string Name { get; set; }

        public bool EqualsOld(PropertyPlacementsSchema other)
        {
            if (Elements.Count != other.Elements.Count)
                return false;

            for (var i = 0; i < Elements.Count; ++i)
                if (!Elements[i].EqualsOld(other.Elements[i]))
                    return false;

            return true;
        }

        public bool Equals(PropertyPlacementsSchema other)
        {
            if (Elements.Count != other.Elements.Count)
                return false;

            for (var i = 0; i < Elements.Count; ++i)
                if (!Elements[i].Equals(other.Elements[i]))
                    return false;

            return true;
        }

        public override string ToString()
        {
            return $"Name: '{Name}', Elements count: {Elements.Count}";
        }
    }
}