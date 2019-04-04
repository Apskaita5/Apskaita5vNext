using System;
using System.Collections.Generic;
using System.Text;

namespace Apskaita5.Common
{
    /// <summary>
    /// Throw this exception when the user is authenticated but not authorized to perform a specific action.
    /// </summary>
    [Serializable]
    public class UnauthorizedException : Exception
    {

        public UnauthorizedException() : base() { }

        public UnauthorizedException(string message) : base(message) { }

        private UnauthorizedException(string message, Exception innerException) : base(message, innerException) { }

    }
}
