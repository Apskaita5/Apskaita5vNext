using System;

namespace Apskaita5.DAL.Common
{

    /// <summary>
    /// Reperesents an entry in an SQL repository and contains SQL query 
    /// for a particular SQL implementation identified by a unique token.
    /// </summary>
    [Serializable]
    public sealed class SqlRepositoryItem
    {
        private string _token = string.Empty;
        private string _query = string.Empty;
        private string _usedByTypes = string.Empty;


        /// <summary>
        /// Gets or sets the token (unique code) of the SQL query in the SQL repository.
        /// </summary>
        public string Token
        {
            get { return _token; }
            set { _token = value.NotNullValue().Trim(); }
        }

        /// <summary>
        /// Gets or sets an SQL query for a particular SQL implementation (e.g. MySql, SQLite, etc.)
        /// </summary>
        public string Query
        {
            get { return _query; }
            set { _query = value.NotNullValue().Trim(); }
        }

        /// <summary>
        /// Gets or sets (business) classes that use the SQL query.
        /// Classes naming format is: Namespace.Class; Namespace.Class; etc.
        /// </summary>
        public string UsedByTypes
        {
            get { return _usedByTypes; }
            set { _usedByTypes = value.NotNullValue().Trim(); }
        }

    }
}
