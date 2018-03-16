using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace Apskaita5.DAL.Common
{
    /// <summary>
    /// Represents a wrapper class to get SQL implementation specific SQL queries by tokens.
    /// </summary>
    /// <remarks>Singleton object that is only accessible for SqlAgentBase implementations.</remarks>
    internal sealed class SqlDictionary
    {

        private static SqlDictionary _current = null;
        private static readonly object _dictLock = new object();
        private static readonly object _instanceLock = new object();

        private Dictionary<Type, Dictionary<string,string>> _tokenDictionary = null;


        /// <summary>
        /// Initializes a new SqlDictionary instance. 
        /// Do not use this constructor. It is only ment to support XML serialization.
        /// </summary>
        private SqlDictionary() 
        {
        }


        /// <summary>
        /// Gets an SqlDictionary instance for the SqlAgent implementation specified or crestes one if not found.
        /// </summary>
        /// <param name="sqlAgent">an ISqlAgent instance for which the SqlDictionary should be initialized.</param>
        /// <param name="skipDuplicateTokens">whether to skip SQL queries with duplicate tokens
        /// (otherwise duplicate tokens cause exception)</param>
        /// <param name="throwOnNoRepositoryFound">whether to throw an exception if no SQL repository
        /// files are found.</param>
        /// <exception cref="ArgumentNullException">Parameter sqlAgent is not specified.</exception>
        /// <exception cref="InvalidOperationException">SQL repository path is not initialized.</exception>
        /// <exception cref="ArgumentException">SQL agent does not implement repository file prefix.</exception>
        /// <exception cref="FileNotFoundException">No repository files found or they contain no data 
        /// for the SQL agent type specified.</exception>
        /// <exception cref="Exception">Failed to load file due to missing query tokens.</exception>
        /// <exception cref="Exception">Failed to load file due to duplicate query token.</exception>
        private Dictionary<string, string> GetOrCreateDictionary(SqlAgentBase sqlAgent, 
            bool skipDuplicateTokens, bool throwOnNoRepositoryFound)
        {

            if (sqlAgent == null) throw new ArgumentNullException(nameof(sqlAgent));
            if (sqlAgent.SqlRepositoryFileNamePrefix.IsNullOrWhiteSpace())
                throw new ArgumentException(Properties.Resources.SqlDictionary_RepositoryFilePrefixNotImplemented);
            if (sqlAgent.SqlRepositoryPath.IsNullOrWhiteSpace())
                throw new InvalidOperationException(Properties.Resources.SqlDictionary_RepositoryPathNotInitialized);

            if (_tokenDictionary == null)
            {
                _tokenDictionary=new Dictionary<Type, Dictionary<string, string>>();
            }

            if (!_tokenDictionary.ContainsKey(sqlAgent.GetType()))
            {
                lock (_dictLock)
                {
                    if (!_tokenDictionary.ContainsKey(sqlAgent.GetType()))
                        _tokenDictionary.Add(sqlAgent.GetType(), CreateDictionary(sqlAgent, false, true));
                }
            }

            return _tokenDictionary[sqlAgent.GetType()];

        }

        private Dictionary<string, string> CreateDictionary(SqlAgentBase sqlAgent, bool skipDuplicateTokens,
            bool throwOnNoRepositoryFound)
        {

            var result = new Dictionary<string, string>();

            foreach (var filePath in GetSqlRepositoryFiles(sqlAgent.SqlRepositoryPath,
                sqlAgent.SqlRepositoryFileNamePrefix))
            {
                ReadSqlRepositoryFile(filePath, result, skipDuplicateTokens);
            }

            if (_tokenDictionary.Count < 1 && throwOnNoRepositoryFound)
                throw new FileNotFoundException(string.Format(Properties.Resources.SqlDictionary_RepositoryFilesNotFound,
                    sqlAgent.GetType().FullName));

            return result;

        }

        private IEnumerable<string> GetSqlRepositoryFiles(string repositoryPath, string requiredPrefix)
        {

            var result = new List<string>();

            foreach (var filePath in System.IO.Directory.GetFiles(repositoryPath,
                "*.*", SearchOption.AllDirectories))
            {
                var fileExtension = System.IO.Path.GetExtension(filePath);
                if (fileExtension != null && fileExtension.ToLower().StartsWith(Constants.SqlRepositoryFileExtension))
                {
                    var fileName = System.IO.Path.GetFileName(filePath);
                    if (fileName != null && fileName.Trim().ToLower().StartsWith(
                        requiredPrefix.Trim().ToLower()))
                    {
                        result.Add(filePath);
                    }
                }
            }

            return result;

        }

        private void ReadSqlRepositoryFile(string filePath, Dictionary<string, string> result, 
            bool skipDuplicateTokens)
        {

            var repository = new SqlRepository();
            try
            {
                repository.LoadXml(System.IO.File.ReadAllText(filePath, new UTF8Encoding(false)));
            }
            catch (Exception)
            {
                try
                {
                    repository.LoadXml(System.IO.File.ReadAllText(filePath, new UTF8Encoding(true)));
                }
                catch (Exception)
                {
                    try
                    {
                        repository.LoadXml(System.IO.File.ReadAllText(filePath, Encoding.Unicode));
                    }
                    catch (Exception)
                    {
                        repository.LoadXml(System.IO.File.ReadAllText(filePath, Encoding.ASCII));
                    }
                }
            }

            foreach (var entry in repository)
            {
                if (entry.Token.IsNullOrWhiteSpace())
                    throw new Exception(string.Format(Properties.Resources.SqlDictionary_NullTokensInRepository,
                        filePath)); //TODO: replace to FileFormatException

                if (result.ContainsKey(entry.Token.Trim().ToLower()))
                {
                    if (!skipDuplicateTokens) 
                        throw new Exception(string.Format(Properties.Resources.SqlDictionary_DuplicateTokensInRepository,
                            filePath, entry.Token)); //TODO: replace to FileFormatException
                }
                else
                {
                    result.Add(entry.Token.Trim().ToLower(), entry.Query.Trim());
                }
            }

        }


        /// <summary>
        /// Gets an SQL query identified by the token for the specific SQL implementation sqlAgent.
        /// Uses ICacheProvider specified by sqlAgent to cache the SQL dictionary.
        /// </summary>
        /// <param name="token">a token (code) that identifies an SQL query in SQL repository</param>
        /// <param name="sqlAgent">an SQL implementation to get the SQL query for</param>
        /// <exception cref="ArgumentNullException">Parameter token is not specified.</exception>
        /// <exception cref="ArgumentNullException">Parameter sqlAgent is not specified.</exception>
        /// <exception cref="InvalidOperationException">SQL repository path is not initialized.</exception>
        /// <exception cref="ArgumentException">SQL agent does not implement repository file prefix.</exception>
        /// <exception cref="FileNotFoundException">No repository files found or they contain no data 
        /// for the SQL agent type specified.</exception>
        /// <exception cref="Exception">Failed to load file due to missing query tokens.</exception>
        /// <exception cref="Exception">Failed to load file due to duplicate query token.</exception>
        /// <exception cref="InvalidOperationException">SQL dictionary failed to initialize for unknown reason.</exception>
        /// <exception cref="InvalidOperationException">SQL query token is unknown.</exception>
        internal static string GetSqlQuery(string token, SqlAgentBase sqlAgent)
        {

            if (token.IsNullOrWhiteSpace()) throw new ArgumentNullException(nameof(token));
            if (sqlAgent == null) throw new ArgumentNullException(nameof(sqlAgent));

            if (_current == null)
            {
                lock (_instanceLock)
                {
                    if (_current == null) _current = new SqlDictionary();
                }
            }

            var dictionaryForSqlAgent = _current.GetOrCreateDictionary(sqlAgent, false, true);

            if (dictionaryForSqlAgent.ContainsKey(token.Trim().ToLower()))
            {
                return dictionaryForSqlAgent[token.Trim().ToLower()];
            }

            throw new InvalidOperationException(string.Format(Properties.Resources.SqlDictionary_UnknownSqlQueryToken, token));

        }

    }
}
