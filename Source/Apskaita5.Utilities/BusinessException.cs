using System;
using System.Collections.Generic;
using System.Text;

namespace Apskaita5.Common
{
    /// <summary>
    /// Use this exception to designate the exceptions due to business rules that do not require extensive background information.
    /// </summary>
    [Serializable]
    public class BusinessException : Exception
    {

        private BusinessException() : base() { }

        public BusinessException(string message) : base(message) { }

        public BusinessException(string message, Exception innerException) : base(message, innerException) { }

    }

}
