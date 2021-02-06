using System;
using System.Threading;

namespace Core_by_MainDen.System
{
    public class Access : IDisposable
    {
        private readonly object __lockObject;

        public Access()
        {
            __lockObject = new object();
        }

        public Access GetAccess()
        {
            Monitor.Enter(__lockObject);
            return this;
        }

        public bool TryGetAccess(int millisecondsTimeout)
        {
            return Monitor.TryEnter(__lockObject, millisecondsTimeout);
        }

        public bool TryGetAccess(TimeSpan timeout)
        {
            return Monitor.TryEnter(__lockObject, timeout);
        }

        public void Dispose()
        {
            Monitor.Exit(__lockObject);
        }
    }
}
