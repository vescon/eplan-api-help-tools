using System;

namespace Vescon.Eplan.Utilities.Exceptions
{
    public class EplanNotInstalledException : Exception
    {
        public EplanNotInstalledException(string message)
            : base(message)
        {
        }
    }
}