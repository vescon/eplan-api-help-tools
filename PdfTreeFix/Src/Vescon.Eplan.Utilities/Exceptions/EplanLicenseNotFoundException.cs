using System;

namespace Vescon.Eplan.Utilities.Exceptions
{
    public class EplanLicenseNotFoundException : Exception
    {
        public EplanLicenseNotFoundException()
            : base("Please check Eplan P8 license availablity.")
        {
        }
    }
}