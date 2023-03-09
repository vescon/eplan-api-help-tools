using System;
using System.Diagnostics;
using System.IO;

namespace Vescon.EplAddin.Qdt
{
    public class TemporaryFileName : IDisposable
    {
        private readonly string _fileName;

        public TemporaryFileName()
        {
            _fileName = Path.GetTempFileName();
        }

        public static implicit operator string(TemporaryFileName x)
        {
            return x._fileName;
        }

        public void Dispose()
        {
            try
            {
                File.Delete(_fileName);
            }
            catch (Exception ex)
            {
                Debug.WriteLine(ex.Message);
            }
        }
    }
}