using System;

namespace Red.Common.Client.Misc
{
    public static class MathExtensions
    {
        /// <summary>
        /// Rounds up to the nearest odd number.
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        public static int RoundUpToNearestEven(this int value) => (value % 2 == 0) ? value : value + 1;
        /// <summary>
        /// Rounds up to the nearest odd number.
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        public static int RoundUpToNearestOdd(this int value) => (value % 2 == 1) ? value : value + 1;
        /// <summary>
        /// Rounds up to the nearest number using a double and remainders.
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        public static double RoundUpToNearestEven(this double value)
        {
            double remainder = value % 1.0;

            if (remainder < 0.5)
            {
                return Math.Floor(value);
            }
            else
            {
                return Math.Ceiling(value);
            }
        }
        /// <summary>
        /// Converts a string to a unit.
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        /// <exception cref="ArgumentException"></exception>
        public static uint ConvertToUInt(this string value)
        {
            uint result;

            if (!uint.TryParse(value, out result))
            {
                throw new ArgumentException("Invalid string format for conversion to uint.");
            }
            return result;
        }
    }
}
