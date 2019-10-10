using System;

namespace Apskaita5.DAL.Common
{

    /// <summary>
    /// Represents an abstract exception that has been thrown by some SQL implementation.
    /// </summary>
    public class SqlException : Exception
    {

        /// <summary>
        /// A code of the exception as defined by the SQL server implementation.
        /// </summary>
        public int Code { get; }

        /// <summary>
        /// An SQL statement that caused an exception.
        /// </summary>
        public string Statement { get; } = string.Empty;

        /// <summary>
        /// A rollback exception if the transaction rollback failed after the initial exception.
        /// </summary>
        public Exception RollbackException { get; } = null;


        public SqlException(string message, int code, string statement, Exception innerException) :
            base(message, innerException)
        {
            Code = code;
            Statement = statement ?? string.Empty;
        }

        public SqlException(string message, int code, string statement, Exception innerException, Exception rollbackException) :
            this(message, code, statement, innerException)
        {
            RollbackException = rollbackException;
        }

    }
}
