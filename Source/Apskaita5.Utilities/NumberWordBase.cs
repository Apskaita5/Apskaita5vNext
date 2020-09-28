using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Apskaita5.Common.LanguageExtensions
{
    /// <summary>
    /// Represents a base class for numeric to natural language convertors.
    /// </summary>
    public abstract class NumberWordBase
    {

        /// <summary>
        /// Gets an ISO 639-1 language code for the language that the implementation uses.
        /// </summary>
        public abstract string Language { get; }


        /// <summary>
        /// Converts the value to the natural language.
        /// </summary>
        /// <param name="value">a value to convert</param>
        /// <param name="currency">a currency string to use (default EUR)</param>
        /// <param name="cents">a cents value to use (default ct.)</param>
        public abstract string ConvertToWords(double value, string currency, string cents);

        /// <summary>
        /// Converts the value to the natural language.
        /// </summary>
        /// <param name="value">a value to convert</param>
        /// <param name="currency">a currency string to use (default EUR)</param>
        /// <param name="cents">a cents value to use (default ct.)</param>
        public abstract string ConvertToWords(decimal value, string currency, string cents);

        /// <summary>
        /// Converts the value to the natural language.
        /// </summary>
        /// <param name="value">a value to convert</param>
        public abstract string ConvertToWords(int value);

    }
}