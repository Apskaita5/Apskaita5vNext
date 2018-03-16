using System;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Xml;
using System.Xml.Serialization;

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

            if (fieldDelimiter == null || fieldDelimiter.Length < 1)
                throw new ArgumentNullException(nameof(fieldDelimiter));

            if (line == null || string.IsNullOrEmpty(line.Trim()))
                return string.Empty;

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
            return (value == null || string.IsNullOrEmpty(value.Trim()));
        }

        /// <summary>
        /// Returns string.Empty if the value is null, otherwise returns value.
        /// </summary>
        /// <param name="value">a string value to evaluate</param>
        /// <returns>string.Empty if the value is null, otherwise - value</returns>
        internal static string NotNullValue(this string value)
        {
            if (value == null) return string.Empty;
            return value;
        }


        /// <summary>
        /// Tries to parse source string as an integer and returns the result if succeeds,
        /// otherwise returns the defaultValue.
        /// </summary>
        /// <param name="source">the string value to parse</param>
        /// <param name="defaultValue">a default integer value to return if the parse fails</param>
        internal static int ParseInt(this string source, int defaultValue)
        {
            try
            {
                return int.Parse(source, NumberStyles.Any, CultureInfo.InvariantCulture);
            }
            catch (Exception)
            {
                return defaultValue;
            }
        }

        /// <summary>
        /// Tries to parse source string as an enum value and returns the result if succeeds,
        /// otherwise returns the defaultValue.
        /// </summary>
        /// <param name="source">the string value to parse</param>
        /// <param name="defaultValue">a default enum value to return if the parse fails</param>
        internal static T ParseEnum<T>(this string source, T defaultValue)
        {
            
            if (source.IsNullOrWhiteSpace()) return defaultValue;
            
            try
            {
                return (T)Enum.Parse(typeof(T), source);
            }
            catch (Exception)
            {

                if (int.TryParse(source, NumberStyles.Any, CultureInfo.InvariantCulture, out int intValue))
                {
                    if (Enum.IsDefined(typeof(T), intValue)) return (T)(object)intValue;
                }

                return defaultValue;

            }

        }

        /// <summary>
        /// Tries to parse source string as a boolean and returns the result if succeeds,
        /// otherwise returns the defaultValue.
        /// </summary>
        /// <param name="source">the string value to parse</param>
        /// <param name="defaultValue">a default boolean value to return if the parse fails</param>
        internal static bool ParseBoolean(this string source, bool defaultValue)
        {
            if (source.IsNullOrWhiteSpace()) return defaultValue;

            if (source.Trim().ToUpperInvariant() == "TRUE") return true;

            if (source.Trim().ToUpperInvariant() == "FALSE") return false;

            if (int.TryParse(source, NumberStyles.Any, CultureInfo.InvariantCulture, out int intValue))
                return (intValue != 0);

            return defaultValue;

        }

    }
}
