﻿using CitizenFX.Core;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Red.Common.Client
{
    public class MathExtender
    {
        public static float ZeroTolerance = 1e-6f;
        public static float Pi = (float)Math.PI; 
        public static float TwoPi = 2 * Pi;
        public static float Tau = 2 * Pi;
        public static float PiOverTwo = Pi / 2;
        public static float PiOverFour = Pi / 4;

        /// <summary>
        /// Round to the nearest integer (Shortened version of Math.Round).
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        public static double Round(double value) => Math.Round(value);

        /// <summary>
        /// Round to the nearest tenth (Shortened version of Math.Round with a digit of 1).
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        public static double RoundToNearestTen(double value) => Math.Round(value, 1);

        /// <summary>
        /// Round to the nearest hundredth (Shortened version of Math.Round with a digit of 2).
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        public static double RoundToNearestHundredth(double value) => Math.Round(value, 2);

        /// <summary>
        /// Round to the nearest thousandth (Shortened version of Math.Round with a digit of 3).
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        public static double RoundToNearestThousandth(double value) => Math.Round(value, 3);

        /// <summary>
        /// Raises a number to the power of 2 using a double value.
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        public static double Square(double value) => value * value;

        /// <summary>
        /// Raises a number to the power of 3 using a double value.
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        public static double Cube(double value) => value * value * value;

        /// <summary>
        /// Converts a floating point (float) value to a double value.
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        public static double ConvertFloatToDouble(float value) => Convert.ToDouble(value);

        /// <summary>
        /// Converts a integer (int) value to a double value.
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        public static double ConvertIntToDouble(int value) => Convert.ToDouble(value);

        /// <summary>
        /// Converts a double value to a floating point (float).
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        public static float ConvertDoubleToFloat(double value) => Convert.ToSingle(value);

        /// <summary>
        /// Converts a integer value to a floating point (float).
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        public static float ConvertIntToFloat(int value) => Convert.ToSingle(value);

        /// <summary>
        /// Rounds a floating point (float) down to the nearest tenth.
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        public static float RoundToLowestTenth(float value) => (float)Math.Floor((float)value / 10) * 10;

        /// <summary>
        /// Raises a number to the power of 2 using a floating point (float) value.
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        public static float Square(float value) => value * value;

        /// <summary>
        /// Raises a number to the power of 3 using a floating point (float) value.
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        public static float Cube(float value) => value * value * value;

        public static int RoundToLowestTenth(int value) => (int)Math.Floor((double)value / 10) * 10;
        public static int ConvertFloatToInt(float value) => Convert.ToInt32(value);
        public static int ConvertDoubleToInt(double value) => Convert.ToInt32(value);
        /// <summary>
        /// Raises a number to the power of 2 using a integer (int) value.
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        public static int Square(int value) => value * value;

        /// <summary>
        /// Raises a number to the power of 3 using a integer (int) value.
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        public static int Cube(int value) => value * value * value;

        /// <summary>
        /// Converts a floating point (float) to a unsigned integer (uint). 
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        public static uint ConvertFloatToUInt(float value) => Convert.ToUInt32(value);
        public static uint ConvertIntToUInt(int value) => Convert.ToUInt32(value);

        /// <summary>
        /// Raises a number to the power of 2 using a unsigned integer (uint) value.
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        public static float Square(uint value) => value * value;
        /// <summary>
        /// Raises a number to the power of 3 using a unsigned integer (uint) value.
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        public static float Cube(uint value) => value * value * value;

        /// <summary>
        /// Converts a double value to a unsigned integer (uint).
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        /// <exception cref="ArgumentOutOfRangeException"></exception>
        public static uint ConvertDoubleToUInt(double value)
        {
            if (value >= 0)
            {
                return (uint)Math.Round(value);
            }
            else
            {
                throw new ArgumentOutOfRangeException("value", "[ERROR]: Value is out of the valid uint range.");
            }
        }

        /// <summary>
        /// Converts a string to a unsigned integer (uint).
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        /// <exception cref="ArgumentException"></exception>
        public static uint ConvertStringToUint(string value)
        {
            if (uint.TryParse(value, out uint uintValue))
            {
                return uintValue;
            }
            else
            {
                throw new ArgumentException("[ERROR]: Invalid input; unable to convert string to uint.");
            }
        }

        /// <summary>
        /// Converts a string to a floating point (float) value.
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        /// <exception cref="ArgumentException"></exception>
        public static float ConvertStringToFloat(string value)
        {
            if (float.TryParse(value, out float floatValue))
            {
                return floatValue;
            }
            else
            {
                throw new ArgumentException("[ERROR]: Invalid input; unable to convert string to float.");
            }
        }

        /// <summary>
        /// Converts a string to a integer (int) value.
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        /// <exception cref="ArgumentException"></exception>
        public static int ConvertStringToInt(string value)
        {
            if (int.TryParse(value, out int intValue))
            {
                return intValue;
            }
            else
            {
                throw new ArgumentException("[ERROR]: Invalid input; unable to convert string to int.");
            }
        }

        /// <summary>
        /// Converts a string to a double value.
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        /// <exception cref="FormatException"></exception>
        public static double ConvertStringToDouble(string value)
        {
            if (double.TryParse(value, out double doubleValue))
            {
                return doubleValue;
            }
            else
            {
                throw new FormatException("[ERROR]: Invalid format; unable to convert string to double.");
            }
        }

        /// <summary>
        /// Determains if a 2 floating points (floats) are close to being equal to another value.
        /// </summary>
        /// <param name="a"></param>
        /// <param name="b"></param>
        /// <returns></returns>
        public static bool NearEqual(float a, float b)
        {
            return NearEqualInternal(a, b);
        }

        /// <summary>
        /// An unsafe method that is used to determine if a number is close to being equal to another number.
        /// </summary>
        /// <param name="a"></param>
        /// <param name="b"></param>
        /// <returns></returns>
        public unsafe static bool NearEqualInternal(float a, float b)
        {
            if (IsZero(a - b))
            {
                return true;
            }

            int aInt = *(int*)&a;
            int bInt = *(int*)&b;

            if ((aInt < 0) != (bInt < 0))
            {
                return false;
            }

            int ulp = Math.Abs(aInt - bInt);

            const int maxUlp = 4;
            return (ulp <= maxUlp);
        }

        /// <summary>
        /// Determains if a 2 integers (ints) are close to being equal to another value.
        /// </summary>
        /// <param name="a"></param>
        /// <param name="b"></param>
        /// <returns></returns>
        public static bool NearEqual(int a, int b)
        {
            return NearEqualInternal(a, b);
        }

        /// <summary>
        /// An unsafe method that is used to determine if a number is close to being equal to another number.
        /// </summary>
        /// <param name="a"></param>
        /// <param name="b"></param>
        /// <returns></returns>
        public unsafe static bool NearEqualInternal(int a, int b)
        {
            if (IsZero(a - b))
            {
                return true;
            }

            int aInt = *(int*)&a;
            int bInt = *(int*)&b;

            if ((aInt < 0) != (bInt < 0))
            {
                return false;
            }

            int ulp = Math.Abs(aInt - bInt);

            const int maxUlp = 4;
            return (ulp <= maxUlp);
        }

        /// <summary>
        /// Converts a Vector3 rotation to a facing direction.
        /// </summary>
        /// <param name="rotation"></param>
        /// <returns></returns>
        public static Vector3 RotationToDirection(Vector3 rotation)
        {
            float rotZ = DegreesToRadians(rotation.Z);
            float rotX = DegreesToRadians(rotation.X);
            float multXY = Math.Abs((float)Math.Cos(rotX));
            return new Vector3((float)-Math.Sin(rotZ) * multXY, (float)Math.Cos(rotZ) * multXY, (float)Math.Sin(rotX));
        }

        /// <summary>
        /// Converts a facing direction to a Vector3 rotation.
        /// </summary>
        /// <param name="rotation"></param>
        /// <returns></returns>
        public static float ConvertDirectionToHeading(Vector3 direction)
        {
            Vector2 dir2 = (Vector2)direction;
            dir2.Normalize();
            return RadiansToDegrees((float)Math.Atan2(dir2.X, dir2.Y));
        }

        /// <summary>
        /// Converts a heading to a facing direction.
        /// </summary>
        /// <param name="heading"></param>
        /// <returns></returns>
        public static Vector3 ConvertHeadingToDirection(float heading)
        {
            heading = DegreesToRadians(heading);
            return new Vector3((float)-Math.Sin(heading), (float)Math.Cos(heading), 0.0f);
        }

        /// <summary>
        /// Converts integer (int) value radians value to degrees (20 Radians -> 1145.92°)
        /// </summary>
        /// <param name="degrees"></param>
        /// <returns></returns>
        public static float ConvertDegreesToRadians(float degrees)
        {
            return degrees * (Pi / 180.0f);
        }

        /// <summary>
        /// Converts double value radians value to degrees (20 Radians -> 1145.92°)
        /// </summary>
        /// <param name="radians"></param>
        /// <returns></returns>
        public static double ConvertRadiansToDegrees(double radians)
        {
            return radians * (Pi / 180.0f);
        }

        /// <summary>
        /// Converts floating point (float) value radians value to degrees (20 Radians -> 1145.92°)
        /// </summary>
        /// <param name="radians"></param>
        /// <returns></returns>
        public static float ConvertRadiansToDegrees(float radians)
        {
            return radians * (Pi / 180.0f);
        }

        public static float RotateHeading(float heading, float degreesToRotate)
        {
            heading = (heading % 360.0f + 360.0f) % 360.0f;
            float rotatedHeading = heading + degreesToRotate;

            rotatedHeading = (rotatedHeading % 360.0f + 360.0f) % 360.0f;
            return rotatedHeading;
        }

        /// <summary>
        /// Determines if a value will be zero based off of a floating point (float) value.
        /// </summary>
        /// <param name="a"></param>
        /// <returns></returns>
        public static bool IsZero(float a)
        {
            return Math.Abs(a) < ZeroTolerance;
        }

        /// <summary>
        /// Determines if a value will be one based off of a floating point (float) value.
        /// </summary>
        /// <param name="a"></param>
        /// <returns></returns>
        public static bool IsOne(float a)
        {
            return IsZero(a - 1.0f);
        }

        /// <summary>
        /// Converts revolutions to degrees using a floating point (float) value. (1 Revolution -> 360°)
        /// </summary>
        /// <param name="revolution"></param>
        /// <returns></returns>
        public static float RevolutionsToDegrees(float revolution)
        {
            return revolution * 360.0f;
        }

        /// <summary>
        /// Converts revolutions to radians using a floating point (float) value. (1 Revolution -> 6.28319 Radians)
        /// </summary>
        /// <param name="revolution"></param>
        /// <returns></returns>
        public static float RevolutionsToRadians(float revolution)
        {
            return revolution * TwoPi;
        }

        /// <summary>
        /// Converts revolutions to gradians using a floating point (float) value. (1 Revolution -> 400 Gradians)
        /// </summary>
        /// <param name="revolution"></param>
        /// <returns></returns>
        public static float RevolutionsToGradians(float revolution)
        {
            return revolution * 400.0f;
        }

        public static float DegreesToRevolutions(float degree)
        {
            return degree / 360.0f;
        }

        public static float DegreesToRadians(float degree)
        {
            return degree * (Pi / 180.0f);
        }

        public static float RadiansToRevolutions(float radian)
        {
            return radian / TwoPi;
        }

        /// <summary>
        /// Converts radians to gradians using floating point (float) value. (5 Radians -> 318.31 Gardians).
        /// </summary>
        /// <param name="radian"></param>
        /// <returns></returns>
        public static float RadiansToGradians(float radian)
        {
            return radian * (200.0f / Pi);
        }

        /// <summary>
        /// Converts gradians to revolutions using floating point (float). (5 Gradians -> 0.0125 Revolution)
        /// </summary>
        /// <param name="gradian"></param>
        /// <returns></returns>
        public static float GradiansToRevolutions(float gradian)
        {
            return gradian / 400.0f;
        }

        /// <summary>
        /// Converts gardian to degrees using a floating point (float) value. (5 Gardians -> 4.5°)
        /// </summary>
        /// <param name="gradian"></param>
        /// <returns></returns>
        public static float GradiansToDegrees(float gradian)
        {
            return gradian * (9.0f / 10.0f);
        }

        public static float GradiansToRadians(float gradian)
        {
            return gradian * (Pi / 200.0f);
        }

        public static float RadiansToDegrees(float radian)
        {
            return radian * (180.0f / Pi);
        }

        /// <summary>
        /// Determines if a integer (int) is to the power of 2. (squared)
        /// </summary>
        /// <param name="number"></param>
        /// <returns></returns>
        public static bool IsPowerOfTwo(int number)
        {
            if (number <= 0)
            {
                return false;
            }

            return (number & (number - 1)) == 0;
        }

        /// <summary>
        /// Determines if a integer (int) is to the power of 2 or is 0.
        /// </summary>
        /// <param name="number"></param>
        /// <returns></returns>
        public static bool IsZeroOrPowerOfTwo(int number)
        {
            if (number < 0)
            {
                return false;
            }

            return number == 0 || (number - 1) == 0;
        }

        /// <summary>
        /// Rotates a heading to normal position using a floating point (float) value.
        /// </summary>
        /// <param name="heading"></param>
        /// <returns></returns>
        public static float NormalizeHeading(float heading)
        {
            heading = (heading % 360.0f + 360.0f) % 360.0f;
            return heading;
        }

        public static float Lerp(float from, float to, float amount)
        {
            return (1 - amount) * from + amount * to;
        }

        public static double Lerp(double from, double to, double amount)
        {
            return (1 - amount) * from + amount * to;
        }

        public static byte Lerp(byte from, byte to, byte amount)
        {
            return (byte)Lerp(from, (float)to, amount);
        }

        public static double UnLerp(double start, double end, double lerpedValue)
        {
            if (start == end)
            {
                return 0;
            }

            return lerpedValue - start / end - start;
        }

        public static int UnLerp(int start, int end, int lerpedValue)
        {
            if (start == end)
            {
                return 0;
            }

            return lerpedValue - start / end - start;
        }

        public static long UnLerp(long start, long end, long lerpedValue)
        {
            if (start == end)
            {
                return 0;
            }

            return lerpedValue - start / end - start;
        }

        public static float UnLerp(float start, float end, float lerpedValue)
        {
            if (start == end)
            {
                return 0;
            }

            return lerpedValue - start / end - start;
        }

        public static float SmoothStep(float amount)
        {
            return (amount <= 0) ? 0 : (amount >= 1) ? 1 : amount * amount * (3 - (2 * amount));
        }

        public static float SmootherStep(float amount)
        {
            return (amount <= 0) ? 0 : (amount >= 1) ? 1 : amount * amount * amount * (amount * ((amount * 6) - 15) + 10);
        }

        /// <summary>
        /// Determines if a digit is identical. (unknown use for this method)
        /// </summary>
        /// <param name="number"></param>
        /// <returns></returns>
        public static bool AreDigitsIdentical(int number)
        {
            if (number < 0)
            {
                number = -number;
            }

            int lastDigital = number % 10;

            while (number > 0)
            {
                int digit = number % 10;

                if (digit != lastDigital)
                {
                    return false;
                }

                number /= 10;
            }

            return false;
        }

        public static int Wrap(int value, int min, int max)
        {
            if (min > max)
            {
                throw new ArgumentException(string.Format("[ERROR]: min {0} should be less than or equal to max {1}", min, max), "min");
            }
            int rangeSize = max - min + 1;

            if (rangeSize < min)
            {
                value += rangeSize * ((min - value) / rangeSize + 1);
            }

            return min + (value - min) % rangeSize;
        }

        public static float Wrap(float value, float min, float max)
        {
            if (NearEqual(min, max))
            {
                return min;
            }

            double minD = min;
            double maxD = max;
            double valueD = value;

            if (minD > maxD)
            {
                throw new ArgumentException(string.Format("[ERROR]: min {0} should be less than or equal to max {1}", min, max), "min");
            }

            var rangeSize = maxD - minD;
            return (float)(minD + (valueD - minD) - (rangeSize * Math.Floor((valueD - minD) / rangeSize)));
        }

        public static float Gauss(float amplitude, float x, float y, float radX, float radY, float sigmaX, float sigmaY)
        {
            return (float)Gauss((double)amplitude, x, y, radX, radY, sigmaX, sigmaY);
        }

        public static double Gauss(double amplitude, double x, double y, double radX, double radY, double sigmaX, double sigmaY)
        {
            return (amplitude * Math.E) - (Math.Pow(x - (radX / 2), 2) / (2 * Math.Pow(sigmaX, 2)) + Math.Pow(y - (radY / 2), 2) / (2 * Math.Pow(sigmaY, 2)));
        }

        public static T Choose<T>(ICollection<T> collection)
        {
            if (collection == null || collection.Count == 0)
            {
                throw new ArgumentException("[ERROR]: The collection is empty or null.");
            }

            int index = new Random().Next(collection.Count);
            return collection.ElementAt(index);
        }

        public static T Choose<T>(T[] array)
        {
            if (array is null || array.Length == 0)
            {
                throw new ArgumentException("[ERROR]: The array is empty or null.");
            }

            int index = new Random().Next(array.Length);
            return array[index];
        }

        public static T Clamp<T>(T value, T min, T max) where T : IComparable<T>
        {
            if (value.CompareTo(min) < 0)
            {
                return min;
            }
            else if (value.CompareTo(max) > 0)
            {
                return max;
            }

            return value;
        }

        public static T Median<T>(IEnumerable<T> values) where T : IComparable<T>
        {
            if (values is null || !values.Any())
            {
                throw new ArgumentException("[ERROR]: The collection is empty or null.");
            }

            List<T> sortedValues = values.OrderBy(v => v).ToList();
            int count = sortedValues.Count;

            if (count % 2 == 0)
            {
                int middle = count / 2;
                T middle1 = sortedValues[middle - 1];
                T middle2 = sortedValues[middle];

                if (typeof(T) == typeof(int))
                {
                    return (T)(object)(((int)(object)middle1 + (int)(object)middle2) / 2);
                }
                else if (typeof(T) == typeof(double))
                {
                    return (T)(object)(((double)(object)middle1 + (double)(object)middle2) / 2);
                }
                else
                {
                    throw new InvalidOperationException("[ERROR]: Unsupported numeric type for Median calculation.");
                }
            }
            else
            {
                int middle = count / 2;
                return sortedValues[middle - 1];
            }
        }

        public static double Normalize(double value, double min, double max)
        {
            if (min >= max)
            {
                throw new ArgumentException("[ERROR]: Min must be less than Max.");
            }

            return (value - min) / (max - min);
        }

        public static ulong Faculty(int number)
        {
            if (number < 0)
            {
                throw new ArgumentException("[ERROR]: Factorial is not defined for negative numbers.");
            }

            ulong result = 1;

            for (int i = 1; i < number; i++)
            {
                result *= (ulong)i;
            }

            return result;
        }

        public static char GetRandomAlphaNumericCharacter()
        {
            Random random = new();

            string alphaNumericChars = "ABCDEFGHIJKLMNOPQRSTUVWXYZabcdefghijklmnopqrstuvwxyz0123456789";
            int index = random.Next(alphaNumericChars.Length);

            return alphaNumericChars[index];
        }

        /// <summary>
        /// Get a random double value from the maximum number set.
        /// </summary>
        /// <param name="min"></param>
        /// <param name="max"></param>
        /// <returns></returns>
        /// <exception cref="ArgumentException"></exception>
        public static double GetRandomDouble(double min, double max)
        {
            if (min > max)
            {
                throw new ArgumentException("[ERROR]: Minimum value must be less than or equal to the maximum value.");
            }

            Random random = new();
            return random.NextDouble() * (max - min) + min;
        }

        /// <summary>
        /// Get a random integer from the maximum number set.
        /// </summary>
        /// <param name="max"></param>
        /// <returns></returns>
        /// <exception cref="ArgumentException"></exception>
        public static int GetRandomInteger(int max)
        {
            if (max <= 0)
            {
                throw new ArgumentException("[ERROR]: Maximum value must be greater than 0.");
            }

            Random random = new();
            return random.Next(max);
        }
        /// <summary>
        /// Get a random integer (int) between the minimum and maximum number set.
        /// </summary>
        /// <param name="min"></param>
        /// <param name="max"></param>
        /// <returns></returns>
        /// <exception cref="ArgumentException"></exception>
        public static int GetRandomInteger(int min, int max)
        {
            if (min > max)
            {
                throw new ArgumentException("[ERROR]: Minimum value must be less than or equal to the maximum value.");
            }

            Random random = new();

            return random.Next(min, max + 1);
        }

        /// <summary>
        /// Get a random floating point (float) value between the minimum and maximum number set.
        /// </summary>
        /// <param name="min"></param>
        /// <param name="max"></param>
        /// <returns></returns>
        /// <exception cref="ArgumentException"></exception>
        public static float GetRandomFloat(float min, float max)
        {
            if (min > max)
            {
                throw new ArgumentException("[ERROR]: Minimum value must be less than or equal to the maximum value.");
            }

            Random random = new();

            float range = max - min;
            float randomValue = (float)(random.NextDouble() * range) + min;

            return randomValue;
        }
    }
}
