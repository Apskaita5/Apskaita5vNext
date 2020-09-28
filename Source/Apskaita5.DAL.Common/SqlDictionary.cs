using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace Apskaita5.DAL.Common
{
    /// <summary>
    /// Represents a wrapper class to get SQL implementation specific SQL queries by tokens.
    /// </summary>
    /// <remarks>Is meant to use as a singleton object.
    /// Possible initialization paths:
    /// - init by constructor;
    /// - init by dedicated method;
    /// - init on first request (not recommended; throws errors too late).</remarks>
    public sealed class SqlDictionary : ISqlDictionary
    {

        private Dictionary<string, Dictionary<string,string>> _tokenDictionary = null;
        private readonly object _dictLock = new object();


        /// <summary>
        /// Gets a path to the folder where the relevant SqlRepository files are located
        /// </summary>
        public string FolderPath { get; }


        private SqlDictionary() {}

        /// <summary>
        /// Creates a new instance of SQL dictionary.
        /// </summary>
        /// <param name="folderPath">path to the folder where the relevant SqlRepository files are located</param>
        public SqlDictionary(string folderPath) : this(folderPath, false) { }

        /// <summary>
        /// Creates a new instance of SQL dictionary.
        /// </summary>
        /// <param name="folderPath">path to the folder where the relevant SqlRepository files are located</param>
        /// <param name="init">whether to initialize dictionary, i.e. load data from files</param>
        public SqlDictionary(string folderPath, bool init)
        {
            if (folderPath.IsNullOrWhiteSpace()) throw new ArgumentNullException(nameof(folderPath));
            FolderPath = folderPath;
            if (init) Initialize();
        }


        /// <summary>
        /// Gets an SQL query or statement by the token for the SQL agent specified.
        /// </summary>
        /// <param name="token">a token (key, name) of the requested query or statement</param>
        /// <param name="sqlAgent">an SQL agent for which the SQL query or statement is meant for</param>
        /// <exception cref="ArgumentNullException">Parameters token or sqlAgent are not specified.</exception>
        /// <exception cref="FileNotFoundException">No repository files found or they contain no data 
        /// for the SQL agent type specified.</exception>
        /// <exception cref="InvalidDataException">Failed to load SQL repository file due to bad format or duplicate query tokens.</exception>
        /// <exception cref="InvalidOperationException">SQL query token is unknown or SQL query dictionary is not available for the SQL implementation.</exception>
        public string GetSqlQuery(string token, ISqlAgent sqlAgent)
        {

            if (token.IsNullOrWhiteSpace()) throw new ArgumentNullException(nameof(token));
            if (sqlAgent.IsNull()) throw new ArgumentNullException(nameof(sqlAgent));

            if (_tokenDictionary.IsNull())
            {
                lock (_dictLock)
                {
                    if (_tokenDictionary.IsNull()) Initialize();
                }
            }

            if (!_tokenDictionary.ContainsKey(sqlAgent.SqlImplementationId))
                throw new InvalidOperationException(string.Format(Properties.Resources.SqlDictionaryNotAvailableException,
                    sqlAgent.Name));

            var dictionaryForSqlAgent = _tokenDictionary[sqlAgent.SqlImplementationId];

            if (!dictionaryForSqlAgent.ContainsKey(token.Trim()))
                throw new InvalidOperationException(string.Format(Properties.Resources.SqlDictionary_UnknownSqlQueryToken, token));

            return dictionaryForSqlAgent[token.Trim()];

        }


        /// <summary>
        /// Initializes SQL dictionary, i.e. loads data from files in the repository folder.
        /// </summary>
        public void Initialize()
        {

            _tokenDictionary = new Dictionary<string, Dictionary<string, string>>(StringComparer.OrdinalIgnoreCase);

            var repository = new SqlRepository();

            foreach (var filePath in GetSqlRepositoryFiles())
            {                   
                repository.LoadFile(filePath, true);
                try
                {
                    if (_tokenDictionary.ContainsKey(repository.SqlImplementation.Trim()))
                    {
                        repository.MergeIntoDictionary(_tokenDictionary[repository.SqlImplementation.Trim()]);
                    }
                    else _tokenDictionary.Add(repository.SqlImplementation.Trim(), repository.GetDictionary());
                }
                catch (InvalidDataException ex)
                {   
                    throw new InvalidDataException(string.Format(Properties.Resources.DuplicateTokensInRepositoryException,
                        filePath, ex.Message));
                }
            }

        }

        private IEnumerable<string> GetSqlRepositoryFiles()
        {
            return Directory.GetFiles(FolderPath, "*.*", SearchOption.AllDirectories)
                .Where(f => (Path.GetExtension(f) ?? string.Empty).StartsWith(
                    Constants.SqlRepositoryFileExtension, StringComparison.OrdinalIgnoreCase));
        }

    }
}
