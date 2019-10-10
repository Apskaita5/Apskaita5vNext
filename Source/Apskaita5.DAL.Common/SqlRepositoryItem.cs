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
            get { return _token ?? string.Empty; }
            set { _token = value?.Trim() ?? string.Empty; }
        }

        /// <summary>
        /// Gets or sets an SQL query for a particular SQL implementation (e.g. MySql, SQLite, etc.)
        /// </summary>
        public string Query
        {
            get { return _query ?? string.Empty; }
            set { _query = value?.Trim() ?? string.Empty; }
        }

        /// <summary>
        /// Gets or sets (business) classes that use the SQL query.
        /// Classes naming format is: Namespace.Class; Namespace.Class; etc.
        /// </summary>
        public string UsedByTypes
        {
            get { return _usedByTypes ?? string.Empty; ; }
            set { _usedByTypes = value?.Trim() ?? string.Empty; }
        }


        public SqlRepositoryItem() {}

        internal SqlRepositoryItem(string delimitedString, string fieldDelimiter)
        {    
            _token = delimitedString.GetDelimitedField(0, fieldDelimiter)?.Trim() ?? string.Empty;
            _query = delimitedString.GetDelimitedField(1, fieldDelimiter)?.Trim() ?? string.Empty;
            _usedByTypes = delimitedString.GetDelimitedField(2, fieldDelimiter)?.Trim() ?? string.Empty;
        }


        /// <summary>
        /// Converts the SQL repository entry into a delimited string:
        /// {Token}{delimiter}{Query}{delimiter}{UsedByTypes}
        /// </summary>
        /// <param name="fieldDelimiter">field delimiter to use</param>
        public string ToDelimitedString(string fieldDelimiter)
        {
            if (null == fieldDelimiter || fieldDelimiter.Length < 1) throw new ArgumentNullException(nameof(fieldDelimiter));
            return string.Format("{0}{1}{2}{1}{3}", Token, fieldDelimiter, Query, UsedByTypes);
        }

    }
}
