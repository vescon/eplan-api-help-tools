using System;
using System.IO;
using SystemIoPath = System.IO.Path;

namespace Vescon.EplAddin.Qdt
{
    public class TemporaryDirectory : IDisposable
    {
        public TemporaryDirectory()
        {
            Path = SystemIoPath.Combine(SystemIoPath.GetTempPath(), Guid.NewGuid().ToString());
            Directory.CreateDirectory(Path);
        }

        public TemporaryDirectory(string directory)
        {
            Path = directory;
            Directory.CreateDirectory(Path);
        }

        public string Path { get; }
        public bool Preserve { get; set; }

        public void Dispose()
        {
            if (!Preserve && Directory.Exists(Path))
                Directory.Delete(Path, true);
        }
    }
}