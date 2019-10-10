using System;
using System.Data.SQLite;
using Apskaita5.DAL.Common;

namespace Apskaita5.DAL.SQLite
{
    internal static class Extensions
    {

        internal static bool IsNullOrWhiteSpace(this string source)
        {
            return (null == source || source.Trim().Length < 1);
        }

        /// <summary>
        /// Returns a value indicating that the object (value) is null. Required due to potential operator overloadings
        /// that cause unpredictable behaviour of standard null == value test.
        /// </summary>
        /// <typeparam name="T">a type of the object to test</typeparam>
        /// <param name="value">an object to test against null</param>
        internal static bool IsNull<T>(this T value) where T : class
        {
            return ReferenceEquals(value, null) || DBNull.Value == value;
        }


        internal static Exception WrapSqlException(this Exception target)
        {

            if (!target.IsNull() && target.GetType() == typeof(AggregateException))
                target = ((AggregateException)target).Flatten().InnerExceptions[0];

            var typedException = target as SQLiteException;
            if (typedException.IsNull()) return target;

            return new SqlException(string.Format(Properties.Resources.SqlExceptionMessage,
                typedException.ErrorCode, typedException.HResult, typedException.Message),
                typedException.ErrorCode, string.Empty, typedException);
        }

        internal static Exception WrapSqlException(this Exception target, string statement)
        {

            if (!target.IsNull() && target.GetType() == typeof(AggregateException))
                target = ((AggregateException)target).Flatten().InnerExceptions[0];

            var typedException = target as SQLiteException;
            if (typedException.IsNull()) return target;

            return new SqlException(string.Format(Properties.Resources.SqlExceptionMessageWithStatement,
                typedException.ErrorCode, typedException.HResult, typedException.Message, Environment.NewLine, statement),
                typedException.ErrorCode, statement, typedException);
        }

        internal static Exception WrapSqlException(this Exception target, string statement, Exception rollbackException)
        {

            if (!target.IsNull() && target.GetType() == typeof(AggregateException))
                target = ((AggregateException)target).Flatten().InnerExceptions[0];
            if (!rollbackException.IsNull() && rollbackException.GetType() == typeof(AggregateException))
                rollbackException = ((AggregateException)rollbackException).Flatten().InnerExceptions[0];

            var typedException = rollbackException as SQLiteException;
            if (typedException.IsNull()) return rollbackException;

            string initialExceptionDescription;
            var initialException = target as SQLiteException;
            if (initialException.IsNull())
            {
                initialExceptionDescription = string.Format(Properties.Resources.NonSqlExceptionDescription,
                    target?.GetType().FullName, target?.Message);
            }
            else
            {
                initialExceptionDescription = string.Format(Properties.Resources.SqlExceptionMessage,
                    initialException.ErrorCode, initialException.HResult, initialException.Message);
            }

            return new SqlException(string.Format(Properties.Resources.SqlExceptionMessageRollbackFailed,
                typedException.ErrorCode, typedException.HResult, typedException.Message, Environment.NewLine,
                initialExceptionDescription, statement), typedException.ErrorCode, statement, typedException);

        }

    }
}
