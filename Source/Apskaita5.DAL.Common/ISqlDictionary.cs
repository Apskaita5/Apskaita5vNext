using System;
using System.Collections.Generic;
using System.Text;

namespace Apskaita5.DAL.Common
{
    /// <summary>
    /// Abstracts SQL dictionary (token-query pair) functionality.
    /// </summary>
    public interface ISqlDictionary
    {
        /// <summary>
        /// Initializes SQL dictionary, i.e. loads data from files or other external sources.
        /// </summary>
        void Initialize();

        /// <summary>
        /// Gets an SQL query or statement by the token for the SQL agent specified.
        /// </summary>
        /// <param name="token">a token (key, name) of the requested query or statement</param>
        /// <param name="sqlAgent">an SQL agent for which the SQL query or statement is meant for</param>
        string GetSqlQuery(string token, ISqlAgent sqlAgent);

    }
}
