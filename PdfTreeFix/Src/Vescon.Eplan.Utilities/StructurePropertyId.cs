using System.ComponentModel;

namespace Vescon.Eplan.Utilities
{
    public enum StructurePropertyId
    {
        [Description("Higher-level function / Plant (=), [11]")]
        HigherLevelFunction = 1120,

        [Description("Mounting location (+), [12]")]
        Location = 1220, 

        [Description("Functional assignment (==), [13]")]
        FunctionalAssignment = 1320,

        [Description("Installation site / Place of installation (++), [14]")]
        InstallationSite = 1420,

        [Description("Document type (&), [15]")]
        DocumentType = 1520,

        [Description("User defined (#), [16]")]
        UserDefined = 1620,

        [Description("Higher-level function number / Installation number [17]")]
        HlfNumber = 1720
    }
}
