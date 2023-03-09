using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Reflection;
using Vescon.Eplan.Utilities.Exceptions;

namespace Vescon.Eplan.Utilities
{
    public class EplanAssemblyResolver : IDisposable
    {
#if EPLAN32bit
        private const string RequiredMinimumEplanVersion = "2.3.5";
        private const string MaximumEplanVersion = "2.3.5";
#else
        private const string RequiredMinimumEplanVersion = "2.4.4";
#endif
        private const string RequiredEplanVariant = "Electric P8";
        private const string DllExtension = ".dll";
        private const string ErxExtension = ".erx";
        private const string EplanEplApiModule = "EplanEplApiModuleu";

        private static string _eplanBinPath;
        private static string _eplanPlatformBinPath;

        public EplanAssemblyResolver()
        {
            AppDomain.CurrentDomain.AssemblyResolve += OnAssemblyResolve;
        }

        public static IEnumerable<string> GetInstalledEplanVersions()
        {
            return new EplanFinder()
                .GetInstalledEplanVersions()
                .Select(x => $"{x.Version} - {x.Variant}")
                .ToList();
        }

        public static string GetEplanBinPath()
        {
            return _eplanBinPath;
        }

        public static Assembly OnAssemblyResolve(object sender, ResolveEventArgs args)
        {
            var assemblyName = args.Name.Split(',')[0];

            Debug.WriteLine("--- Trying to resolve assembly '{0}' in app domain {1}...", assemblyName, AppDomain.CurrentDomain.Id);

            if (!assemblyName.StartsWith("Eplan.")
                && assemblyName != EplanEplApiModule)
            {
                return null; // not an Eplan assembly
            }

            var binPath = DetermineEplanPlatformBinPath(assemblyName);
            var assemblyPath = GetAssemblyPath(binPath, assemblyName);

            if (!File.Exists(assemblyPath))
            {
                Debug.WriteLine("### Eplan assembly '{0}' not resolved. Path not found: {1}", assemblyName, assemblyPath);
                return null; // the dll is missing
            }

            Debug.WriteLine("--- Eplan assembly '{0}' resolved: {1}", assemblyName, assemblyPath);
            return Assembly.LoadFile(assemblyPath);
        }

        public void Dispose()
        {
            EplanApplicationStarter.Stop();
            AppDomain.CurrentDomain.AssemblyResolve -= OnAssemblyResolve;
        }

        private static string DetermineEplanPlatformBinPath(string assemblyName)
        {
            if (_eplanPlatformBinPath != null)
                return _eplanPlatformBinPath;

            var eplanFinder = new EplanFinder();

            var usableEplanVersions = eplanFinder
                .GetInstalledEplanVersions()
                .Where(IsRequiredVersion)
                .OrderBy(x => x.Version)
                .ToList();

            if (!usableEplanVersions.Any())
                throw new EplanNotInstalledException($"Required version of Eplan ({RequiredMinimumEplanVersion}, {RequiredEplanVariant}) not found!");

            foreach (var version in usableEplanVersions)
            {
                _eplanBinPath = Path.GetDirectoryName(version.Path);

                var binPath = eplanFinder.GetPlatformBinPath(_eplanBinPath);
                var path = GetAssemblyPath(binPath, assemblyName);

                if (File.Exists(path))
                {
                    _eplanPlatformBinPath = binPath;
                    break;
                }
            }

            if (_eplanPlatformBinPath == null)
                throw new EplanNotInstalledException("Eplan P8 installation not found.");

            return _eplanPlatformBinPath;
        }

        private static string GetAssemblyPath(string directory, string assemblyName)
        {
            var ext = assemblyName == EplanEplApiModule ? ErxExtension : DllExtension; 
            return Path.Combine(directory, assemblyName) + ext;
        }

        private static bool IsRequiredVersion(EplanFinder.EplanVersion eplanVersion)
        {
            if (eplanVersion.Variant != RequiredEplanVariant)
                return false;

            var parts = eplanVersion.Version.Split('.');
            if (parts.Length < 3)
                return false;
#if EPLAN32bit
            if (string.Compare(eplanVersion.Version, MaximumEplanVersion, StringComparison.Ordinal) > 0)
                return false;
#endif
            return string.Compare(RequiredMinimumEplanVersion, eplanVersion.Version, StringComparison.Ordinal) <= 0;
        }
    }
}