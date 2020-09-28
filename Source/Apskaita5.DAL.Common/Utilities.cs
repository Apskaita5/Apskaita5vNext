using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Xml;
using System.Xml.Serialization;
using Apskaita5.DAL.Common.DbSchema;

namespace Apskaita5.DAL.Common
{
    public static class Utilities
    {
        
        private static readonly DbDataType[] _integerDbDataTypes = new DbDataType[]
            {DbDataType.Integer, DbDataType.IntegerBig, DbDataType.IntegerMedium, 
            DbDataType.IntegerSmall, DbDataType.IntegerTiny};

        /// <summary>
        /// Gets a value indicating whether the specified DbDataType value is an integer type.
        /// </summary>
        /// <param name="dbDataType">The DbDataType value to check.</param>
        public static bool IsDbDataTypeInteger(this DbDataType dbDataType)
        {
            return (_integerDbDataTypes.Contains(dbDataType));
        }

        /// <summary>
        /// Gets a value indicating whether the specified DbDataType value is a floting point type.
        /// </summary>
        /// <param name="dbDataType">The DbDataType value to check.</param>
        public static bool IsDbDataTypeFloat(this DbDataType dbDataType)
        {
            return (dbDataType == DbDataType.Float || dbDataType == DbDataType.Double);
        }


        /// <summary>
        /// Serializes any object to XML string using <see cref="XmlSerializer">XmlSerializer</see>.
        /// </summary>
        /// <typeparam name="T">a type of the object to serialize</typeparam>
        /// <param name="objectToSerialize">an object to serialize</param>
        /// <returns>XML string</returns>
        /// <remarks>need a fixed implementation in order to guarantee file read/write consistency
        /// an object should be XML serializable in order the method to work</remarks>
        internal static string SerializeToXml<T>(T objectToSerialize)
        {

            var xmlSerializer = new XmlSerializer(typeof(T));
            
            var settings = new XmlWriterSettings
                {
                    Indent = true,
                    IndentChars = " ",
                    Encoding = Constants.DefaultXmlFileEncoding
                };

            using (var ms = new MemoryStream())
            {
                using (var writer = XmlWriter.Create(ms, settings))
                {
                    xmlSerializer.Serialize(writer, objectToSerialize);
                    return Constants.DefaultXmlFileEncoding.GetString(ms.ToArray());
                }
            }

        }

        /// <summary>
        /// Deserializes any object from XML string using <see cref="XmlSerializer">XmlSerializer</see>.
        /// </summary>
        /// <typeparam name="T">a type of the object to deserialize</typeparam>
        /// <param name="xmlString">an XML string that contains the deserializable object data</param>
        /// <returns>an object of type T</returns>
        /// <remarks>need a fixed implementation in order to guarantee file read/write consistency
        /// an object should be XML serializable in order the method to work</remarks>
        internal static T DeSerializeFromXml<T>(string xmlString)
        {

            var xmlSerializer = new XmlSerializer(typeof(T));

            using (var textReader = new StringReader(xmlString))
            {
                return (T)xmlSerializer.Deserialize(textReader);
            }

        }


        /// <summary>
        /// Gets a string source by it's index in the delimited string.
        /// Returns string.Empty if the line is empty or if there is no source at fieldIndex.
        /// </summary>
        /// <param name="line">a line (string) that contains delimited fields</param>
        /// <param name="fieldIndex">a zero based index of the source to get</param>
        /// <param name="fieldDelimiter">a string that delimits fields in line</param>
        internal static string GetDelimitedField(this string line, int fieldIndex, string fieldDelimiter)
        {

            if (null == fieldDelimiter || fieldDelimiter.Length < 1)
                throw new ArgumentNullException(nameof(fieldDelimiter));

            if (line.IsNullOrWhiteSpace()) return string.Empty;

            var result = line.Split(new string[] { fieldDelimiter }, StringSplitOptions.None);

            if (result.Length < (fieldIndex + 1)) return string.Empty;

            return result[fieldIndex];

        }

        /// <summary>
        /// Returns true if the string value is null or empty or consists from whitespaces only.
        /// </summary>
        /// <param name="value">a string value to evaluate</param>
        internal static bool IsNullOrWhiteSpace(this string value)
        {
            return (null == value || string.IsNullOrEmpty(value.Trim()));
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


        /// <summary>
        /// Gets a description of SQL statement/query parameters.
        /// </summary>
        /// <param name="parameters">the SQL statement/query parameters to get a description for</param>
        public static string GetDescription(this SqlParam[] parameters)
        {
            if (null == parameters || parameters.Length < 1) return "null";
            return string.Join("; ", parameters.Select(p => p.ToString()).ToArray());
        }

        /// <summary>
        /// Gets a formated name of database object (field, table, index) using formatting
        /// policy defined by an SqlAgent.
        /// </summary>
        /// <param name="unformatedName">unformatted name of the database object</param>
        /// <param name="sqlAgent">an SqlAgent that defines naming convention</param>
        public static string ToConventional(this string unformatedName, ISqlAgent sqlAgent)
        {
            if (sqlAgent.IsNull()) throw new ArgumentNullException(nameof(sqlAgent));
            if (unformatedName.IsNullOrWhiteSpace()) throw new ArgumentNullException(nameof(unformatedName));
            if (sqlAgent.AllSchemaNamesLowerCased) return unformatedName.Trim().ToLower();
            return unformatedName.Trim();
        }

        /// <summary>
        /// Compares database object (field, table, index) names. As the changing object names is
        /// not implemented, it's always case insensitive comparison (Trim + OrdinalIgnoreCase).
        /// </summary>
        /// <param name="source">value to compare</param>
        /// <param name="valueToCompare">value to compare against</param>
        public static bool EqualsByConvention(this string source, string valueToCompare)
        {
            return (source?.Trim() ?? string.Empty).Equals(valueToCompare?.Trim() ?? string.Empty, 
                StringComparison.OrdinalIgnoreCase);
        }

        /// <summary>
        /// Returns a value indicating whether a specified substring occures within the string, 
        /// it's always case insensitive comparison (Trim + OrdinalIgnoreCase).
        /// </summary>
        /// <param name="source">value to compare</param>
        /// <param name="substring">value to compare against</param>
        public static bool ContainsByConvention(this string source, string substring)
        {
            return (source ?? string.Empty).IndexOf(substring?.Trim() ?? string.Empty,
                StringComparison.OrdinalIgnoreCase) >= 0;
        }

        public static IEnumerable<FieldInfo> GetStaticFields<T>()
        {
            var fields = new List<FieldInfo>();

            var type = typeof(T);

            while (type != null)
            {
                fields.AddRange(type.GetFields(BindingFlags.NonPublic | BindingFlags.Public
                    | BindingFlags.Static | BindingFlags.DeclaredOnly));
                type = type.GetTypeInfo().BaseType;
            }

            return fields;
        }

        /// <summary>
        /// Gets a DateTime containing DateTime.Now timestamp with a second precision.
        /// </summary>
        /// <remarks>Required by most SQL engines.</remarks>
        internal static DateTime GetCurrentTimeStamp()
        {
            var result = DateTime.UtcNow;
            result = new DateTime((long)(Math.Floor((double)(result.Ticks / TimeSpan.TicksPerSecond))
                * TimeSpan.TicksPerSecond), DateTimeKind.Utc);
            return result;
        }

    }
}
