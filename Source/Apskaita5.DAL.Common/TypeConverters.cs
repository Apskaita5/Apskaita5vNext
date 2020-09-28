using System;
using System.Collections.Generic;
using System.Globalization;
using System.Text;

namespace Apskaita5.DAL.Common.TypeConverters
{
    public static class TypeConverters
    {

        private static readonly string[] _defaultDateTimeFormats =
            new string[] { "yyyy-MM-dd HH:mm:ss", "yyyy-MM-dd", "HH:mm:ss" };
        private static readonly int[] _possibleGuidStringLengths =
            new int[] { 32, 36, 38, 61, 63 };


        /// <summary>
        /// Gets a value that indicates whether the object is null or DbNull.
        /// </summary>
        /// <param name="value">a value to check</param>
        public static bool IsNull(this object value)
        {
            return ReferenceEquals(value, null) || value == DBNull.Value;
        }


        /// <summary>
        /// Converts (coerces) object value to boolean. Throws an exception if the object value 
        /// cannot be (reasonably) converted to boolean type. 
        /// </summary>
        /// <param name="value">an object value to convert</param>
        /// <remarks>If the underlying value type is numeric, returns "> 0".
        /// If the underlying value type is string, tries to parse value as "true" or "false" (case insensitive)
        /// or to parse Int64 value using InvariantCulture and do "> 0" conversion. 
        /// if the underlying value type is byte[], tries to convert to string using UTF8 encoding 
        /// and parse as string.</remarks>
        /// <exception cref="FormatException">The object value is not in an appropriate format.</exception>
        public static bool? GetBooleanNullable(this object value)
        {

            if (value.IsNull()) return new bool?();

           if (value is bool)
                return (bool)value;

            if (value is sbyte)
                return (sbyte)value > 0;
            if (value is byte)
                return (byte)value > 0;
            if (value is short)
                return (short)value > 0;
            if (value is ushort)
                return (ushort)value > 0;
            if (value is int)
                return (int)value > 0;
            if (value is uint)
                return (uint)value > 0;
            if (value is long)
                return (long)value > 0;
            if (value is ulong)
                return (ulong)value > 0;

            if (TryReadString(value, out string stringValue) && !stringValue.IsNullOrWhiteSpace())
            {
                if (stringValue.Trim().ToUpperInvariant() == "TRUE")
                    return true;
                if (stringValue.Trim().ToUpperInvariant() == "FALSE")
                    return false;

                if (long.TryParse(stringValue, NumberStyles.Any, CultureInfo.InvariantCulture, out long intValue))
                    return (intValue > 0);
            }

            throw new FormatException(string.Format(Properties.Resources.TypeConverters_InvalidFormatException, 
                value.GetType().Name, "bool", value.ToString()));

        }

        /// <summary>
        /// Converts (coerces) object value to byte. Throws an exception if the object value 
        /// cannot be (reasonably) converted to byte type.
        /// </summary>
        /// <param name="value">an object value to convert</param>
        /// <remarks>Does checked conversions if the underlying value type is sbyte. 
        /// (narrowing conversion from int types does not make sense for byte)
        /// Tries to parse value using InvariantCulture if the underlying value type is string.
        /// Tries to convert to string using UTF8 encoding and parse value using InvariantCulture 
        /// if the underlying value type is byte array.</remarks>
        /// <exception cref="FormatException">The object value is not in an appropriate format.</exception>
        public static byte? GetByteNullable(this object value)
        {

            if (value.IsNull()) return new byte?();

            if (value is byte)
                return (byte)value;

            try
            {
                if (value is sbyte)
                    return checked((byte)(sbyte)value);
                if (value is ushort)
                    return checked((byte)(ushort)value);
                if (value is int)
                    return checked((byte)(int)value);
                if (value is uint)
                    return checked((byte)(uint)value);
                if (value is long)
                    return checked((byte)(long)value);
                if (value is ulong)
                    return checked((byte)(ulong)value);
            }
            catch (OverflowException)
            {
                throw new FormatException(string.Format(Properties.Resources.TypeConverters_OverflowException,
                    value is long ? ((ulong)value).ToString() : ((long)value).ToString(), "byte"));
            }             

            if (TryReadString(value, out string stringValue) && !stringValue.IsNullOrWhiteSpace())
            {
                if (byte.TryParse((string)value, NumberStyles.Any, CultureInfo.InvariantCulture, out byte byteValue))
                    return byteValue;
            }

            throw new FormatException(string.Format(Properties.Resources.TypeConverters_InvalidFormatException,
                value.GetType().Name, "byte", value.ToString()));

        }

        /// <summary>
        /// Converts (coerces) object value to sbyte. Throws an exception if the object value 
        /// cannot be (reasonably) converted to sbyte type.
        /// </summary>
        /// <param name="value">an object value to convert</param>
        /// <remarks>Only returns a sbyte value if the underlying value type is sbyte.</remarks>
        /// <exception cref="FormatException">The object value is not in an appropriate format.</exception>
        public static sbyte? GetSByteNullable(this object value)
        {

            if (value.IsNull()) return new sbyte?();

            if (value is sbyte)
                return (sbyte)value;

            throw new FormatException(string.Format(Properties.Resources.TypeConverters_InvalidFormatException,
                value.GetType().Name, "sbyte", value.ToString()));
        }

