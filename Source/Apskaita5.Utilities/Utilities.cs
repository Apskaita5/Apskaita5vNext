using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text;
using System.Xml;
using System.Xml.Serialization;
using System.Reflection;

namespace Apskaita5.Common
{
    /// <summary>
    /// Provides common methods for business logic.
    /// </summary>
    public static class Utilities
    {

        private static readonly string[] _letters = new String[]
            {
                "A", "B", "C", "D", "E", "F", "G", "H", "I", "J", "K", "L", "M", "N", "O", "P",
                "Q", "R", "S", "T", "U", "V", "Z", "W"
            };


        /// <summary>
        /// Converts integer to a letter index, e.g. A., B. etc.
        /// </summary>
        /// <param name="value">the value to convert</param>
        public static string ToLetterIndex(this int value)
        {
            if (value < 1 || value > _letters.Length) return string.Empty;
            return _letters[value - 1];
        }

        /// <summary>
        /// Converts number to its' roman representation, e.g. 1 to I, 2 to II etc..
        /// </summary>
        /// <param name="value">value to convert.</param>
        public static string ToRomanNumber(this int value)
        {

            if (value < 0)
                value = -value;

            var sb = new System.Text.StringBuilder();

            while (value > 0)
            {
                if (value >= 1000)
                {
                    sb.Append("M");
                    value -= 1000;
                }
                else if (value >= 900)
                {
                    sb.Append("CM");
                    value -= 900;
                }
                else if (value >= 500)
                {
                    sb.Append("D");
                    value -= 500;
                }
                else if (value >= 400)
                {
                    sb.Append("CD");
                    value -= 400;
                }
                else if (value >= 100)
                {
                    sb.Append("C");
                    value -= 100;
                }
                else if (value >= 90)
                {
                    sb.Append("XC");
                    value -= 90;
                }
                else if (value >= 50)
                {
                    sb.Append("L");
                    value -= 50;
                }
                else if (value >= 40)
                {
                    sb.Append("XL");
                    value -= 40;
                }
                else if (value >= 10)
                {
                    sb.Append("X");
                    value -= 10;
                }
                else if (value >= 9)
                {
                    sb.Append("IX");
                    value -= 9;
                }
                else if (value >= 5)
                {
                    sb.Append("V");
                    value -= 5;
                }
                else if (value >= 4)
                {
                    sb.Append("IV");
                    value -= 4;
                }
                else if (value >= 1)
                {
                    sb.Append("I");
                    value -= 1;
                }
                else
                {
                    throw new ArgumentOutOfRangeException(nameof(value));
                    // <<-- shouldn't be possble to get here, but it ensures that we will never have an infinite loop (in case the computer is on crack that day).
                }
            }

            return sb.ToString();

        }

        /// <summary>
        /// Gets a string that represents the value formated by the ffdata standarts.
        /// </summary>
        /// <param name="value">number to format</param>
        public static string ToFFDataFormat(this double value)
        {
            return value.AccountingRound(2).ToString("#.00", CultureInfo.InvariantCulture);
        }

        /// <summary>
        /// Gets a string that represents the value formated by the ffdata standarts.
        /// </summary>
        /// <param name="value">number to format</param>
        public static string ToFFDataFormat(this decimal value)
        {
            return value.AccountingRound(2).ToString("#.00", CultureInfo.InvariantCulture);
        }

        /// <summary>
        /// Gets a string that represents the value formated by the ffdata standarts.
        /// </summary>
        /// <param name="value">a value to convert</param>
        public static string ToFFDataFormat(this DateTime value)
        {
            return value.ToString("yyyy-MM-dd");
        }


        /// <summary>
        /// Returns a double value rounded using accounting algorithm.
        /// </summary>
        /// <param name="value">the value to round</param>
        /// <param name="roundOrder">the rounding order</param>
        /// <exception cref="ArgumentOutOfRangeException">Round order should be between 0 and 20.</exception>
        public static double AccountingRound(this double value, int roundOrder)
        {
            if (roundOrder < 0 || roundOrder > 20)
                throw new ArgumentOutOfRangeException(nameof(roundOrder), roundOrder, Apskaita5.Common.Properties.Resources.RoundOrderOutOfRange);

            var intermediate = (long)Math.Floor(value * Math.Pow(10, roundOrder));
            if ((decimal)(intermediate + 0.5) > (decimal)(value * Math.Pow(10, roundOrder)))
            {
                return (intermediate / Math.Pow(10, roundOrder));
            }
            return ((intermediate + 1) / Math.Pow(10, roundOrder));
        }

