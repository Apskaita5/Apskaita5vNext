using Apskaita5.Common.MathExtensions;
using System;
using System.Globalization;

namespace Apskaita5.Common.FormattingExtensions
{
    public static class FormattingExtensions
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
            if (source.IsNullOrWhiteSpace())
                return string.Empty.PadLeft(minLength, padChar);

            if (source.Trim().Length < minLength)
            {
                if (padAtBegining)
                    return source.Trim().PadLeft(minLength, padChar);
                return source.Trim().PadRight(minLength, padChar);
            }

            return source.Trim();
        }

    }
}