        /// <summary>
        /// Converts (coerces) object value to char. Throws an exception if the object value 
        /// cannot be (reasonably) converted to char type.
        /// </summary>
        /// <param name="value">an object value to convert</param>
        /// <remarks>Returns the first char (after Trim) if the underlying value type is string.
        /// Tries to convert to string using UTF8 encoding and to return the first char (after Trim)
        /// if the underlying value type is byte array.</remarks>
        /// <exception cref="FormatException">The object value is not in an appropriate format.</exception>
        public static char? GetCharNullable(this object value)
        {

            if (value.IsNull()) return new char?();

            if (value is char)
                return (char)value;

            if (TryReadString(value, out string stringValue) && stringValue != null && stringValue.Length > 0)
            {
                if (stringValue.Trim().Length > 0) return stringValue.Trim().ToCharArray()[0];
                else return stringValue.ToCharArray()[0];
            }

            throw new FormatException(string.Format(Properties.Resources.TypeConverters_InvalidFormatException,
                value.GetType().Name, "char", value.ToString()));

        }

        /// <summary>
        /// Converts (coerces) object value to Guid. Throws an exception if the object value 
        /// cannot be (reasonably) converted to Guid type.
        /// </summary>
        /// <param name="value">an object value to convert</param>
        /// <remarks>Returns Guid.Empty if the underlying value is null or empty string or empty byte array.
        /// Tries to parse value if the underlying value type is string or 16 byte array.
        /// Tries to convert to string using UTF8 encoding and parse the string 
        /// if the underlying value type is byte array and it's size is not 16.</remarks>
        /// <exception cref="FormatException">The object value is not in an appropriate format.</exception>
        public static Guid? GetGuidNullable(this object value)
        {

            if (value.IsNull()) return new Guid?();

            if (value is Guid)
                return (Guid)value;

            if (value is byte[])
            {
                if (((byte[])value).Length < 1)
                    return Guid.Empty;
                if (((byte[])value).Length == 16)
                    return new Guid((byte[])value);
            }

            if (TryReadString(value, out string stringValue))
            {
                if (stringValue.IsNullOrWhiteSpace())
                    return Guid.Empty;
                if (Array.IndexOf(_possibleGuidStringLengths, stringValue.Trim().Length) > -1)
                    return new Guid(stringValue.Trim());
            }

            throw new FormatException(string.Format(Properties.Resources.TypeConverters_InvalidFormatException,
                value.GetType().Name, "Guid", value.ToString()));

        }

        /// <summary>
        /// Converts (coerces) object value to short. Throws an exception if the object value 
        /// cannot be (reasonably) converted to short type.
        /// </summary>
        /// <param name="value">an object value to convert</param>
        /// <remarks>Does checked conversions if the underlying value type is numeric or decimal.
        /// Tries to parse value using InvariantCulture if the underlying value type is string.
        /// Tries to convert to string using UTF8 encoding and parse value using InvariantCulture 
        /// if the underlying value type is byte array.</remarks>
        /// <exception cref="FormatException">The object value is not in an appropriate format.</exception>
        public static short? GetInt16Nullable(this object value)
        {

            if (value.IsNull()) return new short?();

            if (value is short)
                return (short)value;

            if (value is sbyte)
                return (sbyte)value;
            if (value is byte)
                return (byte)value;

            try
            {
                if (value is ushort)
                    return checked((short)(ushort)value);
                if (value is int)
                    return checked((short)(int)value);
                if (value is uint)
                    return checked((short)(uint)value);
                if (value is long)
                    return checked((short)(long)value);
                if (value is ulong)
                    return checked((short)(ulong)value);
            }
            catch (OverflowException)
            {
                throw new FormatException(string.Format(Properties.Resources.TypeConverters_OverflowException,
                    value is long ? ((ulong)value).ToString() : ((long)value).ToString(), "Int16"));
            }

            
            if (value is decimal)
                return (short)(decimal)value;

            if (TryReadString(value, out string stringValue) && !stringValue.IsNullOrWhiteSpace())
            {
                if (long.TryParse(stringValue, NumberStyles.Any, CultureInfo.InvariantCulture, out long intValue))
                    return checked((short)intValue);
            }

            throw new FormatException(string.Format(Properties.Resources.TypeConverters_InvalidFormatException,
                value.GetType().Name, "Int16", value.ToString()));

        }

