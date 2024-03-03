using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Red.Common.Client.Misc
{
    public static class Math
    {
        public static float ConvertToFloat(this int number) => number;

        public static double ConvertToDouble(this int number) => number;

        public static int ConvertToInt(this string number) => int.Parse(number);

        public static float ConvertToFloat(this string number) => float.Parse(number);
    }
}
