using System.Threading;
using Eplan.EplApi.System;
using Vescon.Eplan.Utilities.Exceptions;

namespace Vescon.Eplan.Utilities
{
    public static class EplanApplicationStarter 
    {
        private static readonly object EplanApplicationLock = new object();
        private static EplanApplication _eplanApplication;

        private static bool _started;

        public static void ThrowIfEplanApiNotInitialized()
        {
            lock (EplanApplicationLock)
            {
                if (!_started)
                    throw new EplanApiNotInitializedException();
            }
        }

        public static string RunningVersion => _eplanApplication?.Version;

        public static string Start()
        {
            lock (EplanApplicationLock)
            {
                if (!_started)
                {
                    _eplanApplication = new EplanApplication(StartEplanApplication());
                    _started = true;
                }

                return RunningVersion;
            }
        }

        public static void Stop()
        {
            lock (EplanApplicationLock)
            {
                if (!_started)
                    return;

                _eplanApplication.Exit();
                _eplanApplication = null;

                _started = false;
            }
        }

        private static EplApplication StartEplanApplication()
        {
            if (Thread.CurrentThread.GetApartmentState() != ApartmentState.STA)
                throw new ThreadStateException("The thread apartment state which access the eplan api is required to be STA");

            var eplApplication = new EplApplication { EplanBinFolder = EplanAssemblyResolver.GetEplanBinPath() };
            eplApplication.Init("Basic");

            if (!eplApplication.License.ToUpperInvariant().Contains("EPLAN"))
                throw new EplanLicenseNotFoundException();

            return eplApplication;
        }

        private class EplanApplication
        {
            private readonly EplApplication _eplApplication;

            public EplanApplication(EplApplication eplApplication)
            {
                _eplApplication = eplApplication;

                // this will be executed if the dongle is removed while Eplan is running
                EplApplication.LicenseRuntimeCheckEvent += (errorId, errorMessage, mode) => EplApplication.LicenseRuntimeCheckCommands.Retry;
            }

            public string Version => _eplApplication.Version;

            public void Exit()
            {
                _eplApplication.Exit();
            }
        }
    }
}