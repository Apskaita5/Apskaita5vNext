using System;

namespace Apskaita5.DAL.Common
{

    /// <summary>
    /// Represents an abstract exception that has been thrown by some SQL implementation.
    /// </summary>
    public class SqlException : Exception
    {

        public int Code { get; private set; }

        public SqlException(string message, int code, Exception innerException) :
            base(message, innerException)
        {
            Code = code;
        }

    }
}