        /// <summary>
        /// Converts (coerces) object value to int. Throws an exception if the object value 
        /// cannot be (reasonably) converted to int type.
        /// </summary>
        /// <param name="value">an object value to convert</param>
        /// <remarks>Does checked conversions if the underlying value type is numeric or decimal.
        /// Tries to parse value using InvariantCulture if the underlying value type is string.
        /// Tries to convert to string using UTF8 encoding and parse value using InvariantCulture 
        /// if the underlying value type is byte array.</remarks>
        /// <exception cref="FormatException">The object value is not in an appropriate format.</exception>
        public static int? GetInt32Nullable(this object value)
        {

            if (value.IsNull()) return new Int32?();

            if (value is int)
                return (int)value;

            if (value is sbyte)
                return (sbyte)value;
            if (value is byte)
                return (byte)value;
            if (value is short)
                return (short)value;
            if (value is ushort)
                return (ushort)value;

            try
            {
                if (value is uint)
                    return checked((int)(uint)value);
                if (value is long)
                    return checked((int)(long)value);
                if (value is ulong)
                    return checked((int)(ulong)value);
            }
            catch (OverflowException)
            {
                throw new FormatException(string.Format(Properties.Resources.TypeConverters_OverflowException,
                    value is long ? ((ulong)value).ToString() : ((long)value).ToString(), "Int32"));
            }
                        
            if (value is decimal)
                return (int)(decimal)value;

            if (TryReadString(value, out string stringValue) && !stringValue.IsNullOrWhiteSpace())
            {
                if (long.TryParse(stringValue, NumberStyles.Any, CultureInfo.InvariantCulture, out long intValue))
                    return checked((int)intValue);
            }

            throw new FormatException(string.Format(Properties.Resources.TypeConverters_InvalidFormatException,
                value.GetType().Name, "Int32", value.ToString()));

        }

        /// <summary>
        /// Converts (coerces) object value to long. Throws an exception if the object value 
        /// cannot be (reasonably) converted to long type.
        /// </summary>
        /// <param name="value">an object value to convert</param>
        /// <remarks>Does checked conversions if the underlying value type is numeric or decimal.
        /// Tries to parse value using InvariantCulture if the underlying value type is string.
        /// Tries to convert to string using UTF8 encoding and parse value using InvariantCulture 
        /// if the underlying value type is byte array.</remarks>
        /// <exception cref="FormatException">The object value is not in an appropriate format.</exception>
        public static long? GetInt64Nullable(this object value)
        {

            if (value.IsNull()) return new long?();

            if (value is long)
                return (long)value;

            if (value is sbyte)
                return (sbyte)value;
            if (value is byte)
                return (byte)value;
            if (value is short)
                return (short)value;
            if (value is ushort)
                return (ushort)value;
            if (value is int)
                return (int)value;
            if (value is uint)
                return (uint)value;

            try
            {
                if (value is ulong)
                    return checked((long)(ulong)value);
            }
            catch (OverflowException)
            {
                throw new FormatException(string.Format(Properties.Resources.TypeConverters_OverflowException,
                    value is long ? ((ulong)value).ToString() : ((long)value).ToString(), "Int64"));
            } 
            
            if (value is decimal)
                return (long)(decimal)value;

            if (TryReadString(value, out string stringValue) && !stringValue.IsNullOrWhiteSpace())
            {
                if (long.TryParse(stringValue, NumberStyles.Any, CultureInfo.InvariantCulture, out long intValue))
                    return intValue;
            }

            throw new FormatException(string.Format(Properties.Resources.TypeConverters_InvalidFormatException,
                value.GetType().Name, "Int64", value.ToString()));

        }

        /// <summary>
        /// Converts (coerces) object value to float. Throws an exception if the object value 
        /// cannot be (reasonably) converted to float type.
        /// </summary>
        /// <param name="value">an object value to convert</param>
        /// <remarks>Tries to parse value using InvariantCulture if the underlying value type is string.
        /// Tries to convert to string using UTF8 encoding and parse value using InvariantCulture 
        /// if the underlying value type is byte array.</remarks>
        /// <exception cref="FormatException">The object value is not in an appropriate format.</exception>
        public static float? GetFloatNullable(this object value)
        {
            if (value.IsNull()) return new float?();

            if (value is float)
                return (float)value;

            if (TryReadString(value, out string stringValue) && !stringValue.IsNullOrWhiteSpace())
            {
                if (float.TryParse(stringValue, NumberStyles.Any, CultureInfo.InvariantCulture, out float floatValue))
                    return floatValue;
            }

            throw new FormatException(string.Format(Properties.Resources.TypeConverters_InvalidFormatException,
                value.GetType().Name, "float", value.ToString()));
        }

        /// <summary>
        /// Converts (coerces) object value to double. Throws an exception if the object value 
        /// cannot be (reasonably) converted to double type.
        /// </summary>
        /// <param name="value">an object value to convert</param>
        /// <remarks>Casts to double if the underlying value type is float.
        /// Tries to parse value using InvariantCulture if the underlying value type is string.
        /// Tries to convert to string using UTF8 encoding and parse value using InvariantCulture 
        /// if the underlying value type is byte array.</remarks>
        /// <exception cref="FormatException">The object value is not in an appropriate format.</exception>
        public static double? GetDoubleNullable(this object value)
        {

            if (value.IsNull()) return new double?();

            if (value is double)
                return (double)value;
            if (value is float)
                return (float)value;

            if (TryReadString(value, out string stringValue) && !stringValue.IsNullOrWhiteSpace())
            {
                if (double.TryParse(stringValue, NumberStyles.Any, CultureInfo.InvariantCulture, out double doubleValue))
                    return doubleValue;
            }

            throw new FormatException(string.Format(Properties.Resources.TypeConverters_InvalidFormatException,
                value.GetType().Name, "double", value.ToString()));

        }
        
