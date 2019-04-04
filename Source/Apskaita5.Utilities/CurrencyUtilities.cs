using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Apskaita5.Common
{
    /// <summary>
    /// Provides common methods for currencies.
    /// </summary>
    public static class CurrencyUtilities
    {

        /// <summary>
        /// An array of all the legal ISO 4217 currency codes.
        /// </summary>
        /// <remarks></remarks>
        public static string[] CurrencyCodes = {"LTL", "USD", "EUR", "RUB", "AFN", "ALL",
        "DZD", "AED", "AMD", "ANG", "AOA", "ARS", "AUD", "AWG", "AZN", "BAM", "BBD", "BDT",
        "BGN", "BHD", "BIF", "BMD", "BND", "BOB", "BRL", "BSD", "BTN", "BWP", "BYR", "BZD",
        "CAD", "CDF", "CHF", "CLP", "CNY", "COP", "CRC", "CUP", "CVE", "CYP", "CZK", "DJF",
        "DKK", "DOP", "DZD", "EEK", "EGP", "ERN", "ETB", "FJD", "FKP", "GBP", "GEL", "GGP",
        "GHS", "GIP", "GMD", "GNF", "GTQ", "GYD", "HKD", "HNL", "HRK", "HTG", "HUF", "IDR",
        "ILS", "IMP", "INR", "IQD", "IRR", "ISK", "JEP", "JMD", "JOD", "JPY", "KES", "KGS",
        "KHR", "KMF", "KPW", "KRW", "KWD", "KYD", "KZT", "LAK", "LBP", "LKR", "LRD", "LSL",
        "LVL", "LYD", "MAD", "MDL", "MGA", "MKD", "MMK", "MNT", "MOP", "MRO", "MTL", "MUR",
        "MVR", "MWK", "MXN", "MYR", "MZN", "NAD", "NGN", "NIO", "NOK", "NPR", "NZD", "OMR",
        "PAB", "PEN", "PGK", "PHP", "PKR", "PLN", "PYG", "QAR", "RON", "RSD", "RWF", "SAR",
        "SBD", "SCR", "SDG", "SEK", "SGD", "SHP", "SLL", "SOS", "SPL", "SRD", "STD", "SVC",
        "SYP", "SZL", "THB", "TJS", "TMM", "TND", "TOP", "TRY", "TTD", "TVD", "TWD", "TZS",
        "UAH", "UGX", "UYU", "UZS", "VEF", "VND", "VUV", "WST", "XAF", "XAG", "XAU", "XCD",
        "XDR", "XOF", "XPD", "XPF", "XPT", "YER", "ZAR", "ZMK", "ZWD"};


        /// <summary>
        /// Checks if currencies <paramref name="currency1" /> and <paramref name="currency2" /> 
        /// identified by ISO 4217 codes represents the same currency assuming empty code string is 
        /// <paramref name="baseCurrency" />.
        /// </summary>
        /// <param name="currency1">First currency ISO 4217 code to check for equality.</param>
        /// <param name="currency2">Second currency ISO 4217 code to check for equality.</param>
        /// <param name="baseCurrency">Base currency identified by ISO 4217 code.</param>
        /// <returns><code>True</code> if <paramref name="currency1" /> and <paramref name="currency2" /> 
        /// represent the same currency.</returns>
        /// <remarks></remarks>
        public static bool CurrenciesEquals(string currency1, string currency2, string baseCurrency)
        {

            if (baseCurrency.IsNullOrWhiteSpace())
                throw new ArgumentNullException(nameof(baseCurrency));


            string validatedCurrency1 = currency1;
            if (validatedCurrency1.IsNullOrWhiteSpace())
                validatedCurrency1 = baseCurrency;


            string validatedCurrency2 = currency2;
            if (validatedCurrency2.IsNullOrWhiteSpace())
                validatedCurrency2 = baseCurrency;


            return validatedCurrency1.EqualsTo(validatedCurrency2);

        }

        /// <summary>
        /// Checks if currencies <paramref name="currencyCode" /> and <paramref name="baseCurrency" /> 
        /// identified by ISO 4217 codes represents the same currency assuming empty code string is 
        /// <paramref name="baseCurrency" />.
        /// </summary>
        /// <param name="currencyCode">Currency ISO 4217 code to check for equality with the <paramref name="baseCurrency" />.</param>
        /// <param name="baseCurrency">Base currency identified by ISO 4217 code.</param>
        /// <returns><code>True</code> if <paramref name="currencyCode" /> and <paramref name="baseCurrency" /> 
        /// represent the same currency.</returns>
        /// <remarks></remarks>
        public static bool IsBaseCurrency(string currencyCode, string baseCurrency)
        {
            return CurrenciesEquals(currencyCode, baseCurrency, baseCurrency);
        }

        /// <summary>
        /// Ensures that the currency code is not null or empty defaulting to <paramref name="baseCurrency" />.
        /// If <paramref name="currencyCode" /> is null or empty returns <paramref name="baseCurrency" />.
        /// Otherwise returns trimed uppercased <paramref name="currencyCode" />
        /// </summary>
        /// <param name="currencyCode">Currency ISO 4217 code to check for null or empty state.</param>
        /// <param name="baseCurrency">Base currency identified by ISO 4217 code.</param>
        /// <returns></returns>
        /// <remarks></remarks>
        public static string GetCurrencySafe(string currencyCode, string baseCurrency)
        {

            if (baseCurrency.IsNullOrWhiteSpace())
                throw new ArgumentNullException(nameof(baseCurrency));


            if (currencyCode.IsNullOrWhiteSpace())
            {
                return baseCurrency;
            }
            else
            {
                return currencyCode.Trim().ToUpperInvariant();
            }

        }

        /// <summary>
        /// Converts <paramref name="amountToConvert" /> accounted in currency <paramref name="originalCurrency" />
        /// with conversion rate <paramref name="originalCurrencyRate" /> with respect to <paramref name="baseCurrency" />
        /// to amount accounted in currency <paramref name="targetCurrency" /> with conversion rate 
        /// <paramref name="targetCurrencyRate" /> with respect to <paramref name="baseCurrency" />.
        /// </summary>
        /// <param name="amountToConvert">Amount to convert accounted in <paramref name="originalCurrency" />.</param>
        /// <param name="originalCurrency">ISO 4217 code for the currency that the <paramref name="amountToConvert" /> is accounted in.</param>
        /// <param name="originalCurrencyRate">Currency rate with respect to the <paramref name="baseCurrency" />
        /// for the currency that the <paramref name="amountToConvert" /> is accounted in. (should be zero or positive)</param>
        /// <param name="targetCurrency">ISO 4217 code for the currency that the returned value is accounted in.</param>
        /// <param name="targetCurrencyRate">Currency rate with respect to the <paramref name="baseCurrency" />
        /// for the currency that the returned value is accounted in.(should be zero or positive)</param>
        /// <param name="baseCurrency">Base currency identified by ISO 4217 code.</param>
        /// <param name="amountSignificantDigits">Round order applied to the result.</param>
        /// <param name="currencyRateSignificantDigits">Round order applied to <paramref name="originalCurrencyRate" /> 
        /// and <paramref name="targetCurrencyRate" />.</param>
        /// <param name="defaultAmount">Return value in case a conversion is not possible, 
        /// e.g. <paramref name="targetCurrencyRate" /> is zero.</param>
        /// <returns><paramref name="amountToConvert" /> converted to <paramref name="targetCurrency" />.</returns>
        /// <returns>amountToConvert value in targetCurrency units</returns>
        /// <exception cref="ArgumentException">Parameters originalCurrencyRate, targetCurrencyRate,
        /// amountSignificantDigits and currencyRateSignificantDigits cannot be negative.</exception>
        public static double ConvertCurrency(double amountToConvert, string originalCurrency,
            double originalCurrencyRate, string targetCurrency, double targetCurrencyRate,
            string baseCurrency, int amountSignificantDigits, int currencyRateSignificantDigits,
            double defaultAmount)
        {

            if ((0.0).GreaterThan(originalCurrencyRate))
                throw new ArgumentException(string.Format(Properties.Resources.ParameterCannotBeNegative, 
                    nameof(originalCurrencyRate)), nameof(originalCurrencyRate));
            if ((0.0).GreaterThan(targetCurrencyRate))
                throw new ArgumentException(string.Format(Properties.Resources.ParameterCannotBeNegative,
                    nameof(targetCurrencyRate)), nameof(targetCurrencyRate));
            if (amountSignificantDigits < 0)
                throw new ArgumentException(string.Format(Properties.Resources.ParameterCannotBeNegative,
                    nameof(amountSignificantDigits)), nameof(amountSignificantDigits));
            if (currencyRateSignificantDigits < 0)
                throw new ArgumentException(string.Format(Properties.Resources.ParameterCannotBeNegative,
                    nameof(currencyRateSignificantDigits)), nameof(currencyRateSignificantDigits));

            // zero remains zero in any currency
            if (amountToConvert.AccountingRound(amountSignificantDigits).EqualsTo(0.0, amountSignificantDigits))
                return 0;


            // if the currencies are the same, amount is the same
            if (CurrenciesEquals(originalCurrency, targetCurrency, baseCurrency))
                return amountToConvert.AccountingRound(amountSignificantDigits);


            // base currency rate with respect to itself is always 1
            double validatedOriginalCurrencyRate = originalCurrencyRate;

            if (IsBaseCurrency(originalCurrency, baseCurrency))
                validatedOriginalCurrencyRate = 1;

            double validatedTargetCurrencyRate = targetCurrencyRate;

            if (IsBaseCurrency(targetCurrency, baseCurrency))
                validatedTargetCurrencyRate = 1;


            // apply currencyRateSignificantDigits
            validatedOriginalCurrencyRate = validatedOriginalCurrencyRate.AccountingRound(currencyRateSignificantDigits);
            validatedTargetCurrencyRate = validatedTargetCurrencyRate.AccountingRound(currencyRateSignificantDigits);

            // do calculus if possible
            if (validatedTargetCurrencyRate.GreaterThan(0.0, currencyRateSignificantDigits))
            {
                return ((amountToConvert * validatedOriginalCurrencyRate).AccountingRound(amountSignificantDigits)
                    / validatedTargetCurrencyRate).AccountingRound(amountSignificantDigits);
            }

            return defaultAmount.AccountingRound(amountSignificantDigits);

        }

        /// <summary>
        /// Converts <paramref name="amountToConvert" /> accounted in currency <paramref name="originalCurrency" />
        /// with conversion rate <paramref name="originalCurrencyRate" /> with respect to <paramref name="baseCurrency" />
        /// to amount accounted in currency <paramref name="targetCurrency" /> with conversion rate 
        /// <paramref name="targetCurrencyRate" /> with respect to <paramref name="baseCurrency" />.
        /// </summary>
        /// <param name="amountToConvert">Amount to convert accounted in <paramref name="originalCurrency" />.</param>
        /// <param name="originalCurrency">ISO 4217 code for the currency that the <paramref name="amountToConvert" /> is accounted in.</param>
        /// <param name="originalCurrencyRate">Currency rate with respect to the <paramref name="baseCurrency" />
        /// for the currency that the <paramref name="amountToConvert" /> is accounted in. (should be zero or positive)</param>
        /// <param name="targetCurrency">ISO 4217 code for the currency that the returned value is accounted in.</param>
        /// <param name="targetCurrencyRate">Currency rate with respect to the <paramref name="baseCurrency" />
        /// for the currency that the returned value is accounted in.(should be zero or positive)</param>
        /// <param name="baseCurrency">Base currency identified by ISO 4217 code.</param>
        /// <param name="amountSignificantDigits">Round order applied to the result.</param>
        /// <param name="currencyRateSignificantDigits">Round order applied to <paramref name="originalCurrencyRate" /> 
        /// and <paramref name="targetCurrencyRate" />.</param>
        /// <param name="defaultAmount">Return value in case a conversion is not possible, 
        /// e.g. <paramref name="targetCurrencyRate" /> is zero.</param>
        /// <returns><paramref name="amountToConvert" /> converted to <paramref name="targetCurrency" />.</returns>
        /// <returns>amountToConvert value in targetCurrency units</returns>
        /// <exception cref="ArgumentException">Parameters originalCurrencyRate, targetCurrencyRate,
        /// amountSignificantDigits and currencyRateSignificantDigits cannot be negative.</exception>
        public static decimal ConvertCurrency(decimal amountToConvert, string originalCurrency,
            decimal originalCurrencyRate, string targetCurrency, decimal targetCurrencyRate,
            string baseCurrency, int amountSignificantDigits, int currencyRateSignificantDigits,
            decimal defaultAmount)
        {

            if (originalCurrencyRate < 0.0m)
                throw new ArgumentException(string.Format(Properties.Resources.ParameterCannotBeNegative,
                    nameof(originalCurrencyRate)), nameof(originalCurrencyRate));
            if (targetCurrencyRate < 0.0m)
                throw new ArgumentException(string.Format(Properties.Resources.ParameterCannotBeNegative,
                    nameof(targetCurrencyRate)), nameof(targetCurrencyRate));
            if (amountSignificantDigits < 0)
                throw new ArgumentException(string.Format(Properties.Resources.ParameterCannotBeNegative,
                    nameof(amountSignificantDigits)), nameof(amountSignificantDigits));
            if (currencyRateSignificantDigits < 0)
                throw new ArgumentException(string.Format(Properties.Resources.ParameterCannotBeNegative,
                    nameof(currencyRateSignificantDigits)), nameof(currencyRateSignificantDigits));
            
            // zero remains zero in any currency
            if (amountToConvert.AccountingRound(amountSignificantDigits) == 0.0m)
                return 0;


            // if the currencies are the same, amount is the same
            if (CurrenciesEquals(originalCurrency, targetCurrency, baseCurrency))
                return amountToConvert.AccountingRound(amountSignificantDigits);


            // base currency rate with respect to itself is always 1
            decimal validatedOriginalCurrencyRate = originalCurrencyRate;

            if (IsBaseCurrency(originalCurrency, baseCurrency))
                validatedOriginalCurrencyRate = 1.0m;

            decimal validatedTargetCurrencyRate = targetCurrencyRate;

            if (IsBaseCurrency(targetCurrency, baseCurrency))
                validatedTargetCurrencyRate = 1.0m;


            // apply currencyRateSignificantDigits
            validatedOriginalCurrencyRate = validatedOriginalCurrencyRate.AccountingRound(currencyRateSignificantDigits);
            validatedTargetCurrencyRate = validatedTargetCurrencyRate.AccountingRound(currencyRateSignificantDigits);

            // do calculus if possible
            if (validatedTargetCurrencyRate.AccountingRound(currencyRateSignificantDigits) > 0.0m)
            {
                return ((amountToConvert * validatedOriginalCurrencyRate).AccountingRound(amountSignificantDigits)
                    / validatedTargetCurrencyRate).AccountingRound(amountSignificantDigits);
            }

            return defaultAmount.AccountingRound(amountSignificantDigits);

        }

        /// <summary>
        /// Converts <paramref name="amountToConvert">amountToConvert</paramref> to the base currency.
        /// </summary>
        /// <param name="amountToConvert">Amount to convert accounted in <paramref name="currency" />.</param>
        /// <param name="currency">ISO 4217 code for the currency that the <paramref name="amountToConvert" /> is accounted in.</param>
        /// <param name="currencyRate">Currency rate with respect to the <paramref name="baseCurrency" />
        /// for the currency that the <paramref name="amountToConvert" /> is accounted in.</param>
        /// <param name="baseCurrency">Base currency identified by ISO 4217 code.</param>
        /// <param name="amountSignificantDigits">Round order applied to the result.</param>
        /// <param name="currencyRateSignificantDigits">Round order applied to <paramref name="currencyRate" />.</param>
        /// <exception cref="ArgumentException">Parameters currencyRate, amountSignificantDigits 
        /// and currencyRateSignificantDigits cannot be negative.</exception>
        /// <remarks>amountToConvert value in baseCurrency units</remarks>
        public static double ConvertToBaseCurrency(double amountToConvert, string currency,
            double currencyRate, string baseCurrency, int amountSignificantDigits,
            int currencyRateSignificantDigits)
        {

            if ((0.0).GreaterThan(currencyRate))
                throw new ArgumentException(string.Format(Apskaita5.Common.Properties.Resources.ParameterCannotBeNegative,
                    nameof(currencyRate)), nameof(currencyRate));
            if (amountSignificantDigits < 0)
                throw new ArgumentException(string.Format(Apskaita5.Common.Properties.Resources.ParameterCannotBeNegative,
                    nameof(amountSignificantDigits)), nameof(amountSignificantDigits));
            if (currencyRateSignificantDigits < 0)
                throw new ArgumentException(string.Format(Apskaita5.Common.Properties.Resources.ParameterCannotBeNegative,
                    nameof(currencyRateSignificantDigits)), nameof(currencyRateSignificantDigits));

            if (IsBaseCurrency(currency, baseCurrency))
                return amountToConvert.AccountingRound(amountSignificantDigits);

            double validatedCurrencyRate = currencyRate.AccountingRound(currencyRateSignificantDigits);
            double validatedAmount = amountToConvert.AccountingRound(amountSignificantDigits);

            return (validatedAmount * validatedCurrencyRate).AccountingRound(amountSignificantDigits);

        }

        /// <summary>
        /// Converts <paramref name="amountToConvert">amountToConvert</paramref> to the base currency.
        /// </summary>
        /// <param name="amountToConvert">Amount to convert accounted in <paramref name="currency" />.</param>
        /// <param name="currency">ISO 4217 code for the currency that the <paramref name="amountToConvert" /> 
        /// is accounted in.</param>
        /// <param name="currencyRate">Currency rate with respect to the <paramref name="baseCurrency" />
        /// for the currency that the <paramref name="amountToConvert" /> is accounted in.</param>
        /// <param name="baseCurrency">Base currency identified by ISO 4217 code.</param>
        /// <param name="amountSignificantDigits">Round order applied to the result.</param>
        /// <param name="currencyRateSignificantDigits">Round order applied to <paramref name="currencyRate" />.</param>
        /// <exception cref="ArgumentException">Parameters currencyRate, amountSignificantDigits 
        /// and currencyRateSignificantDigits cannot be negative.</exception>
        /// <remarks>amountToConvert value in baseCurrency units</remarks>
        public static decimal ConvertToBaseCurrency(decimal amountToConvert, string currency,
            decimal currencyRate, string baseCurrency, int amountSignificantDigits,
            int currencyRateSignificantDigits)
        {

            if (currencyRate < 0.0m)
                throw new ArgumentException(string.Format(Apskaita5.Common.Properties.Resources.ParameterCannotBeNegative,
                    nameof(currencyRate)), nameof(currencyRate));
            if (amountSignificantDigits < 0)
                throw new ArgumentException(string.Format(Apskaita5.Common.Properties.Resources.ParameterCannotBeNegative,
                    nameof(amountSignificantDigits)), nameof(amountSignificantDigits));
            if (currencyRateSignificantDigits < 0)
                throw new ArgumentException(string.Format(Apskaita5.Common.Properties.Resources.ParameterCannotBeNegative,
                    nameof(currencyRateSignificantDigits)), nameof(currencyRateSignificantDigits));
            
            if (IsBaseCurrency(currency, baseCurrency))
                return amountToConvert.AccountingRound(amountSignificantDigits);

            decimal validatedCurrencyRate = currencyRate.AccountingRound(currencyRateSignificantDigits);
            decimal validatedAmount = amountToConvert.AccountingRound(amountSignificantDigits);

            return (validatedAmount * validatedCurrencyRate).AccountingRound(amountSignificantDigits);

        }

        /// <summary>
        /// Converts <paramref name="amountToConvert">amountToConvert</paramref> to the base currency.
        /// </summary>
        /// <param name="amountToConvert">Amount to convert accounted in <paramref name="baseCurrency" />.</param>
        /// <param name="targetCurrency">ISO 4217 code for the currency to convert to.</param>
        /// <param name="currencyRate">Target currency rate with respect to the <paramref name="baseCurrency" />.</param>
        /// <param name="baseCurrency">Base currency identified by ISO 4217 code.</param>
        /// <param name="amountSignificantDigits">Round order applied to the result.</param>
        /// <param name="currencyRateSignificantDigits">Round order applied to <paramref name="currencyRate" />.</param>
        /// <param name="defaultAmount">Return value in case a conversion is not possible, 
        /// e.g. <paramref name="currencyRate" /> is zero.</param>
        /// <returns>amountToConvert value in targetCurrency units</returns>
        /// <exception cref="ArgumentException">Parameters currencyRate, amountSignificantDigits 
        /// and currencyRateSignificantDigits cannot be negative.</exception>
        public static double ConvertFromBaseCurrency(double amountToConvert, string targetCurrency,
            double currencyRate, string baseCurrency, int amountSignificantDigits,
            int currencyRateSignificantDigits, double defaultAmount)
        {

            if ((0.0).GreaterThan(currencyRate))
                throw new ArgumentException(string.Format(Properties.Resources.ParameterCannotBeNegative,
                    nameof(currencyRate)), nameof(currencyRate));
            if (amountSignificantDigits < 0)
                throw new ArgumentException(string.Format(Properties.Resources.ParameterCannotBeNegative,
                    nameof(amountSignificantDigits)), nameof(amountSignificantDigits));
            if (currencyRateSignificantDigits < 0)
                throw new ArgumentException(string.Format(Properties.Resources.ParameterCannotBeNegative,
                    nameof(currencyRateSignificantDigits)), nameof(currencyRateSignificantDigits));
            
            if (IsBaseCurrency(targetCurrency, baseCurrency))
                return amountToConvert.AccountingRound(amountSignificantDigits);

            double validatedCurrencyRate = currencyRate.AccountingRound(currencyRateSignificantDigits);
            double validatedAmount = amountToConvert.AccountingRound(amountSignificantDigits);

            if (validatedCurrencyRate.GreaterThan(0.0, currencyRateSignificantDigits))
            {
                return (validatedAmount / validatedCurrencyRate).AccountingRound(amountSignificantDigits);
            }

            return defaultAmount.AccountingRound(amountSignificantDigits);

        }

        /// <summary>
        /// Converts <paramref name="amountToConvert">amountToConvert</paramref> to the base currency.
        /// </summary>
        /// <param name="amountToConvert">Amount to convert accounted in <paramref name="baseCurrency" />.</param>
        /// <param name="targetCurrency">ISO 4217 code for the currency to convert to.</param>
        /// <param name="currencyRate">Target currency rate with respect to the <paramref name="baseCurrency" />.</param>
        /// <param name="baseCurrency">Base currency identified by ISO 4217 code.</param>
        /// <param name="amountSignificantDigits">Round order applied to the result.</param>
        /// <param name="currencyRateSignificantDigits">Round order applied to the <paramref name="currencyRate" />.</param>
        /// <param name="defaultAmount">Return value in case a conversion is not possible, 
        /// e.g. <paramref name="currencyRate" /> is zero.</param>
        /// <returns>amountToConvert value in targetCurrency units</returns>
        /// <exception cref="ArgumentException">Parameters currencyRate, amountSignificantDigits 
        /// and currencyRateSignificantDigits cannot be negative.</exception>
        public static decimal ConvertFromBaseCurrency(decimal amountToConvert, string targetCurrency,
            decimal currencyRate, string baseCurrency, int amountSignificantDigits,
            int currencyRateSignificantDigits, decimal defaultAmount)
        {

            if (currencyRate < 0.0m)
                throw new ArgumentException(string.Format(Properties.Resources.ParameterCannotBeNegative,
                    nameof(currencyRate)), nameof(currencyRate));
            if (amountSignificantDigits < 0)
                throw new ArgumentException(string.Format(Properties.Resources.ParameterCannotBeNegative,
                    nameof(amountSignificantDigits)), nameof(amountSignificantDigits));
            if (currencyRateSignificantDigits < 0)
                throw new ArgumentException(string.Format(Properties.Resources.ParameterCannotBeNegative,
                    nameof(currencyRateSignificantDigits)), nameof(currencyRateSignificantDigits));
            
            if (IsBaseCurrency(targetCurrency, baseCurrency))
                return amountToConvert.AccountingRound(amountSignificantDigits);

            decimal validatedCurrencyRate = currencyRate.AccountingRound(currencyRateSignificantDigits);
            decimal validatedAmount = amountToConvert.AccountingRound(amountSignificantDigits);

            if (validatedCurrencyRate > 0.0m)
            {
                return (validatedAmount / validatedCurrencyRate).AccountingRound(amountSignificantDigits);
            }

            return defaultAmount.AccountingRound(amountSignificantDigits);

        }

        /// <summary>
        /// Checks if the <paramref name="currencyCode">currency code</paramref> is a valid ISO 4217 code.
        /// </summary>
        /// <param name="currencyCode">A currency code to check.</param>
        /// <param name="emptyCurrencyIsInvalid">Whether to consider a null or empty string as an invalid currency.
        /// Default - consider a null or empty string as the base currency, i.e. valid.</param>
        /// <returns><code>True</code> if the <paramref name="currencyCode" /> is a valid ISO 4217 code..</returns>
        /// <remarks></remarks>
        public static bool IsValidCurrency(string currencyCode, bool emptyCurrencyIsInvalid)
        {

            if (currencyCode.IsNullOrWhiteSpace())
                return !emptyCurrencyIsInvalid;

            return (Array.IndexOf(CurrencyCodes, currencyCode.Trim().ToUpperInvariant()) >= 0);

        }

    }
}