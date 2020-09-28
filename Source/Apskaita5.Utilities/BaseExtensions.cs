using System;

namespace Apskaita5.Common
{
    /// <summary>
    /// Provides common methods for business logic.
    /// </summary>
    public static class BaseExtensions
    {

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
        /// Returns a value indicating that the object (value) is null. Required due to potential operator overloadings
        /// that cause unpredictable behaviour of standard null == value test.
        /// </summary>
        /// <typeparam name="T">a type of the object to test</typeparam>
        /// <param name="value">an object to test against null</param>
        public static bool IsNull<T>(this T value) where T : class
        {
            return ReferenceEquals(value, null) || DBNull.Value == value;
        }

        /// <summary>
        /// Returns true if the string value is null or empty or consists from whitespaces only.
        /// </summary>
        /// <param name="value">a string value to evaluate</param>
        public static bool IsNullOrWhiteSpace(this string value)
        {
            return (null == value || string.IsNullOrEmpty(value.Trim()));
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
            if (null == source && null == stringToCompare) return true;
            if (null == source || null == stringToCompare) return false;
            if (ignoreCase)
            {
                return source.Trim().Equals(stringToCompare.Trim(), StringComparison.OrdinalIgnoreCase);
            }
            return source.Trim().Equals(stringToCompare.Trim(), StringComparison.Ordinal);
        }
                     
    }
}