        /// <summary>
        /// Converts (coerces) object value to decimal. Throws an exception if the object value 
        /// cannot be (reasonably) converted to decimal type.
        /// </summary>
        /// <param name="value">an object value to convert</param>
        /// <remarks>Tries to parse value using InvariantCulture if the underlying value type is string.
        /// Tries to convert to string using UTF8 encoding and parse value using InvariantCulture 
        /// if the underlying value type is byte array.</remarks>
        /// <exception cref="FormatException">The object value is not in an appropriate format.</exception>
        public static decimal? GetDecimalNullable(this object value)
        {

            if (value.IsNull()) return new decimal?();

            if (value is decimal)
                return (decimal)value;

            if (TryReadString(value, out string stringValue) && !stringValue.IsNullOrWhiteSpace())
            {
                if (decimal.TryParse(stringValue, NumberStyles.Any, CultureInfo.InvariantCulture, out decimal decimalValue))
                    return decimalValue;
            }

            throw new FormatException(string.Format(Properties.Resources.TypeConverters_InvalidFormatException,
                value.GetType().Name, "decimal", value.ToString()));

        }

        /// <summary>
        /// Converts (coerces) object value to datetime. Throws an exception if the object value 
        /// cannot be (reasonably) converted to datetime type.
        /// Default datetime formats: "yyyy-MM-dd HH:mm:ss", "yyyy-MM-dd", "HH:mm:ss"
        /// </summary>
        /// <param name="value">an object value to convert</param>
        /// <remarks>Tries to parse the value using default formats ("yyyy-MM-dd HH:mm:ss", 
        /// "yyyy-MM-dd", "HH:mm:ss") if the underlying value type is string. 
        /// Tries to convert to string using UTF8 encoding and parse the string if the underlying value type is byte array.
        /// Tries to cast to DateTime in other cases.</remarks>
        /// <exception cref="FormatException">The object value is not in an appropriate format.</exception>
        public static DateTime? GetDateTimeNullable(this object value)
        {

            if (value.IsNull()) return new DateTime?();

            if (value is DateTime)
                return (DateTime)value;
            if (value is Int64)
                return new DateTime((Int64)value);

            if (TryReadString(value, out string stringValue) && !stringValue.IsNullOrWhiteSpace())
            {
                if (TryParseDateTime(stringValue, out DateTime dateTimeValue))
                    return dateTimeValue;
            }

            throw new FormatException(string.Format(Properties.Resources.TypeConverters_InvalidFormatException,
                value.GetType().Name, "DateTime", value.ToString()));

        }

        /// <summary>
        /// Converts (coerces) object value to datetime using formats provided. 
        /// Throws an exception if the object value cannot be converted to datetime type.
        /// </summary>
        /// <param name="value">an object value to convert</param>
        /// <param name="formats">datetime formats to use if parsing a string value</param>
        /// <remarks>Tries to parse the value using formats specified if the underlying value type is string. 
        /// Tries to convert to string using UTF8 encoding and parse the string if the underlying value type is byte array.
        /// Tries to cast to DateTime in other cases.</remarks>
        /// <exception cref="FormatException">The object value is not in an appropriate format.</exception>
        public static DateTime? GetDateTimeNullable(this object value, string[] formats)
        {

            if (value.IsNull()) return new DateTime?();

            if (value is DateTime)
                return (DateTime)value;
            if (value is Int64)
                return new DateTime((Int64)value);

            if (TryReadString(value, out string stringValue) && !stringValue.IsNullOrWhiteSpace())
            {
                if (TryParseDateTime(stringValue, formats, out DateTime dateTimeValue))
                    return dateTimeValue;
            }

            throw new FormatException(string.Format(Properties.Resources.TypeConverters_InvalidFormatException,
                value.GetType().Name, "DateTime", value.ToString()));

        }

        /// <summary>
        /// Converts (coerces) object value to DateTimeOffset. Throws an exception if the object value 
        /// cannot be (reasonably) converted to DateTimeOffset type.
        /// </summary>
        /// <param name="value">an object value to convert</param>
        /// <remarks>Uses <see cref="GetDateTime">GetDateTime</see> method to get a DateTime
        /// value and initializes a DateTimeOffset value with it.</remarks>
        /// <exception cref="FormatException">The object value is not in an appropriate format.</exception>
        public static DateTimeOffset? GetDateTimeOffsetNullable(this object value)
        {
            return new DateTimeOffset(DateTime.SpecifyKind(GetDateTime(value), DateTimeKind.Utc));
        }

