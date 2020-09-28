using System;
using System.Collections.Generic;
using System.Linq;
using System.Resources;
using System.Globalization;

namespace Apskaita5.Common.LanguageExtensions
{
    /// <summary>
    /// Provides common methods to work with natural languages.
    /// </summary>
    public static class LanguageExtensions
    {

        /// <summary>
        /// An array of all the legal ISO 639-1 language codes.
        /// </summary>
        /// <remarks></remarks>
        private static readonly string[] ValidLanguages = new string[] {"aa", "ab", "ae", "af", "ak", "am",
        "an", "ar", "as", "av", "ay", "az", "ba", "be", "bg", "bh", "bi", "bm", "bn", "bo", "br",
        "bs", "ca", "ce", "ch", "co", "cr", "cs", "cu", "cv", "cy", "da", "de", "dv", "dz", "ee",
        "el", "en", "eo", "es", "et", "eu", "fa", "ff", "fi", "fj", "fo", "fr", "fy", "ga", "gd",
        "gl", "gn", "gu", "gv", "ha", "he", "hi", "ho", "hr", "ht", "hu", "hy", "hz", "ia", "id",
        "ie", "ig", "ii", "ik", "io", "is", "it", "iu", "ja", "jv", "ka", "kg", "ki", "kj", "kk",
        "kl", "km", "kn", "ko", "kr", "ks", "ku", "kv", "kw", "ky", "la", "lb", "lg", "li", "ln",
        "lo", "lt", "lu", "lv", "mg", "mh", "mi", "mk", "ml", "mn", "mr", "ms", "mt", "my", "na",
        "nb", "nd", "ne", "ng", "nl", "nn", "no", "nr", "nv", "ny", "oc", "oj", "om", "or", "os",
        "pa", "pi", "pl", "ps", "pt", "qu", "rm", "rn", "ro", "ru", "rw", "sa", "sc", "sd", "se",
        "sg", "si", "sh", "sk", "sl", "sm", "sn", "so", "sq", "sr", "ss", "st", "su", "sv", "sw",
        "ta", "te", "tg", "th", "ti", "tk", "tl", "tn", "to", "tr", "ts", "tt", "tw", "ty", "ug",
        "uk", "ur", "uz", "ve", "vi", "vo", "wa", "wo", "xh", "yi", "yo", "za", "zh-cn", "zh-tw", "zu"};

        private const string DefaultBaseLanguage = "lt";

        private const string LanguageCodeResourcePrefix = "LanguageCode_";

        private static readonly NumberWordBase DefaultConverter = new NumberWordLT();

        private static readonly Dictionary<string, NumberWordBase> _numConverters =
            new Dictionary<string, NumberWordBase>(StringComparer.OrdinalIgnoreCase)
            { { "LT", new NumberWordLT() }, { "EN", new NumberWordEN() } };


        /// <summary>
        /// Validates language code. Returns true if the <paramref name="languageCode">languageCode</paramref> 
        /// is an ISO 639-1 language code or is null or empty.
        /// </summary>
        /// <param name="languageCode">A language code to check.</param>
        /// <remarks></remarks>
        public static bool IsValidLanguageCode(this string languageCode, bool emptyIsValid = true)
        {
            if (languageCode.IsNullOrWhiteSpace()) return emptyIsValid;
            return !(Array.IndexOf(ValidLanguages, languageCode.Trim().ToLowerInvariant()) < 0);
        }
              
        /// <summary>
        /// Gets a human readable language name in the current culture language 
        ///  by ISO 639-1 language code.
        /// </summary>
        /// <param name="languageCode">ISO 639-1 language code.</param>
        /// <param name="throwOnUnknownLanguage">Whether to throw an exception 
        /// if the <paramref name="languageCode"/> is invalid.</param>
        /// <returns>A human readable language name in the current culture language.</returns>
        public static string GetLanguageName(this string languageCode, bool throwOnUnknownLanguage = true)
        {
            if (languageCode.IsNullOrWhiteSpace()) return string.Empty;

            if (!languageCode.IsValidLanguageCode())
            {
                if (throwOnUnknownLanguage)
                    throw new Exception(string.Format(Properties.Resources.LanguageIsoCodeInvalid, languageCode));
                
                return string.Empty;
            }

            string result = string.Empty;
            try
            {
                var rm = new ResourceManager(typeof(LanguageExtensions));
                result = rm.GetString(LanguageCodeResourcePrefix + languageCode.Trim().ToLowerInvariant().Replace("-", "_"));
            }
            catch (Exception) { }

            if (result.IsNullOrWhiteSpace())
            {
                if (throwOnUnknownLanguage) throw new Exception(string.Format(
                    Properties.Resources.LocalizedResourceMissing,
                        languageCode, CultureInfo.CurrentCulture.Name));

                result = string.Empty;
            }

            return result;
        }