        /// <summary>
        /// Returns a decimal value rounded using accounting algorithm.
        /// </summary>
        /// <param name="value">the value to round</param>
        /// <param name="roundOrder">the rounding order</param>
        /// <exception cref="ArgumentOutOfRangeException">Round order should be between 0 and 20.</exception>
        public static decimal AccountingRound(this decimal value, int roundOrder)
        {
            if (roundOrder < 0 || roundOrder > 20)
                throw new ArgumentOutOfRangeException(nameof(roundOrder), roundOrder, Apskaita5.Common.Properties.Resources.RoundOrderOutOfRange);

            var intermediate = (long)Math.Floor(value * (decimal)Math.Pow(10, roundOrder));
            if ((decimal)(intermediate + 0.5) > (decimal)(value * (decimal)Math.Pow(10, roundOrder)))
            {
                return (decimal)(intermediate / Math.Pow(10, roundOrder));
            }
            return (decimal)((intermediate + 1) / Math.Pow(10, roundOrder));
        }

        /// <summary>
        /// Compares double values for equality using double.Epsilon.
        /// </summary>
        /// <param name="value">the first value to compare</param>
        /// <param name="valueToCompare">the second value to compare</param>
        public static bool EqualsTo(this double value, double valueToCompare)
        {
            return !((Math.Abs(value - valueToCompare) > double.Epsilon));
        }

        /// <summary>
        /// Compares double values for equality using the epsilon provided.
        /// </summary>
        /// <param name="value">the first value to compare</param>
        /// <param name="valueToCompare">the second value to compare</param>
        /// <param name="epsilon">the max value diff to ignore</param>
        public static bool EqualsTo(this double value, double valueToCompare, double epsilon)
        {
            return !((Math.Abs(value - valueToCompare) > epsilon));
        }

        /// <summary>
        /// Compares double values for equality using the round order provided.
        /// </summary>
        /// <param name="value">the first value to compare</param>
        /// <param name="valueToCompare">the second value to compare</param>
        /// <param name="roundOrder">the round order applicable</param>
        public static bool EqualsTo(this double value, double valueToCompare, int roundOrder)
        {
            return !((Math.Abs(value - valueToCompare) > (1.0 / Math.Pow(10.0, roundOrder + 1))));
        }

        /// <summary>
        /// Compares double values for the first value to be greater than the second using double.Epsilon.
        /// </summary>
        /// <param name="value">the first value to compare</param>
        /// <param name="valueToCompare">the second value to compare</param>
        public static bool GreaterThan(this double value, double valueToCompare)
        {
            return ((value - valueToCompare) > double.Epsilon);
        }

        /// <summary>
        /// Compares double values for the first value to be greater than the second using the epsilon provided.
        /// </summary>
        /// <param name="value">the first value to compare</param>
        /// <param name="valueToCompare">the second value to compare</param>
        /// <param name="epsilon">the max value diff to ignore</param>
        public static bool GreaterThan(this double value, double valueToCompare, double epsilon)
        {
            return ((value - valueToCompare) > epsilon);
        }

        /// <summary>
        /// Compares double values for the first value to be greater than the second using the round order provided.
        /// </summary>
        /// <param name="value">the first value to compare</param>
        /// <param name="valueToCompare">the second value to compare</param>
        /// <param name="roundOrder">the round order applicable</param>
        public static bool GreaterThan(this double value, double valueToCompare, int roundOrder)
        {
            return ((value - valueToCompare) > (1.0 / Math.Pow(10.0, roundOrder + 1)));
        }


        /// <summary>
        /// Gets a DateTime containing DateTime.Now timestamp with a second precision.
        /// </summary>
        /// <remarks>Required by most SQL engines.</remarks>
        public static DateTime GetCurrentTimeStamp()
        {
            var result = DateTime.Now;
            result = new DateTime((long)(Math.Floor((double)(result.Ticks / TimeSpan.TicksPerSecond))
                * TimeSpan.TicksPerSecond));
            return result;
        }