        /// <summary>
        /// Converts (coerces) object value to type T enum. Throws an exception if the object value 
        /// cannot be (reasonably) converted to type T enum. 
        /// </summary>
        /// <typeparam name="T">The type of the enumeration.</typeparam>
        /// <param name="value">an object value to convert</param>
        /// <remarks>Does not support flags.
        /// Gets an enum value by converting int value (if it is int value) to the appropriate enum value
        /// or by parsing string value (if it is string value) to the appropriate enum value.</remarks>
        /// <exception cref="ArgumentException">Type T is not an enumeration.</exception>
        /// <exception cref="FormatException">The object value is not in an appropriate format.</exception>
        public static T? GetEnumNullable<T>(this object value) where T : struct
        {
            if (!typeof(T).IsEnum)
                throw new ArgumentException(string.Format(Properties.Resources.TypeConverters_ValueIsNotEnumeration,
                    typeof(T).FullName));

            if (value.IsNull()) return null;

            int intValue = 0;
            bool success = false;

            try
            {
                intValue = value.GetInt32();
                success = true;
            }
            catch (Exception) { }

            if (success)
            {
                if (!Enum.IsDefined(typeof(T), intValue))
                    throw new FormatException(string.Format(Properties.Resources.TypeConverters_EnumNumericValueInvalid,
                        intValue.ToString(), typeof(T).Name));
                return (T)(object)intValue;
            }

            if (TryReadString(value, out string stringValue) && !stringValue.IsNullOrWhiteSpace())
            {
                try
                {
                    return (T)Enum.Parse(typeof(T), stringValue.Trim(), true);
                }
                catch (Exception)
                {
                    throw new FormatException(string.Format(Properties.Resources.TypeConverters_EnumStringValueInvalid,
                        stringValue, typeof(T).Name));
                }
            }

            throw new FormatException(string.Format(Properties.Resources.TypeConverters_InvalidFormatException,
                value.GetType().Name, typeof(T).Name, value.ToString()));

        }


        /// <summary>
        /// Converts (coerces) object value to boolean. Throws an exception if the object value 
        /// cannot be (reasonably) converted to boolean type. 
        /// </summary>
        /// <param name="value">an object value to convert</param>
        /// <remarks>If the underlying value type is numeric, returns "> 0".
        /// If the underlying value type is string, tries to parse value as "true" or "false" (case insensitive)
        /// or to parse Int64 value using InvariantCulture and do "> 0" conversion. 
        /// if the underlying value type is byte[], tries to convert to string using UTF8 encoding 
        /// and parse as string.</remarks>
        /// <exception cref="ArgumentNullException">Failed to convert null value.</exception>
        /// <exception cref="FormatException">The object value is not in an appropriate format.</exception>
        public static bool GetBoolean(this object value)
        {
            var result = GetBooleanNullable(value);
            if (result.HasValue) return result.Value;
            throw new ArgumentNullException();
        }

        /// <summary>
        /// Tries to parse source string as a boolean and returns the result if succeeds,
        /// otherwise returns the defaultValue.
        /// </summary>
        /// <param name="source">the string value to parse</param>
        /// <param name="defaultValue">a default boolean value to return if the parse fails</param>
        internal static bool GetBooleanOrDefault(this string source, bool defaultValue)
        {
            if (source.IsNullOrWhiteSpace()) return defaultValue;

            if (source.Trim().Equals("TRUE", StringComparison.OrdinalIgnoreCase)) return true;

            if (source.Trim().Equals("FALSE", StringComparison.OrdinalIgnoreCase)) return false;

            if (int.TryParse(source, NumberStyles.Any, CultureInfo.InvariantCulture, out int intValue))
                return (intValue > 0);

            return defaultValue;

        }

        /// <summary>
        /// Converts (coerces) object value to byte. Throws an exception if the object value 
        /// cannot be (reasonably) converted to byte type.
        /// </summary>
        /// <param name="value">an object value to convert</param>
        /// <remarks>Does checked conversions if the underlying value type is sbyte. 
        /// (narrowing conversion from int types does not make sense for byte)
        /// Tries to parse value using InvariantCulture if the underlying value type is string.
        /// Tries to convert to string using UTF8 encoding and parse value using InvariantCulture 
        /// if the underlying value type is byte array.</remarks>
        /// <exception cref="ArgumentNullException">Failed to convert null value.</exception>
        /// <exception cref="FormatException">The object value is not in an appropriate format.</exception>
        public static byte GetByte(this object value)
        {
            var result = GetByteNullable(value);
            if (result.HasValue) return result.Value;
            throw new ArgumentNullException();
        }

        /// <summary>
        /// Converts (coerces) object value to sbyte. Throws an exception if the object value 
        /// cannot be (reasonably) converted to sbyte type.
        /// </summary>
        /// <param name="value">an object value to convert</param>
        /// <remarks>Only returns a sbyte value if the underlying value type is sbyte.</remarks>
        /// <exception cref="ArgumentNullException">Failed to convert null value.</exception>
        /// <exception cref="FormatException">The object value is not in an appropriate format.</exception>
        public static sbyte GetSByte(this object value)
        {
            var result = GetSByteNullable(value);
            if (result.HasValue) return result.Value;
            throw new ArgumentNullException();
        }

        /// <summary>
        /// Converts (coerces) object value to char. Throws an exception if the object value 
        /// cannot be (reasonably) converted to char type.
        /// </summary>
        /// <param name="value">an object value to convert</param>
        /// <remarks>Returns the first char (after Trim) if the underlying value type is string.
        /// Tries to convert to string using UTF8 encoding and to return the first char (after Trim)
        /// if the underlying value type is byte array.</remarks>
        /// <exception cref="ArgumentNullException">Failed to convert null value.</exception>
        /// <exception cref="FormatException">The object value is not in an appropriate format.</exception>
        public static char GetChar(this object value)
        {
            var result = GetCharNullable(value);
            if (result.HasValue) return result.Value;
            throw new ArgumentNullException();
        }

