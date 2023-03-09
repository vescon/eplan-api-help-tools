using System;
using System.Security.Cryptography;
using System.Text;
using System.Threading;
using Vescon.Common.Extensions;

namespace Vescon.Eplan.Utilities
{
    public class IgnoreSystemWide : IDisposable
    {
        private readonly Mutex _mutex;
        private readonly bool _isSingleInstance;

        private IgnoreSystemWide(string instanceIdentifier)
        {
            var mutexName = @"Global\" + HashSha256(instanceIdentifier);

            bool createdNew;
            _mutex = new Mutex(true, mutexName, out createdNew);

            _isSingleInstance = createdNew;
        }

        public static T IfSingleInstance<T>(string instanceIdentifier, T returnValueIfRunning, Func<T> action)
        {
            using (var x = new IgnoreSystemWide(instanceIdentifier))
            {
                return x._isSingleInstance 
                    ? action() 
                    : returnValueIfRunning;
            }
        }

        public void Dispose()
        {
            _mutex.Close();
            _mutex.Dispose();
        }

        private static string HashSha256(string text)
        {
            var sb = new StringBuilder();
            var hash = new SHA256Managed().ComputeHash(Encoding.UTF8.GetBytes(text), 0, Encoding.UTF8.GetByteCount(text));
            hash.ForEach(x => sb.Append(x.ToString("X2")));
            return sb.ToString();
        }
    }
}