        /// <summary>
        /// Gets an ISO 639-1 language code by human readable language name in the current culture language.
        /// </summary>
        /// <param name="languageName">A human readable language name in the current culture language.</param>
        /// <param name="throwOnUnknownLanguage">Whether to throw an exception 
        /// if the <paramref name="languageName"/> is invalid.</param>
        /// <returns>an ISO 639-1 language code.</returns>
        public static string GetLanguageCode(this string languageName, bool throwOnUnknownLanguage = true)
        {
            if (languageName.IsNullOrWhiteSpace()) return string.Empty;

            foreach (var value in ValidLanguages)
            {
                if (value.GetLanguageName(false).EqualsTo(languageName, true))
                    return value;
            }

            if (throwOnUnknownLanguage) throw new ArgumentException(string.Format(
                Properties.Resources.LanguageNameUnknown, languageName));

            return string.Empty;
        }

        /// <summary>
        /// Gets a list of human readable language names in the current culture language.
        /// </summary>
        /// <param name="addEmptyLine">Whether to insert an empty string in the begining of the list.</param>
        /// <returns>A list of human readable language names in the current culture language.</returns>
        /// <remarks></remarks>
        public static List<string> GetLanguageNameList(bool addEmptyLine = false)
        {
            var result = ValidLanguages.Select(value => value.GetLanguageName(false)).
                Where(name => !name.IsNullOrWhiteSpace()).ToList();

            result.Sort();

            if (addEmptyLine) result.Insert(0, "");

            return result;
        }

        /// <summary>
        /// Gets a list of ISO 639-1 language codes.
        /// </summary>
        /// <param name="addEmptyLine">whether to insert an empty string in the begining of the list</param>
        /// <returns>A list of ISO 639-1 language codes.</returns>
        /// <remarks></remarks>
        public static List<string> GetLanguageCodeList(bool addEmptyLine = false)
        {
            var result = ValidLanguages.ToList();

            result.Sort();

            if (addEmptyLine) result.Insert(0, "");

            return result;
        }

        /// <summary>
        /// Compares ISO 639-1 language codes and returns true if they are the same.
        /// </summary>
        /// <param name="languageCode">First language code to compare.</param>
        /// <param name="toLanguageCode">Second language code to compare.</param>
        /// <param name="baseLanguageCode">Base language code.</param>
        /// <remarks>Null, empty or invalid language code is considered as a base language.</remarks>
        public static bool LanguageEquals(this string languageCode, string toLanguageCode, 
            string baseLanguageCode = DefaultBaseLanguage)
        {
            var validatedBaseCode = baseLanguageCode?.Trim();
            if (baseLanguageCode.IsNullOrWhiteSpace() || !baseLanguageCode.IsValidLanguageCode())
                validatedBaseCode = DefaultBaseLanguage;

            var validatedCode1 = languageCode?.Trim();
            if (languageCode.IsNullOrWhiteSpace() || !languageCode.IsValidLanguageCode())
                validatedCode1 = validatedBaseCode;

            var validatedCode2 = toLanguageCode?.Trim();
            if (toLanguageCode.IsNullOrWhiteSpace() || !toLanguageCode.IsValidLanguageCode())
                validatedCode2 = validatedBaseCode;

            return validatedCode1.EqualsTo(validatedCode2, true);
        }
           
        /// <summary>
        /// Gets a number to words converter for the specified language.
        /// </summary>
        /// <param name="language">the language (ISO 639-1 code) to get the converter for</param>
        /// <param name="defaultConverter">the default converter to return if no specific converter found (if any)</param>
        /// <param name="customConverters">custom converters to search (if any)</param>
        public static NumberWordBase GetNumberToWordsConverter(this string language, 
            NumberWordBase defaultConverter = null, NumberWordBase[] customConverters = null)
        {
            if (defaultConverter.IsNull()) defaultConverter = DefaultConverter;

            if (language.IsNullOrWhiteSpace()) return defaultConverter;

            if (null != customConverters && customConverters.Length > 0)
            {
                foreach (var customConverter in customConverters)
                {
                    if (customConverter.Language.LanguageEquals(language))
                        return customConverter;
                }
            }

            if (_numConverters.ContainsKey(language.Trim().ToUpperInvariant()))
                return _numConverters[language.Trim().ToUpperInvariant()];

            return defaultConverter;
        }

    }
}