        /// <summary>
        /// Converts (coerces) object value to Guid. Throws an exception if the object value 
        /// cannot be (reasonably) converted to Guid type.
        /// </summary>
        /// <param name="value">an object value to convert</param>
        /// <remarks>Returns Guid.Empty if the underlying value is null or empty string or empty byte array.
        /// Tries to parse value if the underlying value type is string or 16 byte array.
        /// Tries to convert to string using UTF8 encoding and parse the string 
        /// if the underlying value type is byte array and it's size is not 16.</remarks>
        /// <exception cref="ArgumentNullException">Failed to convert null value.</exception>
        /// <exception cref="FormatException">The object value is not in an appropriate format.</exception>
        public static Guid GetGuid(this object value)
        {
            var result = GetGuidNullable(value);
            if (result.HasValue) return result.Value;
            throw new ArgumentNullException();
        }

        /// <summary>
        /// Converts (coerces) object value to short. Throws an exception if the object value 
        /// cannot be (reasonably) converted to short type.
        /// </summary>
        /// <param name="value">an object value to convert</param>
        /// <remarks>Does checked conversions if the underlying value type is numeric or decimal.
        /// Tries to parse value using InvariantCulture if the underlying value type is string.
        /// Tries to convert to string using UTF8 encoding and parse value using InvariantCulture 
        /// if the underlying value type is byte array.</remarks>
        /// <exception cref="ArgumentNullException">Failed to convert null value.</exception>
        /// <exception cref="FormatException">The object value is not in an appropriate format.</exception>
        public static short GetInt16(this object value)
        {
            var result = GetInt16Nullable(value);
            if (result.HasValue) return result.Value;
            throw new ArgumentNullException();
        }

        /// <summary>
        /// Converts (coerces) object value to int. Throws an exception if the object value 
        /// cannot be (reasonably) converted to int type.
        /// </summary>
        /// <param name="value">an object value to convert</param>
        /// <remarks>Does checked conversions if the underlying value type is numeric or decimal.
        /// Tries to parse value using InvariantCulture if the underlying value type is string.
        /// Tries to convert to string using UTF8 encoding and parse value using InvariantCulture 
        /// if the underlying value type is byte array.</remarks>
        /// <exception cref="ArgumentNullException">Failed to convert null value.</exception>
        /// <exception cref="FormatException">The object value is not in an appropriate format.</exception>
        public static int GetInt32(this object value)
        {
            var result = GetInt32Nullable(value);
            if (result.HasValue) return result.Value;
            throw new ArgumentNullException();
        }

        /// <summary>
        /// Tries to parse source string as an integer and returns the result if succeeds,
        /// otherwise returns the defaultValue.
        /// </summary>
        /// <param name="source">the string value to parse</param>
        /// <param name="defaultValue">a default integer value to return if the parse fails</param>
        internal static int GetInt32OrDefault(this string source, int defaultValue)
        {
            if (long.TryParse(source, NumberStyles.Any, CultureInfo.InvariantCulture, out long intValue))
                return checked((int)intValue);
            return defaultValue;
        }

        /// <summary>
        /// Converts (coerces) object value to long. Throws an exception if the object value 
        /// cannot be (reasonably) converted to long type.
        /// </summary>
        /// <param name="value">an object value to convert</param>
        /// <remarks>Does checked conversions if the underlying value type is numeric or decimal.
        /// Tries to parse value using InvariantCulture if the underlying value type is string.
        /// Tries to convert to string using UTF8 encoding and parse value using InvariantCulture 
        /// if the underlying value type is byte array.</remarks>
        /// <exception cref="ArgumentNullException">Failed to convert null value.</exception>
        /// <exception cref="FormatException">The object value is not in an appropriate format.</exception>
        public static long GetInt64(this object value)
        {
            var result = GetInt64Nullable(value);
            if (result.HasValue) return result.Value;
            throw new ArgumentNullException();
        }

        /// <summary>
        /// Converts (coerces) object value to float. Throws an exception if the object value 
        /// cannot be (reasonably) converted to float type.
        /// </summary>
        /// <param name="value">an object value to convert</param>
        /// <remarks>Tries to parse value using InvariantCulture if the underlying value type is string.
        /// Tries to convert to string using UTF8 encoding and parse value using InvariantCulture 
        /// if the underlying value type is byte array.</remarks>
        /// <exception cref="ArgumentNullException">Failed to convert null value.</exception>
        /// <exception cref="FormatException">The object value is not in an appropriate format.</exception>
        public static float GetFloat(this object value)
        {
            var result = GetFloatNullable(value);
            if (result.HasValue) return result.Value;
            throw new ArgumentNullException();
        }