        /// <summary>
        /// Gets a string field by it's index in the delimited string.
        /// Returns string.Empty if the line is empty or if there is no field at fieldIndex.
        /// </summary>
        /// <param name="line">a line (string) that contains delimited fields</param>
        /// <param name="fieldIndex">a zero based index of the field to get</param>
        /// <param name="fieldDelimiter">a string that delimits fields in line</param>
        public static string GetDelimitedField(this string line, int fieldIndex, string fieldDelimiter)
        {

            if (fieldDelimiter == null || string.IsNullOrEmpty(fieldDelimiter.Trim()))
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
        public static bool IsNullOrWhiteSpace(this string value)
        {
            return (value == null || string.IsNullOrEmpty(value.Trim()));
        }

        /// <summary>
        /// Returns string.Empty if the value is null, otherwise returns value.
        /// </summary>
        /// <param name="value">a string value to evaluate</param>
        /// <returns>string.Empty if the value is null, otherwise - value</returns>
        public static string NotNullValue(this string value)
        {
            if (value == null) return string.Empty;
            return value;
        }

        /// <summary>
        /// Adds a newLine to the source string, i.e. source + Environment.NewLine + newLine.
        /// </summary>
        /// <param name="source">the source string to add the newLine to</param>
        /// <param name="newLine">the string to add as a new line</param>
        /// <param name="allowEmptyLine">whether to allow to add an empty line</param>
        public static string AddNewLine(this string source, string newLine, bool allowEmptyLine)
        {
            if (newLine.IsNullOrWhiteSpace())
            {
                if (allowEmptyLine) return source + Environment.NewLine;
                return source;
            }
            return source + Environment.NewLine + newLine;
        }

        /// <summary>
        /// Returns the source string part that is not longer then the maxLength specified.
        /// </summary>
        /// <param name="source">the source string which part is returned</param>
        /// <param name="maxLength">the max allowed return value length</param>
        public static string MaxLengthValue(this string source, int maxLength)
        {
            if (source.IsNullOrWhiteSpace()) return string.Empty;
            if (source.Trim().Length > maxLength) return source.Trim().Substring(0, maxLength);
            return source.Trim();
        }

        /// <summary>
        /// Returns a string that is not shorter then the minLength specified.
        /// If the source string is shorter then the minLength specified, it is padded with the padChar.
        /// </summary>
        /// <param name="source">the source string</param>
        /// <param name="minLength">the min length of the string returned</param>
        /// <param name="padChar">the char to use for padding</param>
        /// <param name="padAtBegining">whether to pad at the begining of the string</param>
        public static string MinLengthValue(this string source, int minLength, char padChar, bool padAtBegining)
        {

            if (source.IsNullOrWhiteSpace()) return string.Empty.PadLeft(minLength, padChar);
            if (source.Trim().Length < minLength)
            {
                if (padAtBegining)
                    return source.Trim().PadLeft(minLength, padChar);
                return source.Trim().PadRight(minLength, padChar);
            }
            return source.Trim();

        }

        /// <summary>
        /// Compares strings in a safe manner, i.e. handles nulls, trims empty spaces, using <see cref="StringComparison.OrdinalIgnoreCase"/>.
        /// </summary>
        /// <param name="source">the first string to compare</param>
        /// <param name="stringToCompare">the second string to compare</param>
        /// <returns>True if the string values are the same.</returns>
        public static bool EqualsTo(this string source, string stringToCompare)
        {
            return EqualsTo(source, stringToCompare, true);
        }

        /// <summary>
        /// Compares strings in a safe manner, i.e. handles nulls, trims empty spaces, using either <see cref="StringComparison.OrdinalIgnoreCase"/>
        /// or <see cref="StringComparison.Ordinal"/>.
        /// </summary>
        /// <param name="source">the first string to compare</param>
        /// <param name="stringToCompare">the second string to compare</param>
        /// <param name="ignoreCase">whether to ignore case when comparing</param>
        /// <returns>True if the string values are the same.</returns>
        public static bool EqualsTo(this string source, string stringToCompare, bool ignoreCase)
        {
            if (source == null && stringToCompare == null) return true;
            if (source == null || stringToCompare == null) return false;
            if (ignoreCase)
            {
                return source.Trim().Equals(stringToCompare.Trim(), StringComparison.OrdinalIgnoreCase);
            }
            return source.Trim().Equals(stringToCompare.Trim(), StringComparison.Ordinal);
        }


        /// <summary>
        /// Serializes any object to XML string using <see cref="XmlSerializer">XmlSerializer</see>.
        /// </summary>
        /// <typeparam name="T">a type of the object to serialize</typeparam>
        /// <param name="objectToSerialize">an object to serialize</param>
        /// <param name="encoding">text encoding to use (if null, UTF8 without BOM is used)</param>
        /// <returns>XML string</returns>
        /// <remarks>an object should be XML serializable in order the method to work</remarks>
        /// <exception cref="ArgumentNullException">objectToSerialize is not specified</exception>
        public static string WriteToXml<T>(T objectToSerialize, Encoding encoding)
        {

            if (!objectToSerialize.GetType().IsValueType && objectToSerialize == null)
                throw new ArgumentNullException(nameof(objectToSerialize));

            if (encoding == null) encoding = new UTF8Encoding(false);

            var xmlSerializer = new XmlSerializer(typeof(T));

            var settings = new XmlWriterSettings
            {
                Indent = true,
                IndentChars = " ",
                Encoding = encoding
            };

            using (var ms = new MemoryStream())
            {
                using (var writer = XmlWriter.Create(ms, settings))
                {
                    xmlSerializer.Serialize(writer, objectToSerialize);
                    return encoding.GetString(ms.ToArray());
                }
            }

        }

        /// <summary>
        /// Deserializes any object from XML string using <see cref="XmlSerializer">XmlSerializer</see>.
        /// </summary>
        /// <typeparam name="T">a type of the object to deserialize</typeparam>
        /// <param name="xmlString">an XML string that contains the deserializable object data</param>
        /// <returns>an object of type T</returns>
        /// <remarks>an object should be XML serializable in order the method to work</remarks>
        /// <exception cref="ArgumentNullException">xmlString is not specified</exception>
        public static T CreateFromXml<T>(string xmlString)
        {

            if (xmlString.IsNullOrWhiteSpace()) throw new ArgumentNullException(nameof(xmlString));

            var xmlSerializer = new XmlSerializer(typeof(T));

            using (var textReader = new StringReader(xmlString))
            {
                return (T)xmlSerializer.Deserialize(textReader);
            }

        }


        ///// <summary>
        ///// Converts byte array to image.
        ///// </summary>
        ///// <param name="source">the byte array to convert</param>
        //public static Image ByteArrayToImage(byte[] source)
        //{

        //    if (source == null || source.Length < 10) return null;

        //    Image result = null;

        //    using (var ms = new MemoryStream(source))
        //    {
        //        try
        //        {
        //            result = System.Drawing.Image.FromStream(ms);
        //        }
        //        catch (Exception){}
        //    }

        //    return result;

        //}

        ///// <summary>
        ///// Converts <see cref="System.Drawing.Image" /> to byte array encoded by 
        ///// <see cref="System.Drawing.Imaging.ImageFormat.Jpeg" />.
        ///// </summary>
        ///// <param name="source">Image to serialize to byte array.</param>
        //public static byte[] ImageToByteArray(Image source)
        //{

        //    if (source == null) return null;

        //    byte[] result = null;

        //    using (var imageToSave = new Bitmap(source.Width, source.Height, source.PixelFormat))
        //    {
        //        using (var gr = Graphics.FromImage(imageToSave))
        //        {
        //            gr.DrawImage(source, new PointF(0, 0));
        //        }

        //        using (var ms = new MemoryStream())
        //        {
        //            try
        //            {
        //                imageToSave.Save(ms, System.Drawing.Imaging.ImageFormat.Jpeg);
        //                result = ms.ToArray();
        //            }
        //            catch (Exception){}
        //        }
        //    }

        //    GC.Collect();

        //    return result;

        //}

    }
}