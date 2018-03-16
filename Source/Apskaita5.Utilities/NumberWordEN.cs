using System;
using System.Globalization;
using System.Text;

namespace Apskaita5.Common
{
    /// <summary>
    /// Represents a numeric to natural language convertor for the English language.
    /// </summary>
    class NumberWordEN : NumberWordBase
    {

        private static string[] _ones =
        {
            "zero",
            "one",
            "two",
            "three",
            "four",
            "five",
            "six",
            "seven",
            "eight",
            "nine"
        };

        private static string[] _teens =
        {
            "ten",
            "eleven",
            "twelve",
            "thirteen",
            "fourteen",
            "fifteen",
            "sixteen",
            "seventeen",
            "eighteen",
            "nineteen"
        };

        private static string[] _tens =
        {
            "",
            "ten",
            "twenty",
            "thirty",
            "forty",
            "fifty",
            "sixty",
            "seventy",
            "eighty",
            "ninety"
        };

        // US Nnumbering:
        private static string[] _thousands =
        {
            "",
            "thousand",
            "million",
            "billion",
            "trillion",
            "quadrillion"
        };


        /// <summary>
        /// Gets an ISO 639-1 language code for the language that the implementation uses, i.e. EN.
        /// </summary>
        public override string Language
        {
            get { return "LT"; }
        }

        /// <summary>
        /// Converts the value to the natural language.
        /// </summary>
        /// <param name="value">a value to convert</param>
        /// <param name="currency">a currency string to use (default EUR)</param>
        /// <param name="cents">a cents value to use (default ct.)</param>
        public override string ConvertToWords(double value, string currency, string cents)
        {
            if (currency.IsNullOrWhiteSpace()) currency = "EUR";
            if (cents.IsNullOrWhiteSpace()) currency = "ct.";
            var strNum = value.ToString("#.00", CultureInfo.InvariantCulture);
            var centsValue = strNum.Substring(strNum.Length - 2, 2);
            var minus = string.Empty;
            if (Math.Sign(value) < 0) minus = "minus ";
            return minus + Convert((decimal)value) + " " + currency.Trim() + " and " + centsValue + " " + cents;
        }

        /// <summary>
        /// Converts the value to the natural language.
        /// </summary>
        /// <param name="value">a value to convert</param>
        /// <param name="currency">a currency string to use (default EUR)</param>
        /// <param name="cents">a cents value to use (default ct.)</param>
        public override string ConvertToWords(decimal value, string currency, string cents)
        {
            if (currency.IsNullOrWhiteSpace()) currency = "EUR";
            if (cents.IsNullOrWhiteSpace()) currency = "ct.";
            var strNum = value.ToString("#.00", CultureInfo.InvariantCulture);
            var centsValue = strNum.Substring(strNum.Length - 2, 2);
            var minus = string.Empty;
            if (Math.Sign(value) < 0) minus = "minus ";
            return minus + Convert(value) + " " + currency.Trim() + " and " + centsValue + " " + cents;
        }

        /// <summary>
        /// Converts the value to the natural language.
        /// </summary>
        /// <param name="value">a value to convert</param>
        public override string ConvertToWords(int value)
        {
            var minus = string.Empty;
            if (Math.Sign(value) < 0) minus = "minus ";
            return minus + Convert((decimal)value);
        }


        /// <summary>
        /// Converts a numeric value to words suitable for the portion of
        /// a check that writes out the amount.
        /// </summary>
        /// <param name="value">Value to be converted</param>
        /// <returns></returns>
        private static string Convert(decimal value)
        {
            string digits, temp;
            bool showThousands = false;
            bool allZeros = true;

            // Use StringBuilder to build result
            StringBuilder builder = new StringBuilder();
            // Convert integer portion of value to string
            digits = ((long)value).ToString();
            // Traverse characters in reverse order
            for (int i = digits.Length - 1; i >= 0; i--)
            {
                int ndigit = (int)(digits[i] - '0');
                int column = (digits.Length - (i + 1));

                // Determine if ones, tens, or hundreds column
                switch (column % 3)
                {
                    case 0:        // Ones position
                        showThousands = true;
                        if (i == 0)
                        {
                            // First digit in number (last in loop)
                            temp = String.Format("{0} ", _ones[ndigit]);
                        }
                        else if (digits[i - 1] == '1')
                        {
                            // This digit is part of "teen" value
                            temp = String.Format("{0} ", _teens[ndigit]);
                            // Skip tens position
                            i--;
                        }
                        else if (ndigit != 0)
                        {
                            // Any non-zero digit
                            temp = String.Format("{0} ", _ones[ndigit]);
                        }
                        else
                        {
                            // This digit is zero. If digit in tens and hundreds
                            // column are also zero, don't show "thousands"
                            temp = String.Empty;
                            // Test for non-zero digit in this grouping
                            if (digits[i - 1] != '0' || (i > 1 && digits[i - 2] != '0'))
                                showThousands = true;
                            else
                                showThousands = false;
                        }

                        // Show "thousands" if non-zero in grouping
                        if (showThousands)
                        {
                            if (column > 0)
                            {
                                temp = String.Format("{0}{1}{2}",
                                    temp,
                                    _thousands[column / 3],
                                    allZeros ? " " : ", ");
                            }
                            // Indicate non-zero digit encountered
                            allZeros = false;
                        }
                        builder.Insert(0, temp);
                        break;

                    case 1:        // Tens column
                        if (ndigit > 0)
                        {
                            temp = String.Format("{0}{1}",
                                _tens[ndigit],
                                (digits[i + 1] != '0') ? "-" : " ");
                            builder.Insert(0, temp);
                        }
                        break;

                    case 2:        // Hundreds column
                        if (ndigit > 0)
                        {
                            temp = String.Format("{0} hundred ", _ones[ndigit]);
                            builder.Insert(0, temp);
                        }
                        break;
                }
            }

            // Capitalize first letter
            return String.Format("{0}{1}",
                Char.ToUpper(builder[0]),
                builder.ToString(1, builder.Length - 1));
        }



    }

}