        /// <summary>
        /// Converts (coerces) object value to double. Throws an exception if the object value 
        /// cannot be (reasonably) converted to double type.
        /// </summary>
        /// <param name="value">an object value to convert</param>
        /// <remarks>Casts to double if the underlying value type is float.
        /// Tries to parse value using InvariantCulture if the underlying value type is string.
        /// Tries to convert to string using UTF8 encoding and parse value using InvariantCulture 
        /// if the underlying value type is byte array.</remarks>
        /// <exception cref="ArgumentNullException">Failed to convert null value.</exception>
        /// <exception cref="FormatException">The object value is not in an appropriate format.</exception>
        public static double GetDouble(this object value)
        {
            var result = GetDoubleNullable(value);
            if (result.HasValue) return result.Value;
            throw new ArgumentNullException();
        }

        /// <summary>
        /// Converts (coerces) object value to string. Throws an exception if the object value 
        /// cannot be (reasonably) converted to string type.
        /// </summary>
        /// <param name="value">an object value to convert</param>
        /// <remarks>Returns null if the underlying value is null.
        /// Tries to convert to string using UTF8 encoding and parse value using InvariantCulture 
        /// if the underlying value type is byte array.
        /// Tries to cast to string in other cases.</remarks>
        public static string GetString(this object value)
        {

            if (value.IsNull())
                return null;

            if (value is string)
                return (string)value;

            if (value is byte[])
            {
                if (TryReadString((byte[])value, out string stringValue))
                    return stringValue;
            }

            return value.ToString();

        }

        /// <summary>
        /// Converts (coerces) object value to decimal. Throws an exception if the object value 
        /// cannot be (reasonably) converted to decimal type.
        /// </summary>
        /// <param name="value">an object value to convert</param>
        /// <remarks>Tries to parse value using InvariantCulture if the underlying value type is string.
        /// Tries to convert to string using UTF8 encoding and parse value using InvariantCulture 
        /// if the underlying value type is byte array.</remarks>
        /// <exception cref="ArgumentNullException">Failed to convert null value.</exception>
        /// <exception cref="FormatException">The object value is not in an appropriate format.</exception>
        public static decimal GetDecimal(this object value)
        {
            var result = GetDecimalNullable(value);
            if (result.HasValue) return result.Value;
            throw new ArgumentNullException();
        }

        /// <summary>
        /// Converts (coerces) object value to datetime. Throws an exception if the object value 
        /// cannot be (reasonably) converted to datetime type.
        /// Default datetime formats: "yyyy-MM-dd HH:mm:ss", "yyyy-MM-dd", "HH:mm:ss"
        /// </summary>
        /// <param name="value">an object value to convert</param>
        /// <remarks>Tries to parse the value using default formats ("yyyy-MM-dd HH:mm:ss", 
        /// "yyyy-MM-dd", "HH:mm:ss") if the underlying value type is string. 
        /// Tries to convert to string using UTF8 encoding and parse the string if the underlying value type is byte array.
        /// Tries to cast to DateTime in other cases.</remarks>
        /// <exception cref="ArgumentNullException">Failed to convert null value.</exception>
        /// <exception cref="FormatException">The object value is not in an appropriate format.</exception>
        public static DateTime GetDateTime(this object value)
        {
            var result = GetDateTimeNullable(value);
            if (result.HasValue) return result.Value;
            throw new ArgumentNullException();
        }

        /// <summary>
        /// Converts (coerces) object value to datetime using formats provided. 
        /// Throws an exception if the object value cannot be converted to datetime type.
        /// </summary>
        /// <param name="value">an object value to convert</param>
        /// <param name="formats">datetime formats to use if parsing a string value</param>
        /// <remarks>Tries to parse the value using formats specified if the underlying value type is string. 
        /// Tries to convert to string using UTF8 encoding and parse the string if the underlying value type is byte array.
        /// Tries to cast to DateTime in other cases.</remarks>
        /// <exception cref="ArgumentNullException">Failed to convert null value.</exception>
        /// <exception cref="FormatException">The object value is not in an appropriate format.</exception>
        public static DateTime GetDateTime(this object value, string[] formats)
        {
            var result = GetDateTimeNullable(value, formats);
            if (result.HasValue) return result.Value;
            throw new ArgumentNullException();
        }

        /// <summary>
        /// Converts (coerces) object value to DateTimeOffset. Throws an exception if the object value 
        /// cannot be (reasonably) converted to DateTimeOffset type.
        /// </summary>
        /// <param name="value">an object value to convert</param>
        /// <remarks>Uses <see cref="GetDateTime">GetDateTime</see> method to get a DateTime
        /// value and initializes a DateTimeOffset value with it.</remarks>
        /// <exception cref="ArgumentNullException">Failed to convert null value.</exception>
        /// <exception cref="FormatException">The object value is not in an appropriate format.</exception>
        public static DateTimeOffset GetDateTimeOffset(this object value)
        {
            var result = GetDateTimeOffsetNullable(value);
            if (result.HasValue) return result.Value;
            throw new ArgumentNullException();
        }

