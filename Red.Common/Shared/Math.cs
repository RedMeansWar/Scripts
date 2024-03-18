using System;

namespace Red.Common
{
    public class MathHelper
    {
        // These are all seperated method for use if extension is not needed.
        #region Variables
        protected readonly static Random random = new(Guid.NewGuid().GetHashCode());
        #endregion
        /// <summary>
        /// Converts a double to a float value.
        /// </summary>
        /// <param name="value">the number that needs to be converted to a float</param>
        /// <returns>the converted float value</returns>
        public static float ConvertDoubleToFloat(double value) => (float)value;

        public static float ConvertIntToFloat(int value) => value;

        /// <summary>.
        /// Converts a double to a int value
        /// </summary>
        /// <param name="value">the number that needs to be converted to a int</param>
        /// <returns>the converted int value</returns>
        public static int ConvertDoubleToInt(double value) => Convert.ToInt32(value);

        /// <summary>
        /// Rounds a double value to an integer based on the specified rounding mode.
        /// </summary>
        /// <param name="value">The double value to be converted.</param>
        /// <param name="mode">The rounding mode to use</param>
        /// <returns>The rounded integer value.</returns>
        /// <exception cref="ArgumentOutOfRangeException">Thrown if an invalid RoundingMode value is provided.</exception>
        public static int RoundToInt(double value, RoundingMode mode)
        {
            switch (mode)
            {
                /// <summary>
                /// Truncates the decimal part of the value. (drop the decimal part)
                /// </summary>
                case RoundingMode.Trunacte:
                    return (int)value;
                /// <summary>
                /// Rounds the value down to the nearest integer.
                /// </summary>
                case RoundingMode.Floor:
                    return (int)Math.Floor(value);
                /// <summary>
                /// Rounds the value up to the nearest integer.
                /// </summary>
                case RoundingMode.Ceiling:
                    return (int)Math.Ceiling(value);
                /// <summary>
                /// Rounds the value towards zero (rounds half to even).
                /// </summary>
                case RoundingMode.RoundingHalfToEven:
                    return Convert.ToInt32(value); // Recommended for most cases
                default:
                    throw new ArgumentOutOfRangeException();
            }
        }

        /// <summary>
        /// Grabs the random interval of 10 between 2 numbers.
        /// </summary>
        /// <param name="one">first number</param>
        /// <param name="two">second number</param>
        /// <returns>A random interval of 10 between 2 numbers</returns>
        public static int RandomIntervalOf10Between(int num1, int num2) => ((int)Math.Round(random.Next(num1, num2) / 10.0)) * 10;

        /// <summary>
        /// Gives a random number or return it as 0 or half of a number.
        /// </summary>
        /// <returns></returns>
        public static double RandomZeroOrHalf() => random.NextDouble() > 0.5 ? 0.5 : 0.0;

        /// <summary>
        /// Calculates the greatest common divisor (GCD) of two integers.
        /// </summary>
        /// <param name="num1">The first integer.</param>
        /// <param name="num2">The second integer.</param>
        /// <returns>The greatest common divisor of num1 and num2.</returns>
        public static int GreatestCommonDivisor(int num1, int num2)
        {
            while (num2 != 0)
            {
                int temp = num2;
                num2 = num1 % num2;
                num1 = temp;
            }

            return num1;
        }

        public static int GCD(int num1, int num2) => GreatestCommonDivisor(num1, num2);

        /// <summary>
        /// Calculates the least common multiple (LCM) of two integers.
        /// </summary>
        /// <param name="num1">The first integer.</param>
        /// <param name="num2">The second integer.</param>
        /// <returns>The least common multiple of num1 and num2</returns>
        public static int LeastCommonMultiple(int num1, int num2)
        {
            if (num1 == 0 || num2 == 0)
            {
                return 0;
            }

            return Math.Abs(num1 * num2) / GreatestCommonDivisor(num1, num2);
        }

        public static float GetRandomFloat(float min, float max)
        {
            Random random = new(Environment.TickCount);

            return (float)random.NextDouble() * (max - min) + min;
        }
    }

    public static class MathExtension
    {
        /// <summary>
        /// Converts a double to a float value.
        /// </summary>
        /// <param name="value">the number that needs to be converted to a float</param>
        /// <returns>the converted float value</returns>
        public static float ConvertDoubleToFloat(this double value) => (float)value;

        public static float ConvertIntToFloat(this int value) => value;

        /// <summary>.
        /// Converts a double to a int value
        /// </summary>
        /// <param name="value">the number that needs to be converted to a int</param>
        /// <returns>the converted int value</returns>
        public static int ConvertDoubleToInt(this double value) => Convert.ToInt32(value);

        /// <summary>
        /// Rounds a double value to an integer based on the specified rounding mode.
        /// </summary>
        /// <param name="value">The double value to be converted.</param>
        /// <param name="mode">The rounding mode to use</param>
        /// <returns>The rounded integer value.</returns>
        /// <exception cref="ArgumentOutOfRangeException">Thrown if an invalid RoundingMode value is provided.</exception>
        public static int RoundToInt(this double value, RoundingMode mode)
        {
            switch (mode)
            {
                /// <summary>
                /// Truncates the decimal part of the value. (drop the decimal part)
                /// </summary>
                case RoundingMode.Trunacte:
                    return (int)value;
                /// <summary>
                /// Rounds the value down to the nearest integer.
                /// </summary>
                case RoundingMode.Floor:
                    return (int)Math.Floor(value);
                /// <summary>
                /// Rounds the value up to the nearest integer.
                /// </summary>
                case RoundingMode.Ceiling:
                    return (int)Math.Ceiling(value);
                /// <summary>
                /// Rounds the value towards zero (rounds half to even).
                /// </summary>
                case RoundingMode.RoundingHalfToEven:
                    return Convert.ToInt32(value); // Recommended for most cases
                default:
                    throw new ArgumentOutOfRangeException();
            }
        }
    }

    /// <summary>
    /// Defines the rounding behavior for converting a double to an integer.
    /// </summary>
    public enum RoundingMode
    {
        Trunacte, // Discards the decimal part
        Floor, // Rounds down to the nearest integer
        Ceiling, // Rounds up to the nearest integer
        RoundingHalfToEven // Uses Convert.ToInt32 for rounding towards zero (default for most cases).
    }
}