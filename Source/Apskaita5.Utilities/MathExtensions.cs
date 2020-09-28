using System;

namespace Apskaita5.Common.MathExtensions
{
    public static class MathExtensions
    {

        /// <summary>
        /// Returns a double value rounded using accounting algorithm.
        /// </summary>
        /// <param name="value">the value to round</param>
        /// <param name="roundOrder">the rounding order</param>
        /// <exception cref="ArgumentOutOfRangeException">Round order should be between 0 and 20.</exception>
        public static double AccountingRound(this double value, int roundOrder)
        {
            if (roundOrder < 0 || roundOrder > 20)
                throw new ArgumentOutOfRangeException(nameof(roundOrder), roundOrder,
                    Properties.Resources.RoundOrderOutOfRange);

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
                throw new ArgumentOutOfRangeException(nameof(roundOrder), roundOrder,
                    Properties.Resources.RoundOrderOutOfRange);

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

    }
}
