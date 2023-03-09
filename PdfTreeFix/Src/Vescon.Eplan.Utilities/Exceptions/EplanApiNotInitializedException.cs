using System;

namespace Vescon.Eplan.Utilities.Exceptions
{
    public class EplanApiNotInitializedException : Exception
    {
        public EplanApiNotInitializedException()
            : base("Eplan API must be initialized before (e.g. by calling the EplanAssemblyResolver.StartEplan method).")
        {
        }
    }
}