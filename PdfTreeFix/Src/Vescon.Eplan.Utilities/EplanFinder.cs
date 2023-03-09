using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using Microsoft.Win32;
using Vescon.Common.Extensions;

namespace Vescon.Eplan.Utilities
{
    /// <remarks>
    /// Based on the code from Eplan.EplApi.Starteru.dll
    /// </remarks>
    internal class EplanFinder
    {
        public EplanVersion[] GetInstalledEplanVersions()
        {
            return GetInstalledEplanVersions(RegistryView.Registry32)
                .Concat(GetInstalledEplanVersions(RegistryView.Registry64))
                .ToArray();
        }

        public string GetPlatformBinPath(string eplanBinDirectory)
        {
            var platformBinPath = eplanBinDirectory;
            if (IsPlatformInPath(platformBinPath) || IsOldVariantWithoutPlatform(platformBinPath))
                return platformBinPath;

            var directoryInfo = new DirectoryInfo(platformBinPath);
            var parent = directoryInfo.Parent;
            if (parent != null 
                && parent.Parent != null 
                && parent.Parent.Parent != null)
            {
                platformBinPath = parent.Parent.Parent.FullName + @"\Platform\" + parent.Name + @"\" + directoryInfo.Name;
                
                if (new DirectoryInfo(platformBinPath).Exists
                    && IsPlatformInPath(platformBinPath))
                {
                    return platformBinPath;
                }
            }

            throw new ArgumentException("The API dlls are not found in path " + platformBinPath);
        }

        private bool IsPlatformInPath(string path)
        {
            var fileInfo = new FileInfo(path + @"\W3Sharedu.dll");
            return fileInfo.Exists;
        }

        private bool IsOldVariantWithoutPlatform(string path)
        {
            var fileInfo = new FileInfo(path + @"\W3u.exe");
            if (!fileInfo.Exists)
                return false;
            
            var versionInfo = FileVersionInfo.GetVersionInfo(fileInfo.FullName);
            return versionInfo.FileMajorPart < 2
                || (versionInfo.FileMajorPart == 2 && versionInfo.FileMinorPart < 1);
        }

        private IEnumerable<EplanVersion> GetInstalledEplanVersions(RegistryView view)
        {
            var rslt = new List<EplanVersion>();

            var baseKey = RegistryKey.OpenBaseKey(RegistryHive.LocalMachine, view);

            var mainKeyName = @"SOFTWARE\EPLAN\EPLAN W3";
            var mainKey = baseKey.OpenSubKey(mainKeyName);
            if (mainKey == null)
                return rslt;
            
            mainKey.GetSubKeyNames()
                .ForEach(x =>
                {
                    var subKeyName = mainKeyName + @"\" + x;
                    var subKey = baseKey.OpenSubKey(subKeyName);
                    if (subKey == null)
                        return;

                    subKey.GetSubKeyNames()
                        .ForEach(y =>
                        {
                            var registryKey4 = baseKey.OpenSubKey(subKeyName + @"\" + y + @"\SystemStartUp");
                            if (registryKey4 == null)
                                return;

                            var exePath = (string)registryKey4.GetValue("Main");
                            if (string.IsNullOrEmpty(exePath))
                                return;

                            exePath = exePath.TrimStart('"').TrimEnd('"');
                            var endIndex = exePath.IndexOf(".exe", StringComparison.Ordinal) + 4;
                            if (endIndex < exePath.Length)
                                exePath = exePath.Remove(endIndex);

                            var fileInfo = new FileInfo(exePath);
                            if (!fileInfo.Exists)
                                return;
                            
                            if (baseKey.OpenSubKey(subKeyName + @"\InstallInfo\Properties") != null
                                && ((string)registryKey4.GetValue("ApplicationType")).Equals("11"))
                                return;
                            
                            rslt.Add(new EplanVersion
                            {
                                Path = exePath,
                                Variant = x,
                                Version = y
                            });
                        });
                });

            return rslt;
        }
        
        internal class EplanVersion
        {
            public string Path { get; set; }
            public string Variant { get; set; }
            public string Version { get; set; }
        }
    }
}
