using System.Collections.Generic;
using Eplan.EplApi.Base;
using Eplan.EplApi.DataModel;
using Vescon.Common.Extensions;

namespace PdfTreeFix.Actions.Data
{
    public class LocationInfo
    {
        public const string Undetermined = "?";

        public LocationInfo(
            Location location, 
            string subPartSeparator)
        {
            Location = location;
            HierarchyId = location.HierarchyId;

            Name = GetName(location, subPartSeparator);
        }

        public Location Location { get; }

        public Project.Hierarchy HierarchyId { get; }
        public string Name { get; }
        
        public string CalculatedText { get; private set; }

        public override string ToString()
        {
            return $"{HierarchyId}, {Name}";
        }

        public void SetDescription(
            string text,
            bool withStoredDescription)
        {
            var mls = withStoredDescription && !Location.Properties.LOCATION_DESCRIPTION_2.IsEmpty 
                  ? Location.Properties.LOCATION_DESCRIPTION_2.ToMultiLangString() 
                  : new MultiLangString();

            if (text.IsNotEmpty())
            {
                if (!mls.ContainsData())
                {
                    mls.AddString(ISOCode.Language.L___, text);
                }
                else
                {
                    var languages = new LanguageList();
                    mls.GetLanguageList(ref languages);
                    for (var i = 0; i < languages.Count; i++)
                    {
                        var language = languages.get_Language(i);
                        var descr = mls.GetString(language);
                        mls.AddString(language, descr.IsEmpty() ? text : text + ", " + descr);
                    }
                }
            }

            Location.LockObject();
            Location.Properties.LOCATION_DESCRIPTION = mls;

            CalculatedText = text;
        }

        private static string GetName(Location location, string subPartSeparator)
        {
            var name = new List<string>();

            var hierarchyId = location.HierarchyId;
            while (location != null 
                   && location.HierarchyId == hierarchyId)
            {
                name.Insert(0, location.Name);

#if VERSION_23
#pragma warning disable 618
                location = location.GetParent();
#pragma warning restore 618
#elif VERSION_24
#pragma warning disable 618
                location = location.GetParent();
#pragma warning restore 618
#else
                location = location.ParentNode;
#endif
            }

            return name.Concatenate(subPartSeparator);
        }

    }
}