using System;
using System.Collections.Generic;
using System.Linq;
using System.Resources;
using System.Globalization;

namespace Apskaita5.Common
{
    /// <summary>
    /// Provides common methods to work with natural languages.
    /// </summary>
    public static class LanguageUtilities
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

        private static readonly string DefaultBaseLanguage = "lt";

        private const string LanguageCodeResourcePrefix = "LanguageCode_";

        private static readonly Dictionary<string, NumberWordBase> _numConverters =
            new Dictionary<string, NumberWordBase>(StringComparer.OrdinalIgnoreCase)
            { { "LT", new NumberWordLT() }, { "EN", new NumberWordEN() } };


        /// <summary>
        /// Validates language code. Returns true if the <paramref name="languageCode">languageCode</paramref> 
        /// is an ISO 639-1 language code or is null or empty.
        /// </summary>
        /// <param name="languageCode">A language code to check.</param>
        /// <remarks></remarks>
        public static bool IsLanguageCodeValid(string languageCode)
        {
            if (languageCode.IsNullOrWhiteSpace()) return true;
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
        public static string GetLanguageName(string languageCode, bool throwOnUnknownLanguage)
        {

            if (languageCode.IsNullOrWhiteSpace()) return string.Empty;

            if (!IsLanguageCodeValid(languageCode))
            {

                if (throwOnUnknownLanguage)
                    throw new Exception(string.Format(Apskaita5.Common.Properties.Resources.LanguageIsoCodeInvalid, languageCode));

                return string.Empty;

            }

            string result = string.Empty;
            try
            {
                var rm = new ResourceManager(typeof(LanguageUtilities));
                result = rm.GetString(LanguageCodeResourcePrefix + languageCode.Trim().ToLowerInvariant().Replace("-", "_"));
            }
            catch (Exception) { }

            if (result.IsNullOrWhiteSpace() && throwOnUnknownLanguage)
                throw new Exception(string.Format(Apskaita5.Common.Properties.Resources.LocalizedResourceMissing,
                    languageCode, CultureInfo.CurrentCulture.Name));

            if (result.IsNullOrWhiteSpace())
                result = string.Empty;

            return result;

        }

        /// <summary>
        /// Gets an ISO 639-1 language code by human readable language name in the current culture language.
        /// </summary>
        /// <param name="languageName">A human readable language name in the current culture language.</param>
        /// <param name="throwOnUnknownLanguage">Whether to throw an exception 
        /// if the <paramref name="languageName"/> is invalid.</param>
        /// <returns>an ISO 639-1 language code.</returns>
        public static string GetLanguageCode(string languageName, bool throwOnUnknownLanguage)
        {

            if (languageName.IsNullOrWhiteSpace()) return string.Empty;

            foreach (var value in ValidLanguages)
            {
                if (GetLanguageName(value, false).EqualsTo(languageName))
                    return value;
            }

            if (throwOnUnknownLanguage)
                throw new ArgumentException(string.Format(Apskaita5.Common.Properties.Resources.LanguageNameUnknown, languageName));

            return string.Empty;

        }

        /// <summary>
        /// Gets a list of human readable language names in the current culture language.
        /// </summary>
        /// <param name="addEmptyLine">Whether to insert an empty string in the begining of the list.</param>
        /// <returns>A list of human readable language names in the current culture language.</returns>
        /// <remarks></remarks>
        public static List<string> GetLanguageNameList(bool addEmptyLine)
        {

            var result = ValidLanguages.Select(value => GetLanguageName(value, false)).
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
        public static List<string> GetLanguageCodeList(bool addEmptyLine)
        {

            var result = ValidLanguages.ToList();

            result.Sort();

            if (addEmptyLine)
                result.Insert(0, "");

            return result;

        }

        /// <summary>
        /// Compares ISO 639-1 language codes and returnes true if they are the same.
        /// </summary>
        /// <param name="languageCode1">First language code to compare.</param>
        /// <param name="languageCode2">Second language code to compare.</param>
        /// <param name="baseLanguageCode">Base language code.</param>
        /// <remarks>Null, empty or invalid language code is considered as a base language.</remarks>
        public static bool LanguagesEquals(string languageCode1, string languageCode2, string baseLanguageCode)
        {

            var validatedBaseCode = baseLanguageCode;
            if (baseLanguageCode.IsNullOrWhiteSpace() || !IsLanguageCodeValid(baseLanguageCode))
                validatedBaseCode = DefaultBaseLanguage;

            var validatedCode1 = languageCode1;
            if (languageCode1.IsNullOrWhiteSpace() || !IsLanguageCodeValid(languageCode1))
                validatedCode1 = validatedBaseCode;

            var validatedCode2 = languageCode2;
            if (languageCode2.IsNullOrWhiteSpace() || !IsLanguageCodeValid(languageCode2))
                validatedCode2 = validatedBaseCode;

            return validatedCode1.EqualsTo(validatedCode2);

        }


        /// <summary>
        /// Gets a number to words converter for the specified language.
        /// </summary>
        /// <param name="language">the language (ISO 639-1 code) to get the converter for</param>
        /// <param name="defaultConverter">the default converter to return if no specific converter found (if any)</param>
        /// <param name="customConverters">custom converters to search (if any)</param>
        public static NumberWordBase GetNumberToWordsConverter(string language, NumberWordBase defaultConverter,
            NumberWordBase[] customConverters)
        {

            if (language.IsNullOrWhiteSpace()) return defaultConverter;

            if (customConverters != null && customConverters.Length > 0)
            {
                foreach (var customConverter in customConverters)
                {
                    if (customConverter.Language.EqualsTo(language))
                        return customConverter;
                }
            }

            if (_numConverters.ContainsKey(language.Trim().ToUpperInvariant()))
                return _numConverters[language.Trim().ToUpperInvariant()];

            return defaultConverter;

        }

    }
}