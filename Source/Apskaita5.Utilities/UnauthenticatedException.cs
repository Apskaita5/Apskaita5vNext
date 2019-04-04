using System;
using System.Collections.Generic;
using System.Text;

namespace Apskaita5.Common
{
    /// <summary>
    /// Throw this exception when the user is not authenticated (not loged in).
    /// </summary>
    [Serializable]
    public class UnauthenticatedException : Exception
    {

        public UnauthenticatedException() : base() { }

        public UnauthenticatedException(string message) : base(message) { }

        private UnauthenticatedException(string message, Exception innerException) : base(message, innerException) { }

    }
}