        /// <summary>
        /// Converts object value to byte[]. Throws an exception if the object value 
        /// cannot be (reasonably) converted to byte[] type, i.e. is not byte[].
        /// </summary>
        /// <param name="value">an object value to convert</param>
        /// <remarks>Returns null if the underlying value is null.</remarks>
        /// <exception cref="FormatException">The value is not a byte array.</exception>
        public static byte[] GetByteArray(this object value)
        {
            if (value.IsNull()) return null;

            if (value is byte[]) return (byte[])((byte[])value).Clone();

            throw new FormatException(string.Format(Properties.Resources.TypeConverters_InvalidFormatException,
                value.GetType().Name, "byte[]", value.ToString()));
        }

        /// <summary>
        /// Converts (coerces) object value to type T enum. Throws an exception if the object value 
        /// cannot be (reasonably) converted to type T enum. 
        /// </summary>
        /// <typeparam name="T">The type of the enumeration.</typeparam>
        /// <param name="value">an object value to convert</param>
        /// <remarks>Does not support flags.
        /// Gets an enum value by converting int value (if it is int value) to the appropriate enum value
        /// or by parsing string value (if it is string value) to the appropriate enum value.</remarks>
        /// <exception cref="ArgumentException">Type T is not an enumeration.</exception>
        /// <exception cref="ArgumentNullException">Failed to convert null value.</exception>
        /// <exception cref="FormatException">The object value is not in an appropriate format.</exception>
        public static T GetEnum<T>(this object value)
        {

            if (value.IsNull()) throw new ArgumentNullException();

            if (!typeof(T).IsEnum)
                throw new ArgumentException(string.Format(Properties.Resources.TypeConverters_ValueIsNotEnumeration, 
                    typeof(T).FullName));

            int intValue = 0;
            bool success = false;

            try
            {
                intValue = value.GetInt32();
                success = true;
            }
            catch (Exception) { }

            if (success)
            {
                if (!Enum.IsDefined(typeof(T), intValue))
                    throw new FormatException(string.Format(Properties.Resources.TypeConverters_EnumNumericValueInvalid,
                        intValue.ToString(), typeof(T).Name));
                return (T)(object)intValue;
            }

            if (TryReadString(value, out string stringValue) && !stringValue.IsNullOrWhiteSpace())
            {
                try
                {
                    return (T)Enum.Parse(typeof(T), stringValue.Trim(), true);
                }
                catch (Exception)
                {
                    throw new FormatException(string.Format(Properties.Resources.TypeConverters_EnumStringValueInvalid,
                        stringValue, typeof(T).Name));
                }
            }

            throw new FormatException(string.Format(Properties.Resources.TypeConverters_InvalidFormatException,
                value.GetType().Name, typeof(T).Name, value.ToString()));

        }

        /// <summary>
        /// Converts (coerces) object value to type T enum. 
        /// Returns default value if object value is null.
        /// Throws a format exception if the object value cannot be (reasonably) converted to type T enum. 
        /// </summary>
        /// <typeparam name="T">The type of the enumeration.</typeparam>
        /// <param name="value">an object value to convert</param>
        /// <param name="defaultValue">a default value to return if object is null</param>
        /// <remarks>Does not support flags.
        /// Gets an enum value by converting int value (if it is int value) to the appropriate enum value
        /// or by parsing string value (if it is string value) to the appropriate enum value.</remarks>
        /// <exception cref="ArgumentException">Type T is not an enumeration.</exception>
        /// <exception cref="ArgumentNullException">Failed to convert null value.</exception>
        /// <exception cref="FormatException">The object value is not in an appropriate format.</exception>
        public static T GetEnumOrDefault<T>(this object value, T defaultValue)
        {
            if (value.IsNull()) return defaultValue;
            return value.GetEnum<T>();
        }

        /// <summary>
        /// Tries to parse source string as an enum value and returns the result if succeeds,
        /// otherwise returns the defaultValue.
        /// </summary>
        /// <param name="source">the string value to parse</param>
        /// <param name="defaultValue">a default enum value to return if the parse fails</param>
        internal static T GetEnumOrDefault<T>(this string source, T defaultValue)
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



        private static bool TryReadString(byte[] source, out string result)
        {
            if (null == source || source.Length < 1)
            {
                result = null;
                return false;
            }
            try
            {
                result = Encoding.UTF8.GetString(source);
                return true;
            }
            catch (Exception)
            {
                result = null;
                return false;
            }
        }

        private static bool TryReadString(object source, out string result)
        {
            if (source is string)
            {
                result = (string)source;
                return true;
            }
            if (source is byte[]) return TryReadString((byte[])source, out result);
            result = null;
            return false;
        }

        private static bool TryParseDateTime(string source, out DateTime result)
        {
            return TryParseDateTime(source, _defaultDateTimeFormats, out result);            
        }

        private static bool TryParseDateTime(string source, string[] formats, out DateTime result)
        {

            if (null == formats || formats.Length < 1)
            {
                result = DateTime.MinValue;
                return false;
            }

            foreach (var format in formats)
            {
                try
                {
                    result = DateTime.ParseExact(source, format, CultureInfo.InvariantCulture, DateTimeStyles.None);
                    return true;
                }
                // ReSharper disable EmptyGeneralCatchClause
                catch (Exception) { }
                // ReSharper restore EmptyGeneralCatchClause
            }

            result = DateTime.MinValue;
            return false;

        }

    }
}
